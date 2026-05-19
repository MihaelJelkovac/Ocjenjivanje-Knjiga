// AJAX pretraga za Index stranice
$(document).ready(function () {
    // Pronađi sve search forme na stranici
    const $searchForm = $('.search-form');
    const $searchInput = $searchForm.find('input[type="text"]');
    const $tableBody = $('table.app-table tbody');
    const $mobileGrid = $('.books-mobile-grid, .authors-mobile-grid, .genres-mobile-grid, .publishers-mobile-grid, .reviews-mobile-grid, .users-mobile-grid');

    if ($searchForm.length === 0) return; // Nema forme za pretragu

    let searchTimeout;

    $searchInput.on('keyup', function () {
        const query = $(this).val().trim();
        const searchUrl = $searchForm.data('search-url');

        clearTimeout(searchTimeout);

        // Ako je input prazan, prikaži sve
        if (!query) {
            $tableBody.find('tr').show();
            $mobileGrid.find('article').show();
            return;
        }

        searchTimeout = setTimeout(function () {
            $.ajax({
                url: searchUrl,
                type: 'GET',
                data: { query: query },
                success: function (data) {
                    // Sakri sve retke
                    $tableBody.find('tr').hide();
                    $mobileGrid.find('article').hide();

                    if (!data || data.length === 0) {
                        // Prikaži poruku "Nema rezultata"
                        return;
                    }

                    // Prikaži samo retke koji odgovaraju
                    data.forEach(function (item) {
                        $(`tr[data-id="${item.id}"]`).show();
                        $(`article[data-id="${item.id}"]`).show();
                    });
                },
                error: function () {
                    console.error('Greška pri pretrazi');
                }
            });
        }, 300);
    });

    // Očisti pretragu kada korisnik klikne na gumb "Očisti"
    $searchForm.find('button[type="reset"]').on('click', function () {
        $searchInput.val('');
        $tableBody.find('tr').show();
        $mobileGrid.find('article').show();
    });
});
