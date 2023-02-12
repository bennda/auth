using DBN.Auth.Auth0.Models;

namespace DBN.Auth.Auth0;

public interface ITokenService
{
    Task<TokenResponse> GetToken(TokenParameters parameters);
}