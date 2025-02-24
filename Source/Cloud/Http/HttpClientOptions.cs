using Result = DotNetToolbox.Results.Result;

namespace DotNetToolbox.Http;

public record HttpClientOptions
    : INamedOptions<HttpClientOptions>, IValidatable {
    public const string DefaultResponseFormat = "application/json";
    public const string DefaultContentType = "application/json";

    public static string SectionName => "HttpClient";
    public static HttpClientOptions Default => new();

    public string BaseAddress { get; init; } = string.Empty;
    public Dictionary<string, string> Endpoints { get; init; } = [];
    public string ContentType { get; init; } = DefaultContentType;
    public string ResponseFormat { get; init; } = DefaultResponseFormat;
    public HttpClientAuthentication? Authentication { get; set; }
    public Dictionary<string, string[]>? CustomHeaders { get; init; }

    public virtual IResult Validate(IMap? context = null) {
        var result = Result.Default;
        var provider = (string?)context?["Provider"] ?? string.Empty;
        var providerPath = string.IsNullOrWhiteSpace(provider) ? string.Empty : $":{provider}";

        result.Add(ValidateBaseAddress(providerPath));
        result.Add(ValidateResponseFormat(providerPath));
        result.Add(ValidateAuthentication(providerPath));
        return result;
    }

    private Result ValidateBaseAddress(string providerPath) {
        var result = ValidationResult.Default;
        var baseAddress = GetConfigurationPath(providerPath, nameof(BaseAddress));
        if (string.IsNullOrWhiteSpace(BaseAddress))
            result.Add(Failure("Http client base address is missing.", baseAddress));
        else if (!Uri.IsWellFormedUriString(BaseAddress, UriKind.Absolute))
            result.Add(Failure("Http client base address is not a valid URI.", baseAddress));
        return result;
    }

    private Result ValidateResponseFormat(string providerPath) {
        var result = ValidationResult.Default;
        var responseFormat = GetConfigurationPath(providerPath, nameof(ResponseFormat));
        if (string.IsNullOrWhiteSpace(ResponseFormat) && !MediaTypeWithQualityHeaderValue.TryParse(ResponseFormat, out _))
            result.Add(Failure("Http client response format value is not valid.", responseFormat));
        return result;
    }

    private Result ValidateAuthentication(string providerPath) {
        var result = ValidationResult.Default;
        var authentication = GetConfigurationPath(providerPath, $"{nameof(Authentication)}:{nameof(HttpClientAuthentication.Value)}");
        if (Authentication is null || Authentication.Type is None)
            return result;
        if (string.IsNullOrWhiteSpace(Authentication.Value))
            result.Add(Failure("The http client authentication value is missing.", authentication));
        return result;
    }

    private static string GetConfigurationPath(string providerPath, string key)
        => $"Configuration[{SectionName}{providerPath}:{key}]";

    public void Configure(HttpClient client) {
        client.BaseAddress = new(BaseAddress);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new(DefaultIfNullOrWhiteSpace(ResponseFormat, DefaultResponseFormat)));
        Authentication?.SetHttpClientAuthentication(client);
        CustomHeaders?.ForEach(client.DefaultRequestHeaders.Add);
    }
}
