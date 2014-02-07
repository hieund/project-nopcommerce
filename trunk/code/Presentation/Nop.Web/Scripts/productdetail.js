$(document).ready(function () {
    //When page loads...
    $(".tab_content").hide(); //Hide all content
    $("ul.tabs li:first").addClass("active").show(); //Activate first tab
    $(".tab_content:first").show(); //Show first tab content

    //On Click Event
    $("ul.tabs li").click(function () {
        $("ul.tabs li").removeClass("active"); //Remove any "active" class
        $(this).addClass("active"); //Add "active" class to selected tab
        $(".tab_content").hide(); //Hide all tab content

        var activeTab = $(this).find("a").attr("href"); //Find the href attribute value to identify the active tab + content
        $(activeTab).fadeIn(); //Fade in the active ID content
        return false;
    });
    $('#gallery-horizotal-thumbnail').royalSlider({
        fullscreen: {
            enabled: true,
            nativeFS: true
        },
        controlNavigation: 'thumbnails',
        autoScaleSlider: true,
        autoScaleSliderWidth: 345,
        autoScaleSliderHeight: 347,
        imgWidth:303,
        loop: false,
        autoPlay: {
            // autoplay options go gere
            enabled: true,
            pauseOnHover: true,
            arrowsAutoHide: false
        },
        numImagesToPreload: 5,
        arrowsNavAutohide: true,
        arrowsNavHideOnTouch: true,
        keyboardNavEnabled: true
    });
    

});
function ChangeProductVariantColor(url) {
    debugger;
    $.ajax({
        url: url,
        type: 'GET',
        cache: true,
        dataType: 'json',
        beforeSend: function () {
            displayAjaxLoading(true);
            this.loadWaiting = true;
        },
        success: function (d) {
            debugger;
            if (d.pn != "")
            {
                $(".product-name h1").html($("#hdProductname").val() + " " + d.pn);
            }
            if (d.p != "0" && d.p != "0₫")
            {
                $(".detail_col2 .price").show();
                $(".detail_col2 .price strong").html(d.p);
            }
            else
            {
                $(".detail_col2 .price").hide();
                $(".promotion").hide();
            }
            if (d.op != "0") {
                $(".detail_col2 .price cite").html(d.op);
            }
            if (d.s != "") {
                $(".detail_col2 p.colorgreen").html(d.s);
            }
            if (d.dis != "0") {
                $(".prodetail .countdown").show();
                $(".prodetail .countdown").html("-"+d.dis+"%");
            }
            else
            {
                $(".prodetail .countdown").hide();
            }
            if (d.id != "0")
            {
                $(".color").removeClass("active");
                $("#color_" + d.id).addClass("active");
                if ($('#rsTmb_' + d.id).html() != undefined) {
                    $("#gallery-horizotal-thumbnail").royalSlider('goTo', parseInt($('#rsTmb_' + d.id).attr("itemid")));
                    $("#gallery-horizotal-thumbnail").royalSlider('autoPlay', 'enabled: true,pauseOnHover: true,arrowsAutoHide: false');
                }
                //$(".btaddcart").attr("onclick", "AjaxCart.addproductvarianttocart('/addproductvarianttocart/" + d.id + "/1', '#product-details-form');return false;")
                //$(".btmua").attr("onclick", "QuickOrder(" + d.id + ");");
            }
            if (d.promo != "") {
                $(".promotion").show();
                $(".promotion").html('<strong>Khuyến mãi:</strong><br />' + d.promo);
            }
            else {
                $(".promotion").hide();
            }
            
        },
        complete:function()
        {
            AjaxCart.setLoadWaiting(false);
        },
        error: function (e) {
        }
    })
}
function QuickOrder(intproductvariantid) {
    var data = {};
    var url = '/quickorder/' + intproductvariantid;
    POSTAjax(url, data, BeforeSendAjax, function (e) {
        if (e != null || e != '') {
            try {
                $('#pu-quickorder').remove();
                $('body').append(e);
            }
            catch (err) { }
            $.fancybox.close();
            $('#pu-quickorder').addClass('step1');
            $("#openfancyquick").fancybox({
                'transitionIn': 'none',
                'transitionOut': 'none',
                'showCloseButton': false,
                'margin': 0,
                'padding': 1,
                'modal': true
            });
            AjaxCart.setLoadWaiting(false);
            $("#openfancyquick").click();
        }
        //$('#dlding').fadeOut(1000);
    }, ErrorAjax, true);
}
function SubmitQuickOrder($form) {
    var valid = true;
    // client validating

    if ($("#BillingAddress_LastName").val() == '') {
        valid = false;
        $form.find($("#BillingAddress_LastName")).parent().find('.formerror').show();
    }

    if ($("#BillingAddress_LastName").length > 50) {
        valid = false;
        $form.find($("#BillingAddress_LastName")).parent().find('.formerror').text('Quá 50 ký tự').show();
    }

    if ($("#BillingAddress_PhoneNumber").val() == '') {
        valid = false;
        $form.find($("#BillingAddress_PhoneNumber")).parent().find('.formerror').show();
    }

    regPhone = /^((09[0-9]{8})|(01[0-9]{9}))$/;
    if (!regPhone.test($("#BillingAddress_PhoneNumber").val())) {
        valid = false;
        $form.find($("#BillingAddress_PhoneNumber")).parent().find('.formerror').text('Chưa đúng định dạng').show();
    }
    if ($("#BillingAddress_Email").val() != '')
    {
        if (!ValidateEmail($("#BillingAddress_Email").val()))
        {
            $(".help").hide();
            $("#BillingAddress_Email").parent().find('span').attr("class", "formerror").text("Email không hợp lệ !").show();
            valid = false;
        }
    }
    if (!valid)
        return;
    //POSTAJAX
    var data = GetAllFormData($form);
    POSTAjax("/checkout/OpcConfirmQuickOrderSave/", data, BeforeSendAjax, function (e) {
        if (e != null && e != '' && e != undefined) {
            console.log(e);
            if (e.success == "1")
            {
                $("#pu-quickorder .modal-header h4").html("Đặt hàng thành công")
                var templatesucc = "<p class='msgsucc'>Quý khách đã đặt mua thành công sản phẩm <b>{productname}</b> với giá <b>{price}đ</b>. Nhân viên của GadGet City sẽ gọi xác nhận và giao hàng trong thời gian sớm nhất.<br /><a href='/orderdetails/{orderid}'>Xem chi tiết đơn hàng.</a></p>";
                templatesucc = templatesucc.replace("{orderid}", e.orderid);
                templatesucc = templatesucc.replace("{productname}", e.productname);
                templatesucc = templatesucc.replace("{price}", e.price);
                $("#pu-quickorder .quickorder").html(templatesucc);
            }
            else if (e.error == "1") {
                alert("Quá trình thực hiện xảy ra lỗi. Vui lòng thử lại sau");
            }
            else {
                alert("Quá trình thực hiện xảy ra lỗi. Vui lòng thử lại sau");
            }
        }
        AjaxCart.setLoadWaiting(false);
    }, ErrorAjax, true);
}
function GetAllFormData($f) {
    var dataElement = {};
    $f.find('input[type=text], input[type=password], input[type=radio]:checked, input[type=hidden], textarea').each(function () {
        dataElement[$(this).attr('name')] = $(this).val();
    });
    $f.find('input[type=checkbox]').each(function () {
        dataElement[$(this).attr('name')] = $(this).attr('checked') == 'checked' ? true : false;
    });
    $f.find('select').each(function () {
        dataElement[$(this).attr('name')] = $(this).val();
        dataElement[$(this).attr('name') + 'text'] = $(this).find('option:selected').text();
    });
    var dataAttach = {};
    $f.find('input[type=text], input[type=password], input[type=radio]:checked, input[type=hidden], textarea, select option:selected').each(function () {
        dataAttach = $.extend({}, dataAttach, $(this).data());
    });
    var dataReturn = $.extend({}, dataElement, dataAttach);
    return dataReturn;
}







