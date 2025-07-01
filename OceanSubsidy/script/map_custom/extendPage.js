var misFunctonFrame = function () {
    this.isleftOpen = false;
    this.leftFrameId = "mis_left_content";
    this.leftFrameIconId = "mis_left_icon";
    this.leftiFrameId = "mis_left_contentFrame";
    this.isdownOpen = false;
    this.downOpenLevel = 0;
    this.downBtnHeight = 0;
    this.downFrameId = "mis_down_content";
    this.downFrameOpenIconId = "mis_down_icon1";
    this.downFrameCloseIconId = "mis_down_icon2";
    this.downiFrameId = "mis_down_contentFrame";
};

misFunctonFrame.prototype = {
    OpenLeftFrame: function () {
        if (!paramMoveAction01.isRunning) {
            paramMoveAction01.isRunning = true;
            var leftFrameWidth = $("#" + this.leftFrameId).width();
            var leftFrameCurrectLeft = 0 - leftFrameWidth;
            var winWidth = gs.RWDHelper.getWindowWidth();
            var downFrameIconRight1 = winWidth / 2;
            var downFrameIconRight2 = downFrameIconRight1 - 36;
            $("#" + useMisFunctonFrame.downFrameId).css("width", winWidth - leftFrameWidth - 1);
            $("#" + useMisFunctonFrame.downFrameOpenIconId).css("right", downFrameIconRight1 - leftFrameWidth);
            $("#" + useMisFunctonFrame.downFrameCloseIconId).css("right", downFrameIconRight2 - leftFrameWidth);

            paramMoveAction01 = {
                runInterval: null,
                ctrlObjcetId: [this.leftFrameId, this.leftFrameIconId],
                ctrlObjcetAttb: ["left", "left"],
                currectValue: [leftFrameCurrectLeft, 0],
                targetValue: [0, leftFrameWidth],
                stepValue: [40, 40],
                isRunning: true,
                endSetHide: [false, false]
            };
            paramMoveAction01.runInterval = setInterval(runMoveAction01, 20);
        }

        $("#" + this.leftFrameId)[0].style.display = "block";
        this.isleftOpen = true;
    },
    CloseLeftFrame: function () {
        if (!paramMoveAction01.isRunning) {
            paramMoveAction01.isRunning = true;
            var leftFrameWidth = $("#" + this.leftFrameId).width();
            var leftFrameTargetLeft = 0 - leftFrameWidth;
            var winWidth = gs.RWDHelper.getWindowWidth();
            var downFrameIconRight1 = winWidth / 2;
            var downFrameIconRight2 = downFrameIconRight1 - 36;
            $("#" + useMisFunctonFrame.downFrameId).css("width", winWidth);
            $("#" + useMisFunctonFrame.downFrameOpenIconId).css("right", downFrameIconRight1);
            $("#" + useMisFunctonFrame.downFrameCloseIconId).css("right", downFrameIconRight2);

            paramMoveAction01 = {
                runInterval: null,
                ctrlObjcetId: [this.leftFrameId, this.leftFrameIconId],
                ctrlObjcetAttb: ["left", "left"],
                currectValue: [0, leftFrameWidth],
                targetValue: [leftFrameTargetLeft, 0],
                stepValue: [-40, -40],
                isRunning: true,
                endSetHide: [true, false]
            };
            paramMoveAction01.runInterval = setInterval(runMoveAction01, 20);
        }

        this.isleftOpen = false;
    },
    OpenDownFrame: function () {
        if (!paramMoveAction01.isRunning) {
            paramMoveAction01.isRunning = true;
            var targetBottom = $("body").height() - gs.RWDHelper.getWindowHeight();
            var downFrameCurrectHeight = $("#" + this.downFrameId).height();
            if (this.downOpenLevel === 0) {
                // 0 to 1
                $("#" + this.downFrameOpenIconId).css("display", "");
                $("#" + this.downFrameCloseIconId).css("display", "");
                var downFrameCurrectBottom = 0 - downFrameCurrectHeight;

                paramMoveAction01 = {
                    runInterval: null,
                    ctrlObjcetId: [this.downFrameId, this.downFrameOpenIconId, this.downFrameCloseIconId],
                    ctrlObjcetAttb: ["bottom", "bottom", "bottom"],
                    currectValue: [downFrameCurrectBottom, 0, 0],
                    targetValue: [targetBottom, downFrameCurrectHeight + targetBottom, downFrameCurrectHeight + targetBottom],
                    stepValue: [40, 40, 40],
                    isRunning: true,
                    endSetHide: [false, false, false]
                };
                paramMoveAction01.runInterval = setInterval(runMoveAction01, 20);

                this.downOpenLevel = this.downOpenLevel + 1;
                $("#" + this.downFrameId).show();
                this.isdownOpen = true;
            } else if (this.downOpenLevel === 1) {
                // 1 to 2
                var intHeight = gs.RWDHelper.getWindowHeight();
                var downFrameTargetHeight = intHeight - 92;

                paramMoveAction01 = {
                    runInterval: null,
                    ctrlObjcetId: [this.downFrameId, this.downFrameOpenIconId, this.downFrameCloseIconId],
                    ctrlObjcetAttb: ["height", "bottom", "bottom"],
                    currectValue: [downFrameCurrectHeight, downFrameCurrectHeight + targetBottom, downFrameCurrectHeight + targetBottom],
                    targetValue: [downFrameTargetHeight, downFrameTargetHeight + targetBottom, downFrameTargetHeight + targetBottom],
                    stepValue: [40, 40, 40],
                    isRunning: true,
                    endSetHide: [false, false, false]
                };
                paramMoveAction01.runInterval = setInterval(runMoveAction01, 20);

                this.downOpenLevel = this.downOpenLevel + 1;
                $("#" + this.downFrameId).show();
                this.isdownOpen = true;
            } else {
                paramMoveAction01.isRunning = false;
            }

            $("#mis_down_iconclose").css("opacity", "1");
            if (this.downOpenLevel >= 2) {
                $("#mis_down_iconopen").css("opacity", "0.2");

            } else {
                $("#mis_down_iconopen").css("opacity", "1");
            }
        }
    },
    CloseDownFrame: function () {
        if (!paramMoveAction01.isRunning) {
            paramMoveAction01.isRunning = true;
            var targetBottom = (document.body.clientHeight || $(window).height()) - gs.RWDHelper.getWindowHeight();
            var downFrameCurrectHeight = JsGetNumeric_Int($("#" + this.downFrameId).css("height"));

            if (this.downOpenLevel === 1) {
                var downFrameTargetBottom = 0 - downFrameCurrectHeight;
                // 1 to 0
                //$("#mis_down_icon1").css("display", "none");
                //$("#mis_down_icon2").css("display", "none");
                paramMoveAction01 = {
                    runInterval: null,
                    ctrlObjcetId: [this.downFrameId, this.downFrameOpenIconId, this.downFrameCloseIconId],
                    ctrlObjcetAttb: ["bottom", "bottom", "bottom"],
                    currectValue: [0, downFrameCurrectHeight, downFrameCurrectHeight],
                    targetValue: [downFrameTargetBottom, targetBottom, targetBottom],
                    stepValue: [-40, -40, -40],
                    isRunning: true,
                    endSetHide: [true, false, false]
                };
                paramMoveAction01.runInterval = setInterval(runMoveAction01, 20);

                this.downOpenLevel = this.downOpenLevel - 1;
                this.isdownOpen = false;
            } else if (this.downOpenLevel === 2) {
                // 2 to 1
                var intHeight = gs.RWDHelper.getWindowHeight();
                var downFrameTargetHeight;
                if ($(window).width() <= 979) {
                    downFrameTargetHeight = intHeight / 3 + 92;
                } else {
                    downFrameTargetHeight = intHeight / 3;
                }

                paramMoveAction01 = {
                    runInterval: null,
                    ctrlObjcetId: [this.downFrameId, this.downFrameOpenIconId, this.downFrameCloseIconId],
                    ctrlObjcetAttb: ["height", "bottom", "bottom"],
                    currectValue: [downFrameCurrectHeight, downFrameCurrectHeight + targetBottom, downFrameCurrectHeight + targetBottom],
                    targetValue: [downFrameTargetHeight, downFrameTargetHeight + targetBottom, downFrameTargetHeight + targetBottom],
                    stepValue: [-40, -40, -40],
                    isRunning: true,
                    endSetHide: [false, false, false]
                };
                paramMoveAction01.runInterval = setInterval(runMoveAction01, 20);

                this.downOpenLevel = this.downOpenLevel - 1;
                this.isdownOpen = true;
            } else {
                paramMoveAction01.isRunning = false;
            }
            $("#mis_down_iconopen").css("opacity", "1");
            if (this.downOpenLevel <= 0) {
                $("#mis_down_iconclose").css("opacity", "0.2");

            } else {
                $("#mis_down_iconclose").css("opacity", "1");
            }
        }
    },
    OpenFunInLeft: function (url) {
        getObject(this.leftiFrameId).src = url;
        if (!this.isleftOpen) {
            misLeftIconClick($("#mis_left_icon_li")[0]);
            //parent.$("#labelLegend").css("left", "478px");
            //parent.$("#btnLegend").css("left", "448px");
            if ($(window).width() <= 979) {
                var width = $(window).width() + "px";
                $("#" + this.downFrameCloseIconId).css("display", "none");
                $("#" + this.downFrameOpenIconId).css("display", "none");
                //$("#mis_left_contentFrame").css("height", "200%");
                $("#mis_left_contentFrame").css("width", width);
                //$("#mis_left_content").css("height", "200%");
                $("#mis_left_content").css("width", "320px");
            }
        }
    },
    OpenFunInDown: function (url, IsFull) {
        //if (getObject(this.downiFrameId).src != url) {
        //    getObject(this.downiFrameId).src = url;
        //}
        if (!this.isdownOpen) {
            if (IsFull) {
                this.OpenFullDownFrame();
            } else {
                this.OpenDownFrame();
            }
        }
    },
    OpenFullDownFrame: function () {
        if (!paramMoveAction01.isRunning) {
            if (this.downOpenLevel === 2) {
                return;
            }
            paramMoveAction01.isRunning = true;
            var intHeight = gs.RWDHelper.getWindowHeight();
            var downFrameCurrectHeight = $("#" + this.downFrameId).height();
            var downFrameTargetHeight = intHeight - 92;

            paramMoveAction01 = {
                runInterval: null,
                ctrlObjcetId: [this.downFrameId, this.downFrameOpenIconId, this.downFrameCloseIconId],
                ctrlObjcetAttb: ["height", "bottom", "bottom"],
                currectValue: [downFrameCurrectHeight, downFrameCurrectHeight, downFrameCurrectHeight],
                targetValue: [downFrameTargetHeight, downFrameTargetHeight, downFrameTargetHeight],
                stepValue: [40, 40, 40],
                isRunning: true,
                endSetHide: [false, false, false]
            };
            paramMoveAction01.runInterval = setInterval(runMoveAction01, 20);
        }
        this.downOpenLevel = 2;
        $("#" + this.downFrameId).show();
        this.isdownOpen = true;
        $("#mis_down_iconopen").css("opacity", "0.2");
        $("#mis_down_iconclose").css("opacity", "1");
        resizeMisFrame();
    },
    CloseFullDownFrame: function () {
        if (!paramMoveAction01.isRunning) {
            if (this.downOpenLevel === 0) {
                return;
            }
            paramMoveAction01.isRunning = true;
            var intHeight = gs.RWDHelper.getWindowHeight();
            var downFrameCurrectHeight = $("#" + this.downFrameId).height();
            var downFrameTargetHeight = 0 - downFrameCurrectHeight;

            paramMoveAction01 = {
                runInterval: null,
                ctrlObjcetId: [this.downFrameId, this.downFrameOpenIconId, this.downFrameCloseIconId],
                ctrlObjcetAttb: ["bottom", "bottom", "bottom"],
                currectValue: [downFrameCurrectHeight, downFrameCurrectHeight, downFrameCurrectHeight],
                targetValue: [downFrameTargetHeight, downFrameTargetHeight, downFrameTargetHeight],
                stepValue: [-40, -40, -40],
                isRunning: true,
                endSetHide: [true, false, false]
            };
            paramMoveAction01.runInterval = setInterval(runMoveAction01, 20);
        }
        this.downOpenLevel = 0;
        $("#" + this.downFrameId).show();
        this.isdownOpen = false;
        $("#mis_down_iconopen").css("opacity", "1");
        $("#mis_down_iconclose").css("opacity", "0.2");
        resizeMisFrame();
    },
    OpenDownonethreeFrame: function () {

        if (this.downOpenLevel === 2) {
            this.CloseDownFrame();
        }
        if (this.downOpenLevel === 0) {
            this.OpenDownFrame();
        }

        resizeMisFrame();
    }

};

