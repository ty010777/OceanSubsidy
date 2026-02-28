<template>
    <div class="block">
        <h5 class="square-title">期程</h5>
        <div class="mt-4">
            <table class="table align-middle gray-table side-table">
                <tbody>
                    <tr>
                        <th><required-label>計畫期程</required-label></th>
                        <td>
                            <div class="d-flex align-items-center gap-2" v-if="end">
                                <div class="input-group w-auto">
                                    <input-tw-date class="w-auto" :max-date="end" v-model="form.StartTime"></input-tw-date>
                                    <span class="input-group-text">至</span>
                                    <input-tw-date class="w-auto" :max-date="end" v-model="form.EndTime"></input-tw-date>
                                </div>
                                <span class="text-teal-dark">(期程不可超過 <tw-date :value="end"></tw-date>)</span>
                            </div>
                            <div class="invalid mt-0" v-if="errors.StartTime || errors.EndTime">{{ errors.StartTime || errors.EndTime }}</div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <h5 class="square-title mt-4">工作項目及預定進度</h5>
        <div class="table-responsive mt-4">
            <table class="table align-middle gray-table">
                <thead>
                    <tr>
                        <th>編號</th>
                        <th><required-label>工作項目</required-label></th>
                        <th><required-label>起訖月份</required-label></th>
                        <th><required-label>預定完成日</required-label></th>
                        <th><required-label>詳細執行內容說明<br><span class="text-danger" v-if="editable">（上限300字）</span></required-label></th>
                        <th v-if="editable">功能</th>
                    </tr>
                </thead>
                <tbody>
                    <tr :key="item" v-for="(item, idx) in filteredItems">
                        <td>
                            {{ String.fromCharCode(65 + idx) }}
                        </td>
                        <td>
                            <input-text :error="errors[`item-${idx}-Title`]" :max-length="100" v-model.trim="item.Title"></input-text>
                        </td>
                        <td>
                            <div class="input-group">
                                <input-month :empty-value="0" v-model="item.Begin"></input-month>
                                <input-month :empty-value="0" v-model="item.End"></input-month>
                            </div>
                            <div class="invalid mt-0" v-if="errors[`item-${idx}-Begin`] || errors[`item-${idx}-End`]">{{ errors[`item-${idx}-Begin`] || errors[`item-${idx}-End`] }}</div>
                        </td>
                        <td>
                            <input-tw-date :error="errors[`item-${idx}-Deadline`]" v-model="item.Deadline"></input-tw-date>
                        </td>
                        <td>
                            <input-textarea :error="errors[`item-${idx}-Content`]" :max-length="300" rows="1" v-model.trim="item.Content"></input-textarea>
                        </td>
                        <td v-if="editable">
                            <div class="d-flex gap-2">
                                <button class="btn btn-sm btn-teal-dark m-0" @click="remove(item)" type="button" v-if="filteredItems.length > 1"><i class="fas fa-trash-alt"></i></button>
                                <button class="btn btn-sm btn-teal-dark m-0" @click="addItem" type="button" v-if="idx + 1 === filteredItems.length"><i class="fas fa-plus"></i></button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <h5 class="square-title mt-4">進度說明</h5>
        <div class="mt-4">
            <table class="table align-middle gray-table">
                <thead>
                    <tr>
                        <th><required-label>完成百分比</required-label></th>
                        <th><required-label>對應工項</required-label></th>
                        <th><required-label>預定完成日</required-label></th>
                        <th><required-label>查核內容概述（請具體明確或量化）<br><span class="text-danger" v-if="editable">（上限300字）</span></required-label></th>
                        <th v-if="editable">功能</th>
                    </tr>
                </thead>
                <tbody>
                    <tr :key="schedule" v-for="(schedule, idx) in filteredSchedules">
                        <td>
                            <input-select :empty-value="0" :error="errors[`schedule-${idx}-Type`]" :options="types" v-model.trim="schedule.Type"></input-select>
                        </td>
                        <td>
                            <input-select :empty-value="0" :error="errors[`schedule-${idx}-ItemID`]" :options="filteredItems" v-model.trim="schedule.ItemID">
                                <template #default="{ index, option }">
                                    <option :value="option.ID">{{ String.fromCharCode(65 + index) }}.{{ option.Title }}</option>
                                </template>
                            </input-select>
                        </td>
                        <td>
                            <input-tw-date :error="errors[`schedule-${idx}-Deadline`]" v-model="schedule.Deadline"></input-tw-date>
                        </td>
                        <td>
                            <input-textarea :error="errors[`schedule-${idx}-Content`]" :max-length="300" rows="1" v-model.trim="schedule.Content"></input-textarea>
                        </td>
                        <td v-if="editable">
                            <div class="d-flex gap-2">
                                <button class="btn btn-sm btn-teal-dark m-0" @click="remove(schedule)" type="button" v-if="filteredSchedules.length > 1"><i class="fas fa-trash-alt"></i></button>
                                <button class="btn btn-sm btn-teal-dark m-0" @click="addSchedule" type="button" v-if="idx + 1 === filteredSchedules.length"><i class="fas fa-plus"></i></button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <template v-if="changeForm">
            <h5 class="square-title mt-5">變更說明</h5>
            <div class="mt-4">
                <table class="table align-middle gray-table side-table">
                    <tbody>
                        <tr>
                            <th><required-label>變更前</required-label></th>
                            <td>
                                <input-textarea :error="errors.Form2Before" rows="4" v-model.trim="changeForm.Form2Before"></input-textarea>
                            </td>
                        </tr>
                        <tr>
                            <th><required-label>變更後</required-label></th>
                            <td>
                                <input-textarea :error="errors.Form2After" rows="4" v-model.trim="changeForm.Form2After"></input-textarea>
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
</template>

