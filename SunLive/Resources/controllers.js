rndApp.controller('landingPageController', function ($scope, $location, filterFilter, unverifiedService) {

    
    $scope.currentPage = 1;
    $scope.itemsPerPage = 10;


    unverifiedService.getAllUnverified().then(function (data) {
        $scope.unverified = data;
        $scope.totalItems = data.length;
        //$scope.noOfPages = Math.ceil(data.length / $scope.itemsPerPage);

        $scope.unVerifiedfiltered = filterFilter($scope.unverified, { 'AmbiguousWord': undefined });
        $scope.noOfPages = Math.ceil($scope.unVerifiedfiltered.length / $scope.itemsPerPage);

    });

    $scope.updateList = function (term) {
        $scope.unVerifiedfiltered = filterFilter($scope.unverified, { 'AmbiguousWord': term });

        $scope.totalItems = $scope.unVerifiedfiltered.length;
        $scope.noOfPages = Math.ceil($scope.unVerifiedfiltered.length / $scope.itemsPerPage);
        console.log($scope.noOfPages);
    };  

    $scope.SaveCorrection = function (recordToUpdate) {
        recordToUpdate.showSpinner = true;
        unverifiedService.saveWordCorrection(recordToUpdate).then(function (data) {
            recordToUpdate.showSpinner = false;
            recordToUpdate.status = LoadingStatus.COMPLETE;
        }, function (status) {
            recordToUpdate.showSpinner = false;
            recordToUpdate.status = LoadingStatus.ERROR;
        });
    };    
        
});

rndApp.controller('cdmExamplesController', function ($scope, $location, filterFilter, unverifiedService) {
    
    $scope.currentPage = 1;
    $scope.itemsPerPage = 10;

    $scope.initConfig = function (token) {

        unverifiedService.getUsageExamples(token).then(function (data) {

            $scope.cdmExamples = data;            

            //For Pagination
            $scope.totalItems = data.length;
            $scope.noOfPages = Math.ceil(data.length / $scope.itemsPerPage);            

        });
    };


});



rndApp.controller('ndcController', function ($scope, $timeout, $location, filterFilter, unverifiedService) {

    $scope.currentPage = 1;
    $scope.itemsPerPage = 10;    
   
    $scope.$watch('word.HumanOverride', function (value) {
        $scope.updateDrugStandards($scope.word.HumanOverride);
    });

    $scope.updateDrugStandards = function (token) {
        $scope.drugData = [];
        unverifiedService.getNDC(token).then(function (data) {
            $scope.drugData = data;

            //For Pagination
            $scope.totalItems = data.length;
            $scope.noOfPages = Math.ceil(data.length / $scope.itemsPerPage);
        });
    };

    $scope.initConfig = function (token) {
        //$scope.updateDrugStandards(token);
    };


});


rndApp.controller('rxNormSuppController', function ($scope, $timeout, $location, filterFilter, unverifiedService) {

    $scope.currentPage = 1;
    $scope.itemsPerPage = 10;

    $scope.$watch('word.HumanOverride', function (value) {
        $scope.updateDrugStandards($scope.word.HumanOverride);
    });

    $scope.updateDrugStandards = function (token) {
        $scope.drugData = [];
        unverifiedService.getRxNorm(token, true).then(function (data) {
            $scope.drugData = data;

            //For Pagination
            $scope.totalItems = data.length;
            $scope.noOfPages = Math.ceil(data.length / $scope.itemsPerPage);
        });
    };

    $scope.initConfig = function (token) {
        //$scope.updateDrugStandards(token);
    };


});


rndApp.controller('rxNormNonSuppController', function ($scope, $timeout, $location, filterFilter, unverifiedService) {

    $scope.currentPage = 1;
    $scope.itemsPerPage = 10;

    $scope.$watch('word.HumanOverride', function (value) {
        $scope.updateDrugStandards($scope.word.HumanOverride);
    });

    $scope.updateDrugStandards = function (token) {
        $scope.drugData = [];
        unverifiedService.getRxNorm(token, false).then(function (data) {    // Note false parameter
            $scope.drugData = data;

            //For Pagination
            $scope.totalItems = data.length;
            console.log($scope.totalItems);
            $scope.noOfPages = Math.ceil(data.length / $scope.itemsPerPage);
        });
    };

    $scope.initConfig = function (token) {
        //$scope.updateDrugStandards(token);
    };


});

rndApp.controller('mddbController', function ($scope, $timeout, $location, filterFilter, unverifiedService) {

    $scope.currentPage = 1;
    $scope.itemsPerPage = 10;

    $scope.$watch('word.HumanOverride', function (value) {
        $scope.updateDrugStandards($scope.word.HumanOverride);
    });

    $scope.updateDrugStandards = function (token) {
        $scope.drugData = [];
        unverifiedService.getMDDB(token, false).then(function (data) {    // Note false parameter
            $scope.drugData = data;

            //For Pagination
            $scope.totalItems = data.length;
            $scope.noOfPages = Math.ceil(data.length / $scope.itemsPerPage);
        });
    };

    $scope.initConfig = function (token) {
        //$scope.updateDrugStandards(token);
    };


});
