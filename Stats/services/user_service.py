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
    ax.bar(df["username"], df["bookings"])
    ax.set_title("Top 3 Users with most bookings")
    ax.set_xlabel("Users")
    ax.set_ylabel("Bookings")
    ax.yaxis.set_major_locator(ticker.MaxNLocator(integer=True))
    
    file_path = os.path.join(IMAGE_DIR, "top_users_plot.png")
    fig.savefig(file_path)
    return send_file(file_path, as_attachment=True)

def generate_users_by_region_plot():
    df = get_users_by_region()
    fig, ax = plt.subplots()
    ax.pie(df["users_count"], labels=df["region"], autopct='%1.1f%%', startangle=90)
    ax.set_title("User Distribution by Regione")
    file_path = os.path.join(IMAGE_DIR, "users_by_region_plot.png")
    fig.savefig(file_path)
    return send_file(file_path, as_attachment=True)

def get_user_info(user_id):
    df = get_user_statistics(user_id)
    if df.empty:
        return jsonify({"message": "User not found"}), 404

    row = df.iloc[0]
    return jsonify({
        "userId": int(row["id"]),
        "username": str(row["username"]),
        "totalBookings": int(row["total_bookings"]),
        "totalSpent": float(row["total_spent"])
    })
