namespace DotNetToolbox.Environment;

[ExcludeFromCodeCoverage(Justification = "Thin wrapper for OS functionality.")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global - Used for externally.
public class DateTimeProvider
    : IHasDefault<DateTimeProvider>
    , IDateTimeProvider {
    public static DateTimeProvider Default { get; } = new();

    public virtual DateTimeOffset Now => DateTimeOffset.Now;
    public virtual DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
    public virtual TimeOnly TimeOfDay => TimeOnly.FromDateTime(DateTime.Now);

    public virtual DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    public virtual DateOnly UtcToday => DateOnly.FromDateTime(UtcNow.DateTime);
    public virtual TimeOnly UtcTimeOfDay => TimeOnly.FromDateTime(UtcNow.DateTime);

    public virtual DateTimeOffset Minimum => DateTimeOffset.MinValue;
    public virtual DateTimeOffset Maximum => DateTimeOffset.MaxValue;
    public virtual DateTimeOffset UnixEpoch => DateTimeOffset.UnixEpoch;

    public virtual DateTimeOffset FromFileTime(long fileTime) => DateTimeOffset.FromFileTime(fileTime);
    public virtual DateTimeOffset FromUnixTimeMilliseconds(long ms) => DateTimeOffset.FromUnixTimeMilliseconds(ms);
    public virtual DateTimeOffset FromUnixTimeSeconds(long s) => DateTimeOffset.FromUnixTimeSeconds(s);

    public virtual DateTimeOffset Parse(string s) => DateTimeOffset.Parse(s);
    public virtual bool TryParse(string s, out DateTimeOffset result)
        => DateTimeOffset.TryParse(s, out result);

    public virtual DateTimeOffset ParseExact(string s, [Syntax(Syntax.DateTimeFormat)] string format)
        => DateTimeOffset.ParseExact(s, format, null, DateTimeStyles.None);
    public virtual bool TryParseExact(
        string s,
        [Syntax(Syntax.DateTimeFormat)] string format,
        out DateTimeOffset result)
        => DateTimeOffset.TryParseExact(s, format, null, DateTimeStyles.None, out result);
}
