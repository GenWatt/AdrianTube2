class MediaPlayer {
    constructor(video) {
        this.video = video;
    }

    play() {
        if (this.video.paused) {
            this.video.muted = true;
            this.video.play();
            this.video.muted = false;
        }
    }

    pause() {
        if (!this.video.paused) {
            this.video.pause();
        }
    }

    togglePlay() {
        if (this.video.paused) {
            this.play();
        } else {
            this.pause();
        }
    }

    mute() {
        this.video.muted = true;
    }

    unmute() {
        this.video.muted = false;
    }

    setVolume(volume) {
        this.video.volume = volume;
    }

    skip(seconds) {
        this.video.currentTime += seconds;
    }

    back(seconds) {
        this.video.currentTime -= seconds;
    }
}

window.setMediaPlayer = function (videoId) {
    const video = document.getElementById(videoId);

    if (!video) {
        console.error("Video not found");
        return;
    }

    if (window.mediaPlayer) {
        window.mediaPlayer.video = video;
        return
    }

    window.mediaPlayer = new MediaPlayer(video);
}

function debounce (func, timeout = 300) {
    let timer;
    return (...args) => {
      clearTimeout(timer);
      timer = setTimeout(() => { func.apply(this, args); }, timeout);
    };
}

// const debouncedScroll = debounce(scrollDownHandler, 2000)

function scrollToVideo(id) {
    const elt = document.getElementById(id);
    const container = document.getElementById("main-container");
    
    if (elt) {
        container.scrollTo({
            top: elt.offsetTop - 100,
            behavior: 'smooth'
        });
    } else {
        console.error("Element not found");
    }
}
  
  let isScrolling = false; // Flag to track scrolling state
  
  async function scrollDownHandler(dotNetHelper) {
    if (dotNetHelper) {
      await dotNetHelper.invokeMethodAsync('ScrollDownHandler');
      scrollToVideo(await dotNetHelper.invokeMethodAsync('GetCurrentShortId'));
    }
  }

  async function scrollUpHandler(dotNetHelper) {
    if (dotNetHelper) {
        await dotNetHelper.invokeMethodAsync('ScrollUpHandler');
        scrollToVideo(await dotNetHelper.invokeMethodAsync('GetCurrentShortId'));
    }
}

let timer = null;
let dotNetHelper = null;
  
  async function scrollingDownOrUp(e) {
    const delta = Math.sign(e.deltaY);;

    if (timer) {
        clearTimeout(timer);
    }

    timer = setTimeout(() => {
        isScrolling = false;
    }, 500);

    if (delta > 0 && !isScrolling) {
        isScrolling = true;
        scrollDownHandler(dotNetHelper);
    } else {
      if (!isScrolling) {
        isScrolling = true;
        await scrollUpHandler(dotNetHelper);
      }
    }
  }

function setScrollingEvent(id, helper) {
    const elt = document.getElementById(id);
    dotNetHelper = helper;

    if (elt) {
        elt.addEventListener('wheel', scrollingDownOrUp);
    } else {
        console.error("Element not found");
    }
}

function removeScrollingEvent(id) {
    const elt = document.getElementById(id);

    if (elt) {
        console.log("removeScrollingEvent");
        elt.removeEventListener('wheel', scrollingDownOrUp);
    } else {
        console.error("Element not found");
    }
}

