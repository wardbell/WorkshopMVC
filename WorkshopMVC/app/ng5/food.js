// ng5 - add a food ViewModel, injecting the 'datacontext'
(function () {
    angular.module('app').controller('food', ['datacontext', foodVm]);

    function foodVm(datacontext) {
        var vm = this;
        vm.foods = datacontext.getFoods(); // synchronous
        vm.link = link;
        vm.title = '"Controller as" ViewModel';

        function link (food) {
            return "api/" + food.id + "/" + food.name;
        };
    }
})();
