﻿namespace DotNetToolbox.AI.Anthropic.DataModels;

public record Usage {
    [JsonPropertyName("input_tokens")]
    public int InputTokens { get; init; }
    [JsonPropertyName("output_tokens")]
    public int OutputTokens { get; init; }
}
