<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Attribute.ascx.cs" Inherits="Comfy.App.Web.QuailtyCode.Attribute" %>
<%@ Register TagPrefix="aspx" Namespace="Comfy.UI.WebControls.WebPopupControl" Assembly="Comfy.UI.WebControls" %>
<%@ Register TagPrefix="aspx" Namespace="Comfy.UI.WebControls.WebButtonEdit" Assembly="Comfy.UI.WebControls" %>
<%@ Register TagPrefix="myc" Namespace="Comfy.UI.WebControls.WebGridView" Assembly="Comfy.UI.WebControls" %>
<script type="text/javascript" src="../Scripts/ext-base.js"></script>
<script type="text/javascript" src="../Scripts/ext-all.js"></script>
<script src="../Scripts/jquery1.4.js" type="text/javascript"></script>
<script src="../Scripts/jquery.move.js" type="text/javascript"></script>
<script type="text/javascript" src="../Scripts/Utilities.js"></script>
<script src="../Scripts/json2.js"></script>
<script type="text/javascript">
    var pageI = 0;
    var pageJ = 100
    var Times = 0;
    var timer;
    var tempFinishVal = '';
    function setConstructionText() {
        var n = ($("input[id$='txtFabricType']").val().split(',')).length - 1;
        if ($("input[id$='txtFabricType']").val() != '' && $("input[id$='txtFabricType']").val().indexOf(cmbConstruction.GetValue().toString()) < 0 && n < 1) {
            $("input[id$='txtFabricType']").val($("input[id$='txtFabricType']").val() + ',' + cmbConstruction.GetValue().toString());
        } else if ($("input[id$='txtFabricType']").val().indexOf(cmbConstruction.GetValue().toString()) < 0 && n < 1) {
            $("input[id$='txtFabricType']").val(cmbConstruction.GetValue().toString());
        }
    }
    function clearCon() {
        $("input[id$='txtFabricType']").val('');
    }
    //    function gridAdd() {
    //        yarnInfoGrid.AddNewRow();
    //        yarnInfoGrid.SetAddPanelFieldValue("Threads", "1");
    //        yarnInfoGrid.SetAddPanelFieldValue("Radio", "100");

    //    }
    //    function gridDelete() {
    //        yarninfoGrid.DeleteRows("Seq");
    //        yarninfoGrid.Refresh();
    //    }
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
            if(vArray[i] ==oldgp)
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

    function disableAttribute() {
        $("#attrBtnAdd").attr("style", "display:none");
        $("#attrBtnDelete").attr("style", "display:none");
        $("#attrBtnEdit").attr("style", "display:none");
        $("#cchange").attr("disabled", "disabled");
        $("#cchange").css({ "background-color": "#EFEFEF" });
        $("#txtFinish").attr("disabled", "disabled");
        $("#txtFinish").css({ "background-color": "#EFEFEF" });
        $("#construction1").attr("readOnly", true);
        $("#construction1").css({ "background-color": "#EFEFEF" });
        $("#construction2").attr("readOnly", true);
        $("#construction2").css({ "background-color": "#EFEFEF" });
        $("#attrBtnClear").attr("style", "display:none");
        // $("input[id$='txtFabricType']").attr("readOnly", true);
        // $("input[id$='txtFabricType']").attr("style", "color:##808080");
        $("input[id$='txtBWWidth']").attr("readOnly", true);
        $("input[id$='txtAWWidth']").attr("readOnly", true);
        $("input[id$='txtBWWidth']").css({ "background-color": "#EFEFEF" });
        $("input[id$='txtAWWidth']").css({ "background-color": "#EFEFEF" });
        $("select[id$='cmbDyeMethod']").attr("disabled", "disabled");
        $("select[id$='cmbPattern']").attr("disabled", "disabled");
        $("select[id$='cmbPattern']").css({ "background-color": "#EFEFEF" });
        $("select[id$='cmbDyeMethod']").css({ "background-color": "#EFEFEF" });
        $("input[id$='Layout']").attr("readOnly", true);
        $("input[id$='Layout']").css({ "background-color": "#EFEFEF" });
        $("input[id$='txtShrinkage']").attr("readOnly", true);
        $("input[id$='txtShrinkage']").css({ "background-color": "#EFEFEF" });
        $("input[id$='txtOneShrinkage']").attr("readOnly", true);
        $("input[id$='txtOneShrinkage']").css({ "background-color": "#EFEFEF" });


        $("select[id$='txtGMTWash']").attr("disabled", "disabled");
        $("select[id$='cmbTextMethod']").attr("disabled", "disabled");

        $("select[id$='cmbTextMethod']").css({ "background-color": "#EFEFEF" });
        $("select[id$='txtGMTWash']").css({ "background-color": "#EFEFEF" });
        $("#finishValTable").attr("readOnly", true);
        //  Combol.SetEnabled(false);

    }


    var textSeparator = ";";







    /*
    function GetDate() {
    pageI = Times * pageJ;
    Times++;

    var param = pageI + "," + pageJ;
    var result = AjaxCallFun("Comfy.App.Core", "Comfy.App.Core.QualityCode.CustomerManager", "GetCustomerOne", param);
    var results = result.split("(@*)");
    if (results[1] == "stop") {
    clearInterval(timer);
    }
    pageI = pageJ;
    pageJ = (parseInt(pageJ) + 100);
    var trhtml = ''
    var tempColumn;
    result = results[0];
    var tempRow = result.split('<?>');
    for (i = 0; i < tempRow.length; i++) {
    if (tempRow[i] != '') {
    tempColumn = tempRow[i].split('<|>');
    trhtml = trhtml + "<tr style='height:20px;padding:0 0 0 2px;cursor: pointer;background-color:" + (i % 2 == 0 ? "#e5f1ff" : "#ffffff") + "'  onclick=''><td style='display:none'>" + tempColumn[0] + "</td><td align='left'>" + tempColumn[1] + "</td></tr>";
    }
    }
    $('#NewTable').append(trhtml);
    }*/

    $(function () {
        //  alert("ss");
        // $("#txtFabricType").hide();

        $("#yarnInfoGridFooterTable").hide();
        $("#yarnInfoGridgridFootDiv").attr("style", "width:372px;height:1px;border:1px solid #A3C0E8;");
    });
