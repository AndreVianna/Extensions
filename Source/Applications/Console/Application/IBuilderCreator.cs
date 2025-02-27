namespace DotNetToolbox.ConsoleApplication.Application;

public interface IBuilderCreator<TApplication, out TBuilder, out TSettings>
    where TApplication : class, IApplication<TApplication, TBuilder, TSettings>
    where TBuilder : class, IApplicationBuilder<TApplication, TBuilder, TSettings>
    where TSettings : ApplicationSettings, new() {
    static abstract TBuilder CreateBuilder(Action<IConfigurationBuilder>? setConfiguration = null);
    static abstract TBuilder CreateBuilder(string[] args, Action<IConfigurationBuilder>? setConfiguration = null);
}
