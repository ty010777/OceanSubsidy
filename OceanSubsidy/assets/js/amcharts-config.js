// 經費執行率
am5.ready(function() {
    // Create root element
    // https://www.amcharts.com/docs/v5/getting-started/#Root_element
    var root = am5.Root.new("chartdiv");
    
    // Set themes
    // https://www.amcharts.com/docs/v5/concepts/themes/
    root.setThemes([
      am5themes_Animated.new(root)
    ]);
    
    // 響應式設計函數
    function updateChartSize() {
      var isTablet = window.innerWidth >= 768 && window.innerWidth <= 1024;
      var chartWidth = isTablet ? 105 : 158;
      var innerRadius = isTablet ? 55 : 95;
      var fontSize = isTablet ? 20 : 28;
      
      // 更新圖表尺寸
      chart.set("width", chartWidth);
      chart.set("innerRadius", innerRadius);
      label.set("fontSize", fontSize);
    }
    
    // Create chart
    // https://www.amcharts.com/docs/v5/charts/percent-charts/pie-chart/
    var chart = root.container.children.push(am5percent.PieChart.new(root, {
      innerRadius: 95,
      width: 158,
      x: am5.p50,
      y: am5.p50,
      centerX: am5.p50,
      centerY: am5.p50,
      layout: root.verticalLayout
    }));
    
    // Create series
    // https://www.amcharts.com/docs/v5/charts/percent-charts/pie-chart/#Series
    var series = chart.series.push(am5percent.PieSeries.new(root, {
      valueField: "size",
      categoryField: "sector"
    }));

    // 設定顏色
    series.set("colors", am5.ColorSet.new(root, {
      colors: [
        am5.color(0x26A69A), // 已執行 - 使用 teal 色
        am5.color(0xF5F5F5)  // 未執行 - 淺灰色
      ]
    }));

    // 隱藏標籤
    series.labels.template.set("visible", false);
    series.ticks.template.set("visible", false);
    
    // Set data - 簡化為單一類別
    // https://www.amcharts.com/docs/v5/charts/percent-charts/pie-chart/#Setting_data
    series.data.setAll([
      { sector: "已執行", size: 82.5 },
      { sector: "未執行", size: 17.5 }
    ]);
    
    
    // Play initial series animation
    // https://www.amcharts.com/docs/v5/concepts/animations/#Animation_of_series
    series.appear(1000, 100);
    
    
    // Add label
    var label = root.tooltipContainer.children.push(am5.Label.new(root, {
      x: am5.p50,
      y: am5.p50,
      centerX: am5.p50,
      centerY: am5.p50,
      fill: am5.color(0x26A69A),
      fontSize: 28,
      fontWeight: "bold"
    }));
    
    // Set static label text
    label.set("text", "82.5%");
    
    // 初始化尺寸
    updateChartSize();
    
    // 監聽視窗大小變動
    window.addEventListener('resize', updateChartSize);
    
    
}); // end am5.ready()

// 第二個圓餅圖 - 支用比
am5.ready(function() {
    var root2 = am5.Root.new("chartdiv2");
    
    // Set themes for chart2
    root2.setThemes([
      am5themes_Animated.new(root2)
    ]);
    
    // 響應式設計函數
    function updateChartSize2() {
      var isTablet = window.innerWidth >= 768 && window.innerWidth <= 1024;
      var chartWidth = isTablet ? 105 : 158;
      var innerRadius = isTablet ? 55 : 95;
      var fontSize = isTablet ? 20 : 28;
      
      // 更新圖表尺寸
      chart2.set("width", chartWidth);
      chart2.set("innerRadius", innerRadius);
      label2.set("fontSize", fontSize);
    }
    
    var chart2 = root2.container.children.push(am5percent.PieChart.new(root2, {
      innerRadius: 95,
      width: 158,
      x: am5.p50,
      y: am5.p50,
      centerX: am5.p50,
      centerY: am5.p50,
      layout: root2.verticalLayout
    }));
    
    // Create series for chart2
    var series2 = chart2.series.push(am5percent.PieSeries.new(root2, {
      valueField: "size",
      categoryField: "sector"
    }));

    // 設定顏色 - 使用 #5C6BC0
    series2.set("colors", am5.ColorSet.new(root2, {
      colors: [
        am5.color(0x5C6BC0), // 支用比 - 使用 indigo 色
        am5.color(0xF5F5F5)  // 未執行 - 淺灰色
      ]
    }));

    // 隱藏標籤
    series2.labels.template.set("visible", false);
    series2.ticks.template.set("visible", false);
    
    // Set data for chart2
    series2.data.setAll([
      { sector: "已支用", size: 75 },
      { sector: "未支用", size: 25 }
    ]);
    
    // Play initial series animation
    series2.appear(1000, 100);
    
    // Add label for chart2
    var label2 = root2.tooltipContainer.children.push(am5.Label.new(root2, {
      x: am5.p50,
      y: am5.p50,
      centerX: am5.p50,
      centerY: am5.p50,
      fill: am5.color(0x5C6BC0),
      fontSize: 28,
      fontWeight: "bold"
    }));
    
    // Set static label text for chart2
    label2.set("text", "75%");
    
    // 初始化尺寸
    updateChartSize2();
    
    // 監聽視窗大小變動
    window.addEventListener('resize', updateChartSize2);
});

