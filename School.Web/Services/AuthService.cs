using Blazored.SessionStorage;
using School.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace School.Web.Services;

public class AuthService
{
    private readonly IHttpClientFactory _factory;
    private readonly ISessionStorageService _sessionStorage;

    private const string TokenKey = "authToken";

    private string? _token;

    public event Action? OnAuthStateChanged;

    public string? Username { get; private set; }
    public string? Role { get; private set; }

    public ClaimsPrincipal CurrentUser { get; private set; } =
        new ClaimsPrincipal(new ClaimsIdentity());

    public bool IsAdmin => GetRole().Equals("Admin", StringComparison.OrdinalIgnoreCase);
    public bool IsTeacher => GetRole().Equals("Teacher", StringComparison.OrdinalIgnoreCase);

    public AuthService(IHttpClientFactory factory,
                       ISessionStorageService sessionStorage)
    {
        _factory = factory;
        _sessionStorage = sessionStorage;
    }

    // Call this when app starts
    public async Task InitializeAsync()
    {
        _token = await _sessionStorage.GetItemAsync<string>(TokenKey);

        if (!string.IsNullOrWhiteSpace(_token))
        {
            DecodeToken(_token);
            OnAuthStateChanged?.Invoke();
        }
        else
        {
            ClearAuthState();
        }
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var client = _factory.CreateClient("SchoolApi");

        var response = await client.PostAsJsonAsync("api/auth/login", new
        {
            username,
            password
        });

        if (!response.IsSuccessStatusCode)
            return false;

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        if (result == null || string.IsNullOrWhiteSpace(result.Token))
            return false;

        _token = result.Token;

        DecodeToken(_token);

        await _sessionStorage.SetItemAsync(TokenKey, _token);

        OnAuthStateChanged?.Invoke();

        return true;
    }

    public HttpClient GetAuthorizedClient()
    {
        var client = _factory.CreateClient("SchoolApi");

        if (!string.IsNullOrWhiteSpace(_token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);
        }

        return client;
    }

    public async Task LogoutAsync()
    {
        _token = null;
        ClearAuthState();

        await _sessionStorage.RemoveItemAsync(TokenKey);
        OnAuthStateChanged?.Invoke();
    }

    private void DecodeToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            ClearAuthState();
            return;
        }

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        // Build ClaimsPrincipal from JWT claims
        var identity = new ClaimsIdentity(jwt.Claims, "jwt");
        CurrentUser = new ClaimsPrincipal(identity);

        // Username claim fallbacks
        Username =
            jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ??
            jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value ??
            jwt.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value ??
            jwt.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value ??
            jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        // Role claim fallbacks
        Role =
            jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ??
            jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value ??
            jwt.Claims.FirstOrDefault(c => c.Type == "roles")?.Value ??
            jwt.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
    }

    private void ClearAuthState()
    {
        Username = null;
        Role = null;
        CurrentUser = new ClaimsPrincipal(new ClaimsIdentity());
    }

    public string GetRole()
    {
        return CurrentUser?.FindFirst(ClaimTypes.Role)?.Value ??
               CurrentUser?.FindFirst("role")?.Value ??
               CurrentUser?.FindFirst("roles")?.Value ??
               Role ??
               string.Empty;
    }

    public bool HasRole(params string[] roles)
    {
        var currentRole = GetRole();
        return roles.Any(r => string.Equals(r, currentRole, StringComparison.OrdinalIgnoreCase));
    }

    public bool IsLoggedIn => !string.IsNullOrWhiteSpace(_token);
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
}