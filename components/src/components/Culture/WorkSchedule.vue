<template>
    <div class="block">
        <h5 class="square-title">期程</h5>
        <div class="mt-4">
            <table class="table align-middle gray-table side-table">
                <tbody>
                    <tr>
                        <th><required-label>計畫期程</required-label></th>
                        <td>
                            <div class="d-flex align-items-center gap-2">
                                <div class="input-group w-auto">
                                    <input-tw-date class="w-auto" v-model="form.StartTime"></input-tw-date>
                                    <span class="input-group-text">至</span>
                                    <input-tw-date class="w-auto" v-model="form.EndTime"></input-tw-date>
                                </div>
<span class="text-teal-dark">(期程不可超過 115/04/30)</span>
                            </div>
                            <div class="invalid mt-0" v-if="errors.StartTime || errors.EndTime">{{ errors.StartTime || errors.EndTime }}</div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <h5 class="square-title mt-4">
            計畫目標與內容
            <button class="btn btn-teal-dark ms-3" @click="addGoal" type="button" v-if="editable"><i class="fas fa-plus"></i>新增計畫目標</button>
        </h5>
        <div class="mt-4">
            <div class="block-form mt-3" :key="goal" v-for="(goal, idx) in filteredGoals">
                <button class="btn btn-teal del" @click="removeGoal(goal)" type="button" v-if="editable && filteredGoals.length > 1">刪除</button>
                <div class="d-flex flex-column gap-2">
                    <label class="text-teal-dark">計畫目標</label>
                    <div>
                        <input-text :error="errors[`goal-${idx}-Title`]" v-model.trim="goal.Title"></input-text>
                    </div>
                </div>
                <div class="mt-4 d-flex flex-column gap-2">
                    <label class="text-teal-dark">預期效益（含量化或質化說明）</label>
                    <div>
                        <input-textarea :error="errors[`goal-${idx}-Content`]" rows="4" v-model.trim="goal.Content"></input-textarea>
                    </div>
                </div>
                <div class="d-flex flex-column gap-3 mt-4">
                    <div class="border" :key="item" v-for="(item, idx1) in goal.Items.filter((item) => !item.Deleted)">
                        <div class="bg-light-teal p-3 text-gray d-flex align-items-center gap-3">
                            <div class="d-flex gap-2 align-items-center flex-grow-1">
                                <label class="text-nowrap"><required-label>重要工作項目</required-label></label>
                                <div class="flex-grow-1">
                                    <input-text :error="errors[`item-${idx}-${idx1}-Title`]" v-model.trim="item.Title"></input-text>
                                </div>
                            </div>
                            <div class="d-flex gap-2">
                                <a class="link-teal" @click.prevent="removeItem(item)" href="#" v-if="editable && goal.Items.filter((item) => !item.Deleted).length > 1">刪除</a>
                                <a aria-expanded="true" class="link-teal" data-bs-toggle="collapse" :href="`#collapse-${idx1}`" role="button">收合</a>
                            </div>
                        </div>
                        <div class="collapse show" :id="`collapse-${idx1}`">
                            <table class="table align-middle gray-table side-table mb-0">
                                <tbody>
                                    <tr>
                                        <th><required-label>實施步驟<br />及起迄月份</required-label></th>
                                        <td class="p-0">
                                            <ul class="item-list">
                                                <li class="step-item section-wrapper" :class="{ disabled: !editable }" :key="step" v-for="(step, idx2) in item.Steps.filter((step) => !step.Deleted)">
                                                    <div class="mt-2 align-self-start text-center">{{ String.fromCharCode(65 + idx2) }}</div>
                                                    <div>
                                                        <input-text :error="errors[`step-${idx}-${idx1}-${idx2}-Title`]" placeholder="如:進行口訪紀錄" v-model.trim="step.Title"></input-text>
                                                    </div>
                                                    <div>
                                                        <div class="input-group">
                                                            <input-month :empty-value="0" v-model="step.Begin"></input-month>
                                                            <span class="input-group-text">至</span>
                                                            <input-month :empty-value="0" v-model="step.End"></input-month>
                                                        </div>
                                                        <div class="invalid mt-0" v-if="errors[`step-${idx}-${idx1}-${idx2}-Begin`] || errors[`step-${idx}-${idx1}-${idx2}-End`]">{{ errors[`step-${idx}-${idx1}-${idx2}-Begin`] || errors[`step-${idx}-${idx1}-${idx2}-End`] }}</div>
                                                    </div>
                                                    <div class="d-flex gap-2 mt-1 align-self-start" v-if="editable">
                                                        <button class="btn btn-sm btn-teal-dark m-0" @click="removeStep(step)" type="button" v-if="item.Steps.filter((step) => !step.Deleted).length > 1">
                                                            <i class="fas fa-trash-alt"></i>
                                                        </button>
                                                        <button class="btn btn-sm btn-teal-dark m-0 add-btn" @click="addStep(item)" type="button"><i class="fas fa-plus"></i></button>
                                                    </div>
                                                </li>
                                            </ul>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th><required-label>績效指標</required-label></th>
                                        <td>
                                            <input-text :error="errors[`item-${idx}-${idx1}-Indicator`]" placeholder="如:完成口訪資料1篇" v-model.trim="item.Indicator"></input-text>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th><required-label>工作進度</required-label></th>
                                        <td>
                                            <div class="row">
                                                <div class="col">
                                                    <div class="fs-16 text-gray">工作進度達 50% 應檢核工作</div>
                                                    <div class="row g-2 align-items-center mt-2 section-wrapper" :key="schedule" v-for="(schedule, idx2) in item.Schedules.filter((schedule) => schedule.Type === 1 && !schedule.Deleted)">
                                                        <div class="mt-0" :class="`col-${editable ? 3 : 4}`">
                                                            <input-month :error="errors[`schedule-${idx}-${idx1}-1-${idx2}-Month`]" :empty-value="0" v-model="schedule.Month"></input-month>
                                                        </div>
                                                        <div class="mt-0" :class="`col-${editable ? 7 : 8}`">
                                                            <input-select :empty-value="0" :error="errors[`schedule-${idx}-${idx1}-1-${idx2}-StepID`]" :options="item.Steps.filter((step) => !step.Deleted)" v-model="schedule.StepID">
                                                                <template #default="{ index, option }">
                                                                    <option :value="option.ID">{{ String.fromCharCode(65 + index) }}.{{ option.Title }}</option>
                                                                </template>
                                                            </input-select>
                                                        </div>
                                                        <div class="col-2 d-flex gap-2 mt-1 align-self-start" v-if="editable">
                                                            <button class="btn btn-sm btn-teal-dark m-0" @click="removeSchedule(schedule)" type="button" v-if="item.Schedules.filter((schedule) => schedule.Type === 1 && !schedule.Deleted).length > 1">
                                                                <i class="fas fa-trash-alt"></i>
                                                            </button>
                                                            <button class="btn btn-sm btn-teal-dark m-0 add-btn" @click="addSchedule(item, 1)" type="button"><i class="fas fa-plus"></i></button>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col">
                                                    <div class="fs-16 text-gray">工作進度達 100% 應檢核工作</div>
                                                    <div class="row g-2 align-items-center mt-2 section-wrapper" :key="schedule" v-for="(schedule, idx2) in item.Schedules.filter((schedule) => schedule.Type === 2 && !schedule.Deleted)">
                                                        <div class="mt-0" :class="`col-${editable ? 3 : 4}`">
                                                            <input-month :error="errors[`schedule-${idx}-${idx1}-2-${idx2}-Month`]" :empty-value="0" v-model="schedule.Month"></input-month>
                                                        </div>
                                                        <div class="mt-0" :class="`col-${editable ? 7 : 8}`">
                                                            <input-select :empty-value="0" :error="errors[`schedule-${idx}-${idx1}-2-${idx2}-StepID`]" :options="item.Steps.filter((step) => !step.Deleted)" v-model="schedule.StepID">
                                                                <template #default="{ index, option }">
                                                                    <option :value="option.ID">{{ String.fromCharCode(65 + index) }}.{{ option.Title }}</option>
                                                                </template>
                                                            </input-select>
                                                        </div>
                                                        <div class="col-2 d-flex gap-2 mt-1 align-self-start" v-if="editable">
                                                            <button class="btn btn-sm btn-teal-dark m-0" @click="removeSchedule(schedule)" type="button" v-if="item.Schedules.filter((schedule) => schedule.Type === 2 && !schedule.Deleted).length > 1">
                                                                <i class="fas fa-trash-alt"></i>
                                                            </button>
                                                            <button class="btn btn-sm btn-teal-dark m-0 add-btn" @click="addSchedule(item, 2)" type="button"><i class="fas fa-plus"></i></button>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
                <button class="btn btn-teal mt-3" @click="addItem(goal)" type="button" v-if="editable">新增工作項目</button>
            </div>
        </div>
    </div>
    <div class="block-bottom bg-light-teal" v-if="editable">
        <button class="btn btn-outline-teal me-3" @click="save()" type="button">暫存</button>
        <button class="btn btn-teal" @click="save(true)" type="button"><i class="fas fa-check"></i>完成本頁，下一步</button>
    </div>
