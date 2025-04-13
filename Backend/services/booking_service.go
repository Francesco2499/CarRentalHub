package services

import (
	"Backend/cache"
	"Backend/models"
	"Backend/repositories"
	"errors"
	"fmt"
	"log"
)

type BookingRequest struct {
	Booking  models.Booking
	Response chan *models.BookingDTO
	Error    chan error
}

var bookingChannel = make(chan BookingRequest, 100)

func init() {
	log.Println("Avviando Goroutine processBookingRequests()...")
	go processBookingRequests()
}

func processBookingRequests() {
	log.Println("Goroutine processBookingRequests avviata!")
	for req := range bookingChannel {
		log.Println("Ricevuta richiesta di prenotazione, elaborazione in corso...")
		createdBooking, err := CreateBooking(&req.Booking)
		if err != nil {
			req.Error <- err
		} else {
			req.Response <- createdBooking
		}
		log.Println("Prenotazione elaborata e risposta inviata")
	}
}

func CreateBooking(booking *models.Booking) (*models.BookingDTO, error) {
	createdBooking, err := repositories.CreateBooking(booking)
	if err != nil {
		return nil, err
	}

	cache.BookingCache.Invalidate()
	cache.VehicleCache.Invalidate()

	return createdBooking, nil
}

func RequestBooking(booking models.Booking) (*models.BookingDTO, error) {
	log.Println("Inviando richiesta di prenotazione al canale...")
	response := make(chan *models.BookingDTO)
	errorChan := make(chan error)

	bookingChannel <- BookingRequest{Booking: booking, Response: response, Error: errorChan}
	log.Println("Richiesta inserita nel canale, in attesa di risposta...")

	select {
	case res := <-response:
		return res, nil
	case err := <-errorChan:
		return nil, err
	}
}

func GetAllBookings(userID int, isAdmin bool) ([]models.BookingDTO, error) {
	cacheKey := fmt.Sprintf("bookings_user_%d_admin_%t", userID, isAdmin)
	if cachedData, found := cache.BookingCache.Get(cacheKey); found {
		log.Println("all bookings found in cache")
		return cachedData.([]models.BookingDTO), nil
	}

	bookings, err := repositories.GetAllBookings(userID, isAdmin)
	if err != nil {
		return nil, err
	}

	cache.BookingCache.Set(cacheKey, bookings)

	return bookings, nil
}

func UpdateBooking(booking *models.Booking) (string, error) {

	available, err := repositories.IsVehicleAvailable(booking.VehicleID, booking.ID, booking.StartDate, booking.EndDate)
	if err != nil {
		return "Errore nella ricerca di veicoli disponibili!", fmt.Errorf("error checking vehicle availability: %w", err)
	}
	if !available {
		return "Il veicolo non è disponibile", errors.New("vehicle is not available for the selected dates")
	}

	err = repositories.UpdateBooking(booking)
	if err != nil {
		return "Errore nell'aggiornamento della prenotazione", err
	}

	cache.BookingCache.Invalidate()
	cache.VehicleCache.Invalidate()

	return "Aggiornamento effettuato", nil
}

func DeleteBooking(id int) error {
	err := repositories.DeleteBooking(id)
	if err != nil {
		return err
	}

	cache.BookingCache.Invalidate()
	cache.VehicleCache.Invalidate()

	return nil
}
