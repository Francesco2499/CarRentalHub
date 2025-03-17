from dotenv import load_dotenv
import os
from pathlib import Path

# Percorso al file .env in CarRentalHub/Backend/.env
dotenv_path = Path(__file__).resolve().parent.parent / 'Backend' / '.env'
load_dotenv(dotenv_path=dotenv_path)

#load_dotenv()  # Carica variabili da .env

DB_CONFIG = {
    "dbname": os.getenv("DB_NAME"),
    "user": os.getenv("DB_USER"),
    "password": os.getenv("DB_PASSWORD"),
    "host": os.getenv("DB_HOST"),
    "port": int(os.getenv("DB_PORT", 5432))
}