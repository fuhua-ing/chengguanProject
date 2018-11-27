//是否有效渲染
function formatter_valid(value, row, index) {
    if (value != null) {
        if (value.toString() == "1")
            return '<span class="label label-success">有效</span>';
        else
            return '<span class="label label-danger">无效</span>';
    }
}
//是否渲染
function formatter_YesOrNo(value, row, index) {
    if (parseInt(value) == 1)
        return '<span class="label label-success">是</span>';
    else
        return '<span class="label label-danger">否</span>';
}
//数字渲染
function formatter_count(value, row, index) {
    if (parseInt(value) == 0)
        return '<span class="label label-danger">' + value + '</span>';
    else
        return '<span class="label label-success">' + value + '</span>';
}

//获取url参数
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}
//得到根窗口对象
function getRootWindow() {
    var w = window;
    while (w.parent.location.href != w.location.href) { w = w.parent; }
    return w;
}
//获取网站根地址
function getRootPath() {
    var port = window.location.port;//获取当前端口号码,如：8086
    if (port == "" || port == "80") {
        //获取主机地址之后的目录，如：/aaa/home/login
        var pathName = window.location.pathname + "/";
        //获取带"/"的项目名，如：/aaa
        var projectName = pathName.substring(0, pathName.substr(1).indexOf('/') + 1);
        return projectName;
    } else {
        return "";//获取当前hostname和port号码,如：http:localhost:8086
    }
}

/***************** cookies start********************/
//写cookies
function setCookie(name, value) {
    var Days = 30;
    var exp = new Date();
    exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
    document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
    document.cookie = name + "=" + escape(value);
}
//获取cookies
function getCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");
    if (arr = document.cookie.match(reg))
        return unescape(arr[2]);
    else
        return null;
}
//删除cookie
function delCookie(name) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie(name);
    if (cval != null)
        document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();
}
/***************** cookies  End ********************/

//字符串截取
function PadLeft(Value, totalWidth, paddingChar) {
    if (paddingChar == null)
        paddingChar = ' ';
    if (Value.length < totalWidth) {
        var paddingString = new String();
        for (i = 1; i <= (totalWidth - Value.length); i++) {
            paddingString += paddingChar;
        }
        return (paddingString + Value);
    } else {
        return Value;
    }
}
//去除字符串前后空格
function trimStr(str) {
    return str.replace(/(^\s*)|(\s*$)/g, "");
}
//获取当前日期
function GetNowDate() {
    var mydate = new Date();
    var str = mydate.getMonth() < 9 ? "0" + (mydate.getMonth() + 1) : (mydate.getMonth() + 1);
    str = mydate.getFullYear() + "-" + str;
    str += "-" + mydate.getDate();
    return str;
}

//格式化日期：yyyy-MM-dd
function formatDate(value) {
    if (value == null || value == undefined) {
        return "";
    }
    var unixTimestamp = new Date(value);
    return unixTimestamp.getFullYear() + "-" + PadLeft((unixTimestamp.getMonth() + 1).toString(), 2, '0') + "-" + PadLeft(unixTimestamp.getDate().toString(), 2, '0');
}
//格式化日期：yyyy-MM-dd HH:mm:ss
function formatDateTime(value) {
    if (value == null || value == undefined) {
        return "";
    }
    var unixTimestamp = new Date(value);
    return unixTimestamp.getFullYear() + "-"
        + PadLeft((unixTimestamp.getMonth() + 1).toString(), 2, '0') + "-"
        + PadLeft(unixTimestamp.getDate().toString(), 2, '0') + " "
        + PadLeft(unixTimestamp.getHours().toString(), 2, '0') + ":"
        + PadLeft(unixTimestamp.getMinutes().toString(), 2, '0') + ":"
        + PadLeft(unixTimestamp.getSeconds().toString(), 2, '0');
}

/****************************** easyui框架方法 start ******************************/

//添加tab窗口
function addPanel(menuid, menutext, url) {
    getRootWindow().addTab(menuid, menutext, url);
}

//刷新tab
function refeshPanel(menuid) {
    getRootWindow().refeshTab(menuid);
}

//关闭tab
function closePanel(menuid) {
    getRootWindow().closeTab(menuid);
}

/****************************** easyui框架方法 end ******************************/

//对象扩展
$.fn.extend({
    //获取表单输入(结合easyui标签)
    getJsonValues: function () {
        var jsones = {};
        var tempRecord = "";
        var texts = this.find("input[type=text],textarea,input[type=hidden],input[type=password]");
        var radios = this.find("radio:checked");
        texts.each(function () {
            var controlName = $(this).attr("name");
            var controlType = $(this).attr("type");
            var controlValue = $(this).val();
            if (controlType == "checkbox") {
                controlValue = $(this).attr("checked") == "checked" ? "1" : "0";
            }
            if (controlName != undefined) {
                jsones[controlName] = controlValue;
            }
        });
        radios.each(function () {
            var controlName = $(this).attr("name");
            var controlValue = $(this).val();
            jsones[controlName] = controlValue;
        });
        this.jsonData = jsones;
        this.jsonStr = JSON.stringify(jsones);
        return this;
    }
});
//类扩展
(function ($) {
    $.extend($, {
        DoAjax: function (opt) {
            var opt = $.extend({
                needWaiting: false,
                url: "",
                typeAction: "post",
                dataParams: {},
                typeAsync: true,
                typeCache: false,
                completeFunc: {},
                successFunc: function () { },
                errorFunc: function (XMLHttpRequest, textStatus, errorThrown) {
                    if (XMLHttpRequest.status == "0")
                        return;
                    else
                        alert('系统异常');
                }
            }, opt);
            if (opt.needWaiting) {
                try {
                    $.messager.progress({ title: '提示', msg: '正在请求', text: '努力中...', interval: '600' });
                    opt.completeFunc = function () {
                        $.messager.progress("close");
                    }
                }
                catch (err) {
                }
            }
            $.ajax({
                url: getRootPath() + opt.url,
                type: opt.typeAction,
                data: opt.dataParams,
                complete: opt.completeFunc,
                success: opt.successFunc,
                error: opt.errorFunc,
                async: opt.typeAsync,
                cache: opt.typeCache
            });
        }
    });
}(jQuery));