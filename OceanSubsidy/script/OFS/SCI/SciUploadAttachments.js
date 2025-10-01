/**
 * 海洋科技專案計畫申請 - 上傳附件頁面 JavaScript
 * SciUploadAttachments.js
 */

// 頁面載入完成後初始化
document.addEventListener('DOMContentLoaded', function() {
    initializePage();
});

/**
 * 初始化頁面
 */
function initializePage() {
    // 綁定提送申請按鈕事件
    bindSubmitButton();
}

/**
 * 綁定提送申請按鈕事件
 */
function bindSubmitButton() {
    // 找到提送申請按鈕
    const submitBtn = document.querySelector('[id$="btnSubmit"]');
    if (submitBtn) {
        submitBtn.addEventListener('click', function(event) {
            event.preventDefault();
            showSubmitConfirmation();
            return false;
        });
    }
    
    // 暫存按鈕直接使用 WebForm 處理，不需要 JavaScript 攔截
}

/**
 * 顯示提送申請確認對話框
 */
function showSubmitConfirmation() {
    // 檢查頁面是否處於檢視模式
    if (isViewMode()) {
        showErrorMessage('目前為檢視模式，無法執行提送申請操作');
        return;
    }

    Swal.fire({
        title: '提送審核確認',
        text: '請問要提送審核嗎？送出後即不可再編輯修改。',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: '確認提送',
        cancelButtonText: '取消',
        reverseButtons: true,
        allowOutsideClick: false,
        allowEscapeKey: false
    }).then((result) => {
        if (result.isConfirmed) {
            // 用戶確認後，觸發隱藏的確認按鈕
            triggerConfirmedSubmit();
        }
    });
}

/**
 * 觸發確認提送的後端處理
 */
function triggerConfirmedSubmit() {
    // 找到隱藏的確認按鈕並觸發點擊
    const confirmedBtn = document.querySelector('[id$="btnSubmitConfirmed"]');
    if (confirmedBtn) {
        confirmedBtn.click();
    } else {
        console.error('找不到確認提送按鈕');
        showErrorMessage('系統錯誤：找不到確認提送按鈕');
    }
}


/**
 * 顯示錯誤訊息
 * @param {string} message - 錯誤訊息
 */
function showErrorMessage(message) {
    Swal.fire({
        title: '錯誤',
        text: message,
        icon: 'error',
        confirmButtonColor: '#d33',
        confirmButtonText: '確認'
    });
}

/**
 * 顯示警告訊息
 * @param {string} message - 警告訊息
 */
function showWarningMessage(message) {
    Swal.fire({
        title: '警告',
        text: message,
        icon: 'warning',
        confirmButtonColor: '#f39c12',
        confirmButtonText: '確認'
    });
}

/**
 * 顯示資訊訊息
 * @param {string} message - 資訊訊息
 */
function showInfoMessage(message) {
    Swal.fire({
        title: '提示',
        text: message,
        icon: 'info',
        confirmButtonColor: '#3085d6',
        confirmButtonText: '確認'
    });
}

/**
 * 取得申請列表頁面 URL
 * @returns {string} 申請列表頁面 URL
 */
function getApplicationChecklistUrl() {
    // 取得當前頁面的基礎路徑
    const currentPath = window.location.pathname;
    const basePath = currentPath.substring(0, currentPath.lastIndexOf('/'));
    
    // 回到上一層的 ApplicationChecklist.aspx
    return basePath.replace('/SCI', '') + '/ApplicationChecklist.aspx';
}

/**
 * 檢查頁面是否處於檢視模式
 * @returns {boolean} true: 檢視模式, false: 編輯模式
 */
function isViewMode() {
    // 檢查頁面是否有 app-mode-view 類別
    return document.body.classList.contains('app-mode-view');
}

/**
 * 處理檔案上傳 - 使用 AJAX，避免 postback
 * @param {string} attachmentNumber - 附件編號
 * @param {HTMLInputElement} fileInput - 檔案輸入元素
 */
