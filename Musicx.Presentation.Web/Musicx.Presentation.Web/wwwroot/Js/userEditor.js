export function setupSettingsSpy(containerSelector, dotNetRef) {
    const container = document.querySelector(containerSelector);
    if (!container) return;

    const sections = Array.from(container.querySelectorAll("section[id]"));
    if (!sections.length) return;

    const updateActive = () => {
        const containerTop = container.getBoundingClientRect().top;
        let activeSection = sections[0].id;

        for (const section of sections) {
            const sectionTop = section.getBoundingClientRect().top - containerTop;
            if (sectionTop <= 80) {
                activeSection = section.id;
            }
        }

        dotNetRef.invokeMethodAsync("SetActiveSettingsCategory", activeSection);
    };

    container.addEventListener("scroll", updateActive, { passive: true });
    updateActive();
}

export function scrollToSettingsSection(containerSelector, sectionId) {
    const container = document.querySelector(containerSelector);
    if (!container) return;

    const section = container.querySelector(`#${sectionId}`);
    if (!section) return;

    section.scrollIntoView({ behavior: "smooth", block: "start" });
}