from datetime import datetime
from flask import jsonify

def validate_and_get_date_range(start_date, end_date):
    """
    Valida il formato delle date se presenti.
    Se uno dei due è presente, l'altro deve esserlo.
    Se entrambi sono assenti, ritorna None (nessun filtro temporale).
    """
    try:
        if start_date and end_date:
            start_date_obj = datetime.strptime(start_date, "%Y-%m-%d")
            end_date_obj = datetime.strptime(end_date, "%Y-%m-%d")

            if start_date_obj > end_date_obj:
                return None, None, jsonify({"error": "La data di inzio deve precedere quella di fine!"}), 400

            return start_date_obj.strftime("%Y-%m-%d"), end_date_obj.strftime("%Y-%m-%d"), None, None

        elif start_date or end_date:
            return None, None, jsonify({"error": "Entrambe le date sono obbligatorie"}), 400

        else:
            # Entrambe assenti => nessun filtro (restituisci None)
            return None, None, None, None

    except ValueError:
        return None, None, jsonify({"error": "Formato date non valido!"}), 400
