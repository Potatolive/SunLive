v2App.controller('PostController',
    ['$scope', '$http', '$filter', '$timeout', 'cropService', 'postService', 'dayInfoService',
        function ($scope, $http, $filter, $timeout, cropService, postService, dayInfoService) {

            $scope.opened = false;

            $scope.open = function (event) {
                try {
                    event.preventDefault();
                    event.stopPropagation();
                    
                }
                catch(e) {
                    console.log(e);
                }

                $scope.opened = true;
            };

            $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
            $scope.format = $scope.formats[0];
            $scope.altInputFormats = ['M!/d!/yyyy'];

            $scope.dateOptions = {
                formatYear: 'yy',
                startingDay: 1
            };  

            $scope.minDate = "2016-01-01";
            $scope.maxDate = new Date();

            $scope.dayInfo = {};
            $scope.dayInfo.newMessage = 0;
            $scope.dayInfo.Source = "both";
            $scope.Latest = false;

            var activate = function () {

                $scope.processing = true;

                var dayInfo = $scope.dayInfo;
                $scope.dayInfo.newMessage = 0;

                var params = {};

                if (!Date.parse(dayInfo.Date)) {
                    var re = /-?\d+/;
                    var m = re.exec(dayInfo.Date);
                    dayInfo.Date = new Date(parseInt(m[0]));
                }

                params.PageName = dayInfo.PageName;
                params.CurrentPage = dayInfo.CurrentPage;
                params.Date = dayInfo.Date;
                params.SelectedHour = dayInfo.SelectedHour;
                params.SelectedMin = dayInfo.SelectedMin;
                params.Source = dayInfo.Source;
                params.Latest = dayInfo.Latest;

                $http(
                {
                    method: 'GET',
                    url: 'Posts',
                    headers: { 'Accept': 'application/json' },
                    params: params
                }).success(function (dayInfo) {
                    
                    //dayInfoService.set(dayInfoReturn);
                    //var dayInfo = dayInfoService.get();

                    if (!Date.parse(dayInfo.Date)) {
                        var re = /-?\d+/;
                        var m = re.exec(dayInfo.Date);
                        dayInfo.Date = new Date(parseInt(m[0]));
                    }

                    var photos = dayInfo.Posts;

                    angular.forEach(photos, function (photo, $http) {
                        var jsonDate = photo.PublishedOnUniversal;
                        var re = /-?\d+/;
                        var m = re.exec(jsonDate);
                        photo.PublishedOnUniversal = new Date(parseInt(m[0]));

                        photo.selection = [0, 0, 230, 230, 230, 230];
                        photo.thumbnail = true;
                        photo.inview = false;

                        photo.photoInView = function (index, inview, inviewpart, event) {
                            if (inview) {
                                photo.inview = true;
                            };
                            return false;
                        }

                        photo.crop = function () {

                            var imageWidth = 0;

                            if(photo.selection[4] < 230) {
                                imageWidth = 230;
                            }
                            

                            var cropInfo = {
                                X: photo.selection[0],
                                Y: photo.selection[1],
                                W: photo.selection[4],
                                H: photo.selection[5],
                                pageName: "AdithyaTV",
                                ImgId: photo._id,
                                imageWidth: imageWidth
                            };

                            console.log(photo.imageHeight);

                            cropService.crop(cropInfo).success(function (response) {
                                photo.CroppedImageURL = response;
                                photo.selection = [0, 0, 230, 230, 230, 230];
                            });
                        };

                        photo.revertCrop = function () {
                            var cropInfo = {
                                pageName: "AdithyaTV",
                                ImgId: photo._id
                            };
                            cropService.revertCrop(cropInfo).success(function (response) {
                                photo.CroppedImageURL = '';
                            });
                        };

                        photo.getImageURL = function () {
                            if (!photo.inview) return "../../Images/images.png";

                            if (photo.isCropped()) {
                                return photo.CroppedImageURL;
                            }

                            return photo.ImageURL;
                        };

                        photo.isCropped = function () {
                            if (photo.CroppedImageURL && photo.CroppedImageURL != '') {
                                return true;
                            }

                            return false;
                        };

                        photo.approve = function () {
                            var post = {
                                pageName: "AdithyaTV",
                                ImgId: photo._id
                            };

                            postService.approve(post).success(function (response) {
                                console.log('Approved');
                                photo.Status = "Approved";
                            });

                        };

                        photo.reject = function () {
                            var post = {
                                pageName: "AdithyaTV",
                                ImgId: photo._id
                            };

                            postService.reject(post).success(function (response) {
                                console.log('Rejected');
                                photo.Status = "Rejected";
                            });
                        };

                        photo.prioritize = function () {
                            var post = {
                                pageName: "AdithyaTV",
                                ImgId: photo._id
                            };

                            postService.prioritize(post).success(function (response) {
                                console.log('Prioritized');
                                photo.Status = "Approved";
                            });
                        };

                        photo.isApproved = function () {
                            if (photo.Status && photo.Status === "New") {
                                return false;
                            }

                            return true;
                        };
                    });

                    $scope.dayInfo.Pages = []

                    for (var i = 0; i < dayInfo.TotalPages; i++) {
                        $scope.dayInfo.Pages.push(i);
                    }

                    $scope.dayInfo.Posts = dayInfo.Posts;
                    $scope.dayInfo.TodaysCount = dayInfo.TodaysCount;

                    if ($scope.dayInfo.CurrentPage == undefined) $scope.dayInfo.CurrentPage = 0;

                    $scope.dayInfo.Latest = false;
                    $scope.processing = false;

                }).error(function (response) {
                    $scope.processing = false;
                });
            }

            var setNow = function () {
                var todaysDate = new Date();
                var dayInfo = $scope.dayInfo;

                dayInfo.Date = todaysDate.toDateString();
                dayInfo.SelectedHour = todaysDate.getHours();
                dayInfo.SelectedMin = todaysDate.getMinutes();
                dayInfo.PageName = "AdithyaTV";
            }

            var next = function (mins) {
                var dayInfo = $scope.dayInfo;

                dayInfo.SelectedMin += mins;

                if (dayInfo.SelectedMin >= 60) {
                    dayInfo.SelectedHour++;
                    dayInfo.SelectedMin -= 60;
                }

                if (dayInfo.SelectedHour >= 24) {
                    var date = new Date((new Date(dayInfo.Date)).getTime() + 1 * 24 * 60 * 60 * 1000);

                    if (!Date.parse(date)) {
                        var re = /-?\d+/;
                        var m = re.exec(dayInfo.Date);
                        dayInfo.Date = new Date(parseInt(m[0]));
                    }
                    else {
                        dayInfo.Date = new Date(date);
                    }
                    console.log(dayInfo.Date);
                    dayInfo.SelectedHour = 0;
                }
            }

            var prev = function (mins) {
                var dayInfo = $scope.dayInfo;
                dayInfo.SelectedMin -= mins;

                if (dayInfo.SelectedMin < 0) {
                    dayInfo.SelectedHour--;
                    dayInfo.SelectedMin = 60 + dayInfo.SelectedMin;
                }

                if (dayInfo.SelectedHour < 0) {
                    var date = new Date((new Date(dayInfo.Date)).getTime() - 1 * 24 * 60 * 60 * 1000);
                    
                    if (!Date.parse(date)) {
                        var re = /-?\d+/;
                        var m = re.exec(dayInfo.Date);
                        dayInfo.Date = new Date(parseInt(m[0]));
                    }
                    else {
                        dayInfo.Date = new Date(date);
                    }
                    console.log(dayInfo.Date);
                    dayInfo.SelectedHour = 24 + dayInfo.SelectedHour;
                }

                console.log(dayInfo);
            }
            
            setNow();
            activate();

            $scope.moveToPage = function (pageNumber) {
                var dayInfo = $scope.dayInfo;
                dayInfo.CurrentPage = pageNumber;
                activate();
            }

            $scope.search = function () {
                activate();
            }

            $scope.now = function () {
                setNow();
                activate();
            }

            $scope.next = function () {
                next(5);
                activate();
            }

            $scope.prev = function () {
                prev(5);
                console.log($scope.dayInfo);
                activate();
            }

            $scope.latest = function () {
                setNow();
                $scope.dayInfo.Latest = true;
                activate();
            }

            $scope.fix = function () {
                $scope.fixing = true;
                $http({
                    method: "POST",
                    url: "../../Post/ClearWhatsApp",
                    data: {},
                }).success(function (msg) {
                    $scope.fixing = false;
                }).
                error(function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(errorThrown);
                    $scope.fixing = false   ;
                });
            }


            $scope.clock = "loading clock..."; // initialise the time variable
            $scope.tickInterval = 1000 //ms

            var tick = function () {
                $scope.clock = Date.now() // get the current time
                $timeout(tick, $scope.tickInterval); // reset the timer
            }

            // Start the timer
            $timeout(tick, $scope.tickInterval);

            var getCount = function() {

                $http({
                    method: "GET",
                    url: "../../V2/TodaysCount/?pageName=AdithyaTV" + "&publishedOn=" + new Date(),
                    contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                    
                }).success(function (msg) {
                    console.log(msg + ' ' + $scope.dayInfo.TodaysCount);
                    $scope.dayInfo.newMessage = msg - $scope.dayInfo.TodaysCount;
                }).error(function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(errorThrown);
                });

                $timeout(getCount, $scope.tickInterval*10); // reset the timer
            }

            $timeout(getCount, $scope.tickInterval*3);
}]);

v2App
.directive('datepickerPopup', function () {
    return {
        restrict: 'EAC',
        require: 'ngModel',
        link: function (scope, element, attr, controller) {
            //remove the default formatter from the input directive to prevent conflict
            controller.$formatters.shift();
        }
    }
});