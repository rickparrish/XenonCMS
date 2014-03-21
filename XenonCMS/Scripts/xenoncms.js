function QueryStringValue(name) {
    var match = RegExp('[?&]' + name + '=([^&]*)').exec(window.location.search);
    return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
}

$(document).ready(function () {
    $("span.field-validation-error").each(function (index) {
        $(this).parent().parent().addClass("has-error");
    });
});
