namespace DBN.Auth.Auth0.Services;

public class Auth0TokenService : ITokenService
{
    private readonly HttpClient _httpClient;
    public Auth0TokenService(HttpClient client)
    {
        _httpClient = client;
    }

    public async Task<TokenResponse> GetToken(TokenParameters parameters)
    {
        var response = new TokenResponse();

        try
        {
            HttpResponseMessage message;
            switch (parameters.GrantType)
            {
                case TokenGrantType.ClientCredentials:
                    message = await GetToken(parameters.Domain, new Dictionary<string, string> {
                        {"grant_type", $"client_credentials" },
                        {"audience", $"{parameters.Audience}" },
                        {"client_id", $"{parameters.Client?.UserName}" },
                        {"client_secret", $"{parameters.Client?.Password}" }
                    });
                    break;
                case TokenGrantType.Password:
                    message = await GetToken(parameters.Domain, new Dictionary<string, string> {
                        {"grant_type", $"password" },
                        {"audience", $"{parameters.Audience}" },
                        {"client_id", $"{parameters.Client?.UserName}" },
                        {"client_secret", $"{parameters.Client?.Password}" },
                        {"username", $"{parameters.User?.UserName}" },
                        {"password", $"{parameters.User?.Password}" },
                        {"scope", $"{parameters.Scope}" }
                    });
                    break;
                case TokenGrantType.AuthorizationCode:
                    message = await GetToken(parameters.Domain, new Dictionary<string, string> {
                        {"grant_type", $"authorization_code" },
                        {"client_id", $"{parameters.Client?.UserName}" },
                        {"client_secret", $"{parameters.Client?.Password}" },
                        {"code", $"{parameters.Code}" },
                        {"redirect_uri", $"{parameters.RedirectUri}" }
                    });
                    break;
                case TokenGrantType.RefreshToken:
                    message = await GetToken(parameters.Domain, new Dictionary<string, string> {
                        {"grant_type", $"refresh_token" },
                        {"client_id", $"{parameters.Client?.UserName}" },
                        {"client_secret", $"{parameters.Client?.Password}" },
                        {"refresh_token", $"{parameters.RefreshToken}" }
                    });
                    break;
                default:
                    throw new NotSupportedException($"invalid grant type: {parameters.GrantType}");
            }

            response.Token = message.IsSuccessStatusCode
                ? await message.Content.ReadFromJsonAsync<Token>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                : null;
            response.IsSuccessful = message.IsSuccessStatusCode;

        }
        catch (Exception ex)
        {
            response.IsSuccessful = false;
            response.Exception = ex;
        }

        return response;
    }

    private async Task<HttpResponseMessage> GetToken(string domain, Dictionary<string, string> values)
    {
        var url = $"https://{domain}/oauth/token";
        return await _httpClient.PostAsJsonAsync(url, values);
    }
}
