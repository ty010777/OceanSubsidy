<template>
    <div ref="container"></div>
</template>

<script setup>
    let root;

    const types = useGrantStore().typeList;

    const props = defineProps({
        list: { required: true, type: Array }
    });

    const container = ref();

    const init = (data) => {
        am5.ready(() => {
            root = am5.Root.new(container.value);
            root.setThemes([am5themes_Animated.new(root)]);

            const chart = root.container.children.push(
                am5xy.XYChart.new(root, {
                    panX: true,
                    panY: true,
                    wheelX: "panX",
                    wheelY: "zoomX",
                    pinchZoomX: true,
                    layout: root.verticalLayout
                })
            );

            const xRenderer = am5xy.AxisRendererX.new(root, { minGridDistance: 30 });

            xRenderer.labels.template.setAll({
                rotation: -45,
                centerY: am5.p50,
                centerX: am5.p100,
                textAlign: "right"
            });

            var xAxis = chart.xAxes.push(
                am5xy.CategoryAxis.new(root, {
                    categoryField: "category",
                    renderer: xRenderer,
                    tooltip: am5.Tooltip.new(root, {})
                })
            );

            xAxis.data.setAll(data);

            var yAxis = chart.yAxes.push(
                am5xy.ValueAxis.new(root, {
                    renderer: am5xy.AxisRendererY.new(root, {})
                })
            );

            var yAxisPercent = chart.yAxes.push(
                am5xy.ValueAxis.new(root, {
                    renderer: am5xy.AxisRendererY.new(root, { opposite: true }),
                    min: 0,
                    max: 1,
                    numberFormat: "#.0%",
                    extraMax: 0.1,
                    strictMinMax: true
                })
            );

            const createBarSeries = (field, name, color) => {
                var series = chart.series.push(
                    am5xy.ColumnSeries.new(root, {
                        name: name,
                        xAxis: xAxis,
                        yAxis: yAxis,
                        valueYField: field,
                        categoryXField: "category",
                        clustered: true,
                        tooltip: am5.Tooltip.new(root, { labelText: "{name}: {valueY}" })
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
            };

            createBarSeries("核定經費", "核定經費", 0x4B7BEC);   // 鮮藍
            createBarSeries("實支金額", "實支金額", 0x8E44AD);   // 深紫
            createBarSeries("已撥付", "已撥付", 0x3498DB);       // 藍青

            const createLineSeries = (field, name, color) => {
                var series = chart.series.push(
                    am5xy.LineSeries.new(root, {
                        name: name,
                        xAxis: xAxis,
                        yAxis: yAxisPercent,
                        valueYField: field,
                        categoryXField: "category",
                        stroke: am5.color(color),
                        fill: am5.color(color),
                        tooltip: am5.Tooltip.new(root, { labelText: "{name}: {valueY.formatNumber('#.0%')}" })
                    })
                );

                series.strokes.template.setAll({
                    strokeWidth: 3
                });

                series.bullets.push(() => {
                    return am5.Bullet.new(root, {
                        sprite: am5.Circle.new(root, {
                            radius: 5,
                            fill: series.get("stroke")
                        })
                    });
                });

                series.data.setAll(data);

                return series;
            };

            createLineSeries("經費執行率(%)", "經費執行率(%)", 0xE67E22); // 橘
            createLineSeries("支用比(%)", "支用比(%)", 0x27AE60);       // 綠

            const legend = chart.children.push(
                am5.Legend.new(root, {
                    centerX: am5.p50,
                    x: am5.p50,
                    layout: root.horizontalLayout,
                    useDefaultMarker: true,
                    maxWidth: 300
                })
            );

            legend.labels.template.setAll({
                fontSize: 12,
                paddingLeft: 4,
                paddingRight: 4,
                marginTop: 4,
                marginBottom: 4,
                marginLeft: 4,
                marginRight: 4
            });

            var legend1 = chart.children.push(
                am5.Legend.new(root, {
                    centerX: am5.p0,
                    x: am5.p0,
                    layout: root.horizontalLayout,
                    useDefaultMarker: true,
                    maxWidth: 300
                })
            );

            legend1.data.setAll([chart.series.getIndex(0), chart.series.getIndex(1), chart.series.getIndex(2)]);

            var legend2 = chart.children.push(
                am5.Legend.new(root, {
                    centerX: am5.p100,
                    x: am5.p100,
                    layout: root.horizontalLayout,
                    useDefaultMarker: true,
                    maxWidth: 300
                })
            );

            legend2.data.setAll([chart.series.getIndex(3), chart.series.getIndex(4)]);

            chart.set("cursor", am5xy.XYCursor.new(root, {}));
        });
    };

    const toPercent = (value1, value2, digits = 2) => {
        if (value2) {
            const base = 10 ** digits;

            return Math.round(value1 * 100 / value2 * base) / base;
        }

        return 0;
    };

    watch(() => props.list, () => {
        init(types.map((item) => {
            const data = { "category": item.title, "核定經費": 0, "實支金額": 0, "已撥付": 0, "經費執行率(%)": 0, "支用比(%)": 0 };
            const info = props.list.find((i) => i.Category === item.code);

            if (info) {
                data["核定經費"] = Math.floor(info.ApprovedAmount / 1000);
                data["實支金額"] = Math.floor(info.SpendAmount / 1000);
                data["已撥付"] = Math.floor(info.PaymentAmount / 1000);
                data["經費執行率(%)"] = toPercent(data["實支金額"], data["核定經費"], 1);
                data["支用比(%)"] = toPercent(data["實支金額"], data["已撥付"], 1);
            }

            return data;
        }));
    });
</script>
