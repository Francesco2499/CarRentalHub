using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Frontend.Services;

public class UserService
{
    private readonly HttpService _httpService;

    public UserService()
    {
        _httpService = new HttpService();
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public async Task<LoginResponse?> Authenticate(string username, string password)
    {
        var requestBody = new
        {
            username,
            password
        };

        // Usa il metodo generico PostAsync per inviare la richiesta di login
        var loginResponse = await _httpService.PostAsync<LoginResponse>("http://localhost:8085/api/v1/auth/login", requestBody);

        if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
        {
            // Memorizza il token nella classe AuthService
            TokenService.SetToken(loginResponse.Token);
        }

        return loginResponse;
    }

    public async Task<RegistrationResponse?> Register(string email, string username, string password, bool isAdmin)
    {
            // Crea il corpo della richiesta JSON con i dati di registrazione
            var requestBody = new
            {
                email,
                username,
                password,
                role = isAdmin ? "admin" : ""
            };

            return await _httpService.PostAsync<RegistrationResponse>("http://localhost:8085/api/v1/auth/register", requestBody);        
    }
}

public record RegistrationResponse(string Message, UserResponse? User);

public record UserResponse(string username, string email, string password, string role);

public record LoginResponse(string Message, string? Token, bool IsAdmin);

