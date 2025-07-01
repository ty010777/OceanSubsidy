function toolSwitch(target) {
    mapUI.getBottomUI().setBottomHeightLevel(0);
    if(target == "fullMap") {
        mapPlugin.backToInitPosition();
        return;
    }

    $("#mapnav li").removeClass("active");

    // 呼叫iframe裡的page_leave事件
    $(".Box iframe").each(function () {
        if ($(this).css("display") != "none" && this.contentWindow.page_leave) {
            this.contentWindow.page_leave();
        }
    });
    $(".Box iframe").hide();
    if (!target) {
        $(".FuncBox").hide();
        return;
    }
    $(".FuncBox").show();
    switch (target) {
        case "measure":
            $("#ifBox02").show("fast");
            $(".nav_02").addClass("active");
            break;
        case "rangeQuery":
            $("#ifBox03").show("fast");
            $(".nav_03").addClass("active");
            break;
        case "locate":
            $("#ifBox04").show("fast");
            $(".nav_04").addClass("active");
            break;
        case "legend":
            $("#ifBox05").show("fast");
            $(".nav_05").addClass("active");
            //$("#ifBox05")[0].contentWindow.resizeLayout();
            break;
        case "imageCapture":
            $("#ifBox06").show("fast");
            $(".nav_06").addClass("active");
            break;
        case "marked":
            $("#ifBox08").show("fast");
            $(".nav_08").addClass("active");
            break;
        case "BaseMap":
            $("#ifBox01").show("fast");
            $(".nav_01").addClass("active");
            break;
        case "closeAll":
            $(".FuncBox").hide();
            $(".Box iframe").each(function () {
                if (this.contentWindow.action_clear) {
                    this.contentWindow.action_clear();
                }
            });
            break;
        case "backMis":
           
            top.showMis();
            break;
    }
    $(".Box iframe").each(function () {
        if ($(this).css("display") != "none" && this.contentWindow.page_active) {
            this.contentWindow.page_active();
        }
    });
}

function resizeBox(contentHeight) {
    var maxHieght = $("html").height() - $("#contentBox").offset().top - 0;
    for (var i = 1; i <= 7; i++) {
        if ($("#ifBox0" + i).css("display") != "none") {
            var height = contentHeight;
            if (height > maxHieght) {
                height = maxHieght;
            }
            if ($("#contentBox").height() != height) {
                $("#contentBox").height(height);
            }
            break;
        }
    }
}

function showHelp(htmlContent) {
    $('#helpContent').html(htmlContent);
    $('#helper').modal('show');
}

function showFeatureDetail(layerID, feature) {
    var serviceInfo = mapPlugin.layersMg.getLayerInfo(layerID).serviceInfo;
    if (serviceInfo.featureDetail) {
        var htmlContent = serviceInfo.featureDetail;
        htmlContent = mapPlugin.genContentFromFeature(feature, htmlContent);
        $('#featureDetailContent').html(htmlContent);
    } else {
        $('#featureDetailContent').html("");
    }
    $('#featureDetail').modal('show');
}

function showIdentifyFeatureDetail(layerID, FeatureId) {
    showFeatureDetail(layerID, mapPlugin.layersMg.getFeatureFromIdentify(layerID, FeatureId));
}
