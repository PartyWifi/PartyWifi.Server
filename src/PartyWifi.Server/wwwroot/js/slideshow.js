// Function to update image
function update() {
    $.get('slideshow/next', function (slideshowImage) {
        // Show new image
        var image = $('#image');

        // Only using fading if activated
        if (location.search.includes('fading')) {
            image.fadeOut(1000, function() {
                image.css('background-image', "url(image/resized/" + slideshowImage.imageId + ")");
                image.fadeIn(1000);
            });
        } else {
            image.css('background-image', "url(image/resized/" + slideshowImage.imageId + ")");
        }       
        
        setTimeout(update, slideshowImage.rotationMs);
    }).fail(function () {
        // Retry after half a second
        setTimeout(update, 500);
    });
}

// Load first image
update();