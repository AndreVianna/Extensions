# DotNetToolbox.Graph

## Introduction
DotNetToolbox.Graph is a robust C# library for .NET 8, designed to provide comprehensive graph-based workflow and execution capabilities. It offers a flexible framework for defining, parsing, and executing complex workflows, with features including node-based execution, conditional branching, and a powerful parser for textual workflow definitions.

## Installation
```shell
dotnet add package DotNetToolbox.Graph
```

## Dependencies
- .NET 8
- DotNetToolbox.Core (automatically included)

## Key Features

1. **Workflow Definition**: Define complex workflows using a node-based structure.
2. **Execution Engine**: Execute workflows with support for various node types.
3. **Conditional Logic**: Implement branching and decision-making within workflows.
4. **Parser**: Convert textual representations into executable workflow structures.
5. **Extensibility**: Easily extend with custom node types and execution strategies.
6. **Asynchronous Execution**: Support for asynchronous workflow operations.
7. **Retry Policies**: Implement custom retry strategies for error handling.
8. **Visualization**: Generate visual representations of workflows.

## Core Components

### Nodes
- `INode`: Base interface for all node types
- `ActionNode`: Represents an executable action
- `IfNode`: Implements conditional branching
- `CaseNode`: Allows for multi-way branching
- `ExitNode`: Defines workflow exit points
- `JumpNode`: Enables non-linear workflow execution

### Workflow
- `IWorkflow`: Interface defining workflow structure and execution
- `Workflow`: Concrete implementation of IWorkflow

### Runner
- `IRunner`: Interface for workflow execution
- `Runner`: Handles the execution of workflows

### Parser
- `WorkflowInterpreter`: Converts textual workflow definitions into executable structures
- `WorkflowLexer`: Tokenizes input for parsing
- `WorkflowParser`: Parses tokens into a workflow structure

### Builders
- `WorkflowBuilder`: Fluent API for constructing workflows programmatically
- `WorkflowGraph`: Generates visual representations of workflows

### Utilities
- `NodeSequence`: Manages unique identifiers for nodes
- `Map`: Holds execution context for workflows

## Detailed Component Descriptions

### Node Types

#### ActionNode
Represents a single executable action within a workflow.

```csharp
public sealed class ActionNode : ActionNode<ActionNode> {
    public ActionNode(string? name, IServiceProvider services)
    public ActionNode(string? tag, string? name, IServiceProvider services)
    public ActionNode(Func<Map, CancellationToken, Task> execute, IServiceProvider services)
    // Additional constructors...
}
```

#### IfNode
Implements conditional branching in the workflow.

```csharp
public class IfNode : IfNode<IfNode> {
    public IfNode(string? name, IServiceProvider services)
    public IfNode(Func<Map, CancellationToken, Task<bool>> predicate, IServiceProvider services)
    // Additional constructors...
}
```

#### CaseNode
Allows for multi-way branching based on a selection.

```csharp
public sealed class CaseNode : CaseNode<CaseNode> {
    public CaseNode(string? name, IServiceProvider services)
    public CaseNode(Func<Map, CancellationToken, Task<string>> select, IServiceProvider services)
    // Additional constructors...
}
```

#### ExitNode
Defines exit points in the workflow.

```csharp
public sealed class ExitNode : ExitNode<ExitNode> {
    public ExitNode(string? tag, int exitCode, IServiceProvider services)
    public ExitNode(int exitCode, IServiceProvider services)
    // Additional constructors...
}
```

#### JumpNode
Enables non-linear execution by jumping to tagged nodes.

```csharp
public sealed class JumpNode : JumpNode<JumpNode> {
    public JumpNode(string targetTag, IServiceProvider services)
}
```

### Workflow Execution

#### Workflow
The main class for defining and managing workflows.

```csharp
public class Workflow : IWorkflow {
    public Workflow(string id, INode start, Map context, IDateTimeProvider? dateTime = null, IGuidProvider? guid = null, ILoggerFactory? loggerFactory = null)
    public Task Run(CancellationToken ct = default)
    public Result Validate()
}
```

#### Runner
Handles the execution of workflows.

