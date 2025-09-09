<template>
    <button class="btn-tooltip" ref="element" style="color:#027f72" type="button">
        <i class="fa-solid fa-circle-info"></i>
    </button>
</template>

<script setup>
    const element = ref();

    let tooltip;

    onMounted(() => {
        tooltip = new bootstrap.Tooltip(element.value);
    });

    onUnmounted(() => {
        if (tooltip) {
            tooltip.dispose();
        }
    });
</script>
