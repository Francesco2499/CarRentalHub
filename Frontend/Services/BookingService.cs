using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Frontend.Models;

namespace Frontend.Services;

public static class BookingService
{
    public static async Task<BookingResponse?> AddBooking(int vehicleId, DateTime? startDate, DateTime? endDate)
    {
        var requestBody = new Dictionary<string, object>
        {
            { "vehicle_id", vehicleId }
        };

        if (startDate.HasValue)
            requestBody["start_date"] = startDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");

        if (endDate.HasValue)
            requestBody["end_date"] = endDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");

        return await HttpService.PostAsync<BookingResponse>("http://localhost:8085/api/v1/booking/new", requestBody);
    }

    public static async Task<List<BookingModel>> GetAllBookings()
    {
        var bookings = await HttpService.GetAsync<List<BookingModel>>("http://localhost:8085/api/v1/booking/getAll");
        return bookings ?? [];
    }

    public static async Task DeleteBooking(int bookingId)
    {
        try
        {
            await HttpService.DeleteAsync<dynamic>($"http://localhost:8085/api/v1/booking/delete/{bookingId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore durante l'eliminazione della prenotazione {bookingId}: {ex.Message}");
        }
    }

    public static async Task<BookingResponse?> EditBooking(BookingModel booking, int vehicleId)
    {
        var requestBody = new Dictionary<string, object>
        {
            { "user_id", booking.UserId },
            { "vehicle_id", vehicleId },
            { "start_date", booking.StartDate.ToString("yyyy-MM-ddTHH:mm:ssZ")},
            { "end_date", booking.EndDate.ToString("yyyy-MM-ddTHH:mm:ssZ")}
        };

        return await HttpService.PutAsync<BookingResponse>($"http://localhost:8085/api/v1/booking/update/{booking.Id}", requestBody);
    }
    public record BookingResponse(BookingModel? Booking, string? Message, object? Error);

}


