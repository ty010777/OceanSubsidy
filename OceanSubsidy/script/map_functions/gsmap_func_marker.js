gs.Core.package("gsmap.func");

gsmap.func.Marker = function (_plugin, targetDom) {
    var plugin = _plugin;
    var $baseDom = $(targetDom);
    var $mainDom;
    var bShow = false;
    var me = this;
    var MarkCollection = new ol.Collection();
    // 讀取template
    $.ajax("map/template_marker.html?" + Date.now()).done(
        function (data) {
            $mainDom = $(data);
            $baseDom.append($mainDom);
            if (bShow) {
                me.show();
            } else {
                me.hide();
            }

            $mainDom.find(".closebtn").on("click", function () { me.hide(); });
            markPointDiv = $mainDom.find("#point");
            markLineDiv = $mainDom.find("#line");
            markPolyDiv = $mainDom.find("#polygon");
            markTxtDiv = $mainDom.find("#text");
            markBoxDiv = $mainDom.find("#box");
            markCirDiv = $mainDom.find("#circle");

            //SetColorCtrl();

            //初始化IconPicker
            //$mainDom.find('#divIcon').IconPicker({
            //    'hasNullIcon': false,
            //    'containerPosition': 'bottom-center'
            //});
            //$mainDom.find("#divIcon").append($(".balaIconPicker-icon-container-displayer"));
            //$mainDom.find(".balaIconPicker-icon-displayer").remove();
            //調整選取icon style
            //$mainDom.find(".balaIconPicker-icon-container-displayer").css("position", "relative");
            //$mainDom.find(".balaIconPicker-icon-container-displayer").css("top", "0px");
            //$mainDom.find(".balaIconPicker-icon-container-displayer").css("left", "0px");
            //$mainDom.find(".balaIconPicker-icon-container-displayer").css("width", "235px");
            //$mainDom.find(".balaIconPicker-icon-container-displayer").css("height", "40px");
            //$mainDom.find(".balaIconPicker-icon-container-displayer").css("display", "");
            //$mainDom.find(".balaIconPicker-icon-container").css("width", "400px");
            // 如果 MarkList 元素存在才進行初始化
            if ($("#MarkList").length > 0) {
                $("#MarkList").sortable({
                    stop: function (event, ui) {
                        var FeatureID = ui.item.attr('name');
                        var targetPos = $("#MarkList").children().index(ui.item);
                        resetMarkZIndex(FeatureID, targetPos);
                    }
                });
            }
            getTypeStyle('Point');
        }
    );

    function clear() {
        //$('#engNameList').text('');
    }


    this.show = function () {
        page_active();
        bShow = true;
        if ($mainDom) {
            $mainDom.width("");
        }
         $("#divMark").show();
         $("#divMark .panel").show();
    };

    this.hide = function () {
        page_leave();
        bShow = false;
        if ($mainDom)
            $mainDom.width(0);
         $("#divMark").hide();
         $("#divMark .panel").hide();
        if (typeof map !== 'undefined' && typeof drawInteraction !== 'undefined') {
            map.removeInteraction(drawInteraction);
        }
    }

    this.clear = clear;

    function page_active() {

    }

    function page_leave() {

    }

    function GetEngName() {
        var data = new FormData();
        data.append("kw", $('#engKW').val());
        $.ajax(
            {
                url: "service/Querydata.asmx/GetEngName",
                type: "POST",
                data: data,
                cache: false,
                contentType: false,
                processData: false,
                success: function (result) {
                    var json = $.parseJSON(result);
                    $baseDom.find('#engNameList').text('');
                    var con = '';
                    for (var i = 0; i < json.length; i++) {
                        con += `<option value="` + json[i].eng_name + `">`;
                    }
                    $baseDom.find('#engNameList').append(con);
                }
            });
    }

    function EngAddKw() {
        $('.filter-box').css('display', 'block');
        var kw = $('#engAdvKW').val();
        if (kw == '')
            return;

        if ($('#bKW_' + kw).length == 0) {
            var content = '<button type="button" class="filter-item-delete kwPanel" id="bKW_' + kw + '" onclick="$(this).remove();">' + kw + '</button>';
            $('#divCond').append(content);
        }
    }

    function resetMarkZIndex(FeatureID, targetPos) {
        MarkCollection.clear();
        var olF = drawSource.getFeatureById(FeatureID);
        if (olF != null) {
            //取出圖徵uid
            var uidArray = [];
            for (i = 0; i < drawSource.getFeatures().length; i++) {
                var uid = drawSource.getFeatures()[i].ol_uid;
                uidArray.push(uid);
                MarkCollection.push(drawSource.getFeatures()[i]);
            }

            drawSource.clear();
            MarkCollection.remove(olF);
            //更新圖徵位置排序
            MarkCollection.insertAt(MarkCollection.getArray().length - targetPos, olF);
            var Features = MarkCollection.getArray();

            //更新圖徵uid
            for (i = 0; i < Features.length; i++) {
                var uid = uidArray[i];
                Features[i].ol_uid = uid;
            }

            //重新繪製圖徵
            drawSource.addFeatures(Features);
            //更新Source&Collection
            drawSource.changed();
            MarkCollection.changed();
        }
    }

}

var pointNum = 0;
var lineNum = 0;
var polyNum = 0;
var txtNum = 0;
var boxNum = 0;
var cirNum = 0;
var angleNum = 0;
var fanNum = 0;
var curveNum = 0;
var markType, markPointDiv, markLineDiv, markPolyDiv, markTxtDiv, markBoxDiv, makeCirDiv, markFanDiv, markCurveDiv;
var pointColor, pointScale, pointOpacity, pointColorPick, pointTxt, pointIsCoord;
var lineColor, lineWidth, lineOpacity, lineStrokePick, lineTxt, lineDash1, lineDash2, lineIsSeg, lineIsLen, lineTypeS, lineTypeE;
var polyColor, polyFillColor, polyWidth, polyOpacity, polyStrokePick, polyFillPick, polyTxt, polyIsArea, polyIsSeg, polyIsLen, polyDash1, polyDash2;
var textColor, textStrokeColor, textWidth, textTxt, textScale, textIsCoord;
var boxColor, boxFillColor, boxWidth, boxOpacity, boxStrokePick, boxFillPick, boxTxt, boxIsArea, boxIsSeg, boxIsLen, boxDash1, boxDash2;
var cirColor, cirFillColor, cirWidth, cirOpacity, cirStrokePick, cirFillPick, cirTxt, cirIsArea, cirIsRadius, cirIsCoord, cirDash1, cirDash2, cirIsGeodesy, cirUnit;
var SelectedImgSrc = "images/dot.svg";
var drawCollection = new ol.Collection();

//選取繪製型態
function getTypeStyle(selectedValue) {
    $("#ddlCategory").val(selectedValue);
    map.removeInteraction(drawInteraction);
    addInteraction(selectedValue, false);
    $(".mkSet").hide();
    $(".mkBtnWrap").find("a").removeClass('active');

    if (selectedValue == "Point") {
        $("#point").show();
        $("#aTypePt").addClass('active');
    }
    else if (selectedValue == "LineString") {
        $("#line").show();
        $("#aTypeLine").addClass('active');
    }
    else if (selectedValue == "Polygon") {
        $("#polygon").show();
        $("#aTypePoly").addClass('active');
        //resetPolyCoords();
    } else if (selectedValue == "Text") {
        $("#text").show();
        $("#aTypeTxt").addClass('active');
    } else if (selectedValue == "Box") {
        $("#box").show();
        $("#aTypeBox").addClass('active');
    } else if (selectedValue == "Circle") {
        $("#circle").show();
        $("#aTypeCir").addClass('active');
    } else if (selectedValue == "Arrows") {
        $("#aTypeArrow").addClass('active');
        map.removeInteraction(drawInteraction);
        //TransformInteraction.on('select', selectEvt);
    }
}

//設定對應的標註介面
function SetTypeActive(type) {
    $(".mkBtnWrap a").removeClass('active');
    switch (type) {
        case 'point':
            $("#aTypePt").addClass('active');
            break;
        case 'line':
            $("#aTypeLine").addClass('active');
            break;
        case 'poly':
            $("#aTypePoly").addClass('active');
            break;
        case 'box':
            $("#aTypeBox").addClass('active');
            break;
        case 'cir':
            $("#aTypeCir").addClass('active');
            break;
        case 'text':
            $("#aTypeTxt").addClass('active');
            break;
    }
}

function setEleTxt(TxtVal, type) {
    window[type + "Txt"] = TxtVal;
}

//啟動標註功能
function addInteraction(selectedType, IsSelectMode) {
    var drawType;
    var geometryFunction = null;
    switch (selectedType) {
        case 'Text':
            drawType = 'Point';
            geometryFunction = null;
            textColor = $('.sp-preview-inner')[4].style.backgroundColor;
            textStrokeColor = $('.sp-preview-inner')[5].style.backgroundColor;
            textWidth = document.getElementById('borderWidth').value;
            textScale = document.getElementById('textScale').value;
            textIsCoord = $("#rdoTxtCoordYes")[0].checked;
            break;
        case 'Point':
            drawType = 'Point';
            geometryFunction = null;
            pointColorPick = '#000000'; //$('.sp-preview-inner')[0].style.backgroundColor;
            pointColor = '#000000'; //$('.sp-preview-inner')[0].style.backgroundColor;
            pointOpacity = 0.7; //pointColor.indexOf("rgba") >= 0 ? pointColor.replace(/^.*,(.+)\)/, '$1') : 1;
            pointScale = 1; //document.getElementById('textScale').value;
            pointIsCoord = false;   //$("#rdoTxtCoordYes")[0].checked;
            break;
        case 'LineString':
            drawType = 'LineString';
            geometryFunction = null;
            lineStrokePick = '#000000'; // $('.sp-preview-inner')[1].style.backgroundColor;
            lineColor = '#000000'; // $('.sp-preview-inner')[1].style.backgroundColor;
            lineWidth = 1; //document.getElementById('linewidth').value;
            lineIsSeg = 1;  // $("#rdoLenYes")[0].checked;
            lineIsLen = 0;  // $("#rdoTotalYes")[0].checked;
            var LineDash = 0;// $("#ddlLineDash").val();
            lineDash1 = parseInt(LineDash);
            lineDash2 = parseInt(LineDash);
            break;
        case 'Polygon':
            drawType = 'Polygon';
            geometryFunction = null;
            polyStrokePick = '#000000'; // $('.sp-preview-inner')[2].style.backgroundColor;
            polyFillPick = '#d0e0e3'; //$('.sp-preview-inner')[3].style.backgroundColor;
            polyColor = '#000000'; //$('.sp-preview-inner')[2].style.backgroundColor;
            polyFillColor = '#d0e0e3'; //$('.sp-preview-inner')[3].style.backgroundColor;
            polyWidth = 1;// document.getElementById('polywidth').value;
            polyIsArea = 0;// $("#rdoPolyAreaYes")[0].checked;
            polyIsSeg = 0;// $("#rdoPolySLenYes")[0].checked;
            //polyIsLen = $("#rdoPolyTLenYes")[0].checked;
            var PolyDash = 0;// $("#ddlPolyDash").val();
            polyDash1 = parseInt(PolyDash);
            polyDash2 = parseInt(PolyDash);
            break;
        case 'Box':
            drawType = 'Circle';
            geometryFunction = ol.interaction.Draw.createBox();
            boxStrokePick = '#000000'; //$('.sp-preview-inner')[6].style.backgroundColor;
            boxFillPick = '#d0e0e3'; //$('.sp-preview-inner')[7].style.backgroundColor;
            boxColor = '#000000'; //$('.sp-preview-inner')[6].style.backgroundColor;
            boxFillColor = '#d0e0e3'; //$('.sp-preview-inner')[7].style.backgroundColor;
            boxWidth = 1;//document.getElementById('boxwidth').value;
            boxIsArea = 0;//$("#rdoBoxAreaYes")[0].checked;
            boxIsSeg = 0;//$("#rdoBoxSLenYes")[0].checked;
            //boxIsLen = $("#rdoBoxTLenYes")[0].checked;
            var BoxDash = 0;// $("#ddlBoxDash").val();
            boxDash1 = parseInt(BoxDash);
            boxDash2 = parseInt(BoxDash);
            break;
        case 'Circle':
            drawType = 'Circle';
            geometryFunction = null;
            cirStrokePick = '#000000'; //$('.sp-preview-inner')[8].style.backgroundColor;
            cirFillPick = '#d0e0e3'; //$('.sp-preview-inner')[9].style.backgroundColor;
            cirColor = '#000000'; //$('.sp-preview-inner')[8].style.backgroundColor;
            cirFillColor = '#d0e0e3'; //$('.sp-preview-inner')[9].style.backgroundColor;
            cirWidth = 1;   // document.getElementById('cirwidth').value;
            //cirIsArea = $("#rdoCirAreaYes")[0].checked;
            //cirIsRadius = $("#rdoRadiusYes")[0].checked;
            cirIsCoord = 0;// $("#rdoCirCoordYes")[0].checked;
            cirIsGeodesy = 0;//$("#rdoCirAcrossYes")[0].checked;
            var CirDash = 0;// $("#ddlCirDash").val();
            cirDash1 = parseInt(CirDash);
            cirDash2 = parseInt(CirDash);
            cirUnit = 'm'; // document.getElementById('ddlCirUnit').value;
            break;
        case 'None':
            return false;
            break;
    }
    //給予指定的圖徵型態，啟動繪製圖徵功能
    if (selectedType == 'Circle' && drawType == 'Circle' && cirIsGeodesy) {
        //map.removeInteraction(drawInteraction);
        selectedType = "Geodesy";
        oltmx.Plugin.prototype.drawFeature('Point', null);
    } else if (drawType != undefined) {
        oltmx.Plugin.prototype.drawFeature(drawType, geometryFunction);
    }
    if (!IsSelectMode) {
        //TransformInteraction.un('select', selectEvt);
    }

    if (selectedType == 'Circle' && drawType == 'Circle' && cirIsGeodesy) {
        map.removeInteraction(drawInteraction);
    } else {
        drawInteraction = oltmx.Plugin.prototype.__gs__drawInteraction;
        drawInteraction.on('drawend', function (e) {
            feature = e.feature;
            geometry = feature.getGeometry();
            drawEnd(feature, geometry, selectedType);
        });
    }
}

