var baseLayerInfos = {};
var baseLayer;
var drawInteraction;
var modifyInteraction, overviewMapControl;
var MarkList;
var pointNum = 0;
var lineNum = 0;
var polyNum = 0;
var txtNum = 0;
var importSingleClickEventActive = false;
var mapPlugin = null;
var spatialResultLayerName = "spatialQuery";
var pointMarker = null;
var markType, markPointDiv, markLineDiv, markPolyDiv, markTxtDiv;
var pointColor, pointScale, pointOpacity, pointColorPick, lineColor, lineWidth, lineOpacity, lineColorPick, polyColor, polyFillColor, polyWidth, polyOpacity, polyStrokePick, polyFillPick, textColor, textStrokeColor, textWidth, textContent, textScale;
var coordCollection = [];
var rangeQueryLayer = [];
var selectedLayer, selectedFeature;
var identifyFeatures;

var testVector = new ol.layer.Vector({
    source: new ol.source.Vector({
        url: './json/Taichung.json',
        format: new ol.format.TopoJSON({
            layers: ['county']
        }),
        overlaps: false
    }),
    zIndex: 9999
});

var kwSource = new ol.source.Vector({
    wrapX: false
});

var kwVector = new ol.layer.Vector({
    zIndex: 9999,
    source: kwSource
});

var drawSource = new ol.source.Vector({
    wrapX: false
});

var drawVector = new ol.layer.Vector({
    zIndex: 9999,
    source: drawSource
});

var markStyle = new ol.style.Style({
    image: new ol.style.Icon({
        anchor: [10, 32],
        anchorXUnits: 'pixels',
        anchorYUnits: 'pixels',
        opacity: 1,
        src: "App_Themes/map/images/location.png"
    }),
    stroke: new ol.style.Stroke({
        color: "rgba(32,32,255,0.8)",
        width: 3
    }),
    fill: new ol.style.Fill({
        color: "rgba(128,128,255,0.2)"
    })
});

function initAutocomplete() {
    autocomplete = new google.maps.places.Autocomplete(
        (document.getElementById('pac-input')),
        { types: ['geocode', 'establishment'] });

    //google map api gps定位
    /*if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var pos = [position.coords.longitude, position.coords.latitude];
            var pos3857 = ol.proj.transform(pos, "EPSG:4326", "EPSG:3857");
            map.getView().setCenter(pos3857);
            map.getView().setZoom(18);
            kwSource.clear();
            var marker = new ol.Feature(new ol.geom.Point(pos3857));
            marker.setStyle(markStyle);
            kwSource.addFeature(marker);
        });
    }*/

    autocomplete.addListener('place_changed', function () {
        var place = autocomplete.getPlace();

        var Center = [place.geometry.location.lng(), place.geometry.location.lat()];
        var Center3857 = ol.proj.transform(Center, "EPSG:4326", "EPSG:3857");
        map.getView().setCenter(Center3857);
        map.getView().setZoom(18);

        kwSource.clear();
        var marker = new ol.Feature(new ol.geom.Point(Center3857));
        marker.setStyle(markStyle);
        kwSource.addFeature(marker);
    });
}

