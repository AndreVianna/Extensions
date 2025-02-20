namespace DotNetToolbox.AI.Agents;

public class AgentSettings
    : IAgentSettings {
    private const byte _maximumRetries = 10;
    private const decimal _maximumTemperature = 2;
    private const decimal _maximumTokenProbabilityCutOff = 1;

    public const uint MinimumOutputTokens = 256;
    private const uint _defaultMaximumOutputTokens = MinimumOutputTokens;
    private const byte _defaultNumberOfRetries = 0;
    private const decimal _defaultTemperature = 1;
    private const decimal _defaultProbabilityCutOff = 0;

    public byte MaximumNumberOfRetries { get; set; } = _defaultNumberOfRetries;
    public uint MaximumOutputTokens { get; set; } = _defaultMaximumOutputTokens;
    public decimal Temperature { get; set; } = _defaultTemperature;
    public decimal TokenProbabilityCutOff { get; set; } = _defaultProbabilityCutOff;
    public List<string> StopSequences { get; set; } = [];
    public bool ResponseIsStream { get; set; }
    public bool ResponseIsJson { get; set; }

    public Result Validate(IMap? context = null) {
        var result = Result.Success();
        if (StopSequences.Count > 0 && StopSequences.Any(string.IsNullOrWhiteSpace))
            result += new ResultError("StopWaiting signals cannot be null, empty, or contain only whitespace.", nameof(StopSequences));
        if (MaximumNumberOfRetries > _maximumRetries)
            result += new ResultError($"The maximum number of retries is {_maximumRetries}. Found: {MaximumNumberOfRetries}", nameof(MaximumNumberOfRetries));
        if (Temperature is < 0 or > _maximumTemperature)
            result += new ResultError($"Content must be between {0} and {_maximumTemperature}. Found: {Temperature}", nameof(Temperature));
        if (TokenProbabilityCutOff is < 0 or > _maximumTokenProbabilityCutOff)
            result += new ResultError($"Content must be between {0} and {_maximumTokenProbabilityCutOff}. Found: {TokenProbabilityCutOff}", nameof(TokenProbabilityCutOff));

        return result;
    }
}
