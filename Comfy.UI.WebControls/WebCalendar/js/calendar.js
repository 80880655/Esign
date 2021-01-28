Date.prototype.format = function (format) {
    var o = {
        "M+": this.getMonth() + 1,
        "d+": this.getDate(),
        "h+": this.getHours(),
        "m+": this.getMinutes(),
        "s+": this.getSeconds(),
        "q+": Math.floor((this.getMonth() + 3) / 3),
        "S": this.getMilliseconds()
    }

    if (/(y+)/.test(format))
        format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.lenght));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(format))
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
    return format;
}
var bMoveable=true;
var strFrame;		

document.writeln('<iframe id="endDateLayer" name="endDateLayer" frameborder="0" width="162px" height="211px" style="position: absolute;  z-index: 9998;display: none"></iframe>');
strFrame='<style>';
strFrame+='INPUT.button{BORDER-RIGHT: #63A3E9 1px solid;BORDER-TOP: #63A3E9 1px solid;BORDER-LEFT: #63A3E9 1px solid;';
strFrame+='BORDER-BOTTOM: #63A3E9 1px solid;BACKGROUND-COLOR: #63A3E9;font-family:宋体;}';
strFrame+='TD{FONT-SIZE: 9pt;font-family:宋体;}';
strFrame+='</style>';
strFrame+='<scr' + 'ipt>';
strFrame+='var datelayerx,datelayery;';
strFrame+='var bDrag;';
strFrame+='function document.onmousemove()';
strFrame+='{if(bDrag && window.event.button==1)';
strFrame+='	{var DateLayer=parent.document.all.endDateLayer.style;';
strFrame+='		DateLayer.posLeft += window.event.clientX-datelayerx;';
strFrame+='		DateLayer.posTop += window.event.clientY-datelayery;}}';
strFrame+='function DragStart()';
strFrame+='{var DateLayer=parent.document.all.endDateLayer.style;';
strFrame+='	datelayerx=window.event.clientX;';
strFrame+='	datelayery=window.event.clientY;';
strFrame+='	bDrag=true;}';
strFrame+='function DragEnd(){';
strFrame+='	bDrag=false;}';
strFrame+='</scr' + 'ipt>';
strFrame+='<div style="z-index:9999;position: absolute; left:0; top:0;" onselectstart="return false">';
strFrame+='<span id=tmpSelectYearLayer  style="z-index: 9999;position: absolute;top: 3; left: 19;display: none"></span>';
strFrame+='<span id=tmpSelectMonthLayer  style="z-index: 9999;position: absolute;top: 3; left: 78;display: none"></span>';
strFrame+='<span id=tmpSelectHourLayer  style="z-index: 9999;position: absolute;top: 188; left: 35px;display: none"></span>';
strFrame+='<span id=tmpSelectMinuteLayer style="z-index:9999;position:absolute;top: 188; left: 77px;display: none"></span>';
strFrame+='<span id=tmpSelectSecondLayer style="z-index:9999;position:absolute;top: 188; left: 119px;display: none"></span>';
strFrame+='<table border=1 cellspacing=0 cellpadding=0 width=142 height=160 bordercolor=#63A3E9 bgcolor=#63A3E9 >';
strFrame+='    <tr><td width=142 height=23  bgcolor=#FFFFFF>';
strFrame+='        <table border=0 cellspacing=1 cellpadding=0 width=158  height=23>';
strFrame+='            <tr align=center >';
strFrame+='                <td width=16 align=center bgcolor=#63A3E9 style="font-size:12px;cursor: pointer;color: #ffffff" ';
strFrame+='					   onclick="parent.meizzPrevM()" title="向前翻 1 月" ><b >&lt;</b></td>';
strFrame+='				   <td width=60 align="center" bgcolor="#63A3E9"  style="font-size:12px;cursor:pointer" ';
strFrame+='				       onmouseover="style.backgroundColor=\'#aaccf3\'"';
strFrame+='					   onmouseout="style.backgroundColor=\'#63A3E9\'" ';
strFrame+='					   onclick="parent.tmpSelectYearInnerHTML(this)" ';
strFrame+='					   title="点击这里选择年份"><span  id=meizzYearHead></span></td>';
strFrame+='				   <td width=48 align="center" style="font-size:12px;font-color: #ffffff;cursor:pointer" ';
strFrame+='					   bgcolor="#63A3E9" onmouseover="style.backgroundColor=\'#aaccf3\'" ';
strFrame+='					   onmouseout="style.backgroundColor=\'#63A3E9\'" ';
strFrame+='					   onclick="parent.tmpSelectMonthInnerHTML(this)" ';
strFrame+='					   title="点击这里选择月份"><span id=meizzMonthHead ></span></td>';
strFrame+='				   <td width=16 bgcolor=#63A3E9 align=center style="font-size:12px;cursor: pointer;color: #ffffff" ';
strFrame+='					   onclick="parent.meizzNextM()" title="向后翻 1 月" ><b >&gt;</b></td>';
strFrame+='			   </tr>';
strFrame+='		   </table></td></tr>';
strFrame+='	   <tr><td width=142 height=18 >';
strFrame+='		   <table border=0 cellspacing=0 cellpadding=2 bgcolor=#63A3E9 ' + (bMoveable? 'onmousedown="DragStart()" onmouseup="DragEnd()"':'');
strFrame+='				BORDERCOLORLIGHT=#63A3E9 BORDERCOLORDARK=#FFFFFF width=140 height=20  style="cursor:' + (bMoveable ? 'move':'default') + '">';
strFrame+='	   <tr><td style="font-size:12px;color:#ffffff" width=20>&nbsp;日</td>';
strFrame+='<td style="font-size:12px;color:#FFFFFF" >&nbsp;一</td><td style="font-size:12px;color:#FFFFFF">&nbsp;二</td>';
strFrame+='<td style="font-size:12px;color:#FFFFFF" >&nbsp;三</td><td style="font-size:12px;color:#FFFFFF" >&nbsp;四</td>';
strFrame+='<td style="font-size:12px;color:#FFFFFF" >&nbsp;五</td><td style="font-size:12px;color:#FFFFFF" >&nbsp;六</td></tr>';
strFrame+='</table></td></tr>';
strFrame+='  <tr ><td width=142 height=120 >';
strFrame+='    <table border=1 cellspacing=2 cellpadding=2 BORDERCOLORLIGHT=#63A3E9 BORDERCOLORDARK=#FFFFFF bgcolor=#fff8ec width=140 height=120 >';
var n=0; for (j=0;j<5;j++){ strFrame+= ' <tr align=center >'; for (i=0;i<7;i++){
strFrame+='<td width=20 height=20 id=meizzDay'+n+' style="font-size:12px" onclick=parent.meizzDayClick(this,0)></td>';n++;}
strFrame+='</tr>';}
strFrame+='      <tr align=center >';
for (i=35;i<37;i++)strFrame+='<td width=20 height=20 id=meizzDay'+i+' style="font-size:12px"  onclick="parent.meizzDayClick(this,0)"></td>';
strFrame+='        <td colspan=5 align=right style="color:#1478eb"><span onclick="parent.setNull()" style="font-size:12px;cursor: pointer"';
strFrame+='         onmouseover="style.color=\'#ff0000\'" onmouseout="style.color=\'#1478eb\'" title="将日期置空">置空</span>&nbsp;&nbsp;<span onclick="parent.meizzToday()" style="font-size:12px;cursor: pointer"';
strFrame+='         onmouseover="style.color=\'#ff0000\'" onmouseout="style.color=\'#1478eb\'" title="当前日期时间">当前</span>&nbsp;&nbsp;<span style="cursor:pointer" id=evaAllOK onmouseover="style.color=\'#ff0000\'" onmouseout="style.color=\'#1478eb\'"  onclick="parent.closeLayer()" title="关闭日历">关闭&nbsp;</span></td></tr>';
strFrame+='    </table></td></tr><tr ><td >';
strFrame+='        <table border=0 cellspacing=1 cellpadding=0 width=100%  bgcolor=#FFFFFF height=22 >';
strFrame+='          <tr bgcolor="#63A3E9"><td id=bUseTimeLayer width=30  style="cursor:pointer" title="点击这里启用/禁用时间"';
strFrame+='				onmouseover="style.backgroundColor=\'#aaccf3\'" align=center onmouseout="style.backgroundColor=\'#63A3E9\'"';
strFrame+='				 onclick="parent.UseTime(this)">';
strFrame+=' <span></span></td>';
strFrame+='             <td style="cursor:pointer" onclick="parent.tmpSelectHourInnerHTML(this)" ';
strFrame+=' onmouseover="style.backgroundColor=\'#aaccf3\'" onmouseout="style.backgroundColor=\'#63A3E9\'"';
strFrame+=' title="点击这里选择时间" align=center width=42>' ;
strFrame+='<span id=meizzHourHead></span></td>';
strFrame+='             <td style="cursor:pointer" onclick="parent.tmpSelectMinuteInnerHTML(this)" ';
strFrame+=' onmouseover="style.backgroundColor=\'#aaccf3\'" onmouseout="style.backgroundColor=\'#63A3E9\'"';
strFrame+=' title="点击这里选择时间" align=center width=42>';
strFrame+='<span id=meizzMinuteHead></span></td>';
strFrame+='             <td style="cursor:pointer" onclick="parent.tmpSelectSecondInnerHTML(this)" ';
strFrame+=' onmouseover="style.backgroundColor=\'#aaccf3\'" onmouseout="style.backgroundColor=\'#63A3E9\'"';
strFrame+=' title="点击这里选择时间" align=center width=42>' ;
strFrame+='<span id=meizzSecondHead></span></td>';
strFrame+='			 </tr></table></td></tr></table></div>';


