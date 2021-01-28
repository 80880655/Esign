<%@ Page Language='C#' AutoEventWireup='true' CodeBehind='CreateFields.aspx.cs' Inherits='Comfy.App.Web.CreateFields' %>

<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>
<html xmlns='http://www.w3.org/1999/xhtml'>
<head runat='server'>

    <script src='Scripts/jquery1.4.js' type='text/javascript'></script>

    <title></title>

    <script type='text/javascript'>
        function AjaxGetValue(dllName, className, tagPrefix, isAdd, isSearch, wGId, dSId, ikey) {   //ajax返回函數值 by ZH
            $.ajax({
            type: 'post',
                url: 'AjaxGetValue.ashx?dllName=' + dllName + '&className=' + className + '&tagPrefix=' + tagPrefix + '&wGId=' + wGId + '&dSId=' + dSId + '&Ikey=' + ikey + '&isAdd=' + isAdd + '&isSearch=' + isSearch,
                cache: false,
                success: function(msg) {
                    $('#Context').html(msg);
                    // document.write(msg);
                }

            });
        }
        function Create() {
            var tagPrefix = escape($('#TagPrefix').val());
            var dllName = escape($('#DllName').val());
            var className = escape($('#ClassName').val());
            var isAdd = $("#IsAdd").attr("checked");
            var isSearch = $("#IsSearch").attr("checked");

            var wGId = escape($('#GridViewId').val());

            var dSId = escape($('#DSId').val());

            var ikey = escape($('#ikey').val());

            if (tagPrefix == "" || !tagPrefix) {
                alert("請填寫前綴");
                return;
            }

            if (wGId == "" || !wGId) {
                alert("請填寫webgridview Id");
                return;
            }

            if (dSId == "" || !dSId) {
                alert("請填寫datasource Id");
                return;
            }

            if (ikey == "" || !ikey) {
                alert("請填寫Ikey");
                return;
            }
            
            AjaxGetValue(dllName, className, tagPrefix,isAdd,isSearch,wGId,dSId,ikey);
        }
    </script>

</head>
<body>
    <form id='form1'>
    <div>
        <table>
            <tr>
                <td align="right">
                    dll名稱：
                </td>
                <td>
                    <input type='text' id='DllName' />
                </td>
                <td align="right">
                    類名稱：
                </td>
                <td colspan="3">
                    <input type='text' id='ClassName' style="width: 400px" />
                </td>
                <td align="right">
                    標籤前綴：
                </td>
                <td>
                    <input type='text' id='TagPrefix' /><br />
                </td>
            </tr>
            <tr>
                <td align="right">
                    WebGridView Id:
                </td>
                <td>
                    <input type='text' id='GridViewId' />
                </td>
                <td align="right">
                    DataSource Id：
                </td>
                <td>
                    <input type='text' id='DSId' style="width: 180px" />
                </td>
                <td>
                    Ikey :
                </td>
                <td>
                    <input type="text" id="ikey" style="width: 182px" />
                </td>
                <td align="right">
                    是否生成新增面板：
                </td>
                <td>
                    <input type="checkbox" id="IsAdd" />
                </td>
            </tr>
            <tr>
                <td align="right">
                    是否生成查詢面板：
                </td>
                <td>
                    <input type="checkbox" id="IsSearch" />
                </td>
            </tr>
        </table>
        <input type='button' value='生成' onclick='Create()' />
        <br />
        <br />
        <div style='background-color: White;' id='Context'>
        </div>
    </div>
    </form>
</body>
</html>
