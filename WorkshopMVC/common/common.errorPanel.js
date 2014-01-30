(function(){
    angular.module('common')
    .controller('common.errorPanel', ['common.errorService', errorPanel]);

    function errorPanel(errorService) {
        var vm = this;
        vm.clearMessages = clearMessages;
        vm.getMessages = getMessages;
        vm.hasMessages = hasMessages;

        function clearMessages() {
            errorService.clearMessages();
        }
        function getMessages() {
            return errorService.messages;
        }
        function hasMessages() {
            return errorService.messages.length > 0;
        }
    }

})();