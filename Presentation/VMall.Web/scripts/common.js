var uid = -1; //用户id
var isGuestSC = 0; //是否允许游客使用购物车(0代表不可以，1代表可以)
var scSubmitType = 0; //购物车的提交方式(0代表跳转到提示页面，1代表跳转到列表页面，2代表ajax提交)

//商城搜索
function mallSearch(keyword) {
    if (keyword == undefined || keyword == null || keyword.length < 1) {
        alert("请输入关键词");
    }
    else {
        window.location.href = "/catalog/search?keyword=" + encodeURIComponent(keyword);
    }
}

//店铺搜索
function storeSearch(storeId, keyword, storeCid, startPrice, endPrice) {
    if (storeId < 1) {
        alert("请先选择店铺");
    }
    else if ((keyword == undefined || keyword == null || keyword.length < 1) && storeCid < 1 && (startPrice == undefined || startPrice == null || startPrice.length < 1) && (endPrice == undefined || endPrice == null || endPrice.length < 1)) {
        //alert("请输入搜索条件");
        window.location.href = "/store/index?storeId=" + storeId;
    }
    else {
        window.location.href = "/store/search?storeId=" + storeId + "&keyword=" + encodeURIComponent(keyword) + "&storeCid=" + storeCid + "&startPrice=" + startPrice + "&endPrice=" + endPrice;
    }
}

//获得购物车快照
var isAlreadyLoadCartSnap = false;
function getCartSnap() {
    if (isGuestSC == 0 && uid < 1) {
        return;
    }
    var cartSnap = document.getElementById("cartSnap");
    cartSnap.style.display = "";
    if (!isAlreadyLoadCartSnap) {
        isAlreadyLoadCartSnap = true;
        Ajax.get("/cart/snap", false, function (data) {
            getCartSnapResponse(data);
        })
    }
}

//处理获得购物车快照的反馈信息
function getCartSnapResponse(data) {
    var cartSnap = document.getElementById("cartSnap");
    try {
        var result = eval("(" + data + ")");
        alert(result.content);
    }
    catch (ex) {
        cartSnap.innerHTML = data;
        document.getElementById("cartSnapProudctCount").innerHTML = document.getElementById("csProudctCount").innerHTML;
    }
}

//关闭购物车快照
function closeCartSnap(event) {
    if (Browser.isFirefox && document.getElementById('cartSnapBox').contains(event.relatedTarget)) return;
    var cartSnap = document.getElementById("cartSnap");
    cartSnap.style.display = "none";
}

//添加商品到收藏夹
function addProductToFavorite(pid) {
    if (pid < 1) {
        alert("请选择商品");
    }
    else if (uid < 1) {
        alert("请先登录");
        window.location.href = "/account/login?returnUrl=" + window.location.href;
    }
    else {
        Ajax.get("/ucenter/addproducttofavorite?pid=" + pid, false, addProductToFavoriteResponse)
    }
}

//处理添加商品到收藏夹的反馈信息
function addProductToFavoriteResponse(data) {
    var result = eval("(" + data + ")");
    alert(result.content);
}

//添加店铺到收藏夹
function addStoreToFavorite(storeId) {
    if (storeId < 1) {
        alert("请选择店铺");
    }
    else if (uid < 1) {
        alert("请先登录");
    }
    else {
        Ajax.get("/ucenter/addstoretofavorite?storeId=" + storeId, false, addStoreToFavoriteResponse)
    }
}

//处理添加店铺到收藏夹的反馈信息
function addStoreToFavoriteResponse(data) {
    var result = eval("(" + data + ")");
    alert(result.content);
}

