<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FlagAttribute.ascx.cs"
    Inherits="Comfy.App.Web.QuailtyCode.FlagAttribute" %>
<%@ Register TagPrefix="aspx" Namespace="Comfy.UI.WebControls.WebPopupControl" Assembly="Comfy.UI.WebControls" %>
<%@ Register TagPrefix="aspx" Namespace="Comfy.UI.WebControls.WebButtonEdit" Assembly="Comfy.UI.WebControls" %>
<%@ Register TagPrefix="myc" Namespace="Comfy.UI.WebControls.WebGridView" Assembly="Comfy.UI.WebControls" %>
<%@ Register TagPrefix="myc" Namespace="Comfy.UI.WebControls.WebGridView" Assembly="Comfy.UI.WebControls" %>
<script type="text/javascript">
    var tempFinishVal = '';
    function disableFlatAttribute() {
        $("#flatAdd").attr("style", "display:none");
        $("#flatEdit").attr("style", "display:none");
        $("#flatDelete").attr("style", "display:none");
        $("#flagchange").attr("disabled", "disabled");
        $("#flagchange").css({ "background-color": "#EFEFEF" });
        $("#flatClear").attr("style", "display:none");
        $("#construction1").attr("readOnly", true);
        $("#construction2").attr("readOnly", true);
        $("#txtFinish").attr("disabled", "disabled");
        $("#txtFinish").css({ "background-color": "#EFEFEF" });
        $("#construction1").css({ "background-color": "#EFEFEF" });
        $("#construction2").css({ "background-color": "#EFEFEF" });
        $("input[id$='construction']").attr("readOnly", true);
        $("input[id$='construction']").css({ "background-color": "#EFEFEF" });
        $("select[id$='cmbPattern']").attr("disabled", "disabled");
        $("select[id$='cmbPattern']").css({ "background-color": "#EFEFEF" });
        $("input[id$='yarnLength']").attr("readOnly", true);
        // $("input[id$='yarnLength']").css({ "color": "#808080" });
        $("input[id$='yarnLength']").css({ "background-color": "#EFEFEF" });
        $("input[id$='Layout']").attr("readOnly", true);
        $("input[id$='Layout']").css({ "background-color": "#EFEFEF" });
        $("#finishValTable").attr("readOnly", true);
    }
    function fNameChange() {
        
        //kingzhang for support 735101 PPO为空时，GP数据也为空
        if ($("#txtQC_Ref_PPO").val() == '') {
            $("select[id$='DropQC_Ref_GP']").empty();
            return;
        }
        //验证编号是否存在  0 为通过 ，1为不通过
        var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "IsEffective", $("#txtQC_Ref_PPO").val());

        if (result == '1') {
            $("select[id$='DropQC_Ref_GP']").empty();
            alert("The PPO which Not in the data inventory");
        }
        else {
            updateQC_Ref_GP();
        }
    }

    function fNameChange2() {
        //kingzhang for support 735101 PPO为空时，GP数据也为空
        if ($("#txtHF_Ref_PPO").val() == '') {
            $("select[id$='DropHF_Ref_GP']").empty();
            return;
        }
        //验证编号是否存在  0 为通过 ，1为不通过
        var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "IsEffective", $("#txtHF_Ref_PPO").val());

        if (result == '1') {
            $("select[id$='DropHF_Ref_GP']").empty();
            alert("The PPO which Not in the data inventory");
        }
        else {
            updateDropHF_Ref_GP();
        }
    }

    //更新QC_Ref_GP控件
    function updateDropHF_Ref_GP() {

		if ($("#txtHF_Ref_PPO").val() == '') return;
        var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "GetQC_Ref_GP", $("#txtHF_Ref_PPO").val());
        var vArray = result.split("|");
        var oldgp = $("#DropHF_Ref_GP").val();
        $("select[id$='DropHF_Ref_GP']").empty();
        for (var i = 0; i < vArray.length; i++) {

            $("select[id$='DropHF_Ref_GP']").append("<option value='" + vArray[i] + "'>" + vArray[i] + "</option>");
            //kingzhang for support 735101 PPO为空时，GP数据也为空 选择之前选择过的
            if (vArray[i] == oldgp)
                $("#DropHF_Ref_GP ").get(0).options[i].selected = true;
        }
    }

    //更新QC_Ref_GP控件
    function updateQC_Ref_GP() {
        
		if ($("#txtQC_Ref_PPO").val() == '')return;
        var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "GetQC_Ref_GP", $("#txtQC_Ref_PPO").val());
        var vArray = result.split("|");
        var oldgp = $("#DropQC_Ref_GP").val();
        $("select[id$='DropQC_Ref_GP']").empty();
        for (var i = 0; i < vArray.length; i++) {
            $("select[id$='DropQC_Ref_GP']").append("<option value='" + vArray[i] + "'>" + vArray[i] + "</option>");
            //kingzhang for support 735101 PPO为空时，GP数据也为空 选择之前选择过的
            if (vArray[i] == oldgp)
                $("#DropQC_Ref_GP ").get(0).options[i].selected = true;
        }
    }

    function QCRefShow(temp) {

        if (temp = '1') {

            $("#QCRef").attr("style", "display:none");
            $("#Tr1").attr("style", "display:none");
            $("#Tr2").attr("style", "display:none");
        }
        else {
            $("#QCRef").attr("style", "display:'block'");
            $("#Tr1").attr("style", "display:'block'");
            $("#Tr2").attr("style", "display:'block'");
        }
    }

    function clearConOne() {
        $("input[id$='construction']").val('');
    }
    function flatGridAdd() {
        flatYarnInfoGrid.AddNewRow();
        flatYarnInfoGrid.SetAddPanelFieldValue("Threads", "1");
        flatYarnInfoGrid.SetAddPanelFieldValue("Radio", "100");
    }
    function flagGridEdit() {
        flatYarnInfoGrid.EditOneRow();
        //  flatYarnInfoGrid.Refresh();
    }
    function flatGridDelete() {
        flatYarnInfoGrid.DeleteRows("Seq");
        flatYarnInfoGrid.Refresh();
    }
    $(function () {
        $("input[id$='txtFabricType']").attr("readonly", "true");
        $("#flatYarnInfoGridFooterTable").hide();
        $("#flatYarnInfoGridgridFootDiv").attr("style", "width:372px;height:1px;border:1px solid #A3C0E8;");
    });
