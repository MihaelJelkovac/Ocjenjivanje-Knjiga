(function () {
    var toggle = document.getElementById('appNavToggle');
    var collapse = document.getElementById('appNavCollapse');

    if (!toggle || !collapse) {
        return;
    }

    toggle.addEventListener('click', function () {
        var isOpen = collapse.classList.toggle('open');
        toggle.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
        toggle.classList.toggle('open', isOpen);
    });

    collapse.addEventListener('click', function (event) {
        if (event.target.closest('a, button[type="submit"]')) {
            collapse.classList.remove('open');
            toggle.setAttribute('aria-expanded', 'false');
            toggle.classList.remove('open');
        }
    });
})();
