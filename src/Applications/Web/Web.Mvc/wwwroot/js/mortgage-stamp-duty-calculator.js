function StampDutyCalculatorFormattedAmount(amount) {
    var formattedAmount = new String(amount);
    var i = formattedAmount.indexOf('.');
    if (i !== -1) {
        formattedAmount = formattedAmount.substr(0, i + 3);
        if (formattedAmount.length - i < 3)
            formattedAmount = formattedAmount + '0';
    }
    return formattedAmount;
}

function StampDutyCalculatorPropertyPriceInvalid(propertyPrice) {
    return isNaN(propertyPrice) || (propertyPrice < 1) || (propertyPrice > 99999999);
}

function StampDutyCalculatorCalculateButtonClick() {
    $('#property-price-row').removeClass('error');
    $('#error-message').hide();
    var errorMessages = new Array();
    var propertyPrice = parseInt($('#property-price').val().replace(',', ''));
    var failed = false;
    var propertyPriceValid = !StampDutyCalculatorPropertyPriceInvalid(propertyPrice);
    if (!propertyPriceValid) {
        $('#property-price-row').addClass('error');
        errorMessages.push('Property price should be between &pound;1 and &pound;99,999,999');
    }
    if (errorMessages.length > 0) {
        $('#error-message').empty();
        for (var index = 0; index < errorMessages.length; index++) {
            $('#error-message').append('<li>' + errorMessages[index] + '</li>');
        }
        $('#error-message').show();
    } else {
        var stampDuty = 0.0;
        var secondHome = $('#second-home')[0].checked;
        var secondHomeAdjustment = secondHome ? 0.03 : 0.00;
        var increment = 0;
        var html = '';
        var rate = (0.12 + secondHomeAdjustment);
        var tax = 0;
        var taxableSum = 0;
        if (propertyPrice > 1500000) {
            taxableSum = propertyPrice - 1500000;
            tax = taxableSum * rate;
        }
        stampDuty += tax;
        html = '<tr><td>&pound;1500000+</td><td style="text-align: center;">' + Math.round(rate * 100) + '%</td><td style="text-align: right;">&pound;' + Math.round(taxableSum) + '</td><td style="text-align: right;">&pound;' + (Math.round(tax * 100) / 100).toFixed(2) + '</td></tr>' + html;
        rate = (0.10 + secondHomeAdjustment);
        tax = 0;
        taxableSum = 0;
        if (propertyPrice > 925000) {
            taxableSum = Math.min(propertyPrice - 925000, 1500000 - 925000)
            tax = taxableSum * rate;
        }
        stampDuty += tax;
        html = '<tr><td>&pound;925000-&pound;1500000</td><td style="text-align: center;">' + Math.round(rate * 100) + '%</td><td style="text-align: right;">&pound;' + Math.round(taxableSum) + '</td><td style="text-align: right;">&pound;' + (Math.round(tax * 100) / 100).toFixed(2) + '</td></tr>' + html;
        rate = (0.05 + secondHomeAdjustment);
        tax = 0;
        taxableSum = 0;
        if (propertyPrice > 250000) {
            taxableSum = Math.min(propertyPrice - 250000, 925000 - 250000);
            tax = taxableSum * rate;
        }
        stampDuty += tax;
        html = '<tr><td>&pound;250000-&pound;925000</td><td style="text-align: center;">' + Math.round(rate * 100) + '%</td><td style="text-align: right;">&pound;' + Math.round(taxableSum) + '</td><td style="text-align: right;">&pound;' + (Math.round(tax * 100) / 100).toFixed(2) + '</td></tr>' + html;
        rate = (0.02 + secondHomeAdjustment);
        tax = 0;
        taxableSum = 0;
        if (propertyPrice > 125000) {
            taxableSum = Math.min(propertyPrice - 125000, 250000 - 125000);
            tax = taxableSum * rate;
        }
        stampDuty += tax;
        html = '<tr><td>&pound;125000-&pound;250000</td><td style="text-align: center;">' + Math.round(rate * 100) + '%</td><td style="text-align: right;">&pound;' + Math.round(taxableSum) + '</td><td style="text-align: right;">&pound;' + (Math.round(tax * 100) / 100).toFixed(2) + '</td></tr>' + html;
        rate = (0.00 + secondHomeAdjustment);
        tax = 0;
        taxableSum = 0;
        if (propertyPrice > 0) {
            taxableSum = Math.min(propertyPrice, 125000);
            tax = taxableSum * rate;
        }
        stampDuty += tax;
        html = '<tr><td>&pound;0-&pound;125000</td><td style="text-align: center;">' + Math.round(rate * 100) + '%</td><td style="text-align: right;">&pound;' + Math.round(taxableSum) + '</td><td style="text-align: right;">&pound;' + (Math.round(tax * 100) / 100).toFixed(2) + '</td></tr>' + html;
        var effectiveRate = (stampDuty / propertyPrice) * 100.0;
        var effectiveRateRounded = Math.round(effectiveRate * 10) / 10;
        $('#results-body').html(html);
        $('#effective-rate').text(effectiveRateRounded);
        $('#stamp-duty').text(StampDutyCalculatorFormattedAmount(stampDuty).toString());
        $('#calculate-button').hide();
        $('#property-price, #second-home').attr('disabled', 'disabled');
        $('#result').hide().fadeIn();
    }
    return false;
}

function StampDutyCalculatorReset() {
    $('#property-price-row').removeClass('error');
    $('#error-message, #result').hide();
    $('#calculate-button').show();
    $('#property-price, #second-home').removeAttr('disabled');
    return false;
}

$(document).ready(function () {
    $('#calculate-button').click(StampDutyCalculatorCalculateButtonClick);
    $('#reset-button').click(StampDutyCalculatorReset);
});