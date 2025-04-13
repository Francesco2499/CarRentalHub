package cache

import (
	"sync"
	"time"
)

type cacheEntry struct {
	data      interface{}
	expiredAt time.Time
}

type Cache struct {
	store sync.Map
	ttl   time.Duration
}

func NewCache(ttl time.Duration) *Cache {
	c := &Cache{ttl: ttl}
	go c.cleanupExpiredEntries()
	return c
}

func (c *Cache) Set(key string, value interface{}) {
	entry := cacheEntry{
		data:      value,
		expiredAt: time.Now().Add(c.ttl),
	}
	c.store.Store(key, entry)
}

func (c *Cache) Get(key string) (interface{}, bool) {
	value, exists := c.store.Load(key)
	if !exists {
		return nil, false
	}

	entry := value.(cacheEntry)
	if time.Now().After(entry.expiredAt) {
		c.store.Delete(key)
		return nil, false
	}
	return entry.data, true
}

func (c *Cache) cleanupExpiredEntries() {
	for {
		time.Sleep(c.ttl / 2)
		c.store.Range(func(key, value interface{}) bool {
			entry := value.(cacheEntry)
			if time.Now().After(entry.expiredAt) {
				c.store.Delete(key)
			}
			return true
		})
	}
}

func (c *Cache) Invalidate() {
	c.store = sync.Map{}
}
