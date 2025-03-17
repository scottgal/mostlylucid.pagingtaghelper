function toggleTheme() {
    document.documentElement.classList.toggle("dark"); // <html> tag

    localStorage.setItem('theme', document.documentElement.classList.contains('dark') ? 'dark' : 'light');

    updateButtonText();
}

function updateButtonText() {
    const isDark = document.documentElement.classList.contains("dark");
    document.querySelector(".light-mode").classList.toggle("hidden", isDark);
    document.querySelector(".dark-mode").classList.toggle("hidden", !isDark);
}

(function () {
    const theme = localStorage.getItem('theme');
    if (theme === 'dark' || (!('theme' in localStorage) && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
        document.documentElement.classList.add('dark');
    } else {
        document.documentElement.classList.remove('dark');
    }
    updateButtonText();
})();

window.toggleTheme = toggleTheme;