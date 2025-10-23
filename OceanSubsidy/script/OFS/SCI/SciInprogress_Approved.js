// SciInprogress_Approved 頁面 JavaScript

document.addEventListener('DOMContentLoaded', function() {
    initApplicationStepSwitch();
    initDownloadPlan();
    initProjectChangeModal();
    initChangeReviewPanel();
    initPlanStopModal();
});

/**
 * 初始化申請表進度切換功能
 */
function initApplicationStepSwitch() {
    // 綁定進度條點擊事件
    const stepItems = document.querySelectorAll('.application-step-container .step-item');
    const tabPanes = document.querySelectorAll('.tab-pane');
    const hdnCurrentStep = document.getElementById('hdnCurrentStep');

    stepItems.forEach((item, index) => {
        item.addEventListener('click', function() {
            const stepNumber = this.getAttribute('data-application-step');
            switchToApplicationStep(stepNumber);
        });
    });

    // 切換申請表步驟
    function switchToApplicationStep(stepNumber) {
        // 移除所有 active 狀態
        stepItems.forEach(item => {
            item.classList.remove('active');
            const statusElement = item.querySelector('.step-status');
            if (statusElement) {
                statusElement.textContent = '';
            }
        });

        // 設定新的 active 狀態
        const targetStep = document.querySelector(`[data-application-step="${stepNumber}"]`);
        if (targetStep) {
            targetStep.classList.add('active');
            let statusElement = targetStep.querySelector('.step-status');
            if (!statusElement) {
                statusElement = document.createElement('div');
                statusElement.className = 'step-status';
                targetStep.querySelector('.step-content').appendChild(statusElement);
            }
            statusElement.textContent = '檢視中';
        }

        // 切換對應的分頁內容
        if (tabPanes.length > 0) {
            tabPanes.forEach(pane => {
                pane.classList.remove('active');
                pane.style.display = 'none';
            });

            const targetTab = document.getElementById(`tab${stepNumber}`);
            if (targetTab) {
                targetTab.classList.add('active');
                targetTab.style.display = 'block';
            }
        }

        // 儲存當前頁簽狀態到 HiddenField
        if (hdnCurrentStep) {
            hdnCurrentStep.value = stepNumber;
        }
    }

    // 從 HiddenField 恢復頁簽狀態（PostBack 後）
    let initialStep = '1';
    if (hdnCurrentStep && hdnCurrentStep.value) {
        initialStep = hdnCurrentStep.value;
    }

    // 初始化到儲存的頁簽或第一個步驟
    switchToApplicationStep(initialStep);
}

/**
 * 顯示查核紀錄
 */
function showAuditRecords() {
    const projectID = getProjectIDFromUrl();
    if (!projectID) {
        Swal.fire({
            title: '錯誤',
            text: '找不到計畫ID',
            icon: 'error',
            confirmButtonText: '確定'
        });
        return;
    }

    // 引導到查核紀錄頁面
    const auditRecordsUrl = `../AuditRecords.aspx?ProjectID=${encodeURIComponent(projectID)}`;
    window.location.href = auditRecordsUrl;
}

/**
 * 從 URL 取得專案ID
 */
function getProjectIDFromUrl() {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('ProjectID');
}

/**
 * 初始化下載核定版計畫書功能
 */
function initDownloadPlan() {
    var btnDownloadPlan = document.getElementById('btnDownloadPlan');
    if (btnDownloadPlan) {
        btnDownloadPlan.addEventListener('click', function() {
            var projectId = getProjectIDFromUrl();

            if (!projectId) {
                Swal.fire({
                    title: '錯誤',
                    text: '找不到計畫ID',
                    icon: 'error',
                    confirmButtonText: '確定'
                });
                return;
            }

            // 直接開啟下載 URL
            // 注意：這裡的 URL 需要在 ASPX 中動態設定，或者使用相對路徑

            var downloadUrl = window.location.origin + window.AppRootPath + '/Service/SCI_Download.ashx?action=downloadApprovedPlan&projectID=' + projectId;
            window.open(downloadUrl, '_blank');
        });
    }
}

/**
 * 初始化計畫變更 Modal
 */
