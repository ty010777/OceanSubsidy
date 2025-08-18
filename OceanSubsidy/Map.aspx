<%@ Page Title="" Language="C#" MasterPageFile="~/MapMaster.master" AutoEventWireup="true"
    CodeFile="Map.aspx.cs" Inherits="Map" Theme="map" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <!-- FontAwesome -->
    <link rel="stylesheet" href="<%= ResolveUrl("~/assets/vendor/fontawesome-free-6.5.2-web/css/all.min.css") %>">
    <link rel="stylesheet" href="<%= ResolveUrl("~/assets/vendor/fontawesome-free-6.5.2-web/css/all.css") %>">

    <!-- Google Icons -->
    <link rel="stylesheet" href="<%= ResolveUrl("~/assets/vendor/googleFont/googleFont-Material-Symbols-Rounded.css") %>">
    <link rel="stylesheet" href="<%= ResolveUrl("~/assets/vendor/googleFont/googleFont-Material-Icons-Round.css") %>">

    <!-- Google Fonts -->
    <link href="<%= ResolveUrl("~/assets/vendor/googleFont/googleFont.css") %>" rel="stylesheet">

    <script type="text/javascript" src="script/map_custom/extendPage.js"></script>
    <!-- 以下順序不可改 -->
    <!-- [if lte IE 9] >
    <%-- 利用 polyfill 補完 IE9 不足的功能 --%>
    <script src="script/map/polyfill.min.js" type="text/javascript"></script>
    <! [endif] -->
    <!-- 同上面預先下載版本 -->
    <%--
    <!-- 指定User Agent，讓polyfill網站依照UA的值，產生相對應的補完JS -->
    <script src="https://cdn.polyfill.io/v2/polyfill.min.js?ua=Mozilla%2F5.0%20(Windows%3B%20U%3B%20MSIE%209.0%3B%20Windows%20NT%206.1)" type="text/javascript"></script>
    --%>
    <%--
    <!-- 讓polyfill網站依照browser給予得User Agent值，產生相對應的補完JS -->
    <script src="https://cdn.polyfill.io/v2/polyfill.min.js" type="text/javascript"></script>
    --%>
    <script src="script/map/jquery-ui.min.js"></script>
    <script src="script/map/gs_core.js"></script>
    <script src="script/map/AssessTools.js"></script>
    <script src="script/map/rwdHelper.js" type="text/javascript"></script>
    <script type="text/javascript" src="script/map/html2canvas.js"></script>
    <script src="script/map/proj4.js" type="text/javascript"></script>

    <script src="script/map/ol.js" type="text/javascript"></script>
    <script src="script/map_custom/pro4_defs.js" type="text/javascript"></script>
    <%--<script src="script/map/ol-debug.js" type="text/javascript"></script>--%>

    <script src="script/map/ImageFilter.js" type="text/javascript"></script>
    <script src="script/map/toBitmapURL.js"></script>

    <%-- 開發模式
        線上應用gsmap.js取代
    --%>
    <script src="script/map/tm_openlayers_plugin.js" type="text/javascript"></script>
    <script src="script/map/tm_openlayers_measure.js" type="text/javascript"></script>
    <script src="script/map/tm_openlayers_locate.js" type="text/javascript"></script>
    <script src="script/map/tm_openlayers_layer_manager.js" type="text/javascript"></script>
    <script src="script/map/tm_ol3_editor.js" type="text/javascript"></script>
    <%--<script src="$map_lib/gsmap.js"></script>--%>
    <%-- 開發模式End --%>

    <script src="script/map_ui/gsmap_ui.js"></script>
    <script src="script/map_ui/gsmap_ui_bottom.js"></script>
    <script src="script/map_ui/gsmap_ui_functions.js"></script>

    <script src="script/map_custom/gsmap_func_Identify.js"></script>
    <script src="script/map_custom/gsmap_func_RangeQuery.js"></script>
    <script src="script/map_functions/gsmap_func_measure.js"></script>
    <script src="script/map_functions/gsmap_func_basemap.js"></script>
    <script src="script/map_functions/gsmap_func_locate.js"></script>
    <script src="script/map_functions/gsmap_func_legend.js"></script>
    <script src="script/map_functions/gsmap_func_export.js"></script>
    <script src="script/map_functions/gsmap_func_marker.js"></script>
    <script src="script/map_functions/gsmap_func_activityQuery.js"></script>

    <!-- SumoSelect -->
    <link href="lib/sumoselect/sumoselect.css" rel="stylesheet" />
    <script src="lib/sumoselect/jquery.sumoselect.min.js"></script>

    <%--<script src="<%= ConfigurationManager.AppSettings["map.Manager.BaseURL"] %>WebService/MapVariables.ashx?ppID=<%= ppID %>" type="text/javascript"></script>--%>
    <script src="script/map_custom/map_vars.js" type="text/javascript"></script>

    <script type="text/javascript">
        gsServerLegendURL = gisServerURL + "gs/ows?service=WMS&request=GetLegendGraphic&format=image%2Fpng&width=18&height=18&legend_options=forceLabels:off&layer=gs%3A";
    </script>

    <%--<script src="<%= ConfigurationManager.AppSettings["map.Manager.BaseURL"] %>WebService/MapLayerConfig.ashx?ppID=<%= ppID %>&assign=map_layers" type="text/javascript"></script>--%>
    <script src="script/map_custom/map_layers.js" type="text/javascript"></script>



    <script src="script/map_custom/map_init.js" type="text/javascript"></script>
    <script src="script/map_custom/map_layers_ext.js"></script>
    <script src="script/map_custom/map_base.js" type="text/javascript"></script>
    <script src="script/map_custom/markList.js" type="text/javascript"></script>
    <!-- ============ -->
    <%--<script src="script/map_custom/page_func.js" type="text/javascript"></script>--%>
    <style type="text/css">
        /*
        設定框選extent的樣式
        */
        .ol-dragbox {
            box-sizing: border-box;
            border-radius: 2px;
            border: 2px solid #FF9900;
        }

        .ol-dragzoom {
            box-sizing: border-box;
            border-radius: 2px;
            border: 2px solid #0000FF;
        }
    </style>
    <script type="text/javascript">
        $.support.cors = true;
        var resizeInterval =
            setInterval(resizeMap, 500);
        function resizeMap() {
            var $map = $(".map");
            if ($map.length > 0) {
                resizeMisFrame();
                $map.height(gs.RWDHelper.getWindowHeight());
                map.updateSize();
                //$("#contentBox").height($("html").height() - 49);
                clearInterval(resizeInterval);
                resizeInterval = null;
                var $iframes = $("#contentBox iframe");
                for (var i = 0; i < $iframes.length; i++) {
                    var cWin = $iframes[i].contentWindow;
                    if (cWin && cWin.resizeLayout) {
                        cWin.resizeLayout();
                    }
                }
            }
        }
        $(window).on("resize", function () {
            clearInterval(resizeInterval);
            resizeInterval = setInterval(resizeMap, 500);
        });

    </script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceInForm" runat="Server">
    <style type="text/css">
        .nano {
            background: #ffffff;
            width: 100%;
            height: 550px;
        }

            .nano > .nano-content {
                padding: 10px;
            }

            .nano > .nano-pane {
                background: rgba(255,255,255,.5);
                width: 8px;
            }
                /* 設定捲軸背景底色, 寬度 */
                .nano > .nano-pane > .nano-slider {
                    background: rgba(0,0,0,.15);
                }
        /* 設定捲軸顏色 */
        body {
            background-color: #000000;
        }
        /* 地圖style */
        .map {
            width: 100%;
            position: relative;
            height: 100%;
        }

        @media(min-width:980px) {
            .ol-zoom {
                top: 0.5em;
                right: .5em;
                left: unset;
            }

            .ol-rotate {
                width: 31px;
                height: 35px;
                top: -3.5em;
                right: .2em;
                transition: opacity .25s linear,visibility 0s linear;
            }
        }

        @media (min-width: 768px) and (max-width: 979px) {
            .ol-rotate {
                width: 31px;
                height: 35px;
                top: 4em;
                right: .2em;
                transition: opacity .25s linear,visibility 0s linear;
            }

            .ol-zoom {
                top: .5em;
                right: .5em;
                left: unset;
            }
        }

        @media(min-width: 321px) and (max-width:767px) {
            .ol-rotate {
                width: 31px;
                height: 35px;
                top: 4em;
                right: .2em;
                transition: opacity .25s linear,visibility 0s linear;
            }

            .ol-zoom {
                top: 4em;
                right: .5em;
                left: unset;
            }
        }

        @media (max-width: 320px) {
            .ol-rotate {
                width: 31px;
                height: 35px;
                top: 4em;
                right: .2em;
                transition: opacity .25s linear,visibility 0s linear;
            }

            .ol-zoom {
                top: .5em;
                right: .5em;
                left: unset;
            }
        }

        .ol-rotate button {
            background-size: 30px auto;
            background-image: url(App_Themes/map/images/northarrow.svg);
            background-repeat: no-repeat;
            background-position: center top;
        }

        .ol-overviewmap .ol-overviewmap-box {
            border: 3px solid #FF0000;
        }

        /* 
        feature popup視窗 style
        */
        .ol-popup {
            position: absolute;
            background-color: white;
            -webkit-filter: drop-shadow(0 1px 4px rgba(0,0,0,0.2));
            filter: drop-shadow(0 1px 4px rgba(0,0,0,0.2));
            border-radius: 10px;
            border: 1px solid #cccccc;
            bottom: 44px;
            left: -48px;
        }

            .ol-popup:after, .ol-popup:before {
                top: 100%;
                border: solid transparent;
                content: " ";
                height: 0;
                width: 0;
                position: absolute;
                pointer-events: none;
            }

            .ol-popup:after {
                border-top-color: white;
                border-width: 10px;
                left: 48px;
                margin-left: -10px;
            }

            .ol-popup:before {
                border-top-color: #cccccc;
                border-width: 11px;
                left: 48px;
                margin-left: -11px;
            }

        .ol-popup-closer {
            text-decoration: none;
            position: absolute;
            top: 2px;
            right: 8px;
        }

            .ol-popup-closer:after {
                content: "✖";
            }

        .no-close .ui-dialog-titlebar-close {
            display: none;
        }

        .ol-overviewmap button {
            right: -152px;
            left: unset;
            bottom: 1px;
            position: absolute;
        }

        .ol-overviewmap:not(.ol-collapsed) button {
            right: 2px;
            left: unset;
            position: absolute;
        }
    </style>
    <!-- measure style -->
    <style>
        .tooltip {
            position: relative;
            background: rgba(0, 0, 0, 0.5);
            border-radius: 4px;
            color: white;
            padding: 4px 8px;
            opacity: 0.7;
            filter: alpha(opacity=70);
            white-space: nowrap;
        }

        .tooltip-measure {
            opacity: 1;
            font-weight: bold;
        }

        .tooltip-static {
            background-color: #ffcc33;
            color: black;
            border: 1px solid white;
        }

            .tooltip-measure:before, .tooltip-static:before {
                border-top: 6px solid rgba(0, 0, 0, 0.5);
                border-right: 6px solid transparent;
                border-left: 6px solid transparent;
                content: "";
                position: absolute;
                bottom: -6px;
                margin-left: -7px;
                left: 50%;
            }

            .tooltip-static:before {
                border-top-color: #ffcc33;
            }

        #heatParams {
            background-color: #FFFFFF;
            left: 10px;
            top: 100px;
            width: 150px;
            height: 120px;
        }

        #container {
            height: 800px;
            min-width: 310px;
            max-width: 1000px;
            margin: 0 auto;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceBody" runat="Server">
    <div>
        <div id="map" class="map" style="background-color: #FFFFFF;">
            <div id="OverviewMap" class="gsmap-overview" style="z-index: 9; position: absolute" onmouseout="$('#OverviewMap').find('button')[0].style.display='none'" onmouseover="$('#OverviewMap').find('button')[0].style.display=''"></div>
            <div id="rotate" style="z-index: 99999; position: absolute"></div>
            <div class="gsmap_search">
                <input id="pac-input" type="text" style="width: 300px;" placeholder="請輸入關鍵字" />
            </div>
            <%--<div style="top:70px; left:10px; position:absolute; z-index:99999" ><input id="Geolocation" class="BtnGPS" type="button" value="" onclick="oltmx.Plugin.prototype.Geolocation();"/></div>--%>
            <img id="exportNortharrow" src="App_Themes/map/images/northarrow.png" style="display: none;" />

            <div id="menu" class="ol-popup Bubble" style="width: 270px;">
                <div class="BtnFunc" style="float: right">
                    <ul>
                        <li class="close"><a href="javascript:void(0);ovMenu.setMap(null);">關閉</a></li>
                    </ul>
                </div>
                <!--關閉+說明-->
                <div id="menu-content" class="MapBubbleStyle">
                </div>
            </div>

            <div id="mouse-position" class="gsmap-mouse-position">
            </div>

            <div id="heatParams" style="display: none; border: solid; border-width: 2px; position: absolute; z-index: 99999">
                <form>
                    <label>半徑範圍</label>
                    <br>
                    <input id="radius" type="range" min="1" max="50" step="1" value="5" onchange="changeRadiusVal()" />
                    <br>
                    <label>模糊度</label>
                    <br>
                    <input id="blur" type="range" min="1" max="50" step="1" value="15" onchange="changeBlurVal()" />
                </form>
            </div>
            <table id="tableCaptureExtent" style="position: absolute; left: 0px; top: 0px; width: 100%; height: 100%; z-index: 9; pointer-events: none; display: none;">
                <tr>
                    <td style="text-align: center; vertical-align: middle;">
                        <div id="divCaptureExtent" style="width: 1024px; height: 768px; border: 2px solid blue; display: inline-block">
                        </div>
                    </td>
                </tr>
            </table>

            <%-- 圖台環境資訊 --%>
            <div class="gsmap-mapenv-info">
            </div>

            <%-- 活動數量說明框 --%>
            <div id="activityLegend" style="position: absolute; top: 10px; right: 10px; 
                 background: white; padding: 15px; border-radius: 8px; 
                 box-shadow: 0 4px 10px rgba(0,0,0,0.15); z-index: 1000; display: none;
                 border: 1px solid #ddd; min-width: 180px;">
                <h6 class="mb-3 fw-bold text-center" style="color: #333; margin: 0 0 10px 0; font-size: 14px;">活動數量圖例</h6>
                <div style="margin-bottom: 8px; display: flex; align-items: center;">
                    <span style="display: inline-block; width: 25px; height: 18px; 
                         background-color: #28a745; margin-right: 10px; border-radius: 3px; opacity: 0.8;"></span>
                    <span style="font-size: 13px; color: #555;">活動件數 ≤ 4 件</span>
                </div>
                <div style="margin-bottom: 8px; display: flex; align-items: center;">
                    <span style="display: inline-block; width: 25px; height: 18px; 
                         background-color: #ffc107; margin-right: 10px; border-radius: 3px; opacity: 0.8;"></span>
                    <span style="font-size: 13px; color: #555;">活動件數 5-9 件</span>
                </div>
                <div style="display: flex; align-items: center;">
                    <span style="display: inline-block; width: 25px; height: 18px; 
                         background-color: #dc3545; margin-right: 10px; border-radius: 3px; opacity: 0.8;"></span>
                    <span style="font-size: 13px; color: #555;">活動件數 ≥ 10 件</span>
                </div>
            </div>

            <!--下方框架-->
            <div id="bottomInfo" class="gsmap-bottom">
                <!--按鈕結尾-->
                <%--<ul id="navBottom" class="gsmap-bottom-nav map_nav nav nav-tabs" role="tablist">
                    <li class="nav-item">
                        <a id="tab-identify" href="#cont-identify" class="nav-link" data-toggle="tab" role="tab" aria-controls="cont-identify" aria-selected="false">圖徵資訊
                        </a>
                    </li>
                    <li class="nav-item">
                        <a id="tab-coord" href="#cont-coord" class="nav-link" data-toggle="tab" role="tab" aria-controls="cont-coord" aria-selected="false">坐標資訊
                        </a>
                    </li>
                </ul>--%>
                <div id="contentBottom" class="gsmap-bottom-content tab-content" style="overflow: auto; background-color: white; position: sticky;">
                    <div id="cont-identify" class="tab-pane fade mapcont__item" role="tabpanel" aria-labelledby="tab-identify">
                        <div id="featureInfo" class="gsmap-bottom-identify MapBubbleStyle">
                            <table class="BubleTable" style="width: 100%; border-width: 1px;">
                                <tbody></tbody>
                            </table>
                        </div>
                    </div>
                    <div id="cont-coord" class="tab-pane fade mapcont__item" role="tabpanel" aria-labelledby="tab-coord">
                        <table class="LayerInfoTable" style="width: 100%;" border="1">
                            <tr>
                                <td>WGS84</td>
                                <td class="_tdWGS84"></td>
                            </tr>
                            <tr>
                                <td>TWD97</td>
                                <td class="_tdTWD97"></td>
                            </tr>
                            <tr>
                                <td>TWD67</td>
                                <td class="_tdTWD67"></td>
                            </tr>
                        </table>
                        <br />
                        <input type="button" class="_btnGoogle BtnBlue" value="開啟Google街景" />
                    </div>
                </div>
            </div>
            <!--下方框架 End-->

        </div>

        <!-- 標註清單 -->
        <div id="divMark" class="draggable position-absolute" style="z-index: 10; top: 10px; left: 380px; width: 150px; display: none;">
            <div class="panel card" style="display: block;">
                <div class="card-header handle">
                    標註圖層                      
                </div>
                <div class="card-body p-2">
                    <!-- body 標註清單-->
                    <div id="MarkList" class="list overflow-auto scroll MarkList" style="max-height: calc(100vh - 96px);"></div>
                </div>
            </div>
        </div>
    </div>

    <div id="helper" class="modal fade" role="dialog">
        <div class="Popup modal-dialog" role="document">
            <div class="modal-content">
                <div class="BtnFunc modal-header">
                    <ul>
                        <li style="float: left">
                            <h1>HELP</h1>
                        </li>
                        <li class="close" data-dismiss="modal" aria-label="Close"><a style="margin-top: 8px; margin-right: -10px;"
                            href="javascript:void(0);$('#helper').modal('hide');">關閉</a>
                        </li>
                    </ul>
                </div>
                <!--關閉+說明-->
                <div class="modal-body">
                    <div id="helpContent">
                        • 請平移地圖，將要匯出的範圍移動至範圍內
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="featureDetail" class="modal fade" role="dialog">
        <div class="Popup modal-dialog" role="document">
            <div class="modal-content">
                <div class="BtnFunc modal-header">
                    <ul>
                        <li style="float: left">
                            <h1>&nbsp;
                            </h1>
                        </li>
                        <li class="close" data-dismiss="modal" aria-label="Close"><a style="margin-top: 8px; margin-right: -10px;"
                            href="javascript:void(0);$('#featureDetail').modal('hide');">關閉</a> </li>
                    </ul>
                </div>
                <!--關閉+說明-->
                <div class="modal-body">
                    <div id="featureDetailContent">
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!--左方框架-->
    <!--內容區塊-->
    <div id="mis_left_content" class="mis_content" style="z-index: 99999; background-color: rgba(255, 255, 255, 0.8); border-right: 1px solid #b1dd82; margin-top: 100px;">
        <iframe id="mis_left_contentFrame" name="mis_left_content_frame" style="width: 100%; height: 100%;" src="Map/mis_left_content.html"></iframe>
    </div>
    <!--內容區塊結尾-->
    <!--按鈕-->
    <div id="mis_left_icon" class="mis_menu" style="display: none;">
        <ul>
            <a href="#">
                <li id="mis_left_icon_li" class="open" onclick="misLeftIconClick(this);">收合</li>
            </a>
        </ul>
    </div>
    <!--按鈕結尾-->
    <!--左方結尾-->


    <script type="text/javascript">
        window.initialWKT3826 = <%= InitialWKT3826_JS %>;

        (function () {
            var params = new URLSearchParams(window.location.search);
            var codes = params.get("codes") || "";
            if (codes === "")
                mapInit();
            else
                mapInit(codes);

        })();

        // 把所有要素轉換成 EPSG:3826 (TWD97 / TM2 zone 121) 座標輸出成 JSON 格式
        function getAllFeaturesWKT3826() {
            var wktFormat = new ol.format.WKT();
            var features = drawSource.getFeatures(); // ol.Feature 陣列

            if (!features || features.length === 0) {
                return "";
            }

            var result = {
                type: "FeatureCollection",
                features: []
            };

            features.forEach(function (feature) {
                var geom = feature.getGeometry();
                if (!geom) return;

                // 如果是 ol.geom.Circle，就先把它轉成 Polygon (64 邊近似圓)
                if (geom.getType() === 'Circle') {
                    geom = ol.geom.Polygon.fromCircle( /** @type {ol.geom.Circle} */(geom), 64);
                }

                // 複製幾何體，避免修改原始資料
                var clonedGeom = geom.clone();
                
                // 從 EPSG:3857 轉換到 EPSG:3826
                clonedGeom.transform('EPSG:3857', 'EPSG:3826');
                
                // 取得名稱（從不同的 style 設定中）
                var name = "";
                var style = feature.getStyle();
                if (style) {
                    if (Array.isArray(style)) {
                        // 處理 style 陣列的情況（例如線段有多個樣式）
                        for (var i = 0; i < style.length; i++) {
                            if (style[i].getText && style[i].getText()) {
                                name = style[i].getText().getText();
                                break;
                            }
                        }
                    } else if (style.getText && style.getText()) {
                        name = style.getText().getText();
                    }
                }
                
                // 將 feature 資訊加入結果
                result.features.push({
                    id: feature.getId(),
                    name: name,
                    wkt: wktFormat.writeGeometry(clonedGeom, 
                        { dataProjection: 'EPSG:3826', featureProjection: 'EPSG:3826' })
                });
            });

            return JSON.stringify(result);
        }

    </script>
    <script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyCMt5GlDE2w0dpESkPnyUO4V-g5FVVXEbw&libraries=places&callback=initAutocomplete"
        async defer></script>
</asp:Content>
