$(function($){
	if ($('#compare-emi-1').length) {
		$("#compare-emi-1 table tbody tr").each(function(){
		var self = $(this)
		var low_rate = self.find('.interest-rate').data('low-interest')
		var high_rate = self.find('.interest-rate').data('high-interest')
		if (low_rate && high_rate) {
			self.find('.interest-rate').text(low_rate+'% - '+high_rate+'%')
		}
		else if (!low_rate && high_rate) {
			self.find('.interest-rate').text('Up to '+high_rate+'%')
		}
		else if (low_rate && !high_rate) {
			self.find('.interest-rate').text(low_rate+'% Onwards')
		}
		$("#principal-amount, #loan-year").blur(function(){
			var calculated_emi = emi_calculation(low_rate,high_rate)
			
			 principal_value();
			 loan_year_value();
			 // emi_calculation();
		});

		$("#principal-amount, #loan-year").on("keyup", function(event){
			if (event.keyCode == 13) {
				var calculated_emi = emi_calculation(low_rate,high_rate)
				
				 principal_value();
				 loan_year_value();
				 // emi_calculation();
			}
		})
	})
		emi_calculation()
		slider_change();
	}
})

function emi_calculation(){
	var low_emi_arr = [];
	$("#compare-emi-1 table tbody tr").each(function(){
		var self = $(this)
		var low_rate = self.find('.interest-rate').data('low-interest')
		var high_rate = self.find('.interest-rate').data('high-interest')
		// console.log(high_rate)
		var principal_rate = $("#principal-amount").val();
		var loan_year = $("#loan-year").val();

		if (low_rate && high_rate) {
			var low_emi = loancalculate(principal_rate,low_rate,loan_year);
			var high_emi = loancalculate(principal_rate,high_rate,loan_year);
			self.find('.calculated-emi').html('$<span class="low_emi">'+low_emi+'</span> - $<span class="high_emi">'+high_emi+'</span>')

		}
		else if (!low_rate && high_rate) {
			var high_emi = loancalculate(principal_rate,high_rate,loan_year);
			self.find('.calculated-emi').html('Up to $<span class="high_emi">'+high_emi+'</span>')
		}
		else if (low_rate && !high_rate) {
			var low_emi = loancalculate(principal_rate,low_rate,loan_year);
			self.find('.calculated-emi').html('$<span class="low_emi">'+low_emi+'</span> Onwards')
		}
		if(low_rate){
			$emi_val_int = parseFloat(self.find('.calculated-emi .low_emi').text().replace(/,/g,''))
			low_emi_arr.push($emi_val_int)
		}
		if(!low_rate && high_rate){
			$emi_val_int = parseFloat(self.find('.calculated-emi .high_emi').text().replace(/,/g,''))
			low_emi_arr.push($emi_val_int)
		}
	})
	var m = Math.min(...low_emi_arr);

	$("#compare-emi-1 table tbody tr").each(function(){
		var self = $(this)
		if (self.find('.calculated-emi .low_emi').text().replace(/,/g,'') == m) {
			self.addClass('lowest_emi')
		}
		else if (self.find('.calculated-emi .high_emi').text().replace(/,/g,'') == m) {
			self.addClass('lowest_emi')
		}
	})
}

function loancalculate(principal_rate,interest_rates,loan_year){
	var loanAmount=principal_rate;
	// console.log(principal_rate)
	var numberOfMonths=loan_year * 12;
	var rateOfInterest=interest_rates;
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



	return emi_str
	// jQuery(".calculated-emi").html(emi_str);
}

function slider_change(){
	$("#finlon-principal-slide").slider({
		range: "min",
		min: 30000,
		max: 500000,
		value: 50000,
		step: 1000,
		slide: function (event, ui) {
			$("#principal-show").text(ui.value);
			$('#principal-amount').val(ui.value);
			
		},
		change:function(){
			emi_calculation();
		}
	});
	$("#principal-show").text($("#finlon-principal-slide").slider("value"));
	$('#principal-amount').val($('#finlon-principal-slide').slider('value'));
	// years
	$("#finlon-year-slide1").slider({
		range: "min",
		min: 2,
		max: 60,
		step: 1,
		value: 3,
		slide: function (event, ui) {
			$("#totalyear-show").text(ui.value);
			$("#loan-year").val(ui.value)
			
		},
		change :function() {
			emi_calculation();
		}
	});
	$("#totalyear-show").text($("#finlon-year-slide1").slider("value"));
	$('#loan-year').val($('#finlon-year-slide1').slider('value'));
}

function principal_value(){
	$text_val=$("#principal-amount").val().toString()
	$new_text_val = parseInt($text_val.replace(/,/g,''))
	var max=$("#finlon-principal-slide").slider("option", "max")
	if($new_text_val > max) {
		$("#principal-amount").val(max)
	}
	var min=$("#finlon-principal-slide").slider("option", "min")
	if ($new_text_val < min) {
		$("#principal-amount").val(min)
	}
	$("#finlon-principal-slide").slider({
		range: "min",
				min: 30000,
				max: 500000,
				value: $new_text_val,
				step: 1000,
				slide: function (event, ui) {
					$("#principal-show").text(ui.value);
					$('#principal-amount').val(ui.value);
					loancalculate();
				}
	});
	$("#principal-show").text($("#finlon-principal-slide").slider("value"));
		loancalculate();
}
function loan_year_value(){
	$loan_year1 = $("#loan-year").val()
	var max = $("#finlon-year-slide1").slider("option", "max");
		if ($loan_year1 > max) {
			$("#loan-year").val(max)
		}
	var min = $("#finlon-year-slide1").slider("option", "min");
		if ($loan_year1 < min) {
			$("#loan-year").val(min)
		}
		$("#finlon-year-slide1").slider({
		range: "min",
		min: 2,
		max: 60,
		step: 1,
		value: $loan_year1,
		slide: function (event, ui) {
			$("#totalyear-show").text(ui.value);
			$("#loan-year").val(ui.value)
			loancalculate();
		}
	});
		$("#totalyear-show").text($("#finlon-year-slide1").slider("value"));
		loancalculate();
}
function onlynumeric(event)
{
	var x = event.which || event.keycode;
	if((x>=48 && x<=57 || x==46)){
		return true;
	}
	else
		return false;
}