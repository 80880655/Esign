function Combo(txtName, divName, tableName, divWidth, divHeight, dllName, className, methodName, jsName) {
    var TxtName = txtName;
    var DivName = divName;
    var DivWidth = divWidth;
    var DivHeight = divHeight;
    var DllName = dllName;
    var ClassName = className;
    var MethodName = methodName;
    var TableName = tableName;
    var JsName = jsName;
    var begin = 0;
    var end = 100;
    this.setValue = function (val) {
        $("#" + txtName).val($(val).find("td:last").text());
        $("#" + divName).hide();
        var text = $(val).find("td:last").text();
        var value = $(val).find("td:first").text();
        try {
            eval(JsName + "Click(text,value)");
        } catch (e) { }
    }

    $(function () {
        eval("$(document).click(function (e) {" +
                "$('#" + DivName + "').hide();" +
             "});" +
           "$('#" + TxtName + "').bind('keyup click',function (event) {" +
                "var str;" +
                "var i;" +
                "var param = $('#" + TxtName + "').val();" +
                "event.stopPropagation();" +
                "if (param == '')" +
                    "param='null';" +
                "$('.divNo').remove();" +
                "$(\"<div class='divLoad'>努力加载中..........</div>\").appendTo($('#"+DivName+"'));" +
                "$('#" + DivName + "').css({" +
                    "'position': 'absolute'," +
                    "'z-index': '10000'," +
                    "'opacity': '0.97'," +
                    "'width': '" + DivWidth + "'," +
                    "'height': '"+DivHeight+"'," +
                    "'top': $('#" + TxtName + "').position().top + $('#" + TxtName + "').outerHeight() + 'px'," +
                    "'left': $('#" + TxtName + "').position().left + 'px'," +
                    "'display': 'block'" +
                "});" +
                "var result = AjaxCallFun('" + DllName + "', '" + ClassName + "', '" + MethodName + "', param);" +
                "if (result == '') {" +
                    "$('#" + TableName + " tr').remove();" +
                    "$('.divLoad').remove();" +
                    "$(\"<div class='divNo'>没有记录</div>\").appendTo($('#"+DivName+"'));" +
                    "return false;" +
                "}" +
                "var trhtml = '';" +
                "var tempColumn;" +
                "var tempRow = result.split('<?>');" +
                "for (i = 0; i < tempRow.length; i++) {" +
                    "if (tempRow[i] != '') {" +
                        "tempColumn = tempRow[i].split('<|>');" +
                        "trhtml = trhtml + \"<tr style='height:20px;padding:0 0 0 2px;cursor: pointer;background-color:\" + (i % 2 == 0 ? '#e5f1ff' : '#ffffff') + \"'  onclick='" + JsName + ".setValue(this)'><td style='display:none'>\" + tempColumn[0] + \"</td><td align='left'>\" + tempColumn[1] + \"</td></tr>\";" +
                    "}" +
                "}" +
                "$('.divLoad').remove();" +
                "$('.divNo').remove();" +
                "$('#" + TableName + " tr').remove();" +
                "$('#" + TableName + "').append(trhtml);" +
            "})");
    });

}