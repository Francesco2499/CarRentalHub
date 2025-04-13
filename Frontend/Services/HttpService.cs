using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Frontend.Helpers;

namespace Frontend.Services
{
    public static class HttpService
    {
        private static readonly HttpClient _httpClient = new();

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public static async Task<TResponse?> PostAsync<TResponse>(string url, object requestBody)
        {
            return await SendRequestAsync<TResponse>(HttpMethod.Post, url, requestBody);
        }

        public static async Task<TResponse?> PutAsync<TResponse>(string url, object requestBody)
        {
            return await SendRequestAsync<TResponse>(HttpMethod.Put, url, requestBody);
        }

        public static async Task<TResponse?> DeleteAsync<TResponse>(string url)
        {
            return await SendRequestAsync<TResponse>(HttpMethod.Delete, url, null);
        }

        public static async Task<TResponse?> GetAsync<TResponse>(string url)
        {
            return await SendRequestAsync<TResponse>(HttpMethod.Get, url, null);
        }

        private static async Task<TResponse?> SendRequestAsync<TResponse>(HttpMethod method, string url, object? requestBody)
        {
            try
            {
                string? token = TokenHelper.GetToken();

                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                using HttpRequestMessage requestMessage = new(method, url);

                if (requestBody != null)
                {
                    requestMessage.Content = JsonContent.Create(requestBody);
                }

                using HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();

                if (response.Content.Headers.ContentLength == 0)
                {
                    return default;
                }

                var responseBody = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<TResponse>(responseBody, JsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore: " + ex.Message);
                return default;
            }
        }
    }
}
