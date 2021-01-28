<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CustomerEditForm.ascx.cs"
    Inherits="Comfy.App.Web.QuailtyCode.CustomerEditForm" %>
<%@ Register TagPrefix="aspx" Namespace="Comfy.UI.WebControls.WebPopupControl" Assembly="Comfy.UI.WebControls" %>
<%@ Register TagPrefix="aspx" Namespace="Comfy.UI.WebControls.WebButtonEdit" Assembly="Comfy.UI.WebControls" %>
<%@ Register TagPrefix="aspx" Namespace="Comfy.UI.WebControls.WebGridView" Assembly="Comfy.UI.WebControls" %>
<div>
    <aspx:WebGridView ID="customerGrid" runat="server" DataSourceId="CustomerOBDS" PageSize="10" Width="433" Height="130"
        CreateSearchPanel="false" CreateAddPanel="true" KeyFieldName="Iden" OnDbClick="EditOneComment()">
        <aspx:Field FieldName="Iden" Caption="Iden" Visible="false" ShowOnEditForm="false">
        </aspx:Field>
        <aspx:Field FieldName="QualityCode" Caption="QualityCode" ShowOnEditForm="false" Visible="false">
        </aspx:Field>
        <aspx:Field FieldName="BuyerId" Caption="Customer" ShowOnEditForm="false">
        </aspx:Field>
        <aspx:Field FieldName="MillComments" Caption="GEK Comments" FieldType="TextArea">
        </aspx:Field>
        <aspx:Field FieldName="Brand" Caption="Brand" ShowOnEditForm="false">
        </aspx:Field>
        <aspx:Field FieldName="CustomerQualityId" Caption="CustomerQualityId" ShowOnEditForm="false">
        </aspx:Field>
        <aspx:Field FieldName="Sales" Caption="Sales" ShowOnEditForm="false">
        </aspx:Field>
        <aspx:Field FieldName="SalesGroup" Caption="SalesGroup" ShowOnEditForm="false">
        </aspx:Field>
        <aspx:Field FieldName="IsFirstOwner" Caption="IsFirstOwner" ShowOnEditForm="false">
        </aspx:Field>
        <aspx:Field FieldName="CreateDate" Caption="CreateDate" FieldType="Date" DateFormat="yyyy-MM-dd hh:mm:ss"
            ShowOnEditForm="false">
        </aspx:Field>
        <aspx:Field FieldName="Creator" Caption="Creator" ShowOnEditForm="false">
        </aspx:Field>
    </aspx:WebGridView>
    <asp:ObjectDataSource ID="CustomerOBDS" runat="server" SelectMethod="GetCustomer"
        UpdateMethod="UpdateCustomer" TypeName="Comfy.App.Web.QuailtyCode.CustomerEditForm" 
        DataObjectTypeName="Comfy.App.Core.QualityCode.QccustomerlibraryModel">
        <SelectParameters>
            <asp:Parameter Name="QualityCode" Type="String"  />
        </SelectParameters>
    </asp:ObjectDataSource>
</div>
<script>
    function EditOneComment() {
        customerGrid.EditOneRow();
    }
</script>
