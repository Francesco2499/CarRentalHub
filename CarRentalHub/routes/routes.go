package routes

import (
	"CarRentalHub/controllers"
	"github.com/gin-gonic/gin"
)

// SetupRoutes imposta tutte le rotte per l'applicazione
func SetupRoutes()  *gin.Engine{
	// Gruppo di rotte per l'autenticazione
	router := gin.Default();

	auth := router.Group("/auth")
	{
		auth.POST("/register", controllers.Register)  // Registrazione utente
		auth.POST("/login", controllers.Login)        // Login utente
	}
	return router
}