//繪製完成後事件
function drawEnd(olFeature, olGeom, selectedType) {
    var Num, styleArr;
    var PK = CreateGuid();
    olFeature.setId(PK);
    var LineStyle = [];
    var HasIconS = "non";
    var HasIconE = "non";
    switch (selectedType) {
        case 'Text':
            Num = txtNum;
            if ($("#rdoTxtCoordYes")[0].checked) {
                var Coord3857 = olFeature.getGeometry().getCoordinates();
                var Coord4326 = ol.proj.transform(Coord3857, "EPSG:3857", "EPSG:4326");
                var CoordTxt = Coord4326[1].toFixed(3) + "," + Coord4326[0].toFixed(3);
                textTxt = textTxt.split('\n')[0].split(" ")[0] + '\n' + CoordTxt;
                olFeature.set("IsCoord", true);
                textIsCoord = true;
                IsCoord = 1;
            }
            styleArr = oltmx.Plugin.prototype.setTextMarkStyle(textTxt, textColor, textWidth, textStrokeColor, textScale, olFeature, 0);
            olFeature.set('num', 'text' + Num);
            if (!(document.getElementById("MarkList").children[selectedType + Num])) {
                genMarkList(PK, selectedType, Num, textTxt, "text");
                txtNum++;
            }
            Name = textTxt;
            break;
        case 'Point':
            var offsetY = 35;
            Num = pointNum;
            var ptTxt = "";
            pointTxt == undefined ? ptTxt = "點" + Num : ptTxt = pointTxt;
            if (false && $("#rdoPtCoordYes")[0].checked) {
                var Coord3857 = olFeature.getGeometry().getCoordinates();
                var Coord4326 = ol.proj.transform(Coord3857, "EPSG:3857", "EPSG:4326");
                var CoordTxt = Coord4326[1].toFixed(3) + "," + Coord4326[0].toFixed(3);
                ptTxt = pointTxt.split('\n')[0].split(" ")[0] + '\n' + CoordTxt;
                $("#txtPtName").val(ptTxt);
                olFeature.set("IsCoord", true);
                pointIsCoord = true;
                IsCoord = 1;
                offsetY = 40;
            }
            if (SelectedImgSrc.indexOf("dot") > -1) {
                styleArr = oltmx.Plugin.prototype.setPointMarkStyle(pointColor, pointScale, ptTxt, olFeature);
                olFeature.set('olScale', parseInt(pointScale));
            } else {
                styleArr = oltmx.Plugin.prototype.setIconMarkStyle(pointColor, pointScale, ptTxt, SelectedImgSrc, pointOpacity, olFeature, offsetY);
            }

            olFeature.set('num', 'point' + Num);
            if (!(document.getElementById("MarkList").children[selectedType + Num])) {
                genMarkList(PK, selectedType, Num, ptTxt, "point");
                pointNum++;
            }
            Name = ptTxt;
            break;
        case 'LineString':
            var LineTypeS = 'non';// $("#ddlLineTypeS").val();
            var LineTypeE = 'non';//$("#ddlLineTypeE").val();
            Num = lineNum;
            var LineTxt = "線" + Num;// $("#txtLineName").val() == "" ? "線" + Num : $("#txtLineName").val();
            var LineGeom = olFeature.getGeometry();

            styleArr = oltmx.Plugin.prototype.setLineMarkStyle(lineColor, lineWidth, LineTxt, lineDash1, lineDash2, olFeature);
            LineStyle.push(styleArr);
            //設定起點箭頭Style
            if (LineTypeS != "non") {
                var IconFileName = LineTypeS;
                HasIconS = LineTypeS;
                var start = LineGeom.getCoordinateAt(0.99);
                var end = LineGeom.getFirstCoordinate();

                var dx = end[0] - start[0];
                var dy = end[1] - start[1];
                var rotation = Math.atan2(dy, dx);
                LineStyle.push(new ol.style.Style({
                    geometry: new ol.geom.Point(end),
                    image: new ol.style.Icon({
                        src: 'images/' + IconFileName + '.png',
                        color: lineColor,
                        anchor: [0.75, 0.5],
                        rotateWithView: true,
                        rotation: -rotation
                    }),
                    zIndex: 9999999
                }));
            }
            //設定終點Style
            if (LineTypeE != "non") {
                var IconFileName = LineTypeE;
                HasIconE = LineTypeE;
                //終點
                var start = LineGeom.getCoordinateAt(0.99);
                var end = LineGeom.getLastCoordinate();
                var dx = end[0] - start[0];
                var dy = end[1] - start[1];
                var rotation = Math.atan2(dy, dx);
                LineStyle.push(new ol.style.Style({
                    geometry: new ol.geom.Point(end),
                    image: new ol.style.Icon({
                        src: 'images/' + IconFileName + '.png',
                        color: lineColor,
                        anchor: [0.75, 0.5],
                        rotateWithView: true,
                        rotation: -rotation
                    }),
                    zIndex: 9999999
                }));
            }

            olFeature.set('num', 'line' + Num);
            olFeature.set('HasIconS', HasIconS);
            olFeature.set('HasIconE', HasIconE);
            if (!(document.getElementById("MarkList").children[selectedType + Num])) {
                genMarkList(PK, selectedType, Num, LineTxt, "line");
                lineNum++;
            }
            Name = LineTxt;

            //if ($("#rdoTotalYes")[0].checked) {
            //    AddLineTotalLen(olFeature, PK, "ddlLineUnit");
            //    olFeature.set("IsTotalNum", true);
            //    lineIsLen = true;
            //    HasTol = 1;
            //}

            //if ($("#rdoLenYes")[0].checked) {
            //    AddLineSegLen(olFeature, PK, "ddlLineUnit");
            //    olFeature.set("IsSegNum", true);
            //    lineIsSeg = true;
            //    HasSeg = 1;
            //}
            lineTypeS = LineTypeS;
            lineTypeE = LineTypeE;
            break;
        case 'Polygon':
            Num = polyNum;
            var PolyTxt = "多邊形" + Num;// $("#txtPolyName").val() == "" ? "多邊形" + Num : $("#txtPolyName").val();
            styleArr = oltmx.Plugin.prototype.setPolyMarkStyle(polyFillColor, polyColor, polyWidth, PolyTxt, polyDash1, polyDash2, olFeature);
            olFeature.set('num', 'poly' + Num);
            if (!(document.getElementById("MarkList").children[selectedType + Num])) {
                genMarkList(PK, selectedType, Num, PolyTxt, "poly");
                polyNum++;
            }

            //if ($("#rdoPolyAreaYes")[0].checked) {
            //    AddArea(olFeature, PK, "Poly");
            //    olFeature.set("HasArea", true);
            //    polyIsArea = true;
            //}
            //邊長顯示
            //if ($("#rdoPolySLenYes")[0].checked) {
            //    AddLineSegLen(olFeature, PK, "ddlPolyUnit");
            //    olFeature.set("IsSegNum", true);
            //    polyIsSeg = true;
            //    HasSeg = 1;
            //}
            //周長顯示
            //if ($("#rdoPolyTLenYes")[0].checked) {
            //    AddLineTotalLen(olFeature, PK, "ddlPolyUnit");
            //    olFeature.set("IsTotalNum", true);
            //    polyIsLen = true;
            //    HasTol = 1;
            //}

            Name = PolyTxt;
            break;
        case 'Box':
            Num = boxNum;
            //$("#txtBoxName").val() == "" ? $("#txtBoxName").val("矩形" + Num) : $("#txtBoxName").val();
            var BoxTxt = "矩形" + Num;
            styleArr = oltmx.Plugin.prototype.setBoxMarkStyle(boxFillColor, boxColor, boxWidth, BoxTxt, boxDash1, boxDash2, olFeature);
            olFeature.set('num', 'box' + Num);
            if (!(document.getElementById("MarkList").children[selectedType + Num])) {
                genMarkList(PK, selectedType, Num, BoxTxt, "box");
                boxNum++;
            }

            //if ($("#rdoBoxAreaYes")[0].checked) {
            //    AddArea(olFeature, PK, "Box");
            //    olFeature.set("HasArea", true);
            //    boxIsArea = true;
            //}
            //邊長顯示
            //if ($("#rdoBoxSLenYes")[0].checked) {
            //    AddLineSegLen(olFeature, PK, "ddlBoxUnit");
            //    olFeature.set("IsSegNum", true);
            //    boxIsSeg = true;
            //    HasSeg = 1;
            //}
            //周長顯示
            //if ($("#rdoBoxTLenYes")[0].checked) {
            //    AddLineTotalLen(olFeature, PK, "ddlBoxUnit");
            //    olFeature.set("IsTotalNum", true);
            //    boxIsLen = true;
            //    HasTol = 1;
            //}

            Name = BoxTxt;
            break;
        case 'Circle':
            Num = cirNum;
            var cirTxt = "圓形" + Num; // $("#txtCirName").val() == "" ? "圓形" + Num : $("#txtCirName").val();
            var Radius = olGeom.getRadius();
            var Center3857 = olGeom.getCenter();
            //var Center4326 = ol.proj.transform(Center3857, 'EPSG:3857', 'EPSG:4326');
            //var Sphere = new ol.Sphere(6378137);
            //var Circle4326 = ol.geom.Polygon.circular(Sphere, Center4326, Radius, 256);
            //olGeom = Circle4326.clone().transform('EPSG:4326', 'EPSG:3857');
            //若需符合大地量測
            //olFeature.setGeometry(olGeom);
            var circleGeom3857 = new ol.geom.Circle(Center3857, Radius);
            olFeature.setGeometry(circleGeom3857);
            olFeature.set('num', 'cir' + Num);
            olFeature.set('Radius', Radius);
            olFeature.set('CirUnit', cirUnit);
            styleArr = oltmx.Plugin.prototype.setCirMarkStyle(cirFillColor, cirColor, cirWidth, cirTxt, cirIsRadius, cirDash1, cirDash2, olFeature);
            if (!(document.getElementById("MarkList").children[selectedType + Num])) {
                genMarkList(PK, selectedType, Num, cirTxt, "cir");
                cirNum++;
            }

            //if ($("#rdoCirAreaYes")[0].checked) {
            //    AddArea(olFeature, PK, "Cir");
            //    olFeature.set("HasArea", true);
            //    cirIsArea = true;
            //    HasArea = 1;
            //}

            //if ($("#rdoRadiusYes")[0].checked) {
            //    AddRadius(olFeature, PK);
            //    olFeature.set("HasRadius", true);
            //    cirIsRadius = true;
            //    HasRadius = 1;
            //}
            //是否顯示經緯度
            //if ($("#rdoCirCoordYes")[0].checked) {
            //    AddCoordMark(olFeature, PK);
            //    olFeature.set("IsCoord", true);
            //    cirIsCoord = true;
            //    IsCoord = 1;
            //}

            Name = cirTxt;
            break;
        case 'CirPolygon':
            Num = cirNum;
            var cirTxt = $("#txtCirName").val() == "" ? "圓形" + Num : $("#txtCirName").val();
            styleArr = oltmx.Plugin.prototype.setCirMarkStyle(cirFillColor, cirColor, cirWidth, cirTxt, cirIsRadius, cirDash1, cirDash2, olFeature);
            olFeature.set('num', 'cir' + Num);
            olFeature.set('CirUnit', cirUnit);
            if (!(document.getElementById("MarkList").children[selectedType + Num])) {
                genMarkList(PK, selectedType, Num, cirTxt, "cir");
                cirNum++;
            }

            //if ($("#rdoCirAreaYes")[0].checked) {
            //    AddArea(olFeature, PK, "Cir");
            //    olFeature.set("HasArea", true);
            //    cirIsArea = true;
            //    HasArea = 1;
            //}

            //if ($("#rdoRadiusYes")[0].checked) {
            //    AddRadius(olFeature, PK);
            //    olFeature.set("HasRadius", true);
            //    cirIsRadius = true;
            //    HasRadius = 1;
            //}
            //是否顯示經緯度
            if ($("#rdoCirCoordYes")[0].checked) {
                olFeature.set("IsCoord", true);
                cirIsCoord = true;
                IsCoord = 1;
            }

            Name = cirTxt;
            break;
    }

    if (selectedType == 'Geodesy') {
        var Coord = olGeom.getCoordinates();
        var Coord4326 = ol.proj.transform(Coord, 'EPSG:3857', 'EPSG:4326');
        if ($("#ddlCirCoordType").val() == "DD") {
            $("#txtCirLatDD").val(Coord4326[1]);
            $("#txtCirLonDD").val(Coord4326[0]);
        } else {
            var lonDMS = deg_to_dms(Coord4326[0]);
            var latDMS = deg_to_dms(Coord4326[1]);
            $("#txtCirLat").val(latDMS);
            $("#txtCirLon").val(lonDMS);
        }
        olFeature.set('type', 'Geodesy');
    } else {
        olFeature.set('type', 'drawfeature');
        drawCollection.push(olFeature);
        var Style = styleArr;
        if (selectedType == "LineString") Style = LineStyle;
        olFeature.setStyle(Style);
        //TransformInteraction.on('select', selectEvt);
    }
    //map.removeInteraction(drawInteraction);
}

function deg_to_dms(deg) {
    var d = Math.floor(deg);
    var minfloat = (deg - d) * 60;
    var m = Math.floor(minfloat);
    var secfloat = (minfloat - m) * 60;
    var s = Math.round(secfloat);
    // After rounding, the seconds might become 60. These two
    // if-tests are not necessary if no rounding is done.
    if (s == 60) {
        m++;
        s = 0;
    }
    if (m == 60) {
        d++;
        m = 0;
    }
    //return [d, m, s];
    return ("" + d + " " + m + " " + s);
}

