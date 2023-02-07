using DBN.Auth.Auth0.Models;

namespace DBN.Auth.Web.Models;

public class TokenViewModel
{

    [Required]
    [DisplayName("grant_type")]
    public TokenGrantType? GrantType { get; set; }

    [Required]
    [DisplayName("domain")]
    public string? Domain { get; set; }

    [Required]
    [DisplayName("client_id")]
    public string? ClientId { get; set; }

    [Required]
    [DisplayName("client_secret")]
    public string? ClientSecret { get; set; }

    [Required]
    [DisplayName("Response")]
    public TokenResponse? TokenResponse { get; set; }
}

public class TokenViewModelClientCredentials : TokenViewModel
{
    [Required]
    [DisplayName("audience")]
    public string? Audience { get; set; }

    public TokenViewModelClientCredentials()
    {
        GrantType = TokenGrantType.ClientCredentials;
    }
}

public class TokenViewModelPassword : TokenViewModel
{
    [Required]
    [DisplayName("audience")]
    public string? Audience { get; set; }

    [Required]
    [DisplayName("username")]
    public string? Username { get; set; }

    [Required]
    [DisplayName("password")]
    public string? Password { get; set; }

    [Required]
    [DisplayName("scope")]
    public string? Scope { get; set; }

    public TokenViewModelPassword()
    {
        GrantType = TokenGrantType.Password;
    }
}

public class TokenViewModelRefreshToken : TokenViewModel
{
    [Required]
    [DisplayName("refresh_token")]
    public string? RefreshToken { get; set; }

    public TokenViewModelRefreshToken()
    {
        GrantType = TokenGrantType.RefreshToken;
    }
}

public class TokenViewModelAuthorizationCode : TokenViewModel
{
    [Required]
    [DisplayName("redirect_uri")]
    public string? RedirectUri { get; set; }

    [Required]
    [DisplayName("code")]
    public string? Code { get; set; }

    public TokenViewModelAuthorizationCode()
    {
        GrantType = TokenGrantType.AuthorizationCode;
    }
}