// ng6 - add a message controller, injecting the 'datacontext'
(function() {
    angular.module('app')
           .controller('message', ['datacontext', messageVm]);

    function messageVm(datacontext) {
        var vm = this;
        vm.message = "...";

        datacontext.getMessage().then(success).catch(fail);

        function success(message) {
            vm.message = message;
        }

        function fail(error) {
            alert("Message retrieval failed with " + error);
        }
    }
})();
