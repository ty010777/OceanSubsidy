<template>
    <select class="form-select" :disabled="disabled || !editable" v-bind="$attrs" v-model="model">
        <option :value="emptyValue" v-if="allowEmpty || (editable && !model)">{{ placeholder }}</option>
        <template :key="item" v-for="(item,idx) in options">
            <slot :index="idx" :option="item">
                <option :value="item[valueName]">{{ item[textName] }}</option>
            </slot>
        </template>
    </select>
    <div class="invalid mt-0" v-if="error">{{ error }}</div>
</template>

<script setup>
    const editable = inject("editable", true);
    const model = defineModel();

    defineOptions({ inheritAttrs: false });

    defineProps({
        allowEmpty: { default: false, type: Boolean },
        disabled: { default: false, type: Boolean },
        emptyValue: { type: [Number, String] },
        error: { type: String },
        options: { default: [], type: Array },
        placeholder: { default: "請選擇", type: String },
        textName: { default: "text", type: String },
        valueName: { default: "value", type: String }
    });
</script>