function SetColorCtrl() {
    jQuery.fn.exists = function () { return this.length > 0; }
    //設定文字的調色盤
    setColorPicker('#textcolor', "#FFFFFF");
    setColorPicker('#textfillcolor', "#000000");
    //設定點的調色盤
    setColorPicker("#pointcolor", "#FF0000");
    //設定線的調色盤
    setColorPicker("#linecolor", "#000000");
    //設定多邊形的調色盤
    setColorPicker("#polycolor", "#000000");
    setColorPicker("#polyfillcolor", "#d0e0e3");
    //設定矩形的調色盤
    setColorPicker("#boxcolor", "#000000");
    setColorPicker("#boxfillcolor", "#d0e0e3");
    //設定圓形的調色盤
    setColorPicker("#circolor", "#000000");
    setColorPicker("#cirfillcolor", "#d0e0e3");

    $(".sp-replacer").addClass("form-control");
}


//建立標註清單
function genMarkList(PK, selectedType, typeNum, label, type) {
    var HasFPK = false;
    var FPK = "";
    var olF = drawSource.getFeatureById(PK);
    if (olF != null) {
        FPK = olF.get("FPK");
        if (FPK != "" && FPK != undefined) {
            HasFPK = true;
        }
    }
    var imgStr = "";
    switch (type) {
        case 'point':
            imgStr = '<span class="icon" title="點"><i class="fas fa-solid fa-location-smile"></i></span>';
            break;
        case 'line':
            imgStr = '<span class="icon" title="線段"><i class="fa-solid fa-slash-back"></i></span>';
            break;
        case 'poly':
            imgStr = '<span class="icon" title="多邊形"><i class="fa-duotone fa-draw-polygon"></i></span>';
            break;
        case 'cir':
            imgStr = '<span class="icon" title="圓形"><i class="fa-duotone fa-draw-circle"></i></span>';
            break;
        case 'box':
            imgStr = '<span class="icon" title="矩形"><i class="fa-duotone fa-draw-square"></i></span>';
            break;
        case 'text':
            imgStr = '<span class="icon" title="文字"><i class="fas fa-solid fa-text"></i></span>';
            break;
    }

    //子項目subItem
    if (HasFPK) {
        //QuyParentMarkType(PK, FPK, imgStr, selectedType, typeNum, label, type);
    } else {
        $("#MarkList").prepend('<div id="' + selectedType + typeNum + '" class="group" name="' + PK + '"><div id="P' + PK + '" class="item align-items-center">'
            //+ imgStr
            + '<span id="Label' + type + typeNum + '" onmouseover="hoverFeature(' + "'" + PK + "', '" + type + "', true" + ')" onmouseout="hoverFeature(' + "'" + PK + "', '" + type + "', false" + ')">' + label + '</span>'
            + '<div class="tools-wrap ms-auto">'
            //+ '<a href="javascript: void(0);" class="layer-tool" title="調整上下層" onclick="setFeatureSort(' + "'" + PK + "'" + '); "><i class="fa-solid fa-arrows-retweet"></i></a>'
            //+ '<a href="javascript: void(0);" class="layer-tool" title="編輯" onclick="selectFeature(' + "'" + PK + "'" + '); "><i class="fa-solid fa-pen"></i></a>'
            + '<a href="javascript: void(0);" class="layer-tool" title="定位" onclick="locateToFeature(' + "'" + PK + "'" + '); "><i class="fa-solid fa-location-dot"></i></a>'
            + '<a href="javascript: void(0);" class="layer-tool" title="匯出GeoJSON" onclick="exportGeoJson(' + "'" + PK + "'" + '); "><i class="fa-solid fa-download"></i></a>'
            + '<a href="javascript: void(0);" class="layer-tool" title="刪除" onclick="deleteMark(' + selectedType + typeNum + ", '" + type + typeNum + "', '" + PK + "'" + ');"><i class="fa-solid fa-trash-can"></i></a>'
            + '</div></div>'
            + '<div id="sub' + PK + '" class="sub"></div>'
            + "<input type='submit' id='BtnEdit" + type + typeNum + "' name='button' class='BtnLabelEdit'  style='display:none;' value='' onclick='EditMark(" + typeNum + ', "' + type + '", "' + PK + '"' + ")'/>"
            + "<input type='button' id='BtnSave" + type + typeNum + "' class='BtnLabelSave' style='display:none;' value='' style='display:none;' onclick='SaveMark(" + typeNum + ', "' + type + '", "' + PK + '"' + ");'/>"

            + '</div>'
        );
    }
}

function resetDrawInteraction() {
    map.removeInteraction(drawInteraction);
    var selectedType = document.getElementById("ddlCategory").value;
    addInteraction(selectedType, true);
}


function setEleTxt(TxtVal, type) {
    window[type + "Txt"] = TxtVal;
}

function setLineType(TypeVal, paramName) {
    window[paramName] = TypeVal;
}

//切換自訂經緯度類型
function switchCoordType(obj, Type, divID) {
    $(obj.parentNode).find(".divCoordType").hide();
    $("#" + divID + Type).show();
}

// #region 透過介面新增標註

//建立自訂點圖徵
function createPoint() {
    var PK = CreateGuid();
    var selectedType = document.getElementById("ddlCategory").value;
    var FeatureType = 1;
    var CoordType = $("#ddlPtCoordType").val();
    var lon = 0;
    var lat = 0;

    if (CoordType == "DMS") {
        var lonObj = $("#txtPtLon").val() != "" ? $("#txtPtLon").val().split(" ") : "";
        var lonD = lonObj[0];
        var lonM = lonObj[1];
        var lonS = lonObj[2];

        var latObj = $("#txtPtLat").val() != "" ? $("#txtPtLat").val().split(" ") : "";
        var latD = latObj[0];
        var latM = latObj[1];
        var latS = latObj[2];

        lon = (lonD != "" ? parseFloat(lonD) : 0)
            + (isNaN(parseFloat(lonM)) ? 0 : parseFloat(lonM)) / 60
            + (isNaN(parseFloat(lonS)) ? 0 : parseFloat(lonS)) / 3600;

        lat = (latD != "" ? parseFloat(latD) : 0)
            + (isNaN(parseFloat(latM)) ? 0 : parseFloat(latM)) / 60
            + (isNaN(parseFloat(latS)) ? 0 : parseFloat(latS)) / 3600;

        if ($("#ddlPtLon").val() == "W") {
            lon = -Math.abs(lon);
        }

        if ($("#ddlPtLat").val() == "S") {
            lat = -Math.abs(lat);
        }
    } else {
        lon = (isNaN(parseFloat($("#txtPtLonDD").val())) ? 0 : parseFloat($("#txtPtLonDD").val()));
        lat = (isNaN(parseFloat($("#txtPtLatDD").val())) ? 0 : parseFloat($("#txtPtLatDD").val()));
    }

    const latValid = lat >= -90 && lat <= 90;
    const lonValid = lon >= -180 && lon <= 180;

    if (!latValid || !lonValid) {
        alert("輸入的經緯度超出合法範圍！");
        return;
    }

    if (lon != 0 && lat != 0) {
        var offSetY = 35;
        var Coord3857 = ol.proj.transform([lon, lat], "EPSG:4326", "EPSG:3857");
        var olGeom = new ol.geom.Point(Coord3857);
        var olFeature = new ol.Feature(olGeom);
        var ptTxt = "點" + pointNum; // $("#txtPtName").val() == "" ? "點" + pointNum : $("#txtPtName").val();
        //if ($("#rdoPtCoordYes")[0].checked) {
        //    var CoordTxt = lon.toFixed(3) + "," + lat.toFixed(3);
        //    ptTxt = $("#txtPtName").val() + '\n' + CoordTxt;
        //    $("#txtPtName").val(ptTxt);
        //    olFeature.set("IsCoord", true);
        //    pointIsCoord = true;
        //    offSetY = 40;
        //}

        var pointStyle = oltmx.Plugin.prototype.setIconMarkStyle(pointColor, pointScale, ptTxt, SelectedImgSrc, pointOpacity, olFeature, offSetY);
        if (SelectedImgSrc.indexOf("dot") > -1) {
            pointStyle = oltmx.Plugin.prototype.setPointMarkStyle(pointColor, pointScale, ptTxt, olFeature);
            olFeature.set('olScale', parseInt(pointScale));
        }
        olFeature.setStyle(pointStyle);
        olFeature.set('type', 'drawfeature');
        olFeature.set('num', 'point' + pointNum);
        olFeature.setId(PK);
        drawSource.addFeature(olFeature);
        if (!(document.getElementById("MarkList").children[selectedType + pointNum])) {
            genMarkList(PK, selectedType, pointNum, ptTxt, "point");
            pointNum++;
        }
        map.removeInteraction(drawInteraction);
        //TransformInteraction.on('select', selectEvt);
    }
}

//建立自訂文字圖徵
function addTxtMark() {
    if ($("#textfacility").val() != "") {
        var PK = CreateGuid();
        var selectedType = document.getElementById("ddlCategory").value;
        var offsetY = 0;
        var CoordType = $("#ddlTxtCoordType").val();
        var lon = 0;
        var lat = 0;

        if (CoordType == "DMS") {
            var lonObj = $("#txtTxtLon").val() != "" ? $("#txtTxtLon").val().split(" ") : "";
            var lonD = lonObj[0];
            var lonM = lonObj[1];
            var lonS = lonObj[2];

            var latObj = $("#txtTxtLat").val() != "" ? $("#txtTxtLat").val().split(" ") : "";
            var latD = latObj[0];
            var latM = latObj[1];
            var latS = latObj[2];

            lon = (lonD != "" ? parseFloat(lonD) : 0)
                + (isNaN(parseFloat(lonM)) ? 0 : parseFloat(lonM)) / 60
                + (isNaN(parseFloat(lonS)) ? 0 : parseFloat(lonS)) / 3600;

            lat = (latD != "" ? parseFloat(latD) : 0)
                + (isNaN(parseFloat(latM)) ? 0 : parseFloat(latM)) / 60
                + (isNaN(parseFloat(latS)) ? 0 : parseFloat(latS)) / 3600;

            if ($("#ddlTxtLon").val() == "W") {
                lon = -Math.abs(lon);
            }

            if ($("#ddlTxtLat").val() == "S") {
                lat = -Math.abs(lat);
            }
        } else {
            lon = (isNaN(parseFloat($("#txtTxtLonDD").val())) ? 0 : parseFloat($("#txtTxtLonDD").val()));
            lat = (isNaN(parseFloat($("#txtTxtLatDD").val())) ? 0 : parseFloat($("#txtTxtLatDD").val()));
        }

        const latValid = lat >= -90 && lat <= 90;
        const lonValid = lon >= -180 && lon <= 180;

        if (!latValid || !lonValid) {
            alert("輸入的經緯度超出合法範圍！");
            return;
        }

        if (lon != 0 && lat != 0) {
            var Coord3857 = ol.proj.transform([lon, lat], "EPSG:4326", "EPSG:3857");
            var olGeom = new ol.geom.Point(Coord3857);
            var olFeature = new ol.Feature(olGeom);
            if ($("#rdoTxtCoordYes")[0].checked) {
                var CoordTxt = lon.toFixed(3) + "," + lat.toFixed(3);
                textTxt = textTxt;
                olFeature.set("IsCoord", true);
                textIsCoord = true;
            }
            var StyleArr = oltmx.Plugin.prototype.setTextMarkStyle(textTxt, textColor, textWidth, textStrokeColor, textScale, olFeature, offsetY);
            olFeature.set('type', 'drawfeature');
            olFeature.set('num', 'text' + txtNum);
            olFeature.setId(PK);
            drawSource.addFeature(olFeature);
            if (!(document.getElementById("MarkList").children[selectedType + txtNum])) {
                genMarkList(PK, selectedType, txtNum, textTxt, "text");
                txtNum++;
            }

            var Style = StyleArr;
            olFeature.setStyle(Style);
            map.removeInteraction(drawInteraction);
            //TransformInteraction.on('select', selectEvt);
        }
    } else {
        alert('請輸入文字內容!');
    }
}

