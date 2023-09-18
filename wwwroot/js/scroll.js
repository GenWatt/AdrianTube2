
class MobileNavHandler {
    constructor(id, navId) {
        this.lastScrollY = 0;
        this.id = id;
        this.navId = navId;
    }

    scrollHandler = (e) => {
        const currentScrollY = e.target.scrollTop;

        if (currentScrollY > this.lastScrollY) {
            this.scrollDownHandler();
        } else {
            this.scrollUpHandler();
        }
   
        this.lastScrollY = currentScrollY;
    }

    scrollDownHandler = () => {
        const nav = document.getElementById(this.navId);
        if (!nav) return
        console.log("scroll down");
        nav.style.bottom = "-100px";
    }

    scrollUpHandler = () => {
        const nav = document.getElementById(this.navId);
        if (!nav) return

        nav.style.bottom = "0";
    }

    setScrollEvent = () => {
        const element = document.getElementById(this.id);

        if (!element) {
            console.error("Element not found");
            return;
        }

        element.addEventListener('scroll', this.scrollHandler);
    }

    removeScrollEvent = () => {
        const element = document.getElementById(this.id);

        if (!element) {
            console.error("Element not found");
            return;
        }

        element.removeEventListener('scroll', this.scrollHandler);
    }
}


function setMobileNavHandler(id, navId) {
    const mobileNavHandler = new MobileNavHandler(id, navId);
    mobileNavHandler.setScrollEvent();
    window.mobileNavHandler = mobileNavHandler;
}