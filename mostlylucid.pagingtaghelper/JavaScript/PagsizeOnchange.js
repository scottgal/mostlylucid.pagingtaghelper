document.addEventListener("DOMContentLoaded", function () {
    document.body.addEventListener("change", function (event) {
        const selectElement = event.target.closest(".page-size-container select[name='pageSize']");
        if (!selectElement) return;

        const pagerContainer = selectElement.closest(".page-size-container");
        const useHtmxInput = pagerContainer.querySelector("input.useHtmx");
        const useHtmx = useHtmxInput ? useHtmxInput.value === "true" : true; // default to true

        if (!useHtmx) {
            const linkUrl = pagerContainer.querySelector("input.linkUrl")?.value || window.location.href;
            const url = new URL(linkUrl, window.location.origin);
            // Preserve all existing query parameters
            const currentParams = new URLSearchParams(window.location.search);

           
            // Ensure all other parameters from the current query string are included
            currentParams.forEach((value, key) => {
                    url.searchParams.set(key, value);
                
            });
            url.searchParams.delete("pageSize");
            url.searchParams.set("pageSize", selectElement.value);
            window.location.href = url.toString();
        }
    });
});