<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GenericError.aspx.cs" Inherits="Comfy.App.Web.GenericError" %>

<%--<%@ Register Assembly="Comfy.UI.Web" Namespace="Comfy.UI.Web.ASPxEditors" TagPrefix="dxe" %>--%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script type="text/javascript" src="Scripts/Utilities.js"></script>

</head>
<body style="overflow: auto">
    <form id="form1" runat="server">
    <div style="padding: 50px">
        <table>
            <tr valign="top">
                <td>
                    <asp:Image ID="img" runat="server" ImageUrl="~/Images/Icon/Warning.gif" EnableViewState="False">
                    </asp:Image>
                </td>
                <td style="padding: 10px">
                    <asp:Literal runat="server" Text="<%$Resources:Caption%>"></asp:Literal>
                    <br />
                    <asp:Literal runat="server" Text="<%$Resources:Try%>"></asp:Literal>
                    <br />
                    *
                    <asp:HyperLink Text="<%$Resources:Return%>" ID="lnkReturn" runat="server">
                    </asp:HyperLink>
                    <br />
                    *
                    <asp:HyperLink Text="<%$Resources:Login%>" ID="lnkLogin" runat="server" NavigateUrl="Login.aspx">
                    </asp:HyperLink>
                </td>
            </tr>
        </table>
        <table>
            <tr valign="top">
                <td nowrap="nowrap">
                    <asp:Literal runat="server" Text="<%$Resources:Message%>"></asp:Literal>
                </td>
                <td style="padding: 10px">
                    <asp:Label ID="lblMessage" runat="server">
                    </asp:Label>
                </td>
            </tr>
        </table>
    </div>

    <script type="text/javascript">
        fixPng($get('img'));
    </script>

    </form>
</body>
</html>
