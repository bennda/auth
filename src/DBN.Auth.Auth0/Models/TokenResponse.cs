namespace DBN.Auth.Auth0.Models;

public class TokenResponse : AuthResponse
{
    [DataMember(IsRequired = false)]
    public Token? Token { get; set; }
}
