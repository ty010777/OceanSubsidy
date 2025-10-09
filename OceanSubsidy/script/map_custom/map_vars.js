// geoserver 的網址
gisServerURL = "https://projects.geosense.tw/geoserver/";


constAutoGenBubbleInfo = "#AUTO#";
continueAreaMeasureMsg = "雙擊左鍵結束量測";
continueLengthMeasureMsg = "雙擊左鍵結束量測";
coordinateTemplate = "({x}, {y})";
defaultBubbleContainerHTML = '<div class="ol-popup GSMap-element-popup-detail-wrap">'
                + '<div class="BtnFunc btnBoxRight" style="float: right">'
                + '    <input type="button" value="X" class="btnS btn_transparent close">'
                + '</div>'
                + '<div class="MapBubbleStyle gs-content">'
                + '</div>'
                + '</div>';
defaultStyle = {
    stroke: { color: 'blue', width: 1 },
    fill: { color: 'rgba(255, 255, 0, 0.1)' },
    icon: {
        anchor: [0.5, 16],
        anchorXUnits: 'fraction',
        anchorYUnits: 'pixel',
        opacity: 0.9,
        src: 'images/map_legend/default.png'
    }
};

locatingStyle = {
    stroke: { color: 'rgba(32,32,255,0.8)', width: 3 },
    fill: { color: "rgba(128,128,255,0.2)" },
    icon: {
        anchor: [10, 32],
        anchorXUnits: 'pixels',
        anchorYUnits: 'pixels',
        opacity: 0.7,
        src: "App_themes/Map/images/location.png"
    },
    text: {
        scale: 0.8,
        stroke: { color: '#000088', width: 5 },
        fill: { color: "#FFFFFF" },
        font: 'bold 25px 微軟正黑體,sans-serif',
        offsetY: 15
    }
};

displayPrj = "EPSG:4326";
getCoordinateHelpMsg = "請選取要取得坐標的位置";
htmlCaptureService = "service/GenImageFromHtml.ashx";
initPosition = [121.0018405, 23.4824873];
initZoom = 8;
locatingPointZoom = 20;
maxZoomLevel = 28;
minZoomLevel = 2;
startAreaMeasureMsg = "請點選起始點";
startLengthMeasureMsg = "請點選起始點";
useGeodesicMeasures = true;


// 圖台顯示的坐標系統
//var displayPrj = "EPSG:3826";

// 初始位置
//var initPosition = [194188, 2597712]; // 坐標系統為displayPrj
//var initZoom = 7;

// setup on map init
var maxZoomLevel = 18;
var minZoomLevel = 0;

//var htmlCaptureService = "service/GenImageFromHtml.ashx";
//var htmlCaptureService = "service/RegularPoxy.ashx";

// 定位到點時，zoom到此level
//var locatingPointZoom = 16;

// how to show coordinates on the map
//var pluginOptions =
//{
//    actives: [
//        // 啟用長度量測API
//        { id: "MeasureLength" },
//        // 啟動面積量測API
//        { id: "MeasureArea" },
//        // 啟動定位API
//        { id: "Locate" },
//        // 啟動圖層管理API
//        { id: "LayersMg", params: { mapLayers: map_layers } },
//        // 啟動編輯API
//        { id: "Editor" }
//    ],
//    // 圖台顯示的坐標系統
//    displayPrj: displayPrj,
//    // 初始位置，如果lenght=4，則表示envelop，坐標系統為displayPrj
//    initPosition: [194188, 2597712],
//    // 初始level，如果初始位置是envelop，此屬性無作用
//    initZoom : 7,
//    htmlCaptureService: "service/RegularPoxy.ashx",
//    // 定位到點時，zoom到此level
//    locatingPointZoom: 16,
//    // 是否使用球面量測計算
//    useGeodesicMeasures: true,
//    // 開始面積計算時的說明文字
//    startAreaMeasureMsg: "請點選起始點",
//    // 啟用擷取圖台坐標位置時的說明文字
//    getCoordinateHelpMsg: "請選取要取得坐標的位置",
//    // 面積量測中的說明文字
//    continueAreaMeasureMsg: "雙擊左鍵結束量測",
//    // 開始長度計算時的說明文字
//    startLengthMeasureMsg: "請點選起始點",
//    // 長度量測中的說明文字
//    continueLengthMeasureMsg: "雙擊左鍵結束量測",
//    // 如果serviceInfo的bubbleContent設成這個值，將自動列出所有欄位
//    constAutoGenBubbleInfo: "#AUTO#",
//    defaultBubbleContainerHTML:
//        '<div class="ol-popup">'
//        + '  <div class="BtnFunc" style="float: right">'
//        + '      <ul>'
//        + '          <li class="close"><a href="javascript:void(0);">關閉</a></li>'
//        + '      </ul>'
//        + '  </div>'
//        + '  <div class="MapBubbleStyle gs-content">'
//        + '  </div>'
//        + '</div>',
//    defaultStyle: {
//        stroke: { color: 'blue', width: 1 },
//        fill: { color: 'rgba(255, 255, 0, 0.1)' },
//        icon: {
//            anchor: [0.5, 16],
//            anchorXUnits: 'fraction',
//            anchorYUnits: 'pixel',
//            opacity: 0.9,
//            src: 'images/map_legend/default.png'
//        }
//    }
//};

// if false, measure is done in simple way on projected plane. Earth curvature is not taken into account
//var useGeodesicMeasures = true;

//var startAreaMeasureMsg = "請點選起始點";

//var getCoordinateHelpMsg = "請選取要取得坐標的位置";

/**
* Message to show when the user is drawing a polygon.
* @type {string}
*/
//var continueAreaMeasureMsg = '雙擊左鍵結束量測';

//var startLengthMeasureMsg = "請點選起始點";

/**
* Message to show when the user is drawing a line.
* @type {string}
*/
//var continueLengthMeasureMsg = '雙擊左鍵結束量測';

/**
* 如果serviceInfo的bubbleContent設成這個值，將自動列出所有欄位
* @type {string}
*/
//var constAutoGenBubbleInfo = "#AUTO#";

/**
 * 設定啟用功能項
 * '00' : 全轄區
 * '01' : 底圖切換
 * '02' : 量測
 * '03' : 範圍查詢
 * '04' : 定位
 * '05' : 圖層套疊
 * '06' : 匯出
 * '08' : 圖台標示
 * '09' : 活動查詢
 * '07' : 關閉全部
 */
var enableFunctions = "00";
//var enableFunctions = "01,02,03,06,07";

/*
* 預設的bubble info layout
* 必須有css="close"的dom，以實現關閉的動作
*/
//var defaultBubbleContainerHTML = '<div class="ol-popup">'
//        + '<div class="BtnFunc" style="float: right">'
//        + '    <ul>'
//        + '        <li class="close"><a href="javascript:void(0);">關閉</a></li>'
//        + '    </ul>'
//        + '</div>'
//        + '<div class="MapBubbleStyle">'
//        + '<table class="BubleTable">'
//        + '</table>'
//        + '</div>'
//        + '</div>';


//var defaultStyle = {
//    stroke: { color: 'blue', width: 1 },
//    fill: { color: 'rgba(255, 255, 0, 0.1)' },
//    icon: {
//        anchor: [0.5, 16],
//        anchorXUnits: 'fraction',
//        anchorYUnits: 'pixel',
//        opacity: 0.9,
//        src: 'images/map_legend/default.png'
//    }
//};