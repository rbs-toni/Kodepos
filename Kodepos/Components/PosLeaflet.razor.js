let marker

export async function getGeo() {
    try {
        const response = await fetch('https://get.geojs.io/v1/ip.json')
        if (!response.ok) throw new Error('Failed to fetch IP')

        const { ip } = await response.json()

        const geoResponse = await fetch(`https://get.geojs.io/v1/ip/geo/${ip}.json`)
        if (!geoResponse.ok) throw new Error('Failed to fetch geo data')

        const geo = await geoResponse.json()
        return geo
    } catch (error) {
        console.error('Error fetching geo data:', error)
        return null
    }
}

export function initMap(loc) {
    const map = L.map('map').setView(loc, 13)

    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(map)

    marker = L.marker(loc).addTo(map);

    return map
}

export function setView(map, latLng) {
    map.flyTo(latLng, 13)
    marker.setLatLng(latLng)
}