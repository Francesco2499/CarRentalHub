import matplotlib.pyplot as plt
import os
from flask import send_file
from repositories.booking_repository import *


IMAGE_DIR = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "static", "images"))
os.makedirs(IMAGE_DIR, exist_ok=True)

def generate_revenue_trend_plot(start_date=None, end_date=None):
    df = get_revenue_trend(start_date, end_date)
    fig, ax = plt.subplots()
    ax.plot(df['date'], df['total_revenue'], marker='o')
    ax.set_title("Revenue Trend")
    ax.set_xlabel("Date")
    ax.set_ylabel("Revenue (€)")
    fig.autofmt_xdate()
    
    path = os.path.join(IMAGE_DIR, "revenue_trend.png")
    fig.savefig(path)
    return send_file(path, as_attachment=True)

def generate_booking_trend_plot(start_date=None, end_date=None):
    df = get_booking_trend(start_date, end_date)
    fig, ax = plt.subplots()
    ax.bar(df['date'], df['booking_count'])
    ax.set_title("Booking Trend")
    ax.set_xlabel("Date")
    ax.set_ylabel("Number of Bookings")
    fig.autofmt_xdate()
    
    path = os.path.join(IMAGE_DIR, "booking_trend.png")
    fig.savefig(path)
    return send_file(path, as_attachment=True)

def generate_revenue_per_vehicle_plot(start_date=None, end_date=None):
    df = get_revenue_per_vehicle(start_date, end_date)
    fig, ax = plt.subplots()
    ax.bar(df['vehicle_model'], df['revenue'])
    ax.set_title("Revenue per Vehicle")
    ax.set_xlabel("Vehicle Model")
    ax.set_ylabel("Revenue (€)")
    
    path = os.path.join(IMAGE_DIR, "revenue_for_vehicle.png")
    fig.savefig(path)
    return send_file(path, as_attachment=True)
