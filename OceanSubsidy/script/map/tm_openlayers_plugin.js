/*@license
===============================================================================
GEOSENSE Digital Technologies Inc. openlayers plugin -- oltmx

Copyright © 2015-2023 GEOSENSE Digital Technologies Inc. All rights reserved.
===============================================================================
*/
function loadjsfile(filename, filetype) {
    var fileref = document.createElement('script');
    fileref.setAttribute("type", "text/javascript");
    fileref.setAttribute("src", filePath);
    if (typeof fileref !== "undefined")
        document.getElementsByTagName("head")[0].appendChild(fileref);
}

function loadcssfile(filePath) {
    var fileref = document.createElement("link");
    fileref.setAttribute("rel", "stylesheet");
    fileref.setAttribute("type", "text/css");
    fileref.setAttribute("href", filePath);
    if (typeof fileref !== "undefined")
        document.getElementsByTagName("head")[0].appendChild(fileref);
}

/**
* openlayers7 TM extention
* @namespace
*/
var oltmx = {};


/**
* @namespace
*/
oltmx.util = {};
/**
* @summary 常用工具
* @class
*/
oltmx.util.Tools = {};

/**
* @summary 將geometry從srcPrj的坐標系統轉成圖台的坐標系統
* @param {ol.Map} map 圖台
* @param {ol.geom.Geometry} geom 任何繼承ol.geom.Geometry的空間幾何
* @param {string|ol.proj.Projection} srcPrj `geom`的坐標系統
* @return {ol.geom.Geometry} clone `geom` 然後將空間資料轉成圖台使用的坐標系統
* @private
*/
oltmx.util.Tools.toMapGeom = function (map, geom, srcPrj) {
    return geom.clone().transform(srcPrj, map.getView().getProjection());
};

/**
* @summary 從圖層管理資訊中取得指定的圖層資訊
* @param {string|entity.LayerInfo} layerInfoOrID layerID 或 layerInfo
* @param {entity.LayerInfo} layersObj 圖層管理資訊
* @return {entity.LayerInfo} 指定的圖層資訊
* @private
*/
oltmx.util.Tools.getLayerInfo = function (layerInfoOrID, layersObj) {
    if (!layerInfoOrID) {
        return layersObj;
    }
    if (typeof layerInfoOrID !== "string") {
        return layerInfoOrID;
    }
    var path = layerInfoOrID.split("/");
    if (path.length === 0) return layersInfo;
    var joinToken = '"].sub["';
    var evalPath = '["' + path.join(joinToken) + '"]';

    try {
        return eval("layersObj" + evalPath);
    } catch (e) {
        return null;
    }
};

/*
* 可參考ol.style.Style的建構options
*       但為了跟ol3只有低耦合度，所以建構的options，也不須new ol3的class，<br/>
*       只取其所有的建構options <br/>
*       如，產生circle style 的 options<br/>
*/

/**
* @summary 利用config產生ol.style.Style
* @desc      
*     {
*         radius: 10,
*         fill: {color:"#00FF00"}, // ol.style.Fill的建構options
*         stroke: {color:"#FF0000", width: 3} // ol.style.Storke的建構options
*     }
*       
* @param {object} config options for style  
*                        config.icon : ol.style.Icon 的建構 options  
*                        config.circle : ol.style.Circle 的建構 options  
*                        config.regularShape : ol.style.RegularShape 的建構 options  
*                        **icon, circle, regularShape 只可以三擇一**  
*                        config.fill : ol.style.Fill 的建構 options  
*                        config.stroke : ol.style.Stroke 的建構 options
* @param {ol.Feature} feature 圖徵物件
* @return {ol.style.Style} 產生的Style
*/
oltmx.util.Tools.getStyleFromOption = function (config, feature) {
    if (!config) {
        return null;
    }
    if (typeof (config) === "function") {
        return config;
    }
    var pointStyle = null;
    var textStyle = null;
    if (config.icon) {
        pointStyle = new ol.style.Icon(config.icon);
    }
    if (config.circle) {
        pointStyle = new ol.style.Circle(
            {
                radius: config.circle.radius,
                fill: new ol.style.Fill(config.circle.fill),
                stroke: new ol.style.Stroke(config.circle.stroke)
            }
        );
    }
    if (config.regularShape) {
        pointStyle = new ol.style.RegularShape(
            {
                points: config.regularShape.points,
                radius: config.regularShape.radius,
                radius1: config.regularShape.radius1,
                radius2: config.regularShape.radius2,
                angle: config.regularShape.angle,
                rotation: config.regularShape.rotation,
                fill: new ol.style.Fill(config.regularShape.fill),
                stroke: new ol.style.Stroke(config.regularShape.stroke)
            }
        );
    }
    if (config.chart) {
        pointStyle = new ol.style.Chart(
            {
                type: config.chart.type,
                radius: config.chart.radius,
                offsetY: config.chart.offsetY,
                data: config.chart.data,
                colors: config.chart.colors,
                rotateWithView: config.chart.rotateWithView,
                animation: config.chart.animation,
                stroke: new ol.style.Stroke(config.chart.stroke)
            }
        );
    }
    if (config.text) {
        var textOptions = JSON.parse(JSON.stringify(config.text));
        textOptions.fill = config.text.fill ? new ol.style.Fill(config.text.fill) : undefined;
        textOptions.stroke = config.text.stroke ? new ol.style.Stroke(config.text.stroke) : undefined;
        if (config.text.labelField
            && feature) {
            textOptions.text = feature.get(config.text.labelField);
        } else if (config.text.labelTemplate
            && feature) {
            textOptions.text = oltmx.util.Tools.genContentFromFeature(feature, config.text.labelTemplate);
        } else {
            textOptions.text = config.text.text || "";
        }
        textStyle = new ol.style.Text(textOptions);
    }

    return new ol.style.Style({
        fill: (config.fill ? new ol.style.Fill(config.fill) : null),
        stroke: (config.stroke ? new ol.style.Stroke(config.stroke) : null),
        image: pointStyle,
        text: textStyle,
        zIndex: config.zIndex
    });
};

oltmx.util.Tools.regexTemplateVar = /(\{)([^}]+)(\})/ig;
/**
* @summary 依據template產生feature的內容
* @param {ol.Feature} feature 圖徵
* @param {string} templateContent 內容的template
* @return {string} 產生的內容
*/
oltmx.util.Tools.genContentFromFeature = function (feature, templateContent) {
    var htmlContent = templateContent;
    var fieldNames = feature.getKeys();
    oltmx.util.Tools.regexTemplateVar.lastIndex = 0;
    var searchResult;
    var keys = feature.getKeys();
    var lowerCaseKeys = [];
    for (var i = 0; i < keys.length; i++) {
        lowerCaseKeys.push(keys[i].toLowerCase());
    }
    while (true) {
        searchResult = oltmx.util.Tools.regexTemplateVar.exec(htmlContent);
        if (!searchResult) break;
        try {
            var matchText = searchResult[2];
            var funcIdx = matchText.indexOf("fn:") + 3;
            var replaceText = null;
			
            if (funcIdx > 2) {
                replaceText = eval(matchText.substr(funcIdx))(feature);
            } else if (matchText === "$Id") {
                replaceText = feature.getId();
            } else {
                matchText = keys[lowerCaseKeys.indexOf(matchText.toLowerCase())];
                replaceText = feature.get(matchText);
            }
			
            if (replaceText === null || replaceText === undefined) {
                replaceText = "";
            }
			
			// make sure repleaceText is string
			replaceText = replaceText + "";
			
            htmlContent = htmlContent.substr(0, searchResult.index)
                + replaceText
                + htmlContent.substr(searchResult.index + searchResult[0].length);
            oltmx.util.Tools.regexTemplateVar.lastIndex = searchResult.index + replaceText.length;
        } catch (e) {
            oltmx.util.Tools.regexTemplateVar.lastIndex = searchResult.index + searchResult[0].length;
        }
    }
    return htmlContent;
};

oltmx.util.Tools.approxResolutionEqual = function (res1, res2) {
    var maxRes = res2;
    if (res1 > res2) {
        maxRes = res1;
    }
    if ((Math.abs(res1 - res2) / maxRes) < 0.000001) {
        return true;
    } else {
        return false;
    }
};

/**
* @summary 將經緯度坐標(float array)轉成度分秒的文字
* @param {float[]} latlon - 指定的圖層
* @param {string} separator 度分秒的分隔字串，如果是null，則為", "
* @param {string[]} arUnits 要顯示度分秒的方式，如果是null，則為["°", "'", "''"]
* @return {string} 以度分秒顯示經緯度
*/
oltmx.util.Tools.toDMSLatLon = function (latlon, separator, arUnits) {
    if (!arUnits || !$.isArray(arUnits) || arUnits.length !== 3) {
        arUnits = ["°", "'", "''"];
    }
    if (!separator) {
        separator = ", ";
    }
    var ret = "";
    var val = latlon[1];
    for (var i = 0; i < arUnits.length; i++) {
        ret += Math.floor(val) + arUnits[i];
        val = (val - Math.floor(val)) * 60;
    }
    ret += separator;
    val = latlon[0];
    for (var i = 0; i < arUnits.length; i++) {
        ret += Math.floor(val) + arUnits[i];
        val = (val - Math.floor(val)) * 60;
    }
    return ret;
};

