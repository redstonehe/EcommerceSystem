<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NativeForM.aspx.cs" Inherits="VMall.Web.payment.NativeForM" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0"/>
    <meta name="apple-mobile-web-app-capable" content="yes"/>
    <meta name="apple-mobile-web-app-status-bar-style" content="black"/>
    <meta content="telephone=no" name="format-detection"/>
    <meta content="yes" name="apple-touch-fullscreen"/>
    <meta name="keywords" content="" />
    <meta name="description" content="" />
    <title>微信支付-汇购</title>
       <link href="/mobile/css/base.css" rel="stylesheet" type="text/css"/>
     <script src="/mobile/scripts/jquery.js" type="text/javascript"></script>
    <style>
        .wx_ok {
            width: 200px;
            display: block;
            height: 36px;
            line-height: 36px;
            font-size: 24px;
            color: #fff;
            background: #15A415;
            text-align: center;
        }
    </style>
</head>
    <script>
        var iCount = setInterval(check, 2000);  //每隔2秒执行一次check函数。

        function check() {
            $.ajax({
                type: "POST",
                //contentType: "application/json", data: { pid: pid, useAddress: useAddress, addressid: addressid, consignee: consignee, mobile: mobile, regionId: regionId, address: address }
                url: "/order/NativePayCheck",
                contentType: "application/x-www-form-urlencoded; charset=utf-8",
                data: { oidList: <%=(string)Session["NativePayOids"]%> },
                //dataType: "json",
                success: function (data) {
                  
                    var json = eval("(" + data + ")");
                    //console.log(json.state);
                    //alert(json.state);
                    if (json.state == "success") {
                        clearInterval(iCount);
                        $(".weixin-sao").html("已成功付款");
                        $("#payQRImg").html('<img alt="" src="/images/wx_ok.jpg" width="200">');
                        $("#payQRwarn").html('<a href="/mob" class="wx_ok"><span style="color:#fff">完成</span></a>');

                    }
                    else if(json.state == "nologin"){
                        clearInterval(iCount);
                        alert("您还未登录，请先登录")
                        window.location.href='/account/login?returnUrl='+window.location.href;
                    }
                },
                error: function (err, ex) {
                }
            });
        }</script>
<body>
     <div class="viewport">
         <header>
  <div class="new-header">
    <a href="javascript:pageBack()" class="new-a-back"><span>返回</span></a>
    <h2>微信支付</h2>
    <a href="javascript:navSH()" class="new-a-brn"><span>导航键</span></a> 
  </div>
  <div class="new-brn-tab" id="nav" style=" display:none;">
    <div class="new-tbl-type"> 
    <a href="@Url.Action("index", "home")" class="new-tbl-cell"><span class="icon">首页</span><p style="color:#6e6e6e;">首页</p></a> 
    <a href="@Url.Action("list", "category")" class="new-tbl-cell"><span class="icon2 on">分类搜索</span><p style="color:#6e6e6e;" class="on">分类搜索</p></a> 
    <a href="@Url.Action("index", "cart")" class="new-tbl-cell"><span class="icon3">购物车</span><p style="color:#6e6e6e;">购物车</p></a> 
    <a href="@Url.Action("index", "ucenter")" class="new-tbl-cell"><span class="icon4 on">个人中心</span><p style="color:#6e6e6e;" class="on">个人中心</p></a> 
    </div>
  </div>
</header>
    <div style="text-align:center;">
        <div>
            <p class="ordermes">订单编号：<%=(string)Session["NativePayOSN"]%></p>
            <p class="ordermes">应付金额：<span style="color:red;">¥<%=(string)Session["NativePayOPrice"]%></span></p>
            <div style="margin-left: 10px; color: #333; font-size: 18px; font-weight: bolder; text-align: center; margin: 25px;" class="weixin-sao">长按二维码识别付款</div>
            <div id="payQRImg">
                    <img src="/webchat/<%=(string)Session["NativePayimageUrl"]%>" alt="" width="200" height="200" /><br />
                </div>
            <div id="payQRwarn" style="margin:0 21%;">
                    <img src="/images/paywzsm.png" alt="" width="200" />

                </div>
            <div style="text-align:center;margin-top: 30px;font-size: 24px;">
                <a href="/mob">返回继续购物</a>
            </div>
        </div>
    </div>
    </div>
</body>
</html>
