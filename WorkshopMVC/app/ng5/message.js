// ng5 - add a message controller, injecting the 'datacontext'
(function () {

    angular.module('app').controller('message', ['datacontext', messageVm]);

    function messageVm(datacontext) {
        this.message = datacontext.getMessage();
    };

})();