var useMisFunctonFrame = new misFunctonFrame();
$(
    function () {
        useMisFunctonFrame.downBtnHeight = $("#" + useMisFunctonFrame.downFrameOpenIconId).height();
    }
);

var extenForMis_js_init = true;
///
function resizeMisFrame() {
    try {
        var deviceHeight = gs.RWDHelper.getWindowHeight();
        var intHeight = deviceHeight;
        var intWidth = gs.RWDHelper.getWindowWidth();
        var leftFrameHeight = intHeight - $("#" + useMisFunctonFrame.leftFrameId).position().top;
        var leftFrameWidth = 450;
        if (intWidth < 480) {
            leftFrameWidth = "100%";
        }
        var leftFrameIconTop = (intHeight / 2) - 18;

        $("#" + useMisFunctonFrame.leftFrameId).css("height", leftFrameHeight);
        $("#" + useMisFunctonFrame.leftFrameId).css("width", leftFrameWidth);

        if (useMisFunctonFrame.isleftOpen) {
            $("#" + useMisFunctonFrame.leftFrameId).css("left", 0);
            $("#" + useMisFunctonFrame.leftFrameIconId).css("left", leftFrameWidth);

        } else {
            $("#" + useMisFunctonFrame.leftFrameId).css("left", (0 - leftFrameWidth));
            $("#" + useMisFunctonFrame.leftFrameIconId).css("left", 0);
        }
        $("#" + useMisFunctonFrame.leftFrameIconId).css("top", leftFrameIconTop);

        //var downFrameWidth = intWidth - leftFrameWidth - 1;
        var downFrameWidth = intWidth;
        var downFrameHeight;
        if (intWidth <= 979) {
            downFrameHeight = intHeight / 3;
            if (useMisFunctonFrame.downOpenLevel === 1) {
                //downFrameHeight = $("#mis_down_content").height() - 92;
            } else if (useMisFunctonFrame.downOpenLevel === 2) {
                downFrameHeight = intHeight - 92;
            }
        } else {
            downFrameHeight = intHeight / 3;
            if (useMisFunctonFrame.downOpenLevel === 2) {
                downFrameHeight = intHeight - 92;
            }
        }
        var downFrameIconRight1 = downFrameWidth / 2;
        var downFrameIconRight2 = downFrameIconRight1 - 36;

        var offset = $("body").height() - deviceHeight;

        $("#" + useMisFunctonFrame.downFrameId).css("width", downFrameWidth);
        $("#" + useMisFunctonFrame.downFrameId).css("height", downFrameHeight);
        $("#" + useMisFunctonFrame.downFrameId).css("bottom", offset);

        if (useMisFunctonFrame.isdownOpen) {
            $("#" + useMisFunctonFrame.downFrameId).css("bottom", offset);
            $("#" + useMisFunctonFrame.downFrameOpenIconId).css("bottom", downFrameHeight + offset);
            $("#" + useMisFunctonFrame.downFrameCloseIconId).css("bottom", downFrameHeight + offset);

        } else {
            $("#" + useMisFunctonFrame.downFrameId).css("bottom", (0 - downFrameHeight + offset));
            $("#" + useMisFunctonFrame.downFrameOpenIconId).css("bottom", offset);
            $("#" + useMisFunctonFrame.downFrameCloseIconId).css("bottom", offset);
        }
        $("#" + useMisFunctonFrame.downFrameOpenIconId).css("right", downFrameIconRight1);
        $("#" + useMisFunctonFrame.downFrameCloseIconId).css("right", downFrameIconRight2);

        if (extenForMis_js_init) {
            extenForMis_js_init = false;
            getObject(useMisFunctonFrame.leftiFrameId).src = "";
        }

    } catch (err) {
        console.error(err);
    }
}

