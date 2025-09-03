var CurPK = "";
var CurLableID = "";
//編輯圖徵
function EditMark(itemNum, markType, PK) {
    $(".mkSet").hide();
    var MarkLabel = document.getElementById("Label" + markType + itemNum);
    //開啟對應標註的div&設定選取樣式
    if (markType == "point") {
        $("#ddlCategory")[0].selectedIndex = 1;
        markPointDiv.show();
        $("#Point" + itemNum).css("background-color", "rgb(52 163 138)");
    } else if (markType == "line") {
        $("#ddlCategory")[0].selectedIndex = 2;
        markLineDiv.show();
        $("#LineString" + itemNum).css("background-color", "rgb(52 163 138)");
    } else if (markType == "poly") {
        $("#ddlCategory")[0].selectedIndex = 4;
        markPolyDiv.show();
        $("#Polygon" + itemNum).css("background-color", "rgb(52 163 138)");
    } else if (markType == "text") {
        $("#ddlCategory")[0].selectedIndex = 6;
        markTxtDiv.show();
        $("#Text" + itemNum).css("background-color", "rgb(52 163 138)");
    } else if (markType == "box") {
        $("#ddlCategory")[0].selectedIndex = 3;
        markBoxDiv.show();
        $("#Box" + itemNum).css("background-color", "rgb(52 163 138)");
    } else if (markType == "cir") {
        $("#ddlCategory")[0].selectedIndex = 5;
        markCirDiv.show();
        if (document.getElementById("CirPolygon" + itemNum)) {
            $("#CirPolygon" + itemNum).css("background-color", "rgb(52 163 138)");
        } else {
            $("#Circle" + itemNum).css("background-color", "rgb(52 163 138)");
        }
    }
    SetTypeActive(markType);

    drawSource.getFeatures().forEach(function (olFeature) {
        var olStyle = olFeature.getStyle();
        if (olStyle.length != undefined) {
            olStyle = olStyle[0];
        }
        if (olFeature.get('num') == markType + itemNum) {
            setOriginStyle(olFeature, markType);
            if (markType == "point") {
                var scale = olStyle.getImage().getScale();
                var pointStyle = null;
                if (olStyle.getImage().getSrc == undefined) {
                    scale = olFeature.get('olScale');
                    pointStyle = oltmx.Plugin.prototype.setPointMarkStyle("#00FFFF", scale, olStyle.getText().getText(), olFeature);
                } else {
                    pointStyle = oltmx.Plugin.prototype.setIconMarkStyle("#00FFFF", scale, olStyle.getText().getText(), olStyle.getImage().getSrc(), olStyle.getImage().getOpacity(), olFeature, 35);
                }
                olFeature.setStyle(pointStyle);
            } else if (markType == "text") {
                olStyle.getText().getStroke().setWidth(5);
                olStyle.getText().getStroke().setColor("#00FFFF");
            } else if (markType == "line") {
                olStyle.getStroke().setWidth(5);
                olStyle.getStroke().setColor("#00FFFF");
            } else {
                olStyle.getStroke().setWidth(5);
                olStyle.getStroke().setColor("#00FFFF");
                olStyle.getFill().setColor("rgba(255, 255, 255, 0.5)");
            }
            drawSource.refresh();
            if (markType == "poly" || markType == "line") {
                oltmx.Plugin.prototype.modifyFeature(olFeature);
            }
        }
    });
    window[markType + "Txt"] = MarkLabel.innerText;

    if (mapPlugin.__gs__modifyInteraction != undefined) {
        mapPlugin.__gs__modifyInteraction.on('modifyend', function (e) {
            var olF = e.features.array_[0];
            var PK = olF.getId();
            if (PK != undefined) {
                var Type = olF.get('num');
                UpdateCoords(olF, Type);
            }
        });
    }
}
//更新圖徵
function SaveMark(itemNum, markType, PK) {
    var MarkLabel = document.getElementById("Label" + markType + itemNum);
    var FeatureNum = markType + itemNum;
    var HasSeg = 0;
    var HasTol = 0;
    var HasArea = 0
    var HasRadius = 0;
    var IsSeg = 0;
    var IsTotal = 0;
    var IsArea = 0;
    var IsRadius = 0;
    var HasAngle = 0;
    var IsCoord = 0;
    var HasIconS = "non";
    var HasIconE = "non";
    var Height = 0;
    var IsCen = 0;

    if (markType == "point") {
        $("#Point" + itemNum).css("background-color", "");
        drawSource.getFeatures().forEach(function (olFeature) {
            if (olFeature.get('num') == FeatureNum) {
                UpdateGeom(olFeature, markType);
                var offsetY = 35;
                //是否顯示座標
                var Coord3857 = olFeature.getGeometry().getCoordinates();
                var Coord4326 = ol.proj.transform(Coord3857, "EPSG:3857", "EPSG:4326");
                var CoordTxt = Coord4326[0].toFixed(3) + "," + Coord4326[1].toFixed(3);
                var ptTxt = pointTxt;
                if (pointIsCoord) {
                    IsCoord = 1;
                    olFeature.set('IsCoord', true);
                    offsetY = 40;
                    ptTxt = pointTxt.split('\n')[0].split(" ")[0] + '\n' + CoordTxt;
                } else {
                    IsCoord = 0;
                    olFeature.set('IsCoord', undefined);
                    ptTxt = pointTxt.split('\n')[0].split(" ")[0];
                }
                //$("#txtPtName").val(ptTxt);
                pointTxt = ptTxt;
                pointColor = pointColorPick;
                var Opacity = pointColor.indexOf("rgba") >= 0 ? pointColor.replace(/^.*,(.+)\)/, '$1') : 1;
                var pointStyle = null;
                if (olFeature.getStyle().getImage().getSrc == undefined) {
                    pointStyle = oltmx.Plugin.prototype.setPointMarkStyle(pointColor, pointScale, pointTxt, olFeature);
                    olFeature.set('olScale', parseInt(pointScale));
                } else {
                    pointStyle = oltmx.Plugin.prototype.setIconMarkStyle(pointColor, pointScale, pointTxt, olFeature.getStyle().getImage().getSrc(), Opacity, olFeature, offsetY);
                }

                olFeature.setStyle(pointStyle);
            }
        });
    } else if (markType == "line") {
        $("#LineString" + itemNum).css("background-color", "");
        drawSource.getFeatures().forEach(function (olFeature) {
            if (olFeature.get('num') == FeatureNum) {
                HasIconS = lineTypeS;
                HasIconE = lineTypeE;
                lineColor = lineStrokePick;
                UpdateGeom(olFeature, markType);
                var LineTxt = lineTxt;
                var LineGeom = olFeature.getGeometry();
                var LineStyle = [];
                lineTxt = LineTxt;
                var StyleArr = oltmx.Plugin.prototype.setLineMarkStyle(lineColor, lineWidth, LineTxt, lineDash1, lineDash2, olFeature);
                LineStyle.push(StyleArr);
                if (HasIconS != "non") {
                    //起點
                    var firStart = LineGeom.getCoordinateAt(0.01);
                    var firEnd = LineGeom.getFirstCoordinate();
                    var dxF = firEnd[0] - firStart[0];
                    var dyF = firEnd[1] - firStart[1];
                    var rotationF = Math.atan2(dyF, dxF);
                    LineStyle.push(new ol.style.Style({
                        geometry: new ol.geom.Point(firEnd),
                        image: new ol.style.Icon({
                            src: 'images/' + HasIconS + '.png',
                            color: lineColor,
                            anchor: [0.75, 0.5],
                            rotateWithView: true,
                            rotation: -rotationF
                        }),
                        zIndex: 9999999
                    }));
                }

                if(HasIconE != "non") {
                    //終點
                    var start = LineGeom.getCoordinateAt(0.99);
                    var end = LineGeom.getLastCoordinate();
                    var dx = end[0] - start[0];
                    var dy = end[1] - start[1];
                    var rotation = Math.atan2(dy, dx);
                    LineStyle.push(new ol.style.Style({
                        geometry: new ol.geom.Point(end),
                        image: new ol.style.Icon({
                            src: 'images/' + HasIconE + '.png',
                            color: lineColor,
                            anchor: [0.75, 0.5],
                            rotateWithView: true,
                            rotation: -rotation
                        }),
                        zIndex: 9999999
                    }));
                }

                olFeature.setStyle(LineStyle);
                olFeature.set('HasIconS', HasIconS);
                olFeature.set('HasIconE', HasIconE);

                //總長度計算判斷
                if (lineIsLen) {
                    HasTol = 1;
                    deleteTolMark(PK);
                    olFeature.set('IsTotalNum', true);
                    AddLineTotalLen(olFeature, PK, "ddlLineUnit");
                }else {
                    HasTol = 0;
                    olFeature.set('IsTotalNum', undefined);
                    deleteTolMark(PK);
                }

                //分段長度計算判斷
                if (lineIsSeg) {
                    HasSeg = 1;
                    deleteSegMark(PK);
                    olFeature.set('IsSegNum', true);
                    AddLineSegLen(olFeature, PK, "ddlLineUnit");
                }else {
                    HasSeg = 0;
                    olFeature.set('IsSegNum', undefined);
                    deleteSegMark(PK);
                }
            }
        });
    } else if (markType == "poly") {
        $("#Polygon" + itemNum).css("background-color", "");
        drawSource.getFeatures().forEach(function (olFeature) {
            if (olFeature.get('num') == FeatureNum) {
                var Txt = polyTxt;
                UpdateGeom(olFeature, markType);
                //面積計算判斷
                if (polyIsArea) {
                    HasArea = 1;
                    deleteAreaMark(PK);
                    olFeature.set('HasArea', true);
                    AddArea(olFeature, PK, "Poly");
                } else {
                    HasArea = 0;
                    olFeature.set('HasArea', undefined);
                    deleteAreaMark(PK);
                }
                polyTxt = Txt;
                polyColor = polyStrokePick;
                polyFillColor = polyFillPick;
                var polyStyle = oltmx.Plugin.prototype.setPolyMarkStyle(polyFillColor, polyColor, polyWidth, polyTxt, polyDash1, polyDash2, olFeature);
                olFeature.setStyle(polyStyle);
                Height = polyHeight != "" ? parseFloat(polyHeight) : 0;
                olFeature.set('Height', polyHeight);
                //總長度計算判斷
                if (polyIsLen) {
                    HasTol = 1;
                    deleteTolMark(PK);
                    olFeature.set('IsTotalNum', true);
                    AddLineTotalLen(olFeature, PK, "ddlPolyUnit");
                } else {
                    HasTol = 0;
                    olFeature.set('IsTotalNum', undefined);
                    deleteTolMark(PK);
                }

                //分段長度計算判斷
                if (polyIsSeg) {
                    HasSeg = 1;
                    deleteSegMark(PK);
                    olFeature.set('IsSegNum', true);
                    AddLineSegLen(olFeature, PK, "ddlPolyUnit");
                } else {
                    HasSeg = 0;
                    olFeature.set('IsSegNum', undefined);
                    deleteSegMark(PK);
                }
            }
        });
    } else if (markType == "text") {
        $("#Text" + itemNum).css("background-color", "");
        drawSource.getFeatures().forEach(function (olFeature) {
            if (olFeature.get('num') == FeatureNum) {
                UpdateGeom(olFeature, markType);
                if (olFeature.get('IsSeg')) IsSeg = 1;
                if (olFeature.get('IsTotal')) IsTotal = 1;
                if (olFeature.get('IsArea')) IsArea = 1;
                if (textIsCoord) {
                    IsCoord = 1;
                    olFeature.set('IsCoord', true);
                    var Coord3857 = olFeature.getGeometry().getCoordinates();
                    var Coord4326 = ol.proj.transform(Coord3857, "EPSG:3857", "EPSG:4326");
                    var CoordTxt = Coord4326[0].toFixed(3) + "," + Coord4326[1].toFixed(3);
                    textTxt = textTxt.split('\n')[0].split(" ")[0] + '\n' + CoordTxt;
                } else if (IsSeg == 0 && IsTotal == 0 && IsArea == 0) {
                    IsCoord = 0;
                    olFeature.set('IsCoord', undefined);
                    textTxt = textTxt.split('\n')[0].split(" ")[0];
                }
                var textStyle = oltmx.Plugin.prototype.setTextMarkStyle(textTxt, textColor, textWidth, textStrokeColor, textScale, olFeature, 0);
                olFeature.setStyle(textStyle);
            }
        });
    } else if (markType == "box") {
        $("#Box" + itemNum).css("background-color", "");
        drawSource.getFeatures().forEach(function (olFeature) {
            if (olFeature.get('num') == FeatureNum) {
                var Txt = boxTxt;
                UpdateGeom(olFeature, markType);
                //面積計算判斷
                if (boxIsArea) {
                    HasArea = 1;
                    deleteAreaMark(PK);
                    olFeature.set('HasArea', true);
                    AddArea(olFeature, PK, "Box");
                } else {
                    HasArea = 0;
                    olFeature.set('HasArea', undefined);
                    deleteAreaMark(PK);
                }
                boxTxt = Txt;
                boxColor = boxStrokePick;
                boxFillColor = boxFillPick
                var boxStyle = oltmx.Plugin.prototype.setBoxMarkStyle(boxFillColor, boxColor, boxWidth, boxTxt, boxDash1, boxDash2, olFeature);
                olFeature.setStyle(boxStyle);
                Height = boxHeight != "" ? parseFloat(boxHeight) : 0;
                olFeature.set('Height', boxHeight);
                //總長度計算判斷
                if (boxIsLen) {
                    HasTol = 1;
                    deleteTolMark(PK);
                    olFeature.set('IsTotalNum', true);
                    AddLineTotalLen(olFeature, PK, "ddlBoxUnit");
                } else {
                    HasTol = 0;
                    olFeature.set('IsTotalNum', undefined);
                    deleteTolMark(PK);
                }

                //分段長度計算判斷
                if (boxIsSeg) {
                    HasSeg = 1;
                    deleteSegMark(PK);
                    olFeature.set('IsSegNum', true);
                    AddLineSegLen(olFeature, PK, "ddlBoxUnit");
                } else {
                    HasSeg = 0;
                    olFeature.set('IsSegNum', undefined);
                    deleteSegMark(PK);
                }
            }
        });
    } else if (markType == "cir") {
        if (document.getElementById("CirPolygon" + itemNum)) {
            $("#CirPolygon" + itemNum).css("background-color", "");
        } else {
            $("#Circle" + itemNum).css("background-color", "");
        }
        drawSource.getFeatures().forEach(function (olFeature) {
            if (olFeature.get('num') == FeatureNum) {
                var Txt = cirTxt;
                UpdateGeom(olFeature, markType);
                //面積計算判斷
                if (cirIsArea && !olFeature.get('HasArea')) {
                    HasArea = 1;
                    olFeature.set('HasArea', true);
                    AddArea(olFeature, PK, "Cir");
                } else if (cirIsArea && olFeature.get('HasArea')) {
                    HasArea = 1;
                    olFeature.set('HasArea', true);
                    ResetAreaTxt(olFeature, PK, "Cir");
                } else {
                    HasArea = 0;
                    olFeature.set('HasArea', undefined);
                    deleteAreaMark(PK);
                }

                //半徑計算
                //if (cirIsRadius) {
                //    HasRadius = 1;
                //    olFeature.set('HasRadius', true);
                //    ResetRadius(olFeature, PK);
                //} else {
                //    HasRadius = 0;
                //    olFeature.set('HasRadius', undefined);
                //    deleteRadiusMark(PK);
                //}
                //是否顯示經緯度
                if (cirIsCoord) {
                    olFeature.set("IsCoord", true);
                    IsCoord = 1;
                    ResetCoordMark(olFeature, PK);
                } else {
                    IsCoord = 0;
                    olFeature.set('IsCoord', undefined);
                    deleteCoordMark(PK);
                }
                cirTxt = Txt;
                cirColor = cirStrokePick
                cirFillColor = cirFillPick;
                var cirStyle = oltmx.Plugin.prototype.setCirMarkStyle(cirFillColor, cirColor, cirWidth, cirTxt, cirIsRadius, cirDash1, cirDash2, olFeature);
                olFeature.setStyle(cirStyle);
                Height = cirHeight != "" ? parseFloat(cirHeight) : 0;
                olFeature.set('Height', cirHeight);
            }
        });
    } 

    MarkLabel.innerText = window[markType + "Txt"];
    Name = window[markType + "Txt"];
    map.removeInteraction(modifyInteraction);
    map.removeInteraction(drawInteraction);
    $("._divMarkCoord").find('input').val("");
    $(".addLine").remove();
    $("._CreatePolyCoord").remove();
}

