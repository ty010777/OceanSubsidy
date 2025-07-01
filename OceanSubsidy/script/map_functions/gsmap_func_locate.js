gs.Core.package("gsmap.func");

gsmap.func.Locate = function (_plugin, targetDom) {
    var plugin = _plugin;
    var $baseDom = $(targetDom);
    var $mainDom;
    var bShow = false;
    var me = this;

    // 讀取template
    $.ajax("map/template_locate.html?" + Date.now()).done(
        function (data) {
            $mainDom = $(data);
            $baseDom.append($mainDom);
            if (bShow) {
                me.show();
            } else {
                me.hide();
            }

            $mainDom.find(".closebtn").on("click", function () { me.hide(); });
            $mainDom.find(".btn_clear").on("click", clear)

            // 初始化行政界定位
            mDistrict.init();

            // 初始化地址定位
            mAddr.init();

            // 初始化坐標定位
            mCoord.init();
			
			initRWD();
        }
    );

    function clear() {
        plugin.locateTool.clearLocatedFeature();
        plugin.hideBubble();
    }


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

    this.clear = clear;

    function page_active() {
        resetRWD();
    }

    function page_leave() {

    }


    function fillDropdownList(target, dataUrl, bKeepFirst, finishFunc) {
        var $target = null;
        if ($.prototype.isPrototypeOf(target)) {
            // target是jQuery物件
            $target = target;
        } else if (typeof target == "string") {
            // target是String
            $target = $("#" + target);
        } else {
            // 必須是html物件
            $target = $(target);
        }
        $.ajax({
            url: dataUrl,
            success: function (data) {
                if (bKeepFirst) $target[0].options.length = 1;
                else $target.children().remove();
                for (var i = 0; i < data.length; i++) {
                    $($target[0]).append("<option value='" + data[i].key + "'>" + data[i].value + "</option");
                }
                if (finishFunc) finishFunc();
            }
        });

    }

    // for district
    var mDistrict = new (function () {
        var bInit = false;
        function init() {
            if (!bInit) {
                $mainDom.find("#ddlCounty").on("change", genTowns);
                $mainDom.find("#ddlTown").on("change", genVillages);
                $mainDom.find("#district_box ._locate").on("click", locateToDistrict)

                genCounties();
                bInit = true;
            }
        }

        function genCounties() {
            if ($("#ddlCounty").children().length === 0) {
                fillDropdownList("ddlCounty", "Service/KeyValueDataSource.ashx?data=county", false, function () { $("#ddlCounty").attr("selected", "selected"); genTowns(); });
            } else {
                return;
            }
        }
        function genTowns() {
            fillDropdownList("ddlTown", "Service/KeyValueDataSource.ashx?data=town&county=" + encodeURIComponent($("#ddlCounty").val()), true);
        }
        function genVillages() {
            fillDropdownList("ddlVillage", "Service/KeyValueDataSource.ashx?data=village&county=" + encodeURIComponent($("#ddlCounty").val()) + "&town=" + encodeURIComponent($("#ddlTown").val()), true);
        }
        function locateToDistrict() {
            var dataUrl = null;
            var labelText = null;
            if ($("#ddlVillage").val() !== "") {
                dataUrl = "Service/GetLocationInfo.ashx?data=village&county=" + encodeURIComponent($("#ddlCounty").val()) + "&town=" + encodeURIComponent($("#ddlTown").val()) + "&village=" + encodeURIComponent($("#ddlVillage").val());
                labelText = $('#ddlCounty :selected').text() + $('#ddlTown :selected').text() + $('#ddlVillage :selected').text();
            } else if ($("#ddlTown").val() !== "") {
                dataUrl = "Service/GetLocationInfo.ashx?data=town&county=" + encodeURIComponent($("#ddlCounty").val()) + "&town=" + encodeURIComponent($("#ddlTown").val());
                labelText = $('#ddlCounty :selected').text() + $('#ddlTown :selected').text();
            } else {
                dataUrl = "Service/GetLocationInfo.ashx?data=county&county=" + encodeURIComponent($("#ddlCounty").val());
                labelText = $('#ddlCounty :selected').text();
            }
            $.ajax({
                url: dataUrl,
                success: function (data) {
                    plugin.locateTool.locateToWKT(data, "EPSG:4326", true, labelText);
                }
            });
        }

        this.init = init;
        this.genCounties = genCounties;
        this.genTowns = genTowns;
        this.genVillages = genVillages;
        this.locateToDistrict = locateToDistrict;
    })();
    // for district end

    // for address
    var mAddr = new (function () {

        var $panel;
        function init() {
            $panel = $mainDom.find("#address_box");

            $panel.find("._locate").on("click", queryAddress);
        }

        function queryAddress() {
            $.ajax("service/TGOS_Geocoder.ashx?addr=" + encodeURIComponent($.trim($("#txtAddress").val())))
                .then(
                    function (resp) {
                        if (resp.AddressList.length == 0) {
                            alert(resp.Info[0].OutTraceInfo);
                        } else {
                            var addr = resp.AddressList[0];
                            var coord = [addr.X, addr.Y];
                            var labelText = $("#txtAddress").val();
                            mapPlugin.showBubble({ coord: coord, proj: "EPSG:4326" }, addr.FULL_ADDR);
                            mapPlugin.locateTool.locateToCoord(coord, "EPSG:4326", labelText);
                        }
                    },
                    function () {
                        alert("目前此服務無法使用");
                    }
                );
        }

        this.init = init;
        this.queryAddress = queryAddress;
    })();
    // for address end

    var mCoord = new (function () {
        var $panel;
        function init() {
            $panel = $mainDom.find("#coordinate_box");

            $panel.find("._locate").on("click", locate);

            new bootstrap.Collapse($('#accordion')[0]);
            //('#accordion .collapse').collapse();
        }

        function locate() {
            var $target = $(event.target);
            switch ($target.attr("locateType")) {
                case "WGS84Decimal":
                    locateByWGS84Decimal();
                    break;
                case "WGS84DMS":
                    locateByWGS84DMS();
                    break;
                case "TWD97":
                    locateByTWD97();
                    break;
                case "TWD67":
                    locateByTWD67();
                    break;
            }
        }
        
        // 以十進位經緯度定位
        function locateByWGS84Decimal() {
            var lat = parseFloat($("#txtLatCoord").val());
            var lng = parseFloat($("#txtLngCoord").val());
            var labelText = lat.toString() + ", " + lng.toString();
            if (isNaN(lat) || isNaN(lng)) {
                alert("經緯度必須是數字");
                return;
            }
            parent.mapPlugin.locateTool.locateToCoord([lng, lat], "EPSG:4326", true, labelText);
            parent.mapPlugin.getMap().getView().setZoom(13);
        }
        // 以度分秒經緯度定位
        function locateByWGS84DMS() {
            var lat = parseFloat($("#txtLatDegree").val())
                + (isNaN(parseFloat($("#txtLatMinute").val())) ? 0 : parseFloat($("#txtLatMinute").val())) / 60
                + (isNaN(parseFloat($("#txtLatSecond").val())) ? 0 : parseFloat($("#txtLatSecond").val())) / 3600;

            var lng = parseFloat($("#txtLngDegree").val())
                + (isNaN(parseFloat($("#txtLngMinute").val())) ? 0 : parseFloat($("#txtLngMinute").val())) / 60
                + (isNaN(parseFloat($("#txtLngSecond").val())) ? 0 : parseFloat($("#txtLngSecond").val())) / 3600;
            var labelText = $("#txtLatDegree").val() + "度"
                + (isNaN(parseFloat($("#txtLatMinute").val())) ? 0 : $("#txtLatMinute").val()) + "分"
                + (isNaN(parseFloat($("#txtLatSecond").val())) ? 0 : $("#txtLatSecond").val()) + "秒, "
                + $("#txtLngDegree").val() + "度"
                + (isNaN(parseFloat($("#txtLngMinute").val())) ? 0 : $("#txtLngMinute").val()) + "分"
                + (isNaN(parseFloat($("#txtLngSecond").val())) ? 0 : $("#txtLngSecond").val()) + "秒";
            if (isNaN(lat) || isNaN(lng)) {
                alert("經緯度必須是數字");
                return;
            }
            parent.mapPlugin.locateTool.locateToCoord([lng, lat], "EPSG:4326", true, labelText);
            parent.mapPlugin.getMap().getView().setZoom(13);
        }
        function locateByTWD97() {
            var coordX = parseFloat($("#txtTWD97X").val());
            var coordY = parseFloat($("#txtTWD97Y").val());
            var labelText = coordX + ", " + coordY;
            if (isNaN(coordX) || isNaN(coordY)) {
                alert("坐標必須是數字");
                return;
            }
            parent.mapPlugin.locateTool.locateToCoord([coordX, coordY], "EPSG:3826", true, labelText);
            parent.mapPlugin.getMap().getView().setZoom(13);
        }
        function locateByTWD67() {
            var coordX = parseFloat($("#txtTWD67X").val());
            var coordY = parseFloat($("#txtTWD67Y").val());
            var labelText = coordX + ", " + coordY;
            if (isNaN(coordX) || isNaN(coordY)) {
                alert("坐標必須是數字");
                return;
            }
            parent.mapPlugin.locateTool.locateToCoord([coordX, coordY], "EPSG:3828", true, labelText);
            parent.mapPlugin.getMap().getView().setZoom(13);
        }
        function switchCoordType(obj) {
            $("._coordTab").removeClass("BtnOrangeActive");
            $(obj).addClass("BtnOrangeActive");
            $("._coordPanel").hide();
            $("#" + obj.value + "Panel").show();
            resizeLayout();
        }

        this.init = init;
        this.locateByWGS84Decimal = locateByWGS84Decimal;
        this.locateByWGS84DMS = locateByWGS84DMS;
        this.locateByTWD97 = locateByTWD97;
        this.locateByTWD67 = locateByTWD67;
        this.switchCoordType = switchCoordType;
    });
	
	
	
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
}
