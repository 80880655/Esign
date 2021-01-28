var sourceUrl = '';
var topStr = top.location.toString();
var strs = topStr.split("/");
if (topStr.toLowerCase().indexOf("localhost:") > 0 || topStr.toLowerCase().indexOf("127.0.0.1:") > 0) {
    sourceUrl = strs[0] + "//" + strs[2];
}
else {
    sourceUrl = strs[0] + "//" + strs[2] + "/" + strs[3];
}

var AppName = "MES";
var DXagent = navigator.userAgent.toLowerCase();
var DXopera = (DXagent.indexOf("opera") > -1);

function AjaxCallFun(dllName, className, methodName, methodParameter) {   //ajax返回函數值 by ZH
    var parameter = escape(methodParameter);
    dllName = escape(dllName);
    className = escape(className);
    methodName = escape(methodName);
    var result = '';
    $.ajax({
        type: "post",
        url: sourceUrl + "/AjaxCallMethod.ashx?dllName=" + dllName + "&className=" + className + "&methodName=" + methodName + "&methodParameter=" + parameter,
        cache: false,
        async: false,
        success: function (msg) {
            result = msg;
        },
        Error:function(msg)
        {
          alert(msg);
        }

    });
    return result;
}

function AjaxCallFunOne(dllName, className, methodName, methodParameter) {   //ajax返回函數值 by ZH
    var parameter = escape(methodParameter);
    dllName = escape(dllName);
    className = escape(className);
    methodName = escape(methodName);
    var result = '';
    $.ajax({
        type: "post",
        url: sourceUrl + "/AjaxCallMethodOne.ashx?dllName=" + dllName + "&className=" + className + "&methodName=" + methodName + "&methodParameter=" + parameter,
        cache: false,
        async: false,
        success: function (msg) {
            result = msg;
        },
        Error: function (msg) {
            alert(msg);
        }

    });
    return result;
}

function GetChildrenNodes(element, arr, gridName) {
    var els = element.childNodes;
    if (els.length == 0)
        return;
    for (var i = 0; i < els.length; i++) {
        var flag = 1;
        if (gridName.length > 0) {
            for (var j = 0; j < gridName.length; j++) {
                if (gridName[j] != "" && els[i].id == gridName[j]) {
                    flag = 0;
                    break;
                }
            }
        }
        if (flag == 0) {
            continue;
        }
        arr.push(els[i]);
        this.GetChildrenNodes(els[i], arr,gridName);
    }
}

function LoadingPattern() {
    var imgUrl = sourceUrl + "/Images/PLoading.gif";
    $("<div class='halfalpha' style='margin:0;padding:0;font-size:22px'><center><img src='" + imgUrl + "'></img></center></div>").css({ "position": "absolute", "z-index": "100000", "left": $(window).width() / 2 - 20, "top": $(window).height() / 2 + $(window).scrollTop(), "background-color": "#B9D8FF", "opacity": "0.9" }).appendTo(document.body);
}

var DXopera9 = (DXagent.indexOf("opera/9") > -1);
var DXsafari = DXagent.indexOf("safari") > -1;
var DXie = (DXagent.indexOf("msie") > -1 && !DXopera);
var DXIE55 = (DXagent.indexOf("5.5") > -1 && DXie);
var DXns = (DXagent.indexOf("mozilla") > -1 || DXagent.indexOf("netscape") > -1 || DXagent.indexOf("firefox") > -1) && !DXsafari && !DXie && !DXopera;
var DXDefaultThemeCookieName = "ASPxCurrentTheme";


function fixPng(element) {
    if (/MSIE (5\.5|6).+Win/.test(navigator.userAgent)) {
        if (element.tagName == 'IMG' && /\.png$/.test(element.src)) {
            var src = element.src;
            element.src = './Images/GIF/Empty.gif';
            element.runtimeStyle.filter = "progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + src + "')";
        }
    }
}

function $get(element, win) {
    win = win || window;
    return win.document.getElementById(element);
}