</script>
<div style="border: 1px solid #FFA200; width: 490px;">
    <table id="fAttributeTable">
        <tr style="width: 1px">
            <td style="width: 100px">
            </td>
            <td style="width: 140px">
            </td>
            <td style="width: 100px">
            </td>
            <td style="width: 140px">
            </td>
        </tr>
        <tr>
            <td style="width: 100px" align="right">
                <asp:Label ID="lblAppName" runat="server" Text="Yarn Info" Width="100px"></asp:Label>
            </td>
            <td style="width: 380px" colspan="3" align="left">
                <div style="width: 378px; border: 1px solid #A3C0E8;">
                    <table>
                        <tr>
                            <td>
                                <myc:WebGridView ID="flatYarnInfoGrid" runat="server" DataSourceId="YarnInfoODS"
                                    PageSize="18" Height="100" Width="372" KeyFieldName="Seq" CreateAddPanel="true"
                                    CreateSearchPanel="false">
                                    <myc:Field FieldName="Seq" Caption="Seq" ShowOnEditForm="false" Width="22">
                                    </myc:Field>
                                    <myc:Field FieldName="YarnType" Caption="YarnType" DataSourceId="yarnTypeSQLDS" Width="215"
                                        ColumnSpan="3" FieldType="ComboBox" ValueField="Yarn_Type" TextField="Description"
                                        Check="not_empty">
                                    </myc:Field>
                                    <myc:Field FieldName="YarnCount" Caption="YarnCount" DataSourceId="yarnCountSQLDS"
                                        Width="58" FieldType="ComboBox" ValueField="Yarn_Count" TextField="Yarn_Count"
                                        Check="not_empty">
                                    </myc:Field>
                                    <myc:Field FieldName="Radio" Caption="Ratio" Width="30" Check="numeric">
                                    </myc:Field>
                                    <myc:Field FieldName="Threads" Caption="Strands" Width="30" Check="integer">
                                    </myc:Field>
                                    <myc:Field FieldName="YarnComponent" Caption="YarnComponent" Width="100" FieldType="TextArea">
                                    </myc:Field>
                                </myc:WebGridView>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <input type="button" value="Add" id="flatAdd" onclick="flatGridAdd()"  class="myButton" style="height: 20px;
                                    width: 60px" />&nbsp;&nbsp;&nbsp;<input type="button" id="flatEdit" value="Edit" class="myButton"
                                        style="height: 22px; width: 60px" onclick="flagGridEdit()" />&nbsp;&nbsp;&nbsp;<input
                                            type="button" value="Delete" id="flatDelete" class="myButton" style="height: 20px; width: 60px" 
                                            onclick="flatGridDelete()" />
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td style="width: 100px" align="right">
                <asp:Label ID="Label1" runat="server" Text="Construction" Width="100px"></asp:Label>
            </td>
            <td style="width: 380px" colspan="3" align="left">
                <div style="border: 1px solid #A3C0E8; width: 378px">
                    <table style="width: 100%; padding: 0; margin: 0px;">
                        <tr style="width: 100%">
                            <td align="left" style="">
                                <table style="width: 100%">
                                    <tr>
                                        <td align="left" colspan="2">
                                            <input type="text" id="flagchange" style="width: 370px" />
                                            <div style="width: 370px; height: 200px; overflow: auto; display: none; z-index: 10000;
                                                background-color: #8BB6EF; border: 2px solid #004080" id="flagSeDiv">
                                                <table style="width: 100%" id="flagSelectTable">
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <input type="text" id="construction1" style="width: 180px; background-color: #A3C0E8" />
                                            <input type="text" id="Con1" style="width: 180px; display: none" />
                                        </td>
                                        <td>
                                            <input type="text" id="construction2" style="width: 180px; background-color: #A3C0E8" />
                                            <input type="text" id="Con2" style="width: 180px; display: none" />
                                            <asp:TextBox ID="construction" runat="server" Width="120px" ClientIDMode="Static"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <%--                       <td align="left">
                            <input type="text" id="flagchange" style="width: 170px" />
                            <div style="width: 370px; height: 200px; overflow: auto; display: none; z-index: 10000;
                                background-color: #8BB6EF; border: 2px solid #004080" id="flagSeDiv">
                                <table style="width: 100%" id="flagSelectTable">
                                </table>
                            </div>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="construction" runat="server" Width="120px"></asp:TextBox>
                        </td>
                        <td>
                            <input type="button" value="Clear" id="flatClear" onclick="clearConOne()" />
                        </td>--%>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td style="width: 100px" align="right">
                <asp:Label ID="Label4" runat="server" Text="Yarn Length" Width="100px"></asp:Label>
            </td>
            <td style="width: 140px">
                <asp:TextBox ID="yarnLength" runat="server" Width="138px" ClientIDMode="Static"></asp:TextBox>
            </td>
            <td style="width: 100px" align="right">
                <asp:Label ID="Label5" runat="server" Text="Pattern" Width="97px"></asp:Label>
            </td>
            <td style="width: 140px" align="left">
                <%--  <asp:TextBox ID="txtPattern" runat="server" Width="134px" ></asp:TextBox>--%>
                <asp:DropDownList ID="cmbPattern" runat="server" Width="135px" DataSourceID="patternSQLDS" ClientIDMode="Static"
                    DataTextField="PARAMETER" DataValueField="PARAMETER" OnDataBound="cmbDyeMethod_DataBound">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td style="width: 100px" align="right">
                <asp:Label ID="Label6" runat="server" Text="Finish" Width="100px"></asp:Label>
            </td>
            <td style="width: 380px" colspan="3">
                <div style="border: 1px solid #A3C0E8; width: 378px;">
                    <table style="width: 100%">
                        <tr>
                            <td align="left">
                                <input type="text" id="txtFinish" style="width: 370px" />
                                <div style="width: 370px; height: 200px; overflow-y: auto; overflow-x: hidden; display: none;
                                    z-index: 10000; background-color: #8BB6EF; border: 2px solid #004080" id="finishDiv">
                                    <table style="width: 100%" id="finishTable">
                                    </table>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <asp:HiddenField ID="finishValue" runat="server" ClientIDMode="Static"></asp:HiddenField>
                                <div id="favaDiv" style="overflow-y: auto; overflow-x: hidden; height: 60px; border: 1px solid #A3C0E8">
                                    <table id="finishValTable" style="width: 100%;">
                                        <tr style="width: 100%">
                                            <td style="width: 90%">
                                            </td>
                                            <td style="width: 10%">
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>

        <tr id="trHC">
            <td style="width: 100px" align="right">
                <asp:Label ID="Label2" runat="server" Text="Heavy Collar" Width="100px"></asp:Label>
            </td>
            <td style="width: 380px" colspan="3" align="left">
                   <asp:DropDownList ID="heavyCollar" runat="server" Width="168px" ClientIDMode="Static">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="HEAVY_FLAT_KNIT" Value="HEAVY_FLAT_KNIT"></asp:ListItem>
                </asp:DropDownList>
            </td>
          
        </tr>
        <tr>
            <td style="width: 100px" align="right">
                <asp:Label ID="Label10" runat="server" Text="Layout" Width="100px"></asp:Label>
            </td>
            <td style="width: 380px" colspan="3" align="left">
                <asp:TextBox ID="Layout" runat="server" Width="374px" TextMode="MultiLine" ClientIDMode="Static"
                    Height="40px"></asp:TextBox>
            </td>
        </tr>
        <tr id="QCRef">
            <td style="width: 100px" align="right">
                <asp:Label ID="Label11" runat="server" Text="QC_Ref_PPO" Width="100px"></asp:Label>
            </td>
            <td style="width: 132px" colspan="1" align="left">
                <asp:TextBox ID="txtQC_Ref_PPO" runat="server" Width="132px" ClientIDMode="Static" Height="20px" onblur="fNameChange();"></asp:TextBox>
            </td>
            <td style="width: 100px" align="right">
                <asp:Label ID="Label12" runat="server" Text="QC_Ref_GP" Width="100px"></asp:Label>
            </td>
            <td style="width: 132px" colspan="1" align="left">
                     <asp:DropDownList ID="DropQC_Ref_GP"   ClientIDMode="Static" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <tr id="Tr1">
            <td style="width: 100px" align="right">
                <asp:Label ID="Label13" runat="server" Text="HF_Ref_PPO" Width="100px"></asp:Label>
            </td>
            <td style="width: 132px" colspan="1" align="left">
                    <asp:TextBox ID="txtHF_Ref_PPO" runat="server" Width="132px" ClientIDMode="Static" Height="20px" onblur="fNameChange2();"></asp:TextBox>
            </td>
            <td style="width: 100px" align="right">
                <asp:Label ID="Label14" runat="server" Text="HF_Ref_GP" Width="100px"></asp:Label>
            </td>
            <td style="width: 132px" colspan="1" align="left">
                   <asp:DropDownList ID="DropHF_Ref_GP"  ClientIDMode="Static" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
          <tr id="Tr2">
          <td style="width: 100px" align="right">
                <asp:Label ID="Label15" runat="server" Text="Remark" Width="100px"></asp:Label>
            </td>
            <td style="width: 380px" colspan="3">
           <asp:TextBox ID="TxtRemart"  runat="server" Width="374px" TextMode="MultiLine" ClientIDMode="Static" Height="40px"></asp:TextBox>
           </td>
        </tr>
    </table>