<script setup>
    const props = defineProps({
        apply: { default: false, type: Boolean },
        id: { type: [Number, String] }
    });

    const types = [
        { value: 1, text: "50%" },
        { value: 2, text: "100%" }
    ];

    const changeForm = ref();
    const end = computed(() => info.value.PlanEndDate);
    const editable = computed(() => isProjectEditable("multiple", form.value.Status, 2));
    const errors = ref({});
    const filteredItems = computed(() => items.value.filter((item) => !item.Deleted));
    const filteredSchedules = computed(() => schedules.value.filter((item) => !item.Deleted));
    const form = ref({});
    const info = ref({});
    const items = ref([]);
    const schedules = ref([]);

    const addItem = () => api.multiple("createItem", { ID: props.id }).subscribe((result) => items.value.push(result));

    const addSchedule = () => schedules.value.push({ Type: 0, ItemID: 0 });

    const emit = defineEmits(["next"]);

    const load = () => {
        rxjs.forkJoin([
            api.multiple("getWorkSchedule", { Apply: props.apply ? "true" : "false", ID: props.id })
        ]).subscribe((result) => {
            const data = result[0];

            form.value = data.Project;
            info.value = data.GrantType;
            items.value = data.Items;
            schedules.value = data.Schedules;

            if (!items.value.length) {
                addItem();
            }

            if (!schedules.value.length) {
                addSchedule();
            }

            useProgressStore().init("multiple", form.value);

            changeForm.value = form.value.changeApply;
            form.value.changeApply = undefined;
        });
    };

    const remove = (item) => item.Deleted = true;

    const save = (submit) => {
        errors.value = {};

        if (submit && !verify()) {
            nextTick(() => document.querySelector(".invalid")?.scrollIntoView({ behavior: "smooth", block: "center" }));
            return;
        }

        const data = {
            Project: form.value,
            Items: items.value,
            Schedules: schedules.value,
            Before: changeForm.value?.Form2Before,
            After: changeForm.value?.Form2After,
            Submit: submit ? "true" : "false"
        };

        api.multiple("saveWorkSchedule", data).subscribe(() => {
            notify({ title: "成功", text: submit ? "儲存成功" : "暫存成功" }).then(() => {
                if (submit) {
                    emit("next");
                } else {
                    load();
                }
            });
        });
    };

    const verify = () => {
        let rules = {
            StartTime: "計畫期程",
            EndTime: "計畫期程"
        };

        errors.value = validateData(form.value, rules);

        rules = {
            Title: "工作項目",
            Begin: { numericality: { greaterThan: 0, message: "^請選擇起迄月份" } },
            End: { numericality: { greaterThan: 0, message: "^請選擇起迄月份" } },
            Deadline: "預定完成日",
            Content: "詳細執行內容說明"
        };

        filteredItems.value.forEach((item, idx) => Object.assign(errors.value, validateData(item, rules, `item-${idx}-`)));

        rules = {
            Type: { numericality: { greaterThan: 0, message: "^請選擇完成百分比" } },
            ItemID: { numericality: { greaterThan: 0, message: "^請選擇對應工項" } },
            Deadline: "預定完成日",
            Content: "查核內容概述"
        };

        filteredSchedules.value.forEach((schedule, idx) => Object.assign(errors.value, validateData(schedule, rules, `schedule-${idx}-`)));

        if (changeForm.value) {
            rules = {
                Form2Before: "變更前說明",
                Form2After: "變更後說明"
            };

            Object.assign(errors.value, validateData(changeForm.value, rules));
        }

        return !Object.keys(errors.value).length;
    };

    onMounted(load);

    provide("editable", editable);
</script>
