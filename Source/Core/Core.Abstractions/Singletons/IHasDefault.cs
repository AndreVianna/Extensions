namespace DotNetToolbox.Singletons;

public interface IHasDefault<out TSelf>
    where TSelf : IHasDefault<TSelf> {
    static abstract TSelf Default { get; }
}
