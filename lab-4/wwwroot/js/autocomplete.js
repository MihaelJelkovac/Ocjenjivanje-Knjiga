// Inicijalizacija svih autocomplete dropdown-a na stranici
$(document).ready(function () {
    // Za svaki autocomplete wrapper na stranici
    $('.autocomplete-wrapper').each(function () {
        const fieldName = $(this).find('.autocomplete-input').data('field-name');
        const searchUrl = $(this).find('.autocomplete-input').data('search-url');

        const $input = $(`#${fieldName}-search`);
        const $resultsList = $(`#${fieldName}-results`);
        const $hiddenInput = $(`#${fieldName}-id`);

        let searchTimeout;

        $input.on("keyup", function () {
            const query = $(this).val().trim();

            clearTimeout(searchTimeout);

            if (query.length < 2) {
                $resultsList.hide();
                return;
            }

            searchTimeout = setTimeout(function () {
                $.ajax({
                    url: searchUrl,
                    type: "GET",
                    data: { query: query },
                    success: function (data) {
                        $resultsList.empty();

                        if (!data || data.length === 0) {
                            $resultsList.append('<li class="no-results">Nema rezultata</li>');
                            $resultsList.show();
                            return;
                        }

                        data.forEach(function (item) {
                            const $li = $(`<li data-id="${item.id}" class="autocomplete-item">${item.text}</li>`);
                            $li.on("click", function () {
                                $input.val(item.text);
                                $hiddenInput.val(item.id);
                                $resultsList.hide();
                            });
                            $resultsList.append($li);
                        });

                        $resultsList.show();
                    },
                    error: function () {
                        $resultsList.empty().append('<li class="error">Greška pri pretrazi</li>').show();
                    }
                });
            }, 300);
        });

        $(document).on("click", function (e) {
            if (!$(e.target).closest(".autocomplete-wrapper").length) {
                $resultsList.hide();
            }
        });
    });
});