try {
    window.frames.endDateLayer.document.writeln(strFrame);
    window.frames.endDateLayer.document.close(); 	//解决ie进度条不结束的问题
    //alert(window.frames.endDateLayer.document);
} catch (e) {
alert(e);
}



//==================================================== WEB 页面显示部分 ======================================================
var outObject;
var outButton;		//点击的按钮
var dateFormat;
var outDate="";		//存放对象的日期
var bUseTime=false;		//是否使用时间
var odatelayer;
var bImg;
bImgSwitch();
try {
    odatelayer = $(window.frames["endDateLayer"].document).contents(); 	//存放日历对象
    odatelayer.find("#bUseTimeLayer").html(bImg);
} catch (e) {
alert(e);
}




function setday(tt,date_format) //主调函数
{
      //  alert("ss");
	if (arguments.length > 2){alert("对不起！传入本控件的参数太多！");return;}
	if (arguments.length == 0){alert("对不起！您没有传回本控件任何参数！");return;}
        var dads= $("#endDateLayer").get(0).style;
	var th = tt;
	var ttop = tt.offsetTop;	//TT控件的定位点高

	var thei = tt.clientHeight;	//TT控件本身的高
	var tleft = tt.offsetLeft;	//TT控件的定位点宽

	var ttyp = tt.type;	//TT控件的类型

	dateFormat = date_format;

	while (tt = tt.offsetParent){ttop+=tt.offsetTop; tleft+=tt.offsetLeft;}
         
        $("#endDateLayer").css("top",(ttyp=="image") ? ttop+thei : ttop+thei+6);
        $("#endDateLayer").css("left",tleft);
   
	//dads.top = (ttyp=="image") ? ttop+thei : ttop+thei+6;
	//dads.left = tleft;
	outObject = th ;
	outButton = th;	//设定外部点击的按钮

	//根据当前输入框的日期显示日历的年月
	var reg = /^(\d+)-(\d{1,2})-(\d{1,2})/;		//不含时间
	var r = outObject.value.match(reg);
	if(r!=null){
		r[2]=r[2]-1;
		var d=new Date(r[1],r[2],r[3]);
		if(d.getFullYear()==r[1] && d.getMonth()==r[2] && d.getDate()==r[3])
		{
			outDate=d;
			parent.meizzTheYear = r[1];
			parent.meizzTheMonth = r[2];
			parent.meizzTheDate = r[3];
		}
		else
		{
			outDate="";
		}
		meizzSetDay(r[1],r[2]+1);
	}
	else
	{
		outDate="";
		meizzSetDay(new Date().getFullYear(), new Date().getMonth() + 1);
	}
	dads.display = '';

	//判断初始化时是否使用时间,非严格验证
	//if (outObject.value.length>10)
	//{
		bUseTime=true;
		bImgSwitch();
		odatelayer.find("#bUseTimeLayer").html(bImg);
		meizzWriteHead(meizzTheYear,meizzTheMonth);
	//}
	//else
	//{
	//	bUseTime=false;
	//	bImgSwitch();
	//	odatelayer.bUseTimeLayer.innerHTML=bImg;
	//	meizzWriteHead(meizzTheYear,meizzTheMonth);
	//}

	try
	{
		event.returnValue=false;
	}
	catch (e)
	{
		//此处排除错误，错误原因暂未找到。
	}
}

