var _rcmsAlbum = null;
var _rcmsViewer = null;
var _rcmsPhotoIndex = null;
var _rcmsPhoto = null;

function GetViewerHtml() {
    return '\
        <div class="rcms-viewer">\
            <div class="rcms-viewer-backdrop">\
            </div>\
            <div class="rcms-viewer-popup">\
                <div class="rcms-viewer-main-outer">\
                    <div class="rcms-viewer-main">\
                        <div class="spinner"></div>\
                    </div>\
                </div>\
                <div class="rcms-viewer-side">\
                    <div class="rcms-viewer-options">\
                        <a href="#"><i class="fa fa-times" aria-hidden="true"></i></a>\
                    </div>\
                    <div class="rcms-viewer-details">\
                        <h3>Photo name</h3>\
                        <p>Photo description</p>\
                    </div>\
                    <div class="rcms-viewer-pager">\
                        <ul>\
                            <li><a href="#">Previous</a></li>\
                            <li><a href="#">Next</a></li>\
                        </ul>\
                    </div>\
                </div>\
            </div>\
        </div>';
}

function HideViewer() {
    if (_rcmsViewer === null)
        return false;
    $(_rcmsViewer).remove();
    $('body').removeClass('rcms-no-scroll');
    _rcmsViewer = null;
    return false;
}

function GetPhotoSizes(photoWidth, photoHeight) {
    var viewerWidth = $(_rcmsViewer).find('.rcms-viewer-main').width();
    var viewerHeight = $(_rcmsViewer).find('.rcms-viewer-main').height();
    var heightRatio = viewerHeight / photoHeight;
    var widthRatio = viewerWidth / photoWidth;
    var ratio = Math.min(heightRatio, widthRatio);
    var width = photoWidth * ratio;
    var height = photoHeight * ratio;
    var top = Math.max((viewerHeight - height) / 2, 0);
    var left = Math.max((viewerWidth - width) / 2, 0);
    return {
        width: width,
        height: height,
        top: top,
        left: left
    };
}

function PhotoLoaded() {
    var photoWidth = $(this)[0].naturalWidth;
    var photoHeight = $(this)[0].naturalHeight;
    var photoSizes = GetPhotoSizes(photoWidth, photoHeight);
    $(this).css('width', photoSizes.width + 'px').css('height', photoSizes.height + 'px').css('top', photoSizes.top + 'px').css('left', photoSizes.left + 'px');
    $(_rcmsViewer).find('.rcms-viewer-main .spinner').hide();
    $(_rcmsViewer).find('.rcms-viewer-main').append($(this));
    _rcmsPhoto = null;
}

function UpdateViewer() {
    if (_rcmsPhoto !== null) {
        _rcmsPhoto.src = '';
        _rcmsPhoto = null;
    }
    var selectedPhoto = $(_rcmsAlbum).find('.rcms-album-photo:nth-child(' + (_rcmsPhotoIndex + 1) + ') a');
    var name = $(selectedPhoto).attr('title');
    var description = $(selectedPhoto).data('description');
    var previewUrl = $(selectedPhoto).attr('href');
    $(_rcmsViewer).find('.rcms-viewer-details h3').text(name);
    $(_rcmsViewer).find('.rcms-viewer-details p').text(description);
    $(_rcmsViewer).find('.rcms-viewer-main .spinner').show();
    $(_rcmsViewer).find('.rcms-viewer-main img').remove();
    _rcmsPhoto = new Image();
    $(_rcmsPhoto).bind("load", PhotoLoaded);
    _rcmsPhoto.src = previewUrl;
}

function ShowViewer() {
    _rcmsAlbum = $(this).parent().parent().parent();
    _rcmsViewer = $(GetViewerHtml());
    _rcmsPhotoIndex = parseInt($(this).data('index'));
    _rcmsMaxPhotoIndex = $(_rcmsAlbum).find('.rcms-album-photo').length - 1;
    $(_rcmsViewer).find('.rcms-viewer-options a').click(HideViewer);
    $(_rcmsViewer).find('.rcms-viewer-pager li:first a').click(ShowPreviousPhoto);
    $(_rcmsViewer).find('.rcms-viewer-pager li:last a').click(ShowNextPhoto);
    UpdateViewer();
    $(_rcmsAlbum).append(_rcmsViewer);
    $('body').addClass('rcms-no-scroll');
    return false;
}

function ShowPreviousPhoto() {
    if (_rcmsViewer === null)
        return false;
    _rcmsPhotoIndex--;
    if (_rcmsPhotoIndex < 0)
        _rcmsPhotoIndex = _rcmsMaxPhotoIndex;
    UpdateViewer();
    return false;
}

function ShowNextPhoto() {
    if (_rcmsViewer === null)
        return false;
    _rcmsPhotoIndex++;
    if (_rcmsPhotoIndex > _rcmsMaxPhotoIndex)
        _rcmsPhotoIndex = 0;
    UpdateViewer();
    return false;
}

function ResizeWindow() {
    if (_rcmsViewer !== null) {
        var image = $(_rcmsViewer).find('.rcms-viewer-main img');
        if (image.length === 1) {
            var photoSizes = GetPhotoSizes($(image)[0].naturalWidth, $(image)[0].naturalHeight);
            $(image).css('width', photoSizes.width + 'px').css('height', photoSizes.height + 'px').css('top', photoSizes.top + 'px').css('left', photoSizes.left + 'px');
        }
    }
}

$(document).ready(function () {
    $('.rcms-album-photo a').click(ShowViewer);
    $(window).resize(ResizeWindow);
    $(document).keyup(function (e) {
        switch (e.keyCode) {
            case 27:
                HideViewer();
                break;
            case 37:
                ShowPreviousPhoto();
                break;
            case 39:
                ShowNextPhoto();
                break;
        }
    });
});