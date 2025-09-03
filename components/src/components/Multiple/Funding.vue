<template>
    <div class="block">
        <h5 class="square-title">申請經費</h5>
        <div class="mt-4">
            <div class="table-responsive mt-3 mb-0">
                <table class="table align-middle gray-table">
                    <thead>
                        <tr>
                            <th class="text-end">申請海委會補助／合作金額 (A)</th>
                            <th class="text-end">申請單位自籌款 (B)</th>
                            <th class="text-end">其他機關補助／合作總金額 (C)</th>
                            <th class="text-end">計畫總經費 (A+B+C)</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>
                                <input-integer :error="errors.ApplyAmount" v-model="form.ApplyAmount"></input-integer>
                            </td>
                            <td>
                                <input-integer :error="errors.SelfAmount" v-model="form.SelfAmount"></input-integer>
                            </td>
                            <td class="text-end">
                                {{ form.OtherAmount?.toLocaleString() }}
                            </td>
                            <td class="text-end">
                                {{ total.toLocaleString() }}
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <ul class="list-unstyled lh-base mb-3">
                <li>補助申請單位為民間團體或學校者，配合款比例應為計畫總經費之 5％以上；</li>
                <li>合作申請單位之分攤比例應為計畫總經費之 20％以上。</li>
            </ul>
            <div class="table-responsive">
                <table class="table align-middle gray-table">
                    <thead>
                        <tr>
                            <th :colspan="editable ? 6 : 5" style="border-bottom-width:1px">其他機關補助／合作金額 (C)</th>
                        </tr>
                        <tr>
                            <th width="1"></th>
                            <th>單位名稱</th>
                            <th class="text-end">申請／分攤補助金額（含尚未核定者）</th>
                            <th>比例</th>
                            <th>申請合作項目</th>
                            <th width="1" v-if="editable">功能</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr :key="item" v-for="(item, idx) in filteredOthers">
                            <td>
                                {{ idx + 1 }}
                            </td>
                            <td>
                                <input-text :error="errors[`other-${idx}-Unit`]" v-model.trim="item.Unit"></input-text>
                            </td>
                            <td>
                                <input-integer :error="errors[`other-${idx}-Amount`]" v-model="item.Amount"></input-integer>
                            </td>
                            <td class="text-end">
                                {{ item.percent || 0 }}%
                            </td>
                            <td>
                                <input-textarea :error="errors[`other-${idx}-Content`]" rows="1" v-model.trim="item.Content"></input-textarea>
                            </td>
                            <td v-if="editable">
                                <div class="d-flex gap-2">
                                    <button class="btn btn-sm btn-teal-dark m-0" @click="remove(item)" type="button" v-if="filteredOthers.length > 1"><i class="fas fa-trash-alt"></i></button>
                                    <button class="btn btn-sm btn-teal-dark m-0" @click="addOther" type="button" v-if="idx + 1 === filteredOthers.length"><i class="fas fa-plus"></i></button>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <h5 class="square-title">經費預算規劃</h5>
        <div class="mt-4">
            <div class="table-responsive mt-3 mb-0">
                <table class="table align-middle gray-table">
                    <thead>
                        <tr>
                            <th>預算項目</th>
                            <th class="text-end">預算金額<br />海洋委員會經費</th>
                            <th class="text-end">預算金額<br />其他配合經費</th>
                            <th class="text-end">預算金額小計</th>
                            <th>計算方式及說明</th>
                            <th width="1" v-if="editable">功能</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr :key="item" v-for="(item, idx) in filteredPlans">
                            <td>
                                <input-text :error="errors[`plan-${idx}-Title`]" v-model.trim="item.Title"></input-text>
                            </td>
                            <td>
                                <input-integer :error="errors[`plan-${idx}-Amount`]" v-model="item.Amount"></input-integer>
                            </td>
                            <td>
                                <input-integer :error="errors[`plan-${idx}-OtherAmount`]" v-model="item.OtherAmount"></input-integer>
                            </td>
                            <td class="text-end">
                                {{ item.sum.toLocaleString() }}
                            </td>
                            <td>
                                <input-textarea :error="errors[`plan-${idx}-Description`]" rows="1" v-model.trim="item.Description"></input-textarea>
                            </td>
                            <td v-if="editable">
                                <div class="d-flex gap-2">
                                    <button class="btn btn-sm btn-teal-dark m-0" @click="remove(item)" type="button" v-if="filteredPlans.length > 1"><i class="fas fa-trash-alt"></i></button>
                                    <button class="btn btn-sm btn-teal-dark m-0" @click="addPlan" type="button" v-if="idx + 1 === filteredPlans.length"><i class="fas fa-plus"></i></button>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <template v-if="changeForm">
            <h5 class="square-title mt-5">變更說明</h5>
            <div class="mt-4">
                <table class="table align-middle gray-table side-table">
                    <tbody>
                        <tr>
                            <th><required-label>變更前</required-label></th>
                            <td>
                                <input-textarea :error="errors.Form3Before" rows="4" v-model.trim="changeForm.Form3Before"></input-textarea>
                            </td>
                        </tr>
                        <tr>
                            <th><required-label>變更後</required-label></th>
                            <td>
                                <input-textarea :error="errors.Form3After" rows="4" v-model.trim="changeForm.Form3After"></input-textarea>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </template>
    </div>
    <div class="block-bottom bg-light-teal" v-if="editable">
        <button class="btn btn-outline-teal me-3" @click="save()" type="button">暫存</button>
        <button class="btn btn-teal" @click="save(true)" type="button"><i class="fas fa-check"></i>完成本頁，下一步</button>
    </div>
    <confirm-modal ref="modal">
        <div class="text-center">
            <div class="mb-3">［提醒您］</div>
            <div class="mb-5">您申請的補助款經費總計，已超過補助原則上限：{{ setting.GrantLimit }}萬</div>
            <div class="mb-2">繼續申請請按【確定】</div>
            <div>返回調整請按【取消】</div>
        </div>
    </confirm-modal>