var MonHead = new Array(12);	//定义阳历中每个月的最大天数

MonHead[0] = 31; MonHead[1] = 28; MonHead[2] = 31; MonHead[3] = 30; MonHead[4]  = 31; MonHead[5]  = 30;
MonHead[6] = 31; MonHead[7] = 31; MonHead[8] = 30; MonHead[9] = 31; MonHead[10] = 30; MonHead[11] = 31;

var meizzTheYear=new Date().getFullYear(); //定义年的变量的初始值

var meizzTheMonth=new Date().getMonth()+1; //定义月的变量的初始值

var meizzTheDate=new Date().getDate();	//定义日的变量的初始值	
var meizzTheHour=new Date().getHours();	//定义小时变量的初始值

var meizzTheMinute=new Date().getMinutes();//定义分钟变量的初始值
var meizzTheSecond=new Date().getSeconds();//定义秒变量的初始值

var meizzWDay=new Array(37);	//定义写日期的数组

function onclick() //任意点击时关闭该控件	//ie6的情况可以由下面的切换焦点处理代替
{ 
	with(window.event)
	{
		if (srcElement != outObject && srcElement != outButton)
		closeLayer();
	}
}

function onkeyup()		//按Esc键关闭，切换焦点关闭
{
	if (window.event.keyCode==27){
		if(outObject)outObject.blur();
		closeLayer();
	}
	else if(document.activeElement)
	{
		if(document.activeElement != outObject && document.activeElement != outButton)
		{
			closeLayer();
		}
	}
}

