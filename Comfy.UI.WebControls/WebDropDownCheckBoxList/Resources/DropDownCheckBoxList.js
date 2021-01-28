var osType; //流覽器類型
var itemCount; //選擇項總數
var vItems; //選擇的值
var tItems; //選擇的文本
var vtItems; //選擇的文本&值
var splitor; //分隔符號
var $hfValueObj;
var $hfTextObj;
var $txtObj;
var $divObj;
var idPrefix;
Array.prototype.initItem = function () {
    for (var i = 0; i < itemCount; i++) {
        this[i] = "";
    }
}
Array.prototype.hasItem = function (item) {
    for (var i = 0; i < itemCount; i++) {
        if (this[i] == item) {
            return true;
        }
    }
    return false;
}
Array.prototype.addItem = function (item) {
    if (this.hasItem(item))
    { return; }
    for (var i = 0; i < itemCount; i++) {
        if (this[i] == "") {
            this[i] = item;
            break;
        }
    }
}
Array.prototype.removeItem = function (item) {
    if (!this.hasItem(item))
    { return; }
    for (var i = 0; i < itemCount; i++) {
        if (this[i] == item) {
            this[i] = "";
            break;
        }
    }
}
Array.prototype.joinItem = function (spChar) {
    var ret = "";
    for (var i = 0; i < itemCount; i++) {
        if (this[i] == "") {
            continue;
        }
        if (ret.length > 0)
        { ret = ret + ','; }
        ret = ret + this[i];
    }
    return ret;
}
function getBrowerOs() {
    var OsObject = "";
    if (navigator.userAgent.indexOf("MSIE") > 0) {
        return "MSIE";       //IE流覽器
    }
    if (isFirefox = navigator.userAgent.indexOf("Firefox") > 0) {
        return "Firefox";     //Firefox流覽器
    }
    if (isSafari = navigator.userAgent.indexOf("Safari") > 0) {
        return "Safari";      //Safan流覽器
    }
    if (isCamino = navigator.userAgent.indexOf("Camino") > 0) {
        return "Camino";   //Camino流覽器
    }
    if (isMozilla = navigator.userAgent.indexOf("Gecko/") > 0) {
        return "Gecko";    //Gecko流覽器
    }
}
function setObjectState(objId) {
    if ($("#" + objId).css("display") == "none")
    { $("#" + objId).css("display", "inline") }
    else
    { $("#" + objId).css("display", "none") }
}
//<!--檢測當前點擊是否在彈出的DIV 範圍;否則隱藏彈出的DIV-->
function responseOnFormClick(e, prefix) {
    idPrefix = prefix;
    $divObj = $("#" + idPrefix + "_div");
    if ($divObj.css("display") == "none") {
        return false;
    }
    var $imgDownObj = $("#" + idPrefix + "_imgDown");
    var $imgUpObj = $("#" + idPrefix + "_imgUp");
    var targetId;
    if ($.browser.msie) { targetId = e.srcElement.id; }
    else { targetId = e.target.id; }
    if (targetId == undefined || targetId.indexOf(idPrefix) < 0) {
        $divObj.css("display", "none");
        $imgDownObj.css("display", "inline");
        $imgUpObj.css("display", "none");
    }
    return false;
}
////顯示或者隱藏彈出的DIV
function toggleDivShowState(prefix, splitorString, iCount) {
    idPrefix = prefix;
    splitor = splitorString;
    itemCount = iCount;
    vItems = new Array(iCount);
    vItems.initItem();
    tItems = new Array(iCount);
    tItems.initItem();
    vtItems = new Array(iCount);
    vtItems.initItem();
    $hfTextObj = $("#" + idPrefix + "_selectItemValueText");
    $hfValueObj = $("#" + idPrefix + "_selectItemValue");
    $divObj = $("#" + idPrefix + "_div");
    $txtObj = $("#" + idPrefix + "_txtMain");
    setObjectState(idPrefix + "_div");
    if ($divObj.css("display") == "inline") {
        selectDefaultItem(idPrefix + "_selectItemValue", idPrefix);
    }
    setObjectState(idPrefix + "_imgDown");
    setObjectState(idPrefix + "_imgUp");
}

