namespace DotNetToolbox.Graph.Parser;

public sealed class WorkflowParser {
    private readonly IEnumerator<Token> _tokens;
    private readonly List<INode> _nodes = [];
    private readonly ResultErrors _errors = [];
    private readonly IServiceProvider _services;
    private int _indentLevel = -1;

    private WorkflowParser(IEnumerable<Token> tokens, IServiceProvider services) {
        _services = services;
        _tokens = tokens.GetEnumerator();
        _tokens.MoveNext();
    }

    public static IValidationResult<INode?> Parse(IEnumerable<Token> tokens, IServiceProvider services) {
        var parser = new WorkflowParser(tokens, services);
        var blockStart = parser.ParseBlock();
        parser.ConnectJumps();
        return From(blockStart, parser._errors);
    }

    private INode? ParseBlock() {
        IncreaseIndent();
        var first = default(INode);
        var lastNode = default(INode);
        var finishBlock = IsEndOfFile();
        while (!finishBlock) {
            var newNode = ParseStatement();
            ConnectNode(lastNode, newNode);
            first ??= newNode;
            lastNode = newNode;
            if (newNode is not null) _nodes.Add(newNode);
            finishBlock = IsEndOfFile() || IsOutdented();
        }
        return first;
    }

    private void ConnectNode(INode? source, INode? target) {
        try {
            source?.ConnectTo(target);
        }
        catch (Exception ex) {
            AddError(ex.Message, target?.Token);
        }
    }

    private bool IsEndOfFile()
        => _tokens.Current.Type is TokenType.EndOfFile;

    private void EnsureIndented() {
        var count = CountToken(TokenType.Indent);
        var isIndented = _indentLevel < count;
        if (_indentLevel > count) AddError($"Invalid indentation. Expected '{_indentLevel}' but found {count}.");
        if (isIndented) _indentLevel = count;
    }

    private void IncreaseIndent() {
        _indentLevel++;
        EnsureIndented();
    }

    private bool IsOutdented() {
        var count = CountToken(TokenType.Indent);
        var isOutdented = _indentLevel > count;
        if (_indentLevel < count) AddError($"Invalid indentation. Expected '{_indentLevel}' but found {count}.");
        if (isOutdented) _indentLevel = count;
        return isOutdented;
    }

    private void DecreaseIndent() {
        if (_indentLevel > 0) _indentLevel--;
        IsOutdented();
    }

    private INode? ParseStatement()
        => _tokens.Current.Type switch {
            TokenType.Identifier => ParseAction(),
            TokenType.If => ParseIf(),
            TokenType.Case => ParseCase(),
            TokenType.Exit => ParseExit(),
            TokenType.GoTo => ParseJumpTo(),
            TokenType.EndOfLine => ParseEndOfLine(),
            TokenType.Error => ParseError(),
            _ => ParseUnknownToken(),
        };

    private INode? ParseUnknownToken() {
        AddError($"Unexpected token: '{_tokens.Current.Type}'.");
        _tokens.MoveNext();
        return null;
    }

    private INode? ParseError() {
        AddError(_tokens.Current.Value);
        _tokens.MoveNext();
        return null;
    }

    private INode? ParseEndOfLine() {
        _tokens.MoveNext();
        return null;
    }

    private INode ParseAction() {
        var token = _tokens.Current;
        var actionName = GetValue(TokenType.Identifier);
        var tag = GetValueOrDefault(TokenType.Tag);
        Ensure(TokenType.EndOfLine);
        var retry = _services.GetService<IRetryPolicy>() ?? RetryPolicy.Default;
        return new ActionNode(tag, actionName, _services) {
            Token = token,
            Retry = retry,
        };
    }

    private INode ParseIf() {
        var token = _tokens.Current;
        Ensure(TokenType.If);
        var tag = GetValueOrDefault(TokenType.Tag);
        var predicateName = GetValue(TokenType.Identifier);
        Ensure(TokenType.EndOfLine);
        var node = new IfNode(tag, predicateName, _services) {
            Token = token,
            Then = ParseBlock(),
            Else = ParseElse(),
        };
        if (node.Then is null)
            AddError("If statement must have a body.");
        return node;
    }

