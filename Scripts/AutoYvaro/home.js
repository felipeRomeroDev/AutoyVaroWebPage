//Testimonial Slider
$(".blog-slider-three").owlCarousel({
    nav: true,
    dots: false,
    loop: true,
    navText: ['<i class="flaticon-left-arrow-1"></i>', '<i class="flaticon-next"></i>'],
    margin: 25,
    responsiveClass: true,
    responsive: {
        0: {
            items: 1,
            nav: true
        },

        650: {
            items: 2,
            nav: true
        },

        1050: {
            items: 3,
            nav: true
        },

        1250: {
            items: 4,
            nav: true,
            loop: true
        }
    },
    thumbs: false,
    smartSpeed: 1300,
    autoplay: true,
    autoplayTimeout: 5000,
    autoplayHoverPause: false,
    responsiveClass: true,
    autoHeight: true,
});

//Testimonial Slider 
$(".quien-slider-three").owlCarousel({
    nav: true,
    dots: false,
    loop: true,    
    autoplay: true,
    autoplayTimeout: 5000,
    navText: ['<i class="fa fa-angle-left"></i>', '<i class="fa fa-angle-right"></i>'],    
    margin: 25,
    items: 1,
    thumbs: false,
    smartSpeed: 1300,
    autoplayHoverPause: false,
    responsiveClass: true,
    autoHeight: true,
});

$(".testimonial-slider-threeww").owlCarousel({
    nav: true,
    dots: false,
    loop: true,
    autoplay: true,
    autoplayTimeout: 10000,
    navText: ['<i class="flaticon-left-arrow-1"></i>', '<i class="flaticon-next"></i>'],
    margin: 25,
    items: 1,
    thumbs: false,
    smartSpeed: 1300,
    autoplay: false,
    autoplayTimeout: 4000,
    autoplayHoverPause: false,
    responsiveClass: true,
    autoHeight: true,
});