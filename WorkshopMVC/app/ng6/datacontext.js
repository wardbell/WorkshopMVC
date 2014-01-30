// The service that handles data access
(function () {
    angular.module('app')
           .factory('datacontext', ['$http', '$timeout', service]); 

    function service($http, $timeout) {

        var model, promise;

        var dc = {
            getFoods: getFoods,
            getMessage: getMessage
        };

        init();

        return dc;

        /* private implementation */

        function getFoods() {
            return promise.then(
                function () {
                    return model.foods;
                }
            );
        }

        function getMessage() {
            return promise.then(
                function () {
                    //throw "oops .. something went wrong";
                    return model.message;
                }
            );
        }

        function init() {
            // Load data as soon as datacontext is created.
            // Delay 3/4 second to simulate network latency; then get for data 
            promise = $timeout(loadDto, 750, false);
        }

        function loadDto() {

            return $http.get('/Angular/Ng6Data').success(success).error(failed);

            function success(data) {
                model = data;
                console.log(model);
                return model;
            }

            function failed(error) {
                console.log("Data load failed: " + error);
                throw error;
            }
        }
    }
})();

