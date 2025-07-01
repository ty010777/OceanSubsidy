/**
* @class
* @summary 圖台定位工具
* @constructor
* @param {oltmx.Plugin} _mapPlugin - TMD Map Plugin
*/
oltmx.util.Locate = function (mapPlugin, options) {
    var plugin = mapPlugin;
    var map = plugin.getMap();
    var layerSource = new ol.source.Vector();
    var locationStyle = new ol.style.Style({
        image: new ol.style.Icon({
            anchor: [10, 32],
            anchorXUnits: 'pixels',
            anchorYUnits: 'pixels',
            opacity: 0.75,
            src: "App_themes/Map/images/location.png"
        }),
        stroke: new ol.style.Stroke({
            color: "rgba(32,32,255,0.8)",
            width: 3
        }),
        fill: new ol.style.Fill({
            color: "rgba(128,128,255,0.2)"
        })
    });

    if (options.locatingStyle) {
        locationStyle = plugin.getStyleFromOption(options.locatingStyle);
    }

    var layer = new ol.layer.Vector({
        source: layerSource,
        style: locationStyle.clone(),
        // TODO ol7定位後，圖層會跑到底圖下面，暫時以此方式處理，確定ol7新的圖層順序機制後，再確定有沒有需要以更好的方式處理。
        zIndex: 99999
    });
    map.addLayer(layer);

    var createLabelStyle = function (text) {
        var textStyle = locationStyle.getText().clone();
        textStyle.setText(text);
        var styles = [
            new ol.style.Style({
                image: locationStyle.getImage().clone(),
                text: textStyle
            })
        ];
        return styles;
    };

    /**
    * @summary 回傳定位圖徵使用的layer
    * @return {ol.Layer} 定位圖徵使用的layer
    * @private
    */
    this.getLayer = function () {
        return layer;
    };

    var me = this;
    // 依照WKT定位，並放入定位圖徵
    /**
    * @summary 依照WKT定位，並放入定位圖徵
    * @param sWKT {string} 向量位置的WKT
    * @param srcPrj {string} sWKT的坐標系統
    * @param bShowFeature {boolean} 是否顯示定位圖徵
    * @param text {string} 定位圖徵顯示的文字
    * @return {ol.Extent} 定位圖徵的Envelop
    */
    this.locateToWKT = function (sWKT, srcPrj, bShowFeature, text) {
        var ret = {
            extent: null
        };
        var wkt = new ol.format.WKT();
        var extent = ol.extent.createEmpty();
        var geom;

        // TODO 這會移除array，後面多WKT的判斷就會無效
        if ($.isArray(sWKT) && sWKT.length == 1) {
            sWKT = sWKT[0];
        }
        var mapPrj = map.getView().getProjection();
        me.clearLocatedFeature();
        if ($.isArray(sWKT)) {
            var geom = new ol.geom.GeometryCollection();
            var arGeom = [];
            for (var i = 0; i < sWKT.length; i++) {
                var feature = wkt.readFeature(sWKT[i], { dataProjection: srcPrj, featureProjection: mapPrj });
                if (bShowFeature) {
                    layerSource.addFeature(feature);
                }
                arGeom.push(feature.getGeometry());
            }
            geom.setGeometries(arGeom);
            extent = geom.getExtent();
            ret.extent = extent;
            map.getView().fit(extent, map.getSize());
            map.getView().setZoom(map.getView().getZoom() - 1);
        } else {
            var feature = wkt.readFeature(sWKT, { dataProjection: srcPrj, featureProjection: mapPrj });
            var labelPoint = createLabelFeature(feature);
            labelPoint.setStyle(createLabelStyle(text));
            //if (feature.getGeometry().getType() == 'Point') {
            //    labelPoint.getStyle()[0].getImage().setOpacity(0.75);
            //}
            me.locateToFeature(feature, bShowFeature);
            layerSource.addFeature(labelPoint);
            ret.extent = feature.getGeometry().getExtent();
        }
        return ret;
    };

    // 依照點坐標定位
    // coord : 為array，length == 2
    // srcPrj : coord的坐標系統
    /**
    * @summary 依照指定的點坐標，並放入定位圖徵
    * @param coord {number[]} 點坐標位置
    * @param srcPrj {string} 點坐標的坐標系統
    * @param bShowFeature {boolean} 是否顯示定位圖徵
    * @param text {string} 定位圖徵顯示的文字
    * @return {ol.Extent} 定位圖徵的Envelop
    */
    this.locateToCoord = function (coord, srcPrj, bShowFeature, text) {
        var targetCoord = coord;
        if (srcPrj) {
            targetCoord = ol.proj.transform(coord, srcPrj, map.getView().getProjection());
        }
        if (bShowFeature) {
            this.clearLocatedFeature();
            var feature = new ol.Feature({ geometry: new ol.geom.Point(targetCoord) });
            feature.setStyle(createLabelStyle(text));
            //feature.getStyle()[0].getImage().setOpacity(1);
            //feature.getStyle()[0].getText().setScale(0.8);
            //feature.getStyle()[0].getText().getStroke().setWidth(6.5);
            layerSource.addFeature(feature);
        }
        map.getView().setZoom(plugin.getOption("locatingPointZoom"));
        map.getView().setCenter(targetCoord);
    };

    /**
    * @summary 依照ol.geom.Geometry物件來定位，並放入定位圖徵
    * @param _geom {ol.geom.Geometry} 坐標位置
    * @param srcPrj {string} 坐標的坐標系統
    * @param bShowFeature {boolean} 是否顯示定位圖徵
    * @param text {string} 定位圖徵顯示的文字
    */
    this.locateToGeom = function (_geom, srcPrj, bShowFeature, text) {
        var feature = null;
        var labelPoint;
        me.clearLocatedFeature();
        var mapPrj = map.getView().getProjection();
        if ($.isArray(_geom)) {
            var geom = null;
            feature = [];
            for (var i = 0; i < _geom.length; i++) {
                if (srcPrj) {
                    geom = _geom[i].clone().transform(srcPrj, mapPrj);
                } else {
                    geom = _geom[i];
                }
                feature[i] = new ol.Feature({ geometry: geom });
            }
            labelPoint = createLabelFeature(feature);
            if (labelPoint.length > 0) {
                for (var i = 0; i < labelPoint.length; i++) {
                    labelPoint[i].setStyle(createLabelStyle(text));
                    //if (geom.getType() === 'Point') {
                    //    labelPoint[i].getStyle()[0].getImage().setOpacity(0.75);
                    //}
                    layerSource.addFeature(labelPoint[i]);
                }
                me.locateToFeature(feature, bShowFeature);
            }
        } else {
            if (srcPrj) {
                geom = _geom.clone().transform(srcPrj, mapPrj);
            }
            else {
                geom = _geom;
            }
            feature = new ol.Feature({ geometry: geom });
            labelPoint = createLabelFeature(feature);
            labelPoint.setStyle(createLabelStyle(text));
            //if (geom.getType() === 'Point') {
            //    labelPoint.getStyle()[0].getImage().setOpacity(0.75);
            //}
            me.locateToFeature(feature, bShowFeature);
            layerSource.addFeature(labelPoint);
        }
    };

    /**
    * @summary 依照gml內容定位，並放入定位圖徵
    * @param _gml {string} gml內容
    * @param srcPrj {string} 來源坐標系統
    * @param bShowFeature {boolean} 是否顯示定位圖徵
    * @param text {string} 定位圖徵顯示的文字
    */
    this.locateToGML = function (_gml, bShowFeature, text) {
        var features = oltmx.format.gml.readFeatures(_gml);
        var labelPoint = createLabelFeature(features);
        var style = createLabelStyle(text);
        if ($.isArray(labelPoint)) {
            labelPoint.forEach(function (f) { f.setStyle(style); });
        }
        else {
            labelPoint.setStyle(style);
        }
        //if (feature.getGeometry().getType() === 'Point') {
        //    labelPoint.getStyle()[0].getImage().setOpacity(0.75);
        //}
        me.locateToFeature(features, bShowFeature);
        layerSource.addFeature(labelPoint);
    };

    /**
    * @summary 依照ol.Feature物件來定位，並放入定位圖徵
    * @param _feature {ol.Feature} gml內容
    * @param bShowFeature {boolean} 是否顯示定位圖徵
    * @return {ol.Extent} 定位圖徵的Envelop
    */
    this.locateToFeature = function (_feature, bShowFeature) {
        var extent = null;
        var mapView = map.getView();
        if ($.isArray(_feature)) {
            geom = [];
            layerSource.clear();
            _feature.forEach(function (f) {
                var geom = null;
                var feature = f.clone();
                geom = feature.getGeometry();
                if (bShowFeature) {
                    layerSource.addFeature(feature);
                }
                extent = extent === null ? geom.getExtent() : ol.extent.extend(extent, geom.getExtent());
            });
            if (extent) {
                map.getView().fit(extent, map.getSize());
                mapView.setZoom(mapView.getZoom() - 1);
            }
        } else {
            var feature = _feature.clone();
            var geom = feature.getGeometry();
            if (bShowFeature) {
                layerSource.clear();
                layerSource.addFeature(feature);
            }
            if (geom && ol.geom.Point.prototype.isPrototypeOf(geom)) {
                map.getView().setCenter(geom.getCoordinates());
                mapView.setZoom(plugin.getOption("locatingPointZoom"));
            } else {
                map.getView().fit(geom.getExtent(), map.getSize());
                mapView.setZoom(mapView.getZoom() - 1);
            }
        }
    };

    this.zoomToLayer = function (layer) {
        me.zoomToExtent(layer.getSource().getExtent());
    };

    /**
    * @summary 定位到指定的ol.Extent位置
    * @param extent {ol.Extent} Envelop內容
    * @param srcPrj {string} 來源坐標系統
    */
    this.zoomToExtent = function (extent, srcPrj) {
        if (srcPrj) {
            extent = ol.proj.transformExtent(extent, srcPrj, map.getView().getProjection());
        }
        map.getView().fit(extent, map.getSize());
    };

    /**
    * @summary 清除定位資訊
    */
    this.clearLocatedFeature = function () {
        layerSource.clear();
    };


    // 找出MultiPolygon中面積最大的Polygon
    function getMaxPoly(polys) {
        var polyObj = [];

        for (var b = 0; b < polys.length; b++) {
            polyObj.push({ poly: polys[b], area: polys[b].getArea() });
        }

        polyObj.sort(function (a, b) { return a.area - b.area; });

        return polyObj[polyObj.length - 1].poly;
    }

    // 判斷feature的型別，設置Label要呈現的點位
    function createLabelFeature(feature) {
        var isArray = true;
        var featureArray = new Array();
        if (!$.isArray(feature)) {
            isArray = false;
            featureArray[0] = feature;
        }
        else {
            featureArray = feature;
        }

        var point = new Array();
        for (var i = 0; i < featureArray.length; i++) {
            var labelPoint;
            var type = featureArray[i].getGeometry().getType();
            if (type === 'MultiPolygon') {
                labelPoint = getMaxPoly(featureArray[i].getGeometry().getPolygons()).getInteriorPoint();
            } else if (type === 'Polygon') {
                labelPoint = featureArray[i].getGeometry().getInteriorPoint();
            } else if (type === 'LineString') {
                var centerCoord = featureArray[i].getGeometry().getCoordinateAt(0.5);
                labelPoint = new ol.geom.Point(centerCoord);
            } else if (type === 'Point') {
                labelPoint = featureArray[i].getGeometry();
            }
            point[i] = new ol.Feature(labelPoint);
        }

        if (isArray)
            return point;
        else
            return point[0];
    }

};

/**
* @summary 允許Plugin使用定位功能
* @private
*/
oltmx.Plugin.prototype.activeLocateTool = function (activeOptions) {
    this.locateTool = new oltmx.util.Locate(this, activeOptions);
};