//function document.ondblclick()
//{
//    alter("测试");
//    closeLayer();
//} 

function meizzWriteHead(yy,mm,ss)	//往 head 中写入当前的年与月
{
	odatelayer.find("#meizzYearHead").text(yy + " 年");
	odatelayer.find("#meizzMonthHead").text(format(mm) + " 月");
	//插入当前小时、分
	odatelayer.find("#meizzHourHead").text(bUseTime?(meizzTheHour+" 时"):""); 
	odatelayer.find("#meizzMinuteHead").text(bUseTime?(meizzTheMinute+" 分"):"");
	odatelayer.find("#meizzSecondHead").text(bUseTime?(meizzTheSecond+" 秒"):"");
}

function tmpSelectYearInnerHTML(strYearThis) //年份的下拉框
{
        var strYear = $(strYearThis).text().substring(0,4);
	if (strYear.match(/\D/)!=null){alert("年份输入参数不是数字！");return;}
	var m = (strYear) ? strYear : new Date().getFullYear();
	if (m < 1000 || m > 9999) {alert("年份值不在 1000 到 9999 之间！");return;}
	var n = m - 50;
	if (n < 1000) n = 1000;
	if (n + 101 > 9999) n = 9974;

	var s = "&nbsp;<select name=tmpSelectYear style='font-size: 12px' "
	s += "onblur='document.all.tmpSelectYearLayer.style.display=\"none\"' "
	s += "onchange='document.all.tmpSelectYearLayer.style.display=\"none\";"
	s += "parent.meizzTheYear = this.value; parent.meizzSetDay(parent.meizzTheYear,parent.meizzTheMonth)'>\r\n";
	var selectInnerHTML = s;
	for (var i = n; i < n + 101; i++)
	{
		if (i == m) { selectInnerHTML += "<option value='" + i + "' selected>" + i + "年" + "</option>\r\n"; }
		else { selectInnerHTML += "<option value='" + i + "'>" + i + "年" + "</option>\r\n"; }
	}
	selectInnerHTML += "</select>";
	odatelayer.find("#tmpSelectYearLayer").show();
	odatelayer.find("#tmpSelectYearLayer").html(selectInnerHTML);
	//odatelayer.find("#tmpSelectYear").get(0).focus();
}



function tmpSelectMonthInnerHTML(strMonthThis) //月份的下拉框
{
        var strMonth = $(strMonthThis).text().length==3?$(strMonthThis).text().substring(0,1):$(strMonthThis).text().substring(0,2); 
	if (strMonth.match(/\D/)!=null){alert("月份输入参数不是数字！");return;}
	var m = (strMonth) ? strMonth : new Date().getMonth() + 1;
	var s = "&nbsp;&nbsp;<select name=tmpSelectMonth style='font-size: 12px' "
	s += "onblur='document.all.tmpSelectMonthLayer.style.display=\"none\"' "
	s += "onchange='document.all.tmpSelectMonthLayer.style.display=\"none\";"
	s += "parent.meizzTheMonth = this.value; parent.meizzSetDay(parent.meizzTheYear,parent.meizzTheMonth)'>\r\n";
	var selectInnerHTML = s;
	for (var i = 1; i < 13; i++)
	{
		if (i == m) { selectInnerHTML += "<option value='"+i+"' selected>"+i+"月"+"</option>\r\n"; }
		else { selectInnerHTML += "<option value='"+i+"'>"+i+"月"+"</option>\r\n"; }
	}
	selectInnerHTML += "</select>";
	odatelayer.find("#tmpSelectMonthLayer").show();
	odatelayer.find("#tmpSelectMonthLayer").html(selectInnerHTML);
	//odatelayer.find("#tmpSelectMonth").get(0).focus();
}

/***** 增加 小时、分钟 ***/
function tmpSelectHourInnerHTML(strHourThis) //小时的下拉框
{

        var strHour = $(strHourThis).text().length==3?$(strHourThis).text().substring(0,1):$(strHourThis).text().substring(0,2);
	if (!bUseTime){return;}

	if (strHour.match(/\D/)!=null){alert("小时输入参数不是数字！");return;}
	var m = (strHour) ? strHour : new Date().getHours();
	var s = "<select name=tmpSelectHour style='font-size: 12px' "
	s += "onblur='document.all.tmpSelectHourLayer.style.display=\"none\"' "
	s += "onchange='document.all.tmpSelectHourLayer.style.display=\"none\";"
	s += "parent.meizzTheHour = this.value; parent.evaSetTime(parent.meizzTheHour,parent.meizzTheMinute);'>\r\n";
	var selectInnerHTML = s;
	for (var i = 0; i < 24; i++)
	{
		if (i == m) { selectInnerHTML += "<option value='"+i+"' selected>"+i+"</option>\r\n"; }
		else { selectInnerHTML += "<option value='"+i+"'>"+i+"</option>\r\n"; }
	}
	selectInnerHTML += "</select>";
	odatelayer.find("#tmpSelectHourLayer").show();
	odatelayer.find("#tmpSelectHourLayer").html(selectInnerHTML);
	//odatelayer.find("#tmpSelectHour").get(0).focus();
}