//添加商品到购物车
function addProductToCart(pid, buyCount) {
    if (pid < 1) {
        alert("请选择商品");
    }
    else if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
    }
    else if (buyCount < 1) {
        alert("请填写购买数量");
    }
    else if (scSubmitType != 2) {
        window.location.href = "/cart/addproduct?pid=" + pid + "&buyCount=" + buyCount;
    }
    else {
        Ajax.get("/cart/addproduct?pid=" + pid + "&buyCount=" + buyCount, false, addProductToCartResponse)
    }
}
//添加商品到购物车--二次确认信息
function addProductToCartWithConfirm(pid, buyCount) {
    var type1 = $(".choose dd").find("a").eq(0).html();
    var type2 = $(".choose dd").find("a").eq(1).html();
    var type_pid_1 = $(".choose dd").find("a").eq(0).attr("data-pid");
    var type_pid_2 = $(".choose dd").find("a").eq(1).attr("data-pid");
    //alert(11);
    //var diglog = '<div class="layer_1"><h2>请再次确认购买的产品用途</h2></div>';
    //diglog += '<div class="layer_2">';
    //diglog += '<span class="span_left" onclick="select_type(this);" data-pid="' + type_pid_1 + '">' + type1 + '</span>';
    //diglog += '<span class="span_right" onclick="select_type(this);" data-pid="' + type_pid_2 + '">' + type2 + '</span>';
    //diglog += '</div>';
    //diglog += '<div class="layer_3">';
    //diglog += "<a onclick='submit_confirm();'>确定</a>";
    //diglog += "</div>";
    //diglog += '<div class="layer_4"><h2>请选择购买的产品用途</h2></div>';
    $(".layer_2 .span_left").attr("data-pid", type_pid_1).html(type1);
    $(".layer_2 .span_right").attr("data-pid", type_pid_2).html(type2);
    if (pid < 1) {
        alert("请选择商品");
    }
    else if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
    }
    else if (buyCount < 1) {
        alert("请填写购买数量");
    }

    layer.open({
        type: 1,
        title: '确认',
        area: ['600px', '600px'],
        shadeClose: true, //点击遮罩关闭
        content: $("#showMsg")//diglog//$("#header")
    });

}
function select_type(ele) {
    $(".layer_4").hide();
    $(".layer_2 span").removeClass("hottest");
    $(ele).addClass("hottest");
    var typetitle=$(ele).html();
    //alert(typetitle)
    if (typetitle.indexOf("代理")>-1) {
        $(".layer_5").show();
        $(".layer_6").show();
    }
    if (typetitle.indexOf("零售")>-1) {
        $(".layer_5").hide();
        $(".layer_6").hide();
    }
}
function select_suittype(ele) {
    $(".layer_4").hide();
    $(".layer_5 span").removeClass("hottest");
    $(ele).addClass("hottest");
}
//提交加入购物车
function submit_confirm() {
    if ($(".layer_2 .hottest").length <= 0) {
        $(".layer_4").show();
        return;
    }
    var suitpid = 0;
    var typetitle = $(".layer_2 .hottest").html();
    //alert(typetitle);
    var agenttype =parseInt($(".agenttype").val());
    if (typetitle.indexOf("代理") > -1) {
        var selectsuit = $(".layer_5 .hottest");
        if (selectsuit.length <= 0 && agenttype<=0) {
            $(".layer_4 h2").html("请选择套餐包类型");
            $(".layer_4").show();
            return;
        }
        var selectsuitpid = selectsuit.attr("data-pid");
        //alert(selectsuitpid);
        suitpid = selectsuitpid;
    }
    var pid = $(".layer_2 .hottest").attr("data-pid");
    var buyCount = $("#buyCount").val();
   // alert(pid);
    if (scSubmitType != 2) {
        window.location.href = "/cart/addproduct?pid=" + pid + "&buyCount=" + buyCount + "&suitpid=" + suitpid;
    }
    else {
        Ajax.get("/cart/addproduct?pid=" + pid + "&buyCount=" + buyCount + "&suitpid=" + suitpid, false, addProductToCartResponse)
    }
    
}
//直接购买商品--二次确认信息
function directBuyProductWithConfirm(pid, buyCount) {
    var type1 = $(".choose dd").find("a").eq(0).html();
    var type2 = $(".choose dd").find("a").eq(1).html();
    var type_pid_1 = $(".choose dd").find("a").eq(0).attr("data-pid");
    var type_pid_2 = $(".choose dd").find("a").eq(1).attr("data-pid");
    //alert(11);
    //var diglog = '<div style="padding:20px;text-align: center;" class="layer_1"><h2>请再次确认购买的产品用途</h2></div>';
    //diglog += '<div style="padding:20px;text-align: center;width:60%;margin:0 auto;" class="layer_2">';
    //diglog += '<span style="" onclick="select_type(this);" data-pid="' + type_pid_1 + '">' + type1 + '</span>';
    //diglog += '<span style="" onclick="select_type(this);" data-pid="' + type_pid_2 + '">' + type2 + '</span>';
    //diglog += '</div>';
    //diglog += '<div style="padding:20px;text-align: center;  margin-top:30px;" class="layer_3">';
    //diglog += "<a style='display:inline-block;width:200px;height:50px;line-height:50px;background:#E94820;color:#fff;font-size:20px;cursor: pointer;' onclick='submit_directbuy();'>确定</a>";
    //diglog += "</div>";
    //diglog += '<div style="padding:20px;text-align: center;color:red;display:none;" class="layer_4"><h2>请选择购买的产品用途</h2></div>';
    $(".layer_2 .span_left").attr("data-pid", type_pid_1).html(type1);
    $(".layer_2 .span_right").attr("data-pid", type_pid_2).html(type2);
    if (pid < 1) {
        alert("请选择商品");
    }
    else if (uid < 1) {
        window.location.href = "/account/login?returnUrl=/cart/directbuyproduct?proid=" + pid + "_" + buyCount;
        //alert("请先登录");
    }
    else if (buyCount < 1) {
        alert("请填写购买数量");
    }
    layer.open({
        type: 1,
        title: '确认',
        area: ['600px', '600px'],
        shadeClose: true, //点击遮罩关闭
        content:$("#showMsg2")// diglog//$("#header")
    });

}
//提交加入立即购买
function submit_directbuy() {
    if ($(".layer_2 .hottest").length <= 0) {
        $(".layer_4").show();
        return;
    }
    var suitpid = 0;
    var typetitle = $(".layer_2 .hottest").html();
    //alert(typetitle);
    var agenttype = parseInt($(".agenttype").val());
    if (typetitle.indexOf("代理") > -1) {
        var selectsuit = $(".layer_5 .hottest");
        if (selectsuit.length <= 0 && agenttype <= 0) {
            $(".layer_4 h2").html("请选择套餐包类型");
            $(".layer_4").show();
            return;
        }
        var selectsuitpid = selectsuit.attr("data-pid");
        //alert(selectsuitpid);
        suitpid = selectsuitpid;
    }
    var pid = $(".layer_2 .hottest").attr("data-pid");
    var buyCount = $("#buyCount").val();
    //alert(pid);
    Ajax.get("/cart/directbuyproduct?pid=" + pid + "&buyCount=" + buyCount + "&suitpid=" + suitpid, false, directBuyProductResponse)

}
//处理添加商品到购物车的反馈信息
function addProductToCartResponse(data) {
    var result = eval("(" + data + ")");
    alert(result.content);
}

