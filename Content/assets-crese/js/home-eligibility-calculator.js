if($("#home-eligibility-calculator").length) {
jQuery (function ( $ ) {

	// age-eligibility 

	age_eligibility()

	$("#eligibility-age-input").on("keyup", function(event){
		$age_input_value = $("#eligibility-age-input").val();
		if (event.keyCode === 13) {
			age_eligibility($age_input_value)
		}
	})

	$("#eligibility-age-input").blur(function(){
			$age_input_value = $("#eligibility-age-input").val();
			age_eligibility($age_input_value)
		})

	//Loan year eligibility

	year_eligibility()

	$("#eligibility-year-input").on("keyup", function(event){
		$year_input = $("#eligibility-year-input").val();
			if (event.keyCode === 13) {
				year_eligibility($year_input);
			}
		})
	
	$("#eligibility-year-input").blur(function(){
		$year_input = $("#eligibility-year-input").val();
		year_eligibility($year_input);
	})

	//monthly income 

	income_eligibility()

	$("#eligibility-income-input").on("keyup", function(event) {
		$income_input = $("#eligibility-income-input").val();
		if (event.keyCode === 13) {
			income_eligibility($income_input)
		} 
	})

	$("#eligibility-income-input").blur(function(){
		$income_input = $("#eligibility-income-input").val();
			income_eligibility($income_input)		
	})

	//other emi(monthly)

	other_emi_eligibility()

	$("#eligibility-other-input").on("keyup", function(event){
		$other_input = $("#eligibility-other-input").val();
		if (event.keyCode === 13) {
			other_emi_eligibility($other_input)
		}
	})

	$("#eligibility-other-input").blur(function(){
		$other_input = $("#eligibility-other-input").val();
		other_emi_eligibility($other_input)
	})

	// interest rate

	interest_rate_eligibility()

	$("#eligibility-rate-input").on("keyup", function(event) {
		$interest_input = $("#eligibility-rate-input").val();
		if(event.keyCode === 13) {
			interest_rate_eligibility($interest_input)
		}
	})

	$("#eligibility-rate-input").blur(function(){
		$interest_input = $("#eligibility-rate-input").val();
		interest_rate_eligibility($interest_input)
	})
})

function age_eligibility($value2=25){
	$("#eligibility-age-slider").slider({
		range:'min',
		min:18,
		max:55,
		value:$value2,
		step:1,
		slide:function(event, ui){
			$("#eligibility-age-show").text(ui.value);
			$("#eligibility-age-input").val(ui.value);
			// console.log();
			year_eligibility()
			loan_eligibility()
		}
	})
	$("#eligibility-age-show").text($("#eligibility-age-slider").slider("value"));
	$("#eligibility-age-input").val($("#eligibility-age-slider").slider("value"));
	year_eligibility()
	loan_eligibility()
}

function year_eligibility($value2=15){

	if ($("#eligibility-age-slider").slider("value") > 30) {
		$year_diff = $("#eligibility-age-input").val()-30
		$year_max = 30-$year_diff
	}
	else
	{
		$year_max = 30
	}

	$("#eligibility-year-slider").slider({
		range:'min',
		min:1,
		max:$year_max,
		value:$value2,
		step:1,
		slide:function(event, ui){
			$("#eligibility-year-show").text(ui.value);
			$("#eligibility-year-input").val(ui.value);
			// console.log();
			loan_eligibility()
		}
	})
	$("#eligibility-year-show").text($("#eligibility-year-slider").slider("value"));
	$("#eligibility-year-input").val($("#eligibility-year-slider").slider("value"));
	loan_eligibility()
}

function income_eligibility($value2=25000){
	$value2 = $value2.toString().replace(/,/g,"")
	$("#eligibility-income-slider").slider({
		range:'min',
		min:8000,
		max:900000,
		value:$value2 ,
		step:1,
		slide:function(event, ui){
			$("#eligibility-income-show").text(ui.value);
			$("#eligibility-income-input").val(ui.value);
			// console.log();
			loan_eligibility()
		}
	})
	$("#eligibility-income-show").text($("#eligibility-income-slider").slider("value"));
	$("#eligibility-income-input").val($("#eligibility-income-slider").slider("value"));
	loan_eligibility()
}

function other_emi_eligibility($value2=0){
	$value2 = $value2.toString().replace(/,/g,'')
	$("#eligibility-other-slider").slider({
		range:'min',
		min:0,
		max:900000,
		value:$value2,
		step:1,
		slide:function(event, ui){
			$("#eligibility-other-show").text(ui.value);
			$("#eligibility-other-input").val(ui.value);
			// console.log();
			loan_eligibility()
		}
	})
	$("#eligibility-other-show").text($("#eligibility-other-slider").slider("value"));
	$("#eligibility-other-input").val($("#eligibility-other-slider").slider("value"));
	loan_eligibility()
}

function interest_rate_eligibility($value2=6){
	$("#eligibility-interest-slide").slider({
		range:'min',
		min:4,
		max:11,
		value:$value2,
		step:0.01,
		slide:function(event, ui){
			$("#eligibility-interest-show").text(ui.value);
			$("#eligibility-rate-input").val(ui.value);
			// console.log();
			loan_eligibility()
		}
	})
	$("#eligibility-interest-show").text($("#eligibility-interest-slide").slider("value"));
	$("#eligibility-rate-input").val($("#eligibility-interest-slide").slider("value"));
	loan_eligibility()
}

}

function loan_eligibility() {
	$income_input = $('#eligibility-income-input').val().toString().replace(/,/g,"").replace(/\B(?=(\d{3})+(?!\d))/g, ",")
	$('#eligibility-income-input').val($income_input)
	$other_emi = $('#eligibility-other-input').val().toString().replace(/,/g,"").replace(/\B(?=(\d{3})+(?!\d))/g, ",")
	$('#eligibility-other-input').val($other_emi)

	var loan_year_np = parseInt($("#eligibility-year-input").val())*12;
	var income_pv = parseInt($("#eligibility-income-input").val().replace(/,/g,""));
	var Other_emi = parseInt($("#eligibility-other-input").val().replace(/,/g,""));
	var interest_rate_ir = parseFloat($("#eligibility-rate-input").val()/1200);
	var Elg = 0;

	//Calculating PMT(Payment Per Period)
	var pmt = PMT(loan_year_np, 100000, interest_rate_ir);

	//Calculating Emi
	if(income_pv <= 20000) {
		Emi = ((income_pv * 0.55) - Other_emi)
	}
	else if(income_pv <= 50000) {
		Emi = ((income_pv * 0.60) - Other_emi)
	}
	else if(income_pv <= 100000) {
		Emi = ((income_pv * 0.65) - Other_emi)
	}
	else {
		Emi = ((income_pv * 0.7) - Other_emi)
	}
	var Elg = Emi / pmt
	// console.log(int_str)

	jQuery("#total-amount").html((Elg*100000).toFixed(0).replace(/\B(?=(\d{3})+(?!\d))/g, ","));
	jQuery("#total-emi").html((Emi).toFixed(0).replace(/\B(?=(\d{3})+(?!\d))/g, ","));

}

function PMT(loan_year_np, income_pv, interest_rate_ir,FV) {
	if (!FV){
		FV = 0
	}
	pvif = Math.pow(1 + interest_rate_ir, loan_year_np);
	pmt =  interest_rate_ir * (income_pv * pvif + FV) / (pvif - 1);
	return pmt;
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