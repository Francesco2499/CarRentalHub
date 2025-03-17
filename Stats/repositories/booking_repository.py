from db import execute_query

def get_revenue_trend(start_date=None, end_date=None):
    if start_date and end_date:
        query = """
        SELECT b.start_date::date AS date, 
               SUM((DATE_PART('day', b.end_date - b.start_date) + 1) * v.price) AS total_revenue
        FROM bookings b
        JOIN vehicles v ON b.vehicle_id = v.id
        WHERE b.start_date BETWEEN %s AND %s
        GROUP BY date
        ORDER BY date;
        """
        params = (start_date, end_date)
        return execute_query(query, params)
    else:
        query = """
        SELECT b.start_date::date AS date, 
               SUM((DATE_PART('day', b.end_date - b.start_date) + 1) * v.price) AS total_revenue
        FROM bookings b
        JOIN vehicles v ON b.vehicle_id = v.id
        WHERE b.start_date BETWEEN NOW() - INTERVAL '30 days' AND NOW()
        GROUP BY date
        ORDER BY date;
        """
        return execute_query(query)

def get_booking_trend(start_date=None, end_date=None):
    if start_date and end_date:
        query = """
        SELECT b.start_date::date AS date,
               COUNT(*) AS booking_count
        FROM bookings b
        WHERE b.start_date BETWEEN %s AND %s
        GROUP BY date
        ORDER BY date;
        """
        params = (start_date, end_date)
        return execute_query(query, params)
    else:
        query = """
        SELECT b.start_date::date AS date,
               COUNT(*) AS booking_count
        FROM bookings b
        WHERE b.start_date BETWEEN NOW() - INTERVAL '30 days' AND NOW()
        GROUP BY date
        ORDER BY date;
        """
        return execute_query(query)

def get_revenue_per_vehicle(start_date=None, end_date=None):
    if start_date and end_date:
        query = """
        SELECT v.model AS vehicle_model,
               SUM((DATE_PART('day', b.end_date - b.start_date) + 1) * v.price) AS revenue
        FROM bookings b
        JOIN vehicles v ON b.vehicle_id = v.id
        WHERE b.start_date BETWEEN %s AND %s
        GROUP BY v.model
        ORDER BY revenue DESC;
        """
        params = (start_date, end_date)
        return execute_query(query, params)
    else:
        query = """
        SELECT v.model AS vehicle_model,
               SUM((DATE_PART('day', b.end_date - b.start_date) + 1) * v.price) AS revenue
        FROM bookings b
        JOIN vehicles v ON b.vehicle_id = v.id
        WHERE b.start_date BETWEEN NOW() - INTERVAL '30 days' AND NOW()
        GROUP BY v.model
        ORDER BY revenue DESC;
        """
        return execute_query(query)
