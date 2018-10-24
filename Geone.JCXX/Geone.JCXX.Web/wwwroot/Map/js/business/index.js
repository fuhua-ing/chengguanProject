var map;
var _rootconfig;
var drawTool;
var _token;
var CONST_CONFIG_PATH = "config/root.config.json";
var dynamicLayerID;



$(document).ready(function () {

    $.get(CONST_CONFIG_PATH, function (data) {
        _rootconfig = data;

        $.get(_rootconfig.services.token_local.url, function (token) {
            _token = token;
            if (_rootconfig.basemap.token === true) {
                if (_rootconfig.basemap.tokenid == "token_scm") {
                    _rootconfig.basemap.url += '?token=' + encodeURI(_rootconfig.services.token_scm.url);
                } else if (_rootconfig.basemap.tokenid == "token_local") {
                    _rootconfig.basemap.url += '?token=' + _token;
                }

            }
            if (_rootconfig.baseimg.token === true) {

                if (_rootconfig.baseimg.tokenid == "token_scm") {
                    _rootconfig.baseimg.url += '?token=' + encodeURI(_rootconfig.services.token_scm.url);
                } else if (_rootconfig.baseimg.tokenid == "token_local") {
                    _rootconfig.baseimg.url += '?token=' + _token;
                }

            }

            //console.log(_rootconfig.basemap.url);
            createBaseMap(_rootconfig);
            createMapTool();
        });
    });
});
/**
创建地图
*/
function createBaseMap(_rootconfig) {
    require(["esri/basemaps",
        "esri/map", "dojo/domReady!"
    ], function (esriBasemaps, Map) {
        esriBasemaps.delorme = {
            baseMapLayers: [{
                url: _rootconfig.baseimg.url,
                id: 'baseimgmap',
                visible: false
            }, {
                url: _rootconfig.basemap.url,
                id: 'basemap'
            }],
            title: "Delorme"
        };

        map = new Map("map", {
            basemap: "delorme",
            sliderStyle: "small",
            slider: false,
            logo: false
        });

        mapOnLoad = map.on("load", function (e) {
            //console.log("map loaded done...");
            map.getLayer('baseimgmap').setVisibility(false); //隐藏影像底图

            setInitExtent();
            InitCaseGISInfo();

        });

    })

}
/**
创建地图工具
*/
function createMapTool() {
    $.each(_rootconfig.maptool, function (idx, item) {
        console.log(item);
        var IMG = document.createElement("img");
        IMG.src = item.img;

        var _div = document.createElement("div");
        _div.setAttribute("id", item.id);
        _div.setAttribute("title", item.label);

        _div.appendChild(IMG);

        document.getElementById('maptool').appendChild(_div);

        $('#' + item.id).bind('click', function () {
            selectFn(item.id);
        });
    });
}

//初始化 加载部件图层、定位案件位置
function InitCaseGISInfo() {

    var caseid = getQueryString("caseid");
    var objectid = getQueryString("objectid");
    var shape = getQueryString("shape");
    //var shapeOld = getQueryString("shapeOld");
    var level = getQueryString("level");

    if (shape == null || shape == "" || shape == undefined) {

    }
    else {
        Locate(caseid, objectid, shape, level);

    }

    ////获取部件图层   
    //$.ajax({
    //    type: "get",
    //    async: true,
    //    contentType: "application/json;charset=utf-8",
    //    url: "../../Web/Ajax/SbjczHandler.ashx?type=getmapinfo&caseid=" + caseid,
    //    data: "",
    //    dataType: 'json',
    //    success: function (rr) {
    //        var jsonMapInfo = eval(rr);
    //        var layerIDs = jsonMapInfo[0].LayerIds;
    //        var layerIDArr;
    //        var layerIDArrInt = [];
    //        if (layerIDs != "null" && layerIDs != null && layerIDs != undefined) {
    //            layerIDArr = layerIDs.split(',');
    //            layerIDArrInt.push(parseInt(layerIDArr[layerIDArr.length - 1]));
    //        }
    //        dynamicLayerID = jsonMapInfo[0].ID;//赋值全局变量 图层id
    //        loadDynamicLayer(jsonMapInfo[0].Url, dynamicLayerID, layerIDArrInt);
    //    },
    //    error: function (XMLHttpRequest, textStatus, errorThrown) {
    //        //console.log("ddd");
    //    },
    //    complete: function () {
    //        //console.log("1");
    //    }
    //});

}

