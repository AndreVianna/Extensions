namespace DotNetToolbox.Data.Strategies;

public record RepositoryStrategyEntry(Type StrategyType, Func<string, IRepositoryStrategy> Create);