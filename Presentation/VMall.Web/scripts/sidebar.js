
$(function () {
    //导航
    $("#channel3").mouseover(function () {
        $(this).text("敬请期待").attr("href", "javascript:void(0);");
    });
    $("#channel3").mouseout(function () {
        $(this).text("旅游专区");
    });
    //热销产品推荐
    $(".hotchange .hotindex").mouseover(function () {
        var i = $(this).index();
        var hotactive = "#686868";
        var hotbg = "#BBB9BA";
        $(this).css("background", hotactive).siblings().css("background", hotbg);
        $(".itemdiv").eq(i).show().siblings('.itemdiv').hide();
    });

});

$(function () {
    //手机端
    $('#showappcode,#appweixincode').mouseover(function () {
        $("#appweixincode").show().stop(true, false).animate({ top: "40px" }, 100);
        $("#showappcode").css("background-color", "#EEEEEE");
    });
    $('#showappcode,#appweixincode').mouseleave(function () {
        $("#appweixincode").stop(true, false).animate({ top: "30px" }, 200, function () {
            $("#appweixincode").hide();
            $("#showappcode").css("background-color", "#DDDDDD");
        });
    });
    var isAlreadyLoadCartList = false;
    //购物车
    $('#shoppingcart,#cartlist').mouseover(function () {

        if (isGuestSC == 0 && uid < 1) {
            return;
        }
        var cartlist = $("#cartlist");
        $("#shoppingcart").css("background", "#EEEEEE");
        if (!isAlreadyLoadCartList) {
            isAlreadyLoadCartList = true;
            Ajax.get("/cart/topcart", false, function (data) {
                $("#cartlist").html(data);
            });
        }

        $("#cartlist").show().stop(true, false).animate({ top: "30px" }, 100);
    });
    $('#shoppingcart,#cartlist').mouseleave(function () {
        $("#shoppingcart").css("background", "#DDDDDD");
        $("#cartlist").stop(true, false).animate({ top: "30px" }, 100, function () {
            $("#cartlist").hide();
        });

    });

    //$('#shoppingcart,#cartlist').hover(function () {
    //    var isAlreadyLoadCartList = false;
    //    if (isGuestSC == 0 && uid < 1) {
    //        return;
    //    }
    //    var cartlist = $("#cartlist");

    //    if (!isAlreadyLoadCartList) {
    //        isAlreadyLoadCartList = true;
    //        Ajax.get("/cart/topcart", false, function (data) {
    //            $("#cartlist").html(data);
    //        })
    //    }
    //    $("#cartlist").show().stop(true, false).animate({ top: "30px" }, 100);
    //},function(){
    //    //alert(11);
    //    //$("#cartlist").hide().stop(true, false).animate({ top: "25px" }, 100);
    //});

    //全部分类
    $(".nav-left").mouseover(function () {
        $("#showcateory").removeClass().stop(true, false).animate({ top: "35px" }, 300);
    });
    $(".nav-left").mouseleave(function () {
        $("#showcateory").stop(true, false).animate({ top: "0px" }, 200, function () {
            $("#showcateory").addClass('hide');
        });
    });

});

function deleteCartProduct(pid, pos) {
    if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
        return;
    }
    Ajax.get("/cart/delpruduct?pid=" + pid + "&pos=" + pos, false, function (data) {
        $("#cartlist").html(data);
        $(".shop-cart").html($(".cartProudctCount").html());
    })
    //$("#cartlist").show().stop(true, false).animate({ top: "30px" }, 100);
}

$(function () {
    //侧边栏会员
    $("#usermessage,#usermessage p,#showmessage").mouseover(function () {
        $("#showmessage").show().stop(true, false).animate({ left: "-105px" }, 200);
        $("#usermessage").css("background", "#D02424");
        //$("#usermessage p img").attr("src", "/images/usermessage-2.png");
        //$("#usermessage p span").css({ color: "#0D9F9F", background: "#FFFFFF" });
    });
    $("#usermessage,#showmessage").mouseleave(function () {
        $("#showmessage").stop(true, false).animate({ left: "-5px" }, 500, function () {
            $("#showmessage").hide();
            $("#usermessage").css("background", "#555555");
            //$("#usermessage p img").attr("src", "/images/usermessage-1.png");
            //$("#usermessage p span").css({ color: "#FFFFFF", background: "#0D9F9F" });
        });
    });

    //侧边栏购物车
    $("#usercart,#usercart p,#showcart").mouseover(function () {
        $("#showcart").show().stop(true, false).animate({ left: "-105px" }, 200);
        $("#usercart").css("background", "#D02424");
        //$("#usercart p img").attr("src", "/images/usercart-2.png");
        //$("#usercart p span").css({ color: "#0D9F9F", background: "#FFFFFF" });
    });
    $("#usercart,#showcart").mouseleave(function () {
        $("#showcart").stop(true, false).animate({ left: "-5px" }, 500, function () {
            $("#showcart").hide();
            $("#usercart").css("background", "#555555");
            //$("#usercart p img").attr("src", "/images/usercart-1.png");
            //$("#usercart p span").css({ color: "#FFFFFF", background: "#0D9F9F" });
        });
    });

    //侧边栏在线客服
    $("#onlineservice,#onlineservice p,#showservice").mouseover(function () {
        $("#showservice").show().stop(true, false).animate({ left: "-105px" }, 200);
        $("#onlineservice").css("background", "#D02424");
       // $("#onlineservice p img").attr("src", "/images/onlineservice-2.png");
    });
    $("#onlineservice,#showservice").mouseleave(function () {
        $("#showservice").stop(true, false).animate({ left: "-5px" }, 500, function () {
            $("#showservice").hide();
            $("#onlineservice").css("background", "#555555");
           // $("#onlineservice p img").attr("src", "/images/onlineservice-1.png");
        });
    });

    //侧边栏微信扫码
    $("#weixincode,#weixincode p,#showcode").mouseover(function () {
        $("#showcode").show().stop(true, false).animate({ left: "-130px" }, 200);
        $("#weixincode").css("background", "#D02424");
        //$("#weixincode p img").attr("src", "/images/weixincode-2.png");
    });
    $("#weixincode,#showcode").mouseleave(function () {
        $("#showcode").stop(true, false).animate({ left: "-5px" }, 500, function () {
            $("#showcode").hide();
            $("#weixincode").css("background", "#555555");
            //$("#weixincode p img").attr("src", "/images/weixincode-1.png");
        });
    });

    //返回顶部

    $("#returntop,#returntop p").mouseover(function () {
        //$("#returntop p img").attr("src", "/images/returntop-2.png");
        $("#returntop").css("background", "#D02424");
    });
    $("#returntop,#returntop p").mouseleave(function () {
        $("#returntop").css("background", "#525252");
        //$("#returntop p img").attr("src", "/images/returntop-1.png");

    });

    //window.onscroll = function () {
    //    var scroll_top = document.documentElement.scrollTop || document.body.scrollTop;
    //    if (parseInt(scroll_top) < 450) {
    //        $("#returntop").fadeOut(200);
    //    }
    //    else {
    //        $("#returntop").fadeIn(200);
    //    }
    //}
});

function ToQQService() {
    var QQArray = new Array("3352854566", "2016673279", "3020155643", "2073989186", "3319605900");
    var index = Math.floor((Math.random() * QQArray.length));
    window.open("http://wpa.qq.com/msgrd?v=3&uin=" + QQArray[index] + "&site=qq&menu=yes");
}


