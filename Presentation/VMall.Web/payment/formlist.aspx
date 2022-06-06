<!DOCTYPE html>
<html>

    <head>
        <meta charset="utf-8" />
        <title>在线支付</title>
        <link rel="stylesheet" href="/css/style.css" type="text/css" />
        <script src="/scripts/jquery.js" type="text/javascript"></script>
    </head>

    <body>
        <section class="top">
            <!--<a href="#">
                    <img src="img/logo.gif" />
            </a>-->
            <h1>在线支付</h1>
        </section>
        <form method="post" action="/zdipay/submitpay" name="payment">
        <section class="fund">
            <label>金额：</label>
            <input type="hidden" name="MemberID" value="10001">
            <input type="hidden" name="TransID" value="">
            <input type="hidden" name="oidList" value="<%=(string)Session["oidList"]%>">
            <div class="fund_money">
                <input name="OrderMoney" type="text" value="<%=(Decimal)Session["allSurplusMoney"]%>" disabled="disabled" />
            </div>
        </section>
        <section class="paypal">
            <div class="paypal_header">
                <label>请选择支付方式：</label>
                <div id="caidan1" onClick="showDiv(1)" class="active">储蓄卡</div>
            </div>
            <div style="clear: both;"></div>
            <div class="bankList" id="kqh_neirong1">
                <label>网银支付<span>需开通网银</span>
                </label>
                <ul>
                    <li>
                        <label for="b01">
                            <a href="#">
                                <input type="radio" name="PayID" value="3002" class="banking" id="bankradio_ICBC">
                                <img src="/images/img/1002.jpg" alt="中国工商银行">
                            </a>
                        </label>
                    </li>
                    <li>
                        <label for="b04">
                            <a href="#">
                                <input type="radio" name="PayID" value="3003" class="banking" id="bankradio_CCB">
                                <img src="/images/img/1003.jpg" alt="中国建设银行">
                            </a>
                        </label>
                    </li>
                    <li>
                        <label for="b03">
                            <a href="#">
                                <input type="radio" name="PayID" value="3005" class="banking" id="bankradio_ABC">
                                <img src="/images/img/1005.jpg" alt="中国农业银行">
                            </a>
                        </label>
                    </li>
                    <li>
                        <label for="b02">
                            <a href="#">
                                <input type="radio" name="PayID" value="3001" class="banking" id="bankradio_CMB">
                                <img src="/images/img/1001.jpg" alt="招商银行">
                            </a>
                        </label>
                    </li>
                    <li>
                        <label for="b05">
                            <a href="#">
                                <input type="radio" name="PayID" value="3026" class="banking" id="bankradio_BOC">
                                <img src="/images/img/1026.jpg" alt="中国银行">
                            </a>
                        </label>
                    </li>
                    <li>
                        <label for="b14_080910">
                            <a href="#">
                                <input type="radio" name="PayID" value="3004" class="banking" id="bankradio_SPDB">
                                <img src="/images/img/1004.jpg" alt="上海浦东发展银行">
                            </a>
                        </label>
                    </li>
                    <li>
                        <label for="b06">
                            <a href="#">
                                <input type="radio" name="PayID" value="3020" class="banking" id="bankradio_BCOM">
                                <img src="/images/img/1020.jpg" alt="交通银行">
                            </a>
                        </label>
                    </li>
                    <li>
                        <label for="b07">
                            <a href="#">
                                <input type="radio" name="PayID" value="3006" class="banking" id="bankradio_CMBC">
                                <img src="/images/img/1006.jpg" alt="中国民生银行">
                            </a>
                        </label>
                    </li>
                    <li>
                        <label for="b13">
                            <a href="#">
                                <input type="radio" name="PayID" value="3009" class="banking" id="bankradio_CIB">
                                <img src="/images/img/1009.jpg" alt="兴业银行">
                            </a>
                        </label>
                    </li>
                    <li>
                        <label for="b_ceb">
                            <a href="#">
                                <input type="radio" name="PayID" value="3022" class="banking" id="bankradio_CEB">
                                <img src="/images/img/1022.jpg" alt="光大银行">
                            </a>
                        </label>
                    </li>
                    <li>
                        <label for="b12">
                            <a href="#">
                                <input type="radio" name="PayID" value="3039" class="banking" id="bankradio_CITIC">
                                <img src="/images/img/1039.jpg" alt="中信银行">
                            </a>
                        </label>
                    </li>
                    <li>
                        <label for="bbank_psbc">
                            <a href="#">
                                <input type="radio" name="PayID" value="3038" class="banking" id="bankradio_PSBC">
                                <img src="/images/img/1038.jpg" alt="中国邮政储蓄银行">
                            </a>
                        </label>
                    </li>
                    <li>
                        <label for="b_pab">
                            <a href="#">
                                <input type="radio" name="PayID" value="3035" class="banking" id="bankradio_PAB">
                                <img src="/images/img/1035.jpg" alt="平安银行">
                            </a>
                        </label>
                    </li>
                    <li>
                        <label for="b10">
                            <a href="#">
                                <input type="radio" name="PayID" value="3036" class="banking" id="bankradio_SDB">
                                <img src="/images/img/1036.jpg" alt="深圳发展银行">
                            </a>
                        </label>
                    </li>
                    <li>
                        <label for="b19">
                            <a href="#">
                                <input type="radio" name="PayID" value="3032" class="banking" id="bankradio_BOB">
                                <img src="/images/img/1032.jpg" alt="北京银行">
                            </a>
                        </label>
                    </li>
                    <li>
                        <label for="b_hsb">
                            <a href="#">
                                <input type="radio" name="PayID" value="3037" class="banking" id="bankradio_HSB">
                                <img src="/images/img/1037.jpg" alt="上海农商银行">
                            </a>
                        </label>
                    </li>
                    <li>
                        <label for="b_shb">
                            <a href="#">
                                <input type="radio" name="PayID" value="3060" class="banking" id="bankradio_SHB">
                                <img src="/images/img/1061.jpg" alt="北京农商银行">
                            </a>
                        </label>
                    </li>
                </ul>
            </div>
        </section>
            <section>
                <div>
                    <input name="TradeDate" type="hidden" class="input" size="30" value="" />
                </div>
 
                <div>
                    <input name="ProductName" type="hidden" class="input" size="30" value="貂皮大衣" />
                </div>
                <div>
                    <input name="ReturnUrl" type="hidden"  class="input" size="60" value="" />
                    <input name="NotifyUrl" type="hidden"  class="input" size="60" value="" />
                </div>
                <%--<div>备注</div>
                <div><input name="AdditionalInfo" type="text" class="input" size="50" value="" /></div>--%>
            </section>
            <input type="button" name="submitpay" value="确认付款" class="submit_button" onclick=" return submitPay();">
        </form>
        <script type="text/javascript">
            function showDiv(n) {
                for (i = 1; i <= 3; i++) {
                    var caidan = document.getElementById('caidan' + i);
                    var kqh_neirong = document.getElementById('kqh_neirong' + i);
                    caidan.className = i == n ? "active" : "";
                    kqh_neirong.style.display = i == n ? "block" : "none";
                }
            }
            //$("input[name='items']:checked").val();
            function submitPay() {
                if ($("input[name='PayID']:checked").val() == null || $("input[name='PayID']:checked").val() == undefined) {
                    alert("请选择支付银行")
                    return false;
                }
                $("form").submit();
            }
        </script>

    </body>

</html>
