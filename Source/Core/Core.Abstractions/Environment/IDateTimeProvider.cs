namespace DotNetToolbox.Environment;

public interface IDateTimeProvider {
    DateTimeOffset Maximum { get; }
    DateTimeOffset Minimum { get; }
    DateTimeOffset Now { get; }
    TimeOnly TimeOfDay { get; }
    DateOnly Today { get; }
    DateTimeOffset UnixEpoch { get; }
    DateTimeOffset UtcNow { get; }
    TimeOnly UtcTimeOfDay { get; }
    DateOnly UtcToday { get; }

    DateTimeOffset FromFileTime(long fileTime);
    DateTimeOffset FromUnixTimeMilliseconds(long ms);
    DateTimeOffset FromUnixTimeSeconds(long s);
    DateTimeOffset Parse(string s);
    DateTimeOffset ParseExact(string s, [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format);
    bool TryParse(string s, out DateTimeOffset result);
    bool TryParseExact(string s, [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format, out DateTimeOffset result);
}
