var _formElementSubmitted = null;

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

function FormValidate(element) {
    var error = false;
    $(element).find('input, textarea').each(function () {
        error = FormFieldValidate($(this)) || error;
    });
    $(element).find('button').prop('disabled', error);
    return error;
}

function FormFieldKeyUp(e) {
    FormValidate($(this).parent().parent());
}

function FormInitialise() {
    FormValidate($(this));
}

function FormInitialiseAll() {
    $('.rcms-form').each(FormInitialise);
}

function FormDisableAll() {
    $('.rcms-form button').prop('disabled', true);
}

function FormSubmitSuccess(data) {
    var htmlEncodedMessage = $('<div/>').text(data.message).html();
    $(_formElementSubmitted).find('.spinner').remove();
    $(_formElementSubmitted).append('<p>' + htmlEncodedMessage + '</p>');
    $(_formElementSubmitted).find('form').remove();
    FormInitialiseAll();
    _formElementSubmitted = null;
}

function FormSubmitFailure() {
    $(_formElementSubmitted).find('.spinner').remove();
    $(_formElementSubmitted).find('form').show();
    FormInitialiseAll();
    _formElementSubmitted = null;
}

function FormGetActionRequest(element) {
    var content = { fields: [] };
    $(element).find('input, textarea').each(function () {
        var formFieldId = parseInt($(this).attr('name').split('_')[1]);
        var value = $.trim($(this).val());
        content.fields.push({ formFieldId: formFieldId, value: value });
    });
    return content;
}

function FormSubmitButtonClick() {
    FormDisableAll();
    _formElementSubmitted = $(this).parent().parent();
    $(_formElementSubmitted).prepend('<div class="spinner"></div>');
    $(_formElementSubmitted).find('form').hide();
    var content = FormGetActionRequest(_formElementSubmitted);
    var url = '/api/v1/element/tenants/6/elementtypes/eafbd5ab-8c98-4edc-b8e1-42f5e8bfe2dc/elements/187/action?pageid=19';
    $.ajax({
        url: url,
        type: 'post',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(content)
    }).done(FormSubmitSuccess).fail(FormSubmitFailure);
    return false;
}

$(document).ready(function () {
    $('.rcms-form input, .rcms-form textarea').keyup(FormFieldKeyUp);
    $('.rcms-form button').click(FormSubmitButtonClick);
    FormInitialiseAll();
});