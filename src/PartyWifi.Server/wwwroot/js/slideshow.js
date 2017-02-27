var latest = ''; // Latest picture known to client
var rotationMs = 0;

// Function to update image
function update() {
    $.get('slideshow/next/' + latest, function (data) {
        // Show new image
        showImage(data.file);

        if (data.file > latest)
            latest = data.file; // Update latest if response is newer 

        setTimeout(update, rotationMs);
    });
}

function showImage(file) {
    $('#image').attr('src', 'slideshow/image/' + file);
}

// Initialize client
$.get('slideshow/init', function (data) {
    latest = data.file;
    rotationMs = data.rotationMs;

    showImage(data.file);
    setTimeout(update, rotationMs);
});