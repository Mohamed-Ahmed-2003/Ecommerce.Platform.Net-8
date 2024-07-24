const options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": true,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
function successNoti(title = '', mesg) {
    toastr.options = options;
    toastr["success"](mesg, title);

}
function errorNoti(title = '', mesg) {
    toastr.options = options;
    toastr["error"](mesg, title);

}

function calculateRemainingTime(endDate) {
    var remainingTime = new Date(endDate) - new Date();

    if (remainingTime <= 0) {
        return "منتهي";
    }

    var days = Math.floor(remainingTime / (1000 * 60 * 60 * 24));
    var hours = Math.floor((remainingTime % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    var minutes = Math.floor((remainingTime % (1000 * 60 * 60)) / (1000 * 60));

    return days + " يوم و " + hours + " ساعة و " + minutes + " دقيقة";
}
function searchEvent() {
    $('#search-btn').on('click', function (e) {
        e.preventDefault(); // Prevent default anchor click behavior

        $('.search_input').toggleClass('expanded');
        $('.search_input').trigger('focus');

        var searchData = $('.search_input').val().trim();
        if (searchData == '') return;

        var desiredController = '/Home';
        var desiredAction = 'Search';

        // Check if the current URL matches the desired controller and action
        var currentPath = window.location.pathname;
        const newUrl = desiredController + '/' + desiredAction + '?input=' + encodeURIComponent(searchData);

        if (currentPath !== desiredController + '/' + desiredAction) {
            window.location.href = newUrl;
            $('.search_input').val(''); // Clear search input
            return;
        }

        // If the current URL matches the desired controller and action, make AJAX call
        $.ajax({
            url: '/Home/SearchResults',
            method: 'GET',
            data: { input: searchData },
            success: function (response) {
                hideSpinner();
                history.pushState({ path: newUrl }, '', newUrl);
                $('#search-results .products-container').html(response);
                $('.search_input').val(''); // Clear search input
            },
            beforeSend: function () {
                showSpinner(); 
            },
            error: function (xhr, status, error) {
                hideSpinner();
                $('#search-results').html('<p class="alert alert-info">لا توجد نتائج لبحثك العجيب  😒😒😒.</p>');
                $('.search_input').val(''); 
                console.error('Error performing search:', error);
            }
        });
    });
}

$(function () {
    searchEvent();
})

function showSpinner(parent = '#main') {
    $(parent).empty();
    const spinnerHtml = ` <div id="spinner-area" class="d-flex justify-content-center mt-5 " style="height:10vh">
                             <div class="spinner-border tomato-color " style="width:5rem; height:5rem" role="status">
                                 <span class="visually-hidden">Loading...</span>
                             </div>
                     </div>`
    $(parent).prepend(spinnerHtml);

}

function hideSpinner() {
    $('#spinner-area').remove();
}

$(function () {

    const cookies = document.cookie.split(';');
    let ar_culture = false;
    for (let cookie of cookies) {
        const [_, cookieValue] = cookie.split('=');
        ar_culture = cookieValue.endsWith('ar-EG');
    }


    var commonConfig = {
        fixedColumns: {
            left: 1,
            right: 1
        },
        info: false, // Hide the showing info
        paging: true, // Enable paging
        pagingType: "simple",
        pageLength: 5, // Set the default number of entries per page
        lengthMenu: [5, 10], // Dropdown menu options for number of entries per page
        "columnDefs": [
            { "className": "dt-center", "targets": "_all" }
        ]
    };
    if (ar_culture)
        commonConfig.language = { url: 'https://cdn.datatables.net/plug-ins/2.0.0/i18n/ar.json' }
    // Specific configuration for #ordersTable
    var ordersTableConfig = {
        dom: 'Bfrtip',
        // Add any other custom options specific to #ordersTable
    };

    // Initialize specific DataTable with id 'ordersTable' merging common and specific configurations
    $('#ordersTable').DataTable($.extend({}, commonConfig, ordersTableConfig));

    // Initialize all other DataTable instances with class '.datatable' using common configuration
    $('.datatable').each(function () {
        $(this).DataTable(commonConfig);
    });
});
document.addEventListener("DOMContentLoaded", function () {
    if ($('textarea.tiny').length > 0) {
        tinymce.init({
            selector: 'textarea.tiny',
            toolbar: "fontsizeselect fontselect | forecolor | image media", // Added 'media' to the toolbar
            fontsize_formats: '8pt 9pt 10pt 11pt 12pt 14pt 16pt 18pt 20pt 22pt 24pt 26pt 28pt 36pt 48pt 72pt 96pt 144pt 200pt',
            font_formats: 'Arial=arial,helvetica,sans-serif;Times New Roman=times new roman,times,serif;Courier New=courier new,courier,monospace;Tahoma=tahoma,arial,helvetica,sans-serif;Verdana=verdana,arial,helvetica,sans-serif;Arial Black=arial black,avant garde;Georgia=georgia,times new roman,times,serif;Comic Sans MS=comic sans ms,sans-serif;Tajawal=Tajawal, sans-serif;Cairo=Cairo, sans-serif;Rakkas=Rakkas, sans-serif;Workbench=Workbench, sans-serif;Dancing Script=Dancing Script, cursive;Roboto=Roboto, sans-serif;Permanent Marker=Permanent Marker, cursive',
            content_css: [
                'https://fonts.googleapis.com/css2?family=Tajawal:wght@400;700&display=swap', // Google Fonts for Arabic
                'https://fonts.googleapis.com/css2?family=Cairo:wght@200..1000&display=swap',
                'https://fonts.googleapis.com/css2?family=Rakkas&display=swap',
                'https://fonts.googleapis.com/css2?family=Workbench&display=swap',
                'https://fonts.googleapis.com/css2?family=Dancing+Script:wght@400..700&display=swap',
                'https://fonts.googleapis.com/css2?family=Roboto:wght@400;700&display=swap', // Google Fonts for English
                'https://fonts.googleapis.com/css2?family=Permanent+Marker&display=swap' // Google Fonts for Permanent Marker
            ],
            plugins: 'advlist autoresize autolink lists link image media charmap print preview anchor', // Added 'media' plugin
            height: 300,
            media_live_embeds: true, // Enable live embeds for media
        });
    }
});

