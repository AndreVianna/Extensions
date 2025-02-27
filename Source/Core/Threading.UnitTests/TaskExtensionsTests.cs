namespace DotNetToolbox.Threading;

public class TaskExtensionsTests {
    private static readonly CancellationTokenSource _tokenSource = new();
    private static readonly CancellationToken _token = _tokenSource.Token;

    static TaskExtensionsTests() {
        _tokenSource.Cancel();
    }

    private static ValueTask TestValueTask() => ValueTask.CompletedTask;
    private static ValueTask TestFaultyValueTask() => ValueTask.FromException(new InvalidOperationException());
    private static ValueTask TestCanceledValueTask() => ValueTask.FromCanceled(_token);

    private static ValueTask<int> TestValueTaskOfT() => ValueTask.FromResult(42);
    private static ValueTask<int> TestFaultyValueTaskOfT() => ValueTask.FromException<int>(new InvalidOperationException());
    private static ValueTask<int> TestCanceledValueTaskOfT() => ValueTask.FromCanceled<int>(_token);

    private static Task TestTask() => Task.CompletedTask;
    private static Task TestFaultyTask() => Task.FromException(new InvalidOperationException());
    private static Task TestCanceledTask() => Task.FromCanceled(_token);

    private static Task<int> TestTaskOfT() => Task.FromResult(42);
    private static Task<int> TestFaultyTaskOfT() => Task.FromException<int>(new InvalidOperationException());
    private static Task<int> TestCanceledTaskOfT() => Task.FromCanceled<int>(_token);

    [Fact]
    public void FireAndForget_ForValueTask_ShouldCallAppropriateHandler() {
        // Arrange
        var goodTask = TestValueTask();
        var faultyTask = TestFaultyValueTask();
        var canceledTask = TestCanceledValueTask();

        var onException = For<Action<Exception>>();
        var onExceptionAndTask = For<Action<ValueTask, Exception>>();

        var onCancel = For<Action<OperationCanceledException>>();
        var onCancelAndTask = For<Action<ValueTask, OperationCanceledException>>();

        // Act & Assert
        goodTask.FireAndForget();
        canceledTask.FireAndForget();
        faultyTask.FireAndForget();

        goodTask.FireAndForget(onCancel);
        onCancel.DidNotReceive().Invoke(Any<OperationCanceledException>());

        canceledTask.FireAndForget(onCancel);
        onCancel.Received(1).Invoke(Any<OperationCanceledException>());
        onCancel.ClearReceivedCalls();

        faultyTask.FireAndForget(onCancel);
        onCancel.DidNotReceive().Invoke(Any<OperationCanceledException>());

        goodTask.FireAndForget(onCancelAndTask);
        onCancelAndTask.DidNotReceive().Invoke(goodTask, Any<OperationCanceledException>());

        canceledTask.FireAndForget(onCancelAndTask);
        onCancelAndTask.Received(1).Invoke(canceledTask, Any<OperationCanceledException>());
        onCancelAndTask.ClearReceivedCalls();

        faultyTask.FireAndForget(onCancelAndTask);
        onCancelAndTask.DidNotReceive().Invoke(faultyTask, Any<OperationCanceledException>());

        goodTask.FireAndForget(onException);
        onException.DidNotReceive().Invoke(Any<Exception>());

        canceledTask.FireAndForget(onException);
        onException.DidNotReceive().Invoke(Any<Exception>());

        faultyTask.FireAndForget(onException);
        onException.Received(1).Invoke(Any<Exception>());
        onException.ClearReceivedCalls();

        goodTask.FireAndForget(onExceptionAndTask);
        onExceptionAndTask.DidNotReceive().Invoke(goodTask, Any<Exception>());

        canceledTask.FireAndForget(onExceptionAndTask);
        onExceptionAndTask.DidNotReceive().Invoke(canceledTask, Any<Exception>());

        faultyTask.FireAndForget(onExceptionAndTask);
        onExceptionAndTask.Received(1).Invoke(faultyTask, Any<Exception>());
        onExceptionAndTask.ClearReceivedCalls();

        goodTask.FireAndForget(onCancel, onException);
        onException.DidNotReceive().Invoke(Any<Exception>());

        canceledTask.FireAndForget(onCancel, onException);
        onCancel.Received(1).Invoke(Any<OperationCanceledException>());
        onCancel.ClearReceivedCalls();
        onException.DidNotReceive().Invoke(Any<Exception>());

        faultyTask.FireAndForget(onCancel, onException);
        onCancel.DidNotReceive().Invoke(Any<OperationCanceledException>());
        onException.Received(1).Invoke(Any<Exception>());
        onException.ClearReceivedCalls();

        goodTask.FireAndForget(onCancelAndTask, onExceptionAndTask);
        onCancelAndTask.DidNotReceive().Invoke(goodTask, Any<OperationCanceledException>());
        onExceptionAndTask.DidNotReceive().Invoke(goodTask, Any<Exception>());

        canceledTask.FireAndForget(onCancelAndTask, onExceptionAndTask);
        onCancelAndTask.Received(1).Invoke(canceledTask, Any<OperationCanceledException>());
        onCancelAndTask.ClearReceivedCalls();
        onExceptionAndTask.DidNotReceive().Invoke(canceledTask, Any<Exception>());

        faultyTask.FireAndForget(onCancelAndTask, onExceptionAndTask);
        onCancelAndTask.DidNotReceive().Invoke(faultyTask, Any<OperationCanceledException>());
        onExceptionAndTask.Received(1).Invoke(faultyTask, Any<Exception>());
        onExceptionAndTask.ClearReceivedCalls();
    }