function initProjectChangeModal() {
    const txtChangeReason = document.getElementById('txtChangeReason');
    const btnConfirmChange = document.getElementById('btnConfirmChange');
    const changePlanModal = document.getElementById('changePlanModal');

    // 確定變更按鈕
    if (btnConfirmChange) {
        btnConfirmChange.addEventListener('click', function() {
            handleProjectChange();
        });
    }

    // Modal 關閉時清空內容
    if (changePlanModal) {
        changePlanModal.addEventListener('hidden.bs.modal', function() {
            if (txtChangeReason) {
                txtChangeReason.value = '';
            }
        });
    }
}

/**
 * 處理計畫變更提交
 */
function handleProjectChange() {
    const txtChangeReason = document.getElementById('txtChangeReason');
    const changeReason = txtChangeReason ? txtChangeReason.value.trim() : '';

    // 驗證是否填寫變更原因
    if (!changeReason) {
        Swal.fire({
            title: '提醒',
            text: '請填寫計畫變更原因',
            icon: 'warning',
            confirmButtonText: '確定'
        });
        return;
    }

    // 再次確認
    Swal.fire({
        title: '確認提交',
        html: '確定要提交計畫變更申請嗎？<br><span class="text-danger fw-bold">審核通過後，預定進度及每月進度資料將清空！</span>',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: '確定提交',
        cancelButtonText: '取消',
        confirmButtonColor: '#2b9b9e',
        cancelButtonColor: '#6c757d'
    }).then((result) => {
        if (result.isConfirmed) {
            submitProjectChange(changeReason);
        }
    });
}

/**
 * 提交計畫變更申請到後端
 */
