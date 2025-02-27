using Task = DotNetToolbox.AI.Jobs.Task;

namespace DotNetToolbox.AI.Chats;

public class Chat(string id, IEnumerable<Message>? messages = null)
        : List<Message>(messages ?? []),
      IChat {
    public Chat(IGuidProvider guid, IEnumerable<Message>? messages = null)
        : this(guid.CreateSortable().ToString(), messages) {
    }
    public Chat(IEnumerable<Message>? messages = null)
        : this(GuidProvider.Default, messages) {
    }

    public string Id { get; } = IsNotNull(id);
    public uint CallCount { get; set; }
    public uint InputTokens { get; set; }
    public uint OutputTokens { get; set; }

    public void SetSystemMessage(string prompt) {
        var message = new Message(MessageRole.System, prompt);
        if (Count == 0) Add(message);
        else this[0] = message;
    }

    public void AppendMessage(MessageRole role, string content)
        => AppendMessage(role, (MessagePart)content);

    public void AppendMessage(MessageRole role, MessagePart content)
        => AppendMessage(role, [content]);

    public void AppendMessage(MessageRole role, IEnumerable<MessagePart> content) {
        if (Count == 0) Add(new(MessageRole.System, Task.Default.Prompt));
        if (this[^1].Role == role) this[^1].AddRange(content);
        else Add(new(role, content));
    }
}
