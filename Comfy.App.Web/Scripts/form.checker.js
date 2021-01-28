/**
*  表单值合法性验证工具类
*  
*  Create By Zhang He, @2011 12 28
*
*  
*
*/



FormCheckerOnPage = {

    'patterns': {
       // "not_empty": [/^\S+$/ig, "不能为空！"],
        "not_empty": [/\S{1,}/, "不能为空！"],
        "integer": [/^\d+$/, "必须为整数！"],
        "numeric": [/^\d+(\.\d+)?$/, "不是合法的数字！"],
        "currency": [/^\d+\.\d{1,2}$/, "不是合法的货币数字！"],
        "email": [/^\w+@\w+(\.(\w){2,3})$/i, "不合法！"],
        "phone": [/^[\d|-]+$/, "不合法！"],
        "must_checked": [/0+/g, "不能为空！"],
        "must_selected": [/0+/g, "不能为空！"],
        "ip_address": [/^([1-9]|[1-9]\d|1\d{2}|2[0-1]\d|22[0-3])(\.(\d|[1-9]\d|1\d{2}|2[0-4]\d|25[0-5])){3}$/, "不是有效的IP地址！"],
        "net_port": [/^[0-65536]$/, "不合法！"],
        "date": [/^([0-9]{4}\-[0-9]{1,2}\-[0-9]{1,2})$/, "不合法！"],
        "mobile_phone": [/^13[5-9]{1}[\d]{8}$/, "不合法！"],
        "unicom_phone": [/^13[0-4]{1}[\d]{8}$/, "不合法！"],
        "telecom_phone": [/^(\d{7,8})|(\d{3,4}\-\d{7,8})$/, "不合法！"],
        "_pattern": [null, "数据不合法！请确认"]
    },

    'getPattern': function (e) {
        var reg = null;
        if (e.check) {
            if (e.check == "_pattern") {
                reg = eval("/" + e.pattern + "/i")
            } else {
                reg = this.patterns[e.check][0];
            }
        }
        if (reg == null) {
            alert("表单项[" + e.name + "]验证Pattern配置错误!");
        }
        return reg;

    },

    'doCheck': function (f) {
        var els = f.elements;
        for (var i = 0; i < els.length; i++) {
            if (els[i].check) {
                var reg = this.getPattern(els[i]);
                var val = this.getValue(els[i]);
                if (val == "" && els[i].required == "false") {
                    continue;
                }
                if (!val.match(reg)) {
                    var elcnname = els[i].cnname ? els[i].cnname : els[i].name;
                    alert(els[i].warning ? els[i].warning : "表单项[" + elcnname + "]" + this.patterns[els[i].check][1]);
                    this.setFocus(els[i]);
                    return false;
                }
            }
        }
        return true;
    },


    'panelDoCheck': function (els) {
        for (var i = 0; i < els.length; i++) {
            if (els[i].check) {
//                var tempStr = "1";
//                if (gridName.length > 0) {
//                    for (var j = 0; j < gridName.length; j++) {
//                        if (gridName[j] != "" && els[i].id.indexOf(gridName[j] + gridName[j]) != -1) {
//                            tempStr = "0"
//                            break;
//                        }
//                    }
//                }

//                if (tempStr == "0") {
//                    continue;
//                }

                var reg = this.getPattern(els[i]);
                var val = this.getValue(els[i]);
                if (val == "" && els[i].required == "false") {
                    continue;
                }
                if (!val.match(reg)) {
                    var elcnname = els[i].cnname ? els[i].cnname : els[i].name;
                    alert(els[i].warning ? els[i].warning : "表单项[" + elcnname + "]" + this.patterns[els[i].check][1]);
                    this.setFocus(els[i]);
                    return false;
                }
            }
        }
        return true;
    },

    'setFocus': function (el) {
        switch (el.type) {
            case "text":
            case "hidden":
            case "password":
            case "file":
            case "textarea": el.focus(); el.select();
            case "checkbox":
            case "radio": var els = this.getElements(el.name); els[0].focus();
            case "select-one":
            case "select-multiple": el.focus();
        }
    },

    'getValue': function (el) {
        switch (el.type) {
            case "text":
            case "hidden":
            case "password":
            case "file":
            case "textarea": return el.value;
            case "checkbox":
            case "radio": return this.getCheckedValue(el);
            case "select-one":
            case "select-multiple": return this.getSelectedValue(el);
        }
    },

    'getCheckedValue': function (el) {
        var s = "";
        var els = this.getElements(el.name);
        for (var i = 0; i < els.length; i++) {
            if (els[i].checked) {
                s += "0"; // 可以通过0+来表示选中个数
            }
        }
        return s;
    },

    'getSelectedValue': function (el) {
        var s = "";
        for (var i = 0; i < el.options.length; i++) {
            if (el.options[i].selected && el.options[i].value != "" && el.options[i].value != "-1") {
                s += "0";
            }
        }
        return s;
    },

    'getElements': function (name) {
        return document.getElementsByName(name);
    }

}

