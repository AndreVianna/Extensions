﻿namespace DotNetToolbox.Http.Options;

public class JwtAuthenticationOptions : AuthenticationOptions {

    public string? PrivateKey { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public IReadOnlyList<Claim> Claims { get; set; } = Array.Empty<Claim>();
    public TimeSpan? ExpiresAfter { get; set; }

    internal override ValidationResult Validate() {
        var result = base.Validate();

        if (string.IsNullOrWhiteSpace(PrivateKey))
            result += new ValidationError(CannotBeNullOrWhiteSpace, nameof(PrivateKey));

        return result;
    }

    internal override void Configure(HttpClient client, ref HttpAuthentication authentication) {
        if (!authentication.IsValid(Jwt)) authentication = CreateJwtToken();
        client.DefaultRequestHeaders.Authorization = authentication;
    }

    internal DateTimeProvider DateTimeProvider { get; set; } = new();

    private HttpAuthentication CreateJwtToken() {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(PrivateKey!));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var expiration = DateTimeProvider.UtcNow + ExpiresAfter;
        var tokenOptions = new JwtSecurityToken(Issuer,
                                                Audience,
                                                Claims,
                                                null,
                                                expiration,
                                                signingCredentials);

        return new() {
            DateTimeProvider = DateTimeProvider,
            Type = Jwt,
            Value = new JwtSecurityTokenHandler().WriteToken(tokenOptions),
            Scheme = Bearer,
            ExpiresOn = expiration,
        };
    }
}