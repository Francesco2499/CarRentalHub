namespace Frontend.Models;

public record UserModel(
    string Username, 
    string Email, 
    string Password, 
    string Region,
    string Role
);
