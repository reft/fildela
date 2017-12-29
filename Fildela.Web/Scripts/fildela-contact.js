(function () {
    'use strict';

    //Validate post mail form
    $('#contact-mail-form').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        locale: 'sv_SE',
        fields: {
            Name: {
                validators: {
                    notEmpty: {
                    },
                    stringLength: {
                        min: 6,
                        max: 150
                    }
                }
            },
            Email: {
                validators: {
                    notEmpty: {
                    },
                    emailAddress: {
                    }
                }
            },
            CategoryID: {
                validators: {
                    notEmpty: {
                    },
                    min: 1
                }
            },
            Message: {
                validators: {
                    notEmpty: {
                    },
                    stringLength: {
                        min: 25,
                        max: 5000
                    },
                }
            }
        }
    }).on('success.form.fv', function (e) {
        $('#contact-post-submit-icon').css('display', 'none');
        $('#contact-post-spinner').css('display', '');
        $('.contact-post-input').prop('readonly', true);
        $('#contact-category').prop('disabled', true);
    });

    //Hide message from server
    $(".contact-post-input").focus(function () {
        $('.contact-message-container').css('display', 'none');
    });

    $('#contact-category').on('change', function () {
        $('#contact-category-hidden').val($(this).val());
    });
})(jQuery);