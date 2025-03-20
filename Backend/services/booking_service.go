package services

import (
	"Backend/cache"
	"Backend/models"
	"Backend/repositories"
	"errors"
	"fmt"
	"log"
)

// Struttura per la richiesta di prenotazione
type BookingRequest struct {
	Booking  models.Booking
	Response chan *models.BookingWithVehicleDTO
	Error    chan error
}

// Cache per le prenotazioni (TTL di 30 secondi)
//var bookingCache = cache.NewCache(30 * time.Second)

// Canale globale per la gestione concorrente delle prenotazioni
var bookingChannel = make(chan BookingRequest, 100)

func init() {
	log.Println("Avviando Goroutine processBookingRequests()...")
	go processBookingRequests() // Avvio della Goroutine worker
}

// Goroutine che processa le richieste di prenotazione
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

func CreateBooking(booking *models.Booking) (*models.BookingWithVehicleDTO, error) {
	createdBooking, err := repositories.CreateBooking(booking)
	if err != nil {
		return nil, err
	}

	// Invalida la cache quando viene creata una nuova prenotazione
	cache.BookingCache.Invalidate()
	cache.VehicleCache.Invalidate()

	return createdBooking, nil
}

// Funzione chiamata dal controller per gestire la prenotazione
func RequestBooking(booking models.Booking) (*models.BookingWithVehicleDTO, error) {
	log.Println("Inviando richiesta di prenotazione al canale...")
	response := make(chan *models.BookingWithVehicleDTO)
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

func GetAllBookings(userID int, isAdmin bool) ([]models.BookingWithVehicleDTO, error) {
	cacheKey := fmt.Sprintf("bookings_user_%d_admin_%t", userID, isAdmin)
	// Controllo se il dato è in cache
	if cachedData, found := cache.BookingCache.Get(cacheKey); found {
		log.Println("all bookings found in cache")
		return cachedData.([]models.BookingWithVehicleDTO), nil
	}

	bookings, err := repositories.GetAllBookings(userID, isAdmin)
	if err != nil {
		return nil, err
	}

	// Salvo in cache
	cache.BookingCache.Set(cacheKey, bookings)

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

	// Invalida la cache dopo l'aggiornamento
	cache.BookingCache.Invalidate()
	cache.VehicleCache.Invalidate()

	return "Aggiornamento effettuato", nil
}

func DeleteBooking(id int) error {
	err := repositories.DeleteBooking(id)
	if err != nil {
		return err
	}

	// Invalida la cache dopo la cancellazione
	cache.BookingCache.Invalidate()
	cache.VehicleCache.Invalidate()

	return nil
}
