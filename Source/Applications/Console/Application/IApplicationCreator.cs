namespace DotNetToolbox.ConsoleApplication.Application;

public interface IApplicationCreator<out TApplication, out TBuilder, out TSettings>
    where TApplication : class, IApplication<TApplication, TBuilder, TSettings>
    where TBuilder : class, IApplicationBuilder<TApplication, TBuilder, TSettings>
    where TSettings : ApplicationSettings, new() {
    static abstract TApplication Create(string[] args, Action<IConfigurationBuilder> setConfiguration, Action<TBuilder> configureBuilder);
    static abstract TApplication Create(Action<IConfigurationBuilder> setConfiguration, Action<TBuilder> configureBuilder);
    static abstract TApplication Create(string[] args, Action<TBuilder> configureBuilder);
    static abstract TApplication Create(Action<TBuilder> configureBuilder);
    static abstract TApplication Create(string[] args, Action<IConfigurationBuilder> setConfiguration);
    static abstract TApplication Create(Action<IConfigurationBuilder> setConfiguration);
    static abstract TApplication Create(string[] args);
    static abstract TApplication Create();
}
