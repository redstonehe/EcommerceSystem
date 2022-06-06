

function Tab() {

    var config = {
        loadingIndex: 0,
        max: 15,
        current: -1,
        notcloseClass: 'content-main',
        activeClass: 'active'
    };

    function getTabIndex() {
        if (config.current < config.max) {
            config.current++;
        }
        return config.current;
    }
    function sumWidth(WidthObjList) {
        var width = 0;
        $(WidthObjList).each(function () {
            width += $(this).outerWidth(true)
        });
        return width
    };
    function getCurrentTab() {
        return $('#J_mainTabs a.' + config.activeClass);
    }
    function getTab(url) {
        return $('#J_mainTabs a[href="' + url + '"]');
    }
    function getIframe(url) {
        return $('#J_mainContent iframe[src="' + url + '"]');
    }
    function getTabHtml(param) {
        var html = '<a href="' + param.url + '" class="active J_menuTab">' + param.title;
        if (param.close) html += '&nbsp;&nbsp;&nbsp;<span class="fa fa-refresh"></span>&nbsp;&nbsp;&nbsp;<i class="fa fa-times-circle"></i>';
        html += '</a>';
        return html;
    }
    function getIframeHtml(url) {
        var html = '<iframe class="J_content active" width="100%" height="100%" frameborder="0" src="' + url + '" seamless></iframe>';
        return html;
    }
    function tabCheck(tag) {
        var param = getParam(tag);
        var url = param.url;

        if ($.trim(url).length == 0) return false

        var active = config.activeClass;
        var tab = getTab(url);
        var content = getIframe(url);
        if (tab.length) {
            if (!tab.hasClass(active)) {
                $(tab).addClass(active).siblings().removeClass(active);
            }
            if (content.length) {
                $(content).addClass(active).show().siblings().hide();
            }
        }
        else {
            tabCreate(param);
        }
        return false
    };
    function tabCreate(param) {
        var index = getTabIndex();
        var tabs = $('#J_mainTabs .page-tabs-content');

        tabs.find('a').removeClass(config.activeClass);
        tabs.append(getTabHtml(param));

        $('#J_mainContent .J_content').removeClass('active').hide();

        loading(true);
        var content = $(getIframeHtml(param.url));
        $('#J_mainContent').append(content);
        content.load(function () {
            loading(false);
        }).show();
    };
    function moveLeft() {
        var tabMarginLeft = Math.abs(parseInt($("#J_mainTabs .page-tabs-content").css("margin-left")));
        var otherWidth = sumWidth($('#J_mainTabs').children().not(".J_menuTabs"));
        var tabZoneWidth = $('#J_mainTabs').outerWidth(true) - otherWidth;
        var px = 0;
        if ($("#J_mainTabs .page-tabs-content").width() < tabZoneWidth) {
            return false
        } else {
            var tabs = $("#J_mainTabs .J_menuTab:first");
            var menuTabs = 0;
            while ((menuTabs + $(tabs).outerWidth(true)) <= tabMarginLeft) {
                menuTabs += $(tabs).outerWidth(true);
                tabs = $(tabs).next()
            }
            menuTabs = 0;
            if (sumWidth($(tabs).prevAll()) > tabZoneWidth) {
                while ((menuTabs + $(tabs).outerWidth(true)) < (tabZoneWidth) && tabs.length > 0) {
                    menuTabs += $(tabs).outerWidth(true);
                    tabs = $(tabs).prev()
                }
                px = sumWidth($(tabs).prevAll())
            }
        }
        $("#J_mainTabs .page-tabs-content").animate({
            marginLeft: 0 - px + "px"
        }, "fast")
    }
    function moveRight() {
        var tabMarginLeft = Math.abs(parseInt($("#J_mainTabs .page-tabs-content").css("margin-left")));
        var otherWidth = sumWidth($('#J_mainTabs').children().not(".J_menuTabs"));
        var tabZoneWidth = $('#J_mainTabs').outerWidth(true) - otherWidth;
        var px = 0;
        if ($("#J_mainTabs .page-tabs-content").width() < tabZoneWidth) {
            return false
        } else {
            var tabs = $("#J_mainTabs .J_menuTab:first");
            var menuTabs = 0;
            while ((menuTabs + $(tabs).outerWidth(true)) <= tabMarginLeft) {
                menuTabs += $(tabs).outerWidth(true);
                tabs = $(tabs).next()
            }
            menuTabs = 0;
            while ((menuTabs + $(tabs).outerWidth(true)) < (tabZoneWidth) && tabs.length > 0) {
                menuTabs += $(tabs).outerWidth(true);
                tabs = $(tabs).next()
            }
            px = sumWidth($(tabs).prevAll());
            if (px > 0) {
                $("#J_mainTabs .page-tabs-content").animate({
                    marginLeft: 0 - px + "px"
                }, "fast")
            }
        }
    }
    function tabClose(tag) {
        var active = config.activeClass;
        var current = tag.parents('.J_menuTab');
        var url = current.attr('href');
        if (current.hasClass(active)) {
            var next = current.next();
            var prev = current.prev();
            if (next.length > 0) {
                next.addClass(active);
            }
            else if (prev.length > 0) {
                prev.addClass(active);
            }
        }
        var content = getIframe(url);
        content.remove();
        current.remove();
        tabCheck(getCurrentTab());
        return false
    }
    function tabCloseAll(all) {
        var as = $("#J_mainTabs .page-tabs-content a").not('.' + config.notcloseClass);
        if (!all) as = as.not('.' + config.activeClass);
        as.each(function () {
            var tab = $(this);
            url = tab.attr('href');
            tab.remove();
            var content = getIframe(url);
            content.remove();
        });
        $("#J_mainTabs .page-tabs-content").css("margin-left", "0");
        var tab = $('#J_mainTabs .page-tabs-content a:last');
        tabCheck(tab);
    }
    function tabEnable() {
        if (!$(this).hasClass("active")) {
            tabCheck($(this));
        }
        return false;
    }
    function getParam(tag) {
        var href = tag.attr('href');
        var param = {
            close: true,
            url: href,
            title: $.trim(tag.text())
        };
        return param;
    }
    function loading(flag) {
        //NProgress.start();
        //setTimeout(function () { NProgress.done(); $('.fade').removeClass('out'); }, 1000);
        if (flag) {
            //NProgress.start();
            //setTimeout(function () { config.loadingIndex = layer.load(1); }, 1000);
            config.loadingIndex = layer.load(2, { time: 1 * 1000, shade: [0.2, '#393D49'] });
        }
        else {
            //NProgress.done();
            layer.close(config.loadingIndex);
            //setTimeout(function () { layer.close(config.loadingIndex); }, 500);
        }
    }
    function init() {
        $(".J_menuItem").on("click", function () {
            //console.log($(this).parent().html());
            $(this).parent().addClass("active").siblings().removeClass("active");
            tabCheck($(this));
            return false;
        });
        $("#J_mainTabs .J_tabReloadCurrent").on("click", function () {
            var tab = getCurrentTab();
            var url = tab.attr('href');
            var content = getIframe(url);
            loading(true);
            content.attr('src', url).load(function () {
                loading(false);
            });
        });
        $("#J_mainTabs .J_tabCloseCurrent").on("click", function () {
            var tab = $("#J_mainTabs .J_menuTab.active i");
            tabClose(tab);
        });
        $("#J_mainTabs .J_menuTabs").on("click", ".J_menuTab i", function () {
            tabClose($(this));
            return false;
        });
        $("#J_mainTabs .J_menuTabs").on("click", ".J_menuTab span", function () {
            var tab = getCurrentTab();
            var url = tab.attr('href');
            var content = getIframe(url);
            loading(true);
            content.attr('src', url).load(function () {
                loading(false);
            });
            return false;
        });
        $("#J_mainTabs .J_menuTabs").on("dblclick", ".J_menuTab", function () {
            var tab = getCurrentTab();
            var url = tab.attr('href');
            var content = getIframe(url);
            loading(true);
            content.attr('src', url).load(function () {
                loading(false);
            });
            return false;
        });
        $("#J_mainTabs .J_tabCloseOther").on("click", function () {
            tabCloseAll(false);
        });
        $("#J_mainTabs .J_menuTabs").on("click", ".J_menuTab", tabEnable);
        $("#J_mainTabs .J_tabLeft").on("click", moveLeft);
        $("#J_mainTabs .J_tabRight").on("click", moveRight);
        $("#J_mainTabs .J_tabCloseAll").on("click", function () {
            tabCloseAll(true);
        })
    }

    init();
}

$(function () {
    var tab = new Tab();
});

//var dynamicProgress = (function () {
//    var div = document.createElement('div');
//    div.className = "progressBar";
//    var style = div.style;
//    style.width = "0%";
//    style.background = "#da251d";
//    style.position = "fixed";
//    style.zIndex = "100";
//    style.transition = "width .4s";
//    document.body.prepend(div)

//    var start = function () {
//        style.height = '2px'
//        style.width = '90%'
//    }
//    var end = function () {
//        style.width = '100%'
//        setTimeout(function () {
//            style.height = '0px'
//            style.width = '0%'
//        }, 400)
//    }
//    return { start: start, end: end }

//})()