namespace DotNetToolbox.Environment;

[ExcludeFromCodeCoverage(Justification = "Thin wrapper for OS functionality.")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global - Used for externally.
public class GuidProvider
    : HasDefault<GuidProvider>,
      IGuidProvider {
    public virtual Guid Create() => Guid.NewGuid();
    public virtual Guid Create(byte[] bytes) => new(bytes);
    public virtual Guid Create(ReadOnlySpan<byte> bytes) => new(bytes);
    public virtual Guid CreateSortable() => Ulid.NewUlid().ToGuid();
    public virtual Guid CreateSortable(byte[] bytes) => new Ulid(bytes).ToGuid();
    public virtual Guid CreateSortable(ReadOnlySpan<byte> bytes) => new Ulid(bytes).ToGuid();

    public virtual Guid Parse(string input) => Guid.Parse(input);
    public virtual Guid Parse(ReadOnlySpan<char> input) => Guid.Parse(input);
    public virtual Guid Parse(string s, IFormatProvider? provider) => Guid.Parse(s, provider);
    public virtual Guid Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Guid.Parse(s, provider);

    public virtual Guid ParseExact(string input, [Syntax(Syntax.GuidFormat)] string format)
        => Guid.ParseExact(input, format);
    public virtual Guid ParseExact(ReadOnlySpan<char> input, [Syntax(Syntax.GuidFormat)] ReadOnlySpan<char> format)
        => Guid.ParseExact(input, format);

    public virtual bool TryParse([NotNullWhen(true)] string? input, out Guid result)
        => Guid.TryParse(input, out result);
    public virtual bool TryParse(ReadOnlySpan<char> input, out Guid result)
        => Guid.TryParse(input, out result);
    public virtual bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Guid result)
        => Guid.TryParse(s, provider, out result);
    public virtual bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Guid result)
        => Guid.TryParse(s, provider, out result);

    public virtual bool TryParseExact([NotNullWhen(true)] string? input, [NotNullWhen(true), Syntax(Syntax.GuidFormat)] string? format, out Guid result)
        => Guid.TryParseExact(input, format, out result);
    public virtual bool TryParseExact(ReadOnlySpan<char> input, [Syntax(Syntax.GuidFormat)] ReadOnlySpan<char> format, out Guid result)
        => Guid.TryParseExact(input, format, out result);
}
