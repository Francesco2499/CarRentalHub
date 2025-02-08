package controllers

import (
	"CarRentalHub/services"
	"CarRentalHub/models"
	"net/http"
	"github.com/gin-gonic/gin"
)

// Register gestisce la registrazione di un nuovo utente.
func Register(c *gin.Context) {
	var user models.User

	// Legge i dati del corpo della richiesta
	if err := c.ShouldBindJSON(&user); err != nil {
		// Se la richiesta non è valida, restituisce errore
		c.JSON(http.StatusBadRequest, gin.H{"error": "Dati non validi"})
		return
	}

	// Chiamata al servizio di registrazione
	newUser, err := services.RegisterUser(user)
	if err != nil {
		// Gestisce eventuali errori durante la registrazione
		c.JSON(http.StatusInternalServerError, gin.H{"error": "Errore durante la registrazione"})
		return
	}

	// Restituisce una risposta di successo con i dati dell'utente
	c.JSON(http.StatusOK, gin.H{"message": "Utente registrato con successo", "user": newUser})
}

// Login gestisce il login degli utenti esistenti.
func Login(c *gin.Context) {
	var user models.User

	// Legge i dati del corpo della richiesta
	if err := c.ShouldBindJSON(&user); err != nil {
		// Se i dati non sono corretti, restituisce errore
		c.JSON(http.StatusBadRequest, gin.H{"error": "Dati non validi"})
		return
	}

	// Chiamata al servizio di autenticazione
	token, err := services.AuthenticateUser(user.Username, user.Password)
	if err != nil {
		// Se l'autenticazione fallisce, restituisce errore
		c.JSON(http.StatusUnauthorized, gin.H{"error": "Autenticazione fallita"})
		return
	}

	// Restituisce il token JWT se l'autenticazione ha avuto successo
	c.JSON(http.StatusOK, gin.H{"message": "Login riuscito", "token": token})
}
