function BorrowCalculatorCalculateButtonClick() {
    $('#applicant-one-guaranteed-income-row, #applicant-one-non-guaranteed-income-row, #applicant-two-guaranteed-income-row, #applicant-two-non-guaranteed-income-row').removeClass('error');
    $('#error-message').hide();
    var errorMessages = new Array();
    var appplicantOneGuaranteedAmount = parseInt($('#applicant-one-guaranteed-income').val().replace(',', ''));
    var appplicantOneNonGuaranteedAmount = parseInt($('#applicant-one-non-guaranteed-income').val().replace(',', ''));
    var appplicantTwoGuaranteedAmount = parseInt($('#applicant-two-guaranteed-income').val().replace(',', ''));
    var appplicantTwoNonGuaranteedAmount = parseInt($('#applicant-two-non-guaranteed-income').val().replace(',', ''));
    if (isNaN(appplicantOneGuaranteedAmount) || (appplicantOneGuaranteedAmount < 0) || (appplicantOneGuaranteedAmount > 99999999)) {
        $('#applicant-one-guaranteed-income-row').addClass('error');
        errorMessages.push('Applicant 1 guaranteed annual income should be between &pound;0 and &pound;99,999,999');
    }
    if (isNaN(appplicantOneNonGuaranteedAmount) || (appplicantOneNonGuaranteedAmount < 0) || (appplicantOneNonGuaranteedAmount > 99999999)) {
        $('#applicant-one-non-guaranteed-income-row').addClass('error');
        errorMessages.push('Applicant 1 non-guaranteed annual income should be between &pound;0 and &pound;99,999,999');
    }
    if (isNaN(appplicantTwoGuaranteedAmount) || (appplicantTwoGuaranteedAmount < 0) || (appplicantTwoGuaranteedAmount > 99999999)) {
        $('#applicant-two-guaranteed-income-row').addClass('error');
        errorMessages.push('Applicant 2 guaranteed annual income should be between &pound;0 and &pound;99,999,999');
    }
    if (isNaN(appplicantTwoNonGuaranteedAmount) || (appplicantTwoNonGuaranteedAmount < 0) || (appplicantTwoNonGuaranteedAmount > 99999999)) {
        $('#applicant-two-non-guaranteed-income-row').addClass('error');
        errorMessages.push('Applicant 2 non-guaranteed annual income should be between &pound;0 and &pound;99,999,999');
    }
    if (errorMessages.length > 0) {
        $('#error-message').empty();
        for (var index = 0; index < errorMessages.length; index++) {
            $('#error-message').append('<li>' + errorMessages[index] + '</li>');
        }
        $('#error-message').show();
    } else {
        var borrowAmount = (appplicantOneGuaranteedAmount * 5) + (appplicantOneNonGuaranteedAmount * 2.5) +
            (appplicantTwoGuaranteedAmount * 5) + (appplicantTwoNonGuaranteedAmount * 2.5);
        $('#borrow-amount').text(borrowAmount);
        $('#calculate-button').hide();
        $('#applicant-one-guaranteed-income, #applicant-one-non-guaranteed-income, #applicant-two-guaranteed-income, #applicant-two-non-guaranteed-income').attr('disabled', 'disabled');
        $('#result').hide().fadeIn();
    }
    return false;
}

function BorrowCalculatorReset() {
    $('#applicant-one-guaranteed-income-row, #applicant-one-non-guaranteed-income-row, #applicant-two-guaranteed-income-row, #applicant-two-non-guaranteed-income-row').removeClass('error');
    $('#error-message, #result').hide();
    $('#calculate-button').show();
    $('#applicant-one-guaranteed-income, #applicant-one-non-guaranteed-income, #applicant-two-guaranteed-income, #applicant-two-non-guaranteed-income').removeAttr('disabled');
    return false;
}

$(document).ready(function () {
    $('#calculate-button').click(BorrowCalculatorCalculateButtonClick);
    $('#reset-button').click(BorrowCalculatorReset);
});