gs.Core.package("gsmap.func");

gsmap.func.Measure = function (_plugin, targetDom) {
    var plugin = _plugin;
    var $baseDom = $(targetDom);
    var $mainDom;
    var bShow = false;
    var me = this;

    var helpHtmls = [
    "<p>• 於圖面上點擊量測範圍，並雙擊滑鼠左鍵結束。</p><p>• 可依需求選擇量測單位。</p>",
    "<p>• 於圖面上繪製量測線段/範圍，並雙擊滑鼠左鍵結束。</p><p>• 可依需求選擇量測單位。 </p>"];
    var helpHtml = "<p>• 於圖面上繪製量測線段/範圍，並雙擊滑鼠左鍵結束。</p>"
                 + "<p>• 可依需求選擇量測單位。 </p>";

    // 讀取template
    $.ajax("map/template_measure.html?" + Date.now()).done(
        function (data) {
            $mainDom = $(data);
            $baseDom.append($mainDom);
            if (bShow) {
                me.show();
            } else {
                me.hide();
            }

            $mainDom.find(".closebtn").on("click", function () { me.hide(); });
            $mainDom.find("._resetMeasure").on("click", resetMeasure);
            $mainDom.find("[data-bs-toggle='tab']").on('shown.bs.tab', resetMeasure);
            $mainDom.find("#lengthUnit").on('click', switchLengthUnit);
            $mainDom.find("#areaUnit").on('click', switchAreaUnit);
            
            initRWD();

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
        //parent.useMisFunctonFrame.CloseFullDownFrame();
        resetMeasure();
        resetRWD();
    }

    function page_leave() {
        stopMeasure();
    }


    function fillLength(partNum, totalLength) {
        this.totalLength = totalLength;
        switchLengthUnit();
        $mainDom.find("#lengthPartNum").text(partNum);
    }
    function fillArea(area, perimeter) {
        this.area = area;
        $mainDom.find("#perimeter").text((perimeter + "").toMoney(2));
        switchAreaUnit();
    }
    function stopMeasure() {
        plugin.measureLengthTool.clear();
        plugin.measureLengthTool.stop();
        plugin.measureAreaTool.clear();
        plugin.measureAreaTool.stop();
        $mainDom.find(".TabBox .BtnTab").removeClass("TabActive");
        $mainDom.find("li.help").hide();
    }
    function resetMeasure() {
        var $target = $mainDom.find(".active[data-bs-toggle='tab']");
        plugin.measureLengthTool.stop();
        plugin.measureLengthTool.clear();
        plugin.measureAreaTool.stop();
        plugin.measureAreaTool.clear();
        switch ($target.attr("id")) {
            case "measureLength":
                plugin.measureLengthTool.start(fillLength);
                helpHtml = helpHtmls[0];
                break;
            case "measureArea":
                plugin.measureAreaTool.start(fillArea);
                helpHtml = helpHtmls[1];
                break;
        }
        $mainDom.find("#totalLength").html(0);
        $mainDom.find("#lengthPartNum").text(0);
        $mainDom.find("#perimeter").text(0);
        $mainDom.find("#area").html(0);
        $mainDom.find("li.help").hide();
    }

    function switchLengthUnit() {
        var currUnitLength = this.totalLength;
        var unitDesc = "公尺(m)";
        switch ($mainDom.find("#lengthUnit").val()) {
            case "m":
                break;
            case "km":
                unitDesc = "公里(km)";
                currUnitLength /= 1000;
                break;
        }
        $mainDom.find("#totalLength").html(((currUnitLength) + "").toMoney(2));
        $mainDom.find("#spanLengthUnit").html(unitDesc);
    }

    function switchAreaUnit() {
        var currUnitArea = this.area;
        var unitDesc = "平方公尺(m<sup>2</sup>)";
        switch ($mainDom.find("#areaUnit").val()) {
            case "m2":
                break;
            case "km2":
                unitDesc = "平方公里(km<sup>2</sup>)";
                currUnitArea /= 1000000;
                break;
            case "Ha":
                unitDesc = "公頃(Ha)";
                currUnitArea /= 10000;
        }
        $mainDom.find("#area").html((currUnitArea + "").toMoney(2));
        $mainDom.find("#spanAreaUnit").html(unitDesc);
    }

    function clear() {
        stopMeasure();
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

    this.clear = clear;
};
