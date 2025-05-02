const CACHE_KEY = 'postcode_stats_cache'

async function getStats(url, dotNetRef, method) {
    let cachedData = await loadFromCache()
    if (cachedData) {
        dotNetRef.invokeMethodAsync(method, cachedData)
        return
    }

    let response = await fetch(url)
    let reader = response.body.getReader()
    let decoder = new TextDecoder()
    let chunks = ''
    let progress = {
        ProvinceCount: 0,
        DistrictCount: 0,
        RegencyCount: 0,
        VillageCount: 0
    }

    let reportInterval = setInterval(() => {
        dotNetRef.invokeMethodAsync(method, progress)
    }, 10)

    while (true) {
        let { done, value } = await reader.read()
        if (done) break
        chunks += decoder.decode(value, { stream: true })
    }

    clearInterval(reportInterval)

    let jsonData = JSON.parse(chunks)
    progress.ProvinceCount = jsonData.Provinces.length
    progress.DistrictCount = jsonData.Districts.length
    progress.RegencyCount = jsonData.Regencies.length
    progress.VillageCount = jsonData.Villages.length

    await saveToCache(progress)
    dotNetRef.invokeMethodAsync(method, progress)
}

async function saveToCache(data) {
    localStorage.setItem(CACHE_KEY, JSON.stringify(data))
}

async function loadFromCache() {
    let cached = localStorage.getItem(CACHE_KEY)
    return cached ? JSON.parse(cached) : null
}

window.getStats = getStats