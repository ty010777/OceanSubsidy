// ClbPayment.js - CLB 請款頁面 JavaScript

// 下載範本功能
function downloadTemplate(fileType) {
    try {
        // 取得 ProjectID 參數
        const urlParams = new URLSearchParams(window.location.search);
        const projectID = urlParams.get('ProjectID') || '';

        // 取得網站根路徑 (從當前路徑往上三層到根目錄)
        const baseUrl = '../../../';

        // 建立下載連結
        let downloadUrl = `${baseUrl}Service/CLB_download.ashx?action=template&type=payment&fileType=${fileType}`;
        if (projectID) {
            downloadUrl += '&projectID=' + encodeURIComponent(projectID);
        }

        // 使用 window.open 開啟下載
        window.open(downloadUrl, '_blank');

    } catch (error) {
        console.error('下載範本時發生錯誤:', error);
        Swal.fire({
            title: '下載失敗！',
            text: '下載範本失敗，請稍後再試',
            icon: 'error',
            confirmButtonText: '確定'
        });
    }
}

// 處理檔案上傳
function handleFileUpload(fileCode, fileInput) {
    const file = fileInput.files[0];
    if (!file) {
        return;
    }

    // 檢查檔案格式
    if (!file.name.toLowerCase().endsWith('.pdf')) {
        Swal.fire({
            title: '檔案格式錯誤！',
            text: '請上傳 pdf 格式的檔案',
            icon: 'warning',
            confirmButtonText: '確定'
        });
        fileInput.value = '';
        return;
    }

    // 檢查檔案大小 (100MB)
    if (file.size > 100 * 1024 * 1024) {
        Swal.fire({
            title: '檔案過大！',
            text: '檔案大小不能超過 100MB',
            icon: 'warning',
            confirmButtonText: '確定'
        });
        fileInput.value = '';
        return;
    }

    // 取得 ProjectID
    const urlParams = new URLSearchParams(window.location.search);
    const projectID = urlParams.get('ProjectID');

    if (!projectID) {
        Swal.fire({
            title: '錯誤！',
            text: '計畫編號不存在，請確認 URL 參數',
            icon: 'error',
            confirmButtonText: '確定'
        });
        return;
    }

    // 顯示上傳中狀態
    document.getElementById('uploadStatus' + fileCode).textContent = '上傳中...';
    document.getElementById('uploadStatus' + fileCode).className = 'text-warning';

    // 建立 FormData - 修正欄位名稱
    const formData = new FormData();
    formData.append('action', 'upload');
    formData.append('fileType', 'Payment');
    formData.append('fileCode', fileCode);
    formData.append('projectID', projectID);
    formData.append('uploadedFile', file);  // ✅ 修正：改為 'uploadedFile'

    // 發送 AJAX 請求
    fetch('../../../Service/CLB_Upload.ashx', {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            // ✅ 修正：ASHX 直接返回 JSON，不需要 .d 包裝
            if (data && data.success) {
                // 更新 UI
                updateUploadStatus(fileCode, true, data.fileName);

                Swal.fire({
                    icon: 'success',
                    title: '上傳成功',
                    text: data.message,  // ✅ 修正：直接使用 data.message
                    timer: 1500,
                    showConfirmButton: false
                });
            } else {
                // 上傳失敗 - 更新狀態並隱藏下載區塊
                const statusElement = document.getElementById(`uploadStatus${fileCode}`);
                const uploadedFileDiv = document.getElementById(`uploadedFile${fileCode}`);
                
                if (statusElement) {
                    statusElement.textContent = '上傳失敗';
                    statusElement.className = 'text-danger';
                }
                if (uploadedFileDiv) {
                    uploadedFileDiv.style.display = 'none';
                }

                Swal.fire({
                    icon: 'error',
                    title: '上傳失敗',
                    text: data?.message || '檔案上傳失敗'  // ✅ 修正：直接使用 data.message
                });
            }
            
            // ✅ 清空 input 值，允許重複上傳相同檔案
            fileInput.value = '';
        })
        .catch(error => {
            // 網路錯誤 - 更新狀態並隱藏下載區塊
            const statusElement = document.getElementById(`uploadStatus${fileCode}`);
            const uploadedFileDiv = document.getElementById(`uploadedFile${fileCode}`);
            
            if (statusElement) {
                statusElement.textContent = '上傳失敗';
                statusElement.className = 'text-danger';
            }
            if (uploadedFileDiv) {
                uploadedFileDiv.style.display = 'none';
            }

            Swal.fire({
                icon: 'error',
                title: '上傳失敗',
                text: `網路錯誤: ${error}`
            });
            
            // ✅ 清空 input 值，允許重複上傳相同檔案
            fileInput.value = '';
        });
}
// 下載已上傳檔案
function downloadUploadedFile(fileType) {
    try {
        // 檢查是否有檔案可以下載
        const fileName = document.getElementById('fileName' + fileType).textContent;
        if (!fileName || fileName.trim() === '') {
            Swal.fire({
                title: '無檔案可下載！',
                text: '請先上傳檔案',
                icon: 'warning',
                confirmButtonText: '確定'
            });
            return;
        }

        // 取得 ProjectID
        const urlParams = new URLSearchParams(window.location.search);
        const projectID = urlParams.get('ProjectID');

        if (!projectID) {
            Swal.fire({
                title: '錯誤！',
                text: '計畫編號不存在，請確認 URL 參數',
                icon: 'error',
                confirmButtonText: '確定'
            });
            return;
        }

        // 取得對應的檔案代碼
        let fileCode = '';
        switch(fileType) {
            case 1:
                fileCode = 'PaymentIncomeStatement';
                break;    
            case 2:
                fileCode = 'PaymentSubsidyList';
                break;    
            case 3:
                fileCode = 'PaymentCostAllocation';
                break;    
            case 4:
                fileCode = 'PaymentVouchers';
                break;    
            case 5:
                fileCode = 'PaymentReceipts';
                break;
            default:
                Swal.fire({
                    title: '錯誤！',
                    text: '無效的檔案類型',
                    icon: 'error',
                    confirmButtonText: '確定'
                });
                return;
        }

        // 建立下載連結
        const downloadUrl = '../../../Service/CLB_download.ashx' +
            '?action=file&projectID=' + encodeURIComponent(projectID) +
            '&fileCode=' + encodeURIComponent(fileCode);

        // 使用 window.open 開啟下載
        window.open(downloadUrl, '_blank');

    } catch (error) {
        console.error('下載檔案時發生錯誤:', error);
        Swal.fire({
            title: '下載失敗！',
            text: '下載檔案失敗，請稍後再試',
            icon: 'error',
            confirmButtonText: '確定'
        });
    }
}