// 初始化圖台
function mapInit(_functions) {
    // layer group for base map
    //baseLayer = new ol.layer.Group();
    // layer group for upper base map

    var centerPosition = [0, 0];
    if (initPosition.length === 4) {
        centerPosition = [(initPosition[0] + initPosition[2]) / 2, (initPosition[1] + initPosition[3]) / 2];
    } else {
        centerPosition = initPosition;
    }

    ol.control.Rotate.render = function (mapEvent) {
        var frameState = mapEvent.frameState;
        if (!frameState) {
            return;
        }
        var rotation = frameState.viewState.rotation;
        if (rotation != this.rotation_) {
            var transform = 'rotate(' + rotation + 'rad)';
            if (this.autoHide_) {
                var contains = this.element.classList.contains(ol.css.CLASS_HIDDEN);
                if (!contains && rotation === 0) {
                    this.element.classList.add(ol.css.CLASS_HIDDEN);
                } else if (contains && rotation !== 0) {
                    this.element.classList.remove(ol.css.CLASS_HIDDEN);
                }
            }
            this.label_.style.msTransform = transform;
            this.label_.style.webkitTransform = transform;
            this.label_.style.transform = transform;
            $(".ol-rotate-reset")[0].style.transform = transform;
        }
        this.rotation_ = rotation;
    };

    // 比例尺線
    var scaleLineControl = new ol.control.ScaleLine({
        className: "gsmap-scale-line"
    });

    //地圖旋轉
    var rotateInteraction = new ol.interaction.DragRotateAndZoom();

    map = new ol.Map({
        target: 'map',
        layers: [drawVector, kwVector],
        logo: false,
        //renderer: 'dom',
        renderer: 'canvas',
        interactions: ol.interaction.defaults.defaults().extend([
            rotateInteraction
        ]),
        controls: ol.control.defaults.defaults({
            attributionOptions: /** @type {olx.control.AttributionOptions} */({
                collapsible: false
            })
        }).extend([
            scaleLineControl
        ]),
        view: new ol.View({
            center: ol.proj.transform(centerPosition, displayPrj, 'EPSG:3857'),
            zoom: initZoom,
            maxZoom: maxZoomLevel,
            minZoom: minZoomLevel
        })
    });

    mapPlugin = new oltmx.Plugin(map,
        {
            actives: [
                { id: "MeasureLength" },
                { id: "MeasureArea" },
                {
                    id: "Locate",
                    params: {
                        locatingStyle: locatingStyle
                    }
                },
                { id: "LayersMg", params: { mapLayers: map_layers, baseLayers: map_base, allowMinRequest: true } },
                { id: "Editor" }
            ],
            displayPrj: displayPrj,
            initPosition: initPosition,
            initZoom: initZoom,
            htmlCaptureService: "service/RegularPoxy.ashx",
            proxyUrlTemplate: "service/proxy2.ashx?{url}",
            locatingPointZoom: 16,
            coordinateTemplate: coordinateTemplate,
            useGeodesicMeasures: true,
            enableUI: true,
            startAreaMeasureMsg: "請點選起始點",
            getCoordinateHelpMsg: "請選取要取得坐標的位置",
            continueAreaMeasureMsg: "雙擊左鍵結束量測",
            startLengthMeasureMsg: "請點選起始點",
            continueLengthMeasureMsg: "雙擊左鍵結束量測",
            constAutoGenBubbleInfo: "#AUTO#",
            defaultBubbleContainerHTML:
                '<div class="ol-popup GSMap-element-popup-detail-wrap">'
                + '<div class="BtnFunc btnBoxRight" style="float: right">'
                + '    <input type="button" value="X" class="btnS btn_transparent close">'
                + '</div>'
                + '<div class="MapBubbleStyle gs-content">'
                + '</div>'
                + '</div>',
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
        }
    );

    map.on('precompose', function (evt) {
        if (evt.context) {
            //evt.context.imageSmoothingEnabled = false;
            evt.context.webkitImageSmoothingEnabled = false;
            evt.context.mozImageSmoothingEnabled = false;
            evt.context.msImageSmoothingEnabled = false;
        }
    });

    var rotateControl = new ol.control.Rotate({
        className: 'ol-rotate',
        target: document.getElementById('rotate'),
        label: '',
        autoHide: true
    });

    $(".ol-compass").remove();
    $(".ol-rotate")[0].style.backgroundColor = 'transparent';
    $(".ol-rotate button")[0].style.backgroundColor = 'transparent';
    $(".ol-rotate")[0].style.width = '38px';
    $(".ol-rotate")[0].style.height = '50px';
    $(".ol-rotate")[0].style.top = '4.5em';
    $(".ol-rotate-reset")[0].style.width = '38px';
    $(".ol-rotate-reset")[0].style.height = '50px';

    if (initPosition.length === 4) {
        var displayExtent = ol.proj.transformExtent(initPosition, displayPrj, 'EPSG:3857');
        map.getView().fit(displayExtent, map.getSize());
    }


    // control for coordinates presentation
    var mousePositionControl = new ol.control.MousePosition({
        coordinateFormat: function (coordinate) {
            var ret = ol.coordinate.format(coordinate, coordinateTemplate, 0);
            return ret;
        },
        projection: displayPrj,
        className: 'custom-mouse-position',
        target: document.getElementById('mouse-position'),
        undefinedHTML: '&nbsp;'
    });

    map.addControl(mousePositionControl);

    // 初始化底圖
    //initBaseLayers();


    if (parent) {
        try {
            parent.mapPlugin = mapPlugin;
        } catch (err) {
            console.error(err);
        }
    }

    identifyFeatures = {};
    //mapPlugin.layersMg.onIdentifyFinish(
    //    function (coord, featuresPerLayer) {
    //        identifyFeatures = featuresPerLayer;
    //        var downWindow = $("#mis_down_contentFrame")[0].contentWindow;
    //        downWindow.onIdentifyFinish(coord, featuresPerLayer);
    //        openMisFunction('Bottom');
    //    }
    //);

    mapUI = new gsmap.ui.MapUI(
        mapPlugin,
        {
            functions: _functions || enableFunctions || "",
            baseMapData: map_base
        }
    );

    //funcIdentify = new gsmap.func.Identify(mapPlugin, { mapUI: mapUI });
    mapPlugin.layersMg.activeIdentify();

    mapPlugin.layersMg.switchTheOverviewMap(
        map_base["通用版電子地圖"],
        true,
        document.getElementById("OverviewMap"),
        function (dom, layer) {

            // adjust UI
            if ($(window).width() <= 979) {
                $(".ol-overviewmap").css("display", "none");
                $("#labelLegend").css("display", "none");
                $("#btnLegend").css("display", "none");
                //$(".nav_08").css("display", "none");
                //$(".nav_06").css("display", "none");
                $("#mis_left_icon").css('display', 'none');
                $("#mis_left_content").css('display', 'none');
                $("#mis_down_content").css("width", "100%");
                $("#mis_down_icon2").css("left", "216px");
                $("#mis_down_icon1").css("left", "180px");
                $(".ol-rotate")[0].style.top = '8.5em';
            }
            $('#OverviewMap').find('button')[0].style.display = 'none';

            $("#OverviewMap").find('button').on('click', function () {
                if ($("#OverviewMap").find('button').find('span')[0].innerText == "»") {
                    $("#OverviewMap")[0].onmouseover = "";
                    $("#OverviewMap")[0].onmouseout = "";
                } else {
                    $("#OverviewMap")[0].onmouseover = function () {
                        $('#OverviewMap').find('button')[0].style.display = '';
                    };

                    $("#OverviewMap")[0].onmouseout = function () {
                        $('#OverviewMap').find('button')[0].style.display = 'none';
                    };
                }
            });

            document.getElementById("OverviewMap").style.right = "180px";
            document.getElementById("OverviewMap").style.bottom = "8em";
            document.getElementById("OverviewMap").style.left = "unset";
        }
    );

    $(".gsmap-mapenv-info").html("坐標系統－<span style='text-decoration:underline'>" + displayPrj + "</sapn>");


    mapPlugin.layersMg.setBaseLayer(map_base["開放街圖(標準)"], true);

    // 添加活動圖層的滑鼠 hover 事件
    var activityTooltipElement = document.createElement('div');
    activityTooltipElement.className = 'activity-tooltip';
    activityTooltipElement.style.cssText = 'position: fixed; background: rgba(0, 0, 0, 0.85); color: white; padding: 8px 12px; border-radius: 4px; font-size: 13px; pointer-events: none; white-space: nowrap; display: none; z-index: 9999; box-shadow: 0 2px 4px rgba(0,0,0,0.3); line-height: 1.5;';
    document.body.appendChild(activityTooltipElement);
    
    map.on('pointermove', function(evt) {
        // 檢查是否在活動圖層上
        var pixel = evt.pixel;
        var feature = null;
        
        // 先嘗試取得活動圖層
        var activityLayer = mapPlugin.layersMg.getLayer('sys/activity');
        
        // 直接使用 forEachFeatureAtPixel，不管圖層
        feature = map.forEachFeatureAtPixel(pixel, function(f, l) {
            // 檢查 feature 是否有活動相關屬性
            if (f && f.get('reportingUnit') && f.get('activityName')) {
                return f;
            }
        });
        
        // 如果還是找不到，嘗試從活動圖層的 source 直接取得
        if (!feature && activityLayer) {
            var source = activityLayer.getSource();
            if (source) {
                var closestFeature = source.getClosestFeatureToCoordinate(evt.coordinate);
                if (closestFeature) {
                    // 檢查距離是否在合理範圍內（10 pixels）
                    var featurePixel = map.getPixelFromCoordinate(closestFeature.getGeometry().getClosestPoint(evt.coordinate));
                    var distance = Math.sqrt(Math.pow(pixel[0] - featurePixel[0], 2) + Math.pow(pixel[1] - featurePixel[1], 2));
                    if (distance < 10) {
                        feature = closestFeature;
                    }
                }
            }
        }

        if (feature && mapPlugin.layersMg.isLayerTurnOn("sys/activity")) {
            // 顯示 tooltip
            var unit = feature.get('reportingUnit') || '未知單位';
            var activity = feature.get('activityName') || '未知活動';
            activityTooltipElement.innerHTML = unit + '<br>' + activity;
            activityTooltipElement.style.display = 'block';
            
            // 設定 tooltip 位置（使用滑鼠位置）
            activityTooltipElement.style.left = (evt.originalEvent.clientX + 15) + 'px';
            activityTooltipElement.style.top = (evt.originalEvent.clientY - 40) + 'px';
            
            // 改變滑鼠游標
            map.getTargetElement().style.cursor = 'pointer';
        } else {
            // 隱藏 tooltip
            activityTooltipElement.style.display = 'none';
            map.getTargetElement().style.cursor = '';
        }
    });
    
    // 當滑鼠離開地圖時隱藏 tooltip
    map.getTargetElement().addEventListener('mouseleave', function() {
        activityTooltipElement.style.display = 'none';
    });

    // 測試MarkerLayer設定
    //mapPlugin.addMarker([121, 24], "", { circle: { radius: 20, stroke: { color: "#FF0000", width: 5 } } }, "EPSG:4326", "test");
    //mapPlugin.setMarkerLayer("test", { MinResolution: 10 });

    loadFeaturesFromWKT();
}

