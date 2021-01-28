using System;
using System.Collections.Generic;
using System.Text;

namespace Comfy.UI.WebControls.WebGridView
{
    public class GridJs
    {
        public static string Js = @"
<script type='text/javascript'>
var $tableId$_valueFromServer;
var $tableId$_SelectedFieldValues;
//調用服務器的方法
function $tableId$_GetServerTime(format) {
    $tableId$_CallBackToTheServer(format, '');
};
//服務器處理完之後調用的函數
function $tableId$_ReceiveDataFromServer(valueReturnFromServer, context) {
    if (context == 'getGridData') {
        var FromServer = valueReturnFromServer;
        eval('$tableId$_valueFromServer = {' + FromServer + '}');
        return;
    }
    if (context == 'getSelectedValue') {
         $tableId$_SelectedFieldValues = valueReturnFromServer;
        return;
    }
    if(context =='deleteRow'){
       return;
    }
    
    if(context == 'addNew'){
        unblock();
        $tableId$_Refresh();
        return;
        
    }    

    if(context == 'update'){
        unblock();
        $tableId$_Refresh();
        return;
        
    }  
    if(context == 'witchPage' && valueReturnFromServer == 'error'){
        alert('輸入的頁數不正確');
        return;
    }
    if(context == 'howPage' && valueReturnFromServer == 'error'){
        alert('輸入每頁顯示多少條的數目有誤(條數範圍大於0小於31)');
        return;
    }    
    $('tr#$tableId$GridHeard').nextAll().remove();
    $('tr#$tableId$trFooter1').nextAll().remove();
    $('#$tableId$Div').animate({scrollTop:0},0);  //將div滾動條置頂
    var valRe = valueReturnFromServer.split('<**>');
    $(valRe[0]).insertAfter('#$tableId$GridHeard');
    $(valRe[1]).insertAfter('#$tableId$trFooter1');
    $tableId$_valueFromServer = false;
    $tableId$_SelectedFieldValues = false;
    $tableId$_kc();
};

//修改單元格的顯示內容
function $tableId$_kc() {
    $('#$tableId$ td').each(function(e) {
        if ($(this).attr('Render')) {
            $(this).html(eval($(this).attr('Render') + '(""' + $(this).text() + '"")'));
        }
    });
};
//獲得某個單元格的值
function $tableId$_getGridValue(field) {
    if (!$tableId$_valueFromServer) {
        alert('請選中一行記錄');
        return false;
    }
    var fields = field.split(';');
    var result = '';
    for (var i = 0; i < fields.length; i++) {
        result += $tableId$_valueFromServer[fields[i]];
        if (i != (fields - 1)) {
            result += ',';
        }
    }

    return result.split(',');
};


//設置相鄰兩行的顏色不一樣,並且取得選中行的值
var $tableId$_currentActiveRow;
function $tableId$_changeActiveRow(obj) {
    if ($tableId$_currentActiveRow)
        $tableId$_currentActiveRow.style.backgroundColor = '';
    $tableId$_currentActiveRow = obj;
    $tableId$_currentActiveRow.style.backgroundColor = '#ffe7a2';

    $('#$tableId$ tr').each(function(e) {
        if (this.style.backgroundColor == '#ffe7a2') {
           $tableId$_CallBackToTheServer('getGridData:' + (e - 1), 'getGridData');

        }
    });
};
//勾選checkbox改變顏色
function $tableId$_checkboxRow(obj) {
   if(obj.checked){
     $(obj).parents().find('#$tableId$ tr ').nextAll().each(function(e){
       if(this.id!='$tableId$trFooter'&&this.id!='$tableId$trFooter1'){
         // this.style.backgroundColor = '#ffbd69';
          $(this).children().first().children().attr('checked', true);

        }
     });
   }else{
     $(obj).parents().find('#$tableId$ tr ').nextAll().each(function(e){
       if(this.id!='$tableId$trFooter'&&this.id!='$tableId$trFooter1')
       //   this.style.backgroundColor = '';
          $(this).children().first().children().attr('checked', false);
      });
   }
};
//獲取checkbox選中的值
function $tableId$_GetSelectedValues(val){
       if(!$tableId$_SelectedFieldValues){
             alert('沒有勾選行');
             return false;
       }
       var strTemp = $tableId$_SelectedFieldValues.split(';');
       var resArray = new Array();　;
       for(var i=0;i<strTemp.length;i++){
          eval('var tempMap = {' + strTemp[i] + '}');
          resArray[i] = tempMap[val];
       }
      return resArray;
}
//獲取checkbox選中的值
function $tableId$_GetSelectedFieldValues(){
     var selectedIndex='';
     $('#$tableId$ tr').each(function(e) {
        if ( $(this).children().first().children().attr('checked')) {
             selectedIndex = selectedIndex+(e-1)+';';
        }
    });
    $tableId$_CallBackToTheServer('getSelectedValue:' + selectedIndex, 'getSelectedValue');
}
//根據主鍵刪除某一行記錄
function $tableId$_DeleteRowByKey(val){
  $tableId$_CallBackToTheServer('deleteRow:' + val, 'deleteRow');
}

function $tableId$_DeleteRowsByKey(vals){
   for (var i = 0; i < vals.length; i++) {
       $tableId$_DeleteRowByKey(vals[i]);
   }
   $tableId$_Refresh();
}

//查找
function $tableId$_GridCheck(value) {
    var pageMessage = $('#$tableId$pageMessage').html();
   // $('#$tableId$Div').animate({scrollTop:0},0);  //將div滾動條置頂
    $tableId$_GetServerTime('check<*>' + value);
};
//刷新當前頁
function $tableId$_Refresh(){
   var pageMessage = $('#$tableId$pageMessage').html();
   $tableId$_CallBackToTheServer(pageMessage+'*refresh', 'refresh');
}

//設置顯示第幾頁
function $tableId$_SetWitchPage(){
    $tableId$_CallBackToTheServer('witchPage:' + $('#$tableId$WitchPage').val(), 'witchPage');
}
//設置一頁顯示多少條
function $tableId$_SetHowPage(){
    $tableId$_CallBackToTheServer('howPage:' + $('#$tableId$HowPage').val(), 'howPage');
}

//排序
function  $tableId$OrderBy(val) {
  var idName = $(val).attr('id');
  $tableId$_CallBackToTheServer('orderBy:' + idName, 'orderBy');
  if(idName.indexOf('*orderAsc') !=-1)  //原來為升序則變為降序
  {
     $(val).attr('id',idName.substring(0,idName.length-9) + '*orderDesc');
  }
  if(idName.indexOf('*orderDesc')!=-1)  //原來為降序則變為升序
  {
     $(val).attr('id',idName.substring(0,idName.length-10) + '*orderAsc');
  }          
}
//改變鼠標形狀
function ChangeMouseStyle(val){
   $(val).css('cursor','pointer');
}

//在Grid中查找內容
function $tableId$_CheckGridValue(val){
  $('#$tableId$ tr td').each(function(i) {
       $(this).removeClass('cellbgcolor');
       if($(this).text()==val&&val!=''){
          $(this).addClass('cellbgcolor');
       }
  });
}

//清除cell中的背景色
function $tableId$_RemoveCellBGC(){
  $('#$tableId$ tr td').each(function(i) {
       $(this).removeClass('cellbgcolor');
  });
}
$(function() {
    $tableId$_kc();
});
//增加model
   function $tableId$_AddNew() {
      $('form').each( function(){
        var f = this;
        var els = f.elements;
        var postStr = '';
        for (var i = 0; i < els.length; i++) {
            if (els[i].check && els[i].id.indexOf('$tableId$*')!=-1) {
                var value = $tableId$_GetValue(els[i]);
                var fieldName = els[i].id.split('*')[1];
                postStr = postStr + (fieldName + '<>:' + value + '<>;');
            }
        }
       if(postStr != ''){
           $tableId$_CallBackToTheServer('addNew:'+postStr, 'addNew');
       }
     });
   }

//更新model
   function $tableId$_Update() {
      $('form').each( function(){
        var f = this;
        var els = f.elements;
        var postStr = '';
        for (var i = 0; i < els.length; i++) {
            if (els[i].check && els[i].id.indexOf('$tableId$*')!=-1) {
                var value = $tableId$_GetValue(els[i]);
                var fieldName = els[i].id.split('*')[1];
                postStr = postStr + (fieldName + '<>:' + value + '<>;');
            }
        }
       if(postStr != ''){
           $tableId$_CallBackToTheServer('updateOld:'+postStr, 'update');
       }
     });
   }

