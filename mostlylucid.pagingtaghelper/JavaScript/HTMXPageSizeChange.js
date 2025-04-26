(() => {
    if (window.__pageSizeListenerAdded) return;

    document.addEventListener('htmx:configRequest', event => {
        const { elt } = event.detail;
        if (elt?.matches('[name="pageSize"]')) {
            const params = new URLSearchParams(window.location.search);

            // Update the pageSize parameter
            params.set('pageSize', elt.value);

            // 🔥 Reset page to 1 if it exists
            if (params.has('page')) {
                params.set('page', '1');
            }

            const paramObj = Object.fromEntries(params.entries());
            event.detail.parameters = paramObj;

            const pageSizeEvent = new CustomEvent('pagesize:updated', {
                detail: {
                    params: paramObj,
                    elt,
                },
            });

            document.dispatchEvent(pageSizeEvent);
        }
    });

    window.__pageSizeListenerAdded = true;
})();