oltmx.util.Tools.mergeOptiions = function (targetOptions, fromOptions) {
    for (var prop in fromOptions) {
        if (targetOptions[prop] === undefined) {
            targetOptions[prop] = fromOptions[prop];
        } else if (typeof targetOptions[prop] === "object") {
            if (typeof fromOptions[prop] === "object") {
                oltmx.util.Tools.mergeOptiions(targetOptions[prop], fromOptions[prop]);
            } else {
                targetOptions[prop] = fromOptions[prop];
            }
        } else {
            targetOptions[prop] = fromOptions[prop];
        }
    }
};

/*
* options = {
*     actives : [
*       {
*           id : "LayersMg",
*           params : {
*               mapLayers : map_Layers // 圖層設定JSON
*           }
*       },
*       {
*           id : "MeasureLengthTool"
*       }
*     }
*
* }
*/

/**
* openlayer3 TM Plugins.
* @class
* @constructor
* @param {ol.Map} _map The map.
* @param {object} options Options for controlling the operation.
*/
oltmx.Plugin = function (_map, options) {
    // 取坐標動作的左鍵事件ID
    var LEFT_CLICK_FUNC_PLUGIN_GET_COORDINATE = "plugin_get_coordinate";
    // 取Default的左鍵事件ID
    var LEFT_CLICK_FUNC_PLUGIN_DEFAULT = "plugin_default";
    // 選擇一個polygon的左鍵事件ID
    var LEFT_CLICK_FUNC_MAP_PLUGIN_SELECTPOLYGON = "map_plugin_selectPolygon";

    // ol3 map
    var map = _map;

    // 使用者現在位置
    var currentPosition;
    // 地理位置物件
    var geolocation;

    // default 環境變數
    var pluginOptions = {
        // 圖台顯示的坐標系統
        displayPrj: "EPSG:3826",

        // 初始位置
        // 坐標系統為displayPrj
        initPosition: [194188, 2597712],
        initZoom: 7,

        proxyUrlTemplate: "service/proxy2.ashx?url={url}",
        //var htmlCaptureService : "service/GenImageFromHtml.ashx";
        htmlCaptureService: "service/RegularPoxy.ashx",

        // 定位到點時，zoom到此level
        locatingPointZoom: 16,

        // 是否停用Plugin裡的ui輸出
        enableUI: false,

        // how to show coordinates on the map
        //coordinateTemplate: "TWD97：{x}, {y}",

        // if false, measure is done in simple way on projected plane. Earth curvature is not taken into account
        useGeodesicMeasures: true,

        startAreaMeasureMsg: "請點選起始點",

        getCoordinateHelpMsg: "請選取要取得坐標的位置",

        /**
        * Message to show when the user is drawing a polygon.
        * @type {string}
        */
        continueAreaMeasureMsg: '雙擊左鍵結束量測',

        startLengthMeasureMsg: "請點選起始點",

        /**
        * Message to show when the user is drawing a line.
        * @type {string}
        */
        continueLengthMeasureMsg: '雙擊左鍵結束量測',

        /**
        * 如果serviceInfo的bubbleContent設成這個值，將自動列出所有欄位
        * @type {string}
        */
        constAutoGenBubbleInfo: "#AUTO#",

        /*
        * 預設的bubble info layout
        * 必須有css="close"的dom，以實現關閉的動作
        */
        defaultBubbleContainerHTML: '<div class="ol-popup GSMap-element-popup-detail-wrap">'
                + '<div class="BtnFunc btnBoxRight" style="float: right">'
                + '    <input type="button" value="X" class="btnS btn_transparent close">'
                + '</div>'
                + '<div class="MapBubbleStyle gs-content">'
                + '</div>'
                + '</div>',

        // 預設圖層樣式
        defaultStyle: {
            stroke: { color: 'blue', width: 1 },
            fill: { color: 'rgba(255, 255, 0, 0.1)' },
            icon: {
                anchor: [0.5, 16],
                anchorXUnits: 'fraction',
                anchorYUnits: 'pixel',
                opacity: 0.9,
                src: 'images/map_legend/default.png'
            }
        }
    };

    oltmx.util.Tools.mergeOptiions(pluginOptions, options);

    displayPrj = options.displayPrj;

    var activedTools = [];
    var me = this;

    /**
     * 長度量測工具
     * @type oltmx.measure.MeasureLength
     */
    this.measureLengthTool = null;
    /**
     * 面積量測工具
     * @type oltmx.measure.MeasureArea
     */
    this.measureAreaTool = null;
    /**
     * 定位工具
     * @type oltmx.util.Locate
     */
    this.locateTool = null;
    /**
     * 圖層管理工具
     * @type oltmx.LayersManager
     */
    this.layersMg = null;
    this.editorTool = null;

    // layer groups
    var baseLayerGroup = new ol.layer.Group({ zIndex: 10 });
    var imageGroup = new ol.layer.Group({ zIndex: 20 });
    var polygonGroup = new ol.layer.Group({ zIndex: 30 });
    var polylineGroup = new ol.layer.Group({ zIndex: 40 });
    var pointGroup = new ol.layer.Group({ zIndex: 50 });
    var unknownTypeGroup = new ol.layer.Group({ zIndex: 60 });
    var measureGroup = new ol.layer.Group({ zIndex: 1000 });

    // 所有臨時向量圖層
    var vectorLayers = {};
    var vectorSources = {};
    var vectorFeatureOriStyle = {};
    var nextFeatureIDs = { "default": 0 };

    /**
    * @summary 啟動特定tool
    * @desc
    *  啟動的function name必須存在 "active" + toolID  
    *  如 toolID = "MeasureLengthTool"， 
    *  則會透過function activeMeasureLengthTool()來啟動長度測量功能  
    * @param {string} toolID 
    *                 Plugin 必須存function名稱為 active+`toolID`
    * @param {Object} params
    *                 呼叫active+`toolID`時，需要給的參數
    * @return {string} 如果發生錯誤時，會回傳訊息；如果成功，則回傳空字串
    * @private
    */
    this.active = function (toolID, params) {
        var errMsg = "";
        if (eval("this.active" + toolID + "Tool")) {
            if (activedTools.indexOf(toolID) < 0) {
                eval("this.active" + toolID + "Tool").bind(me)(params || {});
                activedTools.push(toolID);
            }
        } else {
            errMsg += "不支援[" + toolID + "]的功能";
        }
        return errMsg;
    };

    map.addLayer(baseLayerGroup);
    map.addLayer(imageGroup);
    map.addLayer(polygonGroup);
    map.addLayer(unknownTypeGroup);
    map.addLayer(polylineGroup);
    map.addLayer(pointGroup);
    map.addLayer(measureGroup);

    /**
     * @summary 回傳環境變數
     * @param {string} varName 環境變數名稱
     * @return {any} 環境變數
     */
    this.getOption = function (varName) {
        return pluginOptions[varName];
    };

    /**
     * @summary 取得關聯的map instance
     * @return {ol.Map} map instance
     */
    this.getMap = function () { return map; };


    /*
    * @summary 取得指定的圖層群組
    * @desc
    * groupID could be following: <br/>
    * "baseMap" <br/>
    * "image" <br/>
    * "polygon" <br/>
    * "polyline" <br/>
    * "point" <br/>
    * "unknownType" : 未知型態的圖層 <br/>
    * "measure" : 量測用的圖徵 <br/>
    * @param {string} groupID 圖層群組ID
    * @return {ol.layer.Group} 圖層群組
    */
    this.getLayerGroup = function (groupID) {
        return eval(groupID + "Group");
    };

    /*
    * @summary 取得未知型態的圖層
    * @returns {String} 圖層群組ID
    */
    this.getUnknowTypeGroup = function () {
        return unknownTypeGroup;
    };

    /*
    * @summary 取得底圖圖層群組
    * @desc 底圖的圖層都要放這
    * @return {ol.layer.Group} 圖層群組
    */
    this.getBaseLayerGroup = function () {
        return baseLayerGroup;
    };

    /*
     * 產生透過proxy連線指定url的proxy url
     * @param {string} url 目標url
     * @return {string} 連線目標url的proxy url
     */
    this.genProxyUrl = function (url) {
        return this.getOption("proxyUrlTemplate").replace("{url}", encodeURIComponent(url));
    };

    //================================================
    //  map utility
    //================================================

    /**
    * @summary 將feature的coordinates從指定的坐標系統轉換成圖台的坐標系統
    * @param {ol.Feature} feature 圖徵
    * @param {ol.proj.Projection} srcPrj 圖徵坐標系統
    */
    this.setFeatureCoords = function (feature, srcPrj) {
        feature.setCoordinates(this.transCoordinateToMapPrj(feature.getCoordinates(), srcPrj));
    };

    /**
    * @summary 轉換坐標成圖台的坐標系統
    * @param {ol.Coordinate|ol.Coordinate[]} coords 一個或多個坐標
    * @param {string|ol.proj.Projection} srcPrj 坐標的坐標系統
    * @return {ol.Coordinate|ol.Coordinate[]} 轉換後的坐標
    */
    this.transCoordinateToMapPrj = function (coords, srcPrj) {
        if (!ol.proj.Projection.prototype.isPrototypeOf(srcPrj)) {
            srcPrj = ol.proj.get(srcPrj);
        }
        targetPrj = map.getView().getProjection();

        if ($.isArray(coords[0])) {
            var ret = [];
            for (var idx = 0; idx < coords.length; idx++) {
                ret.push(this.transCoordinateToMapPrj(coords[idx], srcPrj));
            }
            return ret;
        } else {
            return ol.proj.transform(coords, srcPrj, targetPrj);
        }
    };

    /**
    * @summary 將圖台的坐標轉換成特定坐標系統
    * @param {ol.Coordinate|ol.Coordinate[]} coords 一個或多個圖台坐標
    * @param {string|ol.proj.Projection} targetPrj 要轉換成的坐標系統
    * @return {ol.Coordinate|ol.Coordinate[]} 轉換後的坐標
    */
    this.transCoordinateFromMapPrj = function (coords, targetPrj) {
        if (!ol.proj.Projection.prototype.isPrototypeOf(targetPrj)) {
            targetPrj = ol.proj.get(targetPrj);
        }
        srcPrj = map.getView().getProjection();

        if ($.isArray(coords[0])) {
            var ret = [];
            for (var idx = 0; idx < coords.length; idx++) {
                ret.push(this.transCoordinateToMapPrj(coords[idx], targetPrj));
            }
            return ret;
        } else {
            return ol.proj.transform(coords, srcPrj, targetPrj);
        }
    };

    /**
    * @summary 轉換bbox成地圖的坐標系統
    * @param {ol.Extent} extent bbox
    * @param {string|ol.proj.Projection} srcPrj bbox的坐標系統
    * @return {ol.Extent} 轉換後的bbox
    */
    this.transExtentToMapPrj = function (extent, srcPrj) {
        return ol.proj.transformExtent(extent, srcPrj, map.getView().getProjection());
    };

    /**
    * @summary 轉換bbox從圖台的坐標系統轉成指定的坐標系統
    * @param {ol.Extent} extent 圖台的bbox
    * @param {string|ol.proj.Projection} targetPrj 要轉換成的坐標系統
    * @return {ol.Extent} 轉換後的bbox
    */
    this.transExtentFromMapPrj = function (extent, targetPrj) {
        return ol.proj.transformExtent(extent, map.getView().getProjection(), targetPrj);
    };


    var getCoordinateHelpTipElement = null;
    var getCoordinateHelpTip = null;
    var getCoordinateTargetPrj = null;
    var getCoordinateCallback = null;
    var moveHandlerForGettingPoint = function (evt) {
        if (evt.dragging) {
            return;
        }
        if (!getCoordinateHelpTipElement) {
            getCoordinateHelpTipElement = document.createElement('div');
            getCoordinateHelpTipElement.className = 'tooltip';
            getCoordinateHelpTip = new ol.Overlay({
                element: getCoordinateHelpTipElement,
                offset: [15, 0],
                positioning: 'center-left'
            });
            getCoordinateHelpTipElement.innerHTML = me.getOption("getCoordinateHelpMsg");
        }
        map.addOverlay(getCoordinateHelpTip);

        var tooltipCoord = evt.coordinate;
        getCoordinateHelpTip.setPosition(evt.coordinate);
    };

    var singleClickForGettingPoint = function (evt) {
        if (me.currentLeftClickFunc() !== LEFT_CLICK_FUNC_PLUGIN_GET_COORDINATE) {
            return;
        }
        var _callback = getCoordinateCallback;
        var _targetPrj = getCoordinateTargetPrj;

        cancelGetCoordinate();

        _callback(ol.proj.transform(evt.coordinate, map.getView().getProjection(), _targetPrj));
    };

    /**
    * @callback singleClickForGettingPoint_Callback
    * @summary 取得地圖坐標的callback
    * @param {ol.Coordinate} _coord 坐標
    */

    /**
    * @summary 啟動從圖台上取得點坐標的作業
    * @param {singleClickForGettingPoint_Callback} _callback 取得地圖坐標後的callback
    * @param {string|ol.proj.Projection} _targetPrj 取得坐標後，要轉換成的坐標系統 <br/>
    *                                    如果targetPrj未設定，則回傳 displayPrj 的坐標系統
    */
    this.getCoordinate = function (_callback, _targetPrj) {
        if (typeof _callback !== "function") throw new Error("oltmx.util.Tools.getCoordinate 參數必須有callback function");
        map.on("pointermove", moveHandlerForGettingPoint);
        // 擋掉別的SingleClickEvent
        me.activeLeftClickFunc(LEFT_CLICK_FUNC_PLUGIN_GET_COORDINATE);
        map.on("singleclick", singleClickForGettingPoint);
        getCoordinateCallback = _callback;
        getCoordinateTargetPrj = _targetPrj ? _targetPrj : displayPrj;
    };

    /**
    * @summary 取消從圖台上取點坐標的作業
    */
    this.cancelGetCoordinate = function () {
        getCoordinateCallback = null;
        getCoordinateTargetPrj = null;
        map.un("pointermove", moveHandlerForGettingPoint);
        // 恢復別的SingleClickEvent
        map.un("singleclick", singleClickForGettingPoint);
        me.deactiveLeftClickFunc(LEFT_CLICK_FUNC_PLUGIN_GET_COORDINATE);
        map.removeOverlay(getCoordinateHelpTip);
    };
    var cancelGetCoordinate = this.cancelGetCoordinate;

    var selectPolygonDrawInteraction = null;

    /**
    * @summary 啟動取得一個使用者自行點選出來的polygon。
    * @param {function} _callback 使用者選取polygon後，回傳給此callback function
    * @param {String} _targetPrj 指定回傳polygon的坐標系統
    * @private
    */
    this.selectPolygon = function (_callback, _targetPrj) {
        if (typeof _callback !== "function") throw new Error("");
        if (selectPolygonDrawInteraction === null) {
            selectPolygonDrawInteraction = new ol.interaction.Draw({
                source: vectorSources["sys"],
                type: 'Polygon',
                style: new ol.style.Style({
                    image: new ol.style.Circle({
                        radius: 5,
                        fill: new ol.style.Fill({ color: "#ffcc33" })
                    }),
                    stroke: new ol.style.Stroke({
                        color: "rgb(0, 0, 255)",
                        width: 2
                    }),
                    fill: new ol.style.Fill({
                        color: "rgba(0, 0, 125, 0.2)"
                    })
                })
            });
            selectPolygonDrawInteraction.on('drawstart',
                function (evt) {
                    vectorSources["sys"].clear();
                });
            selectPolygonDrawInteraction.on('drawend',
                function (evt) {
                    me.deactiveLeftClickFunc(LEFT_CLICK_FUNC_MAP_PLUGIN_SELECTPOLYGON);
                    map.removeInteraction(selectPolygonDrawInteraction);
                    //console.log("新增圖資");
                    var newFeature = evt.feature;
                    _callback(newFeature.getGeometry().transform(map.getView().getProjection(), _targetPrj));
                });
        }
        me.activeLeftClickFunc(LEFT_CLICK_FUNC_MAP_PLUGIN_SELECTPOLYGON);
        map.addInteraction(selectPolygonDrawInteraction);
    };

    /**
     * 取消選取polygon的動作
     * @private
     */
    this.cancelSelectPolygon = function () {
        if (selectPolygonDrawInteraction) {
            me.deactiveLeftClickFunc(LEFT_CLICK_FUNC_MAP_PLUGIN_SELECTPOLYGON);
            map.removeInteraction(selectPolygonDrawInteraction);
            vectorSources["sys"].clear();
        }
    };

    var dragBox = new ol.interaction.DragBox({
        condition: ol.events.condition.shiftKeyOnly,
        style: new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: 'orange'
            })
        })
    });

    var selectEnd = null;
    /**
    * @summary 啟動在圖台啟動選擇一個bbox
    * @param {selectBBox_Callback}  _callback 在圖台上選擇bbox後的作業.
    * @param {string} _targetPrj 選擇bbox後，bbox坐標轉換成此坐標系統.<br />
    *                 如果未指定，則使用 displayPrj 所設定的坐標系統
    */
    this.selectBBox = function (_callback, _targetPrj) {
        me.stopSelectBBox();
        map.addInteraction(dragBox);
        selectEnd = function (e) {
            //dragBox.dispatchEvent(new ol.DragBoxEvent(ol.DragBoxEventType.BOXSTART, mapBrowserEvent.coordinate));
            var info = [];
            var extent = dragBox.getGeometry().getExtent();
            if (!_targetPrj) _targetPrj = displayPrj;
            try {
                _callback(ol.proj.transformExtent(extent, map.getView().getProjection(), _targetPrj));
            } catch (err) {
                console.log(err);
            }
        };
        dragBox.on('boxend', selectEnd);
    };
    /**
    * @callback selectBBox_Callback
    * @summary 選擇bbox後的callback
    * @param {ol.Extent} bbox 所選擇的bbox
    */

    /** 
    * @summary 停止在圖台啟動選擇一個bbox的作業
    */
    this.stopSelectBBox = function () {
        map.removeInteraction(dragBox);
        if (selectEnd) {
            dragBox.un('boxend', selectEnd);
            selectEnd = null;
        }
    };

    function createVectorLayer(layerName) {
        if (!layerName) layerName = "default";
        vectorSources[layerName] = new ol.source.Vector();
        vectorLayers[layerName] = new ol.layer.Vector({
            source: vectorSources[layerName],
            projection: "EPSG:3857",
            zIndex: layerName === "default" ? 999 : 101,
            style: new ol.style.Style({
                stroke: new ol.style.Stroke({
                    color: "#336dff",
                    width: 2
                }),
                fill: new ol.style.Fill({
                    color: "rgba(167, 220, 245,0.4)"
                })
            })
        });
        nextFeatureIDs[layerName] = 0;
        map.addLayer(vectorLayers[layerName]);
    }

    /**
     * 設定指定臨時圖層的zIndex
     * @param {string} layerName 臨時圖層name
     * @param {int} zindex 臨時圖層的zIndex
     */
    this.setVectorLayerZIndex = function (layerName, zindex) {
        vectorLayers[layerName].setZIndex(zindex);
    };

    // create default vector layer
    createVectorLayer();
    var vectorLayer = vectorLayers["default"];

    // mapPlugin功能用
    createVectorLayer("sys");

    map.on('singleclick', function (evt) {
        if (me.currentLeftClickFunc() !== LEFT_CLICK_FUNC_PLUGIN_DEFAULT) {
            return;
        }
        var coord = evt.coordinate;
        var tolerance = map.getView().getResolution() * 5;
        var toleranceExtent = [coord[0] - tolerance, coord[1] - tolerance, coord[0] + tolerance, coord[1] + tolerance];
        var feature = null;
        var dist = 0;
        vectorLayer.getSource().forEachFeatureIntersectingExtent(toleranceExtent, function (f) {
            var fCoord = f.getGeometry().getCoordinates();
            if (feature === null) {
                feature = f;
                closestCoord = f.getGeometry().getClosestPoint(coord);
                dist = Math.pow(fCoord[0] - coord[0], 2) + Math.pow(fCoord[1] - coord[1], 2);
            } else {
                var newDist = Math.pow(fCoord[0] - coord[0], 2) + Math.pow(fCoord[1] - coord[1], 2);
                if (newDist < dist) {
                    dist = newDist;
                    feature = f;
                }
            }
        });
        if (feature && feature.get("content") && $.trim(feature.get("content")) !== "") {
            me.showBubble({ coord: evt.coordinate }, feature.get("content"));
        } else {
            me.hideBubble();
        }
    });

    /** 修改圖層設定值
     * @param {string} layerName 圖層名稱
     * @param {object} options 設定值
     * @private
     */
    this.setMarkerLayer = function (layerName, options) {
        var layer = vectorLayers[layerName];
        for (var key in options) {
            if (layer["set" + key]) {
                layer["set" + key](options[key]);
            }
        }
    };

    /**
     * @summary 加入一個點marker到臨時圖層
     * @param {ol.Coordinate|ol.geom.Geometry|ol.Feature} coords 點坐標, 或是 ol.geom.Geometry, 或是 ol.Feature
     * @param {string} htmlContent html內容，可顯示於bubble info
     * @param {object} styleOpt ol.style.Icon的建構options，或 getStyleFromOption 的 options
     * @param {string|ol.proj.Projection} srcProj 點坐標的坐標系統
     * @param {string} layerName marker所屬圖層
     * @return {ol.Feature} 此marker的feature
     */
    this.addMarker = function (coords, htmlContent, styleOpt, srcProj, layerName) {
        var feature;
        var geom;
        if (!layerName) layerName = "default";
        if (!vectorLayers[layerName]) {
            createVectorLayer(layerName);
        }
        if (!srcProj) srcProj = displayPrj;
        if ($.isArray(coords)) {
            coords = me.transCoordinateToMapPrj(coords, srcProj);
            geom = new ol.geom.Point(coords);
        } else if (ol.geom.Geometry.prototype.isPrototypeOf(coords)) {
            // 如果是geometry
            geom = coords.clone();
            geom = geom.transform(srcProj, map.getView().getProjection());
        } else if (ol.Feature.prototype.isPrototypeOf(coords)) {
            feature = coords.clone();
            feature.setGeometry(feature.getGeometry().transform(srcProj, map.getView().getProjection()));
            feature.setId(coords.getId());
        }

        if (!feature) {
            feature = new ol.Feature({
                geometry: geom,
                content: htmlContent
            });
        }

        if (!feature.getId()) {
            if (!nextFeatureIDs[layerName]) nextFeatureIDs[layerName] = 0;
            feature.setId(nextFeatureIDs[layerName]++);
        }
        if (ol.style.Style.prototype.isPrototypeOf(styleOpt)) {
            feature.setStyle(styleOpt);
        } else if (styleOpt && styleOpt.src) {
            feature.setStyle(new ol.style.Style({
                image: new ol.style.Icon(styleOpt)
            }));
        } else if (styleOpt) {
            feature.setStyle(me.getStyleFromOption(styleOpt, feature));
        }
        vectorSources[layerName].addFeature(feature);
        return feature;
    };

    var emptyStyle = new ol.style.Style({});

    /**
     * 隱藏臨時圖層的指定圖徵
     * @param {string | number} fid 圖徵ID
     * @param {any} layerName 臨時圖層名稱
     */
    this.hideMarker = function (fid, layerName) {
        if (!layerName) layerName = "default";
        var source = vectorSources[layerName];
        if (!vectorFeatureOriStyle[layerName]) {
            vectorFeatureOriStyle[layerName] = {};
        }
        var oriStyleContainer = vectorFeatureOriStyle[layerName];

        var feature = source.getFeatureById(fid);
        if (feature) {
            oriStyleContainer[feature.getId()] = feature.getStyle();
            feature.setStyle(emptyStyle);
        } else {
            console.error("Marker is not exists");
        }
    };

    /**
     * 顯示臨時圖層的指定圖徵
     * @param {string | number} fid 圖徵ID
     * @param {any} layerName 臨時圖層名稱
     */
    this.showMarker = function (fid, layerName) {
        if (!layerName) layerName = "default";
        var source = vectorSources[layerName];
        if (!vectorFeatureOriStyle[layerName]) {
            vectorFeatureOriStyle[layerName] = {};
        }
        var oriStyleContainer = vectorFeatureOriStyle[layerName];

        var feature = source.getFeatureById(fid);
        if (feature) {
            feature.setStyle(oriStyleContainer[feature.getId()]);
            delete oriStyleContainer[feature.getId()];
        } else {
            console.error("Marker is not exists");
        }
    };

    /**
    * @summary 從臨時圖層移除一個feature
    * @param {ol.Feature} feature 要移除的feature
    * @param {String} layerName 臨時圖層名稱
    */
    this.removeMarker = function (feature, layerName) {
        if (!layerName) layerName = "default";
        if (vectorLayers[layerName]) {
            vectorSources[layerName].removeFeature(feature);
        }
    };

    /**
    * @summary 定位到臨時圖層的一個圖徵
    * @param {string|number} featureID feature ID
    * @param {String} layerName 圖層名稱
    */
    this.locateToMarker = function (featureID, layerName) {
        if (!layerName) layerName = "default";
        var vectorSource = vectorSources[layerName];
        var feature = vectorSource.getFeatureById(featureID);
        if (feature === null) throw new Error("找不到此feature");
        this.locateTool.locateToFeature(feature);
        if (typeof me.getOption("locatingPointZoom") === "number" && ol.geom.Point.prototype.isPrototypeOf(feature.getGeometry())) {
            map.getView().setZoom(me.getOption("locatingPointZoom"));
        }
    };

    /**
     * @deprecated
     * @private
     */
    this.locateToFeature = this.lcoateToMarker;

    /**
    * @summary 取得指定的臨時圖層裡所有圖徵
    * @param {String} layerName 圖層名稱
    * @return {ol.Feature[]} 圖徵陣列
    */
    this.getAllMarker = function (layerName) {
        if (!layerName) layerName = "default";
        if (!vectorLayers[layerName]) return null;
        return vectorSources[layerName].getFeatures();
    };

    /**
     * 依照圖徵ID取得圖徵物件
     * @param {string | number} fid 圖徵ID
     * @param {any} layerName 臨時圖層名稱
     * @returns {ol.Feature} 圖徵物件
     */
    this.getMarkerById = function (fid, layerName) {
        if (!layerName) layerName = "default";
        if (!vectorLayers[layerName]) return null;
        return vectorSources[layerName].getFeatureById(fid);
    };

    /**
     * 取得臨時圖層的bbox
     * @param {any} layerName 臨時圖層名稱
     * @returns {ol.Extent} 臨時圖層的bbox
     */
    this.getMarkerLayerExtent = function (layerName) {
        if (!layerName) layerName = "default";
        if (!vectorLayers[layerName]) return null;
        return vectorSources[layerName].getExtent();
    };

    /**
    * @summary 從臨時圖層取得在extent內的feature
    * @param {ol.Extent} _extent bbox
    * @param {string|ol.proj.Projection} _extentPrj bbox的坐標系統
    * @param {String} layerName 臨時圖層名稱
    * @return {ol.Feature[]} 在extent內的features
    */
    this.getMarkerIntersectingExtent = function (_extent, _extentPrj, layerName) {
        if (!layerName) layerName = "default";
        if (!vectorSources[layerName]) return null;
        var extent = _extent;
        if (extent) {
            extent = me.transExtentToMapPrj(_extent, _extentPrj);
        }
        var result = [];
        vectorSources[layerName].forEachFeatureIntersectingExtent(extent, function (feature) { result.push(feature); });
        return result;
    };

    /**
     * @deprecated
     * @private
     */
    this.getFeatureIntersectingExtent = this.getMarkerIntersectingExtent;

    /**
    * @summary 清除臨時圖層
    * @param {String} layerName 臨時圖層名稱
    */
    this.clearMarkers = function (layerName) {
        if (!layerName) layerName = "default";
        if (!vectorLayers[layerName]) return;

        nextFeatureIDs[layerName] = 0;
        vectorSources[layerName].clear();
    };

    /**
     * @deprecated
     * @private
     */
    this.clearFeatures = this.clearMarkers;

    /**
    * @summary 回到起始位置
    * @description 回到 initPosition所設定的起始位置，<br/>
    *              如果 initPosition是點的話，會zoom到initZoom設定的zoom level
    */
    this.backToInitPosition = function () {
        var initPosition = me.getOption("initPosition");
        if (initPosition.length === 4) {
            var displayExtent = me.transExtentToMapPrj(initPosition, displayPrj);
            map.getView().fit(displayExtent, map.getSize());
        } else {
            map.getView().setCenter(me.transCoordinateToMapPrj(initPosition, displayPrj));
            map.getView().setZoom(me.getOption("initZoom"));
        }
    };

    /**
    * @summary 取得目前使用者位置
    * @return {ol.Coordinate} 坐標系統為 map.getView().getProjection()；如果無法取得位置，則回傳null。
    * @private
    */
    this.getUserPosition = function () {
        if (geolocation)
            return currentPosition;
        return null;
    };

    /**
    * @summary 啟動位置偵測
    * @description 啟動位置偵測，並設定位置顯示的樣式
    * @param {object} styleOpt 內含property - headingStyle與posStyle，headingStyle顯示方向的style，posStyle顯示現在位置的style
    * @param {int} zIndex 使用者圖徵的zIndex，預設為999999
    * @return {ol.Geolocation} Helper class for providing HTML5 Geolocation capabilities.
    * @private
    */
    this.activateGeolocation = function (styleOpt, zIndex) {
        var bShowPos = true;

        geolocation = new ol.Geolocation({
            projection: map.getView().getProjection(),
            tracking: true,
            trackingOptions: {
                enableHighAccuracy: true,
                maximumAge: 2000
            }
        });

        // 使用者方向樣式
        var headingStyle;

        if (styleOpt && styleOpt.headingStyle) {
            headingStyle = oltmx.util.Tools.getStyleFromOption(styleOpt.headingStyle);
        } else {
            headingStyle = new ol.style.Style({
                image: new ol.style.Icon({
                    anchor: [0.5, 1],
                    anchorXUnits: 'fraction',
                    anchorYUnits: 'fraction',
                    opacity: 1.0,
                    src: 'App_Themes/map/images/locateIcon.png'
                })
            });
        }

        // 使用者方向圖徵
        var headingFeature = new ol.Feature();
        headingFeature.setStyle(headingStyle);

        // 使用者位置樣式
        var posStyle;

        if (styleOpt && styleOpt.posStyle) {
            posStyle = oltmx.util.Tools.getStyleFromOption(styleOpt.posStyle);
        } else {
            posStyle = new ol.style.Style({
                image: new ol.style.Circle({
                    radius: 7,
                    fill: new ol.style.Fill({
                        color: '#3399FF'
                    }),
                    stroke: new ol.style.Stroke({
                        color: '#fff',
                        width: 3
                    })
                })
            });
        }

        // 使用者位置圖徵
        posFeature = new ol.Feature();
        posFeature.setStyle(posStyle);

        var iconSource = new ol.source.Vector({
            features: [posFeature, headingFeature]
        });
        var iconLayer = new ol.layer.Vector({
            source: iconSource,
            // 使用者位置應該在最上層，預設zIndex為999999。
            zIndex: zIndex || 999999
        });
        map.addLayer(iconLayer);

        geolocation.on('change', function () {
            var pos = geolocation.getPosition();
            currentPosition = pos;
            var ptPos = new ol.geom.Point(pos);
            posFeature.setGeometry(ptPos);
            var heading = geolocation.getHeading();
            // heading == null，表示瀏覽器不支援方向性
            // heading is NaN，表示尚未取得方向
            if (heading === null) {
                if (iconSource.getFeatures().indexOf(headingFeature) >= 0) iconSource.removeFeature(headingFeature);
            } else if (!isNaN(heading)) {
                if (iconSource.getFeatures().indexOf(headingFeature) < 0) iconSource.addFeature(headingFeature);
                headingFeature.getStyle.setRotation(heading);
                headingFeature.setGeometry(ptPos);
            }
        });
        return geolocation;
    };

    /**
    * @summary 定位到使用者位置，必須先啟動位置偵測，MapPlugin.activateGeolocation
    * @param {object} styleOpt 內含property - headingStyle與posStyle，headingStyle顯示方向的style，posStyle顯示現在位置的style
    * @private
    */
    this.locateToUserPosition = function () {
        if (!geolocation) {
            console.error("請先啟動Geolocation， MapPlugin.activateGeolocation()");
            return;
        }
        if (currentPosition) {
            if (currentPosition) {
                map.getView().setCenter(currentPosition);
            }
        } else {
            geolocation.once('change', cbLocateToUserPosition);
        }
    };

    // call back function for locateToUserPosition
    function cbLocateToUserPosition() {
        var pos = geolocation.getPosition();
        if (pos) {
            map.getView().setCenter(pos);
        }
    }

    // 由 oltmx.util.Tools.genContentFromFeature 取代
    /**
    * 依據feature與template產生對應內容
    */
    this.genContentFromFeature = oltmx.util.Tools.genContentFromFeature;

    /**
    * @summary zoom out 一個level
    */
    this.zoomOut = function () {
        if (map.getView().getZoom() === 0) return;
        map.getView().setZoom(map.getView().getZoom() - 1);
    };

    /**
    * @summary zoom in 一個level
    */
    this.zoomIn = function () {
        map.getView().setZoom(map.getView().getZoom() + 1);
    };

    /**
     * @summary 取得現在的zooom level
     * @return {int} zoom level
     */
    this.getZoom = function () {
        return map.getView().getZoom();
    };

    /**
    * @summary zoom 到指定的level
    * @param {int} zoomLevel 指定的 zoom level
    */
    this.setZoom = function (zoomLevel) {
        map.getView().setZoom(zoomLevel);
    };

    /**
    * @summary 加入map事件，參考ol.Map的on method
    */
    this.on = function () {
        var args = $.makeArray(arguments);
        args[2] = map;
        map.on.apply(map, args);
    };
    /**
    * @summary 加入map一次性事件，參考ol.Map的once method
    */
    this.once = function () {
        var args = $.makeArray(arguments);
        args[2] = map;
        map.once.apply(map, args);
    };
    /**
    * @summary 取消map事件，參考ol.Map的un method
    */
    this.un = function () {
        var args = $.makeArray(arguments);
        args[2] = map;
        map.un.apply(map, args);
    };

    /**
    * @summary 允許Plugin使用編輯功能
    * @private
    */
    this.activeEditorTool = function () {
        me.editorTool = new oltmx.editor.Editor(me);
    };

    // stack structure，堆疊目前左鍵點擊應執行的動作
    var leftClickFuncStack = [];
    /**
    * @summary
    * 加入指定的左鍵點選對應功能
    * @param {string} funcID 功能ID
    */
    this.activeLeftClickFunc = function (funcID) {
        this.deactiveLeftClickFunc(funcID);
        leftClickFuncStack.push(funcID);
    };
    /**
    * @summary 移除特定左鍵點選對應功能
    * @param {string} funcID 功能ID
    */
    this.deactiveLeftClickFunc = function (funcID) {
        var idx = leftClickFuncStack.indexOf(funcID);
        if (idx >= 0) {
            delete leftClickFuncStack[idx];
            leftClickFuncStack = leftClickFuncStack.slice(0, idx).concat(leftClickFuncStack.slice(idx + 1, leftClickFuncStack.length));
        }
    };
    /**
    * @summary 取得目前左鍵點選動作應該對應的功能ID
    * @desc 當圖台同時有不同的左鍵Event需要觸發，可讓左鍵事件判斷，現在是否屬於該功能需要啟動
    * @returns {String} 目前左鍵點選應該對應的功能ID
    */
    this.currentLeftClickFunc = function () {
        return leftClickFuncStack[leftClickFuncStack.length - 1];
    };

    // constructor
    {
        if (me.getOption('defaultBubbleContainerHTML')) {
            oltmx.Bubble.defaultBubbleContainerHTML = me.getOption('defaultBubbleContainerHTML');
        }

        if (pluginOptions) {
            if (pluginOptions.actives) {
                var errMsg = "";
                for (var i = 0; i < pluginOptions.actives.length; i++) {
                    errMsg += (errMsg === "" ? "" : "\n")
                        + this.active(pluginOptions.actives[i].id, pluginOptions.actives[i].params);
                }
                if (errMsg !== "") alert(errMsg);
            }
        }
        me.activeLeftClickFunc(LEFT_CLICK_FUNC_PLUGIN_DEFAULT);

        me.backToInitPosition();
    }

};

