<template>
    <div class="d-flex align-items-center gap-3">
        <span>審核承辦人員：{{ name || "尚無" }}</span>
        <button class="btn btn-teal" @click="open" type="button">{{ title }}</button>
    </div>
    <teleport to="body">
        <div aria-hidden="true" class="modal fade" data-bs-backdrop="static" ref="modal" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-md">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="fs-24 fw-bold text-green-light">{{ title }}</h4>
                        <button aria-label="Close" class="btn-close" data-bs-dismiss="modal" type="button">
                            <i class="fa-solid fa-circle-xmark"></i>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div>
                            <div class="fs-16 text-gray mb-3">承辦人員</div>
                            <input-select :empty-value="0" :options="units" text-name="UnitName" value-name="UnitID" v-model="unit"></input-select>
                            <input-select class="mt-2" :empty-value="0" :options="reviewers" text-name="Name" value-name="UserID" v-model="reviewer"></input-select>
                        </div>
                        <div class="d-flex gap-4 flex-wrap justify-content-center mt-5">
                            <button class="btn btn-gray" data-bs-dismiss="modal" type="button">取消</button>
                            <button class="btn btn-teal" @click="submit" :disabled="disabled" type="button">確認</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </teleport>
</template>

<script setup>
    const props = defineProps({
        id: { type: Number },
        type: { type: String }
    });

    const disabled = computed(() => !reviewer.value || reviewer.value === store[props.type].organizer);
    const modal = ref();
    const name = computed(() => store[props.type].organizerName);
    const reviewer = ref(0);
    const reviewers = computed(() => units.value.find((item) => item.UnitID === unit.value)?.reviewers);
    const store = useProgressStore();
    const title = computed(() => name.value ? "移轉案件" : "認領案件");
    const unit = ref(0);
    const units = ref([]);

    const open = () => {
        api[props.type]("queryReviewUnits").subscribe((result) => {
            units.value = result;

            rxjs.forkJoin(units.value.map((item) => api[props.type]("queryReviewersByUnit", { ID: item.UnitID }))).subscribe((results) => {
                results.forEach((res, idx) => {
                    units.value[idx].reviewers = res;

                    if (res.some((re) => re.UserID === reviewer.value)) {
                        unit.value = units.value[idx].UnitID;
                    }
                });

                bootstrap.Modal.getOrCreateInstance(modal.value).show();
            });
        });
    };

    const submit = () => {
        api[props.type]("saveOrganizer", { ID: props.id, Organizer: reviewer.value }).subscribe((result) => {
            if (result) {
                window.location.reload();
            }
        });
    };

    watch(name, () => reviewer.value = store[props.type].organizer);

    watch(unit, () => {
        if (!reviewers.value.some((re) => re.UserID === reviewer.value)) {
            reviewer.value = 0;
        }
    });
</script>
