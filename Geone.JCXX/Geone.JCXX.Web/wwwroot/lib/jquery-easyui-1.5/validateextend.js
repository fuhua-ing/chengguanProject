$.extend($.fn.validatebox.defaults.rules, {
    beforedate: {
        validator: function (value) {
            var d1 = $.fn.datebox.defaults.parser(value);
            var today = new Date().toDateString();
            var d2 = $.fn.datebox.defaults.parser(today);
            return d1 < d2;
        },
        message: '请输入距当天之后的时间。'
    },
    NumberChar: {//数字和字母组合
        validator: function (value) {
            var patrn = /^[0-9a-zA-Z]*$/g;
            return patrn.test(value);
        },
        message: "该输入项只能由数字和字母组成。"
    },
    OneOrTwo: {//只能输入0或1
        validator: function (value) {
            var patrn = /^[01]$/;
            return patrn.test(value);
        },
        message: "只能输入0或1。"
    },
    checkPwd: {//检查与原密码是否一致
        validator: function (value, param) {
            var checkR = $.ajax({
                async: false,
                cache: false,
                type: 'post',
                url: '../RightServer/ValidPwd',
                data: { 'NewPwd': value, 'OldPwd': $(param[0]).val() }
            }).responseText;
            return checkR == "success";
        },
        message: '*与原密码不一致'
    },
    equals: {
        validator: function (value, param) {
            return value == $(param[0]).val();
        },
        message: '*两次输入不一致。'
    },
    valiphone: {
        validator: function (value) {
            var telrex = /^[0-9]{11}$|^[(][0-9]{3}[)][0-9]{11}$/;
            return telrex.test(value);
        },
        message: '请输入正确格式的手机号码。'
    },
    shortemail: {
        validator: function (value) {
            var telrex = /[1-9]\d{5}(?!\d)/;
            return telrex.test(value);
        },
        message: '请输入正确格式的邮政编码。'
    },
    PhoneOrTel: {
        validator: function (value) {
            var reg1 = /^0?(1)[0-9]{10}$/;
            var reg2 = /(^(\d{3,4}-)?\d{7,8}$)|(^1[0-9]{10}$)/;
            if (reg1.test(value) || reg2.test(value)) {
                return true;
            }
            else return false;
        },
        message: '请输入正确格式的手机号码或固话。'
    },
    password: {
        validator: function (value) {
            //var reg = /^(?=.*[a-zA-Z])(?=.*\d)(?=.*[~!@#$%^&*()_+`\-={}:";'<>?,.\/]).{4,16}$/;
            //return reg.test(value);
            return true;
        },
        message: "密码应包含数字、字母、特殊字符且长度为4-16。"
    },
    telphone: {
        validator: function (value) {
            var reg = /(^(\d{3,4}-)?\d{7,8}$)|(^1[0-9]{10}$)/;
            return reg.test(value);
        },
        message: "请输入正确格式的联系电话。如0512-66668888或1868886666。"
        //message: "联系电话格式，如0512-66668888或1868886666。"
    },
    numberformat: {
        validator: function (value) {
            return !isNaN(value);
        },
        message: "请输入数字。"
    },
    cardNumber: {// 验证身份证
        validator: function (value) {
            var reg = /^[0-9]{15}$|^[0-9]{18}$/;
            return reg.test(value);
        },
        message: '请输入正确格式的身份证件号码。'
    },
    drawNumber: {
        validator: function (value) {
            return (value > 0);
        },
        message: "请输入大于0的数字。"
    },
    //combobox 不允许值为空
    needSelect: {
        validator: function (value) {
            return value != "--请选择--" && value != "";
        },
        message: '请至少选择一项。'
    }
});