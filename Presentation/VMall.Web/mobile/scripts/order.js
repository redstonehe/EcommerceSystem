//获得配送地址列表
function getShipAddressList() {
    Ajax.get("/mob/ucenter/ajaxshipaddresslist", false, getShipAddressListResponse);
}

//处理获得配送地址列表的反馈信息
function getShipAddressListResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        var shipAddressList = "";
        for (var i = 0; i < result.content.count; i++) {
            shipAddressList += "<div class='adressI ship-list'><p>" + result.content.list[i].user + "</p><p class='f14'>" + result.content.list[i].address + "</p><div class='chooseAD'><input type='checkbox' class='radio' name='shipAddressItem' value='" + result.content.list[i].saId + "' onclick='selectShipAddress(" + result.content.list[i].saId + ")'/>送到这里去</div></div>";
        }
        shipAddressList += "<a href='javascript:openAddShipAddressBlock()' class='addAddress'>+添加收货地址</a>";
        document.getElementById("mainBlock").style.display = "none";
        document.getElementById("shipAddressListBlock").style.display = "";
        document.getElementById("shipAddressListBlock").innerHTML = shipAddressList;
    }
    else {
        alert(result.content);
    }
}

//选择配送地址
function selectShipAddress(saId) {
    document.getElementById("saId").value = saId;
    document.getElementById("confirmOrderForm").submit();
}

//打开添加配送地址块
function openAddShipAddressBlock() {
    document.getElementById("addShipAddressBlock").style.display = "";
}

//添加配送地址
function addShipAddress() {
    var addShipAddressForm = document.forms["addShipAddressForm"];

    var consignee = addShipAddressForm.elements["consignee"].value;
    var mobile = addShipAddressForm.elements["mobile"].value;
    var regionId = getSelectedOption(addShipAddressForm.elements["regionId"]).value;
    var address = addShipAddressForm.elements["address"].value;

    if (!verifyAddShipAddress(consignee, mobile, regionId, address)) {
        return;
    }

    Ajax.post("/mob/ucenter/addshipaddress",
            { 'consignee': consignee, 'mobile': mobile, 'regionId': regionId, 'address': address, 'isDefault': 1 },
            false,
            addShipAddressResponse)
}

//验证添加的收货地址
function verifyAddShipAddress(consignee, mobile, regionId, address) {
    if (consignee == "") {
        alert("请填写收货人");
        return false;
    }
    if (mobile == "") {
        alert("请填写手机号");
        return false;
    }
    if (parseInt(regionId) < 1) {
        alert("请选择区域");
        return false;
    }
    if (address == "") {
        alert("请填写详细地址");
        return false;
    }
    return true;
}

//处理添加配送地址的反馈信息
function addShipAddressResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        document.getElementById("saId").value = result.content;
        document.getElementById("confirmOrderForm").submit();
    }
    else {
        var msg = "";
        if (result.state == "full") {
            msg = result.content;
        }
        else
        {
            for (var i = 0; i < result.content.length; i++) {
                msg += result.content[i].msg + "\n";
            }
        }
        alert(msg)
    }
}

//显示支付插件列表
function showPayPluginList() {
    document.getElementById("mainBlock").style.display = "none";
    document.getElementById("payPluginListBlock").style.display = "";
}

//选择支付方式
function selectPayPlugin(paySystemName) {
    document.getElementById("payName").value = paySystemName;
    document.getElementById("confirmOrderForm").submit();
}

//选择汇购卡支付
function selectCashPay(type) {
    document.getElementById("selectCashPay").value = type;
    document.getElementById("confirmOrderForm").submit();
}

//获得有效的优惠劵列表
function getValidCouponList() {
    Ajax.get("/mob/order/getvalidcouponlist?selectedCartItemKeyList=" + document.getElementById("selectedCartItemKeyList").value + "&selectedStoreKeyList=" + document.getElementById("selectedStoreKeyList").value, false, getValidCouponListResponse);
}

//处理获得有效的优惠劵列表的反馈信息
function getValidCouponListResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        if (result.content.length < 1) {
            document.getElementById("validCouponList").innerHTML = "<div class=\"allCell\">此订单暂无可用的优惠券</div>";
            
            document.getElementById("validCouponList").style.display = "";
        }
        else {
            var itemList = "";
            itemList += "<div class=\"allCell\"><label><input type='checkbox' name='couponId' value='0' useMode='1' data-money= '0' onclick='useCoupon(this)'/> 不使用</div>";
            for (var i = 0; i < result.content.length; i++) {

                itemList += "<div class=\"allCell\"><input type='checkbox' name='couponId' value='" + result.content[i].couponId + "' useMode='" + result.content[i].useMode + "' data-money= '" + result.content[i].money + "' onclick='useCoupon(this)'/>" + result.content[i].money + "元</div>";
            }
            document.getElementById("validCouponList").innerHTML = itemList;
            document.getElementById("validCouponList").style.display = "";
        }
        //document.getElementById("mainBlock").style.display = "none";
        //document.getElementById("validCouponListBlcok").style.display = "";
    }
    else {
        alert(result.content);
    }
}

