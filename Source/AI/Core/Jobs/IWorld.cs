﻿
namespace DotNetToolbox.AI.Jobs;

public interface IWorld {
    DateTimeOffset DateTime { get; }

    Result Validate(IContext? context = null);
}
