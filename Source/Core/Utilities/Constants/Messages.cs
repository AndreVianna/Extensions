namespace DotNetToolbox.Constants;

public static class Messages {
    public static CompositeFormat MustBeNull { get; } = CompositeFormat.Parse("Value must be null.");
    public static CompositeFormat MustBeEmpty { get; } = CompositeFormat.Parse("Value must be empty.");
    public static CompositeFormat MustBeEmptyOrWhiteSpace { get; } = CompositeFormat.Parse("Value must be empty or white space.");
    public static CompositeFormat ValueIsValid { get; } = CompositeFormat.Parse("Value is valid.");
    public static CompositeFormat MustBeValid { get; } = CompositeFormat.Parse("Value must be valid.");
    public static CompositeFormat MustBeAValidEmail { get; } = CompositeFormat.Parse("Value must be a valid email.");
    public static CompositeFormat MustBeAValidPassword { get; } = CompositeFormat.Parse("Value must be a valid password.");
    public static CompositeFormat CollectionMustBeEmpty { get; } = CompositeFormat.Parse("The collection must be empty.");
    public static CompositeFormat CollectionMustContainInvalid { get; } = CompositeFormat.Parse("The collection must contain invalid element(s).");
    public static CompositeFormat CollectionMustContainNull { get; } = CompositeFormat.Parse("The collection must contain null element(s).");

    public static CompositeFormat MustBeAfter { get; } = CompositeFormat.Parse("Value must be after {0}. Found: {1}.");
    public static CompositeFormat MustBeBefore { get; } = CompositeFormat.Parse("Value must be before {0}. Found: {1}.");
    public static CompositeFormat MustBeEqualTo { get; } = CompositeFormat.Parse("Value must be equal to {0}. Found: {1}.");
    public static CompositeFormat MustBeGraterThan { get; } = CompositeFormat.Parse("Value must be greater than {0}. Found: {1}.");
    public static CompositeFormat MustBeIn { get; } = CompositeFormat.Parse("Value must be one of these: '{0}'. Found: {1}.");
    public static CompositeFormat MustBeLessThan { get; } = CompositeFormat.Parse("Value must be less than {0}. Found: {1}.");
    public static CompositeFormat MustContain { get; } = CompositeFormat.Parse("Value must contain '{0}'.");
    public static CompositeFormat MustContainValue { get; } = CompositeFormat.Parse("Value must contain the value '{0}'.");
    public static CompositeFormat MustContainKey { get; } = CompositeFormat.Parse("Value must contain the key '{0}'.");
    public static CompositeFormat MustHaveACountOf { get; } = CompositeFormat.Parse("Value count must be {0}. Found: {1}.");
    public static CompositeFormat MustHaveALengthOf { get; } = CompositeFormat.Parse("Value length must be {0}. Found: {1}.");
    public static CompositeFormat MustHaveAMaximumCountOf { get; } = CompositeFormat.Parse("Value maximum count must be {0}. Found: {1}.");
    public static CompositeFormat MustHaveAMaximumLengthOf { get; } = CompositeFormat.Parse("Value maximum length must be {0}. Found: {1}.");
    public static CompositeFormat MustHaveAMinimumCountOf { get; } = CompositeFormat.Parse("Value minimum count must be {0}. Found: {1}.");
    public static CompositeFormat MustHaveAMinimumLengthOf { get; } = CompositeFormat.Parse("Value minimum length must be {0}. Found: {1}.");
    public static CompositeFormat MustBeOfType { get; } = CompositeFormat.Parse("Value must be of type '{0}'. Found: '{1}'.");

    public static string InvertMessage(string message)
        => message switch {
            _ when message.Contains(" cannot ") => message.Replace(" cannot ", " must "),
            _ when message.Contains(" must ") => message.Replace(" must ", " cannot "),
            _ when message.Contains(" is not ") => message.Replace(" is not ", " is "),
            _ when message.Contains(" is ") => message.Replace(" is ", " is not "),
            _ when message.Contains(" does not contain ") => message.Replace(" does not contain ", " contains "),
            _ when message.Contains(" contains ") => message.Replace(" contains ", " does not contain "),
            _ => message,
        };
    public static CompositeFormat InvertMessage(CompositeFormat message)
        => message switch {
            _ when message.Format.Contains(" cannot ") => CompositeFormat.Parse(message.Format.Replace(" cannot ", " must ")),
            _ when message.Format.Contains(" must ") => CompositeFormat.Parse(message.Format.Replace(" must ", " cannot ")),
            _ when message.Format.Contains(" is not ") => CompositeFormat.Parse(message.Format.Replace(" is not ", " is ")),
            _ when message.Format.Contains(" is ") => CompositeFormat.Parse(message.Format.Replace(" is ", " is not ")),
            _ when message.Format.Contains(" does not contain ") => CompositeFormat.Parse(message.Format.Replace(" does not contain ", " contains ")),
            _ when message.Format.Contains(" contains ") => CompositeFormat.Parse(message.Format.Replace(" contains ", " does not contain ")),
            _ => message,
        };
}