    [Fact]
    public void FireAndForget_ForValueTaskOfT_ShouldCallAppropriateHandler() {
        // Arrange
        var goodTask = TestValueTaskOfT();
        var faultyTask = TestFaultyValueTaskOfT();
        var canceledTask = TestCanceledValueTaskOfT();

        var onResult = For<Action<int>>();

        var onException = For<Action<Exception>>();
        var onExceptionAndTask = For<Action<ValueTask<int>, Exception>>();

        var onCancel = For<Action<OperationCanceledException>>();
        var onCancelAndTask = For<Action<ValueTask<int>, OperationCanceledException>>();

        // Act & Assert
        goodTask.FireAndForget(onResult);
        canceledTask.FireAndForget(onResult);
        faultyTask.FireAndForget(onResult);

        goodTask.FireAndForget(onResult, onCancel);
        onCancel.DidNotReceive().Invoke(Any<OperationCanceledException>());

        canceledTask.FireAndForget(onResult, onCancel);
        onCancel.Received(1).Invoke(Any<OperationCanceledException>());
        onCancel.ClearReceivedCalls();

        faultyTask.FireAndForget(onResult, onCancel);
        onCancel.DidNotReceive().Invoke(Any<OperationCanceledException>());

        goodTask.FireAndForget(onResult, onCancelAndTask);
        onCancelAndTask.DidNotReceive().Invoke(goodTask, Any<OperationCanceledException>());

        canceledTask.FireAndForget(onResult, onCancelAndTask);
        onCancelAndTask.Received(1).Invoke(canceledTask, Any<OperationCanceledException>());
        onCancelAndTask.ClearReceivedCalls();

        faultyTask.FireAndForget(onResult, onCancelAndTask);
        onCancelAndTask.DidNotReceive().Invoke(faultyTask, Any<OperationCanceledException>());

        goodTask.FireAndForget(onResult, onException);
        onException.DidNotReceive().Invoke(Any<Exception>());

        canceledTask.FireAndForget(onResult, onException);
        onException.DidNotReceive().Invoke(Any<Exception>());

        faultyTask.FireAndForget(onResult, onException);
        onException.Received(1).Invoke(Any<Exception>());
        onException.ClearReceivedCalls();

        goodTask.FireAndForget(onResult, onExceptionAndTask);
        onExceptionAndTask.DidNotReceive().Invoke(goodTask, Any<Exception>());

        canceledTask.FireAndForget(onResult, onExceptionAndTask);
        onExceptionAndTask.DidNotReceive().Invoke(canceledTask, Any<Exception>());

        faultyTask.FireAndForget(onResult, onExceptionAndTask);
        onExceptionAndTask.Received(1).Invoke(faultyTask, Any<Exception>());
        onExceptionAndTask.ClearReceivedCalls();

        goodTask.FireAndForget(onResult, onCancel, onException);
        onException.DidNotReceive().Invoke(Any<Exception>());

        canceledTask.FireAndForget(onResult, onCancel, onException);
        onCancel.Received(1).Invoke(Any<OperationCanceledException>());
        onCancel.ClearReceivedCalls();
        onException.DidNotReceive().Invoke(Any<Exception>());

        faultyTask.FireAndForget(onResult, onCancel, onException);
        onCancel.DidNotReceive().Invoke(Any<OperationCanceledException>());
        onException.Received(1).Invoke(Any<Exception>());
        onException.ClearReceivedCalls();

        goodTask.FireAndForget(onResult, onCancelAndTask, onExceptionAndTask);
        onCancelAndTask.DidNotReceive().Invoke(goodTask, Any<OperationCanceledException>());
        onExceptionAndTask.DidNotReceive().Invoke(goodTask, Any<Exception>());

        canceledTask.FireAndForget(onResult, onCancelAndTask, onExceptionAndTask);
        onCancelAndTask.Received(1).Invoke(canceledTask, Any<OperationCanceledException>());
        onCancelAndTask.ClearReceivedCalls();
        onExceptionAndTask.DidNotReceive().Invoke(canceledTask, Any<Exception>());

        faultyTask.FireAndForget(onResult, onCancelAndTask, onExceptionAndTask);
        onCancelAndTask.DidNotReceive().Invoke(faultyTask, Any<OperationCanceledException>());
        onExceptionAndTask.Received(1).Invoke(faultyTask, Any<Exception>());
        onExceptionAndTask.ClearReceivedCalls();
    }

