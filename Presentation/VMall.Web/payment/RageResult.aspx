<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RageResult.aspx.cs" Inherits="HuiGouMall.Web.payment.RageResult" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>商户接口-页面通知</title>
</head>
<body>
    
<table style="border: 0px;">
    <thead><tr>
      <th>参数</th>
      <th>返回值</th>
    </tr></thead>
<%
    Dictionary<String, String> displayData = (Dictionary<String, String>)Session["__display_data__"];
    if (null != displayData)
    {

        
%>
    <tr><td>支付订单号</td><td>=</td><td><%=displayData["merOrderId"]%></td></tr>
    
<tr><td>支付状态</td><td>=</td><td><%=displayData["retMsg"]%></td></tr>
<tr><td>支付时间</td><td>=</td><td><%=displayData["dealTime"]%></td></tr>
    <tr><td>支付金额</td><td>=</td><td><%=displayData["payAmount"]%></td></tr>
        <tr><td>支付流水号</td><td>=</td><td><%=displayData["transactionId"]%></td></tr>
    
<% 
  }%>
</table>
<br/>
<a href="/Default.aspx" >返回首页</a>
</body>
</html>
