/**
 * ClbApproved.aspx JavaScript 功能
 * 社團補助案核定計畫頁面相關功能
 */

// document.addEventListener('DOMContentLoaded', function() {
    // 綁定進度條點擊事件
    // const stepItems = document.querySelectorAll('.application-step-container .step-item');
    // const tabPanes = document.querySelectorAll('.tab-pane');
    //
    // stepItems.forEach((item, index) => {
    //     item.addEventListener('click', function() {
    //         const stepNumber = this.getAttribute('data-application-step');
    //         // switchToApplicationStep(stepNumber);
    //     });
    // });

    // 切換申請表步驟
//     function switchToApplicationStep(stepNumber) {
//         // 移除所有 active 狀態
//         stepItems.forEach(item => {
//             item.classList.remove('active');
//             const statusElement = item.querySelector('.step-status');
//             if (statusElement) {
//                 statusElement.textContent = '';
//             }
//         });
//
//         // 設定新的 active 狀態
//         const targetStep = document.querySelector(`[data-application-step="${stepNumber}"]`);
//         if (targetStep) {
//             targetStep.classList.add('active');
//             let statusElement = targetStep.querySelector('.step-status');
//             if (!statusElement) {
//                 statusElement = document.createElement('div');
//                 statusElement.className = 'step-status';
//                 targetStep.querySelector('.step-content').appendChild(statusElement);
//             }
//             statusElement.textContent = '檢視中';
//         }
//
//         // 切換對應的分頁內容
//         if (tabPanes.length > 0) {
//             tabPanes.forEach(pane => {
//                 pane.classList.remove('active');
//                 pane.style.display = 'none';
//             });
//
//             const targetTab = document.getElementById(`tab${stepNumber}`);
//             if (targetTab) {
//                 targetTab.classList.add('active');
//                 targetTab.style.display = 'block';
//                
//                 // 如果切換到第二頁（附件），顯示 UserControl 的上傳附件區塊
//                 if (stepNumber === '2') {
//                     const uploadSection = document.getElementById('uploadAttachmentSection');
//                     const targetDiv = document.getElementById('uploadAttachmentFromUserControl');
//                    
//                     if (uploadSection && targetDiv) {
//                         // 將 UserControl 的附件區塊移動到第二頁
//                         targetDiv.appendChild(uploadSection);
//                         uploadSection.style.display = 'block';
//                     }
//                 }
//             }
//         }
//     }
//
//     // 初始化第一個步驟
//     switchToApplicationStep('1');
//    
//     // 將函式掛載到全域，供外部呼叫
//     window.switchToApplicationStep = switchToApplicationStep;
// });

/**
 * 下載核定計畫書功能
 */
function downloadApprovedPlan() {
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

    // 構建下載 URL
    const downloadUrl = window.location.origin + window.AppRootPath + '/Service/CLB_download.ashx?action=downloadApprovedPlan&projectID=' + encodeURIComponent(projectID);

    // 直接開啟新視窗下載檔案
    window.open(downloadUrl, '_blank');
}

/**
 * 顯示計畫變更紀錄功能
 */
function showChangeHistory() {
    alert('顯示計畫變更紀錄功能 (靜態展示)');
}

/**
 * 計畫變更申請功能
 */
function applyChange() {
    alert('計畫變更申請功能 (靜態展示)');
}

/**
 * 確認計畫變更申請
 */
function confirmPlanChange() {
    const changeReason = document.getElementById('planChangeReason').value.trim();

    if (!changeReason) {
        Swal.fire({
            title: '錯誤',
            text: '請輸入計畫變更原因',
            icon: 'error',
            confirmButtonText: '確定'
        });
        return;
    }

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

    // 顯示確認對話框
    Swal.fire({
        title: '確認變更',
        text: '確定要申請計畫變更嗎？',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: '確定',
        cancelButtonText: '取消'
    }).then((result) => {
        if (result.isConfirmed) {
            // 顯示載入中
            Swal.fire({
                title: '處理中...',
                text: '正在處理計畫變更申請',
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });

            // 發送 AJAX 請求
            fetch('ClbApproved.aspx/ProcessPlanChange', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    ProjectID: projectID,
                    ChangeReason: changeReason
                })
            })
            .then(response => response.json())
            .then(result => {
                if (result.d && result.d.success) {
                    // 關閉 Modal
                    const modal = bootstrap.Modal.getInstance(document.getElementById('planChangeModal'));
                    if (modal) {
                        modal.hide();
                    }

                    // 清空變更原因輸入框
                    document.getElementById('planChangeReason').value = '';

                    // 顯示成功訊息並重新載入頁面
                    Swal.fire({
                        title: '成功',
                        text: '計畫變更申請已通過，現在可以編輯申請表',
                        icon: 'success',
                        confirmButtonText: '確定'
                    }).then(() => {
                        // 重新載入頁面以更新 IsReadOnly 狀態
                        window.location.reload();
                    });
                } else {
                    Swal.fire({
                        title: '錯誤',
                        text: result.d ? result.d.message : '處理計畫變更申請時發生錯誤',
                        icon: 'error',
                        confirmButtonText: '確定'
                    });
                }
            })
            .catch(error => {
                console.error('計畫變更申請 AJAX 錯誤:', error);
                Swal.fire({
                    title: '錯誤',
                    text: '處理計畫變更申請時發生網路錯誤，請稍後再試',
                    icon: 'error',
                    confirmButtonText: '確定'
                });
            });
        }
    });
}

