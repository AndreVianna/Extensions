namespace DotNetToolbox.Singleton;

public class HasEmpty<TSelf> : IHasEmpty<TSelf>
    where TSelf : HasEmpty<TSelf>, new() {
    public static TSelf Empty { get; }= new();
}
