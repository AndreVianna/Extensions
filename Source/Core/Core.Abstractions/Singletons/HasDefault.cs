namespace DotNetToolbox.Singletons;

public class HasDefault<TSelf> : IHasDefault<TSelf>
    where TSelf : HasDefault<TSelf>, new() {
    public static TSelf Default { get; } = new();
}