</div>
<asp:SqlDataSource ID="patternSQLDS" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
    ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="SELECT PARAMETER, IS_ACTIVE FROM PBKNITPARAMETER WHERE (&quot;SORT&quot; = 'PATTERN' and IS_Active='Y')">
</asp:SqlDataSource>
<asp:SqlDataSource ID="constructionSQLDS" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
    ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="SELECT Construction, Description FROM pbKnitConstruction where IS_Active='Y' order by Description asc">
</asp:SqlDataSource>
<asp:SqlDataSource ID="yarnTypeSQLDS" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
    ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="SELECT Yarn_Type,Description||'-->'||Yarn_Type as Description FROM pbKnitYarnType where IS_Active='Y' order by Description asc">
</asp:SqlDataSource>
<asp:SqlDataSource ID="yarnCountSQLDS" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
    ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="SELECT Yarn_Count FROM pbKnitYarnCount where IS_Active='Y' order by Yarn_Count asc">
</asp:SqlDataSource>
<%--<asp:ObjectDataSource ID="FlatYarnInfoODS" runat="server" SelectMethod="GetYarnInfo"
    InsertMethod="AddYarnInfo" DeleteMethod="DeleteYarnInfo" TypeName="Comfy.App.Web.QuailtyCode.FlagAttribute"
    DataObjectTypeName="Comfy.App.Core.QualityCode.YarnInfo">
    <SelectParameters>
        <asp:Parameter Name="orderByField" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>--%>
