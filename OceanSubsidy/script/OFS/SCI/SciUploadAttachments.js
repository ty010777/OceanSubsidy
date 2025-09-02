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
 * 處理檔案選擇事件
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
    handleFileSelect: handleFileSelect
};