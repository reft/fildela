$(function () {
    isajaxrunning = false;
    var interval;

    $('#modal-account-logs-remove-form').formValidation({
    }).on('success.form.fv', function (e) {
        //Prevent form submission
        e.preventDefault();

        var rows = $('.selrow');

        var rowKeys = new Array();
        $(rows).each(function (n) {
            rowKeys[n] = $(this).data('rowkey');
        });

        $.ajax({
            url: '/Account/DeleteLogs/',
            data: AddAntiForgeryToken({ RowKeys: rowKeys }),
            type: 'POST',
            traditional: true,
            dataType: "json",
            beforeSend: DeleteLogsOnBegin,
            success: function (result) {
                if (result.success) {
                    $.ajax({
                        url: '/Account/UpdateLogsContent/',
                        type: 'GET',
                        success: function (data) {
                            $('#account-logs-content').html(data);

                            mySorted = new SortedTable();

                            //Reload scripts
                            $.getScript('/Scripts/fildela-account-all.js');

                            //Load search table script
                            LoadSearchTable();

                            DeleteLogsOnComplete();

                            $('#modal-account-logs-remove-error-container').css('display', 'none');

                            $('#modal-account-logs-remove-success-container').css('display', '');
                            $('#modal-account-logs-remove-success-container').find('.alert-success').html(result.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');

                            $('#modal-account-logs-remove-container-before').css('display', 'none');
                            $('#modal-account-logs-remove-container-after').css('display', '');

                            var counter = 5;
                            var message = $('#modal-account-logs-remove-closing-btn').data('message');
                            interval = setInterval(function () {
                                counter--;

                                $('#modal-account-logs-remove-closing-btn').text(message + ' ' + counter);

                                if (counter == 0) {
                                    $('#modal-account-logs-remove').modal('hide');

                                    clearInterval(interval);
                                }
                            }, 1000);
                        },
                        error: function () {
                            DeleteLogsOnComplete();

                            $('#modal-account-logs-remove-text').css('display', 'none');

                            $('#modal-account-logs-remove-error-container').find('.alert-danger').html($('#modal-account-logs-remove-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                            $('#modal-account-logs-remove-error-container').css('display', '');
                        }
                    });
                }
                else {
                    DeleteLogsOnComplete();

                    $('#modal-account-logs-remove-text').css('display', 'none');

                    $('#modal-account-logs-remove-error-container').css('display', '');
                    $('#modal-account-logs-remove-error-container').find('.alert-danger').html(result.message + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                }
            },
            error: function (result) {
                DeleteLogsOnComplete();

                $('#modal-account-logs-remove-text').css('display', 'none');

                $('#modal-account-logs-remove-error-container').find('.alert-danger').html($('#modal-account-logs-remove-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#modal-account-logs-remove-error-container').css('display', '');
            }
        });
    });

    //Delete logs OnBegin
    function DeleteLogsOnBegin() {
        isajaxrunning = true;

        //Show spinner and add margin to submit button text
        $('#modal-account-logs-remove-spin').css('display', '');
        $('#modal-account-logs-remove-spin').prev().css('margin-left', '17px');

        //Disable submit button
        $('#modal-account-logs-remove-btn').prop('disabled', true);
    }

    function DeleteLogsOnComplete() {
        isajaxrunning = false;

        //Hide spinner and reset margin
        $('#modal-account-logs-remove-spin').css('display', 'none');
        $('#modal-account-logs-remove-spin').prev().css('margin-left', '');

        //Enable submit button
        $('#modal-account-logs-remove-btn').prop('disabled', false);

        //Reset formValidation
        $('#modal-account-logs-remove-form').data('formValidation').resetForm();
        //Reset form
        $('#modal-account-logs-remove-form').trigger("reset");
    }

    $('#modal-account-logs-empty-form').formValidation({
    }).on('success.form.fv', function (e) {
        //Prevent form submission
        e.preventDefault();

        var rows = $('.account-table-tr');

        var rowKeys = new Array();
        $(rows).each(function (n) {
            rowKeys[n] = $(this).data('rowkey');
        });

        $.ajax({
            url: '/Account/DeleteLogs/',
            data: AddAntiForgeryToken({ RowKeys: rowKeys }),
            type: 'POST',
            traditional: true,
            dataType: "json",
            beforeSend: EmptyLogsOnBegin,
            success: function (result) {
                if (result.success) {
                    $.ajax({
                        url: '/Account/UpdateLogsContent/',
                        type: 'GET',
                        success: function (data) {
                            $('#account-logs-content').html(data);

                            mySorted = new SortedTable();

                            //Reload scripts
                            $.getScript('/Scripts/fildela-account-all.js');

                            //Load search table script
                            LoadSearchTable();

                            EmptyLogsOnComplete();

                            $('#modal-account-logs-empty-success-container').css('display', '');
                            $('#modal-account-logs-empty-success-container').find('.alert-success').html(result.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');

                            $('#modal-account-logs-empty-container-before').css('display', 'none');
                            $('#modal-account-logs-empty-container-after').css('display', '');

                            var counter = 5;
                            var message = $('#modal-account-logs-empty-closing-btn').data('message');
                            interval = setInterval(function () {
                                counter--;

                                $('#modal-account-logs-empty-closing-btn').text(message + ' ' + counter);

                                if (counter == 0) {
                                    $('#modal-account-logs-empty').modal('hide');

                                    clearInterval(interval);
                                }
                            }, 1000);
                        },
                        error: function () {
                            EmptyLogsOnComplete();

                            $('#modal-account-logs-empty-text').css('display', 'none');

                            $('#modal-account-logs-empty-error-container').find('.alert-danger').html($('#modal-account-logs-empty-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                            $('#modal-account-logs-empty-error-container').css('display', '');
                        }
                    });
                }
                else {
                    EmptyLogsOnComplete();

                    $('#modal-account-logs-empty-text').css('display', 'none');

                    $('#modal-account-logs-empty-error-container').css('display', '');
                    $('#modal-account-logs-empty-error-container').find('.alert-danger').html(result.message + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                }
            },
            error: function (result) {
                EmptyLogsOnComplete();

                $('#modal-account-logs-empty-text').css('display', 'none');

                $('#modal-account-logs-empty-error-container').find('.alert-danger').html($('#modal-account-logs-empty-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#modal-account-logs-empty-error-container').css('display', '');
            }
        });
    });

    function EmptyLogsOnBegin() {
        isajaxrunning = true;

        //Show spinner and add margin to submit button text
        $('#modal-account-logs-empty-spin').css('display', '');
        $('#modal-account-logs-empty-spin').prev().css('margin-left', '17px');

        //Disable submit button
        $('#modal-account-logs-empty-btn').prop('disabled', true);
    }

    function EmptyLogsOnComplete() {
        isajaxrunning = false;

        //Hide spinner and reset margin
        $('#modal-account-logs-empty-spin').css('display', 'none');
        $('#modal-account-logs-empty-spin').prev().css('margin-left', '');

        //Enable submit button
        $('#modal-account-logs-empty-btn').prop('disabled', false);

        //Reset formValidation
        $('#modal-account-logs-empty-form').data('formValidation').resetForm();
        //Reset form
        $('#modal-account-logs-empty-form').trigger("reset");
    }

    $('#modal-account-logs-remove').bind('hidden.bs.modal', function () {
        $('#modal-account-logs-remove-text').css('display', '');

        $('#modal-account-logs-remove-success-container').css('display', 'none');
        $('#modal-account-logs-remove-error-container').css('display', 'none');

        $('#modal-account-logs-remove-container-before').css('display', '');
        $('#modal-account-logs-remove-container-after').css('display', 'none');

        clearInterval(interval);

        var message = $('#modal-account-logs-remove-closing-btn').data('message');
        $('#modal-account-logs-remove-closing-btn').text(message + ' 5');
    });

    $('#modal-account-logs-remove').on('show.bs.modal', function () {
        var selectedRows = $('.selrow');
        var textEle = $('#modal-account-logs-remove-text');
        var text1 = $(textEle).data('text1');
        var text2 = $(textEle).data('text2');
        var text3 = $(textEle).data('text3');

        var titleEle = $('#modal-account-logs-remove-title');

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

        $('#modal-account-logs-remove-success-container').css('display', 'none');
        $('#modal-account-logs-remove-error-container').css('display', 'none');

        $('#modal-account-logs-remove-container-before').css('display', '');
        $('#modal-account-logs-remove-container-after').css('display', 'none');
    });

    $('#modal-account-logs-empty').bind('hidden.bs.modal', function () {
        $('#modal-account-logs-empty-text').css('display', '');

        $('#modal-account-logs-empty-success-container').css('display', 'none');
        $('#modal-account-logs-empty-error-container').css('display', 'none');

        $('#modal-account-logs-empty-container-before').css('display', '');
        $('#modal-account-logs-empty-container-after').css('display', 'none');

        clearInterval(interval);

        var message = $('#modal-account-logs-empty-closing-btn').data('message');
        $('#modal-account-logs-empty-closing-btn').text(message + ' 5');
    });

    $('#modal-account-logs-empty').on('show.bs.modal', function () {
        $('#modal-account-logs-empty-success-container').css('display', 'none');
        $('#modal-account-logs-empty-error-container').css('display', 'none');

        $('#modal-account-logs-empty-container-before').css('display', '');
        $('#modal-account-logs-empty-container-after').css('display', 'none');
    });
});