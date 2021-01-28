var sourceUrl = '';
var topStr = top.location.toString();
var strs = topStr.split("/");
if (topStr.toLowerCase().indexOf("localhost:") > 0 || topStr.toLowerCase().indexOf("127.0.0.1:") > 0) {
    sourceUrl = strs[0] + "//" + strs[2];
}
else {
    sourceUrl = strs[0] + "//" + strs[2] + "/" + strs[3];
}
//var cssUrl = sourceUrl + "/Css/ui.zdialog.css";
//var jsUrl = sourceUrl + "/Scripts/zdialog.js";
//document.write('<link type="text/css" rel="stylesheet" href="' + cssUrl + '" />');
//document.write('<script type="text/javascript" src="' + jsUrl + '"></script>');
/*$(function() {

// 此段代码用于在顶部外层frame中，自动加入zdialog的引用;
if (top.location.toString() != location.toString()) {
// 先判断是否存在顶层frame
var topDoc = top.document;
if (topDoc.body.outerHTML.indexOf("jquery1.4.js") < 0) {
var js = topDoc.createElement("script");
js.src = sourceUrl + "/Scripts/jquery1.4.js";
topDoc.appendChild(js);
}

if (topDoc.body.outerHTML.indexOf("zdialog.js") < 0) {
var js = topDoc.createElement("script");
js.src = sourceUrl + "/Scripts/zdialog.js";
topDoc.appendChild(js);
}
}

var topDoc = top.document;
if (topDoc.body.outerHTML.indexOf("jquery.move.js") < 0) {
var js = topDoc.createElement("script");
js.src = sourceUrl + "/Scripts/jquery.move.js";
topDoc.appendChild(js);
}

});*/

function openZDailog(name, title, url, width, height, param) {
    dlg = new Dialog(name);
    dlg.Width = width;
    dlg.Height = height;
    dlg.Title = title;
    dlg.URL = sourceUrl + "/" + url;
    dlg.ShowMessageRow = false;
    dlg.ShowButtonRow = false;
    dlg.addParam("window", window);
    dlg.addParam("param", param);
    dlg.show();
    dlg.dragable(name);
}