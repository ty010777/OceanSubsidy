gs.Core.package("gsmap.func");

gsmap.func.Export = function (_plugin, targetDom) {
    var plugin = _plugin;
    var $baseDom = $(targetDom);
    var $mainDom;
    var bShow = false;
    var me = this;

    var overviewMapControl;
    var scaleMM = 30; // mm

    window.gsExportInfo = {};
    window.gsExportInfo.scalePixel = null; // scaleMM 轉成 pixel後的數值
    window.gsExportInfo.scaleReal = null; // scale表示的實際長度
    window.gsExportInfo.scaleLength = null; // scale畫上去的pixel數

    // 讀取template
    $.ajax("map/template_export.html?" + Date.now()).done(
        function (data) {
            $mainDom = $(data);
            $baseDom.append($mainDom);

            $mainDom.find("input[name='paperDirection']").on('click', showPrintExtent)

            $mainDom.find("input[name='capSize']").on('click', showCaptureExtent)

            $mainDom.find("._custSize").on('keyup', showCaptureExtent);

            $mainDom.find("#export-tabs.nav-tabs .nav-link").on(
                'show.bs.tab',
                switchLayerTab
            );

            $mainDom.find(".btn_print")
                .on('click', printIt);

            $mainDom.find(".btn_next")
                .on('click', captureMapElement);

            initRWD();

            overviewMapControl = plugin.getOverviewMap();

            if (bShow) {
                me.show();
            } else {
                me.hide();
            }

            $mainDom.find(".closebtn").on("click", function () { me.hide(); });

        }
    );

    this.show = function () {
        page_active();
        bShow = true;
        if ($mainDom) {
            $mainDom.width("");
        }
    };

    this.hide = function () {
        page_leave();
        bShow = false;
        if ($mainDom)
            $mainDom.width(0);
    }

    function page_active() {
        if (!$mainDom) return;

        if ($mainDom.find("#export-tabs.nav-tabs .nav-link.active").attr("export-type") === 'print') {
            console.log('print');
            showPrintExtent();
        } else {
            console.log('save');
            showCaptureExtent();
        }
        resetRWD();
    }

    function page_leave() {
        hideCaptureExtent();
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

    function switchLayerTab(evt) {
        if ($(evt.target).attr("export-type") === 'save') {
            showCaptureExtent();
        } else {
            showPrintExtent();
        }
    }

    function printIt() {
        createRotatedImage(document.getElementById("exportNortharrow"));
        var srcElement = event.srcElement;
        disableButton(srcElement, "產製列印畫面中....");
        var extentDom = $("#tableCaptureExtent div");
        var capSize = [extentDom.width(), extentDom.height()];
        if (window.gsExportInfo.scalePixel) {
            window.gsExportInfo.scaleReal = plugin.getMap().getView().getResolution() * window.gsExportInfo.scalePixel;
            var unit = "公尺";
            if (window.gsExportInfo.scaleReal > 1000) {
                window.gsExportInfo.scaleReal /= 1000;
                unit = "公里";
            }
            var ori = window.gsExportInfo.scaleReal;
            var digit = 0;
            while (window.gsExportInfo.scaleReal > 10) {
                digit++;
                window.gsExportInfo.scaleReal /= 10;
            }
            window.gsExportInfo.scaleReal = Math.round(window.gsExportInfo.scaleReal);
            for (var i = 0; i < digit; i++) {
                window.gsExportInfo.scaleReal *= 10;
            }
            window.gsExportInfo.scaleLength = Math.round(window.gsExportInfo.scalePixel * window.gsExportInfo.scaleReal / ori);
            window.gsExportInfo.scaleReal = window.gsExportInfo.scaleReal + unit;
        }
        var canvases = $(plugin.getMap().getViewport()).find("canvas")[0];
        canvas = document.createElement('canvas');
        canvas.setAttribute('width', capSize[0]);
        canvas.setAttribute('height', capSize[1]);
        canvas.setAttribute('id', 'printCanvas');
        canvases.appendChild(canvas);

        var ctx = canvas.getContext("2d");
        var mapSize = plugin.getMap().getSize();
        var startPixel = [(mapSize[0] - capSize[0]) / 2, (mapSize[1] - capSize[1]) / 2];
        ctx.drawImage(canvases, -startPixel[0], -startPixel[1]);
        result = canvas.toDataURL();
        showPrintLayout(result);
        enableButton(srcElement, "列印");

    }

    function showPrintLayout(src) {
        window.gsExportInfo.printImgURL = src;
        window.gsExportInfo.printTitle = $mainDom.find("#txtPrintTitle").val();
        setTimeout(function () {
            var printDoc = window.open('map/printLayout.htm', '_blank');
        }, 1);
        //printDoc.location = "printLayout.htm";
    }

    function saveIt() {
        var srcElement = event.srcElement;
        disableButton(srcElement, "圖片產製中...");
        var extentDom = $("#tableCaptureExtent div");
        var capSize = [extentDom.width(), extentDom.height()];
        if (!capSize) alert('您輸入的長寬不正確');
        var canvases = $(plugin.getMap().getViewport()).find("canvas")[0];
        canvas = document.createElement('canvas');
        canvas.setAttribute('width', capSize[0]);
        canvas.setAttribute('height', capSize[1]);
        canvas.setAttribute('id', 'photoCanvas');
        canvases.appendChild(canvas);

        var ctx = canvas.getContext("2d");
        var mapSize = plugin.getMap().getSize();
        var format = $mainDom.find("#ddlImageFormat").val();
        var startPixel = [(mapSize[0] - capSize[0]) / 2, (mapSize[1] - capSize[1]) / 2];
        ctx.drawImage(canvases, -startPixel[0], -startPixel[1]);
        result = canvas.toDataURL();
        canvas.toBlob(function (result) {
            saveAs(result, 'map.' + format);
        });
        enableButton(srcElement, "儲存");
        /*plugin.captureMapCenter(capSize, $("#ddlImageFormat").val(), function (r) {
            if (r.imageURL || r.imageURL == "") {
                window.open("service/GetMapCaptureImg.ashx?imgUrl=" + encodeURIComponent(r.imageURL));
            } else {
                alert("截圖失敗");
            }
            enableButton(srcElement, "儲存");
        });*/

    }

    function disableButton(but, newText) {
        var $it = but;
        if (!$.prototype.isPrototypeOf($it)) {
            $it = $(but);
        }
        $it.prop("disabled", true);
        $it.css("color", "gray");
        $it.attr("value", newText);
    }

    function enableButton(but, newText) {
        var $it = but;
        if (!$.prototype.isPrototypeOf($it)) {
            $it = $(but);
        }
        $it.prop("disabled", false);
        $it.css("color", "");
        $it.attr("value", newText);
    }

    function showCaptureExtent() {
        var capSize = getImageSize();
        if (!capSize) return;
        $("#tableCaptureExtent div").width(capSize[0]).height(capSize[1]);
        $("#tableCaptureExtent").show();
    }

    function showPrintExtent() {
        var $divExtent = $("#tableCaptureExtent div");
        var mmSize = [];
        if ($mainDom.find("input[name='paperDirection']:checked").val() == 'vertical') {
            mmSize = [210 - 38, 297 - 38];
        } else {
            mmSize = [297 - 38, 210 - 38];
        }
        $divExtent.width(mmSize[0] + "mm").height(mmSize[1] + "mm");
        $("#tableCaptureExtent").show();
        if (!window.gsExportInfo.scalePixel && mmSize[0] != 0) {
            window.gsExportInfo.scalePixel = Math.round($divExtent.width() / mmSize[0] * scaleMM);
        }
        var mapSize = plugin.getMap().getSize();
        var capSize = [$divExtent.outerWidth(), $divExtent.outerHeight()];
        if (capSize[0] > mapSize[0]) capSize[0] = mapSize[0];
        if (capSize[1] > mapSize[1]) capSize[1] = mapSize[1];
        $divExtent.outerWidth(capSize[0]);
        $divExtent.outerHeight(capSize[1]);
    }

    function getImageSize() {
        var $checked = $mainDom.find("input[name='capSize']:checked");
        if ($checked.length == 0) return;
        var capSize = $checked.val();
        var capWidth, capHeight;
        if (capSize === "custom") {
            try {
                capWidth = parseInt($mainDom.find("#txtCaptureWidth").val(), 10);
                capHeight = parseInt($mainDom.find("#txtCaptureHeight").val(), 10);
            } catch (e) {
                return null;
            }
        } else {
            capSize = capSize.split(",")
            capWidth = parseInt(capSize[0], 10);
            capHeight = parseInt(capSize[1], 10);
        }
        return [capWidth, capHeight];
    }

    function hideCaptureExtent() {
        $("#tableCaptureExtent").hide();
    }

    //產製地圖元素及圖台截圖圖片
    function captureMapElement() {
        //overviewMapControl.getOverviewMap().getView().setZoom(overviewMapControl.getMap().getView().getZoom() - 2);

        setTimeout(function () {
            //$(".gsmap-scale-line").css("background-color", "#FFF");
            html2canvas(
                $(".gsmap-scale-line")[0],
                {
                    onrendered: function (canvas) {
                        //$(".gsmap-scale-line").css("background-color", "transparent");
                        window.gsExportInfo.ScaleLineImgDataUrl = canvas.toDataURL();
                    }
                }
            );

            html2canvas($(".ol-overviewmap")[0], {
                onrendered: function (canvas) {
                    window.gsExportInfo.OverviewImgDataUrl = canvas.toDataURL();

                }
            });

            if ($("#labelLegend").children().length) {
                html2canvas($("#labelLegend")[0], {
                    // 顯示跨網域圖片
                    allowTaint: false,
                    useCORS: true,
                    onrendered: function (canvas) {
                        window.gsExportInfo.LegendImgDataUrl = canvas.toDataURL();
                    }
                });
            }

            createRotatedImage(document.getElementById("exportNortharrow"));

            var MapImg = document.createElement('img');
            var extentDom = $("#tableCaptureExtent div");
            var capSize = [extentDom.width(), extentDom.height()];
            if (!capSize) alert('您輸入的長寬不正確');
            var canvases = $(plugin.getMap().getViewport()).find("canvas")[0];
            var canvas = document.createElement('canvas');
            canvas.setAttribute('width', capSize[0]);
            canvas.setAttribute('height', capSize[1]);
            canvas.setAttribute('id', 'photoCanvas');
            canvases.appendChild(canvas);

            var ctx = canvas.getContext("2d");
            var mapSize = plugin.getMap().getSize();
            var format = $mainDom.find("#ddlImageFormat").val();
            var startPixel = [Math.round((mapSize[0] - capSize[0]) / 2), Math.round((mapSize[1] - capSize[1]) / 2)];
            ctx.drawImage(canvases, -startPixel[0], -startPixel[1]);
            window.gsExportInfo.MapImgDataUrl = canvas.toDataURL();
            canvases.removeChild(canvas);
            showImageLayout(format);

            //MapImg.src = canvas.toDataURL();
            //var MapImage = MapImg.src.replace('data:image/png;base64,', '');
            //$.ajax({
            //    url: "WebService/mapElement2pic.asmx/SaveMapImage",
            //    type: "POST",
            //    dataType: "json",
            //    contentType: "application/json; charset=utf-8",
            //    cache: false,
            //    data: '{"MapImg": "' + MapImage + '"}',
            //    success: function (response) {
            //        try {
            //            showImageLayout(format);
            //        } catch (err) {
            //            alert("SaveMapImage error");
            //        }
            //    },
            //    error: function (x, e) {
            //        alert(x.responseText);
            //    }
            //});
        }, 500);
    }

    //產製指北針圖片
    function createRotatedImage(img) {
        var rotateAngle = plugin.getMap().getView().getRotation();
        var newCanvas = document.createElement('canvas');
        newCanvas.width = 60;
        newCanvas.height = 68;
        var newCtx = newCanvas.getContext('2d');
        newCtx.save();
        newCtx.translate(img.width / 1.5, img.height / 1.8);
        newCtx.rotate(rotateAngle);
        newCtx.drawImage(img, -img.width / 2, -img.height / 2);
        newCtx.restore();

        window.gsExportInfo.NorthArrowImgDataUrl = newCanvas.toDataURL();

        //var data = newCanvas.toDataURL('image/png');
        //var northArrow = document.createElement('img');
        //northArrow.src = data;
        //var northarrowImage = northArrow.src.replace('data:image/png;base64,', '');
        //
        //$.ajax({
        //    url: "WebService/mapElement2pic.asmx/SaveNortharrowImage",
        //    type: "POST",
        //    dataType: "json",
        //    contentType: "application/json; charset=utf-8",
        //    async: false,
        //    cache: false,
        //    data: '{"NortharrowImg": "' + northarrowImage + '"}',
        //    success: function (response) {
        //        try {
        //        } catch (err) {
        //            alert("SaveNortharrowImage error");
        //        }
        //    },
        //    error: function (x, e) {
        //        alert(x.responseText);
        //    }
        //});
    }

    var imageDoc
    function showImageLayout(format) {
        window.gsExportInfo.imageTitle = $mainDom.find("#txtImageTitle").val();
        setTimeout(function () {
            imageDoc = window.open("Map/SaveImgLayout.htm?format=" + format, '_blank');
        }, 1);
    }
}

