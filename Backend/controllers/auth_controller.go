package controllers

import (
	"Backend/models"
	"Backend/services"
	"net/http"

	"github.com/gin-gonic/gin"
)

// Register gestisce la registrazione di un nuovo utente.
func Register(c *gin.Context) {
	var user models.User

	// Legge i dati del corpo della richiesta
	if err := c.ShouldBindJSON(&user); err != nil {
		// Se la richiesta non è valida, restituisce errore
		c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid data"})
		return
	}

	// Chiamata al servizio di registrazione
	newUser, message, err := services.RegisterUser(user)
	if err != nil {
		// Gestisce eventuali errori durante la registrazione
		c.JSON(http.StatusInternalServerError, gin.H{"message": message})
		return
	}

	// Restituisce una risposta di successo con i dati dell'utente
	c.JSON(http.StatusOK, gin.H{"message": message, "user": newUser})
}

// Login gestisce il login degli utenti esistenti.
func Login(c *gin.Context) {
	var user models.User

	// Legge i dati del corpo della richiesta
	if err := c.ShouldBindJSON(&user); err != nil {
		// Se i dati non sono corretti, restituisce errore
		c.JSON(http.StatusBadRequest, gin.H{"message": "Invalid data"})
		return
	}

	// Chiamata al servizio di autenticazione
	message, token, err := services.AuthenticateUser(user.Username, user.Password)
	if err != nil {
		// Se l'autenticazione fallisce, restituisce errore
		c.JSON(http.StatusOK, gin.H{"message": message})
		return
	}

	// Restituisce il token JWT se l'autenticazione ha avuto successo
	c.JSON(http.StatusOK, gin.H{"message": message, "token": token})
}
