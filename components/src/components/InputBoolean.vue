<template>
    <input-radio-group :options="options" v-model="model" />
</template>

<script setup>
    const model = defineModel();

    const options = [
        { value: true, text: "是" },
        { value: false, text: "否" }
    ];
</script>
