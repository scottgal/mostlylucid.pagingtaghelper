/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./Views/**/*.cshtml"],
  safelist: ["dark", "light"],
  darkMode: "class",
  theme: {
    extend: {},
  },
  plugins: [  require('daisyui')],
}