namespace DotNetToolbox.Results;

public interface IHasDefault<out TSelf> {
    static abstract TSelf Default { get; }
}
