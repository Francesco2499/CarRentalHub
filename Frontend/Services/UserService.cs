using System.Collections.Generic;
using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Helpers;

namespace Frontend.Services;

public class UserService
{
    public static async Task<LoginResponse?> Authenticate(string username, string password)
    {
        var requestBody = new Dictionary<string, object>
        {
            { "username", username },
            { "password", PasswordHasher.HashPassword(password) }
        };

        var loginResponse = await HttpService.PostAsync<LoginResponse>("http://localhost:8085/api/v1/auth/login", requestBody);

        if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
        {
            TokenHelper.SetToken(loginResponse.Token);
        }

        return loginResponse;
    }

    public static async Task<RegistrationResponse?> Register(string email, string username, string password, bool isAdmin, string region)
    {
        var requestBody = new Dictionary<string, object>
        {
            { "email", email },
            { "username", username },
            { "password", PasswordHasher.HashPassword(password) },
            { "region", region },
            { "role", isAdmin ? "admin" : "" }
        };

        return await HttpService.PostAsync<RegistrationResponse>("http://localhost:8085/api/v1/auth/register", requestBody);
    }

    public static async Task<RegistrationResponse?> EditProfile(UserModel user, string? newPassword)
    {
        var requestBody = new Dictionary<string, object>
        {
            { "email", user.Email },
            { "username", user.Username },
            { "region", user.Region }
        };

        if (!string.IsNullOrEmpty(newPassword))
        {
            requestBody["password"] = PasswordHasher.HashPassword(user.Password);
            requestBody["new_password"] = PasswordHasher.HashPassword(newPassword);
        }

        return await HttpService.PutAsync<RegistrationResponse>("http://localhost:8085/api/v1/user/update/me", requestBody);
    }
    public record RegistrationResponse(string? Message, UserModel? User, object? Error);

    public record LoginResponse(string? Message, string? Token, UserModel? User, object? Error);
}



