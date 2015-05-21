var LoadingStatus = {
    LOADING: 0,
    COMPLETE: 1,
    NODATA: 2,
    ERROR: 4
};

var rndApp = angular.module("rndApp", ["ngRoute", "ui.bootstrap", "chieffancypants.loadingBar", "services.hub"]);

rndApp.config(function ($routeProvider, cfpLoadingBarProvider) {
    $routeProvider
        .when("/", {
            templateUrl: "Angular/LabsDemo",
        })
        .otherwise({
            templateUrl: "Angular/LandingPage",
            controller: "landingPageController"
        });
});


