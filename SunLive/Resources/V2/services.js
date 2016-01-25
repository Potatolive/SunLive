v2App.factory('cropService', ['$http', '$q', '$routeParams', function ($http, $q, $routeParams) {
    return {      
        crop: function (cropInfo) {
            return $http({
                method: "POST",
                url: "../../Post/Crop",
                data: cropInfo
            });
        },
        revertCrop: function (cropInfo) {
            return $http({
                method: "POST",
                url: "../../Post/RevertCrop",
                data: cropInfo
            });
        }
    }
}]);

v2App.factory('postService', ['$http', '$q', '$routeParams', function ($http, $q, $routeParams) {
    return {
        approve: function (post) {
            return $http({
                method: "GET",
                url: "../../Post/Approve/?id=" + post.ImgId + "&pageName=" + post.pageName,
                data: post
            });
        },
        reject: function (post) {
            return $http({
                method: "GET",
                url: "../../Post/Reject/?id=" + post.ImgId + "&pageName=" + post.pageName,
                data: post
            });
        },
        prioritize: function (post) {
            return $http({
                method: "GET",
                url: "../../Post/Prioritize/?id=" + post.ImgId + "&pageName=" + post.pageName,
                data: post
            });
        }
    };
}]);

v2App.factory('dayInfoService', [function () {
    var dayInfo = {PageName: 'AdithyaTV', Date: '2016-01-24 00:00:00'}

    return {
        get: function () {
            return dayInfo;
        },
        set: function (value) {
            dayInfo = value;
        }
    };
}]);
