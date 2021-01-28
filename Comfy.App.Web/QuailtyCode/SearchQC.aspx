Available Width<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchQC.aspx.cs" Inherits="Comfy.App.Web.QuailtyCode.SearchQC" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register TagPrefix="Aspx" Namespace="Comfy.UI.WebControls.WebGridView" Assembly="Comfy.UI.WebControls" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../Scripts/jquery1.4.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.cookie.js" type="text/javascript"></script>
    <script type="text/javascript" src="../Scripts/Utilities.js"></script>
    <script src="../Scripts/Combo.js" type="text/javascript"></script>
    <script src="../Scripts/form.checker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../Scripts/ccftag.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <br />
    <div style="padding: 0 0 0 20px">
    Quality Code <input type="text" id="txtQC"/> <input type="button"  value="Search" id="search" onclick="GetCustomer()"/>
    </div>
    <br />
        <div id="footer" style="height: 200px;width:800px; padding: 0 0 0 20px;">
        <Aspx:WebGridView ID="CustomerGrid" runat="server" DataSourceId="CustomerOBDS" PageSize="10"
            CreateSearchPanel="false" CreateAddPanel="false" KeyFieldName="Iden" OnDbClick="searchInfo()">
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
    <asp:ObjectDataSource ID="CustomerOBDS" runat="server" SelectMethod="GetCModels"
        TypeName="Comfy.App.Web.QuailtyCode.QueryQC" DataObjectTypeName="Comfy.App.Core.QualityCode.QccustomerlibraryModel"
        OldValuesParameterFormatString="original_{0}">
        <SelectParameters>
            <asp:Parameter Name="QualityCode" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
        <asp:SqlDataSource ID="customerSQLDS" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
        ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="select CUSTOMER_CD,NAME from gen_customer order by NAME asc">
    </asp:SqlDataSource>
    </form>
</body>
<script>
    var cGrid = CustomerGrid;

    function GetCustomer() {
        if (document.getElementById("txtQC").value == "")
            return;
        var Cust = AjaxCallFun("Comfy.App.Core", "Comfy.App.Core.QualityCode.CustomerManager", "GetCustomer", document.getElementById("txtQC").value);
        if (Cust == "") {
            alert("The quality code does not exists！");
            return;
        }
        if (Cust.split(";")[1] == "1") {
            window.location = "CreateQC.aspx?QC=" + document.getElementById("txtQC").value + "&Search=1&customerId=" + Cust.split(";")[0];
            return;
        }
        alert("Please double click one customer");
        var param = {};
        param.QualityCode = document.getElementById("txtQC").value;
        cGrid.GridCheck(param);
    }


    function searchInfo() {
   
            cGrid.GetRowValues("QualityCode;Status;BuyerId", "OpenCreateForm");

    }

    function OpenCreateForm(val) {
        window.location = "CreateQC.aspx?QC=" + val[0] + "&Search=1&customerId=" + val[2];
    }
</script>
</html>
