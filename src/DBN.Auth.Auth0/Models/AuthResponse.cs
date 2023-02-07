namespace DBN.Auth.Auth0.Models;

public abstract class AuthResponse
{
    public bool IsSuccessful { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
}