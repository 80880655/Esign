//modify by zhanghe at 2012 4 19 . 修改為絕對路徑
//var sourceUrl = '';
//var strs = top.location.toString().split("/");
//if (strs[3].indexOf("aspx") > 0) {
//    sourceUrl = strs[0] + "//" + strs[2];
//} else {
//    sourceUrl = strs[0] + "//" + strs[2] + "/" + strs[3];
//}

var sourceUrl = '';
var topStr = top.location.toString();
var strs = topStr.split("/");
if (topStr.toLowerCase().indexOf("localhost:") > 0 || topStr.toLowerCase().indexOf("127.0.0.1:") > 0) {
    sourceUrl = strs[0] + "//" + strs[2];
}
else {
    sourceUrl = strs[0] + "//" + strs[2] + "/" + strs[3];
}

var CONTEXTPATH = sourceUrl + '/Images/ui.zdialog/'; //图片路径配置

//var CONTEXTPATH = 'Images/ui.zdialog/'; //图片路径配置



var isIE = navigator.userAgent.toLowerCase().indexOf("msie") != -1;
var isGecko = navigator.userAgent.toLowerCase().indexOf("gecko") != -1;

function ___(ele) {
    if (typeof (ele) == 'string') {
        ele = document.getElementById(ele)
        if (!ele) {
            return null;
        }
    }
    if (ele) {
        Core.attachMethod(ele);
    }
    return ele;
}
function $T(tagName, ele) {
    ele = ___(ele);
    ele = ele || document;
    var ts = ele.getElementsByTagName(tagName); //此处返回的不是数组
    var arr = [];
    var len = ts.length;
    for (var i = 0; i < len; i++) {
        arr.push(___(ts[i]));
    }
    return arr;
}
function stopEvent(event) {//阻止一切事件执行,包括浏览器默认的事件
    event = window.event || event;
    if (!event) {
        return;
    }
    if (isGecko) {
        event.preventDefault();
        event.stopPropagation();
    }
    event.cancelBubble = true
    event.returnValue = false;
}

Array.prototype.removeItem = function (s) {
    for (var i = 0; i < this.length; i++) {
        if (s == this[i]) {
            this.splice(i, 1);
        }
    }
}

if (window.HTMLElement) { //给FF添加IE专有的属性和方法
    HTMLElement.prototype.__defineGetter__("parentElement", function () {
        if (this.parentNode == this.ownerDocument) return null;
        return this.parentNode;
    });
    HTMLElement.prototype.__defineSetter__("outerHTML", function (sHTML) {
        var r = this.ownerDocument.createRange();
        r.setStartBefore(this);
        var df = r.createContextualFragment(sHTML);
        this.parentNode.replaceChild(df, this);
        return sHTML;
    });
    HTMLElement.prototype.__defineGetter__("outerHTML", function () {
        var attr;
        var attrs = this.attributes;
        var str = "<" + this.tagName;
        for (var i = 0; i < attrs.length; i++) {
            attr = attrs[i];
            if (attr.specified)
                str += " " + attr.name + '="' + attr.value + '"';
        }
        if (!this.canHaveChildren)
            return str + ">";
        return str + ">" + this.innerHTML + "</" + this.tagName + ">";
    });
    HTMLElement.prototype.__defineSetter__("innerText", function (sText) {
        var parsedText = document.createTextNode(sText);
        this.innerHTML = parsedText;
        return parsedText;
    });
    HTMLElement.prototype.__defineGetter__("innerText", function () {
        var r = this.ownerDocument.createRange();
        r.selectNodeContents(this);
        return r.toString();
    });
}