```csharp
public sealed class Runner : IRunner {
    public Runner(string id, Workflow workflow, IDateTimeProvider? dateTime = null, ILoggerFactory? loggerFactory = null)
    public Task Run(CancellationToken ct = default)
    // Event handlers for workflow execution stages
    public Func<IWorkflow, CancellationToken, Task>? OnStartingWorkflow { set; }
    public Func<IWorkflow, INode, CancellationToken, Task<bool>>? OnExecutingNode { set; }
    public Func<IWorkflow, INode, INode?, CancellationToken, Task<bool>>? OnNodeExecuted { set; }
    public Func<IWorkflow, CancellationToken, Task>? OnWorkflowEnded { set; }
}
```

### Parser Components

#### WorkflowInterpreter
Interprets textual workflow definitions.

```csharp
public class WorkflowInterpreter {
    public WorkflowInterpreter(IServiceProvider services)
    public Result<INode?> InterpretScript(string script)
}
```

#### WorkflowLexer
Tokenizes input for parsing.

```csharp
public sealed class WorkflowLexer {
    public static IEnumerable<Token> Tokenize(string input)
}
```

#### WorkflowParser
Parses tokens into a workflow structure.

```csharp
public sealed class WorkflowParser {
    public static Result<INode?> Parse(IEnumerable<Token> tokens, IServiceProvider services)
}
```

### Builder and Visualization

#### WorkflowBuilder
Provides a fluent API for constructing workflows.

```csharp
public sealed class WorkflowBuilder : IExitBuilder, IIfBuilder, IElseBuilder, IOtherwiseBuilder, IActionBuilder {
    public WorkflowBuilder(IServiceProvider services)
    public INode Build()
    // Various methods for adding nodes and defining workflow structure
    public IWorkflowBuilder Do<TAction>(params object[] args) where TAction : ActionNode<TAction>
    public IIfBuilder If(Func<Map, bool> predicate)
    public ICaseBuilder Case(string selector)
    // Additional methods...
}
```

#### WorkflowGraph
Generates visual representations of workflows.

```csharp
public sealed class WorkflowGraph : IWorkflowGraphSettings {
    public static string Draw(INode node, Action<IWorkflowGraphSettings>? configure = null)
    public IWorkflowGraphSettings Format(GraphFormat format)
    public IWorkflowGraphSettings Direction(GraphDirection direction)
}
```

## Usage Examples

### Defining a Simple Workflow

```csharp
var services = new ServiceCollection().BuildServiceProvider();
var builder = new WorkflowBuilder(services);

var workflow = new Workflow(
    "SimpleWorkflow",
    builder
        .Do("Initialize")
        .If(ctx => ctx.GetData<bool>("condition"))
            .Then(b => b.Do("TrueAction"))
            .Else(b => b.Do("FalseAction"))
        .Do("Finalize")
        .Exit()
        .Build(),
    new Map()
);

await workflow.Run();
```

### Parsing a Workflow from Text

```csharp
var script = @"
Initialize
IF CheckCondition
  TrueAction
ELSE
  FalseAction
Finalize
EXIT
";

var interpreter = new WorkflowInterpreter(services);
var result = interpreter.InterpretScript(script);

if (result.IsSuccessful)
{
    var workflow = new Workflow("ParsedWorkflow", result.Value!, new Map());
    await workflow.Run();
}
```

### Visualizing a Workflow

```csharp
var graph = WorkflowGraph.Draw(workflow.StartNode, settings => {
    settings.Format(GraphFormat.Default)
            .Direction(GraphDirection.Vertical);
});

Console.WriteLine(graph);
```

## Extending the Library

### Creating a Custom Node Type

```csharp
public class CustomNode : Node<CustomNode>
{
    public CustomNode(string? tag, IServiceProvider services) : base(tag, services)
    {
        Label = "CustomNode";
    }

    protected override Task UpdateState(Map context, CancellationToken ct = default)
    {
        // Custom logic here
        return Task.CompletedTask;
    }

    protected override Task<INode?> SelectPath(Map context, CancellationToken ct = default)
    {
        return Task.FromResult(Next);
    }
}
```

### Implementing a Custom Retry Policy

```csharp
public class CustomRetryPolicy : RetryPolicy<CustomRetryPolicy>
{
    public CustomRetryPolicy() : base(3, TimeSpan.FromSeconds(5))
    {
    }

    protected override async Task<bool> TryExecute(Func<Map, CancellationToken, Task> action, Map ctx, CancellationToken ct)
    {
        // Custom retry logic
        await action(ctx, ct);
        return true;
    }
}
```

## Contributing
Contributions to DotNetToolbox.Graph are welcome. Please ensure that your code adheres to the project's coding standards and is covered by unit tests.

## License
This project is licensed under the MIT License - see the LICENSE file for details.