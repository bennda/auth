namespace DBN.Auth.Web.Controllers;

public class TokenController : Controller
{
    private readonly IAuthService _authService;

    public TokenController(IHttpClientFactory httpClientFactory)
    {
        _authService = new Auth0Service(httpClientFactory.CreateClient());
    }

    [HttpGet]
    public IActionResult Index(string? grantType)
    {
        TokenViewModel? viewModel = null;
        switch (grantType?.ToLower())
        {
            case "client_credentials":
                viewModel = new TokenViewModelClientCredentials();
                break;
            case "password":
                viewModel = new TokenViewModelPassword();
                break;
            case "refresh_token":
                viewModel = new TokenViewModelRefreshToken();
                break;
            case "authorization_code":
                viewModel = new TokenViewModelAuthorizationCode();
                break;
            default:
                viewModel = new TokenViewModel();
                break;
        }

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> ClientCredentials(TokenViewModelClientCredentials viewModel)
    {
        viewModel.TokenResponse = await _authService.GetTokenForClientCredentials(
            domain: $"{viewModel.Domain}",
            audience: $"{viewModel.Audience}",
            client: new System.Net.NetworkCredential(viewModel.ClientId, viewModel.ClientSecret));

        return View("Index", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Password(TokenViewModelPassword viewModel)
    {
        viewModel.TokenResponse = await _authService.GetTokenForPassword(
            domain: $"{viewModel.Domain}",
            audience: $"{viewModel.Audience}",
            user: new System.Net.NetworkCredential(viewModel.Username, viewModel.Password),
            client: new System.Net.NetworkCredential(viewModel.ClientId, viewModel.ClientSecret),
            scope: $"{viewModel.Scope}");

        return View("Index", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> RefreshToken(TokenViewModelRefreshToken viewModel)
    {
        viewModel.TokenResponse = await _authService.GetTokenForRefreshToken(
            domain: $"{viewModel.Domain}",
            refreshToken: $"{viewModel.RefreshToken}",
            client: new System.Net.NetworkCredential(viewModel.ClientId, viewModel.ClientSecret));

        return View("Index", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AuthorizationCode(TokenViewModelAuthorizationCode viewModel)
    {
        viewModel.TokenResponse = await _authService.GetTokenForAuthorizationCode(
            domain: $"{viewModel.Domain}",

            client: new System.Net.NetworkCredential(viewModel.ClientId, viewModel.ClientSecret),
            code: $"{viewModel.Code}",
            redirectUri: $"{viewModel.RedirectUri}"
            );

        return View("Index", viewModel);
    }

    [HttpPost]
    public IActionResult Index(TokenViewModel viewModel)
    {
        return View(viewModel);
    }
}