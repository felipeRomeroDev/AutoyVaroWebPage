if ($("#loan-eligibility").length) {
	jQuery (function( $ ) {
		
		// eligibility-principal-slider

		principal_value()

		$("#eligibility-principal-input").on("keyup",function(event){
			$principal_slider_value = $("#eligibility-principal-input").val();

			if (event.keyCode === 13) {
				principal_value($principal_slider_value);
			}
		})
		$("#eligibility-principal-input").blur(function(){
			$principal_slider_value = $("#eligibility-principal-input").val();
			principal_value($principal_slider_value)
		})

		//eligibility-other-slider

		other_emi_value()

		$("#eligibility-other-input").on("keyup",function(event){
		$other_slider_value = $("#eligibility-other-input").val();

			if (event.keyCode === 13) {
				other_emi_value($other_slider_value);
			}
		})
		$("#eligibility-other-input").blur(function(){
			$other_slider_value = $("#eligibility-other-input").val();
			other_emi_value($other_slider_value)
		})

		// gross-income-emi

		income_slide()

		$("#eligibility-income-input").on("keyup",function(event){
		$income_slider_value = $("#eligibility-income-input").val();

			if (event.keyCode === 13) {
				income_slide($income_slider_value);
			}
		})
		$("#eligibility-income-input").blur(function(){
			$other_slider_value = $("#eligibility-income-input").val();
			income_slide($other_slider_value)
		})

		// eligibility-year-slider

		// loan_year_value()

		

		$("#eligibility-year-slide").slider({
		range:"min",
		min:2,
		max:35,
		value:10,
		step:1,
		slide: function(event, ui) {
			$("#eligibility-totalyear-show").text(ui.value);
			$("#eligibility-year-input").val(ui.value);
			loancalculate()
		}
	})
	$("#eligibility-totalyear-show").text($("#eligibility-year-slide").slider("value"));
	$("#eligibility-year-input").val($("#eligibility-year-slide").slider("value"));
	

		$("#eligibility-year-input").on("keyup",function(event){
			if (event.keyCode === 13) {
				loan_year_value();
			}
		})

		$("#eligibility-year-input").blur(function(){
				loan_year_value();
		})


		// eligibility-rate-input

	$("#eligibility-interest-slide").slider({
		range:"min",
		min:6,
		max:12,
		value:8,
		step:0.1,
		slide: function(event, ui){
			$("#eligibility-interest-show").text(ui.value);
			$("#eligibility-rate-input").val(ui.value);
			loancalculate();
		}
	})
	$("#eligibility-interest-show").text($("#eligibility-interest-slide").slider("value"));
	$("#eligibility-rate-input").val($("#eligibility-interest-slide").slider("value"));
		

		$("#eligibility-rate-input").on("keyup",function(event){
			if (event.keyCode === 13) {
				interest_value();
			}
		})

		$("#eligibility-rate-input").blur(function(){
			interest_value();
		})

	loancalculate();
	});

}

function loancalculate()
	{
			var P1 =parseInt(jQuery("#eligibility-principal-show").text());
			var n1 =jQuery("#eligibility-totalyear-show").text();
			var rate1=jQuery("#eligibility-interest-show").text();
			var existing = jQuery("#eligibility-other-show").text();

			var income1 =jQuery("#eligibility-income-show").text();


			var r1 = rate1 / (12 * 100); // to calculate rate percentage..
			var prate1 = (P1 * r1 * Math.pow((1 + r1), n1 * 12)) / (Math.pow((1 + r1), n1 * 12) - 1); // to calculate compound interest..
			var emi1 = Math.ceil(prate1 * 100) / 100; // to parse emi amount..
			var existingLoan = (existing - (existing * 60 / 100));

			if (income1 <= 14999) {
				var incomere = ((income1) * 40 / 100) - existingLoan;
			} else if (income1 <= 29999) {
				var incomere = ((income1) * 45 / 100) - existingLoan;
			} else if (income1 >= 30000) {
				var incomere = ((income1) * 50 / 100) - existingLoan;
			}
			var incomereq = Math.floor(incomere / emi1 * P1);
			var prate2 = (incomereq * r1 * Math.pow((1 + r1), n1 * 12)) / (Math.pow((1 + r1), n1 * 12) - 1); // to calculate compound interest2..
			var emi2 = Math.ceil((prate2) * 100) / 100; // to parse emi2 amount..   //Check again Reminder
			// console.log(P1);
			// to assign value in field1 as fixed upto two decimal..
			if (incomereq > P1) {
			jQuery("#loan-eligibility-monthly-emi").html(emi1.toFixed(0));                
			jQuery("#loan-eligibility-total").html(incomereq);
			jQuery("#loan-eligibility-emi").html('/ '+emi2.toFixed(0)+'&nbsp;EMI');

			} else { 
			jQuery("#loan-eligibility-monthly-emi").html('0');
			jQuery("#loan-eligibility-total").html('0');
			jQuery("#loan-eligibility-emi").html('')
			}
	}

