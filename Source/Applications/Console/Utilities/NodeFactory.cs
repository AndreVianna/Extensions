namespace DotNetToolbox.ConsoleApplication.Utilities;

public static class NodeFactory {
    internal static TChild Create<TChild>(IHasChildren parent, string name, Delegate? configure, Delegate? action)
        where TChild : class, IHasParent {
        var configureWrapper = ConvertToAction<TChild>(configure);
        var actionWrapper = ConvertToTask<TChild>(action);
        var child = InstanceFactory.Create<TChild>(parent, name, configureWrapper, actionWrapper);
        parent.Children.Add(child);
        return child;
    }

    internal static TChild Create<TChild>(IHasChildren parent, string name, Delegate? configure)
        where TChild : class, IHasParent {
        var configureWrapper = ConvertToAction<TChild>(configure);
        var child = InstanceFactory.Create<TChild>(parent, name, configureWrapper);
        parent.Children.Add(child);
        return child;
    }

    internal static TChild Create<TChild>(IHasChildren parent, string name)
        where TChild : class, IHasParent {
        var child = InstanceFactory.Create<TChild>(parent, name);
        parent.Children.Add(child);
        return child;
    }

    internal static TChild Create<TChild>(IHasChildren parent)
        where TChild : class, IHasParent {
        var child = InstanceFactory.Create<TChild>(parent.Application.Services, parent);
        parent.Children.Add(child);
        return child;
    }

    private static Action<TNode> ConvertToAction<TNode>(Delegate? action)
        => action switch {
            null => _ => { }
            ,
            Action func => _ => func(),
            Action<TNode> func => n => func(n),
            _ => throw new ArgumentException("Unsupported delegate type of configuration action", nameof(action)),
        };

    private static Func<TNode, CancellationToken, Task<Result>> ConvertToTask<TNode>(Delegate? action)
        => action switch {
            null => (_, _) => Task.FromResult(Success()),
            Action func => (_, ct) => Task.Run(() => func(), ct).ContinueWith(_ => Success(), ct, TaskContinuationOptions.NotOnFaulted, TaskScheduler.Current),
            Action<TNode> func => (c, ct) => Task.Run(() => func(c), ct).ContinueWith(_ => Success(), ct, TaskContinuationOptions.NotOnFaulted, TaskScheduler.Current),
            Func<Result> func => (_, _) => Task.FromResult(func()),
            Func<Task<Result>> func => (_, _) => func(),
            Func<Task> func => (_, ct) => func().ContinueWith(_ => Success(), ct),
            Func<CancellationToken, Task<Result>> func => (_, ct) => func(ct),
            Func<CancellationToken, Task> func => (_, ct) => func(ct).ContinueWith(_ => Success(), ct),
            Func<TNode, Result> func => (cmd, _) => Task.FromResult(func(cmd)),
            Func<TNode, Task<Result>> func => (cmd, _) => func(cmd),
            Func<TNode, Task> func => (cmd, ct) => func(cmd).ContinueWith(_ => Success(), ct),
            Func<TNode, CancellationToken, Task<Result>> func => (cmd, ct) => func(cmd, ct),
            Func<TNode, CancellationToken, Task> func => (cmd, ct) => func(cmd, ct).ContinueWith(_ => Success(), ct),
            _ => throw new ArgumentException("Unsupported delegate type for action", nameof(action)),
        };
}
