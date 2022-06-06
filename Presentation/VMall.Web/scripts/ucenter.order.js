//取消订单
function cancelOrder(oid, cancelReason) {
    if (confirm("确定要取消订单吗？")) {
        Ajax.post("/ucenter/cancelorder", { 'oid': oid, 'cancelReason': cancelReason }, false, cancelOrderResponse);
    }
}

//确认收货
function confirmReceip(oid) {
    if (confirm("确定要确认收货吗？")) {
        $.ajax({
            type: "GET",
            url: "/UCenter/ConfirmReceiving?r=" + Math.random(),
            data: { oid: oid, },
            success: function (result) {

                var result = eval("(" + result + ")");
                if (result.state == "success") {

                    alert(result.content);
                    window.location.href = window.location.href;
                }
                else if (result.state == "exception") {
                    alert(result.content);
                }
                else if (result.state == "error") {
                    showVerifyError(result.content);
                }
            }
        });
    }
}
//退货申请
function returnApply(oid) {
    if (confirm("确定要申请退货吗？")) {
        $.ajax({
            type: "GET",
            url: "/UCenter/ReturnApply?r=" + Math.random(),
            data: { oid: oid, },
            success: function (result) {

                var result = eval("(" + result + ")");
                if (result.state == "success") {

                    alert(result.content);
                    window.location.href = window.location.href;
                }
                else if (result.state == "exception") {
                    alert(result.content);
                }
                else if (result.state == "error") {
                    showVerifyError(result.content);
                }
            }
        });
    }
}
////换货申请 
//function changeApply(oid) {
//    //int said,int changeType,string changeDesc,int oid = -1
//    var said = 2;
//    var changeType = 0;
//    var changeDesc = "尺码不合适，换大一点";
//    if (confirm("确定要申请换货吗？")) {
//        $.ajax({
//            type: "GET",
//            url: "/UCenter/ChangeApply",//said: said, changeType: changeType, changeDesc: changeDesc, 
//            data: { oid: oid },
//            success: function (result) {

//                var result = eval("(" + result + ")");
//                if (result.state == "success") {

//                    alert(result.content);
//                    window.location.href = window.location.href;
//                }
//                else if (result.state == "exception") {
//                    alert(result.content);
//                }
//                else if (result.state == "error") {
//                    showVerifyError(result.content);
//                }
//            }
//        });
//    }
//    //if (confirm("确定要申请换货吗？")) {
//    //    $.ajax({
//    //        type: "GET",
//    //        url: "/UCenter/ChangeApply",//said: said, changeType: changeType, changeDesc: changeDesc, 
//    //        data: { said: said, changeType: changeType, changeDesc: changeDesc, oid: oid },
//    //        success: function (result) {

//    //            var result = eval("(" + result + ")");
//    //            if (result.state == "success") {

//    //                alert(result.content);
//    //                window.location.href = window.location.href;
//    //            }
//    //            else if (result.state == "exception") {
//    //                alert(result.content);
//    //            }
//    //            else if (result.state == "error") {
//    //                showVerifyError(result.content);
//    //            }
//    //        }
//    //    });
//    //}
//}

//处理取消订单的反馈信息
//function cancelOrderResponse(data) {
//    var result = eval("(" + data + ")");
//    if (result.state == "success") {
//        document.getElementById("orderState" + result.content).innerHTML = "取消";
//        removeNode(document.getElementById("payOrderBut" + result.content));
//        removeNode(document.getElementById("cancelOrderBut" + result.content));
//        alert("取消成功");
//    }
//    else {
//        alert(result.content);
//    }
//}

//处理取消订单的反馈信息
function cancelOrderResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        //document.getElementById("orderState" + result.content).innerHTML = "取消";
        //document.getElementById("cancelOrderBut" + result.content).parentNode.innerHTML = "<a href='" + document.getElementById("orderInfo" + result.content).href + "'>查看</a>";
        alert("取消成功");
        window.location.href = window.location.href;
    }
    else {
        alert(result.content);
    }
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
    var star = getSelectedRadio(reviewProductFrom.elements["star"]).value;
    var message = reviewProductFrom.elements["message"].value;

    if (!verifyReviewProduct(recordId, star, message)) {
        return;
    }
    Ajax.post("/ucenter/reviewproduct?oid=" + oid + "&recordId=" + recordId, { 'star': star, 'message': message }, false, reviewProductResponse);
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

        document.getElementById("reviewProductBlock").style.display = "none";

        document.getElementById("reviewState" + result.content).innerHTML = "已评价";
        document.getElementById("reviewOperate" + result.content).innerHTML = "";

        alert("评价成功");
    }
    else {
        alert(result.content);
    }
}

//评价店铺
function reviewStore() {
    var reviewStoreFrom = document.forms["reviewStoreFrom"];

    var oid = reviewStoreFrom.elements["oid"].value;
    var descriptionStar = getSelectedRadio(reviewStoreFrom.elements["descriptionStar"]).value;
    var serviceStar = getSelectedRadio(reviewStoreFrom.elements["serviceStar"]).value;
    var shipStar = getSelectedRadio(reviewStoreFrom.elements["shipStar"]).value;

    Ajax.post("/ucenter/reviewstore?oid=" + oid, { 'descriptionStar': descriptionStar, 'serviceStar': serviceStar, 'shipStar': shipStar }, false, reviewStoreResponse);
}

//处理评价店铺的反馈信息
function reviewStoreResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        removeNode(document.getElementById("reviewStoreBut"));

        alert("评价成功");
    }
    else {
        alert(result.content);
    }
}