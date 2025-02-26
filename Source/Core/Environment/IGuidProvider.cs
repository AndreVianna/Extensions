namespace DotNetToolbox.Environment;

public interface IGuidProvider {
    Guid Create();
    Guid CreateFrom(byte[] bytes);
    Guid CreateFrom(ReadOnlySpan<byte> bytes);
    Guid CreateSortable();

    Guid Parse(ReadOnlySpan<char> input);
    Guid Parse(ReadOnlySpan<char> s, IFormatProvider? provider);
    Guid Parse(string input);
    Guid Parse(string s, IFormatProvider? provider);
    Guid ParseExact(ReadOnlySpan<char> input, [Syntax("GuidFormat")] ReadOnlySpan<char> format);
    Guid ParseExact(string input, [Syntax("GuidFormat")] string format);
    bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Guid result);
    bool TryParse(ReadOnlySpan<char> input, out Guid result);
    bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Guid result);
    bool TryParse([NotNullWhen(true)] string? input, out Guid result);
    bool TryParseExact(ReadOnlySpan<char> input, [Syntax("GuidFormat")] ReadOnlySpan<char> format, out Guid result);
    bool TryParseExact([NotNullWhen(true)] string? input, [NotNullWhen(true), Syntax("GuidFormat")] string? format, out Guid result);
}