// CALLING AJAX
function POSTAjax(url, dat, befHandle, sucHandle, errHandle, asy) {
    $.ajax({
        async: asy,
        url: url,
        data: dat,
        type: 'POST',
        cache: false,
        beforeSend: function () {
            befHandle();
        },
        success: function (e) {
            sucHandle(e);
        },
        error: function () {
            errHandle();
        }
    });
}
// BeforeSendAjax
function BeforeSendAjax() {
    displayAjaxLoading(true);
    this.loadWaiting = true;
}
// ErrorAjax
function ErrorAjax() {
    // Not implemented yet
    AjaxCart.setLoadWaiting(false);
}

function CheckToCompare(id) {
    $.post('/Catalog/CheckProductToCompare?productId=' + id, {}, function (data) {
        if (data == 0) {
            if (confirm("Sản phẩm so sánh không cùng ngành hàng. Bạn có muốn xóa danh sách sản phẩm so sánh?"))
                setLocation('/so-sanh/them/' + id);
        }
        else
            setLocation('/so-sanh/them/' + id);
    });
}
// !!!!! Validate an email
function ValidateEmail(input) {
    var emailRegex = /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}$/i;
    if (!emailRegex.test(input))
        return false;
    return true;
}
function ProducreviewsAdd(productid)
{
    var data = GetAllFormData($("#productreviewsform"));
    POSTAjax("/catalog/AjaxProductReviewsAdd/", data, BeforeSendAjax, function (e) {
        if (e != null && e != '' && e != undefined) {
            console.log(e);
            if (e.succ == "1") {
                $(".buttons").append("<span class='msg' style='color:green'>Bình luận của bạn đã được gửi thành công</span>")
                $("#AddProductReview_Title").val("");
                $("#AddProductReview_ReviewText").val("");
                
                setTimeout(function () {
                    $(".buttons span.msg").hide();
                }, 2000);
            }
            else if (e.succ == "0" && e.code=="1") {
                $(".buttons").append("<span class='msg' style='color:red'>Quá trình thực hiện xảy ra lỗi. Vui lòng thử lại sau</span>")
                setTimeout(function () {
                    $(".buttons span.msg").remove();
                }, 2000);
            }
            else if (e.succ == "0" && e.code == "2") {
                location.href = "/";
            }
            else if (e.succ == "0" && e.code == "3") {
                location.href = "/login";
            }
            else {
                $(".buttons").append("<span class='msg' style='color:red'>Quá trình thực hiện xảy ra lỗi. Vui lòng thử lại sau</span>")
                setTimeout(function () {
                    $(".buttons span.msg").remove();
                }, 2000);
            }
        }
        AjaxCart.setLoadWaiting(false);
    }, ErrorAjax, true);
    return false;
}