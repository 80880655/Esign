<%@ Page Language="C#" enableEventValidation="false" AutoEventWireup="true" CodeBehind="CreateQC.aspx.cs" Inherits="Comfy.App.Web.QuailtyCode.CreateQC" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Src="Attribute.ascx" TagName="Attribute" TagPrefix="ASPx" %>
<%@ Register Src="FlagAttribute.ascx" TagName="FlatAttribute" TagPrefix="ASPx" %>
<%@ Register Src="TappingAttribute.ascx" TagName="TapAttribute" TagPrefix="ASPx" %>
<%@ Register Src="CustomerEditForm.ascx" TagName="CustomerEditForm" TagPrefix="ASPx" %>
<%@ Register Src="AvaWidth.ascx" TagName="Ava" TagPrefix="ASPx" %>
<%@ Register TagPrefix="Aspx" Namespace="Comfy.UI.WebControls.WebGridView" Assembly="Comfy.UI.WebControls" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../Scripts/jquery1.4.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.cookie.js" type="text/javascript"></script>
    <script type="text/javascript" src="../Scripts/Utilities.js"></script>
    <script src="../Scripts/Combo.js" type="text/javascript"></script>
    <script src="../Scripts/form.checker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../Scripts/ccftag.js"></script>
    <script type="text/javascript" src="../Scripts/ext-base.js"></script>
    <script type="text/javascript" src="../Scripts/ext-all.js"></script>
    <script src="../Scripts/jquery.move.js" type="text/javascript"></script>
    <style>
        .myButton
        {
            border: 1px solid #FFA200;
        }
                html
        {
            overflow-y:auto;
            overflow-x:hidden;
        }
    </style>

    <script language="javascript" type="text/javascript">
        var GMType;
        var TempBu = '';
        var TempYuan = '';
        var shroudFlag = '';
        var ParamsCreate = {};
        $(function () {


            QCRefShow(2);

            var userResult = AjaxCallFun("Comfy.App.Core", "Comfy.App.Core.QualityCode.CustomerManager", "GetUserInfoByUserId", null);

            if (userResult != "") {
                var userInfo = userResult.split('<|>');
                TempBu = userInfo[1];
                $("#SalesTeam").val(userInfo[1]);
                TempYuan = "<option value='" + userInfo[0] + "'>" + userInfo[2] + "</option>";
                $("#Sales").prepend("<option value='" + userInfo[0] + "'>" + userInfo[2] + "</option>");
                $("#txtSales").val(userInfo[0]);
            }
            else {
                $("#SalesTeam").val("");
            }
        });
        function shroudDiv(myId) {
            if (myId.length == 0)
                return;
            var left = myId.offset().left;
            var top = myId.offset().top;
            var width = myId.outerWidth(true);
            var height = myId.outerHeight(true);
            $("<div class='divShroud'>").width(width).height(height).css({ "position": "absolute", "z-index": "100", "left": left, "top": top, "background-color": "#CCC", "opacity": "0.3" }).appendTo(document.body);
        }

        function shroudDiv1(myId) {
            if (myId.length == 0)
                return;
            var left = myId.offset().left;
            var top = myId.offset().top;
            var width = myId.outerWidth(true);
            var height = myId.outerHeight(true);
            $("<div class='divShroud'>").width(width).height(height).css({ "position": "absolute", "z-index": "10001", "left": left, "top": top, "background-color": "#CCC", "opacity": "0.3" }).appendTo(document.body);
        }
        function initQC(MG, QC, cuId) {
            $(function () {
                if (cuId != '') {

                    $("#customerHidden").val(cuId);
                }
                if (QC != "") {
                    $("#tQC").val(QC);
                    $("#tCustomerId").val(cuId);
                    var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.CreateQC", "InitInfo", QC);
                    var strTemp;
                    $("#AnalysisNo").val(result.split("<|>")[14]);
                    $("#Sourcing").val(result.split("<|>")[15]);
                    if (result != "") {
                        if (GMType == "Fabric") {
                            InitTable(result.split("<|>")[10]);
                            cInit(result,'');
                            $("#txtAvaRemark").val(result.split("<|>")[18]);    

                        } else if (GMType == "FlatKnit") {
                            fInit(result,'');
                        } else if (GMType == "Tapping") {
                            tInit(result,'');
                        }
                    }

                    var tempStr = QC + ",Qb," + cuId;

                    var customerMessage = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.CreateQC", "GetCustomerInfo", tempStr);
                    if (customerMessage != "") {
                        var temp = customerMessage.split("<?>");
                        if (temp[0] != "") {
                            $("#customer").val(temp[0].split("<|>")[0]);
                            $("#tchange").val(temp[0].split("<|>")[1]);
                        }
                        $("#SalesTeam").val(temp[1]);
                        $("#SalesTeam").change();
                        $("#txtSales").val(temp[2]);
                        $("#Sales").val(temp[2]);
                      //  $("#Sales option").remove();
                     //   $("#Sales").prepend("<option value='" + temp[2] + "'>" + temp[2] + "</option>");
                        $("#CustomerQC").val(temp[3]);
                        $("#brand").val(temp[4]);
                        $("#GEKComments").val(temp[5]);
                    }
                    // searchStr.QualityCode = val[0];
                    //   setTimeout("customerGrid.GridCheck(searchStr)", 1000);
                }
            })
        }

        $(function () {
            $("#btnQcSearch").click(function () {
                if (SearchQC($("#tQC").val(), true)) {
                    eidtShroud();
                    $("#btnEditQC").attr("disabled", "");
                    return false;
                }
                return false;
            });

        })
        /*     function SearchQCC() {
        if (SearchQC($("#tQC").val(),true)) {
        eidtShroud();
        $("#btnEditQC").attr("disabled", "");
        return false;
        }
        return false;
        }*/

        function SearchQC(tempQC, editFlag) {
            if ((GMType == "Fabric" && tempQC.indexOf("C") < 0 && tempQC.indexOf("E") < 0) ||
               (GMType == "FlatKnit" && tempQC.indexOf("F") < 0 && tempQC.indexOf("E") < 0) ||
               (GMType == "Tapping" && tempQC.indexOf("T") < 0 && tempQC.indexOf("E") < 0)) {
                alert("Material Group is wrong,Please change the material group.");
                return false;
            }
            if (tempQC != "") {
                var strTemp;

                var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.CreateQC", "InitInfo", tempQC); //如果只有状态为Approved的才能查询出来，那么这里的查询语句要加多一个判断。
       
        
                var tgmt = result.split("<|>")[17];

                $("#txtGkNo").val(result.split("<|>")[19]);

            
                if (tgmt == "Flat Knit Fabric") {
                    tgmt = "FlatKnit";
                }

                if (tgmt != GMType) {
                    alert("Please change MaterialGroup.The MaterialGroup of this QualityCode is:" + tgmt);
                    return false;
                }

                if (result == "No") {
                    alert("No such quality code!");
                    return false;
                }
                if (editFlag && result.split("<|>")[16] != "Developed" && result.split("<|>")[16] != "New" && result.split("<|>")[16] != "NEW") {
                    alert("The status is not correct：" + result.split("<|>")[16]);
                    return false;
                }
                if (result != "") {
                    
                    $("#AnalysisNo").val(result.split("<|>")[14]);
                    $("#Sourcing").val(result.split("<|>")[15]);
                    if (GMType == "Fabric") {
                       
                        InitTable(result.split("<|>")[10]); // Quality Width
                       
                        cInit(result,'');
                      
                        $("#txtAvaRemark").val(result.split("<|>")[18]);
                       
                    }
                    else if (GMType == "FlatKnit") {
                        fInit(result, '');
                    } else if (GMType == "Tapping") {
                        tInit(result, '');
                    }
                }
                $("#HiddenQC").val(tempQC);
                if ($("#customerHidden").val() != '') {
                    tempQC = tempQC + '@' + $("#customerHidden").val();
                }
                var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.CreateQC", "GetCustomerInfoOne", tempQC);
                if (result != "") {
                    var attr = result.split("<|>");
                    $("#HiddenCuId").val(attr[0]);
                    $("#tchange").val(attr[2]);
                    $("#customer").val(attr[1]);
                    $("#Sales option").remove();
                    $("#Sales").prepend("<option value='" + attr[3] + "'>" + attr[4] + "</option>");
                    $("#SalesTeam").val(attr[5]);
                    $("#CustomerQC").val(attr[6]);
                    $("#brand").val(attr[7]);
                    $("#GEKComments").val(attr[8]);
                
                }
               
                return true;

            }
        }
        function eidtShroud() {
            $(".divShroud").remove();
            shroudDiv($("#headDiv"));
            shroudDiv($("#attrPanel"));
            shroudDiv($("#GEKComments"));
            shroudDiv($("#avaWidthPanel"));
            shroudDiv($("#avaRemark"));
            shroudFlag = "1";
            $("#btnCopyFrom").attr("disabled", "disabled");
            $("#create").attr("disabled", "disabled");
            $("#btnSaveQC").attr("disabled", "disabled");
            $("#reset").attr("disabled", "disabled");
            $("#cancel").attr("disabled", "disabled");

        }
        function closeSD() {
            $("#sucessDiv").hide();
        }

        function SaveSucess(val) {
            $(function () {
                $("#tQC").val(val);
                /* $("#sucessDiv").css({
                "top": Math.max(($(window).height() / 2 - $("#sucessDiv").outerHeight() / 2 + $(window).scrollTop()), 0) + "px",
                "left": Math.max(($(document.body).width() / 2 - $("#sucessDiv").outerWidth() / 2 + $(window).scrollLeft()), 0) + "px",
                "display": "block"
                });*/
            });
        }


        function setCookie(QC) {
            $.cookie("QualityCode", QC);
            window.opener = null;
            window.open('', '_self');
            window.close();
        }

        function setGMType(val, QC, cuId, aqc) {
            GMType = val;
            if (cuId != '') {
                $(function () {
                    $("#customerHidden").val(cuId);
                });
         
            }
            if (QC != "") {
                initQC(val, QC, cuId);
                $(function () {
                    $('#btnQcSearch').trigger("click");     
                });

              //  $("#btnQcSearch").onFire()
            }
            if (aqc != "") {
                $(function () {
                    $("#tQC").val(aqc);
                    $("#txtCopyQC").val(aqc);
                    finishCopy();
                    alert("Success!");
                    /*  $("#afterQC").text("The quality code is:  " + aqc);
                    $("#sucessDiv").css({
                    "top": Math.max(($(window).height() / 2 - $("#sucessDiv").outerHeight() / 2 + $(window).scrollTop()), 0) + "px",
                    "left": Math.max(($(document.body).width() / 2 - $("#sucessDiv").outerWidth() / 2 + $(window).scrollLeft()), 0) + "px",
                    "display": "block"
                    });*/
                });
            }

        }
        //        function setGMType(val, QC) {
        //            GMType = val;
        //            if (QC != "") {
        //                $(function () {
        //                    $("#afterQC").val("The quality code is:" + QC);
        //                    $("#sucessDiv").css({
        //                        "top": Math.max(($(window).height() / 2 - $("#sucessDiv").outerHeight() / 2 + $(window).scrollTop()), 0) + "px",
        //                        "left": Math.max(($(document.body).width() / 2 - $("#sucessDiv").outerWidth() / 2 + $(window).scrollLeft()), 0) + "px",
        //                        "display": "block"
        //                    });
        //                });

        //            } else {
        //                //  alert("Fail");
        //            }
        //        }
        function showQCDetail(strQC) {
            window.parent.openSearchOne(strQC);
        }

        function showSameQC(result, result_customer) {

            $('#sameQCDiv').html("");
            var strHtml;
            strHtml = "<table><tr>";
            $("#sameDiv").css({
                "top": Math.max(($(window).height() / 2 - $("#sameDiv").outerHeight() / 2 + $(window).scrollTop()), 0) + "px",
                "left": Math.max(($(document.body).width() / 2 - $("#sameDiv").outerWidth() / 2 + $(window).scrollLeft()), 0) + "px",
                "display": "block"
            });
            var retStrs = result.split(",");
            var retStrs_customer = result_customer.split("|");
            for (var i = 0; i < retStrs.length; i++) {
                if (i % 6 == 0) { 
                   strHtml=strHtml+"</tr>"
                }
               if (retStrs[i] != "") {
                   strHtml = strHtml + "<tr><td><u onclick=showQCDetail('" + retStrs[i] + "')>" + retStrs[i] + "</u>&nbsp;&nbsp;&nbsp;&nbsp;" + retStrs_customer[i] + "</td></tr>";
                }

            }
            strHtml = strHtml + "</table>"

            $(strHtml).appendTo("#sameQCDiv");

        }

        //添加客户名称
        function ShowCustomer(result) {
            $('#customerDiv').html("");
            strHtml = "<table><tr>";
            $("#customerDiv").css({
                "top": Math.max(($(window).height() / 2 - $("#customerDiv").outerHeight() / 2 + $(window).scrollTop()), 0) + "px",
                "left": Math.max(($(document.body).width() / 2 - $("#customerDiv").outerWidth() / 2 + $(window).scrollLeft()), 0) + "px",
                "display": "block"
            });
            var retStrs = result.split(",");
            for (var i = 0; i < retStrs.length; i++) {
                if (i % 6 == 0) {
                    strHtml = strHtml + "</tr><tr>"
                }
                if (retStrs[i] != "") {
                    strHtml = strHtml + "<td>Customer:" + retStrs[i] + "</td>";
                }

            }
            strHtml = strHtml + "</tr></table>"

            $(strHtml).appendTo("#customerDiv");
        }

        $(function () {

            if (GMType == "Fabric") {
                $("#avaDiv").css({ "height": "170px" });
            }

            if (GMType == "FlatKnit") {
                $("#trHC").hide();
            }

            $("#customer").hide();
            $("#tchange").change(function () {
                $("#customer").val("");
            });
            $("#txtSales").hide();

            $("#create").click(function () {
                //Add by sunny 20171106 在点击Create按钮的时候，对Ratio进行判断
                var ratioresult = "";
                ratioresult = AjaxCallFunOne("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.CreateQC", "EditVerificationRatio", "");
                if (ratioresult == "falseone") {
                    alert("纱类Radio不可以为空");
                    return false;
                }
                if (ratioresult == "falsetwo") {
                    alert("纱类Radio加起来必须等于100");
                    return false;
                }





                shroudDiv($("#create"));

                if ($("#customer").val() == "") {
                    alert("Customer can no be null!");
                    $(".divShroud").remove();
                    return false;
                }
                if ($("#SalesTeam").val() == "") {
                    alert("SalesTeam can no be null!");
                    $(".divShroud").remove();
                    return false;
                }
                if ($("#Sales").val() == "") {
                    alert("Sales can no be null!");
                    $(".divShroud").remove();
                    return false;
                }
                if ($("#Sales").val() == "") {
                    alert("Sales can no be null!");
                    $(".divShroud").remove();
                    return false;
                }

                var tempCon = GMType + "<>" + $("#Sourcing").val() + "<>" + getFAttribute("CreateQC") + $("#txtGkNo").val();
                var result = AjaxCallFunOne("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.CreateQC", "GetTheSaveQC", tempCon + "#@Create");
                var result_customer = AjaxCallFunOne("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.CreateQC", "GetCustomer", result);
                //sunny 测试 20170725
                alert("result的长度" + result.length());

                if (result != "") {
                    showSameQC(result,result_customer);
                    //ShowCustomer(result_customer);
                    $(".divShroud").remove();
                    return false;
                }

                return true;
            });

            $("#Sales").change(function () {
                $("#txtSales").val($("#Sales").val());
            });
            $("#SalesTeam").change(function () {
                var param = $("#SalesTeam").val();
                if (param != "") {
                    var result = AjaxCallFun("Comfy.App.Core", "Comfy.App.Core.QualityCode.CustomerManager", "GetUserIdByDepartment", param);
                    if (result == "") return;
                    $("#Sales option").remove();
                    var trhtml = '';
                    var tempColumn;
                    var tempRow = result.split("<?>");
                    for (i = 0; i < tempRow.length; i++) {
                        if (tempRow[i] != "") {
                            tempColumn = tempRow[i].split("<|>");
                            $("#txtSales").val(tempColumn[0]);
                            $("#Sales").append("<option value='" + tempColumn[0] + "'>" + tempColumn[1] + "</option>");
                        }
                    }
                }
                else
                    return;

            });

            $("#tchange").css({ "background-color": "#FFFFB9" });
            $("#SalesTeam").css({ "background-color": "#FFFFB9" });
            $("#Sales").css({ "background-color": "#FFFFB9" });
            $("#Sourcing").css({ "background-color": "#FFFFB9" });


        })


    </script>
