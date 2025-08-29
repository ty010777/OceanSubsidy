<template>
    <div class="input-group">
        <span class="input-group-text" v-if="prefix">
            <required-label>{{ prefix }}</required-label>
        </span>
        <input
            class="form-control"
            :disabled="!editable"
            :placeholder="editable ? placeholder : ''"
            :type="type"
            v-bind="$attrs"
            v-model="model"
        />
    </div>
    <div class="invalid mt-0" v-if="error">{{ error }}</div>
</template>

<script setup>
    const editable = inject("editable", true);
    const model = defineModel();

    defineOptions({ inheritAttrs: false });

    defineProps({
        error: { type: String },
        placeholder: { default: "請輸入", type: String },
        prefix: { type: String },
        type: { default: "text", type: String }
    });
</script>
