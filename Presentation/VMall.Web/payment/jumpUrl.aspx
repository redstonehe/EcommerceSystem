<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="jumpUrl.aspx.cs" Inherits="VMall.Web.payment.jumpUrl" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>页面跳转</title>
</head>
<body>
    请稍候，页面跳转中...
<form name="jumpForm" method="post" style="display: none;" action="<%=(string)Session["jumpUrl"]%>">
    <%
        Dictionary<String, String> jumpData = (Dictionary<String, String>)Session["jumpData"];
        if (null != jumpData)
        {
        
        foreach (var dic in jumpData ) {
    %>
    <input type='hidden' name='<%=dic.Key %>' value="<%=dic.Value %>" />
    <%} }%>
</form>
<script type="text/javascript">
    document.jumpForm.submit();
</script>

</body>
</html>
