using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Frontend.Helpers;

namespace Frontend.Services
{
    public class HttpService
    {
        private static readonly HttpClient _httpClient = new();

        // Impostazioni per la serializzazione JSON
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        // Metodo generico per fare richieste POST (sincrono)
        public static TResponse? Post<TResponse>(string url, object requestBody)
        {
            return SendRequest<TResponse>(HttpMethod.Post, url, requestBody);
        }

        // Metodo generico per fare richieste PUT (sincrono)
        public static TResponse? Put<TResponse>(string url, object requestBody)
        {
            return SendRequest<TResponse>(HttpMethod.Put, url, requestBody);
        }

        // Metodo generico per fare richieste DELETE (sincrono)
        public static TResponse? Delete<TResponse>(string url)
        {
            return SendRequest<TResponse>(HttpMethod.Delete, url, null);
        }

        // Metodo generico per fare richieste GET (sincrono)
        public static TResponse? Get<TResponse>(string url)
        {
            return SendRequest<TResponse>(HttpMethod.Get, url, null);
        }

        // Metodo per gestire richieste HTTP in modo sincrono
        private static TResponse? SendRequest<TResponse>(HttpMethod method, string url, object? requestBody)
        {
            try
            {
                // Aggiungi l'header di autorizzazione con il token, se disponibile
                string? token = TokenHelper.GetToken();

                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                HttpRequestMessage requestMessage = new(method, url);

                if (requestBody != null)
                {
                    requestMessage.Content = JsonContent.Create(requestBody);
                }

                // Invia la richiesta HTTP (sincrona)
                HttpResponseMessage response = _httpClient.Send(requestMessage);

                // Leggi il corpo della risposta
                string responseBody = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine("Response Body: " + responseBody);

                // Se la risposta è vuota o nulla, restituisci un valore predefinito
                if (string.IsNullOrWhiteSpace(responseBody))
                {
                    return default;
                }

                // Deserializza la risposta in TResponse
                return JsonSerializer.Deserialize<TResponse>(responseBody, JsonOptions);
            }
            catch (Exception ex)
            {
                // Gestisci errori di connessione e altri errori generali
                Console.WriteLine("Errore: " + ex.Message);
                return default;
            }
        }
    }
}