//建立自訂線段標註
function addLineMark() {
    if (true && $("#txtLineName").val() != "") {
        var PK = CreateGuid();
        var selectedType = document.getElementById("ddlCategory").value;
        var LineCoord = [];
        var HasIconS = "non";
        var HasIconE = "non";
        var CoordType = $("#ddlLineCoordType").val();

        if (CoordType == "DMS") {
            var EachPt = $("#divLineDMS input");
            var EachType = $("#divLineDMS select");
            for (i = 0; i < EachPt.length; i += 2) {
                var latObj = EachPt[i].value != "" ? EachPt[i].value.split(" ") : "";
                var latD = latObj[0];
                var latM = latObj[1];
                var latS = latObj[2];

                var lonObj = EachPt[i + 1].value != "" ? EachPt[i + 1].value.split(" ") : "";
                var lonD = lonObj[0];
                var lonM = lonObj[1];
                var lonS = lonObj[2];

                lon = (lonD != "" ? parseFloat(lonD) : 0)
                    + (isNaN(parseFloat(lonM)) ? 0 : parseFloat(lonM)) / 60
                    + (isNaN(parseFloat(lonS)) ? 0 : parseFloat(lonS)) / 3600;

                lat = (latD != "" ? parseFloat(latD) : 0)
                    + (isNaN(parseFloat(latM)) ? 0 : parseFloat(latM)) / 60
                    + (isNaN(parseFloat(latS)) ? 0 : parseFloat(latS)) / 3600;

                if (EachType[i].value == "W") {
                    lon = -Math.abs(lon);
                }

                if (EachType[i + 1].value == "S") {
                    lat = -Math.abs(lat);
                }
                var Coord3857 = ol.proj.transform([lon, lat], "EPSG:4326", "EPSG:3857");
                LineCoord.push(Coord3857);
            }
        } else {
            var EachPt = $("#divLineDD input");
            for (i = 0; i < EachPt.length; i += 2) {
                lat = (isNaN(parseFloat(EachPt[i].value)) ? 0 : parseFloat(EachPt[i].value));
                lon = (isNaN(parseFloat(EachPt[i + 1].value)) ? 0 : parseFloat(EachPt[i + 1].value));
                var Coord3857 = ol.proj.transform([lon, lat], "EPSG:4326", "EPSG:3857");
                LineCoord.push(Coord3857);
            }
        }

        const latValid = lat >= -90 && lat <= 90;
        const lonValid = lon >= -180 && lon <= 180;

        if (!latValid || !lonValid) {
            alert("輸入的經緯度超出合法範圍！");
            return;
        }

        var olGeom = new ol.geom.LineString(LineCoord);
        var olFeature = new ol.Feature(olGeom);
        var LineTxt = "線" + lineNum; //$("#txtLineName").val() == "" ? $("#txtLineName").val("線" + lineNum) : $("#txtLineName").val();
        var LineGeom = olFeature.getGeometry();
        var LineStyle = [];
        var LineDash = 0;   // $("#ddlLineDash").val();
        var LineTypeS = "non";//$("#ddlLineTypeS").val();
        var LineTypeE = "non";// $("#ddlLineTypeE").val();
        lineDash1 = parseInt(LineDash);
        lineDash2 = parseInt(LineDash);
        var StyleArr = oltmx.Plugin.prototype.setLineMarkStyle(lineColor, lineWidth, LineTxt, lineDash1, lineDash2, olFeature);
        LineStyle.push(StyleArr);
        //設定起點Style
        if (LineTypeS != "non") {
            var IconFileName = LineTypeS;
            HasIconS = LineTypeS;
            var start = LineGeom.getCoordinateAt(0.99);
            var end = LineGeom.getFirstCoordinate();

            var dx = end[0] - start[0];
            var dy = end[1] - start[1];
            var rotation = Math.atan2(dy, dx);
            LineStyle.push(new ol.style.Style({
                geometry: new ol.geom.Point(end),
                image: new ol.style.Icon({
                    src: 'images/' + IconFileName + '.png',
                    color: lineColor,
                    anchor: [0.75, 0.5],
                    rotateWithView: true,
                    rotation: -rotation
                }),
                zIndex: 9999999
            }));
        }

        //設定終點Style
        if (LineTypeE != "non") {
            HasIconE = LineTypeE;
            var IconFileName = LineTypeE;
            //終點
            var start = LineGeom.getCoordinateAt(0.99);
            var end = LineGeom.getLastCoordinate();
            var dx = end[0] - start[0];
            var dy = end[1] - start[1];
            var rotation = Math.atan2(dy, dx);
            LineStyle.push(new ol.style.Style({
                geometry: new ol.geom.Point(end),
                image: new ol.style.Icon({
                    src: 'images/' + IconFileName + '.png',
                    color: lineColor,
                    anchor: [0.75, 0.5],
                    rotateWithView: true,
                    rotation: -rotation
                }),
                zIndex: 9999999
            }));
        }

        olFeature.setStyle(LineStyle);
        olFeature.set('type', 'drawfeature');
        olFeature.set('HasIconS', HasIconS);
        olFeature.set('HasIconE', HasIconE);
        olFeature.set('num', 'line' + lineNum);
        olFeature.setId(PK);

        //if ($("#rdoTotalYes")[0].checked) {
        //    AddLineTotalLen(olFeature, PK, "ddlLineUnit");
        //    olFeature.set("IsTotalNum", true);
        //    lineIsLen = true;
        //    HasTol = 1;
        //}

        //if ($("#rdoLenYes")[0].checked) {
        //    AddLineSegLen(olFeature, PK, "ddlLineUnit");
        //    olFeature.set("IsSegNum", true);
        //    HasSeg = 1;
        //}
        drawSource.addFeature(olFeature);
        if (!(document.getElementById("MarkList").children[selectedType + lineNum])) {
            genMarkList(PK, selectedType, lineNum, LineTxt, "line");
            lineNum++;
        }
        $(".addLine").remove();
        lineTdNum = 3;
        for (i = 0; i < EachPt.length; i++) {
            EachPt[i].value = "";
        }
        map.removeInteraction(drawInteraction);
        //TransformInteraction.on('select', selectEvt);
    } else {
        alert('請輸入名稱!');
    }
}

//建立自訂多邊形
function addPolyMark() {
    let isErr = false;
    if (true && $("#txtPolyName").val() != "") {
        if ($("#tbPolyPt .addPoly").length >= 6) {
            var PK = CreateGuid();
            var selectedType = document.getElementById("ddlCategory").value;
            var polyCoords = [];
            var FirstPt = [];

            var CoordType = $("#ddlPolyCoordType").val();
            if (CoordType == "DMS") {
                var EachPt = $("#divPolyDMS input");
                var EachType = $("#divPolyDMS select");
                for (i = 0; i < EachPt.length; i += 2) {
                    var latObj = EachPt[i].value != "" ? EachPt[i].value.split(" ") : "";
                    var latD = latObj[0];
                    var latM = latObj[1];
                    var latS = latObj[2];

                    var lonObj = EachPt[i + 1].value != "" ? EachPt[i + 1].value.split(" ") : "";
                    var lonD = lonObj[0];
                    var lonM = lonObj[1];
                    var lonS = lonObj[2];

                    lon = (lonD != "" ? parseFloat(lonD) : 0)
                        + (isNaN(parseFloat(lonM)) ? 0 : parseFloat(lonM)) / 60
                        + (isNaN(parseFloat(lonS)) ? 0 : parseFloat(lonS)) / 3600;

                    lat = (latD != "" ? parseFloat(latD) : 0)
                        + (isNaN(parseFloat(latM)) ? 0 : parseFloat(latM)) / 60
                        + (isNaN(parseFloat(latS)) ? 0 : parseFloat(latS)) / 3600;

                    if (EachType[i].value == "W") {
                        lon = -Math.abs(lon);
                    }

                    if (EachType[i + 1].value == "S") {
                        lat = -Math.abs(lat);
                    }

                    // 判斷是否輸入錯誤（經緯反了）
                    if (Math.abs(lat) > 90 && Math.abs(lon) <= 90) {
                        alert("輸入的經緯度超出合法範圍！");
                        isErr = true;
                        break;
                    }

                    var Coord3857 = ol.proj.transform([lon, lat], "EPSG:4326", "EPSG:3857");
                    polyCoords.push(Coord3857);
                    if (i == 0) {
                        FirstPt = Coord3857;
                    }
                }
            } else {
                var EachPt = $("#divPolyDD input");
                for (i = 0; i < EachPt.length; i += 2) {
                    lat = (isNaN(parseFloat(EachPt[i].value)) ? 0 : parseFloat(EachPt[i].value));
                    lon = (isNaN(parseFloat(EachPt[i + 1].value)) ? 0 : parseFloat(EachPt[i + 1].value));

                    // 判斷是否輸入錯誤（經緯反了）
                    if (Math.abs(lat) > 90 && Math.abs(lon) <= 90) {
                        alert("輸入的經緯度超出合法範圍！");
                        isErr = true;
                        break;
                    }

                    var Coord3857 = ol.proj.transform([lon, lat], "EPSG:4326", "EPSG:3857");
                    polyCoords.push(Coord3857);
                    if (i == 0) {
                        FirstPt = Coord3857;
                    }
                }
            }
            polyCoords.push(FirstPt);

            if (isErr) {
                return;
            }
            var olGeom = new ol.geom.Polygon([polyCoords]);
            var olFeature = new ol.Feature(olGeom);
            var PolyTxt = "多邊形" + polyNum; // $("#txtPolyName").val() == "" ? $("#txtPolyName").val("多邊形" + polyNum) : $("#txtPolyName").val();
            var PolyDash = 0;// $("#ddlPolyDash").val();
            polyDash1 = parseInt(PolyDash);
            polyDash2 = parseInt(PolyDash);
            var polyStyle = oltmx.Plugin.prototype.setPolyMarkStyle(polyFillColor, polyColor, polyWidth, PolyTxt, polyDash1, polyDash2, olFeature);
            olFeature.setStyle(polyStyle);
            olFeature.set('type', 'drawfeature');
            olFeature.set('num', 'poly' + polyNum);
            olFeature.setId(PK);

            //if ($("#rdoPolyAreaYes")[0].checked) {
            //    AddArea(olFeature, PK, "Poly");
            //    olFeature.set("HasArea", true);
            //    polyIsArea = true;
            //    HasArea = 1;
            //}
            //邊長顯示
            //if ($("#rdoPolySLenYes")[0].checked) {
            //    AddLineSegLen(olFeature, PK, "ddlPolyUnit");
            //    olFeature.set("IsSegNum", true);
            //    polyIsSeg = true;
            //    HasSeg = 1;
            //}
            //周長顯示
            //if ($("#rdoPolyTLenYes")[0].checked) {
            //    AddLineTotalLen(olFeature, PK, "ddlPolyUnit");
            //    olFeature.set("IsTotalNum", true);
            //    polyIsLen = true;
            //    HasTol = 1;
            //}

            drawSource.addFeature(olFeature);
            if (!(document.getElementById("MarkList").children[selectedType + polyNum])) {
                genMarkList(PK, selectedType, polyNum, PolyTxt, "poly");
                polyNum++;
            }
            map.removeInteraction(drawInteraction);
            //resetPolyCoords();
            $("._CreatePolyCoord").remove();
            //TransformInteraction.on('select', selectEvt);
        } else {
            alert('多邊形至少需要三個點!');
            return false;
        }
    } else {
        alert('請輸入名稱!');
        return false;
    }
}

//建立自訂矩形圖徵
function addBoxMark() {
    if (true && $("#txtBoxName").val() != "") {
        var PK = CreateGuid();
        var selectedType = document.getElementById("ddlCategory").value;
        var boxCoords = [];
        var FirstPt = [];
        var leftUpX = 0;
        var leftUpY = 0;
        var rightDownX = 0;
        var rightDownY = 0;

        var CoordType = $("#ddlBoxCoordType").val();
        if (CoordType == "DMS") {
            var objLU_X = $("#txtBoxLon1").val().split(" ");
            var objLU_Y = $("#txtBoxLat1").val().split(" ");
            var objRD_X = $("#txtBoxLon2").val().split(" ");
            var objRD_Y = $("#txtBoxLat2").val().split(" ");

            leftUpX = (objLU_X[0] != "" ? parseFloat(objLU_X[0]) : 0)
                + (isNaN(parseFloat(objLU_X[1])) ? 0 : parseFloat(objLU_X[1])) / 60
                + (isNaN(parseFloat(objLU_X[2])) ? 0 : parseFloat(objLU_X[2])) / 3600;

            leftUpY = (objLU_Y[0] != "" ? parseFloat(objLU_Y[0]) : 0)
                + (isNaN(parseFloat(objLU_Y[1])) ? 0 : parseFloat(objLU_Y[1])) / 60
                + (isNaN(parseFloat(objLU_Y[2])) ? 0 : parseFloat(objLU_Y[2])) / 3600;

            rightDownX = (objRD_X[0] != "" ? parseFloat(objRD_X[0]) : 0)
                + (isNaN(parseFloat(objRD_X[1])) ? 0 : parseFloat(objRD_X[1])) / 60
                + (isNaN(parseFloat(objRD_X[2])) ? 0 : parseFloat(objRD_X[2])) / 3600;

            rightDownY = (objRD_Y[0] != "" ? parseFloat(objRD_Y[0]) : 0)
                + (isNaN(parseFloat(objRD_Y[1])) ? 0 : parseFloat(objRD_Y[1])) / 60
                + (isNaN(parseFloat(objRD_Y[2])) ? 0 : parseFloat(objRD_Y[2])) / 3600;
        } else {
            leftUpX = (isNaN(parseFloat($("#txtBoxLonDD1").val())) ? 0 : parseFloat($("#txtBoxLonDD1").val()));
            leftUpY = (isNaN(parseFloat($("#txtBoxLatDD1").val())) ? 0 : parseFloat($("#txtBoxLatDD1").val()));
            rightDownX = (isNaN(parseFloat($("#txtBoxLonDD2").val())) ? 0 : parseFloat($("#txtBoxLonDD2").val()));
            rightDownY = (isNaN(parseFloat($("#txtBoxLatDD2").val())) ? 0 : parseFloat($("#txtBoxLatDD2").val()));
        }

        if ((Math.abs(leftUpY) > 90 && Math.abs(leftUpX) <= 90) ||
            (Math.abs(rightDownY) > 90 && Math.abs(rightDownX) <= 90)) {
            alert("輸入的經緯度超出合法範圍！");
            return;
        }

        if (leftUpX != 0 && leftUpY != 0 && rightDownX != 0 && rightDownY != 0) {
            var leftUp = [leftUpX, leftUpY];
            var rightDown = [rightDownX, rightDownY];
            var leftDown = [leftUp[0], rightDown[1]];
            var rightUp = [rightDown[0], leftUp[1]];

            var leftUp3857 = ol.proj.transform(leftUp, "EPSG:4326", "EPSG:3857");
            var rightDown3857 = ol.proj.transform(rightDown, "EPSG:4326", "EPSG:3857");
            var leftDown3857 = ol.proj.transform(leftDown, "EPSG:4326", "EPSG:3857");
            var rightUp3857 = ol.proj.transform(rightUp, "EPSG:4326", "EPSG:3857");

            FirstPt = leftUp3857;
            boxCoords.push(leftUp3857);
            boxCoords.push(leftDown3857);
            boxCoords.push(rightDown3857);
            boxCoords.push(rightUp3857);
            boxCoords.push(FirstPt);

            var olGeom = new ol.geom.Polygon([boxCoords]);
            var olFeature = new ol.Feature(olGeom);
            var BoxTxt = "矩形" + boxNum; // $("#txtBoxName").val() == "" ? "矩形" + boxNum : $("#txtBoxName").val();
            var BoxDash = 0;// $("#ddlBoxDash").val();
            boxDash1 = parseInt(BoxDash);
            boxDash2 = parseInt(BoxDash);
            var StyleArr = oltmx.Plugin.prototype.setBoxMarkStyle(boxFillColor, boxColor, boxWidth, BoxTxt, boxDash1, boxDash2, olFeature);
            var boxStyle = StyleArr;
            olFeature.setStyle(boxStyle);
            olFeature.set('type', 'drawfeature');
            olFeature.set('num', 'box' + boxNum);
            olFeature.setId(PK);

            //if ($("#rdoBoxAreaYes")[0].checked) {
            //    AddArea(olFeature, PK, "Box");
            //    olFeature.set("HasArea", true);
            //    boxIsArea = true;
            //}

            //邊長顯示
            //if ($("#rdoBoxSLenYes")[0].checked) {
            //    AddLineSegLen(olFeature, PK, "ddlBoxUnit");
            //    olFeature.set("IsSegNum", true);
            //    boxIsSeg = true;
            //}
            //周長顯示
            //if ($("#rdoBoxTLenYes")[0].checked) {
            //    AddLineTotalLen(olFeature, PK, "ddlBoxUnit");
            //    olFeature.set("IsTotalNum", true);
            //    boxIsLen = true;
            //    HasTol = 1;
            //}

            drawSource.addFeature(olFeature);
            if (!(document.getElementById("MarkList").children[selectedType + boxNum])) {
                genMarkList(PK, selectedType, boxNum, BoxTxt, "box");
                boxNum++;
            }
            map.removeInteraction(drawInteraction);
            //TransformInteraction.on('select', selectEvt);

            $("#txtBoxLon1").val("");
            $("#txtBoxLat1").val("");
            $("#txtBoxLon2").val("");
            $("#txtBoxLat2").val("");
            $("#txtBoxLonDD1").val("");
            $("#txtBoxLatDD1").val("");
            $("#txtBoxLonDD2").val("");
            $("#txtBoxLatDD2").val("");
        }
    } else {
        alert("請輸入名稱!");
    }
}

