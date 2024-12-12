namespace DotNetToolbox.Graph;

public class Runner(uint id,
                    IWorkflow workflow,
                    IDateTimeProvider? dateTime = null,
                    ILoggerFactory? loggerFactory = null)
    : Runner<Map>(id,
                  workflow,
                  dateTime,
                  loggerFactory);

public class Runner<TContext>(uint id,
                              IWorkflow<TContext> workflow,
                              IDateTimeProvider? dateTime = null,
                              ILoggerFactory? loggerFactory = null)
    : Runner<Runner<TContext>, TContext>(id,
                                         workflow,
                                         dateTime,
                                         loggerFactory)
    where TContext : IMap;

public class Runner<TRunner, TContext>(uint id,
                                       IWorkflow<TContext> workflow,
                                       IDateTimeProvider? dateTime = null,
                                       ILoggerFactory? loggerFactory = null)
    : IRunner<TContext>
    where TRunner : Runner<TRunner, TContext>
    where TContext : IMap {
    private readonly IWorkflow<TContext> _workflow = IsNotNull(workflow);
    private readonly IDateTimeProvider _dateTime = dateTime ?? DateTimeProvider.Default;
    private readonly ILogger _logger = loggerFactory?.CreateLogger<TRunner>() ?? NullLogger<TRunner>.Instance;

    public string WorkflowId => _workflow.Id;
    public uint Id { get; } = id;

    public Func<IWorkflow<TContext>, CancellationToken, Task>? OnStartingWorkflow { private get; set; }
    public Func<IWorkflow<TContext>, INode, CancellationToken, Task<bool>>? OnExecutingNode { private get; set; }
    public Func<IWorkflow<TContext>, INode, INode?, CancellationToken, Task<bool>>? OnNodeExecuted { private get; set; }
    public Func<IWorkflow<TContext>, CancellationToken, Task>? OnWorkflowEnded { private get; set; }

    public DateTimeOffset? Start { get; private set; }
    public DateTimeOffset? End { get; private set; }
    public TimeSpan? ElapsedTime => End is null || Start is null ? null : End - Start;

    public bool HasStarted => Start is not null;
    public bool HasStopped => End is not null;
    public bool IsRunning => HasStarted && !HasStopped;

    public async Task Run(CancellationToken ct = default) {
        try {
            if (IsRunning) throw new InvalidOperationException("This runner is already being executed.");
            Start = _dateTime.UtcNow;
            await StartingRun(_workflow, ct);
            var currentNode = _workflow.StartNode;

            while (currentNode is not null) {
                if (!await ExecutingNode(_workflow, currentNode, ct)) break;
                var nextNode = await currentNode.Run(_workflow.Context, ct);
                if (!await NodeExecuted(_workflow, currentNode, nextNode, ct)) break;
                currentNode = nextNode;
            }
        }
        catch (OperationCanceledException ex) {
            _logger.LogError(ex, message: "The run '{RunId}' of the workflow '{WorkFlowId}' was cancelled.", Id, _workflow.Id);
            throw;
        }
        catch (Exception ex) {
            _logger.LogError(ex, message: "An error occurred during the run '{RunId}' the workflow '{WorkFlowId}'.", Id, _workflow.Id);
            throw;
        }
        finally {
            End = _dateTime.UtcNow;
            await RunEnded(_workflow, ct);
        }
    }

    private Task StartingRun(IWorkflow<TContext> workflow, CancellationToken ct = default) {
        _logger.LogInformation(message: "Starting workflow '{WorkFlowId}' at '{_first}'...", Id, Start);
        return OnStartingWorkflow is null
                   ? Task.CompletedTask
                   : OnStartingWorkflow(workflow, ct);
    }

    private Task<bool> ExecutingNode(IWorkflow<TContext> workflow, INode currentNode, CancellationToken ct = default) {
        _logger.LogInformation(message: "Executing node '{Tag}' during the workflow '{WorkFlowId}'...", currentNode.Id, Id);
        return OnExecutingNode is null
                   ? Task.FromResult(true)
                   : OnExecutingNode(workflow, currentNode, ct);
    }

    private Task<bool> NodeExecuted(IWorkflow<TContext> workflow, INode currentNode, INode? nextNode, CancellationToken ct = default) {
        _logger.LogInformation(message: "Node '{NodeNumberProvider}' executed during the workflow '{WorkFlowId}'.", currentNode.Id, Id);
        return OnNodeExecuted is null
                   ? Task.FromResult(true)
                   : OnNodeExecuted(workflow, currentNode, nextNode, ct);
    }

    private Task RunEnded(IWorkflow<TContext> workflow, CancellationToken ct = default) {
        _logger.LogInformation(message: "The workflow '{WorkFlowId}' ended at '{Exit}' after '{ElapsedTimeInMilliseconds}' milliseconds.", Id, End, ElapsedTime!.Value.TotalMilliseconds);
        return OnWorkflowEnded is null
            ? Task.CompletedTask
            : OnWorkflowEnded(workflow, ct);
    }
}
