// ng6 - add a food ViewModel, injecting the $scope and the datacontext
(function () {
    angular.module('app')
           .controller('food$scope', ['$scope', 'datacontext', foodVm]);

    function foodVm($scope, datacontext) {
        var vm = {
            foods: [],
            link: link,
            title: '"Controller with $scope" ViewModel'
        };

        $scope.vm = vm;

        datacontext.getFoods().then(success);

        function success(foods) {
            vm.foods = foods;
        }

        function link(food) {
            return "api/" + food.id + "/" + food.name;
        };
    }
})();