// Open and close dialogs using data attributes

document.addEventListener("click", (e) => {
    // Open
    const opener = e.target.closest("[data-dialog-open]");
    if (opener) {
        document.getElementById(opener.dataset.dialogOpen)?.showModal();
        return;
    }

    // Close: Cancel and X buttons
    const closer = e.target.closest("[data-dialog-close]");
    if (closer) {
        closer.closest("dialog")?.close();
        return;
    }
});