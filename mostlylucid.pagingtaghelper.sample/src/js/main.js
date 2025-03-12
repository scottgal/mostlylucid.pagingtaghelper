// Function to toggle the theme and store preference
function toggleTheme() {
    if (document.documentElement.classList.contains("dark")) {
        document.documentElement.classList.remove("dark");
        localStorage.setItem("theme", "light");
    } else {
        document.documentElement.classList.add("dark");
        localStorage.setItem("theme", "dark");
    }
    updateButtonText(); // Update button icon
}

// Function to update button text/icon
function updateButtonText() {
    document.querySelector(".light-mode").classList.toggle("hidden", document.documentElement.classList.contains("dark"));
    document.querySelector(".dark-mode").classList.toggle("hidden", !document.documentElement.classList.contains("dark"));
}

// Apply theme on page load
(function () {
    if (localStorage.getItem("theme") === "dark" ||
        (!localStorage.getItem("theme") && window.matchMedia("(prefers-color-scheme: dark)").matches)) {
        document.documentElement.classList.add("dark");
    } else {
        document.documentElement.classList.remove("dark");
    }
    updateButtonText();
})();

// Ensure Webpack doesn't remove toggleTheme() by attaching it to window
window.toggleTheme = toggleTheme;