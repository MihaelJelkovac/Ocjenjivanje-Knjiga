// Inicijalizacija datetime kontrola
$(document).ready(function () {
    // Za svaku datetime kontrolu na stranici
    $('.datetime-control').each(function () {
        const fieldName = $(this).find('.datetime-input').data('field-name');
        const dateFormat = $(this).find('.datetime-input').data('date-format');
        const timeFormat = $(this).find('.datetime-input').data('time-format');

        const $dateInput = $(`#${fieldName}-date`);
        const $timeInput = $(`#${fieldName}-time`);
        const $combinedInput = $(`#${fieldName}-combined`);

        function parseDate(dateStr, format) {
            if (!dateStr) return null;

            if (format.includes('.')) {
                // dd.MM.yyyy format
                const parts = dateStr.split('.');
                if (parts.length !== 3) return null;

                const day = parseInt(parts[0]);
                const month = parseInt(parts[1]);
                const year = parseInt(parts[2]);

                return new Date(year, month - 1, day);
            } else {
                // MM/dd/yyyy format
                const parts = dateStr.split('/');
                if (parts.length !== 3) return null;

                const month = parseInt(parts[0]);
                const day = parseInt(parts[1]);
                const year = parseInt(parts[2]);

                return new Date(year, month - 1, day);
            }
        }

        function updateCombinedValue() {
            const dateStr = $dateInput.val();
            const timeStr = $timeInput.val();

            if (dateStr && timeStr) {
                const dateObj = parseDate(dateStr, dateFormat);

                if (dateObj && !isNaN(dateObj.getTime())) {
                    const [hours, minutes] = timeStr.split(':');
                    dateObj.setHours(parseInt(hours) || 0, parseInt(minutes) || 0, 0);
                    $combinedInput.val(dateObj.toISOString());
                }
            }
        }

        $dateInput.on("change", updateCombinedValue);
        $timeInput.on("change", updateCombinedValue);

        // Validacija datuma
        $dateInput.on("blur", function () {
            const value = $(this).val();
            if (value) {
                const dateObj = parseDate(value, dateFormat);

                if (!dateObj || isNaN(dateObj.getTime())) {
                    $(this).addClass("is-invalid");
                    $combinedInput.val("");
                    return;
                }

                $(this).removeClass("is-invalid");
            }
        });
    });
});