/**
* @summary 取得圖台的坐標系統
* @return {ol.proj.Projection} 坐標系統物件
*/
oltmx.Plugin.prototype.getMapProj = function () {
    return this.getMap().getView().getProjection();
};

/**
* @summary 地圖畫面截圖
* @desc 需要web service配合  
*       web service網址設定於變數 htmlCaptureService
* @param {float}  startCoord  The start coordinate.
* @param {int}  imgSize     Size of the image.
* @param {string}  imgFormat   The image format. png/jpg
* @param {object} param [Optional]發出request使用的額外參數
* @param {...captureMap_Callback}  _callback   The callback.
* @private
*/
oltmx.Plugin.prototype.captureMap = function (startCoord, imgSize, imgFormat) {
    var theMap = this.getMap();
    var params = null;
    var _callback = null;
    if (typeof (arguments[3]) === "function") {
        _callback = arguments[3];
    } else {
        params = arguments[3];
        _callback = arguments[4];
    }
    if (!htmlCaptureService) {
        throw new Error("未設定 capture service 的位址於變數 htmlCaptureService");
    }
    var mapSize = theMap.getSize();
    var mapExtent = this.get2DExtent();
    var canvasData = {};
    var canvases = $(theMap.getViewport()).find("canvas");
    for (var idx = 0; idx < canvases.length; idx++) {
        canvasData[idx] = canvases[idx + ""].toDataURL();
    }

    var reqData = {
        content: theMap.getViewport().outerHTML,
        format: imgFormat,
        fullwidth: mapSize[0],
        fullHeight: mapSize[1],
        imgWidth: imgSize[0],
        imgHeight: imgSize[1],
        startX: Math.round(startCoord[0]),
        startY: Math.round(startCoord[1]),
        baseUri: location.toString(),
        minX: mapExtent[0],
        minY: mapExtent[1],
        maxX: mapExtent[2],
        maxY: mapExtent[3],
        rotation: 180 * (map.getView().getRotation() / Math.PI),
        canvasData: canvasData
    };
    if (params && typeof params === "object") {
        for (var key in params) {
            reqData[key] = params[key];
        }
    }
    $.ajax({
        url: htmlCaptureService,
        method: "POST",
        data: reqData,
        success: _callback,
        error: _callback
    });
};

