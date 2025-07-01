/**
* @namespace entity
*/

/**
 * @typedef entity.MapPluginOptions
 * @property {string|ol.proj.Projection} displayPrj 圖台顯示的坐標系統
 * @property {string} coordinateTemplate 顯示定位點的template，字串內可帶`{x}`表x軸坐標,`{y}`表y軸坐標。顯示的坐標系統為dispalyProj。
 * @property {ol.Coordinate} initPosition 初始位置，坐標系統依照displayPrj
 * @property {int} initZoom 初始zoom level。
 * @property {string} proxyUrlTemplate 使用proxy時，產生url的tempalte。字串內`{url}`表示原始url。 ex: `service/proxy.ashx?{url}`
 * @property {int} locatingPointZoom 定位到點位後，縮到到此zoom level。
 * @property {entity.Style} defaultStyle 呈現點線面樣式時，預設使用此樣式。
 */


/**
 * @typedef entity.Style
 * @summary 樣式json設定內容
 * @property {entity.style.Icon} icon 點位以icon形式設定的json格式
 * @property {entity.style.Circle} circle 點位以圓形樣式設定的json格式
 * @property {ol.style.Stroke~options} stroke 線型樣式json格式，請參考[ol.style.Stroke]{@link https://openlayers.org/en/latest/apidoc/module-ol_style_Stroke-Stroke.html}的options
 * @property {ol.style.Fill~options} fill 填滿型樣式json格式，請參考[ol.style.Fill]{@link https://openlayers.org/en/latest/apidoc/module-ol_style_Fill-Fill.html}的options
 * @property {eneity.style.Text} text 文字樣式json格式
 */

/**
 * @typedef entity.style.Icon
 * @summary 點位以icon形式設定的json格式
 * @property {string} src icon URL
 * @property {Array.<number>} anchor 錨點位置，預設為[0.5, 0.5]
 * @property {string} anchorXUnits 可以是'fraction'或'pixels'，預設為'fraction'
 * @property {string} anchorYUnits 可以是'fraction'或'pixels'，預設為'fraction'
 * @property {number} width icon的寬度的pixel
 * @property {number} height icon的高度的pixel
 * @property {number} opacity 透明度，值為0~1
 */

/**
 * @typedef entity.style.Circle
 * @property {number} radius 半徑
 * @property {ol.style.Stroke~options} stroke 線型樣式json格式，請參考[ol.style.Stroke]{@link https://openlayers.org/en/latest/apidoc/module-ol_style_Stroke-Stroke.html}的options
 * @property {ol.style.Fill~options} fill 填滿型樣式json格式，請參考[ol.style.Fill]{@link https://openlayers.org/en/latest/apidoc/module-ol_style_Fill-Fill.html}的options
 */



/**
* 空間位置完整資訊
* @typedef {{coord:(ol.coordinate), proj:(string|ol.proj.Projection)}} entity.CoordInfo
* @property {ol.Coordinate} coord 坐標點位
* @property {string|ol.proj.Projection} proj 坐標系統
*/
/**
* 欄位資訊
* @typedef {Object} entity.FieldInfo
* @property {string} name - 欄位名稱
* @property {string} dataType - 資料型態，請參考XSD type  
* @property {boolean} nullable - 可否為null
*/


/**
 * 圖層資訊設定
* @typedef {Object} entity.LayerInfo
* @property {string} name - 圖層名稱
* @property {string} geometryType - 圖層向量資料類型，可以是"point", "polyline", "polygon"
* @property {string} type - 節點類型。"folder" 表示圖層群組；"node" 表示實際圖層。
* @property {string} legendIcon - 圖層圖示的url
* @property {function} onAdd - 當圖層被加入時
* @property {function} onRemove - 當圖層被移除時
*/

/**
 * @summary圖層服務設定
 * @typedef {Object} entity.ServiceInfo
 * @property {Function} getGetFeatureInfoUrl - **deprecated:** callback funtion, override source.getGetFeatureInfoUrl(符合ol5以前)。
 * @property {Function} getFeatureInfoUrl - callback funtion, override source.getFeatureInfoUrl(符合ol6以後)。
*/