</head>
<body>
    <form id="form1" runat="server" clientidmode="Static">
    <div id="center" style="border: 1px solid; padding: 2px 0 0px 2px; border-color: white;display:none;">
        <asp:Panel runat="server" ID="rpMenu" SkinID="RoundPanelNavigation" Width="100%">
            <asp:Panel ID="Panel2" runat="server">
                <div id="tabs" style="height: 9000px">
                </div>
            </asp:Panel>
        </asp:Panel>
    </div>
    <center>
        <table style="width: 881px">
            <tr>
                <td style="width: 150px">
                </td>
                <td style="width: 211px">
                </td>
                <td style="width: 130px">
                </td>
                <td style="width: 100px">
                </td>
                <td style="width: 100px">
                </td>
                <td style="width: 100px">
                </td>
                <td style="width: 90px">
                </td>
            </tr>
            <tr id="rowSearch" runat="server">
                <td align="left" colspan="3">
                    <table style="width:100%">
                        <tr style="width:100%">
                            <td  style="width:70px">
                                Quality Code 
                            </td>
                            <td style="width:181px">
                                <asp:TextBox runat="server" ID="tQC" Width="180px" ClientIDMode="Static"></asp:TextBox>
                                <asp:HiddenField runat="server" ID="HiddenCuId" ClientIDMode="Static" />
                                <asp:HiddenField runat="server" ID="HiddenQC" ClientIDMode="Static" />
                                <asp:HiddenField runat="server" ID="HiddenHasPPO" ClientIDMode="Static" />
                            </td>
                            <td align="left">
                                <asp:Button runat="server" ID="btnQcSearch" CssClass="myButton" ClientIDMode="Static"
                                    Text="Search" Height="20px" Width="100px" />
                            </td>
                            <td></td>
                        </tr>
                    </table>
                </td>
                <td align="left" colspan="4">
                    <table style="width: 100%">
                        <tr>
                            <td align="left" style="width:40px">
                                GkNo
                            </td>
                            <td align="left" style="width:141px">
                               <%-- <input type="text" style="width: 140px" id="txtGkNo" />--%>
                                <asp:TextBox runat="server" ID="txtGkNo" Width="180px" ClientIDMode="Static"></asp:TextBox>
                            </td>
                            <td align="left">
                                <input type="button" style="height: 20px" class="myButton" value="GetAttribute" onclick="FindAttr()" />
                            </td>
                            <td></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="7">
                    <div style="height: 3px">
                    </div>
                    <div style="width: 100%; border: 1px solid #FFA200;" id="headDiv">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 150px">
                                </td>
                                <td style="width: 211px">
                                </td>
                                <td style="width: 130px">
                                </td>
                                <td style="width: 100px">
                                </td>
                                <td style="width: 100px">
                                </td>
                                <td style="width: 100px">
                                </td>
                                <td style="width: 90px">
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:HiddenField  runat="server" ID="customerHidden" ClientIDMode="Static"/>
                                    <asp:Label runat="server" ID="lab1" Text="Customer"></asp:Label>
                                </td>
                                <td colspan="2" align="left">
                                    <input type="text" id="tchange" style="width: 348px; margin: 0 0 0 2px;" runat="server"
                                        clientidmode="Static" />
                                    <div style="width: 370px; height: 200px; overflow-y: auto; overflow-x: hidden; display: none;
                                        z-index: 10000; background-color: #8BB6EF; border: 2px solid #004080" id="seDiv">
                                        <table style="width: 100%" id="selectTable">
                                        </table>
                                    </div>
                                    <asp:TextBox ID="customer" runat="server" ClientIDMode="Static"></asp:TextBox>
                                </td>
                                <td align="right">
                                    <asp:Label runat="server" ID="lab2" Text="Sales Team"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList runat="server" ID="SalesTeam" ClientIDMode="Static" Width="98px"
                                        DataSourceID="departmentSQLDS" DataValueField="DEPARTMENT_ID" DataTextField="DEPARTMENT_ID">
                                    </asp:DropDownList>
                                </td>
                                <td align="right">
                                    <asp:Label runat="server" ID="Label1" Text="Sales"></asp:Label>
                                </td>
                                <td align="left">
                                    <select id="Sales" runat="server" style="width: 130px">
                                    </select>
                                    <asp:TextBox runat="server" ID="txtSales" ClientIDMode="Static"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label runat="server" ID="Label2" Text="Customer Quality ID" Width="149px"></asp:Label>
                                </td>
                                <td colspan="6" valign="top" align="left">
                                    <table style="width: 100%; margin: 0; padding: 0">
                                        <tr>
                                            <td align="left">
                                                <asp:TextBox runat="server" ID="CustomerQC" Width="110px" ClientIDMode="Static"></asp:TextBox>
                                            </td>
                                            <td>
                                                <table>
                                                    <tr>
                                                        <td align="left" style="width: 72px">
                                                            <asp:Label runat="server" ID="Label3" Text="AnalysisNo" Width="72px"></asp:Label>
                                                        </td>
                                                        <td align="left" style="width: 155px">
                                                            <asp:TextBox runat="server" ID="AnalysisNo" Width="155px" ClientIDMode="Static"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td align="right" style="width: 70px">
                                                <asp:Label ID="Label6" runat="server" Text="Sourcing"></asp:Label>
                                            </td>
                                            <td align="left" style="width: 100px">
                                                <asp:DropDownList runat="server" ID="Sourcing" Width="99px" ClientIDMode="Static">
                                                    <asp:ListItem Value="Internal" Text="Internal">
                    
                                                    </asp:ListItem>
                                                    <asp:ListItem Value="External" Text="External">
                    
                                                    </asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td colspan="1" align="right" style="width: 71px">
                                                <asp:Label ID="lab21" runat="server" Text="Brand"></asp:Label>
                                            </td>
                                            <td colspan="1" align="left">
                                                <div style="margin: 0 0 0 1px">
                                                    <asp:TextBox runat="server" ID="brand" Width="123px" ClientIDMode="Static"></asp:TextBox></div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td align="left" colspan="3">
                    <asp:Label runat="server" ID="Label4" Text="Quality Attributes"></asp:Label>
                </td>
                <td colspan="4" align="left">
                    &nbsp;&nbsp;<asp:Label runat="server" ID="lab56" Text="GEK Comments"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="3" valign="top">
                    <div style="height: 2px">
                    </div>
                    <asp:Panel runat="server" ID="attrPanel" ClientIDMode="Static">
                        <%-- <ASPx:Attribute runat="server" ID="attribute" />--%>
                    </asp:Panel>
                </td>
                <td colspan="4" valign="top">
                    <table style="width: 100%">
                        <tr>
                            <td align="left">
                                <asp:TextBox TextMode="MultiLine" runat="server" ID="GEKComments" Width="404px" Height="143px"
                                    ClientIDMode="Static" Style="border: 1px solid #FFA200;"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="left">
                                <asp:Label runat="server" ID="Label5" Text="Available Width"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Panel runat="server" ID="avaWidthPanel" ClientIDMode="Static">
                                </asp:Panel>
                                <asp:Panel runat="server" ID="avaRemark" ClientIDMode="Static" >
                                 <table style="width:100%">
                                   <tr>
                                     <td align="left">Available Width Remark</td>
                                   </tr>
                                   <tr>
                                     <td>
                                     <asp:TextBox runat="server" ID="txtAvaRemark" ClientIDMode="Static" Style="border: 1px solid #FFA200;" TextMode="MultiLine" Width="99%" Height="60px"></asp:TextBox>
                                     </td>
                                   </tr>
                                 </table>
                                   
                                   
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <br />
        <div id="divButton" runat="server">
            <table>
                <tr>
                    <td>
                        <input type="reset" id="reset" value="Reset" style="height: 30px; width: 100px" class="myButton" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
                    <td>
                        <asp:Button ID="btnCopyFrom" Height="30" runat="server" Text="Copy From" Width="100"
                            CssClass="myButton" ClientIDMode="Static" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
                    <td>
                        <asp:Button ID="create" Height="30" runat="server" Text="Create" OnClick="create_Click"
                            CssClass="myButton" Width="100" ClientIDMode="Static" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
                    <td>
                        <asp:Button ID="btnEditQC" Height="30" runat="server" Text="Edit" Width="100" ClientIDMode="Static"
                            CssClass="myButton" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
                    <td>
                        <asp:Button ID="btnSaveQC" Height="30" runat="server" Text="Save" Width="100" ClientIDMode="Static"
                            CssClass="myButton" OnClick="btnSaveQC_Click" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
                    <td>
                        <asp:Button runat="server" ID="cancel" ClientIDMode="Static" Width="100" CssClass="myButton"
                            Height="30" Text="Delete" OnClick="cancel_Click" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
                    <td>
                        <asp:Button ID="btnExit" Text="Exit" runat="server" ClientIDMode="Static" Height="30"
                            CssClass="myButton" Width="100" />
                        <!--<input type="button" id="exit" value="Exit" style="height:30px;width:100px" />-->
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
                    <td>
                        <input type="button" id="btnGoToApprove"   runat="server" value="Go To Approve" style="width:120px;height:30px;" class="myButton" onclick="Open();" />
