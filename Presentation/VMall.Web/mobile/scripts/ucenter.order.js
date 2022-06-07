﻿//获得订单列表
var orderListNextPageNumber = 2;
var hasNextPage = false;
function getOrderList(startAddTime, endAddTime, orderState, page) {
    document.getElementById("loadBut").style.display = "none";
    document.getElementById("loadPrompt").style.display = "block";
    Ajax.get("/mob/ucenter/ajaxorderlist?startAddTime=" + startAddTime + "&endAddTime=" + endAddTime + "&orderState=" + orderState + "&page=" + page, false, function (data) {
        getOrderListResponse(data);
    })
}

//处理获得订单列表的反馈信息
function getOrderListResponse(data) {
    $("#orderListBlock").append(data);
    if (hasNextPage) {
            $("#loadBut").css("display", "block");
            $("#loadPrompt").css("display", "none");
            orderListNextPageNumber += 1;
        }
        else {
            $("#loadBut").css("display","none");
            $("#loadPrompt").css("display", "none");
            $("#lastPagePrompt").css("display", "block");
        }

    //try {
    //    var result = eval("(" + data + ")");
    //    var element = document.createElement("div");
    //    element.className = "proItme";
    //    var innerHTML = "";
    //    for (var i = 0; i < result.OrderList.length; i++) {
    //        var oid = result.OrderList[i].oid;
    //        var orderState = result.OrderList[i].orderstate;
    //        var orderProductInfo = null;
    //        for (var j = 0; j < result.OrderProductList.length; j++) {
    //            if (result.OrderProductList[j].Oid == oid) {
    //                if (orderProductInfo == null)
    //                    orderProductInfo = result.OrderProductList[j];
    //            }
    //        }
    //        innerHTML += "<div class='proItme'>";
    //        innerHTML += "<a href='/mob/ucenter/orderinfo?oid=" + oid + "'>";
    //        innerHTML += "<img src='/upload/store/" + orderProductInfo.StoreId + "/product/show/thumb350_350/" + orderProductInfo.ShowImg + "'>";
    //        innerHTML += "<div class='order-msg'><p class='title'>" + orderProductInfo.Name + "</p><p class='price'>¥" + orderProductInfo.DiscountPrice + "<span></span></p><p class='order-data'>" + JsonTimeConert(orderProductInfo.AddTime) + "</p></div>";
    //        innerHTML += "</a>";
    //        innerHTML += "<div class='proBt'>";
    //        innerHTML += "<a class='redBt' href='/mob/ucenter/orderactionlist?oid=" + oid + "' id='orderActionList" + oid + "'>订单跟踪</a>";
    //        if (orderState == 30 && result.OrderList[i].paymode == 1) {
    //            innerHTML += "<a class='redBt' href='/mob/order/payshow?oidList=" + oid + "' id='payOrderBut" + oid + "'>在线支付</a>";
    //        }
    //        if (orderState == 140 && result.OrderList[i].isreview == 0) {
    //            innerHTML += "<a class='redBt' href='/mob/ucenter/revieworder?oid=" + oid + "'>订单评价</a>";
    //        }
    //        if (orderState == 30 || (orderState == 50 && result.OrderList[i].paymode == 0)) {
    //            innerHTML += "<a class='redBt' href='javascript:cancelOrder(" + oid + ", 0)' id='cancelOrderBut" + oid + "'>取消订单</a>";
    //        }
    //        innerHTML += "</div>";
    //        innerHTML += "</div>";
    //    }
    //    element.innerHTML = innerHTML;
    //    document.getElementById("orderListBlock").appendChild(element);
    //    if (result.PageModel.HasNextPage) {
    //        document.getElementById("loadBut").style.display = "block";
    //        document.getElementById("loadPrompt").style.display = "none";
    //        orderListNextPageNumber += 1;
    //    }
    //    else {
    //        document.getElementById("loadBut").style.display = "none";
    //        document.getElementById("loadPrompt").style.display = "none";
    //        document.getElementById("lastPagePrompt").style.display = "block";
    //    }
    //}
    //catch (ex) {
    //    alert("加载错误");
    //}
}

//取消订单
function cancelOrder(oid, cancelReason) {
    if (confirm("确定要取消订单吗？")) {
        Ajax.post("/mob/ucenter/cancelorder", { 'oid': oid, 'cancelReason': cancelReason }, false, cancelOrderResponse);
    }
}

//处理取消订单的反馈信息
function cancelOrderResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        //document.getElementById("cancelOrderBut" + result.content).parentNode.innerHTML = "<a class='redBt' href='" + document.getElementById("orderActionList" + result.content).href + "'>订单跟踪</a>";
        alert("取消成功");
        window.location.href = window.location.href;
    }
    else {
        alert(result.content);
    }
}

