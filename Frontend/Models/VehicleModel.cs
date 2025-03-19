using System;

namespace Frontend.Models;

public record VehicleModel(
    int Id,
    string Model,
    string Category,
    decimal Price,
    string Location
);

