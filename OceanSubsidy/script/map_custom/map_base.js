// 不可為巢狀結構
var map_base =
{
    "開放街圖(標準)": {
        serviceInfo: {
            name: "開放街圖(標準)",
            type: "OSM",
            useProxy: true
        }
    },
    "通用版電子地圖": {
        serviceInfo: {
            name: "通用版電子地圖",
            type: "WMTS",
            url: "http://wmts.nlsc.gov.tw/wmts",
            layer: 'EMAP5',
            matrixSet: 'GoogleMapsCompatible',
            requestEncoding: 'RESTful',
            useProxy: true
        }
    },
    "電子地圖(僅道路)": {
        serviceInfo: {
            name: "電子地圖(僅道路)",
            type: "WMTS",
            url: "http://wmts.nlsc.gov.tw/wmts",
            layer: 'EMAP2',
            matrixSet: 'GoogleMapsCompatible',
            requestEncoding: 'RESTful',
            useProxy: true
        }
    },
    "正射(航照)地圖": {
        serviceInfo: {
            name: "正射(航照)地圖",
            type: "WMTS",
            url: "http://wmts.nlsc.gov.tw/wmts",
            layer: 'PHOTO2',
            matrixSet: 'GoogleMapsCompatible',
            requestEncoding: 'RESTful',
            useProxy: true
        }
    },
    "淺色地圖": {
        serviceInfo: {
            name: "淺色地圖",
            type: "XYZ",
            url: "http://a.basemaps.cartocdn.com/light_all/{z}/{x}/{y}.png"
        }
    },
    "深色地圖": {
        serviceInfo: {
            name: "深色地圖",
            type: "XYZ",
            url: "http://a.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}.png"
        }
    },
    "開放街圖(自行車地圖)": {
        serviceInfo: {
            name: "開放街圖(自行車地圖)",
            type: "XYZ",
            url: 'https://{a-c}.tile.thunderforest.com/cycle/{z}/{x}/{y}.png?apikey=3e459739edbf4cceaac8e73154e08aaf'
        }
    },
    "開放街圖(交通地圖)": {
        serviceInfo: {
            name: "開放街圖(交通地圖)",
            type: "XYZ",
            url: 'https://{a-c}.tile.thunderforest.com/transport/{z}/{x}/{y}.png?apikey=3e459739edbf4cceaac8e73154e08aaf'
        }
    }
}