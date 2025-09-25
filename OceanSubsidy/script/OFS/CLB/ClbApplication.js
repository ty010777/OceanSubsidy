$(document).ready(function() {
    // 初始化頁面
    initializePage();
    
    // 綁定事件
    bindEvents();
});

function initializePage() {
    // 初始化日期選擇器
    initializeDatePickers();
    
    // 同步已載入的日期資料到隱藏欄位
    syncLoadedDatesToHiddenFields();
    
    // 初始化數字輸入格式
    initializeNumberInputs();
    
    // 初始化驗證
    initializeValidation();
}

function bindEvents() {
    // 經費欄位自動計算
    $('.funds-input').on('input', function() {
        calculateTotalFunds();
    });

    // 表單提交前驗證
    $('form').on('submit', function(e) {
        if (!validateForm()) {
            e.preventDefault();
            return false;
        }
    });

    // 初始化上傳附件功能
    initializeFileUpload();
}

// 全域變數
var currentTab = 'application'; // 預設顯示申請表
var enableUploadStep = false; // 是否啟用上傳附件步驟
var isReadOnlyMode = false; // 是否為檢視模式

/**
 * 切換標籤
 * @param {string} tabName - 標籤名稱 ('application' 或 'upload')
 */
function switchTab(tabName) {
    // 檢查是否允許切換到上傳附件標籤
    if (tabName === 'upload' && !enableUploadStep) {
        Swal.fire({
            icon: 'warning',
            title: '無法切換',
            text: '請先完成申請表內容並儲存後，才能進入上傳附件步驟',
            confirmButtonText: '確定'
        });
        return;
    }

    currentTab = tabName;

    // 切換標籤樣式
    $('.application-step .step-item').removeClass('active');

    if (tabName === 'application') {
        $('#applicationTab').addClass('active');
        $('#applicationFormSection').show();
        $('#uploadAttachmentSection').hide();

        // 更新按鈕顯示
        updateButtonVisibility('application', isReadOnlyMode);

        // 更新狀態
        updateStepStatus('application', '編輯中', 'edit');
        if (enableUploadStep) {
            updateStepStatus('upload', '已完成');
        }else{
            updateStepStatus('upload', '');
        }

    } else if (tabName === 'upload') {
        $('#uploadTab').addClass('active');
        $('#applicationFormSection').hide();
        $('#uploadAttachmentSection').show();

        // 更新按鈕顯示
        updateButtonVisibility('upload', isReadOnlyMode);
        // 更新狀態
        updateStepStatus('application', '已完成');

        updateStepStatus('upload', '編輯中', 'edit');
    }
}

/**
 * 更新按鈕顯示狀態
 * @param {string} currentTabName - 目前標籤名稱
 * @param {boolean} isReadOnly - 是否為檢視模式 (可選參數，預設為 false)
 */
function updateButtonVisibility(currentTabName, isReadOnly = false) {
    // 現在按鈕是 HTML button，使用正確的選擇器
    var saveAndNextBtn = $('#btnSaveAndNext');
    var submitAppBtn = $('#btnSubmitApplication');

    // 如果是檢視模式，隱藏所有按鈕
    if (isReadOnly) {
        saveAndNextBtn.hide();
        submitAppBtn.hide();
        return;
    }

    if (currentTabName === 'application') {
        // 申請表標籤：顯示「完成本頁，下一步」
        saveAndNextBtn.show();
        submitAppBtn.hide();
    } else if (currentTabName === 'upload') {
        // 上傳附件標籤：顯示「完成本頁，提送申請」
        saveAndNextBtn.hide();
        submitAppBtn.show();
    }
}

/**
 * 設定是否啟用上傳附件步驟
 * @param {boolean} enable - 是否啟用
 */
// function setUploadStepEnabled(enable) {
    // enableUploadStep = enable;
    //
    // var uploadTab = $('#uploadTab');
    //
    // if (enable) {
    //     // 啟用上傳附件標籤
    //     uploadTab.removeClass('disabled')
    //              .attr('aria-disabled', 'false')
    //              .attr('tabindex', '0');
    //    
    // } else {
    //     // 禁用上傳附件標籤
    //     uploadTab.addClass('disabled')
    //              .attr('aria-disabled', 'true')
    //              .removeAttr('tabindex');
    //
    //     // 移除狀態文字
    //     uploadTab.find('.step-status').remove();
    // }
// }

/**
 * 更新步驟狀態文字（重新定義以避免衝突）
 * @param {string} stepName - 步驟名稱 ('application' 或 'upload')
 * @param {string} status - 狀態文字
 * @param {string} statusClass - 狀態樣式類別 (可選)
 */
