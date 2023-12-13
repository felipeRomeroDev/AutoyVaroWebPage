if ($("#home-loan-calculator").length) {
	jQuery (function( $ ) {
		if ($(".scroll-to-chart").length) {
			$(".scroll-to-chart").on("click",function (){
				$target  = $(this).data('target')
				// console.log($target)
				$('html,body').animate(
				{
					scrollTop: $("#amortization-chart").offset().top-100
				},1000);
				return false
			})
		}       
		var months_arr = ['January', 'Fabuary', 'March','April','May','June','July','August','September','October','November','December'];
		var date = new Date();
		let year_no = date.getFullYear();
		let month_no = date.getMonth();     
		let month = months_arr[month_no];
		$('#start-date').val(month +' '+year_no)


		// emi-principal-slide

		principal_value()

		$("#emi-principal-input").on("keyup",function(event){
			$principal_slider_value = $("#emi-principal-input").val();

			if (event.keyCode === 13) {
				principal_value($principal_slider_value);
			}
		})
		$("#emi-principal-input").blur(function(){
			$principal_slider_value = $("#emi-principal-input").val();
			principal_value($principal_slider_value)
		})

		// emi-year-slider

		// loan_year_value()

		

		$("#emi-year-slide").slider({
		range:"min",
		min:2,
		max:35,
		value:10,
		step:1,
		slide: function(event, ui) {
			$("#emi-totalyear-show").text(ui.value);
			$("#emi-year-input").val(ui.value);
		},
		change: function(event, ui) {
			loancalculate()
		}
	})
	$("#emi-totalyear-show").text($("#emi-year-slide").slider("value"));
	$("#emi-year-input").val($("#emi-year-slide").slider("value"));
	

		$("#emi-year-input").on("keyup",function(event){
			if (event.keyCode === 13) {
				loan_year_value();
			}
		})

		$("#emi-year-input").blur(function(){
				loan_year_value();
		})


		// interest-rate-input

	$("#emi-interest-slide").slider({
		range:"min",
		min:6,
		max:12,
		value:8,
		step:0.1,
		slide: function(event, ui){
			$("#emi-interest-show").text(ui.value);
			$("#interest-rate-input").val(ui.value);
		},
		change: function(event, ui) {
			loancalculate()
		}
	})
	$("#emi-interest-show").text($("#emi-interest-slide").slider("value"));
	$("#interest-rate-input").val($("#emi-interest-slide").slider("value"));
		

		$("#interest-rate-input").on("keyup",function(event){
			if (event.keyCode === 13) {
				interest_value();
			}
		})

		$("#interest-rate-input").blur(function(){
			interest_value();
		})

	loancalculate();
	});
}

function loancalculate()
{
	var loanAmount=jQuery("#emi-principal-show").text();
	var numberOfMonths=jQuery("#emi-totalyear-show").text() * 12;
	var rateOfInterest=jQuery("#emi-interest-show").text();
	
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
	$input_principal = $("#emi-principal-input").val().toString().replace(/,/g,"").replace(/\B(?=(\d{3})+(?!\d))/g, ",");   
	jQuery("#monthly-emi").html(emi_str);
	jQuery("#total-interest").html(int_str);
	jQuery("#total-amount").html(full_str);
	jQuery("#emi-principal-input").val($input_principal);

	amortization_calculator(loanAmount,emi,rateOfInterest)

	$('.datepicker').datepicker({
		autoclose: true,
		format: 'MM yyyy',
		minViewMode: 1,
	})  

	$('#start-date').change(function(){
		amortization_calculator(loanAmount,emi,rateOfInterest)
	})
}