/*
设置地图初始化范围
*/
function setInitExtent() {
    require(["esri/map",
        "esri/geometry/Extent",
        "esri/SpatialReference",
        "dojo/domReady!"
    ], function (Map, Extent, SpatialReference) {
        spatialreference = new SpatialReference(map.spatialReference.wkt);
        startExtent = new Extent(_rootconfig.initextent.xmin, _rootconfig.initextent.ymin, _rootconfig.initextent
            .xmax, _rootconfig.initextent.ymax, spatialreference);
        map.setExtent(startExtent);
    })
}

function loadTileLayer(url, id) {
    $.get(_rootconfig.services.token_local.url, function (token) {
        var layerUrl = url + '?token=' + token;
        require(["esri/map", "esri/layers/ArcGISTiledMapServiceLayer", "dojo/domReady!"],
            function (Map, ArcGISTiledMapServiceLayer) {
                var layer = new ArcGISTiledMapServiceLayer(layerUrl, { id: id });
                map.addLayer(layer);
                //console.log(map);
            });
    });
}

function loadDynamicLayer(url, id, vls) {
    $.get(_rootconfig.services.token_local.url, function (token) {

        var layerUrl = url + '?token=' + token;
        require(["esri/map", "esri/layers/ArcGISDynamicMapServiceLayer", "dojo/domReady!"],
            function (Map, ArcGISDynamicMapServiceLayer) {
                var layer = new ArcGISDynamicMapServiceLayer(layerUrl, { id: id });
                if (vls !== undefined) {
                    layer.setVisibleLayers(vls);
                }
                map.addLayer(layer);
            });
    });
}

/*
通过wkt获取geometry对象
*/
function getGeoByWKT(wktStr) {
    var geo;
    require(['js/lib/wicket.arcgis.js', "dojo/domReady!"], function (Wkt) {
        var wkt = new Wkt.Wkt();
        wkt.read(wktStr);
        var config = {
            spatialReference: {
                wkt: map.spatialReference.wkt
            },
            editable: false
        }
        geo = wkt.toObject(config);
    });
    return geo;
}

/**
通过geometry获取集合要素的wkt值 
*/
function getWKTbyGeo(geometry) {
    var wkt;
    require(['js/lib/wicket.arcgis.js', "dojo/domReady!"], function (Wkt) {
        wkt = new Wkt.Wkt();
        wkt.fromObject(geometry);
        wkt = wkt.write();
    });
    return wkt;
}

/*
根据wkt将地图缩放到指定几何要素的视图范围（不显示符号样式）
*/
function zoomTo(wkt) {
    var geo = getGeoByWKT(wkt);
    map.centerAndZoom(geo, map.getMaxZoom() - 2);
}
//业务相关 定位
function Locate(caseid, objectID, shape, level) {
    //获取图层服务 
    console.log(shape);
    locateByWKT(shape, "");
}
/**
 * 根据wkt定位几何要素（显示符号样式）
 */
function locateByWKT(wkt, level) {
    clearAllGraphicsLayer();
    var geo = getGeoByWKT(wkt);
    var graLayer = getGraphicsLayer("lyr_location");

    addGeometryToMap(geo, graLayer, 'default');
    var zoom;
    if (level == "" || level == undefined || level == null) {
        zoom = map.getMaxZoom() - 2;
    }
    else {
        zoom = level;
    }
    switch (geo.type) {
        case "point":
            map.centerAndZoom(geo, zoom);
            break;
        case "polyline":
            break;
        case "polygon":
            map.centerAndZoom(geo.getCentroid(), zoom);
            break;
    }
}