function ResizeIFrame(win) {
    var content = $get("content", win);
    if (DXIsObjExists(content)) {
        var header = $get("header", win);
        var footer = $get("footer", win);
        content.height = DXGetDocumentClientHeight(win);
        content.width = DXGetDocumentClientWidth(win);
        if (DXIsObjExists(header)) {
            content.height -= header.offsetHeight;
        } else {
            content.height = 400;
        }
        if (DXIsObjExists(footer))
            content.height -= footer.offsetHeight;
        content.style.height = Math.max(content.height, 0) + "px";
        content.style.width = Math.max(content.width, 0) + "px";
        try {
            intH = $(".gridBodyDiv").parent().outerHeight();
            intW = $(".gridBodyDiv").parent().outerWidth();
            if (intH > 0 && intW > 0) {
                $(".gridBodyDiv", "#content").attr("style", "height:" + (parseInt(intH) - 42) + "px;width:" + (parseInt(intW) - 2) + "px;overflow:auto;");
            }
        } catch (e) { 
        }
    }
}
DXattachEventToElement(window, "resize", DXWindowOnResize);
function DXWindowOnResize(evt) {
    ResizeIFrame(window);
}
DXattachEventToElement(window, "load", DXWindowOnLoad);
function DXWindowOnLoad(evt) {
    ResizeIFrame(window);
}
function DXattachEventToElement(element, eventName, func) {
    if (DXns || DXsafari)
        element.addEventListener(eventName, func, true);
    else {
        if (eventName.toLowerCase().indexOf("on") != 0)
            eventName = "on" + eventName;
        element.attachEvent(eventName, func);
    }
}
function DXGetDocumentClientHeight(win) {
    if (DXsafari)
        return win.innerHeight;
    if (DXIE55 || DXopera || win.document.documentElement.clientHeight == 0)
        return win.document.body.clientHeight;
    return win.document.documentElement.clientHeight;
}
function DXGetDocumentClientWidth(win) {
    if (DXsafari)
        return win.innerWidth;
    if (DXIE55 || DXopera || win.document.documentElement.clientWidth == 0)
        return win.document.body.clientWidth;
    return win.document.documentElement.clientWidth;
}
function DXIsObjExists(obj) {
    return (typeof (obj) != "undefined") && (obj != null);
}
function GetRequest() {
    var url = location.search;
    var theRequest = new Object();
    if (url.indexOf("?") != -1) {
        var str = url.substr(1);
        strs = str.split("&");
        for (var i = 0; i < strs.length; i++) {
            theRequest[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
        }
    }
    return theRequest;
}
function OpenForm(title, url, width, height, closeUpEvent) {
    if (parent.window != window) {
        parent.ShowForm(title, url, width, height, closeUpEvent);
    }
    else {
        width = width || Math.max(DXGetDocumentClientWidth(window) - 50, 100);
        height = height || Math.max(DXGetDocumentClientHeight(window) - 100, 100);
        var feature = "width=" + width + ",height=" + height + ",menubar=no,toolbar=no,location=no,scrollbars=yes,resizable=yes,status=no,modal=no,center=yes";
        window.open(url, "pcForm", feature);
    }
}
function ToolBarMenuItemClick(item, clientName) { }

//按回車鍵設置下個焦點(一般應用在onkeydown事件中)
function JsSetFocus(controlID) {
    if (event.keyCode == 13) {
        document.getElementById(controlID).focus();
    }
}

function QueryString(fieldName) {
    var urlString = document.location.search;
    if (urlString != null) {
        var typeQu = fieldName + "=";
        var urlEnd = urlString.indexOf(typeQu);
        if (urlEnd != -1) {
            var paramsUrl = urlString.substring(urlEnd + typeQu.length);
            var isEnd = paramsUrl.indexOf('&');
            if (isEnd != -1) {
                return paramsUrl.substring(0, isEnd);
            }
            else {
                return paramsUrl;
            }
        }
        else {
            return null;
        }
    } else {
        return null;
    }
}

//Js控制小數點位數函數
//需轉換的數字--n
//需保留的小數位數--dotn
function JsControlDot(n, dotn) {
    var sReturn;
    var bit = dotn;
    bit++;
    n = n.toString();
    var point = n.indexOf('.');
    if (point == -1) {
        sReturn = n;
    }
    else {
        if (n.length > point + bit) {
            if (parseInt(n.substring(point + bit, point + bit + 1)) > 4) {
                sReturn = n.substring(0, point) + "." + (parseInt(n.substring(point + 1, point + bit)) + 1);
            }
            else {
                sReturn = n.substring(0, point) + n.substring(point, point + bit);
            }
        }
        else {
            sReturn = n;
        }
    }
    //最后再進行補零
    var dot = sReturn.indexOf('.');
    if (dot == -1) {
        sReturn = sReturn + ".";
        for (var i = 0; i < dotn; i++) {
            sReturn = sReturn + "0";
        }
    }
    else {
        var vplit = sReturn.split('.');
        var tmp = vplit[1].toString();
        var pCount = parseInt(dotn) - parseInt(tmp.length);
        for (var j = 0; j < pCount; j++) {
            sReturn = sReturn + "0";
        }
    }
    return sReturn;
}

function Dragable(name) {
    try {
        $("#_DialogDiv_" + name).jquerymove();
    } catch (e) {
    }
}