function principal_value($principal_value2=30000){
	$text_val=$principal_value2.toString()
	$new_text_val = parseInt($text_val.replace(/,/g,''))
	// console.log('-------'+ $new_text_val)
	$("#eligibility-principal-slide").slider({
		range:"min",
		min:10000,
		max:20000000,
		value:$new_text_val,
		step:1,
		slide: function(event, ui) {
			$("#eligibility-principal-show").text(ui.value);
			$("#eligibility-principal-input").val(ui.value);
			jQuery("#loan-eligibility-principal").html(ui.value);
			loancalculate();
		}
	})
	// console.log( $new_text_val)
	$("#eligibility-principal-show").text($("#eligibility-principal-slide").slider("value"));
	$("#loan-eligibility-principal").text($("#eligibility-principal-slide").slider("value"));
	$("#eligibility-principal-input").val($("#eligibility-principal-slide").slider("value"));
	loancalculate();
}

function other_emi_value($other_value=0) {
	$other_val1=$other_value.toString()
	$new_text_other= parseInt($other_val1.replace(/,/g,''))
	// $new_text_other = 250000
	// console.log('-------'+ $new_text_other)
	$("#eligibility-other-slide").slider({
		range:"min",
		min:0,
		max:20000000,
		value:$new_text_other,
		step:1,
		slide: function(event, ui){
			$("#eligibility-other-show").text(ui.value);
			$("#eligibility-other-input").val(ui.value);
			loancalculate();
		}
	})
	 // console.log($new_text_val)
	 $("#eligibility-other-show").text($("#eligibility-other-slide").slider("value"));
	 $("#eligibility-other-input").val($("#eligibility-other-slide").slider("value"));
	 loancalculate();
}

function income_slide($income_value=20000) {
	$income_val=$income_value.toString()
	$new_text_income= parseInt($income_val.replace(/,/g,''))
	// $new_text_other = 250000
	// console.log('-------'+ $new_text_income)
	$("#eligibility-Grossincome-slide").slider({
		range:"min",
		min:1000,
		max:1000000,
		value:$new_text_income,
		step:1,
		slide: function(event, ui){
			$("#eligibility-income-show").text(ui.value);
			$("#eligibility-income-input").val(ui.value);
			loancalculate();
		}
	})
	 // console.log($new_text_val)
	 $("#eligibility-income-show").text($("#eligibility-Grossincome-slide").slider("value"));
	 $("#eligibility-income-input").val($("#eligibility-Grossincome-slide").slider("value"));
	 loancalculate();
}

function loan_year_value() {
	$loan_year = $("#eligibility-year-input").val()
	$("#eligibility-year-slide").slider({
		range:"min",
		min:2,
		max:35,
		value:$loan_year,
		step:1,
		slide: function(event, ui) {
			$("#eligibility-totalyear-show").text(ui.value);
			$("#eligibility-year-input").val(ui.value);
		}
	})
	
	$("#eligibility-totalyear-show").text($("#eligibility-year-slide").slider("value"));
	$("#eligibility-year-input").val($("#eligibility-year-slide").slider("value"));
	loancalculate();
}
function interest_value() {
	$loan_rate = $("#eligibility-rate-input").val()
	$("#eligibility-interest-slide").slider({
		range:"min",
		min:6,
		max:12,
		value:$loan_rate,
		step:0.1,
		slide: function(event, ui){
			$("#eligibility-interest-show").text(ui.value);
			$("#eligibility-rate-input").val(ui.value);
		}
	})
	$("#eligibility-interest-show").text($("#eligibility-interest-slide").slider("value"));
	$("#eligibility-rate-input").val($("#eligibility-interest-slide").slider("value"));
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