/**
 * 将绘制完成的几何图形添加到地图中
 */
function addGeometryToMap(geometry, gryLayer, symbolkey) {
    require(['esri/graphic', 'esri/symbols/Symbol'], function (Graphic, Symbol) {
        var symbol = getDefaultSymbol(geometry.type, symbolkey)

        var graphic = new Graphic(geometry, symbol);
        gryLayer.add(graphic);
    });
}

/**
 * 根据id获取地图地图中的GraphicsLayer,如果不存在，则创建一个图层，并添加到地图中
 */
function getGraphicsLayer(id) {
    var needLayer = map.getLayer(id);
    if (needLayer == undefined) {
        require(['esri/layers/GraphicsLayer', "dojo/domReady!"], function (GraphicsLayer) {
            needLayer = new GraphicsLayer({
                id: id
            });
            map.addLayer(needLayer);
        });
    }
    return needLayer;
}

/**
绘制要素
*/
function drawGraphic(DRAW_TYPE, callback) {
    var wkt;
    var gry = getGraphicsLayer("lyr_location");
    require(['esri/toolbars/draw',], function (Draw) {
        drawTool = new Draw(map);
        switch (DRAW_TYPE) {
            case "POINT":
                drawTool.activate(Draw.POINT);
                break;
            case "POLYLINE":
                drawTool.activate(Draw.POLYLINE);
                break;
            case "POLYGON":
                drawTool.activate(Draw.POLYGON);
                break;
        }
        drawTool.on("draw-complete", function (event) {

            clearAllGraphicsLayer();

            drawTool.deactivate();
            map.setMapCursor("default");
            addGeometryToMap(event.geometry, gry, 'default');

            wkt = getWKTbyGeo(event.geometry);
            //回调
            callback(wkt);
        });
    });
    return wkt;
}

/**
移除所有除底图外的所有动态和切片图层图层
*/
function removeAllLayers() {
    var layerids = map.layerIds;

    while (layerids.length > 2) {
        var layerid = map.layerIds[map.layerIds.length - 1];
        var layer = map.getLayer(layerid);
        map.removeLayer(layer);
    }
}
/**
 * 清除地图所有图层中的Graphics
 */
function clearAllGraphicsLayer() {
    if (map.graphicsLayerIds === undefined) {
        return;
    }
    $(map.graphicsLayerIds).each(function (idx) {
        var str = map.graphicsLayerIds[idx];
        var layer = map.getLayer(str);
        if (layer.graphics.length > 0) {
            layer.clear();
        }

    });

    if (map.graphics.graphics.length > 0) {
        map.graphics.clear();
    }
}

var _url = "";
function clientPickPatrolPoint(url) {
    _url = url;
    drawGraphic("POINT", function (wkt) {
        slPickPatrolPointCompleted(wkt);
    });
}

function slPickPatrolPointCompleted(wkt) {

    for (var i = 0; i < window.parent.jQuery("iframe").length; i++) {
        var ss = $(window.parent.jQuery("iframe")[i]).attr("src");
        ss = ss.toLowerCase().substring(3);
        var url = _url.toLowerCase();
        if (ss.indexOf("caseinfoeditnew_tab.aspx") > -1) {
            var cnt = window.parent.jQuery("iframe")[i].contentWindow.jQuery("iframe").length;
            for (var j = 0; j < cnt; j++) {
                var _ss = $(window.parent.jQuery("iframe")[i].contentWindow.jQuery("iframe")[j]).attr("src");
                _ss = _ss.toLowerCase().substring(3);
                if (url.indexOf(_ss) > -1) {
                    window.parent.jQuery("iframe")[i].contentWindow.jQuery("iframe")[j].contentWindow.setText("", wkt);
                }
            }
        }
        else if (url.indexOf(ss) > -1) {
            window.parent.jQuery("iframe")[i].contentWindow.setText("", wkt);
        }
    }
    parent.ShowModelWindow();
}

