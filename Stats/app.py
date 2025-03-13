from flask import Flask, jsonify
# from user_stats import *
# from booking_stats import *
from vehicle_stats import *

app = Flask(__name__)

@app.route('/vehicle/stats/availability', methods=['GET'])
def vehicle_stats_availability():
    return generate_vehicle_availability()

@app.route('/vehicle/stats/top5', methods=['GET'])
def vehicle_stats_top():
    return generate_top_vehicles_plot()

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=5005)
