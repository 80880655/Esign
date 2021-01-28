<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestAXInterface.aspx.cs" Inherits="Comfy.App.Web.QuailtyCode.TestAXInterface" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="../Scripts/jquery1.4.js" type="text/javascript"></script>
    <script type="text/javascript" src="../Scripts/Utilities.js"></script>
    <script src="../Scripts/Combo.js" type="text/javascript"></script>
    <title></title>
    <script>
        $(function () {
            $("#btnCallAx").click(function () {
                var result1 = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "CallWS_AX", $("#QC").val());
                alert(result1);
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      Quality Code <input  type="text" id="QC"/><input type="button" id="btnCallAx" value="测试"/>
    </div>
    </form>
</body>
</html>
