$(function () {
    var isajaxrunning = false;

    //Validate delete provider form
    $('#modal-account-settings-remove-authentication-provider-form').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        locale: 'sv_SE',
        fields: {
            Password: {
                validators: {
                    notEmpty: {
                    },
                    stringLength: {
                        min: 6,
                        max: 150
                    }
                },
            }
        }
    }).on('success.form.fv', function (e) {
        e.preventDefault();

        var form = $('#modal-account-settings-remove-authentication-provider-form');

        $.ajax({
            url: '/User/DeleteAccountAuthenticationProvider/',
            data: $(form).serialize(),
            type: 'POST',
            beforeSend: DeleteAccountProviderOnBegin,
            success: DeleteAccountProviderOnSuccess,
            error: function () {
                DeleteAccountProviderOnError();
            },
            complete: function () {
                isajaxrunning = false;
            }
        });
    });

    function DeleteAccountProviderOnBegin() {
        isajaxrunning = true;

        //Show spinner and add margin to submit button text
        $('#modal-account-settings-remove-authentication-provider-spin').css('display', '');
        $('#modal-account-settings-remove-authentication-provider-text').css('margin-left', '17px');

        //Set read only input
        $('#modal-account-settings-remove-authentication-provider-password').prop('readonly', true);
        //Disable submit button
        $('#modal-account-settings-remove-authentication-provider-submitbtn').prop('disabled', true);
    }

    function DeleteAccountProviderOnSuccess(json) {
        //Blur inputs
        $('#modal-account-settings-remove-authentication-provider-form :input').blur();

        //Error
        if (json.message === undefined) {
            $('#account-settings-ajax-error-container').find('.alert-danger').html($('#modal-account-settings-remove-authentication-provider-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
            $('#account-settings-ajax-error-container').css('display', '');

            //Enable password input
            $('#modal-account-settings-remove-authentication-provider-password').prop('readonly', false);
            //Enable submit button
            $('#modal-account-settings-remove-authentication-provider-submitbtn').prop('disabled', false);

            //Reset formValidation
            $('#modal-account-settings-remove-authentication-provider-form').data('formValidation').resetForm();
            //Reset form
            $('#modal-account-settings-remove-authentication-provider-form').trigger("reset");

            //Hide spinner and reset margin
            $('#modal-account-settings-remove-authentication-provider-spin').css('display', 'none');
            $('#modal-account-settings-remove-authentication-provider-text').css('margin-left', '');

            $('#modal-account-settings-remove-authentication-provider').modal('hide');
        }
        else {
            if (json.success) {
                $.ajax({
                    url: '/Account/UpdateSettingsAuthenticationContent/',
                    type: 'GET',
                    success: function (data) {
                        $('#account-settings-authentication-container').html(data);

                        $("body").trigger("AccountSettingsReloadScript");

                        $('#account-settings-ajax-success-container').find('.alert-success').html(json.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');
                        $('#account-settings-ajax-success-container').css('display', '');

                        //Enable password input
                        $('#modal-account-settings-remove-authentication-provider-password').prop('readonly', false);
                        //Enable submit button
                        $('#modal-account-settings-remove-authentication-provider-submitbtn').prop('disabled', false);

                        //Reset formValidation
                        $('#modal-account-settings-remove-authentication-provider-form').data('formValidation').resetForm();
                        //Reset form
                        $('#modal-account-settings-remove-authentication-provider-form').trigger("reset");

                        //Hide spinner and reset margin
                        $('#modal-account-settings-remove-authentication-provider-spin').css('display', 'none');
                        $('#modal-account-settings-remove-authentication-provider-text').css('margin-left', '');

                        $('#modal-account-settings-remove-authentication-provider').modal('hide');
                    },
                    error: function () {
                        $('#account-settings-ajax-error-container').find('.alert-danger').html($('#modal-account-settings-remove-authentication-provider-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                        $('#account-settings-ajax-error-container').css('display', '');

                        //Enable password input
                        $('#modal-account-settings-remove-authentication-provider-password').prop('readonly', false);
                        //Enable submit button
                        $('#modal-account-settings-remove-authentication-provider-submitbtn').prop('disabled', false);

                        //Reset formValidation
                        $('#modal-account-settings-remove-authentication-provider-form').data('formValidation').resetForm();
                        //Reset form
                        $('#modal-account-settings-remove-authentication-provider-form').trigger("reset");

                        //Hide spinner and reset margin
                        $('#modal-account-settings-remove-authentication-provider-spin').css('display', 'none');
                        $('#modal-account-settings-remove-authentication-provider-text').css('margin-left', '');

                        $('#modal-account-settings-remove-authentication-provider').modal('hide');
                    }
                });
            }
            else {
                $('#account-settings-ajax-error-container').find('.alert-danger').html(json.message + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#account-settings-ajax-error-container').css('display', '');

                //Enable password input
                $('#modal-account-settings-remove-authentication-provider-password').prop('readonly', false);
                //Enable submit button
                $('#modal-account-settings-remove-authentication-provider-submitbtn').prop('disabled', false);

                //Reset formValidation
                $('#modal-account-settings-remove-authentication-provider-form').data('formValidation').resetForm();
                //Reset form
                $('#modal-account-settings-remove-authentication-provider-form').trigger("reset");

                //Hide spinner and reset margin
                $('#modal-account-settings-remove-authentication-provider-spin').css('display', 'none');
                $('#modal-account-settings-remove-authentication-provider-text').css('margin-left', '');

                $('#modal-account-settings-remove-authentication-provider').modal('hide');
            }
        }
    }

    function DeleteAccountProviderOnError() {
        //Blur inputs
        $('#modal-account-settings-remove-authentication-provider-form :input').blur();

        //Show default error message
        $('#account-settings-ajax-error-container').find('.alert-danger').html($('#modal-account-settings-remove-authentication-provider-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
        $('#account-settings-ajax-error-container').css('display', '');

        //Enable password input
        $('#modal-account-settings-remove-authentication-provider-password').prop('readonly', false);
        //Enable submit button
        $('#modal-account-settings-remove-authentication-provider-submitbtn').prop('disabled', false);

        //Reset formValidation
        $('#modal-account-settings-remove-authentication-provider-form').data('formValidation').resetForm();
        //Reset form
        $('#modal-account-settings-remove-authentication-provider-form').trigger("reset");

        //Hide spinner and reset margin
        $('#modal-account-settings-remove-authentication-provider-spin').css('display', 'none');
        $('#modal-account-settings-remove-authentication-provider-text').css('margin-left', '');
    }

    //Validate update email form
    $('#account-settings-email-form').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        locale: 'sv_SE',
        fields: {
            Password: {
                validators: {
                    notEmpty: {
                    },
                    stringLength: {
                        min: 6,
                        max: 150
                    }
                },
            },
            NewEmail: {
                validators: {
                    notEmpty: {
                    },
                    emailAddress: {
                    },
                    stringLength: {
                        min: 6,
                        max: 150
                    }
                },
            },
            ConfirmNewEmail: {
                validators: {
                    identical: {
                        field: 'NewEmail'
                    }
                }
            }
        }
    }).on('success.form.fv', function (e) {
        $('#update-email-submit-icon').css('display', 'none');
        $('#account-settings-email-spinner').css('display', '');
    });

    //Validate update password form
    $('#account-settings-password-form').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        locale: 'sv_SE',
        fields: {
            CurrentPassword: {
                validators: {
                    notEmpty: {
                    },
                    stringLength: {
                        min: 6,
                        max: 150,
                    }
                }
            },
            NewPassword: {
                validators: {
                    notEmpty: {
                    },
                    stringLength: {
                        min: 6,
                        max: 150
                    }
                }
            },
            ConfirmNewPassword: {
                validators: {
                    identical: {
                        field: 'NewPassword'
                    }
                }
            }
        }
    }).on('success.form.fv', function (e) {
        $('#update-password-submit-icon').css('display', 'none');
        $('#account-settings-password-spinner').css('display', '');
    });

    //Validate reset account form
    $('#account-settings-reset-form').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        locale: 'sv_SE',
        fields: {
            Password: {
                validators: {
                    notEmpty: {
                    },
                    stringLength: {
                        min: 6,
                        max: 150,
                    }
                },
            }
        }
    }).on('success.form.fv', function (e) {
        //Prevent form submission
        e.preventDefault();

        //Show confirmation modal
        $('#modal-account-settings-reset').modal('show');
    });

    //Validate delete account form
    $('#account-settings-delete-form').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        locale: 'sv_SE',
        fields: {
            Password: {
                validators: {
                    notEmpty: {
                    },
                    stringLength: {
                        min: 6,
                        max: 150,
                    }
                }
            }
        }
    }).on('success.form.fv', function (e) {
        //Prevent form submission
        e.preventDefault();

        //Show confirmation modal
        $('#modal-account-settings-delete').modal('show');
    });

    //Reset account
    $("#modal-account-settings-reset-submitbtn").on('click', function (e) {
        var form = $('#account-settings-reset-form');

        $.ajax({
            url: '/User/ResetUser/',
            data: $(form).serialize(),
            type: 'POST',
            beforeSend: ResetAccountOnBegin,
            success: ResetAccountOnSuccess,
            error: function () {
                ResetAccountOnError();
            },
            complete: function () {
                isajaxrunning = false;
            }
        });
    });

    //Reset account form OnBegin
    function ResetAccountOnBegin() {
        isajaxrunning = true;

        //Show spinner and add margin to submit button text
        $('#modal-account-settings-reset-spin').css('display', '');
        $('#modal-account-settings-reset-spin').prev().css('margin-left', '17px');

        //Set read only inputs
        $('#account-settings-reset-password').prop('readonly', true);
        //Disable submit button
        $('#account-settings-reset-submitbtn').prop('disabled', true);
        //Disable modal submit button
        $('#modal-account-settings-reset-submitbtn').prop('disabled', true);
    }

    //Reset account form OnSuccess
    function ResetAccountOnSuccess(json) {
        //Blur inputs
        $('#account-settings-reset-form :input').blur();

        //Error
        if (json.message === undefined) {
            $('#account-settings-ajax-error-container').find('.alert-danger').html($('#account-settings-reset-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
            $('#account-settings-ajax-error-container').css('display', '');
        }
        else {
            if (json.success) {
                //Clear account usage
                $('.list-group-item').find('.badge').text('0');
                $(".list-group-item[href*='/Account/Logs']").find('.badge').text('1');

                $('#account-settings-ajax-success-container').find('.alert-success').html(json.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');
                $('#account-settings-ajax-success-container').css('display', '');
            }
            else {
                $('#account-settings-ajax-error-container').find('.alert-danger').html(json.message + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#account-settings-ajax-error-container').css('display', '');
            }
        }

        //Enable password input
        $('#account-settings-reset-password').prop('readonly', false);
        //Enable submit button
        $('#account-settings-reset-submitbtn').prop('disabled', false);
        //Enable modal submit button
        $('#modal-account-settings-reset-submitbtn').prop('disabled', false);

        //Reset formValidation
        $('#account-settings-reset-form').data('formValidation').resetForm();
        //Reset form
        $('#account-settings-reset-form').trigger("reset");

        $('#modal-account-settings-reset').modal('hide');

        //Hide spinner and reset margin
        $('#modal-account-settings-reset-spin').css('display', 'none');
        $('#modal-account-settings-reset-spin').prev().css('margin-left', '');
    }

    //Reset account form OnError
    function ResetAccountOnError() {
        //Blur inputs
        $('#account-settings-reset-form :input').blur();

        //Show default error message
        $('#account-settings-ajax-error-container').find('.alert-danger').html($('#account-settings-reset-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
        $('#account-settings-ajax-error-container').css('display', '');

        //Enable password input
        $('#account-settings-reset-password').prop('readonly', false);
        //Enable submit button
        $('#account-settings-reset-submitbtn').prop('disabled', false);
        //Enable modal submit button
        $('#modal-account-settings-reset-submitbtn').prop('disabled', false);

        //Reset formValidation
        $('#account-settings-reset-form').data('formValidation').resetForm();
        //Reset form
        $('#account-settings-reset-form').trigger("reset");

        $('#modal-account-settings-reset').modal('hide');

        //Hide spinner and reset margin
        $('#modal-account-settings-reset-spin').css('display', 'none');
        $('#modal-account-settings-reset-spin').prev().css('margin-left', '');
    }

    //Delete account
    $("#modal-account-settings-delete-submitbtn").on('click', function () {
        var form = $('#account-settings-delete-form');

        var url;
        var accountOwner = $(this).hasClass('account-settings-delete-account-owner');

        if (accountOwner)
            url = '/User/DeleteUser/';
        else
            url = '/User/DeleteGuest/';

        $.ajax({
            url: url,
            data: $(form).serialize(),
            type: 'POST',
            beforeSend: DeleteAccountOnBegin,
            success: DeleteAccountOnSuccess,
            error: function () {
                DeleteAccountOnError();
            },
            complete: function () {
                isajaxrunning = false;
            }
        });
    });

    //Delete account form OnBegin
    function DeleteAccountOnBegin() {
        isajaxrunning = true;

        //Show spinner and add margin to submit button text
        $('#modal-account-settings-delete-spin').css('display', '');
        $('#modal-account-settings-delete-spin').prev().css('margin-left', '17px');

        //Set read only inputs
        $('#account-settings-delete-password').prop('readonly', true);
        //Disable submit button
        $('#account-settings-delete-submitbtn').prop('disabled', true);
        //Disable modal submit button
        $('#modal-account-settings-delete-submitbtn').prop('disabled', true);
    }

    //Reset account form OnSuccess
    function DeleteAccountOnSuccess(json) {
        //Blur inputs
        $('#account-settings-delete-form :input').blur();

        //Error
        if (json.message === undefined) {
            $('#account-settings-ajax-error-container').find('.alert-danger').html($('#account-settings-delete-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
            $('#account-settings-ajax-error-container').css('display', '');
        }
        else {
            if (json.success) {
                var redirecting = $('#modal-account-settings-delete-btntext').data('redirecting');
                $('#modal-account-settings-delete-btntext').text(redirecting);

                window.location.href = '/Home/Index';
            }
            else {
                $('#account-settings-ajax-error-container').find('.alert-danger').html(json.message + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#account-settings-ajax-error-container').css('display', '');
            }
        }

        //Enable password input
        $('#account-settings-delete-password').prop('readonly', false);
        //Enable submit button
        $('#account-settings-delete-submitbtn').prop('disabled', false);
        //Enable modal submit button
        $('#modal-account-settings-delete-submitbtn').prop('disabled', false);

        //Reset formValidation
        $('#account-settings-delete-form').data('formValidation').resetForm();
        //Reset form
        $('#account-settings-delete-form').trigger("reset");

        $('#modal-account-settings-delete').modal('hide');

        //Hide spinner and reset margin
        $('#modal-account-settings-delete-spin').css('display', 'none');
        $('#modal-account-settings-delete-spin').prev().css('margin-left', '');
    }

    //Reset account form OnError
    function DeleteAccountOnError() {
        //Blur inputs
        $('#account-settings-delete-form :input').blur();

        //Show default error message
        $('#account-settings-ajax-error-container').find('.alert-danger').html($('#account-settings-delete-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
        $('#account-settings-ajax-error-container').css('display', '');

        //Enable password input
        $('#account-settings-delete-password').prop('readonly', false);
        //Enable submit button
        $('#account-settings-delete-submitbtn').prop('disabled', false);
        //Enable modal submit button
        $('#modal-account-settings-delete-submitbtn').prop('disabled', false);

        //Reset formValidation
        $('#account-settings-delete-form').data('formValidation').resetForm();
        //Reset form
        $('#account-settings-delete-form').trigger("reset");

        $('#modal-account-settings-delete').modal('hide');

        //Hide spinner and reset margin
        $('#modal-account-settings-delete-spin').css('display', 'none');
        $('#modal-account-settings-delete-spin').prev().css('margin-left', '');
    }

    //Reset form when modal is closed
    $('#modal-account-settings-reset').bind('hide.bs.modal', function (event) {
        //Only reset form if ajax is not running
        if (!isajaxrunning) {
            var form = $('#account-settings-reset-form')[0];

            if (form != undefined) {
                //Reset form
                form.reset();
                //Reset formValidation
                $(form).data('formValidation').resetForm();
            }
        }
    });

    //Reset form when modal is closed
    $('#modal-account-settings-delete').bind('hide.bs.modal', function (event) {
        //Only reset form if ajax is not running
        if (!isajaxrunning) {
            var form = $('#account-settings-delete-form')[0];

            if (form != undefined) {
                //Reset form
                form.reset();
                //Reset formValidation
                $(form).data('formValidation').resetForm();
            }
        }
    });

    //Hide message from server when focus
    $(".account-settings-input").focus(function () {
        $('.account-settings-message-container').css('display', 'none');
    });

    $("body").on("AccountSettingsReloadScript", function () {
        $('.account-settings-remove-authentication-provider-btn').on('click', function () {
            var id = $(this).data('id');
            var name = $(this).data('name');
            var color = $(this).data('color');

            $('#modal-account-settings-remove-authentication-provider-id').val(id);
            $('#modal-account-settings-remove-authentication-provider-name').text(name);
            $('#modal-account-settings-remove-authentication-provider-name').css('color', color);

            $('.account-settings-message-container').css('display', 'none');

            $('#modal-account-settings-remove-authentication-provider').modal('show');
        });
    });

    $("body").trigger("AccountSettingsReloadScript");
});