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

	auth := router.Group("/api/v1/auth")
	{
		auth.POST("/register", controllers.Register) // Registrazione utente
		auth.POST("/login", controllers.Login)       // Login utente
	}

	vehicle := router.Group("/api/v1/vehicle")
	{
		vehicle.GET("/getAll", controllers.GetAllVehicles)
		vehicle.GET("/getById/:id", controllers.GetVehicleById)
	}

	vehicleProtected := router.Group("/api/v1/vehicle")
	vehicleProtected.Use(middleware.AuthMiddleware(""))
	{
		vehicleProtected.POST("/new", middleware.AuthMiddleware("admin"), controllers.CreateVehicle)
		vehicleProtected.PUT("/update/:id", middleware.AuthMiddleware("admin"), controllers.UpdateVehicle)
		vehicleProtected.DELETE("/delete/:id", middleware.AuthMiddleware("admin"), controllers.DeleteVehicle)
	}

	return router
}
