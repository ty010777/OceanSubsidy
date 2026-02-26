<template>
    <textarea
        class="form-control"
        :disabled="!editable"
        :placeholder="editable ? placeholder : ''"
        :rows="rows"
        v-bind="$attrs"
        v-model="model"
    ></textarea>
    <div class="fs-14 text-gray mt-2" v-if="editable && maxLength && rows > 1">
        已輸入 <span class="text-pink">{{ model?.length || 0 }}</span> 個字數(限{{ maxLength }}字數)
    </div>
    <div class="invalid mt-0" v-if="error">{{ error }}</div>
</template>

<script setup>
    const editable = inject("editable", true);
    const model = defineModel();

    const props = defineProps({
        error: { type: String },
        maxLength: { type: Number },
        placeholder: { default: "請輸入", type: String },
        rows: { type: [Number, String] }
    });

    defineOptions({ inheritAttrs: false });

    if (props.maxLength) {
        watch(model, () => {
            if (model.value?.length > props.maxLength) {
                model.value = model.value.substring(0, props.maxLength);
            }
        });
    }
</script>
