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

function exportImgs(path) {
  $.ajax({
      url: '/admin/export',
      type: "POST",
      dataType: "xml/html/script/json", // expected format for response
      contentType: "application/json", // send as JSON
      data: JSON.stringify({ path: path })
  });
}

function approveImg(id) {

};

function deleteImg(id) {
  $.ajax({
    url: '/admin/delete/' + id,
    type: 'DELETE',
    success: function(result) {
        location.reload();
    }
  });
}