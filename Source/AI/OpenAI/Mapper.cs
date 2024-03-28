﻿namespace DotNetToolbox.AI.OpenAI;

public class Mapper
    : IMapper {
    IChatRequest IMapper.CreateRequest(IChat chat, World world, UserProfile user, IAgent agent)
        => CreateRequest(chat, world, user, agent);
    public static ChatRequest CreateRequest(IChat chat, World world, UserProfile user, IAgent agent) {
        var system = new ChatRequestMessage(CreateSystemMessage(chat, world, user, agent));
        return new() {
            Model = agent.Model.Id,
            Temperature = agent.Model.Temperature,
            MaximumOutputTokens = agent.Model.MaximumOutputTokens,
            StopSequences = agent.Model.StopSequences.Count == 0 ? null : [.. agent.Model.StopSequences],
            MinimumTokenProbability = agent.Model.TokenProbabilityCutOff,
            ResponseIsStream = agent.Model.ResponseIsStream,

            Messages = [system, .. chat.Messages.ToArray(o => new ChatRequestMessage(o))],

            NumberOfChoices = 1,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
            Tools = agent.Persona.KnownTools.Count == 0 ? null : agent.Persona.KnownTools.ToArray(ToRequestToolCall),
        };
    }

    private static string CreateSystemMessage(IChat chat, World world, UserProfile user, IAgent agent) {
        var builder = new StringBuilder();
        builder.AppendLine(world.ToString());
        builder.AppendLine(user.ToString());
        builder.AppendLine(agent.Persona.ToString());
        builder.AppendLine(chat.Instructions.ToString());
        return builder.ToString();
    }

    Message IMapper.CreateResponseMessage(IChat chat, IChatResponse response)
        => CreateResponseMessage(chat, (ChatResponse)response);
    public static Message CreateResponseMessage(IChat chat, ChatResponse response) {
        chat.TotalTokens = (uint)(response.Usage?.TotalTokens ?? (int)chat.TotalTokens);
        return response.Choices[0].Message.Content switch {
            ChatResponseToolRequest[] tcs => new("assistant", new MessagePart("tool_calls", tcs)),
            _ => new("assistant", $"{response.Choices[0].Message.Content}"),
        };
    }

    private static ChatRequestTool ToRequestToolCall(Tool tool)
        => new("function", new(tool.Name, CreateParameterList(tool), tool.Description));

    private static ChatRequestToolFunctionCallParameters? CreateParameterList(Tool tool) {
        var parameters = GetParameters(tool);
        var required = GetRequiredParameters(tool);
        return parameters is null && required is null ? null : new(parameters, required);
    }

    private static Dictionary<string, ChatRequestToolFunctionCallParameter>? GetParameters(Tool tool) {
        var result = tool.Arguments.ToDictionary<Argument, string, ChatRequestToolFunctionCallParameter>(k => k.Name, ToParameter);
        return result.Count == 0 ? null : result;
    }

    private static string[]? GetRequiredParameters(Tool tool) {
        var result = tool.Arguments.Where(p => p.IsRequired).ToArray(p => p.Name);
        return result.Length == 0 ? null : result;
    }

    private static ChatRequestToolFunctionCallParameter ToParameter(Argument argument)
        => new(argument.Type.ToString(), argument.Options, argument.Description);
}
