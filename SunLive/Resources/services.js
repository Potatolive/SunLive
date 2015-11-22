rndApp.factory('chartService', function ($http, $q, $routeParams) {
    return {      
        getUniqueChartData: function () {
            var deferred = $q.defer();

            $http({ cache: true, method: 'GET', url: '/Analytics/GetUniqueTimeSeries?pageName=' + $routeParams.pageName})
                .success(function (data, status, headers, config) {
                    deferred.resolve(data);
                })
                .error(function (data, status, headers, config) {
                    console.log(status);
                    deferred.status(reject);
                });
            return deferred.promise;
        },
        getPerDayInfo: function () {
            var deferred = $q.defer();

            $http({ cache: true, method: 'GET', url: '/Analytics/GetPerDayInfo' })
                .success(function (data, status, headers, config) {
                    deferred.resolve(data);
                })
                .error(function (data, status, headers, config) {
                    console.log(status);
                    deferred.status(reject);
                });
            return deferred.promise;
        },
        getTotalMessages: function (fromDate, toDate) {
            var deferred = $q.defer();

            $http({ cache: true, method: 'GET', url: '/Analytics/GetTotalMessages?fromDate=' + fromDate + '&toDate=' + toDate })
                .success(function (data, status, headers, config) {
                    deferred.resolve(data);
                })
                .error(function (data, status, headers, config) {
                    console.log(status);
                    deferred.status(reject);
                });
            return deferred.promise;
        },
        
        saveWordCorrection: function (word) {
            var deferred = $q.defer();
                       
            
            $http({ method: 'POST', url: '/Text/SaveCorrection', data : word})
                .success(function (data, status, headers, config) {
                    deferred.resolve(data);
                })
                .error(function (data, status, headers, config) {
                    deferred.reject(status);
                });

            return deferred.promise;
        },
    };
});