</script>

    <script type="text/javascript">
        function openLog() {
            //            var tab = Ext.getCmp("tabs");
            //            tab.remove(24);
            //            openUrl('24', 'QuailtyCode/Modify_QC_HF_Log.aspx', 'Modify QC & HF Log', 'tabs');
            var qualityCodeVal = $.trim($("#tQC").val());
            if (qualityCodeVal == '') {
                alert('请输入Quality Code再查询！');
                return;
            }
            window.open('Modify_QC_HF_Log.aspx?QualityCode=' + qualityCodeVal, '_blank')
        }
    </script>
<%--<div style="height: 435px; width: 490px; border: 1px solid #FFA200; margin: 0;">--%>
<div style="height: 540px; width: 490px; border: 1px solid #FFA200; margin: 0;">
    <table id="cAttributeTable">
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
                <asp:Label ID="lblAppName" runat="server" Text="<%$ Resources:YarnInfo %>" Width="100px"></asp:Label>
            </td>
            <td style="width: 380px" colspan="3">
                <div style="width: 378px; border: 1px solid #A3C0E8;">
                    <table>
                        <tr>
                            <td>
                                <myc:WebGridView ID="yarnInfoGrid" runat="server" DataSourceId="YarnInfoODS" PageSize="8"
                                    Height="100" Width="372" KeyFieldName="Seq" CreateAddPanel="true" CreateSearchPanel="false">
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
                                    <%--<myc:Field FieldName="YarnComponent" Caption="YarnComponent" Width="100" FieldType="TextArea">
                                    </myc:Field>--%>
                                    
                                    <myc:Field FieldName="YarnComponent" Caption="YarnComponent"  Width="215"
                                        ColumnSpan="3" FieldType="ComboBox" DataSourceId="yarnContentSQLDS" ValueField="Yarn_Content" TextField="Yarn_Content" Check="not_empty">
                                    </myc:Field>
                                </myc:WebGridView>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <input type="button" id="attrBtnAdd" value="Add" onclick="gridAdd()" class="myButton" style="height: 22px; 
                                    width: 60px" />&nbsp;&nbsp;&nbsp;<input type="button" id="attrBtnEdit" value="Edit" class="myButton"
                                        style="height: 22px; width: 60px" onclick="gridEdit()" />&nbsp;&nbsp;&nbsp;<input
                                            type="button" id="attrBtnDelete" value="Delete" style="height: 22px; width: 60px" class="myButton"
                                            onclick="gridDelete()" />
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <%--        <tr>
            <td>
                <input type="text" id="NewText" style="width: 170px" />
                <div style="width: 370px; height: 200px; overflow: auto; display: none; z-index: 10000;
                    background-color: #8BB6EF; border: 2px solid #004080" id="NewDiv">
                    <table style="width: 100%" id="NewTable">
                    </table>
                </div>
            </td>
        </tr>--%>
        <tr>
            <td style="width: 100px" align="right">
                <asp:Label ID="Label1" runat="server" Text="<%$ Resources:FabricType %>" Width="100px"></asp:Label>
            </td>
            <td style="width: 380px" colspan="3">
                <div style="border: 1px solid #A3C0E8; width: 378px">
                    <table style="width: 100%">
                        <tr>
                            <td align="left" colspan="2">
                                <input type="text" id="cchange" style="width: 370px" />
                                <div style="width: 370px; height: 200px; overflow-y: auto; overflow-x: hidden; display: none;
                                    z-index: 10000; background-color: #8BB6EF; border: 2px solid #004080" id="cSeDiv">
                                    <table style="width: 100%" id="cSelectTable">
                                    </table>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <input type="text" id="construction1" style="width: 180px; background-color: #A3C0E8" />
                                <input type="text" id="Con1" style="width: 180px; display: none" />
                            </td>
                            <td align="left">
                                <input type="text" id="construction2" style="width: 180px; background-color: #A3C0E8" />
                                <input type="text" id="Con2" style="width: 177px; display: none" />
                                <asp:TextBox ID="txtFabricType" runat="server" Width="120px" ClientIDMode="Static"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td style="width: 100px" align="right">
                <asp:Label ID="Label2" runat="server" Text="<%$ Resources:BWWeight %>" Width="100px"></asp:Label>
            </td>
            <td style="width: 140px">
                <asp:TextBox ID="txtBWWidth" runat="server" Width="138px" ClientIDMode="Static"></asp:TextBox>
            </td>
            <td style="width: 100px" align="right">
                <asp:Label ID="Label3" runat="server" Text="<%$ Resources:AWWeight %>" Width="100px"></asp:Label>
            </td>
            <td style="width: 140px" align="left">
                <asp:TextBox ID="txtAWWidth" runat="server" Width="130px" ClientIDMode="Static"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="width: 100px" align="right">
                <asp:Label ID="Label4" runat="server" Text="<%$ Resources:DyeMethod %>" Width="100px"></asp:Label>
            </td>
            <td style="width: 140px" align="left">
                <asp:DropDownList ID="cmbDyeMethod" runat="server" Width="138px" DataSourceID="dyeMethodSALDS" ClientIDMode="Static"
                    DataTextField="DESCRIPTION" DataValueField="DYE_METHOD" OnDataBound="cmbDyeMethod_DataBound">
                </asp:DropDownList>
                <%--        <asp:TextBox ID="txtDyeMethod" runat="server" Width="138px" 
                ></asp:TextBox>--%>
            </td>
            <td style="width: 100px" align="right">
                <asp:Label ID="Label5" runat="server" Text="<%$ Resources:Pattern %>" Width="100px"></asp:Label>
            </td>
            <td style="width: 140px" align="left">
                <%--  <asp:TextBox ID="txtPattern" runat="server" Width="134px" ></asp:TextBox>--%>
                <asp:DropDownList ID="cmbPattern" runat="server" Width="133px" DataSourceID="patternSQLDS" ClientIDMode="Static"
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
                                <div style="width: 370px; height: 150px; overflow-y: auto; overflow-x: hidden; display: none;
                                    z-index: 10000; background-color: #8BB6EF; border: 2px solid #004080" id="finishDiv">
                                    <table style="width: 100%" id="finishTable">
                                    </table>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <asp:HiddenField ID="finishValue" runat="server" ClientIDMode="Static"></asp:HiddenField>
                                <%-- <input type="text" id="txtFinishVal" style="width: 370px; background-color: #A3C0E8" />--%>
                                <%--  <textarea id="txtFinishVal" style="width: 370px; height: 50px; background-color: #A3C0E8"></textarea>--%>
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
        <tr>
            <td style="width: 100px" align="right">
                <asp:Label ID="Label9" runat="server" Text="<%$ Resources:TestMethod %>" Width="96px"></asp:Label>
            </td>
            <td style="width: 382px" colspan="3" align="left">
                <asp:DropDownList ID="cmbTextMethod" runat="server" Width="382px" DataSourceID="testMethodSQLDS" ClientIDMode="Static"
                    DataValueField="PARAMETER" DataTextField="PARAMETER" OnDataBound="cmbDyeMethod_DataBound">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td style="width: 100px" align="right">
                <asp:Label ID="Label7" runat="server" Text="<%$ Resources:GMTWash %>" Width="96px"></asp:Label>
            </td>
            <td style="width: 140px" colspan="1" align="left">
                <asp:DropDownList ID="txtGMTWash" runat="server" ClientIDMode="Static" Width="138px">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                    <asp:ListItem Text="N" Value="N"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style="width: 100px" align="right">
                <asp:Label ID="Label8" runat="server" Text="Shrinkage(L/W)%" Width="100px"></asp:Label>
            </td>
            <td style="width: 132px" colspan="1" align="left">
                <div>
                    <asp:TextBox ID="txtShrinkage" runat="server" ClientIDMode="Static" Width="40px"></asp:TextBox>&nbsp;&nbsp;X&nbsp;&nbsp;
                    <asp:TextBox ID="txtOneShrinkage" runat="server" ClientIDMode="Static" Width="40px"></asp:TextBox></div>
            </td>
        </tr>
        <tr>
            <td style="width: 100px" align="right">
                <asp:Label ID="Label10" runat="server" Text="<%$ Resources:Layout %>" Width="100px"></asp:Label>
            </td>
            <td style="width: 380px" colspan="3" align="left">
                <asp:TextBox ID="Layout"  runat="server" Width="374px" TextMode="MultiLine" ClientIDMode="Static" Height="40px"></asp:TextBox>
            </td>
        </tr>
       <%-- <tr id="QCRef" style="display:block;">--%>
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
                     <asp:DropDownList ID="DropQC_Ref_GP" runat="server" ClientIDMode="Static">
                </asp:DropDownList>
            &nbsp;
                                <input type="button" id="historyBtn" value="Log View" 
                         onclick="openLog();" class="myButton" style="height: 22px; 
                                    width: 60px" /></td>
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
                   <asp:DropDownList ID="DropHF_Ref_GP" runat="server"  ClientIDMode="Static">
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
<asp:SqlDataSource ID="testMethodSQLDS" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
    ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="SELECT PARAMETER, IS_ACTIVE FROM PBKNITPARAMETER WHERE (&quot;SORT&quot; = 'Testing_Method' and IS_Active='Y')">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dyeMethodSALDS" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
    ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="SELECT DYE_METHOD,DESCRIPTION FROM PBKNITDYEMETHOD">
