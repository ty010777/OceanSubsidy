<template>
    <div class="block">
        <table class="table align-middle gray-table side-table">
            <tbody>
                <tr>
                    <th>補助類別 全稱</th>
                    <td>{{ form.FullName }}</td>
                </tr>
                <tr>
                    <th>補助類別 簡稱</th>
                    <td class="p-0">
                        <div class="row align-items-center mb-0">
                            <div class="col p-4">{{ form.ShortName }}</div>
                            <div class="col p-4 text-nowrap" style="border-right:8px solid #DDDDDD;background-color:#F5F5F5">代碼</div>
                            <div class="col p-4">{{ form.TypeCode }}</div>
                            <div class="col p-4 text-nowrap" style="border-right:8px solid #DDDDDD;background-color:#F5F5F5">年度</div>
                            <div class="col p-4">{{ form.Year }}</div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <th>總預算經費(元)</th>
                    <td><input-integer class="w-auto" v-model.trim="form.BudgetFees"></input-integer></td>
                </tr>
                <tr>
                    <th>身分標籤</th>
                    <td><input-text v-model.trim="form.TargetTags"></input-text></td>
                </tr>
                <tr>
                    <th>申請期間</th>
                    <td>
                        <div class="d-flex align-items-center flex-wrap gap-2">
                            <div class="input-group" style="width:360px">
                                <input-tw-date v-model="form.ApplyStartDate"></input-tw-date>
                                <span class="input-group-text">至</span>
                                <input-tw-date v-model="form.ApplyEndDate"></input-tw-date>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <th>計畫期程迄日</th>
                    <td>
                        <div class="d-flex align-items-center flex-wrap gap-2">
                            <div class="input-group" style="width:360px">
                                <input-tw-date v-model="form.PlanEndDate"></input-tw-date>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <th>申請階段審查</th>
                    <td>
                        <div style="width:360px">
                            <div class="d-flex align-items-center mb-2">
                                <input class="form-check-input" id="review-1" type="checkbox" v-model="form.Review1Enabled">
                                <label class="form-check-label ms-2 me-2" for="review-1">1.</label>
                                <input-text class="w-auto" aria-label="資格審查" placeholder="資格審查" v-model.trim="form.Review1Title"></input-text>
                            </div>
                            <div class="d-flex align-items-center mb-2">
                                <input class="form-check-input" id="review-2" type="checkbox" v-model="form.Review2Enabled">
                                <label class="form-check-label ms-2 me-2" for="review-2">2.</label>
                                <input-text class="w-auto" aria-label="初審" placeholder="初審" v-model.trim="form.Review2Title"></input-text>
                            </div>
                            <div class="d-flex align-items-center mb-2">
                                <input class="form-check-input" id="review-3" type="checkbox" v-model="form.Review3Enabled">
                                <label class="form-check-label ms-2 me-2" for="review-3">3.</label>
                                <input-text class="w-auto" aria-label="複審" placeholder="複審" v-model.trim="form.Review3Title"></input-text>
                            </div>
                            <div class="d-flex align-items-center mb-2">
                                <input class="form-check-input" id="review-4" type="checkbox" v-model="form.Review4Enabled">
                                <label class="form-check-label ms-2 me-2" for="review-4">4.</label>
                                <input-text class="w-auto" aria-label="決審核定" placeholder="決審核定" v-model.trim="form.Review4Title"></input-text>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <th>補正補件期限</th>
                    <td class="d-flex align-items-center gap-2">
                        <input class="form-check-input" id="overdue-period" type="checkbox" v-model="period">
                        <label class="form-check-label" for="overdue-period">有期限</label>
                        <template v-if="period">
                            <input-integer class="w-auto" v-model.trim="form.OverduePeriod"></input-integer>天
                        </template>
                    </td>
                </tr>
                <tr>
                    <th>期中報告<br>繳交期限</th>
                    <td>
                        <span class="d-flex align-items-center flex-wrap gap-2">
                            <span class="input-group" style="width:360px">
                                <input-tw-date v-model="form.MidtermDeadline"></input-tw-date>
                            </span>
                        </span>
                    </td>
                </tr>
                <tr>
                    <th>期末/成果報告<br>繳交期限</th>
                    <td>
                        <div class="d-flex align-items-center flex-wrap gap-2">
                            <div class="input-group" style="width:360px">
                                <input-tw-date :disabled="form.FinalOneMonth" v-model="form.FinalDeadline"></input-tw-date>
                            </div>
                            <span class="p-2">或</span>
                            <input class="form-check-input" id="final-one-month" type="checkbox" v-model="form.FinalOneMonth" />
                            <label class="form-check-label" for="final-one-month">計畫結束一個月後</label>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
        <div class="d-flex gap-3 flex-wrap justify-content-center mt-4">
            <button class="btn btn-outline-teal" @click="save" type="button">儲存</button>
        </div>
    </div>
</template>

<script setup>
    const props = defineProps({
        id: { type: [Number, String] }
    });

    const form = ref({});
    const period = ref(false);

    const emit = defineEmits(["next"]);

    const save = () => {
        const data = {};

        if (!period.value) {
            form.value.OverduePeriod = null;
        }

        api.system("saveGrantType", { GrantType: Object.assign(form.value, data) }).subscribe((res) => {
            if (res) {
                emit("next");
            }
        });
    };

    onMounted(() => {
        api.system("getGrantType", { ID: props.id }).subscribe((res) => {
            form.value = res.GrantType;

            if (form.value.OverduePeriod) {
                period.value = true;
            }
        });
    });
</script>