function principal_value($principal_value2=300000   ){
	$text_val=$principal_value2.toString()
	$new_text_val = parseInt($text_val.replace(/,/g,''))
	$("#emi-principal-slide").slider({
		range:"min",
		min:100000,
		max:20000000,
		value:$new_text_val,
		step:1000,
		slide: function(event, ui) {
			$("#emi-principal-show").text(ui.value);
			$("#emi-principal-input").val(ui.value);
		},
		change: function(event, ui) {
			loancalculate()
		}
	})
	// console.log( $new_text_val)
	$("#emi-principal-show").text($("#emi-principal-slide").slider("value"));
	$("#emi-principal-input").val($("#emi-principal-slide").slider("value"));
	loancalculate()
}
function loan_year_value() {
	$loan_year = $("#emi-year-input").val()
	$("#emi-year-slide").slider({
		range:"min",
		min:2,
		max:35,
		value:$loan_year,
		step:1,
		slide: function(event, ui) {
			$("#emi-totalyear-show").text(ui.value);
			$("#emi-year-input").val(ui.value);
		},
		change: function(event, ui) {
			loancalculate()
		}
	})
	
	$("#emi-totalyear-show").text($("#emi-year-slide").slider("value"));
	$("#emi-year-input").val($("#emi-year-slide").slider("value"));
	loancalculate()
}
function interest_value() {
	$loan_rate = $("#interest-rate-input").val()
	$("#emi-interest-slide").slider({
		range:"min",
		min:6,
		max:12,
		value:$loan_rate,
		step:0.1,
		slide: function(event, ui){
			$("#emi-interest-show").text(ui.value);
			$("#interest-rate-input").val(ui.value);
		},
		change: function(event, ui) {
			loancalculate()
		}
	})
	$("#emi-interest-show").text($("#emi-interest-slide").slider("value"));
	$("#interest-rate-input").val($("#emi-interest-slide").slider("value"));
	loancalculate()
}

function amortization_calculator(loanAmount,emi,rate) {
	var start_date = $('#start-date').val();
	var months_arr = ['January', 'Fabuary', 'March','April','May','June','July','August','September','October','November','December'];
	var date = new Date(start_date);
	let month_no = date.getMonth();
	let year_no = date.getFullYear();
	i = rate/100;
	let content = '';
	
	let current_balance = loanAmount;
	let total_interest = 0;
	
	let payment_counter_arr = [];
	let principal_paid_arr=[];
	let toward_interest_arr=[];
	

	content += '<div class="amortization-column-outer">';
	content += '<div class="amortization-column sum">';
	content += '<div class="amortization-data year-outer"><i class="fas fa-plus-circle"></i><span>'+year_no+'</span></div>';
	content += '<div class="amortization-data principal-sum"></div>';
	content += '<div class="amortization-data interest-sum"></div>';
	content += '<div class="amortization-data total-payment-sum"></div>';
	content += '<div class="amortization-data balance-sum"></div>';
	content += '</div>';
	content += '<div class="amortization-column-inner">';
	
	while(current_balance > 0){
	let month = months_arr[month_no];
	let payment_counter = month +' '+ year_no;
		let toward_interest = (i/12) * current_balance; // this calculates the portion of monthly payment that goes toward the interest
		
		if (emi > current_balance){
			emi = current_balance +toward_interest;
		}
		
		let toward_balance = emi - toward_interest;
		total_interest += toward_interest;
		current_balance -= toward_balance;

		principal_paid = emi - toward_interest

		var principal_paid_str = principal_paid.toFixed(0).toString().replace(/,/g,"").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
		var toward_interest_str = toward_interest.toFixed(0).toString().replace(/,/g,"").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
		var emi_str = emi.toFixed(0).toString().replace(/,/g,"").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
		var current_balance_str = current_balance.toFixed(0).toString().replace(/,/g,"").replace(/\B(?=(\d{3})+(?!\d))/g, ",");

		content += '<div class="amortization-column">';
		content += '<div class="amortization-data year">'+payment_counter+'</div>';
		content += '<div class="amortization-data principal">$<span>'+principal_paid_str+'</span></div>';
		content += '<div class="amortization-data interest">$<span>'+toward_interest_str+'</span></div>';
		content += '<div class="amortization-data total-payment">$<span>'+emi_str+'</span></div>';
		content += '<div class="amortization-data balance">$<span>'+current_balance_str+'</span></div>';
		content += '</div>';
		
		month_no++;
		if (month_no>=12) {
			month_no=0
			year_no++
			content += '</div>';
			content += '</div>';
			content += '<div class=amortization-column-outer>';
			content += '<div class="amortization-column sum">';
			content += '<div class="amortization-data year-outer"><i class="fas fa-plus-circle"></i><span>'+year_no+'</span></div>';
			content += '<div class="amortization-data principal-sum"></div>';
			content += '<div class="amortization-data interest-sum"></div>';
			content += '<div class="amortization-data total-payment-sum"></div>';
			content += '<div class="amortization-data balance-sum"></div>';
			content += '</div>';
			content += '<div class="amortization-column-inner">';
		}
	}
	
	$('.amortization-content').html(content)

	$('.amortization-column-outer').each(function(){
		if($(this).find('.amortization-column-inner').html() == '')
		{
			$(this).remove()
		}
	})

	$('.amortization-column-outer').each(function(){

		var total_principal=0;
		var total_interest_yearly=0;
		var total_payment_yearly=0
		var total_balance_yearly=0
		$(this).find('.amortization-column-inner .amortization-column').each(function(){
			//principal
			var principal_int = $(this).find('.principal span').html()
			var new_principal_int = parseInt(principal_int.replace(/,/g,''))
			total_principal += new_principal_int

			//interest
			var interest_int=$(this).find(".interest span").html()
			var new_interest_int=parseInt(interest_int.replace(/,/g,''))
			total_interest_yearly += new_interest_int

			// total payment
			var total_payment=$(this).find(".total-payment span").html()
			var new_total_payment_int=parseInt(total_payment.replace(/,/g,''))
			total_payment_yearly += new_total_payment_int

			// balance
			var balance_int=$(this).find(".balance span").html()
			var new_balance_int=parseInt(balance_int.replace(/,/g,''))
			total_balance_yearly += new_balance_int
		})
		var total_principal_str = total_principal.toFixed(0).toString().replace(/,/g,"").replace(/\B(?=(\d{3})+(?!\d))/g, ",")
		var total_interest_str = total_interest_yearly.toFixed(0).toString().replace(/,/g,"").replace(/\B(?=(\d{3})+(?!\d))/g, ",")
		var total_payment_str = total_payment_yearly.toFixed(0).toString().replace(/,/g,"").replace(/\B(?=(\d{3})+(?!\d))/g, ",")
		var total_balance_str = total_balance_yearly.toFixed(0).toString().replace(/,/g,"").replace(/\B(?=(\d{3})+(?!\d))/g, ",")

		$(this).find('.principal-sum').html('$<span>'+total_principal_str+'</span>')
		$(this).find('.interest-sum').html('$<span>'+total_interest_str+'</span>')
		$(this).find(".total-payment-sum").html('$<span>'+total_payment_str+'</span>')
		$(this).find(".balance-sum").html('$<span>'+total_balance_str+'</span>')
		year_no = $(this).find('.year-outer span').html()
		payment_counter_arr.push(year_no);
		principal_paid_arr.push(Math.floor(total_principal));
		toward_interest_arr.push(Math.floor(total_interest_yearly));
		chart_render(payment_counter_arr,principal_paid_arr,toward_interest_arr);

	})



	$('.amortization-column-inner').slideUp(0)
	$(".amortization-data.year-outer").on("click", function(){
		$(this).parent().siblings('.amortization-column-inner').slideToggle(500)
		$(this).find("i").toggleClass("fa-plus-circle")
		$(this).find("i").toggleClass("fa-minus-circle")

	})
}

