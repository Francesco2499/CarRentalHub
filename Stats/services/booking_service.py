import matplotlib.pyplot as plt
import os
from flask import send_file
import matplotlib.ticker as ticker
from repositories.booking_repository import *
import textwrap

IMAGE_DIR = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "static", "images"))
os.makedirs(IMAGE_DIR, exist_ok=True)

def generate_revenue_trend_plot(start_date=None, end_date=None):
    df = get_revenue_trend(start_date, end_date)
    fig, ax = plt.subplots()
    ax.plot(df['date'], df['total_revenue'], marker='o')
    ax.set_title("Trend Guadagno", fontweight='bold', fontsize=16)
    ax.set_xlabel("Periodo", fontweight='bold', fontsize=12, labelpad=10)
    ax.set_ylabel("Guadagno (€)", fontweight='bold', fontsize=12, labelpad=10)
    fig.autofmt_xdate()
    plt.tight_layout()

    path = os.path.join(IMAGE_DIR, "revenue_trend.png")
    fig.savefig(path)
    return send_file(path, as_attachment=True)

def generate_booking_trend_plot(start_date=None, end_date=None):
    df = get_booking_trend(start_date, end_date)
    fig, ax = plt.subplots()
    ax.bar(df['date'], df['booking_count'], width=0.5)
    ax.set_title("Trend Prenotazioni", fontweight='bold', fontsize=16)
    ax.set_xlabel("Periodo", fontweight='bold', fontsize=12, labelpad=10)
    ax.set_ylabel("Numero di prenotazioni", fontweight='bold', fontsize=12, labelpad=10)
    ax.yaxis.set_major_locator(ticker.MaxNLocator(integer=True))

    fig.autofmt_xdate()
    
    plt.tight_layout()

    path = os.path.join(IMAGE_DIR, "booking_trend.png")
    fig.savefig(path)
    plt.close(fig)
    return send_file(path, as_attachment=True)

def generate_revenue_per_vehicle_plot(start_date=None, end_date=None):
    df = get_revenue_per_vehicle(start_date, end_date)
    
    def wrap_labels(labels, width=10):
        return [textwrap.fill(label, width=width) for label in labels]

    wrapped_labels = wrap_labels(df['vehicle_model'], width=12)
    
    fig, ax = plt.subplots(figsize=(10, 6))
    ax.bar(df['vehicle_model'], df['revenue'], width=0.5)
    ax.set_title("Guadagno per veicolo", fontweight='bold', fontsize=16)
    ax.set_xlabel("Veicoli", fontweight='bold', fontsize=12, labelpad=10)
    ax.set_ylabel("Guadagno (€)", fontweight='bold', fontsize=12, labelpad=10)
    
    fig.autofmt_xdate()
    plt.tight_layout()

    path = os.path.join(IMAGE_DIR, "revenue_for_vehicle.png")
    fig.savefig(path)
    plt.close(fig)
    return send_file(path, as_attachment=True)
