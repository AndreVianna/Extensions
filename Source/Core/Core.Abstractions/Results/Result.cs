namespace DotNetToolbox.Results;

public abstract record Result<TState>
    : IHasErrors
    , IHasState<TState>
    where TState : Enum {
    [SetsRequiredMembers]
    protected Result(TState state, IEnumerable<ResultError>? errors = null) {
        State = state;
        Errors = new ResultErrors(errors ?? []);
    }

    public TState State { get; protected init; }
    protected bool Is(TState state) => State.Equals(state);
    ISet<ResultError> IHasErrors.Errors => Errors;
    public HashSet<ResultError> Errors { get; }
    public bool HasErrors => Errors.Count != 0;

    public virtual void EnsureHasNoErrors() {
        if (HasErrors) throw new ResultException(Errors);
    }

    public override int GetHashCode()
        => HashCode.Combine(State.GetHashCode(), Errors.GetHashCode());
}

public abstract record Result<TState, TValue>
    : Result<TState>
    , IHasOutput<TValue>
    where TState : Enum {
    [SetsRequiredMembers]
    protected Result(TState state, TValue value, IEnumerable<ResultError>? errors = null)
        : base(state, errors) {
        Output = HasErrors ? default! : value;
    }

    [field: AllowNull, MaybeNull]
    public TValue Output {
        get { return HasErrors ? throw new ResultException(Errors) : field!; }
    }

    public virtual bool TryGetValue([NotNullWhen(true)] out TValue? value) {
        value = Output;
        return !HasErrors;
    }

    public override int GetHashCode()
        => HashCode.Combine(State.GetHashCode(), Output?.GetHashCode() ?? 0, Errors.GetHashCode());
}
