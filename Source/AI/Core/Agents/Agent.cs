﻿namespace DotNetToolbox.AI.Agents;

public abstract class Agent<TAgent, TMapper, TRequest, TResponse>(string provider,
                                                                  IHttpClientProviderFactory factory,
                                                                  ILogger<TAgent> logger)
    : IAgent
    where TAgent : Agent<TAgent, TMapper, TRequest, TResponse>
    where TMapper : class, IMapper, new()
    where TRequest : class, IChatRequest
    where TResponse : class, IChatResponse {
    private readonly IHttpClientProvider _httpClientProvider = factory.Create(provider);

    protected ILogger<TAgent> Logger { get; } = logger;
    protected TMapper Mapper { get; } = new();

    public World World { get; set; } = default!;
    public Persona Persona { get; set; } = default!;
    public AgentOptions Options { get; set; } = default!;

    public virtual async Task<HttpResult> SendRequest(IResponseAwaiter source, IChat chat, int? number, CancellationToken ct) {
        try {
            var isCompleted = false;
            var count = 1;
            while (!isCompleted) {
                Logger.LogDebug("Sending request {count}...", count++);
                var result = await Submit(chat, ct);
                if (!result.IsOk) return result;
                isCompleted = source switch {
                    IResponseConsumer ac => ac.VerifyResponse(chat.Id, number, chat.Messages[^1]),
                    IAsyncResponseConsumer ac => await ac.VerifyResponse(chat.Id, number, chat.Messages[^1], ct),
                    _ => throw new NotSupportedException(nameof(source)),
                };
            }

            switch (source) {
                case IResponseConsumer ac: ac.ResponseApproved(chat.Id, number, chat.Messages[^1]); break;
                case IAsyncResponseConsumer ac: await ac.ResponseApproved(chat.Id, number, chat.Messages[^1], ct); break;
            }
            Logger.LogDebug("Request completed.");
            return HttpResult.Ok();
        }
        catch (Exception ex) {
            Logger.LogError(ex, "An error occurred while sending the request!");
            return HttpResult.InternalError(ex);
        }
    }

    private async Task<HttpResult> Submit(IChat chat, CancellationToken ct = default) {
        var request = (TRequest)Mapper.CreateRequest(this, chat);
        HttpResponseMessage httpResult = default!;
        try {
            var content = JsonContent.Create(request, options: IAgentOptions.SerializerOptions, mediaType: MediaTypeWithQualityHeaderValue.Parse(HttpClientOptions.DefaultContentType));
            var httpClient = _httpClientProvider.GetHttpClient();
            var chatEndpoint = _httpClientProvider.Options.Endpoints["Chat"];
            httpResult = await httpClient.PostAsync(chatEndpoint, content, ct).ConfigureAwait(false);
            httpResult.EnsureSuccessStatusCode();
            var json = await httpResult.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            var apiResponse = JsonSerializer.Deserialize<TResponse>(json, IAgentOptions.SerializerOptions)!;
            var responseMessage = Mapper.CreateResponseMessage(chat, apiResponse);
            chat.Messages.Add(responseMessage);
            return HttpResult.Ok();
        }
        catch (Exception ex) {
            Logger.LogWarning(ex, "Request failed!");
            switch (httpResult.StatusCode) {
                case HttpStatusCode.BadRequest:
                    var input = JsonSerializer.Serialize(request, IAgentOptions.SerializerOptions);
                    var response = await httpResult.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                    var errorMessage = $"""
                                        RequestPackage: {input}
                                        ResponseContent: {response};
                                        """;
                    return HttpResult.BadRequest(errorMessage);
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.Forbidden:
                    return HttpResult.Unauthorized();
                case HttpStatusCode.NotFound:
                    return HttpResult.NotFound();
                default:
                    return HttpResult.InternalError(ex);
            }
        }
    }
}
