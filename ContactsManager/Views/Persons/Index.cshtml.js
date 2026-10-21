// JavaScript for Index view
(() => {
    const form = document.getElementById("persons-filter");
    const container = document.getElementById("persons-table");
    const status = document.getElementById("persons-status");
    if (!form || !container || !status) return;

    let controller, timer;

    // Load the table via AJAX
    async function load() {
        controller?.abort(); // a newer request supersedes the old one
        controller = new AbortController();
        const params = new URLSearchParams(new FormData(form));

        container.setAttribute("aria-busy", "true");
        try {
            const res = await fetch(`${container.dataset.url}?${params}`, { signal: controller.signal });
            if (!res.ok) throw new Error(res.status);
            container.innerHTML = await res.text();

            // Update the URL in the address bar to reflect the current filter/sort state, without reloading the page.
            history.replaceState(null, "", params.toString() ? `?${params}` : location.pathname);

            const count = container.querySelector("[data-count]")?.dataset.count ?? "0";
            status.textContent = `${count} ${count === "1" ? "result" : "results"}`;
        } catch (err) {
            if (err.name !== "AbortError")
                status.textContent = "Could not update the table. Please try again.";
        } finally {
            container.removeAttribute("aria-busy");
        }
    }

    // Sort links
    container.addEventListener("click", (e) => {
        const link = e.target.closest("a[data-sort]");
        if (!link || e.button !== 0 || e.metaKey || e.ctrlKey || e.shiftKey || e.altKey) return;
        e.preventDefault();
        const q = new URL(link.href).searchParams;
        form.elements.sortBy.value = q.get("sortBy") ?? "";
        form.elements.sortDirection.value = q.get("sortDirection") ?? "";
        load();
    });

    // Row click: forward to the row's real link. Delegated, so it survives table swaps.
    container.addEventListener("click", (e) => {
        if (e.button !== 0 || e.metaKey || e.ctrlKey || e.shiftKey || e.altKey) return;
        if (e.target.closest("a, button, input, select, textarea, label")) return;  // let real controls work
        if (window.getSelection().toString()) return; // person is selecting text

        const link = e.target.closest("tr.row-link")?.querySelector("a[data-row-link]");
        if (link) location.href = link.href;
    });

    // Clear button
    form.querySelector("[data-clear]")?.addEventListener("click", (e) => {
        if (e.button !== 0 || e.metaKey || e.ctrlKey || e.shiftKey || e.altKey) return;
        e.preventDefault();
        form.elements.searchField.value = "";
        form.elements.searchString.value = "";
        form.elements.sortBy.value = "";
        form.elements.sortDirection.value = "";
        load();
    });

    // Search field typing (300ms debounce)
    form.elements.searchString.addEventListener("input", () => {
        clearTimeout(timer);
        timer = setTimeout(load, 300);
    });

    // Search field change (e.g., from autocomplete)
    form.elements.searchField.addEventListener("change", load);

    // Search button and Enter key
    form.addEventListener("submit", (e) => {
        e.preventDefault();
        clearTimeout(timer);
        load();
    });
})();