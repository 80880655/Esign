<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Comfy.App.Web.Index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="Scripts/jquery1.4.js" type="text/javascript"></script>
    <script type="text/javascript">

        var targeturl = "Default.aspx";
       // alert("ss");
        // var newwin = window.open("", "", "scrollbars");
      //  window.location = targeturl;
        window.open(targeturl, 'newwindow', 'height=' + (screen.availHeight-45) + ',width=' + screen.availWidth + ',top=0,left=0,toolbar=no,menubar=no,scrollbars=no, resizable=no,location=no, status=no') 

      //  var newwin = window.open();
      //  newwin.
      //  if (document.all) {
         //   newwin.moveTo(0, 0);
          //  newwin.resizeTo(screen.availWidth, screen.availHeight);
       // }
    //    newwin.location = targeturl;
        window.opener = null;
        window.open('', '_parent', '');
        window.close();
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    </div>
    </form>
</body>
</html>
