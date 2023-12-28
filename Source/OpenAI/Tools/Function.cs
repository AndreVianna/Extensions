﻿namespace DotNetToolbox.OpenAI.Tools;

public record Function {
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required ParameterList Parameters { get; init; }
}
