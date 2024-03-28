﻿
namespace DotNetToolbox.AI.Agents;

public interface IMapper {
    IChatRequest CreateRequest(IChat chat, World world, UserProfile user, IAgent agent);
    Message CreateResponseMessage(IChat chat, IChatResponse response);
}
