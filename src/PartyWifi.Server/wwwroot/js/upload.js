function ImageUpload(file) {
    var self = this;
    self.file = file;
    self.data = ko.observable();
    self.progress = ko.observable(0.00);
    self.isUploading = ko.observable(false);
    
    self.loadPreview = function(callback) {
        var reader = new FileReader();
        reader.onload = function(e) {
            self.data(e.target.result);
            callback();
        }
        reader.readAsDataURL(file);
    }

    self.upload = function(successCallback, failureCallback, alwaysCallback) {
        self.isUploading(true);
        var data = new FormData();
        data.append(self.file.name, self.file);

        $.ajax({
            url: '/image/add',
            type: 'POST',
            data: data,
            processData: false, 
            contentType: false,
            xhr: function() {
                var myXhr = $.ajaxSettings.xhr();
                if (myXhr.upload) {
                    myXhr.upload.addEventListener('progress', function(e) {
                        if (e.lengthComputable) {
                            var percentage = (e.loaded * 100)/e.total;
                            self.progress(Math.round(percentage * 100) / 100);
                        }  
                    }, false);
                }
                return myXhr;
            },
        })
        .always(function() {
            alwaysCallback(self);
        })
        .done(function() {
            successCallback(self);
        })
        .fail(function() {
            failureCallback(self);
        });
    }
}

function UploadViewModel() {
    var self = this;

    self.imageUploads = ko.observableArray();
    self.isUploading = ko.observable(false);
    self.isLoadingFileList = ko.observable(false);

    self.onFileSelectedEvent = function(vm, evt) {
        // clear current uploads - maybe add later
        self.imageUploads([]);
        self.isLoadingFileList(true);
        var loaded = [];
        var files = evt.target.files;
        ko.utils.arrayForEach(files, function(file) {
            var upload = new ImageUpload(file);
            upload.loadPreview(function() {
                loaded.push(upload);

                // if all images are pushed to the array, activate magnific popup
                if (loaded.length == files.length) {
                    // disable busy indicator
                    self.isLoadingFileList(false);
                    // update file list
                    self.imageUploads(loaded);
                    // enable magnific popup
                    $('.image-link').magnificPopup({type:'image'});
                }
            });
        });
    }

    self.uploadImage = function(image) {
        var onSuccessUpload = function(image) {
            self.imageUploads.remove(image);
        }

        var onFailUpload = function(image) {
            alert("handle fail...");
        }

        var onAlways = function(image) {
            if (self.imageUploads().length == 1 && self.imageUploads()[0] == image) {
                self.isUploading(false);
            }
        }
        self.isUploading(true);
        image.upload(onSuccessUpload, onFailUpload, onAlways);
    }

    self.uploadImages = function() {
        for (var i = 0; i < self.imageUploads().length; i++){
            var next = self.imageUploads()[i];
            self.uploadImage(next);
        }
    }

    self.removeImage = function(image) {
        self.imageUploads.remove(image);
    };
}

ko.applyBindings(new UploadViewModel());