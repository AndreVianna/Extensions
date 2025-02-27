using static DotNetToolbox.Results.Result;

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

    public virtual Result Validate(IMap? context = null) {
        var result = Success();
        var provider = (string?)context?["Provider"] ?? string.Empty;
        var providerPath = string.IsNullOrWhiteSpace(provider) ? string.Empty : $":{provider}";

        result += ValidateBaseAddress(providerPath);
        result += ValidateResponseFormat(providerPath);
        result += ValidateAuthentication(providerPath);
        return result;
    }

    private Result ValidateBaseAddress(string providerPath) {
        var result = Result.Default;
        if (string.IsNullOrWhiteSpace(BaseAddress))
            result += new Error("Http client base address is missing.", GetConfigurationPath(providerPath, nameof(BaseAddress)));
        else if (!Uri.IsWellFormedUriString(BaseAddress, UriKind.Absolute))
            result += new Error("Http client base address is not a valid URI.", GetConfigurationPath(providerPath, nameof(BaseAddress)));
        return result;
    }

    private Result ValidateResponseFormat(string providerPath) {
        var result = Result.Default;
        if (string.IsNullOrWhiteSpace(ResponseFormat) && !MediaTypeWithQualityHeaderValue.TryParse(ResponseFormat, out _))
            result += new Error("Http client response format value is not valid.", GetConfigurationPath(providerPath, nameof(ResponseFormat)));
        return result;
    }

    private Result ValidateAuthentication(string providerPath) {
        var result = Result.Default;
        if (Authentication is null || Authentication.Type is None)
            return result;
        if (string.IsNullOrWhiteSpace(Authentication.Value))
            result += new Error("The http client authentication value is missing.", GetConfigurationPath(providerPath, $"{nameof(Authentication)}:{nameof(HttpClientAuthentication.Value)}"));
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
