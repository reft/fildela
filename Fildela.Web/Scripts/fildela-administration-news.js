$(function () {
    var isajaxrunning = false;

    //Validate administration news post news
    $('#modal-administration-news-create-form').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        locale: 'sv_SE',
        fields: {
            CategoryID: {
                validators: {
                    notEmpty: {
                    }
                }
            },
            Title: {
                validators: {
                    notEmpty: {
                    },
                    stringLength: {
                        min: 6,
                        max: 50
                    }
                }
            },
            Image: {
                validators: {
                    notEmpty: {
                    },
                    file: {
                        extension: 'jpeg,jpg',
                        type: 'image/jpeg'
                    }
                }
            },
            Text: {
                validators: {
                    notEmpty: {
                    },
                    stringLength: {
                        min: 50
                    }
                }
            }
        }
    }).on('success.form.fv', function (e) {
        //Prevent form submission
        e.preventDefault();

        var formData = new FormData($(e.target)[0]);

        $.ajax({
            url: '/Administration/InsertNews/',
            data: formData,
            type: 'POST',
            beforeSend: InsertNewsOnBegin,
            success: InsertNewsOnSuccess,
            contentType: false,
            processData: false,
            error: function () {
                InsertNewsOnError();
            },
            complete: function () {
                isajaxrunning = false;
            }
        });
    });

    //Insert news form OnBegin
    function InsertNewsOnBegin() {
        isajaxrunning = true;

        //Show spinner and add margin to submit button text
        $('#modal-administration-news-create-submit-spin').css('display', '');
        $('#modal-administration-news-create-submit-spin').prev().css('margin-left', '17px');

        //Set cursor class for textarea
        $('#modal-administration-news-create-textarea').addClass('textarea-readonly');

        //Set read only inputs
        $('.modal-administration-news-create-input').prop('readonly', true);
        //Disable file input
        $('#modal-news-file-input').prop('disabled', true);
        $('#modal-news-file-input-wrapper').addClass('disabled');
        $('#modal-news-file-input-wrapper').css('background-color', '#eee');

        //Disable submit button
        $('#modal-administration-news-create-submitbtn').prop('disabled', true);
        //Disable select button
        $("#modal-administration-news-create-select").prop("disabled", true);
    }

    //Insert news form OnSuccess
    function InsertNewsOnSuccess(json) {
        //Blur inputs
        $('.modal-administration-news-create-input').blur();

        //Error
        if (json.message === undefined) {
            $('#modal-administration-news-create-error-container').find('.alert-danger').html($('#account-settings-reset-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
            $('#modal-administration-news-create-error-container').css('display', '');
        }
        else {
            if (json.success) {
                $('#modal-administration-news-create-success-container').find('.alert-success').html(json.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');
                $('#modal-administration-news-create-success-container').css('display', '');

                $.ajax({
                    url: '/Administration/UpdateNewsContent/',
                    type: 'GET',
                    dataType: 'html',
                    cache: false,
                    async: false,
                    success: function (data) {
                        //Enable select button
                        $("#modal-administration-news-create-select").prop("disabled", false);

                        //Reset formValidation
                        $('#modal-administration-news-create-form').data('formValidation').resetForm();

                        $('#administration-news-content').html(data);

                        $('#modal-administration-news-create-success-container').find('.alert-success').html(json.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');
                        $('#modal-administration-news-create-success-container').css('display', '');

                        //Set default value for select input
                        $(".select-color").addClass("select-first");
                    },
                    error: function () {
                        InsertNewsOnError();
                    }
                });
            }
            else {
                $('#modal-administration-news-create-error-container').find('.alert-danger').html(json.message + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#modal-administration-news-create-error-container').css('display', '');
            }
        }

        //Enable inputs
        $('.modal-administration-news-create-input').prop('readonly', false);
        //Enable submit button
        $('#modal-administration-news-create-submitbtn').prop('disabled', false);
        //Enable select button
        $("#modal-administration-news-create-select").prop("disabled", false);
        $('#modal-administration-news-create-select').addClass('select-first');
        //Enable file input
        $('#modal-news-file-input').prop('disabled', false);
        $('#modal-news-file-input-wrapper').removeClass('disabled');
        $('#modal-news-file-input').val('');
        $('#modal-news-file-text').css('color', '#999999').text($('#modal-news-file-input').data('message'));

        //Remove cursor class for textarea
        $('#modal-administration-news-create-textarea').removeClass('textarea-readonly');

        //Reset form
        $('#modal-administration-news-create-form').trigger("reset");
        //Reset formValidation
        $('#modal-administration-news-create-form').data('formValidation').resetForm();

        $('#modal-news-file-input-wrapper').css('background-color', '#fff');

        //Hide spinner and reset margin
        $('#modal-administration-news-create-submit-spin').css('display', 'none');
        $('#modal-administration-news-create-submit-spin').prev().css('margin-left', '');
    }

    //Insert news form OnError
    function InsertNewsOnError() {
        //Blur inputs
        $('.modal-administration-news-create-input').blur();

        //Show default error message
        $('#modal-administration-news-create-error-container').find('.alert-danger').html($('#modal-administration-news-create-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
        $('#modal-administration-news-create-error-container').css('display', '');

        $('#modal-administration-news-create-success-container').css('display', 'none');

        //Enable inputs
        $('.modal-administration-news-create-input').prop('readonly', false);
        //Enable submit button
        $('#modal-administration-news-create-submitbtn').prop('disabled', true);
        //Enable select button
        $("#modal-administration-news-create-select").prop("disabled", false);
        $('#modal-administration-news-create-select').addClass('select-first');
        //Enable file input
        $('#modal-news-file-input').prop('disabled', false);
        $('#modal-news-file-input').val('');
        $('#modal-news-file-input-wrapper').removeClass('disabled');
        $('#modal-news-file-text').css('color', '#999999').text($('#modal-news-file-input').data('message'));

        //Remove cursor class for textarea
        $('#modal-administration-news-create-textarea').removeClass('textarea-readonly');

        //Reset formValidation
        $('#modal-administration-news-create-form').data('formValidation').resetForm();
        //Reset form
        $('#modal-administration-news-create-form').trigger("reset");

        //Set default value for select input
        $(".select-color").addClass("select-first");

        $('#modal-news-file-input-wrapper').css('background-color', '#fff');

        //Hide spinner and reset margin
        $('#modal-administration-news-create-submit-spin').css('display', 'none');
        $('#modal-administration-news-create-submit-spin').prev().css('margin-left', '');
    }

    //Hide message from server when focus
    $(".modal-administration-news-create-input").focus(function () {
        $('#modal-administration-news-create-error-container').css('display', 'none');
        $('#modal-administration-news-create-success-container').css('display', 'none');
    });

    //Reset form when modal is closed
    $('#modal-administration-news-create').bind('hide.bs.modal', function (event) {
        $('#modal-administration-news-create-error-container').css('display', 'none');
        $('#modal-administration-news-create-success-container').css('display', 'none');

        //Only reset form if ajax is not running
        if (!isajaxrunning) {
            var form = $(this).find('form')[0];

            if (form != undefined) {
                $(form).data('formValidation').resetForm();
                //Reset form
                form.reset();
            }

            $('#modal-administration-news-create-select').val('0');
            $(".select-color").addClass("select-first");
        }
    });

    //Reset form after modal is opened
    $('#modal-administration-news-create').bind('shown.bs.modal', function (event) {
        //Only reset form if ajax is not running
        if (!isajaxrunning) {
            var form = $(this).find('form')[0];

            if (form != undefined) {
                //Reset formValidation
                $(form).data('formValidation').resetForm();
                //Reset form
                form.reset();
            }

            $('#modal-administration-news-create-select').val('0');
            $('#modal-news-file-text').css('color', '#999999').text($('#modal-news-file-input').data('message'));
            $('#modal-news-file-input').val('');
            $(".select-color").addClass("select-first");
        }
    });

    $('#modal-news-file-input').change(function () {
        if ($(this).val() != '') {
            $('#modal-news-file-text').css('color', '#000').text($(this).val().split('\\').pop());
        }
        else {
            $('#modal-news-file-text').css('color', '#999999').text($(this).data('message'));
        }
    });
});