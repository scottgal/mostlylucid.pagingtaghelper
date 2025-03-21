(function attachPageSizeListener() {
    // Ensure we only attach once if this script is included multiple times
    if (window.__pageSizeListenerAttached) return;
    window.__pageSizeListenerAttached = true;

    // Wait for the DOM to be fully loaded
    document.addEventListener("DOMContentLoaded", () => {
        document.body.addEventListener("change", handlePageSizeChange);
    });

    function handlePageSizeChange(event) {
        // Check if the changed element is a page-size <select> inside .page-size-container
        const select = event.target.closest(".page-size-container select[name='pageSize']");
        if (!select) return;

        const container = select.closest(".page-size-container");
        if (!container) return;

        // Default to "true" if there's no .useHtmx input
        const useHtmx = container.querySelector("input.useHtmx")?.value === "true";
        if (useHtmx) {
            // If using HTMX, we do nothing—HTMX will handle the request
            return;
        }

        // Either use a linkUrl from the container or the current page URL
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

        // Update the pageSize param
        url.searchParams.set("pageSize", select.value);

        // Redirect
        window.location.href = url.toString();
    }
})();