    function $tableId$_GetValue(el) {
        switch (el.type) {
            case 'text':
            case 'hidden':
            case 'password':
            case 'file':
            case 'textarea': return el.value;
            case 'checkbox':
            case 'radio': return $tableId$_GetCheckedValue(el);
            case 'select-one':
            case 'select-multiple': return $tableId$_GetSelectedValue(el);
        }
    }

    function $tableId$_GetCheckedValue(el) {
        var s = '';
        var els = $tableId$_GetElements(el.name);
        for (var i = 0; i < els.length; i++) {
            if (els[i].checked) {
                s += '0'; // 可以通过0+来表示选中个数
            }
        }
        return s;
    }

    function $tableId$_GetSelectedValue(el) {
        var s = '';
        for (var i = 0; i < el.options.length; i++) {
            if (el.options[i].selected && el.options[i].value != '') {
                s += '0';
            }
        }
        return s;
    }

    function $tableId$_GetElements(name) {
        return document.getElementsByName(name);
    }

   //將數據綁定到editor面板中
   function $tableId$_BindForm(data) {
      if(!data){
        alert('請選中一行數據進行編輯');
        return false;
      }
      $('form').each( function(){
		for (var i = 0; i < this.elements.length;i++) {
			var element = this.elements[i];

			if ( element.id.indexOf('$tableId$*')==-1 || typeof(data[element.name.split('*')[1]]) == 'undefined' ) {
				continue;
			}

			// 这里val要转成String类型,则能够对select标签正确赋值
			// By ZhouHuan, 2010-7-14
			var val = data[element.name.split('*')[1]] + ''; 

			/// 过滤掉null值串
			if(val == 'null') {
				val = ''; 
			}

			switch (element.type) {
			case 'text': 
			case 'textarea': 
			case 'hidden':  
			case 'password': 				
				var regS = new RegExp('<br>','g');
				element.value = val.toString().replace(regS,'\n');	
							
				/* 文件上传标签的特殊处理. 把文件类型图标做下载按钮 */
				if (element.name.indexOf('input_upload') == 0) {
					$('#fileicon').attr('fileLink', val);
					$('#fileicon').click(function() {
						downloadFile($(this).attr('fileLink'));
					});
					var f = val.split('/');
					document.getElementById(element.name.substring(6)).innerHTML = f[f.length-1];
				} 
				break;
			case 'radio' : 
			case 'checkbox' : 
				if (val instanceof Array) element.checked = (val.indexOf(element.value) > -1);
				else element.checked = (element.value ==val);
				break;
			case 'select-one' : 
			case 'select-multiple' : 
				for (var j = 0; j < element.options.length; j++) {
					var option = element.options[j];
					//if (val instanceof Array) { // Modified by zhout. val不用转成数组, 直接比较就好. 默认逗号分隔
					if ((val+'').indexOf(',') && option.value != '') { 
						option.selected = ((','+val+',').indexOf(','+option.value+',') > -1);
					} else {
						option.selected = (option.value == val);
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


//back at 2012 4 18  on TextJs.cs
/*
function $tableId$_ReceiveDataFromServer(valueReturnFromServer, context) {
    if (context.indexOf('getGridData') != -1) {
        var FromServer = valueReturnFromServer;
        eval('$tableId$.valueFromServer = {' + FromServer + '}');
        if (context == 'getGridDataToEdit') {  //update
            $tableId$.EditOneRowAfterCallBack($tableId$.valueFromServer);
            return;
        }
        if (context == 'getGridDataToEditAndCallBack') {  //update
            $tableId$.EditOneRowAfterCallBackAndCallBack($tableId$.valueFromServer);
            return;
        }

        $tableId$.CallCurrentRowFun($tableId$.valueFromServer,context);
        return;
    }
    if (context.indexOf('getSelectedValue') != -1) {
        $tableId$.CallSelectedFun(valueReturnFromServer,context);
        return;
    }
    if (context == 'deleteRow') {
        return;
    }

    if (context == 'addNew') {
        $('form').each(function(e, obj) {
            obj.reset();
        });
        return;

    }

    if (context == 'addNewAndClose') {
        $tableId$.Unblock();
        return;

    }

    if (context == 'update') {
        $tableId$.Unblock();
        return;

    }
    if (context == 'witchPage' && valueReturnFromServer == 'error') {
        $('#_TopLoadingPattern').remove();
        alert('輸入的頁數不正確');
        return;
    }
    if (context == 'howPage' && valueReturnFromServer == 'error') {
        $('#_TopLoadingPattern').remove();
        alert('輸入每頁顯示多少條的數目有誤(條數範圍大於0小於21)');
        return;
    }

    $tableId$.AfterCallBack(valueReturnFromServer);
};
*/

/*
    $(function() {
        $tableId$.kc();
        if ($tableId$.AddForm) {
            $('#$tableId$DivF').jquerymove();
            $(window).bind('resize', function() {
                if ($('#$tableId$DivF').css('display') != 'none') {
                    $('._TopGaiDiv').remove();
                    $tableId$.initDiv();
                    $tableId$.shroud();
                }
            });

            $('#$tableId$AddForm').mousedown(function(event) {
                event.stopPropagation();
            });
        }
        if ($tableId$.SearchForm) {
            $('#$tableId$SearchDiv').jquerymove();
            $(window).bind('resize', function() {
                if ($('#$tableId$SearchDiv').css('display') != 'none') {
                    $('._TopGaiDiv').remove();
                    $tableId$.initSearchDiv();
                    $tableId$.shroud();
                }
            });
            $('#$tableId$AddSearchForm').mousedown(function(event) {
                event.stopPropagation();
            });
        }
    });
    */

</script>
";
    }
}