//建立自訂圓形圖徵
function addCirMark() {
    var PK = CreateGuid();
    var selectedType = document.getElementById("ddlCategory").value;

    var CoordType = $("#ddlCirCoordType").val();
    var lon = 0;
    var lat = 0;

    if (CoordType == "DMS") {
        var lonObj = $("#txtCirLon").val() != "" ? $("#txtCirLon").val().split(" ") : "";
        var lonD = lonObj[0];
        var lonM = lonObj[1];
        var lonS = lonObj[2];

        var latObj = $("#txtCirLat").val() != "" ? $("#txtCirLat").val().split(" ") : "";
        var latD = latObj[0];
        var latM = latObj[1];
        var latS = latObj[2];

        lon = (lonD != "" ? parseFloat(lonD) : 0)
            + (isNaN(parseFloat(lonM)) ? 0 : parseFloat(lonM)) / 60
            + (isNaN(parseFloat(lonS)) ? 0 : parseFloat(lonS)) / 3600;

        lat = (latD != "" ? parseFloat(latD) : 0)
            + (isNaN(parseFloat(latM)) ? 0 : parseFloat(latM)) / 60
            + (isNaN(parseFloat(latS)) ? 0 : parseFloat(latS)) / 3600;

        if ($("#ddlCirLon").val() == "W") {
            lon = -Math.abs(lon);
        }

        if ($("#ddlCirLat").val() == "S") {
            lat = -Math.abs(lat);
        }
    } else {
        lon = (isNaN(parseFloat($("#txtCirLonDD").val())) ? 0 : parseFloat($("#txtCirLonDD").val()));
        lat = (isNaN(parseFloat($("#txtCirLatDD").val())) ? 0 : parseFloat($("#txtCirLatDD").val()));
    }

    const latValid = lat >= -90 && lat <= 90;
    const lonValid = lon >= -180 && lon <= 180;

    if (!latValid || !lonValid) {
        alert("輸入的經緯度超出合法範圍！");
        return;
    }

    var radius = $("#txtCirRadius").val() != "" ? parseFloat($("#txtCirRadius").val()) : 0;
    if (lon != 0 && lat != 0 && radius != 0) {
        var CirUnit = 'm';// $("#ddlCirUnit").val();
        if (CirUnit == 'km') {
            radius = radius * 1000;
        } else if (CirUnit == 'nm') {
            radius = radius * 1852;
        }
        var IsGeodesy = false; // $("#rdoCirAcrossYes")[0].checked ? true : false;
        var Center3857 = ol.proj.transform([lon, lat], "EPSG:4326", "EPSG:3857");
        var olFeature = new ol.Feature(new ol.geom.Circle(Center3857, radius));
        var Center4326 = ol.proj.transform(Center3857, 'EPSG:3857', 'EPSG:4326');
        //var Sphere = new ol.Sphere(6378137);
        //var Circle4326 = ol.geom.Polygon.circular(Sphere, Center4326, radius, 256);
        //var Geom = Circle4326.clone().transform('EPSG:4326', 'EPSG:3857');
        //olFeature.setGeometry(Geom);        
        olFeature.set('type', 'drawfeature');
        olFeature.set('num', 'cir' + cirNum);
        olFeature.set('Radius', radius);
        olFeature.set('CirUnit', CirUnit);
        olFeature.setId(PK);
        var cirTxt = "圓形" + cirNum;// $("#txtCirName").val() == "" ? "圓形" + cirNum : $("#txtCirName").val();
        var CirDash = 0;// $("#ddlCirDash").val();
        cirDash1 = parseInt(CirDash);
        cirDash2 = parseInt(CirDash);
        var cirStyle = oltmx.Plugin.prototype.setCirMarkStyle(cirFillColor, cirColor, cirWidth, cirTxt, cirIsRadius, cirDash1, cirDash2, olFeature);
        olFeature.setStyle(cirStyle);

        //if ($("#rdoCirAreaYes")[0].checked) {
        //    AddArea(olFeature, PK, "Cir");
        //    olFeature.set("HasArea", true);
        //}

        //if ($("#rdoRadiusYes")[0].checked) {
        //    AddRadius(olFeature, PK);
        //    olFeature.set("HasRadius", true);
        //    cirIsRadius = true;
        //}
        //是否顯示經緯度
        //if ($("#rdoCirCoordYes")[0].checked) {
        //    AddCoordMark(olFeature, PK);
        //    olFeature.set("IsCoord", true);
        //    cirIsCoord = true;
        //}
        //是否依大地坐標繪製
        if (IsGeodesy) {
            if (Center4326[1] > 77 || Center4326[1] < -77) {
                QuyGeodesyCircle(Center4326[0], Center4326[1], radius, olFeature);
                olFeature.set("Lon", Center4326[0]);
                olFeature.set("Lat", Center4326[1]);
                olFeature.set("CirGeom", olFeature.getGeometry());
            } else {
                var Geom = Circle4326.clone().transform('EPSG:4326', 'EPSG:3857');
                olFeature.setGeometry(Geom);
            }
            olFeature.set("IsCross", true);
            cirIsGeodesy = true;
        }

        drawSource.addFeature(olFeature);
        if (!(document.getElementById("MarkList").children[selectedType + cirNum])) {
            genMarkList(PK, selectedType, cirNum, cirTxt, "cir");
            cirNum++;
        }
        map.removeInteraction(drawInteraction);
        //TransformInteraction.on('select', selectEvt);
    }
}

//刪除自訂坐標
function deleteCoordCtrl(ele) {
    Array.from(ele).forEach(function (item) {
        item.remove();
    });
}

// #endregion

// #region 介面新增座標

//自訂線坐標-產製TextBox
var lineTdNum = 3;
function addLinePt() {
    var Str = "";
    var CoordType = $("#ddlLineCoordType").val();

    if (CoordType == "DMS") {
        Str += '<div class="mb-2 addLine"><div id="divLinePt' + lineTdNum + '" class="d-flex" style="gap: 4px;"><div><div class="input-group mb-1">';
        Str += '<span class="input-group-text">緯度</span>';
        Str += '<select id="ddlLineLat' + lineTdNum + '" class="form-select"><option value="N">N</option><option value="S">S</option></select>';
        Str += '<input type="text" id="txtLineLat' + lineTdNum + '" class="form-control" autocomplete="off" />';
        Str += '</div>';

        Str += '<div class="input-group"><span class="input-group-text">經度</span>';
        Str += '<select id="ddlLineLon' + lineTdNum + '" class="form-select"><option value="E">E</option><option value="W">W</option></select>';
        Str += '<input type="text" id="txtLineLon' + lineTdNum + '" class="form-control" autocomplete="off" />';
        Str += '</div></div>';
        Str += '<button class="btn icon btn-sm btn-danger" type="button" onclick="deletePtByID(' + "'divLinePt" + lineTdNum + "'" + ');"><i class="fa-solid fa-trash-can"></i></button></div>';
        $("#divLineDMS").append(Str);
    } else {
        Str += '<div class="mb-2 addLine"><div id="divLinePtDD' + lineTdNum + '" class="d-flex" style="gap: 4px;"><div><div class="input-group mb-1">';
        Str += '<span class="input-group-text">緯度</span>';
        Str += '<input type="text" id="txtLineLatDD' + lineTdNum + '" class="form-control" autocomplete="off" />';
        Str += '</div>';

        Str += '<div class="input-group"><span class="input-group-text">經度</span>';
        Str += '<input type="text" id="txtLineLonDD' + lineTdNum + '" class="form-control" autocomplete="off" />';
        Str += '</div></div>';
        Str += '<button class="btn icon btn-sm btn-danger" type="button" onclick="deletePtByID(' + "'divLinePtDD" + lineTdNum + "'" + ');"><i class="fa-solid fa-trash-can"></i></button></div>';
        $("#divLineDD").append(Str);
    }
    lineTdNum++;
}

//自訂多邊形坐標-產製TextBox
var polyTdNum = 4;
function addPolyPt() {
    var Str = "";
    var CoordType = $("#ddlPolyCoordType").val();
    //var polyTdNum = ($("#tbPolyPt .addPoly").length / 2) + 1;
    if (CoordType == "DMS") {
        //DMS
        Str += '<div class="mb-2 addPoly _CreatePolyCoord"><div id="divPolyPt' + polyTdNum + '" class="d-flex" style="gap: 4px;"><div><div class="input-group mb-1">';
        Str += '<span class="input-group-text">緯度</span>';
        Str += '<select id="ddlPolyLat' + polyTdNum + '" class="form-select"><option value="N">N</option><option value="S">S</option></select>';
        Str += '<input type="text" id="txtPolyLat' + polyTdNum + '" class="form-control" autocomplete="off" />';
        Str += '</div>';

        Str += '<div class="input-group"><span class="input-group-text">經度</span>';
        Str += '<select id="ddlPolyLon' + polyTdNum + '" class="form-select"><option value="E">E</option><option value="W">W</option></select>';
        Str += '<input type="text" id="txtPolyLon' + polyTdNum + '" class="form-control" autocomplete="off" />';
        Str += '</div></div>';
        Str += '<button class="btn icon btn-sm btn-danger" type="button" onclick="deletePtByID(' + "'divPolyPt" + polyTdNum + "'" + ');"><i class="fa-solid fa-trash-can"></i></button></div>';
        $("#divPolyDMS").append(Str);
    } else {
        //DD
        Str = "";
        Str += '<div class="mb-2 addPoly _CreatePolyCoord"><div id="divPolyPtDD' + polyTdNum + '" class="d-flex" style="gap: 4px;"><div><div class="input-group mb-1">';
        Str += '<span class="input-group-text">緯度</span>';
        Str += '<input type="text" id="txtPolyLatDD' + polyTdNum + '" class="form-control" autocomplete="off" />';
        Str += '</div>';

        Str += '<div class="input-group"><span class="input-group-text">經度</span>';
        Str += '<input type="text" id="txtPolyLonDD' + polyTdNum + '" class="form-control" autocomplete="off" />';
        Str += '</div></div>';
        Str += '<button class="btn icon btn-sm btn-danger" type="button" onclick="deletePtByID(' + "'divPolyPtDD" + polyTdNum + "'" + ');"><i class="fa-solid fa-trash-can"></i></button></div>';
        $("#divPolyDD").append(Str);
    }
    polyTdNum++;
}

//重設面座標
function resetPolyCoords() {
    $("._CreatePolyCoord").remove();
    for (i = 0; i < 3; i++) {
        var polyTdNum = $("#divPolyDMS .addPoly").length + 1;
        var Str = '<div class="mb-2 addPoly _CreatePolyCoord"><div id="divPolyPt' + polyTdNum + '" class="d-flex" style="gap: 4px;"><div><div class="input-group mb-1">';
        Str += '<span class="input-group-text">緯度</span>';
        Str += '<select id="ddlPolyLat' + polyTdNum + '" class="form-select"><option value="N">N</option><option value="S">S</option></select>';
        Str += '<input type="text" id="txtPolyLat' + polyTdNum + '" class="form-control" autocomplete="off" />';
        Str += '</div>';

        Str += '<div class="input-group"><span class="input-group-text">經度</span>';
        Str += '<select id="ddlPolyLon' + polyTdNum + '" class="form-select"><option value="E">E</option><option value="W">W</option></select>';
        Str += '<input type="text" id="txtPolyLon' + polyTdNum + '" class="form-control" autocomplete="off" />';
        Str += '</div></div></div>';
        $("#divPolyDMS").append(Str);
    }

    for (i = 0; i < 3; i++) {
        var polyTdNum = $("#divPolyDD .addPoly").length + 1;
        var Str = '<div class="mb-2 addPoly _CreatePolyCoord"><div id="divPolyPt' + polyTdNum + '" class="d-flex" style="gap: 4px;"><div><div class="input-group mb-1">';
        Str += '<span class="input-group-text">緯度</span>';
        Str += '<input type="text" id="txtPolyLatDD' + polyTdNum + '" class="form-control" autocomplete="off" />';
        Str += '</div>';

        Str += '<div class="input-group"><span class="input-group-text">經度</span>';
        Str += '<input type="text" id="txtPolyLonDD' + polyTdNum + '" class="form-control" autocomplete="off" />';
        Str += '</div></div></div>';
        $("#divPolyDD").append(Str);
    }
}

//刪除指定座標
function deletePtByID(eleID) {
    $("#" + eleID).parent().remove();
}

//定位圖徵
function locateToFeature(PK) {
    var olFeature = drawSource.getFeatureById(PK);
    var ext = olFeature.getGeometry().getExtent();
    map.getView().fit(ext, map.getSize());
}

