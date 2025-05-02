// Fixed headers with alias
const headers = [
  { name: "Province", alias: "Provinsi" },
  { name: "District", alias: "Kecamatan" },
  { name: "Regency", alias: "Kabupaten/Kota" },
  { name: "Village", alias: "Kelurahan/Desa" },
  { name: "Latitude", alias: "Lintang" },
  { name: "Longitude", alias: "Bujur" },
  { name: "Elevation", alias: "Ketinggian" },
  { name: "Timezone", alias: "Zona Waktu" },
  { name: "Code", alias: "Kode Pos" }
]

/**
 * Export data to CSV file
 * @param {Array|string} data - Array of objects or JSON string
 * @param {string} fileName - Output filename without extension
 * @param {string} [delimiter=','] - Column separator
 */
export function exportToCSV(data, fileName, delimiter = ',') {
  try {
    // Convert string input to array
    if (typeof data === 'string') {
      data = JSON.parse(data)
    }

    // Validate data
    if (!Array.isArray(data) || data.length === 0) {
      console.error('Invalid data format')
      return
    }

    // Process the header row using aliases
    const headerRow = headers.map(header => header.alias).join(delimiter)

    // Process data rows
    const csvRows = [
      headerRow,
      ...data.map(row =>
        headers.map(header => String(row[header.name] ?? '')).join(delimiter)
      )
    ]

    // Create CSV content
    const csvContent = csvRows.join('\r\n')

    // Create download link
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8' })
    const url = URL.createObjectURL(blob)

    const link = document.createElement('a')
    link.href = url
    link.download = `${fileName}.csv`
    link.style.display = 'none'

    document.body.appendChild(link)
    link.click()

    // Cleanup
    setTimeout(() => {
      document.body.removeChild(link)
      URL.revokeObjectURL(url)
    }, 100)
  } catch (error) {
    console.error('Error generating CSV:', error)
  }
}

export function exportToXLS(data, fileName) {
  try {
    // Convert string input to array
    if (typeof data === 'string') {
      data = JSON.parse(data)
    }

    // Validate data
    if (!Array.isArray(data) || data.length === 0) {
      console.error('Invalid or empty data')
      return
    }

    const escapeHtml = value =>
      String(value ?? '')
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')

    const headerRow = headers.map(h => `<th>${escapeHtml(h.alias)}</th>`).join('')
    const dataRows = data.map(row =>
      `<tr>${headers.map(h => `<td>${escapeHtml(row[h.name])}</td>`).join('')}</tr>`
    ).join('')

    const table = `
      <table border="1" cellspacing="0" cellpadding="4">
        <thead><tr>${headerRow}</tr></thead>
        <tbody>${dataRows}</tbody>
      </table>
    `

    const html = `
      <html xmlns:o="urn:schemas-microsoft-com:office:office"
            xmlns:x="urn:schemas-microsoft-com:office:excel"
            xmlns="http://www.w3.org/TR/REC-html40">
        <head>
          <!--[if gte mso 9]>
          <xml>
            <x:ExcelWorkbook>
              <x:ExcelWorksheets>
                <x:ExcelWorksheet>
                  <x:Name>Sheet1</x:Name>
                  <x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions>
                </x:ExcelWorksheet>
              </x:ExcelWorksheets>
            </x:ExcelWorkbook>
          </xml>
          <![endif]-->
          <meta charset="UTF-8">
        </head>
        <body>${table}</body>
      </html>
    `

    const blob = new Blob([html], {
      type: 'application/vnd.ms-excel;charset=utf-8'
    })

    const url = URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `${fileName}.xls`
    link.style.display = 'none'

    document.body.appendChild(link)
    link.click()

    // Cleanup
    setTimeout(() => {
      document.body.removeChild(link)
      URL.revokeObjectURL(url)
    }, 100)
  } catch (error) {
    console.error('Error generating XLS:', error)
  }
}
export function exportToPDF(data, fileName, documentTitle = '', documentTitleStyle = 'font-weight: bold; font-size: 12px; text-align: center', headerStyle = 'font-weight: bold; background: #eee;', cellStyle = '') {
  try {
    // Convert string input to array
    if (typeof data === 'string') {
      data = JSON.parse(data)
    }

    // Validate data
    if (!Array.isArray(data) || data.length === 0) {
      console.error('Invalid data format')
      return
    }

    // Create hidden iframe
    const iframe = document.createElement('iframe')
    iframe.style.visibility = 'hidden'
    iframe.style.position = 'fixed'
    iframe.style.height = '0'
    iframe.style.width = '0'
    iframe.srcdoc = '<html><body></body></html>'
    document.body.appendChild(iframe)

    iframe.onload = () => {
      const doc = iframe.contentDocument || iframe.contentWindow.document

      const escapeHtml = s =>
        String(s ?? '')
          .replace(/&/g, '&amp;')
          .replace(/</g, '&lt;')
          .replace(/>/g, '&gt;')
          .replace(/"/g, '&quot;')

      // Build HTML content
      let html = `<div style="${documentTitleStyle}">${escapeHtml(documentTitle)}</div><br>`
      html += `<table border="1" cellspacing="0" cellpadding="4" style="border-collapse: collapse; width: 100%"><thead><tr>`
      html += headers.map(h => `<th style="${headerStyle}">${escapeHtml(h.alias)}</th>`).join('')
      html += `</tr></thead><tbody>`
      html += data.map(row =>
        `<tr>` + headers.map(h => `<td style="${cellStyle}">${escapeHtml(row[h.name])}</td>`).join('') + `</tr>`
      ).join('')
      html += `</tbody></table>`

      doc.body.innerHTML = html
      iframe.contentWindow.focus()
      iframe.contentWindow.print()

      // Cleanup after print
      setTimeout(() => document.body.removeChild(iframe), 1000)
    }
  } catch (error) {
    console.error('Error generating PDF:', error)
  }
}