//直接购买商品
function directBuyProduct(pid, buyCount) {
    if (pid < 1) {
        alert("请选择商品");
    }
    else if (uid < 1) {
        window.location.href = "/account/login?returnUrl=/cart/directbuyproduct?proid=" + pid + "_" + buyCount;
        //alert("请先登录");
    }
    else if (buyCount < 1) {
        alert("请填写购买数量");
    }
    else {
        Ajax.get("/cart/directbuyproduct?pid=" + pid + "&buyCount=" + buyCount, false, directBuyProductResponse)
    }
}

//处理直接购买商品的反馈信息
function directBuyProductResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        window.location.href = result.content;
    }
    else {
        alert(result.content);
    }
}

//添加套装到购物车
function addSuitToCart(pmId, buyCount) {
    if (pmId < 1) {
        alert("请选择套装");
    }
    else if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
    }
    else if (buyCount < 1) {
        alert("请填写购买数量");
    }
    else if (scSubmitType != 2) {
        window.location.href = "/cart/addsuit?pmId=" + pmId + "&buyCount=" + buyCount;
    }
    else {
        Ajax.get("/cart/addsuit?pmId=" + pmId + "&buyCount=" + buyCount, false, addSuitToCartResponse)
    }
}

//处理添加套装到购物车的反馈信息
function addSuitToCartResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state != "stockout") {
        alert(result.content);
    }
    else {
        alert("商品库存不足");
    }
}

//直接购买套装
function directBuySuit(pmId, buyCount) {
    if (pmId < 1) {
        alert("请选择套装");
    }
    else if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
    }
    else if (buyCount < 1) {
        alert("请填写购买数量");
    }
    else {
        Ajax.get("/cart/directbuysuit?pmId=" + pmId + "&buyCount=" + buyCount, false, directBuySuitResponse)
    }
}

