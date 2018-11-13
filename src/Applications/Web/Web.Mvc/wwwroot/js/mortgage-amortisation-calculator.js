var interestPayments = [];
var principalPayments = [];
var balances = [];
var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

function runAmortisationMain(principal, loanDuration, apr, yearsSelected) {

    var date = new Date();
    var month = date.getMonth();
    var year = date.getFullYear();
    month = parseInt($('#amortisation-month').val());
    year = parseInt($('#amortisation-year').val());

    var payments = loanDuration;
    var monthlyInterest = apr / 12.0;
    var monthlyPayment = principal * monthlyInterest / (1 - (1 / Math.pow(1 + monthlyInterest, payments)));

    var headlineText = (yearsSelected ? (loanDuration / 12) + ' years' : loanDuration + ' months');
    $('#amortisation-headline').html('&pound;' + principal + ' at ' + (apr * 100.0) + '% over ' + headlineText + ', monthly payment of &pound;' + rounding(monthlyPayment) + '*');
    var html = '<table class="table"><thead><tr><th>Date</th><th>Interest this payment</th><th>Principal this payment</th><th>Interest paid to date</th><th>Prinicipal paid to date</th><th>Balance to date</th></tr></thead>';

    var principalPaid = 0;
    var interestPaid = 0;
    var yearInterestPaid = 0;
    var yearPrincipalPaid = 0;

    html += '<tbody>';

    for (var i = 1; i <= payments; i++) {

        var interestPayment = principal * monthlyInterest;
        var principalPayment = monthlyPayment - interestPayment;

        principal = principal - principalPayment;
        principalPaid = -(-principalPayment - principalPaid);
        interestPaid = -(-interestPayment - interestPaid);

        var paymentDate = new Date(year, month % 12);
        var roundedInterestPayment = rounding(interestPayment);
        var roundedPrincipalPayment = rounding(principalPayment);
        var roundedPrincipal = rounding(principal);
        html += '<tr><td>' + months[month % 12] + ', ' + year + '</td>';
        html += '<td class="int">&pound;' + roundedInterestPayment + '</td><td class="pri">&pound;' + roundedPrincipalPayment + '</td><td class="int">&pound;' + rounding(interestPaid) + '</td><td class="pri">&pound;' + rounding(principalPaid) + '</td><td>&pound;' + roundedPrincipal + '</td></tr>';
        interestPayments.push([paymentDate.getTime(), parseFloat(roundedInterestPayment)]);
        principalPayments.push([paymentDate.getTime(), parseFloat(roundedPrincipalPayment)]);
        balances.push([paymentDate.getTime(), parseFloat(roundedPrincipal)]);
        month++;

        yearInterestPaid += interestPayment;
        yearPrincipalPaid += principalPayment;

        if (month % 12 === 0) {
            html += '<tr><td><strong>' + year + '</strong></td><td><strong>&pound;' + rounding(yearInterestPaid) + '</strong></td><td><strong>&pound;' + rounding(yearPrincipalPaid) + '</strong></td>' +
                '<td><strong>&pound;' + rounding(interestPaid) + '</strong></td><td><strong>&pound;' + rounding(principalPaid) + '</strong></td><td><strong>&pound;' + rounding(principal) + '</strong></td></tr>';
            yearInterestPaid = 0;
            yearPrincipalPaid = 0;
            year++;
        }
    }
    if (month % 12 !== 0) {
        html += '<tr><td><strong>' + year + '</strong></td><td><strong>&pound;' + rounding(yearInterestPaid) + '</strong></td><td><strong>&pound;' + rounding(yearPrincipalPaid) + '</strong></td>' +
            '<td><strong>&pound;' + rounding(interestPaid) + '</strong></td><td><strong>&pound;' + rounding(principalPaid) + '</strong></td><td><strong>&pound;' + rounding(principal) + '</strong></td></tr>';
    }
    html += '</tbody></table>';
    $('#amortisation-table').html(html);

    return { monthlyPayment: monthlyPayment };
}