var $E = {};
$E.getTopLevelWindow = function () {
    var pw = window;
    while (pw != pw.parent) {
        pw = pw.parent;
    }
    return pw;
}
$E.hide = function (ele) {
    ele = ele || this;
    ele = ___(ele);
    ele.style.display = 'none';
}
$E.show = function (ele) {
    ele = ele || this;
    ele = ___(ele);
    ele.style.display = '';
}
var Core = {};
Core.attachMethod = function (ele) {
    if (!ele || ele["$A"]) {
        return;
    }
    if (ele.nodeType == 9) {
        return;
    }
    var win;
    try {
        if (isGecko) {
            win = ele.ownerDocument.defaultView;
        } else {
            win = ele.ownerDocument.parentWindow;
        }
        for (var prop in $E) {
            ele[prop] = win.$E[prop];
        }
    } catch (ex) {
        //alert("Core.attachMethod:"+ele)//有些对象不能附加属性，如flash
    }
}

function Dialog(strID) {
    if (!strID) {
        alert("错误的Dialog ID！");
        return;
    }
    this.ID = strID;
    this.isModal = true;
    this.Width = 400;
    this.Height = 300;
    this.Top = 0;
    this.Left = 0;
    this.ParentWindow = null;
    this.onLoad = null;
    this.Window = null;

    this.Title = "";
    this.URL = null;
    this.DialogArguments = {};
    this.WindowFlag = false;
    this.Message = null;
    this.MessageTitle = null;
    this.ShowMessageRow = false;
    this.ShowButtonRow = true;
    this.Icon = null;
    this.bgiframeID = null;
    this.bgdivID = null;
}

Dialog._Array = [];

Dialog.prototype.showWindow = function () {
    if (isIE) {
        this.ParentWindow.showModalessDialog(this.URL, this.DialogArguments, "dialogWidth:" + this.Width + ";dialogHeight:" + this.Height + ";help:no;scroll:no;status:no");
    }
    if (isGecko) {
        var sOption = "location=no,menubar=no,status=no;toolbar=no,dependent=yes,dialog=yes,minimizable=no,modal=yes,alwaysRaised=yes,resizable=no";
        this.Window = this.ParentWindow.open('', this.URL, sOption, true);
        var w = this.Window;
        if (!w) {
            alert("发现弹出窗口被阻止，请更改浏览器设置，以便正常使用本功能!");
            return;
        }
        w.moveTo(this.Left, this.Top);
        w.resizeTo(this.Width, this.Height + 30);
        w.focus();
        w.location.href = this.URL;
        w.Parent = this.ParentWindow;
        w.dialogArguments = this.DialogArguments;
    }
}
/* zhout added 2009-08-05 */
Dialog.prototype.getIFrame = function () {
    //if (parent)
    return top.frames("_DialogFrame_" + this.ID);
    //else
    //	return frames("_DialogFrame_" + this.ID);
}
//----------------------------------------------------------
// 根据 URL 获取到 门户 tabs 的frame
//----------------------------------------------------------
Dialog.prototype.getOpenerFrame = function (urlMatches) {
    var framelist, i, j;
    if (parent)
        framelist = parent.frames;
    else
        framelist = frames;
    var matched;
    for (i = 0; i < framelist.length; i++) {
        matched = true;
        if (framelist[i].location != null) {
            for (j = 0; j < urlMatches.length; j++) {
                matched = matched && (framelist[i].location.toString().indexOf(urlMatches[j]) >= 0);
            }
        }
        if (matched)
            return framelist[i];
    }
    return null;
}
//-------------------------------------------------
// 修改标题和描述 Added by zhout 
//-------------------------------------------------
Dialog.prototype.setDialogMessage = function (title, message) {
    var pw = $E.getTopLevelWindow();
    if (this.MessageTitle) {
        pw.document.getElementById("_MessageTitle_" + this.ID).innerHTML = title;
    }
    if (this.Message) {
        pw.document.getElementById("_Message_" + this.ID).innerHTML = message;
    }
}

Dialog.prototype.dragable = function (name) {
    var pw = $E.getTopLevelWindow();
    pw.Dragable(name);
}

