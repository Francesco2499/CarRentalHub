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
	/*err := router.SetTrustedProxies([]string{"127.0.0.1"}) // Imposta solo proxy locali fidati
	if err != nil {
		log.Fatalf("Errore nella configurazione dei proxy: %v", err)
	}*/

	auth := router.Group("/api/v1/auth")
	{
		auth.POST("/register", controllers.Register) // Registrazione utente
		auth.POST("/login", controllers.Login)       // Login utente
	}

	vehicle := router.Group("/api/v1/vehicle")
	vehicle.Use(middleware.AuthMiddleware(""))
	{
		vehicle.GET("/getAll", controllers.GetAllVehicles)
		vehicle.GET("/getById/:id", controllers.GetVehicleById)
		vehicle.GET("/getAllAvailable", controllers.GetAvailableVehicles)
		vehicle.POST("/new", middleware.AuthMiddleware("admin"), controllers.CreateVehicle)
		vehicle.PUT("/update/:id", middleware.AuthMiddleware("admin"), controllers.UpdateVehicle)
		vehicle.DELETE("/delete/:id", middleware.AuthMiddleware("admin"), controllers.DeleteVehicle)
	}

	booking := router.Group("/api/v1/booking")
	booking.Use(middleware.AuthMiddleware("")) // Protezione generale per TUTTE le route
	{
		booking.POST("/new", middleware.AuthMiddleware("admin"), controllers.CreateBooking)                   // Solo admin
		booking.GET("/getAll", controllers.GetAllBookings)                                                    // Accesso per utenti autenticati
		booking.GET("/getById/:id", controllers.GetBookingById)                                               // Accesso per utenti autenticati
		booking.GET("/getByUser/:user_id", middleware.AuthMiddleware("admin"), controllers.GetBookingsByUser) // Solo admin
		booking.PUT("/update/:id", middleware.AuthMiddleware("admin"), controllers.UpdateBooking)             // Solo admin
		booking.DELETE("/delete/:id", middleware.AuthMiddleware("admin"), controllers.DeleteBooking)          // Solo admin
	}

	return router
}
