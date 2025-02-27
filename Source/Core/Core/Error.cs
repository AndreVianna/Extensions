namespace DotNetToolbox;

public record Error(string Message, params IReadOnlyList<string> Sources) {
    public static implicit operator Error(string message) => new(message);

    public void Deconstruct(out string message, out IReadOnlyList<string>? sources) {
        message = Message;
        sources = Sources;
    }
}

public record Error<TCode>(TCode Code, string Message, params IReadOnlyList<string> Sources)
    : Error(Message, Sources) {
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
