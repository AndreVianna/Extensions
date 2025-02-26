namespace DotNetToolbox.Singletons;

public interface IHasEmpty<out TSelf>
    where TSelf : IHasEmpty<TSelf> {
    static abstract TSelf Empty { get; }
}
