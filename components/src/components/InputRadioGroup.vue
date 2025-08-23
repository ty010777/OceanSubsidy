<template>
    <div class="form-check form-check-inline mb-0" :key="item" v-for="(item) in options">
        <input
            @change="change(item[valueName])"
            :checked="item[valueName] === model"
            class="form-check-input"
            :disabled="!editable"
            :id="`${id}-${item[valueName]}`"
            type="radio"
            :value="item[valueName]"
        />
        <label class="form-check-label" :for="`${id}-${item[valueName]}`">{{ item[textName] }}</label>
    </div>
</template>

<script setup>
    const editable = inject("editable", true);
    const model = defineModel();

    const change = (value) => {
        model.value = value;
    };

    defineProps({
        id: { default: () => nextId(), type: String },
        options: { default: [], type: Array },
        textName: { default: "text", type: String },
        valueName: { default: "value", type: String }
    });
</script>
