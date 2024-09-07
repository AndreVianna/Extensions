﻿namespace AI.Sample.Services;

public class AIService(IServiceProvider services, ILogger<AIService> logger)
    : IAIService {
    private readonly IModelHandler _modelHandler = services.GetRequiredService<IModelHandler>();
    private readonly IUserProfileHandler _userHandler = services.GetRequiredService<IUserProfileHandler>();
    private readonly IPersonaHandler _personaHandler = services.GetRequiredService<IPersonaHandler>();
    private readonly ITaskHandler _taskHandler = services.GetRequiredService<ITaskHandler>();
    private readonly IAgentAccessor _connectionAccessor = services.GetRequiredService<IAgentAccessor>();

    public async Task<string> GetNextQuestion(PersonaEntity persona) {
        try {
            var appModel = _modelHandler.Internal ?? throw new InvalidOperationException("No default AI model selected.");
            var httpConnection = _connectionAccessor.GetFor(appModel.Provider!.Name);
            var userProfileEntity = _userHandler.Get() ?? throw new InvalidOperationException("No user found.");
            var personaEntity = _personaHandler.GetByName("Agent Creator") ?? throw new InvalidOperationException("Required persona not found. Name: 'Agent Creator'.");
            var taskEntity = _taskHandler.GetByName("Ask Questions about the AI Agent") ?? throw new InvalidOperationException("Required task not found. Name: 'Ask Questions about the AI Agent'.");
            var context = new JobContext {
                Model = appModel,
                Agent = httpConnection,
                UserProfile = userProfileEntity,
                Persona = personaEntity,
                Task = taskEntity,
            };
            var job = new PersonaGenerationJob(context);
            var result = await job.Execute(persona, CancellationToken.None);
            return result.HasException
                ? throw new("Failed to generate next question: " + result.Exception.Message)
                : result.Value;

            // var chat = new Chat(Guid.NewGuid().ToString(), new Context());
            // var message = new Message(MessageRole.System, """
            //                                               You are an AI assistant helping to generate a persona.
            //                                               Ask relevant questions to gather information for creating a detailed persona.
            //                                               """);
            // chat.Messages.Add(message);

            // // Add user message with current persona information
            // var userMessage = $"""
            //                    I'm creating a persona with the following details:
            //                    Name: {entity.Name}
            //                    Primary Role: {entity.Role}
            //                    Intended Use: {string.Join(", ", entity.Goals)}
            //                    """;
            // if (entity.Traits.Any()) {
            //     userMessage += "Additional Information:\n";
            //     foreach (var query in entity.Questions) {
            //         userMessage += $"""
            //                         Q: {query.Question}
            //                         A: {query.Answer}
            //                         """;
            //     }
            // }
            // userMessage += "What's the next question you'd like to ask to gather more information for this persona?";
            // chat.Messages.Add(new Message(MessageRole.User, userMessage));
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error generating next question for persona {PersonaName}", persona.Name);
            throw;
        }
    }

    //public async Task<string> GeneratePrompt(PersonaEntity persona) {
    //    try {
    //        var appAgent = _agentHandler.Selected ?? throw new InvalidOperationException("No AI agent selected.");
    //        var aiAgent = _aiAgentFactory.Create(appAgent.Provider);

    //        var chat = new Chat(Guid.NewGuid().ToString(), new Context());

    //        // Add system message to set up the context
    //        chat.Messages.Add(new Message(MessageRole.System, "You are an AI assistant tasked with generating a detailed prompt for an AI persona based on the provided information."));

    //        // Add user message with all persona information
    //        var userMessage = $"Generate a detailed prompt for an AI persona with the following information:\n" +
    //                          $"Name: {persona.Name}\n" +
    //                          $"Primary Role: {persona.PrimaryRole}\n" +
    //                          $"Intended Use: {persona.IntendedUse}\n" +
    //                          "Additional Information:\n";
    //        foreach (var query in persona.AdditionalInformation) {
    //            userMessage += $"Q: {query.Question}\nA: {query.Answer}\n";
    //        }
    //        userMessage += "Please create a comprehensive prompt that captures all aspects of this persona, including its role, expertise, communication style, and any other relevant characteristics.";
    //        chat.Messages.Add(new Message(MessageRole.User, userMessage));

    //        var job = new PromptGenerationJob(new JobContext(new ServiceCollection().BuildServiceProvider())) {
    //            World = new World(),
    //            Persona = new AI.Agents.Persona(),
    //            Agent = aiAgent
    //        };

    //        var result = await job.ExecuteAsync(chat, CancellationToken.None);
    //        if (result.HasException) {
    //            throw new Exception("Failed to generate prompt: " + result.Exception.Message);
    //        }

    //        return result.Value;
    //    }
    //    catch (Exception ex) {
    //        _logger.LogError(ex, "Error generating prompt for persona {PersonaName}", persona.Name);
    //        throw;
    //    }
    //}
}