function updateStepStatus(stepName, status, statusClass = '') {
    var targetTab = stepName === 'application' ? $('#applicationTab') : $('#uploadTab');

    // 移除現有狀態
    targetTab.find('.step-status').remove();

    // 新增狀態文字
    if (status) {
        var statusHtml = '<div class="step-status ' + statusClass + '">' + status + '</div>';
        targetTab.find('.step-content').append(statusHtml);
    }
}

/**
 * 初始化頁面標籤功能
 * @param {boolean} uploadEnabled - 是否啟用上傳附件步驟
 * @param {string} initialTab - 初始顯示的標籤 (可選，預設為 'application')
 * @param {boolean} readOnly - 是否為檢視模式 (可選，預設為 false)
 */
function initializeTabSystem(uploadEnabled, initialTab = 'application', readOnly = false) {
    enableUploadStep = uploadEnabled;
    isReadOnlyMode = readOnly;

    // 設定上傳步驟狀態
    // setUploadStepEnabled(uploadEnabled);

    // 切換到初始標籤
    switchTab(initialTab);
}

function initializeDatePickers() {
    // 設定 moment.js 為繁體中文
    if (typeof moment !== 'undefined') {
        moment.locale('zh-tw');
        
        // 統一初始化所有日期選擇器（都使用民國年顯示）
        initializeAllDatePickers();
    }
}

function initializeAllDatePickers() {
    // 統一的日期選擇器配置 - 所有日期都使用民國年顯示，但儲存為西元年
    const datePickerConfig = {
        singleDatePicker: true,
        showDropdowns: true,
        autoUpdateInput: false,
        autoApply: true,
        locale: {
            format: 'tYY/MM/DD' // 使用 moment-taiwan 提供的 tYY 格式
        }
    };
    
    // 通用的日期選擇處理函數
    const handleDateSelection = function(ev, picker) {
        const selectedMoment = moment(picker.startDate);
        
        // 顯示：民國年格式
        const displayDate = selectedMoment.format('tYY/MM/DD');
        
        // 儲存：西元年格式
        const gregorianDate = selectedMoment.format('YYYY/MM/DD');
        
        $(this).val(displayDate);
        $(this).data('gregorian-date', gregorianDate);
        
        // 建立隱藏欄位儲存西元日期供後端使用
        const hiddenFieldName = $(this).attr('name') + '_gregorian';
        let hiddenField = $(`input[name="${hiddenFieldName}"]`);
        if (hiddenField.length === 0) {
            hiddenField = $(`<input type="hidden" name="${hiddenFieldName}" />`);
            $(this).after(hiddenField);
        }
        hiddenField.val(gregorianDate);
    };
    
    // 初始化成立日期選擇器
    $('input[id*="txtCreationDate"]').daterangepicker(datePickerConfig)
        .on('apply.daterangepicker', handleDateSelection);
    
    // 初始化開始日期選擇器
    const startDateSelector = $('input[id*="txtStartDate"]');
    const endDateSelector = $('input[id*="txtEndDate"]');
    
    startDateSelector.daterangepicker(datePickerConfig)
        .on('apply.daterangepicker', function(ev, picker) {
            // 執行通用處理
            handleDateSelection.call(this, ev, picker);
            
            // 額外處理：設定結束日期的最小值為開始日期
            const selectedMoment = moment(picker.startDate);
            const endDatePicker = endDateSelector.data('daterangepicker');
            if (endDatePicker) {
                endDatePicker.minDate = selectedMoment;
            }
        });
    
    // 初始化結束日期選擇器
    endDateSelector.daterangepicker(datePickerConfig)
        .on('apply.daterangepicker', handleDateSelection);
}

function syncLoadedDatesToHiddenFields() {
    // 同步所有具有 data-gregorian-date 屬性的日期欄位到隱藏欄位
    $('.taiwan-date-picker').each(function() {
        const $input = $(this);
        const gregorianDate = $input.data('gregorian-date');
        
        if (gregorianDate) {
            // 建立隱藏欄位儲存西元日期供後端使用
            const hiddenFieldName = $input.attr('name') + '_gregorian';
            let hiddenField = $(`input[name="${hiddenFieldName}"]`);
            if (hiddenField.length === 0) {
                hiddenField = $(`<input type="hidden" name="${hiddenFieldName}" />`);
                $input.after(hiddenField);
            }
            hiddenField.val(gregorianDate);
        }
    });
}

function initializeNumberInputs() {
    // 綁定經費欄位變化事件進行自動計算
    $('input[id*="Funds"]').on('input', function() {
        calculateTotalFunds();
    });
    
    // 綁定經費計算
    $('input[id*="Funds"]').addClass('funds-input');

    ['rbPreviouslySubsidizedYes', 'rbPreviouslySubsidizedNo','rbSubsidyTypeActivity','rbSubsidyTypeOperation','rbSubsidyTypeCreate'].forEach(function (id) {
        var el = document.getElementById(id);
        if (el) {
            el.classList.add('form-check-input', 'check-teal');
        }
    });
}

