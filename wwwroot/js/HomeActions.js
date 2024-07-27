function ChangeViewMode(viewMode) {
    $('.setPage').val($('.current-page').data('page'));
    $('.setViewMode').val(viewMode);

    loadProducts();
}
function loadProducts() {
    const formData = $('#ProductsLoadSettings').serialize();
    $.ajax({
        url: '/Home/GetProducts',
        method: 'POST',
        data:  formData , 
        beforeSend: function () {
            showSpinner('#products-container .row');
        },
        success: function (productsHtml) {
            loadPaginationControls();
            hideSpinner();
            $('#products-container .row').html(productsHtml);
        },
        error: function (error) {
            hideSpinner();
            $('#products-container .row').html('<p class="alert alert-info text-center">لا توجد منتجات في الموقع 😒😒😒</p>');
            console.error('Error loading products:', error);
            $('#pagination-container').empty();
        }
    });
}


function loadPaginationControls() {
    const formData = $('#ProductsLoadSettings').serialize();

    $.ajax({
        url: '/Home/GetPaginationControls',
        method: 'Post',

        data:formData,
        success: function (paginationHtml) {
            $('#pagination-container').html(paginationHtml);
        },
        error: function (xhr, status, error) {
            console.error('Error loading pagination controls:', error);
        }


    });
}

function paginationEvent() {
    $('#pagination-container').on('click', '.page-link', function (e) {
        e.preventDefault();
        $('.setPage').val($(this).data('page'));
        $('.setTag').val($('.tab-current').data("filter"));
        loadProducts();
    });
}