function tmpSelectMinuteInnerHTML(strMinuteThis) //分钟的下拉框
{
	var strMinute = $(strMinuteThis).text().length==3?$(strMinuteThis).text().substring(0,1):$(strMinuteThis).text().substring(0,2);
        if (!bUseTime){return;}

	if (strMinute.match(/\D/)!=null){alert("分钟输入参数不是数字！");return;}
	var m = (strMinute) ? strMinute : new Date().getMinutes();
	var s = "<select name=tmpSelectMinute style='font-size: 12px' "
	s += "onblur='document.all.tmpSelectMinuteLayer.style.display=\"none\"' "
	s += "onchange='document.all.tmpSelectMinuteLayer.style.display=\"none\";"
	s += "parent.meizzTheMinute = this.value; parent.evaSetTime(parent.meizzTheHour,parent.meizzTheMinute);'>\r\n";
	var selectInnerHTML = s;
	for (var i = 0; i < 60; i++)
	{
		if (i == m) { selectInnerHTML += "<option value='"+i+"' selected>"+i+"</option>\r\n"; }
		else { selectInnerHTML += "<option value='"+i+"'>"+i+"</option>\r\n"; }
	}
	selectInnerHTML += "</select>";
	odatelayer.find("#tmpSelectMinuteLayer").show();
	odatelayer.find("#tmpSelectMinuteLayer").html(selectInnerHTML);
	//odatelayer.find("#tmpSelectMinute").get(0).focus();
}

function tmpSelectSecondInnerHTML(strSecondThis) //秒的下拉框
{
	var strSecond = $(strSecondThis).text().length==3?$(strSecondThis).text().substring(0,1):$(strSecondThis).text().substring(0,2);
        if (!bUseTime){return;}

	if (strSecond.match(/\D/)!=null){alert("分钟输入参数不是数字！");return;}
	var m = (strSecond) ? strSecond : new Date().getMinutes();
	var s = "<select name=tmpSelectSecond style='font-size: 12px' "
	s += "onblur='document.all.tmpSelectSecondLayer.style.display=\"none\"' "
	s += "onchange='document.all.tmpSelectSecondLayer.style.display=\"none\";"
	s += "parent.meizzTheSecond = this.value; parent.evaSetTime(parent.meizzTheHour,parent.meizzTheMinute,parent.meizzTheSecond);'>\r\n";
	var selectInnerHTML = s;
	for (var i = 0; i < 60; i++)
	{
		if (i == m) { selectInnerHTML += "<option value='"+i+"' selected>"+i+"</option>\r\n"; }
		else { selectInnerHTML += "<option value='"+i+"'>"+i+"</option>\r\n"; }
	}
	selectInnerHTML += "</select>";
	odatelayer.find("#tmpSelectSecondLayer").show();
	odatelayer.find("#tmpSelectSecondLayer").html(selectInnerHTML);
	//odatelayer.find("#tmpSelectSecond").get(0).focus();
}

function closeLayer()	//这个层的关闭
{
	var o = document.getElementById("endDateLayer");
	if (o != null)
	{
		o.style.display="none";
	}
}

function showLayer()	//这个层的关闭
{
	document.all.endDateLayer.style.display="";
}

function IsPinYear(year)	//判断是否闰平年
{
	if (0==year%4&&((year%100!=0)||(year%400==0))) return true;else return false;
}

function GetMonthCount(year,month)	//闰年二月为29天
{
	var c=MonHead[month-1];if((month==2)&&IsPinYear(year)) c++;return c;
}

function GetDOW(day,month,year)	//求某天的星期几
{
	var dt=new Date(year,month-1,day).getDay()/7; return dt;
}

