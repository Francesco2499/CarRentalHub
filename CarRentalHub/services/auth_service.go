package services

import (
	"CarRentalHub/config"
	"CarRentalHub/helpers"
	"CarRentalHub/models"
	"fmt"
	"log"

	"golang.org/x/crypto/bcrypt"
)

// RegisterUser registra un nuovo utente nel database utilizzando una transazione
func RegisterUser(user models.User) (*models.User, error) {
	// Inizia una nuova transazione
	tx, err := config.DB.Begin()
	if err != nil {
		//log.Println("Errore nell'iniziare la transazione:", err)
		return nil, err
	}

	// Assicura che la transazione venga annullata in caso di errore
	defer func() {
		if err != nil {
			tx.Rollback() // Annulla la transazione in caso di errore
		}
	}()

	// Hash della password
	hashedPassword, err := bcrypt.GenerateFromPassword([]byte(user.Password), bcrypt.DefaultCost)
	if err != nil {
		//log.Println("Errore nel hashing della password:", err)
		return nil, err
	}

	// Prepara la query per inserire l'utente nel database
	query := `INSERT INTO users (username, password, role) VALUES ($1, $2, $3) RETURNING id`

	// Esegui la query all'interno della transazione
	var userID int
	err = tx.QueryRow(query, user.Username, string(hashedPassword), user.Role).Scan(&userID)
	if err != nil {
		//log.Println("Errore nell'inserimento dell'utente:", err)
		return nil, err
	}

	// Imposta l'ID dell'utente nel modello
	user.ID = userID

	// Se tutto è andato bene, conferma la transazione
	err = tx.Commit()
	if err != nil {
		log.Println("Errore nel confermare la transazione:", err)
		return nil, err
	}

	// Restituisci l'utente con l'ID generato
	return &user, nil
}

// AuthenticateUser esegue l'autenticazione dell'utente e restituisce un token JWT.
func AuthenticateUser(username, password string) (string, error) {
	var user models.User
	// Recupera l'utente dal database
	query := `SELECT id, username, password, role FROM users WHERE username = $1`
	err := config.DB.QueryRow(query, username).Scan(&user.ID, &user.Username, &user.Password, &user.Role)
	if err != nil {
		return "", fmt.Errorf("utente non trovato: %v", err)
	}

	// Verifica la password
	err = bcrypt.CompareHashAndPassword([]byte(user.Password), []byte(password))
	if err != nil {
		return "", fmt.Errorf("password non valida")
	}

	// Genera il token JWT
	token, err := helpers.GenerateJWT(user.ID, user.Role)
	if err != nil {
		return "", fmt.Errorf("errore nella generazione del token: %v", err)
	}

	return token, nil
}
