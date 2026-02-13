window.searchOverlay = (function () {
    let overlayOpen = false;
    let cloned = null;
    let originalSelector = null;
    
    function getRect(selector) {
        const el = document.querySelector(selector);
        if (!el) {
            return null;
        }
        return el.getBoundingClientRect();
    }
    
    async function openFromSelector(sourceSelector, targetSelector) {
        const src = document.querySelector(sourceSelector);
        const target = document.querySelector(targetSelector);
        if (!src || !target) {
            document.documentElement.classList.add('f-search-overlay-open');
            overlayOpen = true;
            return;
        }
        
        const srcRect = src.getBoundingClientRect();
        const targetRect = target.getBoundingClientRect();
        const body = document.body;

        cloned = src.cloneNode(true);
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
        body.appendChild(cloned);

        // compute transform to center (we will expand to match target)
        const centerX = (window.innerWidth - targetRect.width) / 2;
        const centerY = (window.innerHeight - targetRect.height) / 2;

        // target transform relative to cloned's current pos
        const dx = centerX - srcRect.left;
        const dy = centerY - srcRect.top;
        const scaleX = targetRect.width / srcRect.width;
        const scaleY = targetRect.height / srcRect.height;
        const scale = Math.min(scaleX, scaleY) * 1.03;

        // trigger overlay show
        document.documentElement.classList.add('f-search-overlay-open');

        // next frame: animate
        requestAnimationFrame(() => {
            cloned.style.transform = `translate(${dx}px, ${dy}px) scale(${scale})`;
            cloned.style.opacity = '0.98';
        });

        overlayOpen = true;

        // after animation, remove cloned
        setTimeout(() => {
            if (cloned) {
                cloned.parentElement.removeChild(cloned);
                cloned = null;
            }
        }, 260);
    }

    function closeOverlay() {
        // simply remove class to hide overlay; you could implement reverse clone animation if you want
        document.documentElement.classList.remove('f-search-overlay-open');
        overlayOpen = false;
    }

    function focusInput(targetSelector) {
        const t = document.querySelector(targetSelector);
        if (t) {
            t.focus();
            // position caret at end
            const val = t.value;
            t.value = '';
            t.value = val;
        }
    }

    // helper for Blazor getElementId - optional
    function getElementId(el) {
        return el && el.id ? el.id : null;
    }

    return {
        openFromSelector: openFromSelector,
        closeOverlay: closeOverlay,
        focusInput: focusInput,
        getElementId: getElementId
    };
})();