//处理直接购买套装的反馈信息
function directBuySuitResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        window.location.href = result.content;
    }
    else {
        alert(result.content);
    }
}

//获得选中的购物车项键列表
function getSelectedCartItemKeyList() {
    var inputList = document.getElementById("cartBody").getElementsByTagName("input");

    var valueList = new Array();
    for (var i = 0; i < inputList.length; i++) {
        if (inputList[i].type == "checkbox" && inputList[i].name == "cartItemCheckbox" && inputList[i].checked) {
            valueList.push(inputList[i].value);
        }
    }

    if (valueList.length < 1) {
        //当取消全部商品时,添加一个字符防止商品全部选中
        return "_";
    }
    else {
        return valueList.join(',');
    }
}

//设置批量选择购物车项复选框
function setBatchSelectCartItemCheckbox() {
    var inputList = document.getElementById("cartBody").getElementsByTagName("input");

    var flag = true;
    for (var i = 0; i < inputList.length; i++) {
        if (inputList[i].type == "checkbox" && inputList[i].name == "cartItemCheckbox" && !inputList[i].checked) {
            document.getElementById("storeCartCheckbox" + inputList[i].getAttribute("storeId")).checked = false;
            flag = false;
        }
    }

    if (flag) {
        document.getElementById("selectAllBut_top").checked = true;
        document.getElementById("selectAllBut_bottom").checked = true;
    }
    else {
        document.getElementById("selectAllBut_top").checked = false;
        document.getElementById("selectAllBut_bottom").checked = false;
    }
}

//删除购物车中商品
function delCartProduct(pid, pos) {
    if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
        return;
    }
    if (pos == 0) {
        Ajax.get("/cart/delpruduct?pid=" + pid + "&pos=" + pos + "&selectedCartItemKeyList=" + getSelectedCartItemKeyList(), false, function (data) {
            try {
                alert(val("(" + data + ")").content);
            }
            catch (ex) {
                document.getElementById("cartBody").innerHTML = data;
                setBatchSelectCartItemCheckbox();
            }
        })
    }
    else {
        Ajax.get("/cart/delpruduct?pid=" + pid + "&pos=" + pos, false, function (data) {
            try {
                alert(val("(" + data + ")").content);
            }
            catch (ex) {
                document.getElementById("cartSnap").innerHTML = data;
                document.getElementById("cartSnapProudctCount").innerHTML = document.getElementById("csProudctCount").innerHTML;
            }
        })
    }
}

//批量删除购物车中选中的商品
function batchDelCartProduct(pids, pos) {
    if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
        return;
    }
    if (pos == 0) {
        Ajax.get("/cart/BatchDelPruduct?pids=" + pids + "&pos=" + pos + "&selectedCartItemKeyList=" + getSelectedCartItemKeyList(), false, function (data) {
            try {
                alert(val("(" + data + ")").content);
            }
            catch (ex) {
                document.getElementById("cartBody").innerHTML = data;
                setBatchSelectCartItemCheckbox();
            }
        })
    }
    else {
        Ajax.get("/cart/BatchDelPruduct?pids=" + pids + "&pos=" + pos, false, function (data) {
            try {
                alert(val("(" + data + ")").content);
            }
            catch (ex) {
                document.getElementById("cartSnap").innerHTML = data;
                document.getElementById("cartSnapProudctCount").innerHTML = document.getElementById("csProudctCount").innerHTML;
            }
        })
    }
}

//删除购物车中套装
function delCartSuit(pmId, pos) {
    if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
        return;
    }
    if (pos == 0) {
        Ajax.get("/cart/delsuit?pmId=" + pmId + "&pos=" + pos + "&selectedCartItemKeyList=" + getSelectedCartItemKeyList(), false, function (data) {
            try {
                alert(eval("(" + data + ")").content);
            }
            catch (ex) {
                document.getElementById("cartBody").innerHTML = data;
                setBatchSelectCartItemCheckbox();
            }
        })
    }
    else {
        Ajax.get("/cart/delsuit?pmId=" + pmId + "&pos=" + pos, false, function (data) {
            try {
                alert(eval("(" + data + ")").content);
            }
            catch (ex) {
                document.getElementById("cartSnap").innerHTML = data;
                document.getElementById("cartSnapProudctCount").innerHTML = document.getElementById("csProudctCount").innerHTML;
            }
        })
    }
}

