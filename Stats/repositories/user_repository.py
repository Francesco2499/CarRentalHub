from db import execute_query

def get_top_3_users():
    query = """
    SELECT u.username, COUNT(*) AS bookings
    FROM users u
    JOIN bookings b ON u.id = b.user_id
    GROUP BY u.username
    ORDER BY bookings DESC
    LIMIT 3;
    """
    return execute_query(query)

def get_users_by_region():
    query = """
    SELECT region, COUNT(*) AS users_count
    FROM users
    GROUP BY region;
    """
    return execute_query(query)

def get_user_statistics(user_id):
    query = f"""
    SELECT 
        u.id,
        u.username,
        COUNT(b.id) AS total_bookings,
        COALESCE(SUM(
            (DATE_PART('day', b.end_date - b.start_date) + 1) * v.price
        ), 0) AS total_spent
    FROM users u
    LEFT JOIN bookings b ON u.id = b.user_id
    LEFT JOIN vehicles v ON b.vehicle_id = v.id
    WHERE u.id = {user_id}
    GROUP BY u.id, u.username;
    """
    return execute_query(query)