/**
* 截圖產製完成後的callbck
* @callback captureMap_Callback
* @param {CaptureMapResult} _data 截圖產生service回傳的資料，內含影像連結
* @private
*/
/**
* 截圖web service回傳資訊
* @typedef {{imageURL:(string)}} CaptureMapResult
* @property {string} imageURL 截圖的影像連結
* @private
*/

/**
* @summary 以圖台中心取指定大小的截圖
* @param {ol.Size} imgSize 指定截圖大小
* @param {string} imgFormat 截圖影像格式png/jpg等
* @param {captureMap_Callback} _callback 截圖產製完成後，呼叫此function
* @private
*/
oltmx.Plugin.prototype.captureMapCenter = function (imgSize, imgFormat) {
    var theMap = this.getMap();
    if (!htmlCaptureService) {
        throw new Error("未設定 capture service 的位址於變數 htmlCaptureService");
    }
    var mapSize = theMap.getSize();
    var startPixel = [(mapSize[0] - imgSize[0]) / 2, (mapSize[1] - imgSize[1]) / 2];
    //var startCoord = theMap.getCoordinateFromPixel(startPixel);
    var params = null;
    var _callback = null;
    if (typeof (arguments[2]) === "function") {
        _callback = arguments[2];
        params = arguments[1];
    } else {
        params = arguments[2];
        _callback = arguments[3];
    }
    this.captureMap(startPixel, imgSize, imgFormat, params, _callback);
};

