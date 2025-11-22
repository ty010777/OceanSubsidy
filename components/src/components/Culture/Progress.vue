<template>
    <div class="block rounded-top-4">
        <div class="horizontal-scrollable">
            <button class="btn-control btn-prev" type="button"><i class="fas fa-angle-left"></i></button>
            <ul class="timeline">
                <li :key="tab" v-for="(tab) in tabs">
                    <span class="year">{{ tab.year - 1911 }}年</span>
                    <ul class="month">
                        <li :class="{ active: current === item, disabled: item.disabled }" :key="item" v-for="(item) in tab.months">
                            <a @click="change(item)" href="javascript:void(0)">{{ item.month }}月</a>
                        </li>
                    </ul>
                </li>
            </ul>
            <button class="btn-control btn-next" type="button"><i class="fas fa-angle-right"></i></button>
        </div>
        <template v-if="current">
            <h5 class="square-title">{{ current.year - 1911 }}年{{ current.month }}月 實際進度</h5>
            <div class="table-responsive mt-4">
                <table class="table align-middle gray-table">
                    <thead>
                        <tr>
                            <th class="text-end">月份</th>
                            <th class="text-center">實際工作執行情形（條列式說明）</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <th class="text-end">{{ current.year - 1911 }}年{{ current.month }}月</th>
                            <td>
                                <input-textarea :error="errors.Description" rows="4" v-model.trim="progress.Description"></input-textarea>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="table-responsive">
                <table class="table align-middle gray-table">
                    <thead>
                        <tr>
                            <th class="text-end">月份</th>
                            <th>重要工作項目</th>
                            <th>工作進度%</th>
                            <th>檢核工作</th>
                            <th>本月是否完成</th>
                            <th>年度目標達成數</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr :key="item" v-for="(item,idx) in list">
                            <th class="text-end">{{ item.Year - 1911 }}年{{ item.Month }}月</th>
                            <td>{{ item.ItemTitle }}</td>
                            <td>{{ item.Type === 1 ? "50%" : "100%" }}</td>
                            <td>{{ item.StepTitle }}</td>
                            <td>
                                <input-radio-group :options="options" v-model="item.Status"></input-radio-group>
                                <div class="invalid mt-0" v-if="errors[`item-${idx}-Status`]">{{ errors[`item-${idx}-Status`] }}</div>
                                <template v-if="[1,2].includes(item.Status)">
                                    <div class="mt-3">
                                        <div class="text-teal pb-2">落後原因</div>
                                        <input-textarea :error="errors[`item-${idx}-DelayReason`]" rows="1" v-model.trim="item.DelayReason"></input-textarea>
                                    </div>
                                    <div class="mt-3">
                                        <div class="text-teal pb-2">改善措施</div>
                                        <input-textarea :error="errors[`item-${idx}-ImprovedWay`]" rows="1" v-model.trim="item.ImprovedWay"></input-textarea>
                                    </div>
                                </template>
                            </td>
                            <td class="text-end" :rowspan="list.length" v-if="!idx">
                                {{ rate }}%
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </template>
    </div>
    <div class="block-bottom bg-light-teal" v-if="current">
        <button class="btn btn-outline-teal me-3" @click="save()" type="button">暫存</button>
        <button class="btn btn-teal" @click="save(true)" type="button"><i class="fas fa-check"></i>提送</button>
    </div>
</template>

