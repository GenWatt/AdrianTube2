window.getScrollHeight = (id) => {
    return document.getElementById(id).scrollHeight;
};

window.getScrollTop = (id) => {
    return document.getElementById(id).scrollTop;
};

window.getClientHeight = (id) => {
    return document.getElementById(id).clientHeight;
};

window.getElementHeight = (id) => {
    console.log(id, document.getElementById(id).offsetHeight);
    return document.getElementById(id).offsetHeight;
}

window.getElementWidth = (id) => {
    return document.getElementById(id).offsetWidth;
}

window.getElementOffsetTop = (id) => {
    return document.getElementById(id).offsetTop;
}

window.getWindowScrollTop = () => {
    return window.scrollY;
}

window.getWindowHeight = () => {
    return window.innerHeight;
}

window.getWindowScrollHeight = () => {
    return document.body.scrollHeight;
}

window.triggerClick = (elt) => elt.click();

window.triggerClickById = (id) => { 
    document.getElementById(id).click();
}

window.setFocus = (id) => {
    document.getElementById(id).focus();
}

window.setOverflowAuto = (id) => {
    document.getElementById(id).style.overflow = "auto";
}

window.setOverflowHidden = (id) => {
    document.getElementById(id).style.overflow = "hidden";
}

window.setTheme = (theme) => {
    document.documentElement.setAttribute('data-bs-theme', theme);
}

window.loadVideo = (id) => {
    const video = document.getElementById(id)
    if (video) {
        video.load();
    } else {
        console.error("Video not found");
    }
}

window.setTooltip = (id, title, placement = 'top') => {
    tippy(`#${id}`, {
        content: title,
        placement,
        theme: 'primary'
    });
}

const keys = {37: 1, 38: 1, 39: 1, 40: 1};

function preventDefault(e) {
  e.preventDefault();
}

function preventDefaultForScrollKeys(e) {
  if (keys[e.keyCode]) {
    preventDefault(e);
    return false;
  }
}

const supportsPassive = false;

try {
    window.addEventListener("test", null, Object.defineProperty({}, 'passive', {
        get: function () { supportsPassive = true; } 
    }));
} catch(e) {}

const wheelOpt = supportsPassive ? { passive: false } : false;
const wheelEvent = 'onwheel' in document.createElement('div') ? 'wheel' : 'mousewheel';

function disableScroll(id) {
    const element = document.getElementById(id);

    if (!element) {
        return;
    }

    element.addEventListener('DOMMouseScroll', preventDefault, false); // older FF
    element.addEventListener(wheelEvent, preventDefault, wheelOpt); // modern desktop
    element.addEventListener('touchmove', preventDefault, wheelOpt); // mobile
    element.addEventListener('keydown', preventDefaultForScrollKeys, false);
}

function enableScroll(id) {
    const element = document.getElementById(id);

    if (!element) {
        return;
    }
    
    element.removeEventListener('DOMMouseScroll', preventDefault, false);
    element.removeEventListener(wheelEvent, preventDefault, wheelOpt); 
    element.removeEventListener('touchmove', preventDefault, wheelOpt);
    element.removeEventListener('keydown', preventDefaultForScrollKeys, false);
}

class ScrollHelper {
    static dotNetHelper;

    static setDotNetHelper(value) {
        ScrollHelper.dotNetHelper = value;
    }

    static onScrollHandler(e) {
        ScrollHelper.dotNetHelper.invokeMethodAsync("LoadMore", e);
    }

    static async onScroll() {
        window.addEventListener('scroll', ScrollHelper.onScrollHandler);
    }

    static async offScroll() {
        window.removeEventListener('scroll', ScrollHelper.onScrollHandler);
    }
}

window.ScrollHelper = ScrollHelper;