    private INode? ParseElse() {
        if (!TokenIs(TokenType.Else))
            return null;
        Ensure(TokenType.Else);
        Ensure(TokenType.EndOfLine);
        return ParseBlock();
    }

    private INode ParseCase() {
        var token = _tokens.Current;
        Ensure(TokenType.Case);
        var selector = GetValue(TokenType.Identifier);
        var tag = GetValueOrDefault(TokenType.Tag);
        Ensure(TokenType.EndOfLine);

        var node = new CaseNode(tag, selector, _services) { Token = token };
        foreach ((var key, var choice) in ParseChoices())
            node.Choices.Add(key, choice);
        return node;
    }

    private IEnumerable<(string, INode?)> ParseChoices() {
        IncreaseIndent();
        while (TryParseCaseOption(out var choice))
            yield return choice;
        if (TryParseOtherwise(out var otherwise))
            yield return (string.Empty, otherwise);
        DecreaseIndent();
    }

    private bool TryParseCaseOption(out (string, INode?) result) {
        result = default;
        if (!TokenIs(TokenType.Is)) return false;

        Ensure(TokenType.Is);
        var key = GetValue(TokenType.String);
        Ensure(TokenType.EndOfLine);
        result = (key, ParseBlock());
        return true;
    }

    private bool TryParseOtherwise(out INode? result) {
        result = default;
        if (!TokenIs(TokenType.Otherwise)) return false;

        Ensure(TokenType.Otherwise);
        Ensure(TokenType.EndOfLine);
        result = ParseBlock();
        Forbid(TokenType.Otherwise);
        return true;
    }

    private INode ParseExit() {
        var token = _tokens.Current;
        Ensure(TokenType.Exit);
        var number = GetValueOrDefault(TokenType.Number); // if number not found default to 0
        if (!int.TryParse(number ?? "0", out var exitCode))
            AddError("Exit code must be a number.");
        var tag = GetValueOrDefault(TokenType.Tag);
        Ensure(TokenType.EndOfLine);

        return new ExitNode(tag, exitCode, _services) { Token = token };
    }

    private INode ParseJumpTo() {
        var token = _tokens.Current;
        Ensure(TokenType.GoTo);
        var target = GetValue(TokenType.Identifier);
        Ensure(TokenType.EndOfLine);

        return new JumpNode(target, _services) { Token = token };
    }

    private void AddError(string? message, Token? token = null) {
        var source = (token ?? _tokens.Current).ToSource();
        message ??= "Unknown error.";
        _errors.Add(new ResultError(message, source));
    }

    private int CountToken(TokenType type) {
        var count = 0;
        while (TokenIs(type)) {
            count++;
            _tokens.MoveNext();
        }

        return count;
    }
    private bool TokenIs(TokenType type)
        => _tokens.Current.Type == type;

    private void Ensure(TokenType type) {
        if (!TokenIs(type)) {
            AddError($"'{type}' expected but found '{_tokens.Current.Type}'.");
            return;
        }
        _tokens.MoveNext();
    }

    private void Forbid(TokenType type) {
        if (TokenIs(type))
            AddError($"'{type}' not allowed here.");
    }

    private string GetValue(TokenType type) {
        var value = _tokens.Current.Value;
        if (!TokenIs(type)) AddError($"'{type}' expected but found '{_tokens.Current.Type}'.");
        else _tokens.MoveNext();
        if (value is null) AddError("Required value not found.");
        return value ?? string.Empty;
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    private string? GetValueOrDefault(TokenType type, string? defaultValue = null) {
        if (!TokenIs(type))
            return defaultValue;
        var value = _tokens.Current.Value ?? defaultValue;
        _tokens.MoveNext();
        return value;
    }

    private void ConnectJumps() {
        foreach (var jumpNode in _nodes.OfType<IJumpNode>()) {
            var targetNode = _nodes.Find(n => n.Tag == jumpNode.TargetTag);
            if (targetNode is null) AddError($"Jump target '{jumpNode.TargetTag}' not found.", jumpNode.Token);
            else jumpNode.ConnectTo(targetNode);
        }
    }
}
