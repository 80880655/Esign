<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Modify_QC_HF_Log.aspx.cs" Inherits="Comfy.App.Web.QuailtyCode.Modify_QC_HF_Log" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register TagPrefix="Aspx" Namespace="Comfy.UI.WebControls.WebGridView" Assembly="Comfy.UI.WebControls" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <div style="height: 30px; font-weight: bolder; background-color:#FFCC73;vertical-align:bottom">
        <br>
        QC_Ref & HF_Ref Modify History</div>
    <div id="footer" style="height: 200px">
        <aspx:webgridview ID="ChangeLogGrid" runat="server" DataSourceId="ChangeLogOBDS" PageSize="10"
            CreateSearchPanel="false" CreateAddPanel="false" KeyFieldName="Iden">
            <Aspx:Field FieldName="Iden" Caption="Iden" Visible="false">
            </Aspx:Field>
            <Aspx:Field FieldName="QualityCode" Caption="QualityCode" Width="100">
            </Aspx:Field>
            <Aspx:Field FieldName="QC_Ref_PPO_Old" Caption="QC_Ref_PPO_Old">
            </Aspx:Field>
            <Aspx:Field FieldName="QC_Ref_GP_Old" Caption="QC_Ref_GP_Old">
            </Aspx:Field>
            <Aspx:Field FieldName="HF_Ref_PPO_Old" Caption="HF_Ref_PPO_Old" Width="100">
            </Aspx:Field>
            <Aspx:Field FieldName="HF_Ref_GP_Old" Caption="HF_Ref_GP_Old" Width="100">
            </Aspx:Field>
            <Aspx:Field FieldName="QC_Ref_PPO_New" Caption="QC_Ref_PPO_New" Width="100">
            </Aspx:Field>
            <Aspx:Field FieldName="QC_Ref_GP_New" Caption="QC_Ref_GP_New" Width="100">
            </Aspx:Field>
            <Aspx:Field FieldName="HF_Ref_PPO_New" Caption="HF_Ref_PPO_New" Width="100">
            </Aspx:Field>
            <Aspx:Field FieldName="HF_Ref_GP_New" Caption="HF_Ref_GP_New" Width="100">
            </Aspx:Field>
            <Aspx:Field FieldName="CreateDate" Caption="Modify Date" FieldType="Date" DateFormat="yyyy-MM-dd"
                Width="130">
            </Aspx:Field>
            <Aspx:Field FieldName="Creator" Caption="User ID" Width="100">
            </Aspx:Field>
        </aspx:webgridview>
        <asp:ObjectDataSource ID="ChangeLogOBDS" runat="server" SelectMethod="GetModelList"
            TypeName="Comfy.App.Core.QualityCode.QC_HF_ChangeLogManager"
            OldValuesParameterFormatString="original_{0}">
            <SelectParameters>
                <asp:QueryStringParameter Name="QualityCode" QueryStringField="QualityCode" 
                    Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </div>
    </div>
    </form>
</body>
</html>