<%--                        <asp:Button ID="btnGoToApprove" Height="30" runat="server" Text="Go To Approve" 
                            Width="120" ClientIDMode="Static"
                            CssClass="myButton" OnClientClick="Open();" />--%>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
                </tr>
            </table>
        </div>
        <div style="width: 200px; height: 90px; position: absolute; background-color: #58BDF2;
            z-index: 10000; display: none;" id="sucessDiv">
            <center>
                <div style="font-size: larger">
                    Successfully saved</div>
                <br />
                <div id="afterQC" style="font-size: larger">
                </div>
                <br />
                <div>
                    <input type="button" id="btnSure" value="Close" onclick="closeSD() " /></div>
            </center>
        </div>
        <div style="width: 300px; height: 90px; position: absolute; background-color: #58BDF2;
            border: 1px solid #FFA200; z-index: 10000; display: none;" id="divCopyFrom">
            <center>
                <div style="height: 25px;">
                </div>
                <div style="font-size: larger">
                    Quality Code<input type="text" id="txtCopyQC" /></div>
                <br />
                <div>
                    <input type="button" id="btnFinishCopy" value="OK" onclick="finishCopy()" style="width: 50px;" />
                    <input type="button" id="btnCancelCoppy" value="Cancel" onclick="cancelCopy()" style="width: 50px;" />
                </div>
            </center>
        </div>

         <div style="width: 500px; height: 320px; position: absolute; background-color: #58BDF2; overflow:auto;
            border: 1px solid #FFA200; z-index: 10000; display: none;" id="sameDiv">
            <center>
                <div style="height: 25px;font-size: larger">
                  Below quality code have the same attributes(click them show the details).
                </div>
                <div id="sameQCDiv">
                   </div>
                   &nbsp;&nbsp;&nbsp;&nbsp;
                <div id="customerDiv">
                   </div>
                <br />
                <div>
                     <asp:Button ID="ccButton" Height="30" runat="server" Text="Continue Create" OnClick="create_Click"
                            CssClass="myButton" Width="120" ClientIDMode="Static" />
                    <input type="button" id="Button2" value="Cancel" onclick="cancelCreate()" class="myButton" style="width: 50px;height:30px" />
                </div>
            </center>
        </div>

        <div style="width: 392px; height: 390px; position: absolute; background-color: White;
            border: 1px solid #FFA200; z-index: 10000; display: none;" id="gridDiv">
            <div style="background-color: #58BDF2; font-size: large;">
                <font color="red">Below PPO has used this Quality Code. Any revision of this Quality
                    code will be brought to below PPO as well. Do you still want to modify it?</font>
            </div>
            <div style="height: 4px">
            </div>
            <div style="text-align: left">
                Edit reason
            </div>
            <div style="text-align: left">
                <asp:TextBox type="text" runat="server" ID="editReason" ClientIDMode="Static" TextMode="MultiLine"
                    Width="380px" Height="40px"></asp:TextBox>
            </div>
            <div>
                <input type="button" id="btnGridOK" value="OK" style="width: 70px; height: 30px" />
                <input type="button" id="btnGridCancel" value="Cancel" style="width: 70px; height: 30px" />
            </div>
            <div style="height: 12px">
            </div>
            <Aspx:WebGridView ID="ppoGridCreate" runat="server" DataSourceId="ppoOBDSCreate"
                PageSize="10" Width="390" Height="160" CreateSearchPanel="false" CreateAddPanel="false"
                KeyFieldName="PPONO">
                <Aspx:Field FieldName="PPONO" Caption="PPONO">
                </Aspx:Field>
                <Aspx:Field FieldName="CustomerId" Caption="CustomerId" Width="80">
                </Aspx:Field>
                <Aspx:Field FieldName="userId" Caption="UserId" Width="80">
                </Aspx:Field>
                <Aspx:Field FieldName="userGroup" Caption="User Group" Width="85">
                </Aspx:Field>
            </Aspx:WebGridView>
        </div>
        <%--        <div>
            <ASPx:FlatAttribute runat="server" ID="flatYarnInfo"></ASPx:FlatAttribute>
        </div>
        <div>
        <ASPx:TapAttribute runat="server" ID="FlatAttribute1"></ASPx:TapAttribute>
        </div>--%>
    </center>
    <%--    <asp:SqlDataSource ID="finishSQLDS" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
        ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="SELECT Finishing_Code, Description||'('||Finishing_Code||')' as Description FROM pbKnitFinish where Washing_Flag='N' and IS_Active='Y' order by Description asc">
    </asp:SqlDataSource>--%>
    <asp:ObjectDataSource ID="ppoOBDSCreate" runat="server" SelectMethod="GetFabricCodeListOne"
        TypeName="Comfy.App.Core.QualityCode.CustomerManager" DataObjectTypeName="Comfy.App.Core.QualityCode.FabricCodeModel">
        <SelectParameters>
            <asp:Parameter Name="QC" Type="string" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:SqlDataSource ID="customerSQLDS" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
        ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="select CUSTOMER_CD,NAME from gen_customer order by NAME asc">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="departmentSQLDS" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
        ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="select distinct(DEPARTMENT_ID) from GEN_USERS where Active='Y' order by DEPARTMENT_ID asc">
    </asp:SqlDataSource>
    <asp:ObjectDataSource ID="YarnInfoODS" runat="server" SelectMethod="GetYarnInfo"
        InsertMethod="AddYarnInfo" UpdateMethod="EditYarnInfo" DeleteMethod="DeleteYarnInfo"
        TypeName="Comfy.App.Web.QuailtyCode.CreateQC" DataObjectTypeName="Comfy.App.Core.QualityCode.YarnInfo">
        <SelectParameters>
            <asp:Parameter Name="orderByField" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    </form>
