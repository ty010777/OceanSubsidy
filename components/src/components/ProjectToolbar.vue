<template>
    <div class="block rounded-top-4 py-4 d-flex justify-content-between" style="position:sticky;top:180px;z-index:15">
        <div class="d-flex gap-2">
            <button class="btn text-teal-dark" type="button" v-if="store[props.type].progress === 2">
                計畫變更申請中
            </button>
            <button class="btn btn-teal-dark" @click="showApplyModal" type="button" v-else>
                計畫變更申請
            </button>
            <button class="btn btn-teal-dark" @click="showLogModal" type="button">
                <i class="fas fa-history"></i>
                計畫變更紀錄
            </button>
            <button class="btn btn-teal-dark" type="button">
                <i class="fa-solid fa-download"></i>
                下載核定計畫書
            </button>
        </div>
        <div class="d-flex gap-2">
            <project-organizer :id="id" :type="type"></project-organizer>
            <button class="btn btn-pink" @click="showTerminateModal" type="button">
                計畫終止
            </button>
        </div>
    </div>
    <teleport to="body">
        <div aria-hidden="true" class="modal fade" data-bs-backdrop="static" ref="applyModal" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-md">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="fs-24 text-green-light fw-bold">計畫變更申請</h4>
                        <button aria-label="Close" class="btn-close" data-bs-dismiss="modal" type="button">
                            <i class="fa-solid fa-circle-xmark"></i>
                        </button>
                    </div>
                    <div class="modal-body">
                        <p class="text-pink lh-base py-3">提醒您：計畫變更審核通過後，每月進度資料將清空，必須重填！</p>
                        <div class="text-gray pb-2">計畫變更原因</div>
                        <input-textarea :error="errors.Reason" v-model.trim="applyForm.Reason"></input-textarea>
                        <div class="d-flex justify-content-center mt-4 gap-4">
                            <button class="btn btn-gray" data-bs-dismiss="modal">取消</button>
                            <button class="btn btn-teal" @click="apply">確定變更</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div aria-hidden="true" class="modal fade" data-bs-backdrop="static" ref="logModal" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-xl">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="fs-24 text-green-light fw-bold">計畫變更紀錄</h4>
                        <button aria-label="Close" class="btn-close" data-bs-dismiss="modal" type="button">
                            <i class="fa-solid fa-circle-xmark"></i>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div class="table-responsive">
                            <table class="table align-middle gray-table lh-base">
                                <thead>
                                    <tr>
                                        <th width="150">版次</th>
                                        <th width="150">變更時間</th>
                                        <th>變更者</th>
                                        <th>變更原因</th>
                                        <th>變更前</th>
                                        <th>變更後</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>核定版</td>
                                        <td>114/04/10</td>
                                        <td>劉申慶</td>
                                        <td>意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容</td>
                                        <td>【期程／工作項目／查核】<br>
                                            1.OO資料，原為OOOOOOOO<br>
                                            2.OO資料，原為OOOOOOOO
                                        </td>
                                        <td>【期程／工作項目／查核】<br>
                                            1.OO資料，原為OOOOOOOO<br>
                                            2.OO資料，原為OOOOOOOO
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div aria-hidden="true" class="modal fade" data-bs-backdrop="static" ref="terminateModal" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-md">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="fs-24 text-green-light fw-bold">計畫終止</h4>
                        <button aria-label="Close" class="btn-close" data-bs-dismiss="modal" type="button">
                            <i class="fa-solid fa-circle-xmark"></i>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div class="d-flex flex-column gap-4 mb-5">
                            <div>
                                <div class="text-gray pb-2">計畫終止原因：</div>
                                <input-textarea :error="errors.RejectReason" v-model.trim="terminateForm.RejectReason"></input-textarea>
                            </div>
                            <div class="d-flex align-items-center">
                                <div class="text-gray text-nowrap">已撥款：</div>
                                <div>700,000</div>
                            </div>
                            <div class="d-flex align-items-center">
                                <div class="text-gray text-nowrap">已追回：</div>
                                <div class="flex-grow-1">
                                    <input-integer :error="errors.RecoveryAmount" v-model.trim="terminateForm.RecoveryAmount"></input-integer>
                                </div>
                            </div>
                        </div>
                        <div class="d-flex justify-content-center mt-4 gap-4">
                            <button class="btn btn-gray" data-bs-dismiss="modal">取消</button>
                            <button class="btn btn-teal" @click="terminate">送出</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </teleport>
</template>

<script setup>
    const store = useProgressStore();

    const props = defineProps({
        id: { type: [Number, String] },
        type: { type: String }
    });

    const applyForm = ref({});
    const applyModal = ref();
    const errors = ref({});
    const logModal = ref();
    const terminateForm = ref({});
    const terminateModal = ref();

    const apply = () => {
        const rules = {
            Reason: "計畫變更原因"
        };

        errors.value = validateData(applyForm.value, rules);

        if (Object.keys(errors.value).length) {
            return;
        }

        api[props.type]("applyChange", applyForm.value).subscribe((res) => {
            if (res) {
                // TODO
            }
        });
    };

    const showApplyModal = () => {
        applyForm.value = { ID: props.id };
        bootstrap.Modal.getOrCreateInstance(applyModal.value).show();
    };

    const showLogModal = () => {
        bootstrap.Modal.getOrCreateInstance(logModal.value).show();
    };

    const showTerminateModal = () => {
        terminateForm.value = { ID: props.id };
        bootstrap.Modal.getOrCreateInstance(terminateModal.value).show();
    };

    const terminate = () => {
        const rules = {
            RejectReason: "計畫終止原因",
            RecoveryAmount: "已追回金額"
        };

        errors.value = validateData(terminateForm.value, rules);

        if (Object.keys(errors.value).length) {
            return;
        }

        api[props.type]("terminate", terminateForm.value).subscribe((res) => {
            if (res) {
                // TODO
            }
        });
    };
</script>