//检查优惠劵的使用模式
function checkCouponUseMode(obj) {
    if (obj.getAttribute("checked") == "true") {
        obj.setAttribute("checked", "false");
        obj.className = "radio";
    }
    else {
        var useMode = obj.getAttribute("useMode");
        if (useMode == "1") {
            var checkboxList = document.getElementById("validCouponList").getElementsByTagName("span");
            for (var i = 0; i < checkboxList.length; i++) {
                checkboxList[i].setAttribute("checked", "false");
                checkboxList[i].className = "radio";
            }
        }
        obj.setAttribute("checked", "true");
        obj.className = "radio checked";
    }
}

//确认选择的优惠劵
function confirmSelectedCoupon() {
    var couponList = "";
    var couponIdCheckboxList = document.getElementById("validCouponList").getElementsByTagName("span");
    for (var i = 0; i < couponIdCheckboxList.length; i++) {
        if (couponIdCheckboxList[i].getAttribute("checked") == "true") {
            couponList += "<div class='sell'><i>惠</i>" + couponIdCheckboxList[i].getAttribute("text") + "</div>";
        }
    }
    document.getElementById("selectCouponList").innerHTML = couponList;
    document.getElementById("mainBlock").style.display = "";
    document.getElementById("validCouponListBlcok").style.display = "none";
}

