public interface IAuthService {
    Task<TokenResponse?> GetToken(string domain, Dictionary<string, string> values);
    Task<TokenResponse?> GetTokenForClientCredentials(string domain, string audience, NetworkCredential client);
    Task<TokenResponse?> GetTokenForPassword(string domain, string audience, NetworkCredential client, NetworkCredential user, string scope);
    Task<TokenResponse?> GetTokenForAuthorizationCode(string domain, NetworkCredential client, string code, string redirectUri);
    Task<TokenResponse?> GetTokenForRefreshToken(string domain, NetworkCredential client, string refreshToken);
}