function HexToTGBA(hexColor) {
    var ColorArr = /rgba?\((\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*(,\s*\d+[\.\d+]*)*\)/g.exec(hexColor);
    return ColorArr;
}

function BackToProjList() {
    toolSwitch('closeAll');
    drawSource.clear();
    getObject("mis_left_contentFrame").src = "Map/projlist.html";
    switchToAddMode();
}

//帶入編輯樣式
function setOriginStyle(olFeature, markType) {
    var olStyle = olFeature.getStyle();
    if (olStyle.length != undefined) {
        olStyle = olStyle[0];
    }
    var Text = olStyle.getText().getText();
    switch (markType) {
        case 'text':
            textColor = olStyle.getText().getFill().getColor();
            textStrokeColor = olStyle.getText().getStroke().getColor();
            textWidth = olStyle.getText().getStroke().getWidth();
            textTxt = olStyle.getText().getText();
            textScale = olStyle.getText().getFont().split(' ')[0].replace('pt', '');
            if (olFeature.get('IsCoord')) {
                textIsCoord = true;
            } else {
                textIsCoord = false;
            }
            break;
        case 'point':
            var RgbaColorArr = olStyle.getImage().getColor != undefined ? olStyle.getImage().getColor() : olStyle.getImage().getFill().getColor();
            pointOpacity = olStyle.getImage().getOpacity();
            var RgbaColor = olStyle.getImage().getColor != undefined ? "rgba(" + [RgbaColorArr[0], RgbaColorArr[1], RgbaColorArr[2], pointOpacity].join(',') + ")" : RgbaColorArr;
            pointColorPick = RgbaColor;
            pointColor = RgbaColor;
            pointScale = olStyle.getImage().getColor != undefined ? olStyle.getImage().getScale() : olFeature.get('olScale');
            if (olFeature.get('IsCoord')) {
                pointIsCoord = true;
            } else {
                pointIsCoord = false;
            }
            break;
        default:
            setTypeParam(olStyle, markType, olFeature);
            break;
    }
    //圖台標示切到對應的樣式
    //setMkCtrlStyle(olFeature, markType, Text);
}

//設定全域變數
function setTypeParam(olStyle, markType, olFeature) {
    window[markType + "Opacity"] = 1; //parseFloat(HexColor.replace(/^.*,(.+)\)/, '$1'));
    window[markType + "StrokePick"] = olStyle.getStroke().getColor();
    window[markType + "Color"] = olStyle.getStroke().getColor();
    window[markType + "Width"] = olStyle.getStroke().getWidth();
    window[markType + "Height"] = olFeature.get('Height') != undefined ? olFeature.get('Height') : 0;
    if (olFeature.get('HasArea')) window[markType + "IsArea"] = true;
    if (olFeature.get('HasRadius')) window[markType + "IsRadius"] = true;
    if (olFeature.get('IsCoord')) cirIsCoord = true;
    if (olFeature.get('IsSegNum')) window[markType + "IsSeg"] = true;
    if (olFeature.get('IsTotalNum')) window[markType + "IsLen"] = true;
    if (olFeature.get('HasSeg')) window[markType + "IsSeg"] = true;
    if (olFeature.get('HasAngle')) window[markType + "IsAng"] = true;
    if (olFeature.get('IsCen')) window[markType + "IsCenter"] = true;

    if (markType != "line") {
        window[markType + "FillPick"] = olStyle.getFill().getColor();
        window[markType + "FillColor"] = olStyle.getFill().getColor();
        if (markType == "cir" || markType == "poly" || markType == "box") {
            var lineDash1 = olStyle.getStroke().getLineDash()[0];
            var lineDash2 = olStyle.getStroke().getLineDash()[1];
            window[markType + "Dash1"] = lineDash1;
            window[markType + "Dash2"] = lineDash2;
        }
    } else if (markType == "line") {
        var HasIconS = olFeature.get('HasIconS');
        var HasIconE = olFeature.get('HasIconE');
        var lineDash1 = olStyle.getStroke().getLineDash()[0];
        var lineDash2 = olStyle.getStroke().getLineDash()[1];
        window[markType + "Dash1"] = lineDash1;
        window[markType + "Dash2"] = lineDash2;
        window[markType + "TypeS"] = HasIconS;
        window[markType + "TypeE"] = HasIconE;
    }
}

//Hex Color To RGBA
function hexToRgb(hex) {
    var RgbaColor = "";
    var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    RgbaColor = "rgba(" + parseInt(result[1], 16) + "," + parseInt(result[2], 16) + "," + parseInt(result[3], 16) + ",1)"; 
    return RgbaColor;
}

//點選列表選取圖徵並編輯樣式
function selectFeature(PK) {
    IsMarkEdit = true;
    TransformInteraction.setActive(true);
    var olFeature = drawSource.getFeatureById(PK);
    var Ext = olFeature.getGeometry().getExtent();
    TransformInteraction.select(olFeature, true);
    //map.getView().fit(Ext, {
    //    size: map.getSize(),
    //    maxZoom: 62
    //});
}

//刪除標註清單項目
function deleteListByID(ListID) {
    $("#" + ListID).remove();
}

//定位圖徵
function locateToFeature(PK) {
    var olFeature = drawSource.getFeatureById(PK);
    var ext = olFeature.getGeometry().getExtent();
    map.getView().fit(ext, map.getSize());
}

function copyFeature(PK) {
    var feature = drawSource.getFeatureById(PK);
    if (feature != null) {
        var FeatureNum = feature.get("num");
        var BtnEID = "BtnEdit" + FeatureNum;
        $("#" + BtnEID).click();
        var BtnSID = "BtnSave" + FeatureNum;
        $("#" + BtnSID).click();
        var olF = feature.clone();
        drawSource.addFeature(olF);
        if (FeatureNum.indexOf("point") > -1) {
            var IconSrc = olF.getStyle().getImage().getSrc();
            SelectedImgSrc = IconSrc;
        }
        //if (FeatureNum.indexOf("text") > -1) {
        //    textTxt = olF.getStyle().getText().getText();
        //}
        checkType(olF);
        var Ext = olF.getGeometry().getExtent();
        Extent = ol.extent.extend(Ext, feature.getGeometry().getExtent());
        map.getView().fit(Extent, map.getSize());
        IsMarkEdit = true;
        TransformInteraction.setActive(true);
        TransformInteraction.select(olF, true);
    }
}

function editFeatureName(PK, labelID) {
    $("#divEditMarkTxt").show();
    CurPK = PK;
    CurLableID = labelID;
}