<script>
    function getFAttribute(strPage) {
        var Con = ($("#Con1").val() == "" ? "" : ($("#Con1").val() + ";")) + ($("#Con2").val() == "" ? "" : ($("#Con2").val() + ";"));
      //  var BW = $("#txtBWWidth").val();
     //   var AW = $("#txtAWWidth").val();
        //  var DM = $("#cmbDyeMethod").val();
        var Pattern = $("#cmbPattern").val();
        var YL = $("#yarnLength").val();
        var Yarn = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode."+strPage, "GetYarnInfo", "1");
        var Finis
        if (Con.indexOf(";") > 0) {
            Con = Con.substring(0, Con.length - 1);
        }
        if (Yarn.indexOf(";") > 0) {
            Yarn = Yarn.substring(0, Yarn.length - 1);
        }
        var Finish = getFinishVal();
        if (Finish.indexOf(";") > 0) {
            Finish = Finish.substring(0, Finish.length - 1);
        }
        return Con + "<>" + Pattern + "<>" + YL + "<>" + Finish + "<>" + Yarn;

    }
    function getFAttributeForUpdate() {
        var Con = ($("#Con1").val() == "" ? "" : ($("#Con1").val() + ",")) + ($("#Con2").val() == "" ? "" : ($("#Con2").val() + ","));
        var Pattern = $("#cmbPattern").val();
        var YL = $("#yarnLength").val();
        var Hy = $("#heavyCollar").val();
        var LY = $("#Layout").val();
        var vtxtQC_Ref_PPO = $("#txtQC_Ref_PPO").val();
        var vDropQC_Ref_GP = $("#DropQC_Ref_GP").val();
        var vtxtHF_Ref_PPO = $("#txtHF_Ref_PPO").val();
        var vDropHF_Ref_GP = $("#DropHF_Ref_GP").val();
        var vTxtRemart = $("#TxtRemart").val();

        if (Con.indexOf(",") > 0) {
            Con = Con.substring(0, Con.length - 1);
        }
        var Finish = getFinishVal();
        if (Finish.indexOf(";") > 0) {
            Finish = Finish.substring(0, Finish.length - 1);
        }
        return Con + "<>" + Pattern + "<>" + YL + "<>" + Finish + "<>" + Hy + "<>" + LY + "<>" + vtxtQC_Ref_PPO + "<>" + vDropQC_Ref_GP + "<>" + vtxtHF_Ref_PPO + "<>" + vDropHF_Ref_GP + "<>" + vTxtRemart; 

    }


    function getFinishVal() {
        var tempStrValueFinish = '';
        var tempStrFinish = '';
        $("#finishValTable td").each(function (index, obj) {
            tempStrFinish = $(obj).text();
            if (tempStrFinish.indexOf("[") > -1) {
                tempStrValueFinish = tempStrValueFinish + tempStrFinish.substring(tempStrFinish.indexOf("[") + 1, tempStrFinish.indexOf("]")) + ";";

                // alert(tempStr.indexOf("]"));
            }
        });
        //  alert(tempStrValue);
        return tempStrValueFinish;
    }
    $(function () {
        $("#construction").attr("style", "display:none");
        $("#construction1").bind('keyup', function (event) {
            if ($("#construction1").attr("readOnly")) {
                return;
            }
            $("#construction1").val("");
            $("#Con1").val();
            SetFabricTypeVal();
        });
        $("#construction2").bind('keyup', function (event) {
            if ($("#construction2").attr("readOnly")) {
                return;
            }
            $("#construction2").val("");
            $("#Con2").val();
            SetFabricTypeVal();
        });
    });

    var flagCombo = new Combo("flagchange", "flagSeDiv", "flagSelectTable", "370px", "200px", "Comfy.App.Core", "Comfy.App.Core.QualityCode.CustomerManager", "GetConstructionInfo", "flagCombo");
    function flagComboClick(text, val) {
        //        var n = ($("input[id$='construction']").val().split(',')).length - 1;
        //        if ($("input[id$='construction']").val() != '' && $("input[id$='construction']").val().indexOf(val) < 0 && n < 1) {
        //            $("input[id$='construction']").val($("input[id$='construction']").val() + ',' + val);
        //        } else if ($("input[id$='construction']").val().indexOf(val) < 0 && n < 1) {
        //            $("input[id$='construction']").val(val);
        //        }
        if ($("#construction1").val() == "") {
            $("#construction1").val(text);
            $("#Con1").val(val);
        } else if ($("#construction2").val() == "") {
            $("#construction2").val(text);
            $("#Con2").val(val);
        } else {
            alert("Only can choose two construction!");
        }
        SetFabricTypeVal();
        $("#flagchange").val("");
    }

    $(function () {
        $("form").submit(function (e) {
            $("#finishValue").get(0).value = getFinishVal();
        });
    })
    function deleteFinish(val, val1) {
        $(val).parent().parent().remove();
        tempFinishVal = tempFinishVal.replace(val1, "");
    }

    var fCombo = new Combo("txtFinish", "finishDiv", "finishTable", "370px", "200px", "Comfy.App.Core", "Comfy.App.Core.QualityCode.CustomerManager", "GetFinishInfo", "fCombo");
    function fComboClick(text, val) {
        if (tempFinishVal.indexOf(val) < 0) {
            var trHtml = "<tr sytle='height:10px'><td style='width:90%'>" + text + "[" + val + "]" + "</td>" +
                     "<td align='left' style='width:10%'><input type='button' style='height:6px;width:16px;border:none;background-color:red;' onclick=deleteFinish(this,'" + val + "') /></td>" + "</tr>";
            $("#finishValTable").append(trHtml);
            tempFinishVal = tempFinishVal + ',' + val;
        }
        $("#txtFinish").val("");
    }

    function SetFabricTypeVal() {
        var tempStr = ($("#Con1").val() == "" ? "" : ($("#Con1").val() + ",")) + ($("#Con2").val() == "" ? "" : $("#Con2").val());
        $("#construction").val(tempStr);
    }
    $(function () {
        $("input[id$='construction']").attr("readOnly", true);
    })

    function fInitHC(val) {
        if (val == "Y" || val == "HEAVY_FLAT_KNIT") {
            $("#heavyCollar").val("HEAVY_FLAT_KNIT");
        }
        if (val == "") {
            $("#heavyCollar").val("");
        }
    }

    function fInit(val, vPPONOFabricPart) {


        flatYarnInfoGrid.Refresh();
        var strS = val.split("<|>");
        

            if (strS[20] == "") {

                $("input[id$='txtQC_Ref_PPO']", "#fAttributeTable").val(vPPONOFabricPart.split("|")[0]);

            }
            else {
                $("input[id$='txtQC_Ref_PPO']", "#fAttributeTable").val(strS[20]);
            }
            $("input[id$='txtHF_Ref_PPO']", "#fAttributeTable").val(strS[22]);

            updateQC_Ref_GP();
            updateDropHF_Ref_GP();
            var count = $("#DropQC_Ref_GP option").length;
            if (strS[21] == "") {

                for (var i = 0; i < count; i++) {
                    if ($("#DropQC_Ref_GP ").get(0).options[i].text == vPPONOFabricPart.split("|")[1]) {
                        $("#DropQC_Ref_GP ").get(0).options[i].selected = true;

                        break;
                    }
                }

            }
            else {
                for (var i = 0; i < count; i++) {
                    if ($("#DropQC_Ref_GP ").get(0).options[i].text == strS[21]) {
                        $("#DropQC_Ref_GP ").get(0).options[i].selected = true;

                        break;
                    }
                }
            }

            var count2 = $("#DropHF_Ref_GP option").length;

            for (var i = 0; i < count2; i++) {
                if ($("#DropHF_Ref_GP ").get(0).options[i].text == strS[23]) {
                    $("#DropHF_Ref_GP ").get(0).options[i].selected = true;

                    break;
                }
            }
      
        $("#TxtRemart").val(strS[24]);

        if (strS[3] == "") {
            $("select[id$='cmbPattern']", "#fAttributeTable").val(" ");
        }
        else {
            $("select[id$='cmbPattern']", "#fAttributeTable").val(strS[3]);
        }
        $("#Layout").val(strS[4]);
        $("input[id$='construction']", "#fAttributeTable").val(strS[8]);
        $("input[id$='yarnLength']", "#fAttributeTable").val(strS[11]);
        if (strS[8] != "") {
            var result = AjaxCallFun("Comfy.App.Core", "Comfy.App.Core.QualityCode.CustomerManager", "GetConstruction", strS[8]);
            $("#construction1").val(result.split("<?>")[0]);
            if (result.split("<?>")[1] != "") {
                $("#construction2").val(result.split("<?>")[1]);
            }
            else {
                $("#construction2").val("");
            }
            $("#Con1").val(strS[8].split(",")[0]);
            if (strS[8].split(",")[1] != "") {
                $("#Con2").val(strS[8].split(",")[1]);
            } else {
                $("#Con2").val("");
            }

        } else {
            $("#Con2").val("");
            $("#construction2").val("");
            $("#construction1").val("");
            $("#Con1").val();
        
        }
        $("#finishValTable tr").remove();
        var tempStrS = strS[9].split("<4>");
        var sstb = '';
        for (i = 0; i < tempStrS.length; i++) {
            if (tempStrS[i] == "")
                continue;
            sstb = tempStrS[i].substring(tempStrS[i].indexOf("[") + 1, tempStrS[i].indexOf("]"));
            var trHtml = "<tr><td style='width:90%'>" + tempStrS[i] + "</td>";

            if (!$("#finishValTable").attr("readOnly")) {
                trHtml = trHtml + "<td align='left' style='width:10%'><input type='button' style='height:6px;border:none;width:16px;background-color:red;' onclick=deleteFinish(this,'" + sstb + "') /></td>" + "</tr>";
            } else {
                trHtml = trHtml + "</tr>";
            }
            $("#finishValTable").append(trHtml);
            tempFinishVal = tempFinishVal + ',' + sstb;
        }
    }

    function fInitRetrive(val, vPPONOFabricPart) {


        flatYarnInfoGrid.Refresh();
        var strS = val.split("<|>");


        if (strS[20] == "") {

            $("input[id$='txtQC_Ref_PPO']", "#fAttributeTable").val(vPPONOFabricPart.split("|")[0]);

        }
        else {
            $("input[id$='txtQC_Ref_PPO']", "#fAttributeTable").val(strS[20]);
        }
        $("input[id$='txtHF_Ref_PPO']", "#fAttributeTable").val(strS[22]);

        updateQC_Ref_GP();
        updateDropHF_Ref_GP();
        var count = $("#DropQC_Ref_GP option").length;
        if (strS[21] == "") {

            for (var i = 0; i < count; i++) {
                if ($("#DropQC_Ref_GP ").get(0).options[i].text == vPPONOFabricPart.split("|")[1]) {
                    $("#DropQC_Ref_GP ").get(0).options[i].selected = true;

                    break;
                }
            }

        }
        else {
            for (var i = 0; i < count; i++) {
                if ($("#DropQC_Ref_GP ").get(0).options[i].text == strS[21]) {
                    $("#DropQC_Ref_GP ").get(0).options[i].selected = true;

                    break;
                }
            }
        }

        var count2 = $("#DropHF_Ref_GP option").length;

        for (var i = 0; i < count2; i++) {
            if ($("#DropHF_Ref_GP ").get(0).options[i].text == strS[23]) {
                $("#DropHF_Ref_GP ").get(0).options[i].selected = true;

                break;
            }
        }

        //$("#TxtRemart").val(strS[24]);

        if (strS[3] == "") {
            $("select[id$='cmbPattern']", "#fAttributeTable").val(" ");
        }
        else {
            $("select[id$='cmbPattern']", "#fAttributeTable").val(strS[3]);
        }
        $("#Layout").val(strS[4]);
        $("input[id$='construction']", "#fAttributeTable").val(strS[8]);
        $("input[id$='yarnLength']", "#fAttributeTable").val(strS[11]);
        if (strS[8] != "") {
            var result = AjaxCallFun("Comfy.App.Core", "Comfy.App.Core.QualityCode.CustomerManager", "GetConstruction", strS[8]);
            $("#construction1").val(result.split("<?>")[0]);
            if (result.split("<?>")[1] != "") {
                $("#construction2").val(result.split("<?>")[1]);
            }
            else {
                $("#construction2").val("");
            }
            $("#Con1").val(strS[8].split(",")[0]);
            if (strS[8].split(",")[1] != "") {
                $("#Con2").val(strS[8].split(",")[1]);
            } else {
                $("#Con2").val("");
            }

        } else {
            $("#Con2").val("");
            $("#construction2").val("");
            $("#construction1").val("");
            $("#Con1").val();

        }
        $("#finishValTable tr").remove();
        var tempStrS = strS[9].split("<4>");
        var sstb = '';
        for (i = 0; i < tempStrS.length; i++) {
            if (tempStrS[i] == "")
                continue;
            sstb = tempStrS[i].substring(tempStrS[i].indexOf("[") + 1, tempStrS[i].indexOf("]"));
            var trHtml = "<tr><td style='width:90%'>" + tempStrS[i] + "</td>";

            if (!$("#finishValTable").attr("readOnly")) {
                trHtml = trHtml + "<td align='left' style='width:10%'><input type='button' style='height:6px;border:none;width:16px;background-color:red;' onclick=deleteFinish(this,'" + sstb + "') /></td>" + "</tr>";
            } else {
                trHtml = trHtml + "</tr>";
            }
            $("#finishValTable").append(trHtml);
            tempFinishVal = tempFinishVal + ',' + sstb;
        }
    }
</script>
