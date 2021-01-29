<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QueryQC.aspx.cs" Inherits="Comfy.App.Web.QuailtyCode.QueryQC" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register TagPrefix="Aspx" Namespace="Comfy.UI.WebControls.WebGridView" Assembly="Comfy.UI.WebControls" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
            <style>
            .myButton
        {
             border:1px solid #FFA200;
            }
    </style>
    <script src="../Scripts/jquery1.4.js" type="text/javascript"></script>
    <script type="text/javascript" src="../Scripts/Utilities.js"></script>
    <script src="../Scripts/Combo.js" type="text/javascript"></script>
    <script src="../Scripts/ccftag.js" type="text/javascript"></script>
    <script type="text/javascript">
        function showAll(val, id, e) {
            var myId = $(id);
            if (myId.length == 0) return;
            $("<div class='ImgClass' onMouseOut='DivRemove(event)'  style='background-color:#FFA200;font-size:12pt'><div onMouseOut='DivRemove(event)'>" + val.replace(/--/g, "<br/>") + "</div></div>").width(200).height(200).css({
                "position": "absolute",
                "z-index": "10001",
                "left": e.x + 170,
                "top": e.y + 50
            }).appendTo(document.body);
        }

        function RedirectURL(val) {            
            //window.open('/QuailtyCodeUAT/export.aspx?' + val);     

            window.open('/QuailtyCode/export.aspx?' + val);  
       }
 
        function SearchQC(val) {
            $(function () {
                var param = {};
                param.QualityCode = val;
                grid.GridCheck(param);
            });
        }
        function MoveDiv(e) {
            $(".ImgClass").css({
                "left": e.x + 15,
                "top": e.y
            });
        }
        function ReportLink(val) {
            
            if (val.substr(0, 1) == "C") {
                return ("<a href= 'http://192.168.7.211/ReportServer/Pages/ReportViewer.aspx?%2fOMD%2fQCFabric&rs:Command=Render&QualityCode="+val+"' target='blank'>" + val + "</a>");

            } else if (val.substr(0, 1) == "F") {
                return ("<a href= 'http://192.168.7.211/ReportServer/Pages/ReportViewer.aspx?%2fOMD%2fQCFlatKnit&rs:Command=Render&QualityCode=" + val + "' target='blank'>" + val + "</a>");
            }
            else {
                return ("<a href= 'http://192.168.7.211/ReportServer/Pages/ReportViewer.aspx?%2fOMD%2fQCTapping&rs:Command=Render&QualityCode=" + val + "' target='blank'>" + val + "</a>");
            }

        }
        function AvaRender(val) {
            if (val.length > 60) {
                return ("<div class='ShowAva'    onMouseOver=showAll('" + val + "',this,event)>" + val.substring(0, 60) + "........</div>");
            }
            else {
                return ("<div class='ShowAva'    onMouseOver=showAll('" + val + "',this,event)>" + val.substring(0, 60) + "</div>");
            }
        }

        function DivRemove(e) {

            $(".ImgClass").remove();
        }

        function showAll1(val, id, e) {
            var myId = $(id);
            if (myId.length == 0) return;
            $("<div class='ImgClass1'  onMouseOut='DivRemove1(event)' style='background-color:#FFA200;font-size:12pt'><div  onMouseOut='DivRemove1(event)'>" + val.replace(/--/g, "<br/>") + "</div></div>").width(200).height(200).css({
                "position": "absolute",
                "z-index": "10001",
                "left": e.x + 170,
                "top": e.y + 50
            }).appendTo(document.body);
        }
        function MoveDiv1(e) {
            $(".ImgClass1").css({
                "left": e.x + 15,
                "top": e.y
            });
        }
        function DivRemove1(e) {

            $(".ImgClass1").remove();
        }
        function YarnRender(val) {
            if (val.length > 60) {
                return ("<div  onMouseOver=\"showAll1('" + val + "',this,event)\">" + val.substring(0, 60) + "........</div>");
            }
            else {
                return ("<div  onMouseOver=\"showAll1('" + val + "',this,event)\">" + val.substring(0, 60) + "</div>");
            }
        }

   
 

    </script>