function loadFeaturesFromWKT() {
    // 1. 防呆：先檢查 initialWKT3826 是否存在、不是 null、不是空字串，也不是字面 "null"
    if (typeof window.initialWKT3826 === 'undefined' || window.initialWKT3826 === null) {
        return;
    }
    var wktText = window.initialWKT3826;
    if (typeof wktText !== 'string') {
        return;
    }
    wktText = wktText.trim();
    if (wktText.length === 0 || wktText.toLowerCase() === "null") {
        return;
    }

    // 2. 用 readGeometry 讀取 geometry（支援單一 geometry 或 GeometryCollection）
    var wktReader = new ol.format.WKT();
    var geometry;
    try {
        geometry = wktReader.readGeometry(wktText, {
            dataProjection: 'EPSG:3826',    // 資料是 3826 座標系統
            featureProjection: 'EPSG:3857'  // 地圖使用 3857 座標系統
        });
    }
    catch (err) {
        console.error("WKT 解析失敗：", err, wktText);
        return;
    }
    
    if (!geometry) {
        return;
    }

    // 3. 提取所有的幾何體
    var geometries = [];
    if (geometry.getType() === 'GeometryCollection') {
        // 如果是 GeometryCollection，取出所有子幾何體
        geometries = geometry.getGeometries();
    } else {
        // 如果是單一幾何體，放入陣列
        geometries = [geometry];
    }

    // 4. 暫時儲存當前的 drawInteraction 狀態
    var originalDrawInteraction = drawInteraction;
    
    // 5. 批次處理所有幾何體
    var successCount = 0;
    var errorCount = 0;
    
    geometries.forEach(function (geom, index) {
        try {
            // 5a. 建立 feature
            var feature = new ol.Feature({
                geometry: geom
            });
            
            // 5b. 設定唯一 ID
            var PK = CreateGuid();
            feature.setId(PK);

            // 5c. 判斷 geometry 類型
            var geomType = geom.getType();
            var selectedType;
            switch (geomType) {
                case 'Point':
                    selectedType = 'Point';
                    break;
                case 'LineString':
                    selectedType = 'LineString';
                    break;
                case 'Polygon':
                    selectedType = 'Polygon';
                    break;
                case 'MultiPoint':
                    selectedType = 'Point';
                    break;
                case 'MultiLineString':
                    selectedType = 'LineString';
                    break;
                case 'MultiPolygon':
                    selectedType = 'Polygon';
                    break;
                default:
                    console.warn('未知的幾何類型：', geomType);
                    selectedType = 'Polygon';
                    break;
            }

            // 5d. 加入到圖層
            drawSource.addFeature(feature);

            // 5e. 套用樣式和加入清單
            // 暫時設定對應的 interaction（但不實際使用）
            if (drawInteraction) {
                map.removeInteraction(drawInteraction);
            }
            addInteraction(selectedType, false);
            drawEnd(feature, geom, selectedType);
            
            successCount++;
        }
        catch (err) {
            errorCount++;
            console.error('處理第 ' + (index + 1) + ' 個幾何體時發生錯誤：', err);
        }
    });
    
    // 6. 清理並恢復狀態
    if (drawInteraction) {
        map.removeInteraction(drawInteraction);
        drawInteraction = null;
    }
    
    // 7. 顯示載入結果（可選）
    if (successCount > 0) {
        console.log('成功載入 ' + successCount + ' 個地理標記');
    }
    if (errorCount > 0) {
        console.warn('載入失敗 ' + errorCount + ' 個地理標記');
    }
}

