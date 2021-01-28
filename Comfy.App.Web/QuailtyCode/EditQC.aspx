<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditQC.aspx.cs" Inherits="Comfy.App.Web.QuailtyCode.EditQC" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Src="Attribute.ascx" TagName="Attribute" TagPrefix="ASPx" %>
<%@ Register Src="AvaWidth.ascx" TagName="Ava" TagPrefix="ASPx" %>
<%@ Register Src="CustomerEditForm.ascx" TagName="CE" TagPrefix="ASPx" %>
<%@ Register TagPrefix="Aspx" Namespace="Comfy.UI.WebControls.WebGridView" Assembly="Comfy.UI.WebControls" %>
<%@ Register TagPrefix="Aspx" Namespace="Comfy.UI.WebControls.WebPopupControl" Assembly="Comfy.UI.WebControls" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../Scripts/jquery1.4.js" type="text/javascript"></script>
    <script type="text/javascript" src="../Scripts/Utilities.js"></script>
    <script src="../Scripts/Combo.js" type="text/javascript"></script>
    <script src="../Scripts/ccftag.js" type="text/javascript"></script>
        <style>
            .myButton
        {
             border:1px solid #FFA200;
            }
    </style>
    <script>
        var GMType;
        var params = {};
        var searchStr = {};
        var gekComment;
        var action;

        function UpdateSucess() {
            alert("Successfully updated！");
        }
        // 添加对Maintain功能页搜索Quality Code显示FabricModel结果
        // by LYH
        function SearchQC() {
            var tempQC = $("#tQC").val();
            $("#form1").get(0).reset();
            $("#tQC").val(tempQC);
            $("#tQCTemp").val(tempQC);
            $("#customerIdDiv").css("display", "inline");

            if ($("#tQC").val() != "") {
                var strTemp;
                
                var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.EditQC", "InitInfo", $("#tQC").val()); //如果只有状态为Approved的才能查询出来，那么这里的查询语句要加多一个判断。

                //if (result.split("<|>|")[1]!=GMType)
                var tgmt = result.split("<|>")[17];

                if (tgmt == "Flat Knit Fabric") {
                    tgmt = "FlatKnit";
                }

                if (tgmt != GMType) {
                    alert("Please change MaterialGroup.The MaterialGroup of this QualityCode is:" + tgmt);
                    return;
                }

                if (result != "") {
                    if (GMType == "Fabric") {
                        InitTable(result.split("<|>")[10]); // Quality Width
                        cInit(result,'');                       // Quality Attributes
                        $("#txtAvaRemark").val(result.split("<|>")[18]);
                    }
                    else if (GMType == "FlatKnit") {
                        fInit(result,'');
                    } else if (GMType == "Tapping") { 
                        tInit(result,'');
                    }
                    //add by zheng zhou 2016-8-3
                    var resultArray = result.split("<|>");
                    var len = resultArray.length;
                    $("#hd_HF_Ref_GP_Old").val(resultArray[len-2]);
                    $("#hd_HF_Ref_PPO_Old").val(resultArray[len - 3]);
                    $("#hd_QC_Ref_GP_Old").val(resultArray[len - 4]);
                    $("#hd_QC_Ref_PPO_Old").val(resultArray[len - 5]);
                    $("#remark_Old").val(resultArray[len - 1]);
                    ////////////////////////////
                }
               
                params.QC = $("#tQC").val();
                setTimeout("editPPOGrid.GridCheck(params)", 1000); //如果同时执行两个Grid的刷新，必须要用SetTimeOut函数将两个Grid刷新的时间错开

                gekResult = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "GetGekCommentAndCustomerQualityId_ByCode", tempQC);
                var gekResultArray = gekResult.split('$');

                $("#GekComments").val(gekResultArray[0]);
                $("#tCustomerQualityId").val(gekResultArray[1]);

                gekComment = gekResultArray[0];
            }
        }
        //add by zheng zhou  save成功后字体变大要刷新一次恢复正常字体
        function SearchQC1() {
            alert("Success！");
            var tempQC = $("#tQC").val();
            $("#form1").get(0).reset();
            $("#tQC").val(tempQC);
            $("#tQCTemp").val(tempQC);
            $("#customerIdDiv").css("display", "inline");

            if ($("#tQC").val() != "") {
                var strTemp;

                var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.EditQC", "InitInfo", $("#tQC").val()); //如果只有状态为Approved的才能查询出来，那么这里的查询语句要加多一个判断。

                //if (result.split("<|>|")[1]!=GMType)
                var tgmt = result.split("<|>")[17];

                if (tgmt == "Flat Knit Fabric") {
                    tgmt = "FlatKnit";
                }

                if (tgmt != GMType) {
                    alert("Please change MaterialGroup.The MaterialGroup of this QualityCode is:" + tgmt);
                    return;
                }

                if (result != "") {
                    if (GMType == "Fabric") {
                        InitTable(result.split("<|>")[10]); // Quality Width
                        cInit(result, '');                       // Quality Attributes
                        $("#txtAvaRemark").val(result.split("<|>")[18]);
                    }
                    else if (GMType == "FlatKnit") {
                        fInit(result, '');
                    } else if (GMType == "Tapping") {
                        tInit(result, '');
                    }
                    //add by zheng zhou 2016-8-3
                    var resultArray = result.split("<|>");
                    var len = resultArray.length;
                    $("#hd_HF_Ref_GP_Old").val(resultArray[len - 2]);
                    $("#hd_HF_Ref_PPO_Old").val(resultArray[len - 3]);
                    $("#hd_QC_Ref_GP_Old").val(resultArray[len - 4]);
                    $("#hd_QC_Ref_PPO_Old").val(resultArray[len - 5]);
                    $("#remark_Old").val(resultArray[len - 1]);
                    ////////////////////////////
                }

                params.QC = $("#tQC").val();
                setTimeout("editPPOGrid.GridCheck(params)", 1000); //如果同时执行两个Grid的刷新，必须要用SetTimeOut函数将两个Grid刷新的时间错开

                gekResult = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "GetGekCommentAndCustomerQualityId_ByCode", tempQC);
                var gekResultArray = gekResult.split('$');

                $("#GekComments").val(gekResultArray[0]);
                $("#tCustomerQualityId").val(gekResultArray[1]);

                gekComment = gekResultArray[0];
            }
        }
        function InitQC() {
            editPPOGrid.GetRowValues('QualityCode;CustomerComment;CustomerId', 'InitQCForm');
        }
        function InitQCForm(val) {

            if (val[0] != "") {
                $("#tQC").val(val[0]);
                $("#tQCTemp").val(val[0]);
                $("#tCustomerId").val(val[2]);
                $("#customerIdDiv").attr("style", "display:inline");
                var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.EditQC", "InitInfo", val[0]);
                var strTemp;
                $("#SalesComments").val(val[1]);
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

//                var tempStr = val[0] + "," + val[2];

              
//                gekComment = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "GetGekComment", tempStr);
                //                $("#GekComments").val(gekComment);

                gekResult = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "GetGekCommentAndCustomerQualityId_ByCode", val[0]);
                var gekResultArray = gekResult.split('$');

                $("#GekComments").val(gekResultArray[0]);
                $("#tCustomerQualityId").val(gekResultArray[1]);

                gekComment = gekResultArray[0];
            }
        }

        function hasPPO() {
            var result = AjaxCallFun("Comfy.App.Core", "Comfy.App.Core.QualityCode.CustomerManager", "HasPPO", $("#tQC").val());
            if (result == "1") {
                return true;
            }
            return false;
        }

        function setGMType(val, QC, cuId) {
            GMType = val;
         //   alert(QC+'  '+cuId);
            $(function () {
                if (GMType == "Fabric") {
                    //  $("#avaDiv").css({ "width": "350px" });
                }
               // SearchQC();
            });
            if (QC != "") {
                // alert(QC);
                $("#tQC").val(QC);
                /*  var param = new Array();
                param.push(QC);
                param.push("");
                param.push(cuId);
                */
                $(function () {
                    $("#tQC").val(QC);
                    setTimeout("SearchQC()", 700);
                    //  InitQCForm(param);
                });
            }
        }

