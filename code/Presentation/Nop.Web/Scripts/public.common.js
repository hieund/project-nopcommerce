function getE(name) {
    if (document.getElementById)
        var elem = document.getElementById(name);
    else if (document.all)
        var elem = document.all[name];
    else if (document.layers)
        var elem = document.layers[name];
    return elem;
}

function OpenWindow(query, w, h, scroll) {
    var l = (screen.width - w) / 2;
    var t = (screen.height - h) / 2;

    winprops = 'resizable=0, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l + 'w';
    if (scroll) winprops += ',scrollbars=1';
    var f = window.open(query, "_blank", winprops);
}

function setLocation(url) {
    window.location.href = url;
}

function displayAjaxLoading(display) {
    if (display) {
        $('.ajax-loading-block-window').show();
    }
    else {
        $('.ajax-loading-block-window').hide('slow');
    }
}

function displayPopupNotification(message, messagetype, modal) {
    //types: success, error
    var container;
    if (messagetype == 'success') {
        //success
        container = $('#dialog-notifications-success');
    }
    else if (messagetype == 'error') {
        //error
        container = $('#dialog-notifications-error');
    }
    else {
        //other
        container = $('#dialog-notifications-success');
    }

    //we do not encode displayed message
    var htmlcode = '';
    if ((typeof message) == 'string') {
        htmlcode = '<p>' + message + '</p>';
    } else {
        for (var i = 0; i < message.length; i++) {
            htmlcode = htmlcode + '<p>' + message[i] + '</p>';
        }
    }

    container.html(htmlcode);

    var isModal = (modal ? true : false);
    container.dialog({modal:isModal});
}


var barNotificationTimeout;
function displayBarNotification(message, messagetype, timeout) {
    debugger;
    clearTimeout(barNotificationTimeout);

    //types: success, error
    var cssclass = 'success';
    if (messagetype == 'success') {
        cssclass = 'success';
    }
    else if (messagetype == 'error') {
        cssclass = 'error';
    }
    //remove previous CSS classes and notifications
    $('#bar-notification')
        .removeClass('success')
        .removeClass('error');
    $('#bar-notification .content').remove();

    //we do not encode displayed message

    //add new notifications
    var htmlcode = '';
    if ((typeof message) == 'string') {
        htmlcode = '<p class="content">' + message + '</p>';
    } else {
        for (var i = 0; i < message.length; i++) {
            htmlcode = htmlcode + '<p class="content">' + message[i] + '</p>';
        }
    }
    $('#bar-notification').append(htmlcode)
        .addClass(cssclass)
        .fadeIn('slow')
        .mouseenter(function ()
            {
                clearTimeout(barNotificationTimeout);
            });

    $('#bar-notification .close').unbind('click').click(function () {
        $('#bar-notification').fadeOut('slow');
    });

    //timeout (if set)
    if (timeout > 0) {
        barNotificationTimeout = setTimeout(function () {
            $('#bar-notification').fadeOut('slow');
        }, timeout);
    }
}

function htmlEncode(value) {
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}

function LoadCollection() {
    $('#divCollection').load("/Catalog/ProductCollections", { productId: $('#hfProductID').val() });
}

function ShowMap(lat, long) {

    var myLatlng = new google.maps.LatLng(lat, long);
    //var myLatlng = new google.maps.LatLng(10.760988, 106.684842);
    var myOptions = {
        zoom: 14,
        center: myLatlng,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    var map = new google.maps.Map(document.getElementById("map"), myOptions);

    //var contentString = '<div id="content">phukien.vn</div>';

    //var infowindow = new google.maps.InfoWindow({content: contentString});

    var marker = new google.maps.Marker({
        position: myLatlng,
        map: map,
        title: 'Click to zoom'
    });

    //google.maps.event.addListener(map, 'center_changed', function () {
    //    window.setTimeout(function () {
    //        map.panTo(marker.getPosition());
    //    }, 3000);
    //});

    google.maps.event.addListener(marker, 'click', function () {
        map.setZoom(14);
        map.setCenter(marker.getPosition());
    });
}
function Toggle(o) {
    var obj = document.getElementById(o);
    if (obj) {
        if ($(obj).is(":visible"))
            $(obj).hide(300);
        else
            $(obj).show(300);
    }
}
var scrollTimer = null;
function floatBanner() {
    scrollTimer = null;
    var left, right;
    var top = $('html').offset().top;
    left = document.getElementById("left-float-banner");
    right = document.getElementById("right-float-banner");
    if (left) {
        $("#left-float-banner").css("left", (($(window).width() - 1000) / 2) - 162 + "px");
        //$("#left-float-banner").css("top", -top + "px");
        $("#left-float-banner").show(1);
        $("#left-float-banner").animate({
            top: -top+"px",
        }, 400);
    }
    if (right) {
        $("#right-float-banner").css("left", (($(window).width() - 1000) / 2) + 1000 + "px");
        $("#right-float-banner").animate({
            top: -top + "px",
        }, 400);
        $("#right-float-banner").show(1);
    }
}
$(window).scroll(function () {
    if (scrollTimer) {
        clearTimeout(scrollTimer);
    }
    scrollTimer = setTimeout(floatBanner, 500);
});
$(window).on("resize", function () {
    floatBanner();
});