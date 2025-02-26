namespace DotNetToolbox;

public record Error
    : IError {
    public Error(string message, params IReadOnlyList<string> sources) {
        Message = message;
        Sources = sources ?? [];
    }

    public string Message { get; }
    public IReadOnlyList<string> Sources { get; }

    public static implicit operator Error(string message) => new(message);

    public void Deconstruct(out string message, out IReadOnlyList<string>? sources) {
        message = Message;
        sources = Sources;
    }
}

public record Error<TCode>
    : Error
    , IError<TCode> {
    public Error(TCode code, string message, params IReadOnlyList<string> sources)
        : base(message, sources){
        Code = code;
    }

    public TCode Code { get; }

    public void Deconstruct(out TCode code, out string message, out IReadOnlyList<string>? sources) {
        code = Code;
        message = Message;
        sources = Sources;
    }

    public void Deconstruct(out TCode code, out string message) {
        code = Code;
        message = Message;
    }
}
