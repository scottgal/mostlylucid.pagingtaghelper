/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.cshtml",
    "./Areas/**/*.cshtml",
    "./src/**/*.js"
  ],
  safelist: [
    "dark",
    "light",
    // Keep DaisyUI component classes that might be dynamically generated
    "btn", "btn-sm", "btn-active", "btn-disabled", "btn-primary", "btn-accent", "btn-outline",
    "join", "join-item",
    "badge", "badge-ghost", "badge-sm",
    "select", "select-primary", "select-sm",
    "modal", "modal-box",
    "htmx-indicator"
  ],
  darkMode: "class",
  theme: {
    extend: {},
  },
  plugins: [require('daisyui')],
  // Optimize for production
  ...(process.env.NODE_ENV === 'production' && {
    experimental: {
      optimizeUniversalDefaults: true
    }
  })
}