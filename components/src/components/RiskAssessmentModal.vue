<template>
    <div class="modal fade" id="riskAssessmentModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="riskAssessmentModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-xl">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="fs-24 fw-bold text-green-light">風險評估</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </div>
                <div class="modal-body">

                    <div class="bg-light-gray p-3 mb-4">
                        <ul class="lh-lg">
                            <li>
                                <span class="text-gray">執行單位:</span>
                                <span>海洋委員會科技文教處科技科</span>
                            </li>
                            <li>
                                <span class="text-gray">風險評估:</span>
                                <span class="text-pink">中風險</span>
                            </li>
                        </ul>
                    </div>


                    <div class="table-responsive">
                        <table class="table align-middle gray-table lh-base">
                            <thead>
                                <tr>
                                    <th>計畫編號 / 計畫名稱
                                    </th>
                                    <th>查核日期 / 人員</th>
                                    <th>風險評估</th>
                                    <th>查核意見</th>
                                    <th>執行單位回覆</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>
                                        <a href="#" class="link-teal">SCI1140001 / 海洋環境監測預警系統建置計畫</a>
                                    </td>
                                    <td class="text-center">
                                        <div>114/08/30</div>
                                        <div>劉某人</div>
                                    </td>
                                    <td>中風險</td>
                                    <td>申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆</td>
                                    <td>申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆</td>
                                </tr>

                            </tbody>
                        </table>
                    </div>

                </div>

            </div>
        </div>
    </div>
</template>

<script setup>
</script>
