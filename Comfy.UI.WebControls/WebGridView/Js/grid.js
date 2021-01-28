
var valueFromServer;
//調用服務器的方法
function GetServerTime(format) {
    CallBackToTheServer(format, "");
};
//服務器處理完之後調用的函數
function ReceiveDataFromServer(valueReturnFromServer, context) {
    if (context == "getGridData") {
        var FromServer = valueReturnFromServer;
        eval("valueFromServer = {" + FromServer + "}");
        return;
    }
    $(valueReturnFromServer).insertAfter('#GridHeard');
    valueFromServer = false;
    kc();
};

//修改單元格的顯示內容
function kc() {
    $("td").each(function(e) {
        if ($(this).attr("Render")) {
            $(this).html(eval($(this).attr("Render") + "('" + $(this).text() + "')"));
        }
    });
};
//獲得某個單元格的值
function getGridValue(field) {
    if (!valueFromServer) {
        alert("請選中一行記錄");
        return "請選中一行記錄";
    }
    // alert(valueFromServer["EmpNo"]);
    var fields = field.split(";");
    var result = "";
    for (var i = 0; i < fields.length; i++) {
        result += valueFromServer[fields[i]];
        if (i != (fields - 1)) {
            result += ",";
        }
    }

    return result.split(",");
};


//設置相鄰兩行的顏色不一樣,並且取得選中行的值
var currentActiveRow;
function changeActiveRow(obj) {
    if (currentActiveRow)
        currentActiveRow.style.backgroundColor = "";
    currentActiveRow = obj;
    currentActiveRow.style.backgroundColor = "#ffe7a2";

    $("#myTable tr").each(function(e) {
        if (this.style.backgroundColor == "#ffe7a2") {
            CallBackToTheServer("getGridData:" + (e - 1), "getGridData");

        }
    });
};
//查找
function GridCheck(value) {
    var pageMessage = $('#pageMessage').html();
    GetServerTime('check<*>' + value);
    $('tr#GridHeard').nextAll().remove();
};

$(function() {
    kc();
});