// ReSharper disable once CheckNamespace - Intended to be in this namespace
namespace System.Threading.Tasks;

internal static class FireAndForgetHandler {
    public static async void Send(ValueTask task, Action<OperationCanceledException>? onCancel, Action<Exception>? onException) {
        try {
            await IsNotNull(task).ConfigureAwait(false); // Fire and forget do not need to capture the current context
        }
        catch (OperationCanceledException ex) {
            onCancel?.Invoke(ex);
        }
        catch (Exception ex) {
            onException?.Invoke(ex);
        }
    }

    public static async void Send(ValueTask task, Action<ValueTask, OperationCanceledException>? onCancel, Action<ValueTask, Exception>? onException) {
        try {
            await IsNotNull(task).ConfigureAwait(false); // Fire and forget do not need to capture the current context
        }
        catch (OperationCanceledException ex) {
            if (onCancel is null)
                return;
            onCancel(task, ex);
        }
        catch (Exception ex) {
            onException?.Invoke(task, ex);
        }
    }

    public static async void Send<TResult>(ValueTask<TResult> task, Action<TResult> onResult, Action<OperationCanceledException>? onCancel, Action<Exception>? onException) {
        try {
            var result = await task.ConfigureAwait(false); // Fire and forget do not need to capture the current context
            onResult(result);
        }
        catch (OperationCanceledException ex) {
            onCancel?.Invoke(ex);
        }
        catch (Exception ex) {
            onException?.Invoke(ex);
        }
    }

    public static async void Send<TResult>(ValueTask<TResult> task, Action<TResult> onResult, Action<ValueTask<TResult>, OperationCanceledException>? onCancel, Action<ValueTask<TResult>, Exception>? onException) {
        try {
            var result = await task.ConfigureAwait(false); // Fire and forget do not need to capture the current context
            onResult(result);
        }
        catch (OperationCanceledException ex) {
            onCancel?.Invoke(task, ex);
        }
        catch (Exception ex) {
            onException?.Invoke(task, ex);
        }
    }

    public static async void Send(Task task, Action<OperationCanceledException>? onCancel, Action<Exception>? onException) {
        try {
            await task.ConfigureAwait(false); // Fire and forget do not need to capture the current context
        }
        catch (OperationCanceledException ex) {
            onCancel?.Invoke(ex);
        }
        catch (Exception ex) {
            onException?.Invoke(ex);
        }
    }

    public static async void Send(Task task, Action<Task, OperationCanceledException>? onCancel, Action<Task, Exception>? onException) {
        try {
            await task.ConfigureAwait(false); // Fire and forget do not need to capture the current context
        }
        catch (OperationCanceledException ex) {
            onCancel?.Invoke(task, ex);
        }
        catch (Exception ex) {
            onException?.Invoke(task, ex);
        }
    }

    public static async void Send<TResult>(Task<TResult> task, Action<TResult> onResult, Action<OperationCanceledException>? onCancel, Action<Exception>? onException) {
        try {
            var result = await task.ConfigureAwait(false); // Fire and forget do not need to capture the current context
            onResult(result);
        }
        catch (OperationCanceledException ex) {
            onCancel?.Invoke(ex);
        }
        catch (Exception ex) {
            onException?.Invoke(ex);
        }
    }

    public static async void Send<TResult>(Task<TResult> task, Action<TResult> onResult, Action<Task<TResult>, OperationCanceledException>? onCancel, Action<Task<TResult>, Exception>? onException) {
        try {
            var result = await task.ConfigureAwait(false); // Fire and forget do not need to capture the current context
            onResult(result);
        }
        catch (OperationCanceledException ex) {
            onCancel?.Invoke(task, ex);
        }
        catch (Exception ex) {
            onException?.Invoke(task, ex);
        }
    }
}
