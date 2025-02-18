namespace DotNetToolbox.Validation;

public interface IValidatable {
    IResult Validate(IMap? context = null);
}

public interface IValidatableAsync {
    Task<IResult> Validate(IMap? context = null, CancellationToken token = default);
}