function selectDefaultItem(hfValueId, prefix) {
    $hfValueObj = $("#" + hfValueId);
    if (!$hfValueObj.val())
    { return; }
    var arr = $hfValueObj.val().split(splitor);
    if (!arr)
    { return; }
    vItems.initItem();
    tItems.initItem();
    vtItems.initItem();
    var subNodes;
    var firstNode;
    var lastNode;
    var nodeValue;
    var nodeText;
    var $tdNodes;
    $divObj = $("#" + prefix + "_div");
    var $tableNodes = $divObj.children('table');
    var tableId = $tableNodes.get(0).id;
    $("#" + tableId + " tr").each(function () {
        $tdNodes = $(this).children('td');
        $tdNodes.each(function () {
            subNodes = this;
            firstNode = subNodes.children[0];
            lastNode = subNodes.children[1];
            nodeValue = firstNode.value;
            if (nodeValue) {
                if (lastNode.innerText)
                { nodeText = lastNode.innerText; }
                else
                { nodeText = lastNode.textContent; }
                if (arr.hasItem(nodeValue)) {
                    if (!vItems.hasItem(nodeValue))
                    { vItems.addItem(nodeValue); }
                    if (!tItems.hasItem(nodeText))
                    { tItems.addItem(nodeText); }
                    if (!vtItems.hasItem(nodeValue + "|" + nodeText))
                    { vtItems.addItem(nodeValue + "|" + nodeText); }
                    if (!tItems.hasItem(nodeText))
                    { tItems.addItem(nodeText); }
                    if (!vtItems.hasItem(nodeValue + "|" + nodeText))
                    { vtItems.addItem(nodeValue + "|" + nodeText); }
                    firstNode.checked = true;
                }
            }
        });
    });

    $txtObj.val(tItems.joinItem(splitor));
    $hfTextObj.val(vtItems.joinItem(splitor));
    $hfValueObj.val(vItems.joinItem(splitor));
}
function mouseUpdateValueWhenCheckItemStateChanged(e, chkObjId, lblObjId, chkAllObjId, prefix) {
    var $chkObj = $("#" + chkObjId);
    var $lblObj = $("#" + lblObjId);
    var nodeValue;
    var nodeText;
    nodeText = $lblObj.text();
    nodeValue = $chkObj.val();
    if (nodeValue) {
        if ($chkObj.get(0).checked) {
            if (!vItems.hasItem(nodeValue))
            { vItems.addItem(nodeValue); }
            if (!tItems.hasItem(nodeText))
            { tItems.addItem(nodeText); }
            if (!vtItems.hasItem(nodeValue + "|" + nodeText))
            { vtItems.addItem(nodeValue + "|" + nodeText); }
        }
        else {
            var $chkAllItem = $("#" + prefix + "_chkAllItemValue");
            $chkAllItem.get(0).checked = false;
            if (vItems.hasItem(nodeValue))
            { vItems.removeItem(nodeValue); }
            if (tItems.hasItem(nodeText))
            { tItems.removeItem(nodeText); }
            if (vtItems.hasItem(nodeValue + "|" + nodeText))
            { vtItems.removeItem(nodeValue + "|" + nodeText); }
        }
        $txtObj.val(tItems.joinItem(splitor));
        $hfTextObj.val(vtItems.joinItem(splitor));
        $hfValueObj.val(vItems.joinItem(splitor));
    }
    //判斷是否選中所有項
    $divObj = $("#" + prefix + "_div");
    var $tableNodes = $divObj.children('table');
    var isAllSelected = true;
    var $tdNodes;
    var subNodes;
    var tableId = $tableNodes.get(0).id;
    $("#" + tableId + " tr").each(function () {
        $tdNodes = $(this).children('td');
        $tdNodes.each(function () {
            subNodes = this;
            firstNode = subNodes.children[0];
            nodeValue = firstNode.value;
            if (nodeValue && !firstNode.checked) {
                isAllSelected = false;
                return;
            }
        });
    });

    //true 則"全選 CheckBox" 選中
    if (isAllSelected) {
        var chkAllObj = $("#" + chkAllObjId).get(0);
        chkAllObj.checked = true;
    }

    //    if (window.event)
    //    { window.event.returnValue = false; }

    //    if (e.preventDefault)
    //    { e.preventDefault(); }

    if (window.event)
    { window.event.cancelBubble = true; }

    if (e.stopPropagation)
    { e.stopPropagation(); }

    return true;
}

