var returnUrl = "/"; //返回地址
var shadowName = ""; //影子账号名

$(function () {
    document.onkeydown = function (ev) {
        var oEvent = ev || event;
        if (!ev) ev = window.event;
        if ((ev.keyCode || ev.which) == 13) {
            login();
        }

        //if (oEvent.keyCode == 116) {
        //    $('input[type="text"]').val('').blur();
        //}
    };
})
//展示验证错误
function showVerifyError(verifyErrorList) {
    if (verifyErrorList != undefined && verifyErrorList != null && verifyErrorList.length > 0) {
        var msg = "";
        var key = "";
        for (var i = 0; i < verifyErrorList.length; i++) {
            msg += verifyErrorList[i].msg + "\n";
            key += verifyErrorList[i].key;
        }
        alert(msg);
        if (key == "accountName")
            $(".userName").focus();
        else if (key == "password")
            $(".passWord").focus();
        else if (key == "verifyCode")
            $(".YZM").focus();
        else if (key == "pnameError")
            $("#pname").focus();
        $("#verifyImage").click();
    }
}

//用户登录
function login() {
    var loginForm = document.forms["loginForm"];

    var accountName = loginForm.elements[shadowName].value;
    var password = loginForm.elements["password"].value;
    var verifyCode = loginForm.elements["verifyCode"] ? loginForm.elements["verifyCode"].value : undefined;
    var isRemember = loginForm.elements["isRemember"] ? loginForm.elements["isRemember"].checked ? 1 : 0 : 0;

    if (!verifyLogin(accountName, password, verifyCode)) {
        return;
    }
    $(".loginBt .loginIn").html("正在登录...").removeAttr("onclick");
    var parms = new Object();
    parms[shadowName] = $.trim(accountName);
    parms["password"] = password;
    parms["verifyCode"] = verifyCode;
    parms["isRemember"] = isRemember;
    Ajax.post("/account/login", parms, false, loginResponse)
}

//验证登录
function verifyLogin(accountName, password, verifyCode) {
    if (accountName.length == 0) {
        alert("请输入帐号名");
        $(".userName").focus();
        return false;
    }
    if ($.trim(accountName).length < 2 || accountName.Length > 50) {
        alert("账户名必须大于2且不大于50个字符");
        $(".userName").focus();
        return false;
    }
    var regName = /[\u4e00-\u9fa5]+/gi;
    if (regName.test(accountName)) {
        alert("账户名不能包含中文字符");
        $(".userName").focus();
        return false;
    }
    if (password.length == 0) {
        alert("请输入密码");
        $(".passWord").focus();
        return false;
    }
    if (password.length < 6) {
        alert("密码长度必须大于6位");
        $(".passWord").focus();
        return false;
    }
    if (verifyCode != undefined && verifyCode.length == 0) {
        alert("请输入验证码");
        $(".YZM").focus();
        return false;
    }
    return true;
}

//处理登录的反馈信息
function loginResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        window.location.href = returnUrl;
    }
    else if (result.state == "lockuser") {
        alert(result.content);
    }
    else if (result.state == "error") {

        showVerifyError(result.content);
        //$(".userName").focus();
        $(".loginBt .loginIn").html("登 &nbsp; 陆").attr("onclick", "login();");
    }
}

//用户注册
function register() {
    var registerForm = document.forms["registerForm"];

    var accountName = registerForm.elements[shadowName].value;
    var password = registerForm.elements["password"].value;
    var confirmPwd = registerForm.elements["confirmPwd"].value;
    var verifyCode = registerForm.elements["verifyCode"] ? registerForm.elements["verifyCode"].value : undefined;
    var pname = registerForm.elements["pname"].value;

    if (!verifyRegister(accountName, password, confirmPwd, pname, verifyCode)) {
        return;
    }

    $(".reg .regBt").html("正在注册...").removeAttr("onclick");
    //var pname = getUrlParamValue("pname");
    var parms = new Object();
    parms[shadowName] = accountName;
    parms["password"] = password;
    parms["confirmPwd"] = confirmPwd;
    parms["verifyCode"] = verifyCode;
    Ajax.post("/account/register?pname=" + pname, parms, false, registerResponse)
}

//用户注册-新
function registerNew() {
    var mobile = $(".userName").val();
    var verifyCode = $(".YZM").val();
    var parentName = $.trim($(".pname").val());
    if (!verifyRegister(mobile, parentName, verifyCode)) {
        return;
    }
    $(".reg .regBt").html("正在注册...").removeAttr("onclick");
    var parms = new Object();
    parms[shadowName] = $.trim(mobile);
    parms["pname"] = parentName;
    parms["verifyCode"] = verifyCode;
    Ajax.post("/account/register?pname=" + parentName, parms, false, registerResponse)
}
//根据Url参数名称返回参数值
function getUrlParamValue(paramName) {
    paramValue = "";
    isFound = false;
    if (this.location.search.indexOf("?") == 0 && this.location.search.indexOf("=") > 1) {
        arrSource = unescape(this.location.search).substring(1, this.location.search.length).split("&");
        i = 0;
        while (i < arrSource.length && !isFound) {
            if (arrSource[i].indexOf("=") > 0) {
                if (arrSource[i].split("=")[0].toLowerCase() == paramName.toLowerCase()) {
                    paramValue = arrSource[i].split("=")[1];
                    isFound = true;
                }
            }
            i++;
        }
    }
    return paramValue;
}
//验证注册
function verifyRegister(accountName, pname, verifyCode) {
    if (accountName.length == 0) {
        alert("请输入帐号名");
        $(".userName").focus();
        return false;
    }
    var mobileReg = /^1[3|4|5|7|8][0-9]\d{8}$/;
    if (!mobileReg.test($.trim(accountName))) {
        alert('请输入正确的手机号码!!!');
        $(".userName").focus();
        return false;
    }
    if ($.trim(pname) == "") {
        alert("推荐人编号不能为空");
        $("#pname").focus();
        return false;
    }
    if (verifyCode != undefined && verifyCode.length == 0) {
        alert("请输入验证码");
        $(".YZM").focus();
        return false;
    }
    return true;
}

