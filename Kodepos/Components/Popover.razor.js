import { computePosition, autoUpdate } from '../lib/floating-ui/floating-ui.min.js'

export function init(referenceId, popperId) {
  const reference = document.getElementById(referenceId)
  const popper = document.getElementById(popperId)

  function updatePosition() {
    computePosition(reference, popper, {
      placement: 'bottom-end',
      strategy: 'fixed'
    }).then(({ x, y }) => {
      Object.assign(popper.style, {
        left: `${x}px`,
        top: `${y}px`
      })
    })
  }
  const cleanup = autoUpdate(reference, popper, updatePosition)
}
