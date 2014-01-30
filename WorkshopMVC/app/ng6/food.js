// ng6 - add a food controller, injecting the 'datacontext'
(function () {
    angular.module('app')
           .controller('food', ['datacontext', foodVm]);

    function foodVm(datacontext) {
        var vm = this;
        vm.foods = [];
        vm.ling = link;
        vm.title = '"Controller as" ViewModel';

        datacontext.getFoods().then(success);

        function success(foods) {
            vm.foods = foods;
        }

        function link(food) {
            return "api/" + food.id + "/" + food.name;
        };
    }
})();