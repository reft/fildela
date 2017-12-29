$(function () {
    $('#associate-external-authentication-form').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        locale: 'sv_SE',
        fields: {
            Email: {
                validators: {
                    notEmpty: {
                    },
                    emailAddress: {
                    },
                    stringLength: {
                        min: 6,
                        max: 150
                    }
                }
            },
            Password: {
                validators: {
                    notEmpty: {
                    },
                    stringLength: {
                        min: 6,
                        max: 150
                    }
                }
            }
        }
    }).on('success.form.fv', function (e) {
        //Prevent form submission
        e.preventDefault();

        $.ajax({
            url: '/User/InsertAccountAuthenticationProvider/',
            data: $(e.target).serialize(),
            type: 'POST',
            beforeSend: AssociateExternalAuthenticationOnBegin,
            success: AssociateExternalAuthenticationOnSuccess,
            error: function () {
                AssociateExternalAuthenticationOnError();
            },
            complete: function () {
            }
        });
    });

    //Associate external authentication form OnBegin
    function AssociateExternalAuthenticationOnBegin() {
        //Hide error message
        $('#error-msg-container').css('display', 'none');

        //Hide icon
        $('#associate-external-authentication-icon').css('display', 'none');

        //Show spinner
        $('#associate-external-authentication-spin').css('display', '');

        //Blur inputs
        $('.associate-external-authentication-input').blur();

        //Set read only inputs
        $('.associate-external-authentication-input').prop('readonly', true);

        //Disable submit button
        $('#associate-external-authentication-submit-btn').prop('disabled', true);
    }

    //Associate external authentication form form OnSuccess
    function AssociateExternalAuthenticationOnSuccess(json) {
        //Blur inputs
        $('.associate-external-authentication-input').blur();

        //Error
        if (json.message === undefined) {
            $('#error-msg').find('.alert-danger').html($('#associate-external-authentication-form').data("error"));
            $('#error-msg-container').css('display', '');
        }
        else {
            if (json.success) {
                if (json.returnURL != undefined) {
                    window.location.href = json.returnURL;
                }
                else {
                    window.location.href = '/Account/Index'
                }
                return false;
            }
            else {
                $('#error-msg').html(json.message);
                $('#error-msg-container').css('display', '');
            }
        }

        //Enable inputs
        $('.associate-external-authentication-input').prop('readonly', false);
        //Enable submit button
        $('.associate-external-authentication-input:submit').prop('disabled', false);

        //Reset formValidation
        $('#associate-external-authentication-form').data('formValidation').resetForm();

        //Reset form
        $('#associate-external-authentication-form').trigger("reset");

        //Show spinner
        $('#associate-external-authentication-icon').css('display', '');

        //Hide spinner
        $('#associate-external-authentication-spin').css('display', 'none');

        //Enable submit button
        $('#associate-external-authentication-submit-btn').prop('disabled', false);
    }

    //Associate external authentication form form OnError
    function AssociateExternalAuthenticationOnError() {
        //Blur inputs
        $('.associate-external-authentication-input').blur();

        //Show default error message
        $('#error-msg').html($('#associate-external-authentication-form').data("error"));
        $('#error-msg-container').css('display', '');

        //Enable inputs
        $('.associate-external-authentication-input').prop('readonly', false);
        //Enable submit button
        $('.associate-external-authentication-input:submit').prop('disabled', false);

        //Reset formValidation
        $('#associate-external-authentication-form').data('formValidation').resetForm();

        //Reset form
        $('#associate-external-authentication-form').trigger("reset");

        //Show spinner
        $('#associate-external-authentication-icon').css('display', '');

        //Hide spinner
        $('#associate-external-authentication-spin').css('display', 'none');

        //Enable submit button
        $('#associate-external-authentication-submit-btn').prop('disabled', false);
    }

    $('.btn-facebook, .btn-google').on('click', function () {
        //Disable button
        $(this).addClass('disabled');
        //Show spinner
        $(this).find('.spinner-modal').css('display', '');
        //Set margin for text
        $(this).find('.sm-text').css('margin-right', '-14px');
    });

    $('.btn-facebook').bind("touch", function (e) {
        //Disable button
        $('.modal-sign-in-facebook-btn').addClass('disabled');
        //Show spinner
        $('#modal-sign-in-facebook-spin').css('display', '');
        $('#modal-sign-in-page-load-facebook-spin').css('display', '');
        //Set margin for text
        $('.modal-sign-in-facebook-sm-text').css('margin-right', '-14px');

        var url = $(e.target).attr('href');

        window.location.href = url;
    });

    $('.account-settings-insert-authentication-provider-btn').on('click', function () {
        //Disable button
        $(this).addClass('disabled');
        //Set padding on btn
        $(this).css('padding-bottom', '4px');
        //Show spinner
        $(this).find('.spinner-button-default').css('display', '');
        //Set margin for text
        $(this).find('.account-settings-insert-authentication-provider-text').css('display', 'none');
    });

    $('.account-settings-insert-authentication-provider-btn').bind("touch", function (e) {
        //Disable button
        $(e.target).addClass('disabled');
        //Set padding on btn
        $(e.target).css('padding-bottom', '4px');
        //Show spinner
        $(e.target).find('.spinner-button-default').css('display', '');
        //Set margin for text
        $(e.target).find('.account-settings-insert-authentication-provider-text').css('display', 'none');
    });
});