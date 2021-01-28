<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AvaWidth.ascx.cs" Inherits="Comfy.App.Web.QuailtyCode.AvaWidth" %>
<script type="text/javascript">
    var avaI = 0;
    //在Table 中增加一行

    function addAva() {
        avaI++;
        var trHtml = "<tr id='tableTr" + avaI + "' >" +
                     "<td style='width:30px'><input type='text' onchange='checkText(this)' id='A" + avaI + "' style='width:28px'/></td>" +
                     "<td style='width:30px'>G</td>" +
                     "<td style='width:30px'><input type='text' onchange='checkText(this)' id='B" + avaI + "' style='width:28px'/></td>" +
                     "<td style='width:30px'>Inch</td>" +
                     "<td style='width:40px'><input type='text' onchange='checkText(this)' id='C" + avaI + "' style='width:38px'/></td>" +
                     "<td style='width:30px'>N</td>" +
                     "<td style='width:30px'><input type='text' onchange='checkText(this)' id='D" + avaI + "' style='width:28px'/></td>" +
                     "<td style='width:30px'>--</td>" +
                     "<td style='width:30px'><input type='text' onchange='checkText(this)' id='E" + avaI + "' style='width:28px'/></td>" +
                     "<td style='width:30px'><input type='button' style='height:6px;border:none;width:16px;background-color:red;' onclick=deleteAvaWidth(this) /> </td>" +
       "</tr>";
        $("#avaTable").append(trHtml);
    }
    function checkText(val) {
        if ($(val).val() != '' && isNaN($(val).val())) {
            $(val).val("");
            alert('Please input a number!');
        }
    }
    //初始化Table
    function InitTable(val) {
        $("#avaTable tr").remove();
        if (val == "" || val == "undefined") {
            return;
        }
        var strs = val.split("<?>");
        var i;
        var strsTemp;
        avaI = 0;
        for (i = 0; i < strs.length; i++) {
            avaI++;
            strsTemp = strs[i].split(",");
            var trHtml = "<tr id='tableTr" + avaI + "'>" +
                     "<td style='width:30px'><input type='text' onchange='checkText(this)' id='A" + avaI + "' value='" + strsTemp[0] + "' style='width:28px'/></td>" +
                     "<td style='width:30px'>G</td>" +
                     "<td style='width:30px'><input type='text' onchange='checkText(this)' id='B" + avaI + "' value='" + strsTemp[1] + "' style='width:28px'/></td>" +
                     "<td style='width:30px'>Inch</td>" +
                     "<td style='width:40px'><input type='text' onchange='checkText(this)' id='C" + avaI + "' value='" + strsTemp[2] + "' style='width:38px'/></td>" +
                     "<td style='width:30px'>N</td>" +
                     "<td style='width:30px'><input type='text' onchange='checkText(this)' id='D" + avaI + "' value='" + strsTemp[3] + "' style='width:28px'/></td>" +
                     "<td style='width:30px'>--</td>" +
                     "<td style='width:30px'><input type='text' onchange='checkText(this)' id='E" + avaI + "' value='" + strsTemp[4] + "' style='width:28px'/></td>" +
                     "<td style='width:30px'><input type='button' style='height:6px;border:none;width:16px;background-color:red;' onclick=deleteAvaWidth(this) /> </td>" +
               "</tr>";
            $("#avaTable").append(trHtml);
        }
    }
    //按删除的时候删除最后一行

    function deleteAva() {
        if (avaI == 0) { return; }
        $("#avaTable tr:last").remove();
        avaI--;
    }


    function deleteAvaWidth(val) {
        $(val).parent().parent().remove();
    }


    function confimAva() {
        var j;
      //  j = avaI - ($("#avaTable").find("tr").length - 1) + 1;
       var strResult='';
       for (j = 1; j <= avaI; j++) {
           if ($("#tableTr" + j).length > 0 && ($("#A" + j).val() != "" || $("#B" + j).val() != "" || $("#C" + j).val() != "" || $("#D" + j).val() != "" || $("#E" + j).val() != "")) {
               strResult = strResult + $("#A" + j).val() + ';' + $("#B" + j).val() + ';' + $("#C" + j).val() + ';' + $("#D" + j).val() + ';' + $("#E" + j).val() + '<>';
           }
       }
       return strResult;
   }

   function confimAvaUpdate() {
       var j;
       //  j = avaI - ($("#avaTable").find("tr").length - 1) + 1;
       var strResult = '';
       for (j = 1; j <= avaI; j++) {
           if ($("#tableTr" + j).length > 0 && ($("#A" + j).val() != "" || $("#B" + j).val() != "" || $("#C" + j).val() != "" || $("#D" + j).val() != "" || $("#E" + j).val() != "")) {
               strResult = strResult + $("#A" + j).val() + ';' + $("#B" + j).val() + ';' + $("#C" + j).val() + ';' + $("#D" + j).val() + ';' + $("#E" + j).val() + '()';
           }
       }
       return strResult;
   }
   //点击提交的时候将table的信息组合成一个字符串，并防止textbox中，这样后台就可以获取到这个字符串的信息。

   $(function () {
       $("form").submit(function (e) {
           
           $("input[id$='avaWidthValue']").get(0).value = confimAva();
       });
   })
</script>
<div id="avaDiv"   style="overflow:auto;height:60px;border:1px solid #FFA200">
<table id="avaTable" style="width:95%">
    <tr id="tableTr0" style="width:100%">
        <td style="width:30px">
        </td>
        <td style="width:30px">
        </td>
        <td style="width:30px">
        </td>
        <td  style="width:30px">
        </td>
        <td style="width:40px">
        </td>
        <td style="width:30px">
        </td>
        <td style="width:30px">
        </td>
        <td style="width:30px">
        </td>
        <td style="width:30px">
        </td>
        <td style="width:30px">
        </td>
    </tr>
</table>
</div>
<div style="height:1px"></div>
<div id="Div1" style="width:350px;">
<center>
<input type="button" value="Add" onclick="addAva()" style="width:60px" class="myButton" />
<%--<input type="button" value="Delete" onclick="deleteAva()" style="width:60px" class="myButton" />--%>
</center>
</div>
<asp:HiddenField ID = "avaWidthValue" runat = "server"></asp:HiddenField>

