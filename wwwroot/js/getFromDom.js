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

class ScrollHelper {
    static dotNetHelper;

    static setDotNetHelper(value) {
        ScrollHelper.dotNetHelper = value;
    }

    static onScrollHandler(e) {
        ScrollHelper.dotNetHelper.invokeMethodAsync("OnScroll", e);
    }

    static async onScroll() {
        window.addEventListener('scroll', ScrollHelper.onScrollHandler);
    }

    static async offScroll() {
        window.removeEventListener('scroll', ScrollHelper.onScrollHandler);
    }
}

window.ScrollHelper = ScrollHelper;