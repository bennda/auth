namespace DBN.Auth.Auth0;

public class AuthApiParameters
{
    public string Domain { get; set; }
    public string Audience { get; set; }
    public Type TokenServiceType { get; set; } = typeof(Auth0TokenService);
    public IEnumerable<AuthPolicyParameter> AuthorizationPolicies { get; }

    public AuthApiParameters(string domain, string audience, IEnumerable<AuthPolicyParameter>? authorizationPolicies = null)
    {
        Domain = domain;
        Audience = audience;
        AuthorizationPolicies = authorizationPolicies ?? Array.Empty<AuthPolicyParameter>();
    }
    public AuthApiParameters(string domain, string audience, AuthPolicyParameter authorizationPolicy)
        : this(domain, audience, new AuthPolicyParameter[] { authorizationPolicy })
    {
    }
}

public class AuthPolicyParameter
{
    public AuthPolicyParameter(string name, AuthPolicyClaim claim) : this(name, new[] { claim })
    {
    }
    public AuthPolicyParameter(string name, IEnumerable<AuthPolicyClaim> claims)
    {
        Name = name;
        Claims = claims;
    }

    public string Name { get; }

    public IEnumerable<AuthPolicyClaim> Claims { get; }
}

public class AuthPolicyClaim
{
    public string Type { get; }
    public string? Issuer { get; set; }
    public string Value { get; }

    public AuthPolicyClaim(string type, string value)
    {
        Type = type;
        Value = value;
    }
    public AuthPolicyClaim(string type, string issuer, string value) : this(type, value)
    {
        Issuer = issuer.StartsWith("http")
            ? issuer
            : $"https://{issuer}/";
    }
}

public class AuthAppParameters
{
    public AuthAppParameters(string domain, string audience, string clientId, string scope, string callbackPath, string callbackHost, IEnumerable<string> permissionRoles)
    {
        Domain = domain;
        Audience = audience;
        ClientId = clientId;
        Scope = scope;
        CallbackPath = callbackPath;
        CallbackHost = callbackHost;
        PermissionRoles = permissionRoles;
    }

    public string Domain { get; set; }
    public string Audience { get; set; }
    public string ClientId { get; set; }
    public string Scope { get; set; }
    public string CallbackPath { get; set; }
    public string CallbackHost { get; set; }
    public IEnumerable<string> PermissionRoles { get; set; }
}