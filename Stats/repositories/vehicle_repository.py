from db import execute_query

def get_vehicle_availability(start_date=None, end_date=None):
    if start_date and end_date:
        query = """
        SELECT 
            COUNT(*) FILTER (
                WHERE EXISTS (
                    SELECT 1
                    FROM bookings b
                    WHERE b.vehicle_id = v.id
                    AND (b.start_date, b.end_date) OVERLAPS (%s, %s)
                )
            ) AS rented,
            COUNT(*) FILTER (
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM bookings b
                    WHERE b.vehicle_id = v.id
                    AND (b.start_date, b.end_date) OVERLAPS (%s, %s)
                )
            ) AS available
        FROM vehicles v;
        """
        params = (start_date, end_date, start_date, end_date)
        return execute_query(query, params)
    else:
        # Default: considera veicolo occupato se ha prenotazione che si sovrappone a "oggi"
        query = """
        SELECT 
            COUNT(*) FILTER (
                WHERE EXISTS (
                    SELECT 1
                    FROM bookings b
                    WHERE b.vehicle_id = v.id
                    AND (b.start_date, b.end_date) OVERLAPS (CURRENT_DATE, CURRENT_DATE)
                )
            ) AS rented,
            COUNT(*) FILTER (
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM bookings b
                    WHERE b.vehicle_id = v.id
                    AND (b.start_date, b.end_date) OVERLAPS (CURRENT_DATE, CURRENT_DATE)
                )
            ) AS available
        FROM vehicles v;
        """
        return execute_query(query)


def get_top_5_vehicles(start_date=None, end_date=None):
    if start_date and end_date:
        query = """
        SELECT v.model AS vehicle_model, COUNT(*) AS rentals
        FROM bookings b
        JOIN vehicles v ON b.vehicle_id = v.id
        WHERE b.start_date >= %s AND b.end_date <= %s
        GROUP BY v.model
        ORDER BY rentals DESC
        LIMIT 5;
        """        
        
        params = (start_date, end_date)
        return execute_query(query, params)
    else:
        query = """
        SELECT v.model AS vehicle_model, COUNT(*) AS rentals
        FROM bookings b
        JOIN vehicles v ON b.vehicle_id = v.id
        GROUP BY v.model
        ORDER BY rentals DESC
        LIMIT 5;
        """
        
        return execute_query(query)
