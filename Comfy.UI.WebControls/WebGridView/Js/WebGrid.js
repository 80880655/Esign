
function WebGridView(name, addFrom, searchFrom) {
    this.valueFromServer = false;
    this.SelectedFieldValues = false;
    this.currentActiveRow = false;
    var gridName = name;
    this.AddForm = addFrom;
    this.SearchForm = searchFrom;
    this.UpdateSelectIndex = false;
    var trGridHeard = $("tr[id$=\"" + name + "GridHeard\"]");
    var trGridFooter = $("tr[id$=\"" + name + "trFooter1\"]");
    //調用服務器的方法
    this.GetServerTime = function (format) {
        eval(gridName + "_CallBackToTheServer(format, '')");
    }

    this.ReceiveDataFromServer = function (valueReturnFromServer, context) {
        if (valueReturnFromServer.indexOf('ExError<>') != -1) {
            $('._TopLoadingPattern').remove();
            alert("Error:" + valueReturnFromServer.split('<>')[1]);
            return;
        }
        if (context.indexOf('getGridData') != -1) {
            var FromServer = valueReturnFromServer;
            try {
                eval(gridName + '.valueFromServer = {' + FromServer + '}');

                if (context == 'getGridDataToEdit') {  //update
                    eval(gridName + ".EditOneRowAfterCallBack(" + gridName + ".valueFromServer)");
                    return;
                }
                if (context == 'getGridDataToEditAndCallBack') {  //update
                    eval(gridName + ".EditOneRowAfterCallBackAndCallBack(" + gridName + ".valueFromServer)");
                    return;
                }

                eval(gridName + ".CallCurrentRowFun(" + gridName + ".valueFromServer, context)");
            } catch (e) {
                alert(e);
            }
            return;
        }
        if (context.indexOf('getSelectedValue') != -1) {
            eval(gridName + ".CallSelectedFun(valueReturnFromServer, context)");
            return;
        }
        if (context == 'deleteRow') {
            eval(gridName + ".Refresh()");
            return;
        }

        if (context == 'addNew') {
            eval(gridName + ".Refresh()");
            return;

        }

        if (context == 'addNewAndClose') {
            eval(gridName + ".Refresh()");
            return;

        }

        if (context == 'update') {
            if (valueReturnFromServer.indexOf('ExError<>') != -1) {
                alert(valueReturnFromServer.split('<>')[1]);
                return;
            }
            eval(gridName + ".Refresh()");
            // eval(gridName + ".Unblock()");
            return;

        }
        if (context == 'witchPage' && valueReturnFromServer == 'error') {
            $('._TopLoadingPattern').remove();
            alert('輸入的頁數不正確');
            return;
        }
        if (context == 'howPage' && valueReturnFromServer == 'error') {
            $('._TopLoadingPattern').remove();
            alert('輸入每頁顯示多少條的數目有誤(條數範圍大於0小於21)');
            return;
        }
        eval(gridName + ".AfterCallBack(valueReturnFromServer)");
    };

    this.kc = function () {
        $("table[id$=\"" + gridName + "\"] td").each(function (e, obj) {
            if ($(obj).attr('Render')) {
                $(obj).html(eval($(obj).attr('Render') + '($(obj).text())'));
            }
        });
    }

    this.IsSelectOneRecord = function () {
        var flag = true;
        $("table[id$=\"" + gridName + "\"] tr").each(function (e, objc) {
            if (objc.style.backgroundColor == '#ffe7a2' || objc.style.backgroundColor == 'rgb(255, 231, 162)') {
                flag = false;
                return false;
            }
        });

        if (flag) {
            return false;
        }
        return true;
    }

    this.GetRowValues = function (field, fun) {
        var flag = true;
        $("table[id$=\"" + gridName + "\"] tr").each(function (e, objc) {
            if (objc.style.backgroundColor == '#ffe7a2' || objc.style.backgroundColor == 'rgb(255, 231, 162)') {
                flag = false;
                eval(gridName + "_CallBackToTheServer('getGridData:' + (e - 1), 'getGridData'+'>'+fun+'>'+field)");  ///add  a  new object
                return false;
            }

        });

        if (flag) {
            alert('Plase choose one record');
            return false;
        }

        //        if (!this.valueFromServer) {
        //            alert('請選中一行記錄');
        //            return false;
        //        }
        //        var fields = field.split(';');
        //        var result = '';
        //        for (var i = 0; i < fields.length; i++) {
        //            result += this.valueFromServer[fields[i]];
        //            if (i != (fields - 1)) {
        //                result += ',';
        //            }
        //        }

        //        return result.split(',');
    }

    this.GetGridValueByRow = function (field, fun, row) {
        var i = 0;
        $("table[id$=\"" + gridName + "\"] tr").each(function (e, objc) {
            i++;
        });
        var wRow = parseInt(row);
        if (wRow >= i) {
            return true;
        }
        wRow = wRow - 1;
        eval(gridName + "_CallBackToTheServer('getGridData:' + wRow, 'getGridData'+'>'+fun+'>'+field)");

    }

    this.CallCurrentRowFun = function (val, context) {
        var allMessage = context.split('>');
        var fields = allMessage[2].split(';');
        var result = '';
        for (var i = 0; i < fields.length; i++) {
            result += val[fields[i]];
            if (i != (fields - 1)) {
                result += '<*>';
            }
        }
        var values = result.split('<*>');
        eval(allMessage[1] + '(values);');
        return true;
    }

    //設置相鄰兩行的顏色不一樣,並且取得選中行的值
    this.changeActiveRow = function (obj) {
        if (this.currentActiveRow) {
            this.currentActiveRow.style.backgroundColor = '';
        }
        this.currentActiveRow = obj;
        this.currentActiveRow.style.backgroundColor = '#ffe7a2';
        //        $('#' + gridName + ' tr').each(function (e, objc) {
        //            if (objc.style.backgroundColor == '#ffe7a2') {
        //                eval(gridName + "_CallBackToTheServer('getGridData:' + (e - 1), 'getGridData')");  ///add  a  new object

        //            }
        //        });
    }
    //勾選checkbox改變顏色
    this.CheckboxRow = function (obj) {
        if (obj.checked) {
            $(obj).parents().find('#' + gridName + ' tr ').nextAll().each(function (e, objc) {
                // this.style.backgroundColor = '#ffbd69';
                $(objc).children().first().children().attr('checked', true);
            });
        } else {
            $(obj).parents().find('#' + gridName + ' tr ').nextAll().each(function (e, objc) {
                //   this.style.backgroundColor = '';
                $(objc).children().first().children().attr('checked', false);
            });
        }
    }
    //獲取checkbox選中的值
    this.GetSelectedValues = function (val, fun) {
        var selectedIndex = '';
        var flag = true;
        $("table[id$=\"" + gridName + "\"] tr").each(function (e, obj) {
            if (obj.id.indexOf(gridName + 'GridHeard')==-1 && $(obj).children().first().children().attr('checked')) {
                selectedIndex = selectedIndex + (e - 1) + ';';
                flag = false;
            }
        });
        if (flag) {
            alert('沒有勾選行');
            return false;
        }
        return eval(gridName + "_CallBackToTheServer('getSelectedValue:' + selectedIndex, 'getSelectedValue'+'>'+fun+'>'+val)");

        //        var strTemp = this.SelectedFieldValues.split(';');
        //        var resArray = new Array(); ;
        //        for (var i = 0; i < strTemp.length; i++) {
        //            eval('var tempMap = {' + strTemp[i] + '}');
        //            resArray[i] = tempMap[val];
        //        }
        //        return resArray;
    }

    //調用勾選checkbox觸發的函數
    this.CallSelectedFun = function (val, context) {
        var allMessage = context.split('>');
        var strTemp = val.split('<sPl>;');
        var mapKey = allMessage[2];
        var resArray = new Array();
        for (var i = 0; i < strTemp.length; i++) {
            eval('var tempMap = {' + strTemp[i] + '}');
            resArray[i] = tempMap[mapKey];
        }
        eval(allMessage[1] + '(resArray);');  //call fun
        return true;
    }

    this.GridAutoSearch = function () {
        $('form').each(function (e, obj) {
            var els = obj.elements;
            var postStr = '';
            for (var i = 0; i < els.length; i++) {
                if (els[i].id.indexOf((gridName + "_Search_")) != -1) {
                    var value = GetValue(els[i]);
                    var fieldName = els[i].id.substring((gridName.length + 8), els[i].id.length);
                    postStr = postStr + (fieldName + '<>:' + value + '<>;');
                }
            }
            if (postStr != '') {
                // eval(gridName + "_CallBackToTheServer('updateOld:'+postStr, 'update')");  ////////////////////////////////
                //this.GridCheck(postStr);
                // eval(gridName + ".GridCheck(postStr)");
                eval(gridName + ".LoadingPattern()");
                eval(gridName + ".GetServerTime('check<*>' + postStr)");
            }
        });

    }
    //查詢面板

    this.PanelSearch = function () {
        $('form').each(function (e, obj) {
            var els = obj.elements;
            var postStr = '';
            for (var i = 0; i < els.length; i++) {
                if (els[i].id.indexOf((gridName + "Search")) != -1) {
                    var value = GetValue(els[i]);
                    var fieldName = els[i].id.substring((gridName.length + 6), els[i].id.length);
                    postStr = postStr + (fieldName + '<>:' + value + '<>;');
                }
            }
            if (postStr != '') {
                // eval(gridName + "_CallBackToTheServer('updateOld:'+postStr, 'update')");  ////////////////////////////////
                //this.GridCheck(postStr);
                //  eval(gridName + ".GridCheck(postStr)");
                eval(gridName + ".LoadingPattern()");
                eval(gridName + ".GetServerTime('check<*>' + postStr)");
            }
        });
        eval(gridName + ".SearchUnblock()");
    }


    //獲取checkbox選中的值
    this.GetSelectedFieldValues = function () {
        var selectedIndex = '';
        $("table[id$=\"" + gridName + "\"] tr").each(function (e, obj) {
            if (objc.id.indexOf(gridName + 'GridHeard')==-1 && $(obj).children().first().children().attr('checked')) {
                selectedIndex = selectedIndex + (e - 1) + ';';
            }
        });
        eval(gridName + "_CallBackToTheServer('getSelectedValue:' + selectedIndex, 'getSelectedValue')");  ///////////////////////////////////////////
    }
    //根據主鍵刪除某一行記錄
    this.DeleteRowByKey = function (val) {
        eval(gridName + "_CallBackToTheServer('deleteRow:' + val, 'deleteRow')"); ///////////////////////////////////
        return true;
    }

    this.DeleteRowsByKey = function (vals) {
        for (var i = 0; i < vals.length; i++) {
            this.DeleteRowByKey(vals[i]);
        }
    }

    //刪處選中行，自動判斷多選或者單選
    this.DeleteRows = function (vals) {
        var selectedIndex = '';
        var flag = true; //GridHeard
        if ($("table[id$=\"" + gridName + "GridHeard\"]").children().first().children().attr("type") == "checkbox") {
            $("table[id$=\"" + gridName + "\"] tr").each(function (e, obj) {
                if (obj.id.indexOf(gridName + 'GridHeard')==-1 && $(obj).children().first().children().attr('checked')) {
                    selectedIndex = selectedIndex + (e - 1) + ';';
                    flag = false;
                }
            });
        }
        if (flag) {
            var flagOne = true;           
           // $('table#' + gridName + ' tr')
            $("table[id$=\"" + gridName + "\"] tr").each(function (e, objc) {
                if (objc.style.backgroundColor == '#ffe7a2'|| objc.style.backgroundColor=='rgb(255, 231, 162)') {
                    flagOne = false;
                    selectedIndex = e - 1 + ';';
                }
            });
            if (flagOne) {
                alert('請選中行');
                return false;
            }
        }
        eval(gridName + "_CallBackToTheServer('deleteRow:' + selectedIndex+'<'+vals, 'deleteRow')");

    }

    //查找
    this.GridCheck = function (map) {
        //var pageMessage = $('#' + gridName + 'pageMessage').html();

        var pageMessage = $("[id$=\"" + gridName + "pageMessage\"]").html();
        // $('#$tableId$Div').animate({scrollTop:0},0);  //將div滾動條置頂
        var value = '';
        for (var key in map) {
            value += (key + "<>:" + map[key] + "<>;");
        }
        this.LoadingPattern();
        this.GetServerTime('check<*>' + value);
    }
    //刷新當前頁
    this.Refresh = function () {
      //  var pageMessage = $('#' + gridName + 'pageMessage').html();
        var pageMessage = $("[id$=\"" + gridName + "pageMessage\"]").html();
        this.LoadingPattern();
        eval(gridName + "_CallBackToTheServer(pageMessage+'*refresh', 'refresh')");    ///////////////////////////////////////
        return;
    }

    //設置顯示第幾頁
    this.SetWitchPage = function () {
        eval(gridName + "_CallBackToTheServer('witchPage:' + $('#" + gridName + "WitchPage').val(), 'witchPage')");  /////////////////////////////
    }
    //設置一頁顯示多少條
    this.SetHowPage = function () {
        eval(gridName + "_CallBackToTheServer('howPage:' + $('#" + gridName + "HowPage').val(), 'howPage')"); /////////////////////////
    }

    //排序
    this.OrderBy = function (val) {
        var idName = $(val).attr('id');
        this.LoadingPattern();
        eval(gridName + "_CallBackToTheServer('orderBy:' + idName, 'orderBy')"); //////////////////////////
        if (idName.indexOf('*orderAsc') != -1)  //原來為升序則變為降序
        {
            $(val).attr('id', idName.substring(0, idName.length - 9) + '*orderDesc');
        }
        if (idName.indexOf('*orderDesc') != -1)  //原來為降序則變為升序
        {
            $(val).attr('id', idName.substring(0, idName.length - 10) + '*orderAsc');
        }
    }
    //改變鼠標形狀
    this.ChangeMouseStyle = function (val) {
        $(val).css('cursor', 'pointer');
    }

    //在Grid中查找內容
    this.CheckGridValue = function (val) {
        $('table[id="' + gridName + '"] tr td').each(function (i, obj) {
            $(obj).removeClass('cellbgcolor');
            if ($(obj).text() == val && val != '') {
                $(obj).addClass('cellbgcolor');
            }
        });
    }

    //清除cell中的背景色
    this.RemoveCellBGC = function () {
        $('table[id="' + gridName + '"] tr td').each(function (i, obj) {
            $(obj).removeClass('cellbgcolor');
        });
    }
    //增加model
    this.AddNew = function () {
        this.UpdateSelectIndex = false;
        try {
            if (!eval(gridName + "AddJudge()")) {
                return;
            }
        } catch (e) {
        }
        var tempFlag = "1";
        $('form').each(function (e, obj) {
            if (!FormChecker.doCheck(obj, gridName)) {
                tempFlag = "2";
                return;
            }
            var els = obj.elements;
            var postStr = '';
            for (var i = 0; i < els.length; i++) {
                if (els[i].id.indexOf((gridName + gridName)) != -1) {
                    var value = GetValue(els[i]);
                    var fieldName = '';
                    if (els[i].id.indexOf("_") != -1) {
                        var tempFieldName = els[i].id.split("_");
                        fieldName = tempFieldName[tempFieldName.length - 1].substring(gridName.length * 2, els[i].id.length);
                    }
                    else {
                        fieldName = els[i].id.substring(gridName.length * 2, els[i].id.length);
                    }
                    postStr = postStr + (fieldName + '<>:' + value + '<>;');
                }
            }
            if (postStr != '') {
                eval(gridName + "_CallBackToTheServer('addNew:'+postStr, 'addNew')"); ////////////////////////////////////
            }
        });
        if (tempFlag == "2")
            return;
        this.ResetDiv();
        // setTimeout(gridName + ".Refresh()",500);
        setTimeout(gridName + ".AfterAddCallBack()", 500);
    }

    this.AddNewAndClose = function () {
        this.UpdateSelectIndex = false;
        try {
            if (!eval(gridName + "AddJudge()")) {
                return;
            }
        } catch (e) {
        }
        var tempFlag = "1";
        $('form').each(function (e, obj) {
            if (!FormChecker.doCheck(obj,gridName)) {
                tempFlag = "2";
                return;
            }
            var els = obj.elements;
            var postStr = '';
            for (var i = 0; i < els.length; i++) {
                if (els[i].id.indexOf((gridName + gridName)) != -1) {
                    var value = GetValue(els[i]);
                   // var fieldName = els[i].id.substring(gridName.length * 2, els[i].id.length);
                    var fieldName = '';
                    if (els[i].id.indexOf("_") != -1) {
                        var tempFieldName = els[i].id.split("_");
                        fieldName = tempFieldName[tempFieldName.length - 1].substring(gridName.length * 2, els[i].id.length);
                    }
                    else {
                        fieldName = els[i].id.substring(gridName.length * 2, els[i].id.length);
                    }

                    postStr = postStr + (fieldName + '<>:' + value + '<>;');
                }
            }
            if (postStr != '') {
                eval(gridName + "_CallBackToTheServer('addNew:'+postStr, 'addNewAndClose')"); ////////////////////////////////////
            }
        });
        if (tempFlag == "2")
            return;
        eval(gridName + ".Unblock()");
        //  setTimeout(gridName + ".Refresh()", 500);
        setTimeout(gridName + ".AfterAddCallBack()", 500);
    }

    //更新model
    this.Update = function () {
        this.UpdateSelectIndex = false;
        try {
            if (!eval(gridName + "EditJudge()")) {
                return;
            }
        } catch (e) {
        }
        var tempFlag = "1";
        $('form').each(function (e, obj) {
            if (!FormChecker.doCheck(obj,gridName)) {
                tempFlag = "2";
                return;
            }
            var els = obj.elements;
            var postStr = '';
            for (var i = 0; i < els.length; i++) {
                if (els[i].id.indexOf((gridName + gridName)) != -1) {
                    var value = GetValue(els[i]);
                    // var fieldName = els[i].id.substring(gridName.length * 2, els[i].id.length);
                    var fieldName = '';
                    if (els[i].id.indexOf("_") != -1) {
                        var tempFieldName = els[i].id.split("_");
                        fieldName = tempFieldName[tempFieldName.length - 1].substring(gridName.length * 2, els[i].id.length);
                    }
                    else {
                        fieldName = els[i].id.substring(gridName.length * 2, els[i].id.length);
                    }
                    postStr = postStr + (fieldName + '<>:' + value + '<>;');
                }
            }
            if (postStr != '') {
                eval(gridName + "_CallBackToTheServer('updateOld:'+postStr, 'update')");  ////////////////////////////////
            }
        });
        if (tempFlag == "2")
            return;
        eval(gridName + ".Unblock()");
        //  setTimeout(gridName + ".Refresh()", 500);
        setTimeout(gridName + ".AfterAddCallBack()", 500);
    }

    //更新model
    this.UpdateClose = function () {
        try {
            if (!eval(gridName + "EditJudge()")) {
                return;
            }
        } catch (e) {
        }
        var tempFlag = "1";
        $('form').each(function (e, obj) {
            if (!FormChecker.doCheck(obj,gridName)) {
                tempFlag = "2";
                return;
            }
            var els = obj.elements;
            var postStr = '';
            for (var i = 0; i < els.length; i++) {
                if (els[i].id.indexOf((gridName + gridName)) != -1) {
                    var value = GetValue(els[i]);
                    // var fieldName = els[i].id.substring(gridName.length * 2, els[i].id.length);
                    var fieldName = '';
                    if (els[i].id.indexOf("_") != -1) {
                        var tempFieldName = els[i].id.split("_");
                        fieldName = tempFieldName[tempFieldName.length - 1].substring(gridName.length * 2, els[i].id.length);
                    }
                    else {
                        fieldName = els[i].id.substring(gridName.length * 2, els[i].id.length);
                    }
                    postStr = postStr + (fieldName + '<>:' + value + '<>;');
                }
            }
            if (postStr != '') {
                eval(gridName + "_CallBackToTheServer('updateOld:'+postStr, 'update')");  ////////////////////////////////
            }
        });
        if (tempFlag == "2")
            return;
        //  setTimeout(gridName + ".Refresh()", 500);
        setTimeout(gridName + ".AfterAddCallBack()", 500);
    }

    var GetValue = function (el) {
        switch (el.type) {
            case 'text':
            case 'hidden':
            case 'password':
            case 'file':
            case 'textarea': return el.value;
            case 'checkbox':
            case 'radio': return GetCheckedValue(el);
            case 'select-one':
            case 'select-multiple': return GetSelectedValue(el);
        }
    }

    var GetCheckedValue = function (el) {
        var s = 'False';
        var els = GetElements(el.name);
        if (els.length == 1) {
            for (var i = 0; i < els.length; i++) {
                if (els[i].checked) {
                    if (els[i].value == 'True')
                        s = 'True'; // 可以通过0+来表示选中个数
                    else {
                        s = '1';
                    }
                }
                else {
                    if (els[i].value == 'True')
                        s = 'False'; // 可以通过0+来表示选中个数
                    else {
                        s = '0';
                    }
                }
            }
        } else {
            s = '';
            for (var i = 0; i < els.length; i++) {
                if (els[i].checked) {
                    s += els[i].value + ",";
                }
            }
            if (s.indexOf(",") != -1) {
                s = s.substring(0, s.length - 1);
            }
        }
        return s;
    }


    var SetCheckedValue = function (el, val) {
        var els = GetElements(el.name);
        if (els.length == 1) {
            els[0].checked = (val == 'True' || val == '1' || val == 'true');
            if (val == 'True' || val == 'true' || val == 'false' || val == 'False')
                els[0].value = 'True';
            else
                els[0].value = '1';
        } else {
            for (var m = 0; m < els.length; m++) {
                els[m].checked = false;     //全部清空
            }
            var value = val.split(",");
            var k = 0;
            for (var i = 0; i < value.length; i++) {
                k = 0;
                for (var j = 0; j < els.length; j++) {
                    if (value[i] == els[j].value) {
                        els[j].checked = true;
                        k++;
                        break;
                    }
                }
            }
        }
    }


    var GetSelectedValue = function (el) {
        var s = '';
        for (var i = 0; i < el.options.length; i++) {
            if (el.options[i].selected && el.options[i].value != '') {
                s = el.options[i].value;
            }
        }
        return s;
    }

    var GetElements = function (name) {
        return document.getElementsByName(name);
    }

    //將數據綁定到editor面板中
    this.BindForm = function (data) {
        if (!data) {
            alert('Plase choose one record to edit');
            return false;
        }
        $('form').each(function (e, obj) {
            for (var i = 0; i < obj.elements.length; i++) {
                var element = obj.elements[i];

                var elementId = '';
                var elementName = '';
                if (element.id.indexOf("_") != -1) {
                    var tempFieldName = element.id.split("_");
                    elementId = tempFieldName[tempFieldName.length - 1]
                }
                else {
                    elementId = element.id;
                }
                if (element.name.indexOf("$") != -1) {
                    var tempFieldName = element.name.split("$");
                    elementName = tempFieldName[tempFieldName.length - 1]
                }
                else
                {
                   elementName = element.name;
                }


               if (elementId.indexOf(gridName + gridName) == -1 || typeof (data[elementName.substring(gridName.length * 2, element.name.length)]) == 'undefined') {
                    continue;
                }

                // 这里val要转成String类型,则能够对select标签正确赋值
                // By ZhouHuan, 2010-7-14
                var val = data[elementId.substring(gridName.length * 2, elementId.length)] + '';

                /// 过滤掉null值串
                if (val == 'null') {
                    val = '';
                }

                switch (element.type) {
                    case 'text':
                    case 'textarea':
                    case 'hidden':
                    case 'password':
                        var regS = new RegExp('<br>', 'g');
                        element.value = val.toString().replace(regS, '\n');

                        /* 文件上传标签的特殊处理. 把文件类型图标做下载按钮 */
                        if (element.name.indexOf('input_upload') == 0) {
                            $('#fileicon').attr('fileLink', val);
                            $('#fileicon').click(function () {
                                downloadFile($(this).attr('fileLink'));
                            });
                            var f = val.split('/');
                            document.getElementById(element.name.substring(6)).innerHTML = f[f.length - 1];
                        }
                        break;
                    case 'radio':
                    case 'checkbox':
                        // if (val instanceof Array) { element.checked = (val.indexOf(element.value) > -1); }
                        // else { element.checked = (val == 'True' || val == '1' || val == 'true'); if (val == 'True' || val == 'true' || val == 'false' || val == 'False') element.value = 'True'; else element.value = '1'; }
                        SetCheckedValue(element, val);
                        break;
                    case 'select-one':
                    case 'select-multiple':
                        for (var j = 0; j < element.options.length; j++) {
                            var option = element.options[j];
                            //if (val instanceof Array) { // Modified by zhout. val不用转成数组, 直接比较就好. 默认逗号分隔
                            if ((val + '').indexOf(',') && option.value != '') {
                                if (((',' + val + ',').indexOf(',' + option.value + ',') > -1)) {
                                    option.selected = ((',' + val + ',').indexOf(',' + option.value + ',') > -1);
                                } else {
                                    option.selected = ((',' + val + ',').indexOf(',' + option.text + ',') > -1);
                                }
                            } else {
                                if (option.value == val)
                                    option.selected = (option.value == val);
                                else
                                    option.selected = (option.text == val);
                            }
                        }
                        if (element.onchange) {
                            element.fireEvent('onchange');
                        }
                        break;
                }
            }
        });
        return true;
    }


    this.ResetDiv = function () {
        $('form').each(function (e, obj) {
            for (var i = 0; i < obj.elements.length; i++) {
                var element = obj.elements[i];
                if (element.id.indexOf(gridName + gridName) == -1) {
                    continue;
                }
                switch (element.type) {
                    case 'text':
                    case 'textarea':
                    case 'hidden':
                    case 'password':
                        element.value = "";
                        break;
                    case 'radio':
                    case 'checkbox':
                        var els = GetElements(element.name);
                        for (var m = 0; m < els.length; m++) {
                            els[m].checked = false;     //全部清空
                        }
                        break;
                    case 'select-one':
                    case 'select-multiple':
                        if (element.options.length > 0) {
                            element.options[0].selected = true;
                        }
                        break;
                }
            }
        });
    }

    this.SearchUnblock = function () {
        $("[id$=\"" + gridName + "SearchDiv\"]").css({
            "display": "none"
        });
        $("._TopGaiDiv").remove();
    }

    this.Unblock = function () {
        $("[id$=\"" + gridName + "DivF\"]").css({
            "display": "none"
        });
        $("._TopGaiDiv").remove();
        this.UpdateSelectIndex = false; //取消選中行
    }

    this.AddCss = function () {
        $("#" + gridName + "trUpdate").hide();
        $("#" + gridName + "trAdd").show();
        $("#" + gridName + "DivHeader").html("<div style='padding:8px 0 0 2px;font-weight:bolder;'> 【Add Panel】</div>");

       // $('#' + gridName + 'trUpdate').attr("style", "display:none");
     //   $('#' + gridName + 'trAdd').attr("style", "display:block");
       // $('#' + gridName + 'DivHeader').html("<div style='padding:8px 0 0 2px;font-weight:bolder;'> 【新增面板】</div>");
    }


    this.ShowSearchForm = function () {
       // $("#" + gridName + "btnSearchCanel").focus();
        $("[id$=\"" + gridName + "btnSearchCanel\"]").focus();
        $("._TopGaiDiv").remove();
        this.shroud();
        this.initSearchDiv();
    }

    this.EditorCss = function () {
        $("[id$=\"" + gridName + "trUpdate\"]").show();
        $("[id$=\"" + gridName + "trAdd\"]").hide();
        $("[id$=\"" + gridName + "DivHeader\"]").html("<div style='padding:8px 0 0 2px;font-weight:bolder;'> 【Edit Panel】</div>");
       // $('#' + gridName + 'trUpdate').attr("style", "display:block");
       // $('#' + gridName + 'trAdd').attr("style", "display:none");
       // $('#' + gridName + 'DivHeader').html("<div style='padding:8px 0 0 2px;font-weight:bolder;'> 【編輯面板】</div>");
    }


    this.AddNewRow = function () {
        this.ResetDiv();
        this.AddCss();
        $("#" + gridName + "bC").focus();
       // $("#" + gridName + "bC").focus();
        $("._TopGaiDiv").remove();
        this.shroud();
        this.initDiv();
    }


    this.initDiv = function () {
        $("#" + gridName + "DivF").css({
            "position": "absolute",
            "display": "block",
            "top": (($(window).height()) / 2 + $(window).scrollTop() - $("span[id$=\"" + gridName + "DivF\"]").outerHeight() / 2) + "px",
            "left": (($(window).width()) / 2 + $(window).scrollLeft() - $("span[id$=\"" + gridName + "DivF\"]").outerWidth() / 2) + "px"
        });
    }


    this.initSearchDiv = function () {
        $("#" + gridName + "SearchDiv").css({
            "position": "absolute",
            "display": "block",
            "top": (($(window).height()) / 2 + $(window).scrollTop() - $("#" + gridName + "SearchDiv").outerHeight() / 2) + "px",
            "left": (($(window).width()) / 2 + $(window).scrollLeft() - $("#" + gridName + "SearchDiv").outerWidth() / 2) + "px"
        });
    }

    //shroud panel
    this.shroud = function () {
        if ($.browser.msie && $.browser.version == "6.0") {
            $("<iframe id='_TopGaiDiv' class='_TopGaiDiv' style='margin:0;padding:0'>").width($(document).width()).height($(document).height()).css({ "position": "absolute", "left": 0, "top": 0, "background-color": "#CCC", "opacity": "0.0", "z-index": "3" }).appendTo(document.body); //加多一層Iframe，因為IE6下div遮不住select
        }
        $("<div id='_TopGaiDiv1' class='_TopGaiDiv' style='margin:0;padding:0'>").width($(document).width()).height($(document).height()).css({ "position": "absolute", "left": 0, "top": 0, "background-color": "#CCC", "opacity": "0.6", "z-index": "4" }).appendTo(document.body);
    }

    //show pattern when waiting
    this.LoadingPattern = function () {
        var imgUrl = '<%=WebResource("Comfy.UI.WebControls.WebGridView.Images.Loading.gif")%>';
        $("<div class='_TopLoadingPattern' style='margin:0;padding:0;font-size:22px'><center><img src='" + imgUrl + "'></img></center></div>").css({ "position": "absolute", "z-index": "100000", "left": $('#' + gridName + 'Div').offset().left + $('#' + gridName + 'Div').outerWidth() / 2 - 20, "top": $('#' + gridName + 'Div').offset().top + $('#' + gridName + 'Div').outerHeight() / 2 - 40, "background-color": "#B9D8FF", "opacity": "0.9" }).appendTo(document.body);
    }

    //edit
    this.EditOneRow = function () {
        this.ResetDiv();
        var flag = true;
        $("table[id$=\"" + gridName + "\"] tr").each(function (e, objc) {
           // alert(objc.style.backgroundColor + "   " + $(objc).css("backgroundColor"));
            if (objc.style.backgroundColor == '#ffe7a2' || objc.style.backgroundColor=='rgb(255, 231, 162)') {
                eval(gridName + ".UpdateSelectIndex=e");
                flag = false;
                eval(gridName + "_CallBackToTheServer('getGridData:' + (e - 1), 'getGridDataToEdit')");  ///add  a  new object
            }
        });

        if (flag) {
            alert('Plase choose one record');
            return false;
        }
    }

    this.NextData = function () {
        var trCount = $("table[id$=\"" + gridName + "\"] tr").length;
        if (trCount == 1) {
            alert("No more data");
            return;
        }
        var trObj = false;
        $("table[id$=\"" + gridName + "\"] tr").each(function (e, objc) {
            if (trObj) {
                eval(gridName + ".UpdateSelectIndex=e");
                objc.style.backgroundColor = '#ffe7a2';
                trObj.style.backgroundColor = '';
                eval(gridName + ".ResetDiv()");
                eval(gridName + ".currentActiveRow=objc");
                eval("ToolBarMenuItemClick('Edit')");

                return false;
            }
            if (objc.style.backgroundColor == '#ffe7a2' || objc.style.backgroundColor == 'rgb(255, 231, 162)') {
                if ((e + 1) == trCount) {
                    alert("No more data");
                    return false;
                } else {
                    trObj = objc;
                }
            }
        });

    }



    this.PreData = function () {
        var trCount = $("table[id$=\"" + gridName + "\"] tr").length;
        if (trCount == 1) {
            alert("No more data");
            return;
        }
        var trObj = false;
        $("table[id$=\"" + gridName + "\"] tr").each(function (e, objc) {
            if (objc.style.backgroundColor != '#ffe7a2'&& objc.style.backgroundColor!='rgb(255, 231, 162)') {
                trObj = objc;
                return true;
            }
            else {
                if (e == 1) {
                    alert("No more data");
                    return false;
                }
                eval(gridName + ".UpdateSelectIndex=(e-1)");
                trObj.style.backgroundColor = '#ffe7a2';
                objc.style.backgroundColor = '';
                eval(gridName + ".ResetDiv()");
                eval(gridName + ".currentActiveRow=trObj");
                eval("ToolBarMenuItemClick('Edit')");
                return false;
            }
        });

    }

    this.EditOneRowAndCallBack = function () {
        var flag = true;
        $("table[id$=\"" + gridName + "\"] tr").each(function (e, objc) {
            if (objc.style.backgroundColor == '#ffe7a2'|| objc.style.backgroundColor=='rgb(255, 231, 162)') {
                flag = false;
                eval(gridName + "_CallBackToTheServer('getGridData:' + (e - 1), 'getGridDataToEditAndCallBack')");  ///add  a  new object
            }
        });

        if (flag) {
            alert('Plase choose one record');
            return false;
        }
    }

    this.EditOneRowAfterCallBack = function (val) {
        try {
            eval("GridViewBeforeEdit(val)");
        }
        catch (e)
        { }
        if (!this.BindForm(val)) {
            return;
        }
        $("[id$=\"" + gridName + "bC1\"]").focus();
       // $("#" + gridName + "bC1").focus();
        this.EditorCss();
        $("._TopGaiDiv").remove();
        this.shroud();
        this.initDiv();
    }


    this.EditOneRowAfterCallBackAndCallBack = function (val) {
        try {
            eval("GridViewBeforeEdit(val)");
        }
        catch (e)
        { }
        if (!this.BindForm(val)) {
            return;
        }
        $("[id$=\"" + gridName + "bC1\"]").focus();
        this.EditorCss();
        $("._TopGaiDiv").remove();
        this.shroud();
        this.initDiv();
        try {
            eval("GridViewAfterEdit(val)");
        }
        catch (e)
        { }
    }

    this.AfterAddCallBack = function () {
        try {
            eval(gridName + "_AfterAddCallBack()");
        }
        catch (e) {

        }
    }

    this.AfterCallBack = function (valueReturnFromServer) {
        if (trGridHeard.length == 0) {
            trGridHeard = $("tr[id$=\"" + name + "GridHeard\"]");
        }
        if (trGridFooter.length == 0) {
            trGridFooter = $("tr[id$=\"" + name + "trFooter1\"]");
        }
        $("[id$=\"" + gridName + "__ChexkBox\"]").attr("checked", false);
        trGridHeard.nextAll().remove();
        trGridFooter.nextAll().remove();
        $("[id$=\"" + gridName + "Div\"]").animate({ scrollTop: 0 }, 0);  //將div滾動條置頂
        var valRe = valueReturnFromServer.split('<**>');
        // $(valRe[1]).insertAfter('#' + gridName + 'GridHeard');
        // $(valRe[0]).insertAfter('#' + gridName + 'trFooter1');
        trGridHeard.after(valRe[1]);
        trGridFooter.after(valRe[0]);
        this.valueFromServer = false;
        this.SelectedFieldValues = false;
        var selectIndex = this.UpdateSelectIndex;
        if (selectIndex != false) {
            $("table[id$=\"" + gridName + "\"] tr").each(function (e, objc) {
                if (selectIndex == e) {
                    objc.style.backgroundColor = '#ffe7a2';
                    eval(gridName + ".currentActiveRow=objc");
                    return false;
                }
            });
        }

        if (typeof (checkBoxArrayObj) != 'undefined') {
            for (var i = 0; i < checkBoxArrayObj.length; i++) {
               // $("#" + gridName + "ChexkBoxOnGrid" + checkBoxArrayObj[i]).attr("checked", true);
                $("[id$=\"" + gridName + "ChexkBoxOnGrid" + checkBoxArrayObj[i] + "\"]").attr("checked", true);
            }
        }

        this.kc();
        $('._TopLoadingPattern').remove();
    }

    this.SetAddPanelFieldValue = function (fieldName, value) {
       // $('#' + gridName + gridName + fieldName).val(value);
        $("[id$=\""+ gridName + gridName + fieldName+"\"]").val(value);
    }

    this.SetSearchPanelFieldValue = function (fieldName, value) {
       // $('#' + gridName + 'Search' + fieldName).val(value);
        $("[id$=\""+ gridName + "Search" + fieldName+"\"]").val(value);
       // $("[id$=\"" + gridName + "ChexkBoxOnGrid" + checkBoxArrayObj[i] + "\"]")
    }

    var intH = '';
    var intW = '';
    $(window).bind('resize', function (event) {
        // alert($(".gridBodyDiv").parent().outerHeight() + "   " + $(".gridBodyDiv").parent().outerWidth());
        try {
            if ($(".gridBodyDiv").parent().outerHeight() != intH || $(".gridBodyDiv").parent().outerWidth() != intW) {
                intH = $(".gridBodyDiv").parent().outerHeight();
                intW = $(".gridBodyDiv").parent().outerWidth();
                if (intH > 0 && intW > 0) {
                    $(".gridBodyDiv", "#content").attr("style", "height:" + (parseInt(intH) - 42) + "px;width:" + (parseInt(intW) - 2) + "px;overflow:auto;");
                }
            }
        } catch (e) {
        }
    });
    //modify on 2012.08.10 createChildren
    /*   $(function () {
    eval(gridName + ".kc();" +
    "if (" + gridName + ".AddForm) {" +
    "$('#" + gridName + "DivF').jquerymove();" +
    "$(window).bind('resize', function() {" +
    "if ($('#" + gridName + "DivF').css('display') != 'none') {" +
    "$('._TopGaiDiv').remove();" +
    gridName + ".initDiv();" +
    gridName + ".shroud();" +
    "}" +
    "});" +

    "$('#" + gridName + "AddForm').mousedown(function(event) {" +
    "event.stopPropagation();" +
    "});" +
    "}" +
    "if (" + gridName + ".SearchForm) {" +
    "$('#" + gridName + "SearchDiv').jquerymove();" +
    "$(window).bind('resize', function() {" +
    "if ($('#" + gridName + "SearchDiv').css('display') != 'none') {" +
    "$('._TopGaiDiv').remove();" +
    gridName + ".initSearchDiv();" +
    gridName + ".shroud();" +
    "}" +
    "});" +
    "$('#'+gridName+'AddSearchForm').mousedown(function(event) {" +
    "event.stopPropagation();" +
    "});" +
    "}");
    });
    */

    $(function () {
        eval(gridName + ".kc();" +
        "if (" + gridName + ".AddForm) {" +
            "$('span[id$=\"" + gridName + "DivF\"]').jquerymove();" +
            "$(window).bind('resize', function() {" +
                "if ($('span[id$=\"" + gridName + "DivF\"]').css('display') != 'none') {" +
                    "$('._TopGaiDiv').remove();" +
                    gridName + ".initDiv();" +
                    gridName + ".shroud();" +
                "}" +
            "});" +

            "$('table[id$=\"" + gridName + "AddForm\"]').mousedown(function(event) {" +
                "event.stopPropagation();" +
            "});" +
        "}" +
        "if (" + gridName + ".SearchForm) {" +
            "$('span[id$=\"" + gridName + "SearchDiv\"]').jquerymove();" +
            "$(window).bind('resize', function() {" +
                "if ($('span[id$=\"" + gridName + "SearchDiv\"]').css('display') != 'none') {" +
                    "$('._TopGaiDiv').remove();" +
                    gridName + ".initSearchDiv();" +
                    gridName + ".shroud();" +
                "}" +
            "});" +
            "$('table[id$=\"" + gridName + "AddSearchForm\"]').mousedown(function(event) {" +
                "event.stopPropagation();" +
            "});" +
        "}");
    });
};