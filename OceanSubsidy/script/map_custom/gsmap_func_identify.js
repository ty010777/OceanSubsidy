gsmap = typeof gsmap === "undefined" ? {} : gsmap;
gsmap.func = gsmap.func || {};

/**
* gsmap.func.Identify construction options
* @typedef {{mapUI:(gsmap.ui.MapUI)}} gsmap.func.Identify_Options
* @property {gsmap.ui.MapUI} mapUI 圖台ui module
*/


/**
 *
 * 結構<![CDATA[
 * <div class=".gsmap-bottom-identify">
 *     identify內容
 * </div>
 * ]]>
 * @param {ol.MapPlugin} plugin MapPlugin instance
 * @param {gsmap.ui.IdentifyUI_Options} options module of bottom UI
 */
gsmap.func.Identify = function (plugin, options) {
    var mapUI = options.mapUI;
    var bottomUI = mapUI.getBottomUI();
    var $identifyContent = mapUI.getBottomUI().getContent$("tab-identify");
    var jQueryIDInvalidChar = /([!"#$%&'()*+,./:;<=>?@\[\\\]^`{|}~])/g;
    var spatialResultLayerName = "spatialQuery";
    var pointStyleOpt = {
        icon: {
            anchor: [0.5, 32],
            anchorXUnits: 'fraction',
            anchorYUnits: 'pixels',
            src: "App_Themes/map/images/location.png"
        }
    };

    // identify開始
    var onIdentifyStart = function () {
        $identifyContent.find(".BubleTable").children().remove();
        $identifyContent.find(".BubleTable").append('<tr><td><img src="App_themes/Map/images/loading-balls.gif"></td></tr>');
    };

    // identify結束
    var onIdentifyFinish = function (coord, featureFromIdentify) {
        if ($.isEmptyObject(featureFromIdentify)) {
            $identifyContent.find(".BubleTable").children().remove();
            $identifyContent.find(".BubleTable").append('<tr><td>無資料</td></tr>');
            renderCoordInfo(coord);
        } else {
            $identifyContent.find(".BubleTable > tbody > tr").last().remove();
            bottomUI.switchTo("tab-identify");
        }
        bottomUI.setBottomHeightLevel(1);
    };

    // 各別Layer完成identify
    var onLayerIdentifyFinish = function (layerInfo, features) {
        if (features.length === 0) return;
        var layerID = plugin.layersMg.getLayerID(layerInfo);
        var layerContent = '<tr><th>' + layerInfo.name + '</th></tr>'
            + '<tr><td id="identity_' + $('<div/>').text(layerID).html() + '" ></td></tr>';
        $identifyContent.find(".BubleTable > tbody").children().last().before(layerContent);
        var $layerContent = $identifyContent.find("#identity_" + layerID.replace(jQueryIDInvalidChar, "\\$1"));
        //featureFromIdentify[layerInfo.layerID] = {};
        for (var i = 0; i < features.length; i++) {
            $layerContent.append(plugin.layersMg.genFeatureBubbleContent(layerInfo, features[i]));
        }
    };

    plugin.layersMg.onIdentifyStart(onIdentifyStart);
    plugin.layersMg.onIdentifyFinish(onIdentifyFinish);
    plugin.layersMg.onLayerIdentifyFinish(onLayerIdentifyFinish);

    // 關閉bubbleInfo
    plugin.layersMg.setEnableBubbleInfo(false);

    //顯示坐標位置
    function renderCoordInfo(coord) {
        singleClickCoord = coord;
        var $coordPanel = bottomUI.getContent$("tab-coord");

        var latlon = ol.proj.transform(singleClickCoord, plugin.getMapProj(), "EPSG:4326");
        var twd97 = ol.proj.transform(singleClickCoord, plugin.getMapProj(), "EPSG:3826");
        var twd67 = ol.proj.transform(singleClickCoord, plugin.getMapProj(), "EPSG:3828");

        var $tdWGS84 = $coordPanel.find("._tdWGS84");
        var $tdTWD97 = $coordPanel.find("._tdTWD97");
        var $tdTWD67 = $coordPanel.find("._tdTWD67");

        $tdWGS84.text(round(latlon[1], 4) + ", " + round(latlon[0], 4));
        $tdTWD97.text(Math.round(twd97[0]) + ", " + Math.round(twd97[1]));
        $tdTWD67.text(Math.round(twd67[0]) + ", " + Math.round(twd67[1]));
        var googleCoord = "http://maps.google.com/maps?f=q&source=s_q&z=16&layer=c&cbll=" + latlon[1] + "," + latlon[0];
        $coordPanel.find("._btnGoogle").attr(
            'onclick',
            'parent.window.open("' + googleCoord + '")'
        );

        plugin.clearFeatures(spatialResultLayerName);
        plugin.clearFeatures("default");
        var thepoint = twd97;
        plugin.addMarker(thepoint, null, pointStyleOpt);
        bottomUI.switchTo("tab-coord");
    }

    plugin.layersMg.activeIdentify();
};
