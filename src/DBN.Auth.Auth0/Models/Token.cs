namespace DBN.Auth.Auth0.Models;

public enum TokenGrantType
{
    [EnumMember(Value = "client_credentials")]
    [JsonPropertyName("client_credentials")]
    ClientCredentials,
    [EnumMember(Value = "password")]
    [JsonPropertyName("password")]
    Password,
    [EnumMember(Value = "authorization_code")]
    [JsonPropertyName("authorization_code")]
    AuthorizationCode,
    [EnumMember(Value = "refresh_token")]
    [JsonPropertyName("refresh_token")]
    RefreshToken
}

public class Token
{
    [DisplayName("access_token")]
    [JsonPropertyName("access_token")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AccessToken { get; set; }

    [DisplayName("refresh_token")]
    [JsonPropertyName("refresh_token")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RefreshToken { get; set; }

    [DisplayName("id_token")]
    [JsonPropertyName("id_token")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? IdToken { get; set; }

    [DisplayName("token_type")]
    [JsonPropertyName("token_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TokenType { get; set; }

    [DisplayName("scope")]
    [JsonPropertyName("scope")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Scope { get; set; }

    [DisplayName("expires_in")]
    [JsonPropertyName("expires_in")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ExpiresIn { get; set; }
}