function initializeValidation() {
    // 必填欄位標示
    $('span.text-danger').each(function() {
        $(this).parent().find('input, select, textarea').addClass('required-field');
    });
}

function calculateTotalFunds() {
    // 使用通用的 ID 選擇器來查找經費輸入欄位
    let subsidyFunds = parseFloatFromFormatted($('input[id*="txtSubsidyFunds"]').val()) || 0;
    let selfFunds = parseFloatFromFormatted($('input[id*="txtSelfFunds"]').val()) || 0;
    let otherGovFunds = parseFloatFromFormatted($('input[id*="txtOtherGovFunds"]').val()) || 0;
    let otherUnitFunds = parseFloatFromFormatted($('input[id*="txtOtherUnitFunds"]').val()) || 0;
    
    let total = subsidyFunds + selfFunds + otherGovFunds + otherUnitFunds;
    
    // 更新總經費顯示，使用千分位格式
    $('span[id*="lblTotalFunds"]').text(total.toLocaleString());
}

function parseFloatFromFormatted(value) {
    if (!value) return 0;
    // number type 輸入框不會有千分位逗號，直接轉換即可
    return parseFloat(value) || 0;
}

function validateForm() {
    let isValid = true;
    let errorMessages = [];
    
    // 簡化驗證：只檢查必填欄位是否有值
    $('.required-field').each(function() {
        let $field = $(this);
        let value = $field.val();
        
        if (!value || value.trim() === '') {
            isValid = false;
            $field.addClass('is-invalid');
            
            let label = $field.closest('tr').find('th').text().replace('*', '').trim();
            errorMessages.push(label + ' 為必填欄位');
        } else {
            $field.removeClass('is-invalid');
        }
    });
    
    // 顯示錯誤訊息
    if (!isValid) {
        Swal.fire({
            icon: 'error',
            title: '表單驗證失敗',
            html: errorMessages.join('<br>'),
            confirmButtonText: '確定'
        });
    }
    
    return isValid;
}

function isValidDate(dateString) {
    let regex = /^\d{4}\/\d{2}\/\d{2}$/;
    if (!regex.test(dateString)) return false;
    
    let date = new Date(dateString);
    return date instanceof Date && !isNaN(date);
}

// 使用 moment-taiwan 套件的輔助函數
function formatTaiwanDate(gregorianDateString) {
    // 使用套件提供的 tYY 格式
    return moment(gregorianDateString).format('tYY/MM/DD');
}

function formatGregorianDate(taiwanDateString) {
    // 從民國年字串解析並轉為西元年
    // 這裡可以使用套件的解析功能，但由於我們主要用於顯示，保持簡單即可
    return moment(taiwanDateString, 'tYY/MM/DD').format('YYYY/MM/DD');
}


function goToNextStep() {
    if (validateForm()) {
        updateStepStatus(2, 'active');
        
        // TODO: 實際的步驟切換邏輯
        // 例如：顯示上傳附件區域，隱藏申請表區域
        Swal.fire({
            icon: 'success',
            title: '申請表已完成',
            text: '即將進入上傳附件步驟',
            timer: 1500,
            showConfirmButton: false
        });
    }
}


// 暫存功能
function tempSave() {
    Swal.fire({
        title: '正在暫存...',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    
    // TODO: 實際的暫存邏輯
    setTimeout(() => {
        Swal.fire({
            icon: 'success',
            title: '暫存成功',
            timer: 1500,
            showConfirmButton: false
        });
    }, 1000);
}

// 提交申請
function submitApplication() {
    if (!validateForm()) {
        return;
    }
    
    Swal.fire({
        title: '確定要提交申請嗎？',
        text: '提交後將無法修改',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: '確定提交',
        cancelButtonText: '取消'
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                title: '正在提交申請...',
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });
            
            // TODO: 實際的提交邏輯
            setTimeout(() => {
                Swal.fire({
                    icon: 'success',
                    title: '申請提交成功',
                    text: '您的申請已成功提交，請等待審核結果',
                    confirmButtonText: '確定'
                }).then(() => {
                    // 導向到申請清單頁面
                    // window.location.href = '/OFS/ApplicationChecklist.aspx';
                });
            }, 2000);
        }
    });
}

// 額外的樣式定義
const customCSS = `
<style>
.required-field.is-invalid {
    border-color: #dc3545;
    box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.25);
}

.funds-input {
    text-align: right;
}

/* 申請補助類型 RadioButton 樣式 */
.subsidy-type-radios .form-check {
    margin-right: 2rem;
}

</style>
`;

