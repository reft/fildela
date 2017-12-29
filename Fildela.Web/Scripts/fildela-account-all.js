(function () {
    'use strict';

    var $contextMenu;

    //Close popup when clicked
    $(".alert-popup").on("click", function () {
        a.close();
    });

    //Clear search value, (some browsers cache this)
    $('.account-search-input').val('');

    $('.account-user-menu-container').on('mouseenter', function (e) {
        if (!$('body').hasClass('mobile'))
            $('.account-user-menu-text').css('border-bottom', '1px solid #2895f1');
    });

    $('.account-user-menu-container').on('mouseleave', function (e) {
        if (!$('body').hasClass('mobile'))
            $('.account-user-menu-text').css('border-bottom', '1px solid rgba(255, 255, 255, 0.00)');
    });

    //Prevent user menu dropdown from closing (desktops)
    $(".account-user-menu-settings").bind("click", function (e) {
        e.stopPropagation();
    });

    //Prevent user menu dropdown from closing (mobiles)
    $('.account-user-menu-settings').bind("touchstart", function (e) {
        e.stopPropagation();
    });

    //Prevent user menu dropdown from closing (desktops)
    $(".account-user-menu-upgrade").bind("click", function (e) {
        e.stopPropagation();
    });

    //Prevent user menu dropdown from closing (mobiles)
    $('.account-user-menu-upgrade').bind("touchstart", function (e) {
        e.stopPropagation();
    });

    //Prevent user menu dropdown from closing (desktops)
    $(".account-user-menu-usage").bind("click", function (e) {
        e.stopPropagation();
    });

    //Prevent user menu dropdown from closing (mobiles)
    $('.account-user-menu-usage').bind("touchstart", function (e) {
        e.stopPropagation();
    });

    //Close usermenu, (required for mobiles)
    $("body").bind("touchstart", function (e) {
        if ($('.account-user-menu-status').hasClass('open') && !$(e.target).hasClass('account-user-menu-text') &&
            !$(e.target).hasClass('account-user-menu-icon') && !$(e.target).hasClass('account-user-menu-container')) {
            $('.account-user-menu-status').removeClass('open');
            e.preventDefault();
        }
    });

    //Display icon when sorting in account tables
    $(".account-table-header").on('click', function (e) {
        var displayedTableRows = $("tr[style*='display: table-row']").length;

        if (displayedTableRows > 1) {
            var eleID = $(this).attr('id');

            //Reverse sort
            if (eleID === 'account-log-header-pit' || eleID === 'account-log-header-ia' ||
                eleID === 'account-guestaccounts-header-datecreated' || eleID === 'account-guestaccounts-header-datestart' ||
                eleID === 'account-guestaccounts-header-dateend' ||
                eleID === 'account-links-header-datecreated' || eleID === 'account-links-header-datestart' ||
                eleID === 'account-links-header-dateend' ||
                eleID === 'account-files-header-created' ||
                eleID === 'account-files-header-size') {
                if ($(this).find('i').hasClass('fa fa-caret-up')) {
                    $('.account-table-header').find('i').removeClass('fa fa-caret-up');
                    $('.account-table-header').find('i').removeClass('fa fa-caret-down');
                    $(this).find('i').addClass('fa fa-caret-down');
                }
                else if (!$(this).find('i').hasClass('fa')) {
                    $(this).find('i').addClass('fa fa-caret-up');
                    $(this).click();
                }
                else {
                    $('.account-table-header').find('i').removeClass('fa fa-caret-up');
                    $('.account-table-header').find('i').removeClass('fa fa-caret-down');
                    $(this).find('i').addClass('fa fa-caret-up');
                }
            }
            else {
                if ($(this).find('i').hasClass('fa fa-caret-down')) {
                    $('.account-table-header').find('i').removeClass('fa fa-caret-up');
                    $('.account-table-header').find('i').removeClass('fa fa-caret-down');
                    $(this).find('i').addClass('fa fa-caret-up');
                }
                else {
                    $('.account-table-header').find('i').removeClass('fa fa-caret-up');
                    $('.account-table-header').find('i').removeClass('fa fa-caret-down');
                    $(this).find('i').addClass('fa fa-caret-down');
                }
            }
        }
    });

    //Hide dropdown menu when clicking on another item
    $(".account-table-tr").on('click', function (e) {
        if ($contextMenu != undefined)
            $contextMenu.hide();
    });

    //Remove cross and reset search result
    $(".account-search-input-reset").on("click", function () {
        $('.account-search-hidden').css('display', '');

        //Show sort caret
        $('.account-table-header').find('i').css('display', '');

        $('.account-search-input').val('');
        $('.account-search-input').css('border', "solid 1px #ccc");

        $('.account-search-input-reset').css('display', 'none');

        //Remove highlight
        var displayedTableRows = $("tr[style*='display: table-row']");
        $(displayedTableRows).unhighlight();

        $('.account-table-header').css('cursor', 'pointer');

        $('.account-table-tr').css('display', 'table-row');

        $('.account-search-input').removeClass('account-search-focus-border');

        $('#account-search-message-container').css('display', 'none');
        $('.account-search-message').text('');
        $('table').css('display', '');
    });

    //Remove focus border
    $(".account-search-input").blur(function () {
        if ($(this).val() === '')
            $('.account-search-input').removeClass('account-search-focus-border');
    });

    //Add focus border
    $(".account-search-input").focus(function () {
        $('.account-search-input').addClass('account-search-focus-border');
    });

    //Close event for all modals
    $('.modal').bind('hidden.bs.modal', function (event) {
        //Remove selected item and set display for tablemenu buttons in account views
        $('.selrow').removeClass('selrow');

        //Remove selected item text
        $('.account-selected-name').html('');

        SetTableMenuDisplay();
    });

    //Get selection count and set display
    function SetTableMenuDisplay() {
        var selected = $('.selrow');
        var count = $('.selrow').length

        switch (count) {
            case 0:
                $('.account-selected-name').html('');

                $('.account-tablemenu-single').css('display', 'none');
                $('.account-tablemenu-multiple').css('display', 'none');
                break;
            case 1:
                $('.account-selected-name').html('[<span style="color:#e74c3c";">' + $(selected).data('nameshorten') + '</span>]');

                $('.account-tablemenu-single').css('display', 'block');
                $('.account-tablemenu-multiple').css('display', 'block');
                break;
            default:
                var type = $(selected).data('type');

                $('.account-selected-name').html('[<span style="color:#e74c3c";">' + count + '</span>] ' + type);

                $('.account-tablemenu-multiple').css('display', 'block');
                $('.account-tablemenu-single').css('display', 'none');
        }
    }

    //Dropdown menu for account, files, right click
    $("body").on("contextmenu", ".account-table-tr", function (e) {
        var selectedItem;

        if ($(e.target).hasClass('account-files-preview-link') || $(e.target).hasClass('account-files-edit-name-container'))
            selectedItem = $(e.target).parent();
        else
            selectedItem = $(e.target);

        //Hide edit container if not right clicking on the same selected row
        if ($(this).find('.account-files-edit-name-container').css('display') != 'table') {
            //Hide textbox and blur
            $('.account-files-edit-name-container').css('display', 'none');

            //Remove btn text to prevent client search from matching
            $('.account-files-edit-name-btn-text').text('');
            //Show filename
            $('.account-files-preview-link').css('display', '');
            //Show fileicon
            $('.account-file-icon').css('display', '');
        }

        if (!$(selectedItem).hasClass('account-table-header') && !$('body').hasClass('mobile') && !$(selectedItem).hasClass('news-table-row')) {
            //Remove previous selected rows
            $('.selrow').not(selectedItem.parent()).removeClass('selrow');

            //Set selected row
            $(selectedItem).parent().addClass('selrow');

            SetTableMenuDisplay();

            //Close account menu
            $('.account-user-menu-status.open').removeClass('open');
            //Close nav menu
            $('#header-dropdown-authenticated').removeClass('open');

            //Open dropdown menu
            $contextMenu = $('#' + $(selectedItem).attr('class'));
            $contextMenu.css({
                display: "block",
                left: e.pageX + 1,
                top: e.pageY
            });
            return false;
        }
    });

    $("body").on('click', function (e) {
        //Hide dropdown if the clicked element doesn't have class 'dropdown-item' and tagname 'A' or if the click was inside a modal
        if (!$(e.target).hasClass('dropdown-item') && $(e.target).prop("tagName") == 'A' || $(this).hasClass('modal-open') || $(e.target).hasClass('account-table-header')) {
            if ($contextMenu != undefined)
                $contextMenu.hide();
        }
        else {
            //Hide dropdown menu and remove selected items when dropdown meny is shown and left-clicking outside
            if (!$(e.target).parent().hasClass('account-table-tr') &&
                !$(e.target).hasClass('account-table-header') && !$(e.target).hasClass('account-files-edit-name-container') &&
                !$(e.target).hasClass('account-files-edit-name-input') && !$(e.target).hasClass('account-files-edit-name-submit')) {
                $('.selrow').removeClass('selrow');

                if ($contextMenu != undefined)
                    $contextMenu.hide();

                SetTableMenuDisplay();
            }

            //Remove selected item when left-clicking outside
            if (!$(e.target).parent().hasClass('account-table-tr') && !$(e.target).hasClass('account-table-header') && !$(e.target).hasClass('account-files-edit-name-input') && !$(e.target).hasClass('account-files-edit-name-submit') && !$(e.target).hasClass('account-files-edit-name-btn-text')) {
                $('.selrow').removeClass('selrow');
                SetTableMenuDisplay();
            }
        }

        //Hide edit file name
        if ($('.account-files-edit-name-container:visible').length > 0 && !$(e.target).hasClass('account-tablemenu-name') && !$(e.target).hasClass('account-files-edit-name-input') && !$(e.target).hasClass('account-files-edit-name-submit') && !$(e.target).hasClass('account-files-edit-name-btn-text')) {
            //Hide textbox and blur
            $('.account-files-edit-name-container').css('display', 'none');
            //Remove btn text to prevent client search from matching
            $('.account-files-edit-name-btn-text').text('');
            //Show filename
            $('.account-files-preview-link').css('display', '');
            //Show fileicon
            $('.account-file-icon').css('display', '');
        }
    });

    //Cancel search when ESC is pressed
    $(document).keyup(function (e) {
        if (e.keyCode == 27) {
            if ($(".account-search-input").is(":focus")) {
                $('.account-search-input-reset').click();

                $('.account-search-input').blur();
            }
        }
    });

    //Account settings and usage tab open event
    $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {
        //Get clicked element (button)
        var button = $(e.target);
        var prevButton = $(button.parent()).find(".btn-primary");

        //Remove active class from previous button
        $(prevButton).removeClass('btn-primary active').addClass('btn-default');

        //Add active class to the new button
        button.addClass('btn-primary active').removeClass('btn-default');

        //Fade message
        $('.account-settings-message-container').fadeOut(150);

        //Reset form when a tab is clicked, (delay)
        var form = $(prevButton).attr('href') + '-form';

        if (form != undefined && $(form).data('formValidation') != null) {
            setTimeout(function () {
                //Reset formValidation
                $(form).data('formValidation').resetForm();

                //Reset form
                $(form)[0].reset();
            }, 150);
        }
    })
})(jQuery);

