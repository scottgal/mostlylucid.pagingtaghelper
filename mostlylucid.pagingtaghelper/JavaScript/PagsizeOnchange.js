(function attachPageSizeListener() {
    if (window.__pageSizeListenerAttached) return;
    window.__pageSizeListenerAttached = true;

    document.addEventListener("DOMContentLoaded", () => {
        document.body.addEventListener("change", handlePageSizeChange);
    });

    function handlePageSizeChange(event) {
        const select = event.target.closest(".page-size-container select[name='pageSize']");
        if (!select) return;

        const container = select.closest(".page-size-container");
        if (!container) return;

        const useHtmx = container.querySelector("input.useHtmx")?.value === "true";
        if (useHtmx) {
            return; // Let HTMX handle it
        }

        const linkUrl = container.querySelector("input.linkUrl")?.value || window.location.href;
        const url = new URL(linkUrl, window.location.origin);

        // Copy existing query params from current location
        const existingParams = new URLSearchParams(window.location.search);
        for (const [key, value] of existingParams.entries()) {
            url.searchParams.set(key, value);
        }

        // If user picked the same pageSize as what's already in the URL, do nothing
        if (url.searchParams.get("pageSize") === select.value) {
            return;
        }

        // 🔥 Update the pageSize param
        url.searchParams.set("pageSize", select.value);

        // 🔥 Reset page number if it exists
        if (url.searchParams.has('page')) {
            url.searchParams.set('page', '1');
        }

        // Redirect to the new URL
        window.location.href = url.toString();
    }
})();