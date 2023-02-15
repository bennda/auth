namespace DBN.Auth.Auth0.Handlers;

internal class HasClaimRequirement : IAuthorizationRequirement
{
    public IEnumerable<AuthPolicyClaim> Claims { get; }

    public HasClaimRequirement(IEnumerable<AuthPolicyClaim> claims)
    {
        Claims = claims ?? throw new ArgumentNullException(nameof(claims));
    }
    public HasClaimRequirement(AuthPolicyClaim claim, string domain)
    {   
        if (string.IsNullOrEmpty(claim.Issuer))
        {
            claim.Issuer = $"https://{domain}/";
        }
        
        Claims = new AuthPolicyClaim[] { claim };
    }
}

internal class HasClaimHandler : AuthorizationHandler<HasClaimRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasClaimRequirement requirement)
    {
        foreach (var req in requirement.Claims)
        {
            var claims = context.User.Claims.Where(c => c.Type == req.Type && c.Issuer == req.Issuer).ToArray();
            if (!claims.Any()) {
                return Task.CompletedTask;
            }

            if (!claims.Any(c => c.Value == req.Value))
            {
                switch (req.Type)
                {
                    case "scope":
                        var scopes = claims.SelectMany(c => c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)).Distinct();
                        if (scopes.Contains(req.Value))
                        {
                            return Task.CompletedTask;
                        }
                        break;
                    default:
                        return Task.CompletedTask;
                }
            }
        }

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}



