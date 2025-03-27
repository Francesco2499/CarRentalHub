using System;
using System.Collections.Generic;
using Frontend.Models;

namespace Frontend.Services;

public class BookingService
{
    public static BookingResponse? AddBooking(int vehicleId, DateTime? startDate, DateTime? endDate)
    {
        var requestBody = new Dictionary<string, object>
        {
            { "vehicle_id", vehicleId }
        };

        if (startDate.HasValue)
            requestBody["start_date"] = startDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");

        if (endDate.HasValue)
            requestBody["end_date"] = endDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");

        return HttpService.Post<BookingResponse>("http://localhost:8085/api/v1/booking/new", requestBody);
    }

    public static List<BookingModel> GetAllBookings()
    {
        var bookings = HttpService.Get<List<BookingModel>>("http://localhost:8085/api/v1/booking/getAll");
        return bookings ?? []; // Restituisce una lista vuota se null
    }

    public static void DeleteBooking(int bookingId)
    {
        try
        {
            HttpService.Delete<dynamic>($"http://localhost:8085/api/v1/booking/delete/{bookingId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore durante l'eliminazione della prenotazione {bookingId}: {ex.Message}");
        }
    }

    public static BookingResponse? EditBooking(BookingModel booking, int vehicleId)
    {
        var requestBody = new Dictionary<string, object>
        {
            { "user_id", booking.UserId },
            { "vehicle_id", vehicleId },
            { "start_date", booking.StartDate.ToString("yyyy-MM-ddTHH:mm:ssZ")},
            { "end_date", booking.EndDate.ToString("yyyy-MM-ddTHH:mm:ssZ")}
        };

        return HttpService.Put<BookingResponse>($"http://localhost:8085/api/v1/booking/update/{booking.Id}", requestBody);
    }
}

public record BookingResponse(BookingModel? Booking, string? Message, object? Error);
