<template>
    <div class="scroll-bottom-panel" v-if="store.status === '審核中' && progress[type].isOrganizer">
        <h5 class="text-pink fs-18 fw-bold mb-3">審查結果</h5>
        <ul class="d-flex flex-column gap-3 mb-3">
            <li class="d-flex gap-2 align-items-center" v-if="store.amount">
                <span class="text-gray">本期撥付：</span>
                <input-integer class="w-auto" :max="store.amount" v-model="form.Amount"></input-integer>
            </li>
            <li class="d-flex gap-2 align-items-center" v-if="store.paid">
                <span class="text-gray">前期累積撥付：</span>
                {{ store.paid.toLocaleString() }}
            </li>
            <li class="d-flex gap-2">
                <span class="text-gray mt-2">審查結果：</span>
                <div class="d-flex flex-column gap-2 align-items-start flex-grow-1 checkPass">
                    <div class="text-nowrap my-2 d-flex align-items-center">
                        <input-radio-group :options="options" v-model="form.Result"></input-radio-group>
                    </div>
                    <input-textarea :error="errors.Reason" placeholder="請輸入原因" rows="4" v-model="form.Reason" v-if="form.Result > 1"></input-textarea>
                </div>
            </li>
        </ul>
        <button class="btn btn-teal d-table mx-auto" @click="submit" :disabled="disabled" type="button">{{ button }}</button>
    </div>
</template>

<script setup>
    const props = defineProps({
        id: { type: [Number, String] },
        type: { type: String }
    });

    const progress = useProgressStore();
    const store = usePaymentStore();

    const options = [
        { value: 1, text: "通過" },
        { value: 2, text: "退回修改" }
    ];

    const disabled = computed(() => !store.stage);
    const errors = ref({});
    const form = ref({ ID: props.id, Result: 1, Reason: "" });

    const button = computed(() => {
        if (form.value.Result === 1) {
            if ((props.type === "culture" && store.stage === 2) || (props.type === "education") || (props.type === "accessibility" && store.stage === 2)) {
                return "確定撥款及結案";
            }

            if ((props.type === "multiple" && store.stage === 3) || (props.type === "literacy" && store.stage === 2)) {
                return "確定核銷及結案";
            }

            return "確定撥款";
        } else {
            return "確定退回";
        }
    });

    const submit = () => {
        const rules = {};

        if (form.value.Result > 1) {
            rules.Reason = "原因"
        }

        errors.value = validateData(form.value, rules);

        if (Object.keys(errors.value).length) {
            toButtom();
            return;
        }

        form.value.Stage = store.stage;

        api[props.type]("reviewPayment", form.value).subscribe((res) => {
            if (res) {
                window.location.href = "../ReviewChecklist.aspx?type=5";
            }
        });
    };

    const toButtom = () => setTimeout(() => document.querySelector(".mis-content").scrollTop = 1000000);

    watch(() => form.value.Result, (value) => {
        if (value > 1) {
            toButtom();
        }
    });

    watch(() => store.amount, (value) => form.value.Amount = value);
</script>
