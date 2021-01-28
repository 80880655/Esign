<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TappingAttribute.ascx.cs"
    Inherits="Comfy.App.Web.QuailtyCode.TappingAttribute" %>
<%@ Register TagPrefix="aspx" Namespace="Comfy.UI.WebControls.WebPopupControl" Assembly="Comfy.UI.WebControls" %>
<%@ Register TagPrefix="aspx" Namespace="Comfy.UI.WebControls.WebButtonEdit" Assembly="Comfy.UI.WebControls" %>
<%@ Register TagPrefix="myc" Namespace="Comfy.UI.WebControls.WebGridView" Assembly="Comfy.UI.WebControls" %>
<%@ Register TagPrefix="myc" Namespace="Comfy.UI.WebControls.WebGridView" Assembly="Comfy.UI.WebControls" %>
<script type="text/javascript">
    function disableTapAttribute() {
        $("#tapAdd").attr("style", "display:none");
        $("#tapDelete").attr("style", "display:none");
        $("#tapEdit").attr("style", "display:none");
        $("input[id$='size']").attr("readOnly", true);
        $("input[id$='size']").css({ "background-color": "#EFEFEF" });
        $("select[id$='tappingType']").attr("disabled", "disabled");
        $("select[id$='tappingType']").css({ "background-color": "#EFEFEF" });
        $("input[id$='yarnLength']").attr("readOnly", true);
        $("input[id$='yarnLength']").css({ "background-color": "#EFEFEF" });
        $("input[id$='Layout']").attr("readOnly", true);
        $("input[id$='Layout']").css({ "background-color": "#EFEFEF" });
        // $("input[id$='yarnLength']").css({ "color": "#808080" });
        // $("input[id$='size']").css({ "color": "#808080" });
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
        if ($("#txtQC_Ref_PPO").val() == '') return;
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

    function clearCon() {
        $("input[id$='txtFabricType']").val('');
    }
    function tapGridAdd() {
        // WarpWeft和YarnDesity为非必填项 by LYH 2014/2/25
        //$("#tapYarnInfoGridtapYarnInfoGridWarpWeft").attr("required", "false");
        //$("#tapYarnInfoGridtapYarnInfoGridYarnDensity").attr("required", "false");

        tapYarnInfoGrid.AddNewRow();
        tapYarnInfoGrid.SetAddPanelFieldValue("Threads", "1");
        tapYarnInfoGrid.SetAddPanelFieldValue("Radio", "100");
    }
    function tapGridEdit() {
        tapYarnInfoGrid.EditOneRow();
        // yarnInfoGrid.Refresh();
    }
    function tapGridDelete() {
        tapYarnInfoGrid.DeleteRows("Seq");
        tapYarnInfoGrid.Refresh();
    }
    $(function () {
        $("#tapYarnInfoGridFooterTable").hide();
        $("#tapYarnInfoGridgridFootDiv").attr("style", "width:372px;height:1px;border:1px solid #A3C0E8;");
    });
</script>
<div style="border: 1px solid #FFA200; width: 490px">
    <table id="tAttributeTable">
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
                <asp:Label ID="label22" runat="server" Text="Yarn Info" Width="100px"></asp:Label>
            </td>
            <td style="width: 380px" colspan="3" align="left">
                <div style="width: 378px; border: 1px solid #A3C0E8;">
                    <table>
                        <tr>
                            <td>
                                <myc:WebGridView ID="tapYarnInfoGrid" runat="server" DataSourceId="YarnInfoODS" PageSize="18"
                                    Height="100" Width="372" KeyFieldName="Seq" CreateAddPanel="true" CreateSearchPanel="false">
                                    <myc:Field FieldName="Seq" Caption="Seq" ShowOnEditForm="false" Width="25">
                                    </myc:Field>
                                    <myc:Field FieldName="YarnType" Caption="YarnType" DataSourceId="yarnTypeSQLDS" Width="315"
                                        ColumnSpan="3" FieldType="ComboBox" ValueField="Yarn_Type" TextField="Description"
                                        Check="not_empty">
                                    </myc:Field>
                                    <myc:Field FieldName="YarnCount" Caption="YarnCount" DataSourceId="yarnCountSQLDS"
                                        Width="58" FieldType="ComboBox" ValueField="Yarn_Count" TextField="Yarn_Count"
                                        Check="not_empty">
                                    </myc:Field>
                                    <myc:Field FieldName="WarpWeft" Caption="WarpWeft" Width="60">
                                    </myc:Field>
                                    <myc:Field FieldName="YarnDensity" Caption="YarnDensity" Width="60">
                                    </myc:Field>
                                    <myc:Field FieldName="Radio" Caption="Ratio" Width="60" Check="numeric">
                                    </myc:Field>
                                    <myc:Field FieldName="Threads" Caption="Strands" Width="60" Check="integer">
                                    </myc:Field>
                                    <myc:Field FieldName="YarnComponent" Caption="YarnComponent" Width="100" FieldType="TextArea">
                                    </myc:Field>
                                </myc:WebGridView>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <input type="button" value="Add" id="tapAdd" onclick="tapGridAdd()" class="myButton" style="height: 20px; 
                                    width: 60px" />&nbsp;&nbsp;&nbsp;<input type="button" id="tapEdit" value="Edit" class="myButton" style="height: 22px;
                                        width: 60px" onclick="tapGridEdit()" />&nbsp;&nbsp;&nbsp;<input type="button" class="myButton" value="Delete"
                                            id="tapDelete" style="height: 20px; width: 60px" onclick="tapGridDelete()" />
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td style="width: 100px" align="right">
                <asp:Label ID="Label4" runat="server" Text="Yarn Length" Width="100px"></asp:Label>
            </td>
            <td style="width: 140px" align="left">
                <asp:TextBox ID="yarnLength" runat="server" Width="138px" ClientIDMode="Static"></asp:TextBox>
            </td>
            <td style="width: 100px" align="right">
                <asp:Label ID="Label1" runat="server" Text="Size" Width="91px"></asp:Label>
            </td>
            <td style="width: 140px" align="left">
                <asp:TextBox ID="size" runat="server" Width="130px" ClientIDMode="Static"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="width: 100px" align="right">
                <asp:Label ID="qwewq" runat="server" Text="Tapping Type" Width="100px"></asp:Label>
            </td>
            <td style="width: 142px" align="left">
                <asp:DropDownList ID="tappingType" runat="server" Width="142px" DataSourceID="tappingTypeSQLDS" ClientIDMode="Static"
                    DataTextField="PARAMETER" DataValueField="PARAMETER" OnDataBound="cmbDyeMethod_DataBound">
                </asp:DropDownList>
            </td>
            <td colspan="2">
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
                     <asp:DropDownList ID="DropQC_Ref_GP" ClientIDMode="Static" runat="server">
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
                   <asp:DropDownList ID="DropHF_Ref_GP" ClientIDMode="Static" runat="server">
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
<asp:SqlDataSource ID="tappingTypeSQLDS" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
    ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="SELECT PARAMETER, IS_ACTIVE FROM PBKNITPARAMETER WHERE (&quot;SORT&quot; = 'TAPPING_TYPE' and IS_Active='Y')">
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
<%--<asp:ObjectDataSource ID="TapYarnInfoODS" runat="server" SelectMethod="GetYarnInfo"
    InsertMethod="AddYarnInfo" DeleteMethod="DeleteYarnInfo" TypeName="Comfy.App.Web.QuailtyCode.TappingAttribute"
    DataObjectTypeName="Comfy.App.Core.QualityCode.YarnInfo">
    <SelectParameters>
        <asp:Parameter Name="orderByField" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>--%>
<script>

    function getFAttribute(strPage) {
        var YL = $("#yarnLength").val();
        var Size = $("#size").val();
        var TT = $("#tappingType").val();
        var Yarn = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode."+strPage, "GetYarnInfo", "1");
        if (Yarn.indexOf(";") > 0) {
            Yarn = Yarn.substring(0, Yarn.length - 1);
        }
        return YL + "<>" + Size + "<>" + TT + "<>" + Yarn;

    }

    function getFAttributeForUpdate() {
        var YL = $("#yarnLength").val();
        var Size = $("#size").val();
        var TT = $("#tappingType").val();
        var LY = $("#Layout").val();
        var vtxtQC_Ref_PPO = $("#txtQC_Ref_PPO").val();
        var vDropQC_Ref_GP = $("#DropQC_Ref_GP").val();
        var vtxtHF_Ref_PPO = $("#txtHF_Ref_PPO").val();
        var vDropHF_Ref_GP = $("#DropHF_Ref_GP").val();
        var vTxtRemart = $("#TxtRemart").val();

        return YL + "<>" + Size + "<>" + TT + "<>" + LY + "<>" + vtxtQC_Ref_PPO + "<>" + vDropQC_Ref_GP + "<>" + vtxtHF_Ref_PPO + "<>" + vDropHF_Ref_GP + "<>" + vTxtRemart; 

    }

    function tInit(val, vPPONOFabricPart) {
        tapYarnInfoGrid.Refresh();
        var strS = val.split("<|>");
        $("input[id$='size']", "#tAttributeTable").val(strS[12]);
        $("#Layout").val(strS[4]);
        $("select[id$='tappingType']", "#tAttributeTable").val(strS[13]);
        $("input[id$='yarnLength']", "#tAttributeTable").val(strS[11]);

//        if (strS[20] == "") {
//            $("#txtQC_Ref").val(vPPONOFabricPart);
//        }
//        else {
//            $("#txtQC_Ref").val(strS[20]);
//        }
        //        $("#txtHandfeelRef").val(strS[21]);
 
        if (strS[20] == "") {

            $("input[id$='txtQC_Ref_PPO']", "#tAttributeTable").val(vPPONOFabricPart.split("|")[0]);

        }
        else {
            $("input[id$='txtQC_Ref_PPO']", "#tAttributeTable").val(strS[20]);
        }
        $("input[id$='txtHF_Ref_PPO']", "#tAttributeTable").val(strS[22]);

        updateQC_Ref_GP();
        updateDropHF_Ref_GP();
        var count = $("#DropQC_Ref_GP option").length;

        if (strS[21] == "") {
            if (vPPONOFabricPart != "") {
                for (var i = 0; i < count; i++) {
                    if ($("#DropQC_Ref_GP ").get(0).options[i].text == vPPONOFabricPart.split("|")[1]) {
                        $("#DropQC_Ref_GP ").get(0).options[i].selected = true;

                        break;
                    }
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

    }
    function tInitRetrive(val, vPPONOFabricPart) {
        tapYarnInfoGrid.Refresh();
        var strS = val.split("<|>");
        $("input[id$='size']", "#tAttributeTable").val(strS[12]);
        $("#Layout").val(strS[4]);
        $("select[id$='tappingType']", "#tAttributeTable").val(strS[13]);
        $("input[id$='yarnLength']", "#tAttributeTable").val(strS[11]);

        //        if (strS[20] == "") {
        //            $("#txtQC_Ref").val(vPPONOFabricPart);
        //        }
        //        else {
        //            $("#txtQC_Ref").val(strS[20]);
        //        }
        //        $("#txtHandfeelRef").val(strS[21]);

        if (strS[20] == "") {

            $("input[id$='txtQC_Ref_PPO']", "#tAttributeTable").val(vPPONOFabricPart.split("|")[0]);

        }
        else {
            $("input[id$='txtQC_Ref_PPO']", "#tAttributeTable").val(strS[20]);
        }
        $("input[id$='txtHF_Ref_PPO']", "#tAttributeTable").val(strS[22]);

        updateQC_Ref_GP();
        updateDropHF_Ref_GP();
        var count = $("#DropQC_Ref_GP option").length;

        if (strS[21] == "") {
            if (vPPONOFabricPart != "") {
                for (var i = 0; i < count; i++) {
                    if ($("#DropQC_Ref_GP ").get(0).options[i].text == vPPONOFabricPart.split("|")[1]) {
                        $("#DropQC_Ref_GP ").get(0).options[i].selected = true;

                        break;
                    }
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

    }
</script>
