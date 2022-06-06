Date.prototype.format = function (format) {
    /* 
    * eg:format="YYYY-MM-dd hh:mm:ss"; 
    */
    var o = {
        "M+": this.getMonth() + 1,  //month  
        "d+": this.getDate(),     //day  
        "h+": this.getHours(),    //hour  
        "m+": this.getMinutes(),  //minute  
        "s+": this.getSeconds(), //second  
        "q+": Math.floor((this.getMonth() + 3) / 3),  //quarter  
        "S": this.getMilliseconds() //millisecond  
    }

    if (/(y+)/.test(format)) {
        format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }

    for (var k in o) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
        }
    }
    return format;
}

function ConertJsonTimeAndFormat(jsonTime, format) {
    //var reg = /-?\d+/gi;
    //var r = jsonTime.match(reg);
    //return new Date(Number(r[0])).format(format);
    return new Date(Number(jsonTime.match(/-?\d+/gi)[0])).format(format);
    //return new Date(eval(jsonTime.replace(/\/Date(\d+)\//gi, "new Date($1)"))).format(format);
}

function JsonTimeConert(jsonTime) {
    var reg = /-?\d+/gi;
    var r = jsonTime.match(reg);
    var data = new Date(Number(r[0]));

    return data.getFullYear() + "/" +
        ((data.getMonth() + 1) < 10 ? "0" + (data.getMonth() + 1) : (data.getMonth() + 1)) + "/" +
        (data.getDate() < 10 ? "0" + data.getDate() : data.getDate()) + " " +
        (data.getHours() < 10 ? "0" + data.getHours() : data.getHours()) + ":" +
        (data.getMinutes() < 10 ? "0" + data.getMinutes() : data.getMinutes()) + ":" +
        (data.getSeconds() < 10 ? "0" + data.getSeconds() : data.getSeconds());
}

//var reg = /-?\d+/g; var jsonTime = "\/Date(-1457056884000)\/";
//var r = jsonTime.match(reg);; alert(r);alert(new Date(Number(r[0])))