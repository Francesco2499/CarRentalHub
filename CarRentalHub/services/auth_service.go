package services

import (
	"CarRentalHub/config"
	"CarRentalHub/helpers"
	"CarRentalHub/models"
	"fmt"
	"regexp"
	"golang.org/x/crypto/bcrypt"
)


func isEmail(s string) bool {
    re := regexp.MustCompile(`^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$`)
    return re.MatchString(s)
}


// RegisterUser registra un nuovo utente nel database utilizzando una transazione
func RegisterUser(user models.User) (*models.User, string, error) {
	// Inizia una nuova transazione
	tx, err := config.DB.Begin()
	if err != nil {
		//log.Println("Errore nell'iniziare la transazione:", err)
		return nil, "Error", err
	}

	// Assicura che la transazione venga annullata in caso di errore
	defer func() {
		if err != nil {
			tx.Rollback() // Annulla la transazione in caso di errore
		}
	}()

	// Verifica se l'email è già in uso
	var count int
	err = tx.QueryRow(`SELECT COUNT(*) FROM users WHERE email = $1`, user.Email).Scan(&count)
	if err != nil {
		return nil, "Error", err
	}
	if count > 0 {
		return nil, "Email già in uso", fmt.Errorf("email già in uso")
	}

	// Verifica se lo username è già in uso
	err = tx.QueryRow(`SELECT COUNT(*) FROM users WHERE username = $1`, user.Username).Scan(&count)
	if err != nil {
		return nil, "Error", err
	}
	if count > 0 {
		return nil, "Username già in uso", fmt.Errorf("username già in uso")
	}

	// Hash della password
	hashedPassword, err := bcrypt.GenerateFromPassword([]byte(user.Password), bcrypt.DefaultCost)
	if err != nil {
		//log.Println("Errore nel hashing della password:", err)
		return nil, "Error", err
	}

	// Prepara la query per inserire l'utente nel database
	query := `INSERT INTO users (username, email, password, role) VALUES ($1, $2, $3, $4) RETURNING id`

	// Esegui la query all'interno della transazione
	var userID int

	if user.Role == "" {
		user.Role = "customer"
	}

	err = tx.QueryRow(query, user.Username, user.Email, string(hashedPassword), user.Role).Scan(&userID)
	if err != nil {
		//log.Println("Errore nell'inserimento dell'utente:", err)
		return nil, "Error", err
	}

	// Imposta l'ID dell'utente nel modello
	user.ID = userID

	// Se tutto è andato bene, conferma la transazione
	err = tx.Commit()
	if err != nil {
		return nil, "Error", err
	}

	// Restituisci l'utente con l'ID generato
	return &user, "Registrazione effettuata!", nil
}

// AuthenticateUser esegue l'autenticazione dell'utente e restituisce un token JWT.
func AuthenticateUser(username, password string) (string, string, error) {
	var user models.User
	var query string

	// Verifica se il valore passato è un'email o uno username
	if isEmail(username) {
		// Se è un'email, esegui la query per cercare l'utente per email
		query = `SELECT * FROM users WHERE email = $1`
	} else {
		// Se è uno username, esegui la query per cercare l'utente per username
		query = `SELECT * role FROM users WHERE username = $1`
	}
	err := config.DB.QueryRow(query, username).Scan(&user.ID, &user.Username, &user.Password, &user.Role)
	if err != nil {
		return "Utente non trovato", "", fmt.Errorf("error: %v", err)
	}

	// Verifica la password
	err = bcrypt.CompareHashAndPassword([]byte(user.Password), []byte(password))
	if err != nil {
		return "Password non valida", "", fmt.Errorf("password error")
	}

	// Genera il token JWT
	token, err := helpers.GenerateJWT(user.ID, user.Role)
	if err != nil {
		return "Errore nella generazione del token", "", fmt.Errorf("errore nella generazione del token: %v", err)
	}

	return "Login effettuato", token, nil
}
