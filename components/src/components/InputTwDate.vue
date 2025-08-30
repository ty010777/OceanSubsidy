<template>
    <input
        class="form-control"
        :disabled="!editable"
        ref="element"
        type="text"
        v-bind="$attrs"
        v-model="datetime"
    />
    <div class="invalid" v-if="error">{{ error }}</div>
</template>

<script setup>
    let picker;

    const props = defineProps({
        displayFormat: { default: "tYY/MM/DD", type: String },
        drops: { default: "down", type: String },
        error: { type: String },
        format: { default: "YYYY-MM-DD", type: String },
        maxDate: { type: String },
        modelValue: { default: "", type: String },
        noFuture: { default: false, type: Boolean }
    });

    const datetime = ref();
    const editable = inject("editable", true);
    const element = ref();

    const emit = defineEmits(["update:modelValue"]);

    const sync = (value) => {
        if (!value && props.modelValue) {
            value = moment(new Date(props.modelValue));
        }

        if (value) {
            datetime.value = value.format(props.displayFormat);

            emit("update:modelValue", value.format(props.format));
        } else {
            datetime.value = "";

            emit("update:modelValue", "");
        }

        picker.setStartDate(datetime.value || undefined);
    };

    defineOptions({ inheritAttrs: false });

    onMounted(() => {
        const input = $(element.value);

        input.daterangepicker({
            autoApply: true,
            autoUpdateInput: false,
            drops: props.drops,
            locale: { format: props.displayFormat },
            maxDate: props.maxDate ? new Date(props.maxDate) : (props.noFuture ? new Date() : undefined),
            showDropdowns: true,
            singleDatePicker: true
        }).on("apply.daterangepicker", (_, picker) => sync(picker.startDate));

        picker = input.data("daterangepicker");

        sync();
    });

    onUnmounted(() => picker.remove());

    watch(() => props.modelValue, () => sync());
</script>
