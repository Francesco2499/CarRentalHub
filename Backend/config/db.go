package config

import (
	"database/sql"
	"fmt"
	"log"
	"os"

	"github.com/joho/godotenv"
	_ "github.com/lib/pq"
)

var db *sql.DB

func InitDB() (*sql.DB, error) {

	// Carica il file .env (se esiste)
	/*if err := godotenv.Load(); err != nil {
		log.Println("Warning: No .env file found, using system environment variables")
	}*/

	if os.Getenv("APP_ENV") != "prod" {
		if err := godotenv.Load(); err != nil {
			log.Println("Warning: No .env file found, using system environment variables")
		}
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
		`CREATE TABLE IF NOT EXISTS car_showrooms (
			id SERIAL PRIMARY KEY,
			name VARCHAR(255) NOT NULL,
			location VARCHAR(255) NOT NULL,
			latitude DECIMAL(10,6) NOT NULL,
			longitude DECIMAL(10,6) NOT NULL
		);`,

		/*`CREATE TABLE IF NOT EXISTS vehicles (
			id SERIAL PRIMARY KEY,
			model VARCHAR(255) NOT NULL,
			category VARCHAR(255) NOT NULL,
			price DECIMAL(10,2) NOT NULL,
			location VARCHAR(255) NOT NULL,
			latitude DECIMAL(10,6) NOT NULL,
			longitude DECIMAL(10,6) NOT NULL
		);`,*/
		`CREATE TABLE IF NOT EXISTS vehicles (
			id SERIAL PRIMARY KEY,
			model VARCHAR(255) NOT NULL,
			category VARCHAR(255) NOT NULL,
			price DECIMAL(10,2) NOT NULL,
			car_showroom_id INT NOT NULL REFERENCES car_showrooms(id) ON DELETE CASCADE
		);`,

		`CREATE TABLE IF NOT EXISTS users (
			id SERIAL PRIMARY KEY,
			username VARCHAR(50) UNIQUE NOT NULL,
			email VARCHAR(100) UNIQUE NOT NULL,
			password TEXT NOT NULL,
			region VARCHAR(50) NOT NULL,
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
			NEW.updated_at = NOW() AT TIME ZONE 'UTC';
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

	// Seeding per utenti
	err := db.QueryRow("SELECT COUNT(*) FROM users").Scan(&count)
	if err != nil {
		return err
	}

	if count == 0 {
		_, err := db.Exec(`
			INSERT INTO users (username, email, password, region, role) 
			VALUES 
				('testadmin', 'admin@example.com', 'e0e6097a6f8af07daf5fc7244336ba37133713a8fc7345c36d667dfa513fabaa', 'Sicilia', 'admin'),
       			('testcustomer', 'testcustomer@example.com', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'Piemonte', 'customer'),
				('testuser', 'testuser@example.com', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'Lombardia', 'customer'),
				('mariorossi', 'mario.rossi@example.com', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'Lazio', 'customer'),
				('giovannibianchi', 'giovanni.bianchi@example.com', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'Sicilia', 'customer'),
				('francescaneri', 'francesca.neri@example.com', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'Emilia-Romagna', 'customer'),
				('lucalongo', 'luca.longo@example.com', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'Toscana', 'customer'),
				('martinamartini', 'martina.martini@example.com', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'Sicilia', 'customer');
				`)

		//Password per test -> ADMIN: securepassword Customer:password123

		if err != nil {
			return err
		}
		fmt.Println("Initial user data entered successfully!")
	} else {
		fmt.Println("User data already present, no new entries inserted.")
	}

	// Seeding per veicoli
	err = db.QueryRow("SELECT COUNT(*) FROM car_showrooms").Scan(&count)
	if err != nil {
		return err
	}

	if count == 0 { // Inserisce i dati solo se la tabella è vuota
		_, err := db.Exec(`
		INSERT INTO car_showrooms (name, location, latitude, longitude) VALUES 
			('CarShowroom CT', 'Catania', 37.502361, 15.087372),
			('CarShowroom RM', 'Roma', 41.906567, 12.477145),
			('CarShowroom NA', 'Napoli', 40.842349, 14.244212),
			('CarShowroom BO', 'Bologna', 44.498144, 11.339746),
			('CarShowroom FI', 'Firenze', 43.771376, 11.252829),
			('CarShowroom MI', 'Milano', 45.435701, 9.228306),
			('CarShowroom PA', 'Palermo', 38.105647, 13.366171),
			('CarShowroom GE', 'Genova', 44.406386, 8.936147),
			('CarShowroom TO', 'Torino', 45.109337, 7.645222),
			('CarShowroom VR', 'Verona', 45.420713, 10.976738),
			('CarShowroom BA', 'Bari', 41.116128, 16.877928),
			('CarShowroom CA', 'Cagliari', 39.220787, 9.125712);
		`)
		if err != nil {
			return err
		}
		fmt.Println("Initial carshowroom data entered successfully!")
	} else {
		fmt.Println("CarShowroom data already present, no new entries inserted.")
	}

	///////////////////////////////////////////////////////////////////////////////

	// Seeding per veicoli
	err = db.QueryRow("SELECT COUNT(*) FROM vehicles").Scan(&count)
	if err != nil {
		return err
	}

	if count == 0 { // Inserisce i dati solo se la tabella è vuota
		_, err := db.Exec(`
		INSERT INTO vehicles (model, category, price, car_showroom_id) VALUES 
			('Toyota Corolla', 'Berlina', 50.00, 1),
			('Ford Fiesta', 'Hatchback', 40.00, 2),
			('BMW X5', 'SUV', 90.00, 3),
			('Audi A4', 'Berlina', 75.00, 1),
			('Fiat Panda', 'City Car', 30.00, 4),
			('Mercedes-Benz GLC', 'SUV', 120.00, 5),
			('Tesla Model 3', 'Berlina Elettrica', 150.00, 6),
			('Jeep Wrangler', 'SUV', 100.00, 7),
			('Peugeot 208', 'Hatchback', 35.00, 8),
			('Alfa Romeo Giulietta', 'Compact', 55.00, 1),
			('Volkswagen Golf', 'Hatchback', 45.00, 6),
			('Renault Clio', 'Hatchback', 38.00, 9),
			('Honda CR-V', 'SUV', 85.00, 7),
			('Nissan Qashqai', 'SUV', 80.00, 2),
			('Skoda Octavia', 'Berlina', 60.00, 10),
			('Dacia Duster', 'SUV', 65.00, 11),
			('Maserati Levante', 'SUV', 200.00, 3),
			('Citroen C3', 'City Car', 33.00, 7),
			('Hyundai Tucson', 'SUV', 75.00, 8),
			('Opel Corsa', 'Hatchback', 36.00, 12),
			('Suzuki Jimny', 'Off-road', 70.00, 10),
			('Kia Sportage', 'SUV', 82.00, 9),
			('Toyota Yaris', 'City Car', 32.00, 11),
			('Volvo XC60', 'SUV', 95.00, 1);
		`)
		if err != nil {
			return err
		}
		fmt.Println("Initial vehicle data entered successfully!")
	} else {
		fmt.Println("Vehicle data already present, no new entries inserted.")
	}

	// Seeding per prenotazioni
	err = db.QueryRow("SELECT COUNT(*) FROM bookings").Scan(&count)
	if err != nil {
		return err
	}

	if count == 0 { // Inserisce i dati solo se la tabella è vuota
		_, err := db.Exec(`
		INSERT INTO bookings (user_id, vehicle_id, start_date, end_date) VALUES 
			(4, 5, '2024-01-10', '2024-01-15'),
			(6, 12, '2024-01-20', '2024-01-27'),
			(5, 10, '2024-02-05', '2024-02-12'),
			(3, 6, '2024-02-18', '2024-02-25'),
			(6, 7, '2024-03-01', '2024-03-07'),
			(3, 1, '2024-03-05', '2024-03-10'),
			(4, 3, '2024-06-10', '2024-06-15'),
			(4, 3, '2024-06-25', '2024-06-30'),
			(2, 8, '2024-07-05', '2024-07-12'),
			(2, 5, '2025-01-10', '2025-01-14'),
			(4, 10, '2025-01-15', '2025-01-20'),
			(6, 7, '2025-02-01', '2025-02-05'),
			(3, 8, '2025-02-01', '2025-02-05'),
			(4, 14, '2025-02-15', '2025-02-20'),
			(2, 12, '2025-02-10', '2025-02-14'),
			(6, 5, '2025-03-01', '2025-03-07'),
			(5, 6, '2025-03-03', '2025-03-05'),
			(5, 7, '2025-03-05', '2025-03-10'),
			(3, 10, '2025-03-01', '2025-03-05'),
			(4, 8, '2025-02-06', '2025-02-10'),
			(6, 3, '2025-03-10', '2025-03-15'),
			(5, 6, '2025-03-15', '2025-03-20'),
			(2, 7, '2025-04-01', '2025-04-05'),
			(4, 5, '2025-04-10', '2025-04-15'),
			(6, 10, '2025-06-01', '2025-06-05'),
			(5, 12, '2025-07-05', '2025-07-10'),
			(3, 3, '2025-08-10', '2025-08-15'),
			(4, 7, '2025-08-18', '2025-08-25'),
			(2, 14, '2025-09-01', '2025-09-07');
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
