namespace DotNetToolbox.AI.Agents;

public interface IAgent {
    AgentSettings Settings { get; }
    Task<TypedResult<HttpStatusCode>> SendRequest(IChat chat, JobContext context, CancellationToken ct = default);
}
