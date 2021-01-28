<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Comfy.App.Web.Login" %>

<%--<%@ Register Assembly="Comfy.UI.Web" Namespace="Comfy.UI.Web.ASPxEditors" TagPrefix="dxe" %>--%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
    <link href="Css/portlets.css" rel="stylesheet" type="text/css" />
    <title>
        <asp:Literal ID="ltlTitle" runat="server" Text="<%$Resources:Login %>"></asp:Literal>
        <asp:Literal ID="ltl_" runat="server" Text=" - "></asp:Literal>
        <asp:Literal ID="ltlAppName" runat="server" Text="<%$Resources:MainRes,AppName %>"></asp:Literal>
    </title>
    <script type="text/javascript" src="Scripts/Utilities.js"></script>
    <script type="text/javascript">
        //<![CDATA[
        function load() {
            if (parent.window.location != window.location)
                parent.window.location = window.location;
            document.getElementById("txtUser").focus();
        }
        //]]>
    </script>
</head>
<body onload="load();" style="height: 100%">
    <form id="form1" runat="server">
    <div id="header" style="width: 1px;">
    </div>  
    <table style="width: 100%; height: 100%;" id="content">
        <tr valign="middle">
            <td align="center">
                <div valign="top" style="height: 400px">
                    <table class="table_login" border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td colspan="3" class="top_login">
                            </td>
                        </tr>
                        <tr>
                            <td class="left_login">
                            </td>
                            <td class="space_login">
                                <table class="table_inner_login" cellpadding="0" cellspacing="0">
                                    <tr class="header_login">
                                        <td colspan="2" valign="middle">
                                            <table style="width: 100%;" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="center">
                                                        <asp:Label ID="ASPxLabel1" runat="server" Text="<%$ Resources:MainRes,AppName %>"
                                                            SkinID="lblAppName" Font-Size="X-Large"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td width="170px" valign="middle" align="right">
                                            <asp:Image ID="ASPxImage3" runat="server" SkinID="imgComputer"></asp:Image>
                                        </td>
                                        <td valign="bottom">
                                            <table class="table_input_login" width="100%" cellpadding="1" cellspacing="1">
                                                <tr>
                                                    <td nowrap="nowrap" align="right">
                                                        <asp:Label ID="lblUser" runat="server" Text="<%$Resources:UserId %>">></asp:Label>
                                                    </td>
                                                    <td align="left" width="300px;">
                                                        <asp:TextBox ID="txtUser" runat="server" Width="140px">
                                                        </asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td nowrap="nowrap" align="right">
                                                        <asp:Label ID="lblPwd" runat="server" Text="密碼:"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtPwd" runat="server" Width="140px" TextMode="Password"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <%--                                                <tr>
                                                    <td nowrap="nowrap" align="right">
                                                        <asp:Label ID="lbSysOrg" runat="server" Text="組織:"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="ddlSysOrg" runat="server" Width="144px" 
                                                            DataSourceID="odsOrg" DataTextField="SystemName" DataValueField="Ikey" 
                                                            ondatabound="ddl_DataBound">
                                                            </asp:DropDownList>
                                                    </td>
                                                </tr>--%>
                                                <tr>
                                                    <td>
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:CheckBox ID="cboPersist" runat="server" Text="<%$Resources:Persist %>"></asp:CheckBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                    </td>
                                                    <td align="left">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:Button ID="btnLogin" runat="server" Text="登入" OnClick="btnLogin_Click"></asp:Button>
                                                                </td>
                                                                <td style="padding-left: 20px;">
                                                                    <asp:HyperLink ID="hlkForgetPwd" runat="server" Text="<%$Resources:ForgotPassword %>" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                            <div style="height: 15px; padding-right: 10px; text-align: right">
                                                <asp:Label ID="lblInfo" runat="server" ForeColor="Red">
                                                </asp:Label>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td class="right_login">
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" class="bottom_login">
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td>
                <asp:ObjectDataSource ID="odsOrg" runat="server" SelectMethod="GetModelList" TypeName="Comfy.App.Core.SignForIE.SysSetting.SysOrg.OrgSystemManger">
                </asp:ObjectDataSource>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