//选择商品星星
function selectProductStar(i) {
    var list = document.getElementById("productStarBlock").getElementsByTagName("span");
    for (var j = 1; j <= 5; j++) {
        if (j <= i) {
            list[j - 1].className = "on";
        }
        else {
            list[j - 1].className = "";
        }
    }
    var reviewProductFrom = document.forms["reviewProductFrom"];
    reviewProductFrom.elements["star"].value = i;
}

//选择描述星星
function selectDescriptionStar(i) {
    var list = document.getElementById("descriptionBlock").getElementsByTagName("span");
    for (var j = 1; j <= 5; j++) {
        if (j <= i) {
            list[j - 1].className = "on";
        }
        else {
            list[j - 1].className = "";
        }
    }
    var reviewProductFrom = document.forms["reviewStoreFrom"];
    reviewProductFrom.elements["descriptionStar"].value = i;
}

//选择服务星星
function selectServiceStar(i) {
    var list = document.getElementById("serviceBlock").getElementsByTagName("span");
    for (var j = 1; j <= 5; j++) {
        if (j <= i) {
            list[j - 1].className = "on";
        }
        else {
            list[j - 1].className = "";
        }
    }
    var reviewProductFrom = document.forms["reviewStoreFrom"];
    reviewProductFrom.elements["serviceStar"].value = i;
}

//选择配送星星
function selectShipStar(i) {
    var list = document.getElementById("shipBlock").getElementsByTagName("span");
    for (var j = 1; j <= 5; j++) {
        if (j <= i) {
            list[j - 1].className = "on";
        }
        else {
            list[j - 1].className = "";
        }
    }
    var reviewProductFrom = document.forms["reviewStoreFrom"];
    reviewProductFrom.elements["shipStar"].value = i;
}

//打开评价商品层
function openReviewProductBlock(recordId) {
    var reviewProductFrom = document.forms["reviewProductFrom"];
    reviewProductFrom.elements["recordId"].value = recordId;
    document.getElementById("reviewProductBlock").style.display = "";
}

//评价商品
function reviewProduct() {
    var reviewProductFrom = document.forms["reviewProductFrom"];

    var oid = reviewProductFrom.elements["oid"].value;
    var recordId = reviewProductFrom.elements["recordId"].value;
    var star = parseInt(reviewProductFrom.elements["star"].value);
    var message = reviewProductFrom.elements["message"].value;

    if (!verifyReviewProduct(recordId, star, message)) {
        return;
    }
    Ajax.post("/mob/ucenter/reviewproduct?oid=" + oid + "&recordId=" + recordId, { 'star': star, 'message': message }, false, reviewProductResponse);
}

//验证评价商品
function verifyReviewProduct(recordId, star, message) {
    if (recordId < 1) {
        alert("请选择商品");
        return false;
    }
    if (star < 1 || star > 5) {
        alert("请选择正确的星星");
        return false;
    }
    if (message.length == 0) {
        alert("请输入评价内容");
        return false;
    }
    if (message.length > 100) {
        alert("评价内容最多输入100个字");
        return false;
    }
    return true;
}

//处理评价商品的反馈信息
function reviewProductResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        var reviewProductFrom = document.forms["reviewProductFrom"];
        reviewProductFrom.elements["recordId"].value = 0;
        reviewProductFrom.elements["message"].value = "";
        selectProductStar(5);

        document.getElementById("reviewProductBlock").style.display = "none";

        document.getElementById("reviewOperate" + result.content).innerHTML = "您已评价";
        document.getElementById("reviewOperate" + result.content).onclick = null;
        document.getElementById("reviewOperate" + result.content).className = "gayBt";
        document.getElementById("reviewOperate" + result.content).style.color = "#999";

        alert("评价成功");
    }
    else {
        alert(result.content);
    }
}

//打开评价店铺层
function openReviewStoreBlock() {
    if (document.getElementById("reviewStoreBlock").style.display == "") {
        document.getElementById("reviewStoreBlock").style.display = "none";
    }
    else {
        document.getElementById("reviewStoreBlock").style.display = "";
    }
}

//评价店铺
function reviewStore() {
    var reviewStoreFrom = document.forms["reviewStoreFrom"];

    var oid = reviewStoreFrom.elements["oid"].value;
    var descriptionStar = parseInt(reviewStoreFrom.elements["descriptionStar"].value);
    var serviceStar = parseInt(reviewStoreFrom.elements["serviceStar"].value);
    var shipStar = parseInt(reviewStoreFrom.elements["shipStar"].value);

    Ajax.post("/mob/ucenter/reviewstore?oid=" + oid, { 'descriptionStar': descriptionStar, 'serviceStar': serviceStar, 'shipStar': shipStar }, false, reviewStoreResponse);
}

//处理评价店铺的反馈信息
function reviewStoreResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        document.getElementById("reviewStoreBlock").style.display = "none";
        removeNode(document.getElementById("reviewStoreBut"));

        alert("评价成功");
    }
    else {
        alert(result.content);
    }
}