namespace DotNetToolbox.Singletons;

public interface IHasDefault<out TSelf> {
    static abstract TSelf Default { get; }
}