</template>

<style scoped>
    .section-wrapper:not(:last-child) .add-btn {
        display: none;
    }

    .step-item.disabled {
        grid-template-columns: .5fr 6fr 4fr;
    }
</style>

<script setup>
    const props = defineProps({
        id: { type: Number }
    });

    const editable = computed(() => isProjectEditable("culture", form.value.Status, 2));
    const errors = ref({});
    const filteredGoals = computed(() => goals.value.filter((goal) => !goal.Deleted));
    const form = ref({});
    const goals = ref([]);

    const addGoal = () => {
        const goal = { Items: [] };

        addItem(goal);

        goals.value.push(goal);
    };

    const addItem = (goal) => {
        const item = { Steps: [], Schedules: [] };

        addStep(item);
        addStep(item);
        addStep(item);
        addSchedule(item, 1);
        addSchedule(item, 2);

        goal.Items.push(item);
    };

    const addSchedule = (item, type) => item.Schedules.push({ Type: type, Month: 0, StepID: 0 });

    const addStep = (item) => item.Steps.push({ Begin: 0, End: 0 });

    const removeGoal = (goal) => goal.Deleted = true;

    const removeItem = (item) => item.Deleted = true;

    const removeSchedule = (schedule) => schedule.Deleted = true;

    const removeStep = (step) => step.Deleted = true;

    const save = (submit) => {
        errors.value = {};

        if (submit && !verify()) {
            nextTick(() => document.querySelector(".invalid")?.scrollIntoView({ behavior: "smooth", block: "center" }));
            return;
        }

        const data = {
            Project: form.value,
            Goals: goals.value,
            Submit: submit ? "true" : "false"
        };

        api.culture("saveWorkSchedule", data).subscribe(() => {
            if (submit) {
                window.location.href = `Funding.aspx?ID=${props.id}`;
            } else {
                window.location.reload();
            }
        });
    };

    const verify = () => {
        const rules = {
            StartTime: "計畫期程",
            EndTime: "計畫期程"
        };

        errors.value = validateData(form.value, rules);

        const goalRules = {
            Title: "計畫目標",
            Content: "預期效益"
        };

        const itemRules = {
            Title: "重要工作項目",
            Indicator: "績效指標"
        };

        const stepRules = {
            Title: "實施步驟",
            Begin: { numericality: { greaterThan: 0, message: "^請選擇起迄月份" } },
            End: { numericality: { greaterThan: 0, message: "^請選擇起迄月份" } }
        };

        const scheduleRules = {
            Month: { numericality: { greaterThan: 0, message: "^請選擇月份" } },
            StepID: { numericality: { greaterThan: 0, message: "^請選擇實施步驟" } }
        };

        goals.value.filter((goal) => !goal.Deleted).forEach((goal, idx) => {
            Object.assign(errors.value, validateData(goal, goalRules, `goal-${idx}-`));

            goal.Items.filter((item) => !item.Deleted).forEach((item, idx1) => {
                Object.assign(errors.value, validateData(item, itemRules, `item-${idx}-${idx1}-`));

                item.Steps.filter((step) => !step.Deleted).forEach((step, idx2) => Object.assign(errors.value, validateData(step, stepRules, `step-${idx}-${idx1}-${idx2}-`)));
                item.Schedules.filter((schedule) => schedule.Type === 1 && !schedule.Deleted).forEach((schedule, idx2) => Object.assign(errors.value, validateData(schedule, scheduleRules, `schedule-${idx}-${idx1}-1-${idx2}-`)));
                item.Schedules.filter((schedule) => schedule.Type === 2 && !schedule.Deleted).forEach((schedule, idx2) => Object.assign(errors.value, validateData(schedule, scheduleRules, `schedule-${idx}-${idx1}-2-${idx2}-`)));
            });
        });

        return !Object.keys(errors.value).length;
    };

    onMounted(() => {
        rxjs.forkJoin([
            api.culture("getWorkSchedule", { ID: props.id })
        ]).subscribe((result) => {
            const data = result[0];

            form.value = data.Project;
            goals.value = data.Goals;

            if (!goals.value.length) {
                addGoal();
            }

            useProgressStore().culture = { step: form.value.FormStep, status: form.value.Status };
        });
    });

    provide("editable", editable);
</script>
