// Function to update image
function update() {
    $.get('slideshow/next', function (slideshowImage) {
        // Show new image
        showImage(slideshowImage);
        setTimeout(update, slideshowImage.rotationMs);
    });
}

function showImage(slideshowImage) {
    $('#image').attr('src', 'slideshow/image/' + slideshowImage.file);
}

// Initialize client
$.get('slideshow/init', function (slideshowImage) {
    showImage(slideshowImage);
    setTimeout(update, slideshowImage.rotationMs);
});