function round(number, decimal_places){
	return(Math.round(number*Math.pow(10,decimal_places)) / Math.pow(10,decimal_places)).toFixed(decimal_places);
}

function chart_render(payment_counter_arr,principal_paid_arr,toward_interest_arr)
{
	var options = {

		chart: {
				type: 'column'
		},

		title: {
				text: ''
		},

		xAxis: {
				categories: payment_counter_arr
		},

		yAxis: {
				allowDecimals: false,
				min: 0,
				title: {
						text: 'Balance' 
				},
				labels: {
					formatter: function () {
						return '$' + this.axis.defaultLabelFormatter.call(this);
					}
				}
		},

		tooltip: {
				formatter: function () {
					y_str = this.y.toFixed(0).toString().replace(/,/g,"").replace(/\B(?=(\d{3})+(?!\d))/g, ",")
					stacktotal_str = this.point.stackTotal.toFixed(0).toString().replace(/,/g,"").replace(/\B(?=(\d{3})+(?!\d))/g, ",")
						return '<b>Year: ' + this.x + '</b><br/>' +
								this.series.name + ': $' + y_str + '<br/>' +
								'Total Payment: $' + stacktotal_str;
				}
		},

		plotOptions: {
				column: {
						stacking: 'normal'
				}
		},

		series: [{
				name: 'Interest',
				data: toward_interest_arr,
				color : '#f7c35f'
		}, {
				name: 'Principal',
				data: principal_paid_arr,
				color : '#e63a27'
		}]
};
	Highcharts.chart('loan-chart-amortization',options);
	Highcharts.setOptions({
		lang: {
			numericSymbols: null,
			thousandsSep: ','
		}
	});
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