</asp:SqlDataSource>
<asp:SqlDataSource ID="patternSQLDS" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
    ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="SELECT PARAMETER, IS_ACTIVE FROM PBKNITPARAMETER WHERE (&quot;SORT&quot; = 'PATTERN' and IS_Active='Y')">
</asp:SqlDataSource>
<asp:SqlDataSource ID="finishSQLDS" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
    ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="SELECT Finishing_Code, Description||'('||Finishing_Code||')' as Description FROM pbKnitFinish where Washing_Flag='N' and IS_Active='Y' order by Description asc">
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
<asp:SqlDataSource ID="yarnContentSQLDS" runat="server" ConnectionString="<%$ ConnectionStrings:SqlServer %>"
    ProviderName="<%$ ConnectionStrings:SqlServer.ProviderName %>" SelectCommand="select Yarn_Content from [SystemDB].dbo.pbYarnTypeContentList WHERE Yarn_Content!='' Group by Yarn_Content">
</asp:SqlDataSource>
<%--<asp:ObjectDataSource ID="YarnInfoODS" runat="server" SelectMethod="GetYarnInfo" 
    InsertMethod="AddYarnInfo" DeleteMethod="DeleteYarnInfo" TypeName="Comfy.App.Web.QuailtyCode.Attribute"
    DataObjectTypeName="Comfy.App.Core.QualityCode.YarnInfo">
    <SelectParameters>
        <asp:Parameter Name="orderByField" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>--%>
