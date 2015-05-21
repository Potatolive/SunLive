rndApp.factory('unverifiedService', function ($http, $q) {
    return {      
        getAllUnverified: function () {
            var deferred = $q.defer();

            $http({ cache: true, method: 'GET', url: '/Text/FindUnverified' })
                .success(function (data, status, headers, config) {
                    deferred.resolve(data);
                })
                .error(function (data, status, headers, config) {
                    console.log(status);
                    deferred.status(reject);
                });
            return deferred.promise;
        },
        getUsageExamples: function (token) {
            var deferred = $q.defer();

            var tokenUrl = '/Text/FindUnverifiedExamples?token=' + token.AmbiguousWord;
            $http({ cache: true, method: 'GET', url: tokenUrl })
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
        getNDC: function (token) {
            var deferred = $q.defer();

            var tokenUrl = '/Text/GetNDC?token=' + token;
            $http({ cache: true, method: 'GET', url: tokenUrl })
                .success(function (data, status, headers, config) {
                    deferred.resolve(data);
                })
                .error(function (data, status, headers, config) {
                    console.log(status);
                    deferred.status(reject);
                });
            return deferred.promise;
        },
        getRxNorm: function (token, decider) {
            var deferred = $q.defer();

            var tokenUrl = '/Text/GetRxNORM?token=' + token + '&&isSuppressed=' + decider;
            $http({ cache: true, method: 'GET', url: tokenUrl })
                .success(function (data, status, headers, config) {
                    deferred.resolve(data);
                })
                .error(function (data, status, headers, config) {
                    console.log(status);
                    deferred.status(reject);
                });
            return deferred.promise;
        },
        getMDDB: function (token) {
            var deferred = $q.defer();

            var tokenUrl = '/Text/GetMDDB?token=' + token;
            $http({ cache: true, method: 'GET', url: tokenUrl })
                .success(function (data, status, headers, config) {
                    deferred.resolve(data);
                })
                .error(function (data, status, headers, config) {
                    console.log(status);
                    deferred.status(reject);
                });
            return deferred.promise;
        },
    };
});