    [Fact]
    public void FireAndForget_ForTask_ShouldCallAppropriateHandler() {
        // Arrange
        var goodTask = TestTask();
        var faultyTask = TestFaultyTask();
        var canceledTask = TestCanceledTask();

        var onException = For<Action<Exception>>();
        var onExceptionAndTask = For<Action<Task, Exception>>();

        var onCancel = For<Action<OperationCanceledException>>();
        var onCancelAndTask = For<Action<Task, OperationCanceledException>>();

        // Act & Assert
        goodTask.FireAndForget();
        canceledTask.FireAndForget();
        faultyTask.FireAndForget();

        goodTask.FireAndForget(onCancel);
        onCancel.DidNotReceive().Invoke(Any<OperationCanceledException>());

        canceledTask.FireAndForget(onCancel);
        onCancel.Received(1).Invoke(Any<OperationCanceledException>());
        onCancel.ClearReceivedCalls();

        faultyTask.FireAndForget(onCancel);
        onCancel.DidNotReceive().Invoke(Any<OperationCanceledException>());

        goodTask.FireAndForget(onCancelAndTask);
        onCancelAndTask.DidNotReceive().Invoke(goodTask, Any<OperationCanceledException>());

        canceledTask.FireAndForget(onCancelAndTask);
        onCancelAndTask.Received(1).Invoke(canceledTask, Any<OperationCanceledException>());
        onCancelAndTask.ClearReceivedCalls();

        faultyTask.FireAndForget(onCancelAndTask);
        onCancelAndTask.DidNotReceive().Invoke(faultyTask, Any<OperationCanceledException>());

        goodTask.FireAndForget(onException);
        onException.DidNotReceive().Invoke(Any<Exception>());

        canceledTask.FireAndForget(onException);
        onException.DidNotReceive().Invoke(Any<Exception>());

        faultyTask.FireAndForget(onException);
        onException.Received(1).Invoke(Any<Exception>());
        onException.ClearReceivedCalls();

        goodTask.FireAndForget(onExceptionAndTask);
        onExceptionAndTask.DidNotReceive().Invoke(goodTask, Any<Exception>());

        canceledTask.FireAndForget(onExceptionAndTask);
        onExceptionAndTask.DidNotReceive().Invoke(canceledTask, Any<Exception>());

        faultyTask.FireAndForget(onExceptionAndTask);
        onExceptionAndTask.Received(1).Invoke(faultyTask, Any<Exception>());
        onExceptionAndTask.ClearReceivedCalls();

        goodTask.FireAndForget(onCancel, onException);
        onException.DidNotReceive().Invoke(Any<Exception>());

        canceledTask.FireAndForget(onCancel, onException);
        onCancel.Received(1).Invoke(Any<OperationCanceledException>());
        onCancel.ClearReceivedCalls();
        onException.DidNotReceive().Invoke(Any<Exception>());

        faultyTask.FireAndForget(onCancel, onException);
        onCancel.DidNotReceive().Invoke(Any<OperationCanceledException>());
        onException.Received(1).Invoke(Any<Exception>());
        onException.ClearReceivedCalls();

        goodTask.FireAndForget(onCancelAndTask, onExceptionAndTask);
        onCancelAndTask.DidNotReceive().Invoke(goodTask, Any<OperationCanceledException>());
        onExceptionAndTask.DidNotReceive().Invoke(goodTask, Any<Exception>());

        canceledTask.FireAndForget(onCancelAndTask, onExceptionAndTask);
        onCancelAndTask.Received(1).Invoke(canceledTask, Any<OperationCanceledException>());
        onCancelAndTask.ClearReceivedCalls();
        onExceptionAndTask.DidNotReceive().Invoke(canceledTask, Any<Exception>());

        faultyTask.FireAndForget(onCancelAndTask, onExceptionAndTask);
        onCancelAndTask.DidNotReceive().Invoke(faultyTask, Any<OperationCanceledException>());
        onExceptionAndTask.Received(1).Invoke(faultyTask, Any<Exception>());
        onExceptionAndTask.ClearReceivedCalls();
    }