//點選列表選取圖徵並編輯樣式
function selectFeature(PK) {
    IsMarkEdit = true;
    //TransformInteraction.setActive(true);
    var olFeature = drawSource.getFeatureById(PK);
    var Ext = olFeature.getGeometry().getExtent();
    //TransformInteraction.select(olFeature, true);
    var Center3857 = ol.extent.getCenter(Ext);
    map.getView().setCenter(Center3857);
    $("#P" + PK).addClass("active");
    CurrentMarkID = PK;
    //map.getView().fit(Ext, {
    //    size: map.getSize(),
    //    maxZoom: 62
    //});
}

//修改標註樣式
function UpdateMark() {
    if (CurrentNum != "") {
        var olF = drawSource.getFeatureById(CurrentMarkID);
        var BtnID = "BtnSave" + CurrentNum;
        $("#" + BtnID).click();
        IsMarkEdit = false;
        MarkOriStyle = null;
        CurrentMarkID = "";
        CurrentNum = "";
        //TransformInteraction.select(olF, false);
    }
    $(".btnBoxCenter").show();
    $("#divUpdateBtn").hide();
}

//列表Hover效果
function hoverFeature(PK, markType, IsHover) {
    var olFeature = drawSource.getFeatureById(PK);
    if (IsHover) {
        if (olFeature != null) {
            //設定預設樣式，取消編輯時用來恢復原本的樣式
            if (olFeature.getStyle().length != undefined) {
                MarkOriStyle = [];
                for (i = 0; i < olFeature.getStyle().length; i++) {
                    MarkOriStyle.push(olFeature.getStyle()[i].clone());
                }
            } else {
                MarkOriStyle = olFeature.getStyle().clone();
            }

            var olStyle = olFeature.getStyle();
            if (olStyle.length != undefined) {
                olStyle = olStyle[0];
            }
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
            //drawSource.refresh();
            olFeature.changed();

        }

        //var Ext = olFeature.getGeometry().getExtent();
        //var Center3857 = ol.extent.getCenter(Ext);
        //map.getView().setCenter(Center3857);
    } else if (MarkOriStyle != null && olFeature != null) {
        //恢復原本樣式
        olFeature.setStyle(MarkOriStyle);
        MarkOriStyle = null;
    }
}

//取消編輯
function CancelUpdateMark() {
    if (MarkOriStyle != null && CurrentMarkID != "") {
        //顯示建立按鈕
        $(".btnBoxCenter").show();
        var olF = drawSource.getFeatureById(CurrentMarkID);
        //恢復原本樣式
        olF.setStyle(MarkOriStyle);
        //取消編輯狀態
        map.removeInteraction(modifyInteraction);
        map.removeInteraction(drawInteraction);
        $("#P" + CurrentMarkID).removeClass("active");
        IsMarkEdit = false;
        MarkOriStyle = null;
        CurrentMarkID = "";
        CurrentNum = "";
        //TransformInteraction.select(olF, false);
        $("#divUpdateBtn").hide();
        $("._divMarkCoord").find('input').val("");
        $(".addLine").remove();
        $("._CreatePolyCoord").remove();
    }
}

//帶入圖徵原始樣式
function setMkCtrlStyle(olFeature, markType, Text) {
    var Coord3857 = olFeature.getGeometry().getCoordinates();
    switch (markType) {
        case 'text':
            $('.sp-preview-inner')[4].style.backgroundColor = textColor;
            $('.sp-preview-inner')[5].style.backgroundColor = textStrokeColor;
            $("#borderWidth").val(textWidth);
            $("#textfacility").val(textTxt);
            $("#textScale").val(textScale);
            var Coord4326 = ol.proj.transform(Coord3857, "EPSG:3857", "EPSG:4326");
            var CoordHDMS = ol.coordinate.toStringHDMS(Coord4326, 5);
            var EachHDMS = CoordHDMS.split(' ');
            $("#ddlTxtLon").val(EachHDMS[7]);
            $("#ddlTxtLat").val(EachHDMS[3]);
            $("#txtTxtLon").val(EachHDMS[4].replace("°", " ") + EachHDMS[5].replace("′", " ") + EachHDMS[6].replace("″", ""));
            $("#txtTxtLat").val(EachHDMS[0].replace("°", " ") + EachHDMS[1].replace("′", " ") + EachHDMS[2].replace("″", ""));
            $("#txtTxtLonDD").val(Coord4326[0]);
            $("#txtTxtLatDD").val(Coord4326[1]);
            setRdoBtnChecked($("#rdoTxtCoordYes"), textIsCoord);
            setRdoBtnChecked($("#rdoTxtCoordNo"), !textIsCoord);
            break;
        case 'point':
            //$("#txtPtName").val(Text);
            //$('.sp-preview-inner')[0].style.backgroundColor = pointColor;
            //$("#iconsize").val(pointScale);
            var Opacity = pointOpacity;
            var Coord4326 = ol.proj.transform(Coord3857, "EPSG:3857", "EPSG:4326");
            var CoordHDMS = ol.coordinate.toStringHDMS(Coord4326, 5);
            var EachHDMS = CoordHDMS.split(' ');
            $("#ddlPtLon").val(EachHDMS[7]);
            $("#ddlPtLat").val(EachHDMS[3]);
            $("#txtPtLon").val(EachHDMS[4].replace("°", " ") + EachHDMS[5].replace("′", " ") + EachHDMS[6].replace("″", ""));
            $("#txtPtLat").val(EachHDMS[0].replace("°", " ") + EachHDMS[1].replace("′", " ") + EachHDMS[2].replace("″", ""));
            $("#txtPtLonDD").val(Coord4326[0]);
            $("#txtPtLatDD").val(Coord4326[1]);
            //var ImgSrc = olFeature.getStyle().getImage().getSrc != undefined ? olFeature.getStyle().getImage().getSrc() : "dot";
            //if (ImgSrc.indexOf("plane") > -1) {
            //    $('.balaIconPicker-common-icon')[1].click();
            //} else if (ImgSrc.indexOf("warship")) {
            //    $('.balaIconPicker-common-icon')[5].click();
            //} else if (ImgSrc.indexOf("ship")) {
            //    $('.balaIconPicker-common-icon')[2].click();
            //} else if (ImgSrc.indexOf("missile")) {
            //    $('.balaIconPicker-common-icon')[3].click();
            //} else if (ImgSrc.indexOf("tank")) {
            //    $('.balaIconPicker-common-icon')[4].click();
            //} else {
            //    $('.balaIconPicker-common-icon')[0].click();
            //}
            //setRdoBtnChecked($("#rdoPtCoordYes"), pointIsCoord);
            //setRdoBtnChecked($("#rdoPtCoordNo"), !pointIsCoord);
            break;
        case 'line':
            var HasIconS = olFeature.get('HasIconS');
            $("#ddlLineTypeS").val(HasIconS);
            var HasIconE = olFeature.get('HasIconE');
            $("#ddlLineTypeE").val(HasIconE);
            $("#txtLineName").val(Text);
            //$('.sp-preview-inner')[1].style.backgroundColor = lineColor;
            $("#linewidth").val(lineWidth);
            $("#ddlLineDash").val(lineDash1);
            setLineScope();
            setLineCoord(Coord3857);
            //setRdoBtnChecked($("#rdoLenYes"), lineIsSeg);
            //setRdoBtnChecked($("#rdoLenNo"), !lineIsSeg);
            //setRdoBtnChecked($("#rdoTotalYes"), lineIsLen);
            //setRdoBtnChecked($("#rdoTotalNo"), !lineIsLen);
            break;
        case 'poly':
            $("#txtPolyName").val(Text);
            //$('.sp-preview-inner')[2].style.backgroundColor = polyColor;
            //$('.sp-preview-inner')[3].style.backgroundColor = polyFillColor;
            $("#polywidth").val(polyWidth);
            $("#polyheight").val(polyHeight);
            $("#ddlPolyDash").val(polyDash1);
            setPolyScope();
            setPolyCoord(Coord3857);
            //setRdoBtnChecked($("#rdoPolyAreaYes"), polyIsArea);
            //setRdoBtnChecked($("#rdoPolyAreaNo"), !polyIsArea);
            //setRdoBtnChecked($("#rdoPolySLenYes"), polyIsSeg);
            //setRdoBtnChecked($("#rdoPolySLenNo"), !polyIsSeg);
            //setRdoBtnChecked($("#rdoPolyTLenYes"), polyIsLen);
            //setRdoBtnChecked($("#rdoPolyTLenNo"), !polyIsLen);
            break;
        case 'box':
            $("#txtBoxName").val(Text);
            //$('.sp-preview-inner')[6].style.backgroundColor = boxColor;
            //$('.sp-preview-inner')[7].style.backgroundColor = boxFillColor;
            $("#boxwidth").val(boxWidth);
            $("#boxheight").val(boxHeight);
            $("#ddlBoxDash").val(boxDash1);
            setBoxScope();
            setBoxCoord(Coord3857);
            //setRdoBtnChecked($("#rdoBoxAreaYes"), boxIsArea);
            //setRdoBtnChecked($("#rdoBoxAreaNo"), !boxIsArea);
            //setRdoBtnChecked($("#rdoBoxSLenYes"), boxIsSeg);
            //setRdoBtnChecked($("#rdoBoxSLenNo"), !boxIsSeg);
            break;
        case 'cir':
            $("#txtCirName").val(Text);
            //$('.sp-preview-inner')[8].style.backgroundColor = cirColor;
            //$('.sp-preview-inner')[9].style.backgroundColor = cirFillColor;
            $("#cirwidth").val(cirWidth);
            $("#cirheight").val(cirHeight);
            $("#ddlCirDash").val(cirDash1);
            var olExt = olFeature.getGeometry().getExtent();
            setCircleCoord(olExt, olFeature);
            //setRdoBtnChecked($("#rdoCirAreaYes"), cirIsArea);
            //setRdoBtnChecked($("#rdoCirAreaNo"), !cirIsArea);
            //setRdoBtnChecked($("#rdoRadiusYes"), cirIsRadius);
            //setRdoBtnChecked($("#rdoRadiusNo"), !cirIsRadius);
            //setRdoBtnChecked($("#rdoCirCoordYes"), cirIsCoord);
            //setRdoBtnChecked($("#rdoCirCoordNo"), !cirIsCoord);
            //setRdoBtnChecked($("#rdoCirAcrossYes"), cirIsGeodesy);
            //setRdoBtnChecked($("#rdoCirAcrossNo"), !cirIsGeodesy);
            break;
    }
}

//帶入線段坐標
function setLineCoord(Coord3857) {
    lineTdNum = 3;
    var ele = document.getElementsByClassName("addLine");
    deleteCoordCtrl(ele);

    if (Coord3857.length > 2) {
        for (i = 2; i < Coord3857.length; i++) {
            var Str = "";
            var StrDD = "";
            //DMS
            Str += '<div class="mb-2 addLine"><div id="divLinePt' + lineTdNum + '" class="d-flex" style="gap: 4px;"><div><div class="input-group mb-1">';
            Str += '<span class="input-group-text">緯度</span>';
            Str += '<select id="ddlLineLat' + lineTdNum + '" class="form-select"><option value="N">N</option><option value="S">S</option></select>';
            Str += '<input type="text" id="txtLineLat' + lineTdNum + '" class="form-control" autocomplete="off" />';
            Str += '</div>';

            Str += '<div class="input-group"><span class="input-group-text">經度</span>';
            Str += '<select id="ddlLineLon' + lineTdNum + '" class="form-select"><option value="E">E</option><option value="W">W</option></select>';
            Str += '<input type="text" id="txtLineLon' + lineTdNum + '" class="form-control" autocomplete="off" />';
            Str += '</div></div>';
            Str += '<button class="btn icon btn-sm btn-danger" type="button" onclick="deletePtByID(' + "'divLinePt" + lineTdNum + "'" + ');"><i class="fa-solid fa-trash-can"></i></button></div>';
            $("#divLineDMS").append(Str);
            //DD
            StrDD += '<div class="mb-2 addLine"><div id="divLinePtDD' + lineTdNum + '" class="d-flex" style="gap: 4px;"><div><div class="input-group mb-1">';
            StrDD += '<span class="input-group-text">緯度</span>';
            StrDD += '<input type="text" id="txtLineLatDD' + lineTdNum + '" class="form-control" autocomplete="off" />';
            StrDD += '</div>';

            StrDD += '<div class="input-group"><span class="input-group-text">經度</span>';
            StrDD += '<input type="text" id="txtLineLonDD' + lineTdNum + '" class="form-control" autocomplete="off" />';
            StrDD += '</div></div>';
            StrDD += '<button class="btn icon btn-sm btn-danger" type="button" onclick="deletePtByID(' + "'divLinePtDD" + lineTdNum + "'" + ');"><i class="fa-solid fa-trash-can"></i></button></div>';
            $("#divLineDD").append(StrDD);
            lineTdNum++;
        }
    }

    for (i = 0; i < Coord3857.length; i++) {
        var Coord4326 = ol.proj.transform(Coord3857[i], "EPSG:3857", "EPSG:4326");
        var CoordHDMS = ol.coordinate.toStringHDMS(Coord4326, 5);
        var EachHDMS = CoordHDMS.split(' ');
        var idx = i + 1;
        $("#ddlLineLon" + idx).val(EachHDMS[7]);
        $("#ddlLineLat" + idx).val(EachHDMS[3]);
        $("#txtLineLon" + idx).val(EachHDMS[4].replace("°", " ") + EachHDMS[5].replace("′", " ") + EachHDMS[6].replace("″", ""));
        $("#txtLineLat" + idx).val(EachHDMS[0].replace("°", " ") + EachHDMS[1].replace("′", " ") + EachHDMS[2].replace("″", ""));
        $("#txtLineLonDD" + idx).val(Coord4326[0]);
        $("#txtLineLatDD" + idx).val(Coord4326[1]);
    }
}

