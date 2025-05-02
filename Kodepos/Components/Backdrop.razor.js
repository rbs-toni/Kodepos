/**
 * Initialize the global click event handler
 * @param {any} dotNetHelper
 * @param {any} id
 */
export function overlayInitialize(dotNetHelper, containerId, id) {

  if (!document.backdropData) {
    document.backdropData = {}
  }

  if (document.backdropData[id]) {
    return
  }

  // Store the data
  document.backdropData[id] = {
    // Click event handler
    clickHandler: async function (event) {
      const isInsideContainer = isClickInsideContainer(event, document.getElementById(containerId))
      const isInsideExcludedElement = !!document.getElementById(id) && isClickInsideContainer(event, document.getElementById(id))
      if (isInsideContainer && !isInsideExcludedElement) {
        dotNetHelper.invokeMethodAsync('OnCloseInteractiveAsync', event)
      }
    }
  }

  // Let the user click on the container (containerId or the entire document)
  document.addEventListener('click', document.backdropData[id].clickHandler)
}

/**
 * Dispose the global click event handler
 */
export function overlayDispose(id) {
  if (document.backdropData[id]) {

    // Remove the event listener
    document.removeEventListener('click', document.backdropData[id].clickHandler)

    // Remove the data
    document.backdropData[id] = null
    delete document.backdropData[id]
  }
}

/**
 * Determines whether a mouse click event occurred inside a specific HTML element.
 */
function isClickInsideContainer(event, container) {
  if (!!container) {
    const rect = container.getBoundingClientRect()

    return (
      event.clientX >= rect.left &&
      event.clientX <= rect.right &&
      event.clientY >= rect.top &&
      event.clientY <= rect.bottom
    )
  }

  // Default is true
  return true
}