    [Fact]
    public void FireAndForget_ForTaskOfT_ShouldCallAppropriateHandler() {
        // Arrange
        var goodTask = TestTaskOfT();
        var faultyTask = TestFaultyTaskOfT();
        var canceledTask = TestCanceledTaskOfT();

        var onResult = For<Action<int>>();

        var onException = For<Action<Exception>>();
        var onExceptionAndTask = For<Action<Task<int>, Exception>>();

        var onCancel = For<Action<OperationCanceledException>>();
        var onCancelAndTask = For<Action<Task<int>, OperationCanceledException>>();

        // Act & Assert
        goodTask.FireAndForget(onResult);
        canceledTask.FireAndForget(onResult);
        faultyTask.FireAndForget(onResult);

        goodTask.FireAndForget(onResult, onCancel);
        onCancel.DidNotReceive().Invoke(Any<OperationCanceledException>());

        canceledTask.FireAndForget(onResult, onCancel);
        onCancel.Received(1).Invoke(Any<OperationCanceledException>());
        onCancel.ClearReceivedCalls();

        faultyTask.FireAndForget(onResult, onCancel);
        onCancel.DidNotReceive().Invoke(Any<OperationCanceledException>());

        goodTask.FireAndForget(onResult, onCancelAndTask);
        onCancelAndTask.DidNotReceive().Invoke(goodTask, Any<OperationCanceledException>());

        canceledTask.FireAndForget(onResult, onCancelAndTask);
        onCancelAndTask.Received(1).Invoke(canceledTask, Any<OperationCanceledException>());
        onCancelAndTask.ClearReceivedCalls();

        faultyTask.FireAndForget(onResult, onCancelAndTask);
        onCancelAndTask.DidNotReceive().Invoke(faultyTask, Any<OperationCanceledException>());

        goodTask.FireAndForget(onResult, onException);
        onException.DidNotReceive().Invoke(Any<Exception>());

        canceledTask.FireAndForget(onResult, onException);
        onException.DidNotReceive().Invoke(Any<Exception>());

        faultyTask.FireAndForget(onResult, onException);
        onException.Received(1).Invoke(Any<Exception>());
        onException.ClearReceivedCalls();

        goodTask.FireAndForget(onResult, onExceptionAndTask);
        onExceptionAndTask.DidNotReceive().Invoke(goodTask, Any<Exception>());

        canceledTask.FireAndForget(onResult, onExceptionAndTask);
        onExceptionAndTask.DidNotReceive().Invoke(canceledTask, Any<Exception>());

        faultyTask.FireAndForget(onResult, onExceptionAndTask);
        onExceptionAndTask.Received(1).Invoke(faultyTask, Any<Exception>());
        onExceptionAndTask.ClearReceivedCalls();

        goodTask.FireAndForget(onResult, onCancel, onException);
        onException.DidNotReceive().Invoke(Any<Exception>());

        canceledTask.FireAndForget(onResult, onCancel, onException);
        onCancel.Received(1).Invoke(Any<OperationCanceledException>());
        onCancel.ClearReceivedCalls();
        onException.DidNotReceive().Invoke(Any<Exception>());

        faultyTask.FireAndForget(onResult, onCancel, onException);
        onCancel.DidNotReceive().Invoke(Any<OperationCanceledException>());
        onException.Received(1).Invoke(Any<Exception>());
        onException.ClearReceivedCalls();

        goodTask.FireAndForget(onResult, onCancelAndTask, onExceptionAndTask);
        onCancelAndTask.DidNotReceive().Invoke(goodTask, Any<OperationCanceledException>());
        onExceptionAndTask.DidNotReceive().Invoke(goodTask, Any<Exception>());

        canceledTask.FireAndForget(onResult, onCancelAndTask, onExceptionAndTask);
        onCancelAndTask.Received(1).Invoke(canceledTask, Any<OperationCanceledException>());
        onCancelAndTask.ClearReceivedCalls();
        onExceptionAndTask.DidNotReceive().Invoke(canceledTask, Any<Exception>());

        faultyTask.FireAndForget(onResult, onCancelAndTask, onExceptionAndTask);
        onCancelAndTask.DidNotReceive().Invoke(faultyTask, Any<OperationCanceledException>());
        onExceptionAndTask.Received(1).Invoke(faultyTask, Any<Exception>());
        onExceptionAndTask.ClearReceivedCalls();
    }
}
