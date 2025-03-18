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

        public static BookingModel? AddBooking(int vehicle_id, DateTime? start_date, DateTime? end_date)
        {
            var requestBody = new
            {
                vehicle_id,
                start_date = start_date?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                end_date = end_date?.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            var result = HttpService.Post<BookingModel?>("http://localhost:8085/api/v1/booking/new", requestBody);
           
            return result;
        }

        public static List<BookingModel> GetAllBookings()
        {
            // Esegui la richiesta GET
            var bookings = HttpService.Get<List<BookingModel>>("http://localhost:8085/api/v1/booking/getAll");

            return bookings ?? []; // Restituisci una lista vuota se bookings è null
        }

           public void DeleteBooking(int bookingId)
        {
            try
            {
                HttpService.Delete<dynamic>($"http://localhost:8085/api/v1/booking/delete/{bookingId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting booking with ID {bookingId}: {ex.Message}");
            }
        }

        public static List<BookingModel> EditBooking(BookingModel booking)
        {
            var requestBody = new
            {
                booking.user_id,
                booking.vehicle_id,
                start_date = booking.start_date.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                end_date = booking.end_date.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            var updatedBookings = HttpService.Put<List<BookingModel>>($"http://localhost:8085/api/v1/booking/update/{booking.Id}", requestBody);

            if (updatedBookings == null)
            {
                return []; 
            }

            return updatedBookings;
        }
    }
}