// 將 CSS 加入到 head
if (document.querySelector('style[data-clb-custom]') === null) {
    $('head').append(customCSS.replace('<style>', '<style data-clb-custom>'));
}

function initializeFileUpload() {
    // 上傳按鈕點擊事件
    $('.btn-primary').filter(':contains("上傳")').on('click', function() {
        let $row = $(this).closest('tr');
        let $fileInput = $row.find('input[type="file"]');
        let file = $fileInput[0].files[0];
        
        if (!file) {
            Swal.fire({
                icon: 'warning',
                title: '請選擇檔案',
                text: '請先選擇要上傳的檔案',
                confirmButtonText: '確定'
            });
            return;
        }
        
        // 模擬上傳過程
        uploadFile($row, file);
    });
    
    // 下載範本按鈕
    $('.btn-outline-secondary').filter(':contains("下載範本")').on('click', function(e) {
        e.preventDefault();
        
        Swal.fire({
            icon: 'info',
            title: '下載範本',
            text: '範本下載功能尚未實作',
            confirmButtonText: '確定'
        });
    });
}

function uploadFile($row, file) {
    let fileName = file.name;
    let attachmentName = $row.find('td:eq(1)').text();
    
    // 顯示上傳進度
    Swal.fire({
        title: '正在上傳...',
        html: `上傳檔案：${fileName}<br>附件：${attachmentName}`,
        allowOutsideClick: false,
        showConfirmButton: false,
        willOpen: () => {
            Swal.showLoading();
        }
    });
    
    // 模擬上傳延遲
    setTimeout(() => {
        // 更新狀態為已上傳
        let $statusBadge = $row.find('.badge');
        $statusBadge.removeClass('bg-warning bg-secondary').addClass('bg-success').text('已上傳');
        
        // 更新進度
        updateUploadProgress();
        
        Swal.fire({
            icon: 'success',
            title: '上傳成功',
            text: `檔案「${fileName}」已成功上傳`,
            confirmButtonText: '確定'
        });
    }, 2000);
}

function updateUploadProgress() {
    // 計算上傳進度
    let totalRequired = $('.badge.bg-warning, .badge.bg-success').filter(':not(.bg-secondary)').length;
    let uploaded = $('.badge.bg-success').length;
    let percentage = totalRequired > 0 ? Math.round((uploaded / totalRequired) * 100) : 0;
    
    // 更新進度條
    $('.progress-bar').css('width', percentage + '%').attr('aria-valuenow', percentage).text(percentage + '%');
    $('.text-muted').filter(':contains("必要附件")').text(`必要附件：${uploaded}/${totalRequired} 已上傳`);
    
    // 更新申請狀態
    let $statusBadge = $('.badge.fs-6');
    let $statusText = $statusBadge.closest('.card-body').find('.text-muted');
    
    if (percentage === 100) {
        $statusBadge.removeClass('bg-warning').addClass('bg-success').text('可提送');
        $statusText.text('所有必要附件已上傳完成，可以提送申請');
    } else {
        $statusBadge.removeClass('bg-success').addClass('bg-warning').text('準備中');
        $statusText.text('尚有必要附件未上傳完成');
    }
}

// 新增的文件上傳處理功能

/**
 * 處理檔案上傳
 * @param {string} fileCode - 檔案代碼 (FILE_CLB1, FILE_CLB2, FILE_CLB3)
 * @param {HTMLInputElement} fileInput - 檔案輸入元素
 */