function submitProjectChange(changeReason) {
    const projectID = getProjectIDFromUrl();

    if (!projectID) {
        Swal.fire({
            title: '錯誤',
            text: '找不到計畫ID',
            icon: 'error',
            confirmButtonText: '確定'
        });
        return;
    }

    // 顯示載入中
    Swal.fire({
        title: '處理中...',
        text: '正在提交計畫變更申請',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    // 呼叫 WebMethod
    $.ajax({
        type: 'POST',
        url: 'SciInprogress_Approved.aspx/SubmitProjectChange',
        data: JSON.stringify({
            projectID: projectID,
            changeReason: changeReason
        }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function(response) {
            const data = response.d;

            if (data.success) {
                Swal.fire({
                    title: '提交成功',
                    text: '計畫變更申請已提交，頁面即將重新載入',
                    icon: 'success',
                    confirmButtonText: '確定'
                }).then(() => {
                    // 關閉 modal
                    const modal = bootstrap.Modal.getInstance(document.getElementById('changePlanModal'));
                    if (modal) {
                        modal.hide();
                    }

                    // 重新載入頁面並切換到編輯模式
                    location.reload();
                });
            } else {
                Swal.fire({
                    title: '提交失敗',
                    text: data.message || '系統發生錯誤，請稍後再試',
                    icon: 'error',
                    confirmButtonText: '確定'
                });
            }
        },
        error: function(xhr, status, error) {
            console.error('Error:', error);
            Swal.fire({
                title: '系統錯誤',
                text: '無法連接到伺服器，請稍後再試',
                icon: 'error',
                confirmButtonText: '確定'
            });
        }
    });
}

/**
 * 初始化計畫變更審查面板
 */
function initChangeReviewPanel() {
    // 綁定 radio 按鈕的 change 事件
    const radioPass = document.getElementById('radioPass');
    const radioReject = document.getElementById('radioReject');

    if (radioPass) {
        radioPass.addEventListener('change', toggleRejectReason);
    }

    if (radioReject) {
        radioReject.addEventListener('change', toggleRejectReason);
    }
}

/**
 * 切換退回原因輸入框的顯示
 */
function toggleRejectReason() {
    const rejectRadio = document.getElementById('radioReject');
    const rejectReasonSection = document.getElementById('rejectReasonSection');

    if (rejectRadio && rejectRadio.checked) {
        rejectReasonSection.classList.remove('d-none');
    } else {
        rejectReasonSection.classList.add('d-none');
    }
}

/**
 * 提交計畫變更審查
 */
function submitChangeReview() {
    // 取得審查結果
    const reviewResult = document.querySelector('input[name="reviewResult"]:checked');
    if (!reviewResult) {
        Swal.fire('錯誤', '請選擇審查結果', 'error');
        return;
    }

    const isPass = reviewResult.value === 'pass';
    let rejectReason = '';

    // 如果是不通過，檢查是否填寫退回原因
    if (!isPass) {
        const rejectReasonText = document.getElementById('rejectReasonText');
        rejectReason = rejectReasonText.textContent.trim();

        if (!rejectReason || rejectReason === '請輸入退回原因') {
            Swal.fire('錯誤', '不通過時請輸入退回原因', 'error');
            return;
        }
    }

    // 取得 ProjectID
    const projectID = getProjectIDFromUrl();

    if (!projectID) {
        Swal.fire('錯誤', '找不到計畫ID', 'error');
        return;
    }

    // 確認提交
    Swal.fire({
        title: '確定提交計畫變更審查？',
        text: '審查結果：' + (isPass ? '通過' : '不通過'),
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: '確定',
        cancelButtonText: '取消'
    }).then((result) => {
        if (result.isConfirmed) {
            // 顯示載入中
            Swal.fire({
                title: '處理中...',
                text: '正在提交審查結果',
                allowOutsideClick: false,
                allowEscapeKey: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });

            // 呼叫後端 WebMethod
            $.ajax({
                type: 'POST',
                url: 'SciInprogress_Approved.aspx/SubmitChangeReview',
                data: JSON.stringify({
                    projectID: projectID,
                    isPass: isPass,
                    rejectReason: rejectReason
                }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function(response) {
                    const data = response.d;

                    if (data.success) {
                        Swal.fire({
                            title: '審查完成',
                            text: data.message,
                            icon: 'success',
                            confirmButtonText: '確定'
                        }).then(() => {
                            // 重新載入頁面
                            location.reload();
                        });
                    } else {
                        Swal.fire({
                            title: '審查失敗',
                            text: data.message || '系統發生錯誤，請稍後再試',
                            icon: 'error',
                            confirmButtonText: '確定'
                        });
                    }
                },
                error: function(xhr, status, error) {
                    console.error('Error:', error);
                    Swal.fire({
                        title: '系統錯誤',
                        text: '無法連接到伺服器，請稍後再試',
                        icon: 'error',
                        confirmButtonText: '確定'
                    });
                }
            });
        }
    });
}

/**
 * 初始化計畫終止 Modal
 */
function initPlanStopModal() {
    const planStopModal = document.getElementById('planStopModal');
    const txtStopReason = document.getElementById('txtStopReason');
    const txtPaidAmount = document.getElementById('txtPaidAmount');
    const txtRecoveredAmount = document.getElementById('txtRecoveredAmount');
    const btnConfirmPlanStop = document.getElementById('btnConfirmPlanStop');

    if (!planStopModal) {
        return;
    }

    // Modal 打開時載入已撥款金額
    planStopModal.addEventListener('show.bs.modal', function() {
        loadPaidAmount();
    });

    // Modal 關閉時清空內容
    planStopModal.addEventListener('hidden.bs.modal', function() {
        if (txtStopReason) {
            txtStopReason.value = '';
        }
        if (txtRecoveredAmount) {
            txtRecoveredAmount.value = '';
        }
        if (txtPaidAmount) {
            txtPaidAmount.value = '載入中...';
        }
    });

    // 送出按鈕點擊事件
    if (btnConfirmPlanStop) {
        btnConfirmPlanStop.addEventListener('click', function() {
            handlePlanStop();
        });
    }
}

/**
 * 載入已撥款金額
 */
function loadPaidAmount() {
    const txtPaidAmount = document.getElementById('txtPaidAmount');
    const projectID = getProjectIDFromUrl();

    if (!projectID) {
        if (txtPaidAmount) {
            txtPaidAmount.value = '無法取得計畫ID';
        }
        return;
    }

    // 呼叫後端 API 取得已撥款金額
    $.ajax({
        type: 'POST',
        url: 'SciInprogress_Approved.aspx/GetPaidAmount',
        data: JSON.stringify({ projectID: projectID }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function(response) {
            if (response.d && response.d.success) {
                // 格式化金額顯示（加上千分位）
                txtPaidAmount.value = response.d.paidAmount.toLocaleString();
            } else {
                txtPaidAmount.value = '0';
            }
        },
        error: function(xhr, status, error) {
            console.error('載入已撥款金額失敗:', error);
            txtPaidAmount.value = '載入失敗';
        }
    });
}

/**
 * 處理計畫終止提交
 */
function handlePlanStop() {
    const txtStopReason = document.getElementById('txtStopReason');
    const txtPaidAmount = document.getElementById('txtPaidAmount');
    const txtRecoveredAmount = document.getElementById('txtRecoveredAmount');

    const stopReason = txtStopReason ? txtStopReason.value.trim() : '';
    const paidAmount = txtPaidAmount ? txtPaidAmount.value : '';
    const recoveredAmount = txtRecoveredAmount ? txtRecoveredAmount.value.trim() : '';

    // 驗證必填欄位
    if (!stopReason) {
        Swal.fire({
            title: '提醒',
            text: '請填寫計畫終止原因',
            icon: 'warning',
            confirmButtonText: '確定'
        });
        return;
    }

    if (!recoveredAmount || recoveredAmount === '') {
        Swal.fire({
            title: '提醒',
            text: '請填寫已追回金額',
            icon: 'warning',
            confirmButtonText: '確定'
        });
        return;
    }

    // 驗證已追回金額是否為有效數字
    const recoveredAmountNum = parseFloat(recoveredAmount);
    if (isNaN(recoveredAmountNum) || recoveredAmountNum < 0) {
        Swal.fire({
            title: '提醒',
            text: '已追回金額必須為有效的正數',
            icon: 'warning',
            confirmButtonText: '確定'
        });
        return;
    }

    // 確認提交
    Swal.fire({
        title: '確認送出',
        html: `
            <div class="text-start">
                <p><strong>計畫終止原因：</strong>${stopReason}</p>
                <p><strong>已撥款金額：</strong>${paidAmount} 元</p>
                <p><strong>已追回金額：</strong>${recoveredAmount} 元</p>
                <p class="text-danger mt-3">確定要終止此計畫嗎？</p>
            </div>
        `,
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: '確定送出',
        cancelButtonText: '取消',
        confirmButtonColor: '#d33',
        cancelButtonColor: '#6c757d'
    }).then((result) => {
        if (result.isConfirmed) {
            submitPlanStop(stopReason, paidAmount, recoveredAmount);
        }
    });
}

/**
 * 提交計畫終止到後端
 */
function submitPlanStop(stopReason, paidAmount, recoveredAmount) {
    const projectID = getProjectIDFromUrl();

    if (!projectID) {
        Swal.fire({
            title: '錯誤',
            text: '找不到計畫ID',
            icon: 'error',
            confirmButtonText: '確定'
        });
        return;
    }

    // 顯示載入中
    Swal.fire({
        title: '處理中...',
        text: '正在提交計畫終止申請',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    // 呼叫 WebMethod
    $.ajax({
        type: 'POST',
        url: 'SciInprogress_Approved.aspx/SubmitPlanStop',
        data: JSON.stringify({
            projectID: projectID,
            stopReason: stopReason,
            recoveredAmount: parseFloat(recoveredAmount)
        }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function(response) {
            const data = response.d;

            if (data.success) {
                Swal.fire({
                    title: '送出成功',
                    text: '計畫終止資料已儲存',
                    icon: 'success',
                    confirmButtonText: '確定'
                }).then(() => {
                    // 關閉 modal
                    const modal = bootstrap.Modal.getInstance(document.getElementById('planStopModal'));
                    if (modal) {
                        modal.hide();
                    }

                    // 重新載入頁面
                    location.reload();
                });
            } else {
                Swal.fire({
                    title: '送出失敗',
                    text: data.message || '系統發生錯誤，請稍後再試',
                    icon: 'error',
                    confirmButtonText: '確定'
                });
            }
        },
        error: function(xhr, status, error) {
            console.error('Error:', error);
            Swal.fire({
                title: '系統錯誤',
                text: '無法連接到伺服器，請稍後再試',
                icon: 'error',
                confirmButtonText: '確定'
            });
        }
    });
}