//帶入多邊形坐標
function setPolyCoord(Coord3857) {
    if ((Coord3857[0].length - 1) >= 3) {
        var ele = document.getElementsByClassName("_CreatePolyCoord");
        deleteCoordCtrl(ele);

        for (i = 3; i < Coord3857[0].length - 1; i++) {
            var Str = "";
            var StrDD = "";
            var polyTdNum = ($("#tbPolyPt .addPoly").length / 2) + 1;
            //DMS
            Str += '<div class="mb-2 addPoly _CreatePolyCoord"><div id="divPolyPt' + polyTdNum + '" class="d-flex" style="gap: 4px;"><div><div class="input-group mb-1">';
            Str += '<span class="input-group-text">緯度</span>';
            Str += '<select id="ddlPolyLat' + polyTdNum + '" class="form-select"><option value="N">N</option><option value="S">S</option></select>';
            Str += '<input type="text" id="txtPolyLat' + polyTdNum + '" class="form-control" autocomplete="off" />';
            Str += '</div>';

            Str += '<div class="input-group"><span class="input-group-text">經度</span>';
            Str += '<select id="ddlPolyLon' + polyTdNum + '" class="form-select"><option value="E">E</option><option value="W">W</option></select>';
            Str += '<input type="text" id="txtPolyLon' + polyTdNum + '" class="form-control" autocomplete="off" />';
            Str += '</div></div>';
            Str += '<button class="btn icon btn-sm btn-danger" type="button" onclick="deletePtByID(' + "'divPolyPt" + polyTdNum + "'" + ');"><i class="fa-solid fa-trash-can"></i></button></div>';
            $("#divPolyDMS").append(Str);
            //DD
            StrDD += '<div class="mb-2 addPoly _CreatePolyCoord"><div id="divPolyPtDD' + polyTdNum + '" class="d-flex" style="gap: 4px;"><div><div class="input-group mb-1">';
            StrDD += '<span class="input-group-text">緯度</span>';
            StrDD += '<input type="text" id="txtPolyLatDD' + polyTdNum + '" class="form-control" autocomplete="off" />';
            StrDD += '</div>';

            StrDD += '<div class="input-group"><span class="input-group-text">經度</span>';
            StrDD += '<input type="text" id="txtPolyLonDD' + polyTdNum + '" class="form-control" autocomplete="off" />';
            StrDD += '</div></div>';
            StrDD += '<button class="btn icon btn-sm btn-danger" type="button" onclick="deletePtByID(' + "'divPolyPtDD" + polyTdNum + "'" + ');"><i class="fa-solid fa-trash-can"></i></button></div>';
            $("#divPolyDD").append(StrDD);
        }

        for (i = 0; i < Coord3857[0].length - 1; i++) {
            var Coord4326 = ol.proj.transform(Coord3857[0][i], "EPSG:3857", "EPSG:4326");
            var CoordHDMS = ol.coordinate.toStringHDMS(Coord4326, 5);
            var EachHDMS = CoordHDMS.split(' ');
            var idx = i + 1;
            $("#ddlPolyLon" + idx).val(EachHDMS[7]);
            $("#ddlPolyLat" + idx).val(EachHDMS[3]);
            $("#txtPolyLon" + idx).val(EachHDMS[4].replace("°", " ") + EachHDMS[5].replace("′", " ") + EachHDMS[6].replace("″", ""));
            $("#txtPolyLat" + idx).val(EachHDMS[0].replace("°", " ") + EachHDMS[1].replace("′", " ") + EachHDMS[2].replace("″", ""));
            $("#txtPolyLonDD" + idx).val(Coord4326[0]);
            $("#txtPolyLatDD" + idx).val(Coord4326[1]);
        }
    } else {
        alert('多邊形至少需要三個點!');
        return false;
    }
}

//帶入矩形坐標
function setBoxCoord(Coord3857) {
    var Coord4326_1 = ol.proj.transform(Coord3857[0][0], "EPSG:3857", "EPSG:4326");
    var CoordHDMS_1 = ol.coordinate.toStringHDMS(Coord4326_1, 5);
    var EachHDMS_1 = CoordHDMS_1.split(' ');
    $("#ddlBoxLon1").val(EachHDMS_1[7]);
    $("#ddlBoxLat1").val(EachHDMS_1[3]);
    $("#txtBoxLon1").val(EachHDMS_1[4].replace("°", "") + " " + EachHDMS_1[5].replace("′", "") + " " + EachHDMS_1[6].replace("″", ""));
    $("#txtBoxLat1").val(EachHDMS_1[0].replace("°", "") + " " + EachHDMS_1[1].replace("′", "") + " " + EachHDMS_1[2].replace("″", ""));
    $("#txtBoxLonDD1").val(Coord4326_1[0]);
    $("#txtBoxLatDD1").val(Coord4326_1[1]);

    //最後一個點
    var Coord4326_2 = ol.proj.transform(Coord3857[0][2], "EPSG:3857", "EPSG:4326");
    var CoordHDMS_2 = ol.coordinate.toStringHDMS(Coord4326_2, 5);
    var EachHDMS_2 = CoordHDMS_2.split(' ');
    $("#ddlBoxLon2").val(EachHDMS_2[7]);
    $("#ddlBoxLat2").val(EachHDMS_2[3]);
    $("#txtBoxLon2").val(EachHDMS_2[4].replace("°", "") + " " + EachHDMS_2[5].replace("′", "") + " " + EachHDMS_2[6].replace("″", ""));
    $("#txtBoxLat2").val(EachHDMS_2[0].replace("°", "") + " " + EachHDMS_2[1].replace("′", "") + " " + EachHDMS_2[2].replace("″", ""));
    $("#txtBoxLonDD2").val(Coord4326_2[0]);
    $("#txtBoxLatDD2").val(Coord4326_2[1]);
}

//帶入圓形坐標
function setCircleCoord(Extent, olFeature) {
    var Geom = olFeature.getGeometry();
    var GeomType = Geom.getType();
    var Center3857 = ol.extent.getCenter(Extent);
    var CirLastPt = Geom.getLastCoordinate();
    var LineCoord = [Center3857, CirLastPt];
    var c1 = ol.proj.transform(Center3857, "EPSG:3857", 'EPSG:4326');
    var c2 = ol.proj.transform(CirLastPt, "EPSG:3857", 'EPSG:4326');
    if (GeomType == "MultiLineString") {
        var OriLon = olFeature.get("Lon");
        var OriLat = olFeature.get("Lat");
        c1 = [OriLon, OriLat];
    }
    var CoordHDMS = ol.coordinate.toStringHDMS(c1, 5);
    var EachHDMS = CoordHDMS.split(' ');

    $("#ddlCirLon").val(EachHDMS[7]);
    $("#ddlCirLat").val(EachHDMS[3]);
    $("#txtCirLon").val(EachHDMS[4].replace("°", "") + " " + EachHDMS[5].replace("′", "") + " " + EachHDMS[6].replace("″", ""));
    $("#txtCirLat").val(EachHDMS[0].replace("°", "") + " " + EachHDMS[1].replace("′", "") + " " + EachHDMS[2].replace("″", ""));
    $("#txtCirLonDD").val(c1[0]);
    $("#txtCirLatDD").val(c1[1]);
    var Radius = olFeature.get('Radius'); //均為公尺
    var CirUnit = olFeature.get('CirUnit');
    $("#ddlCirUnit").val(CirUnit);
    if (CirUnit == "km") {
        Radius = Radius / 1000;
    } else if (CirUnit == "nm") {
        Radius = Radius / 1852;
    }
    $("#txtCirRadius").val(Radius);
}

//開啟自訂線段坐標
function setLineScope() {
    $("#tbLinePt").show();
    $("#btnAddLinePt").show();
    $("#btnCreateLine").show();
}
//開啟自訂多邊形坐標
function setPolyScope() {
    $("#tbScope").show();
    $("#btnAddPt").show();
    $("#btnCreatePoly").show();
}
//開啟自訂矩形坐標
function setBoxScope() {
    $("#tbBoxScope").show();
    $("#btnCreateBox").show();
}

function setRdoBtnChecked(elem, IsCheck) {
    $(elem).prop("checked", IsCheck);
}

// #endregion


// #region

//計算線圖徵-線段總長度
function AddLineTotalLen(olF, FPK, ddlID) {
    var offsetY = 0;
    var LenUnit = $("#" + ddlID).val();
    var Geom = olF.getGeometry();
    var Length = calcLength(Geom);
    var LenTxt = (Length / 1000).toFixed(3) + ' ' + 'km';
    if (LenUnit == "m") {
        LenTxt = (LenTxt.split(" ")[0] * 1000).toFixed(3) + ' ' + 'm';
    } else if (LenUnit == "nm") {
        LenTxt = (LenTxt.split(" ")[0] * 0.5399568034557235).toFixed(3) + ' ' + 'nm';
    }
    var PK = CreateGuid();
    var Extent = Geom.getExtent();
    var Center = getCenterOfExtent(Extent);
    var LenPt = new ol.Feature(new ol.geom.Point(Center));
    drawSource.addFeature(LenPt);
    var Style = oltmx.Plugin.prototype.setTextMarkStyle(LenTxt, 'rgba(255,255,255,1)', 5, 'rgb(50, 168, 82)', '10', LenPt, offsetY);
    LenPt.setStyle(Style);
    LenPt.set('type', 'drawfeature');
    LenPt.set('FPK', FPK);
    LenPt.set('IsTotal', true);
    LenPt.setId(PK);
    LenPt.set('num', 'text' + txtNum);
    drawCollection.push(LenPt);
    if (!(document.getElementById("MarkList").children['Text' + txtNum])) {
        genMarkList(PK, 'Text', txtNum, LenTxt, "text");
        txtNum++;
    }
}

//計算圖徵-各線段總長度
function AddLineSegLen(olF, FPK, UnitID) {
    var LenUnit = $("#" + UnitID).val();
    var coordinates = olF.getGeometry().getCoordinates();
    coordinates.length == 1 ? coordinates = coordinates[0] : coordinates;
    for (i = 0; i < coordinates.length - 1; i++) {
        var PK = CreateGuid();
        var firstPt = coordinates[i];
        var SecPt = coordinates[i + 1];
        var Geom = new ol.geom.LineString([firstPt, SecPt]);
        var Length = calcLength(Geom);
        var LenTxt = (Length / 1000).toFixed(3) + ' ' + 'km';
        var Extent = Geom.getExtent();
        var Center = getCenterOfExtent(Extent);
        var LenPt = new ol.Feature(new ol.geom.Point(Center));
        LenPt.set("LenKmTxt", LenTxt);

        if (LenUnit == "m") {
            LenTxt = (LenTxt.split(" ")[0] * 1000).toFixed(3) + ' ' + 'm';
        } else if (LenUnit == "nm") {
            LenTxt = (LenTxt.split(" ")[0] * 0.5399568034557235).toFixed(3) + ' ' + 'nm';
        }
        drawSource.addFeature(LenPt);
        var Style = oltmx.Plugin.prototype.setTextMarkStyle(LenTxt, 'rgba(255,255,255,1)', 5, 'rgb(0, 0, 255)', '10', LenPt, 0);
        LenPt.setStyle(Style);
        LenPt.set('type', 'drawfeature');
        LenPt.set('FPK', FPK);
        LenPt.set('IsSeg', true);
        LenPt.setId(PK);
        LenPt.set('num', 'text' + txtNum);
        drawCollection.push(LenPt);
        if (!(document.getElementById("MarkList").children['Text' + txtNum])) {
            genMarkList(PK, 'Text', txtNum, LenTxt, "text");
            txtNum++;
        }
    }

}

//重新設定線段長度文字
function ResetSegLenTxt(LineFeature, PK, UnitID) {
    var LenUnit = $("#" + UnitID).val();
    var features = drawSource.getFeatures();
    features.forEach(function (f) {
        if (f.get('FPK') == PK && f.get('IsSeg')) {
            var LenTxt = f.get("LenKmTxt");
            if (LenUnit == "m") {
                LenTxt = (LenTxt.split(" ")[0] * 1000).toFixed(3) + ' ' + 'm';
            } else if (LenUnit == "nm") {
                LenTxt = (LenTxt.split(" ")[0] * 0.5399568034557235).toFixed(3) + ' ' + 'nm';
            }
            var FeatureNum = f.get("num");
            f.getStyle().getText().setText(LenTxt);
            $("#Label" + FeatureNum).html(LenTxt);
        }
    });
}

//計算圖徵-面積
function AddArea(olFeature, FPK, Type) {
    var Unit = $("#ddl" + Type + "Unit").val();
    var offsetY = -25;
    var PK = CreateGuid();
    var Geom = olFeature.getGeometry();
    var Area = calculateArea(Geom);
    var AreaTxt = (Area / 1000000).toFixed(3) + ' ' + 'km' + superScript('<sup>2</sup>');
    if (Unit == "m") {
        AreaTxt = Area.toFixed(3) + ' ' + 'm' + superScript('<sup>2</sup>');
    } else if (Unit == "nm") {
        AreaTxt = (AreaTxt.split(" ")[0] * 0.2915533495981229).toFixed(3) + ' ' + 'nm' + superScript('<sup>2</sup>');
    }
    var Ext = Geom.getExtent();
    var Center = ol.extent.getCenter(Ext);
    var AreaPt = new ol.Feature(new ol.geom.Point(Center));
    drawSource.addFeature(AreaPt);
    var Style = oltmx.Plugin.prototype.setTextMarkStyle(AreaTxt, 'rgba(255,255,255,1)', 5, 'rgb(50, 168, 82)', '10', AreaPt, offsetY);
    AreaPt.setStyle(Style);
    AreaPt.set('type', 'drawfeature');
    AreaPt.set('FPK', FPK);
    AreaPt.set('IsArea', true);
    AreaPt.setId(PK);
    AreaPt.set('num', 'text' + txtNum);
    drawCollection.push(AreaPt);
    if (!(document.getElementById("MarkList").children['Text' + txtNum])) {
        genMarkList(PK, 'Text', txtNum, AreaTxt, "text", "../App_Themes/map/images/font.png", "20", "20");
        txtNum++;
    }
}

