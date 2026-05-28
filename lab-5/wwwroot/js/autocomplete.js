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

        // Try to resolve the visible text to an id when input loses focus
        function resolveInputToId() {
            return new Promise(function (resolve) {
                const visible = $input.val().trim();
                const hiddenVal = $hiddenInput.val();

                if (!visible) {
                    $hiddenInput.val('');
                    $input.removeClass('is-invalid');
                    resolve(true);
                    return;
                }

                if (hiddenVal) {
                    $input.removeClass('is-invalid');
                    resolve(true);
                    return;
                }

                $.ajax({
                    url: searchUrl,
                    type: 'GET',
                    data: { query: visible },
                    success: function (data) {
                        if (data && data.length === 1 && data[0].text.trim().toLowerCase() === visible.toLowerCase()) {
                            $hiddenInput.val(data[0].id);
                            $input.removeClass('is-invalid');
                            resolve(true);
                        } else {
                            $input.addClass('is-invalid');
                            resolve(false);
                        }
                    },
                    error: function () {
                        $input.addClass('is-invalid');
                        resolve(false);
                    }
                });
            });
        }

        $input.on('blur', function () {
            // small timeout to allow click on results to register first
            setTimeout(function () { resolveInputToId(); }, 150);
        });

        // Ensure autocomplete resolves before the parent form submits
        const $form = $input.closest('form');
        if ($form.length) {
            $form.on('submit', function (e) {
                // Prevent default and resolve first
                e.preventDefault();
                resolveInputToId().then(function (ok) {
                    if (ok) {
                        // Unbind this submit handler to avoid loops and submit
                        $form.off('submit');
                        $form.submit();
                    } else {
                        $input.focus();
                    }
                });
            });
        }

        $(document).on("click", function (e) {
            if (!$(e.target).closest(".autocomplete-wrapper").length) {
                $resultsList.hide();
            }
        });
    });
});
