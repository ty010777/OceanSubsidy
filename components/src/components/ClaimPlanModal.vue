<template>
    <div class="modal fade" id="claimPlanModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="claimPlanModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-md">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="fs-24 fw-bold text-green-light">認領案件</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </div>
                <div class="modal-body">

                    <div class="">
                        <div class="fs-16 text-gray mb-3">承辦人員</div>
                        <select class="form-select" name="" id="">
                            <option value="">海洋委員會科技文教處科技科</option>
                            <option value="">海洋委員會科技文教處科技科</option>
                            <option value="">海洋委員會科技文教處科技科</option>
                            <option value="">海洋委員會科技文教處科技科</option>
                        </select>
                        <select class="form-select mt-2" name="" id="">
                            <option value="">正小名</option>
                            <option value="">正小名</option>
                            <option value="">正小名</option>
                            <option value="">正小名</option>
                        </select>
                    </div>

                    <div class="d-flex gap-4 flex-wrap justify-content-center mt-5">
                        <button type="button" class="btn btn-gray" data-bs-dismiss="modal">
                            取消
                        </button>
                        <button type="button" class="btn btn-teal" onclick="window.location.href='information-plan-review-get.html'">
                            確認
                        </button>
                    </div>

                </div>

            </div>
        </div>
    </div>
</template>

<script setup>
</script>
