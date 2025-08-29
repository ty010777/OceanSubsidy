<template>
    <input-select :options="options" v-model="model" />
</template>

<script setup>
    const model = defineModel();
    const options = [];

    for (let i = 1; i <= 12; i++) {
        options.push({ value: i, text: `${i}月` });
    }
</script>
