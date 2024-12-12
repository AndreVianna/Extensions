namespace DotNetToolbox.Graph.Factories;

public interface INodeFactory {
    TNode Create<TNode>(string? tag = null, params object[] args)
        where TNode : Node<TNode>;

    IActionNode CreateAction(Func<IMap, CancellationToken, Task> action);
    IActionNode CreateAction(string tag, Func<IMap, CancellationToken, Task> action);
    IActionNode CreateAction(Action<IMap> action);
    IActionNode CreateAction(string tag, Action<IMap> action);

    IIfNode CreateIf(Func<IMap, CancellationToken, Task<bool>> predicate);
    IIfNode CreateIf(string tag, Func<IMap, CancellationToken, Task<bool>> predicate);
    IIfNode CreateIf(string tag,
                     Func<IMap, CancellationToken, Task<bool>> predicate,
                     INode truePath,
                     INode? falsePath = null);
    IIfNode CreateIf(Func<IMap, CancellationToken, Task<bool>> predicate,
                   INode truePath,
                   INode? falsePath = null);
    IIfNode CreateIf(Func<IMap, bool> predicate);
    IIfNode CreateIf(string tag, Func<IMap, bool> predicate);
    IIfNode CreateIf(string tag,
                     Func<IMap, bool> predicate,
                     INode truePath,
                     INode? falsePath = null);
    IIfNode CreateIf(Func<IMap, bool> predicate,
                   INode truePath,
                   INode? falsePath = null);

    ICaseNode CreateCase(Func<IMap, CancellationToken, Task<string>> selectPath);
    ICaseNode CreateCase(string tag, Func<IMap, CancellationToken, Task<string>> selectPath);
    ICaseNode CreateCase(string tag,
                         Func<IMap, CancellationToken, Task<string>> selectPath,
                         Dictionary<string, INode?> choices,
                         INode? otherwise = null);
    ICaseNode CreateCase(Func<IMap, CancellationToken, Task<string>> selectPath,
                         Dictionary<string, INode?> choices,
                         INode? otherwise = null);
    ICaseNode CreateCase(Func<IMap, string> selectPath);
    ICaseNode CreateCase(string tag, Func<IMap, string> selectPath);
    ICaseNode CreateCase(string tag,
                         Func<IMap, string> selectPath,
                         Dictionary<string, INode?> choices,
                         INode? otherwise = null);
    ICaseNode CreateCase(Func<IMap, string> selectPath,
                         Dictionary<string, INode?> choices,
                         INode? otherwise = null);

    IJumpNode CreateJump(string targetTag);

    IExitNode CreateExit(int exitCode = 0);
    IExitNode CreateExit(string tag, int exitCode = 0);
}