function meizzPrevY()	//往前翻 Year
{
	if(meizzTheYear > 999 && meizzTheYear <10000){meizzTheYear--;}
	else{alert("年份超出范围（1000-9999）！");}
	meizzSetDay(meizzTheYear,meizzTheMonth);
}
function meizzNextY()	//往后翻 Year
{
	if(meizzTheYear > 999 && meizzTheYear <10000){meizzTheYear++;}
	else{alert("年份超出范围（1000-9999）！");}
	meizzSetDay(meizzTheYear,meizzTheMonth);
}
function setNull()
{
	outObject.value = '';
	closeLayer();
}
function meizzToday()	//Today Button
{
	parent.meizzTheYear		= new Date().getFullYear();
	parent.meizzTheMonth	= new Date().getMonth()+1;
	parent.meizzTheDate		= new Date().getDate();
	parent.meizzTheHour		= new Date().getHours();
	parent.meizzTheMinute	= new Date().getMinutes();
	parent.meizzTheSecond	= new Date().getSeconds();
	var meizzTheSecond		= new Date().getSeconds();

	if (meizzTheMonth<10 && meizzTheMonth.length<2)	//格式化成两位数字
	{
		parent.meizzTheMonth="0"+parent.meizzTheMonth;
	}
	if (parent.meizzTheDate<10 && parent.meizzTheDate.length<2)	//格式化成两位数字
	{
		parent.meizzTheDate="0"+parent.meizzTheDate;
	}
	//meizzSetDay(meizzTheYear,meizzTheMonth);
	if(outObject)
	{
		if (bUseTime)
		{
		   
			var myDate= parent.meizzTheYear + "-" + format( parent.meizzTheMonth) + "-" + 
							format(parent.meizzTheDate) + " " + format(parent.meizzTheHour) + ":" + 
							format(parent.meizzTheMinute) + ":" + format(parent.meizzTheSecond);
			//注：在这里你可以输出改成你想要的格式

			var dd = new Date( Date.parse(myDate.replace(/-/g,"/")));
			outObject.value = dd.format(dateFormat);
			//outObject.fireEvent('onchange');
			$(outObject).change();

		}
		else
		{
		//	outObject.value= parent.meizzTheYear + "-" + format( parent.meizzTheMonth) + "-" +
							//format(parent.meizzTheDate); //注：在这里你可以输出改成你想要的格式

			var myDate = parent.meizzTheYear + "-" + format(parent.meizzTheMonth) + "-" +
							format(parent.meizzTheDate) + " " + format(parent.meizzTheHour) + ":" +
							format(parent.meizzTheMinute) + ":" + format(parent.meizzTheSecond);
			//注：在这里你可以输出改成你想要的格式

			var dd = new Date(Date.parse(myDate.replace(/-/g, "/")));
			outObject.value = dd.format(dateFormat);
			//outObject.fireEvent('onchange');
			$(outObject).change();
		}
	}
	closeLayer();
}
function meizzPrevM()	//往前翻月份
{
	if(meizzTheMonth>1){meizzTheMonth--}else{meizzTheYear--;meizzTheMonth=12;}
	meizzSetDay(meizzTheYear,meizzTheMonth);
}
function meizzNextM()	//往后翻月份
{
	if(meizzTheMonth==12){meizzTheYear++;meizzTheMonth=1}else{meizzTheMonth++}
	meizzSetDay(meizzTheYear,meizzTheMonth);
}

