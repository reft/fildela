$(function () {
    var maxBlockSize = 256 * 1024;//Each file will be split in 256 KB.
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
    var cancel = false;

    function barUpdateAccountFilesUpload(val) {
        if (val < 5)
            $('#modal-account-files-upload-progressbar').css('width', 5 + '%').attr('aria-valuenow', 5);
        else
            $('#modal-account-files-upload-progressbar').css('width', val + '%').attr('aria-valuenow', val);

        $('#modal-account-files-upload-progressbar').find('span').text(Math.round(val) + '%');
    }

    $(document).ready(function () {
        $("#modal-account-files-upload-file-input").bind('change', handleFileSelectAccountFilesUpload);
        if (window.File && window.FileReader && window.FileList && window.Blob) {
            // Great success! All the File APIs are supported.
        } else {
            var errorContainer = $('#modal-account-files-upload-error-container');
            $(errorContainer).find('.alert-danger').html($(errorContainer).data("errorapi") + '<i class="glyphicon glyphicon-remove pull-right"></i>');
        }
    });

    //Read the file and find out how many blocks we would need to split it.
    function handleFileSelectAccountFilesUpload(e) {
        resetUploadAccountFilesUpload();

        maxBlockSize = 256 * 1024;
        currentFilePointerurrentFilePointer = 0;
        totalBytesRemaining = 0;
        var files = e.target.files;
        selectedFile = files[0];
        var fileSize = selectedFile.size;

        if (selectedFile.name.length < 1 || selectedFile.name.length > 150) {
            accountUploadUploadError($('#modal-account-files-upload-error-container').data("length"));
            return false;
        }

        $('#modal-account-files-upload-file-input').prop('disabled', true);
        $('#modal-account-files-upload-file-input-wrapper').addClass('disabled');
        $('#modal-account-files-upload-file-input-wrapper').find('p').css('margin-left', '17px');
        $('#modal-account-files-upload-file-spin').css('display', '');

        if (fileSize < maxBlockSize) {
            maxBlockSize = fileSize;
            //console.log("max block size = " + maxBlockSize);
        }
        totalBytesRemaining = fileSize;
        if (fileSize % maxBlockSize == 0) {
            numberOfBlocks = fileSize / maxBlockSize;
        } else {
            numberOfBlocks = parseInt(fileSize / maxBlockSize, 10) + 1;
        }

        //console.log("total blocks = " + numberOfBlocks);

        $.ajax({
            url: '/Account/GetUploadSASURI',
            data: AddAntiForgeryToken({ FileName: selectedFile.name, FileSize: selectedFile.size }),
            type: 'POST',
            dataType: 'json',
            async: true,
            cache: false,
            success: function (result) {
                if (result.success) {
                    $('#modal-account-files-upload-error-container').css('display', 'none');
                    $('#modal-account-files-upload-success-container').css('display', 'none');

                    var title = $('#modal-account-files-upload-title');
                    $(title).text($(title).data('working'));

                    $('#modal-account-files-upload-container-idle').css('display', 'none');
                    $('#modal-account-files-upload-container-working').css('display', '');

                    baseUrl = result.blobSASURI;

                    var indexOfQueryStart = baseUrl.indexOf("?");
                    submitUri = baseUrl.substring(0, indexOfQueryStart) + "/" + result.blobName + baseUrl.substring(indexOfQueryStart);
                    blobName = result.blobName;

                    uploadFileInBlocksAccountFilesUpload();
                }
                else {
                    accountUploadUploadError(result.message);
                }
            },
            error: function (xhr, desc, err) {
                accountUploadUploadError($('#modal-account-files-upload-error-container').data("error"));
            },
            complete: function () {
                $('#modal-account-files-upload-file-spin').css('display', 'none');
                $('#modal-account-files-upload-file-input').prop('disabled', false);
                $('#modal-account-files-upload-file-input-wrapper').removeClass('disabled');
                $('#modal-account-files-upload-file-input-wrapper').find('p').css('margin-left', '');
            }
        });
    }

    var readerAccountFilesUpload = new FileReader();

    readerAccountFilesUpload.onloadend = function (evt) {
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
                    bytesUploadedytesUploaded += requestData.length;
                    var percentComplete = ((parseFloat(bytesUploadedytesUploaded) / parseFloat(selectedFile.size)) * 100).toFixed(2);
                    barUpdateAccountFilesUpload(percentComplete);

                    uploadFileInBlocksAccountFilesUpload();
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

    function uploadFileInBlocksAccountFilesUpload() {
        if (totalBytesRemaining > 0) {
            //console.log("current file pointer = " + currentFilePointerurrentFilePointer + " bytes read = " + maxBlockSize);
            var fileContent = selectedFile.slice(currentFilePointerurrentFilePointer, currentFilePointerurrentFilePointer + maxBlockSize);
            var blockId = blockIdPrefixlockIdPrefix + pad(blockIds.length, 6);
            //console.log("block id = " + blockId);
            blockIds.push(btoa(blockId));
            readerAccountFilesUpload.readAsArrayBuffer(fileContent);
            currentFilePointerurrentFilePointer += maxBlockSize;
            totalBytesRemaining -= maxBlockSize;
            if (totalBytesRemaining < maxBlockSize) {
                maxBlockSize = totalBytesRemaining;
            }
            //Enable cancel button

            if (blockId == 'block-000001') {
                $('#modal-account-files-upload-cancel-upload-btn').prop('disabled', false);
            }
        } else {
            commitBlockListAccountFilesUpload();
        }
    }

    function commitBlockListAccountFilesUpload() {
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

                barUpdateAccountFilesUpload(100);

                $('#modal-account-files-upload-cancel-upload-btn').prop('disabled', true);
                $('#modal-account-files-upload-cancel-upload-btn').find('p').css('margin-left', '17px');
                $('#modal-account-files-upload-complete-file-spin').css('display', '');

                //Upload metadata
                $.ajax({
                    url: '/Account/InsertFile',
                    data: AddAntiForgeryToken({ BlobName: blobName, FileName: selectedFile.name }),
                    type: 'POST',
                    dataType: 'json',
                    async: true,
                    cache: false,
                    success: function (result) {
                        if (result.success) {
                            //Updated stored bytes in files model
                            $('#modal-account-files-upload-stored-bytes').text(result.storedbytes);

                            //Update files content
                            $.ajax({
                                url: '/Account/UpdateFilesContent/',
                                type: 'GET',
                                dataType: 'html',
                                cache: false,
                                async: false,
                                success: function (data) {
                                    $('#account-files-content').html(data);
                                    mySorted = new SortedTable();

                                    //Reload scripts
                                    $.getScript('/Scripts/fildela-account-all.js');
                                    $("body").trigger("AccountFilesReloadScript");

                                    //Load search table script
                                    LoadSearchTable();

                                    $('#modal-account-files-upload-pretext').css('display', 'none');

                                    $('#modal-account-files-upload-success-container').css('display', '');
                                    $('#modal-account-files-upload-success-container').find('.alert-success').html(result.message + '<i class="glyphicon glyphicon-ok pull-right"></i>');
                                },
                                error: function () {
                                    accountUploadUploadError($('#modal-account-files-upload-error-container').data("error"));
                                },
                                complete: function () {
                                    resetUploadAccountFilesUpload();
                                }
                            });
                        }
                        else {
                            accountUploadUploadError(result.message);
                        }
                    },
                    error: function (xhr, desc, err) {
                        accountUploadUploadError($('#modal-account-files-upload-error-container').data("error"));
                    },
                    complete: function () {
                        $('#modal-account-files-upload-cancel-upload-btn').find('p').css('margin-left', '');
                        $('#modal-account-files-upload-complete-file-spin').css('display', 'none');
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

    function resetUploadAccountFilesUpload() {
        //Reset values
        numberOfBlocks = 1;
        selectedFile = null;
        currentFilePointerurrentFilePointer = 0;
        totalBytesRemaining = 0;
        blockIds = new Array();
        blockIdPrefixlockIdPrefix = "block-";
        submitUri = null;
        bytesUploadedytesUploaded = 0;
        baseUrl = null;
        blobName = null;
        cancel = false;

        //Reset progress bar
        barUpdateAccountFilesUpload(0);

        $('#modal-account-files-upload-cancel-upload-btn').prop('disabled', true);

        //Reset file input
        var control = $("#modal-account-files-upload-file-input");
        control.replaceWith(control = control.clone(true));

        //Reset title
        var title = $('#modal-account-files-upload-title');
        $(title).text($(title).data('idle'));

        //Reset containers
        $('#modal-account-files-upload-container-idle').css('display', '');
        $('#modal-account-files-upload-container-working').css('display', 'none');
    };

    function accountUploadUploadError(message) {
        $('#modal-account-files-upload-pretext').css('display', 'none');

        //Hide success message
        $('#modal-account-files-upload-success-container').css('display', 'none');

        //Show message
        $('#modal-account-files-upload-error-container').css('display', '');
        $('#modal-account-files-upload-error-container').find('.alert-danger').html(message + '<i class="glyphicon glyphicon-remove pull-right"></i>');

        resetUploadAccountFilesUpload();
    };

    $('#modal-account-files-upload-cancel-upload-btn').click(function (e) {
        cancel = true;

        if (navigator.appName == "Microsoft Internet Explorer") {
            window.document.execCommand('Stop');
        } else {
            window.stop();
        }

        accountUploadUploadError($('#modal-account-files-upload-error-container').data("cancel"));
    });
});