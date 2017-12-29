$(function () {
    var isajaxrunning = false;
    var interval;

    //Validate account guestaccounts create
    $('#modal-account-guestaccounts-create-form').formValidation({
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
            ConfirmEmail: {
                validators: {
                    identical: {
                        field: 'Email'
                    }
                }
            },
            DateStart: {
                validators: {
                    notEmpty: {
                    },
                    stringLength: {
                        min: 16,
                        max: 16
                    }
                }
            },
            DateExpires: {
                validators: {
                    notEmpty: {
                    },
                    stringLength: {
                        min: 16,
                        max: 16
                    }
                }
            }
        }
    }).on('success.form.fv', function (e) {
        //Prevent form submission
        e.preventDefault();

        $.ajax({
            url: '/Account/InsertAccountLink/',
            data: $(e.target).serialize(),
            type: 'POST',
            beforeSend: InsertAccountLinkOnBegin,
            success: InsertAccountLinkOnSuccess,
            error: function () {
                $('#modal-account-guestaccounts-create-error-container').find('.alert-danger').html($('#modal-account-guestaccounts-create-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#modal-account-guestaccounts-create-error-container').css('display', '');

                InsertAccountLinkOnError();
            },
            complete: function () {
                isajaxrunning = false;
            }
        });
    });

    $('#modal-account-guestaccounts-remove-form').formValidation({
    }).on('success.form.fv', function (e) {
        //Prevent form submission
        e.preventDefault();

        var rows = $('.selrow');

        var guestIDs = new Array();
        $(rows).each(function (n) {
            guestIDs[n] = $(this).data('guestid');
        });

        $.ajax({
            url: '/Account/DeleteAccountLinks/',
            data: AddAntiForgeryToken({ GuestIDs: guestIDs }),
            type: 'POST',
            traditional: true,
            dataType: "json",
            beforeSend: DeleteAccountLinksOnBegin,
            success: function (result) {
                if (result.success) {
                    $.ajax({
                        url: '/Account/UpdateGuestAccountsContent/',
                        type: 'GET',
                        success: function (data) {
                            $('#account-guestaccounts-content').html(data);

                            mySorted = new SortedTable();

                            //Reload scripts
                            $.getScript('/Scripts/fildela-account-all.js');
                            $("body").trigger("AccountGuestAccountsReloadScript");

                            //Load search table script
                            LoadSearchTable();

                            DeleteAccountLinksOnComplete();

                            $('#modal-account-guestaccounts-remove-error-container').css('display', 'none');

                            $('#modal-account-guestaccounts-remove-success-container').css('display', '');
                            $('#modal-account-guestaccounts-remove-success-container').find('.alert-success').html(result.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');

                            $('#modal-account-guestaccounts-remove-container-before').css('display', 'none');
                            $('#modal-account-guestaccounts-remove-container-after').css('display', '');

                            var counter = 5;
                            var message = $('#modal-account-guestaccounts-remove-closing-btn').data('message');
                            interval = setInterval(function () {
                                counter--;

                                $('#modal-account-guestaccounts-remove-closing-btn').text(message + ' ' + counter);

                                if (counter == 0) {
                                    $('#modal-account-guestaccounts-remove').modal('hide');

                                    clearInterval(interval);
                                }
                            }, 1000);
                        },
                        error: function () {
                            DeleteAccountLinksOnComplete();

                            $('#modal-account-guestaccounts-remove-text').css('display', 'none');

                            $('#modal-account-guestaccounts-remove-error-container').find('.alert-danger').html($('#modal-account-guestaccounts-remove-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                            $('#modal-account-guestaccounts-remove-error-container').css('display', '');
                        }
                    });
                }
                else {
                    DeleteAccountLinksOnComplete();

                    $('#modal-account-guestaccounts-remove-text').css('display', 'none');

                    $('#modal-account-guestaccounts-remove-error-container').css('display', '');
                    $('#modal-account-guestaccounts-remove-error-container').find('.alert-danger').html(result.message + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                }
            },
            error: function (result) {
                DeleteAccountLinksOnComplete();

                $('#modal-account-guestaccounts-remove-text').css('display', 'none');

                $('#modal-account-guestaccounts-remove-error-container').find('.alert-danger').html($('#modal-account-guestaccounts-remove-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#modal-account-guestaccounts-remove-error-container').css('display', '');
            }
        });
    });

    $('#modal-account-pending-guestaccounts-remove-form').formValidation({
    }).on('success.form.fv', function (e) {
        //Prevent form submission
        e.preventDefault();

        var guestEmail = $(this).data('name');

        $.ajax({
            url: '/Account/DeletePendingAccountLink/',
            data: AddAntiForgeryToken({ GuestEmail: guestEmail }),
            type: 'POST',
            dataType: "json",
            beforeSend: DeletePendingAccountLinksOnBegin,
            success: function (result) {
                if (result.success) {
                    $.ajax({
                        url: '/Account/UpdateGuestAccountsContent/',
                        type: 'GET',
                        success: function (data) {
                            $('#account-guestaccounts-content').html(data);

                            mySorted = new SortedTable();

                            //Reload scripts
                            $.getScript('/Scripts/fildela-account-all.js');
                            $("body").trigger("AccountGuestAccountsReloadScript");

                            //Load search table script
                            LoadSearchTable();

                            DeletePendingAccountLinksOnComplete();

                            $('#modal-account-pending-guestaccounts-remove-error-container').css('display', 'none');

                            $('#modal-account-pending-guestaccounts-remove-success-container').css('display', '');
                            $('#modal-account-pending-guestaccounts-remove-success-container').find('.alert-success').html(result.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');

                            $('#modal-account-pending-guestaccounts-remove-container-before').css('display', 'none');
                            $('#modal-account-pending-guestaccounts-remove-container-after').css('display', '');

                            var counter = 5;
                            var message = $('#modal-account-pending-guestaccounts-remove-closing-btn').data('message');
                            interval = setInterval(function () {
                                counter--;

                                $('#modal-account-pending-guestaccounts-remove-closing-btn').text(message + ' ' + counter);

                                if (counter == 0) {
                                    $('#modal-account-pending-guestaccounts-remove').modal('hide');

                                    clearInterval(interval);

                                    $('#modal-account-pending-guestaccounts-remove-closing-btn').text(message + ' ' + 5);

                                    $('#modal-account-pending-guestaccounts-remove-success-container').css('display', 'none');
                                    $('#modal-account-pending-guestaccounts-remove-error-container').css('display', 'none');

                                    $('#modal-account-pending-guestaccounts-remove-container-before').css('display', '');
                                    $('#modal-account-pending-guestaccounts-remove-container-after').css('display', 'none');
                                }
                            }, 1000);
                        },
                        error: function () {
                            DeletePendingAccountLinksOnComplete();

                            $('#modal-account-pending-guestaccounts-remove-text').css('display', 'none');

                            $('#modal-account-pending-guestaccounts-remove-error-container').find('.alert-danger').html($('#modal-account-pending-guestaccounts-remove-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                            $('#modal-account-pending-guestaccounts-remove-error-container').css('display', '');
                        }
                    });
                }
                else {
                    DeletePendingAccountLinksOnComplete();

                    $('#modal-account-pending-guestaccounts-remove-text').css('display', 'none');

                    $('#modal-account-pending-guestaccounts-remove-error-container').css('display', '');
                    $('#modal-account-pending-guestaccounts-remove-error-container').find('.alert-danger').html(result.message + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                }
            },
            error: function (result) {
                DeletePendingAccountLinksOnComplete();

                $('#modal-account-pending-guestaccounts-remove-text').css('display', 'none');

                $('#modal-account-pending-guestaccounts-remove-error-container').find('.alert-danger').html($('#modal-account-guestaccounts-remove-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#modal-account-pending-guestaccounts-remove-error-container').css('display', '');
            }
        });
    });

    //Delete pending accountlinks OnBegin
    function DeletePendingAccountLinksOnBegin() {
        isajaxrunning = true;

        //Show spinner and add margin to submit button text
        $('#modal-account-pending-guestaccounts-remove-spin').css('display', '');
        $('#modal-account-pending-guestaccounts-remove-spin').prev().css('margin-left', '17px');

        //Disable submit button
        $('#modal-account-pending-guestaccounts-remove-btn').prop('disabled', true);
    }

    //Delete pending accountlinks OnComplete
    function DeletePendingAccountLinksOnComplete() {
        isajaxrunning = false;

        //Hide spinner and reset margin
        $('#modal-account-pending-guestaccounts-remove-spin').css('display', 'none');
        $('#modal-account-pending-guestaccounts-remove-spin').prev().css('margin-left', '');

        //Enable submit button
        $('#modal-account-pending-guestaccounts-remove-btn').prop('disabled', false);

        //Reset formValidation
        $('#modal-account-pending-guestaccounts-remove-form').data('formValidation').resetForm();
        //Reset form
        $('#modal-account-pending-guestaccounts-remove-form').trigger("reset");
    }

    //Delete accountlinks OnBegin
    function DeleteAccountLinksOnBegin() {
        isajaxrunning = true;

        //Show spinner and add margin to submit button text
        $('#modal-account-guestaccounts-remove-spin').css('display', '');
        $('#modal-account-guestaccounts-remove-spin').prev().css('margin-left', '17px');

        //Disable submit button
        $('#modal-account-guestaccounts-remove-btn').prop('disabled', true);
    }

    //Delete accountlinks OnComplete
    function DeleteAccountLinksOnComplete() {
        isajaxrunning = false;

        //Hide spinner and reset margin
        $('#modal-account-guestaccounts-remove-spin').css('display', 'none');
        $('#modal-account-guestaccounts-remove-spin').prev().css('margin-left', '');

        //Enable submit button
        $('#modal-account-guestaccounts-remove-btn').prop('disabled', false);

        //Reset formValidation
        $('#modal-account-guestaccounts-remove-form').data('formValidation').resetForm();
        //Reset form
        $('#modal-account-guestaccounts-remove-form').trigger("reset");
    }

    $('#modal-account-guestaccounts-empty-form').formValidation({
    }).on('success.form.fv', function (e) {
        //Prevent form submission
        e.preventDefault();

        var rows = $('.account-table-tr');

        var guestIDs = new Array();
        $(rows).each(function (n) {
            guestIDs[n] = $(this).data('guestid');
        });

        $.ajax({
            url: '/Account/DeleteAccountLinks/',
            data: AddAntiForgeryToken({ GuestIDs: guestIDs }),
            type: 'POST',
            traditional: true,
            dataType: "json",
            beforeSend: EmptyAccountLinksOnBegin,
            success: function (result) {
                if (result.success) {
                    $.ajax({
                        url: '/Account/UpdateGuestAccountsContent/',
                        type: 'GET',
                        success: function (data) {
                            $('#account-guestaccounts-content').html(data);

                            mySorted = new SortedTable();

                            //Reload scripts
                            $.getScript('/Scripts/fildela-account-all.js');
                            $("body").trigger("AccountGuestAccountsReloadScript");

                            //Load search table script
                            LoadSearchTable();

                            EmptyAccountLinksOnComplete();

                            $('#modal-account-guestaccounts-empty-error-container').css('display', 'none');

                            $('#modal-account-guestaccounts-empty-success-container').css('display', '');
                            $('#modal-account-guestaccounts-empty-success-container').find('.alert-success').html(result.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');

                            $('#modal-account-guestaccounts-empty-container-before').css('display', 'none');
                            $('#modal-account-guestaccounts-empty-container-after').css('display', '');

                            var counter = 5;
                            var message = $('#modal-account-guestaccounts-empty-closing-btn').data('message');
                            interval = setInterval(function () {
                                counter--;

                                $('#modal-account-guestaccounts-empty-closing-btn').text(message + ' ' + counter);

                                if (counter == 0) {
                                    $('#modal-account-guestaccounts-empty').modal('hide');

                                    clearInterval(interval);
                                }
                            }, 1000);
                        },
                        error: function () {
                            EmptyAccountLinksOnComplete();

                            $('#modal-account-guestaccounts-empty-text').css('display', 'none');

                            $('#modal-account-guestaccounts-empty-error-container').find('.alert-danger').html($('#modal-account-guestaccounts-empty-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                            $('#modal-account-guestaccounts-empty-error-container').css('display', '');
                        }
                    });
                }
                else {
                    EmptyAccountLinksOnComplete();

                    $('#modal-account-guestaccounts-empty-text').css('display', 'none');

                    $('#modal-account-guestaccounts-empty-error-container').css('display', '');
                    $('#modal-account-guestaccounts-empty-error-container').find('.alert-danger').html(result.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');
                }
            },
            error: function (result) {
                EmptyAccountLinksOnComplete();

                $('#modal-account-guestaccounts-empty-text').css('display', 'none');

                $('#modal-account-guestaccounts-empty-error-container').find('.alert-danger').html($('#modal-account-guestaccounts-empty-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#modal-account-guestaccounts-empty-error-container').css('display', '');
            }
        });
    });

    //Empty accountlinks OnBegin
    function EmptyAccountLinksOnBegin() {
        isajaxrunning = true;

        //Show spinner and add margin to submit button text
        $('#modal-account-guestaccounts-empty-spin').css('display', '');
        $('#modal-account-guestaccounts-empty-spin').prev().css('margin-left', '17px');

        //Disable submit button
        $('#modal-account-guestaccounts-empty-btn').prop('disabled', true);
    }

    //Empty accountlinks OnComplete
    function EmptyAccountLinksOnComplete() {
        isajaxrunning = false;

        //Hide spinner and reset margin
        $('#modal-account-guestaccounts-empty-spin').css('display', 'none');
        $('#modal-account-guestaccounts-empty-spin').prev().css('margin-left', '');

        //Enable submit button
        $('#modal-account-guestaccounts-empty-btn').prop('disabled', false);

        //Reset formValidation
        $('#modal-account-guestaccounts-empty-form').data('formValidation').resetForm();
        //Reset form
        $('#modal-account-guestaccounts-empty-form').trigger("reset");
    }

    //Insert accountlink form OnBegin
    function InsertAccountLinkOnBegin() {
        isajaxrunning = true;

        //Show spinner and add margin to submit button text
        $('#modal-account-guestaccounts-create-submit-spin').css('display', '');
        $('#modal-account-guestaccounts-create-submit-spin').prev().css('margin-left', '17px');

        //Disable select2 input
        $("#account-guestaccounts-create-permissions").select2('disable');

        //Disable datepicker
        $('#account-guestaccount-create-startdate').find(':input').prop('disabled', true);
        $('#account-guestaccount-create-enddate').find(':input').prop('disabled', true);

        //Set read only on inputs
        $('.modal-account-guestaccounts-create-input').prop('readonly', true);
        //Disable submit button
        $('#modal-account-guestaccounts-create-submitbtn').prop('disabled', true);

        //Set cursor for datepicker button
        $('#account-guestaccount-create-startdate').find('.input-group-addon').css('cursor', 'default');
        //Set cursor for datepicker button
        $('#account-guestaccount-create-enddate').find('.input-group-addon').css('cursor', 'default');
    }

    //Insert accountlink form OnSuccess
    function InsertAccountLinkOnSuccess(json) {
        //Blur inputs
        $('.modal-account-guestaccounts-create-input').blur();

        //Error
        if (json.message === undefined) {
            $('#modal-account-guestaccounts-create-error-container').find('.alert-danger').html($('#modal-account-guestaccounts-create-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
            $('#modal-account-guestaccounts-create-error-container').css('display', '');

            //Enable inputs
            $('.modal-account-guestaccounts-create-input').prop('readonly', false);
            //Enable submit button
            $('#modal-account-guestaccounts-create-submitbtn').prop('disabled', false);

            //Set cursor for datepicker button
            $('#account-guestaccount-create-startdate').find('.input-group-addon').css('cursor', 'pointer');
            //Set cursor for datepicker button
            $('#account-guestaccount-create-enddate').find('.input-group-addon').css('cursor', 'pointer');

            //Enable datepicker
            $('#account-guestaccount-create-startdate').find(':input').prop('disabled', false);
            $('#account-guestaccount-create-enddate').find(':input').prop('disabled', false);
            $('#account-guestaccount-create-startdate').find(':input').prop('readonly', true);
            $('#account-guestaccount-create-enddate').find(':input').prop('readonly', true);
            $('#account-guestaccount-create-enddate').data().DateTimePicker.date(null);
            $('#account-guestaccount-create-startdate').data().DateTimePicker.date(null);

            //Reset form
            $('#modal-account-guestaccounts-create-form').trigger("reset");
            //Reset form validation
            $('#modal-account-guestaccounts-create-form').data('formValidation').resetForm();

            //Hide spinner and reset margin
            $('#modal-account-guestaccounts-create-submit-spin').css('display', 'none');
            $('#modal-account-guestaccounts-create-submit-spin').prev().css('margin-left', '');

            //Enable select2 input
            $("#account-guestaccounts-create-permissions").select2('enable');
            //Reset select2 value
            $("#account-guestaccounts-create-permissions").select2("val", "");
        }
        else {
            if (json.success) {
                $.ajax({
                    url: '/Account/UpdateGuestAccountsContent/',
                    type: 'GET',
                    success: function (data) {
                        $('#account-guestaccounts-content').html(data);

                        mySorted = new SortedTable();

                        //Reload scripts
                        $.getScript('/Scripts/fildela-account-all.js');
                        $("body").trigger("AccountGuestAccountsReloadScript");

                        //Load search table script
                        LoadSearchTable();

                        $('#modal-account-guestaccounts-create-error-container').css('display', 'none');

                        $('#modal-account-guestaccounts-create-success-container').find('.alert-success').html(json.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');
                        $('#modal-account-guestaccounts-create-success-container').css('display', '');

                        //Enable inputs
                        $('.modal-account-guestaccounts-create-input').prop('readonly', false);
                        //Enable submit button
                        $('#modal-account-guestaccounts-create-submitbtn').prop('disabled', false);

                        //Enable datepicker
                        $('#account-guestaccount-create-startdate').find(':input').prop('disabled', false);
                        $('#account-guestaccount-create-enddate').find(':input').prop('disabled', false);
                        $('#account-guestaccount-create-startdate').find(':input').prop('readonly', true);
                        $('#account-guestaccount-create-enddate').find(':input').prop('readonly', true);
                        $('#account-guestaccount-create-enddate').data().DateTimePicker.date(null);
                        $('#account-guestaccount-create-startdate').data().DateTimePicker.date(null);

                        //Set cursor for datepicker button
                        $('#account-guestaccount-create-startdate').find('.input-group-addon').css('cursor', 'pointer');
                        //Set cursor for datepicker button
                        $('#account-guestaccount-create-enddate').find('.input-group-addon').css('cursor', 'pointer');

                        //Reset form
                        $('#modal-account-guestaccounts-create-form').trigger("reset");
                        //Reset form validation
                        $('#modal-account-guestaccounts-create-form').data('formValidation').resetForm();

                        //Hide spinner and reset margin
                        $('#modal-account-guestaccounts-create-submit-spin').css('display', 'none');
                        $('#modal-account-guestaccounts-create-submit-spin').prev().css('margin-left', '');

                        //Enable select2 input
                        $("#account-guestaccounts-create-permissions").select2('enable');
                        //Reset select2 value
                        $("#account-guestaccounts-create-permissions").select2("val", "");
                    },
                    error: function () {
                        InsertAccountLinkOnError();

                        $('#modal-account-guestaccounts-create-error-container').find('.alert-danger').html($('#modal-account-guestaccounts-remove-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                        $('#modal-account-guestaccounts-create-error-container').css('display', '');

                        //Enable inputs
                        $('.modal-account-guestaccounts-create-input').prop('readonly', false);
                        //Enable submit button
                        $('#modal-account-guestaccounts-create-submitbtn').prop('disabled', false);

                        //Set cursor for datepicker button
                        $('#account-guestaccount-create-startdate').find('.input-group-addon').css('cursor', 'pointer');
                        //Set cursor for datepicker button
                        $('#account-guestaccount-create-enddate').find('.input-group-addon').css('cursor', 'pointer');

                        //Enable datepicker
                        $('#account-guestaccount-create-startdate').find(':input').prop('disabled', false);
                        $('#account-guestaccount-create-enddate').find(':input').prop('disabled', false);
                        $('#account-guestaccount-create-startdate').find(':input').prop('readonly', true);
                        $('#account-guestaccount-create-enddate').find(':input').prop('readonly', true);
                        $('#account-guestaccount-create-enddate').data().DateTimePicker.date(null);
                        $('#account-guestaccount-create-startdate').data().DateTimePicker.date(null);

                        //Reset form
                        $('#modal-account-guestaccounts-create-form').trigger("reset");
                        //Reset form validation
                        $('#modal-account-guestaccounts-create-form').data('formValidation').resetForm();

                        //Hide spinner and reset margin
                        $('#modal-account-guestaccounts-create-submit-spin').css('display', 'none');
                        $('#modal-account-guestaccounts-create-submit-spin').prev().css('margin-left', '');

                        //Enable select2 input
                        $("#account-guestaccounts-create-permissions").select2('enable');
                        //Reset select2 value
                        $("#account-guestaccounts-create-permissions").select2("val", "");
                    }
                });
            }
            else {
                $('#modal-account-guestaccounts-create-error-container').find('.alert-danger').html(json.message + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#modal-account-guestaccounts-create-error-container').css('display', '');

                //Enable inputs
                $('.modal-account-guestaccounts-create-input').prop('readonly', false);
                //Enable submit button
                $('#modal-account-guestaccounts-create-submitbtn').prop('disabled', false);

                //Set cursor for datepicker button
                $('#account-guestaccount-create-startdate').find('.input-group-addon').css('cursor', 'pointer');
                //Set cursor for datepicker button
                $('#account-guestaccount-create-enddate').find('.input-group-addon').css('cursor', 'pointer');

                //Enable datepicker
                $('#account-guestaccount-create-startdate').find(':input').prop('disabled', false);
                $('#account-guestaccount-create-enddate').find(':input').prop('disabled', false);
                $('#account-guestaccount-create-startdate').find(':input').prop('readonly', true);
                $('#account-guestaccount-create-enddate').find(':input').prop('readonly', true);
                $('#account-guestaccount-create-enddate').data().DateTimePicker.date(null);
                $('#account-guestaccount-create-startdate').data().DateTimePicker.date(null);

                //Reset form
                $('#modal-account-guestaccounts-create-form').trigger("reset");
                //Reset form validation
                $('#modal-account-guestaccounts-create-form').data('formValidation').resetForm();

                //Hide spinner and reset margin
                $('#modal-account-guestaccounts-create-submit-spin').css('display', 'none');
                $('#modal-account-guestaccounts-create-submit-spin').prev().css('margin-left', '');

                //Enable select2 input
                $("#account-guestaccounts-create-permissions").select2('enable');
                //Reset select2 value
                $("#account-guestaccounts-create-permissions").select2("val", "");
            }
        }
    }

    //Insert accountlink form OnError
    function InsertAccountLinkOnError() {
        //Blur inputs
        $('.modal-account-guestaccounts-create-input').blur();

        //Set cursor for datepicker button
        $('#account-guestaccount-create-startdate').find('.input-group-addon').css('cursor', 'pointer');
        //Set cursor for datepicker button
        $('#account-guestaccount-create-enddate').find('.input-group-addon').css('cursor', 'pointer');

        //Enable datepicker
        $('#account-guestaccount-create-startdate').find(':input').prop('disabled', false);
        $('#account-guestaccount-create-enddate').find(':input').prop('disabled', false);
        $('#account-guestaccount-create-startdate').find(':input').prop('readonly', true);
        $('#account-guestaccount-create-enddate').find(':input').prop('readonly', true);
        $('#account-guestaccount-create-enddate').data().DateTimePicker.date(null);
        $('#account-guestaccount-create-startdate').data().DateTimePicker.date(null);

        //Enable inputs
        $('.modal-account-guestaccounts-create-input').prop('readonly', false);
        //Enable submit button
        $('#modal-account-guestaccounts-create-submitbtn').prop('disabled', false);

        //Reset formValidation
        $('#modal-account-guestaccounts-create-form').data('formValidation').resetForm();
        //Reset form
        $('#modal-account-guestaccounts-create-form').trigger("reset");

        //Hide spinner and reset margin
        $('#modal-account-guestaccounts-create-submit-spin').css('display', 'none');
        $('#modal-account-guestaccounts-create-submit-spin').prev().css('margin-left', '');

        //Enable select2 input
        $("#account-guestaccounts-create-permissions").select2('enable');
        //Reset select2 value
        $("#account-guestaccounts-create-permissions").select2("val", "");
    }

    //Hide message from server when focus
    $(".modal-account-guestaccounts-create-input").focus(function () {
        $('#modal-account-guestaccounts-create-success-container').css('display', 'none');
        $('#modal-account-guestaccounts-create-error-container').css('display', 'none');
    });

    //Reset form when modal is closed
    $('#modal-account-guestaccounts-create').bind('hide.bs.modal', function (event) {
        $('#modal-account-guestaccounts-create-success-container').css('display', 'none');
        $('#modal-account-guestaccounts-create-error-container').css('display', 'none');

        //Reset datepicker values
        $('#account-guestaccount-create-enddate').data().DateTimePicker.date(null);
        $('#account-guestaccount-create-startdate').data().DateTimePicker.date(null);

        //Only reset form if ajax is not running
        if (!isajaxrunning) {
            var form = $(this).find('form')[0];

            if (form != undefined) {
                $(form).data('formValidation').resetForm();
                //Reset form
                form.reset();

                //Reset select2 value
                $("#account-guestaccounts-create-permissions").select2("val", "");
            }
        }
    });

    //Reset form after modal is opened
    $('#modal-account-guestaccounts-create').bind('shown.bs.modal', function (event) {
        $('#modal-account-guestaccounts-create-success-container').css('display', 'none');
        $('#modal-account-guestaccounts-create-error-container').css('display', 'none');

        //Only reset form if ajax is not running
        if (!isajaxrunning) {
            var form = $(this).find('form')[0];

            if (form != undefined) {
                $(form).data('formValidation').resetForm();
                //Reset form
                form.reset();

                //Reset select2 value
                $("#account-guestaccounts-create-permissions").select2("val", "");
            }
        }
    });



    $(document).ready(function () {
        $("body").on("AccountGuestAccountsReloadScript", function () {
            $('.account-guestaccounts-pending-remove').on('click', function () {
                var email = $(this).data('name');
                $('#modal-account-pending-guestaccounts-remove-form').data('name', email);

                $('#modal-account-pending-guestaccounts-remove').modal('show');
            });
        });

        $("body").trigger("AccountGuestAccountsReloadScript");
    });

    $('#modal-account-pending-guestaccounts-remove').bind('show.bs.modal', function () {
        var textEle = $('#modal-account-pending-guestaccounts-remove-text');

        var text1 = $(textEle).data('text1');
        var textEmail = $('#modal-account-pending-guestaccounts-remove-form').data('name');

        $(textEle).html(text1 + ' [<span style="color:#e74c3c;">' + textEmail + '</span>]?');
    });

    $('#modal-account-pending-guestaccounts-remove').bind('hidden.bs.modal', function () {
        $('#modal-account-pending-guestaccounts-remove-error-container').css('display', 'none');
        $('#modal-account-pending-guestaccounts-remove-text').css('display', '');

        $('#modal-account-pending-guestaccounts-remove-success-container').css('display', 'none');
        $('#modal-account-pending-guestaccounts-remove-error-container').css('display', 'none');

        $('#modal-account-pending-guestaccounts-remove-container-before').css('display', '');
        $('#modal-account-pending-guestaccounts-remove-container-after').css('display', 'none');

        var message = $('#modal-account-pending-guestaccounts-remove-closing-btn').data('message');
        $('#modal-account-pending-guestaccounts-remove-closing-btn').text(message + ' 5');

        clearInterval(interval);
    });

    $('#modal-account-guestaccounts-remove').bind('hidden.bs.modal', function () {
        $('#modal-account-guestaccounts-remove-error-container').css('display', 'none');
        $('#modal-account-guestaccounts-remove-text').css('display', '');

        $('#modal-account-guestaccounts-remove-success-container').css('display', 'none');
        $('#modal-account-guestaccounts-remove-error-container').css('display', 'none');

        $('#modal-account-guestaccounts-remove-container-before').css('display', '');
        $('#modal-account-guestaccounts-remove-container-after').css('display', 'none');

        var message = $('#modal-account-guestaccounts-remove-closing-btn').data('message');
        $('#modal-account-guestaccounts-remove-closing-btn').text(message + ' 5');

        clearInterval(interval);
    });

    $('#modal-account-guestaccounts-remove').on('show.bs.modal', function () {
        var titleEle = $('#modal-account-guestaccounts-remove-title');

        var selectedRows = $('.selrow');
        var textEle = $('#modal-account-guestaccounts-remove-text');
        var text1 = $(textEle).data('text1');
        var text2 = $(textEle).data('text2');
        var text3 = $(textEle).data('text3');

        if (selectedRows.length === 1) {
            $(textEle).html(text1 + ' [<span style="color:#e74c3c;">' + $('.selrow:first').data('name') + '</span>]?');
            $(titleEle).text($(titleEle).data('text1'));
        }
        else if (selectedRows.length > 1) {
            $(textEle).html(text1 + ' [<span style="color:#e74c3c;">' + selectedRows.length + '</span>] ' + text2 + '?');
            $(titleEle).text($(titleEle).data('text2'));
        }
        else {
            $(textEle).text(text3);
        }

        $('#modal-account-links-remove-success-container').css('display', 'none');
        $('#modal-account-links-remove-error-container').css('display', 'none');

        $('#modal-account-links-remove-container-before').css('display', '');
        $('#modal-account-links-remove-container-after').css('display', 'none');

        $('#modal-account-guestaccounts-remove-success-container').css('display', 'none');
        $('#modal-account-guestaccounts-remove-error-container').css('display', 'none');

        $('#modal-account-guestaccounts-remove-container-before').css('display', '');
        $('#modal-account-guestaccounts-remove-container-after').css('display', 'none');
    });

    $('#modal-account-guestaccounts-empty').bind('hidden.bs.modal', function () {
        $('#modal-account-guestaccounts-empty-error-container').css('display', 'none');
        $('#modal-account-guestaccounts-empty-text').css('display', '');

        $('#modal-account-guestaccounts-empty-success-container').css('display', 'none');
        $('#modal-account-guestaccounts-empty-error-container').css('display', 'none');

        $('#modal-account-guestaccounts-empty-container-before').css('display', '');
        $('#modal-account-guestaccounts-empty-container-after').css('display', 'none');

        var message = $('#modal-account-guestaccounts-empty-closing-btn').data('message');
        $('#modal-account-guestaccounts-empty-closing-btn').text(message + ' 5');

        clearInterval(interval);
    });

    $('#modal-account-guestaccounts-empty').on('show.bs.modal', function () {
        $('#modal-account-guestaccounts-empty-success-container').css('display', 'none');
        $('#modal-account-guestaccounts-empty-error-container').css('display', 'none');

        $('#modal-account-guestaccounts-empty-container-before').css('display', '');
        $('#modal-account-guestaccounts-empty-container-after').css('display', 'none');
    });
});