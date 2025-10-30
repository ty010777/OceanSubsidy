// SciInprogress Master Page 共用 JavaScript

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
