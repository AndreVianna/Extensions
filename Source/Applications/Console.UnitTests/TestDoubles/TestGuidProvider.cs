using System.Diagnostics.CodeAnalysis;

namespace DotNetToolbox.ConsoleApplication.TestDoubles;

internal sealed class TestGuidProvider
    : IGuidProvider {
    public Guid Create() => throw new NotImplementedException();

    public Guid CreateFrom(byte[] bytes) => throw new NotImplementedException();

    public Guid CreateFrom(ReadOnlySpan<byte> bytes) => throw new NotImplementedException();

    public Guid CreateSortable() => throw new NotImplementedException();

    public Guid Parse(ReadOnlySpan<char> input) => throw new NotImplementedException();

    public Guid Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => throw new NotImplementedException();

    public Guid Parse(string input) => throw new NotImplementedException();

    public Guid Parse(string s, IFormatProvider? provider) => throw new NotImplementedException();

    public Guid ParseExact(ReadOnlySpan<char> input, ReadOnlySpan<char> format) => throw new NotImplementedException();

    public Guid ParseExact(string input, string format) => throw new NotImplementedException();

    public bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Guid result) => throw new NotImplementedException();

    public bool TryParse(ReadOnlySpan<char> input, out Guid result) => throw new NotImplementedException();

    public bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Guid result) => throw new NotImplementedException();

    public bool TryParse([NotNullWhen(true)] string? input, out Guid result) => throw new NotImplementedException();

    public bool TryParseExact(ReadOnlySpan<char> input, ReadOnlySpan<char> format, out Guid result) => throw new NotImplementedException();

    public bool TryParseExact([NotNullWhen(true)] string? input, [NotNullWhen(true)] string? format, out Guid result) => throw new NotImplementedException();
}
