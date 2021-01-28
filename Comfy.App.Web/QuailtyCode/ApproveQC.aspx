<%@ Page Language="C#" enableEventValidation="false" AutoEventWireup="true" CodeBehind="ApproveQC.aspx.cs" Inherits="Comfy.App.Web.QuailtyCode.ApproveQC" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register TagPrefix="Aspx" Namespace="Comfy.UI.WebControls.WebGridView" Assembly="Comfy.UI.WebControls" %>
<%@ Register Src="Attribute.ascx" TagName="Attribute" TagPrefix="ASPx" %>
<%@ Register Src="AvaWidth.ascx" TagName="Ava" TagPrefix="ASPx" %>
<%@ Register Src="CustomerEditForm.ascx" TagName="CE" TagPrefix="ASPx" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="../Scripts/jquery1.4.js" type="text/javascript"></script>
    <script type="text/javascript" src="../Scripts/Utilities.js"></script>
    <script src="../Scripts/Combo.js" type="text/javascript"></script>
    <title></title>
    <style>
        .mydiv
        {
            border: #909090 1px solid;
            background: #fff;
            color: #333;
            filter: progid:DXImageTransform.Microsoft.Shadow(color=#909090,direction=120,strength=4); /*ie*/
            -moz-box-shadow: 2px 2px 10px #909090; /*firefox*/
            -webkit-box-shadow: 2px 2px 10px #909090; /*safari或chrome*/
            box-shadow: 2px 2px 10px #909090; /*opera或ie9*/
        }
        .myButton
        {
             border:1px solid #FFA200;
            }
    </style>
    <script>

        //add by sunny 定义一个全局变量，用来查出系统里已有的并且审批过的相同的quality code
        var GetApprovedQc="";
        var GMType;
        var param = new Array();
        var searchStr = {};
        $(function () {
            $("#SalesComments").attr("readOnly", true);
            //  $("#SalesComments").css({ "color": "#808080" });
            $("#SalesComments").css({ "background-color": "#EFEFEF" });
            $("#tQC").attr("style", "display:none");
            $("#tCustomerId").attr("style", "display:none");
        })
        function setGMType(val, QC, cuId, CustomerComment) {

            GMType = val;
            if (QC != "" && QC!="undefined") {
                $("#txtQualityCode").val(QC);
                param.push(QC);
                param.push(CustomerComment);
                param.push(cuId);
                param.push("");
                param.push("");
                $(function () {
                    // $("#tQC").val(QC);

                    setTimeout("InitQCForm(param)", 400);
                });
            }
            else {
                $('#txtQualityCode').val('');
            }
            parent.qcValue = '';
        }
        function OpenSearch() {
            //  alert('ss');

            return "<span style='cursor:pointer;color:Blue;' onclick='searchInfo()'><u>Open</u></span>";

        }

    </script>
    <style type="text/css">
        html
        {
            overflow: auto;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" style="margin: 0; padding: 0">
    <center>
        <div style="width: 950px; margin: 0; padding: 0">
            <table style="width: 100%; margin: 0; padding: 0">
                <tr style="height: 1px">
                    <td style="width: 480px">
                    </td>
                    <td style="width: 5px">
                    </td>
                    <td style="width: 445px">
                        <asp:TextBox runat="server" ID="tQC" ClientIDMode="Static"></asp:TextBox>
                        <asp:TextBox runat="server" ID="tCustomerId" ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left" valign="top">
                        <div style="height: 30px; width: 100%">
                            <table style="width: 100%">
                                <tr>
                                    <td align="left">
                                        PPO List&nbsp;&nbsp;
                                        <input type="button" value="Refresh"  style="width:60px" onclick="GridRefresh()" class="myButton" />
                                          <%--<asp:TextBox runat="server" ID="txtMG" ClientIDMode="Static" Visible="false"></asp:TextBox>--%>                                          <%--<asp:HiddenField  runat="server" ID="txtMG" ClientIDMode="Static" Visible="false" />--%>
                                          <input name="ppoGridSearchMG" type="text" id="ppoGridSearchMG" style="display:none" />
                                    </td>
                                    <td align="left">
                                        <input type="button" value="Search" style="width:60px" onclick="SearchQC()" class="myButton" />
                                    </td>
                                    <td align="right">
                                        <div style="padding: 0 8px 0 0">
                                            QualityCode<input id="txtQualityCode" type="text" readonly="readonly" style="width: 90px;" /></div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div style="border:1px solid #FFA200;width:438px;">
                            <ASPx:WebGridView ID="ppoGrid" runat="server" DataSourceId="ppoOBDS" PageSize="10"
                                Width="436" Height="220" CreateSearchPanel="true" CreateAddPanel="false" KeyFieldName="PPONO"
                                OnClick="InitQC()" OnDbClick="searchInfo()">
                                <ASPx:Field FieldName="PPONO" Caption="PPONO" width="100" ShowOnSearchForm="true">
                                </ASPx:Field>
                                <ASPx:Field FieldName="QualityCode" Caption="QualityCode" width="80" ShowOnSearchForm="true">
                                </ASPx:Field>
                                <ASPx:Field FieldName="FabricPart" Caption="FabricPart" width="60">
                                </ASPx:Field>
                                <ASPx:Field FieldName="CustomerId" Caption="CustomerId" ShowOnSearchForm="true" >
                                </ASPx:Field>
                                <ASPx:Field FieldName="CustomerName" Caption="CustomerName" >
                                </ASPx:Field>
                                <ASPx:Field FieldName="FabricCode" Caption="FabricCode">
                                </ASPx:Field>
                                <ASPx:Field FieldName="ComboCode" Caption="ComboCode">
                                </ASPx:Field>
                                <ASPx:Field FieldName="ComboName" Caption="ComboName">
                                </ASPx:Field>
                                <ASPx:Field FieldName="SampleApprove" Caption="SampleApprove">
                                </ASPx:Field>
                                <ASPx:Field FieldName="Iden" Caption="Iden" visible="false">
                                </ASPx:Field>
                                <ASPx:Field FieldName="CustomerComment" Caption="CustomerComment" visible="false">
                                </ASPx:Field>
                                <ASPx:Field FieldName="FabricWidth" Caption="FabricWidth">
                                </ASPx:Field>
                                <ASPx:Field FieldName="LastModiDate" Caption="LastModiDate" FieldType="Date" DateFormat="yyyy-MM-dd hh:mm:ss">
                                </ASPx:Field>
                                <ASPx:Field FieldName="LastModiUserId" Caption="LastModiUserId">
                                </ASPx:Field>
                                <ASPx:Field FieldName="ViewFlag" Caption="ViewFlag" visible="false">
                                </ASPx:Field>
                            </ASPx:WebGridView>
                        </div>
                        <div>
                            Sales Comments
                        </div>
                        <div>
                            <asp:TextBox ID="SalesComments" ClientIDMode="Static" runat="server" TextMode="MultiLine"
                                Style="border: 1px solid #FFA200;" Width="433px" Height="60px"></asp:TextBox>
                        </div>
                        <div>
                            <asp:Label runat="server" ID="labGKC" Text="GEK Comments"></asp:Label>
                        </div>
                        <div>
                            <asp:Panel runat="server" ID="gridCustomerPanel">
                                <%--                            <ASPx:CE runat="server" ID="CEGrid" />--%>
                                <%--                                <asp:TextBox ID="GekComments" ClientIDMode="Static" runat="server" TextMode="MultiLine"
                                    Width="433px" Height="135px"></asp:TextBox>
                            </asp:Panel>--%>
                            </asp:Panel>
                        </div>
                                                                                <asp:Panel runat="server" ID="avaRemark" ClientIDMode="Static" >
                                 <table style="width:100%">
                                   <tr>
                                     <td align="left">Available Width Remark</td>
                                   </tr>
                                   <tr>
                                     <td>
                                     <asp:TextBox runat="server" ID="txtAvaRemark" ClientIDMode="Static" Style="border: 1px solid #FFA200;" TextMode="MultiLine" Width="97%" Height="60px"></asp:TextBox>
                                     </td>
                                   </tr>
                                 </table>
                                   
                                   
                                </asp:Panel>

                    </td>
                    <td style="width: 5px">
                        &nbsp;
                    </td>
                    <td align="left" valign="top">
                        <div>
                            Quality Attributes&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <input type="button" value="Retrive..." id="btnRetrive" style="width: 100px" class="myButton" /><asp:Label ID="IdenLevel" runat="server" Text="IdenLevel" Visible="False"></asp:Label>
                        </div>
                        <div style="height: 8px">
                        </div>
                        <asp:Panel runat="server" ID="CFTAttribute">
                            <%--<ASPx:Attribute runat="server" ID="attribute" />--%>
                        </asp:Panel>
                        <asp:Label runat="server" ID="labAW" Text="Available Width">
                            
                        </asp:Label>
                        <asp:Panel runat="server" ID="AvaPanel">
                            <%-- <ASPx:Ava ID="AvaWidth" runat="server"></ASPx:Ava>--%>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" align="center">
                        <div style="height: 2px">
                        </div>
                        <asp:Button ID="btnQuery" Text="Query" runat="server" Height="30" 
                            ClientIDMode="Static" CssClass="myButton"
                            Width="100"  />
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnSave"  ClientIDMode="Static" Text="Save" runat="server" OnClick="btnSave_Click" Height="30" CssClass="myButton"
                            Width="100" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnApproval" Text="Approve" runat="server" ClientIDMode="Static" CssClass="myButton"
                            Height="30" Width="100"  />&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnUnApprove" Text="UnApprove" runat="server" ClientIDMode="Static" CssClass="myButton"
                            Height="30" Width="100" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnShutdown" Text="Shut Down" runat="server" ClientIDMode="Static" CssClass="myButton"
                            Height="30" Width="100" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnShutdownAndDisabled" Text="Shut Down And Disabled" runat="server" CssClass="myButton"
                            ClientIDMode="Static" Height="30" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnExit" Text="Exit" runat="server" ClientIDMode="Static" Height="30" CssClass="myButton"
                            Width="100" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="mydiv" style="width: 392px; height: 170px; position: absolute;  z-index: 10000; display: none;" id="gridDiv">
            <div style="height: 4px">
            </div>
            <div style="text-align: left">
                <table>
                    <tr>
                        <td align="left">
                            QualityCode
                        </td>
                        <td align="left">
                            <input type="text" id="unQC" style="border:1px solid #FFA200; height:18px" />
                        </td>
                    </tr>
                </table>
            </div>
            <div style="height: 8px">
            </div>
            <div style="text-align: left">
                UnApprove reason
            </div>
            <div style="text-align: left; border:1px solid #FFA200" >
                <asp:TextBox runat="server" ID="unApproveReason" ClientIDMode="Static" BorderStyle="None" 
                    TextMode="MultiLine" Width="388px" Height="60px"></asp:TextBox>
            </div>
            <div style="height: 8px">
            </div>
            <div>
                <input type="button" id="btnDivUnApprove" onclick="FunUnApprove()"  value="UnApprove" style="width: 80px; height: 30px; border:1px solid #FFA200" />
                <input type="button" id="btnDivCancel" value="Cancel" onclick="CancelUnApprove()"
                    style="width: 80px; height: 30px;border:1px solid #FFA200" />
            </div>
            <div style="height: 12px">
            </div>
        </div>
        <div style="width: 300px; height: 90px; position: absolute; background-color: #58BDF2;
            z-index: 10000; display: none;" id="OpenInfoDiv">
            <table>
                <tr>
                    <td>
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                        BatchNo
                    </td>
                    <td>
                        <select id="batchNoSelect" style="width: 100px">
                        </select>
                    </td>
                    <td>
                        TestNo
                    </td>
                    <td>
                        <select id="testNoSelect" style="width: 100px">
                        </select>
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <input type="button" value="Open" onclick="openDD()" />
                    </td>
                    <td colspan="2" align="center">
                        <input type="button" value="Cancel" onclick="CloseDiv()" />
                    </td>
                </tr>
            </table>
        </div>
    </center>
    <asp:ObjectDataSource ID="ppoOBDS" runat="server" SelectMethod="GetFabricCodeList"
        TypeName="Comfy.App.Core.QualityCode.CustomerManager" DataObjectTypeName="Comfy.App.Core.QualityCode.FabricCodeModel">
        <SelectParameters>
            <asp:Parameter Name="MG" Type="String" DefaultValue="null"  />
            <%--<asp:FormParameter  FormField="txtMG" Type="String" Name="MG" DefaultValue="null"/>--%>
            <asp:Parameter Name="PPONO" Type="String" DefaultValue="null" />
            <asp:Parameter Name="QualityCode" Type="String" DefaultValue="null" />
            <asp:Parameter Name="CustomerId" Type="String" DefaultValue="null" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="YarnInfoODS" runat="server" SelectMethod="GetYarnInfo"
        InsertMethod="AddYarnInfo" UpdateMethod="EditYarnInfo" DeleteMethod="DeleteYarnInfo"
        TypeName="Comfy.App.Web.QuailtyCode.ApproveQC" DataObjectTypeName="Comfy.App.Core.QualityCode.YarnInfo">
        <SelectParameters>
            <asp:Parameter Name="orderByField" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <div style="width: 500px; height: 320px; position: absolute; background-color: #58BDF2; overflow:auto;
            border: 1px solid #FFA200; z-index: 10000; display: none;" id="sameDiv">
            <center>
                <div style="height: 25px;font-size: larger">
                  Below quality code have the same attributes(click them show the details).
                </div>
                <div id="sameQCDiv">
                   </div>
                <br />
                <br />
                <br />
                <div>
                  Replace By<input type="text" id="txtRB" style="width:100px"/>
                </div>
                <br />
                <div>
                     <input id="caButton"  value="Continue Approve" style="width: 120px;height:30px" type="button"
                            class="myButton"  />
                    <input type="button" id="Button2" value="Cancel" onclick="cancelCreate()" class="myButton" style="width: 50px;height:30px" />
                </div>
            </center>
        </div>

    </form>
</body>
<script type="text/javascript">
    var grid = ppoGrid;
    var strTempBatchNo;
    var strTempTestNo;
    var PPONO;
    function SearchQC() {
        grid.ShowSearchForm();
    }
    function GridRefresh() {
        grid.Refresh();
    }
    $(function () {
        $("#btnExit").click(function () {
            try {
                window.parent.closeApprove();

            } catch (e) {
                window.opener = null;
                window.open('', '_self');
                window.close();
            }

        });

        $("#caButton").click(function () {
            cancelCreate();
            if (grid.IsSelectOneRecord()) {
                grid.GetRowValues('QualityCode;PPONO;FabricPart;ComboCode;Iden', 'ApproveQCFun');
            } else if ($("#txtQualityCode").val() != "") {
                var paramsd = new Array();
                paramsd.push($("#txtQualityCode").val());
                paramsd.push("-1");
                paramsd.push("-1");
                paramsd.push("-1");
                paramsd.push("-1");
                ApproveQCFun(paramsd);

            }
        });

        $("#btnSave").click(function () {
            //  var params = getFAttributeForUpdate();
            var result
            var params
            if ($("#tQC").val() == "") {
                return;
            }

            var resultratio
            var paramsratio
            paramsratio = "";
            resultratio = AjaxCallFunOne("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "VerificationRatioSave", params);
            if (resultratio == "FalseOne") {
                alert("Ratio不可以为空");
                return false;
            }

            if (resultratio == "FalseTwo") {
                alert("Ratio总和加起来要等于100");
                return false;
            }




            //kingzhang for support 735101 存在PPO时，GP不能为空
            if (GMType == "Fabric" && CheckDataPPOAndGP("Fabric")) {
                params = $("#tQC").val() + "<>" + getFAttributeForUpdate() + "<>" + confimAvaUpdate() + "<>" + $("#tCustomerId").val() + "<>" + $("#GekComments").val() + "<>" + $("#txtAvaRemark").val();
                result = AjaxCallFunOne("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "UpdatedFabric", params);
            } else if (GMType == "FlatKnit" && CheckDataPPOAndGP("FlatKnit")) {
                params = $("#tQC").val() + "<>" + getFAttributeForUpdate() + "<>" + $("#tCustomerId").val() + "<>" + $("#GekComments").val();
                result = AjaxCallFunOne("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "UpdatedFlat", params);
            }
            else if (GMType == "Tapping" && CheckDataPPOAndGP("Tapping")) {
                params = $("#tQC").val() + "<>" + getFAttributeForUpdate() + "<>" + $("#tCustomerId").val() + "<>" + $("#GekComments").val();
             
                result = AjaxCallFunOne("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "UpdatedTapping", params);
            }else
            { 
                return false;
            }
            if (result != "") {
                alert("出错：" + result);
                return false;
            }
            alert("保存成功");
            return false;
        });
    })
    //kingzhang for support 735101 存在PPO时，GP不能为空
    function CheckDataPPOAndGP(type) {
        var resultData = getFAttributeForUpdate();
        var retStrs = resultData.split("<>");
        if (GMType == "Fabric" ) {
            if(retStrs[10]!='' && (retStrs[11] == '' || retStrs[11] =='null'))
            {
                alert("请选择QC_Ref_GP");
                return false;
            }
            if (retStrs[12] != '' && (retStrs[13] == '' || retStrs[13] == 'null'))
            {
                alert("请选择HF_Ref_GP");
                return false;
            }
        } else if (GMType == "FlatKnit") {
            if (retStrs[6] != '' && (retStrs[7] == '' || retStrs[7] == 'null'))
            {
                alert("请选择QC_Ref_GP");
                return false;
            }
            if (retStrs[8] != '' && (retStrs[9] == '' || retStrs[9] == 'null')) {
                alert("请选择HF_Ref_GP");
                return false;
            }
        }
        else if (GMType == "Tapping") {
            if (retStrs[4] != '' && (retStrs[5] == '' || retStrs[5] == 'null')) {
                alert("请选择QC_Ref_GP");
                return false;
            }
            if (retStrs[6] != '' && (retStrs[7] == '' || retStrs[7] == 'null')) {
                alert("请选择HF_Ref_GP");
                return false;
            }
        }
        return true;
    }

    function CancelUnApprove() {
        $("#gridDiv").hide();
    }

    function ow(owurl) {
        var tmp = window.open("about:blank", "", "fullscreen=1")
        tmp.moveTo(0, 0);
        tmp.resizeTo(screen.width + 20, screen.height);
        tmp.focus();
        tmp.location = owurl;
    }
    function InitQC() {
        grid.GetRowValues('QualityCode;CustomerComment;CustomerId;PPONO;FabricPart', 'InitQCForm');
    }
    function searchInfo() {
        grid.GetRowValues('PPONO;QualityCode', 'OpenSearchForm');
    }
    function openDD() {
        if (PPONO != "" && $("#batchNoSelect").val() != "" && $("#testNoSelect").val() != "") {
            var htmlUrl = "http://192.168.7.70/newweb/gkmis/phywebtest/right.asp?Ppo_no='KSF13AA010283'&batch_no='" + $("#batchNoSelect").val() + "'&Test_No='" + $("#testNoSelect").val() + "'&method=1&isbeforyear=&isnick=N";
            // ow(htmlUrl);
            window.open(htmlUrl);
            $("#OpenInfoDiv").hide();

        }
        else {
            alert("Please choose the infomation!");
        }
    }
    function CloseDiv() {
        $("#OpenInfoDiv").hide();
    }
    function showDiv() {
        $("#OpenInfoDiv").css({
            "top": Math.max(($(window).height() / 2 - $("#OpenInfoDiv").outerHeight() / 2 + $(window).scrollTop()), 0) + "px",
            "left": Math.max(($(document.body).width() / 2 - $("#OpenInfoDiv").outerWidth() / 2 + $(window).scrollLeft()), 0) + "px",
            "display": "block"
        });
    }
    function showQCDetail(strQC) {
        window.parent.openSearchOne(strQC);
    }
    function cancelCreate() {
        $("#sameDiv").hide();
    }
    function showSameQC(result) {
        $('#sameQCDiv').html("");
        var strHtml;
        strHtml = "<table><tr>";
        $("#sameDiv").css({
            "top": Math.max(($(window).height() / 2 - $("#sameDiv").outerHeight() / 2 + $(window).scrollTop()), 0) + "px",
            "left": Math.max(($(document.body).width() / 2 - $("#sameDiv").outerWidth() / 2 + $(window).scrollLeft()), 0) + "px",
            "display": "block"
        });
        var retStrs = result.split(",");
        if (retStrs.length == 2) {
            $("#txtRB").val(retStrs[0]);
        }
        for (var i = 0; i < retStrs.length; i++) {
            if (i % 6 == 0) {
                strHtml = strHtml + "</tr><tr>"
            }
            if (retStrs[i] != "") {
                strHtml = strHtml + "<td><u onclick=showQCDetail('" + retStrs[i] + "')>" + retStrs[i] + "</u><td>";
            }

        }
        strHtml = strHtml + "</tr></table>"

        $(strHtml).appendTo("#sameQCDiv");
    }

    $(function () {

        $("#btnUnApprove").click(function () {
            $("#gridDiv").css({
                "top": Math.max(($(window).height() / 2 - $("#gridDiv").outerHeight() / 2 + $(window).scrollTop()), 0) + "px",
                "left": Math.max(($(document.body).width() / 2 - $("#gridDiv").outerWidth() / 2 + $(window).scrollLeft()), 0) + "px",
                "display": "block"
            });
            return false;
        });


        $("#batchNoSelect").change(function () {
            var i;
            var batchNO = $("#batchNoSelect").val();
            for (i = 0; i < strTempTestNo.length; i++) {
                if (strTempTestNo[i].indexOf(batchNO) > 0) {
                    $("#testNoSelect").append("<option value='" + strTempTestNo[i].split("<?>")[2] + "'>" + strTempTestNo[i].split("<?>")[2] + "</option>");
                }
            }

        });


        $("#btnQuery").click(function () {
            window.parent.openSearch();
            return false;
        });
        $("#btnShutdown").click(function () {
            if (!confirm("Confirm to shutdown this QualtyCode?")) {
                return false;
            }
            if (grid.IsSelectOneRecord()) {
                grid.GetRowValues('QualityCode;PPONO;FabricPart;ComboCode;Iden', 'ShutDownQCFun');
            } else if ($("#txtQualityCode").val() != "") {
                var paramsd = new Array();
                paramsd.push($("#txtQualityCode").val());
                paramsd.push("-1");
                paramsd.push("-1");
                paramsd.push("-1");
                paramsd.push("-1");
                ShutDownQCFun(paramsd);

            }
            return false;

        });
        $("#btnShutdownAndDisabled").click(function () {
            if (!confirm("Confirm to shutdown and disabled this QualtyCode?")) {
                return false;
            }
            if (grid.IsSelectOneRecord()) {
                grid.GetRowValues('QualityCode;PPONO;FabricPart;ComboCode;Iden', 'ShutDownAndDisabledQCFun');
            } else if ($("#txtQualityCode").val() != "") {
                var paramsd = new Array();
                paramsd.push($("#txtQualityCode").val());
                paramsd.push("-1");
                paramsd.push("-1");
                paramsd.push("-1");
                paramsd.push("-1");
                ShutDownAndDisabledQCFun(paramsd);

            }
            return false;
        });




        $("#btnApproval").click(function () {

            //add by sunny 保存数据前确认
            var resultratio
            var paramsratio
            paramsratio = "";
            resultratio = AjaxCallFunOne("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "VerificationRatioSave", params);
            if (resultratio == "FalseOne") {
                alert("Ratio不可以为空");
                return false;
            }

            if (resultratio == "FalseTwo") {
                alert("Ratio总和加起来要等于100");
                return false;
            }





            if (!confirm("Confirm to Approve this QualtyCode?")) {
                return false;
            }


            //            if (($.trim($("#txtQC_Ref_PPO").val()) == "") || ($.trim($("#DropQC_Ref_GP").val()) == "")) {
            //                alert("QC_REF_PPO和QC_REF_OP都不能为空！");
            //                return false;
            //            }


            //  start   Approve 之前保存数据 2017 0714 add by sunny  
            var resultsave = "";
            var params = "";
            if ($("#tQC").val() == "") {
                return;
            }
            //kingzhang for support 735101 存在PPO时，GP不能为空
            if (GMType == "Fabric" && CheckDataPPOAndGP("Fabric")) {
                params = $("#tQC").val() + "<>" + getFAttributeForUpdate() + "<>" + confimAvaUpdate() + "<>" + $("#tCustomerId").val() + "<>" + $("#GekComments").val() + "<>" + $("#txtAvaRemark").val();
                resultsave = AjaxCallFunOne("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "UpdatedFabric", params);
            } else if (GMType == "FlatKnit" && CheckDataPPOAndGP("FlatKnit")) {
                params = $("#tQC").val() + "<>" + getFAttributeForUpdate() + "<>" + $("#tCustomerId").val() + "<>" + $("#GekComments").val();
                result = AjaxCallFunOne("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "UpdatedFlat", params);
            }
            else if (GMType == "Tapping" && CheckDataPPOAndGP("Tapping")) {
                params = $("#tQC").val() + "<>" + getFAttributeForUpdate() + "<>" + $("#tCustomerId").val() + "<>" + $("#GekComments").val();

                resultsave = AjaxCallFunOne("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "UpdatedTapping", params);
            } else {
                return false;
            }
            if (resultsave != "") {
                alert("出错：" + resultsave);
                return false;
            }
            //  end   Approve 之前保存数据 2017 0714 add by sunny  
          



            $("#txtRB").val("");
            var tempQC = $("#txtQualityCode").val();
            if (tempQC != "") {

                
                var Sourcing = AjaxCallFunOne("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "GetSourcing", tempQC);
                
                var tempCon = GMType + "<>" + Sourcing + "<>" + getFAttribute("ApproveQC");
                var result = AjaxCallFunOne("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.CreateQC", "GetTheSaveQC", tempCon + "#@Approve");
                //20190730 kingzhang for support 597477 判断GM为Y时，AW不能为（0和空）
                if (result == "AWWidth") {
                    alert("当GMTWash选择Y时，AWWeight不能为0或者空,请重新填写！");
                    return false;
                }


                if (result != "" && result != (tempQC + ",")) {                   
                    //Add by Sunny 2017 0720
                    GetApprovedQc = result;
                    showSameQC(result);
                    return false;
                }
            }


            //  return false;

            if (grid.IsSelectOneRecord()) {
                grid.GetRowValues('QualityCode;PPONO;FabricPart;ComboCode;Iden', 'ApproveQCFun');
            } else if ($("#txtQualityCode").val() != "") {
                var paramsd = new Array();
                paramsd.push($("#txtQualityCode").val());
                paramsd.push("-1");
                paramsd.push("-1");
                paramsd.push("-1");
                paramsd.push("-1");
                ApproveQCFun(paramsd);

            }

//            if (typeof (window.parent.document.getElementById("btnQcSearch")) != "undefined") {
//                window.parent.document.getElementById("btnQcSearch").click();
//            }
            $("#btnQcSearch", window.parent.document).click();

            return false;
        });




        $("#btnRetrive").click(function () {
            grid.GetRowValues('QualityCode;PPONO;FabricPart', 'Retrive');
          
        });
        $("#ppoGridSearchMG").val(GMType);
        var tempP = {};
        tempP.MG = GMType;
        grid.GridCheck(tempP);

    })

    function GetPhyWebTest(val) {
        var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "GetPhyWebTestData", val[0] + "," + val[1] + "," + val[2]);
        if (result.indexOf("error") >= 0) {
            alert('没有找到对应的标准数据' + result);
            return;
        }
        var com = $("#GekComments").val();
        if (com != '') {
            if (com.indexOf(result) == -1)
                $("#GekComments").val(com + result);
        }
        else {
            $("#GekComments").val(result);
        }
    }

    function FunUnApprove() {
        if (!confirm("Confirm to UnApprove this QualtyCode?")) {
            return false;
        }
        var tempQC = $("#unQC").val();
        var tempResaon = $("#unApproveReason").val();
        if (tempResaon.replace(/[ ]/g, "") == "") {
            alert("Please input reason!");
            return;
        }
        if (tempQC.replace(/[ ]/g, "") == "") {
            alert("Please input QualityCode!");
            return;
        }
        var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "UnApprove", tempQC + ",mn" + tempResaon);
      
        $("#unQC").val("");
        $("#unApproveReason").val("");
        $("#gridDiv").hide();
        if (result.substr(0,1) == "0") {
            //alert("The PPO which used this QaulityCode had been SUBMIT production,can not UnApprove");
            alert("Quality Code " + tempQC + " 已经用在了下面的大货PPO里面，如果需要修改PPO的属性，需要请相关同事unSubmit PPO。\n" +
            "-----------------------------------------------------------------------\n"+
            result.substr(1, result.length - 1) +
            "-----------------------------------------------------------------------\n" +
            "如果只修改非Quality Code的信息，请进入‘Maintain Quality Code’界面进行修改！"
            );

            return;
        }
        if (result == "1") {
            alert("Sucessfully UnApprove!");
            return;
        }
        if (result == "2") {
            alert("The QualityCode is not exit or the status is not Approved");
            return;
        }
        if (result == "3") {
            alert("The PPO which used this QaulityCode had been arranged production,can not UnApprove");
            return;
        }
       

        alert("error:" + result);




        
    }

    function ApproveQCFun(val) {
        var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "Approve", val[0] + ",mn" + val[1] + ",mn" + val[2] + ",mn" + val[3] + ",mn"+val[4]+",mn"+ $("#txtRB").val()+",mn"+ GetApprovedQc);
        if (result == "1") {
            alert("Successfully approved!");
            
            //kingzhang for support 742015
            if (GMType == "Fabric") {
                params = $("#tQC").val() + "<>" + getFAttributeForUpdate();
                resultsave = AjaxCallFunOne("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "UpdateESCM_Ref", params);
            }

            grid.Refresh();
            //上传AX代码
           // var result1 = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "CallWS_AX", val[0]);
          //  if (result1 != "Sucess") {
               // alert(result1);
          //  } // Call AX Interface
 
        } else {
            alert("Fail!");
        }
        return false;
    }

    function ShutDownQCFun(val) {
        var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "ShutDown", val[0] + ",mn" + val[1] + ",mn" + val[2] + ",mn" + val[3] + ",mn" + val[4]);
        if (result == "1") {
            alert("Successfully shut down!");
            grid.Refresh();
        } else {
            alert("Fail!");
        }
        return false;
    }

    function ShutDownAndDisabledQCFun(val) {
        var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "ShutDownAndDisabled", val[0] + ",mn" + val[1] + ",mn" + val[2] + ",mn" + val[3] + ",mn" + val[4]);
        if (result == "1") {
            alert("Successfully shut down and disabled!");
            grid.Refresh();
        } else {
            alert("Fail!");
        }
        return false;
    }
    function Retrive(val) {

      

        var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "InitInfoOne", val[0] + "," + val[1] + "," + GMType);
        if (result.indexOf("error") >= 0) {
            alert('没有找到对应的数据' + result);
            return;
        }
        if (GMType == "Fabric") {
            cInitRetrive(result, '');//kingzhang for support 690024 cInitRetrive方法,去掉对备注的赋值
        } else if (GMType == "FlatKnit") {
            fInitRetrive(result, '');//kingzhang for support 690024 新增fInitRetrive和tInitRetrive方法，参考fInit和tInit,去掉对备注的赋值
        } else if (GMType == "Tapping") {
            tInitRetrive(result, '');
        }

        $("#DropQC_Ref_GP").val(val[2]);


        GetPhyWebTest(val);
    }


    function OpenSearchForm(val) {
        var i, j;
        PPONO = val[0];
        var result = AjaxCallFun("Comfy.App.Core", "Comfy.App.Core.QualityCode.CustomerManager", "GetGKInfo", val[0] + "," + val[1]);
        $("#batchNoSelect option").remove();
        $("#testNoSelect option").remove();
        $("#batchNoSelect").append("<option value=''></option>");
        strTempBatchNo = "";
        if (result != "") {
            var strS = result.split("<*>");
            strTempTestNo = strS;
            for (i = 0; i < strS.length; i++) {
                if (strS[i] != "") {
                    var strChis = strS[i].split("<?>");
                    if (strTempBatchNo.indexOf(strChis[1]) == -1) {
                        $("#batchNoSelect").append("<option value='" + strChis[1] + "'>" + strChis[1] + "</option>");
                        strTempBatchNo = strTempBatchNo + strChis[1] + ",";
                    }
                }
            }
            showDiv();
        }
        else {
            alert("No Information!");
        }
        //alert(result);
    }

    $(function () {
        $("#btnSave").click(function () {
            if ($("#tQC").val() == "") {
                alert("Please select one quality code");
                return false;
            }
            return true;
        });
    });

    function InitQCForm(val) {
   

       

        if (val[0] != "") {
            //kingzhang for support  740959 用户可能是在approve界面查询QC显示的，所以改为在加载数据时也判断是否可以审批
            var gekApproveStaus = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "GetApproveStaus", val[0]);
            if (gekApproveStaus == "0")
            { document.getElementById("btnApproval").disabled = true; }

            $("#tQC").val(val[0]);
            $("#txtQualityCode").val(val[0]);
            $("#tCustomerId").val(val[2]);

            var vPPONOFabricPart = val[3] + "|" + val[4];
//            alert(vPPONOFabricPart);
            var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "InitInfo", val[0]);
            var strTemp;
            var strHC;

            $("#SalesComments").val(val[1]);
            if (result != "") {
                if (GMType == "Fabric") {
                    InitTable(result.split("<|>")[10]);
                    cInit(result, vPPONOFabricPart);
                    $("#txtAvaRemark").val(result.split("<|>")[18]);
                } else if (GMType == "FlatKnit") {
                    fInit(result, vPPONOFabricPart);
                    if (val.length > 4) {//取后重领
                        strHC = AjaxCallFun("Comfy.App.Core", "Comfy.App.Core.QualityCode.CustomerManager", "GetHeavyCollar", val[0] + "," + val[3] + "," + val[4]);
                        fInitHC(strHC);
                    }
                    else {
                        strHC = AjaxCallFun("Comfy.App.Core", "Comfy.App.Core.QualityCode.CustomerManager", "GetHeavyCollarOne", val[0]);
                        fInitHC(strHC);
                    }
               
                } else if (GMType == "Tapping") {
                    tInit(result, vPPONOFabricPart);
                }
            }

            var tempStr = val[0] + "," + val[2];
            var tempStr1 = val[0] + "," + val[3];

            var gekComment = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "GetGekComment", tempStr);
            $("#GekComments").val(gekComment);
            //GridRefresh();
            //REF四个控件从SPPO系统取值  add by zheng zhou 2016-8-11
            var refStr = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "GetRefByQC", tempStr1);
            var refStrArray = refStr.split(',');
            if (refStrArray.length > 0) {
                $('#txtQC_Ref_PPO').val(refStrArray[0]);
                $('#DropQC_Ref_GP').val(refStrArray[1]);
                $('#txtHF_Ref_PPO').val(refStrArray[2]);
                $('#DropHF_Ref_GP').val(refStrArray[3]);
            }

        }
    }
</script>
</html>
