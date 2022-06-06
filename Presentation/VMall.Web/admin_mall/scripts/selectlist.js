var jBoxConfig = {};
jBoxConfig.defaults = {
    border: 3, /* 窗口的外边框像素大小,必须是0以上的整数 */
    top: '30px', /* 窗口离顶部的距离,可以是百分比或像素(如 '100px') */
    opacity: 0.5, /* 窗口隔离层的透明度,如果设置为0,则不显示隔离层 */
    timeout: 0, /* 窗口显示多少毫秒后自动关闭,如果设置为0,则不自动关闭 */
    showType: 'fade', /* 窗口显示的类型,可选值有:show、fade、slide */
    showSpeed: 'fast', /* 窗口显示的速度,可选值有:'slow'、'fast'、表示毫秒的整数 */
    showIcon: true, /* 是否显示窗口标题的图标，true显示，false不显示，或自定义的CSS样式类名（以为图标为背景） */
    showClose: true, /* 是否显示窗口右上角的关闭按钮 */
    draggable: true, /* 是否可以拖动窗口 */
    dragLimit: true, /* 在可以拖动窗口的情况下，是否限制在可视范围 */
    dragClone: false, /* 在可以拖动窗口的情况下，鼠标按下时窗口是否克隆窗口 */
    persistent: true, /* 在显示隔离层的情况下，点击隔离层时，是否坚持窗口不关闭 */
    showScrolling: true, /* 是否显示浏览的滚动条 */
    iframeScrolling: 'auto', /* 在窗口内容使用iframe:前缀标识的情况下，iframe的scrolling属性值，可选值有：'auto'、'yes'、'no' */
    width: 'auto', /* 窗口的宽度，值为'auto'或表示像素的整数 */
    height: 'auto', /* 窗口的高度，值为'auto'或表示像素的整数 */
    
    loaded: function (h) {
        $(".jbox-button-panel").eq(1).css("height", "auto");
    }, /* 窗口加载完成后执行的函数，需要注意的是，如果是ajax或iframe也是要等加载完http请求才算窗口加载完成，参数h表示窗口内容的jQuery对象 */
    
};
$.jBox.setDefaults(jBoxConfig);

/*分类选择层开始*/
var openCategorySelectLayerBut = null;
var categorySelectLayerHtml = "<div class='selectBoxProgressBar'><p><img src='/admin_mall/content/images/progressbar.gif'/></p></div>";

function categoryTree(obj, layer) {
    var state = $(obj).attr("class");
    if (state == "open") {
        $(obj).parent().parent().nextAll().each(function (index) {
            var flag = parseInt($(this).attr("layer")) - layer;
            if (flag == 1) {
                $(this).show();
            }
            else if (flag == 0) {
                return false;
            }
        })
        $(obj).removeClass("open").addClass("close");
    }
    else if (state == "close") {
        $(obj).parent().parent().nextAll().each(function (index) {
            if (parseInt($(this).attr("layer")) > layer) {
                $(this).hide();
                $(this).find("th span").each(function (i) {
                    if (typeof ($(this).attr("class")) != "undefined" && $(this).attr("class") !== "") {
                        $(this).removeClass("close").addClass("open");
                    }
                })
            }
            else {
                return false;
            }
        })
        $(obj).removeClass("close").addClass("open");
    }
}

function setSelectedCategory(selectedCateId, selectedCateName) {
    $(openCategorySelectLayerBut).parent().find(".CateId").val(selectedCateId);
    $(openCategorySelectLayerBut).val(selectedCateName).parent().find(".CategoryName").val(selectedCateName);
    $.jBox.close('categorySelectLayer');
}

function ajaxCategorySelectList() {
    $.jBox.setContent(categorySelectLayerHtml);
    $.get("/admin_mall/cache/category/selectlist.js?t=" + new Date(), function (data) {
        $.jBox.setContent(data);
    })
}

function openCategorySelectLayer(openLayerBut) {
    $.jBox('html:categorySelectLayer', {
        id: 'categorySelectLayer',
        width: 750,
        buttons: { '关闭': true },
        title: "选择类别"
    });
    openCategorySelectLayerBut = openLayerBut;
    ajaxCategorySelectList();
}
/*分类选择层结束*/

