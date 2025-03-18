using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        // Metodo che gestisce le richieste in base al tipo di metodo HTTP (GET, POST, PUT, DELETE) in modo sincrono
        private static TResponse? SendRequest<TResponse>(HttpMethod method, string url, object? requestBody)
        {
            try
            {
                // Aggiungi l'header di autorizzazione con il token, se disponibile
                string? token = TokenService.GetToken();

                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                HttpRequestMessage requestMessage = new HttpRequestMessage(method, url);

                if (requestBody != null)
                {
                    // Imposta il corpo della richiesta JSON per POST, PUT, DELETE
                    requestMessage.Content = JsonContent.Create(requestBody);
                }

                // Invia la richiesta HTTP (sincrona)
                HttpResponseMessage response = _httpClient.Send(requestMessage); // Rimuoviamo l'await, facendo la richiesta sincrona

                // Leggi il corpo della risposta
                string responseBody = response.Content.ReadAsStringAsync().Result; // Usando .Result per ottenere il risultato sincrono

                Console.WriteLine("Response Body: " + responseBody);

                // Se la risposta è vuota o null, restituisci un valore predefinito
                if (string.IsNullOrWhiteSpace(responseBody) || responseBody == null)
                {
                    // Se la risposta è vuota o null, ritorna un valore predefinito come una lista vuota o null
                    return default; // O anche new List<BookingModel>() se ti aspetti una lista
                }

                // Se la risposta non è vuota, deserializza normalmente
                return JsonSerializer.Deserialize<TResponse?>(responseBody, JsonOptions) ?? throw new InvalidOperationException("Deserializzazione fallita.");
            }
            catch (Exception ex)
            {
                // Gestisci gli errori di connessione o altri errori
                Console.WriteLine("Errore: " + ex.Message);
                return default; // Se c'è un errore, ritorna un valore predefinito
            }
        }
    }
}
