from flask import Flask, jsonify
# from user_stats import *
# from booking_stats import *
#from vehicle_stats import *
from controllers.vehicle_controller import vehicle_stats_blueprint
from controllers.user_controller import user_stats_blueprint

app = Flask(__name__)
'''
@app.route('/vehicle/stats/availability', methods=['GET'])
def vehicle_stats_availability():
    return generate_vehicle_availability()

@app.route('/vehicle/stats/top5', methods=['GET'])
def vehicle_stats_top():
    return generate_top_vehicles_plot()
'''

app.register_blueprint(vehicle_stats_blueprint, url_prefix='/vehicles/stats')
app.register_blueprint(user_stats_blueprint, url_prefix='/users/stats')
#app.register_blueprint(booking_stats_blueprint, url_prefix='/bookings/stats')

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=5005)
