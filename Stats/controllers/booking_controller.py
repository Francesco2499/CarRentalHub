from flask import Blueprint, request
from services.booking_service import (
    generate_revenue_trend_plot, 
    generate_booking_trend_plot,
    generate_revenue_per_vehicle_plot)
from utils.date_utils import validate_and_get_date_range

booking_stats_blueprint = Blueprint('booking_stats', __name__)

@booking_stats_blueprint.route('/revenue/trend', methods=['GET'])
def revenue_trend():
    start_date = request.args.get("start_date")
    end_date = request.args.get("end_date")
    
    start_date, end_date, error_response, status_code = validate_and_get_date_range(start_date, end_date)
    if error_response:
        return error_response, status_code
    
    return generate_revenue_trend_plot(start_date, end_date)

@booking_stats_blueprint.route('/numbookings/trend', methods=['GET'])
def booking_trend():
    start_date = request.args.get("start_date")
    end_date = request.args.get("end_date")
    
    start_date, end_date, error_response, status_code = validate_and_get_date_range(start_date, end_date)
    if error_response:
        return error_response, status_code
    
    return generate_booking_trend_plot(start_date, end_date)

@booking_stats_blueprint.route('/revenueforvehicle/trend', methods=['GET'])
def revenue_per_vehicle():
    start_date = request.args.get("start_date")
    end_date = request.args.get("end_date")
    
    start_date, end_date, error_response, status_code = validate_and_get_date_range(start_date, end_date)
    if error_response:
        return error_response, status_code
    
    return generate_revenue_per_vehicle_plot(start_date, end_date)
