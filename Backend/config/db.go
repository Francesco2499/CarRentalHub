package config

/*import (
	"database/sql"
	"fmt"
	_ "github.com/lib/pq"
)

var DB *sql.DB

func SetupDB() error {
	var err error
	connStr := "user=admin password=yourpassword dbname=carrentalhub sslmode=disable"
	DB, err = sql.Open("postgres", connStr)
	if err != nil {
		return fmt.Errorf("Errore di connessione al database: %v", err)
	}
	if err := DB.Ping(); err != nil {
		return fmt.Errorf("Errore nel ping del database: %v", err)
	}
	return nil
}*/

import (
	"database/sql"
	"fmt"
	"log"
	"os"

	_ "github.com/lib/pq"
)

var db *sql.DB

func InitDB() (*sql.DB, error) {
	// Usa variabili d'ambiente per la connessione al database
	dsn := fmt.Sprintf(
		"host=%s user=%s password=%s dbname=%s sslmode=disable",
		getEnv("DB_HOST", "localhost"),
		getEnv("DB_USER", "postgres"),
		getEnv("DB_PASSWORD", "password"),
		getEnv("DB_NAME", "CarRentalHub"),
	)

	var err error
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
	}

	for _, table := range tables {
		_, err := db.Exec(table)
		if err != nil {
			return err
		}
	}

	fmt.Println("Tables created/verified successfully!")
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

	return nil
}

func GetDB() *sql.DB {
	if db == nil {
		log.Fatal("Database connection not initialized! Call InitDB() before using GetDB().")
	}
	return db
}

func getEnv(key, fallback string) string {
	if value, exists := os.LookupEnv(key); exists {
		return value
	}
	return fallback
}
