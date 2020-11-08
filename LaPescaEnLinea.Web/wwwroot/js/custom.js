
$(function () {
    "use strict";

    // for navbar background color when scrolling
     
    $(window).scroll(function () {
        var $scrolling = $(this).scrollTop();
        var bc2top = $("#back-top-btn");
        var stickytop = $(".navbar");
        if ($scrolling >= 160) {
            stickytop.addClass('sticky');
        } else {
            stickytop.removeClass('sticky');
        }
        if ($scrolling > 150) {
            bc2top.fadeIn(2000);
        } else {
            bc2top.fadeOut(2000);
        }
    });
 
  
    //animation scroll js
        var nav = $('nav'),
        navOffset = nav.offset().top,
        $window = $(window);
    /* navOffset ends */
   
    //animation scroll js
    var html_body = $('html, body');
    $('nav a').on('click', function () {
        if (location.pathname.replace(/^\//, '') === this.pathname.replace(/^\//, '') && location.hostname === this.hostname) {
            var target = $(this.hash);
            target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
            if (target.length) {
                html_body.animate({
                    scrollTop: target.offset().top - 80
                }, 1000);
                return false;
            }
        }
    });


    // navbar js ends here

    //BACK TO TO BUTTON JAVA SCRIPT PART START
    var bc2top = $('#back-top-btn');
    bc2top.on('click', function () {
        html_body.animate({
            scrollTop: 0
        }, 100);
    });

    // Closes responsive menu when a scroll link is clicked
    $('.nav-link').on('click', function () {
        $('.navbar-collapse').collapse('hide');
    });
    
    /* -------------------------------------
	          Preloader				
	 	-------------------------------------- */
    $('#preloader').delay(2500).fadeOut(1000);
    
    
    /* -------------------------------------
	         datepicker js				
	 	-------------------------------------- */
   $('#datepicker').datepicker({
    format: 'mm/dd/yyyy',
    startDate: '-3d'
});
     $('#datepicker1').datepicker({
    format: 'mm/dd/yyyy',
    startDate: '-3d'
});
      $('#datepicker2').datepicker({
    format: 'mm/dd/yyyy',
    startDate: '-3d'
});
       $('#datepicker3').datepicker({
    format: 'mm/dd/yyyy',
    startDate: '-3d'
});
     $('#datepicker4').datepicker({
    format: 'mm/dd/yyyy',
    startDate: '-3d'
});
      $('#datepicker5').datepicker({
    format: 'mm/dd/yyyy',
    startDate: '-3d'
});
         $('#datepicker6').datepicker({
    format: 'mm/dd/yyyy',
    startDate: '-3d'
});
     $('#datepicker7').datepicker({
    format: 'mm/dd/yyyy',
    startDate: '-3d'
});
      $('#datepicker8').datepicker({
    format: 'mm/dd/yyyy',
    startDate: '-3d'
});
         $('#datepicker9').datepicker({
    format: 'mm/dd/yyyy',
    startDate: '-3d'
});
     $('#datepicker10').datepicker({
    format: 'mm/dd/yyyy',
    startDate: '-3d'
});
      $('#datepicker11').datepicker({
    format: 'mm/dd/yyyy',
    startDate: '-3d'
});
     /* -------------------------------------
	         travel js				
	 	-------------------------------------- */
    $(document).ready(function(e) {
try {
$(".get-app select").msDropDown();
} catch(e) {
alert(e.message);
}
});
    
    /* -------------------------------------
	         top-day-slick js				
	 	-------------------------------------- */
    $('.top-day-slick').slick({
        autoplay: true,
        arrows: true,
        dots: false,
        prevArrow: '<i class="fa fa-angle-right todaysNext"></i>',
        nextArrow: '<i class="fa fa-angle-left todaysprev"></i>',
        infinite: true,
        slidesToShow: 3,
        autoplaySpeed: 4000,
        slidesToScroll: 1,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 1,
                    infinite: true,

                }
    },
            {
                breakpoint: 950,
                settings: {
                    arrows: false,
                    slidesToShow: 2,
                    slidesToScroll: 1
                }
    },
            {
                breakpoint: 767.98,
                settings: {
                    arrows: false,
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
    }
  ]
    });

    /* -------------------------------------
	       recomended-slick js				
	 	-------------------------------------- */
    $('.recomended-slick').slick({
        autoplay: true,
        arrows: true,
        dots: false,
        prevArrow: '<i class="fa fa-angle-right recomandNext"></i>',
        nextArrow: '<i class="fa fa-angle-left recomandprev"></i>',
        infinite: true,
        slidesToShow: 3,
        autoplaySpeed: 4000,
        slidesToScroll: 1,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 1,
                    infinite: true,

                }
    },
            {
                breakpoint: 910,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 1
                }
    },
            {
                breakpoint: 767.98,
                settings: {
                    arrows: false,
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
    }
  ]
    });
    
    
    /* -------------------------------------
	       artical-slick js				
	 	-------------------------------------- */
    $('.artical-slick').slick({
        autoplay: true,
        arrows: true,
        dots: false,
        prevArrow: '<i class="fa fa-angle-right articalNext"></i>',
        nextArrow: '<i class="fa fa-angle-left articalprev"></i>',
        infinite: true,
        slidesToShow: 2,
        autoplaySpeed: 4000,
        slidesToScroll: 1,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 1,
                    infinite: true,

                }
    },
            {
                breakpoint: 920,
                settings: {
                     arrows: false,
                    slidesToShow: 2,
                    slidesToScroll: 1
                }
    },
            {
                breakpoint: 767.98,
                settings: {
                    arrows: false,
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
    }
  ]
    });
    
    

    /* -------------------------------------
	          Preloader				
	 	-------------------------------------- */
    $('.preloader').delay(2500).fadeOut(1000);

});


jQuery.extend(jQuery.validator.messages, {
    required: "Este campo es obligatorio.",
    remote: "Por favor, rellena este campo.",
    email: "Por favor, escribe una direcci\u00F3n de correo v\u00E1lida",
    url: "Por favor, escribe una URL v\u00E1lida.",
    date: "Por favor, escribe una fecha v\u00E1lida.",
    dateISO: "Por favor, escribe una fecha (ISO) v\u00E1lida.",
    number: "Por favor, escribe un número entero v\u00E1lido.",
    digits: "Por favor, escribe s\u00F3lo dí­gitos.",
    creditcard: "Por favor, escribe un número de tarjeta v\u00E1lido.",
    equalTo: "Por favor, escribe el mismo valor de nuevo.",
    accept: "Por favor, escribe un valor con una extensi\u00F3n aceptada.",
    maxlength: jQuery.validator.format("Por favor, no escribas m\u00E1s de {0} caracteres."),
    minlength: jQuery.validator.format("Por favor, no escribas menos de {0} caracteres."),
    rangelength: jQuery.validator.format("Por favor, escribe un valor entre {0} y {1} caracteres."),
    range: jQuery.validator.format("Por favor, escribe un valor entre {0} y {1}."),
    max: jQuery.validator.format("Por favor, escribe un valor menor o igual a {0}."),
    min: jQuery.validator.format("Por favor, escribe un valor mayor o igual a {0}.")
});