</head>
<body scroll="no">
    <form id="form1" runat="server">
    <div id="header">
        <div style="height: 28px; font-weight: bolder; width: 100%; background-color:#FFCC73">
            <table>
                <tr>
                    <td>
                        Quality Code Message
                    </td>
                    <td>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <input type="button" id="btnSearch" value="Search" onclick="Search()" style="width:90px" class="myButton" />
                    </td>
                    <td>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <input type="button" id="btnEdit" value="Maintain" onclick="Edit()" runat="server" style="width:90px"  class="myButton"/>
                    </td>
                    <td>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <input type="button" id="btnApprove" value="Approve" onclick="ApproveFun()" runat="server" style="width:90px"   class="myButton"/>                    
                    </td>
                    <td>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <input type="button" id="btnCreate" value="Edit" onclick="CreateFun()" runat="server" style="width:90px"  class="myButton" />                    
                    </td>
                    <td>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:CheckBox ID="cboxShowOwn" Text="Show My Own" Checked="false" 
                            runat="server" AutoPostBack="true" oncheckedchanged="cboxShowOwn_CheckedChanged" />        
                    </td>
                    <td>
                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        
                        <input type="button"  id="Unnamed1Click"    style="width:90px"  class="myButton"   value="export" />   

                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp

                        <input type="button"  id="Unnamed2Click"    style="width:120px"  class="myButton"   value="export CSV" /> 

                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="content" style="overflow: auto;">
        <Aspx:WebGridView ID="MainGrid" runat="server" DataSourceId="MainOBDS" PageSize="7"
            Height="200" CreateSearchPanel="true" CreateAddPanel="false" KeyFieldName="QualityCode"
            OnClick="SearchCustomer()">
            <Aspx:Field FieldName="QualityCode" Caption="QualityCode" Width="60" ShowOnSearchForm="true" FRender="ReportLink" >
            </Aspx:Field>
            <Aspx:Field FieldName="Status" Caption="Status" Width="60" ShowOnSearchForm="true"
                FieldType="ComboBox">
                <Aspx:Item Text="New" Value="New" />
                <Aspx:Item Text="Developed" Value="Developed" />
                <Aspx:Item Text="Approved" Value="Approved" />
                <Aspx:Item Text="Disabled" Value="Disabled" />
            </Aspx:Field>

            <Aspx:Field FieldName="ReplaceBy" Caption="ReplaceBy" Width="60">
            </Aspx:Field>

            <Aspx:Field FieldName="Repeat" Caption="Repeat" Width="100">
            </Aspx:Field>

            <%--modify:gaofeng 2021年1月18日11:45:52 2021-0001 QC System Enhancement from sales ; 拆分字段ApprovedFromSPPO 成2个新增的字段 --begin
            <Aspx:Field FieldName="ApprovedFromSPPO" Caption="ApprovedFromSPPO" Width="120">
            </Aspx:Field>
            
            modify:gaofeng 2021年1月18日11:45:52 2021-0001 QC System Enhancement from sales ; 拆分字段ApprovedFromSPPO 成2个新增的字段 --end--%>

            <Aspx:Field FieldName="ApprovedFromSPPO" Caption="ApprovedFromSPPO" Width="160">
            </Aspx:Field>

            <Aspx:Field FieldName="ApprovedFromSPPO_Usage" Caption="ApprovedFromSPPO-Usage" Width="180">
            </Aspx:Field>



            <Aspx:Field FieldName="Construction" Caption="Construction" Width="150">
            </Aspx:Field>

            <Aspx:Field FieldName="Finishing" Caption="Finishing" Width="150">
            </Aspx:Field>

            <Aspx:Field FieldName="Pattern" Caption="Pattern" ShowOnSearchForm="true" DataSourceId="patternSQLDS"
                FieldType="ComboBox" Width="70" TextField="PARAMETER" ValueField="PARAMETER">
            </Aspx:Field>
            <Aspx:Field FieldName="DyeMethod" Caption="DyeMethod" Width="60" FieldType="ComboBox"
                DataSourceId="dyeMethodSALDS" TextField="DESCRIPTION" ValueField="DYE_METHOD">
            </Aspx:Field>
            <Aspx:Field FieldName="BfGmmm" Caption="BfGmmm" ShowOnSearchForm="true" Width="60">
            </Aspx:Field>
            <Aspx:Field FieldName="AfGmmm" Caption="AfGmmm" ShowOnSearchForm="true" Width="60">
            </Aspx:Field>

            <Aspx:Field FieldName="Shrinkage" Caption="Shrinkage" Width="60">
            </Aspx:Field>
            <Aspx:Field FieldName="ShrinkageTestingMethod" Caption="ShrinkageTestingMethod">
            </Aspx:Field>
            <Aspx:Field FieldName="GmtWashing" Caption="GmtWashing" Width="60">
            </Aspx:Field>

            <Aspx:Field FieldName="CustomerQualityId" Caption="CustomerQualityId"  ShowOnSearchForm="true">
            </Aspx:Field>


             <Aspx:Field FieldName="AvaWidth" Caption="Available Width" Width="200" FRender="AvaRender">
            </Aspx:Field>
            <Aspx:Field FieldName="Remark" Caption="Available Width Remark" Width="200" FRender="AvaRender">
            </Aspx:Field>
            <Aspx:Field FieldName="YarnInfo" Caption="YarnInfo" Width="200" FRender="YarnRender" >
            </Aspx:Field>  

            <Aspx:Field FieldName="Layout" Caption="Layout">
            </Aspx:Field>
            <Aspx:Field FieldName="YarnLength" Caption="YarnLength">
            </Aspx:Field>
            <Aspx:Field FieldName="TappingType" Caption="TappingType">
            </Aspx:Field>
            <Aspx:Field FieldName="SpecialType" Caption="Heavy_Flat_Knit">
            </Aspx:Field>
            <Aspx:Field FieldName="MillComments" Caption="MillComments" >
            </Aspx:Field>
            <Aspx:Field FieldName="Measurement" Caption="Measurement">
            </Aspx:Field>

            <Aspx:Field FieldName="Sourcing" Caption="Sourcing" Width="60" ShowOnSearchForm="true"
                FieldType="ComboBox">
                <Aspx:Item Text="Internal" Value="Internal" />
                <Aspx:Item Text="External" Value="External" />
            </Aspx:Field>
                  <Aspx:Field FieldName="CreateDate" Caption="CreateDate" FieldType="Date" DateFormat="yyyy-MM-dd hh:mm:ss" ShowOnSearchForm="True"
                Width="120">
            </Aspx:Field>

            <Aspx:Field FieldName="MaterialGroup" Caption="MaterialGroup" Width="90" ShowOnSearchForm="true"
                FieldType="ComboBox">
                <Aspx:Item Text="Fabric" Value="Fabric" />
                <Aspx:Item Text="Flat Knit Fabric" Value="Flat Knit Fabric" />
                <Aspx:Item Text="Tapping" Value="Tapping" />
            </Aspx:Field>
            <Aspx:Field FieldName="AnalysisNo" Caption="AnalysisNo">
            </Aspx:Field>
            <Aspx:Field FieldName="RefQualityCode" Caption="RefQualityCode">
            </Aspx:Field>

      
            <Aspx:Field FieldName="Creator" Caption="Creator" Width="60" ShowOnSearchForm="true">
            </Aspx:Field>
            <Aspx:Field FieldName="ApproveDate" Caption="ApproveDate" FieldType="Date" DateFormat="yyyy-MM-dd hh:mm:ss">
            </Aspx:Field>
            <Aspx:Field FieldName="Approver" Caption="Approver">
            </Aspx:Field>       


            <Aspx:Field FieldName="Con" Caption="Construction" Visible="false" ShowOnSearchForm="true" ColumnSpan="2"
                DataSourceId="constructionSQLDS" TextField="Description" ValueField="Construction" FieldType="ComboBox">
            </Aspx:Field>
            <Aspx:Field FieldName="Finish" Caption="Finishing" Visible="false" ShowOnSearchForm="true"
                DataSourceId="finishSQLDS" TextField="Description" ValueField="Finishing_Code" FieldType="ComboBox">
            </Aspx:Field>
            <Aspx:Field FieldName="SalesTeam" Caption="SalesTeam" Visible="false" ShowOnSearchForm="true"
                FieldType="ComboBox" DataSourceId="departmentSQLDS" ValueField="DEPARTMENT_ID"
                TextField="DEPARTMENT_ID">
            </Aspx:Field>
            <Aspx:Field FieldName="Sales" Caption="Sales" Visible="false" ShowOnSearchForm="true">
            </Aspx:Field>
                  <Aspx:Field FieldName="BuyserIds" Caption="BuyerIds" ShowOnSearchForm="false">
            </Aspx:Field>
                  <Aspx:Field FieldName="Brank" Caption="Branks"  ShowOnSearchForm="false">
            </Aspx:Field>      

            <Aspx:Field FieldName="LastUpdateTime" Caption="LastUpdateTime" FieldType="Date"
                Visible="false" DateFormat="yyyy-MM-dd hh:mm:ss">
            </Aspx:Field>
            <Aspx:Field FieldName="LastUpdateBy" Caption="LastUpdateBy" Visible="false">
            </Aspx:Field>
            <Aspx:Field FieldName="CustomerCode" Caption="CustomerCode" Width="60" ShowOnSearchForm="true" IsExport="false">
            </Aspx:Field>
            <Aspx:Field FieldName="GK_NO" Caption="GK_NO" Width="80">
            </Aspx:Field>
             <Aspx:Field FieldName="QC_Ref_PPO" Caption="QC_Ref_PPO" Width="80">
            </Aspx:Field>
            <Aspx:Field FieldName="QC_Ref_GP" Caption="QC_Ref_GP" Width="80">
            </Aspx:Field>
             <Aspx:Field FieldName="HF_Ref_PPO" Caption="HF_Ref_PPO" Width="80">
            </Aspx:Field>
            <Aspx:Field FieldName="HF_Ref_GP" Caption="HF_Ref_GP" Width="80">
            </Aspx:Field>

            <Aspx:Field FieldName="CustomerComment" Caption="SalesComment">  </Aspx:Field>  
            
   
        </Aspx:WebGridView>
    </div>
    <div style="height: 14px; font-weight: bolder; background-color:#FFCC73">
        Customer Message</div>
    <div id="footer" style="height: 200px">
        <Aspx:WebGridView ID="CustomerGrid" runat="server" DataSourceId="CustomerOBDS" PageSize="10"
            CreateSearchPanel="false" CreateAddPanel="false" KeyFieldName="Iden">
            <Aspx:Field FieldName="Iden" Caption="Iden" Visible="false">
            </Aspx:Field>
            <Aspx:Field FieldName="QualityCode" Caption="QualityCode" Width="60">
            </Aspx:Field>
            <Aspx:Field FieldName="BuyerId" Caption="BuyerId" FieldType="ComboBox" DataSourceId="customerSQLDS"
                TextField="NAME" ValueField="CUSTOMER_CD">
            </Aspx:Field>
            <Aspx:Field FieldName="Status" Caption="Status" Visible="false"></Aspx:Field>
            <Aspx:Field FieldName="Brand" Caption="Brand">
            </Aspx:Field>
            <Aspx:Field FieldName="CustomerQualityId" Caption="CustomerQualityId">
            </Aspx:Field>
            <Aspx:Field FieldName="Sales" Caption="Sales" Width="60">
            </Aspx:Field>
            <Aspx:Field FieldName="SalesGroup" Caption="SalesGroup" Width="60">
            </Aspx:Field>
            <Aspx:Field FieldName="MillComments" Caption="MillComments">
            </Aspx:Field>
            <Aspx:Field FieldName="IsFirstOwner" Caption="IsFirstOwner" Width="60">
            </Aspx:Field>
            <Aspx:Field FieldName="CreateDate" Caption="CreateDate" FieldType="Date" DateFormat="yyyy-MM-dd"
                Width="110">
            </Aspx:Field>
            <Aspx:Field FieldName="Creator" Caption="Creator" Width="80">
            </Aspx:Field>
        </Aspx:WebGridView>
    </div>
    <asp:ObjectDataSource ID="MainOBDS" runat="server" SelectMethod="GetModels" TypeName="Comfy.App.Web.QuailtyCode.QueryQC"
        DataObjectTypeName="Comfy.App.Core.QualityCode.QcmaininfoModel" OldValuesParameterFormatString="original_{0}">
        <SelectParameters>
            <asp:Parameter Name="QualityCode" Type="String" />
            <asp:Parameter Name="MaterialGroup" Type="String" />
            <asp:Parameter Name="Status" Type="String" />
            <asp:Parameter Name="Pattern" Type="String" />
            <asp:Parameter Name="Sourcing" Type="String" />
            <asp:Parameter Name="BfGmmm" Type="Int32" />
            <asp:Parameter Name="AfGmmm" Type="Int32" />
            <asp:Parameter Name="Finish" Type="String" />
            <asp:Parameter Name="Creator" Type="String" />
            <asp:Parameter Name="Con" Type="String" />
            <asp:Parameter Name="CustomerQualityId" Type="String" />
            <asp:Parameter Name="Sales" Type="String" />
            <asp:Parameter Name="SalesTeam" Type="String" />
            <asp:Parameter Name="DyeMethod" Type="String" />
            <asp:Parameter Name="pageSize" Type="Int32" />
            <asp:Parameter Name="startPage" Type="Int32" />
            <asp:Parameter Name="orderByField" Type="String" />
            <asp:Parameter Name="CustomerCode" Type="String" />
            <asp:Parameter Name="BeginCreateDate" Type="DateTime" />
            <asp:Parameter Name="EndCreateDate" Type="DateTime" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:SqlDataSource ID="finishSQLDS" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
        ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="SELECT Finishing_Code, Description  FROM pbKnitFinish where Washing_Flag='N' and IS_Active='Y' order by Description asc">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="constructionSQLDS" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
        ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="SELECT Construction, Description FROM pbKnitConstruction where IS_Active='Y' order by Description asc">
    </asp:SqlDataSource>
    <asp:ObjectDataSource ID="CustomerOBDS" runat="server" SelectMethod="GetCModels"
        TypeName="Comfy.App.Web.QuailtyCode.QueryQC" DataObjectTypeName="Comfy.App.Core.QualityCode.QccustomerlibraryModel"
        OldValuesParameterFormatString="original_{0}">
        <SelectParameters>
            <asp:Parameter Name="QualityCode" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:SqlDataSource ID="departmentSQLDS" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
        ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="select distinct(DEPARTMENT_ID) from GEN_USERS where Active='Y' order by DEPARTMENT_ID asc">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="patternSQLDS" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
        ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="SELECT PARAMETER, IS_ACTIVE FROM PBKNITPARAMETER WHERE (&quot;SORT&quot; = 'PATTERN' and IS_Active='Y')">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="customerSQLDS" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
        ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="select CUSTOMER_CD,NAME from gen_customer order by NAME asc">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dyeMethodSALDS" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
        ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="SELECT DYE_METHOD,DESCRIPTION FROM PBKNITDYEMETHOD">
    </asp:SqlDataSource>
    </form>
