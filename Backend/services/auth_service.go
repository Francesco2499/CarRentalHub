package services

import (
	"Backend/helpers"
	"Backend/models"
	"Backend/repositories"
	"fmt"
	"regexp"

	"golang.org/x/crypto/bcrypt"
)

func isEmail(s string) bool {
	re := regexp.MustCompile(`^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$`)
	return re.MatchString(s)
}

func VerifyPassword(hashedPassword, password string) error {
	return bcrypt.CompareHashAndPassword([]byte(hashedPassword), []byte(password))
}

// RegisterUser registra un nuovo utente utilizzando il repository
func RegisterUser(user models.User) (*models.User, string, error) {
	// Verifica se l'email è già in uso
	if _, err := repositories.FindByEmail(user.Email); err == nil {
		return nil, "Email already in use", fmt.Errorf("email already in use")
	}

	// Verifica se lo username è già in uso
	if _, err := repositories.FindByUsername(user.Username); err == nil {
		return nil, "Username already in use", fmt.Errorf("Username already in use")
	}

	// Se il ruolo non è specificato, assegna il valore di default
	if user.Role == "" {
		user.Role = "customer"
	}

	// Salva il nuovo utente nel database
	if err := repositories.SaveUser(&user); err != nil {
		return nil, "Error", err
	}

	return &user, "Registration done!", nil
}

// AuthenticateUser esegue l'autenticazione dell'utente e restituisce un token JWT.
func AuthenticateUser(username, password string) (string, string, bool, error) {
	var user *models.User
	var err error
	var isAdmin bool

	// Verifica se il valore passato è un'email o uno username
	if isEmail(username) {
		user, err = repositories.FindByEmail(username)
	} else {
		user, err = repositories.FindByUsername(username)
	}

	if err != nil {
		return "User not found", "", false, fmt.Errorf("error: %v", err)
	}

	// Verifica la password
	if err := VerifyPassword(user.Password, password); err != nil {
		return "Invalid password", "", false, fmt.Errorf("password error")
	}

	// Genera il token JWT
	token, err := helpers.GenerateJWT(user.ID, user.Role)
	if err != nil {
		return "Error genereating token", "", false, fmt.Errorf("error genereating token: %v", err)
	}

	isAdmin = user.Role == "admin"

	return "Login done", token, isAdmin, nil
}

func GetUserIdByUsername(username string) (int, error) {

	user, err := repositories.FindByUsername(username)
	if err != nil {
		return 0, fmt.Errorf("user %s not found: %w", username, err)
	}
	return user.ID, nil
}
