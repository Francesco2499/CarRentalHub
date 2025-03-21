using System;
using System.Security.Cryptography;
using System.Text;

namespace Frontend.Helpers;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        // Converte la password in un array di byte
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));

        // Converte il risultato in una stringa esadecimale
        StringBuilder builder = new();
        foreach (byte b in bytes)
        {
            builder.Append(b.ToString("x2")); // Converte il byte in formato esadecimale
        }
        return builder.ToString();
    }
}
