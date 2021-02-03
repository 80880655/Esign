<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="exportCsv.aspx.cs" Inherits="Comfy.App.Web.exportCsv" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <script language="javascript" type="text/javascript">

         function downloadFile(realName, outName, type) {
        
             //alert("/DownLoadFile.aspx?realName=" + realName + "&outName=" + outName + ((type != undefined && type.length > 0) ? "&type=" + type : ""), "");

             // PRD
             window.open("/QuailtyCode/DownLoadFile.aspx?realName=" + realName + "&outName=" + outName + ((type != undefined && type.length > 0) ? "&type=" + type : ""), "");


             // UAT
             //window.open("/QuailtyCode/DownLoadFile.aspx?realName=" + realName + "&outName=" + outName + ((type != undefined && type.length > 0) ? "&type=" + type : ""), "");
             
         }
    </script>
    <title></title>
</head>
<body>


    <form id="form1" runat="server">
    <div>
 <table width="100%" style="font-size: small;">
            <tr>
                <td>
                 
                        <legend>点击导出数据，格式为CSV </legend>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="Button1" runat="server" Text="ExportCSV"
                            onmouseover="this.style.backgroundPosition='left -42px'" onmouseout="this.style.backgroundPosition='left top'"
                            Class="btn btn-danger" OnClick="BtnExport_Click1" />
                        <br />
                     
                </td>
            </tr>
        </table>

    
    </div>
    </form>


</body>
</html>
