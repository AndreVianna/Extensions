namespace DotNetToolbox.Singleton;

public interface IHasDefault<out TSelf>
    where TSelf : IHasDefault<TSelf> {
    public static abstract TSelf Default { get; }
}
