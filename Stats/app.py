from flask import Flask
from controllers.vehicle_controller import vehicle_stats_blueprint
from controllers.user_controller import user_stats_blueprint
from controllers.booking_controller import booking_stats_blueprint

app = Flask(__name__)
app.register_blueprint(vehicle_stats_blueprint, url_prefix='/vehicles/stats')
app.register_blueprint(user_stats_blueprint, url_prefix='/users/stats')
app.register_blueprint(booking_stats_blueprint, url_prefix='/bookings/stats')

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=5005)