/**
* @summary 取得圖台2D的bbox
* @return {ol.Extent} bbox
*/
oltmx.Plugin.prototype.get2DExtent = function () {
    var theMap = this.getMap();
    return theMap.getView().calculateExtent(theMap.getSize());
};

/**
* @summary 在圖台上顯示一個bubble info
* @param {entity.CoordInfo} coordInfo 坐標資訊
* @param {string} htmlContent 顯示內容(html)
* @private
*/
oltmx.Plugin.prototype.showBubble = function (coordInfo, htmlContent) {
    var coord = coordInfo.coord;
    if (coordInfo.proj) {
        coord = ol.proj.transform(coord, coordInfo.proj, "EPSG:3857");
    }
    oltmx.Bubble.show(this.getMap(), coord, htmlContent);
};

/**
* @summary 隱藏圖台上的bubble info
* @private
*/
oltmx.Plugin.prototype.hideBubble = function () {
    oltmx.Bubble.hide(this.getMap());
};

/**
* @summary 利用config產生ol.style.Style
* @desc 可參考ol.style.Style的建構options
*       但為了跟ol3只有低耦合度，所以建構的options，也不須new ol3的class，<br/>
*       只取其所有的建構options <br/>
*       如，產生circle style 的 options<br/>
*       
*     {
*         radius: 10,
*         fill: {color:"#00FF00"}, // ol.style.Fill的建構options
*         stroke: {color:"#FF0000", width: 3} // ol.style.Storke的建構options
*     }
*       
* @param {object} config options for style  
*                        config.icon : ol.style.Icon 的建構 options  
*                        config.circle : ol.style.Circle 的建構 options  
*                        config.regularShape : ol.style.RegularShape 的建構 options  
*                        **icon, circle, regularShape 只可以三擇一**  
*                        config.fill : ol.style.Fill 的建構 options  
*                        config.stroke : ol.style.Stroke 的建構 options  
* @return {ol.style.Style} 產生的Style
* @private
*/
oltmx.Plugin.prototype.getStyleFromOption = oltmx.util.Tools.getStyleFromOption;

