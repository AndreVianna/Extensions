// ReSharper disable once CheckNamespace - Intended to be in this namespace
namespace System.Threading.Tasks;

public static class TaskExtensions {
    public static void FireAndForget(this Task task)
        => FireAndForgetHandler.Send(task, default, default(Action<Exception>));
    public static void FireAndForget(this Task task, Action<Exception> onException)
        => FireAndForgetHandler.Send(task, default, IsNotNull(onException));
    public static void FireAndForget(this Task task, Action<OperationCanceledException> onCancel)
        => FireAndForgetHandler.Send(task, IsNotNull(onCancel), default);
    public static void FireAndForget(this Task task, Action<OperationCanceledException> onCancel, Action<Exception> onException)
        => FireAndForgetHandler.Send(task, IsNotNull(onCancel), IsNotNull(onException));
    public static void FireAndForget(this Task task, Action<Task, Exception> onException)
        => FireAndForgetHandler.Send(task, default, IsNotNull(onException));
    public static void FireAndForget(this Task task, Action<Task, OperationCanceledException> onCancel)
        => FireAndForgetHandler.Send(task, IsNotNull(onCancel), default);
    public static void FireAndForget(this Task task, Action<Task, OperationCanceledException> onCancel, Action<Task, Exception> onException)
        => FireAndForgetHandler.Send(task, IsNotNull(onCancel), IsNotNull(onException));

    public static void FireAndForget<TResult>(this Task<TResult> task, Action<TResult> onResult)
        => FireAndForgetHandler.Send(task, IsNotNull(onResult), default, default(Action<Exception>));
    public static void FireAndForget<TResult>(this Task<TResult> task, Action<TResult> onResult, Action<Exception> onException)
        => FireAndForgetHandler.Send(task, IsNotNull(onResult), default, IsNotNull(onException));
    public static void FireAndForget<TResult>(this Task<TResult> task, Action<TResult> onResult, Action<OperationCanceledException> onCancel)
        => FireAndForgetHandler.Send(task, IsNotNull(onResult), IsNotNull(onCancel), default);
    public static void FireAndForget<TResult>(this Task<TResult> task, Action<TResult> onResult, Action<OperationCanceledException> onCancel, Action<Exception> onException)
        => FireAndForgetHandler.Send(task, IsNotNull(onResult), IsNotNull(onCancel), IsNotNull(onException));
    public static void FireAndForget<TResult>(this Task<TResult> task, Action<TResult> onResult, Action<Task<TResult>, Exception> onException)
        => FireAndForgetHandler.Send(task, IsNotNull(onResult), default, IsNotNull(onException));
    public static void FireAndForget<TResult>(this Task<TResult> task, Action<TResult> onResult, Action<Task<TResult>, OperationCanceledException> onCancel)
        => FireAndForgetHandler.Send(task, IsNotNull(onResult), IsNotNull(onCancel), default);
    public static void FireAndForget<TResult>(this Task<TResult> task, Action<TResult> onResult, Action<Task<TResult>, OperationCanceledException> onCancel, Action<Task<TResult>, Exception> onException)
        => FireAndForgetHandler.Send(task, IsNotNull(onResult), IsNotNull(onCancel), IsNotNull(onException));
}
