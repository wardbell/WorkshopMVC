// Shell ViewModel
(function() {
    angular.module('app').controller('shell', ['common.errorService','config', shellVm]);
    
    function shellVm(errorService,config) {

        var vm = this;

        vm.addTestError = addTestError;
        vm.errorPanel = config.errorPanelView;
        vm.isFoodActive = false;
        vm.isFoodScopeActive = false;
        vm.isMessageActive = false;
        vm.selectedView = '';
        vm.showMessage = showMessage;
        vm.showFood = showFood;
        vm.showFood$Scope = showFood$Scope;

        showMessage(); // initial state

        /* private implementation */

        function activate(viewName) {
            vm.isFoodActive =      viewName === 'food';
            vm.isFoodScopeActive = viewName === 'food$scope';
            vm.isMessageActive =   viewName === 'message';
            vm.selectedView = getViewUrl(viewName);
        }

        function addTestError() {
            errorService.addTestMessage();
        }

        function getViewUrl(viewName) {
            return '/app/' + viewName + '.html';
        }

        function showMessage() { activate("message"); }
        function showFood() { activate("food"); }
        function showFood$Scope() { activate("food$scope"); }
    }
})();

