using Amazon.Lambda.Core;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace Valkyrie.Functions.Handlers;
public class AuthorizerFunction
{
    private readonly IConfiguration _configuration;
    private static readonly HttpClient httpClient = new HttpClient();
    private static JsonWebKeySet? jwks;
    private readonly string issuer;
    private readonly string jwksUri;

    public AuthorizerFunction() : this(BuildConfiguration()) { }

    // For DI/testing
    public AuthorizerFunction(IConfiguration configuration)
    {
        _configuration = configuration;
        issuer = _configuration["Authentication:Issuer"] ?? "";
        jwksUri = _configuration["Authentication:JwksUri"] ?? "";
    }

    public async Task<APIGatewayCustomAuthorizerResponse> FunctionHandler(
        APIGatewayCustomAuthorizerRequest request,
        ILambdaContext context)
    {
        string? token = request.AuthorizationToken?.Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            return Deny(request.MethodArn);

        // Fetch JWKS if not cached
        if (jwks == null)
        {
            var jwksJson = await httpClient.GetStringAsync(jwksUri);
            jwks = new JsonWebKeySet(jwksJson);
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = issuer,
            ValidateIssuer = true,
            ValidateAudience = false,
            IssuerSigningKeys = jwks.Keys
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            // Extraer roles de los claims
            var roles = principal.Claims
                .Where(c => c.Type == "roles" || c.Type == "role" || c.Type == "cognito:groups")
                .Select(c => c.Value)
                .ToList();
            string? userRole = roles.FirstOrDefault();
            return Allow(principal, request.MethodArn, userRole);
        }
        catch
        {
            return Deny(request.MethodArn);
        }
    }

    private APIGatewayCustomAuthorizerResponse Allow(ClaimsPrincipal principal, string methodArn, string? userRole)
    {
        var contextDict = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(userRole))
        {
            contextDict["role"] = userRole;
        }
        if (methodArn == null)
            throw new ArgumentNullException(nameof(methodArn));
        return new APIGatewayCustomAuthorizerResponse
        {
            PrincipalID = principal.Identity?.Name ?? "user",
            PolicyDocument = new APIGatewayCustomAuthorizerPolicy
            {
                Version = "2012-10-17",
                Statement = new List<APIGatewayCustomAuthorizerPolicy.IAMStatement>
                {
                    new APIGatewayCustomAuthorizerPolicy.IAMStatement
                    {
                        Action = new HashSet<string> { "execute-api:Invoke" },
                        Effect = "Allow",
                        Resource = new HashSet<string> { methodArn }
                    }
                }
            },
            Context = contextDict
        };
    }

    private APIGatewayCustomAuthorizerResponse Deny(string methodArn)
    {
        if (methodArn == null)
            throw new ArgumentNullException(nameof(methodArn));
        return new APIGatewayCustomAuthorizerResponse
        {
            PrincipalID = "unauthorized",
            PolicyDocument = new APIGatewayCustomAuthorizerPolicy
            {
                Version = "2012-10-17",
                Statement = new List<APIGatewayCustomAuthorizerPolicy.IAMStatement>
                {
                    new APIGatewayCustomAuthorizerPolicy.IAMStatement
                    {
                        Action = new HashSet<string> { "execute-api:Invoke" },
                        Effect = "Deny",
                        Resource = new HashSet<string> { methodArn }
                    }
                }
            }
        };
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();
    }
}

// Policy response classes for API Gateway Lambda Authorizer
public class APIGatewayCustomAuthorizerResponse
{
    public string PrincipalID { get; set; } = string.Empty;
    public APIGatewayCustomAuthorizerPolicy PolicyDocument { get; set; } = new();
    public IDictionary<string, object>? Context { get; set; }
}

public class APIGatewayCustomAuthorizerPolicy
{
    public string Version { get; set; } = "2012-10-17";
    public IList<IAMStatement> Statement { get; set; } = new List<IAMStatement>();

    public class IAMStatement
    {
        public HashSet<string> Action { get; set; } = new();
        public string Effect { get; set; } = string.Empty;
        public HashSet<string> Resource { get; set; } = new();
    }
}

// API Gateway custom authorizer request (for completeness)
public class APIGatewayCustomAuthorizerRequest
{
    public string? Type { get; set; }
    public string? AuthorizationToken { get; set; }
    public string? MethodArn { get; set; }
}

