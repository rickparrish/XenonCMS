function QueryStringValue(name) {
    var match = RegExp('[?&]' + name + '=([^&]*)').exec(window.location.search);
    return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
}

$(document).ready(function () {
    // Check if the CDN loaded the css
    if ($('#BootstrapCssTest').is(':visible') === true) {
        $('#SiteTheme').attr('href', '/Content/bootstrap.' + SiteTheme + '.min.css');
    }
    
    // Make errors appear more bootstrap-like
    $("span.field-validation-error").each(function (index) {
        $(this).parent().parent().addClass("has-error");
    });
});