//删除购物车中满赠
function delCartFullSend(pmId, pos) {
    if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
        return;
    }
    if (pos == 0) {
        Ajax.get("/cart/delfullsend?pmId=" + pmId + "&pos=" + pos + "&selectedCartItemKeyList=" + getSelectedCartItemKeyList(), false, function (data) {
            try {
                alert(eval("(" + data + ")").content);
            }
            catch (ex) {
                document.getElementById("cartBody").innerHTML = data;
                setBatchSelectCartItemCheckbox();
            }
        })
    }
    else {
        Ajax.get("/cart/delfullsend?pmId=" + pmId + "&pos=" + pos, false, function (data) {
            try {
                alert(eval("(" + data + ")").content);
            }
            catch (ex) {
                document.getElementById("cartSnap").innerHTML = data;
                document.getElementById("cartSnapProudctCount").innerHTML = document.getElementById("csProudctCount").innerHTML;
            }
        })
    }
}

//清空购物车
function clearCart(pos) {
    if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
        return;
    }
    Ajax.get("/cart/clear?pos=" + pos, false, function (data) {
        try {
            alert(eval("(" + data + ")").content);
        }
        catch (ex) {
            if (pos == 0) {
                document.getElementById("cartBody").innerHTML = data;
            }
            else {
                document.getElementById("cartSnap").innerHTML = data;
                document.getElementById("cartSnapProudctCount").innerHTML = "0";
            }
        }
    })
}

//改变商品数量
function changePruductCount(pid, buyCount) {
    if (!isInt(buyCount)) {
        alert('请输入数字');
    }
    else if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
    }
    else {
        var key = "0_" + pid;
        var inputList = document.getElementById("cartBody").getElementsByTagName("input");
        for (var i = 0; i < inputList.length; i++) {
            if (inputList[i].type == "checkbox" && inputList[i].value == key) {
                inputList[i].checked = true;
                break;
            }
        }
        Ajax.get("/cart/changepruductcount?pid=" + pid + "&buyCount=" + buyCount + "&selectedCartItemKeyList=" + getSelectedCartItemKeyList(), false, function (data) {
            try {
                alert(eval("(" + data + ")").content);
            }
            catch (ex) {
                document.getElementById("cartBody").innerHTML = data;
                setBatchSelectCartItemCheckbox();
            }
        })
    }
}

//改变套装数量
function changeSuitCount(pmId, buyCount) {
    if (!isInt(buyCount)) {
        alert('请输入数字');
    }
    else if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
    }
    else {
        var key = "1_" + pmId;
        var inputList = document.getElementById("cartBody").getElementsByTagName("input");
        for (var i = 0; i < inputList.length; i++) {
            if (inputList[i].type == "checkbox" && inputList[i].value == key) {
                inputList[i].checked = true;
                break;
            }
        }
        Ajax.get("/cart/changesuitcount?pmId=" + pmId + "&buyCount=" + buyCount + "&selectedCartItemKeyList=" + getSelectedCartItemKeyList(), false, function (data) {
            try {
                alert(eval("(" + data + ")").content);
            }
            catch (ex) {
                document.getElementById("cartBody").innerHTML = data;
                setBatchSelectCartItemCheckbox();
            }
        })
    }
}

//获取满赠商品
function getFullSend(pmId) {
    if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
        return;
    }
    Ajax.get("/cart/getfullsend?pmId=" + pmId, false, function (data) {
        getFullSendResponse(data, pmId);
    })
}

//处理获取满赠商品的反馈信息
var selectedFullSendPid = 0;
function getFullSendResponse(data, pmId) {
    var result = eval("(" + data + ")");
    if (result.state != "success") {
        alert(result.content);
    }
    else {
        if (result.content.length < 1) {
            alert("满赠商品不存在");
            return;
        }
        var html = "<table width='100%' border='0' cellpadding='0' cellspacing='0'>";
        for (var i = 0; i < result.content.length; i++) {
            html += "<tr><td width='30' align='center'><input type='radio' name='fullSendProduct' value='" + result.content[i].pid + "' onclick='selectedFullSendPid=this.value'/></td><td width='70'><img src='/upload/store/" + result.content[i].storeId + "/product/show/thumb60_60/" + result.content[i].showImg + "' width='50' height='50' /></td><td valign='top'><a href='" + result.content[i].url + "'>" + result.content[i].name + "</a><em>¥" + result.content[i].shopPrice + "</em></td></tr>";
        }
        html += "</table>";
        selectedFullSendPid = 0;
        document.getElementById("fullSendProductList" + pmId).innerHTML = html;
        document.getElementById("fullSendBlock" + pmId).style.display = "block";
    }
}

