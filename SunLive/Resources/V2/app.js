var LoadingStatus = {
    LOADING: 0,
    COMPLETE: 1,
    NODATA: 2,
    ERROR: 4
};

var v2App = angular.module("v2App",
    [
        "ngRoute",
        "ui.bootstrap",
        'akoenig.deckgrid',
        'ngJcrop',
        'angular-inview',
        'ng-bootstrap-datepicker'
    ]);

v2App.config(["$routeProvider", "ngJcropConfigProvider", function ($routeProvider, ngJcropConfigProvider) {
    $routeProvider
        .otherwise({
            templateUrl: "../Resources/V2/App.html",
            controller: "PostController"
        });

    ngJcropConfigProvider.setJcropConfig({
        bgColor: 'black',
        bgOpacity: .5,
        aspectRatio: 1,
        maxWidth: 230,
        maxHeight: 500
    });

    ngJcropConfigProvider.setPreviewStyle({
        'display': 'none'
    });
}]);