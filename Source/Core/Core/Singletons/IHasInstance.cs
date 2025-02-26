namespace DotNetToolbox.Singletons;

public interface IHasInstance<out TSelf>
    where TSelf : IHasInstance<TSelf> {
    static abstract TSelf Instance { get; }
}