function round(num, digit) {
    return Math.round(num * Math.pow(10, digit)) / Math.pow(10, digit);
}

// 開關底圖
function switchBaseLayer(layerName) {
    var theLayer = baseLayerInfos[layerName];
    if (theLayer == null) {
        alert("此圖層尚未介接完成或其圖資提供單位無法正常提供服務");
        return false;
    }
    for (var key in baseLayerInfos) {
        var tmpLayer = baseLayerInfos[key];
        // set index of layer
        //map.getLayers().setAt(99, tmpLayer);
    }
    if (theLayer.display) {
        mapPlugin.getBaseLayerGroup().getLayers().remove(baseLayerInfos[layerName].layer);
    } else {
        mapPlugin.getBaseLayerGroup().getLayers().push(baseLayerInfos[layerName].layer);
    }
    theLayer.display = !theLayer.display;
    return true;
}

function getBaseLayerOpacity(layerName) {
    return baseLayerInfos[layerName].layer.getOpacity();
}

function setBaseLayerOpacity(layerName, opacity) {
    var theLayer = baseLayerInfos[layerName].layer;
    if (theLayer == null) {
        return false;
    }
    if (opacity > 1) {
        opacity /= 100;
    }
    theLayer.setOpacity(opacity);
}

function changeBaseLayerPostion(layerName, targetPos) {
    var theLayer = baseLayerInfos[layerName];
    if (theLayer == null) {
        console.log("changeBaseLayerPostion(layerName, targetPos) -- 找不到此圖層[" + layerName + "]");
        return;
    }
    var layers = mapPlugin.getBaseLayerGroup().getLayers();
    layers.remove(baseLayerInfos[layerName].layer);
    layers.insertAt(layers.getArray().length - targetPos, baseLayerInfos[layerName].layer);
}


