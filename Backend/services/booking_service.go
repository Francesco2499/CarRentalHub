package services

import (
	"Backend/cache"
	"Backend/models"
	"Backend/repositories"
	"errors"
	"fmt"
	"log"
	"time"
)

// Struttura per la richiesta di prenotazione
type BookingRequest struct {
	Booking  models.Booking
	Response chan error // Canale per la risposta
}

// Cache per le prenotazioni (TTL di 30 secondi)
var bookingCache = cache.NewCache(30 * time.Second)

// Canale globale per la gestione concorrente delle prenotazioni
var bookingChannel = make(chan BookingRequest, 100)

func init() {
	go processBookingRequests() // Avvio della Goroutine worker
}

// Goroutine che processa le richieste di prenotazione
func processBookingRequests() {
	for req := range bookingChannel {
		err := CreateBooking(&req.Booking)
		req.Response <- err // Invia la risposta sul canale
	}
}

func CreateBooking(booking *models.Booking) error {
	err := repositories.CreateBooking(booking)
	if err != nil {
		return err
	}
	// Invalida la cache quando viene creata una nuova prenotazione
	bookingCache.Invalidate()

	return nil
}

// Funzione chiamata dal controller per gestire la prenotazione
func RequestBooking(booking models.Booking) error {
	response := make(chan error)
	bookingChannel <- BookingRequest{Booking: booking, Response: response}
	return <-response // Attende la risposta dalla Goroutine
}

func GetAllBookings(userID int, isAdmin bool) ([]models.BookingWithVehicleDTO, error) {
	cacheKey := fmt.Sprintf("bookings_user_%d_admin_%t", userID, isAdmin)
	// Controllo se il dato è in cache
	if cachedData, found := bookingCache.Get(cacheKey); found {
		log.Println("all bookings found in cache")
		return cachedData.([]models.BookingWithVehicleDTO), nil
	}

	bookings, err := repositories.GetAllBookings(userID, isAdmin)
	if err != nil {
		return nil, err
	}

	// Salvo in cache
	bookingCache.Set(cacheKey, bookings)

	return bookings, nil
}

/*func GetBookingsByUser(userID int) ([]models.Booking, error) {
	cacheKey := fmt.Sprintf("bookings_user_%d", userID)

	// Controllo se il dato è in cache
	if cachedData, found := bookingCache.Get(cacheKey); found {
		log.Println("bookings for user found in cache")
		return cachedData.([]models.Booking), nil
	}

	// Recupero dal database
	bookings, err := repositories.GetBookingsByUser(userID)
	if err != nil {
		return nil, err
	}

	// Salvo in cache
	bookingCache.Set(cacheKey, bookings)

	return bookings, nil
}*/

/*func GetBookingById(id int, userID int, isAdmin bool) (*models.Booking, error) {
	cacheKey := fmt.Sprintf("booking_id_%d_user_%d_admin_%t", id, userID, isAdmin)

	// Controllo se il dato è in cache
	if cachedData, found := bookingCache.Get(cacheKey); found {
		log.Println("bookings by id found in cache")
		return cachedData.(*models.Booking), nil
	}

	// Recupero dal database
	booking, err := repositories.GetBookingById(id, userID, isAdmin)
	if err != nil {
		return nil, err
	}

	// Salvo in cache
	bookingCache.Set(cacheKey, booking)

	return booking, nil
}*/

func UpdateBooking(booking *models.Booking) error {

	available, err := repositories.IsVehicleAvailable(booking.VehicleID, booking.ID, booking.StartDate, booking.EndDate)
	if err != nil {
		return fmt.Errorf("Error checking vehicle availability: %w", err)
	}
	if !available {
		return errors.New("Vehicle is not available for the selected dates")
	}

	err = repositories.UpdateBooking(booking)
	if err != nil {
		return err
	}

	// Invalida la cache dopo l'aggiornamento
	bookingCache.Invalidate()

	return nil
}

func DeleteBooking(id int) error {
	err := repositories.DeleteBooking(id)
	if err != nil {
		return err
	}

	// Invalida la cache dopo la cancellazione
	bookingCache.Invalidate()
	return nil
}
