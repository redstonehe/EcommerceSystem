<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="webMerOrderForm.aspx.cs" Inherits="HuiGouMall.Web.payment.webMerOrderForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>商户订单数据(模拟)</title>
</head>
<body>
<%
    string curTime = DateTime.Now.ToString("yyyyMMddHHmmss");
    string validTime = DateTime.Now.AddDays(2).ToString("yyyyMMddHHmmss");
    StringBuilder sb = new StringBuilder();
    for (int i = 0; i < 6;i++ )
    {
        Random rd = new Random();
        sb.Append(rd.Next(6));
    }
    string ran = sb.ToString();
%>
	<p>商户订单数据</p>
	<form action="CreateWebOrder.aspx"
		method="post">
		<p>协议版本号:<input type="text" name="ver" value="1.0.0" />
		</p>
		<p>业务类型:<input type="text" name="reqType" value="B0002" />
		</p>
		<p>跨境业务类型:<select id="busiRange" name="busiRange">
				<option value="122030">货物贸易</option>
				<option value="222024">航空机票</option>
				<option value="223029">酒店住宿</option>
				<option value="223022">学费教育</option>
			</select>
		</p>
		
		<p>时间戳:<input type="text" name="ts" value="<%=curTime + ran%>" />
		</p>
		
		<p>
			<a id="moButton" href="#">商户订单号</a>:
			<input type="text" id="merOrderId" name="merOrderId"
				value="SH<%=curTime%>" />
		</p>
		
		<p>订单币种:<input type="text" name="currency" value="CNY" />
		</p>
		
		<p>订单金额:<input type="text" name="orderAmount" value="100.00" />
		</p>
		
		<p>订单概要:<input type="text" name="orderSummary" value="测试订单数据"/>
		</p>

		<p>订单日期:<input type="text" name="orderTime" value="<%=curTime%>"/>
		</p>
		<p>订单有效期:<input type="text" name="orderEffTime" value="<%=validTime%>"/>
		</p>
		<p>时区:<input type="text" name="timeZone" value="GMT+8" />
		</p>
		<!-- 10.7.34.188
		<p>前台跳转地址:<input type="text" name="pageUrl" value="http://<%=Request.ServerVariables.Get("LOCAL_ADDR")+":"+Request.ServerVariables.Get("Server_Port") %>/listen/PayPageReturn.aspx"/>
		</p>
		<p>后台通知地址:<input type="text" name="bgUrl" value="http://<%=Request.ServerVariables.Get("LOCAL_ADDR")+":"+Request.ServerVariables.Get("Server_Port") %>/listen/PayReturnNotify.aspx"/>
		</p>
        -->
        <p>前台跳转地址:<input type="text" name="pageUrl" value="http://<%="www.hhwtop.com"+":"+Request.ServerVariables.Get("Server_Port") %>/payment/listen/PayPageReturn.aspx"/>
		</p>
		<p>后台通知地址:<input type="text" name="bgUrl" value="http://<%="www.hhwtop.com"+":"+Request.ServerVariables.Get("Server_Port") %>/payment/listen/PayReturnNotify.aspx"/>
		</p>


		<p>扩展字段1:<input type="text" name="ext1" value="">
		</p>
		<p>扩展字段2:<input type="text" name="ext2" value="">
		</p>

		<input type="submit" value="提交" />
	</form>
	<a href="/Default.aspx">返回首页</a>
</body>
</html>