function getLayerInfo(path) {
    return eval("map_layers" + path);
}

function changeRadiusVal() {
    var layer = mapPlugin.layersMg.getLayer("災害/地震熱區圖");
    var radius = parent.document.getElementById("radius").value;
    layer.setRadius(parseInt(radius, 10));
}

function changeBlurVal() {
    var layer = mapPlugin.layersMg.getLayer("災害/地震熱區圖");
    var blur = parent.document.getElementById("blur").value;
    layer.setBlur(parseInt(blur, 10));
}

function genWMTSLayer(serviceInfo) {
    var layer;
    var result = serviceInfo.result;
    var options = ol.source.WMTS.optionsFromCapabilities(result,
        {
            layer: serviceInfo.params.layer,
            matrixSet: serviceInfo.matrixSet,
            projection: serviceInfo.targetPrj,
            requestEncoding: "RESTful",
        });

    layer = new ol.layer.Tile({
        source: new ol.source.WMTS(options)
    });

    layer.getSource().setTileLoadFunction(
        serviceInfo.useProxy ?
            function (imageTile, src) {
                imageTile.getImage().src = "service/proxy2.ashx?" + encodeURIComponent(src);
            } : undefined
    );
    return layer;
}

function clearAllMark() {
    drawSource.clear();
    kwSource.clear();
    if ($("#mis_left_content")[0].style.display == "block") {
        if (MarkList != undefined) {
            MarkList[0].innerHTML = "";
        }
        useMisFunctonFrame.CloseLeftFrame();
        $("#mis_left_icon_li").removeClass("close");
        $("#mis_left_icon_li").addClass("open");
    }

    if ($("#mis_down_content")[0].style.display == "block") {
        useMisFunctonFrame.CloseFullDownFrame();
        $("#mis_down_iconopen").removeClass("close");
        $("#mis_down_iconopen").addClass("open");
        $("#mis_down_iconclose").removeClass("open");
        $("#mis_down_iconclose").addClass("close");
    }

    pointNum = 0;
    lineNum = 0;
    polyNum = 0;
    txtNum = 0;

    mapPlugin.clearFeatures(spatialResultLayerName);
    mapPlugin.clearFeatures("default");
    pointMarker = null;
}

