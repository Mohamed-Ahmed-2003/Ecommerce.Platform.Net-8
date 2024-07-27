function UpdatePriceHtmlDiv(res) {
    if (res.discountedTotal != "") {
        return `<div class="text-center"><h5><b>بعد الخصم</b></h5></div>
                        <hr>
                        <span dir="ltr" class="text-decoration-line-through total ">${res.total}</span>
                    <div class="col d-flex justify-content-between">
                        <strong class="discounted-total">${res.discountedTotal}</strong>

                        المجموع</div>`;
    } else {
        return `
            <div class="col d-flex justify-content-between">
                <strong class="total">${res.total}</strong>
                المجموع
            </div>`;
    }
}


function AddToWish(id) {
    $.ajax({
        type: "GET",
        url: "/Wishlist/AddToWishlist", 
        data: { productId: id },
        success: () => {
            successNoti(``, 'تمت اضافته الي قائمة المفضلات');
        },
        error: () => {
            errorNoti('حدث خطأ ', 'اثناء اضافة المنتج الي قائمة المفضلات');
        }
    });
}




function AddToCart(id, quan = 1) {
    $.ajax({
        type: "GET",
        url: "/Cart/AddToCart",
        data: { productId: id, quan: quan },
        success: () => {
            successNoti(`ولعانة`, 'تمت اضافته الي سلة المشتريات ');
        },
        error: () => {
            errorNoti('حدث خطأ', 'اثناء اضافة المنتج الي  سلة المشتريات');
        }
    });
}

function RemoveFromWishlist(id) {
    let itemBox;
    $.ajax({
        type: "GET",
        url: "/Wishlist/RemoveFromWishlist",
        data: { Id: id},
        success: () => {
            if (itemBox == null)
                itemBox = $(`#wish-item-${id}`);
            itemBox.remove();

            if ($('.wish-item').length == 0) {
                $('.wishlist-card').html('<p class="alert alert-info">لا توجد منتجات مفضلة 😒😒😒.</p>');
            }

            successNoti(`عملية ازالة`, 'تم ازالة العنصر المطلوب');
        },
        error: () => {
            errorNoti('هههه', "لم يتم ازالة العنصر");
        }
    });
}function RemoveFromCart(id, itemBox = null) {
    $.ajax({
        type: "GET",
        url: "/Cart/RemoveFromCart",
        data: { OrderItemId: id},
        success: (res) => {
            if (itemBox == null)
                itemBox = $(`#order-item-${id}`);

            itemBox.remove();
            $(`#discount-${id}`).remove();

            if ($('.order-item-box').length == 0) {
                $('.cart-card').html('<p  class="alert alert-info text-center">لا توجد عناصر في السلة 😒😒😒.</p>');
            }

            $('.total').text(res.total);

            let finalPriceHtml = UpdatePriceHtmlDiv(res);

            $('.total-price-box').html(finalPriceHtml);

            successNoti(`عملية ازالة`, 'تم حذف العنصر المطلوب');
        },
        error: (res) => {
            errorNoti('هههه', "لم يتم حذف العنصر");
        }
    });
}
function UpdateCart(id, quan) {

    const itemBox = $(`#order-item-${id}`);
    const quantity = itemBox.find('.qty');
    const newQuan = Number(quantity.text()) + quan;
    if (newQuan <= 0) {
        RemoveFromCart(id, itemBox);
        return;
    }

    $.ajax({
        type: "GET",
        url: "/Cart/UpdateQuantity",
        data: { orderItemId: id, quan: quan },
        success: (res) => {
            quantity.html(newQuan);
            $('.total').text(res.total);

            let finalPriceHtml = UpdatePriceHtmlDiv(res);
          

            $('.total-price-box').html(finalPriceHtml);
           
            successNoti(`ولعانة`, res.mesg);
        },
        error: (res) => {
            errorNoti('هههه', res.mesg);
        }
    });
}
    