export function openFromSelector(sourceSelector, targetSelector) {
    const src = document.querySelector(sourceSelector);
    const target = document.querySelector(targetSelector);

    if (!src || !target) {
        document.documentElement.classList.add('f-search-overlay-open');
        return;
    }

    const srcRect = src.getBoundingClientRect();
    const targetRect = target.getBoundingClientRect();

    const cloned = src.cloneNode(true);
    cloned.style.backgroundColor = 'gray';
    cloned.style.position = 'fixed';
    cloned.style.left = `${srcRect.left}px`;
    cloned.style.top = `${srcRect.top}px`;
    cloned.style.width = `${srcRect.width}px`;
    cloned.style.height = `${srcRect.height}px`;
    cloned.style.margin = '0';
    cloned.style.zIndex = '99999';
    cloned.style.transition = 'transform 220ms cubic-bezier(.25,.1,.25,1), opacity 180ms ease-out';
    cloned.style.willChange = 'transform, opacity';
    document.body.appendChild(cloned);

    const centerX = (window.innerWidth - targetRect.width) / 2;
    const centerY = (window.innerHeight - targetRect.height) / 2;

    const dx = centerX - srcRect.left;
    const dy = centerY - srcRect.top;
    const scaleX = targetRect.width / srcRect.width;
    const scaleY = targetRect.height / srcRect.height;
    const scale = Math.min(scaleX, scaleY) * 1.03;

    document.documentElement.classList.add('f-search-overlay-open');

    requestAnimationFrame(() => {
        cloned.style.transform = `translate(${dx}px, ${dy}px) scale(${scale})`;
        cloned.style.opacity = '0.98';
    });

    setTimeout(() => {
        cloned.remove();
    }, 260);
}

export function closeOverlay() {
    document.documentElement.classList.remove('f-search-overlay-open');
}

export function focusInput(targetSelector) {
    const t = document.querySelector(targetSelector);
    if (!t) {
        return;
    }

    t.focus();
    const val = t.value;
    t.value = '';
    t.value = val;
}