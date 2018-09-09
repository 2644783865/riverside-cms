angular
    .module('albumApp', [])
    .controller('AlbumController', ['$scope', '$http', function ($scope, $http) {
        $scope.initialise = function (photoCount) {
            $scope.photoCount = photoCount;
        };
        $scope.showViewer = function () {
            $scope.visible = true;
            $('body').addClass('rcms-no-scroll');
        };
        $scope.hideViewer = function () {
            $scope.visible = false;
            $('body').removeClass('rcms-no-scroll');
            $scope.selectedPhoto = undefined;
            $scope.selectedIndex = undefined;
        };
        $scope.previous = function () {
            alert('previous');
        };
        $scope.next = function () {
            if ($scope.selectedIndex != undefined) {
                var index = $scope.selectedIndex + 1;
                if (index > $scope.photoCount - 1)
                    index = 0;
                $scope.showPhoto(index);
            }
        };

        $scope.showPhoto = function (e) {
            var imageUrl = $(e.currentTarget).attr('href');
            var index = $(e.currentTarget).data('index');
            var photo = { "imageUrl": imageUrl };
            $scope.loadingVisual = true;
            $scope.selectedPhoto = photo;
            $scope.selectedIndex = index;
            if (!$scope.visible)
                $scope.showViewer();
        };
        $scope.keyDown = function (event) {
            switch (event.which) {
                case 27:
                    $scope.hideViewer();
                    break;
                case 37:
                    $scope.previous();
                    break;
                case 39:
                    $scope.next();
                    break;
                default:
                    return;
            }
            event.preventDefault();
        };
    }])
    .directive('visual', function () {
        return {
            link: function (scope, element, attrs) {
                element.bind("load", function (e) {
                    scope.loadingVisual = false;
                    scope.visualWidth = $(this)[0].naturalWidth;
                    scope.visualHeight = $(this)[0].naturalHeight;
                    scope.$apply();
                    scope.$emit('visualLoaded');
                });
            }
        };
    })
    .directive('visualiser', function () {
        return function (scope, element, attrs) {
            var w = $(element);
            scope.getVisualiserDimensions = function () {
                return { 'h': w.height(), 'w': w.width() };
            };
            scope.$watch(scope.getVisualiserDimensions, function (newValue, oldValue) {
                scope.visualiserHeight = newValue.h;
                scope.visualiserWidth = newValue.w;
                scope.visualStyle = function () {
                    if (scope.visualWidth == undefined || scope.visualHeight == undefined) {
                        return {
                            'display': 'none'
                        };
                    }
                    var visualWidth = scope.visualWidth;
                    var visualHeight = scope.visualHeight;
                    var visualiserHeight = newValue.h;
                    var visualiserWidth = newValue.w;
                    var heightRatio = visualiserHeight / visualHeight;
                    var widthRatio = visualiserWidth / visualWidth;
                    var ratio = Math.min(heightRatio, widthRatio);
                    var width = visualWidth * ratio;
                    var height = visualHeight * ratio;
                    var top = Math.max((visualiserHeight - height) / 2, 0);
                    var left = Math.max((visualiserWidth - width) / 2, 0);
                    scope.scale = ratio;
                    return {
                        'height': height + 'px',
                        'width': width + 'px',
                        'top': top + 'px',
                        'left': left + 'px'
                    };
                };
            }, true);
            $(window).resize(function () {
                scope.$apply();
            });
        };
    });