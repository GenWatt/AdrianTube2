class MediaPlayer {
    constructor(video) {
        this.video = video;
    }

    play() {
        if (this.video.paused) {
            this.video.play();
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

    window.mediaPlayer = new MediaPlayer(video);
}

let lastScroll = 0;

function scrollingDownOrUp(e) {
    const currentScroll = window.scrollY;

    if (currentScroll > lastScroll) {
        if (window.dotNetHelper) {
            window.dotNetHelper.invokeMethodAsync('ScrollingDown');
        }
    } else {
        if (window.dotNetHelper) {
            window.dotNetHelper.invokeMethodAsync('ScrollingUp');
        }
    }

    lastScroll = currentScroll;
    
}

function setScrollingEvent(id) {
    const elt = document.getElementById(id);
    if (elt) {
        elt.addEventListener('scroll', scrollingDownOrUp);
    } else {
        console.error("Element not found");
    }
}

function removeScrollingEvent(id) {
    const elt = document.getElementById(id);
    if (elt) {
        elt.removeEventListener('scroll', scrollingDownOrUp);
    } else {
        console.error("Element not found");
    }
}

