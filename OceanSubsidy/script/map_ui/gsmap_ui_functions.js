gsmap = typeof gsmap === "undefined" ? {} : gsmap;
gsmap.ui = gsmap.ui || {};

gsmap.ui.Functions = function (_plugin, options) {
    var plugin = _plugin;
    var $baseDom = $(plugin.getMap().getTargetElement()).find(".GSMap-wrapper");
    var $mainDom = $baseDom.find(".mapnav");
    var $toggleNode;
    var funcClick = options.funcClick;
    var me = this;

    var funcContainer = {};

    $.ajax("map/template_functions.html?" + Date.now()).done(
        function (data) {
            $mainDom = $(data);
            $baseDom.append($mainDom);

            $baseDom.find(".GSMap-menu-collapse").click(function (e) {
                e.preventDefault();
                $baseDom.toggleClass("toggled");
                if ($baseDom.hasClass("toggled")) {
                    $(".GSMap-panel").addClass("mapnavOpen");
                } else {
                    $(".GSMap-panel").removeClass("mapnavOpen");
                }
            });

            $mainDom.find(".GSMap-menu li").hide();
            if (options && options.functions) {
                var arsEnableFuncs = options.functions.split(',');
                // 只顯示啟用的功能
                arsEnableFuncs.forEach(function (navID) {
                    $mainDom.find('.nav_' + navID +" a").on("click", function () { me.switchTo(this); });
                    $mainDom.find('.nav_' + navID).closest("li").show();
                });
            }

            for (var key in gsmap.func) {
                if (gsmap.func[key].prototype.preInit) {
                    gsmap.func[key].prototype.preInit();
                }
            }

            $(window).on('resize', resize);
            resize();
            //$mainDom.mapUI = me;
        }
    );

    function resize() {
        //hideWhenMobile();
    }

    function hideWhenMobile() {
        var targets = ["Measure", "Export"];

        var size = [gs.RWDHelper.getWindowHeight(), gs.RWDHelper.getWindowWidth()].sort().reverse();

        for (var i = 0; i < targets.length; i++) {
            var func = funcContainer[targets[i]];
            if (size[0] < 600 || size[1] < 800) {
                if (func && func.hide) func.hide();
                $("#mapnav a[do='" + targets[i] + "']").parent().hide();
            } else {
                $("#mapnav a[do='" + targets[i] + "']").parent().show();
            }
        }
    }

    this.switchTo = function (target) {
        var $target = $(target);

        if (!$target.attr("do")) {
            return;
        }

        var theFunc = eval("do" + $target.attr("do"));

        if (!theFunc || typeof theFunc !== "function") return;

        $mainDom.find("a").removeClass("active");

        for (var key in funcContainer) {
            if (funcContainer[key] && typeof funcContainer[key].hide === "function") {
                funcContainer[key].hide();
            }
        }

        if (theFunc()) {
            $target.addClass("active");
        }

    };


    function toggleFunc(E) {
        E.preventDefault();
        let $box = $baseDom.find('.map_menu');
        if ($box.hasClass("box-close")) {
            newClassName = "map_menu box-open";
            $box.removeClass("box-close");
            $box.addClass("box-open");
        } else {
            $box.removeClass("box-open");
            $box.addClass("box-close");
        }
    }

    this.getFuncModule = function (funcName) {
        return funcContainer[funcName];
    }

    function doBaseMap() {
        if (!funcContainer["basemap"]) funcContainer["basemap"] = new gsmap.func.BaseMap(plugin, $baseDom[0], options.baseMapData);

        var func = funcContainer["basemap"];
        func.show();
        return true;
    }

    function doMarker() {
        if (!funcContainer["marker"]) {
            funcContainer["marker"] = new gsmap.func.Marker(plugin, $baseDom[0]);
        }
        var m = funcContainer["marker"];
        m.show();
        return true;
    }

    function doActivityQuery() {
        if (!funcContainer["activityquery"]) {
            funcContainer["activityquery"] = new gsmap.func.ActivityQuery(plugin, $baseDom[0]);
        }
        var m = funcContainer["activityquery"];
        m.show();
        return true;
    }

    function doMeasure() {
        if (!funcContainer["measure"]) funcContainer["measure"] = new gsmap.func.Measure(plugin, $baseDom[0]);

        var func = funcContainer["measure"];
        func.show();
        return true;
    }

    function doLocate() {
        if (!funcContainer["locate"]) funcContainer["locate"] = new gsmap.func.Locate(plugin, $baseDom[0]);

        var func = funcContainer["locate"];
        func.show();
        return true;
    }

    function doLegend() {
        if (!funcContainer["legend"]) funcContainer["legend"] = new gsmap.func.Legend(plugin, $baseDom[0], map_base);

        var func = funcContainer["legend"];
        func.show();
        return true;
    }

    // 匯出
    function doExport() {
        if (!funcContainer["export"]) funcContainer["export"] = new gsmap.func.Export(plugin, $baseDom[0]);

        var func = funcContainer["export"];
        func.show();
        return true;
    }

    function doInitPlace() {
        mapPlugin.backToInitPosition();
        return false;
    }

    function doCloseAll() {
        for (var key in funcContainer) {
		    if (funcContainer[key]) {
		        if (typeof funcContainer[key].hide === "function")
                funcContainer[key].hide();
		        if (typeof funcContainer[key].clear === "function")
		            funcContainer[key].clear();
            }
        }
        return false;
    }


};