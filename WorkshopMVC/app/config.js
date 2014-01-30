/* Configuration for the Yak */
(function () {

    angular.module('app').factory('config', config);

    function config() {
        var cfg = {
            errorPanelView:'../common/common.errorPanel.html'
        };
        return cfg;
    }
})();
