<template>
    <div class="block">
        <h5 class="square-title">{{ setting.DisbursementType }}{{ setting.Note }}</h5>
        <p class="text-teal fw-bold py-3">核定經費: {{ project.ApprovedAmount?.toLocaleString() }}元</p>
        <div class="table-responsive mb-0">
            <table class="table align-middle gray-table">
                <thead>
                    <tr>
                        <th class="text-end">本期請款金額(元)</th>
                        <th class="text-end">前期已撥付金額(元)</th>
                        <th>累積實支金額(元)</th>
                        <th class="text-end">累積經費執行率</th>
                        <th class="text-end">支用比</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td class="text-end">{{ amount }}</td>
                        <td class="text-end">--</td>
                        <td><input-integer :max="max" v-model="amount"></input-integer></td>
                        <td class="text-end">{{ rate }}%</td>
                        <td class="text-end">{{ rate2 }}</td>
                    </tr>
                </tbody>
            </table>
        </div>
        <template v-if="isApproved">
            <p class="text-teal fw-bold pb-3">本期實際撥款: {{ payment.CurrentActualPaidAmount.toLocaleString() }}元</p>
            <p class="text-teal fw-bold pb-3">累積實際撥款: {{ payment.CurrentActualPaidAmount.toLocaleString() }}元</p>
        </template>
        <h5 class="square-title mt-4">檔案上傳</h5>
        <p class="text-pink mt-3 lh-base">請下載附件範本，填寫資料及公文用印後上傳（僅支援PDF）。</p>
        <div class="mt-4">
            <table class="table align-middle gray-table">
                <thead class="text-center">
                    <tr>
                        <th width="60">附件編號</th>
                        <th>附件名稱</th>
                        <th width="180" v-if="editable">狀態</th>
                        <th width="350">上傳附件</th>
                    </tr>
                </thead>
                <tbody>
                    <tr :key="item" v-for="(item, idx) in docs">
                        <td class="text-center">
                            {{ idx + 1 }}
                        </td>
                        <td>
                            <div>
                                <required-label>{{ item.Title }}</required-label>
                            </div>
                            <a class="btn btn-sm btn-teal-dark rounded-pill mt-2" :href="item.Template" style="width:140px" type="button" v-if="editable && item.Template">
                                <i class="fas fa-file-download me-1"></i>範本下載
                            </a>
                        </td>
                        <td class="text-center" v-if="editable">
                            <span v-if="item.Uploaded">已上傳</span>
                            <span class="text-pink" v-else>尚未上傳</span>
                        </td>
                        <td>
                            <input-file accept=".pdf" @file="(file) => add(item, file)" v-if="editable && !item.Uploaded"></input-file>
                            <div class="tag-group mt-2 gap-1" v-if="item.Uploaded">
                                <template :key="file" v-for="(file) in item.Files">
                                    <span class="tag tag-green-light" v-if="!file.Deleted">
                                        <a class="tag-link my-1" :download="file.Name" :href="download(file.Path)">{{ file.Name }}</a>
                                        <button class="tag-btn" @click="file.Deleted = true" type="button" v-if="editable">
                                            <i class="fa-solid fa-circle-xmark"></i>
                                        </button>
                                    </span>
                                </template>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    <div class="block-bottom bg-light-teal" v-if="editable">
        <button class="btn btn-outline-teal me-3" @click="save()" type="button">暫存</button>
        <button class="btn btn-teal" @click="save(true)" :disabled="disabled" type="button"><i class="fas fa-check"></i>提送</button>
    </div>
</template>

<script setup>
    const props = defineProps({
        id: { type: [Number, String] },
        setting: { type: Object }
    });

    const amount = ref(0);
    const disabled = computed(() => docs.value.some((doc) => !doc.Files.filter((file) => !file.Deleted).length));
    const editable = computed(() => !payment.value || payment.value.Status === "請款中");
    const isApproved = computed(() => payment.value?.Status === "通過");
    const max = ref(0);
    const payment = ref();
    const project = ref({});
    const rate = computed(() => project.value.ApprovedAmount ? Math.min(Math.round(amount.value * 100 / project.value.ApprovedAmount * 100) / 100, 100) : 0);
    const rate2 = computed(() => isApproved.value ? (Math.round(amount.value * 100 / payment.value.CurrentActualPaidAmount * 100) / 100 + "%") : "--");

    const docs = ref([
        { Type: 11, Title: "領據", Files: [] }
    ]);

    const add = (item, file) => {
        if (file.Type === "application/pdf") {
            item.Files.push({
                Type: item.Type,
                Path: file.Path,
                Name: `${project.value.ProjectID}_第一期請款_${item.Title}.pdf`
            });
        }
    };

    const download = api.download;

    const emit = defineEmits(["next"]);

    const load = () => {
        api.accessibility("getPayment", { ID: props.id }).subscribe((result) => {
            project.value = result.Project;

            useProgressStore().init("accessibility", project.value);

            docs.value = docs.value.filter((doc) => !doc.Excludes?.includes(project.value.OrgCategory));

            docs.value.forEach((doc) => {
                doc.Files = result.Attachments.filter((file) => file.Type === doc.Type);
                doc.Uploaded = computed(() => doc.Files.filter((file) => !file.Deleted).length);
            });

            //--

            payment.value = result.Payments.find((item) => item.Stage === 1);

            if (payment.value) {
                amount.value = payment.value.TotalSpentAmount;
            }

            max.value = Math.round(project.value.ApprovedAmount * props.setting.DisbursementRatioPct / 100);

            usePaymentStore().init(payment.value);
        });
    };

    const save = (submit) => {
        const data = {
            Payment: {
                ProjectID: props.id,
                Stage: 1,
                ActDisbursementRatioPct: props.setting.DisbursementRatioPct,
                TotalSpentAmount: amount.value
            },
            Attachments: docs.value.reduce((list, item) => list.concat(item.Files), []),
            Submit: submit ? "true" : "false"
        };

        api.accessibility("savePayment", data).subscribe((res) => {
            if (res) {
                if (submit) {
                    emit("next", load);
                } else {
                    load();
                }
            }
        });
    };

    onMounted(load);

    provide("editable", editable);
</script>
