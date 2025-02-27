namespace DotNetToolbox.AI.Jobs;

public class Job(string id, JobContext context)
    : IJob {
    private readonly Chat _chat = new(id);

    public Job(IGuidProvider guid, JobContext context)
        : this(guid.CreateSortable().ToString(), context) {
    }
    public Job(JobContext context)
        : this(GuidProvider.Default, context) {
    }
    public string Id { get; } = id;
    public Dictionary<Type, Func<object, string>> Converters { get; } = [];

    public async Task<Result> Execute(CancellationToken ct) {
        try {
            SetSystemMessage();
            SetUserMessage();
            var response = await context.Agent.SendRequest(_chat, context, ct);
            if (!response.IsSuccessful) return response.Errors.ToArray();
            SetAgentResponse();
            return Result.Success();
        }
        catch (Exception ex) {
            throw new InvalidOperationException($"Chat: {_chat.DumpAsJson()}", ex);
        }
    }

    private void SetSystemMessage() {
        if (_chat.Count != 0) return;
        var message = $"""
            # Agent Description
            {context.Persona.Prompt}

            # Task Description
            {context.Task.Prompt}
            """;
        _chat.AppendMessage(MessageRole.System, message);
    }

    private void SetUserMessage() {
        var message = JobInputHelper.FormatInput(context.Input, context.Task.InputTemplate, Converters);
        _chat.AppendMessage(MessageRole.User, message);
    }

    private void SetAgentResponse()
        => context.Output = JobOutputHelper.ExtractOutput(context.Task.ResponseType, _chat[^1]);
}