//处理注册的反馈信息
function registerResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        window.location.href = returnUrl;
    }
    else if (result.state == "exception") {
        alert(result.content);
    }
    else if (result.state == "error") {
        showVerifyError(result.content);
        //$(".userName").focus();
        $(".reg .regBt").html("立即注册").attr("onclick", "register();");
    }
}

//找回密码
function findPwd() {
    var findPwdForm = document.forms["findPwdForm"];

    var accountName = findPwdForm.elements[shadowName].value;
    var verifyCode = findPwdForm.elements["verifyCode"].value;

    if (!verifyFindPwd(accountName, verifyCode)) {
        return;
    }

    var parms = new Object();
    parms[shadowName] = accountName;
    parms["verifyCode"] = verifyCode;
    Ajax.post("/account/findpwd", parms, false, findPwdResponse)
}

//验证找回密码
function verifyFindPwd(accountName, verifyCode) {
    if (accountName.length == 0) {
        alert("请输入帐号名");
        return false;
    }
    if ($.trim(accountName).length < 2 || accountName.Length > 50) {
        alert("账户名必须大于2且不大于50个字符");
        $(".userName").focus();
        return false;
    }
    var regName = /[\u4e00-\u9fa5]+/gi;
    if (regName.test(accountName)) {
        alert("账户名不能包含中文字符");
        $(".userName").focus();
        return false;
    }
    if (verifyCode != undefined && verifyCode.length == 0) {
        alert("请输入验证码");
        return false;
    }
    return true;
}

//处理找回密码的反馈信息
function findPwdResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        window.location.href = result.content;
    }
    else if (result.state == "nocanfind") {
        alert(result.content);
    }
    else if (result.state == "error") {
        showVerifyError(result.content);
    }
}

function timeCheckForFindPwd(n, o, param) {
    var t = n;
    if (n == 0) {
        $(o).attr("href", "javascript:sendFindPwdMobile(" + param + ")");
        $(o).css("color", "#FFFFFF").html('重新发送');
        n = t;
    } else {
        n--;
        $(o).html('重新发送&nbsp;' + n);
        setTimeout(function () {
            timeCheckForFindPwd(n, o, param);
        }, 1000);
    }
}

//发送找回密码短信
function sendFindPwdMobile(uid) {

    Ajax.get("/account/sendfindpwdmobile?uid=" + uid, false, function (data) {
        var result = eval("(" + data + ")");
        if (result.state == "success") {
            $("#sendSuccess").show();
            $(".sendMobileCode").attr("href", "javascript:void(0);").css("color", "#C1C1C1");
            timeCheckForFindPwd(120, $(".sendMobileCode"), uid);

        } else {
            alert(result.content)
        }
    })
}



//验证找回密码短信
function verifyFindPwdMobile(uid, mobileCode) {
    if (mobileCode.length == 0) {
        alert("请输入短信验证码");
        return;
    }
    Ajax.post("/account/verifyfindpwdmobile?uid=" + uid, { 'mobileCode': mobileCode }, false, function (data) {
        var result = eval("(" + data + ")");
        if (result.state == "success") {
            window.location.href = result.content;
        }
        else {
            alert(result.content)
        }
    })
}



//发送找回密码邮件
function sendFindPwdEmail(uid) {
    Ajax.get("/account/sendfindpwdemail?uid=" + uid, false, function (data) {
        var result = eval("(" + data + ")");
        alert(result.content)
    })
}

//重置用户密码
function resetPwd(v) {
    var resetPwdForm = document.forms["resetPwdForm"];

    var password = resetPwdForm.elements["password"].value;
    var confirmPwd = resetPwdForm.elements["confirmPwd"].value;

    if (!verifyResetPwd(password, confirmPwd)) {
        return;
    }

    var parms = new Object();
    parms["password"] = password;
    parms["confirmPwd"] = confirmPwd;
    Ajax.post("/account/resetpwd?v=" + v, parms, false, resetPwdResponse)
}

//验证重置密码
function verifyResetPwd(password, confirmPwd) {
    if (password.length == 0) {
        alert("请输入密码");
        return false;
    }
    if (password != confirmPwd) {
        alert("两次输入的密码不一样");
        return false;
    }
    return true;
}

//处理验证重置密码的反馈信息
function resetPwdResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        alert("密码修改成功,请重新登录");
        window.location.href = result.content;
    }
    else if (result.state == "error") {
        showVerifyError(result.content);
    }
}