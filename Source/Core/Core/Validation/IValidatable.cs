namespace DotNetToolbox.Validation;

public interface IValidatable : IValidatable<IMap>;

public interface IValidatable<in TContext>
    where TContext : class {
    Result Validate(TContext? context = null);
}

public interface IAsyncValidatable : IAsyncValidatable<IMap>;

public interface IAsyncValidatable<in TContext>
    where TContext : class {
    Task<Result> ValidateAsync(TContext? context = null, CancellationToken token = default);
}
