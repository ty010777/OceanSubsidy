gsmap = typeof gsmap === "undefined" ? {} : gsmap;
gsmap.ui = gsmap.ui || {};

/**
* gsmap.func.Identify construction options
* @typedef {{functions:(string)}} gsmap.ui.MapUI_Options
* @property {string} functions 圖台ui module
*/

/**
 * MapUI
 *   |-- BottomUI
 *   |-- FunctionUI
 * @param {ol.MapPlugin} plugin MapPlugin instance
 * @param {gsmap.ui.MapUI_Options} options MapUI options
 */
gsmap.ui.MapUI = function (plugin, options) {
    var $mapDom = $(plugin.getMap().getTargetElement());
    if (!$mapDom.find(".GSMap-wrapper").length) {
        $mapDom.append("<div class='GSMap-wrapper' />");
    }

    var bottomUI = new gsmap.ui.BottomUI(plugin);
    var functionsUI = new gsmap.ui.Functions(plugin, options);

    this.getBottomUI = function () {
        return bottomUI;
    };

    this.getFunctionsUI = function () {
        return functionsUI;
    };
};