/**
根据图层id清除该图层中的素有内容
*/
function clearOneGraphicLayer(id) {
    var layer = map.getLayer(id);
    if (layer != undefined && layer.graphics.length > 0) {
        layer.clear();
    }
}

/**
maptool 根据id选择执行的方法
*/
function selectFn(id) {
    switch (id) {
        case 'btn_fullmap':
            setInitExtent();
            break;
        case 'btn_basemap':
            map.getLayer('basemap').setVisibility(true); //显示地图
            map.getLayer('baseimgmap').setVisibility(false); //隐藏影像
            break;
        case 'btn_imgmap':
            map.getLayer('basemap').setVisibility(false); //隐藏地图
            map.getLayer('baseimgmap').setVisibility(true); //显示影像
            break;
        case 'btn_zoomin':
            var currentZoom = map.getZoom();
            map.setZoom(currentZoom + 1);
            break;
        case 'btn_zoomout':
            var currentZoom1 = map.getZoom();
            map.setZoom(currentZoom1 - 1);
            break;
        case 'btn_clear':
            clearAllGraphicsLayer();
            removeAllLayers();
            break;
        case 'btn_iquery':
            iQuery();
            break;
        default:
            break;
    }

}
$(document).ready(function () {
    $("#myslidedown").click(function () {
        $("#div_header").slideToggle("fast");
    })

    $("#btn_close").click(function () {

        $("#infopanel").slideToggle("fast");
        clearOneGraphicLayer('lyr_iquery');
    })
})

// I查询
function iQuery() {
    require(['esri/toolbars/draw',], function (Draw) {
        drawTool = new Draw(map);
        drawTool.activate(Draw.POINT);
        drawTool.on("draw-complete", function (event) {

            drawTool.deactivate();
            var x = event.geometry.x;
            var y = event.geometry.y;
            //var wkt = getWKTbyGeo(event.geometry);
            doQuery(x, y, dynamicLayerID);
            //console.log(event); 
            //alert(wkt);
        });
    });

    // showMessage('I查询开始啦...');


}

//执行i查询，显示查询结果
function doQuery(x, y, layerID) {
    if (layerID == undefined) {
        showMessage('未找到对应的部件图层.');
        return;
    }
    var url_iquery = _rootconfig.services.iquery.url;
    url_iquery = url_iquery.replace("#x#", x).replace("#y#", y).replace("#layerId#", layerID).replace("#radom#", Math.random());

    $.get(url_iquery, function (result) {
        var data = JSON.parse(result);
        console.log(data);
        if (data == undefined || data.length < 1) {
            showMessage('未查询到结果...');
            return;
        } else {
            showMessage('查询到一条结果.');
        }

        initinfolist(data);
        $("#infopanel").show('fast');
    });
}
//  初始化I查询结果展示
function initinfolist(data) {

    var geo = getGeoByWKT(data.shape);
    var graLayer = getGraphicsLayer("lyr_iquery");

    clearOneGraphicLayer('lyr_iquery');

    addGeometryToMap(geo, graLayer, 'highlight');
    map.centerAndZoom(geo, map.getMaxZoom() - 2);

    $('#title').text(data.name);
    $('#infolist').empty();

    $.each(data.attribute, function (key, value) {

        var c_div = document.createElement("div");
        c_div.className = "container-fluid";

        var row_div = document.createElement("div");
        row_div.className = "row row-margin";

        var l_div = document.createElement("div");
        l_div.className = "col-md-4 l-div";
        l_div.innerHTML = key;

        var r_div = document.createElement("div");
        r_div.className = "col-md-8 r-div";
        r_div.innerHTML = value;

        row_div.appendChild(l_div);
        row_div.appendChild(r_div);
        c_div.appendChild(row_div);

        document.getElementById('infolist').appendChild(c_div);
    })
}

// 显示提示信息
function showMessage(content) {

    $('#msgpanel').show();
    $('#msgContent').text(content);

    var timer = setTimeout(function () {
        $('#msgpanel').hide();
    }, 3000)
}
//提取参数
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}