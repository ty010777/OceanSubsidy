/**
 * SciInprogress_Contract.js
 * 科專計畫契約資料頁面 JavaScript
 */

// 頁面載入時執行
document.addEventListener('DOMContentLoaded', function() {
    loadUploadedFiles();
});

/**
 * 取得當前 ProjectID
 */
function getProjectID() {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('ProjectID');
}

/**
 * 取得當前 OrgCategory
 */
function getOrgCategory() {
    return document.body.getAttribute('data-org-category') || '';
}

/**
 * 根據範本類型取得實際的 fileCode
 */
function getFileCode(templateType) {
    const orgCategory = getOrgCategory();
    const isAcademic = orgCategory === 'Academic' || orgCategory === 'Legal';

    switch (templateType) {
        case 'CONTRACT_CONFIDENTIALITY':
            return isAcademic ? 'CONTRACT_AC_CONFIDENTIALITY' : '';
        case 'CONTRACT_PRIVACY':
            return isAcademic ? 'CONTRACT_AC_PRIVACY' : 'CONTRACT_OTECH_PRIVACY';
        default:
            return '';
    }
}

/**
 * 下載範本檔案
 * @param {string} templateType - 範本類型 (CONTRACT_CONFIDENTIALITY, CONTRACT_PRIVACY)
 */
function downloadTemplate(templateType) {
    const projectID = getProjectID();

    if (!projectID) {
        Swal.fire({
            icon: 'error',
            title: '錯誤',
            text: '找不到計畫編號'
        });
        return;
    }

    const fileCode = getFileCode(templateType);

    if (!fileCode) {
        Swal.fire({
            icon: 'error',
            title: '錯誤',
            text: '無法取得範本檔案'
        });
        return;
    }

    // 呼叫下載 API
    const downloadUrl = `/Service/SCI_Download.ashx?action=downloadTemplate&fileCode=${fileCode}&ProjectID=${projectID}`;
    window.open(downloadUrl, '_blank');
}

/**
 * 處理檔案選擇
 * @param {string} templateType - 範本類型
 * @param {HTMLInputElement} input - 檔案輸入元素
 */
function handleFileSelect(templateType, input) {
    const file = input.files[0];
    if (!file) return;

    // 驗證檔案類型
    if (file.type !== 'application/pdf') {
        Swal.fire({
            icon: 'warning',
            title: '檔案格式錯誤',
            text: '僅支援 PDF 格式檔案'
        });
        input.value = '';
        return;
    }

    // 驗證檔案大小（10MB）
    const maxSize = 10 * 1024 * 1024;
    if (file.size > maxSize) {
        Swal.fire({
            icon: 'warning',
            title: '檔案過大',
            text: '檔案大小不可超過 10MB'
        });
        input.value = '';
        return;
    }

    // 上傳檔案
    uploadFile(templateType, file, input);
}

/**
 * 上傳檔案
 * @param {string} templateType - 範本類型
 * @param {File} file - 檔案物件
 * @param {HTMLInputElement} input - 檔案輸入元素
 */
function uploadFile(templateType, file, input) {
    const projectID = getProjectID();
    const fileCode = getFileCode(templateType);

    if (!projectID || !fileCode) {
        Swal.fire({
            icon: 'error',
            title: '錯誤',
            text: '參數錯誤'
        });
        return;
    }

    const formData = new FormData();
    formData.append('action', 'upload');
    formData.append('projectID', projectID);
    formData.append('fileCode', fileCode);
    formData.append('file', file);

    // 顯示上傳中狀態
    const uploadButton = input.parentElement.querySelector('button');
    const originalText = uploadButton.innerHTML;
    uploadButton.disabled = true;
    uploadButton.innerHTML = '<i class="fas fa-spinner fa-spin me-1"></i>上傳中...';

    fetch('/Service/SCI_Upload.ashx', {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            Swal.fire({
                icon: 'success',
                title: '上傳成功',
                text: '檔案已成功上傳',
                timer: 2000,
                showConfirmButton: false
            });
            // 重新載入已上傳檔案列表
            loadUploadedFiles();
        } else {
            Swal.fire({
                icon: 'error',
                title: '上傳失敗',
                text: data.message
            });
        }
    })
    .catch(error => {
        Swal.fire({
            icon: 'error',
            title: '上傳失敗',
            text: error.message
        });
    })
    .finally(() => {
        // 恢復按鈕狀態
        uploadButton.disabled = false;
        uploadButton.innerHTML = originalText;
        input.value = '';
    });
}

