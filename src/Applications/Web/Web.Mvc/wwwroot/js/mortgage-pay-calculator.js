function PayCalculatorRound(number) {
    return Math.round(number * 100) / 100;
}

function PayCalculatorFormattedAmount(amount) {
    amount = PayCalculatorRound(amount);
    var formattedAmount = new String(amount);
    var i = formattedAmount.indexOf('.');
    if (i !== -1) {
        formattedAmount = formattedAmount.substr(0, i + 3);
        if (formattedAmount.length - i < 3)
            formattedAmount = formattedAmount + '0';
    }
    return formattedAmount;
}

function PayCalculatorCalculate(amount, rate, time) {
    if (rate <= 0) {
        if (time <= 0) {
            return amount;
        } else {
            return amount / time;
        }
    }
    var ln = Math.pow(1 + rate, time);
    return (amount / ln) / ((1 - (1 / ln)) / rate);
}

function PayCalculatorCalculateButtonClick() {
    $('#required-loan-amount-row, #loan-duration-row, #interest-rate-row').removeClass('error');
    $('#error-message').hide();
    var errorMessages = new Array();
    var requiredLoanAmount = parseFloat($('#required-loan-amount').val().replace(',', ''));
    var loanDuration = parseFloat($('#loan-duration').val().replace(',', ''));
    var interestRate = parseFloat($('#interest-rate').val().replace(',', ''));
    var yearsSelected = $('#selected-interval').html() == 'Years';
    if (yearsSelected & !isNaN(loanDuration))
        loanDuration = loanDuration * 12;
    var failed = false;
    if (isNaN(requiredLoanAmount) || (requiredLoanAmount < 1) || (requiredLoanAmount > 99999999)) {
        $('#required-loan-amount-row').addClass('error');
        errorMessages.push('Required loan amount should be between &pound;1 and &pound;99,999,999');
    }
    if (isNaN(loanDuration) || (loanDuration < 60) || (loanDuration > 480)) {
        $('#loan-duration-row').addClass('error');
        if (yearsSelected)
            errorMessages.push('Loan duration should be between 5 and 40 years');
        else
            errorMessages.push('Loan duration should be between 60 and 480 months');
    }
    if (isNaN(interestRate) || (interestRate < .001) || (interestRate > 1000)) {
        $('#interest-rate-row').addClass('error');
        errorMessages.push('Interest rate should be between 0.001% and 1000%');
    }
    if (errorMessages.length > 0) {
        $('#error-message').empty();
        for (var index = 0; index < errorMessages.length; index++) {
            $('#error-message').append('<li>' + errorMessages[index] + '</li>');
        }
        $('#error-message').show();
    } else {
        var interestOnlyAmount = (requiredLoanAmount * interestRate) / 1200.0;
        var repaymentAmount = PayCalculatorCalculate(requiredLoanAmount, interestRate / 1200.0, loanDuration);
        $('#interest-only-amount').text(PayCalculatorFormattedAmount(interestOnlyAmount).toString());
        $('#repayment-amount').text(PayCalculatorFormattedAmount(interestOnlyAmount + repaymentAmount).toString());
        $('#calculate-button').hide();
        $('#required-loan-amount, #loan-duration, #interest-rate, #selected-interval-dropdown').attr('disabled', 'disabled');
        $('#result').hide().fadeIn();
    }
    return false;
}

function PayCalculatorReset() {
    $('#required-loan-amount-row, #loan-duration-row, #interest-rate-row').removeClass('error');
    $('#error-message, #result').hide();
    $('#calculate-button').show();
    $('#required-loan-amount, #loan-duration, #interest-rate, #selected-interval-dropdown').removeAttr('disabled');
    return false;
}

function PayCalculatorSelectMonths() {
    $('#selected-interval').html('Months');
    return true;
}

function PayCalculatorSelectYears() {
    $('#selected-interval').html('Years');
    return true;
}

$(document).ready(function () {
    $('#calculate-button').click(PayCalculatorCalculateButtonClick);
    $('#reset-button').click(PayCalculatorReset);
    $('#select-months').click(PayCalculatorSelectMonths);
    $('#select-years').click(PayCalculatorSelectYears);
});