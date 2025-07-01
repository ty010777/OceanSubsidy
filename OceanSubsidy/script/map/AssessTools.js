
//轉換成民國曆
function toLocalDate(source, clientside_arguments) {
    var DateValue = source._textbox.get_Value();
    var year = parseInt(DateValue.substr(0, 4)) - 1911;
    source._textbox.set_Value(year + DateValue.substr(4, 6));
}

function toTWDate(theDate) {
    if (typeof theDate === 'string') {
        theDate = new Date(theDate);
    }
    return (theDate.getFullYear() - 1911) + '/' + (theDate.getMonth() + 1) + '/' + theDate.getDate();
}

function toADDate(theTWDate, delimiter = '/') {
    var partDate = theTWDate.split(delimiter);
    if (partDate.length < 3) return null;

    return new Date(parseInt(partDate[0]) + 1911, parseInt(partDate[1]) - 1, parseInt(partDate[2]));
}

function toDateString(theDate, delimiter = '/') {
    if (typeof theDate === 'string') {
        theDate = new Date(theDate);
    }
    if (typeof theDate === 'number') {
        theDate = new Date(theDate);
    }

    return (theDate.getFullYear()) + delimiter + (theDate.getMonth() + 1) + delimiter + theDate.getDate();
}

// JScript 檔
//PROTOTYPE SETTING

//字串分割，取 x 值之後所有值
String.prototype.mid = function (x) {
    var str = this;
    var beforeStr = str.substring(0, x);
    var newStr = str.replace(beforeStr, "");

    return newStr;
}

//清除前後空白
String.prototype.Trim = function () {
    return this.replace(/(^\s*)|(\s*$)/g, "");
}

//原字串向右靠，字串長度不足length時，則補charcode
String.prototype.PadLeft = function (Length, charCode) {
    var thisValue = this;

    while (thisValue.length < Length) {
        thisValue = charCode + thisValue;
    }
    return thisValue;
}

String.prototype.PadRight = function (Length, charCode) {
    var thisValue = this;
    while (thisValue.length < Length) {
        thisValue = thisValue + charCode;
    }
    return thisValue;
}
//取代全部
String.prototype.replaceAll = function (fromValue, toValue) {
    //gi 不分大小寫
    var myRe = new RegExp(fromValue, "gi");

    return this.replace(myRe, toValue);
}

//顯示訊息
String.prototype.alert = function () {
    alert(this);
}

//金額欄位，三位一撇
String.prototype.toMoney = function (fractionNum) {
    var thisValue = this.replaceAll(",", "");
    if (isNaN(parseFloat(thisValue))) {
        thisValue = "0";
    }
    else {
        thisValue = parseFloat(thisValue, 10).toString();
    }


    var fraction = "";
    var newValue = "";
    if (thisValue.indexOf(".") > 0) {
        fraction = thisValue.substring(thisValue.indexOf(".") + 1, thisValue.length);
        thisValue = thisValue.substring(0, thisValue.indexOf("."));
    }

    var minus = "";
    if (thisValue.indexOf("-") > -1) {
        minus = "-";
        thisValue = thisValue.replaceAll("-", "");
    }

    if (isFinite(thisValue)) {
        var j = 0;
        for (i = thisValue.length - 1; i >= 0; i--) {
            j++;
            newValue = thisValue.charAt(i) + newValue;
            if ((j % 3 == 0) && (i != 0)) {
                newValue = "," + newValue;
            }
        }
        thisValue = newValue;
    }
    thisValue = minus + thisValue;
    if (fraction != "") {
        if (fractionNum) {
            fraction = fraction.substr(0, fractionNum);
        }
        thisValue = thisValue + "." + fraction;
    }
    return thisValue;
}
//轉成 Unicode
String.prototype.toEscape = function () {
    return escape(this);
}

//轉成數值
String.prototype.toNum = function () {
    var val = this.replaceAll(",", "");

    if (isNaN(parseFloat(val))) {
        if(arguments.length > 0)
            return arguments[0];
    }
    else {
        return parseFloat(val);
    }
}

String.prototype.toUnEscape = function () {
    return unescape(this);
}

String.prototype.STUFF = function (x, str) {
    var theValue = this;
    if (theValue.length > x) {
        theValue = theValue.substring(0, x) + str + theValue.mid(x);
    }
    else {
        theValue = theValue.PadRight(x, 0) + ".0";
    }
    return theValue;
}

String.prototype.LastSTUFF = function (x, str) {
    var theValue = this;
    var newValue = '';

    if (theValue.length >= x) {
        newValue = theValue.substring(0, theValue.length - x) + str + theValue.substr(theValue.length - x, x);
    }
    else {
        while (theValue.length < x) {
            theValue = '0' + theValue;
        }
        newValue = str + theValue;
    }

    return newValue;
}

//清除開頭是 0
function clearZero(val) {
    var tmpVal = val;
    var i = 0;
    var baseNum = 200;
    while (parseInt(tmpVal.substring(0, 1)) == 0) {
        tmpVal = tmpVal.substring(1, tmpVal.length);
        i++;
        if (i >= baseNum) {
            break;
        }
    }
    return tmpVal;
}

//日期比較
Date.prototype.Compare = function (compDate) {
    var thisValue = this;
    return this.getTime() - compDate.getTime();
}


//判斷是否為數值
function isNumeric(val) {
    var value = val.replaceAll(",", "");

    if (isNaN(parseFloat(value))) {
        return false;
    }
    else {
        return true;
    }
}

//預覽列印
function previewScreen(block) {
    var value = block.innerHTML;
    var printPage = window.open("", "printPage", "");
    printPage.document.open();
    printPage.document.write("<OBJECT classid='CLSID:8856F961-340A-11D0-A96B-00C04FD705A2' height=0 id=wc name=wc width=0></OBJECT>");
    printPage.document.write("<HTML><head></head><BODY onload='javascript:wc.execwb(7,1);window.close()'>");
    printPage.document.write("<PRE>");
    printPage.document.write(value);
    printPage.document.write("</PRE>");
    printPage.document.close("</BODY></HTML>");
}

// 複寫properies指定Object的properties到現在的Object
function gs_overwriteProperties(curObj, fromObj) {
    for (let key in fromObj) {
        if (fromObj[key] === undefined) continue;
        curObj[key] = fromObj[key];
    }
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
                $($target[0]).append("<option value='" + data[i].key + "'>" + data[i].value + "</option>");
            }
            if (finishFunc) finishFunc();
        }
    });
}

function fillByGenFunc(target, dataUrl, genFunc, finishFunc) {
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
            for (var i = 0; i < data.length; i++) {
                $($target[0]).append(this.genFunc(data[i]));
            }
            if (this.finishFunc) this.finishFunc();
        }.bind(
            {
                genFunc: genFunc,
                finishFunc: finishFunc
            }
        )
    });
}

function roundTo(num, digitNum) {
    var scaleDigit = Math.pow(10, digitNum);
    return Math.round(num * scaleDigit) / scaleDigit;

}

function showItTemperal(obj) {
    $(obj).show(1000).animate({ "doNothing": '' }, 2000).hide(1000);
}