/**
 * 載入已上傳的檔案列表
 */
function loadUploadedFiles() {
    const projectID = getProjectID();
    if (!projectID) return;

    const orgCategory = getOrgCategory();
    const isAcademic = orgCategory === 'Academic' || orgCategory === 'Legal';

    // 載入保密切結書（僅學研）
    if (isAcademic) {
        loadFilesByCode('CONTRACT_AC_CONFIDENTIALITY', 'filesConfidentiality', 'statusConfidentiality');
    }

    // 載入個資同意書
    const privacyCode = isAcademic ? 'CONTRACT_AC_PRIVACY' : 'CONTRACT_OTECH_PRIVACY';
    loadFilesByCode(privacyCode, 'filesPrivacy', 'statusPrivacy');
}

/**
 * 根據 fileCode 載入檔案
 * @param {string} fileCode - 檔案代碼
 * @param {string} containerId - 容器 ID
 * @param {string} statusId - 狀態 ID
 */
function loadFilesByCode(fileCode, containerId, statusId) {
    const projectID = getProjectID();

    fetch(`/Service/SCI_Download.ashx?action=getContractFiles&projectID=${projectID}&fileCode=${fileCode}`)
    .then(response => response.json())
    .then(data => {
        const container = document.getElementById(containerId);
        const status = document.getElementById(statusId);

        if (!container || !status) return;

        container.innerHTML = '';

        if (data.success && data.files && data.files.length > 0) {
            // 有檔案，顯示檔案列表
            status.textContent = '已上傳';
            status.className = '';

            data.files.forEach(fileInfo => {
                const tag = createFileTag(fileInfo, fileCode);
                container.appendChild(tag);
            });
        } else {
            // 無檔案
            status.textContent = '尚未上傳';
            status.className = 'text-pink';
        }
    })
    .catch(error => {
        console.error('載入檔案列表失敗：', error);
    });
}

/**
 * 建立檔案標籤
 * @param {Object} fileInfo - 檔案資訊
 * @param {string} fileCode - 檔案代碼
 * @returns {HTMLElement}
 */
function createFileTag(fileInfo, fileCode) {
    const tag = document.createElement('span');
    tag.className = 'tag tag-green-light';

    const link = document.createElement('a');
    link.className = 'tag-link';
    link.href = `/Service/SCI_Download.ashx?action=downloadFile&projectID=${getProjectID()}&fileCode=${fileCode}&fileName=${encodeURIComponent(fileInfo.FileName)}`;
    link.target = '_blank';
    link.textContent = fileInfo.FileName;

    const deleteBtn = document.createElement('button');
    deleteBtn.type = 'button';
    deleteBtn.className = 'tag-btn';
    deleteBtn.innerHTML = '<i class="fa-solid fa-circle-xmark"></i>';
    deleteBtn.onclick = function() {
        deleteFile(fileCode, fileInfo.FileName);
    };

    tag.appendChild(link);
    tag.appendChild(deleteBtn);

    return tag;
}

/**
 * 刪除檔案
 * @param {string} fileCode - 檔案代碼
 * @param {string} fileName - 檔案名稱
 */
function deleteFile(fileCode, fileName) {
    Swal.fire({
        title: '確認刪除',
        text: '確定要刪除此檔案嗎？',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: '確定刪除',
        cancelButtonText: '取消'
    }).then((result) => {
        if (!result.isConfirmed) {
            return;
        }

        const projectID = getProjectID();
        const formData = new FormData();
        formData.append('action', 'delete');
        formData.append('projectID', projectID);
        formData.append('fileCode', fileCode);
        formData.append('fileName', fileName);

        fetch('/Service/SCI_Upload.ashx', {
            method: 'POST',
            body: formData
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                Swal.fire({
                    icon: 'success',
                    title: '刪除成功',
                    text: '檔案已成功刪除',
                    timer: 2000,
                    showConfirmButton: false
                });
                // 重新載入已上傳檔案列表
                loadUploadedFiles();
            } else {
                Swal.fire({
                    icon: 'error',
                    title: '刪除失敗',
                    text: data.message
                });
            }
        })
        .catch(error => {
            Swal.fire({
                icon: 'error',
                title: '刪除失敗',
                text: error.message
            });
        });
    });
}
