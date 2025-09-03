<template>
    <div class="scroll-bottom-panel" v-if="store[type].organizer">
        <h5 class="text-pink fs-18 fw-bold mb-3">審查結果</h5>
        <ul class="d-flex flex-column gap-3 mb-3">
            <li class="d-flex gap-2">
                <span class="text-gray">同單位申請計畫數 :</span>
                <a class="link-teal fw-bold" @click="showOther" href="javascript:void(0)">3</a>
            </li>
            <li class="d-flex gap-2">
                <span class="text-gray">風險評估 :</span>
                <span class="text-pink">中風險</span>
                <span>( <a class="link-teal fw-bold" @click="showRisk" href="javascript:void(0)">1</a> 筆記錄 )</span>
            </li>
            <li class="d-flex gap-2">
                <span class="text-gray mt-2">審查結果：</span>
                <div class="d-flex flex-column gap-2 align-items-start flex-grow-1 checkPass">
                    <div class="text-nowrap mb-2 d-flex align-items-center">
                        <input-radio-group :options="options" v-model="form.Result"></input-radio-group>
                        <input-tw-date class="form-control" drops="up" v-model="form.CorrectionDeadline"></input-tw-date>
                    </div>
                    <div class="invalid mt-0" v-if="errors.CorrectionDeadline">{{ errors.CorrectionDeadline }}</div>
                    <input-textarea :error="errors.Reason" placeholder="請輸入原因" rows="4" v-model="form.Reason" v-if="form.Result > 1"></input-textarea>
                </div>
            </li>
        </ul>
        <button class="btn btn-teal d-table mx-auto" @click="submit" type="button">確定</button>
    </div>
</template>

<script setup>
    const props = defineProps({
        id: { type: Number },
        type: { type: String }
    });

    const store = useProgressStore();

    const options = [
        { value: 1, text: "通過" },
        { value: 2, text: "不通過" },
        { value: 3, text: "退回補正補件" }
    ];

    const errors = ref({});
    const form = ref({ ID: props.id, Result: 1 });

    const showOther = () => {
    };

    const showRisk = () => {
    };

    const submit = () => {
        const rules = {};

        if (form.value.Result > 1) {
            rules.Reason = "原因"
        }

        if (form.value.Result === 3) {
            rules.CorrectionDeadline = "補正期限"
        }

        errors.value = validateData(form.value, rules);

        if (Object.keys(errors.value).length) {
            toButtom();
            return;
        }

        api[props.type]("reviewApplication", form.value).subscribe((res) => {
            if (res) {
                // TODO
            }
        });
    };

    const toButtom = () => setTimeout(() => document.querySelector(".mis-content").scrollTop = 1000000);

    watch(() => form.value.Result, (value) => {
        if (value > 1) {
            toButtom();
        }
    });
</script>
