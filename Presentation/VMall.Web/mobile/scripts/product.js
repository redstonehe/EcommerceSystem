//增加商品数量
function addProuctCount() {
    var buyCountInput = document.getElementById("buyCount");
    var buyCount = buyCountInput.value;
    var buypid = $("#buypid").val();
    var minbuycount = $(".minbuycount").length > 0 ? $(".minbuycount").html() : 0;
    if (!isInt(buyCount)) {
        alert('请输入数字');
        return false;
    }
    if (!isInt(minbuycount)) {
        alert('请输入数字');
        return false;
    }
    if (minbuycount > 0 && (buypid == 3233 || buypid == 3234 || buypid == 3235 || buypid == 3236))
        buyCountInput.value = parseInt(buyCount) + (1 * minbuycount);
    else
        buyCountInput.value = parseInt(buyCount) + 1;

}

//减少商品数量
function cutProductCount() {
    var buyCountInput = document.getElementById("buyCount");
    var buyCount = buyCountInput.value;
    var buypid = $("#buypid").val();
    var minbuycount = $(".minbuycount").length > 0 ? $(".minbuycount").html() : 0;
    if (!isInt(buyCount)) {
        alert('请输入数字');
        return false;
    }
    if (!isInt(minbuycount)) {
        alert('请输入数字');
        return false;
    }
    var count = parseInt(buyCount);
    if (minbuycount > 0 && (buypid == 3233 || buypid == 3234 || buypid == 3235 || buypid == 3236)) {
        if (count > 1 * minbuycount)
            buyCountInput.value = count - (1 * minbuycount);
    }
    else {
        if (count > 1) {
            buyCountInput.value = count - 1;
        }
    }
}

//添加商品到收藏夹
function addProductToFavorite(pid) {
    if (pid < 1) {
        alert("请选择商品");
    }
    else if (uid < 1) {
        alert("请先登录");
    }
    else {
        Ajax.get("/mob/ucenter/addproducttofavorite?pid=" + pid, false, addProductToFavoriteResponse)
    }
}

//处理添加商品到收藏夹的反馈信息
function addProductToFavoriteResponse(data) {
    var result = eval("(" + data + ")");
    alert(result.content);
    $(".goods-collect-button span").addClass("active");
}

//添加商品到购物车
function addProductToCart(pid, buyCount, type) {
    if (pid < 1) {
        alert("请选择商品");
    }
    else if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
    }
    else if (buyCount < 1) {
        alert("请填写购买数量");
    }
    else {
        Ajax.get("/mob/cart/addproduct?pid=" + pid + "&buyCount=" + buyCount, false, function (data) {
            addProductToCartResponse(type, data);
        });
    }
}
//添加商品到购物车--二次确认信息
function addProductToCartWithConfirm(pid, buyCount, type) {
    var type1 = $(".itmeP .dd ").find("a").eq(0).html();
    var type2 = $(".itmeP .dd").find("a").eq(1).html();
    var type_pid_1 = $(".itmeP .dd").find("a").eq(0).attr("data-pid");
    var type_pid_2 = $(".itmeP .dd").find("a").eq(1).attr("data-pid");
    //alert(11);
    //var diglog = '<div  class="layer_1"><h2>请再次确认购买的产品用途</h2></div>';
    //diglog += '<div  class="layer_2">';
    //diglog += '<span class="span_left" onclick="select_type(this);" data-pid="' + type_pid_1 + '">' + type1 + '</span>';
    //diglog += '<span class="span_right"  onclick="select_type(this);" data-pid="' + type_pid_2 + '">' + type2 + '</span>';
    //diglog += '</div>';
    //diglog += '<div class="layer_3">';
    //diglog += '<a  onclick="submit_confirm(0);">确定</a>';
    //diglog += '</div>';
    //diglog += '<div  class="layer_4"><h2>请选择购买的产品用途</h2></div>';
  
    $(".layer_2 .span_left").attr("data-pid", type_pid_1).html(type1);
    $(".layer_2 .span_right").attr("data-pid", type_pid_2).html(type2);
    var diglog = $("#showMsg").html();
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
        area: ['600px', '360px'],
        shadeClose: true, //点击遮罩关闭
        content: diglog// $("#showMsg")// diglog//$("#header")
    });

}

