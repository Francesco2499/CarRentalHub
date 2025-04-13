package cache

import "time"

// Cache TTL globale
var ttl = 30 * time.Second

var BookingCache = NewCache(ttl)
var VehicleCache = NewCache(ttl)
