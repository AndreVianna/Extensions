namespace DotNetToolbox.Results;

public abstract record OperationResult<TType>
    : IHasErrors
    where TType : Enum {
    [SetsRequiredMembers]
    protected OperationResult(TType state, IEnumerable<OperationError>? errors = null) {
        State = state;
        Errors = new OperationErrors(errors ?? []);
    }

    protected TType State { get; init; }
    protected bool Is(TType state) => State.Equals(state);
    public IEnumerable<OperationError> Errors { get; init; }
    public bool HasErrors => Errors.Any();

    public virtual void EnsureHasNoErrors() {
        if (HasErrors) throw new OperationException(Errors);
    }
}

public abstract record OperationResult<TType, TValue>
    : OperationResult<TType>
    , IReturnsValue<TValue>
    where TType : Enum {
    [SetsRequiredMembers]
    protected OperationResult(TType state, TValue value, IEnumerable<OperationError>? errors = null)
        : base(state, errors) {
        Value = value;
    }

    [SetsRequiredMembers]
    protected OperationResult(TType state, IEnumerable<OperationError>? errors = null)
        : this(state, default!, errors) {
    }

    public TValue Value { get; }

    public virtual bool TryGetValue([NotNullWhen(true)] out TValue value) {
        value = Value;
        return !HasErrors;
    }
}
