<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Comfy.App.Web.Home" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>
        <asp:Literal runat="server" Text="Home - "></asp:Literal>
        <asp:Literal ID="ltlAppName" runat="server" Text="<%$Resources:MainRes,AppName %>"></asp:Literal>
    </title>
    <script type="text/javascript" src="Scripts/Utilities.js"></script>
</head>
<body style="height: 100%;">
    <form id="form1" runat="server">
    <div id="content" style="overflow: auto;">
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
            <tr>
                <td >
                            <asp:Image  runat="server" ImageUrl="~/Images/Icon/Main.png"/>
                </td>
            </tr>
        </table>
        <br />
    </div>
    <asp:XmlDataSource ID="XmlDataSource1" runat="server" DataFile="~/App_Data/HomeXMLDataSource.xml"
        XPath="//Item"></asp:XmlDataSource>
    </form>
</body>
</html>
