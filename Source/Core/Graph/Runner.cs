﻿namespace DotNetToolbox.Graph;

public sealed class Runner(Workflow workflow,
                           IDateTimeProvider? dateTime = null,
                           ILoggerFactory? loggerFactory = null)
    : IRunner {
    private readonly Workflow _workflow = IsNotNull(workflow);
    private readonly IDateTimeProvider _dateTime = dateTime ?? DateTimeProvider.Default;
    private readonly ILogger _logger = loggerFactory?.CreateLogger<Runner>() ?? NullLogger<Runner>.Instance;

    public event EventHandler<NodeEventArgs>? OnNodeExecuting;
    public event EventHandler<NodeEventArgs>? OnNodeExecuted;
    public event EventHandler<WorkflowEventArgs>? OnRunStarting;
    public event EventHandler<WorkflowEventArgs>? OnRunEnded;

    public DateTimeOffset? Start { get; private set; }
    public DateTimeOffset? End { get; private set; }
    public TimeSpan? ElapsedTime => End is null || Start is null ? null : End - Start;

    public bool HasStarted => Start is not null;
    public bool HasStopped => End is not null;
    public bool IsRunning => HasStarted && !HasStopped;

    public void Run() {
        try {
            if (IsRunning)
                throw new InvalidOperationException("This runner is already being executed.");
            Start = _dateTime.UtcNow;
            StartingRun(new(_workflow));
            var currentNode = _workflow.StartingNode;

            while (currentNode is not null) {
                if (!ExecutingNode(new(_workflow.Context, currentNode)))
                    break;
                var result = currentNode.Run(_workflow.Context);
                if (!NodeExecuted(new(_workflow.Context, currentNode)))
                    break;
                currentNode = result;
            }
        }
        catch (Exception ex) {
            _logger.LogError(ex, message: "An error occurred while running the workflow '{Id}'.", _workflow.Id);
            throw;
        }
        finally {
            End = _dateTime.UtcNow;
            RunEnded(new(_workflow));
        }
    }

    private void StartingRun(WorkflowEventArgs e) {
        _logger.LogInformation(message: "Starting run of workflow '{Id}' at '{Start}'...", _workflow.Id, Start);
        OnRunStarting?.Invoke(this, e);
    }

    private bool ExecutingNode(NodeEventArgs e) {
        if (OnNodeExecuting is null)
            return e.Node is not null;
        _logger.LogInformation(message: "Executing node '{Id}'...", _workflow.Id);

        OnNodeExecuting?.Invoke(this, e);
        return !e.Cancel;
    }

    private bool NodeExecuted(NodeEventArgs e) {
        if (OnNodeExecuted is null)
            return e.Node is not null;

        OnNodeExecuted?.Invoke(this, e);
        _logger.LogInformation(message: "Node '{Id}' executed.", _workflow.Id);
        return !e.Cancel;
    }

    private void RunEnded(WorkflowEventArgs e) {
        OnRunEnded?.Invoke(this, e);
        _logger.LogInformation(message: "Run of workflow '{Id}' ended at '{End}' after '{ElapsedTime}' minutes.", _workflow.Id, End, ElapsedTime!.Value.TotalMinutes);
    }
}