/*频道专区选择层开始*/
var oldSearchChannelOfSelectList = "";
var openChannelSelectLayerBut = null;
var ChannelSelectLayerHtml = "<div class='selectBoxProgressBar'><p><img src='/admin_mall/content/images/progressbar.gif'/></p></div>";

function setSelectedChannel(item) {
    $(openChannelSelectLayerBut).parent().find(".ChannelId").val($(item).attr("ChannelId"));
    var channelName = $(item).text();
    $(openChannelSelectLayerBut).val(channelName);
    $(openChannelSelectLayerBut).parent().find(".ChannelName").val(channelName);
    //if (channelName == "积分专区") {
    //    $("#cutpercent").show()
    //} else {
    //    $("#cutpercent").hide()
    //}
    $.jBox.close('channelSelectLayer');
}


function SelectChannel(item) {
    $(item).prev().attr("checked", true);

}

function ajaxChannelSelectList(channelName, pageNumber) {
    $.jBox.setContent(ChannelSelectLayerHtml);
    $.get("/malladmin/channel/selectlist?t=" + new Date(), {
        'channelName': channelName,
        'pageNumber': pageNumber
    }, function (data) {
        var listObj = eval("(" + data + ")");
        var html = "<div id='selectBrandBox'><table width='100%' ><tr><td>频道专区名称：<input type='text' id='searchChannelOfSelectList'  name='searchChannelOfSelectList' style='width:120px;height:18px;'> <input type='image' onclick='searchChannelSelectList()' src='/admin_mall/content/images/s.jpg' class='searchBut'></td></tr><tr><td><div id='selectChannelBoxCon'><ul>";
        for (var i = 0; i < listObj.items.length; i++) {
            html += "<li><a onclick='setSelectedChannel(this)' channelId='" + listObj.items[i].id + "' style='display: inline-table'>" + listObj.items[i].name + "</a></li>";
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

        html += "<a href='javascript:;' class='bt' onclick='goChannelSelectListPage(this)' pageNumber='1'><<</a>";
        for (var j = startPageNumber; j <= endPageNumber; j++) {
            if (j != pageNumber) {
                html += "<a href='javascript:;' class='bt' onclick='goChannelSelectListPage(this)' pageNumber='" + j + "'>" + j + "</a>";
            }
            else {
                html += "<a href='javascript:;' class='bt hot'>" + j + "</a>";
            }
        }
        html += "<a href='javascript:;' class='bt' onclick='goChannelSelectListPage(this)' pageNumber='" + totalPages + "'>>></a>";

        html += "</div></td></tr></table></div>"
        $.jBox.setContent(html);
        $("#searchChannelOfSelectList").val(oldSearchChannelOfSelectList);
    })
}

function searchChannelSelectList() {
    oldSearchChannelOfSelectList = $("#searchChannelOfSelectList").val();
    ajaxChannelSelectList(oldSearchChannelOfSelectList);
}

function goChannelSelectListPage(pageObj) {
    oldSearchChannelOfSelectList = $("#searchChannelOfSelectList").val();
    ajaxChannelSelectList(oldSearchChannelOfSelectList, $(pageObj).attr("pageNumber"));
}

function openChannelSelectLayer(openLayerBut) {
    $(".showChanNalme").remove();
    var submit = function (v, h, f) {
        if (v = 'ok') {
            var selectList = h.find("#selectChannelBoxCon ul li input:checked");
            if (selectList.length < 1) {
                $.jBox.alert('请选择频道', '提示');
            }
            var idList = "";
            var nameList = "";
            var addinput = "";
            $.each(selectList, function (i, n) {
                idList += $(n).val() + ",";
                nameList += $(n).next().html() + ",";
                addinput += "<input class='showChanNalme' type='button' value='" + $(n).next().html() + "' style='background: url(/admin_mall/content/images/selectbg.gif) repeat-x center center;border: 1px solid #bec7d4;height:22px;padding:5px;margin-left:5px;'/>";
                
            });
            
            $(openChannelSelectLayerBut).parent().find(".ChannelId").val(idList.substr(0,idList.length-1));
            var channelName = nameList.substr(0, nameList.length - 1);
            
            $(openChannelSelectLayerBut).parent().after(addinput);
            $(openChannelSelectLayerBut).parent().find(".ChannelName").val(channelName);
            $.jBox.close('channelSelectLayer');
        }

    };
    $.jBox('html:channelSelectLayer', {
        id: 'channelSelectLayer',
        width: 750,
        submit: submit,
        buttons: { '确定': 'ok', '关闭': true },
        title: "选择频道"
    });
    openChannelSelectLayerBut = openLayerBut;
    ajaxChannelSelectList();

}
/*频道专区选择层结束*/

/*品牌选择层开始*/
var oldSearchBrandOfSelectList = "";
var openBrandSelectLayerBut = null;
var brandSelectLayerHtml = "<div class='selectBoxProgressBar'><p><img src='/admin_mall/content/images/progressbar.gif'/></p></div>";

function setSelectedBrand(item) {
    $(openBrandSelectLayerBut).parent().find(".BrandId").val($(item).attr("brandId"));
    var brandName = $(item).text();
    $(openBrandSelectLayerBut).val(brandName);
    $(openBrandSelectLayerBut).parent().find(".BrandName").val(brandName);
    $.jBox.close('brandSelectLayer');
}

function ajaxBrandSelectList(brandName, pageNumber) {
    $.jBox.setContent(brandSelectLayerHtml);
    $.get("/malladmin/brand/selectlist?t=" + new Date(), {
        'brandName': brandName,
        'pageNumber': pageNumber
    }, function (data) {
        var listObj = eval("(" + data + ")");
        var html = "<div id='selectBrandBox'><table width='100%' ><tr><td>品牌名称：<input type='text' id='searchBrandOfSelectList'  name='searchBrandOfSelectList' style='width:120px;height:18px;'> <input type='image' onclick='searchBrandSelectList()' src='/admin_mall/content/images/s.jpg' class='searchBut'></td></tr><tr><td><div id='selectBrandBoxCon'><ul>";
        for (var i = 0; i < listObj.items.length; i++) {
            html += "<li><a onclick='setSelectedBrand(this)' brandId='" + listObj.items[i].id + "'>" + listObj.items[i].name + "</a></li>";
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

        html += "<a href='javascript:;' class='bt' onclick='goBrandSelectListPage(this)' pageNumber='1'><<</a>";
        for (var j = startPageNumber; j <= endPageNumber; j++) {
            if (j != pageNumber) {
                html += "<a href='javascript:;' class='bt' onclick='goBrandSelectListPage(this)' pageNumber='" + j + "'>" + j + "</a>";
            }
            else {
                html += "<a href='javascript:;' class='bt hot'>" + j + "</a>";
            }
        }
        html += "<a href='javascript:;' class='bt' onclick='goBrandSelectListPage(this)' pageNumber='" + totalPages + "'>>></a>";

        html += "</div></td></tr></table></div>"
        $.jBox.setContent(html);
        $("#searchBrandOfSelectList").val(oldSearchBrandOfSelectList);
    })
}

function searchBrandSelectList() {
    oldSearchBrandOfSelectList = $("#searchBrandOfSelectList").val();
    ajaxBrandSelectList(oldSearchBrandOfSelectList);
}

function goBrandSelectListPage(pageObj) {
    oldSearchBrandOfSelectList = $("#searchBrandOfSelectList").val();
    ajaxBrandSelectList(oldSearchBrandOfSelectList, $(pageObj).attr("pageNumber"));
}

function openBrandSelectLayer(openLayerBut) {
    $.jBox('html:brandSelectLayer', {
        id: 'brandSelectLayer',
        width: 750,
        buttons: { '关闭': true },
        title: "选择品牌"
    });
    openBrandSelectLayerBut = openLayerBut;
    ajaxBrandSelectList();

}
/*品牌选择层结束*/

/*商品选择层开始*/
var oldSearchProductOfSelectList = "";
var openProductSelectLayerBut = null;
var productSelectLayerHtml = "<div class='selectBoxProgressBar'><p><img src='/admin_mall/content/images/progressbar.gif'/></p></div>";

function setSelectedProduct(item) {
    $(openProductSelectLayerBut).parent().find(".Pid").val($(item).attr("pid"));
    var productName = $(item).text();
    $(openProductSelectLayerBut).val(productName);
    $(openProductSelectLayerBut).parent().find(".ProductName").val(productName);
    $.jBox.close('productSelectLayer');
}

function ajaxProductSelectList(storeId, productName, pageNumber) {
    $.jBox.setContent(productSelectLayerHtml);
    $.get("/malladmin/product/productselectlist?t=" + new Date(), {
        'storeId': storeId,
        'productName': productName,
        'pageNumber': pageNumber
    }, function (data) {
        var listObj = eval("(" + data + ")");
        var html = "<div id='selectProductBox'><table width='100%' ><tr><td>商品名称：<input type='text' id='searchProductOfSelectList'  name='searchProductOfSelectList' style='width:120px;height:18px;'> <input type='image' onclick='searchProductSelectList(" + storeId + ")' src='/admin_mall/content/images/s.jpg' class='searchBut'></td></tr><tr><td><div id='selectProductBoxCon'><ul>";
        for (var i = 0; i < listObj.items.length; i++) {
            html += "<li><a onclick='setSelectedProduct(this)' pid='" + listObj.items[i].id + "'>" + listObj.items[i].name + "</a></li>";
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

/*店铺选择层开始*/
var oldSearchStoreOfSelectList = "";
var openStoreSelectLayerBut = null;
var storeSelectLayerHtml = "<div class='selectBoxProgressBar'><p><img src='/admin_mall/content/images/progressbar.gif'/></p></div>";

function setSelectedStore(item) {
    $(openStoreSelectLayerBut).parent().find(".StoreId").val($(item).attr("storeId"));
    var storeName = $(item).text();
    $(openStoreSelectLayerBut).val(storeName);
    $(openStoreSelectLayerBut).parent().find(".StoreName").val(storeName);
    $.jBox.close('storeSelectLayer');
}

function ajaxStoreSelectList(storeName, pageNumber) {
    $.jBox.setContent(storeSelectLayerHtml);
    $.get("/malladmin/store/storeselectlist?t=" + new Date(), {
        'storeName': storeName,
        'pageNumber': pageNumber
    }, function (data) {
        var listObj = eval("(" + data + ")");
        var html = "<div id='selectStoreBox'><table width='100%' ><tr><td>店铺名称：<input type='text' id='searchStoreOfSelectList'  name='searchStoreOfSelectList' style='width:120px;height:18px;'> <input type='image' onclick='searchStoreSelectList()' src='/admin_mall/content/images/s.jpg' class='searchBut'></td></tr><tr><td><div id='selectStoreBoxCon'><ul>";
        html+="<li><a onclick='setSelectedStore(this)' storeid='-1'>全部</a></li>";
        for (var i = 0; i < listObj.items.length; i++) {
            html += "<li><a onclick='setSelectedStore(this)' storeId='" + listObj.items[i].id + "'>" + listObj.items[i].name + "</a></li>";
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

        html += "<a href='javascript:;' class='bt' onclick='goStoreSelectListPage(this)' pageNumber='1'><<</a>";
        for (var j = startPageNumber; j <= endPageNumber; j++) {
            if (j != pageNumber) {
                html += "<a href='javascript:;' class='bt' onclick='goStoreSelectListPage(this)' pageNumber='" + j + "'>" + j + "</a>";
            }
            else {
                html += "<a href='javascript:;' class='bt hot'>" + j + "</a>";
            }
        }
        html += "<a href='javascript:;' class='bt' onclick='goStoreSelectListPage(this)' pageNumber='" + totalPages + "'>>></a>";

        html += "</div></td></tr></table></div>"
        $.jBox.setContent(html);
        $("#searchStoreOfSelectList").val(oldSearchStoreOfSelectList);
    })
}

function searchStoreSelectList() {
    oldSearchStoreOfSelectList = $("#searchStoreOfSelectList").val();
    ajaxStoreSelectList(oldSearchStoreOfSelectList);
}

function goStoreSelectListPage(pageObj) {
    oldSearchStoreOfSelectList = $("#searchStoreOfSelectList").val();
    ajaxStoreSelectList(oldSearchStoreOfSelectList, $(pageObj).attr("pageNumber"));
}

function openStoreSelectLayer(openLayerBut) {
    $.jBox('html:storeSelectLayer', {
        id: 'storeSelectLayer',
        width: 750,
        buttons: { '关闭': true },
        title: "选择店铺",
        loaded: function (h) {
            $(".jbox-button-panel").eq(1).css("height", "auto");
        }
    });
    openStoreSelectLayerBut = openLayerBut;
    ajaxStoreSelectList();

}
/*店铺选择层结束*/

/*配送公司选择层开始*/
var openShipCompanySelectLayerBut = null;
var shipCompanySelectLayerHtml = "<div class='selectBoxProgressBar'><p><img src='/admin_mall/content/images/progressbar.gif'/></p></div>";

function setSelectedShipCompany(item) {
    $(openShipCompanySelectLayerBut).parent().find(".ShipCoId").val($(item).attr("shipCoId"));
    var shipCoName = $(item).text();
    $(openShipCompanySelectLayerBut).val(shipCoName);
    $(openShipCompanySelectLayerBut).parent().find(".ShipCoName").val(shipCoName);
    $.jBox.close('shipCompanySelectLayer');
}

function ajaxShipCompanySelectList(pageNumber) {
    $.jBox.setContent(shipCompanySelectLayerHtml);
    $.get("/malladmin/shipcompany/selectlist?t=" + new Date(), {
        'pageNumber': pageNumber
    }, function (data) {
        var listObj = eval("(" + data + ")");
        var html = "<div id='selectShipCompanyBox'><table width='100%' ><tr><td><div id='selectShipCompanyBoxCon'><ul>";
        for (var i = 0; i < listObj.items.length; i++) {
            html += "<li><a onclick='setSelectedShipCompany(this)' shipCoId='" + listObj.items[i].id + "'>" + listObj.items[i].name + "</a></li>";
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

        html += "<a href='javascript:;' class='bt' onclick='goShipCompanySelectListPage(this)' pageNumber='1'><<</a>";
        for (var j = startPageNumber; j <= endPageNumber; j++) {
            if (j != pageNumber) {
                html += "<a href='javascript:;' class='bt' onclick='goShipCompanySelectListPage(this)' pageNumber='" + j + "'>" + j + "</a>";
            }
            else {
                html += "<a href='javascript:;' class='bt hot'>" + j + "</a>";
            }
        }
        html += "<a href='javascript:;' class='bt' onclick='goShipCompanySelectListPage(this)' pageNumber='" + totalPages + "'>>></a>";

        html += "</div></td></tr></table></div>"
        $.jBox.setContent(html);
    })
}

function goShipCompanySelectListPage(pageObj) {
    ajaxShipCompanySelectList($(pageObj).attr("pageNumber"));
}

function openShipCompanySelectLayer(openLayerBut) {
    $.jBox('html:shipCompanySelectLayer', {
        id: 'shipCompanySelectLayer',
        width: 750,
        buttons: { '关闭': true },
        title: "选择配送公司"
    });
    openShipCompanySelectLayerBut = openLayerBut;
    ajaxShipCompanySelectList();

}
/*配送公司选择层结束*/


/*区域省份选择层开始*/
var oldSearchRegionOfSelectList = "";
var openRegionSelectLayerBut = null;
var RegionSelectLayerHtml = "<div class='selectBoxProgressBar'><p><img src='/admin_mall/content/images/progressbar.gif'/></p></div>";

function setSelectedRegion(item) {
    $(openRegionSelectLayerBut).parent().find(".RegionId").val($(item).attr("RegionId"));
    var RegionName = $(item).text();
    $(openRegionSelectLayerBut).val(RegionName);
    $(openRegionSelectLayerBut).parent().find(".RegionName").val(RegionName);
    $.jBox.close('RegionSelectLayer');
}

//
function SelectRegion(item) {
    $(item).prev().attr("checked", true);

}
//选择市
function SelectCity(item) {
    if ($(item).parent().hasClass("hot")) {
        $(item).parent().removeClass("hot");
    } else {
        if ($(item).parent().parent().parent().prev().prev().attr("checked")==false) {
            $(item).parent().parent().parent().prev().prev().attr("checked",true)
        }
        $(item).parent().addClass("hot");
    }
}
function ajaxRegionSelectList(RegionName, pageNumber) {
    $.jBox.setContent(RegionSelectLayerHtml);
    $.get("/malladmin/store/SelectList?t=" + new Date(), {
        'pageNumber': pageNumber
    }, function (data) {
        var listObj = eval("(" + data + ")");
        var html = "<div id='selectBrandBox' style='width:1110px'><table width='100%' ><tr><td><div id='selectRegionBoxCon'><ul>";
        //var html = "<div id='selectRegionBox'><table width='100%' ><tr><td><div id='selectRegionBoxCon'><ul>";
        html += "<li><input type='checkbox' value='-1' /><a onclick='SelectRegion(this)' RegionId='-1' style='display: inline-table'>默认区域</a></li>";
        for (var i = 0; i < listObj.items.length; i++) {
            html += "<li><input type='checkbox' value='" + listObj.items[i].id + "' /><a onclick='SelectRegion(this)' RegionId='" + listObj.items[i].id + "' style='display: inline-table'>" + listObj.items[i].name + "</a>";
            html += "<div  class='cityList'> <ul>";
            for (var j = 0; j < listObj.items[i].citys.length; j++) {
                html += "<li><a onclick='SelectCity(this)' RegionId='" + listObj.items[i].id + "'  cityid='" + listObj.items[i].citys[j].cityid + "' style='display: inline-table;border:none;background:none;'>" + listObj.items[i].citys[j].cityname + "</a></li>";
            }
           
            html += "</ul></div>";
            html += "</li>";
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

        html += "<a href='javascript:;' class='bt' onclick='goRegionSelectListPage(this)' pageNumber='1'><<</a>";
        for (var j = startPageNumber; j <= endPageNumber; j++) {
            if (j != pageNumber) {
                html += "<a href='javascript:;' class='bt' onclick='goRegionSelectListPage(this)' pageNumber='" + j + "'>" + j + "</a>";
            }
            else {
                html += "<a href='javascript:;' class='bt hot'>" + j + "</a>";
            }
        }
        html += "<a href='javascript:;' class='bt' onclick='goRegionSelectListPage(this)' pageNumber='" + totalPages + "'>>></a>";

        html += "</div></td></tr></table></div>"
        $.jBox.setContent(html);
        $("#searchRegionOfSelectList").val(oldSearchRegionOfSelectList);
    })
}

function searchRegionSelectList() {
    oldSearchRegionOfSelectList = $("#searchRegionOfSelectList").val();
    ajaxRegionSelectList(oldSearchRegionOfSelectList);
}

function goRegionSelectListPage(pageObj) {
    oldSearchRegionOfSelectList = $("#searchRegionOfSelectList").val();
    ajaxRegionSelectList(oldSearchRegionOfSelectList, $(pageObj).attr("pageNumber"));
}

function openRegionSelectLayer(openLayerBut) {
    $(".showChanNalme").remove();
    var submit = function (v, h, f) {
        if (v = 'ok') {
            var selectList = h.find("#selectRegionBoxCon ul li input:checked");
            if (selectList.length < 1) {
                $.jBox.alert('请选择区域', '提示');
            }
            var idList = "";
            var nameList = "";
            var addinput = "";
            var cityidList = "";
            var cityname = "";
            $.each(selectList, function (i, n) {
                idList += $(n).val() + ",";
                nameList += $(n).next().html() ;
                var cityidSelect = $($(n)).next().next().find("ul li[class='hot']");
                var cityname="";
                if (cityidSelect.length > 0) {
                    cityname += "(";
                    $.each(cityidSelect, function (j, k) {
                        cityidList += $(k).find("a").attr("cityid") + ",";
                        cityname += $(k).find("a").html() + ",";
                    });
                    cityname = cityname.substr(0, cityname.length - 1)
                    cityname += "),";
                } else {
                    nameList += ","
                }
                nameList += cityname;
                addinput += "<input class='showChanNalme' type='button' value='" + $(n).next().html() + cityname + "' style='background: url(/admin_mall/content/images/selectbg.gif) repeat-x center center;border: 1px solid #bec7d4;height:22px;padding:5px;margin-left:5px;'/>";

            });
          
            //alert(cityidList);
            $(openRegionSelectLayerBut).parent().find(".RegionId").val(idList.substr(0, idList.length - 1));
            $(openRegionSelectLayerBut).parent().find(".CityId").val(cityidList.substr(0, cityidList.length - 1));
            var RegionName = nameList.substr(0, nameList.length - 1);

            $(openRegionSelectLayerBut).parent().after(addinput);
            $(openRegionSelectLayerBut).parent().find(".RegionName").val(RegionName);


            $.jBox.close('RegionSelectLayer');
        }

    };
    $.jBox('html:RegionSelectLayer', {
        id: 'RegionSelectLayer',
        width: 1100,
        top:10,
        submit: submit,
        buttons: { '确定': 'ok', '关闭': true },
        title: "选择区域"
    });
    openRegionSelectLayerBut = openLayerBut;
    ajaxRegionSelectList();

}
/*省份区域选择层结束*/

/*优惠券选择层开始*/
var oldSearchCouponOfSelectList = "";
var openCouponSelectLayerBut = null;
var CouponSelectLayerHtml = "<div class='selectBoxProgressBar'><p><img src='/admin_mall/content/images/progressbar.gif'/></p></div>";

function setSelectedCoupon(item) {
    $(openCouponSelectLayerBut).parent().find(".CouponTypeId").val($(item).attr("coupontypeid"));
    var CouponName = $(item).text();
    $(openCouponSelectLayerBut).val(CouponName);
    $(openCouponSelectLayerBut).parent().find(".CouponName").val(CouponName);
    $.jBox.close('CouponSelectLayer');
}

function ajaxCouponSelectList(Pid, CouponName, pageNumber) {
    $.jBox.setContent(CouponSelectLayerHtml);
    $.get("/malladmin/Coupon/CouponTypeSelectList?t=" + new Date(), {
        'pId': Pid,
        'couponTypeName': CouponName,
        'pageNumber': pageNumber
    }, function (data) {
        var listObj = eval("(" + data + ")");
        var html = "<div id='selectProductBox'><table width='100%'><tr><td><div id='selectCouponBoxCon'><ul>";
        for (var i = 0; i < listObj.items.length; i++) {
            html += "<li><a onclick='setSelectedCoupon(this)' coupontypeid='" + listObj.items[i].id + "'>" + listObj.items[i].name + "(面值：" + listObj.items[i].money+ ")</a></li>";
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

        html += "<a href='javascript:;' class='bt' onclick='goCouponSelectListPage(this," + Pid + ")' pageNumber='1'><<</a>";
        for (var j = startPageNumber; j <= endPageNumber; j++) {
            if (j != pageNumber) {
                html += "<a href='javascript:;' class='bt' onclick='goCouponSelectListPage(this," + Pid + ")' pageNumber='" + j + "'>" + j + "</a>";
            }
            else {
                html += "<a href='javascript:;' class='bt hot'>" + j + "</a>";
            }
        }
        html += "<a href='javascript:;' class='bt' onclick='goCouponSelectListPage(this," + Pid + ")' pageNumber='" + totalPages + "'>>></a>";

        html += "</div></td></tr></table></div>"
        $.jBox.setContent(html);
        $("#searchCouponOfSelectList").val(oldSearchCouponOfSelectList);
    })
}

function searchCouponSelectList(Pid) {
    oldSearchCouponOfSelectList = $("#searchCouponOfSelectList").val();
    ajaxCouponSelectList(Pid, oldSearchCouponOfSelectList);
}

function goCouponSelectListPage(pageObj, Pid) {
    oldSearchCouponOfSelectList = $("#searchCouponOfSelectList").val();
    ajaxCouponSelectList(Pid, oldSearchCouponOfSelectList, $(pageObj).attr("pageNumber"));
}

function openCouponSelectLayer(openLayerBut) {
    var pid=$("#Pid").val();
    if (pid>0) {
    $.jBox('html:CouponSelectLayer', {
        id: 'CouponSelectLayer',
        width: 950,
        buttons: { '关闭': true },
        title: "选择优惠券"
    });
    openCouponSelectLayerBut = openLayerBut;
    ajaxCouponSelectList(pid);
    } else {
        alert("请先选择商品");
    }

}
/*优惠券选择层结束*/