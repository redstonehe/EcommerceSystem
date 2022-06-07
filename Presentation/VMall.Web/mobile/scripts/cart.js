﻿//获得选中的购物车项键列表
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
        document.getElementById("selectAllBut_bottom").checked = true;
    }
    else {
        document.getElementById("selectAllBut_bottom").checked = false;
    }
}

//删除购物车中商品
function delCartProduct(pid) {
    if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
        return;
    }
    Ajax.get("/mob/cart/delpruduct?pid=" + pid + "&selectedCartItemKeyList=" + getSelectedCartItemKeyList(), false, function (data) {
        try {
            alert(val("(" + data + ")").content);
        }
        catch (ex) {
            document.getElementById("cartBody").innerHTML = data;
            setBatchSelectCartItemCheckbox();
        }
    })
}

//删除购物车中套装
function delCartSuit(pmId) {
    if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
        return;
    }
    Ajax.get("/mob/cart/delsuit?pmId=" + pmId + "&selectedCartItemKeyList=" + getSelectedCartItemKeyList(), false, function (data) {
        try {
            alert(eval("(" + data + ")").content);
        }
        catch (ex) {
            document.getElementById("cartBody").innerHTML = data;
            setBatchSelectCartItemCheckbox();
        }
    })
}

//删除购物车中满赠
function delCartFullSend(pmId) {
    if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
        return;
    }
    Ajax.get("/mob/cart/delfullsend?pmId=" + pmId + "&selectedCartItemKeyList=" + getSelectedCartItemKeyList(), false, function (data) {
        try {
            alert(eval("(" + data + ")").content);
        }
        catch (ex) {
            document.getElementById("cartBody").innerHTML = data;
            setBatchSelectCartItemCheckbox();
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
        Ajax.get("/mob/cart/changepruductcount?pid=" + pid + "&buyCount=" + buyCount + "&selectedCartItemKeyList=" + getSelectedCartItemKeyList(), false, function (data) {
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
        Ajax.get("/mob/cart/changesuitcount?pmId=" + pmId + "&buyCount=" + buyCount + "&selectedCartItemKeyList=" + getSelectedCartItemKeyList(), false, function (data) {
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

//商品购买数量输入框事件
var tempProductNumber = 0;
function productNumberFocus(obj) {
    tempProductNumber = obj.value;
    //obj.value = "";
}
function productNumberBlur(obj, itemId, itemType, mincount) {
    var value = obj.value;
    if (value == "") {
        obj.value = tempProductNumber;
    }
    else {
        if (!isInt(value)) {
            alert("只能输入数字!");
            obj.value = tempProductNumber;
        }
        if (value < mincount) {
            alert("购买数量不能小于最低起购数量!");
            obj.value = tempProductNumber;
        }
        else {
            if (itemType == 0) {
                changePruductCount(itemId, value);
            }
            else {
                changeSuitCount(itemId, value);
            }
        }
    }
}

//获取满赠商品
var selectedFullSendPmId = 0;
function getFullSend(pmId) {
    if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
        return;
    }
    Ajax.get("/mob/cart/getfullsend?pmId=" + pmId, false, function (data) {
        selectedFullSendPmId = pmId;
        getFullSendResponse(data);
    })
}

//处理获取满赠商品的反馈信息
var selectedFullSendPid = 0;
function getFullSendResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state != "success") {
        alert(result.content);
    }
    else {
        if (result.content.length < 1) {
            alert("满赠商品不存在");
            return;
        }

        var html = "";
        for (var i = 0; i < result.content.length; i++) {
            html += "<div class='proInfo'>";
            html += "<div class='price'>¥" + result.content[i].shopPrice + "</div>";
            html += "<div class='proInfo1'><input type='radio' name='fullSendProduct' class='checkbox' value='" + result.content[i].pid + "' onclick='selectedFullSendPid=this.value'/></div>";
            html += "<div class='proInfo2 change'>";
            html += "<a href='" + result.content[i].url + "' class='proImg'><img src='/upload/store/" + result.content[i].storeId + "/product/show/thumb300_300/" + result.content[i].showImg + "' width='59' height='59' /></a>";
            html += "<div class='text'>";
            html += "<a href='" + result.content[i].url + "'>" + result.content[i].name + "</a>";
            html += "<div class='nb'>x1</div></div></div></div>";
        }
        document.getElementById("fullSendProductList").innerHTML = html;
        document.getElementById("fullSendBlock").style.display = "block";
        document.getElementById("fullSendMask").style.display = "block";
    }
}

//关闭满赠层
function closeFullSendBlock() {
    selectedFullSendPmId = 0;
    selectedFullSendPid = 0;
    document.getElementById("fullSendProductList").innerHTML = "";
    document.getElementById("fullSendBlock").style.display = "none";
    document.getElementById("fullSendMask").style.display = "none";
}

//添加满赠商品
function addFullSend() {
    if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
    }
    else if (selectedFullSendPmId < 1) {
        alert("请先选择促销活动");
    }
    else if (selectedFullSendPid < 1) {
        alert("请先选择商品");
    }
    else {
        Ajax.get("/mob/cart/addfullsend?pmId=" + selectedFullSendPmId + "&pid=" + selectedFullSendPid + "&selectedCartItemKeyList=" + getSelectedCartItemKeyList(), false, function (data) {
            try {
                alert(eval("(" + data + ")").content);
            }
            catch (ex) {
                document.getElementById("cartBody").innerHTML = data;
                setBatchSelectCartItemCheckbox();
            }
        })
        closeFullSendBlock();
    }
}

//取消或选中购物车项
function cancelOrSelectCartItem() {
    if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
        return;
    }
    Ajax.get("/mob/cart/cancelorselectcartitem?selectedCartItemKeyList=" + getSelectedCartItemKeyList(), false, function (data) {
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
        Ajax.get("/mob/cart/selectallcartitem", false, function (data) {
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
        document.getElementById("productAmount").innerHTML = "0.00";
        document.getElementById("fullCut").innerHTML = "0";
        document.getElementById("orderAmount").innerHTML = "0.00";
    }
}

//前往确认订单
//function goConfirmOrder() {
//    if (isGuestSC == 0 && uid < 1) {
//        alert("请先登录");
//        return;
//    }
//    var inputList = document.getElementById("cartBody").getElementsByTagName("input");

//    var checkboxList = new Array();
//    for (var i = 0; i < inputList.length; i++) {
//        if (inputList[i].type == "checkbox") {
//            checkboxList.push(inputList[i]);
//        }
//    }

//    var valueList = new Array();
//    for (var i = 0; i < checkboxList.length; i++) {
//        if (checkboxList[i].checked) {
//            valueList.push(checkboxList[i].value);
//        }
//    }

//    var storeList = $("input[data-store='1']");
//    var storeboxList = new Array();
//    for (var i = 0; i < storeList.length; i++) {
//        var storeid = $(storeList[i]).attr("storeid");
//        var checkStore = $("input[storeid='" + storeid + "'][checked='checked'][name='cartItemCheckbox']");
//        if (checkStore.length > 0) {
//            storeboxList.push(storeid);
//        }
//    }
//    //var storevalueList = new Array();
//    //for (var i = 0; i < storeboxList.length; i++) {
//    //    if (storeboxList[i].checked) {
//    //        storevalueList.push($(storeboxList[i]).attr("storeid"));
//    //    }
//    //}

//    if (valueList.length < 1) {
//        alert("请先选择购物车商品");
//    }
//    else {
//        if (valueList.length != checkboxList.length) {
//            document.getElementById("selectedCartItemKeyList").value = valueList.join(',');
//        }
//        $("#selectedStoreKeyList").val(storeboxList.join(','));
//        document.forms[0].submit();
//    }
//}