(function(){
    angular.module('common').factory('common.errorService', service);

    function service() {
        var messages = [];
        var testErrNum = 1;

        return {
            addMessage: addMessage,
            addTestMessage: addTestMessage,
            clearMessages: clearMessages,
            messages: messages
        };

        function addMessage(msg) {
            messages.push(msg);
        }

        function addTestMessage() {
            addMessage("Added test error message " +
                testErrNum++ + " at " + new Date().toTimeString());
        }
        function clearMessages() {
            messages.length = 0;
        }
    }

})();