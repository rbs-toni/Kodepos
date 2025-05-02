function processData(data, dotNetRef, method) {
    const provinces = new Set()
    const regencies = new Set()
    const districts = new Set()
    const villages = new Set()

    for (const item of data) {
        provinces.add(item.province)
        regencies.add(item.regency)
        districts.add(item.district)
        villages.add(item.village)
    }

    dotNetRef.invokeMethodAsync(method, {
        ProvinceCount: provinces.size,
        RegencyCount: regencies.size,
        DistrictCount: districts.size,
        VillageCount: villages.size
    })
}

async function openDatabase() {
    return new Promise((resolve, reject) => {
        const request = indexedDB.open("PostcodeDB", 1)
        request.onupgradeneeded = (event) => {
            const db = event.target.result
            if (!db.objectStoreNames.contains("cache")) {
                db.createObjectStore("cache")
            }
        }
        request.onsuccess = () => resolve(request.result)
        request.onerror = () => reject(request.error)
    })
}

async function saveToDB(db, key, value) {
    return new Promise((resolve, reject) => {
        const tx = db.transaction("cache", "readwrite")
        tx.objectStore("cache").put(value, key)
        tx.oncomplete = () => resolve()
        tx.onerror = () => reject(tx.error)
    })
}

async function getFromDB(db, key) {
    return new Promise((resolve, reject) => {
        const tx = db.transaction("cache", "readonly")
        const request = tx.objectStore("cache").get(key)
        request.onsuccess = () => resolve(request.result || null)
        request.onerror = () => reject(request.error)
    })
}

export const postcodeRepository = {
    async searchWithPagination(keyword, page = 1, pageSize = 50, sortOptions) {
        const { sortBy, dir } = sortOptions ?? {}
        const db = await openDatabase()
        const cachedData = await getFromDB(db, "postcode_data")

        // Filter data only if keyword is provided
        const filteredData = keyword && keyword.trim()
            ? cachedData.filter(item =>
                item.province.toLowerCase().includes(keyword.toLowerCase()) ||
                item.regency.toLowerCase().includes(keyword.toLowerCase()) ||
                item.district.toLowerCase().includes(keyword.toLowerCase()) ||
                item.village.toLowerCase().includes(keyword.toLowerCase())
            )
            : cachedData

        const sortedData = sortBy && dir
            ? [...filteredData].sort((a, b) => {
                const aVal = (a[sortBy] ?? '').toString().toLowerCase()
                const bVal = (b[sortBy] ?? '').toString().toLowerCase()

                if (aVal < bVal) return dir === 'asc' ? -1 : 1
                if (aVal > bVal) return dir === 'asc' ? 1 : -1
                return 0
            })
            : filteredData

        const totalItems = filteredData.length
        const totalPages = Math.ceil(totalItems / pageSize)
        const currentPage = Math.max(1, Math.min(page, totalPages))
        const hasNext = currentPage < totalPages
        const hasPrevious = currentPage > 1

        const startIndex = (currentPage - 1) * pageSize
        const paginatedData = sortedData.slice(startIndex, startIndex + pageSize)

        return {
            totalItems,
            totalPages,
            hasNext,
            hasPrevious,
            items: paginatedData,
            page: currentPage,
            pageSize
        }
    }
}

export async function getStats(dotNetRef, method) {
    const db = await openDatabase()
    const cachedData = await getFromDB(db, "postcode_data")

    if (cachedData) {
        console.log("Loading data from IndexedDB cache...")
        processData(cachedData, dotNetRef, method)
        return
    }

    console.log("Fetching JSON from server...")
    const response = await fetch('/data/kodepos.json')
    if (!response.ok) throw new Error("Failed to fetch JSON")

    const data = await response.json()
    console.log(`Finished loading ${data.length} records`)

    await saveToDB(db, "postcode_data", data)
    processData(data, dotNetRef, method)
}