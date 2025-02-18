namespace DotNetToolbox.Singleton;

public interface IHasInstance<out TSelf>
    where TSelf : IHasInstance<TSelf> {
    public static abstract TSelf Instance { get; }
}
