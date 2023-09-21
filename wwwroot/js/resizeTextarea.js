window.resizeTextArea = function (id) {
    var textarea = document.getElementById(id);
    textarea.style.height = "auto";
    textarea.style.height = textarea.scrollHeight + "px";
}

class ToastPositionHelper {
    constructor (dotnetHelper) {
        this.dotnetHelper = dotnetHelper;
    }

    handleResize = (e) => {
        if (window.innerWidth < 768) {
            this.dotnetHelper.invokeMethodAsync('SetToastPosition', 'top-center');
        } else {
            this.dotnetHelper.invokeMethodAsync('SetToastPosition', 'bottom-right');
        }
    }

    setResizeEvent = () => {
        window.addEventListener('resize',  this.handleResize);
    }

    removeResizeEvent = () => {
        window.removeEventListener('resize', this.handleResize);
    }
}

function setToastPositionHelper(dotnetHelper) {
    const toastPositionHelper = new ToastPositionHelper(dotnetHelper);
    window.toastPositionHelper = toastPositionHelper;
    toastPositionHelper.setResizeEvent();
    toastPositionHelper.handleResize();
}