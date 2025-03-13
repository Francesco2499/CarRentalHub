from db import get_db_connection, execute_query
import matplotlib.pyplot as plt
import io
from flask import send_file
import os

# Directory dove salvare le immagini
IMAGE_DIR = "static/images"
os.makedirs(IMAGE_DIR, exist_ok=True)

def generate_vehicle_availability():
    # Query per ottenere la disponibilità dei veicoli (veicoli disponibili vs in noleggio)
    query_availability = """
    SELECT 
    COUNT(DISTINCT CASE 
        WHEN b.start_date <= NOW() AND b.end_date >= NOW() THEN v.id
        END) AS rented,
    COUNT(DISTINCT CASE 
        WHEN (b.start_date > NOW() OR b.end_date < NOW() OR b.end_date IS NULL) THEN v.id 
        END) AS available
    FROM vehicles v
    LEFT JOIN bookings b ON v.id = b.vehicle_id
    """
    
    # Eseguiamo la query per la disponibilità
    availability = execute_query(query_availability).iloc[0]

    fig2, ax2 = plt.subplots()
    ax2.pie([availability['available'], availability['rented']],
            labels=["Disponibili", "Noleggiati"],
            autopct='%1.1f%%', startangle=90)
    ax2.set_title("Disponibilità dei Veicoli")
    plot_availability = "availability_plot.png"
    fig2.savefig(os.path.join(IMAGE_DIR, plot_availability))

    # Statistiche aggiuntive (Disponibilità veicoli)
    return send_file(os.path.join(IMAGE_DIR, plot_availability), as_attachment=True)



def generate_top_vehicles_plot():
    # Query per ottenere i veicoli più noleggiati
    query_top_5_vehicles = """
    SELECT v.model AS vehicle_model, COUNT(*) AS rentals
    FROM bookings b
    JOIN vehicles v ON b.vehicle_id = v.id
    GROUP BY v.model
    ORDER BY rentals DESC
    LIMIT 5;
    """
    
    # Eseguiamo la query per ottenere i dati
    df_top_vehicles = execute_query(query_top_5_vehicles)
    
    # Statistiche: Top 5 veicoli più noleggiati
    top_5_rented = df_top_vehicles
    
    # Grafico: Top 5 veicoli più noleggiati
    vehicle_models = top_5_rented['vehicle_model'].values  # Estrai i veicoli
    rentals = top_5_rented['rentals'].values  # Estrai il numero di noleggi
    
    # Creare il grafico
    fig1, ax1 = plt.subplots()
    ax1.bar([str(vehicle_model) for vehicle_model in vehicle_models], rentals)    
    ax1.set_title("Top 5 Veicoli più Noleggiati")
    ax1.set_xlabel("Modello Veicolo")
    ax1.set_ylabel("Numero di Noleggi")
    plot_top_vehicles = "top_vehicles_plot.png"
    fig1.savefig(os.path.join(IMAGE_DIR, plot_top_vehicles))
    
    return send_file(os.path.join(IMAGE_DIR, plot_top_vehicles), as_attachment=True)

