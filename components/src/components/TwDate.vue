<template>
    <span>{{ date || defaultValue }}</span>
</template>

<script setup>
    const props = defineProps({
        defaultValue: { default: "", type: String },
        value: { default: null, type: String }
    });

    const toTwDate = (date) => {
        const matches = date?.match(/^(\d+)[\/-](\d+)[\/-](\d+)/);

        return matches ? `${parseInt(matches[1], 10) - 1911}/${matches[2]}/${matches[3]}` : "";
    };

    const date = ref(toTwDate(props.value));

    watch(() => props.value, (value) => date.value = toTwDate(value));
</script>
