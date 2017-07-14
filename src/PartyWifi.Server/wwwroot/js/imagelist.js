function ImageViewModel(image) {
    var self = this;
    var model = image;

    self.identifier = model.identifier;
    self.isDeleted = ko.observable(model.isDeleted);
    self.uploadDate = new Date(Date.parse(model.uploadDate));
}

function ImageListViewModel() {
    var self = this;

    var maxPerLoad = 10;
    var totalImages = 0;
    self.isLoading = ko.observable(false);
    self.images = ko.observableArray();

    var load = function (offset, callback) {
        $.ajax({
            url: '/api/images?limit=' + maxPerLoad + '&offset=' + offset,
            type: 'GET'
        }).done(function(data) {
            totalImages = data.total;
            ko.utils.arrayForEach(data.images,
                function(image) {
                    self.images.push(new ImageViewModel(image));
                });
        }).fail(function() {
            //TODO: error handling
        }).always(function() {
            if (callback)
                callback();
        });
    }

    self.imagesUpdated = function() {
        partyWifi.applyMagnificPopup();
    }

    // Remove an image
    self.remove = function (image) {
        $.ajax({
            url: '/api/images/' + image.identifier,
            type: 'DELETE'
        }).done(function(data) {
            image.isDeleted(true);
        }).fail(function() {
            //TODO: error handling
        });
    }

    // Load more images on scroll down
    $(window).scroll(function () {
        var currentImageCount = self.images().length;;
        if (totalImages == currentImageCount)
            return;

        if (!self.isLoading() && ($(window).scrollTop() > $(document).height() - $(window).height() - 100)) {
            self.isLoading(true);

            // offset are the current count of images
            load(currentImageCount, function () {
                // reset value of loading once content loaded
                self.isLoading(false);
            });
        }
    });

    // Initial load of images
    load(0);
}

ko.applyBindings(new ImageListViewModel());