function plotGraph(principal, monthlyPayment) {
    // create the chart
    $('#amortisation-chart').highcharts({
        credits: {
            enabled: false
        },
        chart: {
            type: 'area'
        },
        title: {
            text: ''
        },
        tooltip: {
            formatter: function () {
                var date = new Date(this.x);
                var month = date.getMonth();
                var year = date.getFullYear();
                var s = '<b>' + months[month] + ' ' + year + '</b>';
                $.each(this.points, function () {
                    s += '<br/>' + this.series.name + ': ' + this.y.toFixed(2);
                });
                return s;
            },
            shared: true
        },
        xAxis: {
            type: 'datetime'
        },
        yAxis: [{
            min: 0,
            max: monthlyPayment,
            title: {
                text: 'Monthly payment'
            },
            stackLabels: {
                enabled: false
            }
        },
        {
            min: 0,
            max: principal,
            opposite: true,
            title: {
                text: 'Balance'
            }
        }],
        legend: {
            align: 'center',
            x: -30,
            verticalAlign: 'top',
            y: 25,
            floating: true,
            backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || 'white',
            borderColor: '#CCC',
            borderWidth: 1,
            shadow: false
        },
        plotOptions: {
            column: {
                stacking: 'normal',
                dataLabels: {
                    enabled: false,
                    color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'white',
                    style: {
                        textShadow: '0 0 3px black'
                    }
                }
            }
        },
        series: [{
            name: 'Principal',
            data: principalPayments,
            type: 'area',
            color: '#03265E',
            yAxis: 0
        }, {
            name: 'Interest',
            data: interestPayments,
            type: 'area',
            color: '#119FAD',
            yAxis: 0
        }, {
            name: 'Balance',
            data: balances,
            type: 'spline',
            color: '#000000',
            yAxis: 1
        }]
    });
}

function reset() {
    $('#error-message').hide();
    $('#principal-label, #apr-label, #loan-duration-label, #principal-field, #apr-field, #loan-duration-field').removeClass('error');
    $('#amortisation-results').hide();
    $('#amortisation-table').empty();
    $('#amortisation-chart').empty();
    interestPayments.length = 0;
    principalPayments.length = 0;
    balances.length = 0;
    return false;
}

function amortisationCalculatorCalculateButtonClick() {
    reset();
    var errorMessages = [];
    var principal = parseInt($('#principal').val().replace(',', ''));
    if (isNaN(principal) || (principal < 1) || (principal > 99999999)) {
        $('#principal-label, #principal-field').addClass('error');
        errorMessages.push('Required loan amount should be between &pound;1 and &pound;99,999,999');
    }
    var apr = parseFloat($('#apr').val());
    if (isNaN(apr) || (apr < 0.1) || (apr > 20)) {
        $('#apr-label, #apr-field').addClass('error');
        errorMessages.push('Interest rate should be between 0.1% and 20%');
    };
    apr = apr / 100.0;
    var yearsSelected = $('#selected-interval').html() == 'Years';
    var loanDuration = parseInt($('#loan-duration').val());
    if (yearsSelected & !isNaN(loanDuration))
        loanDuration = loanDuration * 12;
    if (isNaN(loanDuration) || (loanDuration < 60) || (loanDuration > 480)) {
        $('#loan-duration-label, #loan-duration-field').addClass('error');
        if (yearsSelected)
            errorMessages.push('Loan duration should be between 5 and 40 years');
        else
            errorMessages.push('Loan duration should be between 60 and 480 months');
    }
    if (errorMessages.length > 0) {
        $('#error-message').empty();
        for (var index = 0; index < errorMessages.length; index++) {
            $('#error-message').append('<li>' + errorMessages[index] + '</li>');
        }
        $('#error-message').show();
    } else {
        $('#amortisation-results').show();
        var results = runAmortisationMain(principal, loanDuration, apr, yearsSelected);
        plotGraph(principal, results.monthlyPayment);
    }
    return false;
}

function rounding(n) {
    return n.toFixed(2);
}

function setupStartDate() {
    var date = new Date();
    var currentMonth = date.getMonth();
    var currentYear = date.getFullYear();
    for (var year = currentYear - 25; year <= currentYear + 1; year++) {
        var selected = (year === currentYear) ? 'selected="selected" ' : '';
        $('#amortisation-year').append('<option ' + selected + 'value="' + year + '">' + year + '</option>');
    }
    $('#amortisation-month').val(currentMonth);
}

function selectMonths() {
    $('#selected-interval').html('Months');
    return true;
}

function selectYears() {
    $('#selected-interval').html('Years');
    return true;
}

$(document).ready(function () {
    setupStartDate();
    $('#calculate-button').click(amortisationCalculatorCalculateButtonClick);
    $('#reset-button').click(reset);
    $('#select-months').click(selectMonths);
    $('#select-years').click(selectYears);
});