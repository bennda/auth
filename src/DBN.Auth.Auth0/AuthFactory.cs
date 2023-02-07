namespace DBN.Auth.Auth0;

public class AuthFactory
{
    public static ITokenService CreateAuth0TokenService(HttpClient httpClient)
    {
        return new Auth0TokenService(httpClient);
    }
}
