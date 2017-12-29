//Sort table
$(document).ready(function () {
    LoadSearchTable();
});

function LoadSearchTable() {
    var LightTableFilter = (function (Arr) {

        var _input;

        function _onInputEvent(e) {
            _input = e.target;
            var tables = document.getElementsByClassName(_input.getAttribute('data-table'));
            Arr.forEach.call(tables, function (table) {
                Arr.forEach.call(table.tBodies, function (tbody) {
                    Arr.forEach.call(tbody.rows, _filter);
                });
            });

            var displayedTableRows = $("tr[style*='display: table-row']").length;

            if ($(this).val().replace(/^\s\s*/, '').replace(/\s\s*$/, '') === '') {
                $('.account-search-hidden').css('display', '');

                //Hide cross
                $('#account-search-message-container').css('display', 'none');
                $('.account-search-message').text('');
                $('table').css('display', '');
                $('.account-search-input').css('border', "solid 1px #ccc");
                $('.account-search-input-reset').css('display', 'none');

                //Reset margin for container
                if (displayedTableRows === 0)
                    $('#account-search-message-container').css('margin-top', '-20px');
                else
                    $('#account-search-message-container').css('margin-top', '0px');

                $('#account-search-message-container').css('display', 'none');
                $('.account-search-message').text('');

                //Show sort caret
                $('.account-table-header').find('i').css('display', '');
                $('.account-table-header').find('i').css('display', '');
            }
            else {
                $('.account-search-hidden').css('display', 'none');

                $('.account-search-input').css('border-right', "0px");

                $('.account-search-input-reset').css('display', 'table-cell');
                $('.account-search-input').addClass('account-search-focus-border');

                if (displayedTableRows === 0) {
                    $('.account-search-message').addClass('account-search-space');
                    $('#account-search-message-container').css('margin-top', '-20px');

                    //Show sort caret
                    $('.account-table-header').find('i').css('display', '');
                    $('.account-table-header').find('i').css('display', '');

                    $('.account-search-message').html($(this).data("search_gave") + ' <span class="account-search-message-count">0</span> ' + $(this).data("hits"));
                    $('#account-search-message-container').css('display', '');
                    $('table').css('display', 'none');
                }
                else if (displayedTableRows > 1) {
                    $('.account-search-message').removeClass('account-search-space');
                    $('#account-search-message-container').css('margin-top', '0px');

                    //Show sort caret
                    $('.account-table-header').find('i').css('display', '');
                    $('.account-table-header').find('i').css('display', '');

                    $('.account-search-message').html($(this).data("search_gave") + ' <span class="account-search-message-count">' + displayedTableRows + '</span> ' + $(this).data("hits"));
                    $('#account-search-message-container').css('display', '');
                    $('table').css('display', '');
                }
                else {
                    $('.account-search-message').removeClass('account-search-space');
                    $('#account-search-message-container').css('margin-top', '0px');

                    //Hide sort caret
                    $('.account-table-header').find('i').css('display', 'none');
                    $('.account-table-header').find('i').css('display', 'none');

                    $('.account-search-message').html($(this).data("search_gave") + ' <span class="account-search-message-count">' + displayedTableRows + '</span> ' + $(this).data("hit"));
                    $('#account-search-message-container').css('display', '');
                    $('table').css('display', '');
                }
            }
        }

        function _filter(row) {
            var text = row.textContent.toLowerCase(), val = _input.value.toLowerCase().replace(/^\s\s*/, '').replace(/\s\s*$/, '');

            //Set value to all search boxes
            $('.account-search-input').val(_input.value);

            var displayedTableRows = $("tr[style*='display: table-row']");
            $(displayedTableRows).unhighlight();
            $(displayedTableRows).highlight(["jQuery", val, "plugin"]);

            row.style.display = text.indexOf(val) === -1 ? 'none' : 'table-row';
        }

        return {
            init: function () {
                var inputs = document.getElementsByClassName('light-table-filter');
                Arr.forEach.call(inputs, function (input) {
                    input.oninput = _onInputEvent;
                });
            }
        };
    })(Array.prototype);

    if ($('.account-table-tr').length > 0) {
        LightTableFilter.init();
    }
};