/*
* @summary 啟用畫圖徵之功能ol.interaction.Draw
* @desc 依照不同的圖徵型態繪製圖徵
* @param {string|object} drawType 圖徵型態
*                 drawType : 'Point', 'LineString', 'Polygon', 'MultiPoint', 'MultiLineString', 'MultiPolygon' or 'Circle'
* @param {StyleOption} styleOption 樣式設定
* @param {function} callback callback when draw end. function(feature, options). option = {drawType, styleOption}. return boolean: ture means to clear drawing feature.
*/
oltmx.Plugin.prototype.drawFeature = function (drawType, geometryFunction, styleOption, callback) {
    var options = {};
    if (arguments.length === 1
        && $.isPlainObject(arguments[0])) {
        options = arguments[0];
    } else {
        options.drawType = drawType;
        options.geometryFunction = geometryFunction;
        options.styleOption = styleOption;
        options.callback = callback;
    }

    if (options.drawType.toLowerCase() === "box") {
        options.drawType = "LineString";
        options.geometryFunction = function (coordinates, geometry) {
            if (!geometry) {
                geometry = new ol.geom.Polygon(null);
            }
            var start = coordinates[0];
            var end = coordinates[1];
            if (coordinates.length == 2) {
                geometry.setCoordinates([
                    [start, [start[0], end[1]], end, [end[0], start[1]], start]
                ]);
            }
            return geometry;
        };
        options.maxPoints = 2;
        options.freehand = true;
    }

    options.style = null;
    if (ol.style.Style.prototype.isPrototypeOf(options.styleOption)) {
        options.style = options.styleOption;
    } else if ($.isPlainObject(options.styleOption)) {
        options.style = oltmx.util.Tools.getStyleFromOption(options.styleOption)
    }

    var source;
    try {
        // 有版本直接使用到外部變數
        // 歷史共業，確定無人使用後移除
        source = drawSource;
    } catch (err) {
        source = new ol.Collection();
    }

    this.endFeatureMaintain();

    this.__gs__drawInteraction = new ol.interaction.Draw({
        source: source,
        type: options.drawType,
        geometryFunction: options.geometryFunction,
        maxPoints: options.maxPoints,
        stopClick: true,
        style: options.style,
        freehand: options.freehand
    });

    options.source = source;
    options.map = map;  //this.getMap();
    options.interaction = this.__gs__drawInteraction;

    this.__gs__drawInteraction.on(
        'drawend',
        function (evt) {
            evt.feature.setStyle(this.style);
            if (this.callback) {
                if (this.callback(evt.feature, this)) {
                    this.source
                        .once(
                            'addfeature',
                            function (evt) {
                                if (this.feature === evt.feature) {
                                    if (evt.target.removeFeature) {
                                        evt.target.removeFeature(evt.feature);
                                    } else if (evt.target.remove) {
                                        evt.target.remove(evt.feature);
                                    } else {
                                        console.warn("Source has no remove or removeFeature method.");
                                    }
                                }
                            }.bind({ feature: evt.feature })
                        );
                }
            }
        }.bind(options)
    );

    //this.getMap().addInteraction(this.__gs__drawInteraction);
    map.addInteraction(this.__gs__drawInteraction);
};

