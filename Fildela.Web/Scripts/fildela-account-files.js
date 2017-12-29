$(function () {
    var isajaxrunning = false;
    var interval;

    $('#modal-account-files-remove-form').formValidation({
    }).on('success.form.fv', function (e) {
        //Prevent form submission
        e.preventDefault();

        var rows = $('.selrow');

        var rowKeys = new Array();
        $(rows).each(function (n) {
            rowKeys[n] = $(this).data('rowkey');
        });

        $.ajax({
            url: '/Account/DeleteFiles/',
            data: AddAntiForgeryToken({ RowKeys: rowKeys }),
            type: 'POST',
            traditional: true,
            dataType: "json",
            beforeSend: DeleteFilesOnBegin,
            success: function (result) {
                if (result.success) {
                    //Updated stored bytes in files model
                    $('#modal-account-files-upload-stored-bytes').text(result.storedbytes);

                    $.ajax({
                        url: '/Account/UpdateFilesContent/',
                        type: 'GET',
                        success: function (data) {
                            $('#account-files-content').html(data);

                            mySorted = new SortedTable();

                            //Reload scripts
                            $.getScript('/Scripts/fildela-account-all.js');
                            $("body").trigger("AccountFilesReloadScript");

                            //Load search table script
                            LoadSearchTable();

                            DeleteFilesOnComplete();

                            $('#modal-account-files-remove-error-container').css('display', 'none');

                            $('#modal-account-files-remove-success-container').css('display', '');
                            $('#modal-account-files-remove-success-container').find('.alert-success').html(result.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');

                            $('#modal-account-files-remove-container-before').css('display', 'none');
                            $('#modal-account-files-remove-container-after').css('display', '');

                            var counter = 5;
                            var message = $('#modal-account-files-remove-closing-btn').data('message');
                            interval = setInterval(function () {
                                counter--;

                                $('#modal-account-files-remove-closing-btn').text(message + ' ' + counter);

                                if (counter == 0) {
                                    clearInterval(interval);

                                    $('#modal-account-files-remove').modal('hide');

                                    $('#modal-account-files-remove-closing-btn').text(message + ' 5');
                                }
                            }, 1000);
                        },
                        error: function () {
                            DeleteFilesOnComplete();

                            $('#modal-account-files-remove-content-text').css('display', 'none');

                            $('#modal-account-files-remove-error-container').find('.alert-danger').html($('#modal-account-files-remove-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                            $('#modal-account-files-remove-error-container').css('display', '');
                        }
                    });
                }
                else {
                    DeleteFilesOnComplete();

                    $('#modal-account-files-remove-content-text').css('display', 'none');

                    $('#modal-account-files-remove-error-container').css('display', '');
                    $('#modal-account-files-remove-error-container').find('.alert-danger').html(result.message + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                }
            },
            error: function (result) {
                DeleteFilesOnComplete();

                $('#modal-account-files-remove-content-text').css('display', 'none');

                $('#modal-account-files-remove-error-container').find('.alert-danger').html($('#modal-account-files-remove-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#modal-account-files-remove-error-container').css('display', '');
            }
        });
    });

    function DeleteFilesOnBegin() {
        isajaxrunning = true;

        //Show spinner and add margin to submit button text
        $('#modal-account-files-remove-spin').css('display', '');
        $('#modal-account-files-remove-spin').prev().css('margin-left', '17px');

        //Disable submit button
        $('#modal-account-files-remove-btn').prop('disabled', true);
    }

    function DeleteFilesOnComplete() {
        isajaxrunning = false;

        //Hide spinner and reset margin
        $('#modal-account-files-remove-spin').css('display', 'none');
        $('#modal-account-files-remove-spin').prev().css('margin-left', '');

        //Enable submit button
        $('#modal-account-files-remove-btn').prop('disabled', false);

        //Reset formValidation
        $('#modal-account-files-remove-form').data('formValidation').resetForm();
        //Reset form
        $('#modal-account-files-remove-form').trigger("reset");
    }

    $('#modal-account-files-empty-form').formValidation({
    }).on('success.form.fv', function (e) {
        //Prevent form submission

        e.preventDefault();
        var rows = $('.account-table-tr');

        var rowKeys = new Array();
        $(rows).each(function (n) {
            rowKeys[n] = $(this).data('rowkey');
        });

        $.ajax({
            url: '/Account/DeleteFiles/',
            data: AddAntiForgeryToken({ RowKeys: rowKeys }),
            type: 'POST',
            traditional: true,
            dataType: "json",
            beforeSend: EmptyFilesOnBegin,
            success: function (result) {
                if (result.success) {
                    //Updated stored bytes in files model
                    $('#modal-account-files-upload-stored-bytes').text(result.storedbytes);

                    $.ajax({
                        url: '/Account/UpdateFilesContent/',
                        type: 'GET',
                        success: function (data) {
                            $('#account-files-content').html(data);

                            mySorted = new SortedTable();

                            //Reload scripts
                            $.getScript('/Scripts/fildela-account-all.js');
                            $("body").trigger("AccountFilesReloadScript");

                            //Load search table script
                            LoadSearchTable();

                            EmptyFilesOnComplete();

                            $('#modal-account-files-empty-error-container').css('display', 'none');

                            $('#modal-account-files-empty-success-container').css('display', '');
                            $('#modal-account-files-empty-success-container').find('.alert-success').html(result.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');

                            $('#modal-account-files-empty-container-before').css('display', 'none');
                            $('#modal-account-files-empty-container-after').css('display', '');

                            var counter = 5;
                            var message = $('#modal-account-files-empty-closing-btn').data('message');
                            interval = setInterval(function () {
                                counter--;

                                $('#modal-account-files-empty-closing-btn').text(message + ' ' + counter);

                                if (counter == 0) {
                                    clearInterval(interval);

                                    $('#modal-account-files-empty').modal('hide');

                                    $('#modal-account-files-empty-closing-btn').text(message + ' 5');
                                }
                            }, 1000);
                        },
                        error: function () {
                            EmptyFilesOnComplete();

                            $('#modal-account-files-empty-content-text').css('display', 'none');

                            $('#modal-account-files-empty-error-container').find('.alert-danger').html($('#modal-account-files-empty-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                            $('#modal-account-files-empty-error-container').css('display', '');
                        }
                    });
                }
                else {
                    EmptyFilesOnComplete();

                    $('#modal-account-files-empty-content-text').css('display', 'none');

                    $('#modal-account-files-empty-error-container').css('display', '');
                    $('#modal-account-files-empty-error-container').find('.alert-danger').html(result.message + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                }
            },
            error: function (result) {
                EmptyFilesOnComplete();

                $('#modal-account-files-empty-content-text').css('display', 'none');

                $('#modal-account-files-empty-error-container').find('.alert-danger').html($('#modal-account-files-empty-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#modal-account-files-empty-error-container').css('display', '');
            }
        });
    });

    function EmptyFilesOnBegin() {
        isajaxrunning = true;

        //Show spinner and add margin to submit button text
        $('#modal-account-files-empty-spin').css('display', '');
        $('#modal-account-files-empty-spin').prev().css('margin-left', '17px');

        //Disable submit button
        $('#modal-account-files-empty-btn').prop('disabled', true);
    }

    function EmptyFilesOnComplete() {
        isajaxrunning = false;

        //Hide spinner and reset margin
        $('#modal-account-files-empty-spin').css('display', 'none');
        $('#modal-account-files-empty-spin').prev().css('margin-left', '');

        //Enable submit button
        $('#modal-account-files-empty-btn').prop('disabled', false);

        //Reset formValidation
        $('#modal-account-files-empty-form').data('formValidation').resetForm();
        //Reset form
        $('#modal-account-files-empty-form').trigger("reset");
    }

    $('#modal-account-files-remove').bind('hidden.bs.modal', function (e) {
        $('#modal-account-files-remove-success-container').css('display', 'none');
        $('#modal-account-files-remove-error-container').css('display', 'none');

        $('#modal-account-files-remove-container-before').css('display', '');
        $('#modal-account-files-remove-container-after').css('display', 'none');

        clearInterval(interval);

        var message = $('#modal-account-files-remove-closing-btn').data('message');
        $('#modal-account-files-remove-closing-btn').text(message + ' 5');

        $('#modal-account-files-remove-content-text').css('display', '');
    });

    $('#modal-account-files-remove').on('show.bs.modal', function () {
        var selectedRows = $('.selrow');
        var textEle = $('#modal-account-files-remove-content-text');
        var text1 = $(textEle).data('text1');
        var text2 = $(textEle).data('text2');
        var text3 = $(textEle).data('text3');
        var text4 = $(textEle).data('text4');

        var titleEle = $('#modal-account-files-remove-title');

        if (selectedRows.length === 1) {
            $(textEle).html(text1 + ' [<span style="color:#e74c3c;">' + $('.selrow:first').data('name') + '</span>] ' + text3);
            $(titleEle).text($(titleEle).data('text1'));
        }
        else if (selectedRows.length > 1) {
            $(textEle).html(text1 + ' <span style="color:#e74c3c;">[' + selectedRows.length + ']</span> ' + text2 + ' ' + text3);
            $(titleEle).text($(titleEle).data('text2'));
        }
        else {
            $(textEle).text(text4);
            $(titleEle).text($(titleEle).data('text2'));
        }

        $('#modal-account-files-remove-success-container').css('display', 'none');
        $('#modal-account-files-remove-error-container').css('display', 'none');

        $('#modal-account-files-remove-container-before').css('display', '');
        $('#modal-account-files-remove-container-after').css('display', 'none');
    });

    $('#modal-account-files-empty').bind('hidden.bs.modal', function (e) {
        $('#modal-account-files-empty-success-container').css('display', 'none');
        $('#modal-account-files-empty-error-container').css('display', 'none');

        $('#modal-account-files-empty-container-before').css('display', '');
        $('#modal-account-files-empty-container-after').css('display', 'none');

        clearInterval(interval);

        var message = $('#modal-account-files-empty-closing-btn').data('message');
        $('#modal-account-files-empty-closing-btn').text(message + ' 5');

        $('#modal-account-files-empty-content-text').css('display', '');
    });

    $('#modal-account-files-empty').on('show.bs.modal', function () {
        $('#modal-account-files-empty-success-container').css('display', 'none');
        $('#modal-account-files-empty-error-container').css('display', 'none');

        $('#modal-account-files-empty-container-before').css('display', '');
        $('#modal-account-files-empty-container-after').css('display', 'none');
    });

    $("body").on("AccountFilesReloadScript", function () {
        //Cancel edit name when ESC is pressed
        $(document).keyup(function (e) {
            if (e.keyCode == 27) {
                if ($('.account-files-edit-name-container:visible').length > 0) {
                    //Remove btn text to prevent client search from matching
                    $('.account-files-edit-name-btn-text').text('');
                    //Hide textbox and blur
                    $('.account-files-edit-name-container').css('display', 'none');
                    //Show filename
                    $('.account-files-preview-link').css('display', '');
                    //Show fileicon
                    $('.account-file-icon').css('display', '');
                }
            }
        });

        $('.account-files-edit-name-input').bind('keyup', function (e) {
            if (e.keyCode === 13 && !isajaxrunning) { // 13 (enter)
                var btn = $(this).next().find('.btn');

                $(btn).click();
            }
        });

        $(".account-files-edit-name-submit").on("click", function () {
            var row = $('.selrow:first');

            var inputtext = $(row).find('.account-files-edit-name-input').val();

            var namedisplayshidden = $(row).find('.account-files-table-name-display-hidden');
            var namedisplaysshown = $(row).find('.account-files-table-name-display-shown');

            //Remove btn text to prevent client search from matching
            $('.account-files-edit-name-btn-text').text('');

            if (inputtext.length > 0 && inputtext.length < 151 && inputtext != $(namedisplayshidden).text() && !isajaxrunning) {
                isajaxrunning = true;

                var activeSortHeader = '';
                var isSortDown;
                if ($('.sortedplus').length > 0) {
                    isSortDown = true;
                    activeSortHeader = '#' + $('.sortedplus').attr('id');
                }
                else {
                    isSortDown = false;
                    activeSortHeader = '#' + $('.sortedminus').attr('id');
                }

                var btn = $(this);
                var btntext = $(btn).find('.account-files-edit-name-btn-text');
                var btnspinner = $(btn).find('.input-group-spinner');

                var rowkey = $(btn).data('rowkey');
                $.ajax({
                    url: '/Account/UpdateFileName/',
                    data: AddAntiForgeryToken({ RowKey: rowkey, FileName: inputtext }),
                    dataType: "json",
                    type: 'POST',
                    beforeSend: function () {
                        $(btn).prop('disabled', true);
                        $(btntext).css('display', 'none');
                        $(btnspinner).css('display', '');
                    },
                    success: function (data) {
                        if (data) {
                            $(namedisplaysshown).text(inputtext);
                            $(namedisplayshidden).text(inputtext.toLowerCase());

                            //Sort
                            if (activeSortHeader === '#account-files-header-name')
                                $('#account-files-header-type').click();
                            else
                                $('#account-files-header-name').click();

                            $(activeSortHeader).click();
                            if (isSortDown)
                                $(activeSortHeader).click();
                        }
                    },
                    complete: function () {
                        $(btn).prop('disabled', false);
                        $(btnspinner).css('display', 'none');
                        $(btntext).css('display', '');

                        editNameComplete();
                    }
                });
            }
            else {
                editNameComplete();
            }

            function editNameComplete() {
                $(row).find('.account-files-preview-link').css('display', '');
                $(row).find('.account-file-icon').css('display', '');
                $(row).find('.account-files-edit-name-container').css('display', 'none');

                isajaxrunning = false;
            }
        });

        $('.account-files-preview-link').on('click', function () {
            var selectedItem = $(this).closest('.account-table-tr');

            //Remove previous selected rows
            $('.selrow').not(selectedItem).removeClass('selrow');

            //Set selected row
            $(selectedItem).addClass('selrow');

            $('.account-selected-name').html('[<span style="color:#e74c3c";">' + $(selectedItem).data('nameshorten') + '</span>]');
        });

        //Hide dropdown menu when dropdown menu download item is clicked
        $("#account-files-dropdown-download").on("click", function () {
            $("#account-files-contextmenu").hide();
            $('.account-table-tr').find('td').removeClass('account-table-td-selected');
        });

        //Hide dropdown menu when dropdown menu edit name item is clicked
        $("#account-files-dropdown-name").on("click", function () {
            $("#account-files-contextmenu").hide();
            $('.account-table-tr').find('td').removeClass('account-table-td-selected');
        });

        //Show edit file name input
        $(".account-tablemenu-name").on("click", function () {
            if (!isajaxrunning) {
                var filename = $('.selrow:first').find('.account-files-preview-link');
                var selectedItem = $('.selrow:first');

                //Hide filename
                $(selectedItem).find('.account-files-preview-link').css('display', 'none');
                //Hide fileicon
                $(selectedItem).find('.account-file-icon').css('display', 'none');


                //Show btn text
                $('.account-files-edit-name-btn-text').text($('.account-files-edit-name-btn-text:first').data('text'));
                //Display edit filename container
                $(selectedItem).find('.account-files-edit-name-container').css('display', '');
                //Set edit filename textbox value
                $(selectedItem).find('.account-files-edit-name-input').val(filename.text()).select();
            }
        });

        $(".account-files-download").on("click", function () {
            if (!isajaxrunning) {
                isajaxrunning = true;

                var row = $('.selrow:first');
                var fileIcon = $(row).find('img');
                var spinner = $(row).find('.spinner-table');

                //Hide icon
                $(fileIcon).css('display', 'none');
                //Show spinner
                $(spinner).css('display', '');

                //Remove btn text to prevent client search from matching
                $('.account-files-edit-name-btn-text').text('');

                $(row).find('.account-files-preview-link').css('display', '');
                $(row).find('.account-files-edit-name-container').css('display', 'none');

                var blobName = $(row).data('rowkey');

                $.ajax({
                    url: '/Account/DownloadFile',
                    data: { BlobName: blobName },
                    type: 'GET',
                    success: function (result) {
                        window.open(result, '_self', false)
                    },
                    complete: function () {
                        //Show icon
                        $(fileIcon).css('display', '');
                        //Hide spinner
                        $(spinner).css('display', 'none');

                        isajaxrunning = false;
                    }
                });
            }
        });
    });

    $("body").trigger("AccountFilesReloadScript");
});