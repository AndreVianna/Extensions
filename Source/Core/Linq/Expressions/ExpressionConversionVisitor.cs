namespace System.Linq.Expressions;

public class ExpressionConversionVisitor(IEnumerable<ParameterExpression> parentParameters, IEnumerable<TypeMapper> mappers)
    : ExpressionVisitor {
    private readonly List<ParameterExpression> _parameters = [.. IsNotNull(parentParameters)];

    public ExpressionConversionVisitor(params TypeMapper[] mappers)
        : this([], mappers) {
    }

    protected override Expression VisitLambda<TDelegate>(Expression<TDelegate> node) {
        var lambdaParameters = node.Parameters.ToList(i => (ParameterExpression)Visit(i));
        _parameters.AddRange(lambdaParameters);
        var visitor = new ExpressionConversionVisitor(_parameters, mappers);
        var body = visitor.Visit(node.Body);
        return Expression.Lambda(body, lambdaParameters);
    }

    protected override Expression VisitParameter(ParameterExpression node) {
        var typeMapping = GetTypeMapper(node.Type);
        return typeMapping == null
            ? base.VisitParameter(node)
            : Expression.Parameter(typeMapping.TargetType, node.Name);
    }

    protected override Expression VisitConstant(ConstantExpression node) {
        if (node.Value is null)
            return base.VisitConstant(node);
        var typeMapping = GetTypeMapper(node.Value.GetType());
        if (typeMapping == null)
            return base.VisitConstant(node);
        var convertedValue = typeMapping.Convert?.Invoke(node.Value) ?? node.Value;
        return Expression.Constant(convertedValue, typeMapping.TargetType);
    }

    protected override Expression VisitMember(MemberExpression node) {
        var typeMapping = GetTypeMapper(node.Member.DeclaringType);
        if (typeMapping == null)
            return base.VisitMember(node);
        var newMember = typeMapping.TargetType.GetMember(node.Member.Name).FirstOrDefault()
            ?? throw new InvalidOperationException($"No member with the name {node.Member.Name} exists on type {typeMapping.TargetType.Name}.");
        var newExpression = Visit(node.Expression);
        return Expression.MakeMemberAccess(newExpression, newMember);
    }

    protected override Expression VisitMethodCall(MethodCallExpression node) {
        try {
            var method = node.Method;
            var arguments = node.Arguments.ToArray(Visit);
            if (method.IsGenericMethod) {
                var genericArguments = method.GetGenericArguments();
                var transformedGenericArguments = genericArguments.ToArray(t => GetTypeMapper(t)?.TargetType ?? t);
                method = method.GetGenericMethodDefinition().MakeGenericMethod(transformedGenericArguments);
            }

            var caller = Visit(node.Object);
#pragma warning disable CS8620
            return Expression.Call(caller, method, arguments);
#pragma warning restore CS8620
        }
        catch (Exception ex) {
            throw new InvalidOperationException($"Failed to convert method call '{node.Method.Name}'.", ex);
        }
    }

    protected override Expression VisitBinary(BinaryExpression node) {
        var left = Visit(node.Left);
        var right = Visit(node.Right);

        var conversion = VisitAndConvert(node.Conversion, nameof(VisitBinary));

        return Expression.MakeBinary(node.NodeType, left, right, node.IsLiftedToNull, node.Method, conversion);
    }

    protected override Expression VisitUnary(UnaryExpression node) {
        var operand = Visit(node.Operand);
        return Expression.MakeUnary(node.NodeType, operand, node.Type, node.Method);
    }

    protected override Expression VisitConditional(ConditionalExpression node) {
        var test = Visit(node.Test);
        var ifTrue = Visit(node.IfTrue);
        var ifFalse = Visit(node.IfFalse);
        return Expression.Condition(test, ifTrue, ifFalse);
    }

    protected override Expression VisitNew(NewExpression node) {
        var typeMapping = GetTypeMapper(node.Type);
        if (typeMapping == null)
            return base.VisitNew(node);
        var arguments = node.Arguments.ToArray(Visit);
        var types = arguments.ToArray(a => a!.Type);
        var constructor = typeMapping.TargetType.GetConstructor(types)
                       ?? throw new InvalidOperationException($"No matching constructor for type '{typeMapping.TargetType.Name}'");
        var members = arguments.OfType<MemberExpression>().ToArray(m => m.Member);
#pragma warning disable CS8620
        return Expression.New(constructor, arguments, members);
#pragma warning restore CS8620
    }

    protected override Expression VisitMemberInit(MemberInitExpression node) {
        var newExpression = (NewExpression)VisitNew(node.NewExpression);
        var bindings = node.Bindings.Select(VisitMemberBinding).ToList();
        return Expression.MemberInit(newExpression, bindings);
    }

    protected override Expression VisitNewArray(NewArrayExpression node) {
        var expressions = node.Expressions.Select(Visit);
        var elementType = node.Type.GetElementType()!;
        elementType = GetTypeMapper(elementType)?.TargetType ?? elementType;
        return node.NodeType == ExpressionType.NewArrayInit
            ? Expression.NewArrayInit(elementType, expressions!)
            : (Expression)Expression.NewArrayBounds(elementType, expressions!);
    }

    private TypeMapper? GetTypeMapper(Type? sourceType)
        => mappers.FirstOrDefault(m => m.SourceType == sourceType);
}