// 刪除已上傳的檔案
function deleteUploadedFile(fileType) {
    // 取得 ProjectID
    const urlParams = new URLSearchParams(window.location.search);
    const projectID = urlParams.get('ProjectID');
    
    if (!projectID) {
        Swal.fire({
            title: '錯誤！',
            text: '計畫編號不存在，請確認 URL 參數',
            icon: 'error',
            confirmButtonText: '確定'
        });
        return;
    }

    Swal.fire({
        title: '確認刪除',
        text: '確定要刪除這個檔案嗎？',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: '刪除',
        cancelButtonText: '取消'
    }).then((result) => {
        if (result.isConfirmed) {
            // 建立 FormData
            const formData = new FormData();
            formData.append('action', 'delete');
            formData.append('fileType', 'Payment');
            formData.append('fileCode', fileType);
            formData.append('projectID', projectID);

            // 發送 AJAX 請求到 CLB_Upload.ashx
            fetch('../../../Service/CLB_Upload.ashx', {
                method: 'POST',
                body: formData
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    // 更新 UI
                    updateUploadStatus(fileType, false, '');
                    
                    Swal.fire({
                        icon: 'success',
                        title: '刪除成功',
                        text: data.message,
                        timer: 1500,
                        showConfirmButton: false
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: '刪除失敗',
                        text: data?.message || '檔案刪除失敗'
                    });
                }
            })
            .catch(error => {
                Swal.fire({
                    icon: 'error',
                    title: '刪除失敗',
                    text: `網路錯誤: ${error}`
                });
            });
        }
    });
}

// 更新上傳狀態 UI
function updateUploadStatus(fileType, isUploaded, fileName) {
    const statusElement = document.getElementById(`uploadStatus${fileType}`);
    const fileNameElement = document.getElementById(`fileName${fileType}`);
    const uploadedFileDiv = document.getElementById(`uploadedFile${fileType}`);
    
    if (statusElement && fileNameElement && uploadedFileDiv) {
        if (isUploaded) {
            statusElement.textContent = '已上傳';
            statusElement.className = 'text-success';
            fileNameElement.textContent = fileName;
            uploadedFileDiv.style.display = 'flex';
        } else {
            statusElement.textContent = '尚未上傳';
            statusElement.className = 'text-pink';
            fileNameElement.textContent = '';
            uploadedFileDiv.style.display = 'none';
        }
    }
}

