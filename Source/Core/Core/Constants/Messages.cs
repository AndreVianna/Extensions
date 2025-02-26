namespace DotNetToolbox.Constants;

public static class Messages {
    public static string Null => "null";
    public static CompositeFormat Quotes { get; } = CompositeFormat.Parse("'{0}'");

    public static CompositeFormat TheLengthOf { get; } = CompositeFormat.Parse("The length of '{0}'");
    public static CompositeFormat TheElementCountOf { get; } = CompositeFormat.Parse("The element count of '{0}'");

    public static CompositeFormat IsInvalid { get; } = CompositeFormat.Parse("The value of {0} is invalid.");
    public static CompositeFormat ContainsInvalid { get; } = CompositeFormat.Parse("{0} contains invalid values.");

    public static CompositeFormat MustBeNull { get; } = CompositeFormat.Parse("{0} must be null.");
    public static CompositeFormat MustBeEmpty { get; } = CompositeFormat.Parse("{0} must be empty.");
    public static CompositeFormat MustBeValid { get; } = CompositeFormat.Parse("{0} must be valid.");
    public static CompositeFormat MustBeAnEmail { get; } = CompositeFormat.Parse("{0} must be a valid email.");
    public static CompositeFormat MustBeAPassword { get; } = CompositeFormat.Parse("{0} must be a valid password.");

    public static CompositeFormat MustBeAfter { get; } = CompositeFormat.Parse("{0} must be after {1}.");
    public static CompositeFormat MustBeBefore { get; } = CompositeFormat.Parse("{0} must be before {1}.");
    public static CompositeFormat MustBeEqualTo { get; } = CompositeFormat.Parse("{0} must be equal to {0}.");
    public static CompositeFormat MustBeGraterThan { get; } = CompositeFormat.Parse("{0} must be greater than {1}.");
    public static CompositeFormat MustBeGraterThanOrEqualTo { get; } = CompositeFormat.Parse("{0} must be greater than or equal to {1}.");
    public static CompositeFormat MustBeLessThan { get; } = CompositeFormat.Parse("{0} must be less than {1}.");
    public static CompositeFormat MustBeLessThanOrEqualTo { get; } = CompositeFormat.Parse("{0} must be less than or equal to {1}.");
    public static CompositeFormat MustBeIn { get; } = CompositeFormat.Parse("{0} must be in {1}.");
    public static CompositeFormat MustContain { get; } = CompositeFormat.Parse("{0} must contain {1}.");
    public static CompositeFormat MustContainKey { get; } = CompositeFormat.Parse("{0} must contain {1} as a key.");
    public static CompositeFormat MustBeOfType { get; } = CompositeFormat.Parse("{0} must be of type '{1}'.");

    public static CompositeFormat CannotBeNull { get; } = CompositeFormat.Parse("{0} cannot be null.");
    public static CompositeFormat CannotBeEmpty { get; } = CompositeFormat.Parse("{0} cannot be empty.");
    public static CompositeFormat CannotBeNullOrEmpty { get; } = CompositeFormat.Parse("{0} cannot be null or empty.");
    public static CompositeFormat CannotBeWhiteSpace { get; } = CompositeFormat.Parse("{0} cannot be white spaces only.");
    public static CompositeFormat CannotBeNullOrWhiteSpace { get; } = CompositeFormat.Parse("{0} cannot be null or white spaces only.");
    public static CompositeFormat CannotBeEmptyOrWhiteSpace { get; } = CompositeFormat.Parse("{0} cannot be empty or white spaces only.");

    public static CompositeFormat CannotContainInvalid { get; } = CompositeFormat.Parse("{0} cannot contain any invalid element.");
    public static CompositeFormat CannotContainNulls { get; } = CompositeFormat.Parse("{0} cannot contain any null element.");

    public static CompositeFormat CannotBeAfter { get; } = CompositeFormat.Parse("{0} cannot be after {1}.");
    public static CompositeFormat CannotBeBefore { get; } = CompositeFormat.Parse("{0} cannot be before {1}.");
    public static CompositeFormat CannotBeEqualTo { get; } = CompositeFormat.Parse("{0} cannot be equal to {0}.");
    public static CompositeFormat CannotBeGraterThan { get; } = CompositeFormat.Parse("{0} cannot be greater than {1}.");
    public static CompositeFormat CannotBeGraterThanOrEqualTo { get; } = CompositeFormat.Parse("{0} cannot be greater than or equal to {1}.");
    public static CompositeFormat CannotBeLessThan { get; } = CompositeFormat.Parse("{0} cannot be less than {1}.");
    public static CompositeFormat CannotBeLessThanOrEqualTo { get; } = CompositeFormat.Parse("{0} cannot be less than or equal to {1}.");
    public static CompositeFormat CannotBeIn { get; } = CompositeFormat.Parse("{0} cannot be in {1}.");
    public static CompositeFormat CannotContain { get; } = CompositeFormat.Parse("{0} cannot contain {1}.");
    public static CompositeFormat CannotContainKey { get; } = CompositeFormat.Parse("{0} cannot contain {1} as a key.");
    public static CompositeFormat CannotBeOfType { get; } = CompositeFormat.Parse("{0} cannot be of type '{1}'.");

    public static string Quoted(string name) => string.Format(null, Quotes, name);

    public static string BuildMessage(CompositeFormat message, params IEnumerable<string> arguments)
        => string.Format(null, message, arguments);

    public static string BuildMessage(string target, CompositeFormat message, params IEnumerable<string> arguments)
        => string.Format(null, message, [Quoted(target), .. arguments]);
}
