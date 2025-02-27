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

    public virtual async Task<TypedResult<HttpStatusCode>> SendRequest(IChat chat, JobContext context, CancellationToken ct = default) {
        try {
            var lastMessage = chat[^1];
            if (lastMessage.Role != MessageRole.User) throw new NotImplementedException();
            var originalUserMessage = lastMessage.ToString();
            lastMessage.Add("\n# Answer:\n");

            var finalMessage = new Message(MessageRole.Assistant);
            var result = await PostRequest(chat, context, ct).ConfigureAwait(false);
            while (result is { IsSuccessful: true, Value.IsPartial: true }) {
                _logger.LogDebug("Response is incomplete.");
                var addedMessage = result.Value;
                finalMessage.AddRange(addedMessage);
                lastMessage.AddRange(addedMessage);
                result = await PostRequest(chat, context, ct).ConfigureAwait(false);
            }

            _logger.LogDebug("Request completed.");
            chat[^1] = new(MessageRole.User, originalUserMessage);
            if (!result.IsSuccessful) return result;

            finalMessage.AddRange(result.Value);
            chat.Add(finalMessage);
            return TypedResult.As(HttpStatusCode.OK, finalMessage);
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "Request failed!");
            throw;
        }
    }

    private async Task<TypedResult<HttpStatusCode, Message>> PostRequest(IChat chat, JobContext context, CancellationToken ct) {
        _logger.LogDebug("Sending request {RequestNumber} for {ChatId}...", ++chat.CallCount, chat.Id);
        var httpRequest = CreateRequest(chat, context);
        var mediaType = MediaTypeWithQualityHeaderValue.Parse(HttpClientOptions.DefaultContentType);
        var httpRequestContent = JsonContent.Create(httpRequest, mediaType, options: IAgentSettings.SerializerOptions);
        using var httpClient = _httpClientProvider.GetHttpClient();
        var chatEndpoint = _httpClientProvider.Options.Endpoints["Chat"];
        using var httpResponse = await httpClient.PostAsync(chatEndpoint, httpRequestContent, ct).ConfigureAwait(false);
        return await ProcessResponse(chat, context, httpResponse, ct);
    }

    private async Task<TypedResult<HttpStatusCode, Message>> ProcessResponse(IChat chat, JobContext context, HttpResponseMessage httpResponse, CancellationToken ct) {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (httpResponse.StatusCode) {
            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.Forbidden:
                _logger.LogDebug("Authentication failed.");
                return TypedResult.As(httpResponse.StatusCode, [new("Authentication failed.")]).WithNo<Message>();
            case HttpStatusCode.NotFound:
                _logger.LogDebug("Agent endpoint not found.");
                return TypedResult.As(httpResponse.StatusCode, [new("Agent endpoint not found.")]).WithNo<Message>();
            case HttpStatusCode.BadRequest:
                _logger.LogDebug("Invalid request.");
                var badContent = await httpResponse.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                return TypedResult.As(httpResponse.StatusCode, [new($"Invalid request.{System.Environment.NewLine}{badContent}")]).WithNo<Message>();
            default:
                _logger.LogDebug("Response received.");
                var content = await httpResponse.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                var output = JsonSerializer.Deserialize<TResponse>(content, IAgentSettings.SerializerOptions)!;
                var message = ExtractMessage(chat, context, output);
                return TypedResult.As(HttpStatusCode.OK, message);
        }
    }

    protected abstract TRequest CreateRequest(IChat chat, JobContext context);
    protected abstract Message ExtractMessage(IChat chat, JobContext context, TResponse response);
}
