gs.Core.package("gsmap.func");

gsmap.func.BaseMap = function (_plugin, targetDom, baseMapData) {
    var plugin = _plugin;
    var $baseDom = $(targetDom);
    var $mainDom;
    var bShow = false;
    var me = this;

    // 讀取template
    $.ajax("map/template_basemap.html?" + Date.now()).done(
        function (data) {
            $mainDom = $(data);
            $baseDom.append($mainDom);

            initRWD();

            createBaseMap();

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
        resetRWD();
    }

    function page_leave() {

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


    function createBaseMap() {
        $mainDom.find(".BasicMapTable").children().remove();
        if (baseMapData)
            createFolder($mainDom.find(".BasicMapTable"), baseMapData);
    }

    function createFolder($parentFolderTable, data) {
        var $tr, $td1;
        for (var layerID in data) {
            var mapInfo = data[layerID];
            if (!mapInfo) continue;

            $tr = $('<tr><td/></tr>');
            $parentFolderTable.append($tr);

            $td1 = $tr.find("td:last");

            $td1.on("click", function () { swicthMapVisible(this.srcDom, this.layerID); }.bind({ srcDom: $td1[0], layerID: layerID }));
            $td1.append(layerID);
            $td1.attr("id", "td" + layerID);
        }

        $tr = $("<tr><td /></tr>");
        $parentFolderTable.append($tr);

        $td1 = $tr.find("td:last");
        $td1.on("click", function () { swicthMapVisible(this.srcDom, null); }.bind({ srcDom: $td1[0] }));
        $td1.append("無底圖");
        $td1.attr("id", "td無底圖");


        $mainDom.find(".BasicMapTable td[id='td"+plugin.layersMg.getBaseLayerID()+"']").addClass("active");
    }

    function swicthMapVisible(obj, layerID) {
        for (i = 0; i < $mainDom.find(".BasicMapTable td").length; i++) {
            $mainDom.find(".BasicMapTable td")[i].className = "";
        }
        obj.className = "active";

        if (layerID === null) {
            // 無底圖
            plugin.getBaseLayerGroup().getLayers().forEach(function (BaseLayer) {
                plugin.getBaseLayerGroup().getLayers().remove(BaseLayer);
            });
        } else {
            plugin.layersMg.setBaseLayer(layerID, true);
        }
    }

}
