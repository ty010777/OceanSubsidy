<template>
    <span class="text-pink" v-if="editable">* </span>
    <slot></slot>
</template>

<script setup>
    const editable = inject("editable", true);
</script>
