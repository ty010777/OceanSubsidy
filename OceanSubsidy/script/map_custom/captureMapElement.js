//刪除mapElement資料夾中檔案
function deleteLastFiles() {
    $.ajax({
        url: "../WebService/mapElement2pic.asmx/deleteLastFiles",
        type: "POST",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        async: false,
        cache: false,       
        success: function (response) {
            try {
            } catch (err) {
                alert("deleteLastFiles error");
            }
        },
        error: function (x, e) {
            alert(x.responseText);
        }
    });
}

//產製地圖元素及圖台截圖圖片
function captureMapElement() {
    parent.overviewMapControl.getOverviewMap().getView().setZoom(parent.overviewMapControl.getMap().getView().getZoom() - 2);

    setTimeout(function() {
        deleteLastFiles();
        parent.html2canvas(parent.$(".gsmap-scale-line")[0], {
            onrendered: function (canvas) {
                var sclaeLine = document.createElement('img');
                var ctx = canvas.getContext('2d');
                ctx.webkitImageSmoothingEnabled = false;
                ctx.mozImageSmoothingEnabled = false;
                ctx.imageSmoothingEnabled = false;
                ctx.msImageSmoothingEnabled = false;
                sclaeLine.src = toBitmapURL(canvas);//canvas.toDataURL();
                sclaeLine.width = 150;
                sclaeLine.height = 23;
                var ScaleLineImg = sclaeLine.src.replace('data:image/bmp;base64,', '');
                $.ajax({
                    url: "../WebService/mapElement2pic.asmx/SaveScaleLineImage",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    async: false,
                    cache: false,
                    data: '{"ScaleLineImg": "' + ScaleLineImg + '"}',
                    success: function (response) {
                        try {
                        } catch (err) {
                            alert("SaveScaleLineImage error");
                        }
                    },
                    error: function (x, e) {
                        alert(x.responseText);
                    }
                });
            }
        });

        parent.html2canvas(parent.$(".ol-overviewmap")[0], {
            onrendered: function (canvas) {
                var Overview = document.createElement('img');
                var ctx = canvas.getContext('2d');
                ctx.webkitImageSmoothingEnabled = false;
                ctx.mozImageSmoothingEnabled = false;
                ctx.imageSmoothingEnabled = false;
                ctx.msImageSmoothingEnabled = false;
                Overview.src = canvas.toDataURL();
                var OveviewImage = Overview.src.replace('data:image/png;base64,', '');
                $.ajax({
                    url: "../WebService/mapElement2pic.asmx/SaveOveviewImage",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    async: false,
                    cache: false,
                    data: '{"OveviewImage": "' + OveviewImage + '"}',
                    success: function (response) {
                        try {
                        } catch (err) {
                            alert("SaveOveviewImage error");
                        }
                    },
                    error: function (x, e) {
                        alert(x.responseText);
                    }
                });
            }
        });

        if (parent.$("#labelLegend")[0].innerText.replace(/\s/g, '') != "") {
            parent.html2canvas(parent.$("#labelLegend")[0], {
                onrendered: function (canvas) {
                    var lengendImg = document.createElement('img');
                    var ctx = canvas.getContext('2d');
                    ctx.webkitImageSmoothingEnabled = false;
                    ctx.mozImageSmoothingEnabled = false;
                    ctx.imageSmoothingEnabled = false;
                    ctx.msImageSmoothingEnabled = false;
                    lengendImg.src = canvas.toDataURL();
                    var LegendImage = lengendImg.src.replace('data:image/png;base64,', '');
                    $.ajax({
                        url: "../WebService/mapElement2pic.asmx/SaveLegendImage",
                        type: "POST",
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        async: false,
                        cache: false,
                        data: '{"LegendImg": "' + LegendImage + '"}',
                        success: function (response) {
                            try {
                            } catch (err) {
                                alert("SaveLegendImage error");
                            }
                        },
                        error: function (x, e) {
                            alert(x.responseText);
                        }
                    });
                }
            });
        }

        createRotatedImage(parent.document.getElementById("exportNortharrow"));

        var MapImg = document.createElement('img');
        var extentDom = parent.$("#tableCaptureExtent div");
        var capSize = [extentDom.width(), extentDom.height()];
        if (!capSize) alert('您輸入的長寬不正確');
        var canvases = $(parent.map.getViewport()).find("canvas")[0];
        var canvas = parent.document.createElement('canvas');
        canvas.setAttribute('width', capSize[0]);
        canvas.setAttribute('height', capSize[1]);
        canvas.setAttribute('id', 'photoCanvas');
        canvases.appendChild(canvas);

        var ctx = canvas.getContext("2d");
        var mapSize = parent.mapPlugin.getMap().getSize();
        var format = $("#ddlImageFormat").val();
        var startPixel = [(mapSize[0] - capSize[0]) / 2, (mapSize[1] - capSize[1]) / 2];
        ctx.drawImage(canvases, -startPixel[0], -startPixel[1]);
        MapImg.src = canvas.toDataURL();
        var MapImage = MapImg.src.replace('data:image/png;base64,', '');
        $.ajax({
            url: "../WebService/mapElement2pic.asmx/SaveMapImage",
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            cache: false,
            data: '{"MapImg": "' + MapImage + '"}',
            success: function (response) {
                try {
                    showImageLayout(format);
                } catch (err) {
                    alert("SaveMapImage error");
                }
            },
            error: function (x, e) {
                alert(x.responseText);
            }
        });   
    }, 500);
}

//產製指北針圖片
function createRotatedImage(img) {
    var rotateAngle = parent.map.getView().getRotation();
    var newCanvas = parent.document.createElement('canvas');
    newCanvas.width = 60;
    newCanvas.height = 68;
    var newCtx = newCanvas.getContext('2d');
    newCtx.save();
    newCtx.translate(img.width / 1.5, img.height / 1.8);
    newCtx.rotate(rotateAngle);
    newCtx.drawImage(img, -img.width / 2, -img.height / 2);
    newCtx.restore();

    var data = newCanvas.toDataURL('image/png');
    var northArrow = parent.document.createElement('img');
    northArrow.src = data;
    var northarrowImage = northArrow.src.replace('data:image/png;base64,', '');

    $.ajax({
        url: "../WebService/mapElement2pic.asmx/SaveNortharrowImage",
        type: "POST",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        async: false,
        cache: false,
        data: '{"NortharrowImg": "' + northarrowImage + '"}',
        success: function (response) {
            try {
            } catch (err) {
                alert("SaveNortharrowImage error");
            }
        },
        error: function (x, e) {
            alert(x.responseText);
        }
    });
}

function showImageLayout(format) {
    imageTitle = $("#txtImageTitle").val();
    setTimeout(function () {
        var imageDoc = window.open("../Map/SaveImgLayout.htm?format=" + format, '_blank');
    }, 1);
}