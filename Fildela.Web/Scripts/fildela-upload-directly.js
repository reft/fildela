$(function () {
    var maxBlockSize = 256 * 1024; //Each file will be split in 256 KB.
    var numberOfBlocks = 1;
    var selectedFile = null;
    var currentFilePointer = 0;
    var totalBytesRemaining = 0;
    var blockIds = new Array();
    var blockIdPrefix = "block-";
    var submitUri = null;
    var bytesUploaded = 0;
    var baseUrl = null;
    var blobName = null;
    var uploadComplete = null;
    var cancel = false;

    function barUpdateUploadDirectly(val) {
        if (val < 5)
            $('#upload-directly-progressbar').css('width', 5 + '%').attr('aria-valuenow', 5);
        else
            $('#upload-directly-progressbar').css('width', val + '%').attr('aria-valuenow', val);

        $('#upload-directly-progressbar').find('span').text(Math.round(val) + '%');
    }

    $(document).ready(function () {
        $("#modal-upload-directly-file-input").bind('change', handleFileSelectUploadDirectly);
        if (window.File && window.FileReader && window.FileList && window.Blob) {
            // Great success! All the File APIs are supported.
        } else {
            var errorContainer = $('#modal-upload-directly-error-container');
            $(errorContainer).find('.alert-danger').html($(errorContainer).data("errorapi") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
        }
    });

    //Read the file and find out how many blocks we would need to split it.
    function handleFileSelectUploadDirectly(e) {
        resetUploadUploadDirectly();

        maxBlockSize = 256 * 1024;
        currentFilePointer = 0;
        totalBytesRemaining = 0;
        var files = e.target.files;
        selectedFile = files[0];
        var fileSize = selectedFile.size;

        if (selectedFile.name.length < 1 || selectedFile.name.length > 150) {
            uploadDirectlyUploadError($('#modal-upload-directly-error-container').data("length"));
            return false;
        }

        $('#modal-upload-directly-file-input').prop('disabled', true);
        $('#modal-upload-directly-file-input-wrapper').addClass('disabled');
        $('#modal-upload-directly-file-input-wrapper').find('p').css('margin-left', '17px');
        $('#modal-upload-directly-file-spin').css('display', '');

        if (fileSize < maxBlockSize) {
            maxBlockSize = fileSize;
        }
        totalBytesRemaining = fileSize;
        if (fileSize % maxBlockSize == 0) {
            numberOfBlocks = fileSize / maxBlockSize;
        } else {
            numberOfBlocks = parseInt(fileSize / maxBlockSize, 10) + 1;
        }

        $.ajax({
            url: '/UploadDirectly/GetUploadSASURI',
            data: AddAntiForgeryToken({ FileName: selectedFile.name, FileSize: selectedFile.size }),
            type: 'POST',
            dataType: 'json',
            async: true,
            cache: false,
            success: function (result) {
                if (result.success) {
                    //console.log("max block size = " + maxBlockSize);
                    //console.log("total blocks = " + numberOfBlocks);

                    var title = $('#modal-direct-upload-title');
                    $(title).text($(title).data('working'));

                    $('#modal-upload-directly-container-idle').css('display', 'none');
                    $('#modal-upload-directly-container-working').css('display', '');
                    $('#modal-upload-directly-error-container').css('display', 'none');

                    baseUrl = result.blobSASURI;

                    var indexOfQueryStart = baseUrl.indexOf("?");
                    submitUri = baseUrl.substring(0, indexOfQueryStart) + "/" + result.blobName + baseUrl.substring(indexOfQueryStart);
                    blobName = result.blobName;

                    uploadFileInBlocksUploadDirectly();
                }
                else {
                    uploadDirectlyUploadError(result.message);
                }
            },
            error: function (xhr, desc, err) {
                uploadDirectlyUploadError($('#modal-upload-directly-error-container').data("error"));
            },
            complete: function () {
                $('#modal-upload-directly-file-spin').css('display', 'none');
                $('#modal-upload-directly-file-input').prop('disabled', false);
                $('#modal-upload-directly-file-input-wrapper').removeClass('disabled');
                $('#modal-upload-directly-file-input-wrapper').find('p').css('margin-left', '');
            }
        });
    }

    var readerUploadDirectly = new FileReader();

    readerUploadDirectly.onloadend = function (evt) {
        if (evt.target.readyState == FileReader.DONE) { // DONE == 2
            var uri = submitUri + '&comp=block&blockid=' + blockIds[blockIds.length - 1];
            var requestData = new Uint8Array(evt.target.result);
            $.ajax({
                url: uri,
                type: "PUT",
                data: requestData,
                processData: false,
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('x-ms-blob-type', 'BlockBlob');
                    //xhr.setRequestHeader('Content-Length', requestData.length);
                },
                success: function (data, status) {
                    //console.log(data);
                    //console.log(status);
                    bytesUploaded += requestData.length;
                    var percentComplete = ((parseFloat(bytesUploaded) / parseFloat(selectedFile.size)) * 100).toFixed(2);
                    barUpdateUploadDirectly(percentComplete);

                    uploadFileInBlocksUploadDirectly();
                },
                error: function (xhr, desc, err) {
                    if (!cancel) {
                        console.log(desc);
                        console.log(err);
                    }
                }
            });
        }
    };

    function uploadFileInBlocksUploadDirectly() {
        if (totalBytesRemaining > 0) {
            //console.log("current file pointer = " + currentFilePointer + " bytes read = " + maxBlockSize);
            var fileContent = selectedFile.slice(currentFilePointer, currentFilePointer + maxBlockSize);
            var blockId = blockIdPrefix + pad(blockIds.length, 6);
            //console.log("block id = " + blockId);
            blockIds.push(btoa(blockId));
            readerUploadDirectly.readAsArrayBuffer(fileContent);
            currentFilePointer += maxBlockSize;
            totalBytesRemaining -= maxBlockSize;
            if (totalBytesRemaining < maxBlockSize) {
                maxBlockSize = totalBytesRemaining;
            }
            //Enable cancel button
            if (blockId == 'block-000001') {
                $('#modal-upload-directly-cancel-upload-btn').prop('disabled', false);
            }
        } else {
            commitBlockListUploadDirectly();
        }
    }

    function commitBlockListUploadDirectly() {
        var uri = submitUri + '&comp=blocklist';
        //console.log(uri);
        var requestBody = '<?xml version="1.0" encoding="utf-8"?><BlockList>';
        for (var i = 0; i < blockIds.length; i++) {
            requestBody += '<Latest>' + blockIds[i] + '</Latest>';
        }
        requestBody += '</BlockList>';
        //console.log(requestBody);
        $.ajax({
            url: uri,
            type: "PUT",
            data: requestBody,
            beforeSend: function (xhr) {
                xhr.setRequestHeader('x-ms-blob-content-type', selectedFile.type);
                //xhr.setRequestHeader('Content-Length', requestBody.length);
            },
            success: function (data, status) {
                //console.log(data);
                //console.log(status);

                barUpdateUploadDirectly(100);

                $('#modal-upload-directly-cancel-upload-btn').prop('disabled', true);
                $('#modal-upload-directly-cancel-upload-btn').find('p').css('margin-left', '17px');
                $('#modal-upload-directly-complete-file-spin').css('display', '');

                //Upload metadata
                $.ajax({
                    url: '/UploadDirectly/InsertUploadDirectly',
                    data: AddAntiForgeryToken({ BlobName: blobName, FileName: selectedFile.name }),
                    type: 'POST',
                    dataType: 'json',
                    async: true,
                    cache: false,
                    success: function (result) {
                        if (result.success) {
                            $('#modal-upload-directly-complete-container').html(result.viewString);

                            uploadComplete = true;

                            $("body").trigger("UploadDirectlyComplete");

                            var sharefile = $('#modal-direct-upload-complete-title').data('sharefile');
                            $('#modal-direct-upload-complete-title').text(sharefile + ' - ' + selectedFile.name);
                            $('#modal-direct-upload').modal('hide');
                            $('#modal-direct-upload-complete').modal('show');
                        }
                        else {
                            uploadDirectlyUploadError(result.message);
                        }
                    },
                    error: function (xhr, desc, err) {
                        uploadDirectlyUploadError($('#modal-upload-directly-error-container').data("error"));
                    },
                    complete: function () {
                        $('#modal-upload-directly-cancel-upload-btn').find('p').css('margin-left', '');
                        $('#modal-upload-directly-complete-file-spin').css('display', 'none');
                    }
                });
            },
            error: function (xhr, desc, err) {
                console.log(desc);
                console.log(err);
            }
        });

    }

    function pad(number, length) {
        var str = '' + number;
        while (str.length < length) {
            str = '0' + str;
        }
        return str;
    }

    function resetUploadUploadDirectly() {
        //Reset values
        numberOfBlocks = 1;
        selectedFile = null;
        currentFilePointer = 0;
        totalBytesRemaining = 0;
        blockIds = new Array();
        blockIdPrefix = "block-";
        submitUri = null;
        bytesUploaded = 0;
        baseUrl = null;
        blobName = null;
        uploadComplete = null;
        cancel = false;

        //Reset progress bar
        barUpdateUploadDirectly(0);

        $('#modal-upload-directly-cancel-upload-btn').prop('disabled', true);

        //Reset file input
        var control = $("#modal-upload-directly-file-input");
        control.replaceWith(control = control.clone(true));
    };

    function uploadDirectlyUploadError(message) {
        //Reset title
        var title = $('#modal-direct-upload-title');
        $(title).text($(title).data('idle'));

        //Reset containers
        $('#modal-upload-directly-container-idle').css('display', '');
        $('#modal-upload-directly-container-working').css('display', 'none');

        //Hide note div
        $('#modal-upload-directly-note-container').css('display', 'none');

        //Hide text
        $('#modal-upload-directly-container-idle').find('.modal-content-text').css('display', 'none');

        //Show message
        $('#modal-upload-directly-error-container').css('display', '');
        $('#modal-upload-directly-error-container').find('.alert-danger').html(message + '<i class="glyphicon glyphicon-remove pull-right"></i>');

        resetUploadUploadDirectly();
    };

    $(document).ready(function () {
        $("body").on("UploadDirectlyComplete", function () {
            //Go back button for upload directly
            $('#modal-upload-directly-go-back').click(function (event) {
                uploadComplete = false;

                $('#modal-upload-directly-container-idle').css('display', '');
                $('#modal-upload-directly-container-working').css('display', 'none');

                var title = $('#modal-direct-upload-title');
                $(title).text($(title).data('idle'));

                $('#modal-upload-directly-complete-container').empty();

                $('#modal-direct-upload-complete').modal('hide');
                $('#modal-direct-upload').modal('show');
            });

            $('#modal-direct-upload-link-text').on('keyup', function (e) {
                if ($('body').hasClass('modbile')) {
                    var downloadURL = $('#modal-direct-upload-link-text').data('downloadurl');
                    var input = $('#modal-direct-upload-link-text');

                    $(input).val(downloadURL);
                }
            });

            $('#modal-direct-upload-link-text').on('touchend', function (e) {
                $('#modal-direct-upload-link-text').focus();
                $('#modal-direct-upload-link-text').setSelectionRange(0, 9999);
            });

            $('#modal-direct-upload-link-text').mousedown(function (event) {
                switch (event.which) {
                    case 3:
                        $('#modal-direct-upload-link-text').select();
                        $('#modal-direct-upload-link-text').focus();
                        break;
                }
            });

            $('#modal-direct-upload-link-text').on("click", function () {
                $('#modal-direct-upload-link-text').select();
            });

            $('#modal-direct-upload-complete').on('show.bs.modal', function () {
                if ($('body').hasClass('mobile')) {
                    $('#modal-direct-upload-link-text').prop('readonly', false);
                }
            });

            $("#modal-direct-upload-send-email-input").focus(function () {
                $('#modal-upload-directly-complete-success-container').css('display', 'none');
                $('#modal-upload-directly-complete-error-container').css('display', 'none');
            });

            //Validate send email form
            $('#modal-direct-upload-send-email-form').formValidation({
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
                            }
                        }
                    },
                    BlobName: {
                        validators: {
                            notEmpty: {
                            },
                            emailAddress: {
                            }
                        }
                    },
                    StartDate: {
                        validators: {
                            notEmpty: {
                            },
                            emailAddress: {
                            }
                        }
                    },
                    EndDate: {
                        validators: {
                            notEmpty: {
                            },
                            emailAddress: {
                            }
                        }
                    }
                }
            }).on('success.form.fv', function (e) {
                //Prevent form submission
                e.preventDefault();

                $.ajax({
                    url: '/UploadDirectly/SendDownloadLinkToEmail/',
                    data: $(e.target).serialize(),
                    type: 'POST',
                    beforeSend: SendDownloadLinkToEmailOnBegin,
                    success: SendDownloadLinkToEmailOnSuccess,
                    error: function () {
                        SendDownloadLinkToEmailOnError();
                    }
                });
            });
        });

        $('#modal-direct-upload').bind('show.bs.modal', function (event) {
            //Show note div
            $('#modal-upload-directly-note-container').css('display', '');

            //Only reset form if no file has been uploaded
            if (uploadComplete) {
                $('#modal-direct-upload').modal('hide');
                $('#modal-direct-upload-complete').modal('show');
                return false;
            }
        });

        $('#modal-direct-upload-complete').each(function () {
            var t = $('#modal-direct-upload-complete'),
                d = t.find('.modal-dialog'),
                fadeClass = (t.is('.fade') ? 'fade' : '');

            // read and store dialog height
            d.data('height', d.height());
        });

        //Phase two - set margin-top on every dialog show
        $('#modal-direct-upload-complete').on('show.bs.modal', function () {
            //Show note div
            $('#modal-upload-directly-note-container').css('display', '');

            var t = $(this),
                d = t.find('.modal-dialog'),
                dh = 364,
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

        $("body").trigger("UploadDirectlyComplete");
    });

    //Send download link to email form OnBegin
    function SendDownloadLinkToEmailOnBegin() {
        //Show spinner
        $('#modal-direct-upload-send-email-spin').css('display', '');
        //Hide text
        $('#modal-direct-upload-send-email-text').css('display', 'none');

        //Blur input
        $('#modal-direct-upload-send-email-input').blur();

        //Set read only inputs
        $('#modal-direct-upload-send-email-input').prop('readonly', true);
        //Disable submit button
        $('#modal-direct-upload-send-email-btn').prop('disabled', true);
    }

    //Send download link to email form OnSuccess
    function SendDownloadLinkToEmailOnSuccess(json) {
        //Blur input
        $('#modal-direct-upload-send-email-input').blur();

        //Error
        if (json.message === undefined) {
            $('#modal-upload-directly-complete-error-container').find('.alert-danger').html($('#modal-direct-upload-send-email-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
            $('#modal-upload-directly-complete-error-container').css('display', '');
        }
        else {
            //Show message
            if (json.success) {
                $('#modal-upload-directly-complete-success-container').find('.alert-success').html(json.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');
                $('#modal-upload-directly-complete-success-container').css('display', '');
            }
            else {
                $('#modal-upload-directly-complete-error-container').find('.alert-danger').html(json.message + '<i class="glyphicon glyphicon-remove pull-right"></i>');
                $('#modal-upload-directly-complete-error-container').css('display', '');
            }
        }

        //Set read only inputs
        $('#modal-direct-upload-send-email-input').prop('readonly', false);
        //Disable submit button
        $('#modal-direct-upload-send-email-btn').prop('disabled', false);

        //Reset formValidation
        $('#modal-direct-upload-send-email-form').data('formValidation').resetForm();
        //Reset form
        $('#modal-direct-upload-send-email-form').trigger("reset");

        //Hide spinner
        $('#modal-direct-upload-send-email-spin').css('display', 'none');
        //Show text
        $('#modal-direct-upload-send-email-text').css('display', '');
    }

    //Send download link to email form OnError
    function SendDownloadLinkToEmailOnError() {
        //Blur input
        $('#modal-direct-upload-send-email-input').blur();

        //Show default error message
        $('#modal-upload-directly-complete-error-container').find('.alert-danger').html($('#modal-direct-upload-send-email-form').data("error") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
        $('#modal-upload-directly-complete-error-container').css('display', '');

        //Set read only inputs
        $('#modal-direct-upload-send-email-input').prop('readonly', false);
        //Disable submit button
        $('#modal-direct-upload-send-email-btn').prop('disabled', false);

        //Reset formValidation
        $('#modal-direct-upload-send-email-form').data('formValidation').resetForm();
        //Reset form
        $('#modal-direct-upload-send-email-form').trigger("reset");

        //Hide spinner
        $('#modal-direct-upload-send-email-spin').css('display', 'none');
        //Show text
        $('#modal-direct-upload-send-email-text').css('display', '');
    }

    $('#modal-upload-directly-cancel-upload-btn').click(function (e) {
        cancel = true;

        if (navigator.appName == "Microsoft Internet Explorer") {
            window.document.execCommand('Stop');
        } else {
            window.stop();
        }

        //Reset title
        var title = $('#modal-direct-upload-title');
        $(title).text($(title).data('idle'));

        uploadDirectlyUploadError($('#modal-upload-directly-error-container').data("cancel"));

        //Reset containers
        $('#modal-upload-directly-container-idle').css('display', '');
        $('#modal-upload-directly-container-working').css('display', 'none');
    });

    $('#modal-direct-upload-complete').on('show.bs.modal', function () {
        if ($('body').hasClass('mobile')) {
            $('#modal-direct-upload-link-text').prop('readonly', false);
        }

        var downloadURL = $('#modal-direct-upload-link-text').data('downloadurl');
        var input = $('#modal-direct-upload-link-text');

        $(input).val(downloadURL);
    });

    $('#modal-direct-upload-link-text').on('keyup', function (e) {
        if ($('body').hasClass('mobile')) {
            var downloadURL = $('#modal-direct-upload-link-text').data('downloadurl');
            var input = $('#modal-direct-upload-link-text');

            $(input).val(downloadURL);
        }
    });

    $('#modal-direct-upload-link-text').on('touchend', function (e) {
        $('#modal-direct-upload-link-text').focus();
        $('#modal-direct-upload-link-text').setSelectionRange(0, 9999);
    });

    $('#modal-direct-upload-link-text').mousedown(function (event) {
        switch (event.which) {
            case 3:
                $('#modal-direct-upload-link-text').select();
                $('#modal-direct-upload-link-text').focus();
                break;
        }
    });

    $('#modal-direct-upload-link-text').on("click", function () {
        $('#modal-direct-upload-link-text').select();
    });
});