//提交订单
function submitOrder() {
    if ($(".agreesign").length > 0) {
        if (!$(".agreesign").is(':checked')) {
            alert("请先阅读并同意电子签约协议");
            return;
        }
    }
    var selectedCartItemKeyList = document.getElementById("selectedCartItemKeyList").value;
    var selectedStoreKeyList = document.getElementById("selectedStoreKeyList").value;
    var saId = document.getElementById("saId").value;
    var payName = document.getElementById("payName").value;
    var payCreditCount = document.getElementById("payCreditCount") && document.getElementById("payCreditCount").checked == "checked" ? document.getElementById("payCreditCount").value : 0;

    var couponIdList = "";
    var couponIdCheckboxList = document.getElementById("validCouponList").getElementsByTagName("input");
    for (var i = 0; i < couponIdCheckboxList.length; i++) {
        if (couponIdCheckboxList[i].checked == true) {
            couponIdList += couponIdCheckboxList[i].value + ",";
        }
    }
    if (couponIdList.length > 0)
        couponIdList = couponIdList.substring(0, couponIdList.length - 1);
    //发票信息
    var invoice = $("#invoice") ? $("#invoice").val() : "";
    var invoice_title = "";
    var invoice_id = "";
    var invoice_regaddr = "";
    var invoice_regmobile = "";
    var invoice_bank = "";
    var invoice_bankno = "";
    var invoicetype = $(".ul-type .invoice-item-selected").val();
    if (invoicetype == 0 || invoicetype == 1) {
        var invoicebody = $(".ul-body .invoice-item-selected");
        var invoicebodyvalue = invoicebody.val();
        if (invoicebody.length <= 0) {
            alert("选择开票主体");
            return;
        }
        invoice_title = $.trim($(".invoice-title").val());
        invoice_id = $.trim($(".invoice-id").val());
        invoice_regaddr = $.trim($(".invoice-regaddr").val());
        invoice_regmobile = $.trim($(".invoice-regmobile").val());
        invoice_bank = $.trim($(".invoice-bank").val());
        invoice_bankno = $.trim($(".invoice-bankno").val());
        if (invoicebody.val() == 0) {
            if (invoice_title == "") {
                alert("填写抬头");
                return;
            }
        }
        else if (invoicebody.val() == 1) {
            if (invoice_title == "" || invoice_id == "" || invoice_regaddr == "" || invoice_regmobile == "" || invoice_bank == "" || invoice_bankno == "") {
                alert("填写完整的发票信息");
                return;
            }
        }
    }
    var haiMiCount = $("#haiMiCount") ? $("#haiMiCount").val() : 0;
    var hongBaoCount = $("#hongBaoCount") ? $("#hongBaoCount").val() : 0;

    //代理、佣金账户
    var daiLiCount = $("#daiLiCount") ? $("#daiLiCount").val() : 0;
    var YongJinCount = $("#YongJinCount") ? $("#YongJinCount").val() : 0;

    //汇购卡券
    var isUserCash = $("#cashcouponCard").is(':checked');
    var selectCashPay = $("#selectCashPay").val();
    var paypswd = $.trim($("#paypassword").val());
    var cashCount = $("#CashCount") && isUserCash ? $("#CashCount").val() : 0;
    if (isUserCash && cashCount <= 0) {
        alert("当前已选择使用汇购卡支付，请输入使用数额");
        return;
    }
    if (isUserCash && cashCount > 0) {
        if (paypswd == "") {
            alert("支付密码不能为空");
            return;
        }
    }

    var cashId = $("input[name='cashId']:checked").length > 0 && isUserCash ? $("input[name='cashId']:checked").val() : 0;

    var selectCash = $("input[name='cashId']:checked");
    var selectcashIds = "";
    for (var i = 0; i < selectCash.length; i++) {
        if (selectCash[i].checked == true) {
            selectcashIds += $("input[name='cashId']:checked")[i].value + ",";
        }
    }
    if (selectCash.length > 0) {
        selectcashIds = selectcashIds.substring(0, selectcashIds.length - 1);
    }

    var allFullCut = document.getElementById("fullCut") ? document.getElementById("fullCut").value : 0;
    var buyerRemark = document.getElementById("buyerRemark") ? document.getElementById("buyerRemark").value : "";
    var isidcard = $("#isuseridcard").val();
    var idcard = $.trim($("#idcard").val());
    if (!verifySubmitOrder(saId, payName, buyerRemark)) {
        return;
    }
    //if (hongbaoCheck == "1") {
    //    alert("超过红包限额");
    //    return;
    //}

    //兑换码
    var selectexcode = $("input[name='excodeid']:checked");
    var selectexIds = "";
    for (var i = 0; i < selectexcode.length; i++) {
        if (selectexcode[i].checked == true) {
            selectexIds += $("input[name='excodeid']:checked")[i].value + ",";
        }
    }
    if (selectexcode.length > 0) {
        selectexIds = selectexIds.substring(0, selectexIds.length - 1);
    }

    //var exchangecode = $(".exchangecode") ? $.trim($(".exchangecode").val()) : "";
    if (isUseExCode == "1" && selectexIds == "") {
        alert("请选择兑换码");
        return;
    }
    var exselect = $("#exselectBlock").length ? 1 : 0;
    var exselectname = $("input[name='exselectname']:checked").length ? $("input[name='exselectname']:checked").val() : "";
    if (exselect == 1 && exselectname == "") {
        alert("请选择兑换码所需具体信息，如城市或书籍种类");
        return;
    }
    if (isUserIdCard == "1" && idcard == "") {
        alert("身份证信息不能为空");
        return;
    }
    var reg = /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/;
    if (isUserIdCard == "1" && reg.test(idcard) === false) {
        alert("身份证格式不正确,请重新输入");
        return;
    }

    //防重复提交
    $("#addorder").html("正在提交...").css("color", "red").attr("href", "javascript:void(0);");
    Ajax.post("/mob/order/submitorder",
            { 'selectedCartItemKeyList': selectedCartItemKeyList, 'selectedStoreKeyList': selectedStoreKeyList, 'saId': saId, 'payName': payName, 'payCreditCount': payCreditCount, 'haiMiCount': haiMiCount, 'hongBaoCount': hongBaoCount, 'couponIdList': couponIdList, 'fullCut': allFullCut, 'buyerRemark': buyerRemark, 'invoice': invoice, 'isuseridcard': isidcard, 'idcard': idcard, "cashCount": cashCount, "cashId": selectcashIds, "payPswd": paypswd, "selectCashPay": selectCashPay, "selectexIds": selectexIds, "exselectname": exselectname, "daiLiCount": daiLiCount, "YongJinCount": YongJinCount, "invoice_title": invoice_title, "invoice_id": invoice_id, "invoice_regaddr": invoice_regaddr, "invoice_regmobile": invoice_regmobile, "invoice_bank": invoice_bank, "invoice_bankno": invoice_bankno, "invoicetype": invoicetype, "invoicebodyvalue": invoicebodyvalue },
            false,
            submitOrderResponse)
}

//验证提交订单
function verifySubmitOrder(saId, payName, buyerRemark) {
    if (saId < 1) {
        alert("请填写收货人信息");
        return false;
    }
    if (payName.length < 1) {
        alert("配送方式不能为空");
        return false;
    }
    if ($.trim(buyerRemark).length > 125) {
        alert("最多只能输入125个字");
        return false;
    }
    return true;
}

//处理提交订单的反馈信息
function submitOrderResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state != "success") {
        alert(result.content);
    }
    else {
        window.location.href = result.content;
    }
}


function usesign() {
    var diglog = $("#usesigningDiv").html();
    layer.open({
        type: 1,
        title: '电子协议',
        area: ['500px', '300px'],
        shadeClose: true, //点击遮罩关闭
        content: diglog// diglog//$("#header")
    });
}
function closeusesign() {
    layer.close(layer.index);
}