function handleFileUpload(attachmentNumber, fileInput) {
    const file = fileInput.files[0];
    if (!file) {
        return;
    }

    // 檢查是否處於檢視模式
    if (isViewMode()) {
        showWarningMessage('目前為檢視模式，無法執行檔案上傳操作');
        fileInput.value = '';
        return;
    }

    // 檢查檔案格式
    const fileExt = '.' + file.name.split('.').pop().toLowerCase();
    if (fileExt !== '.pdf') {
        showWarningMessage('僅支援PDF格式檔案上傳');
        fileInput.value = '';
        return;
    }

    // 檢查檔案大小 (10MB)
    const maxSize = 10 * 1024 * 1024;
    if (file.size > maxSize) {
        showWarningMessage('檔案大小不能超過10MB');
        fileInput.value = '';
        return;
    }

    // 顯示上傳中的訊息
    Swal.fire({
        title: '檔案上傳中',
        text: '請稍候...',
        allowOutsideClick: false,
        showConfirmButton: false,
        willOpen: () => {
            Swal.showLoading();
        }
    });

    // 建立 FormData 物件
    const formData = new FormData();
    formData.append('uploadedFile', file);
    formData.append('fileCode', attachmentNumber);
    formData.append('action', 'upload');
    formData.append('fileType', 'SciApplication');

    // 取得 ProjectID
    const projectID = getProjectId();
    if (projectID) {
        formData.append('projectID', projectID);
    }

    // 發送 AJAX 請求到 SCI_Upload.ashx
    fetch('/Service/SCI_Upload.ashx', {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data && data.success) {
            // 直接更新 UI，不重新載入頁面
            updateFileStatusUI(attachmentNumber, data.fileName, data.relativePath);

            Swal.fire({
                icon: 'success',
                title: '上傳成功',
                text: '檔案已成功上傳！',
                timer: 1500,
                showConfirmButton: false
            });

            // 清空檔案輸入
            fileInput.value = '';
        } else {
            showErrorMessage(data?.message || '檔案上傳失敗');
            fileInput.value = '';
        }
    })
    .catch(error => {
        console.error('Upload error:', error);
        showErrorMessage('檔案上傳失敗，請稍後再試。');
        fileInput.value = '';
    });
}

/**
 * 刪除檔案 - 使用 AJAX，避免頁面重新載入
 * @param {string} projectId - 專案ID
 * @param {string} fileCode - 檔案代碼
 * @param {HTMLElement} btnElement - 按鈕元素
 */
function deleteFile(projectId, fileCode, btnElement) {
    // 檢查是否處於檢視模式
    if (isViewMode()) {
        showWarningMessage('目前為檢視模式，無法執行檔案刪除操作');
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
            // 顯示刪除中的訊息
            Swal.fire({
                title: '正在刪除檔案',
                text: '請稍候...',
                allowOutsideClick: false,
                showConfirmButton: false,
                willOpen: () => {
                    Swal.showLoading();
                }
            });

            // 建立 FormData
            const formData = new FormData();
            formData.append('action', 'delete');
            formData.append('fileType', 'SciApplication');
            formData.append('fileCode', fileCode);
            formData.append('projectID', projectId);

            // 發送 AJAX 請求到 SCI_Upload.ashx
            fetch('/Service/SCI_Upload.ashx', {
                method: 'POST',
                body: formData
            })
            .then(response => response.json())
            .then(data => {
                if (data && data.success) {
                    // 直接更新 UI，不重新載入頁面
                    resetFileStatusUI(fileCode);

                    Swal.fire({
                        icon: 'success',
                        title: '刪除成功',
                        text: '檔案已成功刪除！',
                        timer: 1500,
                        showConfirmButton: false
                    });
                } else {
                    showErrorMessage(data?.message || '檔案刪除失敗');
                }
            })
            .catch(error => {
                console.error('Delete error:', error);
                showErrorMessage('檔案刪除失敗，請稍後再試。');
            });
        }
    });
}