Dialog.prototype.show = function () {
    var pw = $E.getTopLevelWindow();
    var doc = pw.document;
    var cw = doc.compatMode == "BackCompat" ? doc.body.clientWidth : doc.documentElement.clientWidth;
    var ch = doc.compatMode == "BackCompat" ? doc.body.clientHeight : doc.documentElement.clientHeight; //必须考虑文本框处于页面边缘处，控件显示不全的问题
    var sl = Math.max(doc.documentElement.scrollLeft, doc.body.scrollLeft);
    var st = Math.max(doc.documentElement.scrollTop, doc.body.scrollTop); //考虑滚动的情况
    var sw = Math.max(doc.documentElement.scrollWidth, doc.body.scrollWidth);
    var sh = Math.max(doc.documentElement.scrollHeight, doc.body.scrollHeight); //考虑滚动的情况
    sw = Math.max(sw, cw);
    sh = Math.max(sh, ch);
    //	alert("\n"+cw+"\n"+ch+"\n"+sw+"\n"+sh)

    if (!this.ParentWindow) {
        this.ParentWindow = window;
    }
    this.DialogArguments._DialogInstance = this;
    this.DialogArguments.ID = this.ID;

    if (!this.Height) {
        this.Height = this.Width / 2;
    }

    if (this.Top == 0) {
        this.Top = (ch - this.Height - 50) / 2 + st - 8;
    }
    if (this.Left == 0) {
        this.Left = (cw - this.Width - 12) / 2 + sl;
    }
    if (this.ShowButtonRow) {//按钮行高36
        this.Top -= 18;
    }
    if (this.WindowFlag) {
        this.showWindow();
        return;
    }
    var arr = [];
    arr.push("<table style='-moz-user-select:none;' oncontextmenu='stopEvent(event);' onselectstart='stopEvent(event);' border='0' cellpadding='0' cellspacing='0' width='" + (parseInt(this.Width) + 36) + "'>");
    arr.push("  <tr style='cursor:;'>");
    arr.push("    <td width='13' height='33' style=\"background-image:url(" + CONTEXTPATH + "dialog_lt.png) !important;background-image: none;filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + CONTEXTPATH + "dialog_lt.png', sizingMethod='crop');\"><div style='width:13px;'></div></td>");
    arr.push("    <td height='33' style=\"background-image:url(" + CONTEXTPATH + "dialog_ct.png) !important;background-image: none;filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + CONTEXTPATH + "dialog_ct.png', sizingMethod='crop');\"><div style=\"float:left;font-weight:bold; color:#FFFFFF; padding:9px 0 0 4px;\">" + this.Title + "</div>");
    arr.push("      <div id=\"_DialogCloseBtn\" style=\"position: relative;cursor:pointer; float:right; margin:5px 0 0; _margin:4px 0 0;height:17px; width:28px; background-image:url(" + CONTEXTPATH + "dialog_closebtn.gif)\" onMouseOver=\"this.style.backgroundImage='url(" + CONTEXTPATH + "dialog_closebtn_over.gif)'\" onMouseOut=\"this.style.backgroundImage='url(" + CONTEXTPATH + "dialog_closebtn.gif)'\" drag='false' onClick=\"Dialog.getInstance('" + this.ID + "').closeEx();\"></div></td>");
    arr.push("    <td width='13' height='33' style=\"background-image:url(" + CONTEXTPATH + "dialog_rt.png) !important;background-image: none;filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + CONTEXTPATH + "dialog_rt.png', sizingMethod='crop');\"><div style=\"width:13px;\"></div></td>");
    arr.push("  </tr>");
    arr.push("  <tr drag='false'><td width='13' style=\"background-image:url(" + CONTEXTPATH + "dialog_mlm.png) !important;background-image: none;filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + CONTEXTPATH + "dialog_mlm.png', sizingMethod='crop');\"></td>");
    arr.push("    <td align='center' valign='top'>");
    arr.push("    <table width='100%' border='0' cellpadding='0' cellspacing='0' bgcolor='#FFFFFF'>");
    arr.push("        <tr id='_MessageRow_" + this.ID + "' style='display:none'>");
    arr.push("          <td height='50' valign='top'><table id='_MessageTable_" + this.ID + "' width='100%' border='0' cellspacing='0' cellpadding='8' style=\" background:#EAECE9 url(" + CONTEXTPATH + "dialog_bg.jpg) no-repeat right top;\">");
    arr.push("              <tr><td width='40' height='50' align='center'><img id='_MessageIcon_" + this.ID + "' src='" + CONTEXTPATH + "window.gif' width='32' height='32'> </td>");
    arr.push("                <td align='left' style='line-height:16px;'>");
    arr.push("                <h5 class='fb' id='_MessageTitle_" + this.ID + "'>&nbsp;</h5>");
    arr.push("                <div id='_Message_" + this.ID + "'>&nbsp;</div></td>");
    arr.push("              </tr></table></td></tr>");
    arr.push("        <tr><td align='center' valign='top'>");
    arr.push("          <iframe src='");
    //if(this.URL.indexOf(":")==-1){
    //	arr.push(CONTEXTPATH+this.URL);
    //}else{
    arr.push(this.URL);
    //}
    arr.push("' id='_DialogFrame_" + this.ID + "' allowTransparency='true'  width='" + (parseInt(this.Width) + 15) + "' height='" + this.Height + "' frameborder='0' style=\"background-color: #transparent; border:none;\"></iframe></td></tr>");
    arr.push("        <tr drag='false' id='_ButtonRow_" + this.ID + "'><td height='36'>");
    arr.push("            <div id='_DialogButtons_" + this.ID + "' style='text-align:right; border-top:#dadee5 1px solid; padding:8px 8px; background-color:#f6f6f6;'>");
    arr.push("           	<input id='_ButtonOK_" + this.ID + "' class='zdlgbtn' type='button' value='确 定' style='height:20px'>");
    arr.push("           	<input id='_ButtonCancel_" + this.ID + "' type='button' class='zdlgbtn' style='height:20px' onclick=\"Dialog.getInstance('" + this.ID + "').close();\" value='取 消'>");
    arr.push("            </div></td></tr>");
    arr.push("      </table></td>");
    arr.push("    <td width='13' style=\"background-image:url(" + CONTEXTPATH + "dialog_mrm.png) !important;background-image: none;filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(enable=false, src='" + CONTEXTPATH + "dialog_mrm.png', sizingMethod='crop');\"></td></tr>");
    arr.push("  <tr><td width='13' height='13' style=\"background-image:url(" + CONTEXTPATH + "dialog_lb.png) !important;background-image: none;filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(enable=false, src='" + CONTEXTPATH + "dialog_lb.png', sizingMethod='crop');\"></td>");
    arr.push("    <td style=\"background-image:url(" + CONTEXTPATH + "dialog_cb.png) !important;background-image: none;filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(enable=false, src='" + CONTEXTPATH + "dialog_cb.png', sizingMethod='crop');\"></td>");
    arr.push("    <td width='13' height='13' style=\"background-image:url(" + CONTEXTPATH + "dialog_rb.png) !important;background-image: none;filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(enable=false, src='" + CONTEXTPATH + "dialog_rb.png', sizingMethod='crop');\"></td>");
    arr.push("  </tr></table>");

    this.TopWindow = pw;

    var bgiframe = pw.document.getElementById("_DialogBGIframeABC"); // Added document. zhout
    var bgdiv = pw.document.getElementById("_DialogBGDivABC"); // Added document. zhout
    if (!bgiframe) {
        bgiframe = pw.document.createElement("iframe"); 	// Added document. zhout
        bgiframe.id = "_DialogBGIframeABC";
        $E.hide(bgiframe);
        pw.document.body.appendChild(bgiframe); // Added document. zhout
    }
    if (!bgdiv) {
        bgdiv = pw.document.createElement("div"); 	// Added document. zhout
        bgdiv.id = "_DialogBGDivABC";
        $E.hide(bgdiv);
        pw.document.body.appendChild(bgdiv); // Added document. zhout
    }

    var div = pw.document.getElementById("_DialogDiv_" + this.ID);
    if (!div) {
        div = pw.document.createElement("div");
        $E.hide(div);
        div.id = "_DialogDiv_" + this.ID;
        div.className = "dialogdiv";
        div.setAttribute("dragStart", "Dialog.dragStart");
        pw.document.body.appendChild(div);
    }

    this.DialogDiv = div;
    div.innerHTML = arr.join('\n');

    pw.document.getElementById("_DialogFrame_" + this.ID).DialogInstance = this;

    this.OKButton = pw.document.getElementById("_ButtonOK_" + this.ID);
    this.CancelButton = pw.document.getElementById("_ButtonCancel_" + this.ID);

    //显示标题图片
    if (this.ShowMessageRow) {
        $E.show(pw.document.getElementById("_MessageRow_" + this.ID));
        if (this.MessageTitle) {
            pw.document.getElementById("_MessageTitle_" + this.ID).innerHTML = this.MessageTitle;
        }
        if (this.Message) {
            pw.document.getElementById("_Message_" + this.ID).innerHTML = this.Message;
        }
    }

    //显示按钮栏
    if (!this.ShowButtonRow) {
        pw.document.getElementById("_ButtonRow_" + this.ID).style.display = "none";
    }
    if (this.CancelEvent) {
        this.CancelButton.onclick = this.CancelEvent;
    }
    if (this.OKEvent) {
        this.OKButton.onclick = this.OKEvent;
    }
    if (!this.AlertFlag) {
        if ($.browser.msie && $.browser.version == "6.0") {
            $E.show(bgiframe);
        }
        $E.show(bgdiv);
        this.bgiframeID = "_DialogBGIframeABC";
        this.bgdivID = "_DialogBGDivABC";
    } else {
        bgiframe = pw.document.getElementById("_AlertBGIframe");
        bgidiv = pw.document.getElementById("_AlertBGDiv");
        if (!bgiframe) {
            bgiframe = pw.document.createElement("iframe");
            bgiframe.id = "_AlertBGIframe";
            $E.hide(bgiframe);
            pw.$T("body")[0].appendChild(bgiframe);
            bgiframe.style.cssText = "background-color:#333;position:absolute;left:0px;top:0px;opacity:0;filter:alpha(opacity=0.5);width:" + sw + "px;height:" + sh + "px;z-index:100991";

            bgdiv = pw.document.createElement("div");
            bgidiv.id = "_AlertBGDiv";
            $E.hide(bgdiv);
            pw.$T("body")[0].appendChild(bgdiv);
            bgdiv.style.cssText = "background-color:#333;position:absolute;left:0px;top:0px;opacity:0;filter:alpha(opacity=0.5);width:" + sw + "px;height:" + sh + "px;z-index:100992";
        }
        $E.show(bgiframe);
        $E.show(bgdiv);
        this.bgiframeID = "_AlertBGIframe";
        this.bgidivID = "_AlertBGDiv";
    }
    this.DialogDiv.style.cssText = "position:absolute; display:block;z-index:" + (this.AlertFlag ? 100994 : 100990) + ";left:" + this.Left + "px;top:" + this.Top + "px";

    //判断当前窗口是否是对话框，如果是，则将其置在bgiframe之后
    if (!this.AlertFlag) {
        var win = window;
        var flag = false;
        while (win != win.parent) {//需要考虑父窗口是弹出窗口中的一个iframe的情况
            if (win._DialogInstance) {
                win._DialogInstance.DialogDiv.style.zIndex = 100959;
                flag = true;
                break;
            }
            win = win.parent;
        }
        if (!flag) {
            bgiframe.style.cssText = "background-color:#333;position:absolute;left:0px;top:0px;opacity:0;filter:alpha(opacity=0);width:" + sw + "px;height:" + sh + "px;z-index:100960";
            bgdiv.style.cssText = "background-color:#333;position:absolute;left:0px;top:0px;opacity:0;filter:alpha(opacity=12);width:" + sw + "px;height:" + sh + "px;z-index:100961";
        }
    }
    //this.OKButton.focus();
    //放入队列中，以便于ESC时正确关闭
    pw.Dialog._Array.push(this.ID);
    $("#_DialogCloseBtn", $E.getTopLevelWindow().document).mousedown(function (event) {   //找到最頂層頁面的元素
        event.stopPropagation();
    });
}

Dialog.prototype.addParam = function (paramName, paramValue) {
    this.DialogArguments[paramName] = paramValue;
}
/* zhout added 2009-08-05 */
Dialog.prototype.getParam = function (paramName) {
    return this.DialogArguments[paramName];
}
/* zhout added 2010-11-10*/
Dialog.prototype.closeEx = function () {
    if (this.CancelEvent) this.CancelButton.click();
    this.close();
}
Dialog.prototype.close = function () {
    if (this.WindowFlag) {
        this.ParentWindow.$D = null;
        this.ParentWindow.$DW = null;
        this.Window.opener = null;
        this.Window.close();
        this.Window = null;
    } else {
        //如果上级窗口是对话框，则将其置于bgiframe前		
        var pw = $E.getTopLevelWindow();
        var win = window;
        var flag = false;

        while (win != win.parent) {
            if (win._DialogInstance) {
                flag = true;
               // win._DialogInstance.DialogDiv.style.zIndex = 100960;
                win._DialogInstance.DialogDiv.style.zIndex = 110960;
                break;
            }
            win = win.parent;
        }
       if (this.AlertFlag) {
            $E.hide(pw.___("_AlertBGIframe"));
            $E.hide(pw.___("_AlertBGDiv"));
       }
        if (!flag && !this.AlertFlag) {//此处是为处理弹出窗口被关闭后iframe立即被重定向时背景层不消失的问题
            pw.eval('window._OpacityFunc = function(){var w = $E.getTopLevelWindow();$E.hide(w.___("_DialogBGIframeABC"));}');
            pw.eval('window._OpacityFuncDiv = function(){var w = $E.getTopLevelWindow();$E.hide(w.___("_DialogBGDivABC"));}');
            pw._OpacityFunc();
            pw._OpacityFuncDiv();
        }
        this.DialogDiv.outerHTML = "";
        pw.Dialog._Array.removeItem(this.ID);
    }
}

Dialog.prototype.addButton = function (id, txt, func) {
    var html = "<input id='_Button_" + this.ID + "_" + id + "' class='zdlgbtn' style='height:20px' type='button' value='" + txt + "'> ";
    var pw = $E.getTopLevelWindow();
    pw.document.getElementById("_DialogButtons_" + this.ID).$T("input")[0].getParent("a").insertAdjacentHTML("beforeBegin", html);
    pw.document.getElementById("_Button_" + this.ID + "_" + id).onclick = func;
}

Dialog.close = function (evt) {
    window.Args._DialogInstance.close();
}

Dialog.getInstance = function (id) {
    var pw = $E.getTopLevelWindow()
    var f = pw.___("_DialogFrame_" + id);
    if (!f) {
        return null;
    }
    return f.DialogInstance;
}

Dialog.AlertNo = 0;
Dialog.alert = function (msg, func, w, h) {
    var pw = $E.getTopLevelWindow()
    var diag = new Dialog("_DialogAlert" + Dialog.AlertNo++);
    diag.ParentWindow = pw;
    diag.Width = w ? w : 300;
    diag.Height = h ? h : 120;
    diag.Title = "系统提示";
    diag.URL = "javascript:void(0);";
    diag.AlertFlag = true;
    diag.CancelEvent = function () {
        diag.close();
        if (func) {
            func();
        }
    };
    diag.show();
    pw.___("_AlertBGIframe").style.display = "";
    $E.hide(pw.___("_ButtonOK_" + diag.ID));
    var win = pw.___("_DialogFrame_" + diag.ID).contentWindow;
    var doc = win.document;
    doc.open();
    doc.write("<body oncontextmenu='return false;' scrol='yes'></body>");
    var arr = [];
    arr.push("<table height='100%' border='0' align='center' cellpadding='10' cellspacing='0'>");
    arr.push("<tr><td align='right'><img id='Icon' src='" + CONTEXTPATH + "icon_alert.gif' width='34' height='34' align='absmiddle'></td>");
    arr.push("<td align='left' id='Message' style='font-size:9pt'>" + msg + "</td></tr></table>");
    var div = doc.createElement("div");
    div.innerHTML = arr.join('');
    doc.body.appendChild(div);
    doc.close();
    var h = Math.max(doc.documentElement.scrollHeight, doc.body.scrollHeight);
    var w = Math.max(doc.documentElement.scrollWidth, doc.body.scrollWidth);
    if (w > 300) {
        win.frameElement.width = w;
    }
    if (h > 120) {
        win.frameElement.height = h;
    }

    diag.CancelButton.value = "确 定";
    diag.CancelButton.focus();
    pw.___("_DialogButtons_" + diag.ID).style.textAlign = "center";
}

Dialog.confirm = function (msg, func1, func2, w, h) {
    var pw = $E.getTopLevelWindow()
    var diag = new Dialog("_DialogAlert" + Dialog.AlertNo++);
    diag.Width = w ? w : 300;
    diag.Height = h ? h : 120;
    diag.Title = "信息确认";
    diag.URL = "javascript:void(0);";
    diag.AlertFlag = true;
    diag.CancelEvent = function () {
        diag.close();
        if (func2) {
            func2();
        }
    };
    diag.OKEvent = function () {
        diag.close();
        if (func1) {
            func1();
        }
    };
    diag.show();
    pw.___("_AlertBGIframe").style.dispaly = "";
    var win = pw.___("_DialogFrame_" + diag.ID).contentWindow;
    var doc = win.document;
    doc.open();
    doc.write("<body oncontextmenu='return false;'></body>");
    var arr = [];
    arr.push("<table height='100%' border='0' align='center' cellpadding='10' cellspacing='0'>");
    arr.push("<tr><td align='right'><img id='Icon' src='" + CONTEXTPATH + "icon_query.gif' width='34' height='34' align='absmiddle'></td>");
    arr.push("<td align='left' id='Message' style='font-size:9pt'>" + msg + "</td></tr></table>");
    var div = doc.createElement("div");
    div.innerHTML = arr.join('');
    doc.body.appendChild(div);
    doc.close();
    //diag.OKButton.focus();
    pw.___("_DialogButtons_" + diag.ID).style.textAlign = "center";
}

var _DialogInstance = window.frameElement ? window.frameElement.DialogInstance : null;
var Page = {};
Page.onDialogLoad = function () {
    if (_DialogInstance) {
        if (_DialogInstance.Title) {
            document.title = _DialogInstance.Title;
        }
        window.Args = _DialogInstance.DialogArguments;
        _DialogInstance.Window = window;
        window.Parent = _DialogInstance.ParentWindow;
    }
}

Page.onDialogLoad();

PageOnLoad = function () {
    var d = _DialogInstance;
    if (d) {
        try {
            d.ParentWindow.$D = d;
            d.ParentWindow.$DW = d.Window;
            var flag = false;
            if (!this.AlertFlag) {
                var win = d.ParentWindow;
                while (win != win.parent) {
                    if (win._DialogInstance) {
                        flag = true;
                        break;
                    }
                    win = win.parent;
                }
                if (!flag) {
                    $E.getTopLevelWindow().___("_DialogBGIframeABC").style.opacity = "0";
                    $E.getTopLevelWindow().___("_DialogBGIframeABC").style.filter = "alpha(opacity=0)";

                    $E.getTopLevelWindow().___("_DialogBGDivABC").style.opacity = "0";
                    $E.getTopLevelWindow().___("_DialogBGDivABC").style.filter = "alpha(opacity=12)";
                }
            }
            if (d.AlertFlag) {
                $E.show($E.getTopLevelWindow().___("_AlertBGIframe"));
                $E.show($E.getTopLevelWindow().___("_AlertBGDiv"));
            }
            if (d.ShowButtonRow && (d.CancelButton.visibility == true)) {
                d.CancelButton.focus();
            }
            if (d.onLoad) {
                d.onLoad();
            }
        } catch (ex) { }
    }
}

Dialog.onKeyUp = function (event) {
    if (event.keyCode == 9) {
        var pw = $E.getTopLevelWindow();
        if (pw.Dialog._Array.length > 0) {
            stopEvent(event);
        }
    }
    if (event.keyCode == 27) {
        var pw = $E.getTopLevelWindow();
        if (pw.Dialog._Array.length > 0) {
            //Page.mousedown();
            //Page.click();
            var diag = pw.Dialog.getInstance(pw.Dialog._Array[pw.Dialog._Array.length - 1]);
            diag.CancelButton.onclick.apply(diag.CancelButton, []);
        }
    }
}

Dialog.dragStart = function (evt) {
    // DragManager.doDrag(evt,this.getParent("div"));//拖拽处理
}
Dialog.setPosition = function () {
    if (window.parent != window) return;
    var pw = $E.getTopLevelWindow();
    var DialogArr = pw.Dialog._Array;
    if (DialogArr == null || DialogArr.length == 0) return;

    for (i = 0; i < DialogArr.length; i++) {
        pw.___("_DialogFrame_" + DialogArr[i]).DialogInstance.setPosition();
    }
}
Dialog.prototype.setPosition = function () {
    var pw = $E.getTopLevelWindow();
    var doc = pw.document;
    var cw = doc.compatMode == "BackCompat" ? doc.body.clientWidth : doc.documentElement.clientWidth;
    var ch = doc.compatMode == "BackCompat" ? doc.body.clientHeight : doc.documentElement.clientHeight; //必须考虑文本框处于页面边缘处，控件显示不全的问题
    var sl = Math.max(doc.documentElement.scrollLeft, doc.body.scrollLeft);
    var st = Math.max(doc.documentElement.scrollTop, doc.body.scrollTop); //考虑滚动的情况
    var sw = Math.max(doc.documentElement.scrollWidth, doc.body.scrollWidth);
    var sh = Math.max(doc.documentElement.scrollHeight, doc.body.scrollHeight);
    sw = Math.max(sw, cw);
    sh = Math.max(sh, ch);
    this.Top = (ch - this.Height - 30) / 2 + st - 8; //有8像素的透明背景
    this.Left = (cw - this.Width - 12) / 2 + sl;
    if (this.ShowButtonRow) {//按钮行高36
        this.Top -= 18;
    }
    this.DialogDiv.style.top = this.Top + "px";
    this.DialogDiv.style.left = this.Left + "px";
    pw.___(this.bgiframeID).style.width = sw + "px";
    pw.___(this.bgiframeID).style.height = sh + "px";
    pw.___(this.bgdivID).style.width = sw + "px";
    pw.___(this.bgdivID).style.height = sh + "px";
}

//按ESC关闭对话框
if (isIE) {
    document.attachEvent("onkeydown", Dialog.onKeyUp);
    window.attachEvent("onload", PageOnLoad);
    window.attachEvent('onresize', Dialog.setPosition);
} else {
    document.addEventListener("keydown", Dialog.onKeyUp, false);
    window.addEventListener("load", PageOnLoad, false);
    window.addEventListener('resize', Dialog.setPosition, false);
}

/*function Dragable(name) {
try {
$("#_DialogDiv_" + name).jquerymoveDiv();
} catch (e) {
}
}*/
