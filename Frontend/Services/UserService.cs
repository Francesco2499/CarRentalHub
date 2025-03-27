using System.Collections.Generic;
using Frontend.Models;
using Frontend.Helpers;
namespace Frontend.Services;

public class UserService
{

    public static LoginResponse? Authenticate(string username, string password)
    {
        var requestBody = new Dictionary<string, object>
        {
            { "username", username },
            { "password", PasswordHasher.HashPassword(password) }
        };

        var loginResponse = HttpService.Post<LoginResponse>("http://localhost:8085/api/v1/auth/login", requestBody);

        if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
        {
            TokenHelper.SetToken(loginResponse.Token);
        }

        return loginResponse;
    }

    public static RegistrationResponse? Register(string email, string username, string password, bool isAdmin, string region)
    {
        var requestBody = new Dictionary<string, object>
        {
            { "email", email },
            { "username", username },
            { "password", PasswordHasher.HashPassword(password) },
            { "region", region },
            { "role", isAdmin ? "admin" : "" }
        };

        return HttpService.Post<RegistrationResponse>("http://localhost:8085/api/v1/auth/register", requestBody);
    }

    public static RegistrationResponse? EditProfile(UserModel user, string? newPassword)
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

        return HttpService.Put<RegistrationResponse>("http://localhost:8085/api/v1/user/update/me", requestBody);
    }
}

public record RegistrationResponse(string? Message, UserModel? User, object? Error);

public record LoginResponse(string? Message, string? Token, UserModel? User, object? Error);