// TODO: 整理代码
function meizzSetDay(yy,mm)	//主要的写程序**********
{
	meizzWriteHead(yy,mm);
	//设置当前年月的公共变量为传入值

	meizzTheYear=yy;
	meizzTheMonth=mm;

	for (var i = 0; i < 37; i++){meizzWDay[i]=""};	//将显示框的内容全部清空

	var day1 = 1,day2=1,firstday = new Date(yy,mm-1,1).getDay();	//某月第一天的星期几

	for (i=0;i<firstday;i++)meizzWDay[i]=GetMonthCount(mm==1?yy-1:yy,mm==1?12:mm-1)-firstday+i+1	//上个月的最后几天

	for (i = firstday; day1 < GetMonthCount(yy,mm)+1; i++) { meizzWDay[i]=day1;day1++; }
	for (i=firstday+GetMonthCount(yy,mm);i<37;i++) { meizzWDay[i]=day2;day2++; }
	for (i = 0; i < 37; i++)
	{
		var da =odatelayer.find("#meizzDay"+i).get(0);	//书写新的一个月的日期星期排列

	if (meizzWDay[i]!="")
	{
		//初始化边框
		da.borderColorLight="#63A3E9";
		da.borderColorDark="#63A3E9";
		da.style.color="#1478eb";
		if(i<firstday)		//上个月的部分
		{
			da.innerHTML="<b><font color=#BCBABC>" + meizzWDay[i] + "</font></b>";
			da.title=(mm==1?12:mm-1) +"月" + meizzWDay[i] + "日";
			da.onclick=Function("meizzDayClick(this,-1)");

			if(!outDate)
				da.style.backgroundColor = ((mm==1?yy-1:yy) == new Date().getFullYear() && (mm==1?12:mm-1) == new Date().getMonth()+1 && meizzWDay[i] == new Date().getDate()) ?"#5CEFA0":"#f5f5f5";
			else
			{
				da.style.backgroundColor =((mm==1?yy-1:yy)==outDate.getFullYear() && (mm==1?12:mm-1)== outDate.getMonth() + 1 && meizzWDay[i]==outDate.getDate())? "#84C1FF" :
				(((mm==1?yy-1:yy) == new Date().getFullYear() && (mm==1?12:mm-1) == new Date().getMonth()+1 && meizzWDay[i] == new Date().getDate()) ? "#5CEFA0":"#f5f5f5");
				//将选中的日期显示为凹下去
				if((mm==1?yy-1:yy)==outDate.getFullYear() && (mm==1?12:mm-1)== outDate.getMonth() + 1 && meizzWDay[i]==outDate.getDate())
				{
					da.borderColorLight="#FFFFFF";
					da.borderColorDark="#63A3E9";
				}
			}
		}
		else if (i>=firstday+GetMonthCount(yy,mm))		//下个月的部分
		{
			da.innerHTML="<b><font color=#BCBABC>" + meizzWDay[i] + "</font></b>";
			da.title=(mm==12?1:mm+1) +"月" + meizzWDay[i] + "日";
			da.onclick=Function("meizzDayClick(this,1)");
			if(!outDate)
				da.style.backgroundColor = ((mm==12?yy+1:yy) == new Date().getFullYear() && 
					(mm==12?1:mm+1) == new Date().getMonth()+1 && meizzWDay[i] == new Date().getDate()) ?
					 "#5CEFA0":"#f5f5f5";
			else
			{
				da.style.backgroundColor =((mm==12?yy+1:yy)==outDate.getFullYear() && (mm==12?1:mm+1)== outDate.getMonth() + 1 && meizzWDay[i]==outDate.getDate())? "#84C1FF" :(((mm==12?yy+1:yy) == new Date().getFullYear() && (mm==12?1:mm+1) == new Date().getMonth()+1 && meizzWDay[i] == new Date().getDate()) ? "#5CEFA0":"#f5f5f5");
				//将选中的日期显示为凹下去

				if((mm==12?yy+1:yy)==outDate.getFullYear() && (mm==12?1:mm+1)== outDate.getMonth() + 1 && meizzWDay[i]==outDate.getDate())
				{
					da.borderColorLight="#FFFFFF";
					da.borderColorDark="#63A3E9";
				}
			}
		}
		else		//本月的部分

		{
			da.innerHTML="<b>" + meizzWDay[i] + "</b>";
			da.title=mm +"月" + meizzWDay[i] + "日";
			da.onclick=Function("meizzDayClick(this,0)");		//给td赋予onclick事件的处理

			//如果是当前选择的日期，则显示亮蓝色的背景；如果是当前日期，则显示暗黄色背景
			if(!outDate)
				da.style.backgroundColor = (yy == new Date().getFullYear() && mm == new Date().getMonth()+1 && meizzWDay[i] == new Date().getDate())?"#5CEFA0":"#f5f5f5";
			else
			{
				da.style.backgroundColor =(yy==outDate.getFullYear() && mm== outDate.getMonth() + 1 && meizzWDay[i]==outDate.getDate())?
					"#84C1FF":((yy == new Date().getFullYear() && mm == new Date().getMonth()+1 && meizzWDay[i] == new Date().getDate())?"#5CEFA0":"#f5f5f5");
				//将选中的日期显示为凹下去

				if(yy==outDate.getFullYear() && mm== outDate.getMonth() + 1 && meizzWDay[i]==outDate.getDate())
				{
					da.borderColorLight="#FFFFFF";
					da.borderColorDark="#63A3E9";
				}
			}
		}
				da.style.cursor="pointer"
			}
		else { da.innerHTML="";da.style.backgroundColor="";da.style.cursor="default"; }
	}
}

function meizzDayClick(nThis,ex)	//点击显示框选取日期，主输入函数*************
{
        var n = $(nThis).text();
	parent.meizzTheDate=n;
	var yy=meizzTheYear;
	var mm = parseInt(meizzTheMonth)+ex;	//ex表示偏移量，用于选择上个月份和下个月份的日期
	var hh=meizzTheHour;
	var mi=meizzTheMinute;
	var se=meizzTheSecond;
	//判断月份，并进行对应的处理

	if(mm<1){
		yy--;
		mm=12+mm;
	}
	else if(mm>12){
		yy++;
		mm=mm-12;
	}

	if (mm < 10)	{mm = "0" + mm;}
	if (hh<10)		{hh="0" + hh;}	//时
	if (mi<10)		{mi="0" + mi;}	//分
	if (se<10)		{se="0" + se;}	//秒

	if (outObject)
	{
		if (!n) {	//outObject.value=""; 
			return;}
		if ( n < 10){n = "0" + n;}

		WriteDateTo(yy,mm,n,hh,mi,se);

		closeLayer(); 
		if (bUseTime)
		{
			try
			{
				//outButton.click();
				closeLayer();
				
				
			}
			catch (e)
			{
				setday(outObject);
			}
		}
	}
	else {closeLayer(); alert("您所要输出的控件对象并不存在！");}
}

