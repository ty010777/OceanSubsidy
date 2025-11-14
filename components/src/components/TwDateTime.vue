<template>
    <span>{{ datetime || defaultValue }}</span>
</template>

<script setup>
    const props = defineProps({
        defaultValue: { default: "", type: String },
        value: { default: null, type: String }
    });

    const toTwDateTime = (value) => {
        const matches = value?.match(/^(\d+)[\/-](\d+)[\/-](\d+)[ T](\d+:\d+(:\d+)?)/);

        return matches ? `${parseInt(matches[1], 10) - 1911}/${matches[2]}/${matches[3]} ${matches[4]}` : "";
    };

    const datetime = ref(toTwDateTime(props.value));

    watch(() => props.value, (value) => datetime.value = toTwDateTime(value));
</script>
