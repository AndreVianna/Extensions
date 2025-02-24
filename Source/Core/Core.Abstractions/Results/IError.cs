
namespace DotNetToolbox.Results;

public interface IError {
    string Message { get; }
    IReadOnlyList<string> Sources { get; }
}

public interface IError<out TCode>
    : IError {
    TCode Code { get; }
}
