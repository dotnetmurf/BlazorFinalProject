window.scrollToTopComplete = function () {
    window.scrollTo({ top: 0, behavior: 'auto' });

    const scrollableElements = [
        document.documentElement,
        document.body,
        document.querySelector('.main-content-area'),
        document.querySelector('.content'),
        document.querySelector('[data-scroll-container]')
    ];

    scrollableElements.forEach(element => {
        if (element) {
            element.scrollTop = 0;
            element.scrollLeft = 0;
        }
    });

    return true;
};

window.scrollToElement = function (selector) {
    const element = document.querySelector(selector);
    if (element) {
        element.scrollIntoView({ behavior: 'smooth' });
    }
};
