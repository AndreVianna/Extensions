using static System.Diagnostics.CodeAnalysis.StringSyntaxAttribute;

namespace DotNetToolbox.Environment;

public interface IStringGuidProvider {
    string Create();
    string Create(byte[] bytes);
    string Create(ReadOnlySpan<byte> bytes);
    string CreateSortable();
    string CreateSortable(byte[] bytes);
    string CreateSortable(ReadOnlySpan<byte> bytes);

    string EnsureIsValid(ReadOnlySpan<char> input);
    string EnsureIsValid(ReadOnlySpan<char> s, IFormatProvider? provider);
    string EnsureIsValid(string input);
    string EnsureIsValid(string s, IFormatProvider? provider);
    string EnsureIsValid(ReadOnlySpan<char> input, [StringSyntax(GuidFormat)] ReadOnlySpan<char> format);
    string EnsureIsValid(string input, [StringSyntax(GuidFormat)] string format);
    bool IsValid(ReadOnlySpan<char> s, IFormatProvider? provider);
    bool IsValid(ReadOnlySpan<char> input);
    bool IsValid([NotNullWhen(true)] string? s, IFormatProvider? provider);
    bool IsValid([NotNullWhen(true)] string? input);
    bool IsValid(ReadOnlySpan<char> input, [StringSyntax(GuidFormat)] ReadOnlySpan<char> format);
    bool IsValid([NotNullWhen(true)] string? input, [NotNullWhen(true), StringSyntax(GuidFormat)] string? format);
}
