﻿namespace DotNetToolbox.AI.Jobs;

public class ChatJob : IJobStrategy<string[], string> {
    public string Instructions => "Engage in a conversation based on the provided messages.";

    public void AddPrompt(IMessages chat, string[] input, IJobContext context) {
        var isUser = true;
        foreach (var message in input) {
            var type = isUser ? MessageRole.User : MessageRole.Assistant;
            chat.Messages.Add(new(type, message));
            isUser = !isUser;
        }
    }

    public string GetResult(IMessages chat, IJobContext context) {
        var message = chat.Messages.Last(m => m.Role == MessageRole.Assistant);
        return message.Text;
    }
}