<script>
    function getFAttribute(strPage) {
        var Con = ($("#Con1").val() == "" ? "" : ($("#Con1").val() + ";")) + ($("#Con2").val() == "" ? "" : ($("#Con2").val() + ";"));
        if (Con.indexOf(";") > 0) {
            Con = Con.substring(0, Con.length - 1);
        }
        var BW = $("#txtBWWidth").val();
        var AW = $("#txtAWWidth").val();
        var DM = $("#cmbDyeMethod").val();
        var Pattern = $("#cmbPattern").val();
        var TM = $("#cmbTextMethod").val();
        var GM = $("#txtGMTWash").val();
        var SK = $("#txtShrinkage").val() + "X" + $("#txtOneShrinkage").val();
        var Yarn = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode." + strPage, "GetYarnInfo", "1");
        if (Yarn.indexOf(";") > 0) {
            Yarn = Yarn.substring(0, Yarn.length - 1);
        }
        var Finish = getFinishVal();
        if (Finish.indexOf(";") > 0) {
            Finish = Finish.substring(0, Finish.length - 1);
        }
        return Con + "<>" + BW + "<>" + AW + "<>" + DM + "<>" + Pattern + "<>" + TM + "<>" + GM
                + "<>" + SK + "<>" + Finish + "<>" + Yarn;

    }

    function getFAttributeForUpdate() {
        var Con = ($("#Con1").val() == "" ? "" : ($("#Con1").val() + ",")) + ($("#Con2").val() == "" ? "" : ($("#Con2").val() + ","));
        if (Con.indexOf(";") > 0) {
            Con = Con.substring(0, Con.length - 1);
        }
        var BW = $("#txtBWWidth").val();
        var AW = $("#txtAWWidth").val();
        var DM = $("#cmbDyeMethod").val();
        var Pattern = $("#cmbPattern").val();
        var TM = $("#cmbTextMethod").val();
        var GM = $("#txtGMTWash").val();
        var SK = $("#txtShrinkage").val() + "X" + $("#txtOneShrinkage").val();
        var LY = $("#Layout").val();

        var vtxtQC_Ref_PPO = $("#txtQC_Ref_PPO").val();
        var vDropQC_Ref_GP = $("#DropQC_Ref_GP").val();
        var vtxtHF_Ref_PPO = $("#txtHF_Ref_PPO").val();
        var vDropHF_Ref_GP = $("#DropHF_Ref_GP").val();
        var vTxtRemart = $("#TxtRemart").val();
//        alert(document.getElementById("DropQC_Ref_GP"));
        
        var Finish = getFinishVal();
        if (Finish.indexOf(";") > 0) {
            Finish = Finish.substring(0, Finish.length - 1);
        }
        return Con + "<>" + BW + "<>" + AW + "<>" + DM + "<>" + Pattern + "<>" + TM + "<>" + GM
                + "<>" + SK + "<>" + Finish + "<>" + LY + "<>" + vtxtQC_Ref_PPO + "<>" + vDropQC_Ref_GP + "<>" + vtxtHF_Ref_PPO + "<>" + vDropHF_Ref_GP + "<>" + vTxtRemart;

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
        $("form").submit(function (e) {
            $("#finishValue").get(0).value = getFinishVal();
        });
    })
    $(function () {
        $("#txtFabricType").attr("style", "display:none");
       // $("#cmbDyeMethod").val("");
        $("#construction1").bind('keyup', function (event) {
            if ($("#construction1").attr("readOnly")) {
                return;
            }
            $("#construction1").val("");
            $("#Con1").val("");
            SetFabricTypeVal();
        });
        $("#construction2").bind('keyup', function (event) {
            if ($("#construction2").attr("readOnly")) {
                return;
            }
            $("#construction2").val("");
            $("#Con2").val("");
            SetFabricTypeVal();
        });

        $("#txtAWWidth").change(function () {
            if ($("#txtAWWidth").val() != "" && isNaN($("#txtAWWidth").val())) {
                $("#txtAWWidth").val("");
                alert("Please input a number!");
            }
        });

        $("#txtBWWidth").change(function () {
            if ($("#txtBWWidth").val() != "" && isNaN($("#txtBWWidth").val())) {
                $("#txtBWWidth").val("");
                alert("Please input a number!");
            }
        });
    });
    

    function gridAdd() {
        yarnInfoGrid.AddNewRow();
        yarnInfoGrid.SetAddPanelFieldValue("Threads", "1");
        yarnInfoGrid.SetAddPanelFieldValue("Radio", "100");

        $("#ctl06_yarnInfoGridyarnInfoGridYarnComponent").empty();
        $("#ctl05_yarnInfoGridyarnInfoGridYarnComponent").empty();
    }
    function gridEdit() {
        //获取纱类型并对字符进行截取
        var yarnType = yarnInfoGrid.currentActiveRow.children[1].innerText;
        var index = yarnType.indexOf('-->');
        yarnType = yarnType.substring(index+3, yarnType.length);
        //获取纱内容
        var yarnContent = yarnInfoGrid.currentActiveRow.children[5].innerText;


        yarnInfoGrid.EditOneRow();
        // yarnInfoGrid.Refresh(); 

        getYranContentData106(yarnType, yarnContent);
        getYranContentData105(yarnType, yarnContent);     
    }


    function getYranContentData105(yarnType, yarnContent) {
        //调用获取AJAX数据接口来获取二级关联的数据
        $.ajax({              
            type: "post", //要用post方式                 
            url: "GetAjaxData.aspx/GetYranContent",//方法所在页面和方法名
            contentType: "application/json; charset=utf-8",     
            dataType: "json",
            data: "{\"yranType\":\""+yarnType+"\"}",
            success: function(data) {                    
                //alert(data.d);//返回的数据用data.d获取内容
                $("#ctl05_yarnInfoGridyarnInfoGridYarnComponent").empty(); 
                if (isJSON_test(data.d)) {
                    var jsonObj = JSON.parse(data.d);
                    for (var i = 0; i < jsonObj.length; i++) {
                        $("#ctl05_yarnInfoGridyarnInfoGridYarnComponent").append("<option value='" + jsonObj[i]["Yarn_Content"] + "'>" + jsonObj[i]["Yarn_Content"] + "</option>");
                    }
                    $("#ctl05_yarnInfoGridyarnInfoGridYarnComponent").val(yarnContent);
                } else {
                    alert("YarnContent No Data!");
                }
                
            },
            error: function(err) {     
    
            }     
        });
    }

    function getYranContentData106(yarnType, yarnContent) {
        //调用获取AJAX数据接口来获取二级关联的数据
        $.ajax({              
            type: "post", //要用post方式                 
            url: "GetAjaxData.aspx/GetYranContent",//方法所在页面和方法名
            contentType: "application/json; charset=utf-8",     
            dataType: "json",
            data: "{\"yranType\":\""+yarnType+"\"}",
            success: function(data) {                    
                //alert(data.d);//返回的数据用data.d获取内容
                $("#ctl06_yarnInfoGridyarnInfoGridYarnComponent").empty(); 
                if (isJSON_test(data.d)) {
                    var jsonObj = JSON.parse(data.d); 
                    for (var i=0;i<jsonObj.length;i++){
                        $("#ctl06_yarnInfoGridyarnInfoGridYarnComponent").append("<option value='"+jsonObj[i]["Yarn_Content"]+"'>"+jsonObj[i]["Yarn_Content"]+"</option>");
                    }
                    $("#ctl06_yarnInfoGridyarnInfoGridYarnComponent").val(yarnContent);
                }else {
                    alert("YarnContent No Data!");
                }                
            },
            error: function(err) {     
    
            }     
        });
    }
    
    function gridDelete() {
        yarnInfoGrid.DeleteRows("Seq");
        yarnInfoGrid.Refresh();
    }
    var cCombo = new Combo("cchange", "cSeDiv", "cSelectTable", "370px", "200px", "Comfy.App.Core", "Comfy.App.Core.QualityCode.CustomerManager", "GetConstructionInfo", "cCombo");
    function cComboClick(text, val) {
        if ($("#construction1").val() == "") {
            $("#construction1").val(text);
            $("#Con1").val(val);
        } else if ($("#construction2").val() == "") {
            $("#construction2").val(text);
            $("#Con2").val(val);
        } else {
            alert("Only can choose one or two construction!");
        }
        SetFabricTypeVal();
        $("#cchange").val("");
    }

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
        $("#txtFabricType").val(tempStr);
    }

    function cInit(val, vPPONOFabricPart) {
  
        yarnInfoGrid.Refresh();
        var i, j;
        var strS = val.split("<|>");
        $("input[id$='txtBWWidth']", "#cAttributeTable").val(strS[0]);
        $("input[id$='txtAWWidth']", "#cAttributeTable").val(strS[1]);
        $("select[id$='cmbDyeMethod']", "#cAttributeTable").val(strS[2]);
        $("select[id$='cmbPattern']", "#cAttributeTable").val(strS[3]);
        $("#Layout").val(strS[4]);
        $("input[id$='txtShrinkage']", "#cAttributeTable").val(strS[5].split("X")[0]);
        $("input[id$='txtOneShrinkage']", "#cAttributeTable").val(strS[5].split("X")[1]);
        $("select[id$='cmbTextMethod']", "#cAttributeTable").val(strS[6]);
        $("select[id$='txtGMTWash']", "#cAttributeTable").val(strS[7]);
        $("input[id$='txtFabricType']", "#cAttributeTable").val(strS[8]);
        //add by linyob 20181218 先初始化为空字符串,不然搜索下一个qc的时候ref ppo为空字符串 就不会覆盖上一个qc的 ref ppo
        $("input[id$='txtQC_Ref_PPO']", "#cAttributeTable").val('');

        if (strS[20] == "") {
            if (vPPONOFabricPart != "") {
                $("input[id$='txtQC_Ref_PPO']", "#cAttributeTable").val(vPPONOFabricPart.split("|")[0]);
            }
        }
        else {
            $("input[id$='txtQC_Ref_PPO']", "#cAttributeTable").val(strS[20]);
        }
        $("input[id$='txtHF_Ref_PPO']", "#cAttributeTable").val(strS[22]);

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

        if (strS[8] != "") {
            var result = AjaxCallFun("Comfy.App.Core", "Comfy.App.Core.QualityCode.CustomerManager", "GetConstruction", strS[8]);
            $("#construction1").val(result.split("<?>")[0]);
            if (result.split("<?>")[1] != "") {
                $("#construction2").val(result.split("<?>")[1]);
            } else {
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
        // checkListBox.UnselectAll();
        // checkListBox.SelectValues(tempStrS);

        // UpdateText();
        return;
        /*    var arr = new Array();
        if (strS[9] != "") {
        var tempStrS = strS[9].split(",");
        GetChildrenNodes($("span[id$='cbFinishing']", "#cAttributeTable").get(0), arr, ""); //获取checkboxlist 的所有元素

        for (j = 0; j < arr.length; j++) {
        if (arr[j].type == "checkbox") {
        $(arr[j]).attr("checked", false);
        }
        }
        for (i = 0; i < tempStrS.length; i++) {
        for (j = 0; j < arr.length; j++) {
        if ($(arr[j]).text().indexOf("(" + tempStrS[i] + ")") > 0) {
        $(arr[j]).prev().attr("checked", true);
        break;
        }
        }
        }
        }*/
    }

    function cInitRetrive(val, vPPONOFabricPart) {
        var DropQC_Ref_GP = $("#DropQC_Ref_GP ").val();
        yarnInfoGrid.Refresh();
        var i, j;
        var strS = val.split("<|>");
        $("input[id$='txtBWWidth']", "#cAttributeTable").val(strS[0]);
        $("input[id$='txtAWWidth']", "#cAttributeTable").val(strS[1]);
        $("select[id$='cmbDyeMethod']", "#cAttributeTable").val(strS[2]);
        $("select[id$='cmbPattern']", "#cAttributeTable").val(strS[3]);
        $("#Layout").val(strS[4]);
        $("input[id$='txtShrinkage']", "#cAttributeTable").val(strS[5].split("X")[0]);
        $("input[id$='txtOneShrinkage']", "#cAttributeTable").val(strS[5].split("X")[1]);
        $("select[id$='cmbTextMethod']", "#cAttributeTable").val(strS[6]);
        $("select[id$='txtGMTWash']", "#cAttributeTable").val(strS[7]);
        $("input[id$='txtFabricType']", "#cAttributeTable").val(strS[8]);

        if (strS[20] == "") {
            if (vPPONOFabricPart != "") {
                $("input[id$='txtQC_Ref_PPO']", "#cAttributeTable").val(vPPONOFabricPart.split("|")[0]);
            }
        }
        else {
            $("input[id$='txtQC_Ref_PPO']", "#cAttributeTable").val(strS[20]);
        }

        if (strS[22] == "") {
            $("input[id$='txtHF_Ref_PPO']", "#cAttributeTable").val(strS[22]);
        }
        

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
        if (strS[23] == "" || strS[23] == undefined) {
            for (var i = 0; i < count2; i++) {
                if ($("#DropHF_Ref_GP ").get(0).options[i].text ==  DropQC_Ref_GP) {
                    $("#DropHF_Ref_GP ").get(0).options[i].selected = true;

                    break;
                }
            }
        } else {
            for (var i = 0; i < count2; i++) {
                if ($("#DropHF_Ref_GP ").get(0).options[i].text == strS[23]) {
                    $("#DropHF_Ref_GP ").get(0).options[i].selected = true;

                    break;
                }
            }
        }


  

        //$("#TxtRemart").val(strS[24]);

        if (strS[8] != "") {
            var result = AjaxCallFun("Comfy.App.Core", "Comfy.App.Core.QualityCode.CustomerManager", "GetConstruction", strS[8]);
            $("#construction1").val(result.split("<?>")[0]);
            if (result.split("<?>")[1] != "") {
                $("#construction2").val(result.split("<?>")[1]);
            } else {
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

        return;
        
    }

    $("#ctl06_yarnInfoGridyarnInfoGridYarnType").change(function () {  
         //初始化yran_content
        var yarnType = $("#ctl06_yarnInfoGridyarnInfoGridYarnType").val();
        //调用获取AJAX数据接口来获取二级关联的数据
        $.ajax({              
            type: "post", //要用post方式                 
            url: "GetAjaxData.aspx/GetYranContent",//方法所在页面和方法名
            contentType: "application/json; charset=utf-8",     
            dataType: "json",
            data: "{\"yranType\":\""+yarnType+"\"}",
            success: function(data) {                    
                //alert(data.d);//返回的数据用data.d获取内容                
                $("#ctl06_yarnInfoGridyarnInfoGridYarnComponent").empty(); 
                if (isJSON_test(data.d)) {
                    var jsonObj = JSON.parse(data.d);
                    for (var i = 0; i < jsonObj.length; i++) {
                        $("#ctl06_yarnInfoGridyarnInfoGridYarnComponent").append("<option value='" + jsonObj[i]["Yarn_Content"] + "'>" + jsonObj[i]["Yarn_Content"] + "</option>");
                    }
                } else {
                    alert("YarnContent No Data!");
                }                
            },
            error: function(err) {     
                    
            }     
        });
    });

    $("#ctl05_yarnInfoGridyarnInfoGridYarnType").change(function () {  
         //初始化yran_content
        var yarnType = $("#ctl05_yarnInfoGridyarnInfoGridYarnType").val();
        //调用获取AJAX数据接口来获取二级关联的数据
        $.ajax({              
            type: "post", //要用post方式                 
            url: "GetAjaxData.aspx/GetYranContent",//方法所在页面和方法名
            contentType: "application/json; charset=utf-8",     
            dataType: "json",
            data: "{\"yranType\":\""+yarnType+"\"}",
            success: function(data) {                    
                //alert(data.d);//返回的数据用data.d获取内容                
                $("#ctl05_yarnInfoGridyarnInfoGridYarnComponent").empty();
                if (isJSON_test(data.d)) {
                    var jsonObj = JSON.parse(data.d);
                    for (var i=0;i<jsonObj.length;i++){
                        $("#ctl05_yarnInfoGridyarnInfoGridYarnComponent").append("<option value='"+jsonObj[i]["Yarn_Content"]+"'>"+jsonObj[i]["Yarn_Content"]+"</option>");
                    }
                }else {
                    alert("YarnContent No Data!");
                }                    
            },
            error: function(err) {     
                    
            }     
        });
    });

    function isJSON_test(str) {
        if (typeof str == 'string') {
            try {
                var obj=JSON.parse(str);
                return true;
            } catch(e) {
                return false;
            }
        }
        console.log('It is not a string!')
 }　
    
</script>
