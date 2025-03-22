function setTheme(theme) {
    const html = document.documentElement;
    const isDark = theme === 'dark';

    html.classList.toggle('dark', isDark);
    localStorage.setItem('theme', theme);

    // Update icons or labels
    document.querySelector('.light-mode')?.classList.toggle('hidden', isDark);
    document.querySelector('.dark-mode')?.classList.toggle('hidden', !isDark);
}

function toggleTheme() {
    const isDark = document.documentElement.classList.contains('dark');
    setTheme(isDark ? 'light' : 'dark');
    applyHighlightTheme();
    document.querySelectorAll('pre code').forEach((el) => {
        el.removeAttribute('data-highlighted');
        el.classList.remove('hljs');
        hljs.highlightElement(el);
    });
}

function applyHighlightTheme() {
    const prefersDark = document.documentElement.classList.contains("dark");

    document.getElementById("hljs-theme-dark").disabled = !prefersDark;
    document.getElementById("hljs-theme-light").disabled = prefersDark;
}

function initTheme() {
    const savedTheme = localStorage.getItem('theme');
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;

    if (savedTheme === 'dark' || (!savedTheme && prefersDark)) {
        setTheme('dark');
    } else {
        setTheme('light');
    }
    applyHighlightTheme();
}

document.addEventListener('DOMContentLoaded', () => {
    initTheme();
    document.body.addEventListener('htmx:afterSettle', function(evt) {
        document.querySelectorAll('pre code').forEach((el) => {
            hljs.highlightElement(el);
        });
    });
    
    // Highlight.js setup
    document.querySelectorAll('pre code').forEach((el) => {
        hljs.highlightElement(el);
    });
});

// Expose to global scope
window.toggleTheme = toggleTheme;