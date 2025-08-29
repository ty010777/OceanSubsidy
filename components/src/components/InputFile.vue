<template>
    <button class="btn btn-teal-dark" @click="input.click()" type="button"><i class="fas fa-file-upload me-1"></i>上傳</button>
    <input @change="change" class="d-none" ref="input" type="file" v-bind="$attrs" />
</template>

<script setup>
    const input = ref();

    const change = () => {
        Array.from(input.value.files).forEach((file) => api.upload(file).subscribe((result) => emit("file", result)));

        input.value.value = "";
    };

    const emit = defineEmits(["file"]);

    defineOptions({ inheritAttrs: false });
</script>