function misLeftIconClick(myObj) {
    if (useMisFunctonFrame.isleftOpen) {
        useMisFunctonFrame.CloseLeftFrame();
        myObj.className = "open";
        if ($(window).width() <= 979) {
            $("#mis_down_icon2").css("display", "");
            $("#mis_down_icon1").css("display", "");
        }
    } else {
        useMisFunctonFrame.OpenLeftFrame();
        myObj.className = "close";
    }
}

function misDownOpenIconClick(myObj) {
    useMisFunctonFrame.OpenDownFrame();
}

function misDownCloseIconClick(myObj) {
    useMisFunctonFrame.CloseDownFrame();
}

function openMisFunction(functionKeyWord) {
    switch (functionKeyWord) {
        case "MarkList":
            useMisFunctonFrame.OpenFunInLeft("Map/marklist.htm");
            break;
        case "Bottom":
            useMisFunctonFrame.OpenFunInDown("Map/bottom.htm", false);
            break;


        default:

            break;
    }
}

///
var paramMoveAction01 = {
    runInterval: null,
    ctrlObjcetId: [],
    ctrlObjcetAttb: [],
    currectValue: [],
    targetValue: [],
    stepValue: [],
    isRunning: false,
    endSetHide: []
};
function runMoveAction01() {
    var isTotalEnd = true;

    for (var i = 0; i < paramMoveAction01.ctrlObjcetId.length; i++) {
        var isEnd = false;
        paramMoveAction01.currectValue[i] = paramMoveAction01.currectValue[i] + paramMoveAction01.stepValue[i];
        if (paramMoveAction01.stepValue[i] > 0) {
            if (paramMoveAction01.currectValue[i] >= paramMoveAction01.targetValue[i]) {
                paramMoveAction01.currectValue[i] = paramMoveAction01.targetValue[i];
                isEnd = true;
            } else {
                isTotalEnd = false;
            }
        } else {
            if (paramMoveAction01.currectValue[i] <= paramMoveAction01.targetValue[i]) {
                paramMoveAction01.currectValue[i] = paramMoveAction01.targetValue[i];
                isEnd = true;
            } else {
                isTotalEnd = false;
            }
        }
        $("#" + paramMoveAction01.ctrlObjcetId[i]).css(paramMoveAction01.ctrlObjcetAttb[i], paramMoveAction01.currectValue[i]);

        if (isEnd) {
            if (paramMoveAction01.endSetHide[i]) {
                $("#" + paramMoveAction01.ctrlObjcetId[i]).hide();
            }
        }
    }

    if (isTotalEnd) {
        clearInterval(paramMoveAction01.runInterval);
        //resizeMisFrame();
        paramMoveAction01.isRunning = false;
    }
}

// 取得數值並轉為int，若傳入的為非數值則傳回零
// 需引用 String.prototype.replaceAll
function JsGetNumeric_Int(sValue) {
    var RtValue = 0;
    var temp = new String(sValue);

    try {
        temp = temp.replace(/\,/g, "");
        temp = parseInt(temp);
        if (isNaN(temp)) {
            RtValue = 0;
        } else {
            RtValue = temp;
        }
    } catch (err) {
        RtValue = 0;
    }

    return RtValue;
}

// get the layer object called "name"
function getObject(id) {
    var theObj = document.getElementById(id);
    if (theObj !== null) {
        return theObj;
    } else {
        return null;
    }
}