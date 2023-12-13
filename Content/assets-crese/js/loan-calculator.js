if ($("#personal-loan-calculator").length) {
jQuery( function( $ ) {

	// Prinipal Slider      

	
	$("#finlon-principal-slide").slider({
		range: "min",
		min: 30000,
		max: 500000,
		value: 100000,
		step: 1000,
		slide: function (event, ui) {
			$("#principal-show").text(ui.value);
			$('#principal-input').val(ui.value);
			loancalculate();
		},
		change: function(){
			chart_render();
		}
	});
	$("#principal-show").text($("#finlon-principal-slide").slider("value"));
	$('#principal-input').val($('#finlon-principal-slide').slider('value'));

	$("#principal-input").on('keyup',function(event) {
		if (event.keyCode === 13) {
			principal_value();
		}
	})
	$("#principal-input").blur(function(event) {
			principal_value();
	})

	// totalyear-show Slider

	$("#finlon-year-slide").slider({
		range: "min",
		min: 6,
		max: 60,
		step: 1,
		value: 12,
		slide: function (event, ui) {
			$("#totalyear-show").text(ui.value);
			$("#loan-year-input").val(ui.value)
			loancalculate();
		},
		change: function(){
			chart_render();
		}
	});
	$("#totalyear-show").text($("#finlon-year-slide").slider("value"));
	$('#loan-year-input').val($('#finlon-year-slide').slider('value'));
	
	$('#loan-year-input').blur(function(){      
		loan_year_value()
	})

	$("#loan-year-input").on('keyup',function(event) {
		if (event.keyCode === 13) {
			loan_year_value();
		}
	})
	
	// finlon-intrest-slide

	$("#finlon-intrest-slide").slider({
		range: "min",
		min: 8,
		max: 24,
		step:0.1,
		value: 10,
		slide: function(event, ui) {
			$("#intrest").text(ui.value);
			$("#interest-rate-input").val(ui.value)
			loancalculate();
		},
		change: function(){
			chart_render();
		}

	});
	$("#intrest").text($("#finlon-intrest-slide").slider("value"));
	$("#interest-rate-input").val($("#finlon-intrest-slide").slider("value"));

	$("#interest-rate-input").blur(function(){
			interest_rate_value()
	}); 
	$("#interest-rate-input").on('keyup',function(event) {
		if (event.keyCode === 13) {
			interest_rate_value();
		}
	})
	loancalculate();
	chart_render();
});
// if (typeof(functionName) != 'loancalculate') {
	function loancalculate()
	{
		var loanAmount=jQuery("#principal-show").text();
		var numberOfMonths=jQuery("#totalyear-show").text();
		var rateOfInterest=jQuery("#intrest").text();

		var monthlyInterestRatio = (rateOfInterest/100)/12;

		var top = Math.pow((1+monthlyInterestRatio), numberOfMonths);
		var bottom = top -1;
		var sp = top / bottom;
		var emi = ((loanAmount * monthlyInterestRatio) * sp);
		var full = numberOfMonths * emi;
		var interest = full - loanAmount;
		var int_pge = (interest / full) * 100;
		$("#tbl_int-pge").html(int_pge.toFixed(2)+" %");
		$("#tbl_loan_pge").html((100-int_pge.toFixed(2)+" %"));

		var emi_str = emi.toFixed(2).toString().replace(/,/g,"").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
		var loanAmount_str = loanAmount.toString().replace(/,/g,"").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
		var full_str = full.toFixed(2).toString().replace(/,/g,"").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
		var int_str = interest.toFixed(2).toString().replace(/,/g,"").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
		$input_principal = $('#principal-input').val().toString().replace(/,/g,"").replace(/\B(?=(\d{3})+(?!\d))/g, ",");

		jQuery("#monthly-emi").html(emi_str);
		jQuery("#total-interest").html(int_str);
		jQuery("#total-amount").html(full_str);
		jQuery('#principal-input').val($input_principal);

		if ($('#loan-chart').length) {
			// chart_render()
		}
	}
// }
}

