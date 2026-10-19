document.addEventListener("click", (event) => {
    const button = event.target.closest("[data-add-set]");
    if (!button) return;

    const exercise = button.closest("[data-exercise-entry]");
    const rows = exercise.querySelectorAll(".set-row");
    const nextIndex = rows.length;
    const maxSets = Number(button.dataset.maxSets);
    if (nextIndex >= maxSets) {
        button.disabled = true;
        return;
    }

    const row = rows[rows.length - 1].cloneNode(true);
    row.querySelectorAll("[name]").forEach((element) => {
        element.name = element.name.replace(/\.Sets\[\d+\]/, `.Sets[${nextIndex}]`);
        if (element.id) element.id = element.id.replace(/Sets_\d+__/g, `Sets_${nextIndex}__`);
        if (element.name.endsWith(".SetNumber")) {
            element.value = String(nextIndex + 1);
        } else if (element.type !== "hidden") {
            element.value = "";
        }
    });

    row.querySelectorAll("[for]").forEach((element) => {
        element.htmlFor = element.htmlFor.replace(/Sets_\d+__/g, `Sets_${nextIndex}__`);
    });
    row.querySelectorAll("[data-valmsg-for]").forEach((element) => {
        element.dataset.valmsgFor = element.dataset.valmsgFor.replace(/\.Sets\[\d+\]/, `.Sets[${nextIndex}]`);
        element.textContent = "";
    });
    row.querySelector(".set-number").textContent = String(nextIndex + 1);
    rows[rows.length - 1].after(row);

    if (nextIndex + 1 >= maxSets) button.disabled = true;
});
