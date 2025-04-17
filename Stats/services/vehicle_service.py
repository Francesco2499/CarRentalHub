import matplotlib.pyplot as plt
import matplotlib.ticker as ticker
import os
from flask import send_file
from repositories.vehicle_repository import (
    get_vehicle_availability,
    get_top_5_vehicles
)
import textwrap

IMAGE_DIR = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "static", "images"))
os.makedirs(IMAGE_DIR, exist_ok=True)

def generate_availability_plot(start_date=None, end_date=None):
    availability = get_vehicle_availability(start_date, end_date).iloc[0]
    fig, ax = plt.subplots()
    ax.pie([availability['available'], availability['rented']],
           labels=["Disponibili", "Prenotati"],
           autopct='%1.1f%%', startangle=90,
           colors=['#1f77b4', '#28a745'])
    ax.set_title("Disponibilità Veicoli", fontweight='bold', fontsize=16)
    
    file_path = os.path.join(IMAGE_DIR, "availability_plot.png")
    fig.savefig(file_path)
    plt.close(fig)
    
    return send_file(file_path, as_attachment=True)

def generate_top_vehicles_plot(start_date=None, end_date=None):
    data = get_top_5_vehicles(start_date, end_date)

    def wrap_labels(labels, width=10):
        return [textwrap.fill(label, width=width) for label in labels]

    wrap_labels(data['vehicle_model'], width=12)

    fig, ax = plt.subplots(figsize=(10, 6))
    ax.bar(data['vehicle_model'], data['rentals'], width=0.5)
    
    ax.set_title("Top 5 Veicoli più richiesti", fontweight='bold', fontsize=16)
    ax.set_xlabel("Veicolo", fontweight='bold', fontsize=12, labelpad=10)
    ax.set_ylabel("Numero di prenotazioni", fontweight='bold', fontsize=12, labelpad=10)
    ax.yaxis.set_major_locator(ticker.MaxNLocator(integer=True))
        
    plt.tight_layout()

    file_path = os.path.join(IMAGE_DIR, "top_vehicles_plot.png")
    fig.savefig(file_path)
    plt.close(fig)
    
    return send_file(file_path, as_attachment=True)

