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

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public static TResponse? Post<TResponse>(string url, object requestBody)
        {
            return SendRequest<TResponse>(HttpMethod.Post, url, requestBody);
        }

        public static TResponse? Put<TResponse>(string url, object requestBody)
        {
            return SendRequest<TResponse>(HttpMethod.Put, url, requestBody);
        }

        public static TResponse? Delete<TResponse>(string url)
        {
            return SendRequest<TResponse>(HttpMethod.Delete, url, null);
        }

        public static TResponse? Get<TResponse>(string url)
        {
            return SendRequest<TResponse>(HttpMethod.Get, url, null);
        }

        private static TResponse? SendRequest<TResponse>(HttpMethod method, string url, object? requestBody)
        {
            try
            {
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

                HttpResponseMessage response = _httpClient.Send(requestMessage);

                string responseBody = response.Content.ReadAsStringAsync().Result;

                if (string.IsNullOrWhiteSpace(responseBody))
                {
                    return default;
                }

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
