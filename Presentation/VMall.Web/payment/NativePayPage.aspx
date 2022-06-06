<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NativePayPage.aspx.cs" Inherits="VMall.Web.payment.NativePayPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>微信支付-xx商城</title>
    <link href="/css/base.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/jquery.js" type="text/javascript"></script>
    <style>
        .wechatpay {
            width: 900px;
            margin: 20px auto;
            margin-bottom: 50px;
            border: 2px solid #e8e8e8;
            background: #FFF;
            /*height: 355px;*/
            position: relative;
        }

        #payTop {
            padding: 30px 0;
            margin-top: 10px;
            width: 1000px;
        }

        .box {
            width: 1000px;
            margin: 0 auto;
            text-align: left;
        }

        .left {
            float: left;
        }

        #payTop h2 {
            float: left;
            height: 80px;
            line-height: 80px;
            border-left: 1px solid #e3e3e3;
            padding-left: 15px;
            margin-left: 15px;
            font-size: 24px;
            font-weight: normal;
            color: #333;
        }

        .clear {
            clear: both;
        }

        .wechatlogo {
            margin: 10px;
        }

        .orderinfo {
            background: #F2F2F2;
        }

            .orderinfo .ordermes {
                display: inline-block;
                margin: 20px;
                font-size: 18px;
                font-weight: bolder;
            }

            .orderinfo .paywarn {
                display: inline-block;
                margin: 20px;
                color: #A8A8A8;
                font-size: 14px;
            }

        .paycode {
            text-align: center;
            margin: 20px;
        }

            .paycode .item {
                display: inline-block;
            }

        .red {
            color: red;
        }

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
    <script>
        var iCount = setInterval(check, 2000);  //每隔2秒执行一次check函数。

        function check() {
            var oids='<%=(string)Session["NativePayOids"]%>';
            $.ajax({
                type: "POST",
                //contentType: "application/json", data: { pid: pid, useAddress: useAddress, addressid: addressid, consignee: consignee, mobile: mobile, regionId: regionId, address: address }
                url: "/order/NativePayCheck",
                contentType: "application/x-www-form-urlencoded; charset=utf-8",
                data: { oidList: oids },
                //dataType: "json",
                success: function (data) {
                  
                    var json = eval("(" + data + ")");
                    //console.log(json.state);
                    //alert(json.state);
                    if (json.state == "success") {
                        clearInterval(iCount);
                        $(".weixin-sao").html("已成功付款");
                        $("#payQRImg").html('<img alt="" src="/images/wx_ok.jpg" width="200">');
                        ///order/ResultPay?oids=" + jsonMap["AdditionalInfo"] + "&paystatus=" + jsonMap["Result"]
                        $("#payQRwarn").html('<a href="/order/ResultPay?oids=<%=(string)Session["NativePayOids"]%>&paystatus=1" class="wx_ok"><span style="color:#fff">完成</span></a>');
                        window.location.href='/order/ResultPay?oids=<%=(string)Session["NativePayOids"]%>&paystatus=1';

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
</head>
<body>
    <div id="payTop" class="box">
        <a href="/" class="left">
            <img src="/images/logo_s.jpg" width="155" height="65" /></a>
        <h2>收银台</h2>
        <div class="clear"></div>
    </div>
    <div class="box wechatpay">
        <div class="wechatlogo">
            <img src="/images/WePayLogo.png" width="200" height="60" />
        </div>
        <div class="orderinfo" data-state="fail">
            <p class="ordermes">订单编号：<%=(string)Session["NativePayOSN"]%></p>
            <p class="ordermes">应付金额：<span class="red">¥<%=(string)Session["NativePayOPrice"]%></span></p>
            <br />
            <p class="paywarn">请您在提交订单后1小时内支付，以便订单尽快处理，否则订单会自动取消</p>
        </div>

        <div class="paycode">
            <div class="item">
                <div style="margin-left: 10px; color: #333; font-size: 18px; font-weight: bolder; text-align: center; margin: 5px;" class="weixin-sao">扫一扫付款</div>
                <div style="margin-left: 10px; color: red; font-size: 18px; font-weight: bolder; text-align: center;" class="weixin-money">¥<%=(string)Session["NativePayOPrice"]%></div>
                <br />
                <div id="payQRImg">
                    <img src="/webchat/<%=(string)Session["NativePayimageUrl"]%>" alt="" width="200" height="200" /><br />
                </div>
                <div id="payQRwarn">
                    <img src="/images/paywzsm.png" alt="" width="200" />

                </div>
            </div>
            <div class="item">
                <img src="/images/paytpts.jpg" alt="" />
            </div>
        </div>
    </div>

    <link href="/css/footer2016.css" rel="stylesheet" />
    <script src="/scripts/sidebar.js" type="text/javascript"></script>

    <!-- footer start -->
    <div class="footer">
        <!-- 快递服务部分 -->
        <div class="bkzf box1000">
            <ul class="bkzf-list ">
                <li>
                    <div class="footservice1">
                        <p class="bl-title">全国包邮</p>
                        <p class="bl-text">全国各大城市满99包邮</p>
                    </div>
                </li>
                <li>
                    <div class="Fleft footservice2">
                        <p class="bl-title">H-BOX</p>
                        <p class="bl-text">优质物流极速送达</p>
                    </div>
                </li>
                <li>
                    <div class="Fleft footservice3">
                        <p class="bl-title">正品保障</p>
                        <p class="bl-text">层层筛选购物无忧</p>
                    </div>
                </li>
                <li>
                    <div class="Fleft footservice4">
                        <p class="bl-title">售后无忧</p>
                        <p class="bl-text">7×24小时无休客服</p>
                    </div>
                </li>
            </ul>
            <div class="clear"></div>
        </div>

        <!-- 新手指引部分 -->
        <div class="xszy box1000">
            <ul class="xszy-list">
                <li>
                    <span>促销说明</span>
                        <p>
                            <a href="/help/question?id=16">优惠劵</a>
                        </p>
                        <p>
                            <a href="/help/question?id=15">促销活动</a>
                        </p>
                </li>
                <li>
                    <span>特色服务</span>
                        <p>
                            <a href="/help/question?id=24">全球购</a>
                        </p>
                        <p>
                            <a href="/help/question?id=26">全球购常见问答</a>
                        </p>
                </li>
                <li>
                    <span>购物指南</span>
                        <p>
                            <a href="/help/question?id=7">购物流程</a>
                        </p>
                        <p>
                            <a href="/help/question?id=8">推广流程</a>
                        </p>
                        <p>
                            <a href="/help/question?id=21">交易条款</a>
                        </p>
                        <p>
                            <a href="/help/question?id=22">相关咨询</a>
                        </p>
                </li>
                <li>
                    <span>配送方式</span>
                        <p>
                            <a href="/help/question?id=17">全球购购物送达</a>
                        </p>
                        <p>
                            <a href="/help/question?id=10">顺风蚁极速送达</a>
                        </p>
                </li>
                <li>
                    <span>支付方式</span>
                        <p>
                            <a href="/help/question?id=12">在线支付</a>
                        </p>
                        <p>
                            <a href="/help/question?id=25">支付相关</a>
                        </p>
                        <p>
                            <a href="/help/question?id=19">银行汇款</a>
                        </p>
                        <p>
                            <a href="/help/question?id=18">第三方支付</a>
                        </p>
                </li>
                <li>
                    <span>售后服务</span>
                        <p>
                            <a href="/help/question?id=13">售后政策</a>
                        </p>
                        <p>
                            <a href="/help/question?id=20">客服在线</a>
                        </p>
                </li>

        </ul>
            <div class="clear"></div>
        </div>

        <!-- 网站链接-备案 -->
        <div class="link-record">
            <!-- 链接 -->
            <div class="link">
                <a href="#">关于我们</a><span>|</span>
                <a href="/help/servicecenter">联系我们</a><span>|</span>
                <a href="#">人才招聘</a><span>|</span>
                <a href="#">商家入驻</a><span>|</span>
                <a href="#">营销中心</a><span>|</span>
                <a href="/mob">手机端</a><span>|</span>
                <a href="#">友情链接</a><span>|</span>
                <a href="#">新闻动态</a><span>|</span>
                <a href="#">网站导航</a><span>|</span>
                <a href="#">帮助中心</a><span>|</span>
                <a href="#">广告合作</a><span></span>
            </div>

            <!-- 备案 -->
            <div class="record">
                <p>网络ICP备案号：ICP备xxxxxxxxx号-1   xxxxxxxxxxxxxx有限公司 版权所有 Copyright© 2016-2018 www.xxxxxxxx.com. All Rights Reserved</p>
            </div>
            <div class="authentication">
                <a href="http://www.cyberpolice.cn/" target="_blank">
                    <img width="108" height="40" src="/images/foot3.png" class="footImg"></a>
                <%--<a href="javascript:void(0);" target="_blank">
                    <img width="108" height="40" src="/images/foot2.png" class="footImg"></a>
                <a href="javascript:void(0);" target="_blank">
                    <img width="108" height="40" src="/images/foot4.png" class="footImg"></a>--%>
                <a href="http://www.12377.cn/" target="_blank">
                    <img width="108" height="40" src="/images/foot1.png" class="footImg"></a>
                <%--<a href="http://www.miitbeian.gov.cn/" target="_blank">
                    <img width="108" height="40" src="/images/foot8.png" class="footImg"></a>--%>
                <%--<a href="http://kexin.knet.cn/" target="_blank">
                    <img width="108" height="40" src="http://www.hhwtop.com//images/foot9.png" class="footImg"></a>
                <a href="http://www.szfw.org/" target="_blank">
                    <img width="112" height="40" src="http://www.hhwtop.com//images/foot10.png" class="footImg"></a>--%>
            </div>
        </div>
    </div>
    <!-- footer end -->

</body>
</html>