// 提送請款（isDraft: true=暫存, false/undefined=提送）
function submitReimbursement(isDraft = false) {
    // 取得 ProjectID
    const urlParams = new URLSearchParams(window.location.search);
    const projectID = urlParams.get('ProjectID');
    
    if (!projectID) {
        Swal.fire({
            title: '錯誤！',
            text: '計畫編號不存在，請確認 URL 參數',
            icon: 'error',
            confirmButtonText: '確定'
        });
        return;
    }

    var currentAmount = parseFloat(document.getElementById('currentAmount').textContent.replace(/,/g, '')) || 0;
    var accumulatedAmount = parseFloat(document.getElementById('accumulatedAmountInput').value) || 0;
    
    // 驗證
    if (accumulatedAmount <= 0) {
        Swal.fire({
            title: '請輸入累積實支金額！',
            icon: 'warning',
            confirmButtonText: '確定'
        });
        return;
    }
    
    if (!isDraft && currentAmount <= 0) {
        Swal.fire({
            title: '本期請款金額不可為0！',
            icon: 'warning',
            confirmButtonText: '確定'
        });
        return;
    }
    
    // CLB 採實支實付制，檢查請款金額不可超過核定經費
    var approvedSubsidy = parseFloat(document.getElementById('approvedSubsidy').textContent.replace(/,/g, '')) || 0;
    
    if (currentAmount > approvedSubsidy) {
        Swal.fire({
            title: '請款金額超過核定經費！',
            text: `請款金額不可超過核定經費 (${formatNumber(approvedSubsidy)} 元)`,
            icon: 'warning',
            confirmButtonText: '確定'
        });
        return;
    }
    
    let actionText = isDraft ? '暫存' : '提送';
    
    Swal.fire({
        title: `確定${actionText}請款？`,
        text: `本期請款: ${formatNumber(currentAmount)} 元`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#6c757d',
        confirmButtonText: actionText,
        cancelButtonText: '取消'
    }).then((result) => {
        if (result.isConfirmed) {
            // 顯示載入中
            Swal.fire({
                title: `${actionText}中...`,
                text: '請稍候',
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });
            
            // 呼叫後端 WebMethod
            $.ajax({
                type: "POST",
                url: "ClbPayment.aspx/SubmitReimbursement",
                data: JSON.stringify({ 
                    projectID: projectID, 
                    isDraft: isDraft,
                    accumulatedAmount: accumulatedAmount,
                    currentAmount: currentAmount
                }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(response) {
                    const data = response.d;
                    if (data.success) {
                        Swal.fire({
                            title: `${actionText}成功！`,
                            text: data.message,
                            icon: 'success',
                            confirmButtonText: '確定'
                        }).then(() => {
                            // 重新載入頁面以更新狀態
                            window.location.reload();
                        });
                    } else {
                        Swal.fire({
                            title: `${actionText}失敗！`,
                            text: data.message,
                            icon: 'error',
                            confirmButtonText: '確定'
                        });
                    }
                },
                error: function(xhr, status, error) {
                    console.error(`${actionText}時發生錯誤:`, error);
                    Swal.fire({
                        title: `${actionText}失敗！`,
                        text: `${actionText}時發生錯誤，請稍後再試`,
                        icon: 'error',
                        confirmButtonText: '確定'
                    });
                }
            });
        }
    });
}

// 提交審核
function submitReview() {
    // 取得 ProjectID
    const urlParams = new URLSearchParams(window.location.search);
    const projectID = urlParams.get('ProjectID');
    
    if (!projectID) {
        Swal.fire({
            title: '錯誤！',
            text: '計畫編號不存在，請確認 URL 參數',
            icon: 'error',
            confirmButtonText: '確定'
        });
        return;
    }

    let currentPayment = parseFloat(document.getElementById('currentPayment').value) || 0;
    let reviewResult = document.getElementById('radio-pass').checked ? 'pass' : 'return';
    let reviewComment = document.getElementById('reviewComment').textContent.trim();
    
    var resultText = reviewResult === 'pass' ? '通過' : '退回修改';
    
    Swal.fire({
        title: '確定提交審核結果？',
        text: `本期撥款: ${formatNumber(currentPayment)} 元\n審查結果: ${resultText}`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#6c757d',
        confirmButtonText: '提交',
        cancelButtonText: '取消'
    }).then((result) => {
        if (result.isConfirmed) {
            // 顯示載入中
            Swal.fire({
                title: '提交中...',
                text: '請稍候',
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });
            
            // 呼叫後端 WebMethod
            $.ajax({
                type: "POST",
                url: "ClbPayment.aspx/SubmitReview",
                data: JSON.stringify({ 
                    projectID: projectID, 
                    reviewResult: reviewResult,
                    reviewComment: reviewComment,
                    currentPayment: currentPayment
                }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(response) {
                    const data = response.d;
                    if (data.success) {
                        Swal.fire({
                            title: '提交成功！',
                            text: data.message,
                            icon: 'success',
                            confirmButtonText: '確定'
                        }).then(() => {
                            // 重新載入頁面以更新狀態
                            window.location.reload();
                        });
                    } else {
                        Swal.fire({
                            title: '提交失敗！',
                            text: data.message,
                            icon: 'error',
                            confirmButtonText: '確定'
                        });
                    }
                },
                error: function(xhr, status, error) {
                    console.error('提交審核結果時發生錯誤:', error);
                    Swal.fire({
                        title: '提交失敗！',
                        text: '提交審核結果時發生錯誤，請稍後再試',
                        icon: 'error',
                        confirmButtonText: '確定'
                    });
                }
            });
        }
    });
}

// 格式化數字顯示
function formatNumber(num) {
    if (num === null || num === undefined || num === 0) {
        return '0';
    }
    return Math.round(num).toLocaleString();
}

// 檢視模式功能（僅在審核中或通過時執行）
function initializeViewMode() {
    var paymentStatus = window.paymentStatus || '';
    
    if (paymentStatus === '審核中' || paymentStatus === '通過') {
        console.log('執行檢視模式，當前狀態：' + paymentStatus);
        
        // 隱藏有 view-mode class 的元件
        const viewModeElements = document.querySelectorAll('.view-mode');
        viewModeElements.forEach(function(element) {
            element.classList.add('d-none');
        });
        
        // 為 body 新增 app-mode-view class
        document.body.classList.add('app-mode-view');
        
        // 為 paymentTable 新增 hide-col-3 class
        const paymentTable = document.getElementById('paymentTable');
        if (paymentTable) {
            paymentTable.classList.add('hide-col-3');
        }
        
    }
}

// 頁面載入時初始化
document.addEventListener('DOMContentLoaded', function() {
    // 計算第二期數值
    calculatePhase2Values();
    
    // 綁定累積實支金額輸入框的變化事件
    const accumulatedAmountInput = document.getElementById('accumulatedAmountInput');
    if (accumulatedAmountInput) {
        accumulatedAmountInput.addEventListener('input', function() {
            calculatePhase2Values();
            updateRemainingAmount();
        });
    }
});

// CLB 第一期動態計算
function calculatePhase2Values() {
    var accumulated = parseFloat(document.getElementById('accumulatedAmountInput').value) || 0;
    var previous = 0; // CLB 僅有一期，前期已撥付固定為 0
    var approved = parseFloat(document.getElementById('approvedSubsidy').textContent.replace(/,/g, '')) || 0;
    
    // 本期請款金額 = 累積實支金額 (因為前期撥付為 0)
    var currentAmount = Math.max(0, accumulated);
    document.getElementById('currentAmount').textContent = formatNumber(currentAmount);
    
    // 執行率 = 累積實支 ÷ 核定經費 × 100%
    var rate = approved > 0 ? (accumulated / approved * 100) : 0;
    document.getElementById('executionRate').textContent = rate.toFixed(2) + '%';
    
    // 支用比計算：僅在審核通過後才計算
    updateUsageRatio();
    
    // 同步更新審查面板的本期撥款
    const currentPaymentInput = document.getElementById('currentPayment');
    if (currentPaymentInput) {
        currentPaymentInput.value = currentAmount;
    }
}

// 更新賸餘款顯示
function updateRemainingAmount() {
    var approvedSubsidy = parseFloat(document.getElementById('approvedSubsidy').textContent.replace(/,/g, '')) || 0;
    var accumulatedAmount = parseFloat(document.getElementById('accumulatedAmountInput').value) || 0;
    var remainingAmount = approvedSubsidy - accumulatedAmount;
    
    document.getElementById('remainingAmount').textContent = formatNumber(remainingAmount);
}

// 更新支用比顯示
function updateUsageRatio() {
    // 取得頁面上的狀態資訊（這個需要從後端傳入）
    var paymentStatus = window.paymentStatus || '';
    var currentActualPaidAmount = window.currentActualPaidAmount || 0;
    
    if (paymentStatus === '通過' && currentActualPaidAmount > 0) {
        var totalSpentAmount = parseFloat(document.getElementById('accumulatedAmountInput').value) || 0;
        
        // 支用比 = 累積實支金額 ÷ 本期實際撥款金額 × 100%
        var usageRate = (totalSpentAmount / currentActualPaidAmount) * 100;
        
        // 若結果 > 100%，顯示為 100%
        if (usageRate > 100) {
            usageRate = 100;
        }
        
        document.getElementById('usageRatio').textContent = usageRate.toFixed(2) + '%';
    } else {
        // 未審核通過時顯示 "--"
        document.getElementById('usageRatio').textContent = '--';
    }
}