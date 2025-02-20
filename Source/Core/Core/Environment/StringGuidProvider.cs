namespace DotNetToolbox.Environment;

[ExcludeFromCodeCoverage(Justification = "Thin wrapper for OS functionality.")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global - Used for externally.
public class StringGuidProvider(IGuidProvider guid)
    : HasDefault<StringGuidProvider>,
      IStringGuidProvider {
    public StringGuidProvider()
        : this(GuidProvider.Default) {
    }

    public virtual string Create() => guid.Create().ToString();
    public virtual string Create(byte[] bytes) => guid.Create(bytes).ToString();
    public virtual string Create(ReadOnlySpan<byte> bytes) => guid.Create(bytes).ToString();
    public virtual string CreateSortable() => guid.CreateSortable().ToString();
    public virtual string CreateSortable(byte[] bytes) => guid.CreateSortable(bytes).ToString();
    public virtual string CreateSortable(ReadOnlySpan<byte> bytes) => guid.CreateSortable(bytes).ToString();

    public virtual string EnsureIsValid(string input) => guid.Parse(input).ToString();
    public virtual string EnsureIsValid(ReadOnlySpan<char> input) => guid.Parse(input).ToString();
    public virtual string EnsureIsValid(string s, IFormatProvider? provider) => guid.Parse(s, provider).ToString();
    public virtual string EnsureIsValid(ReadOnlySpan<char> s, IFormatProvider? provider) => guid.Parse(s, provider).ToString();

    public virtual string EnsureIsValid(string input, [Syntax(Syntax.GuidFormat)] string format)
        => guid.ParseExact(input, format).ToString();
    public virtual string EnsureIsValid(ReadOnlySpan<char> input, [Syntax(Syntax.GuidFormat)] ReadOnlySpan<char> format)
        => guid.ParseExact(input, format).ToString();

    public virtual bool IsValid([NotNullWhen(true)] string? input)
        => guid.TryParse(input, out _);
    public virtual bool IsValid(ReadOnlySpan<char> input)
        => guid.TryParse(input, out _);
    public virtual bool IsValid([NotNullWhen(true)] string? s, IFormatProvider? provider)
        => guid.TryParse(s, provider, out _);
    public virtual bool IsValid(ReadOnlySpan<char> s, IFormatProvider? provider)
        => guid.TryParse(s, provider, out _);

    public virtual bool IsValid([NotNullWhen(true)] string? input, [NotNullWhen(true), Syntax(Syntax.GuidFormat)] string? format)
        => guid.TryParseExact(input, format, out _);
    public virtual bool IsValid(ReadOnlySpan<char> input, [Syntax(Syntax.GuidFormat)] ReadOnlySpan<char> format)
        => guid.TryParseExact(input, format, out _);
}