//長條混合折線圖表
am5.ready(function () {
    var root3 = am5.Root.new("chartdiv3");
    
    root3.setThemes([am5themes_Animated.new(root3)]);

    var chart3 = root3.container.children.push(
      am5xy.XYChart.new(root3, {
        panX: true,
        panY: true,
        wheelX: "panX",
        wheelY: "zoomX",
        pinchZoomX: true,
        layout: root3.verticalLayout
      })
    );

    var xRenderer = am5xy.AxisRendererX.new(root3, {
      minGridDistance: 30
    });
    xRenderer.labels.template.setAll({
      rotation: -45,
      centerY: am5.p50,
      centerX: am5.p100,
      textAlign: "right"
    });

    var xAxis = chart3.xAxes.push(
      am5xy.CategoryAxis.new(root3, {
        categoryField: "category",
        renderer: xRenderer,
        tooltip: am5.Tooltip.new(root3, {})
      })
    );

    var yAxis = chart3.yAxes.push(
      am5xy.ValueAxis.new(root3, {
        renderer: am5xy.AxisRendererY.new(root3, {})
      })
    );

    var yAxisPercent = chart3.yAxes.push(
      am5xy.ValueAxis.new(root3, {
        renderer: am5xy.AxisRendererY.new(root3, { opposite: true }),
        min: 0,
        max: 1,
        numberFormat: "#.0%",
        extraMax: 0.1,
        strictMinMax: true
      })
    );

    var data = [
      { category: "科專類", 核定經費: 1000, 實支金額: 800, 已撥付: 900 },
      { category: "文化類", 核定經費: 1200, 實支金額: 1100, 已撥付: 1150 },
      { category: "學校.民間類", 核定經費: 900, 實支金額: 600, 已撥付: 800 },
      { category: "學校社團類", 核定經費: 1100, 實支金額: 700, 已撥付: 900 },
      { category: "多元類", 核定經費: 1000, 實支金額: 950, 已撥付: 1000 },
      { category: "素養類", 核定經費: 950, 實支金額: 500, 已撥付: 700 },
      { category: "無障礙類", 核定經費: 800, 實支金額: 640, 已撥付: 700 }
    ];

    data.forEach(d => {
      d["經費執行率(%)"] = d["核定經費"] > 0 ? d["實支金額"] / d["核定經費"] : 0;
      d["支用比(%)"] = d["已撥付"] > 0 ? d["實支金額"] / d["已撥付"] : 0;
    });

    xAxis.data.setAll(data);

    function createBarSeries(field, name, color) {
      var series = chart3.series.push(
        am5xy.ColumnSeries.new(root3, {
          name: name,
          xAxis: xAxis,
          yAxis: yAxis,
          valueYField: field,
          categoryXField: "category",
          clustered: true,
          tooltip: am5.Tooltip.new(root3, {
            labelText: "{name}: {valueY}"
          })
        })
      );
      series.data.setAll(data);
      series.columns.template.setAll({
        fillOpacity: 1,
        strokeOpacity: 0,
        fill: am5.color(color),
        cornerRadiusTL: 0,
        cornerRadiusTR: 0,
        width: am5.percent(70)
      });
      return series;
    }

    createBarSeries("核定經費", "核定經費", 0x2196F3);   // 核定經費 - 藍色
    createBarSeries("實支金額", "實支金額", 0xCDDC39);   // 實支金額 - 黃綠色
    createBarSeries("已撥付", "已撥付", 0xFF7043);       // 已撥付 - 橘色

    // 為長條圖系列添加動畫設定
    chart3.series.each(function(series) {
      if (series instanceof am5xy.ColumnSeries) {
        series.appear(1500, 0); // 動畫持續1秒，延遲200ms
      }
    });

    function createLineSeries(field, name, color) {
      var series = chart3.series.push(
        am5xy.LineSeries.new(root3, {
          name: name,
          xAxis: xAxis,
          yAxis: yAxisPercent,
          valueYField: field,
          categoryXField: "category",
          stroke: am5.color(color),
          fill: am5.color(color),
          tooltip: am5.Tooltip.new(root3, {
            labelText: "{name}: {valueY.formatNumber('#.0%')}"
          })
        })
      );
      series.strokes.template.setAll({
        strokeWidth: 3
      });
      series.bullets.push(() => {
        return am5.Bullet.new(root3, {
          sprite: am5.Circle.new(root3, {
            radius: 5,
            fill: series.get("stroke")
          })
        });
      });
      series.data.setAll(data);
      
      // 為折線圖系列添加動畫設定
      series.appear(1000, 300); // 動畫持續1.5秒，延遲1秒
      
      return series;
    }

    createLineSeries("經費執行率(%)", "經費執行率(%)", 0x26A69A); // 經費執行率 - teal 色
    createLineSeries("支用比(%)", "支用比(%)", 0x5C6BC0);       // 支用比 - indigo 色

    var legend = chart3.children.push(
      am5.Legend.new(root3, {
        centerX: am5.p50,
        x: am5.p50,
        layout: root3.horizontalLayout,
        useDefaultMarker: true,
      })
    );

    // 設定圖例項目間距 - 縮小項目之間的距離
    legend.itemContainers.template.setAll({
      paddingLeft: 0,
      paddingRight: 0,
      marginLeft: 0,
      marginRight:-40
    });

    legend.data.setAll(chart3.series.values);
    chart3.set("cursor", am5xy.XYCursor.new(root3, {}));
});