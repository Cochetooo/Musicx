window.genreView = {
    scrollHorizontal: (element, direction, amount) => {
        if (!element) return;
        const delta = direction === 'left' ? -amount : amount;
        element.scrollBy({ left: delta, behavior: 'smooth' });
    },
    getHorizontalState: (element) => {
        if (!element) return { canScrollLeft: false, canScrollRight: false };
        const maxScroll = element.scrollWidth - element.clientWidth;
        return {
            canScrollLeft: element.scrollLeft > 4,
            canScrollRight: element.scrollLeft < maxScroll - 4
        };
    }
};