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

    self.upload = function(successCallback, failureCallback) {
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
    self.uploadPossible = ko.computed(function() {
        return self.imageUploads().length > 0;
    });

    self.onFileSelectedEvent = function(vm, evt) {
        self.imageUploads([]);
        var files = evt.target.files;
        ko.utils.arrayForEach(files, function(file) {
            var upload = new ImageUpload(file);
            upload.loadPreview(function() {
                self.imageUploads.push(upload);

                // if all images are pushed to the array, activate magnific popup
                if (self.imageUploads().length == files.length){
                    $('.image-link').magnificPopup({type:'image'});
                }
            });
        });
    }

    self.uploadImage = function(image) {
        var onSuccessUpload = function(image) {
            self.removeImage(image);
        }

        var onFailUpload = function(image) {
            alert("handle fail...");
        }

        image.upload(onSuccessUpload, onFailUpload);
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