package routes

import (
	"Backend/controllers"
	"Backend/middleware"

	"github.com/gin-gonic/gin"
)

// SetupRoutes imposta tutte le rotte per l'applicazione
func SetupRoutes() *gin.Engine {
	// Gruppo di rotte per l'autenticazione
	router := gin.Default()
	api := router.Group("/api/v1")

	auth := api.Group("/auth")
	{
		auth.POST("/register", controllers.Register) // Registrazione utente
		auth.POST("/login", controllers.Login)       // Login utente
	}

	user := api.Group("/user")
	user.Use(middleware.AuthMiddleware("")) // Tutti gli utenti autenticati
	{
		user.PUT("/update/me", controllers.UpdateCurrentUser)
		user.DELETE("/delete/me", controllers.DeleteCurrentUser)
	}

	vehicle := api.Group("/vehicle")
	vehicle.Use(middleware.AuthMiddleware(""))
	{
		vehicle.GET("/getAll", controllers.GetAllVehicles)
		vehicle.GET("/getById/:id", controllers.GetVehicleById)
		vehicle.GET("/getAllAvailable", controllers.GetAvailableVehicles)
		vehicle.POST("/new", middleware.AuthMiddleware("admin"), controllers.CreateVehicle)
		vehicle.PUT("/update/:id", middleware.AuthMiddleware("admin"), controllers.UpdateVehicle)
		vehicle.DELETE("/delete/:id", middleware.AuthMiddleware("admin"), controllers.DeleteVehicle)
	}

	booking := api.Group("/booking")
	booking.Use(middleware.AuthMiddleware("")) // Protezione generale per TUTTE le route
	{
		booking.POST("/new", controllers.CreateBooking)
		booking.GET("/getAll", controllers.GetAllBookings) // Accesso per utenti autenticati
		booking.PUT("/update/:id", middleware.AuthMiddleware("admin"), controllers.UpdateBooking)
		booking.DELETE("/delete/:id", middleware.AuthMiddleware("admin"), controllers.DeleteBooking)
	}

	return router
}
