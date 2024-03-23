﻿namespace DotNetToolbox.AI.OpenAI;
public class BackgroundAgent(World world,
                             Persona persona,
                             IAgentOptions options,
                             IHttpClientProvider httpClientProvider,
                             ILogger<BackgroundAgent> logger)
    : BackgroundAgent<BackgroundAgent,
        Mapper,
        ChatRequest,
        ChatResponse>(world, persona, options, httpClientProvider, logger) {
    public BackgroundAgent(Persona persona,
                           IAgentOptions options,
                           IDateTimeProvider dateTime,
                           IHttpClientProvider httpClientProvider,
                           ILogger<BackgroundAgent> logger)
        : this(new(dateTime), persona, options, httpClientProvider, logger) {
    }
}
