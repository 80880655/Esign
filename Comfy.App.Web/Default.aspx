<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Comfy.App.Web.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>
        <asp:Literal ID="ltlAppName" runat="server" Text="<%$Resources:MainRes,AppName%>"></asp:Literal>
    </title>
    <%--    <link rel="shortcut icon" href="favicon.ico" />--%>
    <script type="text/javascript" src="Scripts/ext-base.js"></script>
    <script type="text/javascript" src="Scripts/ext-all.js"></script>
    <script src="Scripts/jquery1.4.js" type="text/javascript"></script>
    <script src="Scripts/jquery.move.js" type="text/javascript"></script>
    <script type="text/javascript" src="Scripts/Utilities.js"></script>
    <style type="text/css">
        .mnuTopMenu
        {
            background-color: Transparent;
            font-family: 宋體;
            font-size: 12px;
            position: relative;
            top: 34px;
        }
        
        .DynamicMenuStyle /*動態功能表矩形區域樣式*/
        {
            background-color: Transparent;
            border: solid 1px #ACC3DF;
            padding: 1px 1px 1px 1px;
            text-align: left;
        }
        
        .DynamicHoverStyle /*動態功能表項:滑鼠懸停時的樣式*/
        {
            background-color: #F7DFA5; /*#7C6F57;*/
        }
        
        .DynamicSelectedStyle /*動態功能表項:選擇時的樣式*/
        {
            /*background-color:Gainsboro;*/
            color: red;
        }
        
        .DynamicMenuItemStyle /*動態功能表項樣式*/
        {
            padding: 0px 25px 2px 2px;
            color: #333333;
        }
        
        
        .StaticSelectedStyle /*靜態功能表項:選擇時的樣式*/
        {
            /*background-color:Gainsboro;*/
            color: red;
        }
        
        .StaticMenuItemStyle /*靜態功能表項樣式*/
        {
            cursor: hand;
            padding: 2px 5px 2px 5px;
            width: 90px;
            color: #333333;
            background-color: Transparent;
        }
        
        .StaticHoverStyle /*靜態功能表項:滑鼠懸停時的樣式*/
        {
            background-color: #84BCCD; /*#7C6F57;*/
            cursor: hand;
            color: #333333;
        }
    </style>
    <script type="text/javascript">
        if ((typeof Range !== "undefined") && !Range.prototype.createContextualFragment) {  
            Range.prototype.createContextualFragment = function (html) {
                var frag = document.createDocumentFragment(),
         div = document.createElement("div");
                frag.appendChild(div);
                div.outerHTML = html;
                return frag;
            };
        }
        var tempTab;
        function openTabByFun(val) {
              tempTab = val;
              $(function () {
                  if (tempTab == "1") {
                      setTimeout("openUrl('20', 'QuailtyCode/CreateQC.aspx?MG=Fabric', 'Create Quality Code', 'tabs')", 500);
                  }
                  else if (tempTab == "3") {
                      setTimeout("openUrl('21', 'QuailtyCode/EditQC.aspx?MG=Fabric', 'Maintain Quality Code', 'tabs')",500);
                  }
                  else if (tempTab == "2") {
                      setTimeout("openUrl('22', 'QuailtyCode/ApproveQC.aspx?MG=Fabric', 'Approve Quality Code', 'tabs')",500);
                  }
                  else {
                      setTimeout("openUrl('23', 'QuailtyCode/QueryQC.aspx', 'Search Quality Code', 'tabs')",500);
                  }
              });
        }
 </script>
 
