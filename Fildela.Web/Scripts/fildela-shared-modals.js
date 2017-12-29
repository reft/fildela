$(function () {
    var sethtmlpos = 0;
    var modalfocused = false;
    var isajaxrunning = false;
    var isajaxaccountlinksrunning = false;

    //Validate sign in form
    $('#modal-sign-in-form').formValidation({
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

        var url;
        var usertype = $('#modal-sign-in-input-select').val();
        var type = '';

        if (usertype == 1) {
            url = '/User/SignInUser/';
            type = 'POST';
        }
        else {
            url = '/User/GetAccountLinksForGuest/';
            type = 'GET';
        }

        $.ajax({
            url: url,
            data: $(e.target).serialize(),
            type: type,
            beforeSend: SignInOnBegin,
            success: SignInOnSuccess,
            error: function () {
                SignInOnError();
            },
            complete: function () {
                isajaxrunning = false;
            }
        });
    });

    //Validate sign in page load form (REDIRECT)
    $('#modal-sign-in-page-load-form').formValidation({
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

        var url;
        var usertype = $('#modal-sign-in-page-load-input-select').val();

        if (usertype == 1)
            url = '/User/SignInUser/';
        else
            url = '/User/GetAccountLinksForGuest/';

        $.ajax({
            url: url,
            data: $(e.target).serialize(),
            type: 'POST',
            beforeSend: SignInPageLoadOnBegin,
            success: SignInPageLoadOnSuccess,
            error: function () {
                SignInPageLoadOnError();
            },
            complete: function () {
                isajaxrunning = false;
            }
        });
    });

    //Validate reset password form
    $('#modal-reset-password-form').formValidation({
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
            }
        }
    }).on('success.form.fv', function (e) {
        //Prevent form submission
        e.preventDefault();

        var url;
        var usertype = $('#modal-reset-password-input-select').val();

        if (usertype == 1)
            url = '/User/ResetPasswordUser/';
        else
            url = '/User/ResetPasswordGuest/';

        $.ajax({
            url: url,
            data: $(e.target).serialize(),
            type: 'POST',
            beforeSend: ResetPasswordOnBegin,
            success: ResetPasswordOnSuccess,
            error: function () {
                ResetPasswordOnError();
            },
            complete: function () {
                isajaxrunning = false;
            }
        });
    });

    //Validate register form
    $('#modal-register-form').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        locale: 'sv_SE',
        fields: {
            'AgreeUserAgreement': {
                validators: {
                    notEmpty: {
                    }
                }
            },
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
            ConfirmEmail: {
                validators: {
                    identical: {
                        field: 'Email'
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
            },
            ConfirmPassword: {
                validators: {
                    identical: {
                        field: 'Password'
                    }
                }
            }
        }
    }).on('success.form.fv', function (e) {
        //Prevent form submission
        e.preventDefault();

        $.ajax({
            url: '/User/Register/',
            data: $(e.target).serialize(),
            type: 'POST',
            beforeSend: RegisterOnBegin,
            success: RegisterOnSuccess,
            error: function () {
                RegisterOnError();
            },
            complete: function () {
                isajaxrunning = false;
            }
        });
    });

    //Register form OnBegin
    function RegisterOnBegin() {
        isajaxrunning = true;

        //Show spinner and add margin to submit button text
        $('#modal-register-submit-spin').css('display', '');
        $('#modal-register-submit-spin').prev().css('margin-left', '17px');

        //Blur inputs
        $('.modal-register-input').blur();

        //Set read only inputs
        $('.modal-register-input').prop('readonly', true);
        //Disable submit button
        $('.modal-register-input:submit').prop('disabled', true);
        //Disable agreement checkbox
        $('.modal-register-input:checkbox').prop('disabled', true);

        //Set default cursor for agreement text
        $('#modal-register-agreement-text-container').find('label').css('cursor', 'default');
    }

    //Register form OnSuccess
    function RegisterOnSuccess(json) {
        //Blur inputs
        $('.modal-register-input').blur();

        //Error
        if (json.message === undefined) {
            $('#modal-register-error-container').find('.alert-danger').html($('#modal-register-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
            $('#modal-register-error-container').css('display', '');
        }
        else {
            //Show message
            if (json.success) {
                $('#modal-register-success-container').find('.alert-success').html(json.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');
                $('#modal-register-success-container').css('display', '');
            }
            else {
                $('#modal-register-error-container').find('.alert-danger').html(json.message + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#modal-register-error-container').css('display', '');
            }
        }

        //Enable inputs
        $('.modal-register-input').prop('readonly', false);
        //Enable submit button
        $('.modal-register-input:submit').prop('disabled', false);
        //Enable agreement checkbox
        $('.modal-register-input:checkbox').prop('disabled', false);
        //Reset cursor for agreement text
        $('#modal-register-agreement-text-container').find('label').css('cursor', '');

        //Reset formValidation
        $('#modal-register-form').data('formValidation').resetForm();
        //Reset form
        $('#modal-register-form').trigger("reset");

        //Hide spinner and reset margin
        $('#modal-register-submit-spin').css('display', 'none');
        $('#modal-register-submit-spin').prev().css('margin-left', '');
    }

    //Register form OnError
    function RegisterOnError() {
        //Blur inputs
        $('.modal-register-input').blur();

        //Show default error message
        $('#modal-register-error-container').find('.alert-danger').html($('#modal-register-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
        $('#modal-register-error-container').css('display', '');

        //Enable inputs
        $('.modal-register-input').prop('readonly', false);
        //Enable submit button
        $('.modal-register-input:submit').prop('disabled', false);
        //Enable agreement checkbox
        $('.modal-register-input:checkbox').prop('disabled', false);
        //Reset cursor for agreement text
        $('#modal-register-agreement-text-container').find('label').css('cursor', '');

        //Reset formValidation
        $('#modal-register-form').data('formValidation').resetForm();
        //Reset form
        $('#modal-register-form').trigger("reset");

        //Hide spinner and reset margin
        $('#modal-register-submit-spin').css('display', 'none');
        $('#modal-register-submit-spin').prev().css('margin-left', '');
    }

    //Remove disabled on checkbox so data is submitted
    $('#modal-register-form').on('submit', function () {
        $("#modal-register-checkbox").prop("disabled", false);
    });

    //Sign in form OnBegin
    function SignInOnBegin() {
        isajaxrunning = true;

        //Show spinner and add margin to submit button text
        $('#modal-sign-in-submit-spin').css('display', '');
        $('#modal-sign-in-submit-spin').prev().css('margin-left', '17px');

        //Blur inputs
        $('.modal-sign-in-input').blur();

        //Set read only inputs
        $('.modal-sign-in-input').prop('readonly', true);
        //Disable submit button
        $('.modal-sign-in-input:submit').prop('disabled', true);
        //Disable select button
        $("#modal-sign-in-input-select").prop("disabled", true);
    }

    //Sign in form OnSuccess
    function SignInOnSuccess(json) {
        var validateField = false;

        //Blur inputs
        $('.modal-sign-in-input').blur();

        //Error
        if (json.message === undefined) {
            $('#modal-sign-in-error-container').find('.alert-danger').html($('#modal-sign-in-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
            $('#modal-sign-in-error-container').css('display', '');
        }
        else {
            if (json.success) {
                if (json.viewString === undefined) {
                    var status = $('#modal-sign-in-btn-text').data('redirecting');
                    $('#modal-sign-in-btn-text').text(status);

                    //Sign in ACCOUNT OWNER success, redirect to account page
                    if (json.returnUrl != undefined)
                        window.location.href = json.returnUrl;
                    else
                        window.location.href = '/Account/Index'

                    return false;
                }
                else {
                    //Guest is signing in, update view with accountlinks
                    $('#modal-sign-in-account-links-container').html(json.viewString);

                    isajaxrunning = false;

                    $('#modal-sign-in').modal('hide');
                    $('#modal-sign-in-account-links').modal('show');

                    $('#modal-sign-in-account-links-success-container').find('.alert-success').html(json.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');
                    $('#modal-sign-in-account-links-success-container').css('display', '');

                    $("#modal-sign-in-input-select").val("0");
                    $("body").trigger("SignInGuest");
                }
            }
            else {
                validateField = true;

                $('#modal-sign-in-error-container').find('.alert-danger').html(json.message + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#modal-sign-in-error-container').css('display', '');
            }
        }

        //Enable inputs
        $('#modal-sign-in-form input').prop('readonly', false);
        //Enable submit button
        $('.modal-sign-in-input:submit').prop('disabled', false);
        //Enable input select
        $("#modal-sign-in-input-select").prop("disabled", false);

        //Reset formValidation
        $('#modal-sign-in-form').data('formValidation').resetForm();
        //Reset password field
        $('#modal-sign-in-password').val('');

        //Hide spinner and reset margin
        $('#modal-sign-in-submit-spin').css('display', 'none');
        $('#modal-sign-in-submit-spin').prev().css('margin-left', '');

        if (validateField) {
            $('#modal-sign-in-form').formValidation('revalidateField', 'Email');
        }
    }

    //Sign in form OnError
    function SignInOnError() {
        //Blur inputs
        $('.modal-sign-in-input').blur();

        //Show default error message
        $('#modal-sign-in-error-container').find('.alert-danger').html($('#modal-register-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
        $('#modal-sign-in-error-container').css('display', '');

        //Enable inputs
        $('#modal-sign-in-form input').prop('readonly', false);
        //Enable submit button
        $('.modal-sign-in-input:submit').prop('disabled', false);
        //Enable input select
        $("#modal-sign-in-input-select").prop("disabled", false);

        //Reset formValidation
        $('#modal-sign-in-form').data('formValidation').resetForm();
        //Reset password field
        $('#modal-sign-in-password').val('');

        //Hide spinner and reset margin
        $('#modal-sign-in-submit-spin').css('display', 'none');
        $('#modal-sign-in-submit-spin').prev().css('margin-left', '');
    }

    //Sign in page load form OnBegin
    function SignInPageLoadOnBegin() {
        isajaxrunning = true;

        //Show spinner and add margin to submit button text
        $('#modal-sign-in-page-load-submit-spin').css('display', '');
        $('#modal-sign-in-page-load-submit-spin').prev().css('margin-left', '17px');

        //Blur inputs
        $('.modal-sign-in-page-load-input').blur();

        //Set read only inputs
        $('.modal-sign-in-page-load-input').prop('readonly', true);
        //Disable submit button
        $('.modal-sign-in-page-load-input:submit').prop('disabled', true);
        //Disable select button
        $("#modal-sign-in-page-load-input-select").prop("disabled", true);
    }

    //Remove disabled on input selects so data is submitted
    $('#modal-ign-in-page-load-form').on('submit', function () {
        $("#modal-ign-in-page-load-input-select").prop("disabled", false);
    });

    //Sign in page load form OnSuccess
    function SignInPageLoadOnSuccess(json) {
        var validateField = false;

        //Blur inputs
        $('.modal-sign-in-page-load-input').blur();

        //Error
        if (json.message === undefined) {
            $('#modal-sign-in-page-load-error-container').find('.alert-danger').html($('#modal-sign-in-page-load-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
            $('#modal-sign-in-page-load-error-container').css('display', '');
        }
        else {
            if (json.success) {
                if (json.viewString === undefined) {
                    var status = $('#modal-sign-in-page-load-btn-text').data('redirecting');
                    $('#modal-sign-in-page-load-btn-text').text(status);

                    //Sign in ACCOUNT OWNER success, redirect to account page
                    if (json.returnUrl != undefined)
                        window.location.href = json.returnUrl;
                    else
                        window.location.href = '/Account/Index'

                    return false;
                }
                else {
                    //Guest is signing in, update view with accountlinks
                    $('#modal-sign-in-account-links-container').html(json.viewString);

                    isajaxrunning = false;

                    $('#modal-sign-in-page-load').modal('hide');
                    $('#modal-sign-in-account-links').modal('show');

                    $('#modal-sign-in-account-links-success-container').find('.alert-success').html(json.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');
                    $('#modal-sign-in-account-links-success-container').css('display', '');

                    $("#modal-sign-in-input-select").val("0");
                    $("body").trigger("SignInGuest");
                }
            }
            else {
                validateField = true;

                $('#modal-sign-in-page-load-error-container').find('.alert-danger').html(json.message + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#modal-sign-in-page-load-error-container').css('display', '');
            }
        }

        //Enable inputs
        $('#modal-sign-in-page-load-form input').prop('readonly', false);
        //Enable submit button
        $('.modal-sign-in-page-load-input:submit').prop('disabled', false);
        //Enable input select
        $("#modal-sign-in-page-load-input-select").prop("disabled", false);

        //Reset formValidation
        $('#modal-sign-in-page-load-form').data('formValidation').resetForm();
        //Reset password field
        $('#modal-sign-in-page-load-password').val('');

        //Hide spinner and reset margin
        $('#modal-sign-in-page-load-submit-spin').css('display', 'none');
        $('#modal-sign-in-page-load-submit-spin').prev().css('margin-left', '');

        if (validateField) {
            $('#modal-sign-in-page-load-form').formValidation('revalidateField', 'Email');
        }
    }

    //Sign in page load form OnError
    function SignInPageLoadOnError() {
        //Blur inputs
        $('.modal-sign-in-page-load-input').blur();

        //Show default error message
        $('#modal-sign-in-page-load-error-container').find('.alert-danger').html($('#modal-register-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
        $('#modal-sign-in-page-load-error-container').css('display', '');

        //Enable inputs
        $('#modal-sign-in-page-load-form input').prop('readonly', false);
        //Enable submit button
        $('.modal-sign-in-page-load-input:submit').prop('disabled', false);
        //Enable input select
        $("#modal-sign-in-page-load-input-select").prop("disabled", false);

        //Reset formValidation
        $('#modal-sign-in-page-load-form').data('formValidation').resetForm();
        //Reset password field
        $('#modal-sign-in-page-load-password').val('');

        //Hide spinner and reset margin
        $('#modal-sign-in-page-load-submit-spin').css('display', 'none');
        $('#modal-sign-in-page-load-submit-spin').prev().css('margin-left', '');
    }

    //Reset password form OnBegin
    function ResetPasswordOnBegin() {
        isajaxrunning = true;

        //Show spinner and add margin to submit button text
        $('#modal-reset-password-submit-spin').css('display', '');
        $('#modal-reset-password-submit-spin').prev().css('margin-left', '17px');

        //Blur inputs
        $('.modal-reset-password-input').blur();

        //Set read only inputs
        $('.modal-reset-password-input').prop('readonly', true);
        //Disable submit button
        $('.modal-reset-password-input:submit').prop('disabled', true);
        //Disable select button
        $("#modal-reset-password-input-select").prop("disabled", true);
    }

    //Reset password form OnSuccess
    function ResetPasswordOnSuccess(json) {
        var validateField = false;

        //Blur inputs
        $('.modal-reset-password-input').blur();

        //Error
        if (json.message === undefined) {
            $('#modal-reset-password-error-container').find('.alert-danger').html($('#modal-reset-password-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
            $('#modal-reset-password-error-container').css('display', '');
        }
        else {
            //Show message
            if (json.success) {
                $('#modal-reset-password-success-container').find('.alert-success').html(json.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');
                $('#modal-reset-password-success-container').css('display', '');
            }
            else {
                $('#modal-reset-password-error-container').find('.alert-danger').html(json.message + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#modal-reset-password-error-container').css('display', '');
            }
        }

        //Enable inputs
        $('#modal-reset-password-form input').prop('readonly', false);
        //Enable submit button
        $('.modal-reset-password-input:submit').prop('disabled', false);
        //Enable input select
        $("#modal-reset-password-input-select").prop("disabled", false);

        //Reset formValidation
        $('#modal-reset-password-form').data('formValidation').resetForm();
        //Reset form
        $('#modal-reset-password-form').trigger("reset");

        //Hide spinner and reset margin
        $('#modal-reset-password-submit-spin').css('display', 'none');
        $('#modal-reset-password-submit-spin').prev().css('margin-left', '');
    }

    //Reset password form OnError
    function ResetPasswordOnError() {
        //Blur inputs
        $('.modal-reset-password-input').blur();

        //Show default error message
        $('#modal-reset-password-error-container').find('.alert-danger').html($('#modal-register-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
        $('#modal-reset-password-error-container').css('display', '');

        //Enable inputs
        $('#modal-reset-password-form input').prop('readonly', false);
        //Enable submit button
        $('.modal-reset-password-input:submit').prop('disabled', false);
        //Remove disabled on input selects so data is submitted
        $("#modal-reset-password-input-select").prop("disabled", false);

        //Reset formValidation
        $('#modal-reset-password-form').data('formValidation').resetForm();
        //Reset form
        $('#modal-reset-password-form').trigger("reset");

        //Hide spinner and reset margin
        $('#modal-reset-password-submit-spin').css('display', 'none');
        $('#modal-reset-password-submit-spin').prev().css('margin-left', '');
    }

    //Hide message from server when focus
    $(".modal-sign-in-page-load-input").focus(function () {
        $('#modal-sign-in-page-load-default-error-container').css('display', 'none');
        $('#modal-sign-in-page-load-error-container').css('display', 'none');
    });

    $(".modal-sign-in-input").focus(function () {
        $('#modal-sign-in-success-container').css('display', 'none');
        $('#modal-sign-in-error-container').css('display', 'none');
    });

    $(".modal-register-input").focus(function () {
        $('#modal-register-success-container').css('display', 'none');
        $('#modal-register-error-container').css('display', 'none');
    });

    $(".modal-reset-password-input").focus(function () {
        $('#modal-reset-password-success-container').css('display', 'none');
        $('#modal-reset-password-error-container').css('display', 'none');
    });

    //Hide message from server when close
    $('#modal-sign-in').bind('hidden.bs.modal', function () {
        $('#modal-sign-in-error-container').css('display', 'none');
    });

    $('#modal-sign-in-page-load').bind('hidden.bs.modal', function () {
        $('#modal-sign-in-page-load-error-container').css('display', 'none');
    });

    $('#modal-register').bind('hidden.bs.modal', function () {
        $('#modal-register-success-container').css('display', 'none');
        $('#modal-register-error-container').css('display', 'none');
    });

    $('#modal-reset-password').bind('hidden.bs.modal', function () {
        $('#modal-reset-password-success-container').css('display', 'none');
        $('#modal-reset-password-error-container').css('display', 'none');
    });

    $('#modal-direct-upload').bind('hidden.bs.modal', function () {
        $('#modal-upload-directly-error-container').css('display', 'none');
        $('#modal-upload-directly-container-idle').find('.modal-content-text').css('display', '');
    });

    $('#modal-direct-upload-complete').bind('hidden.bs.modal', function () {
        $('#modal-upload-directly-complete-error-container').css('display', 'none');
        $('#modal-upload-directly-complete-success-container').css('display', 'none');
    });

    $('#modal-account-files-upload').bind('hidden.bs.modal', function () {
        $('#modal-account-files-upload-pretext').css('display', '');
        $('#modal-account-files-upload-success-container').css('display', 'none');
        $('#modal-account-files-upload-error-container').css('display', 'none');
    });

    //Reset form when modal is closed
    $('#modal-sign-in, #modal-register, #modal-reset-password, #modal-sign-in-page-load').bind('hide.bs.modal', function (event) {
        //Only reset form if ajax is not running
        if (!isajaxrunning) {
            var form = $(this).find('form')[0];
            if (form != undefined) {
                $(form).data('formValidation').resetForm();
                //Reset form
                form.reset();
            }
        }
    });

    $('#modal-sign-in').bind('shown.bs.modal', function (event) {
        if (!isajaxrunning) {
            var form = $(this).find('form')[0];

            if (form != undefined) {
                //Reset formValidation
                $(form).data('formValidation').resetForm();
                //Reset form
                form.reset();
            }
        }
    });

    $('#modal-sign-in-page-load').bind('shown.bs.modal', function (event) {
        if (!isajaxrunning) {
            var form = $(this).find('form')[0];

            if (form != undefined) {
                //Reset formValidation
                $(form).data('formValidation').resetForm();
                //Reset form
                form.reset();
            }
        }
    });

    //Reset form after modal is opened
    $('#modal-reset-password').bind('shown.bs.modal', function (event) {
        //Only reset form if ajax is not running
        if (!isajaxrunning) {
            var form = $(this).find('form')[0];

            if (form != undefined) {
                //Reset formValidation
                $(form).data('formValidation').resetForm();
                //Reset form
                form.reset();
            }
        }
    });

    //Reset form after modal is opened
    $('#modal-register').bind('shown.bs.modal', function (event) {
        //Only reset form if ajax is not running
        if (!isajaxrunning) {
            var form = $(this).find('form')[0];

            if (form != undefined) {
                //Reset formValidation
                $(form).data('formValidation').resetForm();
                //Reset form
                form.reset();
            }
        }
    });

    //Close event for useragreement modal
    $('#modal-useragreement').bind('hidden.bs.modal', function (event) {
        //Prevent shifting when closing this modal
        $("body").addClass("modal-open");

        //Prevent shifting on all but IE and mobiles
        if (navigator.userAgent.indexOf("Trident/") > -1 == false && navigator.userAgent.indexOf("MSIE ") > -1 == false && $('body').hasClass('mobile') == false) {
            $('body').css('padding-right', '17px');
        }

        $('#modal-register').focus();
    });

    //Upload directly modal, focus modal so ESC button works after file select
    $('#modal-direct-upload-file').click(function (event) {
        if (!$('body').hasClass('mobile')) {
            $('#modal-direct-upload').blur();
            $('#modal-direct-upload-file').blur();
            modalfocused = true;
        }
    });
    $('#modal-direct-upload-file').change(function (event) {
        if (!$('body').hasClass('mobile'))
            $('#modal-direct-upload').focus();
    });
    $(document).on('keyup.dismiss.modal', function (e) {
        if (e.which == 27 && modalfocused == true && $('body').hasClass('mobile') == false) {
            $('#modal-direct-upload').focus();
            modalfocused = false;
        }
    })

    //Hide account, dropdown menu when dropdown menu item is clicked
    $('.modal-account-from-dropdown').bind('show.bs.modal', function () {
        $("#account-files-contextmenu").hide();
        $("#account-links-contextmenu").hide();
        $("#account-guestaccounts-contextmenu").hide();
        $("#account-log-contextmenu").hide();
    });

    //Focus file input when modal is opened
    $('#modal-direct-upload').on('shown.bs.modal', function () {
        if (!$('body').hasClass('mobile'))
            $('#modal-direct-upload-file').focus();
    });

    //Swap from current modal to new modal
    $(".modal-sign-in-reset-password").click(function () {
        $('#modal-sign-in').modal('hide');
        $('#modal-reset-password').modal('show');
    });

    $(".modal-sign-in-register").click(function () {
        $('#modal-sign-in').modal('hide');
        $('#modal-register').modal('show');
    });

    $(".modal-sign-in-page-load-reset-password").click(function () {
        $('#modal-sign-in-page-load').modal('hide');
        $('#modal-reset-password').modal('show');
    });

    $(".modal-sign-in-page-load-register").click(function () {
        $('#modal-sign-in-page-load').modal('hide');
        $('#modal-register').modal('show');
    });

    $(".modal-reset-password-sign-in").click(function () {
        $('#modal-reset-password').modal('hide');
        $('#modal-sign-in').modal('show');
    });

    $(".modal-register-sign-in").click(function () {
        $('#modal-register').modal('hide');
        $('#modal-sign-in').modal('show');
    });

    $('#modal-useragreement-back').click(function () {
        $('#modal-useragreement').modal('hide');
    });

    //Destroy html content when modal is closed
    $('#modal-sign-in-account-links').bind('hidden.bs.modal', function () {
        if (!isajaxrunning && !isajaxaccountlinksrunning)
            $('#modal-sign-in-account-links-container').empty();
    });

    //Center account links modal
    $('#modal-sign-in-account-links').on('show.bs.modal', function () {
        var t = $(this),
            d = t.find('.modal-dialog'),
            dh = 320,
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

    $('#modal-sign-in, #modal-sign-in-page-load').bind('show.bs.modal', function (event) {
        //Only reset form if ajax is not running
        if (isajaxaccountlinksrunning) {
            $('#modal-sign-in').modal('hide');
            $('#modal-sign-in-page-load').modal('hide');

            $('#modal-sign-in-account-links').modal('show');
            return false;
        }
    });

    //Sign in account links form OnBegin
    function SignInAccountLinksOnBegin() {
        isajaxrunning = true;
        isajaxaccountlinksrunning = true;

        //Show spinner and add margin to submit button text
        $('#modal-sign-in-account-links-submit-spin').css('display', '');
        $('#modal-sign-in-account-links-submit-spin').prev().css('margin-left', '17px');

        //Blur inputs
        $('.modal-sign-in-account-links-input').blur();

        //Disable submit button
        $('.modal-sign-in-account-links-input:submit').prop('disabled', true);
        //Disable select button
        $("#modal-sign-in-account-links-input-select").prop("disabled", true);
    }

    //Remove disabled on inputs so data is submitted
    $('#modal-sign-in-account-links-form').on('submit', function () {
        $("#modal-sign-in-account-links-input-select").prop("disabled", false);
    });

    //Sign in account links form OnSuccess
    function SignInAccountLinksOnSuccess(json) {
        //Blur inputs
        $('.modal-sign-in-account-links-input').blur();

        //Error
        if (json.message === undefined) {
            $('#modal-sign-in-account-links-error-container').find('.alert-danger').html($('#modal-sign-in-account-links-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
            $('#modal-sign-in-account-links-error-container').css('display', '');
        }
        else {
            if (json.success) {
                var status = $('#modal-sign-in-account-links-btn-text').data('redirecting');
                $('#modal-sign-in-account-links-btn-text').text(status);

                if (json.returnUrl != undefined)
                    window.location.href = json.returnUrl;
                else
                    window.location.href = '/Account/Index'

                return false;
            }
            else {
                //Hide success message
                $('#modal-sign-in-account-links-success-container').css('display', 'none');

                //Show error message
                $('#modal-sign-in-account-links-error-container').find('.alert-danger').html(json.message + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#modal-sign-in-account-links-error-container').css('display', '');
            }
        }

        //Enable submit button
        $('.modal-sign-in-account-links-input:submit').prop('disabled', false);
        //Enable input select
        $("#modal-sign-in-account-links-input-select").prop("disabled", false);

        //Reset formValidation
        $('#modal-sign-in-account-links-form').data('formValidation').resetForm();
        //Reset form
        $('#modal-sign-in-account-links-form').trigger("reset");

        //Hide spinner and reset margin
        $('#modal-sign-in-account-links-submit-spin').css('display', 'none');
        $('#modal-sign-in-account-links-submit-spin').prev().css('margin-left', '');
    }

    //Sign in account links form OnError
    function SignInAccountLinksOnError() {
        //Blur inputs
        $('.modal-sign-in-account-links-input').blur();

        //Hide success message
        $('#modal-sign-in-account-links-success-container').css('display', 'none');

        //Show default error message
        $('#modal-sign-in-account-links-error-container').find('.alert-danger').html($('#modal-sign-in-account-links-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
        $('#modal-sign-in-account-links-error-container').css('display', '');

        //Enable submit button
        $('.modal-sign-in-account-links-input:submit').prop('disabled', false);
        //Enable input select
        $("#modal-sign-in-account-links-input-select").prop("disabled", false);

        //Reset formValidation
        $('#modal-sign-in-account-links-form').data('formValidation').resetForm();
        //Reset form
        $('#modal-sign-in-account-links-form').trigger("reset");

        //Hide spinner and reset margin
        $('#modal-sign-in-account-links-submit-spin').css('display', 'none');
        $('#modal-sign-in-account-links-submit-spin').prev().css('margin-left', '');
    }

    $('.btn-google').bind("touchend", function (e) {
        //Disable button
        $('.modal-sign-in-google-btn').addClass('disabled');
        //Show spinner
        $('#modal-sign-in-google-spin').css('display', '');
        $('#modal-sign-in-page-load-google-spin').css('display', '');
        //Set margin for text
        $('.modal-sign-in-google-sm-text').css('margin-right', '-14px');

        var url = $(e.target).attr('href');

        window.location.href = url;
    });

    $("body").on("SignInGuest", function () {
        //Set color for contact, select input
        function colorizeContactSelect() {
            if ($(this).val() === "0" || $(this).val() === null)
                $(this).addClass("select-first");
            else
                $(this).removeClass("select-first")

            $('#modal-sign-in-input-select option:first-child').attr('disabled', 'disabled');
        }

        //Set select color
        $("#modal-sign-in-account-links-input-select").on('change keyup', colorizeContactSelect).change();

        //Validate sign in account links form
        $('#modal-sign-in-account-links-form').formValidation({
            framework: 'bootstrap',
            icon: {
                valid: 'glyphicon glyphicon-ok',
                invalid: 'glyphicon glyphicon-remove',
                validating: 'glyphicon glyphicon-refresh'
            },
            locale: 'sv_SE',
            fields: {
                AccountOwnerID: {
                    validators: {
                        notEmpty: {
                        },
                        min: 1
                    }
                },
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
                url: '/User/SignInGuest/',
                data: $(e.target).serialize(),
                type: 'POST',
                beforeSend: SignInAccountLinksOnBegin,
                success: SignInAccountLinksOnSuccess,
                error: function () {
                    SignInAccountLinksOnError();
                },
                complete: function () {
                    isajaxrunning = false;
                    isajaxaccountlinksrunning = false;
                }
            });

            $('#modal-sign-in-account-links').each(function () {
                var t = $('#modal-sign-in-account-links'),
                    d = t.find('.modal-dialog'),
                    fadeClass = (t.is('.fade') ? 'fade' : '');

                // read and store dialog height
                d.data('height', d.height());
            });

            //Phase two - set margin-top on every dialog show
            $('#modal-sign-in-account-links').on('show.bs.modal', function () {
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
        });
    });

    $("body").trigger("SignInGuest");
});