//        function setGMType(val, QC, cuId) {
//            GMType = val;

//            if (QC != "") {
//                $("#tQC").val(QC);
//                //SearchQC();
//                location.reload();
//                setTimeout("SearchQC()", 700);
//            }
//        }

        $(function () {
            $("#tCustomerId").attr("style", "display:none");
            $("#customerIdDiv").attr("style", "display:inline");
            $("#tQCTemp").attr("style", "display:none");
            $("#commentsDiv").css("display", "none");

            $("#btnAssignTo").click(function () {
                $("#assignCustomerId").val("");
                $("#assignChange").val("");
                $("#assginGekComment").val("");
                popu.Show();
                $("#assignQC").val($("#tQC").val());
                return false;
            });
            $("#GekComments").click(function () {
                if (typeof (gekComment) == "undefined")
                    return;

                $("#txtComments").val("");

                $("#commentsDiv").css({ "width": "400px",
                    "height": "80px",
                    "position": "absolute",
                    "z-index": "10001",
                    "left": "300px",
                    "top": "300px",
                    "display": "block"
                });
                //var param = $("#tQC").val();

                //AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.EditQC", "GetGekCommentDetail", param);

            });
            $("#SaveComments").click(function () {
                //alert($("#txtComments").val());
                if ($.trim($("#txtComments").val()) == "") {
                    alert("Please input Comment!");
                    return;
                }
                else {
                    var d = new Date();
                    var vYear = d.getFullYear();
                    var vMon = d.getMonth() + 1;
                    var vDay = d.getDate();
                    var h = d.getHours();
                    var m = d.getMinutes();
                    var se = d.getSeconds();
                    s = vYear + "-" + (vMon < 10 ? "0" + vMon : vMon) + "-" + (vDay < 10 ? "0" + vDay : vDay) + " " + (h < 10 ? "0" + h : h) + ":" + (m < 10 ? "0" + m : m) + ":" + (se < 10 ? "0" + se : se);

                    var userid = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.EditQC", "GetUserID", "1");
                    var inputStr = s + "    " + userid + "    " + $.trim($("#txtComments").val()) + "\n";

                    //判断GEKComments文本框是否有输入新的数据，如果有的话，点击则进行修改，否则进行添加。
                    //                    alert(gekComment.length);
                    //                    alert($("#GekComments").val().length);
                    //                    if (gekComment == $("#GekComments").val()) {
                    //                        action = "add";
                    //                    }
                    //                    else {
                    //                        action = "modify";
                    //                    }

                    //                    if (action == "add") {
                    if ((gekComment.replace(/\n/g, "").replace(/\r/g, "") == $("#GekComments").val().replace(/\n/g, "").replace(/\r/g, ""))) {
                        if ($("#GekComments").val().substring($("#GekComments").val().length - 1, 1) == "\n") {
                            $("#GekComments").val($("#GekComments").val() + inputStr);
                        }
                        else {
                            $("#GekComments").val($("#GekComments").val() + "\n" + inputStr);
                        }

                    }
                    else {
                        var commentsArray = $("#GekComments").val().split("\n");
                        var newComments = "";
                        for (var i = 0; i < commentsArray.length - 2; i++) {
                            newComments = newComments + commentsArray[i].toString() + "\n";
                        }
                        $("#GekComments").val(newComments + inputStr);
                    }

                    $("#commentsDiv").css("display", "none");
                }
            });
            $("#ExitComments").click(function () {
                $("#commentsDiv").css("display", "none");
            });

            //add by zheng zhou
            if ($.trim($("#tQC").val()) != '') {
                SearchQC();
            }
            ///end by zheng zhou
        });

        function CheckCustomerQualityId() {
            if (isNaN($.trim($("#tCustomerId").val())))
            {
                alert("请输入有效CustomerQualityId");
                return false;
            }
                
            else
                return true;
        }
    </script>
    <style type="text/css">
        html
        {
            overflow-y:auto;
            overflow-x:hidden;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" >
    <center>
        <div style="width: 954PX">
            <table>
                <tr>
                    <td style="width: 460px" align="left">
                        Quality Code
                        <asp:TextBox runat="server" ID="tQC" Width="70px" ClientIDMode="Static"></asp:TextBox>
                        <input type="button" value="Search" onclick="SearchQC()" style="height:25px;width:100px" class="myButton"/>
                        <div id="customerIdDiv" style="vertical-align:bottom">
                          <span>Cus. Quality ID</span>
                          <asp:TextBox runat="server" ID="tCustomerQualityId" ClientIDMode="Static" style="width:80px"></asp:TextBox>
                          <asp:TextBox runat="server" ID="tCustomerId" ClientIDMode="Static" style="width:100px"></asp:TextBox>
                          <asp:TextBox runat="server" ID="tQCTemp" ClientIDMode="Static" style="width:100px"></asp:TextBox>
                             </div>
                    </td>
                    <td style="width: 2px">
                    </td>
                    <td style="width: 490px">
                    </td>
                </tr>
                <tr>
                    <td align="left" valign="top">
                    <div style="height:13px"></div>
                        <div style="border:1px solid #FFA200;width:438px;">
                            <Aspx:WebGridView ID="editPPOGrid" runat="server" DataSourceId="ppoOBDS" PageSize="10"
                                Width="436" Height="271" CreateSearchPanel="false" CreateAddPanel="false" KeyFieldName="PPONO"
                                OnClick="InitQC()">
                                <Aspx:Field FieldName="PPONO" Caption="PPONO">
                                </Aspx:Field>
                                <Aspx:Field FieldName="QualityCode" Caption="QualityCode">
                                </Aspx:Field>
                                <Aspx:Field FieldName="FabricPart" Caption="FabricPart">
                                </Aspx:Field>
                                <Aspx:Field FieldName="CustomerId" Caption="CustomerId">
                                </Aspx:Field>
                                <Aspx:Field FieldName="Status" Caption="Status">
                                </Aspx:Field>
                                <Aspx:Field FieldName="ComboName" Caption="ComboName">
                                </Aspx:Field>
                                <Aspx:Field FieldName="CustomerComment" Caption="CustomerComment">
                                </Aspx:Field>
                                <Aspx:Field FieldName="ViewFlag" Caption="ViewFlag">
                                </Aspx:Field>
                            </Aspx:WebGridView>
                        </div>
                        <div>
                            Sales Comments
                        </div>
                        <div>
                            <asp:TextBox ID="SalesComments" ClientIDMode="Static" runat="server" TextMode="MultiLine" style="border:1px solid #FFA200;"
                                Width="433px" Height="45px"></asp:TextBox>
                        </div>
                        <div>
                            <asp:Label runat="server" ID="labelGekComment" Text="GEK Comments">

                            </asp:Label>
                        </div>

                        <div>
                            <asp:Panel runat="server" ID="gridCustomerPanle">
                            </asp:Panel>
                            <%-- <Aspx:CE runat="server" ID="CEGrid" />--%>
                        </div>

                        <div id="commentsDiv" style="text-align:center;display:none;background-color:White;border:1px solid #FFA200;">
                            <br />  
                            <span>Comment:</span><asp:TextBox ID="txtComments" Width="230px" runat="server"></asp:TextBox>
                            <br />
                            <br />
                            <input type="button" id="SaveComments" value="OK" class="myButton" />&nbsp;&nbsp;&nbsp;&nbsp;
                            <input type="button" id="ExitComments" value="Exit" class="myButton" />
                        </div>


                                 <asp:Panel runat="server" ID="avaRemark" ClientIDMode="Static" >
                                 <table style="width:100%">
                                   <tr>
                                     <td align="left">Available Width Remark</td>
                                   </tr>
                                   <tr>
                                     <td>
                                     <asp:TextBox runat="server" ID="txtAvaRemark" ClientIDMode="Static" Style="border: 1px solid #FFA200;" TextMode="MultiLine" Width="96%" Height="60px"></asp:TextBox>
                                     </td>
                                   </tr>
                                 </table>
                                   
                                   
                                </asp:Panel>

                    </td>
                    <td style="width: 2px">
                    </td>
                    <td align="left" valign="top">
                            Quality Attributes
                            <asp:Panel runat="server" ID="CFTAttribute" HorizontalAlign="Left">
                                <%--<ASPx:Attribute runat="server" ID="attribute" />--%>
                                <asp:HiddenField ID="hd_QC_Ref_PPO_Old" runat="server"/>
                                <asp:HiddenField ID="hd_QC_Ref_GP_Old" runat="server"/>
                                <asp:HiddenField ID="hd_HF_Ref_PPO_Old" runat="server"/>
                                <asp:HiddenField ID="hd_HF_Ref_GP_Old" runat="server"/>
                                <asp:HiddenField ID="remark_Old" runat="server"/>
                            </asp:Panel>
                            <asp:Label runat="server" ID="AWPanel" Text="Available Width">
                            
                            </asp:Label>
                            <asp:Panel runat="server" ID="AvaPanel">
                                <%-- <ASPx:Ava ID="AvaWidth" runat="server"></ASPx:Ava>--%>
                            </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" align="center">
                    <div style="height:2px"></div>
                        <input id="HidArributeValue" type="hidden"  runat="server"/>

                        <asp:Button  CssClass="myButton" ID="btnSave" Text="Save" runat="server" OnClick="btnSave_Click" Height="30" Width="100" />
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button  CssClass="myButton" ID="btnSaveAs" Text="Save As" runat="server" 
                            ClientIDMode="Static"  Height="30" Width="100"  />&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button  CssClass="myButton" ID="btnAssignTo" Text="Assign To" runat="server" ClientIDMode="Static"  Height="30" Width="100"  />&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button  CssClass="myButton" ID="btnDisable" Text="Disable" runat="server" ClientIDMode="Static"  Height="30" Width="100" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button  CssClass="myButton" ID="btnExit" Text="Exit" runat="server" ClientIDMode="Static"  Height="30" Width="100"  />&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button  CssClass="myButton" ID="btnTransferToAX" Text="Import to AX" runat="server" ClientIDMode="Static"  Height="30" Width="100"  />
                    </td>
                </tr>
            </table>
        </div>
    </center>
    <Aspx:PopupControl runat="server" ID="popu" Width="500" Height="306" Display="false"
        HeadText="Assign To">
        <ContentCollection>
            <table>
                <tr>
                    <td align="right">
                        Customer
                    </td>
                    <td>
                        <input type="text" id="assignChange" style="width: 350px" />
                        <div style="width: 370px; height: 200px; overflow: auto; display: none; z-index: 1000000000;
                            background-color: #8BB6EF; border: 2px solid #004080" id="assignDiv">
                            <table style="width: 100%" id="assignTable">
                            </table>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        Customer Quality ID
                    </td>
                    <td>
                        <input type="text" id="assignQC" readonly="readonly" style="display: none" />
                        <input type="text" id="CQI" />
                        <input type="text" id="assignCustomerId" style="display: none" />
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        Sales team
                    </td>
                    <td colspan="1">
                        <table>
                            <tr>
                                <td>
                                    <asp:DropDownList runat="server" ID="SalesTeam" ClientIDMode="Static" Width="98px"
                                        DataSourceID="departmentSQLDS" DataValueField="DEPARTMENT_ID" DataTextField="DEPARTMENT_ID">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    Sales
                                    <select id="Sales" style="width: 130px">
                                    </select>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        Brand
                    </td>
                    <td colspan="1">
                        <table>
                            <tr>
                                <td>
                                    
                                     <input  ID="brand" style="Width:126px" type="text">
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        GEK Comments
                    </td>
                    <td>
                        <textarea id="assginGekComment" style="width: 350PX; height: 50px"></textarea>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <br />
                        <input type="button" value="Confirm" id="assignConfirm" />
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <input type="button" value="Cancel" id="assignCancel" />
                    </td>
                </tr>
            </table>
        </ContentCollection>
    </Aspx:PopupControl>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
        ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="select distinct(DEPARTMENT_ID) from GEN_USERS where Active='Y' order by DEPARTMENT_ID asc">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="customerSQLDS" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
        ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="select CUSTOMER_CD,NAME from gen_customer order by NAME asc">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="departmentSQLDS" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
        ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="select distinct(DEPARTMENT_ID) from GEN_USERS where Active='Y' order by DEPARTMENT_ID asc">
    </asp:SqlDataSource>
    <asp:ObjectDataSource ID="ppoOBDS" runat="server" SelectMethod="GetFabricCodeListOne"
        TypeName="Comfy.App.Core.QualityCode.CustomerManager" DataObjectTypeName="Comfy.App.Core.QualityCode.FabricCodeModel">
        <SelectParameters>
            <asp:Parameter Name="QC" Type="string" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <!--
    <asp:ObjectDataSource ID="YarnInfoODS" runat="server" SelectMethod="GetYarnInfo"
        InsertMethod="AddYarnInfo" UpdateMethod="EditYarnInfo" DeleteMethod="DeleteYarnInfo" TypeName="Comfy.App.Web.QuailtyCode.EditQC"
        DataObjectTypeName="Comfy.App.Core.QualityCode.YarnInfo">
        <SelectParameters>
            <asp:Parameter Name="orderByField" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    -->
    </form>
</body>
<script type="text/javascript">
    var grid = editPPOGrid;
    var j = 0

    $(function () {
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


        $("#SalesTeam").prepend("<option value=''></option>");
        $("#SalesTeam").val("");
        $("#assignConfirm").click(function () {
            if ($("#assignQC").val() == "") {
                alert("Please choose one quality code!");
                return false;
            }
            if ($("#assignCustomerId").val() == "") {
                alert("CustomerID can not be null!");
                return false;
            }
            if ($("#SalesTeam").val() == "") {
                alert("SalesTeam can not be null!");
                return false;
            }
            if ($("#Sales").val() == "") {
                alert("Sales can not be null!");
                return false;
            }
            var paramsb = $("#assignQC").val() + "(?$)" + $("#assignCustomerId").val() + "(?$)" + $("#assginGekComment").val() + "(?$)" + $("#CQI").val() + "(?$)" + $("#SalesTeam").val() + "(?$)" + $("#Sales").val() + "(?$)" + $("#Brand").val();
            var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.EditQC", "AssignQC", paramsb);
            if (result == "1") {
                alert("Successfully assigned!");
                popu.Hide();
            } else {
                alert("Fail" + result);
            }

        })

        $("#btnSave").click(function () {
            if ($("#tQC").val() == "") {
                alert("Please open one quality code!");
                return false;
            }
            else {

                if ($("#tQC").val() != $("#tQCTemp").val()) {
                    alert("Quality Code文本框和内容不一致，请先点击Search按钮");
                    return false;
                }
                if (GMType == "Fabric") {
                    var params = getFAttributeForUpdate()
                    var retStrs = params.split("<>");
                    if (retStrs[10] != '' && (retStrs[11] == '' || retStrs[11] == 'null')) {
                        alert("请选择QC_Ref_GP");
                        return false;
                    }
                    if (retStrs[12] != '' && (retStrs[13] == '' || retStrs[13] == 'null')) {
                        alert("请选择HF_Ref_GP");
                        return false;
                    }
                    $("#HidArributeValue").val(params);

                }
                return true;
            }
        });

        $("#btnSaveAs").click(function () {
            if ($("#tQC").val() == "") {
                alert("Please open one quality code!");
                return false;
            }
            //            if (GMType == "Fabric") {
            window.parent.saveAs("QuailtyCode/CreateQC.aspx?MG=" + GMType + "&QC=" + $("#tQC").val() + "&customerId=" + $("#tCustomerId").val());
            // openZDailog("CreateQC", "Create Quailty Code", "QuailtyCode/CreateQC.aspx?MG=" + GMType + "&QC=" + $("#tQC").val() + "&customerId=" + $("#tCustomerId").val(), "900", "550", "");
            //            } else {
            //                window.parent.saveAs("QuailtyCode/CreateQC.aspx?MG=" + GMType + "&QC=" + $("#tQC").val() + "&customerId=" + $("#tCustomerId").val());
            //              //  openZDailog("CreateQC", "Create Quailty Code", "QuailtyCode/CreateQC.aspx?MG=" + GMType + "&QC=" + $("#tQC").val() + "&customerId=" + $("#tCustomerId").val(), "900", "380", "");
            //            }

            return false;
        });

        $("#assignCancel").click(function () {
            popu.Hide();
        });

        $("#SalesComments").attr("readOnly", true);
        $("#SalesComments").css({ "background-color": "#EFEFEF" });
        $("#GekComments").css({ "background-color": "#EFEFEF" });
        if (GMType == "Fabric") {
            disableAttribute();
        } else if (GMType == "FlatKnit") {
            $("#trHC").hide();
            disableFlatAttribute();
        } else if (GMType == "Tapping") {
            disableTapAttribute();
        }

    });
    var combo = new Combo("tchange", "seDiv", "selectTable", "370px", "200px", "Comfy.App.Core", "Comfy.App.Core.QualityCode.CustomerManager", "GetCustomerInfo", "combo");
    function comboClick(text, val) {
        $("#customer").val(val);
    }

    $(function () {
        $("#btnExit").click(function () {
            try {
                window.parent.closeEdit();

                //  Dialog.getInstance("EditQC").close();
            } catch (e) {
                window.opener = null;
                window.open('', '_self');
                window.close();
                //  Dialog.getInstance("EditQC").close();
            }
            return false;
        });

        $("#btnDisable").click(function () {
            if (!confirm("Confirm to disable this QualtyCode?")) {
                return false;
            }
            if (hasPPO()) {
                alert("The quality code haved used in some PPO,can not Disabled");
                return false;
            }
            var tempQC = $("#tQC").val();
            if (tempQC != "") {
                var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.EditQC", "DisableQC", tempQC);
                if (result == "1") {
                    alert("Success!");
                } else {
                    alert("Fail  " + result);
                }
            } else {
                alert("Please input one quality code!");
            }
            return false;

        });

        var tempP = {};
        tempP.MG = GMType;
        grid.GridCheck(tempP);


    });


    $("#btnTransferToAX").click(function () {

        if (!confirm("Confirm to transfer this QualtyCode to AX,continue?"))
            return false;

        if (hasPPO() == false)
         {
             if (confirm("The quality code is not used in bulk order, continue?") == false)
                return false;    
         }
         var sQC = $("#tQC").val();
         var result = AjaxCallFun("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.ApproveQC", "CallWS_AX", sQC);
         alert(result);
//        var result = AjaxCallFunOne("Comfy.App.Web", "Comfy.App.Web.QuailtyCode.EditQC", "QCTrasferToAX", sQC);
//        alert(result);
        return false;
    });


    var assignCombo = new Combo("assignChange", "assignDiv", "assignTable", "370px", "200px", "Comfy.App.Core", "Comfy.App.Core.QualityCode.CustomerManager", "GetCustomerInfo", "assignCombo");
    function assignComboClick(text, val) {
        $("#assignCustomerId").val(val);
    }
    
</script>
</html>