function mouseUpdateValueWhenSelectAllStateChanged(e, chkObj, prefix) {
    vItems.initItem();
    tItems.initItem();
    vtItems.initItem();
    var subNodes;
    var firstNode;
    var lastNode;
    var nodeValue;
    var nodeText;
    var $tdNodes;
    $divObj = $("#" + prefix + "_div");
    var $tableNodes = $divObj.children('table');
    var tableId = $tableNodes.get(0).id;
    $("#" + tableId + " tr").each(function () {
        $tdNodes = $(this).children('td');
        $tdNodes.each(function () {
            subNodes = this;
            firstNode = subNodes.children[0];
            lastNode = subNodes.children[1];
            nodeValue = firstNode.value;
            if (chkObj.checked && nodeValue) {
                if (lastNode.innerText) { nodeText = lastNode.innerText; }
                else { nodeText = lastNode.textContent; }
                if (!vItems.hasItem(nodeValue))
                { vItems.addItem(nodeValue); }
                if (!tItems.hasItem(nodeText))
                { tItems.addItem(nodeText); }
                if (!vtItems.hasItem(nodeValue + "|" + nodeText))
                { vtItems.addItem(nodeValue + "|" + nodeText); }
            }

            if (nodeValue)
            { firstNode.checked = chkObj.checked; }
        });
    });

    $txtObj.val(tItems.joinItem(splitor));
    $hfTextObj.val(vtItems.joinItem(splitor));
    $hfValueObj.val(vItems.joinItem(splitor));

    if (window.event)
    { window.event.cancelBubble = true; }

    if (e.stopPropagation)
    { e.stopPropagation(); }

    return true;
}

function clickChangeValueWhenCheckAllCheckBox(e, chkId, prefix) {
    return mouseUpdateValueWhenCheckAllCheckBox(e, chkId, prefix);
}

function mouseUpdateValueWhenCheckAllCheckBox(e, chkId, prefix) {
    var chkObj = $("#" + chkId).get(0);
    if (chkObj.checked)
    { chkObj.checked = false; }
    else
    { chkObj.checked = true; }

    return mouseUpdateValueWhenSelectAllStateChanged(e, chkObj, prefix);
}

function clickChangeValueWhenCheckItemCheckBox(e, chkId, spanId, chkAllObjId, prefix) {
    return mouseUpdateValueWhenCheckItemCheckBox(e, chkId, spanId, chkAllObjId, prefix);
}

function mouseUpdateValueWhenCheckItemCheckBox(e, chkId, spanId, chkAllObjId, prefix) {
    var chkObj = $("#" + chkId).get(0);
    if (chkObj.checked)
    { chkObj.checked = false; }
    else
    { chkObj.checked = true; }

    return mouseUpdateValueWhenCheckItemStateChanged(e, chkId, spanId, chkAllObjId, prefix);
}

function setStyleOnMouseOver(spanId) {
    var spanObj = document.getElementById(spanId);
    spanObj.style.backgroundColor = "#3399FF";
}

function setStyleOnMouseOut(spanId) {
    var spanObj = document.getElementById(spanId);
    spanObj.style.backgroundColor = "#ECECE3";
}



