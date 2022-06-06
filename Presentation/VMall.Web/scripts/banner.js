var n = 0, count;
var t;
var b = true;
var page = $('.banner .others .pagination');
//var prevBtn = $(".banner .slider .prev");
//var nextBtn = $(".banner .slider .next");
var sliderLi = $(".banner .slider  li");

count = sliderLi.length;

//初始化
sliderLi.not(":first-child").hide();

for (j = 0; j < sliderLi.length; j++) {
    var pi = "";
    if (j == 0) {
        pi = '<i>1</i>';
    } else {
        pi = '<i></i>';
    }
    page.append(pi);
};
var pageIndex = $(".banner .others .pagination i");
pageIndex.eq(0).addClass("on");

//渐隐
function fade(n) {
    sliderLi.eq(n).fadeIn(800).siblings().fadeOut(400);
}

//分页
function pageChange(m) {
    pageIndex.eq(m).addClass('on').siblings().removeClass('on');
    pageIndex.eq(m).html(m + 1).siblings().html("");
}

//上一张
//prevBtn.click(function () {
//    if (t) {
//        clearInterval(t);
//        if (n <= 0) {
//            n = count - 1;
//        } else {
//            n--;
//        }
//        fade(n);
//        pageChange(n);
//    };
//    t = setInterval(showAuto, 5000);
//});

//下一张
//nextBtn.click(function () {
//    //if (b) {
//    //    b = false;
//    if (t) {
//        clearInterval(t);
//        if (n >= sliderLi.length - 1) {
//            n = 0;
//        } else {
//            n++;
//        }
//        fade(n);
//        pageChange(n);
//    };
//    t = setInterval(showAuto, 5000);
//    //}
//});

//圆点点击
pageIndex.click(function () {
    if (t) {
        clearInterval(t);
        var i = $(this).index(); //获取Li元素内的值，即012
        n = i;
        fade(n);
        pageChange(n);
    };
    t = setInterval(showAuto, 5000);
});

//鼠标悬停停止
$(".banner .slider .bannerlist").hover(function () {
    if (t) {
        clearInterval(t)
    }
    //prevBtn.show();
    //nextBtn.show();
}, function () {
    //prevBtn.hide();
    //nextBtn.hide();
    t = setInterval(showAuto, 5000);
});

//自动播放
function showAuto() {
    if (t) {
        clearInterval(t);
        if (n >= sliderLi.length - 1) {
            n = 0;
        } else {
            n++;
        }
        fade(n);
        pageChange(n);
    }
};
t = setInterval(showAuto, 5000);



//导航
$(function () {

    $("#channel3").mouseover(function () {
        $(this).text("敬请期待").attr("href", "javascript:void(0);");

    });
    $("#channel3").mouseout(function () {
        $(this).text("旅游专区");

    });
});