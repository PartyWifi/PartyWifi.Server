// Function to update image
function update() {
    $.get('slideshow/next', function (slideshowImage) {
        // Show new image
        $('#image').attr('src', 'slideshow/image/' + slideshowImage.imageId);
        setTimeout(update, slideshowImage.rotationMs);
    });
}

// Load first image
update();