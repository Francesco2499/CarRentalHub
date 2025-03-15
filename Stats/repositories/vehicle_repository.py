from db import execute_query

def get_vehicle_availability():
    query = """
    SELECT 
    COUNT(DISTINCT CASE 
        WHEN b.start_date <= NOW() AND b.end_date >= NOW() THEN v.id
        END) AS rented,
    COUNT(DISTINCT CASE 
        WHEN (b.start_date > NOW() OR b.end_date < NOW() OR b.end_date IS NULL) THEN v.id 
        END) AS available
    FROM vehicles v
    LEFT JOIN bookings b ON v.id = b.vehicle_id
    """
    return execute_query(query)

def get_top_5_vehicles():
    query = """
    SELECT v.model AS vehicle_model, COUNT(*) AS rentals
    FROM bookings b
    JOIN vehicles v ON b.vehicle_id = v.id
    GROUP BY v.model
    ORDER BY rentals DESC
    LIMIT 5;
    """
    return execute_query(query)
