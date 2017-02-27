var latest = ''; // Latest picture known to client

// Function to update image
function update() {
    $.get('slideshow/next/' + latest, function (data) {
        // Show new image
        $('#image').attr('src', 'slideshow/image/' + data.file);
        if (data.file > latest)
            latest = data.file; // Update latest if response is newer 
    });
}

// Initialize client
$.get('slideshow/init', function (data) {
    latest = data.file;
    setInterval(update, data.rotationMs);
});