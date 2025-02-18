namespace DotNetToolbox.Singleton;

public interface IHasEmpty<out TSelf>
    where TSelf : IHasEmpty<TSelf> {
    public static abstract TSelf Empty { get; }
}
