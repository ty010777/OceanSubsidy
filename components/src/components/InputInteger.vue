<template>
    <input
        class="form-control"
        :disabled="!editable"
        :placeholder="editable ? placeholder : ''"
        style="text-align:right"
        :type="type"
        v-bind="$attrs"
        v-model="model"
    />
    <div class="invalid mt-0" v-if="error">{{ error }}</div>
</template>

<script setup>
    const editable = inject("editable", true);
    const model = defineModel();

    const props = defineProps({
        error: { type: String },
        max: { type: Number },
        placeholder: { default: "請輸入", type: String },
        type: { default: "text", type: String }
    });

    defineOptions({ inheritAttrs: false });

    watch(model, (value) => {
        if (value) {
            const num = (parseInt(value) || 0).toString();

            if (value.toString() !== num) {
                model.value = num;
            } else if (props.max && Number(num) > props.max) {
                model.value = props.max;
            }
        }
    });
</script>
