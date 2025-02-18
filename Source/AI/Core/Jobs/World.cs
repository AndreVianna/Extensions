using DotNetToolbox.Validation;

namespace DotNetToolbox.AI.Jobs;

public class World(IDateTimeProvider? dateTime = null)
    : Map,
      IValidatable,
      IWorld {
    private readonly IDateTimeProvider _dateTime = dateTime ?? DateTimeProvider.Default;

    public DateTimeOffset DateTime => _dateTime.Now;

    public Result Validate(IMap? context = null)
        => Result.Success();
}