function format(n)	//格式化数字为两位字符表示
{
	var m=new String();
	var tmp=new String(n);
	if (n<10 && tmp.length<2)
	{
		m="0"+n;
	}
	else
	{
		m=n;
	}
	return m;
}

function evaSetTime()		//设置用户选择的小时、分钟
{
	odatelayer.find("#meizzHourHead").text(meizzTheHour+" 时");
	odatelayer.find("#meizzMinuteHead").text(meizzTheMinute+" 分");
	odatelayer.find("#meizzSecondHead").text(meizzTheSecond+" 秒");
	WriteDateTo(meizzTheYear,meizzTheMonth,meizzTheDate,meizzTheHour,meizzTheMinute,meizzTheSecond)
}

function evaSetTimeNothing()	//设置时间控件为空
{
	odatelayer.find("#meizzHourHead").text("");
	odatelayer.find("#meizzMinuteHead").text("");
	odatelayer.find("#meizzSecondHead").text("");
	WriteDateTo(meizzTheYear,meizzTheMonth,meizzTheDate,meizzTheHour,meizzTheMinute,meizzTheSecond)
}

function evaSetTimeNow()	//设置时间控件为当前时间
{
	odatelayer.find("#meizzHourHead").text(new Date().getHours()+" 时");
	odatelayer.find("#meizzMinuteHead").text(new Date().getMinutes()+" 分");
	odatelayer.find("#meizzSecondHead").text(new Date().getSeconds()+" 秒");
	meizzTheHour = new Date().getHours();
	meizzTheMinute = new Date().getMinutes();
	meizzTheSecond = new Date().getSeconds();
	WriteDateTo(meizzTheYear,meizzTheMonth,meizzTheDate,meizzTheHour,meizzTheMinute,meizzTheSecond)
}

function UseTime(ctl)
{
	bUseTime=!bUseTime;
	if (bUseTime)
	{
		bImgSwitch();
		ctl.innerHTML=bImg;
		evaSetTime();		//显示时间，用户原来选择的时间
		//evaSetTimeNow();	//显示当前时间
	}
	else
	{
		bImgSwitch();
		ctl.innerHTML=bImg;
		evaSetTimeNothing();
	}
}

function WriteDateTo(yy,mm,n,hh,mi,se)
{
	if (bUseTime)
	{
		//outObject.value= yy + "-" + format(mm) + "-" + format(n) + " " + format(hh) + ":" + format(mi) + ":" + format(se); //注：在这里你可以输出改成你想要的格式
	    var myDate = yy + "-" + format(mm) + "-" + format(n) + " " + format(hh) + ":" + format(mi) + ":" + format(se);
	    //注：在这里你可以输出改成你想要的格式

	    var dd = new Date(Date.parse(myDate.replace(/-/g, "/")));
	    outObject.value = dd.format(dateFormat);
	    // outObject.fireEvent('onchange');
	    $(outObject).change();
    }
	else
	{
	   // outObject.value = yy + "-" + format(mm) + "-" + format(n); //注：在这里你可以输出改成你想要的格式

	    var myDate = yy + "-" + format(mm) + "-" + format(n) + " " + format(hh) + ":" + format(mi) + ":" + format(se);
	    //注：在这里你可以输出改成你想要的格式

	    var dd = new Date(Date.parse(myDate.replace(/-/g, "/")));
	    outObject.value = dd.format(dateFormat);
	  //  outObject.fireEvent('onchange');
	    $(outObject).change();
	}
}

function bImgSwitch()
{
	if (bUseTime)
	{
		bImg="开启";
	}
	else
	{
		bImg="关闭";
	}
}

function checkDate(date) {
    var reg = /^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$/;
    var regdatetime = /^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d$/;
    var n = date.indexOf(" ");
    if (n <= 0 || date.length <= 10) {
        if (!reg.test(date)) {
            alert("日期格式不正确！")
        }
    }
    else {
        if (!regdatetime.test(date)) {
            alert("日期或时间格式不正确！")
        }
    }
}


$(function () {
    $(document).click(function () {
        with (window.event) {
            if (srcElement != outObject && srcElement != outButton)
                closeLayer();
        }

    }); //任意点击时关闭该控件	//ie6的情况可以由下面的切换焦点处理代替

    $(document).keyup(function () {

        if (window.event.keyCode == 27) {
            if (outObject) outObject.blur();
            closeLayer();
        }
        else if (document.activeElement) {
            if (document.activeElement != outObject && document.activeElement != outButton) {
                closeLayer();
            }
        }
    });
    })