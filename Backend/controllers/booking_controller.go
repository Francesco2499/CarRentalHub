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
	bookings, err := services.GetAllBookings(userID, isAdmin)
	if err != nil {
		log.Printf("Error retrieving bookings: %v", err)
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	log.Printf("Successfully retrieved %d bookings", len(bookings))
	c.JSON(http.StatusOK, bookings)
}

func GetBookingsByUser(c *gin.Context) {
	log.Println("Received request to fetch bookings by user")
	userID, err := strconv.Atoi(c.Param("user_id"))
	if err != nil {
		log.Println("Error: Invalid user ID")
		c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid user ID"})
		return
	}
	bookings, err := services.GetBookingsByUser(userID)
	if err != nil {
		log.Printf("Error retrieving bookings: %v", err)
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	c.JSON(http.StatusOK, bookings)
}

func GetBookingById(c *gin.Context) {
	log.Println("Received request to fetch a booking")
	id, err := strconv.Atoi(c.Param("id"))
	if err != nil {
		log.Println("Error: Invalid booking ID")
		c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid booking ID"})
		return
	}
	userID, isAdmin := extractUserFromContext(c)
	booking, err := services.GetBookingById(id, userID, isAdmin)
	if err != nil {
		log.Printf("Error retrieving booking: %v", err)
		c.JSON(http.StatusNotFound, gin.H{"error": err.Error()})
		return
	}
	c.JSON(http.StatusOK, booking)
}

func CreateBooking(c *gin.Context) {
	log.Println("Received request to create a new booking")
	var booking models.Booking
	if err := c.ShouldBindJSON(&booking); err != nil {
		log.Printf("Invalid request payload: %v", err)
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}
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
	// Recupera userID dal contesto della richiesta
	userID, exists := c.Get("userID")
	if !exists {
		return 0, false // Nessun utente autenticato
	}

	// Recupera il ruolo dell'utente (admin o customer)
	isAdmin, exists := c.Get("isAdmin")
	if !exists {
		return userID.(int), false // Assume che sia un cliente di default
	}

	return userID.(int), isAdmin.(bool)
}