/**
 * 下載範本檔案
 * @param {string} fileCode - 檔案代碼
 */
function downloadTemplate(fileCode) {
    console.log('downloadTemplate called with fileCode:', fileCode);
    const projectID = getProjectId();
    const url = '/Service/SCI_Download.ashx?action=downloadTemplate&fileCode=' + encodeURIComponent(fileCode) + '&ProjectID=' + encodeURIComponent(projectID);
    console.log('Download URL:', url);
    window.location.href = url;
}

/**
 * 下載已上傳的檔案
 * @param {string} projectId - 專案ID
 * @param {string} fileCode - 檔案代碼
 * @param {string} fileName - 檔案名稱
 */
function downloadFile(projectId, fileCode, fileName) {
    console.log('downloadFile called with projectId:', projectId, 'fileCode:', fileCode, 'fileName:', fileName);
    const url = '/Service/SCI_Download.ashx?action=downloadFile&projectId=' + encodeURIComponent(projectId) +
                '&fileCode=' + encodeURIComponent(fileCode) +
                '&fileName=' + encodeURIComponent(fileName);
    window.location.href = url;
}

/**
 * 取得專案 ID
 * @returns {string} 專案ID
 */
function getProjectId() {
    // 嘗試從隱藏欄位取得 ProjectID
    const hiddenField = document.querySelector('[id*="hidProjectID"]');
    if (hiddenField && hiddenField.value) {
        return hiddenField.value;
    }

    // 嘗試從 URL 參數取得 ProjectID
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('ProjectID') || '';
}

/**
 * 更新檔案狀態 UI
 * @param {string} fileCode - 檔案代碼
 * @param {string} fileName - 檔案名稱
 * @param {string} relativePath - 相對路徑
 */
function updateFileStatusUI(fileCode, fileName, relativePath) {
    // 依據檔案編號找出對應的狀態標籤
    let statusLabelSelector = '';
    let filesPanelSelector = '';

    if (fileCode.startsWith('FILE_AC')) {
        const num = fileCode.replace('FILE_AC', '');
        statusLabelSelector = `[id*="lblStatusAcademic${num}"]`;
        filesPanelSelector = `[id*="pnlFilesAcademic${num}"]`;
    } else if (fileCode.startsWith('FILE_OTech')) {
        const num = fileCode.replace('FILE_OTech', '');
        statusLabelSelector = `[id*="lblStatus_OTech${num}"], [id*="lblStatusOTech${num}"], [id*="lblStatus${num}_OT"]`;
        filesPanelSelector = `[id*="pnlFiles_OTech${num}"]`;
    } else if (fileCode === 'FILE_AC9') {
        statusLabelSelector = `[id*="lblStatus9"]`;
        filesPanelSelector = `[id*="pnlFilesAcademic9"]`;
    }

    const statusLabel = document.querySelector(statusLabelSelector);
    let filesPanel = document.querySelector(filesPanelSelector);

    if (statusLabel) {
        // 更新狀態標籤
        statusLabel.textContent = '已上傳';
        statusLabel.classList.remove('text-muted');
        statusLabel.classList.add('text-success');

        // 如果檔案面板不存在，創建一個
        if (!filesPanel) {
            const parentTd = statusLabel.closest('td').nextElementSibling;
            if (parentTd) {
                let existingPanel = parentTd.querySelector('.tag-group');
                if (!existingPanel) {
                    existingPanel = document.createElement('div');
                    existingPanel.className = 'tag-group mt-2 gap-1';
                    existingPanel.style.display = 'block';
                    parentTd.appendChild(existingPanel);
                }
                filesPanel = existingPanel;
            }
        }

        if (filesPanel) {
            // 清空並重建檔案面板內容
            filesPanel.innerHTML = '';
            filesPanel.style.display = 'block';

            // 建立檔案標籤 HTML
            const fileTagHtml = `
                <span class="tag tag-green-light">
                    <a class="tag-link" href="#" onclick="downloadFile('${getProjectId()}', '${fileCode}', '${fileName}'); return false;">
                        ${fileName}
                    </a>
                    <button type="button" class="tag-btn" onclick="deleteFile('${getProjectId()}', '${fileCode}')">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </span>
            `;

            filesPanel.innerHTML = fileTagHtml;
        }
    }
}

