<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ToolBar.ascx.cs" Inherits="Comfy.App.Web.Core.UC.ToolBar" %>
<div class="PagerPanel">
    <table style="width: 100%;" cellpadding="0" cellspacing="0">
        <tr>
            <td align="right" style="padding-right: 10px">
                <asp:Menu OnMenuItemClick="Menu1_MenuItemClick" runat="server" ID="ToolBar123" Orientation="Horizontal" CssClass="NavigationMenu">
                    <StaticMenuItemStyle ItemSpacing="5" />
                    <DynamicMenuItemStyle CssClass="dynamicMenuItemStyle" />
                    <DynamicSelectedStyle CssClass="menuItemSelected" />
                    <DynamicHoverStyle CssClass="menuItemMouseOver" />
                    <DynamicMenuStyle CssClass="menuItem" />
                    <Items>
                        <asp:MenuItem Text="查找" ImageUrl="~/Images/ToolBar/Search.png"></asp:MenuItem>
                        <asp:MenuItem Text="清除" ImageUrl="~/Images/ToolBar/Clear.png"></asp:MenuItem>
                        <asp:MenuItem Text="新增" ImageUrl="~/Images/ToolBar/New.png"></asp:MenuItem>
                        <asp:MenuItem Text="編輯" ImageUrl="~/Images/ToolBar/Edit.png"></asp:MenuItem>
                        <asp:MenuItem Text="刪除" ImageUrl="~/Images/ToolBar/Delete.png"></asp:MenuItem>
                        <asp:MenuItem Text="審核" ImageUrl="~/Images/ToolBar/Cutomize.png"></asp:MenuItem>
                        <asp:MenuItem Text="導入" ImageUrl="~/Images/ToolBar/Import.png"></asp:MenuItem>
                        <asp:MenuItem Text="導出" ImageUrl="~/Images/ToolBar/Export.png">
                            <asp:MenuItem Text="XLS" ImageUrl="~/Images/ToolBar/xls.png"></asp:MenuItem>
                            <asp:MenuItem Text="CSV" ImageUrl="~/Images/ToolBar/csv.png"></asp:MenuItem>
                        </asp:MenuItem>
                        <asp:MenuItem Text="幫助" ImageUrl="~/Images/ToolBar/Help.png"></asp:MenuItem>
                    </Items>
                </asp:Menu>
            </td>
        </tr>
    </table>
</div>
