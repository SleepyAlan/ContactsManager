// Dropdown menus close on:
// - outside click
// - on Escape
// - after choosing an item

document.addEventListener("click", (e) => {
    document.querySelectorAll("details.menu[open]").forEach((menu) => {
        if (!menu.contains(e.target) || e.target.closest(".menu-list a")) menu.open = false;
    });
});

document.addEventListener("keydown", (e) => {
    if (e.key !== "Escape") return;
    const menu = document.querySelector("details.menu[open]");
    if (menu) {
        menu.open = false;
        menu.querySelector("summary").focus();   // return focus to the trigger
    }
});