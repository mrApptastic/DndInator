// Development service worker - minimal caching for development
self.addEventListener('install', event => {
    console.log('Service worker installed (dev)');
    self.skipWaiting();
});

self.addEventListener('activate', event => {
    console.log('Service worker activated (dev)');
    return self.clients.claim();
});

self.addEventListener('fetch', event => {
    // In development, just let the browser handle the fetch
    event.respondWith(fetch(event.request));
});
