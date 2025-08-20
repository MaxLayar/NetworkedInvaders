class Timer {
    constructor(duration, onTick, onEnd) {
        this.duration = duration;
        this.timeleft = duration;
        this.interval = null;
        this.onTick = onTick;
        this.onEnd = onEnd;
    }

    start() {
        if (this.interval) return;
        this.interval = setInterval(() => {
            this.timeleft--;
            if (this.onTick) this.onTick(this.timeleft);

            if (this.timeleft < 0) {
                this.stop();
                if (this.onEnd) this.onEnd();
            }
        }, 1000);
    }

    stop() {
        if (this.interval) {
            clearInterval(this.interval);
            this.interval = null;
        }
    }

    reset() {
        this.stop();
        this.timeleft = this.duration;
    }
}

module.exports = Timer;
