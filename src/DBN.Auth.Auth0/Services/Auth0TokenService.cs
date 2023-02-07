namespace DBN.Auth.Auth0.Services;

public class Auth0TokenService : ITokenService
{
    private readonly HttpClient _httpClient;
    public Auth0TokenService(HttpClient client)
    {
        _httpClient = client;
    }

    public async Task<TokenResponse?> GetToken(TokenParameters parameters)
    {
        switch (parameters.GrantType)
        {
            case TokenGrantType.ClientCredentials:
                return await GetTokenForClientCredentials(parameters.Domain, $"{parameters.Audience}", parameters.Client);
            case TokenGrantType.Password:
                return await GetTokenForPassword(parameters.Domain, $"{parameters.Audience}", parameters.Client, parameters.User, $"{parameters.Scope}");
            case TokenGrantType.AuthorizationCode:
                return await GetTokenForAuthorizationCode(parameters.Domain, parameters.Client, $"{parameters.Code}", $"{parameters.RedirectUri}");
            case TokenGrantType.RefreshToken:
                return await GetTokenForRefreshToken(parameters.Domain, parameters.Client, $"{parameters.RefreshToken}");
            default:
                return null;
        }
    }

    private async Task<TokenResponse?> GetToken(string domain, Dictionary<string, string> values)
    {
        var url = $"https://{domain}/oauth/token";
        var httpResponseMessage = await _httpClient.PostAsJsonAsync(url, values);

        Token? token = null;
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            token = await httpResponseMessage.Content.ReadFromJsonAsync<Token>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        return new TokenResponse
        {
            IsSuccessful = httpResponseMessage.IsSuccessStatusCode,
            StatusCode = httpResponseMessage.StatusCode,
            Token = token
        };
    }

    private async Task<TokenResponse?> GetTokenForClientCredentials(string domain, string audience, NetworkCredential? client)
    {
        return await GetToken(domain, new Dictionary<string, string> {
            {"grant_type", $"client_credentials" },
            {"audience", $"{audience}" },
            {"client_id", $"{client?.UserName}" },
            {"client_secret", $"{client?.Password}" }
        });
    }

    private async Task<TokenResponse?> GetTokenForPassword(string domain, string audience, NetworkCredential? client, NetworkCredential? user, string scope)
    {
        return await GetToken(domain, new Dictionary<string, string> {
            {"grant_type", $"password" },
            {"audience", $"{audience}" },
            {"client_id", $"{client?.UserName}" },
            {"client_secret", $"{client?.Password}" },
            {"username", $"{user?.UserName}" },
            {"password", $"{user?.Password}" },
            {"scope", $"{scope}" }
        });
    }

    private async Task<TokenResponse?> GetTokenForAuthorizationCode(string domain, NetworkCredential? client, string code, string redirectUri)
    {
        return await GetToken(domain, new Dictionary<string, string> {
            {"grant_type", $"authorization_code" },
            {"client_id", $"{client?.UserName}" },
            {"client_secret", $"{client?.Password}" },
            {"code", $"{code}" },
            {"redirect_uri", $"{redirectUri}" }
        });
    }

    private async Task<TokenResponse?> GetTokenForRefreshToken(string domain, NetworkCredential? client, string refreshToken)
    {
        return await GetToken(domain, new Dictionary<string, string> {
            {"grant_type", $"refresh_token" },
            {"client_id", $"{client?.UserName}" },
            {"client_secret", $"{client?.Password}" },
            {"refresh_token", $"{refreshToken}" }
        });
    }

    private async Task<TokenResponse?> GetManagementToken(string domain, NetworkCredential? client)
    {
        return await GetTokenForClientCredentials(domain, $"https://{domain}/api/v2/", client);
    }
}
