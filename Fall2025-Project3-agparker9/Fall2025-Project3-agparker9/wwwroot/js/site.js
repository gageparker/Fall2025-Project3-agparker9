// carousel.js - Movie Poster Carousel functionality
document.addEventListener('DOMContentLoaded', function() {
    const posters = document.querySelectorAll('.poster-item');
    const prevBtn = document.getElementById('prevBtn');
    const nextBtn = document.getElementById('nextBtn');
    const indicators = document.querySelectorAll('.indicator-dot');
    let currentIndex = 0;
    const totalPosters = posters.length;

    console.log('Carousel initialized');
    console.log('Total posters found:', totalPosters);
    console.log('Prev button:', prevBtn);
    console.log('Next button:', nextBtn);

    if (totalPosters === 0) {
        console.log('No posters found - exiting');
        return;
    }

    function showPoster(index) {
        console.log('Showing poster at index:', index);

        posters.forEach(poster => poster.classList.remove('active'));
        indicators.forEach(indicator => indicator.classList.remove('active'));

        posters[index].classList.add('active');
        if (indicators[index]) {
            indicators[index].classList.add('active');
        }
        currentIndex = index;
    }

    function nextPoster() {
        const newIndex = (currentIndex + 1) % totalPosters;
        console.log('Next clicked - moving from', currentIndex, 'to', newIndex);
        showPoster(newIndex);
    }

    function prevPoster() {
        const newIndex = (currentIndex - 1 + totalPosters) % totalPosters;
        console.log('Prev clicked - moving from', currentIndex, 'to', newIndex);
        showPoster(newIndex);
    }

    if (prevBtn) {
        prevBtn.addEventListener('click', function(e) {
            e.preventDefault();
            console.log('Prev button clicked');
            prevPoster();
        });
    }

    if (nextBtn) {
        nextBtn.addEventListener('click', function(e) {
            e.preventDefault();
            console.log('Next button clicked');
            nextPoster();
        });
    }

    indicators.forEach(indicator => {
        indicator.addEventListener('click', function() {
            const index = parseInt(this.getAttribute('data-index'));
            console.log('Indicator clicked for index:', index);
            showPoster(index);
        });
    });

    document.addEventListener('keydown', function(e) {
        if (e.key === 'ArrowLeft') {
            console.log('Left arrow key pressed');
            prevPoster();
        } else if (e.key === 'ArrowRight') {
            console.log('Right arrow key pressed');
            nextPoster();
        }
    });

    console.log('Carousel setup complete');
});