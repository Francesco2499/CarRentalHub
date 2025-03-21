import matplotlib.pyplot as plt
import matplotlib.ticker as ticker
import os
from flask import send_file
from repositories.vehicle_repository import *

IMAGE_DIR = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "static", "images"))
os.makedirs(IMAGE_DIR, exist_ok=True)

def generate_availability_plot(start_date=None, end_date=None):
    availability = get_vehicle_availability(start_date, end_date).iloc[0]
    fig, ax = plt.subplots()
    ax.pie([availability['available'], availability['rented']],
           labels=["Available", "Rented"],
           autopct='%1.1f%%', startangle=90)
    ax.set_title("Vehicle Availability")
    
    file_path = os.path.join(IMAGE_DIR, "availability_plot.png")
    fig.savefig(file_path)
    return send_file(file_path, as_attachment=True)

def generate_top_vehicles_plot(start_date=None, end_date=None):
    data = get_top_5_vehicles(start_date, end_date)
    fig, ax = plt.subplots()
    ax.bar(data['vehicle_model'], data['rentals'])
    ax.set_title("Top 5 Most Rented Vehicles")
    ax.set_xlabel("Vehicle Model")
    ax.set_ylabel("Number of Rentals")
    ax.yaxis.set_major_locator(ticker.MaxNLocator(integer=True))
    
    file_path = os.path.join(IMAGE_DIR, "top_vehicles_plot.png")
    fig.savefig(file_path)
    return send_file(file_path, as_attachment=True)