/*
* @summary 啟用修改圖徵功能
* @desc 於標註清單點選欲編輯的圖徵，並進行編輯
* @param {ol.Feature|ol.Collection|ol.source.Vector} 來源圖徵
* @param {function} callback when a feature is drawn end.
*/
oltmx.Plugin.prototype.modifyFeature = function (obj, callback) {
    this.endFeatureMaintain();

    var modifyOptions = {};
    if (ol.Feature.prototype.isPrototypeOf(obj)) {
        var modifyCollection = new ol.Collection();
        modifyCollection.push(obj);
        modifyOptions.features = modifyCollection;
    } else if (ol.Collection.prototype.isPrototypeOf(obj)) {
        modifyOptions.features = obj;
    } else if (ol.source.Vector.prototype.isPrototypeOf(obj)) {
        modifyOptions.source = obj;
    } else {
        throw new Error("未知的來源圖徵obj物件");
    }

    this.endFeatureMaintain();

    this.__gs__modifyInteraction = new ol.interaction.Modify(modifyOptions);
    modifyOptions.callback = callback;

    this.__gs__modifyInteraction.on(
        'drawend',
        function (evt) {
            if (this.callback) {
                this.callback(evt.features, this);
            }
        }.bind(modifyOptions)
    );

    map.addInteraction(this.__gs__modifyInteraction);
};

oltmx.Plugin.prototype.endFeatureMaintain = function () {
    if (this.__gs__drawInteraction) {
        map.removeInteraction(this.__gs__drawInteraction);
        this.__gs__drawInteraction = null;
    }
    if (this.__gs__modifyInteraction) {
        map.removeInteraction(this.__gs__modifyInteraction);
        this.__gs__modifyInteraction = null;
    }
};

/*
* @summary 設定點圖徵樣式
* @param {ol.ColorLike} pointColor 點顏色
* @param {number} pointScale 點放大比例
* @param {string} pointTxt label內容
* @param {ol.Feature} feature 圖徵
* @return {ol.style.Style} 產生的點圖徵樣式
*/
oltmx.Plugin.prototype.setPointMarkStyle = function (pointColor, pointScale, pointTxt, feature) {
    var pointStyleConfig = {
        text: {
            text: pointTxt,
            fill: {
                color: '#000000'
            },
            stroke: {
                width: 4,
                color: '#FFFFFF'
            },
            font: "12pt 微軟正黑體",
            offsetY: 25
        },
        circle: {
            radius: pointScale * 3,
            fill: {
                color: pointColor
            },
            stroke: {
                color: pointColor
            }
        }
    };
    return oltmx.util.Tools.getStyleFromOption(pointStyleConfig, feature);
};

/**
* @summary 設定點圖徵樣式-含圖示
* @return {ol.style.Style} 產生的點圖徵樣式
*/
oltmx.Plugin.prototype.setIconMarkStyle = function (pointColor, pointScale, pointTxt, iconSrc, Opacity, feature, offsetY) {
    var pointStyleConfig = {
        text: {
            text: pointTxt,
            fill: {
                color: '#FFFFFF'
            },
            stroke: {
                width: 4,
                color: '#000000'
            },
            font: "12pt 微軟正黑體",
            offsetY: offsetY
        },
        icon: {
            src: iconSrc,
            color: pointColor,
            scale: pointScale,
            opacity: Opacity
        }
    };
    return oltmx.util.Tools.getStyleFromOption(pointStyleConfig, feature);
};

/*
* @summary 設定線圖徵樣式
* @param {ol.ColorLike} lineColor 線顏色
* @param {number} lineWidth 線寬
* @param {string} lineTxt label內容
* @param {ol.Feature} feature 圖徵
* @return {ol.style.Style} 產生的線圖徵樣式
*/
oltmx.Plugin.prototype.setLineMarkStyle = function (lineColor, lineWidth, lineTxt, lineDash1, lineDash2, feature) {
    var lineStyleConfig = {
        text: {
            text: lineTxt,
            fill: {
                color: '#FFFFFF'
            },
            stroke: {
                width: 4,
                color: '#000000'
            },
            font: "12pt 微軟正黑體",
            offsetY: 20
        },
        stroke: {
            color: lineColor,
            width: parseInt(lineWidth),
            lineDash: [lineDash1, lineDash2]
        }
    };
    return oltmx.util.Tools.getStyleFromOption(lineStyleConfig, feature);
};

/**
* @summary 設定面圖徵樣式
* @param {ol.ColorLike} polyFillColor 填滿色
* @param {ol.ColorLike} polyStrokeColor 外框顏色
* @param {number} polyWidth 外框線寬
* @param {string} polyTxt label內容
* @param {ol.Feature} feature 圖徵
* @return {ol.style.Style} 產生的面圖徵樣式
*/
oltmx.Plugin.prototype.setPolyMarkStyle = function (polyFillColor, polyStrokeColor, polyWidth, polyTxt, lineDash1, lineDash2, feature) {
    var polyStyleConfig = {
        text: {
            text: polyTxt,
            fill: {
                color: '#FFFFFF'
            },
            stroke: {
                width: 4,
                color: '#000000'
            },
            font: "12pt 微軟正黑體",
            offsetY: 0,
            overflow: true
        },
        fill: {
            color: polyFillColor
        },
        stroke: {
            color: polyStrokeColor,
            width: parseInt(polyWidth),
            lineDash: [lineDash1, lineDash2]
        }
    };
    return oltmx.util.Tools.getStyleFromOption(polyStyleConfig, feature);
};

/*
* @summary 設定文字圖徵樣式
* @param {string} Text label內容
* @param {ol.ColorLike} textFillColor 填滿色
* @param {number} textWidth 外框線寬
* @param {ol.ColorLike} textStrokeColor 外框顏色
* @param {number} textScale 文字放大比例
* @param {ol.Feature} feature 圖徵
* @return {ol.style.Style} 產生的文字圖徵樣式
*/
oltmx.Plugin.prototype.setTextMarkStyle = function (Text, textFillColor, textWidth, textStrokeColor, textScale, feature, offsetY) {
    var textStyleConfig = {
        text: {
            text: Text,
            fill: {
                color: textFillColor
            },
            stroke: {
                width: parseInt(textWidth),
                color: textStrokeColor
            },
            font: textScale + "pt 微軟正黑體",
            offsetY: offsetY
        },
        icon: {
            src: 'images/map_legend/empty.png',
            opacity: 0
        }
    };
    return oltmx.util.Tools.getStyleFromOption(textStyleConfig, feature);
};

/**
* @summary 設定矩形圖徵樣式
* @return {ol.style.Style} 產生的矩形圖徵樣式
*/
oltmx.Plugin.prototype.setBoxMarkStyle = function (boxFillColor, boxStrokeColor, boxWidth, boxTxt, lineDash1, lineDash2, feature) {
    var boxStyleConfig = {
        text: {
            text: boxTxt,
            fill: {
                color: '#FFFFFF'
            },
            stroke: {
                width: 4,
                color: '#000000'
            },
            font: "12pt 微軟正黑體",
            offsetY: 0,
            overflow: true
        },
        fill: {
            color: boxFillColor
        },
        stroke: {
            color: boxStrokeColor,
            width: parseInt(boxWidth),
            lineDash: [lineDash1, lineDash2]
        }
    };
    return oltmx.util.Tools.getStyleFromOption(boxStyleConfig, feature);
};

/**
* @summary 設定圓形圖徵樣式
* @return {ol.style.Style} 產生的圓形圖徵樣式
*/
oltmx.Plugin.prototype.setCirMarkStyle = function (cirFillColor, cirStrokeColor, cirWidth, cirTxt, IsRadius, lineDash1, lineDash2, feature) {
    var cirStyleConfig = {
        IsRadius: IsRadius,
        text: {
            text: cirTxt,
            fill: {
                color: '#FFFFFF'
            },
            stroke: {
                width: 4,
                color: '#000000'
            },
            font: "12pt 微軟正黑體",
            offsetY: 0,
            overflow: true
        },
        fill: {
            color: cirFillColor
        },
        stroke: {
            color: cirStrokeColor,
            width: parseInt(cirWidth),
            lineDash: [lineDash1, lineDash2]
        }
    };
    return oltmx.util.Tools.getStyleFromOption(cirStyleConfig, feature);
};

/*
* @summary 設定鷹眼地圖
* @param {object} position for overviewmap  
*                        divBottom : 鷹眼圖底部位置 
*                        divLeft : 鷹眼圖靠左位置  
*                        divRight : 鷹眼圖靠右位置   
*                        layer : 鷹眼圖底圖
*                        elementOVMap : 鷹眼圖底圖的Element
*/
oltmx.Plugin.prototype.setOverviewMap = function (divBottom, divTop, divLeft, divRight, layer, elementOVMap) {
    var map = this.getMap();
    map.removeControl(this.overviewMapControl);
    if (layer != null) {
        this.overviewMapControl = new ol.control.OverviewMap({
            className: 'ol-overviewmap',
            layers: [layer],
            target: elementOVMap,
            collapsible: true,
            collapsed: false
        });
        map.addControl(this.overviewMapControl);

        elementOVMap.style.bottom = divBottom;
        elementOVMap.style.top = divTop;
        elementOVMap.style.left = divLeft;
        elementOVMap.style.right = divRight;
    }
};

oltmx.Plugin.prototype.getOverviewMap = function () {
    return this.overviewMapControl;
}

