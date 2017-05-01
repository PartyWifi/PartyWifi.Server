$(document).ready(function() {
    $('.image-link').magnificPopup({
        type:'image',
        closeOnContentClick: true,
        enableEscapeKey: true,
        callbacks: {   
          open: function() {
            location.href = location.href.split('#')[0] + "#popup";
          },
          close: function() {
            if (location.hash) 
              history.go(-1);
          }
        }
    });
});

$(window).on('hashchange',function() {
    if (location.href.indexOf("#popup") < 0) {
        $.magnificPopup.close(); 
    }
});