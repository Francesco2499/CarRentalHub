using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Diagnostics; // Aggiungi questa direttiva per il debug


namespace Frontend.Services
{
    public class HttpService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        // Impostazioni per la serializzazione JSON
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        // Metodo generico per fare richieste POST
        public async Task<TResponse?> PostAsync<TResponse>(string url, object requestBody)
        {
            return await SendRequestAsync<TResponse>(HttpMethod.Post, url, requestBody);
        }

        // Metodo generico per fare richieste PUT
        public async Task<TResponse?> PutAsync<TResponse>(string url, object requestBody)
        {
            return await SendRequestAsync<TResponse>(HttpMethod.Put, url, requestBody);
        }

        // Metodo generico per fare richieste DELETE
        public async Task<TResponse?> DeleteAsync<TResponse>(string url)
        {
            return await SendRequestAsync<TResponse>(HttpMethod.Delete, url, null);
        }

        // Metodo generico per fare richieste GET
        public async Task<TResponse?> GetAsync<TResponse>(string url)
        {
            return await SendRequestAsync<TResponse>(HttpMethod.Get, url, null);
        }

        // Metodo che gestisce le richieste in base al tipo di metodo HTTP (GET, POST, PUT, DELETE)
        private async Task<TResponse?> SendRequestAsync<TResponse>(HttpMethod method, string url, object? requestBody)
        {
            try
            {
                // Aggiungi l'header di autorizzazione con il token, se disponibile
                string? token = TokenService.GetToken();

                Debug.WriteLine($"Token ottenuto: {token}");

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

                // Invia la richiesta HTTP
                HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);

                // Ottieni la risposta come stringa
                string responseBody = await response.Content.ReadAsStringAsync();

                // Se la richiesta ha avuto successo, deserializza il corpo JSON nella risposta desiderata
                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<TResponse>(responseBody, JsonOptions);
                }
                else
                {
                    // In caso di errore, restituisci un oggetto di tipo TResponse (potrebbe essere un errore)
                    return JsonSerializer.Deserialize<TResponse>(responseBody, JsonOptions);
                }
            }
            catch (Exception ex)
            {
                // Gestisci gli errori di connessione
                return JsonSerializer.Deserialize<TResponse>($"{{\"Message\": \"Errore di connessione: {ex.Message}\", \"Token\": null}}", JsonOptions);
            }
        }
    }
}
