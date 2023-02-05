public class Auth0Service : IAuthService
{ 
    private readonly HttpClient _httpClient;
    public Auth0Service(HttpClient client) {
        _httpClient = client;
    }

    public async Task<TokenResponse?> GetToken(string domain, Dictionary<string, string> values) {
        var url = $"https://{domain}/oauth/token";
        var httpResponseMessage = await _httpClient.PostAsJsonAsync(url, values);

        Token? token = null;
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            token = await httpResponseMessage.Content.ReadFromJsonAsync<Token>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                
            });
        }

        return new TokenResponse {
            IsSuccessful = httpResponseMessage.IsSuccessStatusCode,
            StatusCode = httpResponseMessage.StatusCode,
            Token = token
        };
    }

    public async Task<TokenResponse?> GetTokenForClientCredentials(string domain, string audience, NetworkCredential client)
    {
        return await GetToken(domain, new Dictionary<string, string> {
            {"grant_type", $"client_credentials" },
            {"audience", $"{audience}" },
            {"client_id", $"{client.UserName}" },
            {"client_secret", $"{client.Password}" }
        });
    }

    public async Task<TokenResponse?> GetTokenForPassword(string domain, string audience, NetworkCredential client, NetworkCredential user, string scope)
    {
        return await GetToken(domain, new Dictionary<string, string> {
            {"grant_type", $"password" },
            {"audience", $"{audience}" },
            {"client_id", $"{client.UserName}" },
            {"client_secret", $"{client.Password}" },
            {"username", $"{user.UserName}" },
            {"password", $"{user.Password}" },
            {"scope", $"{scope}" }
        });
    }

    public async Task<TokenResponse?> GetTokenForAuthorizationCode(string domain, NetworkCredential client, string code, string redirectUri)
    {
        return await GetToken(domain, new Dictionary<string, string> {
            {"grant_type", $"authorization_code" },
            {"client_id", $"{client.UserName}" },
            {"client_secret", $"{client.Password}" },
            {"code", $"{code}" },
            {"redirect_uri", $"{redirectUri}" }
        });
    }

    public async Task<TokenResponse?> GetTokenForRefreshToken(string domain, NetworkCredential client, string refreshToken)
    {
        return await GetToken(domain, new Dictionary<string, string> {
            {"grant_type", $"refresh_token" },
            {"client_id", $"{client.UserName}" },
            {"client_secret", $"{client.Password}" },
            {"refresh_token", $"{refreshToken}" }
        });
    }
}