</head>
<body scroll="no" style="height: 9000px">
    <form id="form1" runat="server">
    <div id="north" style="z-index: 10000">
        <div id="divHeader" style="height: 40px; overflow: hidden;">
            <table id="headerTable" style="width: 100%; height: 40px;" cellpadding="0" cellspacing="0">
                <tr>
                    <td align="left" style="padding-left: 13px; padding-top: 2px; width: 140px;">                    
                        <asp:Label ID="lblAppName" runat="server" Text="<%$Resources:MainRes,AppName%>" SkinID="lblAppName"
                            Width="140">
                        </asp:Label>
                        
                    </td>
                    <td align="left" valign="top">
                        <asp:Image ID="ImgLogl" runat="server" ImageUrl="~/Images/Icon/Logo.png" Width="54px"
                            Height="37PX" />
                    </td>
                    <%--                    <td align="right" style="width: 10px; padding-right: 15px">
                        <table cellpadding="0" cellspacing="0">
                            <tr valign="middle" style="height: 30px">
                                <td align="center" style="white-space: nowrap">
                                    &nbsp;</td>
                            </tr>
                        </table>
                    </td>--%>
                </tr>
            </table>
        </div>
    </div>
    <div id="center" style="border: 1px solid; padding: 2px 0 0px 2px; border-color: white;">
        <asp:Panel runat="server" ID="rpMenu" SkinID="RoundPanelNavigation" Width="100%">
            <asp:Panel ID="Panel2" runat="server">
                <div id="tabs" style="height: 9000px">
                </div>
            </asp:Panel>
        </asp:Panel>
    </div>
    <div id="south" style="border: 1px">
        <table id="bottomTable" cellpadding="0" cellspacing="0">
            <tr>
                <td width="20px" style="padding-left: 10px" align="right">
                    <asp:HyperLink ID="lnkUserIcon" runat="server" ImageUrl="~/Images/Icon/User.gif">
                    </asp:HyperLink>
                </td>
                <td width="200px" align="left" style="padding-left: 5px; white-space: nowrap;">
                    <asp:HyperLink ID="lblUserName" runat="server" Text="User" Font-Underline="False">
                    </asp:HyperLink>
                </td>
                <td align="right" style="padding-right: 10px; white-space: nowrap;">
                    <asp:Literal ID="ltlCopyRight" runat="server" Text="<%$Resources:MainRes,AppCopyright%>"></asp:Literal>
                    |
                    <asp:Literal ID="ltlVersion" runat="server" Text="<%$Resources:MainRes,AppVersion%>"></asp:Literal>
                    <%=System.Reflection.Assembly.GetAssembly(typeof(Comfy.App.Web.Default)).GetName().Version.ToString()%>
                </td>
            </tr>
        </table>
    </div>
    <div id="west" style="border: 1px solid; border-color: white;background-color:#FFCC73" >
        <asp:Panel ID="div_1" SkinID="RoundPanelNavigation" Width="98%" runat="server">
            <table style="width: 99%; height: 30px;" cellpadding="0" cellspacing="5">
                <tr>
                    <td style="width: 0%;">
                        <asp:Image ID="imNav2" runat="server" ImageUrl="~/Images/Icon/SideBar.gif"></asp:Image>
                    </td>
                    <td style="width: 100%;">
                        <asp:Label ID="lbNav2" runat="server" Text="<%$Resources:NavBar%>" SkinID="lblAppName">
                        </asp:Label>
                    </td>
                    <td style="width: 0%;">
                        <asp:ImageButton ID="imgClose" runat="server" ImageUrl="~/App_Themes/Aqua/Main/App/pcCloseButton.png"
                            ToolTip="<%$Resources:CloseNav%>" OnClientClick="CloseFun();" />
                    </td>
                </tr>
            </table>
            <div id="lineDiv" style="width: 100%; height: 1px; border: 1px solid White;">
            </div>
            <div id="westPanel" style="overflow: auto; width: 100%; padding-left: 4px;">
                <div id="navPanel" style="padding-top: 5px;">
                    <table>
                        <tr>
                            <td>
                                <div style="width: 150px; height: 50px;">
                                    <center style="margin: 10px 0 0px 0;">
                                        <div>
                                            <font color="#4B78CA" size="2">Material Group</font>
                                        </div>
                                        <div>
                                            <select id="mgSelect" style="width: 150px" onchange='ChangeTab()'>
                                                <option value="Fabric">Fabric</option>
                                                <option value="FlatKnit">Flat Knit</option>
                                                <option value="Tapping">Tapping</option>
                                            </select>
                                        </div>
                                    </center>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Button ID="btnCreate" runat="server" Text="<%$Resources:MenuCreate%>"  ForeColor="White" style="border:none"
                                    BackColor="#5DB85B" OnClientClick="Open(1)" Font-Bold="false" Height="50px" Width="150PX" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Button ID="btnApprove" runat="server" Text="<%$Resources:MenuApprove%>" ForeColor="White" style="border:none"
                                    BackColor="#FFA200" OnClientClick="Open(3)" Font-Bold="false" Height="50px" Width="150PX" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Button ID="btnMaintain" runat="server" Text="Maintain Quality Code" BackColor="#418BCA" ForeColor="White" style="border:none"
                                    OnClientClick="Open(2)" Font-Bold="false" Height="50px" Width="150PX" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Button ID="btnSearch" runat="server" Text="<%$Resources:MenuSearch%>" ForeColor="White" style="border:none"
                                    BackColor="#F97E76" OnClientClick="Open(4)" Font-Bold="false" Height="50px" Width="150PX" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </asp:Panel>
    </div>
    <script type="text/javascript">
        var popupDialog;
        var Toptabs;
        Ext.ux.TabCloseMenu = function () {
            var tabs, menu, ctxItem;
            this.init = function (tp) {
                tabs = tp;
                tabs.on('contextmenu', onContextMenu);
            }
            function onContextMenu(ts, item, e) {
                if (!menu) { // create context menu on first right click
                    menu = new Ext.menu.Menu([{
                        id: tabs.id + '-close',
                        text: 'Closs this page',
                        handler: function () {
                            tabs.remove(ctxItem);
                        }
                    }, {
                        id: tabs.id + '-close-others',
                        text: 'Closs other page',
                        handler: function () {
                            tabs.items.each(function (item) {
                                if (item.closable && item != ctxItem) {
                                    tabs.remove(item);
                                }
                            });
                        }
                    }
                    ]);
                }
                ctxItem = item;
                var items = menu.items;
                items.get(tabs.id + '-close').setDisabled(!item.closable);
                var disableOthers = true;
                tabs.items.each(function () {
                    if (this != item && this.closable) {
                        disableOthers = false;
                        return false;
                    }
                });
                items.get(tabs.id + '-close-others').setDisabled(disableOthers);
                menu.showAt(e.getPoint());
            }
        };
        layout = function () {
            Ext.state.Manager.setProvider(new Ext.state.CookieProvider());

            var viewport = new Ext.Viewport({
                layout: 'border',
                items: [
                new Ext.BoxComponent({ region: 'north', el: 'north', margins: '0 0 2 0', height: 43 }),
                new Ext.BoxComponent({ region: 'south', el: 'south', height: 25 }),
                {
                    region: 'west',
                    id: 'west-panel',
                    split: true,
                    width: 170,
                    minSize: 170,
                    maxSize: 400,
                    collapsible: true,
                    autoScroll: false,
                    margins: '0 0 0 5',
                    contentEl: 'west',
                    header: false,
                    border: false,
                    layoutConfig: { animate: true },
                    listeners: {
                        collapse: function () { $("#imgClose").hide(); },
                        expand: function () { $("#imgClose").show(); }
                    }
                },
                {
                    region: 'center',
                    id: 'center-panel',
                    split: true,
                    width: 200,
                    minSize: 200,
                    maxSize: 400,
                    collapsible: true,
                    autoScroll: false,
                    margins: '0 5 0 0',
                    contentEl: 'center',
                    header: false,
                    border: false,
                    listeners: {
                        resize: resizPanel
                    },
                    layoutConfig: { animate: true }
                }
                ]
            });
            var tabs = new Ext.TabPanel({
                id: 'tabs',
                renderTo: 'tabs',
                enableTabScroll: true,
                width: 600,
                height: 250,
                defaults: { autoScroll: false },
                plugins: new Ext.ux.TabCloseMenu()
            });
            tabs.add({
                html: '<iframe id="tabs_iframe_home" src="home.aspx" width="100%" height="100%" frameBorder="0"></iframe>',
                title: '<%=Resources.MainRes.AppHome%>',
                id: 'home',
                closable: false,
                autoScroll: true,
                iconCls: "icon_home",
                listeners: { activate: tabActivate }
            }).show();
            resizPanel();
            // openUrl('17', 'IE/ProposalList.aspx', '我的工作', 'tabs');
        };
        function resizPanel() {
            var c = $get("center-panel");
            var w = $get("westPanel");
            var tabs = Ext.getCmp("tabs");
            var west = Ext.getCmp("west-panel");
            if (c && w && tabs) {
                var n = $get("navPanel");
                var width = west.getInnerWidth() - 12;
                w.style.width = Math.max(width, 0) + "px";
                w.style.height = Math.max(c.offsetHeight - 42, 0) + "px";
                if (n.offsetHeight > w.offsetHeight) {
                    width -= 17;
                }
                n.style.width = Math.max(width, 0) + "px";
                tabs.setSize(c.offsetWidth - 12, c.offsetHeight - 13);
                $get("tabs").style.height = "100%";
                if (DXie) {
                    try {
                        ResizeIFrame($get('tabs_iframe_' + tabs.getActiveTab().id).contentWindow);
                    } catch (e) { } //exception will be thrown when contentWindow from another domain
                }
            }
        }
        openUrl = function (id, url, title, target) {
            if (target == "tabs")
                addTab(id, url, title);
            else if (target == "popup")
                OpenForm(title, url);
            else
                window.open(url, target);
        }
        reloadTab = function () {
            var tabs = Ext.getCmp("tabs");
            if (tabs)
                $get('tabs_iframe_' + tabs.getActiveTab().id).contentWindow.location.reload();
        }
        openTab = function (id) {
            var tabs = Ext.getCmp("tabs");
            if (tabs)
                tabs.setActiveTab(id);
        }
        tabActivate = function (tab) {
            resizPanel();
        }
        function addTab(id, url, title) {
            title = decodeURI(title);
            var tabs = Ext.getCmp("tabs");
            var tab = tabs.getComponent(id);
            if (!tab) {
                tab = tabs.add({
                    id: id,
                    title: title,
                    html: '<iframe id="tabs_iframe_' + id + '" src="' + url + '" width="100%" height="100%" frameBorder="0"></iframe>',
                    closable: true,
                    listeners: { activate: tabActivate }
                });
            }
            tabs.setActiveTab(tab);
        }
        Ext.onReady(layout);

        //        function ShowForm(title, url, width, height) {
        //            width = width || Math.max(DXGetDocumentClientWidth(window) - 50, 100);
        //            height = height || Math.max(DXGetDocumentClientHeight(window) - 100, 100);
        //            pcForm.SetHeaderText(title);
        //            pcForm.SetSize(width, height);
        //            pcForm.SetContentHtml('');
        //            pcForm.SetContentUrl(url);
        //            if (pcForm.IsVisible())
        //                pcForm.UpdatePosition();
        //            pcForm.Show();
        //        }

        function ShowForm(title, url, width, height) {
            var options = "{modal:true}";
            options = $.extend({ title: "" + title + "" }, options || {});
            var dialog = new Boxy("<div style=\"width:" + width + "px;height:" + height + "px \"><iframe id=\"IframeEdit\" frameborder=\"0\"  style=\"width:100%;height:100%\" src=\"" + url + "\"></iframe></div>", options);
            popupDialog = dialog;

        }


        function Show(ida, idb) {
            if (document.getElementById(idb).style.display == "block") {
                $("#" + ida).attr("src", "Images/nbExpand.png");
                $("#" + idb).hide(200);
            }
            else {
                $("#" + ida).attr("src", "Images/nbCollapse.png");
                $("#" + idb).show(200);
            }
        }

        function CloseFun() {
            Ext.getCmp('west-panel').collapse();
        }

        function Refresh() {
            reloadTab();
        }

        function closeCreate() {
            var tab = Ext.getCmp("tabs");
            tab.remove(20);
        }
        function closeApprove() {
            var tab = Ext.getCmp("tabs");
            tab.remove(22);
        }
        function closeEdit() {
            var tab = Ext.getCmp("tabs");
            tab.remove(21);
        }
 

        function ChangeTab() {
            var Toptabs = Ext.getCmp("tabs");
            var tid = Toptabs.getActiveTab().id;
            if (tid == '20') {
                if (confirm("Do you want to change to: " + $("#mgSelect").val() + "?")) {
                    Toptabs.remove(20);
                    openUrl('20', 'QuailtyCode/CreateQC.aspx?MG=' + $("#mgSelect").val(), 'Create Quality Code', 'tabs');
                }
            }
            if (tid == '21') {
                if (confirm("Do you want to change to: " + $("#mgSelect").val() + "?")) {
                    Toptabs.remove(21);
                    openUrl('21', 'QuailtyCode/EditQC.aspx?MG=' + $("#mgSelect").val(), 'Maintain Quality Code', 'tabs');
                }
            }
            if (tid == '22') {
                if (confirm("Do you want to change to: " + $("#mgSelect").val() + "?")) {
                    Toptabs.remove(22);
                    openUrl('22', 'QuailtyCode/ApproveQC.aspx?MG=' + $("#mgSelect").val(), 'Approve Quality Code', 'tabs');
                }
            }

        }
        //Add by zheng zhou
        var qcValue;
        //end by zheng zhou
        function Open(val) {
            var Toptabs = Ext.getCmp("tabs");
            if (val == 1) {
                Toptabs.remove(20);
                if ($("#mgSelect").val() == "Fabric") {
                    openUrl('20', 'QuailtyCode/CreateQC.aspx?MG=Fabric', 'Create Quality Code', 'tabs');
                }
                else if ($("#mgSelect").val() == "FlatKnit") {
                    openUrl('20', 'QuailtyCode/CreateQC.aspx?MG=FlatKnit', 'Create Quality Code', 'tabs');
                }
                else if ($("#mgSelect").val() == "Tapping") {
                    openUrl('20', 'QuailtyCode/CreateQC.aspx?MG=Tapping', 'Create Quality Code', 'tabs');
                }
            }
            else if (val == 2) {
                Toptabs.remove(21);
                if ($("#mgSelect").val() == "Fabric") {
                    openUrl('21', 'QuailtyCode/EditQC.aspx?MG=Fabric', 'Maintain Quality Code', 'tabs');
                }
                else if ($("#mgSelect").val() == "FlatKnit") {
                    openUrl('21', 'QuailtyCode/EditQC.aspx?MG=FlatKnit', 'Maintain Quality Code', 'tabs');
                }
                else if ($("#mgSelect").val() == "Tapping") {
                    openUrl('21', 'QuailtyCode/EditQC.aspx?MG=Tapping', 'Maintain Quality Code', 'tabs');
                }
            }
            if (val == 3) {
                Toptabs.remove(22);
                if ($("#mgSelect").val() == "Fabric") {
                    $("#baseFormBackDiv").hide();
                    openUrl('22', 'QuailtyCode/ApproveQC.aspx?MG=Fabric&qc=' + qcValue, 'Approve Quality Code', 'tabs');
                }
                else if ($("#mgSelect").val() == "FlatKnit") {
                    $("#baseFormBackDiv").hide();
                    openUrl('22', 'QuailtyCode/ApproveQC.aspx?MG=FlatKnit&qc=' + qcValue, 'Approve Quality Code', 'tabs');
                }
                else if ($("#mgSelect").val() == "Tapping") {
                    $("#baseFormBackDiv").hide();
                    openUrl('22', 'QuailtyCode/ApproveQC.aspx?MG=Tapping&qc=' + qcValue, 'Approve Quality Code', 'tabs');
                }
            }
            if (val == 4) {
                openUrl('23', 'QuailtyCode/QueryQC.aspx', 'Search Quality Code', 'tabs');
            }
            return false;
        }

        function saveAs(val) {
            var tab = Ext.getCmp("tabs");
            tab.remove(20);
            openUrl('20', val, 'Create Quality Code', 'tabs');

        }

        function Maintain(val) {
            var tab = Ext.getCmp("tabs");
            tab.remove(21);
            openUrl('21', val, 'Maintain Quality Code', 'tabs');
        }
        function ApproveWin(val) {
            var tab = Ext.getCmp("tabs");
            tab.remove(22);
            openUrl('22', val, 'Approve Quality Code', 'tabs');
        }
        function openSearch() {
            var tab = Ext.getCmp("tabs");
            tab.remove(23);
            openUrl('23', 'QuailtyCode/QueryQC.aspx', 'Search Quality Code', 'tabs');
        }

        function openSearchOne(val) {
            var tab = Ext.getCmp("tabs");
            tab.remove(23);
            openUrl('23', 'QuailtyCode/QueryQC.aspx?QC='+val, 'Search Quality Code', 'tabs');
        }

    </script>
    </form>
</body>
</html>
