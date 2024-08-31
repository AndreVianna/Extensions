﻿var app = Lola.Create(args, cb => {
    cb.AddAppSettings(); // This will add the values from appsettings.json to the context
    cb.AddUserSecrets<Program>(); // This will add the values from the user secrets to the context
}, ab => {
    ab.ConfigureLogging((loggingBuilder) => {
        var logPath = Path.Combine("logs", "lola-.log");
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(ab.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.File(logPath,
                fileSizeLimitBytes: 5 * 1024 * 1024,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7) // 5MB file size limit
            .CreateLogger();

        loggingBuilder.AddSerilog(dispose: true);
    });
    ab.SetOutputHandler(new AnsiOutput());
    ab.SetInputHandler(new AnsiInput());
    ab.Services.Configure<LolaSettings>(ab.Configuration.GetSection("Lola"));
    ab.Services.AddOptions<LolaSettings>();

    ab.Services.AddSingleton<IProviderRepositoryStrategy, ProviderRepositoryStrategy>();
    ab.Services.AddScoped<IProviderRepository, ProviderRepository>();
    ab.Services.AddScoped<IProviderHandler, ProviderHandler>();
    ab.Services.AddScoped(p => new Lazy<IProviderRepository>(() => p.GetRequiredService<IProviderRepository>()));
    ab.Services.AddScoped(p => new Lazy<IProviderHandler>(() => p.GetRequiredService<IProviderHandler>()));

    ab.Services.AddSingleton<IAgentRepositoryStrategy, AgentRepositoryStrategy>();
    ab.Services.AddScoped<IAgentRepository, AgentRepository>();
    ab.Services.AddScoped<IAgentHandler, AgentHandler>();
    ab.Services.AddScoped(p => new Lazy<IAgentRepository>(() => p.GetRequiredService<IAgentRepository>()));
    ab.Services.AddScoped(p => new Lazy<IAgentHandler>(() => p.GetRequiredService<IAgentHandler>()));

    ab.Services.AddSingleton<IModelRepositoryStrategy, ModelRepositoryStrategy>();
    ab.Services.AddScoped<IModelRepository, ModelRepository>();
    ab.Services.AddScoped<IModelHandler, ModelHandler>();
    ab.Services.AddScoped(p => new Lazy<IModelRepository>(() => p.GetRequiredService<IModelRepository>()));
    ab.Services.AddScoped(p => new Lazy<IModelHandler>(() => p.GetRequiredService<IModelHandler>()));
});

try {
    await app.RunAsync();
}
finally {
    Log.CloseAndFlush();
}
