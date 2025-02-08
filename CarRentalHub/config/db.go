package config

import (
	"database/sql"
	"fmt"
	_ "github.com/lib/pq"
)

var DB *sql.DB

func SetupDB() error {
	var err error
	connStr := "user=postgres password=yourpassword dbname=carrentalhub sslmode=disable"
	DB, err = sql.Open("postgres", connStr)
	if err != nil {
		return fmt.Errorf("Errore di connessione al database: %v", err)
	}
	if err := DB.Ping(); err != nil {
		return fmt.Errorf("Errore nel ping del database: %v", err)
	}
	return nil
}
