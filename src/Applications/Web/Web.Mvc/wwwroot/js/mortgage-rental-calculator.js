function BuyToLetCalculatorFormattedAmount(amount) {
    var formattedAmount = new String(amount);
    var i = formattedAmount.indexOf('.');
    if (i != -1) {
        formattedAmount = formattedAmount.substr(0, i + 3);
        if (formattedAmount.length - i < 3)
            formattedAmount = formattedAmount + '0';
    }
    return formattedAmount;
}

function BuyToLetCalculatorPropertValueInvalid(propertyValue) {
    return isNaN(propertyValue) || (propertyValue < 1) || (propertyValue > 99999999);
}

function BuyToLetCalculatorAvailableDepositInvalid(availableDeposit, propertyValueValid, propertyValue) {
    return isNaN(availableDeposit) ||
        (availableDeposit < 1) ||
        (availableDeposit > 99999999) ||
        (propertyValueValid && availableDeposit < propertyValue * 0.25) ||
        (propertyValueValid && availableDeposit >= propertyValue);
}

function BuyToLetCalculatorCalculateButtonClick() {
    $('#property-value-row, #available-deposit-row').removeClass('error');
    $('#error-message').hide();
    var errorMessages = new Array();
    var propertyValue = parseInt($('#property-value').val().replace(',', ''));
    var availableDeposit = parseInt($('#available-deposit').val().replace(',', ''));
    var failed = false;
    var propertyValueValid = !BuyToLetCalculatorPropertValueInvalid(propertyValue);
    if (!propertyValueValid) {
        $('#property-value-row').addClass('error');
        errorMessages.push('Property value should be between &pound;1 and &pound;99,999,999');
    }
    if (BuyToLetCalculatorAvailableDepositInvalid(availableDeposit, propertyValueValid, propertyValue)) {
        $('#available-deposit-row').addClass('error');
        if (propertyValueValid && availableDeposit >= propertyValue)
            errorMessages.push('Available deposit should be less than property value');
        else
            errorMessages.push('Available deposit should be between &pound;1 and &pound;99,999,999 and at least 25% of the property value');
    }
    if (errorMessages.length > 0) {
        $('#error-message').empty();
        for (var index = 0; index < errorMessages.length; index++) {
            $('#error-message').append('<li>' + errorMessages[index] + '</li>');
        }
        $('#error-message').show();
    } else {
        var monthlyRent = (propertyValue - availableDeposit) * 0.005;
        $('#monthly-rent').text(BuyToLetCalculatorFormattedAmount(monthlyRent).toString());
        $('#calculate-button').hide();
        $('#property-value, #available-deposit').attr('disabled', 'disabled');
        $('#result').hide().fadeIn();
    }
    return false;
}

function BuyToLetCalculatorReset() {
    $('#property-value-row, #available-deposit-row').removeClass('error');
    $('#error-message, #result').hide();
    $('#calculate-button').show();
    $('#property-value, #available-deposit').removeAttr('disabled');
    return false;
}

function BuyToLetCalculatorUpdateRequiredLoan() {
    var propertyValue = parseInt($('#property-value').val().replace(',', ''));
    var availableDeposit = parseInt($('#available-deposit').val().replace(',', ''));
    var propertyValueValid = !BuyToLetCalculatorPropertValueInvalid(propertyValue);
    var availableDepositValid = !BuyToLetCalculatorAvailableDepositInvalid(availableDeposit, propertyValueValid, propertyValue);
    if (propertyValueValid && availableDepositValid)
        $('#required-loan').val(propertyValue - availableDeposit);
    else
        $('#required-loan').val('');
    return false;
}

$(document).ready(function () {
    $('#calculate-button').click(BuyToLetCalculatorCalculateButtonClick);
    $('#reset-button').click(BuyToLetCalculatorReset);
    $('#property-value, #available-deposit').keyup(BuyToLetCalculatorUpdateRequiredLoan);
});