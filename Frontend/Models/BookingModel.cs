using System;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Models;

public record BookingModel(
    int Id,
    int user_id,
    int vehicle_id,
    string vehicle_model,
    DateTime start_date,
    DateTime end_date,
    DateTime created_at,
    DateTime updated_at
);