using System;
using System.Text.Json.Serialization;

namespace Frontend.Models;

public record BookingModel(
    int Id,
    [property: JsonPropertyName("user_id")] int UserId,
    string Username,
    [property: JsonPropertyName("vehicle_id")] int VehicleId,
    [property: JsonPropertyName("vehicle_model")] string VehicleModel,
    [property: JsonPropertyName("total_price")] float TotalPrice,
    [property: JsonPropertyName("start_date")] DateTime StartDate,
    [property: JsonPropertyName("end_date")] DateTime EndDate,
    [property: JsonPropertyName("created_at")] DateTime CreatedAt
);