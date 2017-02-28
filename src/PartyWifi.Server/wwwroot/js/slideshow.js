// Function to update image
function update() {
    $.get('slideshow/next', function (slideshowImage) {
        // Show new image
        var image = $('#image');

        image.fadeOut(1000, function() {
            image.attr('src', 'slideshow/image/' + slideshowImage.imageId);
            image.fadeIn(1000);
        });
        
        setTimeout(update, slideshowImage.rotationMs);
    });
}

// Load first image
update();