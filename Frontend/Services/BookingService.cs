using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Frontend.Models;

namespace Frontend.Services
{
    public class BookingService
    {
        private readonly HttpService _httpService;

        public BookingService()
        {
            _httpService = new HttpService();
        }
        
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public async Task<BookingModel?> AddBooking(int vehicle_id, DateTime? start_date, DateTime? end_date)
        {
            var requestBody = new
            {
                vehicle_id,
                start_date = start_date?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                end_date = end_date?.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

 
            return await _httpService.PostAsync<BookingModel?>($"http://localhost:8085/api/v1/booking/new", requestBody);
        }
    }
}
