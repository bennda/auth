public interface ITokenService
{
    Task<TokenResponse?> GetToken(TokenParameters parameters);
}

public class TokenParameters
{
    public static TokenParameters ForClientCredentials(string domain, string audience, NetworkCredential client)
    {
        return new TokenParameters(domain, TokenGrantType.ClientCredentials)
        {
            Audience = audience,
            Client = client
        };
    }
    
    public static TokenParameters ForPassword(string domain, string audience, NetworkCredential client, NetworkCredential user, string scope)
    {
        return new TokenParameters(domain, TokenGrantType.Password)
        {
            Audience = audience,
            Client = client,
            User = user,
            Scope = scope
        };
    }
    
    public static TokenParameters ForAuthorizationCode(string domain, NetworkCredential client, string code, string redirectUri)
    {
        return new TokenParameters(domain, TokenGrantType.AuthorizationCode)
        {
            Client = client,
            Code = code,
            RedirectUri = redirectUri
        };
    }
    
    public static TokenParameters ForRefreshToken(string domain, NetworkCredential client, string refreshToken)
    {
        return new TokenParameters(domain, TokenGrantType.RefreshToken)
        {
            Client = client,
            RefreshToken = refreshToken
        };
    }

    private TokenParameters(string domain, TokenGrantType grantType)
    {
        Domain = domain;
        GrantType = grantType;
    }

    public string Domain { get; }
    public TokenGrantType GrantType { get; }
    public string? Audience { get; set; }
    public string? Scope { get; set; }
    public string? Code { get; set; }
    public string? RedirectUri { get; set; }
    public string? RefreshToken { get; set; }
    public NetworkCredential? Client { get; set; }
    public NetworkCredential? User { get; set; }
}