function deleteMark(divId, markNum) {
    drawSource.getFeatures().forEach(function (olFeature) {
        if (olFeature.getGeometry().getType() == "Point") {
            if (olFeature.get('num') == markNum) {
                drawSource.removeFeature(olFeature);
                $(divId).remove();
            }
        } else if (olFeature.getGeometry().getType() == "LineString") {
            if (olFeature.get('num') == markNum) {
                drawSource.removeFeature(olFeature);
                $(divId).remove();
            }
        } else if (olFeature.getGeometry().getType() == "Polygon") {
            if (olFeature.get('num') == markNum) {
                drawSource.removeFeature(olFeature);
                $(divId).remove();
            }
        }
        else if (olFeature.getGeometry().getType() == "Circle") {
            if (olFeature.get('num') == markNum) {
                drawSource.removeFeature(olFeature);
                $(divId).remove();
            }
        }
    });
}

function updateBottom() {
    var iframe = document.getElementById('mis_down_contentFrame');
    var innerDoc = (iframe.contentDocument) ? iframe.contentDocument : iframe.contentWindow.document;
    var CurrentTab = innerDoc.getElementsByClassName("active")[0];
    if (CurrentTab != undefined && CurrentTab.innerText == "圖層查詢") {
        CurrentTab.click();
    }
}

function openMarkList() {
    setTimeout(function () {
        if (drawSource.getFeatures().length == 0) {
            openMisFunction('MarkList');
        } else {
            useMisFunctonFrame.OpenLeftFrame();
        }
    }, 300);
}

//建立GUID
function CreateGuid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
}

