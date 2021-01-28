<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="export.aspx.cs" Inherits="YsYarnWHAutoRejection.export" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
           
    
<head runat="server">
     <script language="javascript" type="text/javascript">

         function downloadFile(realName, outName, type) {
        
             //alert("/DownLoadFile.aspx?realName=" + realName + "&outName=" + outName + ((type != undefined && type.length > 0) ? "&type=" + type : ""), "");
             window.open("/QuailtyCode/DownLoadFile.aspx?realName=" + realName + "&outName=" + outName + ((type != undefined && type.length > 0) ? "&type=" + type : ""), "");
             
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
                 
                        <legend>点击导出数据</legend>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="BtnExport" runat="server" Text="Export"
                            onmouseover="this.style.backgroundPosition='left -42px'" onmouseout="this.style.backgroundPosition='left top'"
                            Class="btn btn-danger" OnClick="BtnExport_Click" />
                        <br />
                     
                </td>
            </tr>
        </table>

    
    </div>
    </form>
</body>
</html>
