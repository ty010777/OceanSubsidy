gsmap = typeof gsmap === "undefined" ? {} : gsmap;
gsmap.ui = gsmap.ui || {};

/**
 * 控制下方資訊面板，內涵頁籤介面
 * 可控制下方資訊面板開關語高度
 * 可取得指定頁籤的content
 * 
 * 結構<![CDATA[
 * <div class="gsmap-bottom">
 *   <ul class="gsmap-bottom-nav">
 *      <li>頁籤一</li>
 *      <li>頁籤二</li>
 *   </ul>
 *   <div class="gsmap-bottom-content">
 *      <div>
 *          identify內容
 *      </div>
 *      <div>
 *          內容二
 *      </div>
 *   </div>
 * </div>
 * ]]>
 * @param {ol.MapPlugin} _plugin MapPlugin instance
 */
gsmap.ui.BottomUI = function (_plugin) {
    var plugin = _plugin;
    var $mapDom = $(plugin.getMap().getTargetElement());
    var $bottom = $mapDom.find(".gsmap-bottom");
    var $nav = $bottom.find(".gsmap-bottom-nav");
    var $contentBottom = $bottom.find(".gsmap-bottom-content");
    var tabID2ContentID = {};
    var bottomPanelHeightLevel = 0;
    var me = this;

    $nav.find(".nav-link").on('click',
        function () {
            if ($(this).parent().hasClass("active")) {
                if (bottomPanelHeightLevel === 0) {
                    me.setBottomHeightLevel(1);
                } else {
                    me.setBottomHeightLevel(0);
                }
            } else {
                if (bottomPanelHeightLevel === 0) {
                    me.setBottomHeightLevel(1);
                }
            }
        }
    );


    /**
     * 加入一個新的tab content到bottom
     * @param {string} titleHtml html code for tab Name
     * @param {string} tabContentCssClass tab content css class
     * @param {jQueryDom} $contentDom jQuery DOM of content
     * @return {string} id of the added tab
     */
    this.addContent = function (titleHtml, tabContentCssClass, $contentDom) {
        var now = Date.now();
        var tabID = "tab" + now;
        var contentID = "bottomContent" + now;
        var $tab = $("<li class='nav-item'><a id='" + tabID + "' seq='" + now + "' href='#" + contentID + "' class='nav-link'></a></li>");
        $tab.find("a").html(titleHtml);
        $nav.append($tab);
        var $tabContent = $("<div id='" + contentID + "' css='tab-pane fade' role='tabpanel'>").addClass(tabContentCssClass);
        if ($contentDom) $tabContent.append($contentDom);
        tabID2ContentID(tabID, contentID);
        return tabID;
    };

    /**
     * switch to the tab By ID
     * @param {string} tabID ID of the tab
     */
    this.switchTo = function (tabID) {
        $("#" + tabID).tab("show");
    };

    /**
     * 取得指定tab對應內容的jQuery DOM
     * @param {string} tabID ID of the tab
     * @return {jQueryDom} 指定tab的對應內容
     */
    this.getContent$ = function (tabID) {
        return $($("#" + tabID).attr("href"));
    };

    this.getActiveTabID = function () {
        return $nav.find(".active").attr("id");
    };

    /** 取得目前的顯示內容的jQuery DOM
     * @return {jQueryDom} 目前的顯示內容
     * */
    this.getActiveContent$ = function () {
        return this.getContent$($nav.find(".active").attr("id"));
    };

    /**
     * 設定高度level
     * @param {int} newHeightLevel level
     */
    this.setBottomHeightLevel = function (newHeightLevel) {
        if (newHeightLevel === bottomPanelHeightLevel) return;
        bottomPanelHeightLevel = newHeightLevel;
        resetBottomHeight();
    };

    var resetBottomHeight = function () {
        if ($bottom.length === 0) return;
        $bottom.css("overflow", "");
        var newHeight;
        if (bottomPanelHeightLevel === 0) {
            newHeight = $nav.height();
            $bottom.css("overflow", "hidden");
            $bottom.animate(
                {
                    scrollTop: 0,
                    height: newHeight
                }
            );
        } else if (bottomPanelHeightLevel === 1) {
            newHeight = $mapDom.height() * 0.4;
            $bottom.animate(
                {
                    scrollTop: 0,
                    height: newHeight
                },
                function () {
                    setTimeout(
                        function () {
                            $contentBottom.height(newHeight - ($contentBottom.offset().top - $contentBottom.offsetParent().children().first().offset().top));
                        }, 10);
                }
            );
        } else if (bottomPanelHeightLevel === 2) {
            newHeight = $mapDom.height() - (mobileCheck() ? 0 : 100);
            $bottom.animate(
                {
                    scrollTop: 0,
                    height: newHeight
                },
                function () {
                    setTimeout(
                        function () {
                            $contentBottom.height(newHeight - ($contentBottom.offset().top - $contentBottom.offsetParent().children().first().offset().top));
                        }, 10);
                }
            );
        }

        if (gs.RWDHelper.mobileCheck()) {
            $(plugin.getMap().getTargetElement()).find(".gsmap-scale-line").animate(
                {
                    bottom: newHeight + 10
                });
        }
    };
};