</template>

<script setup>
    const props = defineProps({
        id: { type: [Number, String] }
    });

    const changeForm = ref();
    const editable = computed(() => isProjectEditable("multiple", form.value.Status, 3));
    const errors = ref({});
    const filteredOthers = computed(() => others.value.filter((item) => !item.Deleted));
    const filteredPlans = computed(() => plans.value.filter((item) => !item.Deleted));
    const form = ref({});
    const modal = ref();
    const others = ref([]);
    const plans = ref([]);
    const setting = ref({});
    const total = ref(0);

    const addOther = () => others.value.push({});

    const addPlan = () => plans.value.push({});

    const emit = defineEmits(["next"]);

    const load = () => {
        rxjs.forkJoin([
            api.multiple("getFunding", { ID: props.id })
        ]).subscribe((result) => {
            const data = result[0];

            form.value = data.Project;
            others.value = data.OtherSubsidies;
            plans.value = data.BudgetPlans;
            setting.value = data.GrantTargetSetting;

            if (!others.value.length) {
                addOther();
            }

            if (!plans.value.length) {
                addPlan();
            }

            useProgressStore().init("multiple", form.value);

            changeForm.value = form.value.changeApply;
            form.value.changeApply = undefined;
        });
    };

    const remove = (item) => item.Deleted = true;

    const save = (submit, confirm) => {
        errors.value = {};

        if (submit && !verify()) {
            nextTick(() => document.querySelector(".invalid")?.scrollIntoView({ behavior: "smooth", block: "center" }));
            return;
        }

        if (!confirm && form.value.ApplyAmount > setting.value.GrantLimit * 10000) {
            modal.value.show(() => save(submit, true));
            return;
        }

        const data = {
            Project: form.value,
            OtherSubsidies: others.value,
            BudgetPlans: plans.value,
            Before: changeForm.value?.Form3Before,
            After: changeForm.value?.Form3After,
            Submit: submit ? "true" : "false"
        };

        api.multiple("saveFunding", data).subscribe(() => {
            if (submit) {
                emit("next");
            } else {
                load();
            }
        });
    };

    const verify = () => {
        let rules = {
            ApplyAmount: { presence: { allowEmpty: false, message: `^請輸入申請金額` }, numericality: { greaterThan: 0, message: "^申請金額不可為 0" } },
            SelfAmount: "自籌款"
        };

        errors.value = validateData(form.value, rules);

        rules = {
            Unit: "單位名稱",
            Amount: { presence: { allowEmpty: false, message: `^請輸入申請金額` }, numericality: { greaterThan: 0, message: "^申請金額不可為 0" } },
            Content: "申請合作項目"
        };

        others.value.forEach((other, index) => Object.assign(errors.value, validateData(other, rules, `other-${index}-`)));

        rules = {
            Title: "預算項目",
            Amount: "海委會經費",
            OtherAmount: "其他經費",
            Description: "計算方式及說明"
        };

        plans.value.forEach((plan, index) => Object.assign(errors.value, validateData(plan, rules, `plan-${index}-`)));

        if (changeForm.value) {
            rules = {
                Form3Before: "變更前說明",
                Form3After: "變更後說明"
            };

            Object.assign(errors.value, validateData(changeForm.value, rules));
        }

        return !Object.keys(errors.value).length;
    };

    onMounted(load);

    provide("editable", editable);

    watchEffect(() => {
        form.value.OtherAmount = filteredOthers.value.reduce((sum, other) => sum + toInt(other.Amount), 0);

        total.value = toInt(form.value.ApplyAmount) + toInt(form.value.SelfAmount) + toInt(form.value.OtherAmount);

        filteredOthers.value.forEach((other) => other.percent = Math.round(toInt(other.Amount) / form.value.OtherAmount * 10000) / 100);

        plans.value.forEach((plan) => plan.sum = toInt(plan.Amount) + toInt(plan.OtherAmount));
    });
</script>