function principal_value(){
		$text_val=$('#principal-input').val().toString()
		$new_text_val = parseInt($text_val.replace(/,/g,''))
		var max = $("#finlon-principal-slide").slider("option", "max");
		if ($new_text_val > max) {
			$('#principal-input').val(max)
		}
		var min = $("#finlon-principal-slide").slider("option", "min");
		if ($new_text_val < min) {
			$('#principal-input').val(min)
		}

		$("#finlon-principal-slide").slider({
		range: "min",
		min: 30000,
		max: 500000,
		value: $new_text_val,
		step: 1000,
		slide: function (event, ui) {
			$("#principal-show").text(ui.value);
			$('#principal-input').val(ui.value);
			loancalculate();
		}
	});
	$("#principal-show").text($("#finlon-principal-slide").slider("value"));
		loancalculate();
		chart_render()
}

function loan_year_value(){
	$loan_year = $("#loan-year-input").val()
	var max = $("#finlon-year-slide").slider("option", "max");
		if ($loan_year > max) {
			$('#loan-year-input').val(max)
		}
	var min = $("#finlon-year-slide").slider("option", "min");
		if ($loan_year < min) {
			$('#loan-year-input').val(min)
		}


		$("#finlon-year-slide").slider({
		range: "min",
		min: 6,
		max: 60,
		step: 1,
		value: $loan_year,
		slide: function (event, ui) {
			$("#totalyear-show").text(ui.value);
			$("#loan-year-input").val(ui.value)
			loancalculate();
		}
	});
		$("#totalyear-show").text($("#finlon-year-slide").slider("value"));
		loancalculate();
		chart_render();
}

function interest_rate_value(){
	$loan_rate = $("#interest-rate-input").val()    
	var max = $("#finlon-year-slide").slider("option", "max");
		if ($loan_rate > max) {
			$('#interest-rate-input').val(max)
		}
	
	
	$("#finlon-intrest-slide").slider({
		range: "min",
		min: 8,
		max: 24,
		step:0.1,
		value: $loan_rate,
		slide: function(event, ui) {
			$("#intrest").text(ui.value);
			$("#interest-rate-input").val(ui.value)
			loancalculate();
		}
	});
		$("#intrest").text($("#finlon-intrest-slide").slider("value"));
		$("#interest-rate-input").val($("#finlon-intrest-slide").slider("value"));
		loancalculate();
		chart_render(); 
}

function chart_render()
{
	var options = chart_options();
	Highcharts.chart('loan-chart',options);
		if($(window).width() <= 499){
			// Highcharts.chart('loan-chart',options).setSize(270,270);
		}
}

function chart_options(){
	var principal_chart = parseInt($('#principal-show').text());
	var interest_chart = $('#total-interest').text().toString();
	var new_interest_chart = parseFloat(interest_chart.replace(/,/g,''))

	var options = {
				chart: {
					plotBackgroundColor: null,
					plotBorderWidth: 0,
					plotShadow: false,
				},
				title: {
					text: '',
					align: 'center',
					verticalAlign: 'middle',
					y: 40
				},
				colors:['#e63a27','#1c1c27'],
				plotOptions: {
					pie: {
						dataLabels: {
							enabled: true,
							distance: -50,
							style: {
								fontWeight: 'bold',
								color: 'white'
							}
						},
						startAngle: -90,
						endAngle: 90,
						center: ['50%', '120%'],
						size: '230%'
					}
				},
				series: [{
					type: 'pie',
					name: '',
					innerSize: '50%',
					data: [
						['Principal value', principal_chart],
						['Interest Value', new_interest_chart],
						{
							dataLabels: {
							   enabled: true
						   }
						}
					]
				}]
			};
	return options;
}

function value() {
		$text_val=$('#principal-input').val().toString()
		$input_value = $text_val.replace(/,/g,'')
		
		// var input_value = $('#principal-input').val()
		console.log($.isNumeric($input_value))
		console.log($input_value)
				
		if ($.isNumeric($input_value) == false) {
			$('#principal-input').val($('#principal-show').text())
		}
}

function onlynumeric(event)
{
	var x = event.which || event.keycode;
		if((x>=48 && x<=57 && x==190))
			return true;
		else
			return false;
}