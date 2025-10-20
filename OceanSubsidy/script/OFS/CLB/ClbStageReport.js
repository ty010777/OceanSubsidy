// ClbStageReport.js - 成果報告審查頁面 JavaScript

// 下載成果報告書範本
function downloadTemplate() {
    try {
        // 取得 ProjectID 參數
        const urlParams = new URLSearchParams(window.location.search);
        const projectID = urlParams.get('ProjectID') || '';

        // 使用 window.AppRootPath 處理虛擬路徑
        const appRootPath = window.AppRootPath || '';

        // 建立下載連結
        let downloadUrl = appRootPath + '/Service/CLB_download.ashx?action=template&type=report';
        if (projectID) {
            downloadUrl += '&projectID=' + encodeURIComponent(projectID);
        }

        // 使用 window.open 開啟下載
        window.open(downloadUrl, '_blank');

    } catch (error) {
        console.error('下載範本時發生錯誤:', error);
        Swal.fire({
            title: '下載失敗！',
            text: '下載範本失敗,請稍後再試',
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
    if (!file.name.toLowerCase().endsWith('.zip')) {
        Swal.fire({
            title: '檔案格式錯誤！',
            text: '請上傳 ZIP 格式的檔案',
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

    // 建立 FormData
    const formData = new FormData();
    formData.append('action', 'upload');
    formData.append('fileType', 'StageReport');
    formData.append('fileCode', fileCode);
    formData.append('projectID', projectID);
    formData.append('uploadedFile', file);

    // 使用 window.AppRootPath 處理虛擬路徑
    const appRootPath = window.AppRootPath || '';

    // 發送 AJAX 請求
    fetch(appRootPath + '/Service/CLB_Upload.ashx', {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            // 上傳成功
            document.getElementById('uploadStatus' + fileCode).textContent = '已上傳';
            document.getElementById('uploadStatus' + fileCode).className = 'text-success';
            document.getElementById('fileName' + fileCode).textContent = data.fileName;
            document.getElementById('uploadedFile' + fileCode).style.display = 'flex';
            document.getElementById('uploadedFile' + fileCode).classList.remove('d-none');
            
            // 儲存檔案路徑供下載使用
            document.getElementById('uploadedFile' + fileCode).dataset.filePath = data.relativePath;
            
            Swal.fire({
                title: '上傳成功！',
                text: data.message,
                icon: 'success',
                confirmButtonText: '確定'
            });
        } else {
            // 上傳失敗 - 隱藏已上傳檔案區塊
            document.getElementById('uploadStatus' + fileCode).textContent = '上傳失敗';
            document.getElementById('uploadStatus' + fileCode).className = 'text-danger';
            document.getElementById('uploadedFile' + fileCode).style.display = 'none';
            document.getElementById('uploadedFile' + fileCode).classList.add('d-none');
            
            Swal.fire({
                title: '上傳失敗！',
                text: data.message,
                icon: 'error',
                confirmButtonText: '確定'
            });
        }
    })
    .catch(error => {
        console.error('上傳檔案時發生錯誤:', error);
        document.getElementById('uploadStatus' + fileCode).textContent = '上傳失敗';
        document.getElementById('uploadStatus' + fileCode).className = 'text-danger';
        // 隱藏已上傳檔案區塊
        document.getElementById('uploadedFile' + fileCode).style.display = 'none';
        document.getElementById('uploadedFile' + fileCode).classList.add('d-none');
        
        Swal.fire({
            title: '上傳失敗！',
            text: '上傳檔案時發生錯誤，請稍後再試',
            icon: 'error',
            confirmButtonText: '確定'
        });
    })
    .finally(() => {
        // 清空 input
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
            case '1':
                fileCode = 'StageReport1';
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

        // 使用 window.AppRootPath 處理虛擬路徑
        const appRootPath = window.AppRootPath || '';

        // 建立下載連結
        const downloadUrl = appRootPath + '/Service/CLB_download.ashx' +
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

// 刪除已上傳檔案
function deleteUploadedFile(fileType) {
    Swal.fire({
        title: '確定要刪除此檔案嗎？',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: '刪除',
        cancelButtonText: '取消'
    }).then((result) => {
        if (!result.isConfirmed) {
            return;
        }

        try {
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

            // 將 fileType (1) 轉換為正確的 fileCode (StageReport1)
            let fileCode = '';
            switch(fileType) {
                case 1:
                case '1':
                    fileCode = 'StageReport1';
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

            // 建立 FormData
            const formData = new FormData();
            formData.append('action', 'delete');
            formData.append('fileType', 'StageReport');
            formData.append('fileCode', fileCode);
            formData.append('projectID', projectID);

            // 使用 window.AppRootPath 處理虛擬路徑
            const appRootPath = window.AppRootPath || '';

            // 發送 AJAX 請求
            fetch(appRootPath + '/Service/CLB_Upload.ashx', {
                method: 'POST',
                body: formData
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    // 刪除成功，重置 UI
                    document.getElementById('uploadStatus' + fileType).textContent = '尚未上傳';
                    document.getElementById('uploadStatus' + fileType).className = 'text-pink';
                    document.getElementById('fileName' + fileType).textContent = '';
                    document.getElementById('uploadedFile' + fileType).style.display = 'none';
                    document.getElementById('uploadedFile' + fileType).classList.add('d-none');
                    
                    Swal.fire({
                        title: '刪除成功！',
                        text: data.message,
                        icon: 'success',
                        confirmButtonText: '確定'
                    });
                } else {
                    Swal.fire({
                        title: '刪除失敗！',
                        text: data.message,
                        icon: 'error',
                        confirmButtonText: '確定'
                    });
                }
            })
            .catch(error => {
                console.error('刪除檔案時發生錯誤:', error);
                Swal.fire({
                    title: '刪除失敗！',
                    text: '刪除檔案時發生錯誤，請稍後再試',
                    icon: 'error',
                    confirmButtonText: '確定'
                });
            });
            
        } catch (error) {
            console.error('刪除檔案時發生錯誤:', error);
            Swal.fire({
                title: '刪除失敗！',
                text: '刪除檔案失敗，請稍後再試',
                icon: 'error',
                confirmButtonText: '確定'
            });
        }
    });
}

// 提送報告
function submitReport(isDraft = false) {
    let actionText = isDraft ? '暫存' : '提送';
    
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
        title: `確定${actionText}成果報告？`,
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
                url: "ClbStageReport.aspx/SubmitStageReport",
                data: JSON.stringify({ 
                    projectID: projectID, 
                    isDraft: isDraft 
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
                            // 重新載入頁面以更新狀態和權限條件
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

// 切換審查委員名單顯示
function toggleReviewerList() {
    var needMemberCheckbox = document.getElementById('needmember');
    var reviewerListSection = document.getElementById('reviewerListSection');
    var reviewResultSection = document.getElementById('reviewResultSection');
    var submitReviewButton = document.getElementById('submitReviewButton');
        
    if (needMemberCheckbox.checked) {
        // 顯示審查委員名單
        reviewerListSection.style.display = 'block';
        // 隱藏審查結果
        reviewResultSection.style.display = 'none';
        // 隱藏確定按鈕
        if (submitReviewButton) {
            submitReviewButton.style.display = 'none';
        }
        // 預設選擇批次輸入
        document.getElementById('radio-batch').checked = true;
        toggleInputMode();
    } else {
        // 隱藏審查委員名單
        reviewerListSection.style.display = 'none';
        // 顯示審查結果
        reviewResultSection.style.display = 'flex';
        // 顯示確定按鈕
        if (submitReviewButton) {
            submitReviewButton.style.display = 'block';
        }
    }
}

// 切換輸入模式（逐筆輸入 vs 批次輸入）
function toggleInputMode() {
    var singleInput = document.getElementById('radio-single');
    var batchInput = document.getElementById('radio-batch');
    var singleSection = document.getElementById('singleInputSection');
    var batchSection = document.getElementById('batchInputSection');
    
    if (singleInput.checked) {
        // 顯示逐筆輸入，隱藏批次輸入
        singleSection.style.display = 'block';
        batchSection.style.display = 'none';
        // 添加預設的一行
        addReviewerRow();
    } else if (batchInput.checked) {
        // 顯示批次輸入，隱藏逐筆輸入
        singleSection.style.display = 'none';
        batchSection.style.display = 'block';
        // 清空逐筆輸入的所有行
        clearAllReviewerRows();
    }
}

// 添加審查委員行
function addReviewerRow() {
    var tbody = document.getElementById('reviewerTableBody');
    var rowCount = tbody.rows.length + 1;
    
    var newRow = document.createElement('tr');
    newRow.innerHTML = `
        <td>${rowCount}</td>
        <td><input type="text" class="form-control" placeholder="姓名"></td>
        <td><input type="text" class="form-control" placeholder="Email"></td>
        <td>    
            <div class="d-flex gap-2">
                <button type="button" class="tag-btn" onclick="removeReviewerRow(this)">
                    <i class="fa-solid fa-circle-xmark"></i>
                </button>
                <button type="button" class="btn btn-sm btn-teal" onclick="addReviewerRow()">
                    <i class="fas fa-plus"></i>
                </button>
            </div>
        </td>
    `;
    
    tbody.appendChild(newRow);
    updateRowNumbers();
}

// 刪除審查委員行
function removeReviewerRow(button) {
    var tbody = document.getElementById('reviewerTableBody');
    // 至少保留一行
    if (tbody.rows.length > 1) {
        var row = button.closest('tr');
        row.remove();
        updateRowNumbers();
    }
}

// 更新行號
function updateRowNumbers() {
    var tbody = document.getElementById('reviewerTableBody');
    var rows = tbody.getElementsByTagName('tr');
    
    for (var i = 0; i < rows.length; i++) {
        rows[i].cells[0].textContent = i + 1;
    }
}

// 清空所有審查委員行
function clearAllReviewerRows() {
    var tbody = document.getElementById('reviewerTableBody');
    tbody.innerHTML = '';
}

// 提送審查委員 (靜態功能)
function submitReviewers() {
    Swal.fire({
        title: '提送成功！',
        text: '提送審查委員功能 (靜態展示)',
        icon: 'success',
        confirmButtonText: '確定'
    });
}

// 提交審核
function submitReview() {
    // 取得審查結果
    var reviewResult = document.querySelector('input[name="reviewResult"]:checked')?.value;
    if (!reviewResult) {
        Swal.fire({
            title: '請選擇審查結果！',
            icon: 'warning',
            confirmButtonText: '確定'
        });
        return;
    }
    
    // 取得審查意見
    var reviewComment = document.getElementById('reviewComment')?.textContent || '';
    
    var resultText = reviewResult === 'pass' ? '通過' : '不通過';
    
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
        title: '確定提交審核結果？',
        text: `審查結果：${resultText}`,
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
                url: "ClbStageReport.aspx/SubmitReview",
                data: JSON.stringify({ 
                    projectID: projectID, 
                    reviewResult: reviewResult,
                    reviewComment: reviewComment
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

// 初始化頁面狀態（由後端呼叫）
window.initializePageState = function(stateInfo) {
    try {
        console.log('初始化頁面狀態:', stateInfo);
        
        // 標記狀態已初始化
        window.pageStateInitialized = true;
        
        // 根據狀態控制按鈕顯示
        controlButtonVisibility(stateInfo);
        
        // 根據狀態控制審查區塊顯示
        controlReviewPanelVisibility(stateInfo);
        
    } catch (error) {
        console.error('初始化頁面狀態時發生錯誤:', error);
    }
};

// 控制按鈕顯示/隱藏
function controlButtonVisibility(stateInfo) {
    // 範本下載按鈕
    const downloadTemplateBtn = document.querySelector('button[onclick="downloadTemplate()"]');
    
    // 上傳相關元素 - 更精確的選擇器
    const uploadFileInputs = document.querySelectorAll('input[type="file"]');
    const uploadButtons = document.querySelectorAll('button[onclick*="handleFileUpload"], button[onclick*="fileInput"]');
    
    // 刪除按鈕
    const deleteButtons = document.querySelectorAll('button[onclick*="deleteUploadedFile"]');
    
    // 暫存和提送按鈕
    const draftBtn = document.querySelector('button[onclick="submitReport(true)"]');
    const submitBtn = document.querySelector('button[onclick="submitReport(false)"]');
    
    // StageTable 元素
    const stageTable = document.getElementById('StageTable');
    
    // 檔案輸入元素永遠保持隱藏狀態
    uploadFileInputs.forEach(el => el.style.display = 'none');
    
    if (stateInfo.canEdit) {
        // 可以編輯：顯示範本下載、上傳、暫存、提送按鈕，以及刪除按鈕
        if (downloadTemplateBtn) downloadTemplateBtn.style.display = '';
        uploadButtons.forEach(el => el.style.display = '');
        deleteButtons.forEach(el => el.style.display = '');
        if (draftBtn) draftBtn.style.display = '';
        if (submitBtn) submitBtn.style.display = '';
        // 可以編輯時移除 hide-col-3 class
        if (stageTable) stageTable.classList.remove('hide-col-3');
    } else {
        // 不能編輯的情況
        if (stateInfo.status === '通過') {
            // 通過狀態：隱藏所有按鈕，添加 hide-col-3 class
            if (downloadTemplateBtn) downloadTemplateBtn.style.display = 'none';
            uploadButtons.forEach(el => el.style.display = 'none');
            deleteButtons.forEach(el => el.style.display = 'none');
            if (draftBtn) draftBtn.style.display = 'none';
            if (submitBtn) submitBtn.style.display = 'none';
            if (stageTable) stageTable.classList.add('hide-col-3');
        } else if (stateInfo.status === '審核中') {
            // 審核中狀態：隱藏範本下載、暫存和提送按鈕，以及上傳和刪除功能，添加 hide-col-3 class
            if (downloadTemplateBtn) downloadTemplateBtn.style.display = 'none';
            if (draftBtn) draftBtn.style.display = 'none';
            if (submitBtn) submitBtn.style.display = 'none';
            // 上傳和刪除功能在審核中狀態也應該隱藏
            uploadButtons.forEach(el => el.style.display = 'none');
            deleteButtons.forEach(el => el.style.display = 'none');
            if (stageTable) stageTable.classList.add('hide-col-3');
        }
    }
}

// 控制審查區塊顯示/隱藏
function controlReviewPanelVisibility(stateInfo) {
    const reviewPanel = document.getElementById('reviewPanel');
    
    if (reviewPanel) {
        if (stateInfo.showReviewPanel && stateInfo.canReview) {
            // 顯示審查區塊
            reviewPanel.style.display = 'block';
            // 預設隱藏審查委員名單，顯示審查結果
            if (document.getElementById('reviewerListSection')) {
                document.getElementById('reviewerListSection').style.display = 'none';
            }
            if (document.getElementById('reviewResultSection')) {
                document.getElementById('reviewResultSection').style.display = 'flex';
            }
        } else {
            // 隱藏審查區塊
            reviewPanel.style.display = 'none';
        }
    }
}

// 初始化頁面
document.addEventListener('DOMContentLoaded', function() {
    // 如果沒有從後端呼叫 initializePageState，則使用預設設定
    setTimeout(function() {
        if (!window.pageStateInitialized) {
            // 預設隱藏審查委員名單，顯示審查結果
            if (document.getElementById('reviewerListSection')) {
                document.getElementById('reviewerListSection').style.display = 'none';
            }
            if (document.getElementById('reviewResultSection')) {
                document.getElementById('reviewResultSection').style.display = 'flex';
            }
        }
    }, 100);
});