/**
 * ClbApproved.aspx JavaScript 功能
 * 社團補助案核定計畫頁面相關功能
 */

document.addEventListener('DOMContentLoaded', function() {
    // 綁定進度條點擊事件
    const stepItems = document.querySelectorAll('.application-step-container .step-item');
    const tabPanes = document.querySelectorAll('.tab-pane');

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
                
                // 如果切換到第二頁（附件），顯示 UserControl 的上傳附件區塊
                if (stepNumber === '2') {
                    const uploadSection = document.getElementById('uploadAttachmentSection');
                    const targetDiv = document.getElementById('uploadAttachmentFromUserControl');
                    
                    if (uploadSection && targetDiv) {
                        // 將 UserControl 的附件區塊移動到第二頁
                        targetDiv.appendChild(uploadSection);
                        uploadSection.style.display = 'block';
                    }
                }
            }
        }
    }

    // 初始化第一個步驟
    switchToApplicationStep('1');
    
    // 將函式掛載到全域，供外部呼叫
    window.switchToApplicationStep = switchToApplicationStep;
});

/**
 * 下載核定計畫書功能
 */
function downloadApprovedPlan() {
    alert('下載核定計畫書功能 (靜態展示)');
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