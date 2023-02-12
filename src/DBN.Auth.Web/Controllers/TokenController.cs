namespace DBN.Auth.Web.Controllers;

[Route("/")]
public class TokenController : Controller
{
    private readonly ITokenService _authService;

    public TokenController(IHttpClientFactory httpClientFactory)
    {
        _authService = AuthFactory.CreateAuth0TokenService(httpClientFactory.CreateClient());
    }

    [HttpGet]
    [Route("/{type?}")]
    public async Task<IActionResult> Index(string? type)
    {
        return View("Index",await Task.FromResult(Enum.TryParse(type, true, out TokenGrantType grantType)
            ? TokenViewModel.Create(grantType)
            : new TokenViewModel()));
    }

    [HttpPost]
    [Route("/ClientCredentials")]
    public async Task<IActionResult> ClientCredentials(TokenRequestClientCredentials request)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", await Task.FromResult(TokenViewModel.Create(request)));
        }
        var response = await _authService.GetToken(TokenParameters.ForClientCredentials(
            domain: $"{request.Domain}",
            audience: $"{request.Audience}",
            client: new NetworkCredential(request.ClientId, request.ClientSecret)));

        return View("Index", await Task.FromResult(TokenViewModel.Create(request, response)));
    }

    [HttpPost]
    [Route("/Password")]
    public async Task<IActionResult> Password(TokenRequestPassword request)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", await Task.FromResult(TokenViewModel.Create(request)));
        }
        var response = await _authService.GetToken(TokenParameters.ForPassword(
            domain: $"{request.Domain}",
            audience: $"{request.Audience}",
            user: new NetworkCredential(request.Username, request.Password),
            client: new NetworkCredential(request.ClientId, request.ClientSecret),
            scope: $"{request.Scope}"));

        return View("Index", await Task.FromResult(TokenViewModel.Create(request, response)));
    }

    [HttpPost]
    [Route("/RefreshToken")]
    public async Task<IActionResult> RefreshToken(TokenRequestRefreshToken request)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", await Task.FromResult(TokenViewModel.Create(request)));
        }
        var response = await _authService.GetToken(TokenParameters.ForRefreshToken(
            domain: $"{request.Domain}",
            refreshToken: $"{request.RefreshToken}",
            client: new NetworkCredential(request.ClientId, request.ClientSecret)));

        return View("Index", await Task.FromResult(TokenViewModel.Create(request, response)));
    }

    [HttpPost]
    [Route("/AuthorizationCode")]
    public async Task<IActionResult> AuthorizationCode(TokenRequestAuthorizationCode request)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", await Task.FromResult(TokenViewModel.Create(request)));
        }
        var response = await _authService.GetToken(TokenParameters.ForAuthorizationCode(
            domain: $"{request.Domain}",
            client: new NetworkCredential(request.ClientId, request.ClientSecret),
            code: $"{request.Code}",
            redirectUri: $"{request.RedirectUri}"));

        return View("Index", await Task.FromResult(TokenViewModel.Create(request, response)));
    }
}