</body>
<script>
    var grid = MainGrid;
    var tempMG;
    var cGrid = CustomerGrid;
    var paramsb = {};

    // 添加文本框对customer的下拉数据绑定和选择 by LYH 2014/2/25
    var combo = new Combo("MainGridSearchCustomerCode", "seDiv", "selectTable", "370px", "200px", "Comfy.App.Core", "Comfy.App.Core.QualityCode.CustomerManager", "GetCustomerInfo", "combo");
    function comboClick(text, val) {
        $("#MainGridSearchCustomerCode").val(val);
    }
    $(function () {
        // 添加对CustomerCode的数据绑定 by LYH 2014/2/25
        //        $("[id$=\"" + gridName + "btnSearchCanel\"]").focus();
        //$("[id$=\"" + MainGrid + "SearchBuyerId\"]").focus();
        if ($("#seDiv").length <= 0) {
            $("#MainGridSearchCustomerCode").after("<div style=\"width: 370px; height: 200px; overflow: auto; display: none; z-index: 10000;" +
                        "background-color: #8BB6EF; border: 2px solid #004080\" id=\"seDiv\">" +
                        "<table style=\"width: 100%\" id=\"selectTable\">" +
                        "</table>" +
                    "</div>" +
                    "<input type=\"hidden\" id=\"customer\" />"
                    );
        }

        $("#customer").hide();
        $("#MainGridSearchCustomerCode").change(function () {
            $("#customer").val("");
        });
    });
    // END by LYH 2014/2/25



    function Search() {
        MainGrid.ShowSearchForm();
    }
    function SearchCustomer() {
        grid.GetRowValues('QualityCode;MaterialGroup', 'GetCustomer');
    }
    function GetCustomer(val) {
        tempMG = val[1];
        if (tempMG == "Flat Knit Fabric") {
            tempMG = "FlatKnit";
        }
        var param = {};
        param.QualityCode = val[0];
        cGrid.GridCheck(param);
    }

    function Edit() {
        //  cGrid.GetRowValues("QualityCode;BuyerId", "OpenEditForm");
        if (grid.IsSelectOneRecord()) {
            grid.GetRowValues("QualityCode", "OpenEditForm");
        } else {
            alert("Please select one quality code record!");
        }
        return false;

    }

    function ApproveFun() {
        if (cGrid.IsSelectOneRecord()) {
            cGrid.GetRowValues("QualityCode;Status;BuyerId", "OpenApproveForm");
        } else {
            alert("Please select one customer record!");
        }
        return false;
    }

    function CreateFun() {

        if (cGrid.IsSelectOneRecord()) {
            cGrid.GetRowValues("QualityCode;Status;BuyerId", "OpenCreateForm");
        } else {
            alert("Please select one customer record!");
        }
        return false;
        /*
        if (grid.IsSelectOneRecord()) {
        grid.GetRowValues("QualityCode", "OpenCreateForm");
        } else {
        alert("Please select one quality code record!");
        }
        return false;*/
    }

    function OpenCreateForm(val) {

        //2018-07-06 linyob add
        if (val[1].toUpperCase() == "APPROVED")
        {
            alert("This quality code can not Edit,because the status is:" + val[1]);
            return;
           
        }


        window.parent.saveAs("QuailtyCode/CreateQC.aspx?MG=" + tempMG + "&QC=" + val[0] + "&customerId=" + val[2]);
    }

    function OpenApproveForm(val) {
        if (val[1].toUpperCase() != "NEW") {
            alert("This quality code can not approve,because the status is:" + val[1]);
            return;
        }
        window.parent.ApproveWin("QuailtyCode/ApproveQC.aspx?MG=" + tempMG + "&QC=" + val[0] + "&customerId=" + val[2]);
    }

    function OpenEditForm(val) {
        window.parent.Maintain("QuailtyCode/EditQC.aspx?MG=" + tempMG + "&QC=" + val[0] + "&customerId=4");
    }


    function CallAJax(path, parm, callback, asy) {
        //设置默认值
        //alert(err.responseText);
        if (asy == null)
            asy = true;

        $.ajax({
            type: "Post",
            url: path,
            async: asy,
            data: parm,
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (data) { callback(data.d); },
            error: function (err) {
            },
            complete: function (xhr) { xhr = null; }

        });
    }

    $("#Unnamed1Click").click(function () {
     
        var gpath = "./QueryQC.aspx/Unnamed1_Click";
        var gparam = "{ }";
        CallAJax(gpath, gparam, function (context) {
            if (context.getcondition != "") {
                window.open('/QuailtyCode/export.aspx?' + context.getcondition);
            }
        });

    });

    //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --begin

    $("#Unnamed2Click").click(function () {

        //var gpath = "./QueryQC.aspx/Unnamed2_Click";

        var gpath = "./QueryQC.aspx/Unnamed2_Click";
        var gparam = "{ }";
        CallAJax(gpath, gparam, function (context) {
            if (context.getcondition != "") {

                // PRD
                window.open('/QuailtyCode/QuailtyCode/exportCsv.aspx?' + context.getcondition);

                // UAT
                //window.open('/QuailtyCode/exportCsv.aspx?' + context.getcondition);

  
            }
        });


        //var gpath = "./QueryQC.aspx/Unnamed2_Click";
        //var gparam = "{ }";
        //CallAJax(gpath, gparam, function (context) {
        //    if (context.getcondition != "") {
        //        window.open('/QuailtyCode/export.aspx?' + context.getcondition);
        //    }
        //});

    });

    //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --end


    $("#MainGridbtnSearch").click(function () {
        // tempMG = "";
        // paramsb.QualityCode = "";
        // setTimeout("cGrid.GridCheck(paramsb)", 1500);
    });
</script>
</html>
