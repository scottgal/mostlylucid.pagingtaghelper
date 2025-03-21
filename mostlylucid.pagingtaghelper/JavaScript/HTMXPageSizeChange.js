(() => {
    if (window.__pageSizeListenerAdded) return;

    document.addEventListener('htmx:configRequest', event => {
        const { elt } = event.detail;
        if (elt?.matches('[name="pageSize"]')) {
            const params = new URLSearchParams(window.location.search);
            params.set('pageSize', elt.value); // This will update or add the pageSize param
            event.detail.parameters = Object.fromEntries(params.entries());
        }
    });
    window.__pageSizeListenerAdded = true;
})();