<script setup>
    const options = [
        { value: 3, text: "完成" },
        { value: 2, text: "部分完成" },
        { value: 1, text: "未完成" }
    ];

    const props = defineProps({
        id: { type: [Number, String] }
    });

    const current = ref();
    const errors = ref({});
    const finished = ref(0); //前期已完成總數
    const form = ref({});
    const list = ref([]);
    const progress = ref({});
    const rate = computed(() => Math.round((finished.value + list.value.filter((item) => item.Status === 3).length) * 100 / schedules.value.length * 10) / 10);
    const schedules = ref([]);
    const tabs = ref([]);

    const change = (target) => {
        current.value = target;

        load();
    };

    const emit = defineEmits(["next"]);

    const load = () => {
        api.culture("getMonthlyProgress", { ID: props.id, Year: current.value.year - 1911, Month: current.value.month }).subscribe((result) => {
            const logs = result.Logs;
            const items = result.Items;
            const steps = result.Steps;

            let found = false;

            finished.value = 0;
            progress.value = result.Progress;
            schedules.value = result.Schedules;
            list.value = [];

            tabs.value.forEach((tab) => {
                tab.months.forEach((item) => {
                    if (!found) {
                        if (item === current.value) {
                            found = true;
                        }

                        schedules.value.forEach((schedule) => {
                            if (schedule.Month === item.month) {
                                if (found || schedule.Status !== 3) {
                                    const log = logs.find((i) => i.ScheduleID === schedule.ID) || {
                                        ScheduleID: schedule.ID,
                                        Status: schedule.Status
                                    };

                                    list.value.push(Object.assign(log, {
                                        Year: tab.year,
                                        Month: schedule.Month,
                                        ItemTitle: items.find((i) => i.ID === schedule.ItemID)?.Title,
                                        Type: schedule.Type,
                                        StepTitle: steps.find((i) => i.ID === schedule.StepID)?.Title,
                                    }));
                                } else {
                                    finished.value++;
                                }
                            }
                        });
                    }
                });
            });
        });
    };

    const save = (submit) => {
        errors.value = {};

        if (submit && !verify()) {
            nextTick(() => document.querySelector(".invalid")?.scrollIntoView({ behavior: "smooth", block: "center" }));
            return;
        }

        const data = {
            ID: progress.value.ID,
            Description: progress.value.Description || "",
            Status: submit ? 1 : 0,
            Logs: list.value.map((item) => ({
                ID: item.ID || 0,
                ScheduleID: item.ScheduleID,
                Status: item.Status,
                DelayReason: item.DelayReason || "",
                ImprovedWay: item.ImprovedWay || ""
            }))
        };

        api.culture("saveMonthlyProgress", data).subscribe((res) => {
            if (res && submit) {
                emit("next");
            }
        });
    };

    const verify = () => {
        let rules = {
            Description: "實際工作執行情形"
        };

        errors.value = validateData(progress.value, rules);

        list.value.forEach((item, index) => {
            rules = {
                Status: { numericality: { greaterThan: 0, message: "^請輸入本月是否完成" } }
            };

            if ([1,2].includes(item.Status)) {
                rules["DelayReason"] = "落後原因";
                rules["ImprovedWay"] = "改善措施";
            }

            Object.assign(errors.value, validateData(item, rules, `item-${index}-`));
        });

        return !Object.keys(errors.value).length;
    };

    onMounted(() => {
        rxjs.forkJoin([
            api.culture("getApplication", { ID: props.id }),
            api.culture("getSubmitedMonthlyProgress", { ID: props.id })
        ]).subscribe((result) => {
            form.value = result[0].Project;

            useProgressStore().init("culture", form.value);

            const start = new Date(form.value.StartTime);
            const end = new Date(form.value.EndTime);
            const now = Date.now();

            while (true) {
                const year = start.getFullYear();
                const month = start.getMonth();

                const data = {
                    year,
                    month: month + 1,
                    disabled: new Date(year, month, 20).getTime() > now
                };

                let tab = tabs.value.find((item) => item.year === year);

                if (tab) {
                    tab.months.push(data);
                } else {
                    tabs.value.push({ year, months: [data] });
                }

                if (year * 100 + month >= end.getFullYear() * 100 + end.getMonth()) {
                    break;
                }

                start.setMonth(month + 1);
            }

            //--

            let found = false;
            const submited = result[1].List;

            tabs.value.forEach((tab) => {
                tab.months.forEach((data) => {
                    if (!found && !submited.some((item) => item.Year === data.year - 1911 && item.Month === data.month)) {
                        found = true;
                        change(data);
                    }
                });
            });

            if (!found) {
                change(tabs.value[0].months[0]);
            }
        });
    });
</script>
