package controllers

import (
	"Backend/models"
	"Backend/services"
	"log"
	"net/http"
	"strconv"

	"github.com/gin-gonic/gin"
)

func GetAllBookings(c *gin.Context) {
	log.Println("Received request to fetch all bookings")
	userID, isAdmin := extractUserFromContext(c)
	bookingsDTO, err := services.GetAllBookings(userID, isAdmin)
	if err != nil {
		log.Printf("Error retrieving bookings: %v", err)
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	log.Printf("Successfully retrieved %d bookings", len(bookingsDTO))
	c.JSON(http.StatusOK, bookingsDTO)
}

/*func GetBookingsByUser(c *gin.Context) {
	log.Println("Received request to fetch bookings by user")

	var userID int
	var err error

	// Controlliamo se abbiamo ricevuto `user_id` come parametro nella richiesta
	userIDParam := c.Param("user_id")
	if userIDParam != "" {
		userID, err = strconv.Atoi(userIDParam)
		if err != nil {
			log.Println("Error: Invalid user ID format")
			c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid user ID"})
			return
		}
	} else {
		// Se `user_id` non è presente, proviamo con `username`
		username := c.Query("username")
		if username == "" {
			log.Println("Error: No user ID or username provided")
			c.JSON(http.StatusBadRequest, gin.H{"error": "You must provide either user_id or username"})
			return
		}

		// Recuperiamo user_id dal database usando lo username
		userID, err = services.GetUserIdByUsername(username)
		if err != nil {
			log.Printf("Error retrieving user ID for username %s: %v", username, err)
			c.JSON(http.StatusNotFound, gin.H{"error": "User not found"})
			return
		}
	}

	// Recuperiamo le prenotazioni dell'utente
	bookingsDTO, err := services.GetBookingsByUser(userID)
	if err != nil {
		log.Printf("Error retrieving bookings: %v", err)
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}

	c.JSON(http.StatusOK, bookingsDTO)
}*/

/*func GetBookingById(c *gin.Context) {
	log.Println("Received request to fetch a booking")
	id, err := strconv.Atoi(c.Param("id"))
	if err != nil {
		log.Println("Error: Invalid booking ID")
		c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid booking ID"})
		return
	}
	userID, isAdmin := extractUserFromContext(c)
	bookingDTO, err := services.GetBookingById(id, userID, isAdmin)
	if err != nil {
		log.Printf("Error retrieving booking: %v", err)
		c.JSON(http.StatusNotFound, gin.H{"error": err.Error()})
		return
	}
	c.JSON(http.StatusOK, bookingDTO)
}*/

func CreateBooking(c *gin.Context) {
	log.Println("Received request to create a new booking")
	userID, _ := extractUserFromContext(c)
	if userID == 0 {
		log.Println("Unauthorized: Missing or invalid token")
		c.JSON(http.StatusUnauthorized, gin.H{"error": "Unauthorized"})
		return
	}

	var booking models.Booking
	if err := c.ShouldBindJSON(&booking); err != nil {
		log.Printf("Invalid request payload: %v", err)
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}

	booking.UserID = userID
	if err := services.CreateBooking(&booking); err != nil {
		log.Printf("Error saving booking: %v", err)
		c.JSON(http.StatusConflict, gin.H{"error": err.Error()})
		return
	}
	log.Println("Booking created successfully")
	c.JSON(http.StatusCreated, booking)
}

func UpdateBooking(c *gin.Context) {
	log.Println("Received request to update a booking")
	id, err := strconv.Atoi(c.Param("id"))
	if err != nil {
		log.Println("Error: Invalid booking ID")
		c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid booking ID"})
		return
	}
	var booking models.Booking
	if err := c.ShouldBindJSON(&booking); err != nil {
		log.Println("Error: Decode JSON failed")
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}
	booking.ID = id
	if err := services.UpdateBooking(&booking); err != nil {
		log.Printf("Error while updating booking ID %d: %v", id, err)
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	log.Printf("Booking with ID %d updated successfully", id)
	c.JSON(http.StatusOK, booking)
}

func DeleteBooking(c *gin.Context) {
	log.Println("Received request to delete a booking")
	id, err := strconv.Atoi(c.Param("id"))
	if err != nil {
		log.Println("Error: Invalid booking ID")
		c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid booking ID"})
		return
	}
	if err := services.DeleteBooking(id); err != nil {
		log.Printf("Error while deleting booking ID %d: %v", id, err)
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	log.Printf("Booking ID %d deleted successfully", id)
	c.Status(http.StatusNoContent)
}

func extractUserFromContext(c *gin.Context) (int, bool) {

	for key, value := range c.Keys {
		log.Printf("Key: %s, Value: %v", key, value)
	}

	// Recupera userID dal contesto della richiesta
	userID, exists := c.Get("userID")
	if !exists {
		return 0, false // Nessun utente autenticato
	}

	// Recupera il ruolo dell'utente (admin o customer)
	role, exists := c.Get("role")
	if !exists {
		return userID.(int), false // Assume che sia un cliente di default
	}

	isAdmin := (role == "admin")

	log.Printf("UserID: %v, isAdmin: %v", userID, isAdmin)
	return userID.(int), isAdmin
}
