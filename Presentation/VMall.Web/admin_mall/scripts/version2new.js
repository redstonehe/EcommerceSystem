$(function () {
    var textArray = $(".rightH1").text().split(/[\n]/);
    var breadcrumbText = "";
    $.each(textArray, function (i, n) {
        if($.trim(n)!=""){
            //alert(n);
            breadcrumbText = n;
            return false;
        }
    });
    var navArray = breadcrumbText.split(">>");

    var text = "";
    text += "<ol class=\"breadcrumb\">";
    text += "            <li><a href=\"#\"><i class=\"fa fa-home\"></i>首页</a></li>";
    text += "            <li><a href=\"#\">" + navArray[0] + "</a></li>";
    text += "            <li class=\"active\">" + navArray[1] + "</li>";
    text += "        </ol>";
    $(".rightH1").prepend(text);

    $('.rightH1').contents().filter(function () {
        return this.nodeType == 3;
    }).remove();

     $(".rightH1 .right a").each(function (i) {
         //alert($(this).html());
         var imgBtn=$(this).find("img");
         
         if (imgBtn.attr("src").indexOf("goback.jpg") != -1) {
             $(this).removeClass("menuBT").addClass("btn btn-primary btn-sm border-radius-lg").prepend("<i class='fa fa-reply fa-lg'></i>");
         }
         if (imgBtn.attr("src").indexOf("edit.jpg") != -1) {
             $(this).removeClass("menuBT").addClass("btn btn-primary btn-sm border-radius-lg").prepend("<i class='fa fa-sitemap fa-lg'></i>");
         }
         else if (imgBtn.attr("src").indexOf("add.jpg") != -1) {
             $(this).removeClass("menuBT").addClass("btn btn-primary btn-sm border-radius-lg").prepend("<i class='fa fa-plus-circle fa-lg'></i>");
         }
         imgBtn.remove();
       
    })
    //$(".rightH1 .right a img").remove();
    //$(".rightH1 .right a").removeClass("menuBT").addClass("btn btn-primary btn-sm border-radius-lg").prepend("<i class='fa fa-reply fa-lg'></i>");
});