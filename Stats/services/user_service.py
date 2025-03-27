import matplotlib.pyplot as plt
import matplotlib.ticker as ticker
import os
from flask import send_file, jsonify
from repositories.user_repository import *

IMAGE_DIR = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "static", "images"))
os.makedirs(IMAGE_DIR, exist_ok=True)

def generate_top_users_plot():
    df = get_top_3_users()
    fig, ax = plt.subplots()
    ax.bar(df["username"], df["bookings"], width=0.5)
    ax.set_title("Top 3 Users with most bookings", fontweight='bold', fontsize=16)
    ax.set_xlabel("Utenti", fontweight='bold', fontsize=12, labelpad=10)
    ax.set_ylabel("Prenotazioni", fontweight='bold', fontsize=12, labelpad=10)
    ax.yaxis.set_major_locator(ticker.MaxNLocator(integer=True))
    
    plt.tight_layout()

    file_path = os.path.join(IMAGE_DIR, "top_users_plot.png")
    fig.savefig(file_path)
    plt.close(fig)
    
    return send_file(file_path, as_attachment=True)

def generate_users_by_region_plot():
    df = get_users_by_region()
    fig, ax = plt.subplots()
    ax.pie(df["users_count"], labels=df["region"], autopct='%1.1f%%', startangle=90)
    ax.set_title("Distribuzione utenti per regione", fontweight='bold', fontsize=16)
    
    plt.tight_layout()

    file_path = os.path.join(IMAGE_DIR, "users_by_region_plot.png")
    fig.savefig(file_path)
    plt.close(fig)
    
    return send_file(file_path, as_attachment=True)

def get_user_info(user_id):
    df = get_user_statistics(user_id)
    if df.empty:
        return jsonify({"error": "Utente non trovato!"}), 404

    row = df.iloc[0]
    return jsonify({
        "userId": int(row["id"]),
        "username": str(row["username"]),
        "totalBookings": int(row["total_bookings"]),
        "totalSpent": float(row["total_spent"])
    })
