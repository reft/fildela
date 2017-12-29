$(function () {
    //Validate administration news post news
    $('#news-search-form').formValidation({
        framework: 'bootstrap',
        err: {
            container: '#news-search-form-error'
        },
        locale: 'sv_SE',
        fields: {
            newstitle: {
                validators: {
                    notEmpty: {
                    }
                }
            }
        }
    }).on('success.form.fv', function (e) {
        $('#news-search-btn-spinner').css('display', '');
        $('#news-search-btn-text').css('display', 'none');
    });
});