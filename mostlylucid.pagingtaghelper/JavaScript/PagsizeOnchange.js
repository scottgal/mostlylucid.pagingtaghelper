document.addEventListener("DOMContentLoaded", function () {
    document.body.addEventListener("change", function (event) {
        const selectElement = event.target.closest(".pager-container select[name='pageSize']");
        if (!selectElement) return;

        const pagerContainer = selectElement.closest(".pager-container");
        const useHtmxInput = pagerContainer.querySelector("input.useHtmx");
        const useHtmx = useHtmxInput ? useHtmxInput.value === "true" : true; // default to true

        if (!useHtmx) {
            const pageInput = pagerContainer.querySelector("[name='page']");
            const searchInput = pagerContainer.querySelector("[name='search']");

            const page = pageInput ? pageInput.value : "1";
            const search = searchInput ? searchInput.value : "";
            const pageSize = selectElement.value;
            const linkUrl =  pagerContainer.querySelector("input.linkUrl").value ?? "";
            
            const url = new URL(linkUrl, window.location.origin);
            url.searchParams.set("page", page);
            url.searchParams.set("pageSize", pageSize);

            if (search) {
                url.searchParams.set("search", search);
            }

            window.location.href = url.toString();
        }
    });
});