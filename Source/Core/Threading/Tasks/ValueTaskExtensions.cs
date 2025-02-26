// ReSharper disable once CheckNamespace - Intended to be in this namespace
namespace System.Threading.Tasks;

public static class ValueTaskExtensions {
    public static void Wait(this ValueTask task) {
        var awaiter = task.GetAwaiter();
        if (awaiter.IsCompleted) {
            awaiter.GetResult();
            return;
        }
        task.AsTask().GetAwaiter().GetResult();
    }

    public static T GetResult<T>(this ValueTask<T> task) {
        var awaiter = task.GetAwaiter();
        return awaiter.IsCompleted
            ? awaiter.GetResult()
            : task.AsTask().GetAwaiter().GetResult();
    }

    public static void FireAndForget(this ValueTask task)
        => FireAndForgetHandler.Send(task, default, default(Action<Exception>));
    public static void FireAndForget(this ValueTask task, Action<Exception> onException)
        => FireAndForgetHandler.Send(task, default, IsNotNull(onException));
    public static void FireAndForget(this ValueTask task, Action<OperationCanceledException> onCancel)
        => FireAndForgetHandler.Send(task, IsNotNull(onCancel), default);
    public static void FireAndForget(this ValueTask task, Action<OperationCanceledException> onCancel, Action<Exception> onException)
        => FireAndForgetHandler.Send(task, IsNotNull(onCancel), IsNotNull(onException));
    public static void FireAndForget(this ValueTask task, Action<ValueTask, Exception> onException)
        => FireAndForgetHandler.Send(task, default, IsNotNull(onException));
    public static void FireAndForget(this ValueTask task, Action<ValueTask, OperationCanceledException> onCancel)
        => FireAndForgetHandler.Send(task, IsNotNull(onCancel), default);
    public static void FireAndForget(this ValueTask task, Action<ValueTask, OperationCanceledException> onCancel, Action<ValueTask, Exception> onException)
        => FireAndForgetHandler.Send(task, IsNotNull(onCancel), IsNotNull(onException));

    public static void FireAndForget<TResult>(this ValueTask<TResult> task, Action<TResult> onResult)
        => FireAndForgetHandler.Send(task, IsNotNull(onResult), default, default(Action<Exception>));
    public static void FireAndForget<TResult>(this ValueTask<TResult> task, Action<TResult> onResult, Action<Exception> onException)
        => FireAndForgetHandler.Send(task, onResult, default, IsNotNull(onException));
    public static void FireAndForget<TResult>(this ValueTask<TResult> task, Action<TResult> onResult, Action<OperationCanceledException> onCancel)
        => FireAndForgetHandler.Send(task, onResult, IsNotNull(onCancel), default);
    public static void FireAndForget<TResult>(this ValueTask<TResult> task, Action<TResult> onResult, Action<OperationCanceledException> onCancel, Action<Exception> onException)
        => FireAndForgetHandler.Send(task, onResult, IsNotNull(onCancel), IsNotNull(onException));
    public static void FireAndForget<TResult>(this ValueTask<TResult> task, Action<TResult> onResult, Action<ValueTask<TResult>, Exception> onException)
        => FireAndForgetHandler.Send(task, IsNotNull(onResult), default, IsNotNull(onException));
    public static void FireAndForget<TResult>(this ValueTask<TResult> task, Action<TResult> onResult, Action<ValueTask<TResult>, OperationCanceledException> onCancel)
        => FireAndForgetHandler.Send(task, onResult, IsNotNull(onCancel), default);
    public static void FireAndForget<TResult>(this ValueTask<TResult> task, Action<TResult> onResult, Action<ValueTask<TResult>, OperationCanceledException> onCancel, Action<ValueTask<TResult>, Exception> onException)
        => FireAndForgetHandler.Send(task, onResult, IsNotNull(onCancel), IsNotNull(onException));
}