function handleFileUpload(fileCode, fileInput) {
    const file = fileInput.files[0];
    if (!file) {
        return;
    }

    // 檢查檔案格式
    if (!file.name.toLowerCase().endsWith('.pdf')) {
        showErrorMessage('僅支援PDF格式檔案上傳。');
        fileInput.value = '';
        return;
    }
    
    // 檢查檔案大小 (10MB)
    const maxSize = 10 * 1024 * 1024; // 10MB in bytes
    if (file.size > maxSize) {
        showErrorMessage('檔案大小不能超過10MB。');
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
    formData.append('fileCode', fileCode);
    formData.append('action', 'upload');
    formData.append('fileType', 'Application');  // 表示這是申請表的檔案上傳
    
    // 取得 ProjectID
    const projectID = getProjectID();
    if (projectID) {
        formData.append('projectID', projectID);
    }

    // 發送 AJAX 請求到 CLB_Upload.ashx
    fetch('../../../Service/CLB_Upload.ashx', {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data && data.success) {
            // 直接更新 UI，不重新載入頁面
            updateFileStatusUIFromJS(fileCode, data.fileName, data.relativePath);
            
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
 * 刪除檔案
 * @param {string} fileCode - 檔案代碼
 */
function deleteFile(fileCode) {
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
            performFileDelete(fileCode);
        }
    });
}

/**
 * 執行檔案刪除
 * @param {string} fileCode - 檔案代碼
 */
function performFileDelete(fileCode) {
    const projectID = getProjectID();
    if (!projectID) {
        showErrorMessage('計畫編號不存在');
        return;
    }

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
    formData.append('fileType', 'Application');
    formData.append('fileCode', fileCode);
    formData.append('projectID', projectID);

    // 發送 AJAX 請求到 CLB_Upload.ashx
    fetch('../../../Service/CLB_Upload.ashx', {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data && data.success) {
            // 直接更新 UI，不重新載入頁面
            resetFileStatusUIFromJS(fileCode);
            
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

/**
 * 取得目前的 ProjectID
 * @returns {string} ProjectID
 */
function getProjectID() {
    // 嘗試從隱藏欄位取得 ProjectID
    const hiddenField = $('[id*="hidProjectID"]');
    if (hiddenField.length > 0 && hiddenField.val()) {
        return hiddenField.val();
    }
    
    // 嘗試從 URL 參數取得 ProjectID
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('ProjectID') || '';
}

/**
 * 顯示成功訊息
 * @param {string} message - 訊息內容
 */
function showSuccessMessage(message) {
    Swal.fire({
        icon: 'success',
        title: '成功',
        text: message,
        confirmButtonText: '確定'
    });
}

/**
 * 顯示錯誤訊息
 * @param {string} message - 錯誤訊息
 */
function showErrorMessage(message) {
    Swal.fire({
        icon: 'error',
        title: '錯誤',
        text: message,
        confirmButtonText: '確定'
    });
}

/**
 * 顯示警告訊息
 * @param {string} message - 警告訊息
 */
function showWarningMessage(message) {
    Swal.fire({
        icon: 'warning',
        title: '警告',
        text: message,
        confirmButtonText: '確定'
    });
}

/**
 * 顯示資訊訊息
 * @param {string} message - 資訊訊息
 */
function showInfoMessage(message) {
    Swal.fire({
        icon: 'info',
        title: '提示',
        text: message,
        confirmButtonText: '確定'
    });
}

/**
 * 下載範本檔案
 * @param {string} templateType - 範本類型 ('1', '2', '3')
 */
function downloadTemplate(templateType) {
    // 取得當前的 ProjectID
    const projectID = getProjectID();
    
    // 取得網站根路徑
    const baseUrl = window.location.origin + '/';
    
    // 構建下載 URL
    let downloadUrl = baseUrl + 'Service/CLB_download.ashx?action=template&type=' + templateType;
    if (projectID) {
        downloadUrl += '&projectID=' + encodeURIComponent(projectID);
    }
    
    // 開啟下載
    window.open(downloadUrl, '_blank');
}

/**
 * 下載已上傳的檔案
 * @param {string} fileCode - 檔案代碼
 */
function downloadUploadedFile(fileCode) {
    const projectID = getProjectID();
    if (!projectID) {
        showErrorMessage('計畫編號不存在');
        return;
    }

    // 取得網站根路徑
    const baseUrl = window.location.origin + '/';
    
    // 構建下載 URL
    const downloadUrl = baseUrl + 'Service/CLB_download.ashx?action=file&projectID=' + 
                       encodeURIComponent(projectID) + '&fileCode=' + encodeURIComponent(fileCode);
    
    // 開啟下載
    window.open(downloadUrl, '_blank');
}

/**
 * 從 JavaScript 端更新檔案狀態 UI
 * @param {string} fileCode - 檔案代碼
 * @param {string} fileName - 檔案名稱
 * @param {string} relativePath - 相對路徑
 */
function updateFileStatusUIFromJS(fileCode, fileName, relativePath) {
    // 取得對應的狀態標籤和檔案面板
    // FILE_CLB1 -> CLB1, FILE_CLB2 -> CLB2, etc.
    const statusLabelId = `lblStatus${fileCode.replace('FILE_', '')}`;
    const filesPanelId = `pnlFiles${fileCode.replace('FILE_', '')}`;
    
    const statusLabel = $(`[id*="${statusLabelId}"]`);
    let filesPanel = $(`[id*="${filesPanelId}"]`);
    
    if (statusLabel.length > 0) {
        // 更新狀態標籤
        statusLabel.text('已上傳').removeClass('text-muted').addClass('text-success');
        
        // 如果檔案面板不存在，可能是因為 Visible="false"，嘗試找到它的容器並創建
        if (filesPanel.length === 0) {
            // 在狀態標籤的父容器中尋找可能的檔案面板位置
            const parentTd = statusLabel.closest('td').next('td');
            if (parentTd.length > 0) {
                // 在上傳按鈕後面查找或創建檔案面板
                let existingPanel = parentTd.find('.tag-group');
                if (existingPanel.length === 0) {
                    // 創建新的檔案面板
                    existingPanel = $(`<div class="tag-group mt-2 gap-1" id="${filesPanelId}_js"></div>`);
                    parentTd.append(existingPanel);
                }
                filesPanel = existingPanel;
            }
        }
        
        if (filesPanel.length > 0) {
            // 清空並重建檔案面板內容，確保面板可見
            filesPanel.empty().show().css('display', 'block');
            
            // 建立檔案標籤 HTML
            const fileTagHtml = `
                <span class="tag tag-green-light">
                    <a class="tag-link" href="#" onclick="downloadUploadedFile('${fileCode}'); return false;" target="_blank">
                        ${fileName}
                    </a>
                    <button type="button" class="tag-btn" onclick="deleteFile('${fileCode}')">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </span>
            `;
            
            filesPanel.html(fileTagHtml);
            
            console.log(`File status updated for ${fileCode}: ${fileName}`);
        } else {
            console.warn(`Files panel not found for ${fileCode}`);
        }
    } else {
        console.warn(`Status label not found for ${fileCode}`);
    }
}

/**
 * 從 JavaScript 端重置檔案狀態 UI
 * @param {string} fileCode - 檔案代碼
 */
function resetFileStatusUIFromJS(fileCode) {
    // 取得對應的狀態標籤和檔案面板
    // FILE_CLB1 -> CLB1, FILE_CLB2 -> CLB2, etc.
    const statusLabelId = `lblStatus${fileCode.replace('FILE_', '')}`;
    const filesPanelId = `pnlFiles${fileCode.replace('FILE_', '')}`;
    
    const statusLabel = $(`[id*="${statusLabelId}"]`);
    let filesPanel = $(`[id*="${filesPanelId}"]`);
    
    if (statusLabel.length > 0) {
        // 重置狀態標籤
        statusLabel.text('未上傳').removeClass('text-success').addClass('text-muted');
        
        // 查找檔案面板，包括可能由 JavaScript 創建的面板
        if (filesPanel.length === 0) {
            filesPanel = $(`#${filesPanelId}_js`);
        }
        
        if (filesPanel.length > 0) {
            // 隱藏並清空檔案面板
            filesPanel.hide().empty();
        }
        
        // 同時查找並移除可能存在的其他檔案面板
        statusLabel.closest('td').next('td').find('.tag-group').hide().empty();
        
        console.log(`File status reset for ${fileCode}`);
    }
}


/**
 * 導航到指定的步驟（保留舊函數名稱以相容現有代碼）
 */
function navigateToStep(stepIndex) {
    // 轉換為新的標籤切換方式
    var tabName = stepIndex === 0 ? 'application' : 'upload';
    switchTab(tabName);
}

/**
 * 執行最終申請提送
 */
function submitApplicationFinal(projectID) {
    // 如果 projectID 為空，嘗試從 URL 參數取得
    if (!projectID) {
        const urlParams = new URLSearchParams(window.location.search);
        projectID = urlParams.get('ProjectID');
    }
    
    // 如果還是為空，嘗試從全域函數取得
    if (!projectID) {
        projectID = getProjectID();
    }
    
    if (!projectID) {
        Swal.fire({
            icon: 'error',
            title: '錯誤',
            text: '無法取得計畫編號，請重新進入頁面',
            confirmButtonText: '確定'
        });
        return;
    }

    // 顯示載入中訊息
    Swal.fire({
        title: '提送中...',
        text: '正在處理您的申請，請稍候。',
        allowOutsideClick: false,
        allowEscapeKey: false,
        showConfirmButton: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    // 準備 AJAX 資料
    const formData = new FormData();
    formData.append('action', 'submitApplication');
    formData.append('projectID', projectID);
    formData.append('txtChangeBefore', $('[id*="txtChangeBefore"]').val() || '');
    formData.append('txtChangeAfter', $('[id*="txtChangeAfter"]').val() || '');

    // 執行 fetch 請求
    fetch(window.location.pathname, {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(result => {
        if (result.success) {
            // 提送成功
            Swal.fire({
                icon: 'success',
                title: '提送成功！',
                html: result.message + '<br><br>系統將自動導向申請列表頁面。',
                timer: 3000,
                showConfirmButton: false
            }).then(() => {
                // 導向申請列表頁面
                window.location.href = window.AppRootPath+'/OFS/ApplicationChecklist.aspx';
            });
        } else {
            // 提送失敗
            Swal.fire({
                icon: 'error',
                title: '提送失敗',
                text: result.message || '未知錯誤，請稍後再試',
                confirmButtonText: '確定'
            });
        }
    })
    .catch(error => {
        console.error('Submit error:', error);
        Swal.fire({
            icon: 'error',
            title: '提送失敗',
            text: '網路連線錯誤，請檢查您的網路連線後再試',
            confirmButtonText: '確定'
        });
    });
}

/**
 * 處理進度條步驟的鍵盤事件
 */
function handleStepKeydown(event, stepIndex) {
    // Enter 或 Space 鍵觸發點擊事件
    if (event.key === 'Enter' || event.key === ' ') {
        event.preventDefault();
        navigateToStep(stepIndex);
    }
}

/**
 * 初始化進度條的鍵盤事件處理
 */
function initializeStepKeyboardEvents() {
    $('.application-step .step-item').each(function(index) {
        $(this).on('keydown', function(event) {
            handleStepKeydown(event, index);
        });
        
        // 為可聚焦的步驟設定 tabindex
        if (!$(this).hasClass('disabled') && $(this).attr('aria-disabled') !== 'true') {
            $(this).attr('tabindex', '0');
        }
    });
}

// 頁面載入完成後初始化鍵盤事件
$(document).ready(function() {
    // 延遲執行，確保動態設定的 disabled 狀態已經生效
    setTimeout(initializeStepKeyboardEvents, 100);
});

/**
 * 處理暫存按鈕點擊
 */
function handleTempSave() {
    // 顯示載入中訊息
    Swal.fire({
        title: '暫存中...',
        text: '正在儲存您的資料，請稍候。',
        allowOutsideClick: false,
        allowEscapeKey: false,
        showConfirmButton: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    // 收集表單資料
    const formData = collectFormData();
    formData.append('action', 'tempSave');

    // 發送 AJAX 請求
    fetch(window.location.pathname, {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            Swal.fire({
                icon: 'success',
                title: '暫存成功！',
                html: `計畫編號：<strong>${data.projectID}</strong><br>資料已成功暫存`,
                confirmButtonText: '確定'
            }).then(() => {
                // 如果是新建的專案，需要重新載入頁面以更新 URL
                if (window.location.search.indexOf('ProjectID=') === -1) {
                    window.location.href = `ClbApplication.aspx?ProjectID=${data.projectID}`;
                }
            });
        } else {
            Swal.fire({
                icon: 'error',
                title: '暫存失敗',
                text: data.message || '未知錯誤，請稍後再試',
                confirmButtonText: '確定'
            });
        }
    })
    .catch(error => {
        console.error('TempSave error:', error);
        Swal.fire({
            icon: 'error',
            title: '暫存失敗',
            text: '網路連線錯誤，請檢查您的網路連線後再試',
            confirmButtonText: '確定'
        });
    });
}

/**
 * 處理儲存並下一步按鈕點擊
 */
function handleSaveAndNext() {
    // 顯示載入中訊息
    Swal.fire({
        title: '儲存中...',
        text: '正在儲存您的資料，請稍候。',
        allowOutsideClick: false,
        allowEscapeKey: false,
        showConfirmButton: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    // 收集表單資料
    const formData = collectFormData();
    formData.append('action', 'saveAndNext');

    // 發送 AJAX 請求
    fetch(window.location.pathname, {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            Swal.fire({
                icon: 'success',
                title: '儲存成功！',
                html: `計畫編號：<strong>${data.projectID}</strong><br>即將進入上傳附件步驟`,
                timer: 2000,
                showConfirmButton: false
            }).then(() => {
                const currentPage = window.location.pathname.split('/').pop();

                if (currentPage == 'ClbApplication.aspx') {
                    // 只有當前頁面不是 ClbApplication.aspx 才跳轉
                    window.location.href = `ClbApplication.aspx?ProjectID=${data.projectID}`;
                }            
            });
        } else {
            Swal.fire({
                icon: 'error',
                title: '儲存失敗',
                text: data.message || '未知錯誤，請稍後再試',
                confirmButtonText: '確定'
            });
        }
    })
    .catch(error => {
        console.error('SaveAndNext error:', error);
        Swal.fire({
            icon: 'error',
            title: '儲存失敗',
            text: '網路連線錯誤，請檢查您的網路連線後再試',
            confirmButtonText: '確定'
        });
    });
}

/**
 * 處理提送申請按鈕點擊
 */
function handleSubmitApplication() {
    const projectID = getProjectID();

    if (!projectID) {
        Swal.fire({
            icon: 'error',
            title: '錯誤',
            text: '計畫編號不存在，請重新進入頁面',
            confirmButtonText: '確定'
        });
        return;
    }

    // 顯示確認對話框
    Swal.fire({
        icon: 'warning',
        title: '確認提送申請',
        html: '是否要進行提送？<br><span style="color: red;">提送後將無法再編輯資料。</span>',
        showCancelButton: true,
        confirmButtonText: '是，提送申請',
        cancelButtonText: '取消',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            // 使用者確認提送，執行提送邏輯
            submitApplicationFinal(projectID);
        }
    });
}

/**
 * 收集表單資料
 */
function collectFormData() {
    const formData = new FormData();

    // 基本資料
    const projectID = getProjectID();
    if (projectID) formData.append('projectID', projectID);

    formData.append('year', $('[id*="hidYear"]').val() || new Date().getFullYear());
    formData.append('subsidyPlanType', $('[id*="hidSubsidyPlanType"]').val() || '學校社團');
    formData.append('projectNameTw', $('[id*="txtProjectNameTw"]').val() || '');

    // 申請補助類型
    let subsidyType = '';
    if ($('[id*="rbSubsidyTypeCreate"]').prop('checked')) subsidyType = 'Startup';
    else if ($('[id*="rbSubsidyTypeOperation"]').prop('checked')) subsidyType = 'Admin';
    else if ($('[id*="rbSubsidyTypeActivity"]').prop('checked')) subsidyType = 'Public';
    formData.append('subsidyType', subsidyType);

    formData.append('schoolName', $('[id*="txtSchoolName"]').val() || '');
    formData.append('schoolIDNumber', $('[id*="txtSchoolIDNumber"]').val() || '');
    formData.append('clubName', $('[id*="txtClubName"]').val() || '');
    formData.append('address', $('[id*="txtAddress"]').val() || '');

    // 日期欄位 - 從 data attribute 取得西元日期
    const creationDate = $('[id*="txtCreationDate"]').data('gregorian-date');
    if (creationDate) formData.append('creationDate', creationDate);

    const startDate = $('[id*="txtStartDate"]').data('gregorian-date');
    if (startDate) formData.append('startDate', startDate);

    const endDate = $('[id*="txtEndDate"]').data('gregorian-date');
    if (endDate) formData.append('endDate', endDate);

    // 計畫資訊
    formData.append('purpose', $('[id*="txtPurpose"]').val() || '');
    formData.append('planContent', $('[id*="txtPlanContent"]').val() || '');
    formData.append('preBenefits', $('[id*="txtPreBenefits"]').val() || '');
    formData.append('planLocation', $('[id*="txtPlanLocation"]').val() || '');
    formData.append('estimatedPeople', $('[id*="txtEstimatedPeople"]').val() || '');
    formData.append('emergencyPlan', $('[id*="txtEmergencyPlan"]').val() || '');

    // 經費資料
    formData.append('subsidyFunds', $('[id*="txtSubsidyFunds"]').val() || '0');
    formData.append('selfFunds', $('[id*="txtSelfFunds"]').val() || '0');
    formData.append('otherGovFunds', $('[id*="txtOtherGovFunds"]').val() || '0');
    formData.append('otherUnitFunds', $('[id*="txtOtherUnitFunds"]').val() || '0');

    const previouslySubsidized = $('[id*="rbPreviouslySubsidizedYes"]').prop('checked');
    formData.append('previouslySubsidized', previouslySubsidized);

    formData.append('fundingDescription', $('[id*="txtFundingDescription"]').val() || '');

    // 人員資料
    formData.append('teacherName', $('[id*="txtTeacherName"]').val() || '');
    formData.append('teacherJobTitle', $('[id*="txtTeacherJobTitle"]').val() || '');
    formData.append('teacherPhone', $('[id*="txtTeacherPhone"]').val() || '');
    formData.append('contactName', $('[id*="txtContactName"]').val() || '');
    formData.append('contactJobTitle', $('[id*="txtContactJobTitle"]').val() || '');
    formData.append('contactPhone', $('[id*="txtContactPhone"]').val() || '');
    formData.append('txtChangeBefore', $('[id*="txtChangeBefore"]').val() || '');
    formData.append('txtChangeAfter', $('[id*="txtChangeAfter"]').val() || '');

    return formData;
}

// 將函數公開給全域範圍以供 HTML 中的 onclick 事件使用
window.handleFileUpload = handleFileUpload;
window.deleteFile = deleteFile;
window.downloadTemplate = downloadTemplate;
window.downloadUploadedFile = downloadUploadedFile;
window.updateFileStatusUIFromJS = updateFileStatusUIFromJS;
window.resetFileStatusUIFromJS = resetFileStatusUIFromJS;
window.submitApplicationFinal = submitApplicationFinal;
window.navigateToStep = navigateToStep;
window.switchTab = switchTab;
window.initializeTabSystem = initializeTabSystem;
window.handleTempSave = handleTempSave;
window.handleSaveAndNext = handleSaveAndNext;
window.handleSubmitApplication = handleSubmitApplication;