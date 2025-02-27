namespace DotNetToolbox.AI.OpenAI.Chats;

[method: SetsRequiredMembers]
public class ChatRequest(IAgent connection, IModel model, IChat chat)
    : IChatRequest {
    string IChatRequest.Map => (string?)Messages[0].Content ?? string.Empty;
    IEnumerable<IChatRequestMessage> IChatRequest.Messages => Messages[1..];

    [JsonPropertyName("model")]
    public required string Model { get; init; } = model.Key;
    [JsonPropertyName("messages")]
    public ChatRequestMessage[] Messages { get; } = SetMessages(chat);

    [JsonPropertyName("max_tokens")]
    public uint MaximumOutputTokens { get; set; } = SetMaximumOutputTokens(connection, model);

    [JsonPropertyName("frequency_penalty")]
    public decimal? FrequencyPenalty { get; set; }
    [JsonPropertyName("presence_penalty")]
    public decimal? PresencePenalty { get; set; }
    [JsonPropertyName("temperature")]
    public decimal? Temperature { get; set; }

    [JsonPropertyName("n")]
    public int? NumberOfChoices { get; set; }
    [JsonPropertyName("stop")]
    public string[]? StopSequences { get; set; }
    [JsonPropertyName("top_p")]
    public decimal? MinimumTokenProbability { get; set; }
    [JsonPropertyName("stream")]
    public bool? ResponseIsStream { get; set; }
    [JsonPropertyName("tools")]
    public ChatRequestTool[]? Tools { get; set; }
    [JsonPropertyName("tool_choice")]
    public ChatRequestForceToolCall? ForceToolCall { get; set; }
    [JsonPropertyName("response_format")]
    public ChatRequestResponseFormat? ResponseFormat { get; set; }

    private static ChatRequestMessage[] SetMessages(IChat chat)
        => chat.ToArray(m => new ChatRequestMessage(m));

    private static uint SetMaximumOutputTokens(IAgent agent, IModel model)
        => agent.Settings.MaximumOutputTokens > AgentSettings.MinimumOutputTokens
        && agent.Settings.MaximumOutputTokens < model.MaximumOutputTokens
            ? agent.Settings.MaximumOutputTokens
            : model.MaximumOutputTokens;
}
