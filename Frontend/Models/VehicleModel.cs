using System;
using System.Text.Json.Serialization;

namespace Frontend.Models;

public record VehicleModel(
    int Id,
    string Model,
    string Category,
    decimal Price,
    [property: JsonPropertyName("car_showroom_id")] int CarShowroomID,
    [property: JsonPropertyName("car_showroom_name")] string ShowroomName

);

