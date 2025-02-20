﻿namespace DotNetToolbox.Validation;

public interface IValidatable : IValidatable<IMap>;

public interface IValidatable<in TContext>
    where TContext : class {
    IValidationResult Validate(TContext? context = null);
}

public interface IAsyncValidatable : IAsyncValidatable<IMap>;

public interface IAsyncValidatable<in TContext>
    where TContext : class {
    Task<IValidationResult> ValidateAsync(TContext? context = null, CancellationToken token = default);
}
