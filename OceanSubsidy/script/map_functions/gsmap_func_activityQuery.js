gs.Core.package("gsmap.func");

gsmap.func.ActivityQuery = function (_plugin, targetDom) {
    var plugin = _plugin;
    var $baseDom = $(targetDom);
    var $mainDom;
    var me = this;
    var currentSortField = null;  // 當前排序欄位
    var sortAscending = true;     // 排序方向
    var queryData = [];           // 儲存查詢結果資料
    var countyActivityData = {};  // 儲存縣市活動資料

    // 設定全域變數供 dataParser 使用
    window.countyActivityData = countyActivityData;

    // 讀取template
    $.ajax("map/template_activityQuery.html?" + Date.now()).done(
        function (data) {
            $mainDom = $(data);
            $baseDom.append($mainDom);
            if ($mainDom.width() > 0) {
                me.show();
            } else {
                me.hide();
            }

            init();

            initRWD();
        }
    );

    this.show = function () {
        page_active();
        if ($mainDom) {
            $mainDom.width("");
        }
    };

    this.hide = function () {
        page_leave();
        if ($mainDom)
            $mainDom.width(0);
    }

    function page_active() {
        resetRWD();
    }

    // 取得地圖物件
    function getMap() {
        return plugin.getMap ? plugin.getMap() : plugin.map;
    }

    function page_leave() {
        // 清理縣市圖層
        if (plugin.layersMg.isLayerTurnOn("sys/activityCounty")) {
            plugin.layersMg.switchTheLayer("sys/activityCounty", false);
            var map = getMap();
            if (map) {
                map.un('singleclick', handleCountyClick);
            }
        }

        // 隱藏說明框
        hideLegend();
    }

    function init() {
        // 綁定年份
        $mainDom.find("select._year").each(function () {
            fillDropdownList(this, "Service/KeyValueDataSource.ashx?data=year", false);
        });

        // 綁定單位
        $mainDom.find("select._unit").each(function () {
            var $select = $(this);
            fillDropdownList(this, "Service/KeyValueDataSource.ashx?data=unit", false, function() {
                // 填充完成後初始化 SumoSelect
                $select.SumoSelect({
                    selectAll: true,
                    selectAlltext: '全選',
                    search: true,
                    searchText: '搜尋...',
                    noMatch: '無符合項目',
                    locale: ['確定', '取消', '全選'],
                    captionFormat: '已選擇 {0} 個',
                    captionFormatAllSelected: '已選擇全部 ({0})',
                    okCancelInMulti: true,
                    csvDispCount: 2,
                    up: false
                });
                
                // 初始化後立即全選
                $select[0].sumo.selectAll();
            });
        });

        $mainDom.find(".btn_query").on('click', query);
        $mainDom.find(".closebtn").on("click", function () { me.hide(); });

        // 綁定排序功能
        $mainDom.on('click', '.sortable-header', function () {
            var sortField = $(this).data('sort');
            handleSort(sortField);
        });
    }

    // 取得查詢參數
    function getQueryParams() {
        return {
            yFrom: $mainDom.find("#yearFrom").val(),
            qFrom: $mainDom.find("#quarterFrom").val(),
            yTo: $mainDom.find("#yearTo").val(),
            qTo: $mainDom.find("#quarterTo").val(),
            units: $mainDom.find("#unit").val(),
            actName: $mainDom.find("#actName").val()
        };
    }

    // 建立查詢 URL
    function buildQueryUrl(baseUrl, params) {
        var url = baseUrl;
        if (params.yFrom) url += "&yearFrom=" + encodeURIComponent(params.yFrom);
        if (params.qFrom) url += "&quarterFrom=" + encodeURIComponent(params.qFrom);
        if (params.yTo) url += "&yearTo=" + encodeURIComponent(params.yTo);
        if (params.qTo) url += "&quarterTo=" + encodeURIComponent(params.qTo);
        if (params.units && params.units.length > 0) {
            url += "&unit=" + params.units.map(encodeURIComponent).join(",");
        }
        if (params.actName) url += "&actName=" + encodeURIComponent(params.actName);
        return url;
    }

    function query() {
        // 清空查詢資料快取
        queryData = [];

        plugin.layersMg.switchTheLayer("sys/activity", false);
        plugin.layersMg.switchTheLayer("sys/activityCounty", false);

        // 清空結果列表，顯示載入中
        var $resultList = $mainDom.find("#resultList");
        $resultList.empty();
        $resultList.append('<li class="list-group-item text-center py-3"><i class="fa fa-spinner fa-spin"></i> 查詢中...</li>');
        $mainDom.find("#result").html('查詢結果：<span class="activity-query-label fw-bold">查詢中...</span>');

        // 1. 先讀取所有 UI 參數
        var params = getQueryParams();

        // 2. 查詢縣市活動統計資料
        var queryUrl = buildQueryUrl("Service/OSI_CountyData.ashx?data=countyActivities", params);

        // 3. 設定縣市圖層 URL
        var svc = plugin.layersMg.getLayerInfo("sys/activityCounty").serviceInfo;
        svc.url = queryUrl;

        // 4. 設定縣市資料載入回呼函數
        window.onCountyDataLoaded = function (data) {
            // 更新結果顯示為縣市統計
            updateCountyResultList(data);

            // 顯示活動數量說明框
            showLegend();

            // 綁定點擊事件
            var map = getMap();
            if (map) {
                map.on('singleclick', handleCountyClick);
            }
        };

        // 5. 顯示縣市圖層
        if (!plugin.layersMg.isLayerTurnOn("sys/activityCounty")) {
            plugin.layersMg.switchTheLayer("sys/activityCounty", true);
        }
        plugin.layersMg.refreshLayer("sys/activityCounty");
    }


    // 處理縣市點擊事件
    function handleCountyClick(evt) {
        var map = getMap();
        if (!map) return;

        var countyLayer = plugin.layersMg.getLayer("sys/activityCounty");
        if (!countyLayer) return;

        var feature = map.forEachFeatureAtPixel(evt.pixel, function (feature) {
            return feature;
        }, {
            layerFilter: function (layer) {
                return layer === countyLayer;
            }
        });

        if (feature) {
            var countyId = feature.get('county_id');
            var countyName = feature.get('c_name');
            var reportIds = feature.get('report_ids');
            var activityCount = feature.get('activity_count');

            if (reportIds && reportIds.length > 0) {
                // 顯示該縣市的活動 Features（使用快取資料）
                showCountyActivities(true);

                // Zoom 進點擊的縣市
                var countyExtent = feature.getGeometry().getExtent();
                if (countyExtent && !ol.extent.isEmpty(countyExtent)) {
                    map.getView().fit(countyExtent, {
                        padding: [50, 50, 50, 50],
                        duration: 1000
                    });
                }
            }
        }
    }

    // 顯示縣市的活動
    function showCountyActivities(useExistingData) {
        // 預設使用快取資料
        if (useExistingData === undefined) {
            useExistingData = true;
        }

        // 隱藏縣市圖層
        if (plugin.layersMg.isLayerTurnOn("sys/activityCounty")) {
            plugin.layersMg.switchTheLayer("sys/activityCounty", false);
            hideLegend();
            var map = getMap();
            if (map) {
                map.un('singleclick', handleCountyClick);
            }
        }

        // 如果有已存在的資料且要求使用它
        if (useExistingData && queryData && queryData.length > 0) {
            // 取得活動圖層
            var activityLayer = plugin.layersMg.getLayer("sys/activity");

            // 如果圖層已存在，先清除舊資料
            if (activityLayer) {
                var source = activityLayer.getSource();
                if (source) {
                    source.clear();
                }
            }

            // 取得活動圖層的 serviceInfo
            var layerInfo = plugin.layersMg.getLayerInfo("sys/activity");
            var svc = layerInfo.serviceInfo;

            // 直接使用 dataParser 處理 queryData
            if (svc.dataParser && typeof svc.dataParser === 'function') {
                var features = svc.dataParser(queryData);

                // 如果圖層不存在或未開啟，先開啟它
                if (!plugin.layersMg.isLayerTurnOn("sys/activity")) {
                    plugin.layersMg.switchTheLayer("sys/activity", true);
                    activityLayer = plugin.layersMg.getLayer("sys/activity");
                }

                // 將 features 加到圖層
                if (activityLayer) {
                    var source = activityLayer.getSource();
                    if (source && features.length > 0) {
                        source.addFeatures(features);
                    }
                }
            }
        } else {
            // 原本的邏輯：呼叫 API
            var params = getQueryParams();
            var svc = plugin.layersMg.getLayerInfo("sys/activity").serviceInfo;
            svc.url = buildQueryUrl("Service/OSI_ActivityData.ashx?data=activity", params);

            if (!plugin.layersMg.isLayerTurnOn("sys/activity")) {
                plugin.layersMg.switchTheLayer("sys/activity", true);
            }
            plugin.layersMg.refreshLayer("sys/activity");
        }

        // 隱藏說明框
        hideLegend();
    }

    // 更新縣市結果列表 - 改為顯示活動資訊
    function updateCountyResultList(countyData) {
        // 取得所有縣市的 report IDs
        var allReportIds = [];
        countyData.forEach(function (county) {
            if (county.report_ids) {
                var ids = county.report_ids.split(',');
                allReportIds = allReportIds.concat(ids);
            }
        });

        // 去除重複
        allReportIds = [...new Set(allReportIds)];

        if (allReportIds.length === 0) {
            $mainDom.find("#result").html('查詢結果：共 <span class="activity-query-label fw-bold">0</span> 筆活動');
            var $resultList = $mainDom.find("#resultList");
            $resultList.empty();
            $resultList.append('<li class="list-group-item text-center py-3">無符合條件的查詢結果</li>');
            return;
        }

        // 呼叫 OSI_ActivityData.ashx 取得活動詳細資料
        var params = getQueryParams();
        var activityUrl = buildQueryUrl("Service/OSI_ActivityData.ashx?data=activity", params);

        // 加入所有的 report IDs
        activityUrl += "&reportIds=" + allReportIds.join(",");

        $.ajax({
            url: activityUrl,
            type: 'GET',
            dataType: 'json',
            success: function (activities) {
                // 儲存查詢結果
                queryData = activities;

                // 更新結果數量
                $mainDom.find("#result").html(
                    '查詢結果：共 <span class="activity-query-label fw-bold">' + activities.length + '</span> 筆活動'
                );

                // 清空現有列表
                var $resultList = $mainDom.find("#resultList");
                $resultList.empty();

                // 如果沒有結果
                if (activities.length === 0) {
                    $resultList.append('<li class="list-group-item text-center py-3">無符合條件的查詢結果</li>');
                    return;
                }

                // 顯示活動列表
                displayActivityList($resultList, activities);
            },
            error: function (xhr, status, error) {
                console.error("查詢活動資料失敗:", error);               
                $mainDom.find("#result").html('查詢結果：共 <span class="activity-query-label fw-bold">0</span> 筆活動');
                var $resultList = $mainDom.find("#resultList");
                $resultList.empty();
            }
        });
    }

    // 顯示活動數量說明框
    function showLegend() {
        $("#activityLegend").show();
    }

    // 隱藏活動數量說明框
    function hideLegend() {
        $("#activityLegend").hide();
    }

    // Feature定位到地圖上
    function locateActivity(wkt, reportId, color) {
        if (!wkt) {
            console.log("無法定位：缺少 WKT 資料");
            return;
        }

        var map = getMap();
        if (!map) {
            console.error("無法定位：找不到地圖物件");
            return;
        }

        try {
            // 使用 OpenLayers 的 WKT 格式解析器
            var format = new ol.format.WKT();
            var extent = null;
            var features = [];

            // 處理 GEOMETRYCOLLECTION
            if (wkt.indexOf('GEOMETRYCOLLECTION') === 0) {
                var geomCollection = format.readGeometry(wkt, {
                    dataProjection: 'EPSG:3826',  // TWD97 / TM2 zone 121
                    featureProjection: 'EPSG:3857'  // Web Mercator
                });

                // 取得所有子幾何
                var geometries = geomCollection.getGeometries();
                extent = ol.extent.createEmpty();

                geometries.forEach(function (geom) {
                    var feature = new ol.Feature({
                        geometry: geom
                    });
                    features.push(feature);
                    ol.extent.extend(extent, geom.getExtent());
                });
            } else {
                // 處理一般的幾何類型
                var feature = format.readFeature(wkt, {
                    dataProjection: 'EPSG:3826',  // TWD97 / TM2 zone 121
                    featureProjection: 'EPSG:3857'  // Web Mercator
                });

                if (feature) {
                    features.push(feature);
                    extent = feature.getGeometry().getExtent();
                }
            }

            // 顯示活動圖層（使用快取資料）
            showCountyActivities(true);

            if (extent && !ol.extent.isEmpty(extent)) {
                // Zoom 到點擊的 Feature
                map.getView().fit(extent, {
                    padding: [50, 50, 50, 50],  // 留些邊距
                    duration: 1000  // 動畫時間
                });
            }
        } catch (e) {
            console.error("解析 WKT 失敗:", e);
            console.error("WKT:", wkt);
        }
    }
    function initRWD() {
        // 手機模式下執行
        if ($(window).width() <= 500) {
            // 固定面板
            $mainDom.find(".fixedbtn").click(function (e) {
                e.preventDefault();
                $baseDom.removeClass("toggled");
                $mainDom.removeClass("mapnavOpen");

                $baseDom.addClass("_fixedpanel");
                resetRWD();
            });
            // 恢復面板
            $mainDom.find(".revertbtn").click(function (e) {
                e.preventDefault();
                $baseDom.removeClass("_fixedpanel");
                resetRWD();
            });

            resetRWD();
        }
    }

    function resetRWD() {
        if (!$mainDom) return;

        // 固定按鈕顯示
        if ($baseDom.hasClass("_fixedpanel")) {
            $mainDom.addClass("fixedpanel");
            $mainDom.find(".fixedbtn").hide();
            $mainDom.find(".revertbtn").show();
        } else {
            $mainDom.removeClass("fixedpanel");
            $mainDom.find(".fixedbtn").show();
            $mainDom.find(".revertbtn").hide();
        }

        if ($baseDom.hasClass("toggled")) {
            $mainDom.addClass("mapnavOpen");
        }
        else {
            $mainDom.removeClass("mapnavOpen");
        }
    }

    // 處理排序
    function handleSort(field) {
        // 如果沒有資料，不處理
        if (queryData.length === 0) return;

        // 如果點擊的是同一個欄位，切換排序方向
        if (currentSortField === field) {
            sortAscending = !sortAscending;
        } else {
            currentSortField = field;
            sortAscending = true;
        }

        // 排序資料
        var sortedData = queryData.slice(); // 複製陣列
        sortedData.sort(function (a, b) {
            var valueA, valueB;

            if (field === 'unit') {
                valueA = a.reportingUnit || '';
                valueB = b.reportingUnit || '';
            } else if (field === 'activity') {
                valueA = a.activityName || '';
                valueB = b.activityName || '';
            }

            // 字串比較
            if (sortAscending) {
                return valueA.localeCompare(valueB, 'zh-TW');
            } else {
                return valueB.localeCompare(valueA, 'zh-TW');
            }
        });

        // 更新排序圖示
        updateSortIcons();

        // 重新顯示列表
        displayResultList(sortedData);
    }

    // 更新排序圖示
    function updateSortIcons() {
        // 重設所有圖示
        $mainDom.find('.sort-icon').removeClass('active').html('▼');

        // 更新當前排序欄位的圖示
        if (currentSortField) {
            var $icon = $mainDom.find('#sort-' + currentSortField);
            $icon.addClass('active');
            $icon.html(sortAscending ? '▲' : '▼');
        }
    }

    // 顯示結果列表
    function displayResultList(data) {
        var $resultList = $mainDom.find("#resultList");
        displayActivityList($resultList, data);
    }

    // 顯示活動列表的共用函數
    function displayActivityList($container, activities) {
        $container.empty();

        activities.forEach(function (item) {
            var listItem = `
                <li class="list-group-item d-flex justify-content-between align-items-start py-2"
                    data-reportid="${item.reportID}" 
                    data-wkt="${item.wkt}" 
                    data-color="${item.color || '#3388ff'}">
                    <div class="me-2" style="width: 30%;">
                        <div class="unit-name">${item.reportingUnit || '未知單位'}</div>
                    </div>
                    <div class="flex-grow-1 me-2" style="width: 55%;">
                        <div class="activity-name">
                            ${item.activityName}
                        </div>
                    </div>
                    <div class="d-flex justify-content-center align-items-center" style="width: 15%;">
                        <input type="button" name="" class="btn_pin_circle locate-btn" value="">
                    </div>
                </li>
            `;
            $container.append(listItem);
        });

        // 綁定定位按鈕點擊事件
        $container.find(".locate-btn").on("click", function () {
            var $li = $(this).closest("li");
            var wkt = $li.data("wkt");
            var reportId = $li.data("reportid");
            var color = $li.data("color");

            if (wkt) {
                locateActivity(wkt, reportId, color);
            }
        });
    }
}