//重新設定面積標註文字
function ResetAreaTxt(olFeature, FPK, Type) {
    var Unit = $("#ddl" + Type + "Unit").val();
    var Geom = olFeature.getGeometry();
    var Area = calculateArea(Geom);
    var AreaTxt = (Area / 1000000).toFixed(3) + ' ' + 'km' + superScript('<sup>2</sup>');
    if (Unit == "m") {
        AreaTxt = Area.toFixed(3) + ' ' + 'm' + superScript('<sup>2</sup>');
    } else if (Unit == "nm") {
        AreaTxt = (AreaTxt.split(" ")[0] * 0.2915533495981229).toFixed(3) + ' ' + 'nm' + superScript('<sup>2</sup>');
    }

    var features = drawSource.getFeatures();
    features.forEach(function (f) {
        if (f.get('FPK') == FPK && f.get('IsArea')) {
            var FeatureNum = f.get("num");
            f.getStyle().getText().setText(AreaTxt);
            $("#Label" + FeatureNum).html(AreaTxt);
        }
    });
}

//新增半徑標註
function AddRadius(olFeature, FPK) {
    var Unit = $("#ddlCirUnit").val();
    var PK = CreateGuid();
    var Geom = olFeature.getGeometry();
    var Ext = Geom.getExtent();
    var Center3857 = ol.extent.getCenter(Ext);
    var CirLastPt = Geom.getLastCoordinate();
    var LineCoord = [Center3857, CirLastPt];
    var c1 = ol.proj.transform(Center3857, "EPSG:3857", 'EPSG:4326');
    var c2 = ol.proj.transform(CirLastPt, "EPSG:3857", 'EPSG:4326');
    var Radius = wgs84Sphere.haversineDistance(c1, c2) / 1000;
    if (Unit == "m") {
        Radius = (Radius * 1000).toFixed(2) + ' ' + 'm';
    } else if (Unit == "nm") {
        Radius = (Radius * 0.5399568034557235).toFixed(3) + ' ' + 'nm';
    } else {
        Radius = Radius.toFixed(2) + ' ' + 'km';
    }
    var RadiusFeature = new ol.Feature(new ol.geom.LineString(LineCoord));
    RadiusFeature.setId(PK);
    RadiusFeature.set('FPK', FPK);
    RadiusFeature.set('type', 'drawfeature');
    RadiusFeature.set('IsRadius', true);
    RadiusFeature.setStyle(markStyle(Radius, 13));
    drawSource.addFeature(RadiusFeature);
    drawCollection.push(RadiusFeature);
}

//重新設定半徑標註
function ResetRadius(olFeature, FPK) {
    deleteRadiusMark(FPK);
    AddRadius(olFeature, FPK);
}

//新增經緯度標註
function AddCoordMark(olFeature, FPK) {
    var CoordFeature = null;
    var CoordTxt = "";
    var PK = CreateGuid();
    var olGeom = olFeature.getGeometry();
    var olType = olFeature.get("num");
    if (olType.indexOf("fan") > -1) {
        var Coord3857 = olGeom.getCoordinates()[0][0];
        var Coord4326 = ol.proj.transform(Coord3857, "EPSG:3857", "EPSG:4326");
        CoordTxt = Coord4326[0].toFixed(3) + ", " + Coord4326[1].toFixed(3);
        CoordFeature = new ol.Feature(new ol.geom.Point(Coord3857));
    } else {
        var Coord3857 = ol.extent.getCenter(olGeom.getExtent());
        var Coord4326 = ol.proj.transform(Coord3857, "EPSG:3857", "EPSG:4326");
        CoordTxt = Coord4326[1].toFixed(3) + ", " + Coord4326[0].toFixed(3);
        CoordFeature = new ol.Feature(new ol.geom.Point(Coord3857));
    }
    CoordFeature.setId(PK);
    CoordFeature.set('FPK', FPK);
    CoordFeature.set('type', 'drawfeature');
    CoordFeature.set('IsCoordMark', true);
    CoordFeature.setStyle(markStyle(CoordTxt, 25));
    drawSource.addFeature(CoordFeature);
    drawCollection.push(CoordFeature);
}

//重新設定經緯度標註
function ResetCoordMark(olFeature, FPK) {
    deleteCoordMark(FPK);
    AddCoordMark(olFeature, FPK);
}

//設定依大地坐標繪製
function QuyGeodesyCircle(Lon, Lat, radius, olFeature) {
    var d = new FormData();
    d.append('Lon', Lon);
    d.append('Lat', Lat);
    d.append('Radius', radius);

    $.ajax(
        {
            url: "WebService/QuyMarker.asmx/GetGeodesyCircle",
            type: "POST",
            data: d,
            cache: false,
            contentType: false,
            processData: false,
            success: function (result) {
                var json = $.parseJSON(result);
                if (json.length > 0) {
                    var Coords3857 = [];
                    var wktForm = new ol.format.WKT();
                    var WKT = json[0].GeodesyCircle;
                    var olGeom = wktForm.readGeometry(WKT);
                    var Coords = olGeom.getCoordinates()[0];
                    var SecIdx = 0;
                    for (i = 0; i < Coords.length; i++) {
                        if (i != Coords.length - 1) {
                            var curCoord = Coords[i];
                            var nextCoord = Coords[i + 1];
                            if (180 - Math.abs(curCoord[0]) <= 10 && 180 - Math.abs(nextCoord[0]) <= 10) {
                                if ((curCoord[0] > 0 && nextCoord[0] < 0) || (curCoord[0] < 0 && nextCoord[0] > 0)) {
                                    SecIdx = i + 1;
                                    break;
                                }
                            }
                        }
                    }

                    /*for (i = 0; i < Coords.length; i++) {
                        var lon = Coords[i][0];
                        var lat = Coords[i][1];
                        if (i == SecIdx) {
                            lon = Coords[i][0] < 0 ? Coords[i][0] + 360 : Coords[i][0];
                            lat = Coords[i][1] < 0 ? Coords[i][1] + 85 : Coords[i][1];
                        }
                        var Coord3857 = ol.proj.transform([lon, lat], "EPSG:4326", "EPSG:3857");
                        Coords3857.push(Coord3857);
                    }*/

                    var Coords3857_1 = [];
                    for (i = 0; i < SecIdx; i++) {
                        var lon = Coords[i][0];// < 0 ? Coords[i][0] + 360 : Coords[i][0];
                        var lat = Coords[i][1];// < 0 ? Coords[i][1] + 85 : Coords[i][1];
                        var Coord3857 = ol.proj.transform([lon, lat], "EPSG:4326", "EPSG:3857");
                        Coords3857_1.push(Coord3857);
                    }
                    //Coords3857_1.push(Coords3857_1[0]);

                    var Coords3857_2 = [];
                    for (i = SecIdx; i < Coords.length; i++) {
                        var lon = Coords[i][0];// < 0 ? Coords[i][0] + 360 : Coords[i][0];
                        var lat = Coords[i][1];// < 0 ? Coords[i][1] + 85 : Coords[i][1];
                        var Coord3857 = ol.proj.transform([lon, lat], "EPSG:4326", "EPSG:3857");
                        Coords3857_2.push(Coord3857);
                    }
                    //Coords3857_2.push(Coords3857_2[0]);

                    for (i = 0; i < Coords.length; i++) {
                        var lon = Coords[i][0] < 0 ? Coords[i][0] + 360 : Coords[i][0];
                        var lat = Coords[i][1] < 0 ? Coords[i][1] + 85 : Coords[i][1];
                        var Coord3857 = ol.proj.transform([lon, lat], "EPSG:4326", "EPSG:3857");
                        Coords3857.push(Coord3857);
                    }
                    //var olGeom3857 = new ol.geom.Polygon([Coords3857]); //olGeom.clone().transform('EPSG:4326', 'EPSG:3857');
                    //var olGeom3857 = olGeom.clone().transform('EPSG:4326', 'EPSG:3857');
                    //var olGeom3857 = new ol.geom.MultiPolygon([[Coords3857_2, Coords3857_1.reverse()]]);
                    var olGeom3857 = new ol.geom.MultiLineString([Coords3857_1, Coords3857_2]);
                    olFeature.setGeometry(olGeom3857);
                }
            }
        });
}

//重新設定長度文字
function ResetTolLenTxt(LineFeature, PK, ddlID) {
    var LenUnit = $("#" + ddlID).val();
    var Geom = LineFeature.getGeometry();
    var Length = parent.calcLength(Geom);
    var LenTxt = (Length / 1000).toFixed(0) + ' ' + 'km';
    if (LenUnit == "m") {
        LenTxt = (LenTxt.split(" ")[0] * 1000).toFixed(0) + ' ' + 'm';
    } else if (LenUnit == "nm") {
        LenTxt = (LenTxt.split(" ")[0] * 0.5399568034557235).toFixed(0) + ' ' + 'nm';
    }

    var features = parent.drawSource.getFeatures();
    features.forEach(function (f) {
        if (f.get('FPK') == PK && f.get('IsTotal')) {
            var FeatureNum = f.get("num");
            f.getStyle().getText().setText(LenTxt);
            parent.$("#Label" + FeatureNum).html(LenTxt);
        }
    });
}

// #endregion

// #region


//刪除標註&清單
function deleteMark(divId, markNum, PK) {
    var ConfirmYes = confirm('是否要刪除圖徵？');
    if (ConfirmYes) {
        var olFeature = drawSource.getFeatureById(PK);
        drawSource.removeFeature(olFeature);
        drawCollection.remove(olFeature);

        if (markNum.indexOf("azimuth") > -1) {
            var NorthLineID = markNum.replace("azimuth", "NorthLine");
            var NorthLine = drawSource.getFeatureById(NorthLineID);
            drawSource.removeFeature(NorthLine);
            drawCollection.remove(NorthLine);
        }

        //移除半徑圖徵
        var RadiusID = PK + "Radius"
        var olRF = drawSource.getFeatureById(RadiusID);
        if (olRF != null) {
            drawSource.removeFeature(olRF);
        }

        $(divId).remove();

        if (olFeature.get('IsSegNum') || olFeature.get('HasSeg')) {
            deleteSegMark(PK);
        }

        if (olFeature.get('IsTotalNum')) {
            deleteTolMark(PK);
        }

        if (olFeature.get('HasArea')) {
            deleteAreaMark(PK);
        }

        if (olFeature.get('HasRadius')) {
            deleteRadiusMark(PK);
        }

        if (olFeature.get('HasAngle')) {
            deleAngleMark(PK);
        }

        if (olFeature.get('IsCen')) {
            deleteCenMark(PK);
        }

        if (olFeature.get('IsCoord')) {
            deleteCoordMark(PK);
        }
    }
}

//刪除總長度標註
function deleteTolMark(PK) {
    var features = drawSource.getFeatures();
    features.forEach(function (f) {
        if (f.get('FPK') == PK && f.get('IsTotal')) {
            var TolPK = f.getId();
            drawSource.removeFeature(f);
            drawCollection.remove(f);
            var listID = f.get('num');
            if (listID.indexOf('text') > -1) {
                listID = listID.replace('text', 'Text');
                deleteListByID(listID);
            }
        }
    });
    //確認標註清單子項目是否還存在
    CheckSubItem(PK);
}

//刪除線段長度標註
function deleteSegMark(PK) {
    var features = drawSource.getFeatures();
    features.forEach(function (f) {
        if (f.get('FPK') == PK && f.get('IsSeg')) {
            var SegPK = f.getId();
            drawSource.removeFeature(f);
            drawCollection.remove(f);
            var listID = f.get('num');
            if (listID.indexOf('text') > -1) {
                listID = listID.replace('text', 'Text');
                deleteListByID(listID);
            }
        }
    });
    //確認標註清單子項目是否還存在
    CheckSubItem(PK);
}

//刪除面積標註
function deleteAreaMark(PK) {
    var features = drawSource.getFeatures();
    features.forEach(function (f) {
        if (f.get('FPK') == PK && f.get('IsArea')) {
            var AreaPK = f.getId();
            drawSource.removeFeature(f);
            drawCollection.remove(f);
            var listID = f.get('num');
            if (listID.indexOf('text') > -1) {
                listID = listID.replace('text', 'Text');
                deleteListByID(listID);
            }
        }
    });
    //確認標註清單子項目是否還存在
    CheckSubItem(PK);
}

//刪除半徑標註
function deleteRadiusMark(PK) {
    var features = drawSource.getFeatures();
    features.forEach(function (f) {
        if (f.get('FPK') == PK && f.get('IsRadius')) {
            drawSource.removeFeature(f);
            drawCollection.remove(f);
        }
    });
    //確認標註清單子項目是否還存在
    CheckSubItem(PK);
}

//刪除經緯度標註
function deleteCoordMark(PK) {
    var features = drawSource.getFeatures();
    features.forEach(function (f) {
        if (f.get('FPK') == PK && f.get('IsCoordMark')) {
            drawSource.removeFeature(f);
            drawCollection.remove(f);
        }
    });
    //確認標註清單子項目是否還存在
    CheckSubItem(PK);
}

//刪除中心點標註
function deleteCenMark(PK) {
    var features = drawSource.getFeatures();
    features.forEach(function (f) {
        if (f.get('FPK') == PK && f.get('IsCenMark')) {
            drawSource.removeFeature(f);
            drawCollection.remove(f);
        }
    });
    //確認標註清單子項目是否還存在
    CheckSubItem(PK);
}

//確認標註清單子項目是否還存在
function CheckSubItem(PK) {
    var SubItemLen = $("#P" + PK).parent().find(".sub .item").length;
    if (SubItemLen == 0) {
        $("#P" + PK).removeClass("active");
    }
}

//標註匯出GeoJson
function exportGeoJson(PK) {
    var olFeature = drawSource.getFeatureById(PK);
    var gjFormat = new ol.format.GeoJSON();
    if (olFeature != null) {
        const json = gjFormat.writeFeature(olFeature, {
            dataProjection: 'EPSG:4326',
            featureProjection: 'EPSG:3857'
        });
        downloadMark(json, 'GeoJson.json', 'application/json');
    }
}

//下載檔案
function downloadMark(content, fileName, contentType) {
    var a = document.createElement("a");
    var file = new Blob([content], { type: contentType });
    a.href = URL.createObjectURL(file);
    a.download = fileName;
    a.click();
}

// #endregion