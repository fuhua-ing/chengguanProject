/**
 * esyui-tab拓展方法，可以通过id验证tab的存在
 *
 */
$.extend($.fn.tabs.methods, {
    getTabById: function (jq, id) {
        var tabs = $.data(jq[0], 'tabs').tabs;
        for (var i = 0; i < tabs.length; i++) {
            var tab = tabs[i];
            if (tab.panel('options').id == id) {
                return tab;
            }
        }
        return null;
    },
    selectById: function (jq, id) {
        var tab;
        var tabs = $.data(jq[0], 'tabs').tabs;
        for (var i = 0; i < tabs.length; i++) {
            tab = tabs[i];
            if (tab.panel('options').id == id) {
                break;
            }
        }
        if (tab != undefined) {
            var curTabIndex = $(jq[0]).tabs("getTabIndex", tab);
            $(jq[0]).tabs('select', curTabIndex);
        }
    },
    existsById: function (jq, id) {
        return $(jq[0]).tabs('getTabById', id) != null;
    }
});

/**
 * esyui-datagrid拓展方法，获取
 *
 */
$.extend($.fn.datagrid.methods, {
    IsSelectedRow: function (jq) {
        var isOK = true;
        var oGrid = $(jq);
        var row = (oGrid.datagrid('getSelected'));
        if (row == undefined || row == null) {
            isOK = false;
            $.messager.alert('提示', '您没有选择任何项，请您选中后再操作！');
        }
        return isOK;
    }
});

/**
 *
 * 依赖easyui.min.js
 *
 */
$.fn.extend({
    //datagrid统一方法
    InitGird: function (opt) {
        var $gird = $(this);
        var opt = $.extend({
            //工具栏ID
            toolbar: '#ToolBar',
            //是否自适应行
            fitColumns: true,
            //宽度
            width: $(window).width(),
            //高度
            height: $(window).height(),
            //url请求方式
            method: 'post',
            //url地址
            url: null,
            //参数
            queryParams: {},
            //指明哪一个字段是标识字段
            idField: "ID",
            //分页控件
            pagination: true,
            //在设置分页属性的时候初始化页码
            pageNumber: 1,
            //在设置分页属性的时候初始化页面大小
            pageSize: 10,
            //在设置分页属性的时候 初始化页面大小选择列表
            pageList: [10, 20, 30],
            //如果为true，则显示一个行号列
            rownumbers: true,
            //是否显示斑马线效果。
            striped: true,
            //单选
            singleSelect: true,
            //自动行高
            autoRowHeight: false,
            //服务端排序
            remoteSort: false,
        }, opt);
        opt.url = config_jcxx_url + opt.url;
        $gird.datagrid(opt);
    },
    InitTreeGird: function (opt) {
        var $gird = $(this);
        var opt = $.extend({
            //工具栏ID
            toolbar: '#ToolBar',
            //是否自适应行
            fitColumns: true,
            //宽度
            width: $(window).width(),
            //高度
            height: $(window).height(),
            //url请求方式
            method: 'post',
            //url地址
            url: null,
            //定义关键字段来标识树节点
            idField: null,
            //定义树节点字段
            treeField: null,
            //分页控件
            pagination: true,
            //在设置分页属性的时候初始化页码
            pageNumber: 1,
            //在设置分页属性的时候初始化页面大小
            pageSize: 10,
            //在设置分页属性的时候 初始化页面大小选择列表
            pageList: [10, 20, 30],
            //如果为true，则显示一个行号列
            rownumbers: true,
            //是否显示斑马线效果。
            striped: true,
            //单选
            singleSelect: true,
            //自动行高
            autoRowHeight: false
        }, opt);
        opt.url = config_jcxx_url + opt.url;
        $gird.treegrid(opt);
        //窗口尺寸变化
        $(window).resize(function () {
            $gird.treegrid('resize', {
                width: $(window).width(),
                height: $(window).height()
            });
        });
        //搜索栏切换显示
        $(".StTitle .s_s_morebtn,.s_s_close a").click(function () {
            $(".sipac_rtsearchbar").toggle();
            $(".sipac_searchBox").toggle();
            $gird.treegrid('resize', {
                width: $(window).width(),
                height: $(window).height()
            });
        });
    },
    //窗口统一方法
    OpenWin: function (opt) {
        var $win = $(this);
        var opt = $.extend({
            //标题
            title: "",
            //宽度
            width: 600,
            //高度
            height: 400,
            //url地址
            url: null,
            //定义是否显示可折叠按钮。
            collapsible: false,
            //定义是否显示最小化按钮。
            minimizable: false,
            //定义是否显示最大化按钮。
            maximizable: false,
            //定义是否可以关闭窗口。
            closable: true,
            //定义是否可以关闭窗口。
            closed: true,
            //定义是否能够拖拽窗口
            draggable: true,
            //定义是否能够改变窗口大小
            resizable: true,
            //如果设置为true，在窗体显示的时候显示阴影
            shadow: true,
            //定义如何布局窗口，如果设置为true，窗口将显示在它的父容器中，否则将显示在所有元素的上面
            inline: false,
            //定义是否将窗体显示为模式化窗口
            modal: true
        }, opt);
        opt.url = config_jcxx_url + opt.url;
        if (opt.url != null) {
            $win.html('<iframe  src="' + opt.url + '" style="margin:0;border:0;padding:0;width:99%;height:95%;" ></iframe>');
        }
        $win.window(opt);
        $win.window('open');
    },
    //数据字典统一方法
    InitDictCombobox: function (opt) {
        var defaultAll = $(this).hasClass("defaultAll");
        var $combobox = $(this);
        var opt = $.extend({
            //请求方式
            method: 'get',
            //url地址
            url: null,
            parantID: null,
            //可以编辑
            editable: false,
            //值字段
            valueField: 'ItemCode',
            //显示字段
            textField: 'ItemName',
            //加载渲染
            loadFilter: function (data, parent) {
                if (defaultAll)
                    data.unshift({ ItemCode: "", ItemName: "所有" });
                return data;
            }
        }, opt);

        if (opt.url != null) {
            opt.url = config_jcxx_url + '/DictManager/GetItemListByCategoryCode?CategoryCode=' + opt.url;
        }
        if (opt.url != null && opt.parantID != null) {
            opt.url += "&&Note=" + opt.parantID;
        }
        $combobox.combobox(opt);
    },
    InitDeptCombobox: function (opt) {
        var defaultAll = $(this).hasClass("defaultAll");
        var $combobox = $(this);
        var opt = $.extend({
            //请求方式
            method: 'get',
            //url地址
            url: null,
            //可以编辑
            editable: false,
            //值字段
            valueField: 'ID',
            //显示字段
            textField: 'DeptName',
            //加载渲染
            loadFilter: function (data) {
                if (defaultAll)
                    data.unshift({ ID: "", DeptName: "所有" });
                return data;
            }
        }, opt);

        opt.url = config_jcxx_url + '/DeptManager/GetItemList';

        $combobox.combobox(opt);
    }
});