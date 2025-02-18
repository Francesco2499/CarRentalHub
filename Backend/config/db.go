package config

import (
	"database/sql"
	"fmt"
	"log"
	"os"

	_ "github.com/lib/pq"
	"github.com/joho/godotenv"
)

var db *sql.DB

func InitDB() (*sql.DB, error) {

	// Carica il file .env (se esiste)
	if err := godotenv.Load(); err != nil {
		log.Println("Warning: No .env file found, using system environment variables")
	}

	host, err := getEnv("DB_HOST")
	if err != nil {
		log.Fatal(err)
	}
	user, err := getEnv("DB_USER")
	if err != nil {
		log.Fatal(err)
	}
	password, err := getEnv("DB_PASSWORD")
	if err != nil {
		log.Fatal(err)
	}
	dbname, err := getEnv("DB_NAME")
	if err != nil {
		log.Fatal(err)
	}

	// Usa variabili d'ambiente per la connessione al database
	/*dsn := fmt.Sprintf(
		"host=%s user=%s password=%s dbname=%s sslmode=disable",
		getEnv("DB_HOST", "localhost"),
		getEnv("DB_USER", "postgres"),
		getEnv("DB_PASSWORD", "password"),
		getEnv("DB_NAME", "CarRentalHub"),
	)*/

	dsn := fmt.Sprintf(
		"host=%s user=%s password=%s dbname=%s sslmode=disable",
		host, user, password, dbname,
	)


	//var err error
	db, err = sql.Open("postgres", dsn)
	if err != nil {
		log.Fatal("Errore nella connessione al database:", err)
		return nil, err
	}

	// Testa la connessione con Ping()
	if err := db.Ping(); err != nil {
		log.Fatal("Database ping error:", err)
		return nil, err
	}

	fmt.Println("Database connected successfully!")

	fmt.Println("Starting table creation...")
	if err := CreateTables(); err != nil {
		log.Fatal("Error creating tables:", err)
	}

	if err := SeedData(); err != nil {
		log.Fatal("Error entering test data:", err)
	}

	return db, nil
}

// CreateTables crea le tabelle se non esistono
func CreateTables() error {
	tables := []string{
		`CREATE TABLE IF NOT EXISTS vehicles (
			id SERIAL PRIMARY KEY,
			model VARCHAR(255) NOT NULL,
			category VARCHAR(255) NOT NULL,
			price DECIMAL(10,2) NOT NULL,
			available BOOLEAN DEFAULT TRUE,
			location VARCHAR(255) NOT NULL
		);`,

		`CREATE TABLE IF NOT EXISTS users (
			id SERIAL PRIMARY KEY,
			username VARCHAR(50) UNIQUE NOT NULL,
			email VARCHAR(100) UNIQUE NOT NULL,
			password TEXT NOT NULL,
			role VARCHAR(20) NOT NULL CHECK (role IN ('admin', 'customer'))
		);`,

		`CREATE TABLE IF NOT EXISTS bookings (
			id SERIAL PRIMARY KEY,
			user_id INT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
			vehicle_id INT NOT NULL REFERENCES vehicles(id) ON DELETE CASCADE,
			start_date TIMESTAMP NOT NULL,
			end_date TIMESTAMP NOT NULL,
			created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
			updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
		);`,
	}

	for _, table := range tables {
		_, err := db.Exec(table)
		if err != nil {
			return err
		}
	}

	fmt.Println("Tables created/verified successfully!")

	// Creazione della funzione per aggiornare `updated_at`
	_, err := db.Exec(`
		CREATE OR REPLACE FUNCTION update_timestamp()
		RETURNS TRIGGER AS $$
		BEGIN
			NEW.updated_at = NOW();
			RETURN NEW;
		END;
		$$ LANGUAGE plpgsql;
	`)
	if err != nil {
		return fmt.Errorf("error creating update_timestamp function: %w", err)
	}

	// Verifica se il trigger esiste prima di crearlo
	var triggerExists bool
	err = db.QueryRow("SELECT EXISTS (SELECT 1 FROM pg_trigger WHERE tgname = 'trigger_update_booking_timestamp')").Scan(&triggerExists)
	if err != nil {
		return fmt.Errorf("error checking if trigger exists: %w", err)
	}

	if !triggerExists {
		_, err = db.Exec(`
			CREATE TRIGGER trigger_update_booking_timestamp
			BEFORE UPDATE ON bookings
			FOR EACH ROW
			EXECUTE FUNCTION update_timestamp();
		`)
		if err != nil {
			return fmt.Errorf("error creating update_timestamp trigger: %w", err)
		}
		fmt.Println("Trigger for updating `updated_at` in bookings created successfully!")
	} else {
		fmt.Println("Trigger `trigger_update_booking_timestamp` already exists.")
	}

	return nil
}

func SeedData() error {
	var count int

	err := db.QueryRow("SELECT COUNT(*) FROM users").Scan(&count)
	if err != nil {
		return err
	}

	if count == 0 { // Inserisce i dati solo se la tabella è vuota
		_, err := db.Exec(`
			INSERT INTO users (username, email, password, role) 
			VALUES ('adminUser', 'admin@example.com', 'hashed_password', 'admin'),
       		('customerUser', 'customer@example.com', 'hashed_password', 'customer');
		`)

		if err != nil {
			return err
		}
		fmt.Println("Initial user data entered successfully!")
	} else {
		fmt.Println("User data already present, no new entries inserted.")
	}

	err = db.QueryRow("SELECT COUNT(*) FROM vehicles").Scan(&count)
	if err != nil {
		return err
	}

	if count == 0 { // Inserisce i dati solo se la tabella è vuota
		_, err := db.Exec(`
			INSERT INTO vehicles (model, category, price, available, location) VALUES 
			('Toyota Corolla', 'Sedan', 50.00, TRUE, 'Milan'),
			('Ford Fiesta', 'Hatchback', 40.00, TRUE, 'Rome'),
			('BMW X5', 'SUV', 90.00, FALSE, 'Naples');
		`)
		if err != nil {
			return err
		}
		fmt.Println("Initial data entered successfully!")
	} else {
		fmt.Println("Data vehicle already present, no new entries inserted.")
	}

	// Seeding for bookings
	err = db.QueryRow("SELECT COUNT(*) FROM bookings").Scan(&count)
	if err != nil {
		return err
	}

	if count == 0 { // Insert data only if the bookings table is empty
		_, err := db.Exec(`
			INSERT INTO bookings (user_id, vehicle_id, start_date, end_date) VALUES 
			(1, 2, '2025-03-01 10:00:00', '2025-03-10 10:00:00'),
			(2, 3, '2025-04-05 08:00:00', '2025-04-12 08:00:00');
		`)
		if err != nil {
			return err
		}
		fmt.Println("Initial booking data inserted successfully!")
	} else {
		fmt.Println("Bookings table already populated, no new entries added.")
	}

	return nil
}

func GetDB() *sql.DB {
	if db == nil {
		log.Fatal("Database connection not initialized! Call InitDB() before using GetDB().")
	}
	return db
}

/*func getEnv(key, fallback string) string {
	if value, exists := os.LookupEnv(key); exists {
		return value
	}
	return fallback
}*/

func getEnv(key string) (string, error) {
	value, exists := os.LookupEnv(key)
	if !exists {
		return "", fmt.Errorf("environment variable %s not set", key)
	}
	return value, nil
}

