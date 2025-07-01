# GSMap Openlayers-Plugin change log

## v2.0.0a
* ol 升級到7.2.2
    * getGetFeatureInfoUrl => getFeatureInfoUrl - method name change only
	* wgs84Sphere.haversineDistance(c1, c2) => ol.sphere.getDistance(c1, c2, ol.sphere.DEFAULT_RADIUS) - 已確認新舊版量測值一致
	* 以oca的iOceanMap測試，tm_ol3_editor.js無法使用
* 前端介面bug fixed

## v1.2 (2020-03-18)
* 將相同image service(WMS, ArcImageService)同service(by URL)、幾何類型的疊圖放在同一個request中，強化疊圖效率。
* 

## v1.1
* RWD設計
  * 彈性改變identify輸出位置
* 環境變數可以指定傳入Plugin
* 底圖管理加入LayersManager
* 截圖功能改進

## v1.0
* 圖台操控Plugin
* 圖層管理 -> 設定檔化
* 定位動作 -> 元件化
* 測量動作 -> 元件化
* 圖層編輯功能
* 截圖功能