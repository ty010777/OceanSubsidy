/// <reference path="../../MapMaster.master" />
/*****

*****/
var chiayiwaterLegendURL = "service/proxy2.ashx?" + gisServerURL + "ows?service=WMS&request=GetLegendGraphic&format=image%2Fpng&width=18&height=18&legend_options=forceLabels:off&layer=chiayiwater%3A";
var chiayiwaterLargeLegendURL = gisServerURL + "ows?service=WMS&request=GetLegendGraphic&format=image%2Fpng&width=26&height=26&legend_options=forceLabels:off&layer=chiayiwater%3A";
var treeLayerSelectInteraction = null;

var gisLegendURL = gisServerURL + "ows?service=WMS&request=GetLegendGraphic&format=image%2Fpng&width=18&height=18&legend_options=forceLabels:off&layer=OceanSubsidy%3A";

var map_layers =
{
    "行政界": {
        "name": "行政界",
        "type": "folder",
        "sub": {
            "0CAD8E29-739F-4CE6-B614-972A329DCA10": {
                "name": "縣市界",
                "type": "node",
                "legendIcon": gisLegendURL + "map_County",
                "maxLevel": "16",
                "geometryType": "polygon",
                "serviceInfo": {
                    "type": "WMS",
                    "url": gisServerURL + "ows",
                    "targetPrj": "EPSG:3826",
                    "params": {
                        "version": "1.3.0",
                        "layers": "OceanSubsidy:map_County",
                        "styles": "",
                        FORMAT: 'image/png8'
                    },
                }
            },
            "993E80BD-857F-4CEC-8CBB-52FBCCCD4377": {
                "name": "鄉鎮界",
                "type": "node",
                "legendIcon": gisLegendURL + "map_Town",
                "minLevel": "10",
                "geometryType": "polygon",
                "serviceInfo": {
                    "type": "WMS",
                    "url": gisServerURL + "ows",
                    "targetPrj": "EPSG:3826",
                    "params": {
                        "version": "1.3.0",
                        "layers": "OceanSubsidy:map_Town",
                        "styles": "",
                        FORMAT: 'image/png8'
                    },
                }
            },
            "E440EF52-AC4C-47D4-A367-CE2938DD489D": {
                "name": "村里界",
                "type": "node",
                "legendIcon": gisLegendURL + "map_Village",
                "minLevel": "14",
                "geometryType": "polygon",
                "serviceInfo": {
                    "type": "WMS",
                    "url": gisServerURL + "ows",
                    "targetPrj": "EPSG:3826",
                    "params": {
                        "version": "1.3.0",
                        "layers": "OceanSubsidy:map_Village",
                        "styles": "",
                        FORMAT: 'image/png8'
                    }
                }
            }
        }
    },
    "重要區域": {
        name: "重要區域",
        type: "folder",
        sub: {
            "全國縣市海域分區": {
                name: "全國縣市海域分區",
                type: "node",
                legendIcon: gisLegendURL + "map_CountyOceanSection",
                geometryType: "polygon",
                serviceInfo: {
                    type: "WMS",
                    url: gisServerURL + "ows",
                    targetPrj: "EPSG:3826",
                    params: {
                        VERSION: "1.3.0",
                        layers: "OceanSubsidy:map_CountyOceanSection",
                        FORMAT: 'image/png8'
                    }
                }
            },
            "3浬範圍": {
                name: "3浬範圍",
                type: "node",
                legendIcon: gisLegendURL + "map_3SeaMileArea",
                geometryType: "polygon",
                serviceInfo: {
                    type: "WMS",
                    url: gisServerURL + "ows",
                    targetPrj: "EPSG:3826",
                    params: {
                        VERSION: "1.3.0",
                        layers: "OceanSubsidy:map_3SeaMileArea",
                        FORMAT: 'image/png8'
                    }
                }
            },
            "12浬範圍": {
                name: "12浬範圍",
                type: "node",
                legendIcon: gisLegendURL + "map_12SeaMileline",
                geometryType: "polygon",
                serviceInfo: {
                    type: "WMS",
                    url: gisServerURL + "ows",
                    targetPrj: "EPSG:3826",
                    params: {
                        VERSION: "1.3.0",
                        layers: "OceanSubsidy:map_12SeaMileline",
                        FORMAT: 'image/png8'
                    }
                }
            },
            "24浬範圍": {
                name: "24浬範圍",
                type: "node",
                legendIcon: gisLegendURL + "map_12SeaMileline",
                geometryType: "polygon",
                serviceInfo: {
                    type: "WMS",
                    url: gisServerURL + "ows",
                    targetPrj: "EPSG:3826",
                    params: {
                        VERSION: "1.3.0",
                        layers: "OceanSubsidy:map_24SeaMileline",
                        FORMAT: 'image/png8'
                    }
                }
            },
            "海岸地區範圍": {
                name: "海岸地區範圍",
                type: "node",
                legendIcon: gisLegendURL + "map_CoastArea",
                geometryType: "polygon",
                serviceInfo: {
                    type: "WMS",
                    url: gisServerURL + "ows",
                    targetPrj: "EPSG:3826",
                    params: {
                        VERSION: "1.3.0",
                        layers: "OceanSubsidy:map_CoastArea",
                        FORMAT: 'image/png8'
                    },
                }
            },
        }
    },
    "sys": {
        name: "sys",
        type: "folder",
        hidden: true,
        sub: {
            "activityCounty": {
                name: "活動縣市統計",
                type: "node",
                geometryType: "polygon",
                zIndex: 1000,
                serviceInfo: {
                    type: "vector",
                    url: "Service/OSI_CountyData.ashx?data=countyActivities",
                    strategy: "all",
                    dataParser: function (data) {
                        var features = [];
                        var format = new ol.format.WKT();
                        
                        // 更新全域的 countyActivityData 變數（如果存在）
                        if (window.countyActivityData) {
                            window.countyActivityData = {};
                            if (Array.isArray(data)) {
                                data.forEach(function(item) {
                                    window.countyActivityData[item.county_id] = item;
                                });
                            }
                        }
                        
                        // 觸發縣市資料載入事件（如果定義了處理函數）
                        if (window.onCountyDataLoaded && typeof window.onCountyDataLoaded === 'function') {
                            window.onCountyDataLoaded(data);
                        }
                        
                        if (Array.isArray(data)) {
                            data.forEach(function(countyInfo) {
                                if (countyInfo.geom && countyInfo.activity_count > 0) {
                                    try {
                                        // SRID 3826 (TWD97 / TM2 zone 121) 轉換到 Web Mercator (3826)
                                        var feature = format.readFeature(countyInfo.geom, {
                                            dataProjection: 'EPSG:3826',
                                            featureProjection: 'EPSG:3857'
                                        });
                                        
                                        feature.setProperties({
                                            county_id: countyInfo.county_id,
                                            c_name: countyInfo.c_name,
                                            activity_count: countyInfo.activity_count,
                                            report_ids: countyInfo.report_ids,
                                            color: countyInfo.color
                                        });
                                        
                                        features.push(feature);
                                    } catch (e) {
                                        console.error("解析縣市 WKT 失敗:", countyInfo.c_name, e);
                                    }
                                }
                            });
                        }
                        
                        return features;
                    },
                    style: function(feature, resolution) {
                        var color = feature.get('color') || 'transparent';
                        var activityCount = feature.get('activity_count') || 0;
                        
                        // 根據活動數量設定透明度
                        var opacity = activityCount > 0 ? 0.6 : 0;
                        
                        // 轉換顏色為 rgba
                        var fillColor = color;
                        if (color !== 'transparent' && color.startsWith('#')) {
                            var r = parseInt(color.substr(1, 2), 16);
                            var g = parseInt(color.substr(3, 2), 16);
                            var b = parseInt(color.substr(5, 2), 16);
                            fillColor = 'rgba(' + r + ',' + g + ',' + b + ',' + opacity + ')';
                        }
                        
                        return new ol.style.Style({
                            fill: new ol.style.Fill({
                                color: color === 'transparent' ? 'transparent' : fillColor
                            }),
                            stroke: new ol.style.Stroke({
                                color: '#2c3e50',
                                width: 2,
                                lineDash: activityCount === 0 ? [5, 5] : null
                            }),
                            text: new ol.style.Text({
                                text: feature.get('c_name'),
                                font: 'bold 12px Microsoft JhengHei',
                                fill: new ol.style.Fill({
                                    color: '#2c3e50'
                                }),
                                stroke: new ol.style.Stroke({
                                    color: '#ffffff',
                                    width: 3
                                }),
                                overflow: true
                            })
                        });
                    }
                }
            },
            "activity": {
                name: "活動查詢結果",
                type: "node",
                geometryType: "point",  // 使用 point 類型，樣式中再根據實際幾何類型調整
                zIndex: 1000,  // 設定高 zIndex 確保顯示在最上層
                serviceInfo: {
                    type: "vector",
                    url: "Service/OSI_ActivityData.ashx",
                    strategy: "all",  // 使用 all 策略，只載入一次
                    dataParser: function (data) {
                        // 更新查詢結果列表（如果有定義 updateActivityResultList 函數）
                        if (window.updateActivityResultList && typeof window.updateActivityResultList === 'function') {
                            window.updateActivityResultList(data);
                        }

                        // 將查詢結果轉換為 OpenLayers features
                        var features = [];
                        
                        if (Array.isArray(data)) {
                            data.forEach(function(item) {
                                if (item.wkt) {
                                    try {
                                        var format = new ol.format.WKT();
                                        
                                        // 處理 GEOMETRYCOLLECTION
                                        if (item.wkt.indexOf('GEOMETRYCOLLECTION') === 0) {
                                            // 解析 GEOMETRYCOLLECTION
                                            var geomCollection = format.readGeometry(item.wkt, {
                                                dataProjection: 'EPSG:3826',
                                                featureProjection: 'EPSG:3857'
                                            });
                                            
                                            // 取得所有子幾何
                                            var geometries = geomCollection.getGeometries();
                                            
                                            // 為每個子幾何建立一個 feature
                                            geometries.forEach(function(geom, index) {
                                                var feature = new ol.Feature({
                                                    geometry: geom
                                                });
                                                
                                                // 將其他屬性加到 feature
                                                feature.setProperties({
                                                    reportID: item.reportID + '_' + index,
                                                    reportingUnit: item.reportingUnit,
                                                    activityName: item.activityName,
                                                    color: item.color || '#3388ff',
                                                    originalReportID: item.reportID
                                                });
                                                
                                                features.push(feature);
                                            });
                                        } else {
                                            // 處理一般的幾何類型
                                            var feature = format.readFeature(item.wkt, {
                                                dataProjection: 'EPSG:3826',
                                                featureProjection: 'EPSG:3857'
                                            });
                                            
                                            // 將其他屬性加到 feature
                                            feature.setProperties({
                                                reportID: item.reportID,
                                                reportingUnit: item.reportingUnit,
                                                activityName: item.activityName,
                                                color: item.color || '#3388ff'
                                            });
                                            
                                            features.push(feature);
                                        }
                                    } catch (e) {
                                        console.error("解析 WKT 失敗 for " + item.activityName + ":", e);
                                        console.error("WKT:", item.wkt);
                                    }
                                }
                            });
                        }
                        
                        return features;
                    },
                    style: function(feature, resolution) {
                        // 根據幾何類型設定不同的樣式
                        var geometry = feature.getGeometry();
                        var type = geometry.getType();
                        var color = feature.get('color') || '#3388ff';
                        
                        // 將顏色轉換為 rgba 格式（用於半透明填充）
                        var fillColor = color;
                        if (color.startsWith('#')) {
                            var r = parseInt(color.substr(1, 2), 16);
                            var g = parseInt(color.substr(3, 2), 16);
                            var b = parseInt(color.substr(5, 2), 16);
                            fillColor = 'rgba(' + r + ',' + g + ',' + b + ', 0.3)';
                        }
                        
                        if (type === 'Point' || type === 'MultiPoint') {
                            return new ol.style.Style({
                                image: new ol.style.Circle({
                                    radius: 8,
                                    fill: new ol.style.Fill({
                                        color: color
                                    }),
                                    stroke: new ol.style.Stroke({
                                        color: '#ffffff',
                                        width: 2
                                    })
                                }),
                                zIndex: 1000
                            });
                        } else if (type === 'LineString' || type === 'MultiLineString') {
                            return new ol.style.Style({
                                stroke: new ol.style.Stroke({
                                    color: color,
                                    width: 4
                                }),
                                zIndex: 1000
                            });
                        } else if (type === 'Polygon' || type === 'MultiPolygon') {
                            return new ol.style.Style({
                                stroke: new ol.style.Stroke({
                                    color: color,
                                    width: 3
                                }),
                                fill: new ol.style.Fill({
                                    color: fillColor
                                }),
                                zIndex: 1000
                            });
                        }
                    }
                }
            }
        }
    },
};