//关闭满赠层
function closeFullSendBlock(pmId) {
    selectedFullSendPid = 0;
    document.getElementById("fullSendProductList" + pmId).innerHTML = "";
    document.getElementById("fullSendBlock" + pmId).style.display = "none";
}

//添加满赠商品
function addFullSend(pmId) {
    if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
    }
    else if (selectedFullSendPid < 1) {
        alert("请先选择商品");
    }
    else {
        Ajax.get("/cart/addfullsend?pmId=" + pmId + "&pid=" + selectedFullSendPid + "&selectedCartItemKeyList=" + getSelectedCartItemKeyList(), false, function (data) {
            try {
                alert(eval("(" + data + ")").content);
            }
            catch (ex) {
                document.getElementById("cartBody").innerHTML = data;
                setBatchSelectCartItemCheckbox();
            }
        })
        closeFullSendBlock(pmId);
    }
}

//取消或选中购物车项
function cancelOrSelectCartItem() {
    if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
        return;
    }
    Ajax.get("/cart/cancelorselectcartitem?selectedCartItemKeyList=" + getSelectedCartItemKeyList(), false, function (data) {
        try {
            alert(eval("(" + data + ")").content);
        }
        catch (ex) {
            document.getElementById("cartBody").innerHTML = data;
            setBatchSelectCartItemCheckbox();
        }
    })
}

//取消或选中店铺购物车
function cancelOrSelectStoreCart(obj) {
    var checked = obj.checked;
    var storeId = obj.getAttribute("storeId");
    var inputList = document.getElementById("cartBody").getElementsByTagName("input");
    for (var i = 0; i < inputList.length; i++) {
        if (inputList[i].type == "checkbox" && inputList[i].name == "cartItemCheckbox" && inputList[i].getAttribute("storeId") == storeId) {
            inputList[i].checked = checked;
        }
    }
    cancelOrSelectCartItem();
}

//取消或选中全部购物车项
function cancelOrSelectAllCartItem(obj) {
    if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
        return;
    }
    if (obj.checked) {
        Ajax.get("/cart/selectallcartitem", false, function (data) {
            try {
                alert(eval("(" + data + ")").content);
            }
            catch (ex) {
                document.getElementById("cartBody").innerHTML = data;
            }
        })
    }
    else {
        var inputList = document.getElementById("cartBody").getElementsByTagName("input");
        for (var i = 0; i < inputList.length; i++) {
            if (inputList[i].type == "checkbox") {
                inputList[i].checked = false;
            }
        }
        $("#buySumtotalCount").html("0");
        $(".subTotalDiacount").html("0.00");
        $(".subTotalPrice").html("0.00");
        //document.getElementById("productAmount").innerHTML = "0.00";
        //document.getElementById("fullCut").innerHTML = "0";
        $("#orderAmount").html("0.00");
    }
}



//获取优惠劵
function getCoupon(uid, couponTypeId) {
    if (uid < 1) {
        alert("请先登陆");
    }
    else if (couponTypeId < 1) {
        alert("请选择优惠劵");
    }
    else {
        Ajax.get("/coupon/getcoupon?couponTypeId=" + couponTypeId, false, getCouponResponse)
    }
}

//处理获取优惠劵的反馈信息
function getCouponResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        alert("领取成功");
    }
    else {
        alert(result.content);
    }
}

//分页跳转
function page_goto(ele) {
    var gopage = $(ele).prev().prev().val();
    //alert(gopage);
    var jumpUrl = $(ele).attr("url");
    var goUrl = "";
    if (jumpUrl.indexOf("page") > -1) {
        goUrl = jumpUrl.substring(0, jumpUrl.length - 1) + gopage;
    }
    else
        goUrl = jumpUrl.substring(0, jumpUrl.lastIndexOf("-") + 1) + gopage + ".html";
    //alert(goUrl);
    window.location.href = goUrl;
}
