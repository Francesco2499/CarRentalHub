from datetime import datetime, timedelta
from flask import jsonify

def validate_and_get_date_range(start_date, end_date, default_days_range=30):
    """
    Verifica che le date siano nel formato corretto. 
    Se non fornite, applica valori di default: 
    - end_date = oggi
    - start_date = oggi - default_days_range
    """

    try:
        if end_date:
            end_date_obj = datetime.strptime(end_date, "%Y-%m-%d")
        else:
            end_date_obj = datetime.today()

        if start_date:
            start_date_obj = datetime.strptime(start_date, "%Y-%m-%d")
        else:
            start_date_obj = end_date_obj - timedelta(days=default_days_range)

        if start_date_obj > end_date_obj:
            return None, None, jsonify({"error": "Start date must be before or equal to end date"}), 400

        # ritorna stringhe ben formattate da usare nelle query
        return start_date_obj.strftime("%Y-%m-%d"), end_date_obj.strftime("%Y-%m-%d"), None, None

    except ValueError:
        return None, None, jsonify({"error": "Invalid date format. Expected YYYY-MM-DD"}), 400