</body>
<script type="text/javascript">
    var gridCreate = ppoGridCreate;
    var combo = new Combo("tchange", "seDiv", "selectTable", "370px", "200px", "Comfy.App.Core", "Comfy.App.Core.QualityCode.CustomerManager", "GetCustomerInfo", "combo");
    function comboClick(text, val) {
        $("#customer").val(val);
    }

    function cancelCopy() {
        $("#divCopyFrom").hide();
    }
    function cancelCreate() {
        $("#sameDiv").hide();
    }
    function finishCopy() {
        $("#divCopyFrom").hide();
        if (!SearchQC($("#txtCopyQC").val(), false)) {
            return;
        }
        $("#CustomerQC").val("");
        $("#AnalysisNo").val("");
        //$("#txtGkNo").val("");
        copyShroud();
    }

    function copyShroud() {
        $("#btnEditQC").attr("disabled", "disabled");
        $("#btnSaveQC").attr("disabled", "disabled");
        $("#cancel").attr("disabled", "disabled");

    }

    function userCanEdit() {
      //  var result = AjaxCallFun("Comfy.App.Core", "Comfy.App.Core.QualityCode.CustomerManager", "UserCanEdit", $("#SalesTeam").val());
     //   if (result == "1") {
      ////    //  return true;
      //  }
     //   alert("You have no power to edit this quality code!");
        //  return false;
        return true;
    }

    function hasPPO() {
        var result = AjaxCallFun("Comfy.App.Core", "Comfy.App.Core.QualityCode.CustomerManager", "HasPPO", $("#tQC").val());
        if (result == "1") {
            return true;
        }
        return false;
    }

    $(function () {
        $("#btnGridOK").click(function () {
            if ($("#editReason").val().replace(/\s+/g, "") == "") {
                alert("Plase write the edit reason!");
                return;
            }
            $(".divShroud").remove();
            $("#btnEditQC").attr("disabled", "disabled");
            $("#btnSaveQC").attr("disabled", "");
            $("#cancel").attr("disabled", "");
            $("#gridDiv").hide();
            editDisabled();
            $("#HiddenHasPPO").val("1"); //记录是否有开过订单，1代表有，空代表无
        });
        $("#btnGridCancel").click(function () {
            $(".divShroud").remove();
            shroudDiv($("#headDiv"));
            shroudDiv($("#attrPanel"));
            shroudDiv($("#GEKComments"));
            shroudDiv($("#avaWidthPanel"));
            $("#gridDiv").hide();
        });
        $("#btnEditQC").click(function () {
            var bFlag = false;
            if (userCanEdit()) {
                bFlag = hasPPO();
                if (bFlag) {
                    ParamsCreate.QC = $("#tQC").val();
                    gridCreate.GridCheck(ParamsCreate);
                    $("#gridDiv").css({
                        "top": Math.max(($(window).height() / 2 - $("#gridDiv").outerHeight() / 2 + $(window).scrollTop()), 0) + "px",
                        "left": Math.max(($(document.body).width() / 2 - $("#gridDiv").outerWidth() / 2 + $(window).scrollLeft()), 0) + "px",
                        "display": "block"
                    });
                }
            } else {
                return false;
            }
            if (!bFlag) {
                $(".divShroud").remove();
                $("#btnEditQC").attr("disabled", "disabled");
                $("#btnSaveQC").attr("disabled", "");
                $("#cancel").attr("disabled", "");
                editDisabled();
            } else {
                $(".divShroud").remove();
                shroudDiv($("#form1"));
            }
            return false;
        });

        $("#btnCopyFrom").click(function () {
            $("#divCopyFrom").css({
                "top": Math.max(($(window).height() / 2 - $("#divCopyFrom").outerHeight() / 2 + $(window).scrollTop()), 0) + "px",
                "left": Math.max(($(document.body).width() / 2 - $("#divCopyFrom").outerWidth() / 2 + $(window).scrollLeft()), 0) + "px",
                "display": "block"
            });
            return false;
        });
    })

    function FindAttr() {

        if ($("#txtGkNo").val() != "") {
            //Add by sunny  2017 07-11
            var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.CreateQC", "InitInfoGkNo", $("#txtGkNo").val());
            if (result.indexOf("error") >= 0) {
                alert(result);
                return;
            }
            if (result == "0") {
                alert("The GkNo  does not exist!");
                return;
            }
            else
            {
                var txtGkNo = $("#txtGkNo").val();
                if (GMType == "Fabric") {
                    if (txtGkNo.substring(0, 1) == "B" || (txtGkNo.substring(0, 1) == "S" && txtGkNo.substring(4, 5) == "B")) {
                    }
                    else {
                        alert("GK_NO与当前类别【'" + GMType + "'】不一致请核查");
                        return;
                    }
                }

                if (GMType == "FlatKnit") {
                    if (txtGkNo.substring(0, 1) == "C" || (txtGkNo.substring(0, 1) == "S" && txtGkNo.substring(4, 5) == "C")) {

                    }
                    else {
                        alert("GK_NO与当前类别【'" + GMType + "'】不一致请核查");
                        return;
                    }
                }
                if (GMType == "Tapping") {
                    if (txtGkNo.substring(0, 1) == "T") {
                    }
                    else {
                        alert("GK_NO与当前类别【'" + GMType + "'】不一致请核查");
                        return;
                    }
                }
            }

            if (result.split("<|>").length > 17) {
                $("#GEKComments").val(result.split("<|>")[17]);
            }
            if (GMType == "Fabric") {
                cInit(result,'');
            } else if (GMType == "FlatKnit") {
                fInit(result,'');
            } else if (GMType == "Tapping") {
                tInit(result,'');
            }
        }
    }

    function editDisabled() {

      //  $("#tchange").attr("disabled", "disabled");
      //  $("#SalesTeam").attr("disabled", "disabled");
      //  $("#Sales").attr("disabled", "disabled");
        $("#Sourcing").attr("disabled", "disabled");
    }

    $(function () {

        $("#btnSaveQC").click(function () {

            var ratioresult = "";
            ratioresult = AjaxCallFunOne("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.CreateQC", "EditVerificationRatio", "");
            if (ratioresult == "falseone") {
                alert("纱类Radio不可以为空");
                return false;
            }
            if (ratioresult == "falsetwo") {
                alert("纱类Radio加起来必须等于100");
                return false;
            }




            shroudDiv($("#btnSaveQC"));
            return true;
        });

        $("#ccButton").click(function () {

            shroudDiv1($("#ccButton"));
            return true;
        });
        $(window).bind('resize', function () {
            if (shroudFlag == "1") {
                $(".divShroud").remove();
                shroudDiv($("#headDiv"));
                shroudDiv($("#attrPanel"));
                shroudDiv($("#GEKComments"));
                shroudDiv($("#avaWidthPanel"));
            }
            ;
        });

        $("#btnExit").click(function () {
            try {
                window.parent.closeCreate();

            } catch (e) {
                window.opener = null;
                window.open('', '_self');
                window.close();
            }
        });
        $("#cancel").click(function () {
            if (!confirm("Confirm to delete this QualtyCode?")) {
                return false;
            }
            return true;
        });
        $("#reset").click(function () {
            $("#SalesTeam").val(TempBu);
            $("#Sales option").remove();
            $("#Sales").prepend(TempYuan);

        });



        $("#AnalysisNo").change(function (event) {
            if ($("#AnalysisNo").val() != "") {
                var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.CreateQC", "InitInfoAN", $("#AnalysisNo").val());
                if (result.indexOf("error") >= 0) {
                    alert(result);
                    return;
                }
                if (result == "0") {
                    alert("The analysis no does not exist!");
                    return;
                }
                if (GMType == "Fabric") {
                    cInit(result,'');
                } else if (GMType == "FlatKnit") {
                    fInit(result,'');
                } else if (GMType == "Tapping") {
                    tInit(result,'');
                }
            }
        });
    })

    function Open() {
        //window.opener.open(a);
        if ($.trim($("#tQC").val()) == '') {
            alert("请先创建新的Quality Code！");
        }
        else {
            parent.qcValue = $("#tQC").val();
            window.parent.document.getElementById("btnApprove").click();
        }

}
</script>
</html>
