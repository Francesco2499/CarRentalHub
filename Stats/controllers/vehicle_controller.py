from flask import Blueprint, request
from services.vehicle_service import (
    generate_availability_plot,
    generate_top_vehicles_plot
)
from utils.date_utils import validate_and_get_date_range

vehicle_stats_blueprint = Blueprint('vehicle_stats', __name__)

@vehicle_stats_blueprint.route('/availability', methods=['GET'])
def vehicle_stats_availability():
    start_date = request.args.get('start_date')
    end_date = request.args.get('end_date')
    
    start_date, end_date, error_response, status_code = validate_and_get_date_range(start_date, end_date)
    if error_response:
        return error_response, status_code
    
    return generate_availability_plot(start_date, end_date)

@vehicle_stats_blueprint.route('/top5', methods=['GET'])
def vehicle_stats_top5():
    start_date = request.args.get('start_date')
    end_date = request.args.get('end_date')
    
    start_date, end_date, error_response, status_code = validate_and_get_date_range(start_date, end_date)
    if error_response:
        return error_response, status_code
    
    return generate_top_vehicles_plot(start_date, end_date)
