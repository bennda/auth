namespace DBN.Auth.Web.Models;

public class TokenViewModel
{
    public static TokenViewModel Create(TokenGrantType? grantType = null)
    {
        if (grantType == null)
        {
            return new TokenViewModel();
        }
        return Create(TokenRequest.Create(grantType.Value));
    }
    public static TokenViewModel Create(TokenRequest request, TokenResponse? response = null)
    {
        return new TokenViewModel
        {
            GrantType = request.GrantType,
            TokenRequest = request,
            TokenResponse = response
        };
    }

    [DisplayName("grant_type")]
    public TokenGrantType? GrantType
    {
        get; set;
    }

    public SelectListItem[] GrantTypes { get; } = new SelectListItem[] {
        new SelectListItem {Text="", Value=null},
        new SelectListItem {Text="client_credentials", Value=$"{TokenGrantType.ClientCredentials}" },
        new SelectListItem {Text="password", Value=$"{TokenGrantType.Password}" },
        new SelectListItem {Text="authorization_code", Value=$"{TokenGrantType.AuthorizationCode}" },
        new SelectListItem {Text="refresh_token", Value=$"{TokenGrantType.RefreshToken}" }};

    public TokenRequest? TokenRequest { get; set; }
    public TokenResponse? TokenResponse { get; set; }
}

public class TokenRequest
{
    public static TokenRequest Create(TokenGrantType grantType)
    {
        TokenRequest request;

        switch (grantType)
        {
            case TokenGrantType.ClientCredentials:
                request = new TokenRequestClientCredentials();
                break;
            case TokenGrantType.Password:
                request = new TokenRequestPassword();
                break;
            case TokenGrantType.RefreshToken:
                request = new TokenRequestRefreshToken();
                break;
            case TokenGrantType.AuthorizationCode:
                request = new TokenRequestAuthorizationCode();
                break;
            default:
                throw new NotSupportedException();
        }

        return request;
    }

    [Required]
    [DisplayName("grant_type")]
    public virtual TokenGrantType? GrantType { get; }

    [Required(ErrorMessage = "domain is required", AllowEmptyStrings = false)]
    [DisplayName("domain")]
    public string? Domain { get; set; }

    [Required(ErrorMessage = "client_id is required", AllowEmptyStrings = false)]
    [DisplayName("client_id")]
    public string? ClientId { get; set; }

    [Required(ErrorMessage = "client_secret is required", AllowEmptyStrings = false)]
    [DisplayName("client_secret")]
    public string? ClientSecret { get; set; }
}

public class TokenRequestClientCredentials : TokenRequest
{
    [Required(ErrorMessage = "audience is required", AllowEmptyStrings = false)]
    [DisplayName("audience")]
    public string? Audience { get; set; }
    public override TokenGrantType? GrantType { get; } = TokenGrantType.ClientCredentials;
}

public class TokenRequestPassword : TokenRequest
{
    [Required(ErrorMessage = "audience is required", AllowEmptyStrings = false)]
    [DisplayName("audience")]
    public string? Audience { get; set; }

    [Required(ErrorMessage = "username is required", AllowEmptyStrings = false)]
    [DisplayName("username")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "password is required", AllowEmptyStrings = false)]
    [DisplayName("password")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "scope is required", AllowEmptyStrings = false)]
    [DisplayName("scope")]
    public string? Scope { get; set; }
    public override TokenGrantType? GrantType { get; } = TokenGrantType.Password;
}

public class TokenRequestRefreshToken : TokenRequest
{
    [Required(ErrorMessage = "refresh_token is required", AllowEmptyStrings = false)]
    [DisplayName("refresh_token")]
    public string? RefreshToken { get; set; }
    public override TokenGrantType? GrantType { get; } = TokenGrantType.RefreshToken;
}

public class TokenRequestAuthorizationCode : TokenRequest
{
    [Required(ErrorMessage = "redirect_uri is required", AllowEmptyStrings = false)]
    [DisplayName("redirect_uri")]
    public string? RedirectUri { get; set; }

    [Required(ErrorMessage = "code is required", AllowEmptyStrings = false)]
    [DisplayName("code")]
    public string? Code { get; set; }
    public override TokenGrantType? GrantType { get; } = TokenGrantType.AuthorizationCode;
}