/**
 * 顯示查核紀錄功能
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
 * 確認移轉案件 (使用 AJAX 避免 PostBack)
 */
function confirmTransfer() {
    // 取得選擇的部門和承辦人員
    const ddlDepartment = document.querySelector('[id*="ddlDepartment"]');
    const ddlReviewer = document.querySelector('[id*="ddlReviewer"]');
    
    if (!ddlDepartment || !ddlReviewer) {
        Swal.fire({
            title: '錯誤',
            text: '找不到選擇控制項',
            icon: 'error',
            confirmButtonText: '確定'
        });
        return;
    }
    
    const selectedDepartmentID = ddlDepartment.value;
    const selectedReviewerAccount = ddlReviewer.value;
    const departmentName = ddlDepartment.options[ddlDepartment.selectedIndex].text;
    const reviewerText = ddlReviewer.options[ddlReviewer.selectedIndex].text;
    
    // 驗證必填欄位
    if (!selectedDepartmentID) {
        Swal.fire({
            title: '錯誤',
            text: '請選擇部門',
            icon: 'error',
            confirmButtonText: '確定'
        });
        return;
    }
    
    if (!selectedReviewerAccount) {
        Swal.fire({
            title: '錯誤',
            text: '請選擇承辦人員',
            icon: 'error',
            confirmButtonText: '確定'
        });
        return;
    }
    
    // 提取承辦人員姓名（移除帳號部分）
    let reviewerName = reviewerText;
    if (reviewerName.includes('(') && reviewerName.includes(')')) {
        reviewerName = reviewerName.split('(')[0].trim();
    }
    
    // 取得專案ID
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
    
    // 使用 AJAX 呼叫後端方法
    const data = {
        ProjectID: projectID,
        DepartmentID: selectedDepartmentID,
        DepartmentName: departmentName,
        ReviewerAccount: selectedReviewerAccount,
        ReviewerName: reviewerName
    };
    
    // 顯示載入中
    Swal.fire({
        title: '處理中...',
        text: '正在移轉案件',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    
    // 發送 AJAX 請求
    fetch('ClbApproved.aspx/TransferProject', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data)
    })
    .then(response => response.json())
    .then(result => {
        if (result.d && result.d.success) {
            // 更新頁面上的承辦人員顯示
            const lblReviewerName = document.querySelector('[id*="lblReviewerName"]');
            if (lblReviewerName) {
                lblReviewerName.textContent = reviewerName;
            }
            
            // 清空選項並關閉 Modal
            ddlDepartment.selectedIndex = 0;
            ddlReviewer.innerHTML = '<option value="">請選擇承辦人員</option>';
            
            // 關閉 Modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('transferCaseModal'));
            if (modal) {
                modal.hide();
            }
            
            // 顯示成功訊息
            Swal.fire({
                title: '成功',
                text: '案件移轉完成',
                icon: 'success',
                confirmButtonText: '確定'
            });
        } else {
            // 顯示錯誤訊息
            Swal.fire({
                title: '錯誤',
                text: result.d ? result.d.message : '移轉案件時發生錯誤',
                icon: 'error',
                confirmButtonText: '確定'
            });
        }
    })
    .catch(error => {
        console.error('移轉案件 AJAX 錯誤:', error);
        Swal.fire({
            title: '錯誤',
            text: '移轉案件時發生網路錯誤，請稍後再試',
            icon: 'error',
            confirmButtonText: '確定'
        });
    });
}

/**
 * 從 URL 取得專案ID
 */
function getProjectIDFromUrl() {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('ProjectID');
}

/**
 * DOMContentLoaded 事件監聽
 */
document.addEventListener('DOMContentLoaded', function() {
    initPlanStopModal();
});

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
        url: 'ClbApproved.aspx/GetPaidAmount',
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
        url: 'ClbApproved.aspx/SubmitPlanStop',
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