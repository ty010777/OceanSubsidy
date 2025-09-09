<template>
    <div ref="container"></div>
</template>

<script setup>
    let label;
    let root;
    let series;

    const props = defineProps({
        active: { required: true, type: String },
        color: { required: true, type: Number },
        inactive: { required: true, type: String },
        percent: { required: true, type: Number }
    });

    const container = ref();

    onMounted(() => {
        am5.ready(() => {
            root = am5.Root.new(container.value);
            root.setThemes([am5themes_Animated.new(root)]);

            //--

            const chart = root.container.children.push(am5percent.PieChart.new(root, {
                innerRadius: 95,
                width: 158,
                x: am5.p50,
                y: am5.p50,
                centerX: am5.p50,
                centerY: am5.p50,
                layout: root.verticalLayout
            }));

            //--

            series = chart.series.push(am5percent.PieSeries.new(root, {
                valueField: "size",
                categoryField: "sector"
            }));

            series.set("colors", am5.ColorSet.new(root, {
                colors: [am5.color(props.color), am5.color(0xF5F5F5)]
            }));

            series.labels.template.set("visible", false);
            series.ticks.template.set("visible", false);

            series.data.setAll([
                { sector: props.active, size: props.percent },
                { sector: props.inactive, size: 100 - props.percent }
            ]);

            series.appear(1000, 100);

            //--

            label = root.tooltipContainer.children.push(am5.Label.new(root, {
                x: am5.p50,
                y: am5.p50,
                centerX: am5.p50,
                centerY: am5.p50,
                fill: am5.color(props.color),
                fontSize: 28,
                fontWeight: "bold"
            }));

            label.set("text", `${props.percent}%`);

            //--

            const updateChartSize = () => {
                var isTablet = window.innerWidth >= 768 && window.innerWidth <= 1024;
                var chartWidth = isTablet ? 105 : 158;
                var innerRadius = isTablet ? 55 : 95;
                var fontSize = isTablet ? 20 : 28;

                chart.set("width", chartWidth);
                chart.set("innerRadius", innerRadius);
                label.set("fontSize", fontSize);
            };

            updateChartSize();

            window.addEventListener("resize", updateChartSize);
        });
    });

    watch(() => props.percent, (value) => {
        label.set("text", `${value}%`);

        series.data.setAll([
            { sector: props.active, size: value },
            { sector: props.inactive, size: 100 - value }
        ]);
    });
</script>
