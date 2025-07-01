gsmap = typeof gsmap === "undefined" ? {} : gsmap;
gsmap.func = gsmap.func || {};

/**
* gsmap.func.RangeQuery construction options
* @typedef {{mapUI:(gsmap.ui.MapUI)}} gsmap.func.RangeQuery_Options
* @property {gsmap.ui.MapUI} mapUI 圖台ui module
*/


/**
 *
 * 結構<![CDATA[
 * <div class=".gsmap-bottom-identify">
 *     identify內容
 * </div>
 * ]]>
 * @param {ol.MapPlugin} _plugin MapPlugin instance
 * @param {JQueryDom} $identifyContent jQuery DOM of identify content
 * @param {gsmap.ui.IdentifyUI_Options} options module of bottom UI
 */
gsmap.func.RangeQuery = function (_plugin, $identifyContent, options) {

};