//(function (d, s, id) {
//    var js, fjs = d.getElementsByTagName(s)[0];
//    if (d.getElementById(id)) { return; }
//    js = d.createElement(s); js.id = id;
//    js.src = "//connect.facebook.net/sv_SE/sdk.js";
//    fjs.parentNode.insertBefore(js, fjs);
//}(document, 'script', 'facebook-jssdk'));

$(function () {
    window.fbAsyncInit = function () {
        FB.init({
            appId: '1525858801060556',
            xfbml: true,
            version: 'v2.5'
        });
    };

    //Remove transition attributes for navbar
    if ($("body").hasClass("mobile")) {
        $('.navbar-toggle').removeAttr('data-toggle').removeAttr('data-target');
    }

    if ($('#index-outer-message-container').children().length > 0) {
        $("#index-outer-message-container").animate({ top: '36px', opacity: '1' }, 1000);
    }

    AddAntiForgeryToken = function (data) {
        data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
        return data;
    };

    $(".select2-input").focus(function () {
        $('.modal-form-message-container').css('display', 'none');
    });
});

(function () {
    //Add mobile class to body
    if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|ipad|iris|kindle|Android|Silk|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino/i.test(navigator.userAgent)
    || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(navigator.userAgent.substr(0, 4))) {
        $("body").addClass("mobile");
    }

    //Animate index message
    $('.index-inner-message-container').click(function (e) {
        $("#index-outer-message-container").animate({ top: '-36px', opacity: '1' }, 1000);

        $(".index-inner-message-container").css('cursor', 'default');
    });

    //Hide index message if no click was detected
    function HideIndexMessage() {
        $("#index-outer-message-container").animate({ top: '-36px', opacity: '1' }, 1000);

        $(".index-inner-message-container").css('cursor', 'default');
    }

    if ($('#index-outer-message-container').children().length > 0) {
        if ($('body').hasClass('mobile'))
            setTimeout(HideIndexMessage, 8000)
        else
            setTimeout(HideIndexMessage, 4000)
    }

    //Close nav if click outside (DESKTOP)
    $("body").on('mousedown', function (event) {
        if ($(".navbar-collapse").hasClass("in") && $(event.target).prop("tagName") != 'A')
            $('.collapse').collapse('hide');
    });

    //Close nav if click outside (MOBILE)
    $("body").on('touchstart', function (event) {
        if ($('#navbar-container').is(':visible') && $(event.target).prop("tagName") != 'A' && !$(event.target).hasClass('navbar-toggle') && !$(event.target).hasClass('navbar-header')) {
            $('#navbar-container').css('display', 'none');
            $(".dropdown .menu-item").blur();
            //close event... La till navbar-header
        }
    });

    //Open nav without transition (MOBILE)
    $('.navbar-toggle').click(function () {
        if ($('body').hasClass('mobile')) {
            if ($('#navbar-container').is(':hidden'))
                $('#navbar-container').css('display', 'block');
            else
                $('#navbar-container').css('display', 'none');
        }
    });

    //Remove focus from dropdown, menu-item, when closed
    $(".menu-item .dropdown-item").click(function () {
        $(".dropdown .menu-item").blur();
    });

    //Set color for contact, select input
    function colorizeContactSelect() {
        if ($(this).val() === "0" || $(this).val() === null)
            $(this).addClass("select-first");
        else
            $(this).removeClass("select-first")
    }

    //Prevent header dropdown from closing (desktops)
    $("#header-dropdown-sign-out").bind("click", function (e) {
        e.stopPropagation();
    });

    //Prevent header dropdown from closing (mobiles)
    $('#header-dropdown-sign-out').bind("touchstart", function (e) {
        e.stopPropagation();
    });

    //Prevent header dropdown from closing (desktops)
    $("#header-dropdown-account-index-link").bind("click", function (e) {
        e.stopPropagation();
    });

    //Prevent header dropdown from closing (mobiles)
    $('#header-dropdown-account-index-link').bind("touchstart", function (e) {
        e.stopPropagation();
    });

    //Prevent header dropdown from closing (desktops)
    $("#header-dropdown-administration-link").bind("click", function (e) {
        e.stopPropagation();
    });

    //Prevent header dropdown from closing (mobiles)
    $('#header-dropdown-administration-link').bind("touchstart", function (e) {
        e.stopPropagation();
    });

    //Center all modals - phase one - store every dialog's height
    $('.modal').each(function () {
        var t = $(this),
            d = t.find('.modal-dialog'),
            fadeClass = (t.is('.fade') ? 'fade' : '');
        // render dialog
        t.removeClass('fade')
            .addClass('invisible')
            .css('display', 'block');
        // read and store dialog height
        d.data('height', d.height());
        // hide dialog again
        t.css('display', '')
            .removeClass('invisible')
            .addClass(fadeClass);
    });
    //Phase two - set margin-top on every dialog show
    $('.modal').on('show.bs.modal', function () {
        var t = $(this),
            d = t.find('.modal-dialog'),
            dh = d.data('height'),
            w = $(window).width(),
            h = $(window).height();
        // if it is desktop & dialog is lower than viewport
        // (set your own values)
        if (w > 380 && (dh + 60) < h) {
            d.css('margin-top', Math.round(1 * (h - dh) / 2));
        } else {
            d.css('margin-top', '');
        }
    });

    //Open event for all modals (show)
    $('.modal').bind('show.bs.modal', function (event) {
        //Collaps nav if nav is opened
        if ($(".navbar-collapse").hasClass("in"))
            $(".navbar-collapse").collapse('hide');

        if ($('.alert-popup').length > 0) {
            $('.alert-popup').remove();
        }

        //Dont proceed if register modal is already open, (dont calculate scroll position/set html properties)
        if ($('body').hasClass('mobile') && $('#modal-register').hasClass('in') == false) {
            sethtmlpos = $(window).scrollTop();
            $('html').css('width', '100%');
            $('html').css('position', 'fixed');
            $('html').css('margin-top', '-' + sethtmlpos + 'px');
            $('.modal .modal-body').css('overflow-y', 'auto');
        }
        else {
            //Keep same width on footer when model is opened to prevent it from shifting
            var footerwidth = $('#footer-container').width();
            $('#footer-container').css('width', footerwidth + 'px');

            //Prevent shifting on safari
            if (navigator.userAgent.indexOf("Safari") > -1 && navigator.userAgent.indexOf('Chrome') == -1) {
                $('body').css('overflow-y', 'hidden');
                $('body').css('padding-right', '17px');
            }
        }
    });

    //Close event for all modals
    $('.modal').bind('hidden.bs.modal', function (event) {
        //Dont proceed if its user agreement modal, (two modals up at the same time)
        if ($(event.target).attr('id') != 'modal-useragreement') {
            if ($('body').hasClass('mobile')) {
                $('html').css('width', '');
                $('html').css('position', '');
                $('html').css('margin-top', '0px');
                window.scrollTo(0, sethtmlpos);
            }
            else {
                //Reset width for footer
                $('#footer-container').css('width', '');

                //Reset prevent shifting on safari
                if (navigator.userAgent.indexOf("Safari") > -1 && navigator.userAgent.indexOf('Chrome') == -1) {
                    $('body').css('overflow-y', 'scroll');
                    $('body').css('padding-right', '0px');
                }
            }
        }
    });

    $(".select-color").on('change keyup', colorizeContactSelect).change();
})(jQuery);

//Submit form
$('.trigger-next-form').click(function () {
    $(this).find('form').submit();
});