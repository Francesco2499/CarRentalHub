package helpers

import (
	"fmt"
	"os"
	"time"

	"github.com/golang-jwt/jwt/v5"
	"github.com/joho/godotenv"
)

var jwtSecretKey []byte

type Claims struct {
	UserID int    `json:"user_id"`
	Role   string `json:"role"`
	jwt.RegisteredClaims
}

func init() {
	err := godotenv.Load()
	if err != nil {
		fmt.Println("Errore nel caricamento del file .env:", err)
	}

	secret := os.Getenv("JWT_SECRET_KEY")
	if secret == "" {
		fmt.Println("JWT_SECRET_KEY non definita nel file .env")
	} else {
		jwtSecretKey = []byte(secret)
	}
}

func GenerateJWT(userID int, role string) (string, error) {

	if jwtSecretKey == nil {
		return "", fmt.Errorf("chiave segreta non disponibile")
	}

	claims := &Claims{
		UserID: userID,
		Role:   role,
		RegisteredClaims: jwt.RegisteredClaims{
			ExpiresAt: jwt.NewNumericDate(time.Now().Add(24 * time.Hour)),
			Issuer:    "CarRentalHub",
		},
	}

	token := jwt.NewWithClaims(jwt.SigningMethodHS256, claims)
	tokenString, err := token.SignedString(jwtSecretKey)
	if err != nil {
		return "", fmt.Errorf("unable to generate token: %v", err)
	}

	return tokenString, nil
}

func ValidateJWT(tokenString string) (*Claims, error) {

	if jwtSecretKey == nil {
		return nil, fmt.Errorf("chiave segreta non disponibile")
	}

	token, err := jwt.ParseWithClaims(tokenString, &Claims{}, func(token *jwt.Token) (interface{}, error) {
		return jwtSecretKey, nil
	})

	if err != nil || !token.Valid {
		return nil, fmt.Errorf("invalid token")
	}

	claims, ok := token.Claims.(*Claims)
	if !ok {
		return nil, fmt.Errorf("token malformed")
	}

	return claims, nil
}
