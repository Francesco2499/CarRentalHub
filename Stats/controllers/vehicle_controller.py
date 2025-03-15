from flask import Blueprint
from services.vehicle_service import *

vehicle_stats_blueprint = Blueprint('vehicle_stats', __name__)

@vehicle_stats_blueprint.route('/availability', methods=['GET'])
def vehicle_stats_availability():
    return generate_availability_plot()

@vehicle_stats_blueprint.route('/top5', methods=['GET'])
def vehicle_stats_top5():
    return generate_top_vehicles_plot()
