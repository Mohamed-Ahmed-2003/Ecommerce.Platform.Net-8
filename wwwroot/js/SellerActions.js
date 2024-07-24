const filterOptionsListener = function  () {
    $('.filter-btn').on('click', function () {
        var filterOption = $(this).data('filter');
        $.ajax({
            type: 'GET',
            url: '/Sales/GetSalesCount',
            data: { sellerId: 'sellerId', filterOption: filterOption },
            success: function (data) {
            },
            error: function (xhr, status, error) {
                // Handle error
                console.error(error);
            }
        });

    });
}