function select_type(ele) {
    $(".layer_4").hide();
    $(".layer_2 span").removeClass("hottest");
    $(ele).addClass("hottest");
    var typetitle = $(ele).html();
    //alert(typetitle)
    if (typetitle.indexOf("代理") > -1) {
        $(".layer_5").show();
        $(".layer_6").show();
    }
    if (typetitle.indexOf("零售") > -1) {
        $(".layer_5").hide();
        $(".layer_6").hide();
    }
}
function select_suittype(ele) {
    $(".layer_4").hide();
    $(".layer_5 i").hide();
    $(".layer_5 span").removeClass("hottest");
    $(ele).addClass("hottest");
    $(ele).next().show();
}
//提交加入购物车
function submit_confirm(type) {
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
    layer.closeAll();
    Ajax.get("/mob/cart/addproduct?pid=" + pid + "&buyCount=" + buyCount + "&suitpid=" + suitpid, false, function (data) {
        addProductToCartResponse(type, data);
    });
    
}
//直接购买商品--二次确认信息
function directBuyProductWithConfirm(pid, buyCount) {
    var type1 = $(".itmeP .dd ").find("a").eq(0).html();
    var type2 = $(".itmeP .dd ").find("a").eq(1).html();
    var type_pid_1 = $(".itmeP .dd").find("a").eq(0).attr("data-pid");
    var type_pid_2 = $(".itmeP .dd").find("a").eq(1).attr("data-pid");
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
    var diglog = $("#showMsg2").html();
    if (pid < 1) {
        alert("请选择商品");
    }
    else if (uid < 1) {
        window.location.href = "/mob/account/login?returnUrl=/cart/directbuyproduct?proid=" + pid + "_" + buyCount;
        //alert("请先登录");
    }
    else if (buyCount < 1) {
        alert("请填写购买数量");
    }
    layer.open({
        type: 1,
        title: '确认',
        area: ['600px', '400px'],
        shadeClose: true, //点击遮罩关闭
        content: diglog//$("#showMsg2")//diglog//$("#header")
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
    Ajax.get("/mob/cart/directbuyproduct?pid=" + pid + "&buyCount=" + buyCount + "&suitpid=" + suitpid, false, directBuyProductResponse)

}
//处理添加商品到购物车的反馈信息
function addProductToCartResponse(type, data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        if (type == 0) {
            window.location.href = "/mob/cart/index";
        }
        else {
            document.getElementById("addResult1").style.display = "block";
            document.getElementById("addResult2").style.display = "block";
        }
    }
    else {
        alert(result.content);
    }
}
//直接购买商品
function directBuyProduct(pid, buyCount) {
    if (pid < 1) {
        alert("请选择商品");
    }
    else if (uid < 1) {
        //alert("请先登录");
        window.location.href = "/mob/account/login?returnUrl=/mob/cart/directbuyproduct?proid=" + pid + "_" + buyCount;
    }
    else if (buyCount < 1) {
        alert("请填写购买数量");
    }
    else {
        Ajax.get("/mob/cart/directbuyproduct?pid=" + pid + "&buyCount=" + buyCount, false, directBuyProductResponse)
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
function addSuitToCart(pmId, buyCount, type) {
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
        Ajax.get("/mob/cart/addsuit?pmId=" + pmId + "&buyCount=" + buyCount, false, function (data) {
            addSuitToCartResponse(type, data);
        });
    }
}

//处理添加套装到购物车的反馈信息
function addSuitToCartResponse(type, data) {
    var result = eval("(" + data + ")");
    if (result.state != "stockout") {
        if (type == 0) {
            window.location.href = "/mob/cart/index";
        }
        else {
            document.getElementById("addResult1").style.display = "block";
            document.getElementById("addResult2").style.display = "block";
        }
    }
    else {
        alert("商品库存不足");
    }
}

//获得商品评价列表
function getProductReviewList(pid, reviewType, page) {
    Ajax.get("/mob/catalog/ajaxproductreviewlist?pid=" + pid + "&reviewType=" + reviewType + "&page=" + page, false, getProductReviewListResponse)
}

//处理获得商品评价的反馈信息
function getProductReviewListResponse(data) {
    document.getElementById("productReviewList").innerHTML = data;
}

//获得商品咨询列表
function getProductConsultList(pid, consultTypeId, consultMessage, page) {
    Ajax.get("/mob/catalog/ajaxproductconsultlist?pid=" + pid + "&consultTypeId=" + consultTypeId + "&consultMessage=" + encodeURIComponent(consultMessage) + "&page=" + page, false, getProductConsultListResponse)
}

//处理获得商品咨询的反馈信息
function getProductConsultListResponse(data) {
    document.getElementById("productConsultList").innerHTML = data;
}