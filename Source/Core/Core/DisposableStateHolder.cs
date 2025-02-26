// ReSharper disable once CheckNamespace - Intended to be in this namespace
namespace System;

public class DisposableStateHolder(object? state)
    : DisposableStateHolder<object?>(state);

public class DisposableStateHolder<TState>(TState state)
    : IDisposable {
    public TState State { get; } = state;
    public void Dispose() {
        if (State is IDisposable disposable)
            disposable.Dispose();
        GC.SuppressFinalize(this);
    }
}
