const audio = new Audio("pop.mp3");

export async function playSound() {
    if(!audio.paused) {
        audio.pause();
        audio.currentTime = 0;
    }
    await audio.play();
}