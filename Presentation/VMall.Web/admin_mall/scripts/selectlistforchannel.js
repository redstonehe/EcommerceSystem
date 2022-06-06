
/*商品选择层开始*/
var oldSearchProductOfSelectList = "";
var openProductSelectLayerBut = null;
var productSelectLayerHtml = "<div class='selectBoxProgressBar'><p><img src='/admin_mall/content/images/progressbar.gif'/></p></div>";

function setSelectedProduct(item) {
    $(openProductSelectLayerBut).parent().find(".Pid").val($(item).attr("pid"));
    var productName = $(item).text();
    $(openProductSelectLayerBut).val(productName);
    $(openProductSelectLayerBut).parent().find(".ProductName").val(productName);
    $(openProductSelectLayerBut).parent().find(".State").val($(item).attr("state"));
    $(openProductSelectLayerBut).parent().find(".ShopPrice").val($(item).attr("shopprice"));
    $.jBox.close('productSelectLayer');
}

function ajaxProductSelectList(storeId, productName, pageNumber) {
    $.jBox.setContent(productSelectLayerHtml);
    $.get("/malladmin/Channel/productselectlist?t=" + new Date(), {
        'channelId': storeId,
        'productName': productName,
        'pageNumber': pageNumber
    }, function (data) {
        var listObj = eval("(" + data + ")");
        var html = "<div id='selectProductBox'><table width='100%' ><tr><td>商品名称：<input type='text' id='searchProductOfSelectList'  name='searchProductOfSelectList' style='width:120px;height:18px;'> <input type='image' onclick='searchProductSelectList(" + storeId + ")' src='/admin_mall/content/images/s.jpg' class='searchBut'></td></tr><tr><td><div id='selectProductBoxCon'><ul>";
        for (var i = 0; i < listObj.items.length; i++) {
            html += "<li><a onclick='setSelectedProduct(this)' pid='" + listObj.items[i].id + "' state='" + listObj.items[i].state + "' shopprice='" + listObj.items[i].shopprice + "'>" + listObj.items[i].name + "</a></li>";
        }
        html += "<div class='clear'></div></ul><div class='clear'></div></div></td></tr><tr><td><div class='page' style='position:static;'>";

        var pageNumber = parseInt(listObj.pageNumber);
        var totalPages = parseInt(listObj.totalPages);
        var startPageNumber = 1;
        var endPageNumber = 1;
        if (pageNumber == totalPages && totalPages >= 9) {
            startPageNumber = totalPages - 9;
        }
        else if (pageNumber - 4 >= 1) {
            startPageNumber = pageNumber - 4;
        }
        else {
            startPageNumber = 1;
        }
        if (pageNumber == 1 && totalPages >= 9) {
            endPageNumber = 9;
        }
        else if (pageNumber + 4 <= totalPages) {
            endPageNumber = pageNumber + 4;
        }
        else {
            endPageNumber = totalPages;
        }

        html += "<a href='javascript:;' class='bt' onclick='goProductSelectListPage(this," + storeId + ")' pageNumber='1'><<</a>";
        for (var j = startPageNumber; j <= endPageNumber; j++) {
            if (j != pageNumber) {
                html += "<a href='javascript:;' class='bt' onclick='goProductSelectListPage(this," + storeId + ")' pageNumber='" + j + "'>" + j + "</a>";
            }
            else {
                html += "<a href='javascript:;' class='bt hot'>" + j + "</a>";
            }
        }
        html += "<a href='javascript:;' class='bt' onclick='goProductSelectListPage(this," + storeId + ")' pageNumber='" + totalPages + "'>>></a>";

        html += "</div></td></tr></table></div>"
        $.jBox.setContent(html);
        $("#searchProductOfSelectList").val(oldSearchProductOfSelectList);
    })
}

function searchProductSelectList(storeId) {
    oldSearchProductOfSelectList = $("#searchProductOfSelectList").val();
    ajaxProductSelectList(storeId, oldSearchProductOfSelectList);
}

function goProductSelectListPage(pageObj, storeId) {
    oldSearchProductOfSelectList = $("#searchProductOfSelectList").val();
    ajaxProductSelectList(storeId, oldSearchProductOfSelectList, $(pageObj).attr("pageNumber"));
}

function openProductSelectLayer(openLayerBut, storeId) {
    $.jBox('html:productSelectLayer', {
        id: 'productSelectLayer',
        width: 950,
        buttons: { '关闭': true },
        title: "选择商品"
    });
    openProductSelectLayerBut = openLayerBut;
    ajaxProductSelectList(storeId);

}
/*商品选择层结束*/

