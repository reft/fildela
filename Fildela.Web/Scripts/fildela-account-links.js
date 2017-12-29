$(function () {
    var isajaxrunning = false;
    var interval;

    //Validate account links create
    $('#modal-account-links-file-create-form').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        locale: 'sv_SE',
        excluded: ':disabled',
        fields: {
            File: {
                validators: {
                    notEmpty: {
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
            url: '/Account/InsertLink/',
            data: $(e.target).serialize(),
            type: 'POST',
            beforeSend: InsertLinkOnBegin,
            success: InsertLinkOnSuccess,
            error: function () {
                $('#modal-account-links-file-create-error-container').find('.alert-danger').html($('#modal-account-links-file-create-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#modal-account-links-file-create-error-container').css('display', '');

                InsertLinkOnError();
            },
            complete: function () {
                var selectValidation = $('#account-links-create-form-group-container-file').find('.form-control-feedback');
                var selectBorder = $('.select2-drop-active');
                $(selectBorder).css('border-left', '1px solid #5897fb');
                $(selectBorder).css('border-right', '1px solid #5897fb');
                $(selectBorder).css('border-bottom', '1px solid #5897fb');
            }
        });
    });

    $('#modal-account-links-remove-form').formValidation({
    }).on('success.form.fv', function (e) {
        //Prevent form submission
        e.preventDefault();

        var rows = $('.selrow');

        var rowKeys = new Array();
        $(rows).each(function (n) {
            rowKeys[n] = $(this).data('rowkey');
        });

        $.ajax({
            url: '/Account/DeleteLinks/',
            data: AddAntiForgeryToken({ RowKeys: rowKeys }),
            type: 'POST',
            traditional: true,
            dataType: "json",
            beforeSend: DeleteLinksOnBegin,
            success: function (result) {
                if (result.success) {
                    $.ajax({
                        url: '/Account/UpdateLinksContent/',
                        type: 'GET',
                        success: function (data) {
                            $('#account-links-content').html(data);

                            mySorted = new SortedTable();

                            //Reload scripts
                            $.getScript('/Scripts/fildela-account-all.js');

                            //Load search table script
                            LoadSearchTable();

                            DeleteLinksOnComplete();

                            $('#modal-account-links-remove-error-container').css('display', 'none');

                            $('#modal-account-links-remove-success-container').css('display', '');
                            $('#modal-account-links-remove-success-container').find('.alert-success').html(result.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');

                            $('#modal-account-links-remove-container-before').css('display', 'none');
                            $('#modal-account-links-remove-container-after').css('display', '');

                            var counter = 5;
                            var message = $('#modal-account-links-remove-closing-btn').data('message');
                            interval = setInterval(function () {
                                counter--;

                                $('#modal-account-links-remove-closing-btn').text(message + ' ' + counter);

                                if (counter == 0) {
                                    $('#modal-account-links-remove').modal('hide');

                                    clearInterval(interval);
                                }
                            }, 1000);
                        },
                        error: function () {
                            $('#modal-account-links-remove-text').css('display', 'none');

                            DeleteLinksOnComplete();

                            $('#modal-account-links-remove-error-container').find('.alert-danger').html($('#modal-account-links-remove-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                            $('#modal-account-links-remove-error-container').css('display', '');
                        }
                    });
                }
                else {
                    $('#modal-account-links-remove-text').css('display', 'none');

                    DeleteLinksOnComplete();

                    $('#modal-account-links-remove-error-container').css('display', '');
                    $('#modal-account-links-remove-error-container').find('.alert-danger').html(result.message + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                }
            },
            error: function (result) {
                $('#modal-account-links-remove-text').css('display', 'none');

                DeleteLinksOnComplete();

                $('#modal-account-links-remove-error-container').find('.alert-danger').html($('#modal-account-links-remove-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#modal-account-links-remove-error-container').css('display', '');
            }
        });
    });

    //Delete links OnBegin
    function DeleteLinksOnBegin() {
        isajaxrunning = true;

        //Show spinner and add margin to submit button text
        $('#modal-account-links-remove-spin').css('display', '');
        $('#modal-account-links-remove-spin').prev().css('margin-left', '17px');

        //Disable submit button
        $('#modal-account-links-remove-btn').prop('disabled', true);
    }

    function DeleteLinksOnComplete() {
        isajaxrunning = false;

        //Hide spinner and reset margin
        $('#modal-account-links-remove-spin').css('display', 'none');
        $('#modal-account-links-remove-spin').prev().css('margin-left', '');

        //Enable submit button
        $('#modal-account-links-remove-btn').prop('disabled', false);

        //Reset formValidation
        $('#modal-account-links-remove-form').data('formValidation').resetForm();
        //Reset form
        $('#modal-account-links-remove-form').trigger("reset");
    }

    $('#modal-account-links-empty-form').formValidation({
    }).on('success.form.fv', function (e) {
        //Prevent form submission
        e.preventDefault();

        var rows = $('.account-table-tr');

        var rowKeys = new Array();
        $(rows).each(function (n) {
            rowKeys[n] = $(this).data('rowkey');
        });

        $.ajax({
            url: '/Account/DeleteLinks/',
            data: AddAntiForgeryToken({ RowKeys: rowKeys }),
            type: 'POST',
            traditional: true,
            dataType: "json",
            beforeSend: EmptyLinksOnBegin,
            success: function (result) {
                if (result.success) {
                    $.ajax({
                        url: '/Account/UpdateLinksContent/',
                        type: 'GET',
                        success: function (data) {
                            $('#account-links-content').html(data);

                            mySorted = new SortedTable();

                            //Reload scripts
                            $.getScript('/Scripts/fildela-account-all.js');

                            //Load search table script
                            LoadSearchTable();

                            EmptyLinksOnComplete();

                            $('#modal-account-links-empty-error-container').css('display', 'none');

                            $('#modal-account-links-empty-success-container').css('display', '');
                            $('#modal-account-links-empty-success-container').find('.alert-success').html(result.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');

                            $('#modal-account-links-empty-container-before').css('display', 'none');
                            $('#modal-account-links-empty-container-after').css('display', '');

                            var counter = 5;
                            var message = $('#modal-account-links-empty-closing-btn').data('message');
                            interval = setInterval(function () {
                                counter--;

                                $('#modal-account-links-empty-closing-btn').text(message + ' ' + counter);

                                if (counter == 0) {
                                    $('#modal-account-links-empty').modal('hide');

                                    clearInterval(interval);
                                }
                            }, 1000);
                        },
                        error: function () {
                            $('#modal-account-links-empty-text').css('display', 'none');

                            EmptyLinksOnComplete();

                            $('#modal-account-links-empty-error-container').find('.alert-danger').html($('#modal-account-links-empty-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                            $('#modal-account-links-empty-error-container').css('display', '');
                        }
                    });
                }
                else {
                    $('#modal-account-links-empty-text').css('display', 'none');

                    EmptyLinksOnComplete();

                    $('#modal-account-links-empty-error-container').css('display', '');
                    $('#modal-account-links-empty-error-container').find('.alert-danger').html(result.message + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                }
            },
            error: function (result) {
                $('#modal-account-links-empty-text').css('display', 'none');

                EmptyLinksOnComplete();

                $('#modal-account-links-empty-error-container').find('.alert-danger').html($('#modal-account-links-empty-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#modal-account-links-empty-error-container').css('display', '');
            }
        });
    });

    function EmptyLinksOnBegin() {
        isajaxrunning = true;

        //Show spinner and add margin to submit button text
        $('#modal-account-links-empty-spin').css('display', '');
        $('#modal-account-links-empty-spin').prev().css('margin-left', '17px');

        //Disable submit button
        $('#modal-account-links-empty-btn').prop('disabled', true);
    }

    function EmptyLinksOnComplete() {
        isajaxrunning = false;

        //Hide spinner and reset margin
        $('#modal-account-links-empty-spin').css('display', 'none');
        $('#modal-account-links-empty-spin').prev().css('margin-left', '');

        //Enable submit button
        $('#modal-account-links-empty-btn').prop('disabled', false);

        //Reset formValidation
        $('#modal-account-links-empty-form').data('formValidation').resetForm();
        //Reset form
        $('#modal-account-links-empty-form').trigger("reset");
    }

    //Insert link form OnBegin
    function InsertLinkOnBegin() {
        isajaxrunning = true;

        //Show spinner and add margin to submit button text
        $('#modal-account-links-file-create-submit-spin').css('display', '');
        $('#modal-account-links-file-create-submit-spin').prev().css('margin-left', '17px');

        //Disable select2 input
        $("#account-links-create-file").select2('disable');

        //Disable datepicker
        $('#account-links-create-startdate').find(':input').prop('disabled', true);
        $('#account-links-create-enddate').find(':input').prop('disabled', true);

        //Set read only on inputs
        $('.modal-account-links-file-create-input').prop('readonly', true);
        //Disable submit button
        $('#modal-account-links-file-create-submitbtn').prop('disabled', true);

        //Set cursor for datepicker button
        $('#account-links-create-startdate').find('.input-group-addon').css('cursor', 'default');
        //Set cursor for datepicker button
        $('#account-links-create-enddate').find('.input-group-addon').css('cursor', 'default');
    }

    //Insert accountlink form OnSuccess
    function InsertLinkOnSuccess(json) {
        if (json.success) {
            $.ajax({
                url: '/Account/UpdateLinksContent/',
                type: 'GET',
                dataType: 'html',
                cache: false,
                async: false,
                success: function (data) {
                    $('#account-links-content').html(data);
                    mySorted = new SortedTable();

                    $.getScript('/Scripts/fildela-account-all.js');

                    //Load search table script
                    LoadSearchTable();

                    //Blur inputs
                    $('.modal-account-links-file-create-input').blur();

                    //Error
                    if (json.message === undefined) {
                        $('#modal-account-links-file-create-error-container').find('.alert-danger').html($('#modal-account-links-file-create-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                        $('#modal-account-links-file-create-error-container').css('display', '');
                    }
                    else {
                        if (json.success) {
                            $('#modal-account-links-file-create-error-container').css('display', 'none');

                            $('#modal-account-links-file-create-success-container').find('.alert-success').html(json.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');
                            $('#modal-account-links-file-create-success-container').css('display', '');
                        }
                        else {
                            $('#modal-account-links-file-create-error-container').find('.alert-danger').html(json.message + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                            $('#modal-account-links-file-create-error-container').css('display', '');
                        }
                    }

                    //Enable inputs
                    $('.modal-account-links-file-create-input').prop('readonly', false);
                    //Enable submit button
                    $('#modal-account-links-file-create-submitbtn').prop('disabled', false);

                    //Enable datepicker
                    $('#account-links-create-startdate').find(':input').prop('disabled', false);
                    $('#account-links-create-enddate').find(':input').prop('disabled', false);
                    $('#account-links-create-startdate-text').prop('readonly', true);
                    $('#account-links-create-enddate-text').prop('readonly', true);
                    $('#account-links-create-startdate').data().DateTimePicker.date(null);
                    $('#account-links-create-enddate').data().DateTimePicker.date(null);

                    //Set cursor for datepicker button
                    $('#account-links-create-startdate').find('.input-group-addon').css('cursor', 'pointer');
                    //Set cursor for datepicker button
                    $('#account-links-create-enddate').find('.input-group-addon').css('cursor', 'pointer');

                    //Reset form
                    $('#modal-account-links-file-create-form').trigger("reset");
                    //Reset form validation
                    $('#modal-account-links-file-create-form').data('formValidation').resetForm();

                    //Hide spinner and reset margin
                    $('#modal-account-links-file-create-submit-spin').css('display', 'none');
                    $('#modal-account-links-file-create-submit-spin').prev().css('margin-left', '');

                    //Enable select2 input
                    $("#account-links-create-file").select2('enable');
                    //Reset select2 value
                    $("#account-links-create-file").select2("val", "");
                },
                error: function () {
                    $('#modal-account-links-file-create-error-container').find('.alert-danger').html($('#modal-account-links-file-create-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                    $('#modal-account-links-file-create-error-container').css('display', '');

                    InsertLinkOnError();
                },
                complete: function () {
                    isajaxrunning = false;
                }
            });
        }
        else {
            $('#modal-account-links-file-create-error-container').find('.alert-danger').html(json.message + '<i class="glyphicon glyphicon-remove pull-right"></i>');
            $('#modal-account-links-file-create-error-container').css('display', '');

            InsertLinkOnError();
        }
    }

    //Insert accountlink form OnError
    function InsertLinkOnError() {
        //Blur inputs
        $('.modal-account-links-file-create-input').blur();

        //Set cursor for datepicker button
        $('#account-links-create-startdate').find('.input-group-addon').css('cursor', 'pointer');
        //Set cursor for datepicker button
        $('#account-links-create-enddate').find('.input-group-addon').css('cursor', 'pointer');

        //Enable inputs
        $('.modal-account-links-file-create-input').prop('readonly', false);
        //Enable submit button
        $('#modal-account-links-file-create-submitbtn').prop('disabled', false);

        //Enable datepicker
        $('#account-links-create-startdate').find(':input').prop('disabled', false);
        $('#account-links-create-enddate').find(':input').prop('disabled', false);
        $('#account-links-create-startdate-text').prop('readonly', true);
        $('#account-links-create-enddate-text').prop('readonly', true);
        $('#account-links-create-startdate').data().DateTimePicker.date(null);
        $('#account-links-create-enddate').data().DateTimePicker.date(null);

        //Reset formValidation
        $('#modal-account-links-file-create-form').data('formValidation').resetForm();
        //Reset form
        $('#modal-account-links-file-create-form').trigger("reset");

        $('#account-links-create-file-json').val('');

        //Hide spinner and reset margin
        $('#modal-account-links-file-create-submit-spin').css('display', 'none');
        $('#modal-account-links-file-create-submit-spin').prev().css('margin-left', '');

        //Enable select2 input
        $("#account-links-create-file").select2('enable');
        //Reset select2 value
        $("#account-links-create-file").select2("val", "");
    }

    //Hide message from server when focus
    $(".modal-account-links-file-create-input").focus(function () {
        $('#modal-account-links-file-create-success-container').css('display', 'none');
        $('#modal-account-links-file-create-error-container').css('display', 'none');
    });

    //Reset form when modal is closed
    $('#modal-account-links-file-create').bind('hide.bs.modal', function (event) {
        $('#modal-account-links-file-create-success-container').css('display', 'none');
        $('#modal-account-links-file-create-error-container').css('display', 'none');

        //Reset datepicker values
        $('#account-links-create-startdate').data().DateTimePicker.date(null);
        $('#account-links-create-enddate').data().DateTimePicker.date(null);

        //Only reset form if ajax is not running
        if (!isajaxrunning) {
            var form = $(this).find('form')[0];

            if (form != undefined) {
                $(form).data('formValidation').resetForm();
                //Reset form
                form.reset();

                //Reset select2 value
                $("#account-links-create-file").select2("val", "");

                $('#account-links-create-file-json').val('');
            }
        }
    });

    //Reset form after modal is opened
    $('#modal-account-links-file-create').bind('shown.bs.modal', function (event) {
        $('#modal-account-links-file-create-success-container').css('display', 'none');
        $('#modal-account-links-file-create-error-container').css('display', 'none');

        //Only reset form if ajax is not running
        if (!isajaxrunning) {
            var form = $(this).find('form')[0];

            if (form != undefined) {
                $(form).data('formValidation').resetForm();
                //Reset form
                form.reset();

                //Reset select2 value
                $("#account-links-create-file").select2("val", "");

                $('#account-links-create-file-json').val('');
            }
        }
    });

    $('#modal-account-links-url').on('show.bs.modal', function () {
        if ($('body').hasClass('mobile')) {
            $('#modal-account-links-url-input').prop('readonly', false);
        }

        var downloadURL = $('.selrow:first').data('downloadurl');
        var input = $('#modal-account-links-url-input');

        $(input).val(downloadURL);
    });

    $('#modal-account-links-url-input').on('keyup', function (e) {
        if ($('body').hasClass('mobile')) {
            var downloadURL = $('.selrow:first').data('downloadurl');
            var input = $('#modal-account-links-url-input');

            $(input).val(downloadURL);
        }
    });

    $('#modal-account-links-url-input').on('touchend', function (e) {
        $('#modal-account-links-url-input').focus();
        $('#modal-account-links-url-input').setSelectionRange(0, 9999);
    });

    $('#modal-account-links-url-input').mousedown(function (event) {
        switch (event.which) {
            case 3:
                $('#modal-account-links-url-input').select();
                $('#modal-account-links-url-input').focus();
                break;
        }
    });

    $('#modal-account-links-url-input').on("click", function () {
        $('#modal-account-links-url-input').select();
    });

    $('#modal-account-links-remove').bind('hidden.bs.modal', function () {
        $('#modal-account-links-remove-text').css('display', '');

        $('#modal-account-links-remove-success-container').css('display', 'none');
        $('#modal-account-links-remove-error-container').css('display', 'none');

        var message = $('#modal-account-links-remove-closing-btn').data('message');
        $('#modal-account-links-remove-closing-btn').text(message + ' 5');

        $('#modal-account-links-remove-container-before').css('display', '');
        $('#modal-account-links-remove-container-after').css('display', 'none');

        var message = $('#modal-account-links-remove-closing-btn').data('message');
        $('#modal-account-links-remove-closing-btn').text(message + ' 5');

        clearInterval(interval);
    });

    $('#modal-account-links-remove').on('show.bs.modal', function () {
        var titleEle = $('#modal-account-links-remove-title');

        var selectedRows = $('.selrow');
        var textEle = $('#modal-account-links-remove-text');
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
    });

    $('#modal-account-links-empty').bind('hidden.bs.modal', function () {
        $('#modal-account-links-empty-text').css('display', '');

        $('#modal-account-links-empty-success-container').css('display', 'none');
        $('#modal-account-links-empty-error-container').css('display', 'none');

        var message = $('#modal-account-links-empty-closing-btn').data('message');
        $('#modal-account-links-empty-closing-btn').text(message + ' 5');

        $('#modal-account-links-empty-container-before').css('display', '');
        $('#modal-account-links-empty-container-after').css('display', 'none');

        var message = $('#modal-account-links-empty-closing-btn').data('message');
        $('#modal-account-links-empty-closing-btn').text(message + ' 5');

        clearInterval(interval);
    });

    $('#modal-account-links-empty').on('show.bs.modal', function () {
        $('#modal-account-links-empty-success-container').css('display', 'none');
        $('#modal-account-links-empty-error-container').css('display', 'none');

        $('#modal-account-links-empty-container-before').css('display', '');
        $('#modal-account-links-empty-container-after').css('display', 'none');
    });
});