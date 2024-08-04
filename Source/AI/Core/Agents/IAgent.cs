﻿namespace DotNetToolbox.AI.Agents;

public interface IAgent {
    AgentModel Model { get; }
    Persona Persona { get; }
    List<Tool> Tools { get; }
    Task<HttpResult> SendRequest(IJob job, IChat chat, CancellationToken ct = default);
}
