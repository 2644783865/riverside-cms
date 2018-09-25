function FormFieldValidate(field) {
    var required = parseInt($(field).data('required')) === 1;
    var value = $.trim($(field).val());    
    var error = required && value === '';
    if (error)
        $(field).parent().addClass('has-error');
    else
        $(field).parent().removeClass('has-error');
    return error;
}

function FormValidate(form) {
    var error = false;
    $(form).find('input, textarea').each(function () {
        error = FormFieldValidate($(this)) || error;
    });
    $(form).find('button').prop('disabled', error);
    return error;
}

function FormSubmit() {
    var form = $(this).parent();
    var message = $(form).data('submittedmessage');
    var htmlEncodedMessage = $('<div/>').text(message).html();
    $(form).empty();
    $(form).append('<p>' + htmlEncodedMessage + '</p>');
    return false;
}

function FormFieldKeyUp(e) {
    var form = $(this).parent().parent();
    FormValidate(form);
}

$(document).ready(function () {
    $('.rcms-form input, .rcms-form textarea').keyup(FormFieldKeyUp);
    $('.rcms-form button').click(FormSubmit);
});