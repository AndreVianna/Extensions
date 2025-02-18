namespace DotNetToolbox.AI.Agents;

public abstract class Agent<TAgent, TRequest, TResponse>
    : IAgent
    where TAgent : Agent<TAgent, TRequest, TResponse> {
    private readonly IHttpClientProvider _httpClientProvider;
    private readonly ILogger<TAgent> _logger;

    protected Agent(string provider, IServiceProvider services, ILogger<TAgent> logger) {
        var httpClientProviderAccessor = services.GetRequiredService<IHttpClientProviderAccessor>();
        _httpClientProvider = httpClientProviderAccessor.Get(provider);
        _logger = logger;
    }

    public AgentSettings Settings { get; } = new();

    public virtual async Task<HttpResult> SendRequest(IChat chat, JobContext context, CancellationToken ct = default) {
        try {
            var lastMessage = chat[^1];
            if (lastMessage.Role != MessageRole.User) throw new NotImplementedException();
            var originalUserMessage = lastMessage.ToString();
            lastMessage.Add("\n# Answer:\n");

            var finalMessage = new Message(MessageRole.Assistant);
            var result = await PostRequest(chat, context, ct).ConfigureAwait(false);
            while (result.IsSuccess && result.Value!.IsPartial) {
                _logger.LogDebug("Response is incomplete.");
                var addedMessage = result.Value;
                finalMessage.AddRange(addedMessage);
                lastMessage.AddRange(addedMessage);
                result = await PostRequest(chat, context, ct).ConfigureAwait(false);
            }

            _logger.LogDebug("Request completed.");
            chat[^1] = new(MessageRole.User, originalUserMessage);
            if (!result.IsSuccess) return result;

            finalMessage.AddRange(result.Value!);
            chat.Add(finalMessage);
            return HttpResult.Ok();
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "Request failed!");
            return HttpResult.InternalError<Message>(ex);
        }
    }

    private async Task<HttpResult<Message>> PostRequest(IChat chat, JobContext context, CancellationToken ct) {
        _logger.LogDebug("Sending request {RequestNumber} for {ChatId}...", ++chat.CallCount, chat.Id);
        var httpRequest = CreateRequest(chat, context);
        var mediaType = MediaTypeWithQualityHeaderValue.Parse(HttpClientOptions.DefaultContentType);
        var httpRequestContent = JsonContent.Create(httpRequest, mediaType, options: IAgentSettings.SerializerOptions);
        using var httpClient = _httpClientProvider.GetHttpClient();
        var chatEndpoint = _httpClientProvider.Options.Endpoints["Chat"];
        using var httpResponse = await httpClient.PostAsync(chatEndpoint, httpRequestContent, ct).ConfigureAwait(false);
        return await ProcessResponse(chat, context, httpResponse, ct);
    }

    private async Task<HttpResult<Message>> ProcessResponse(IChat chat, JobContext context, HttpResponseMessage httpResponse, CancellationToken ct) {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (httpResponse.StatusCode) {
            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.Forbidden:
                _logger.LogDebug("Authentication failed.");
                return HttpResult.Unauthorized<Message>();
            case HttpStatusCode.NotFound:
                _logger.LogDebug("Agent endpoint not found.");
                return HttpResult.NotFound<Message>();
            case HttpStatusCode.BadRequest:
                _logger.LogDebug("Invalid request.");
                var badContent = await httpResponse.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                return HttpResult.BadRequest<Message>(new OperationError(badContent));
            default:
                _logger.LogDebug("Response received.");
                var content = await httpResponse.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                var output = JsonSerializer.Deserialize<TResponse>(content, IAgentSettings.SerializerOptions)!;
                var message = ExtractMessage(chat, context, output);
                return HttpResult.Ok(message);
        }
    }

    protected abstract TRequest CreateRequest(IChat chat, JobContext context);
    protected abstract Message ExtractMessage(IChat chat, JobContext context, TResponse response);
}
