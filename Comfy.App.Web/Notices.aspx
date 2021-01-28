<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Notices.aspx.cs" Inherits="Comfy.App.Web.Notices" %>

<%@ Import Namespace=" Comfy.App.Core.Notice" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<%
    string IKey = Server.UrlDecode(Request.QueryString["IKey"]);
    AppNoticeModel notice = new AppNoticeManager().GetOneModel(IKey);
%>
<head id="Head1" runat="server">
    <title></title>
</head>
<body style="font-family:'Times New Roman',Georgia,Serif;">
<center>
<br />
<br />
    <table width="100%" style="margin:0px">
        <tr style="margin: 4px">
            <td align="right">
               <font color="green"> 標題：</font>
                <br />
            </td>
            <td align="left">
                <%=notice.HeaderText %>
                <br />
            </td>
        </tr>
        <tr><td colspan="2"><hr /></td></tr>
        <tr style="margin: 4px">
            <td align="right">
                <font  color="green"> 發佈時間：</font>
                <br />
            </td>
            <td align="left">
                <%=notice.ReleaseDate.ToString().Substring(0,10) %>
                <br />
            </td>
        </tr>
          <tr><td colspan="2"><hr /></td></tr>
        <tr style="margin: 4px">
            <td align="right">
               <font color="green"> 內容：</font>
            </td>
            <td align="left">
                <textarea rows="20" style="width: 640px; border-width:1px;  " readonly="true"><%=notice.Text %></textarea>
            </td>
        </tr>
    </table>
    </center>
</body>
</html>