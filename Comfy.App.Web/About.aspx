<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="Comfy.App.Web.About" %>


<%--<%@ Register Assembly="Comfy.UI.Web" Namespace="Comfy.UI.Web.ASPxEditors" TagPrefix="dxe" %>--%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%;" cellpadding="0" cellspacing="0">
            <tr class="header_login">
                <td colspan="2" valign="top">
                    <table style="width: 100%;" cellpadding="0" cellspacing="0">
                        <tr>
                            <td style="width: 1px; padding-left: 20px;">
                                <asp:HyperLink ID="lnkLogo" runat="server" Text="" SkinID="hlLogo" NavigateUrl="<%$Resources:MainRes,AppLogoLink%>"
                                    EnableDefaultAppearance="false" EnableViewState="false" Target="_blank">
                                </asp:HyperLink>
                            </td>
                            <td style="width: 1px">
                                <table cellpadding="0" cellspacing="0">
                                    <tr valign="bottom" style="height: 30px">
                                        <td align="center" style="white-space: nowrap">
                                            <asp:Label ID="lblAppName" runat="server" Text="<%$Resources:MainRes,AppName%>"
                                                SkinID="lblAppName">
                                            </asp:Label>
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td align="center" style="white-space: nowrap">
                                            <asp:Image ID="imgLt" runat="server" SkinID="imgLt">
                                            </asp:Image>
                                            <asp:Label ID="lblAppNameEn" runat="server" Text="<%$Resources:MainRes,AppNameEn%>"
                                                SkinID="lblAppNameEn">
                                            </asp:Label>
                                            <asp:Image ID="imgRt" runat="server" SkinID="imgRt">
                                            </asp:Image>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td align="right">
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table style="width:100%">
                        <tr>
                            <td class="labelCell">
                                版本
                            </td>
                            <td class="controlCell">
                                4.0
                            </td>
                        </tr>
                        <tr>
                            <td class="labelCell">
                            公司
                            </td>
                            <td class="controlCell">
                                MOEG
                            </td>
                        </tr>
                        <tr>
                            <td class="labelCell">
                            聯系
                            </td>
                            <td class="controlCell">
                                ITS/MES
                            </td>
                        </tr>
                        <tr>
                            <td class="labelCell">
                                說明
                            </td>
                            <td class="controlCell">
                                <asp:TextBox runat="server" ReadOnly="true" Width="300px" Height="80px" 
                                    Text="版權所有，盜版必究" TextMode="MultiLine"></asp:TextBox>
                            </td>
                           
                        </tr>
                    </table>
                </td>
            </tr>
        </table>


    </div>
    </form>
</body>
</html>
