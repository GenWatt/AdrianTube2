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

window.setTheme = (theme) => {
    console.log("Setting theme to " + theme);
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