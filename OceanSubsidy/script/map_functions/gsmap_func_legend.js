gs.Core.package("gsmap.func");

gsmap.func.Legend = function (_plugin, targetDom) {
    var plugin = _plugin;
    var $baseDom = $(targetDom);
    var $mainDom;
    var bShow = false;
    var me = this;

    var map;

    var externalGroupID = "externalGroup";

    // 讀取template
    $.ajax("map/template_legend.html?" + Date.now()).done(
        function (data) {
            $mainDom = $(data);
            $baseDom.append($mainDom);

            map = plugin.getMap();

            createLegend();

            $mainDom.find("#addlayer_box input:radio[name='layerType']").on('click', switchToLayerInfo);
            $mainDom.find("#addlayer_box input:radio[name='layerType']:checked").click();

            $mainDom.find(".nav-tabs *[href='#existsLayer_box']").on(
                    'show.bs.tab',
                    showExistLayer
                );

            $mainDom.find(".BtnClear")
                .on(
                    'click',
                    hideAllLayer
                );

            $(map.getTargetElement()).append($mainDom.find("._toMap"));

            $("#btnLegend").on('click', swicthLegend);

            $mainDom.find("#addlayer_box ._addLayer").on('click', addLayer);
            $mainDom.find("#addlayer_box ._closeLayerAddingPanel").on('click', closeLayerAddingPanel);

            $("#labelLegend").sortable(
                    {
                        stop: function (event, ui) {
                            resetLayersZIndex();
                        }
                    }
                );
            $("#labelLegend").disableSelection();
            initRWD();

            if (bShow) {
                me.show();
            } else {
                me.hide();
            }

            $mainDom.find(".closebtn").on("click", function () { me.hide(); });

            map.getView().on(
                'change:resolution',
                function () {
                    var layerInfos = plugin.layersMg.getLayerInfo();
                    for (var key in layerInfos)
                        resetLegendCheckbox(layerInfos[key]);
                }
            );
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
    };

    this.clear = function () {
        action_clear();
    }

    // 開啟此功能UI時
    function page_active() {
        resetRWD();
    }

    // 關閉此功能UI時
    function page_leave() {

    }

    // 點選清除時
    function action_clear() {
        hideAllLayer();
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


    function createLegend() {
        var parentMapLayerObj = plugin.layersMg.getLayerInfo();
        var $legend = $mainDom.find("#gs_legend_layers");
        var $mainGroupTemplate = $mainDom.find("#template_gslegend_mainGroup");

        parentMapLayerObj[externalGroupID] = {
            name: "外部圖層",
            type: "folder",
            sub: []
        }

        var defaultTab = null;
        // 產生頁籤
        for (var id in parentMapLayerObj) {
            if (isHidden(parentMapLayerObj[id])) continue;
            // 不可以數字為第一個字母(bootstrap限制)
            var domID = "M" + id.replace(/\-/g, '');
            var $newMainGroup = $mainGroupTemplate.clone();
            $newMainGroup.attr("id", null);
            $newMainGroup.find(".collapse").attr("id", domID);
            $legend.append($newMainGroup);
            $newMainGroup.show();

            $newMainGroup.find(".card-header a")
                .attr("data-bs-target", "#" + domID)
                .attr("href", "javascript:void(0)")
                .text(parentMapLayerObj[id].name);

            createFolder($newMainGroup.find(".Tree-menu"), parentMapLayerObj[id].sub);
        }
    }

    // create layer folder
    function createFolder($parentFolderTable, data) {
        $parentFolderTable.children().remove();

        for (var layerID in data) {
            var layerInfo = data[layerID];
            if (!layerInfo) continue;
            if (isHidden(layerInfo)) continue;

            var layerName = layerInfo.name;
            if (!layerName || layerName.replace(/ /g, '') === '') layerName = "未命名";

            var $sub = $("<li>").attr("layerID", plugin.layersMg.getLayerID(layerInfo));

            var $checkbox = $('<input type="checkbox" class="visible_switch" />');
            $checkbox.on(
                        'change',
                        function () {
                            switchLayerVisible(event.target, $(event.target).parent().attr("layerID"));
                        }
                    );

            if (layerInfo.type == "folder") {
                $checkbox.addClass("_folder");
                $sub
                    .append($checkbox)
                    .append('<input checked="checked" id="switch_' + layerID + '" type="checkbox" class="switch">')
                    .append('<i>')
                    .append($('<label for="switch_' + layerID + '"></label>').text(layerName))
                    .append('<ul>');

                if (layerInfo.visible) $sub.find(':checkbox.visible_switch').prop('checked', true);

                if (layerInfo.sub) {
                    createFolder($sub.find("ul"), layerInfo.sub);
                    if ($sub.find(" > ul > li").length > 0) {
                        $parentFolderTable.append($sub);
                    }
                }
            } else {
                $sub.append($checkbox)

                $parentFolderTable.append($sub);
                var layer = plugin.layersMg.getLayer(layerInfo);
                if (layer && plugin.layersMg.isLayerTurnOn(layerInfo)) $sub.find(':checkbox.visible_switch').prop('checked', true);
                // create a layer
                $sub.addClass("TreeNode");
                if (layerInfo.legendIcon) {
                    $sub.append("<img src='" + layerInfo.legendIcon + "' style='width:18px;'/>");
                    $sub.append($("<span>").text(" " + layerName));
                    $sub.append('<input type="button" class="BtnTrans" style="display:none" />');
                } else if (layerInfo.legend) {
                    var align = "left";
                    if (layerInfo.legend.align) align = layerInfo.legend.align;
                    if (align == "bottom") {
                        $sub.append($("<span>").text(" " + layerName));
                        $sub.append("<br/>");
                        $sub.append("<img src='" + layerInfo.legend.url + "' style='padding-left:10px;'/>");
                        $sub.append('<input type="button" class="BtnTrans" style="display:none"/>');
                    } else {
                        $sub.append("<img src='" + layerInfo.legend.url + "'/>");
                        $sub.append($("<span>").text(" " + layerName));
                        $sub.append('<input type="button" class="BtnTrans" style="display:none"/>');
                    }
                }
                $sub.find(".BtnTrans")
                    .on(
                            'click',
                            function () {
                                showLayerOpacitySilder($(event.target).parent().attr("layerID"));
                            }
                        );

                resetLegendCheckbox(layerInfo);
            }
        }

    }

    function hideAllLayer() {
        var existLayers = plugin.layersMg.getTurnOnLayers();
        for (var key in existLayers) {
            plugin.layersMg.switchTheLayer(existLayers[key], false);
        }

        $mainDom.find("#gs_legend_layers input.visible_switch[type='checkbox']:checked").prop('checked', false);

        //// 先關閉群組
        //$mainDom.find("#gs_legend_layers input._folder.visible_switch[type='checkbox']:checked").click();
        //// 再關閉還開著的個別圖層
        //$mainDom.find("#gs_legend_layers input.visible_switch[type='checkbox']:checked").click();

        $("#labelLegend")[0].innerText = "";
        $("#labelLegend").css('display', 'none');
        $("#btnLegend").css('display', 'none');
    }

    // 重設checkbox是否active，透明度button是否顯示
    function resetLegendCheckbox(layerInfos) {
        if (layerInfos.type === "folder") {
            layerInfos = layerInfos.sub;
        } else if (layerInfos.type === "node") {
            layerInfos = [layerInfos];
        }
        for (var key in layerInfos) {
            var layerInfo = layerInfos[key];
            if (!layerInfo) {
                continue;
            }

            if (layerInfo.type === "folder") {
                resetLegendCheckbox(layerInfo.sub);
            } else {
                let $td = $mainDom.find("#gs_legend_layers li[layerID='" + plugin.layersMg.getLayerID(layerInfo) + "']");
                if (plugin.layersMg.getLayerShowup(layerInfo)) {
                    $td.find(":checkbox.visible_switch").css("opacity", 1);
                    if (plugin.layersMg.isLayerTurnOn(layerInfo)) {
                        $td.find(".BtnTrans").show();
                    } else {
                        $td.find(".BtnTrans").hide();
                    }
                } else {
                    $td.find(":checkbox.visible_switch").css("opacity", 0.2);
                    $td.find(".BtnTrans").hide();
                }
            }
        }
    }

    function switchToExternalGroup() {
        var layerID = externalGroupID;
        closeLayerAddingPanel();
        var parentMapLayerObj = plugin.layersMg.getLayerInfo();
        createFolder($("#M" + layerID + " .Tree-menu"), parentMapLayerObj[layerID].sub)
        $("#M" + layerID).collapse('show');
    }

    function showExistLayer() {
        $mainDom.find("#legendTableExists").children().remove();
        var existLayer = plugin.layersMg.getTurnOnLayers();
        createFolder($mainDom.find("#legendTableExists"), existLayer.reverse());
    }

    function switchLayerVisible(obj, layerID) {
        var layerInfo = plugin.layersMg.getLayerInfo(layerID);
        var legendIcon = layerInfo.legendIcon;
        var layerName = layerInfo.name;
        if (!layerName || layerName.replace(/ /g, '') === '') layerName = "未命名";

        var disabled = !obj.checked;
        plugin.layersMg.switchTheLayer(layerID, obj.checked, "top");

        var $checkbox = $mainDom.find("*[layerID='" + layerID + "']").find(":checkbox.visible_switch");
        if (disabled) $checkbox.prop("disabled", true);
        $checkbox.prop("checked", $(obj).prop("checked"));
        if (disabled) $checkbox.prop("disabled", false);

        var layerFolder;
        if (layerID.split('/').length == 2) {
            layerFolder = layerID.split('/')[0];
        } else if (layerID.split('/').length == 3) {
            layerFolder = layerID.split('/')[0] + "/" + layerID.split('/')[1];
        }

        if (obj.checked) {
            $(obj).parent().find(".BtnTrans").show();
        } else {
            $(obj).parent().find(".BtnTrans").hide();
        }

        resetLegendCheckbox(layerInfo);
        switchLayerLegend();

        //parent.updateBottom();
    }

    function switchLayerLegend() {
        var layerInfo;
        var targetInfos;
        targetInfos = plugin.layersMg.getTurnOnLayers();
        $("#labelLegend").children().remove();
        for (var idx = 0; idx < targetInfos.length; idx++) {
            layerInfo = targetInfos[idx];
            if (isHidden(layerInfo)) continue;
            var layerName = layerInfo.name;
            if (!layerName || layerName.replace(/ /g, '') === "") layerName = "未命名";
            var layerID = plugin.layersMg.getLayerID(layerInfo);
            $("#labelLegend").prepend(
                "<div id='" + layerID + "_legend' style='padding:2px'>"
                + '<span class="ui-icon ui-icon-arrowthick-2-n-s"></span>'
                + "<img id='" + layerID + "_Img' src=" + layerInfo.legendIcon + " height='30' />&nbsp;"
                + "<span style='font:14px 微軟正黑體; font-weight:bold;'>" + layerName + "</span>"
                + "</div>");
        }

        resetLayersZIndex();

        if (plugin.layersMg.getTurnOnLayers().length > 0
            && $(window).width() > 979) {
            $("#labelLegend").css('display', '');
            $("#btnLegend").css('display', '');
        } else {
            $("#labelLegend").css('display', 'none');
            $("#btnLegend").css('display', 'none');
        }
    }

    function swicthLegend() {
        if ($("#labelLegend").css("display") == "block") {
            $("#labelLegend").css("display", "none");
        } else {
            $("#labelLegend").css("display", "block");
        }

        if ($("#btnLegend").hasClass("active")) {
            $("#btnLegend").removeClass('active');
        } else {
            $("#btnLegend").addClass('active');
        }
    }

    function resetLayersZIndex() {
        var sortedLayers = [];
        $("#labelLegend div").each(
                function () {
                    var layerID = $(this).attr("id");
                    if (!layerID) return;
                    sortedLayers.push($(this).attr("id").replace(/\_legend$/, ''));
                }
            );
        plugin.layersMg.resetLayersZIndex(sortedLayers.reverse());
    }

    function isHidden(layerInfo) {
        if (layerInfo.hidden) return true;

        if (!layerInfo.layerID) return false;

        var parentInfo = plugin.layersMg.getParentInfo(layerInfo);
        if (parentInfo) return isHidden(parentInfo);
        return false;
    }


    // 顯示透明度拖拉bar
    function showLayerOpacitySilder(target) {
        var opacity = plugin.layersMg.getLayer(target).getOpacity();
        $mainDom.find("#opacitySlider").slider({
            value: opacity * 100,
            orientation: "horizontal",
            range: "min",
            animate: true,
            stop: function () {
                plugin.layersMg.getLayer(target).setOpacity($mainDom.find("#opacitySlider").slider("value") / 100);
            }
        });
        $mainDom.find("#layerOpacityPanel").css("left", window.event.clientX - 200).css("top", window.event.clientY - 40).show();
    }

    // ===============================
    // external layer
    // ===============================

    function closeLayerAddingPanel() {
        $(".nav-tabs *[data-bs-toggle='tab'][href='#legend']").tab('show');
        plugin.layersMg.deactiveDragAndDrop();
    }

    function switchToLayerInfo() {
        var target = $(event.target).attr("layerType");
        $mainDom.find(".LayerInfo").hide();
        $mainDom.find("#" + target).show();
        if (target == "kmlLayerInfo") {
            if (typeof FileReader == "undefined") {
                $mainDom.find("#kmlLayerInfo tr:eq(1)").hide();
                $mainDom.find("#kmlLayerInfo tr:eq(2)").show();
            } else {
                // 支援HTML5 drag-and-drop才可以使用拖拉套用
                $mainDom.find("#kmlLayerInfo tr:eq(1)").show();
                $mainDom.find("#kmlLayerInfo tr:eq(2)").hide();
                plugin.layersMg.activeDragAndDrop(function (evt) {
                    try {
                        var layerKey = new Date().getTime().toString();
                        var layerInfo = {
                            name: $mainDom.find("#kmlLayerName").val(),
                            type: "node",
                            legendIcon: "App_Themes/map/images/16.png",
                            serviceInfo: {
                                type: "vector",
                                bubbleContent: "#AUTO#"
                            }
                        };
                        var layerID = plugin.layersMg.addLayerInfo(externalGroupID, layerKey, layerInfo);
                        var layer = plugin.layersMg.addLayer(layerInfo);
                        switchLayerLegend();
                        layer.getSource().addFeatures(evt.features);
                        $mainDom.find("#kmlLayerName").val("");
                        $mainDom.find("#kmlURL").val("");
                        switchToExternalGroup();
                        $mainDom.find("#gs_legend_layers li[layerID='" + layerID + "'] :checkbox.visible_switch").prop('checked', true);
                        plugin.locateTool.zoomToLayer(layer);

                    } catch (e) {
                        console.error(e);
                    }
                });
            }
        } else {
            plugin.layersMg.deactiveDragAndDrop();
        }
    }

    function addLayer() {
        var layerType = $mainDom.find("input[name='layerType']:checked").val();
        if (layerType == "kml") {
            $mainDom.find("#ifKmlUpload").contents().find("#btnUpload").click();
        } else if (layerType == "gml") {
            var layerInfo = {
                name: $mainDom.find("#gmlLayerName").val(),
                type: "node",
                legendIcon: "App_Themes/map/images/16.png",
                serviceInfo: {
                    type: "gml",
                    url: $mainDom.find("#gmlURL").val()
                }
            };
            plugin.layersMg.addLayerInfo(externalGroupID, new Date().getTime().toString(), layerInfo);
            var layer = plugin.layersMg.addLayer(layerInfo);
            $mainDom.find("#gmlLayerName").val("");
            $mainDom.find("#gmlURL").val("");
            switchToExternalGroup();
            plugin.locateTool.zoomToLayer(layer);
        } else if (layerType == "wms") {
            plugin.layersMg.addWMSSource(externalGroupID, $mainDom.find("#wmsURL").val(),
                function () {
                    switchToExternalGroup();
                }
            );
        } else if (layerType == "wfs") {
            plugin.layersMg.addWFSSource(externalGroupID, $mainDom.find("#wfsURL").val(),
                function () {
                    switchToExternalGroup();
                }
            );
        } else if (layerType == "shp") {
            $mainDom.find("#ifShpUpload").contents().find("#hfCRS").val($mainDom.find("#ddlShpCoordSys").val());
            $mainDom.find("#ifShpUpload").contents().find("#btnUpload").click();
        } else if (layerType == "wmts") {
            plugin.layersMg.addWMTSSource(externalGroupID, $mainDom.find("#wmtsURL").val(),
                function () {
                    switchToExternalGroup();
                }
            );
        }
        switchLayerLegend();
        //closeLayerAddingPanel();
    }

    function addKMLLayerInfo(layerName, kmlURL) {
        if (!layerName || layerName.replace(/ /g, "") == "") {
            layerName = $mainDom.find("#kmlLayerName").val();
        }
        var layerInfo = {
            name: layerName,
            type: "node",
            legendIcon: "App_Themes/map/images/16.png",
            serviceInfo: {
                type: "kml",
                url: kmlURL,
                bubbleContent: "#AUTO#"
            }
        };
        plugin.layersMg.addLayerInfo(externalGroupID, new Date().getTime().toString(), layerInfo);
        var layer = plugin.layersMg.addLayer(layerInfo);
        switchLayerLegend();
        return layer;
    }

    function addKML(kmlURL) {
        var layer = addKMLLayerInfo($mainDom.find("#kmlLayerName").val(), kmlURL);
        $mainDom.find("#kmlLayerName").val("");
        switchToExternalGroup();
        plugin.locateTool.zoomToLayer(layer);
    }

    function addShp(kmlURL) {
        var layer = addKMLLayerInfo($mainDom.find("#shpLayerName").val(), kmlURL);
        $mainDom.find("#shpLayerName").val("");
        switchToExternalGroup();
        plugin.locateTool.zoomToLayer(layer);
    }

    // ===============================
    // external layer END
    // ===============================

    this.addShp = addShp;

}