/**
 * 重置檔案狀態 UI
 * @param {string} fileCode - 檔案代碼
 */
function resetFileStatusUI(fileCode) {
    // 依據檔案編號找出對應的狀態標籤
    let statusLabelSelector = '';
    let filesPanelSelector = '';

    if (fileCode.startsWith('FILE_AC')) {
        const num = fileCode.replace('FILE_AC', '');
        statusLabelSelector = `[id*="lblStatusAcademic${num}"]`;
        filesPanelSelector = `[id*="pnlFilesAcademic${num}"]`;
    } else if (fileCode.startsWith('FILE_OTech')) {
        const num = fileCode.replace('FILE_OTech', '');
        statusLabelSelector = `[id*="lblStatus_OTech${num}"], [id*="lblStatusOTech${num}"], [id*="lblStatus${num}_OT"]`;
        filesPanelSelector = `[id*="pnlFiles_OTech${num}"]`;
    } else if (fileCode === 'FILE_AC9') {
        statusLabelSelector = `[id*="lblStatus9"]`;
        filesPanelSelector = `[id*="pnlFilesAcademic9"]`;
    }

    const statusLabel = document.querySelector(statusLabelSelector);
    const filesPanel = document.querySelector(filesPanelSelector);

    if (statusLabel) {
        // 重置狀態標籤
        statusLabel.textContent = '未上傳';
        statusLabel.classList.remove('text-success');
        statusLabel.classList.add('text-muted');

        // 隱藏並清空檔案面板
        if (filesPanel) {
            filesPanel.style.display = 'none';
            filesPanel.innerHTML = '';
        }

        // 同時查找並移除可能存在的其他檔案面板
        const parentTd = statusLabel.closest('td').nextElementSibling;
        if (parentTd) {
            const tagGroups = parentTd.querySelectorAll('.tag-group');
            tagGroups.forEach(group => {
                group.style.display = 'none';
                group.innerHTML = '';
            });
        }
    }
}

/**
 * 處理檔案選擇事件（保留舊函數以兼容）
 * @param {HTMLElement} fileInput - 檔案輸入元素
 */
function handleFileSelect(fileInput) {
    // 檢查是否處於檢視模式
    if (isViewMode()) {
        showWarningMessage('目前為檢視模式，無法執行檔案選擇操作');
        fileInput.value = '';
        return;
    }

    const file = fileInput.files[0];
    if (file) {
        // 檢查檔案格式
        if (!file.name.toLowerCase().endsWith('.pdf')) {
            showWarningMessage('僅支援PDF格式檔案上傳。');
            fileInput.value = '';
            return;
        }

        // 檢查檔案大小 (10MB)
        const maxSize = 10 * 1024 * 1024; // 10MB in bytes
        if (file.size > maxSize) {
            showWarningMessage('檔案大小不能超過10MB。');
            fileInput.value = '';
            return;
        }

        // 顯示選中的檔案名稱
        console.log('選中檔案:', file.name, '大小:', (file.size / 1024 / 1024).toFixed(2) + 'MB');
    }
}

// 公開的函數供 C# 後端調用
window.SciUploadAttachments = {
    showErrorMessage: showErrorMessage,
    showWarningMessage: showWarningMessage,
    showInfoMessage: showInfoMessage,
    handleFileSelect: handleFileSelect,
    handleFileUpload: handleFileUpload,
    deleteFile: deleteFile,
    downloadTemplate: downloadTemplate,
    downloadFile: downloadFile,
    getProjectId: getProjectId,
    updateFileStatusUI: updateFileStatusUI,
    resetFileStatusUI: resetFileStatusUI
};