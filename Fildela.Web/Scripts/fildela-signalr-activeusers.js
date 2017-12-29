(function () {
    'use strict';

    var con = $.hubConnection();
    //con.logging = true;
    var hub = con.createHubProxy('ActiveUsers');

    hub.on('onHitRecorded', function (i) {
        if (i != 0) {
            $('.active-users-text').text(i);

            if ($('#active-users-container-header').css("visibility") == "hidden")
                $('#active-users-container-header').css('visibility', 'visible');

            if ($('#active-users-container-index').css("visibility") == "hidden")
                $('#active-users-container-index').css('visibility', 'visible');

            if (!$('body').hasClass('mobile'))
                sessionStorage.activeusers = i;
        }
    });
    con.start(function () {
        hub.invoke('RegisterUserConnect');
    });
})(jQuery);