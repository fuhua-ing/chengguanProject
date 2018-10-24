/**
 *根据几何要素获取默认符号样式
 */
function getDefaultSymbol(geo, key) {
    var symbol;
    require([
            'esri/Color',
            'esri/symbols/Symbol',
            'esri/symbols/SimpleMarkerSymbol',
            'esri/symbols/PictureMarkerSymbol',
            'esri/symbols/SimpleLineSymbol',
            'esri/symbols/SimpleFillSymbol'
    ],
        function (Color, Symbol, SimpleMarkerSymbol, PictureMarkerSymbol, SimpleLineSymbol, SimpleFillSymbol) {
            var symbolid = geo + "_" + key;
            switch (symbolid) {
                case "point_default":
                    symbol = new PictureMarkerSymbol({
                        "url": "images/point.png",
                        "height": 30,
                        "width": 30,
                        "xoffset": 0,
                        "yoffset": 15
                    });
                    break;
                case "point_highlight":
                    symbol = new PictureMarkerSymbol({
                        "url": "images/point_selected.png",
                        "height": 30,
                        "width": 30,
                        "xoffset": 0,
                        "yoffset": 15
                    });
                    break;
                case "polyline_default":
                    symbol = new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([255, 0, 0, 255]),
                        3);
                    break;
                case "polyline_highlight":
                    symbol = new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([0, 255, 255, 255]),
                        3);
                    break;
                case "polygon_default":
                    symbol = new SimpleFillSymbol({
                        "type": "esriSFS",
                        "style": "esriSFSSolid",
                        "color": [255, 0, 0, 120],
                        "outline": {
                            "type": "esriSLS",
                            "style": "esriSLSSolid",
                            "color": [255, 0, 0, 255],
                            "width": 2
                        }
                    });
                    break;
                case "polygon_highlight":
                    symbol = new SimpleFillSymbol({ "type": "esriSFS", "style": "esriSFSSolid", "color": [0, 255, 255, 120], "outline": { "type": "esriSLS", "style": "esriSLSSolid", "color": [0, 255, 255, 255], "width": 2 } });
                    break;
            }
        });
    return symbol;
}