/*
* @summary 將圖徵匯出KML字串
* @param {object} Features Array
*                 Features : ex [ol.Feature, ol.Feature]
* @return {KML String} <kml xmlnx="">...</kml>
*/
oltmx.Plugin.prototype.export2KML = function (Features) {
    var kmlFormat = new ol.format.KML();
    var kmlString = kmlFormat.writeFeatures(Features, {
        dataProjection: 'EPSG:4326',
        featureProjection: 'EPSG:3857'
    });
    return kmlString;
};

/*
 * @summary [Deprecated] DO NOT USE IT.
 */
oltmx.Plugin.prototype.Geolocation = function () {

    var kwSource = new ol.source.Vector({
        wrapX: false
    });

    var kwVector = new ol.layer.Vector({
        zIndex: 9999,
        source: kwSource
    });

    kwSource.clear();
    var iconStyle = new ol.style.Style({
        image: new ol.style.Icon({
            anchor: [0.5, 100],
            anchorXUnits: 'fraction',
            anchorYUnits: 'pixels',
            opacity: 1.0,
            src: 'App_Themes/map/images/locateIcon.png'
        })
    });

    navigator.geolocation.getCurrentPosition(function (position) {
        //目前位置經緯度坐標
        pos = [position.coords.longitude, position.coords.latitude];
        var pos3857 = ol.proj.transform(pos, "EPSG:4326", "EPSG:3857");
        var pos97 = ol.proj.transform(pos, "EPSG:4326", "EPSG:3826");
        map.getView().setCenter(pos3857);
        map.getView().setZoom(18);

        var iconFeature = new ol.Feature(new ol.geom.Point(pos3857));
        iconFeature.setStyle(iconStyle);
        kwSource.addFeature(iconFeature);
    });
};

/**
* @summary 坐標系統工具
* @class
*/
oltmx.util.Proj = {};

/**
* @summary 計算`sourceProj`的resolution
* @param {ol.proj.ProjectionLike} sourceProj 要轉換成此坐標系統的resolution
* @param {ol.proj.ProjectionLike} sourceProj 要從此坐標系統的resolution轉換
* @param {ol.Coordinate} targetCenter 要轉換的坐標
* @param {float} targetResolution 要轉換的resolution
* @return {float} 轉換後的resolution
* @static
* @note copy from ol.reproj.calculateSourceResolution(ol-debug.js)
* @private
*/

oltmx.util.Proj.calculateSourceResolution =
    function (sourceProj, targetProj, targetCenter, targetResolution) {
        sourceProj = ol.proj.get(sourceProj);
        targetProj = ol.proj.get(targetProj);

        // 以下從ol.reproj.calculateSourceResolution複製
        // 不可用ol.reproj.calculateSourceResolution替代
        // 因為ol.reproj.calculateSourceResolution只能在ol-debug.js用
        var sourceCenter = ol.proj.transform(targetCenter, targetProj, sourceProj);

        // calculate the ideal resolution of the source data
        var sourceResolution =
            ol.proj.getPointResolution(targetProj, targetResolution, targetCenter);

        var targetMetersPerUnit = targetProj.getMetersPerUnit();
        if (targetMetersPerUnit !== undefined) {
            sourceResolution *= targetMetersPerUnit;
        }
        var sourceMetersPerUnit = sourceProj.getMetersPerUnit();
        if (sourceMetersPerUnit !== undefined) {
            sourceResolution /= sourceMetersPerUnit;
        }

        // Based on the projection properties, the point resolution at the specified
        // coordinates may be slightly different. We need to reverse-compensate this
        // in order to achieve optimal results.

        var sourceExtent = sourceProj.getExtent();
        if (!sourceExtent || ol.extent.containsCoordinate(sourceExtent, sourceCenter)) {
            var compensationFactor =
                ol.proj.getPointResolution(sourceProj, sourceResolution, sourceCenter) /
                sourceResolution;
            if (isFinite(compensationFactor) && compensationFactor > 0) {
                sourceResolution /= compensationFactor;
            }
        }

        return sourceResolution;
    };

/**
* Tool for Bubble Info
* @class
* @private
*/
oltmx.Bubble = {};
oltmx.Bubble.__currentBubble = {};
oltmx.Bubble.defaultBubbleContainerHTML = '<div class="ol-popup">'
    + '<div class="BtnFunc" style="float: right">'
    + '    <ul>'
    + '        <li class="close"><a href="javascript:void(0);">關閉</a></li>'
    + '    </ul>'
    + '</div>'
    + '<div class="MapBubbleStyle gs-content">'
    + '</div>'
    + '</div>';

// 參數1:popup dom id
// 參數2:popup 參數
/**
* @summary 取得特定的bubble info overlay
* @desc 一個bubble info係 ol.Overlay  
*       此function參數的options  
*       為`ol.Overlay`的建構options  
*       但可以加上domID這個property，表示bubbleInfo的ID
* @param {string|Object} arg1 
*                        如果是string，則為bubble info的ID；  
*                        如果是object, 則為options
* @param {Object} arg2 
*                 如果`arg1`為id，  
*                 則此參數必為options
* @return {ol.Overlay} 代表指定的bubble info overlay
* @private
*/
oltmx.Bubble.get =
    function () {
        var options;
        var bubbleName = "_default_bubble_";
        if (arguments.length > 0) {
            if (typeof arguments[0] === "string") {
                bubbleName = arguments[0];
            } else if (typeof arguments[0] === "object") {
                options = arguments[0];
                if (options.domID) {
                    bubbleName = options.domID;
                }
            }
        }
        if (oltmx.Bubble.__currentBubble[bubbleName]) {
            return oltmx.Bubble.__currentBubble[bubbleName];
        }
        var $bubbleElement = $("#" + bubbleName);
        if (options && options.element) {
            $bubbleElement = $(options.element);
        }
        if ($bubbleElement.length === 0) {
            $bubbleElement = $(oltmx.Bubble.defaultBubbleContainerHTML);
            $bubbleElement.attr("id", bubbleName);
        }
        bubbleElement = $bubbleElement[0];
        if (!options && arguments.length > 1) {
            options = arguments[1];
            if (!options.element) {
                options.element = bubbleElement;
            }
        } else {
            options = {
                element: bubbleElement,
                autoPan: true,
                autoPanAnimation: {
                    duration: 100
                },
                offset: [0, 34]
            };
        }

        var bubbleOverlay = new ol.Overlay(options);
        //$bubbleElement.find(".close").attr("onclick", "oltmx.Bubble.get('" + bubbleName + "').setMap(null);");
        $bubbleElement.find(".close")
            .on(
                "click",
                function () {
                    oltmx.Bubble.get(this.bubbleName).setMap(null);
                }.bind(
                    {
                        bubbleName: bubbleName
                    }
                )
            );
        oltmx.Bubble.__currentBubble[bubbleName] = bubbleOverlay;
        return bubbleOverlay;
    };

/**
* @summary 顯示bubble info
* @desc 在`map`上的`coord`位置顯示bubble info，  
*       其內容填入html格式的內容`htmlContent`
* @param {ol.Map} map 圖台
* @param {ol.Coordinate} coord 圖台坐標系統的坐標
* @param {string|DOM} htmlContent html格式的內容或一個DOM element
* @private
*/
oltmx.Bubble.show = function (map, coord, htmlContent) {
    var overLay = oltmx.Bubble.get();
    overLay.setMap(map);
    var $olayElement = $(overLay.getElement());
    if (typeof htmlContent === "string") {
        $olayElement.find(".gs-content").html(htmlContent);
    } else {
        $olayElement.find(".gs-content").children().remove();
        $olayElement.find(".gs-content").append(htmlContent);
    }
    overLay.setPosition(coord);
};

/**
* @summary 隱藏圖台上的bubble info
* @param {ol.Map} map 圖台
* @private
*/
oltmx.Bubble.hide = function (map) {
    oltmx.Bubble.get().setMap(null);
};

/**
* @namespace
* @summary 資料格式相關
* @private
*/
oltmx.format = {};
/**
* @class
* @summary gml格式相關
* @private
*/
oltmx.format.gml = {};
/**
 * @function
 * @summary 將GML轉換成ol.Feature[]
 * @param {String|XMLDom} _gmlContent GML內容
 * @return {ol.Feature[]} 圖徵
 * @private
 */
oltmx.format.gml.readFeatures = function (_gmlContent) {
    var gmlContent = _gmlContent;

    if (typeof gmlContent === "string") {
        gmlContent = new DOMParser().parseFromString(_gmlContent, "text/xml");
    }

    if (!gmlContent) return null;
    var gmlFormat = null;
    var gmlNS = null;
    for (var i = 0; i < gmlContent.documentElement.attributes.length; i++) {
        if (gmlContent.documentElement.attributes[i].nodeValue === "http://www.opengis.net/gml") {
            gmlNS = gmlContent.documentElement.attributes[i].nodeValue;
            break;
        }
    }
    if (!gmlNS) {
        alert("抱歉，本系統僅支援GML3.1.1與GML2.1.2");
        return null;
    }
    var $position = $(gmlContent).find("coordinates:first");
    var srsName = $(gmlContent).find("*[srsName]:first").attr("srsName");
    if ($position.length > 0) {
        gmlFormat = new ol.format.GML2({
            srsName: srsName
        });
    } else {
        gmlFormat = new ol.format.GML3({
            srsName: srsName
        });
    }
    return gmlFormat.readFeatures(gmlContent, {
        dataProjection: srsName,
        featureProjection: "EPSG:3857"
    });
};