// ng5 - add a food ViewModel, injecting the $scope and the datacontext
(function () {
    angular.module('app')
           .controller('food$scope', ['$scope', 'datacontext', foodVm]);

    function foodVm($scope, datacontext) {
        var vm = {
            foods: datacontext.getFoods(), // synchronous
            link: link,
            title: '"Controller with $scope" ViewModel'
        };

        $scope.vm = vm;

        function link(food) {
            return "api/" + food.id + "/" + food.name;
        };
    }
})();