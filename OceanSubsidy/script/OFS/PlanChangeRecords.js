$(document).ready(function() {
    // 頁面初始化
    initializePage();
});

/**
 * 頁面初始化
 */
function initializePage() {
    // 初始化表格功能
    initializeTable();

    // 綁定匯出按鈕事件
    bindExportButton();
}

/**
 * 初始化表格功能
 */
function initializeTable() {
    // 可以在這裡添加表格相關的功能
    // 例如：排序、篩選等

    // 檢查是否有資料
    const tableBody = $('#ChangeRecordsTable tbody');
    const rowCount = tableBody.find('tr').length;

    if (rowCount === 0) {
        console.log('目前無變更紀錄');
    }
}

/**
 * 綁定匯出按鈕事件
 */
function bindExportButton() {
    // 匯出按鈕點擊事件
    $('button[id*="btnExportRecords"], input[type="submit"][id*="btnExportRecords"]').on('click', function(e) {
        const recordCount = $('#ChangeRecordsTable tbody tr').length;

        if (recordCount === 0) {
            e.preventDefault();
            showMessage('目前無變更紀錄可匯出', 'warning');
            return false;
        }
    });
}

/**
 * 顯示訊息
 * @param {string} message - 訊息內容
 * @param {string} type - 訊息類型 (success, error, warning, info)
 */
function showMessage(message, type = 'info') {
    let icon = 'info';
    let title = '提示';

    switch(type) {
        case 'success':
            icon = 'success';
            title = '成功';
            break;
        case 'error':
            icon = 'error';
            title = '錯誤';
            break;
        case 'warning':
            icon = 'warning';
            title = '警告';
            break;
        default:
            icon = 'info';
            title = '提示';
    }

    if (typeof Swal !== 'undefined') {
        Swal.fire({
            icon: icon,
            title: title,
            text: message,
            confirmButtonText: '確定'
        });
    } else {
        alert(message);
    }
}

/**
 * 確認對話框
 * @param {string} message - 訊息內容
 * @param {function} callback - 確認後的回調函數
 */
function confirmAction(message, callback) {
    if (typeof Swal !== 'undefined') {
        Swal.fire({
            title: '確認',
            text: message,
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: '確定',
            cancelButtonText: '取消'
        }).then((result) => {
            if (result.isConfirmed && typeof callback === 'function') {
                callback();
            }
        });
    } else {
        if (confirm(message) && typeof callback === 'function') {
            callback();
        }
    }
}

/**
 * 格式化日期時間
 * @param {string} dateString - 日期字串
 * @returns {string} 格式化後的日期字串
 */
function formatDateTime(dateString) {
    if (!dateString) return '-';

    try {
        const date = new Date(dateString);
        if (isNaN(date.getTime())) return dateString;

        const year = date.getFullYear() - 1911; // 轉換為民國年
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        const hours = String(date.getHours()).padStart(2, '0');
        const minutes = String(date.getMinutes()).padStart(2, '0');

        return `${year}/${month}/${day} ${hours}:${minutes}`;
    } catch (e) {
        console.error('日期格式轉換錯誤:', e);
        return dateString;
    }
}
