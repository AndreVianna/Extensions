namespace DotNetToolbox.Singletons;

public class HasInstance<TSelf> : IHasInstance<TSelf>
    where TSelf : HasInstance<TSelf>, new() {
    public static TSelf Instance { get; } = new();
}
