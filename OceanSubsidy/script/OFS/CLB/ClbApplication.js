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

    // 初始化經費資訊管理
    initializeFundsManagement();

    // 初始化經費預算規劃管理
    initializeBudgetPlanManagement();

    // 初始化經費說明管理
    initializeFundingDescriptionManagement();

    // 初始化 textarea 自動調整高度
    initializeTextareaAutoResize();

    // 初始化字數計算功能
    initializeCharCounter();
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
        // 取得 currentStepNumber，如果未定義則預設為 1
        var currentStep = window.currentStepNumber || 1;

        // 選擇「申請表」標籤時：
        // - 申請表：永遠顯示「編輯中」
        updateStepStatus('application', '編輯中', 'edit');

        // - 上傳附件：根據 currentStep 判斷
        if (currentStep >= 3) {
            // CurrentStep = 3：已提送，上傳附件已完成
            updateStepStatus('upload', '已完成');
        } else {
            // CurrentStep < 3：上傳附件未完成或未開始
            updateStepStatus('upload', '');
        }

    } else if (tabName === 'upload') {
        $('#uploadTab').addClass('active');
        $('#applicationFormSection').hide();
        $('#uploadAttachmentSection').show();

        // 更新按鈕顯示
        updateButtonVisibility('upload', isReadOnlyMode);

        // 更新狀態
        // 選擇「上傳附件」標籤時：
        // - 申請表：顯示「已完成」（因為能進入上傳附件，表示申請表已完成）
        updateStepStatus('application', '已完成');

        // - 上傳附件：顯示「編輯中」
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
    ['rbPreviouslySubsidizedYes', 'rbPreviouslySubsidizedNo','rbSubsidyTypeActivity','rbSubsidyTypeOperation','rbSubsidyTypeCreate'].forEach(function (id) {
        var el = document.getElementById(id);
        if (el) {
            el.classList.add('form-check-input', 'check-teal');
        }
    });

    // 綁定申請補助類型變更事件
    bindSubsidyTypeChange();

    // 初始化時執行一次，確保初始狀態正確
    handleSubsidyTypeChange();

    // 初始化申請海委會補助和自籌款的數字檔控
    initializeAmountInputValidation();

    // 綁定申請金額變更事件（檢查是否超過補助限制）
    $('#txtApplyAmount').on('input', function() {
        checkGrantLimit();
    });

    // 綁定「最近兩年曾獲本會補助」變更事件
    bindPreviouslySubsidizedChange();

    // 初始化經費說明欄位顯示狀態
    handlePreviouslySubsidizedChange();
}

function initializeValidation() {
    // 必填欄位標示
    $('span.text-danger').each(function() {
        $(this).parent().find('input, select, textarea').addClass('required-field');
    });
}

/**
 * 初始化 textarea 自動調整高度功能
 */
function initializeTextareaAutoResize() {
    // 綁定所有具有 textarea-auto-resize class 的 textarea
    $(document).on('input', 'textarea.textarea-auto-resize', function() {
        autoResizeTextarea(this);
    });

    // 初始化時調整所有現有 textarea 的高度
    $('textarea.textarea-auto-resize').each(function() {
        autoResizeTextarea(this);
    });
}

/**
 * 自動調整單個 textarea 的高度
 * @param {HTMLElement} textarea - textarea 元素
 */
function autoResizeTextarea(textarea) {
    // 重置高度以取得正確的 scrollHeight
    textarea.style.height = 'auto';
    // 設定新高度
    textarea.style.height = textarea.scrollHeight + 'px';
}

function parseFloatFromFormatted(value) {
    if (!value) return 0;
    // number type 輸入框不會有千分位逗號，直接轉換即可
    return parseFloat(value) || 0;
}

/**
 * 初始化申請海委會補助和自籌款的數字檔控
 */
function initializeAmountInputValidation() {
    // 綁定申請海委會補助金額輸入事件
    $('#txtApplyAmount').on('input', function() {
        validateAndFormatAmountInput($(this));
    });

    // 綁定自籌款金額輸入事件
    $('#txtSelfAmount').on('input', function() {
        validateAndFormatAmountInput($(this));
    });
}

/**
 * 驗證並格式化金額輸入
 * @param {jQuery} $input - 輸入欄位的 jQuery 物件
 */
function validateAndFormatAmountInput($input) {
    let value = $input.val();

    // 移除所有非數字字元
    value = value.replace(/[^\d]/g, '');

    // 如果是空值，直接返回
    if (value === '') {
        $input.val('');
        return;
    }

    // 轉換為數字
    let numValue = parseInt(value, 10);

    // 檢查是否為負數或非數字
    if (isNaN(numValue) || numValue < 0) {
        $input.val('');
        return;
    }

    // 格式化為千分位並設定回輸入欄位
    $input.val(numValue.toLocaleString('en-US'));
}

/**
 * 綁定申請補助類型變更事件
 */
function bindSubsidyTypeChange() {
    // 綁定所有補助類型的 RadioButton change 事件
    $('[id*="rbSubsidyTypeCreate"], [id*="rbSubsidyTypeOperation"], [id*="rbSubsidyTypeActivity"]').on('change', function() {
        handleSubsidyTypeChange();
        // 變更補助類型時也檢查金額限制
        checkGrantLimit();
    });
}

/**
 * 處理申請補助類型變更
 * - 「公共活動費」被選取時，顯示 public-view 元素
 * - 「社務補助」或「公共活動費」被選取時，顯示 affairs-view 元素
 */
function handleSubsidyTypeChange() {
    // 檢查各補助類型是否被選取
    const isPublicActivitySelected = $('[id*="rbSubsidyTypeActivity"]').prop('checked');
    const isOperationSelected = $('[id*="rbSubsidyTypeOperation"]').prop('checked');

    // 取得所有 class="public-view" 的元素
    const publicViewElements = $('.public-view');

    // 取得所有 class="affairs-view" 的元素
    const affairsViewElements = $('.affairs-view');

    // 處理 public-view 元素
    if (isPublicActivitySelected) {
        // 「公共活動費」被選取：顯示元素
        publicViewElements.removeClass('d-none');
    } else {
        // 「公共活動費」未被選取：隱藏元素
        publicViewElements.addClass('d-none');
    }

    // 處理 affairs-view 元素
    if (isOperationSelected || isPublicActivitySelected) {
        // 「社務補助」或「公共活動費」被選取：顯示元素
        affairsViewElements.removeClass('d-none');
    } else {
        // 「創社補助」被選取：隱藏元素
        affairsViewElements.addClass('d-none');
    }
}

/**
 * 檢查補助金額是否超過限制
 * 讀取 OFS_GrantTargetSetting 表格，檢查當前申請補助類型的金額上限
 * 超過時會顯示紅色警告，但不擋控（僅提示）
 */
function checkGrantLimit() {
    // 檢查是否已載入補助額度限制資料
    if (!window.grantLimitData) {
        return;
    }

    // 取得當前選擇的補助類型
    let targetTypeID = '';
    if ($('[id*="rbSubsidyTypeCreate"]').prop('checked')) {
        targetTypeID = 'CLB1'; // 創社補助
    } else if ($('[id*="rbSubsidyTypeOperation"]').prop('checked')) {
        targetTypeID = 'CLB2'; // 社務活動補助
    } else if ($('[id*="rbSubsidyTypeActivity"]').prop('checked')) {
        targetTypeID = 'CLB3'; // 公共活動費
    }

    // 如果沒有選擇補助類型，清除紅色提示
    if (!targetTypeID) {
        $('#txtApplyAmount').removeClass('text-danger');
        return;
    }

    // 取得該補助類型的限制資料
    const limitData = window.grantLimitData[targetTypeID];
    if (!limitData || !limitData.GrantLimit) {
        return;
    }

    // 取得申請補助金額（使用 txtApplyAmount）
    const applyAmountText = $('#txtApplyAmount').val() || '';
    const applyAmount = parseFloat(applyAmountText.replace(/,/g, '')) || 0;

    // 比較金額
    if (applyAmount > limitData.GrantLimit) {
        // 超過限制，顯示紅色文字（僅提示，不擋控）
        $('#txtApplyAmount').addClass('text-danger');
    } else {
        // 未超過限制，移除紅色文字
        $('#txtApplyAmount').removeClass('text-danger');
    }
}

/**
 * 綁定「最近兩年曾獲本會補助」變更事件
 */
function bindPreviouslySubsidizedChange() {
    // 綁定 RadioButton change 事件
    $('[id*="rbPreviouslySubsidizedYes"], [id*="rbPreviouslySubsidizedNo"]').on('change', function() {
        handlePreviouslySubsidizedChange();
    });
}

/**
 * 處理「最近兩年曾獲本會補助」變更
 * - 選擇「是」時，顯示經費說明欄位
 * - 選擇「否」時，隱藏經費說明欄位
 */
function handlePreviouslySubsidizedChange() {
    // 檢查是否選擇「是」
    const isPreviouslySubsidizedYes = $('[id*="rbPreviouslySubsidizedYes"]').prop('checked');

    // 取得經費說明欄位的 row
    const fundingDescriptionRow = $('.funding-description-row');

    if (isPreviouslySubsidizedYes) {
        // 選擇「是」：顯示經費說明欄位
        fundingDescriptionRow.removeClass('d-none');
    } else {
        // 選擇「否」：隱藏經費說明欄位
        fundingDescriptionRow.addClass('d-none');
    }
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
    var  upPath= window.location.origin+window.AppRootPath+'/Service/CLB_Upload.ashx';
    // 發送 AJAX 請求到 CLB_Upload.ashx
    fetch(upPath, {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data && data.success) {
            // 直接更新 UI，不重新載入頁面
            updateFileStatusUIFromJS(fileCode, data.fileName, data.relativePath, data.fileId);

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
 * 根據 ID 刪除檔案
 * @param {number} fileId - 檔案 ID
 * @param {string} fileCode - 檔案代碼
 */
function deleteFileById(fileId, fileCode) {
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
            performFileDeleteById(fileId, fileCode);
        }
    });
}

/**
 * 執行根據 ID 刪除檔案
 * @param {number} fileId - 檔案 ID
 * @param {string} fileCode - 檔案代碼
 */
function performFileDeleteById(fileId, fileCode) {
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
    formData.append('action', 'deleteById');
    formData.append('fileId', fileId);

    // 發送 AJAX 請求到 CLB_Upload.ashx
    var deletePath = window.location.origin + window.AppRootPath + '/Service/CLB_Upload.ashx';
    fetch(deletePath, {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data && data.success) {
            // 移除該檔案標籤
            $(`.tag[data-file-id="${fileId}"]`).remove();

            // 檢查是否還有檔案
            const statusLabelId = `lblStatus${fileCode.replace('FILE_', '')}`;
            const filesPanelId = `pnlFiles${fileCode.replace('FILE_', '')}`;
            const filesPanel = $(`[id*="${filesPanelId}"]`);

            if (filesPanel.find('.tag').length === 0) {
                // 沒有檔案了，重置狀態
                $(`[id*="${statusLabelId}"]`).text('未上傳')
                    .removeClass('text-success').addClass('text-muted');
                filesPanel.hide();
            }

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

    // 取得網站根路徑，確保與上傳、刪除使用相同的路徑格式
    const downloadPath = window.location.origin + (window.AppRootPath || '') + '/Service/CLB_download.ashx';

    // 構建下載 URL
    let downloadUrl = downloadPath + '?action=template&type=' + templateType;
    if (projectID) {
        downloadUrl += '&projectID=' + encodeURIComponent(projectID);
    }

    // 開啟下載
    window.open(downloadUrl, '_blank');
}

/**
 * 根據 ID 下載已上傳的檔案
 * @param {number} fileId - 檔案 ID
 */
function downloadUploadedFileById(fileId) {
    // 取得網站根路徑
    const downloadPath = window.location.origin + (window.AppRootPath || '') + '/Service/CLB_download.ashx';

    // 構建下載 URL
    const downloadUrl = downloadPath + '?action=fileById&fileId=' + encodeURIComponent(fileId);

    // 開啟下載
    window.open(downloadUrl, '_blank');
}

/**
 * 從 JavaScript 端更新檔案狀態 UI
 * @param {string} fileCode - 檔案代碼
 * @param {string} fileName - 檔案名稱
 * @param {string} relativePath - 相對路徑
 * @param {number} fileId - 檔案 ID
 */
function updateFileStatusUIFromJS(fileCode, fileName, relativePath, fileId) {
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
            // 確保面板可見
            filesPanel.show().css('display', 'block');

            // FILE_CLB4 允許多檔，追加新檔案；其他檔案只允許單檔，清空後再添加
            if (fileCode !== 'FILE_CLB4') {
                filesPanel.empty();
            }

            // 建立新的檔案標籤
            const fileTagHtml = `
                <span class="tag tag-green-light" data-file-id="${fileId}">
                    <a class="tag-link" href="#" onclick="downloadUploadedFileById(${fileId}); return false;">
                        ${fileName}
                    </a>
                    <button type="button" class="tag-btn" onclick="deleteFileById(${fileId}, '${fileCode}')">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </span>
            `;

            filesPanel.append(fileTagHtml);

            console.log(`File added for ${fileCode}: ${fileName} (ID: ${fileId})`);
        } else {
            console.warn(`Files panel not found for ${fileCode}`);
        }
    } else {
        console.warn(`Status label not found for ${fileCode}`);
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
    // 驗證字數是否超過限制
    const charCountErrors = validateCharCount();
    if (charCountErrors) {
        let errorMessage = '以下欄位字數超過限制：<br><br>';
        charCountErrors.forEach(function(error) {
            errorMessage += `<strong>${error.fieldName}</strong>：已輸入 ${error.currentLength} 字，超過限制 ${error.maxLength} 字<br>`;
        });
        errorMessage += '<br>請修改後再進行暫存。';

        Swal.fire({
            icon: 'error',
            title: '字數超過限制',
            html: errorMessage,
            confirmButtonText: '確定'
        });
        return;
    }

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
    // 檢查是否超過補助金額限制
    const exceedsLimit = checkIfExceedsGrantLimit();

    if (exceedsLimit) {
        // 計算補助上限的萬元單位
        const grantLimitInWan = exceedsLimit.grantLimit / 10000;

        // 如果超過限制，先顯示提示訊息（僅供確認，不擋控）
        Swal.fire({
            icon: 'warning',
            title: '[提醒您]',
            html: `您申請的補助款經費總計<span class="fw-bold">${exceedsLimit.subsidyFunds.toLocaleString()}</span>元<br>` +
                  `已超過 ${exceedsLimit.targetName} 補助原則上限:<span class="fw-bold">${grantLimitInWan.toLocaleString()}</span>萬元<br><br>` +
                  `繼續申請請按 【確定】<br>` +
                  `返回調整請按 【取消】`,
            showCancelButton: true,
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            reverseButtons: true
        }).then((result) => {
            if (result.isConfirmed) {
                // 使用者確認後，執行儲存
                performSaveAndNext();
            }
        });
    } else {
        // 未超過限制，直接執行儲存
        performSaveAndNext();
    }
}

/**
 * 檢查是否超過補助金額限制
 * @returns {Object|null} 如果超過限制，回傳相關資訊；否則回傳 null
 */
function checkIfExceedsGrantLimit() {
    // 檢查是否已載入補助額度限制資料
    if (!window.grantLimitData) {
        return null;
    }

    // 取得當前選擇的補助類型
    let targetTypeID = '';
    let targetName = '';

    if ($('[id*="rbSubsidyTypeCreate"]').prop('checked')) {
        targetTypeID = 'CLB1';
    } else if ($('[id*="rbSubsidyTypeOperation"]').prop('checked')) {
        targetTypeID = 'CLB2';
    } else if ($('[id*="rbSubsidyTypeActivity"]').prop('checked')) {
        targetTypeID = 'CLB3';
    }

    // 如果沒有選擇補助類型，不檢查
    if (!targetTypeID) {
        return null;
    }

    // 取得該補助類型的限制資料
    const limitData = window.grantLimitData[targetTypeID];
    if (!limitData || !limitData.GrantLimit) {
        return null;
    }

    // 取得申請補助金額（使用 txtApplyAmount，即申請海委會補助金額）
    const applyAmountText = $('#txtApplyAmount').val() || '';
    const subsidyFunds = parseFloat(applyAmountText.replace(/,/g, '')) || 0;

    // 比較金額
    if (subsidyFunds > limitData.GrantLimit) {
        return {
            subsidyFunds: subsidyFunds,
            grantLimit: limitData.GrantLimit,
            targetName: limitData.TargetName
        };
    }

    return null;
}

/**
 * 執行儲存並下一步的實際操作
 */
function performSaveAndNext() {
    // 驗證字數是否超過限制
    const charCountErrors = validateCharCount();
    if (charCountErrors) {
        let errorMessage = '以下欄位字數超過限制：<br><br>';
        charCountErrors.forEach(function(error) {
            errorMessage += `<strong>${error.fieldName}</strong>：已輸入 ${error.currentLength} 字，超過限制 ${error.maxLength} 字<br>`;
        });
        errorMessage += '<br>請修改後再進行儲存。';

        Swal.fire({
            icon: 'error',
            title: '字數超過限制',
            html: errorMessage,
            confirmButtonText: '確定'
        });
        return;
    }

    // 先驗證其他補助明細資料
    const validationResult = validateOtherSubsidyData();
    if (!validationResult.isValid) {
        Swal.fire({
            icon: 'error',
            title: '請完整填寫其他補助明細',
            html: validationResult.errorMessages.join('<br>'),
            confirmButtonText: '確定'
        });
        return;
    }

    // 驗證經費預算規劃資料
    const budgetValidationResult = validateBudgetPlanData();
    if (!budgetValidationResult.isValid) {
        Swal.fire({
            icon: 'error',
            title: '請完整填寫經費預算規劃',
            html: budgetValidationResult.errorMessages.join('<br>'),
            confirmButtonText: '確定'
        });
        return;
    }

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
                html: data.message || '未知錯誤，請稍後再試',
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

    // 檢查補助類型
    const isPublicActivity = subsidyType === 'Public';
    const isOperation = subsidyType === 'Admin';

    formData.append('schoolName', $('[id*="txtSchoolName"]').val() || '');
    formData.append('clubName', $('[id*="txtClubName"]').val() || '');

    // 只有在「社務補助」或「公共活動費」時才收集成立日期
    if (isOperation || isPublicActivity) {
        const creationDate = $('[id*="txtCreationDate"]').data('gregorian-date');
        if (creationDate) formData.append('creationDate', creationDate);
    }

    // 只有在「公共活動費」時才收集這些欄位
    if (isPublicActivity) {
        formData.append('schoolIDNumber', $('[id*="txtSchoolIDNumber"]').val() || '');
        formData.append('address', $('[id*="txtAddress"]').val() || '');
    }

    // 日期欄位 - 計畫執行期間（所有類型都需要）
    const startDate = $('[id*="txtStartDate"]').data('gregorian-date');
    if (startDate) formData.append('startDate', startDate);

    const endDate = $('[id*="txtEndDate"]').data('gregorian-date');
    if (endDate) formData.append('endDate', endDate);

    // 計畫資訊
    formData.append('purpose', $('[id*="txtPurpose"]').val() || '');
    formData.append('planContent', $('[id*="txtPlanContent"]').val() || '');
    formData.append('preBenefits', $('[id*="txtPreBenefits"]').val() || '');
    formData.append('estimatedPeople', $('[id*="txtEstimatedPeople"]').val() || '');

    // 只有在「公共活動費」時才收集這些欄位
    if (isPublicActivity) {
        formData.append('planLocation', $('[id*="txtPlanLocation"]').val() || '');
        formData.append('emergencyPlan', $('[id*="txtEmergencyPlan"]').val() || '');
    }

    // 經費資料 - 使用 ID 取得
    // A: 申請海委會補助／合作金額(元)
    const applyAmountText = $('#txtApplyAmount').val() || '';
    const applyAmount = parseInt(applyAmountText.replace(/,/g, '')) || null;
    if (applyAmount !== null) {
        formData.append('applyAmount', applyAmount);
    }

    // B: 申請單位自籌款(元)
    const selfAmountText = $('#txtSelfAmount').val() || '';
    if (selfAmountText !== '') {
        const selfAmount = parseInt(selfAmountText.replace(/,/g, ''));
        if (!isNaN(selfAmount)) {
            formData.append('selfAmount', selfAmount);
        }
    }

    // C: 其他機關補助／合作總金額(元) - 從顯示的文字取得（已經由 calculateOtherSubsidyTotal 計算好）
    const otherAmountText = $('#lblOtherAmount').text().replace(/,/g, '');
    const otherAmount = parseInt(otherAmountText) || null;
    if (otherAmount !== null) {
        formData.append('otherAmount', otherAmount);
    }

    // 收集「最近兩年曾獲本會補助」的值
    const isPreviouslySubsidized = $('[id*="rbPreviouslySubsidizedYes"]').prop('checked');
    formData.append('isPreviouslySubsidized', isPreviouslySubsidized);

    // 收集其他補助明細資料（OFS_CLB_Other_Subsidy 表）
    const otherSubsidyData = collectOtherSubsidyData();
    formData.append('otherSubsidyData', JSON.stringify(otherSubsidyData));

    // 收集經費預算規劃資料（OFS_CLB_Budget_Plan 表）
    const budgetPlanData = collectBudgetPlanData();
    formData.append('budgetPlanData', JSON.stringify(budgetPlanData));

    // 收集經費說明資料（OFS_CLB_Received_Subsidy 表）
    const receivedSubsidyData = collectFundingDescriptionData();
    formData.append('receivedSubsidyData', JSON.stringify(receivedSubsidyData));

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

// ========================================
// 經費資訊管理功能
// ========================================

/**
 * 初始化經費資訊管理功能
 */
function initializeFundsManagement() {
    // 綁定新增按鈕事件
    bindAddSubsidyRowEvent();

    // 綁定刪除按鈕事件（使用事件委派）
    bindDeleteSubsidyRowEvents();

    // 綁定金額輸入事件（用於自動計算）
    bindSubsidyAmountInputEvents();

    // 初始計算總金額和比例
    calculateOtherSubsidyTotal();
}

/**
 * 綁定新增其他補助資料按鈕事件
 */
function bindAddSubsidyRowEvent() {
    // 使用 ID 綁定新增按鈕
    $('#btnAddOtherSubsidy').off('click').on('click', function(e) {
        e.preventDefault();
        addOtherSubsidyRow();
    });
}

/**
 * 綁定刪除按鈕事件（使用事件委派）
 */
function bindDeleteSubsidyRowEvents() {
    // 使用事件委派，綁定到 document
    $(document).off('click', '.btn-teal-dark:has(.fa-trash-alt)').on('click', '.btn-teal-dark:has(.fa-trash-alt)', function(e) {
        e.preventDefault();
        const $row = $(this).closest('tr');
        deleteOtherSubsidyRow($row);
    });
}

/**
 * 綁定其他補助金額輸入事件
 */
function bindSubsidyAmountInputEvents() {
    // 使用事件委派綁定其他補助表格的金額輸入欄位
    $(document).off('input', '#tbodyOtherSubsidy input[type="text"][style*="right"]').on('input', '#tbodyOtherSubsidy input[type="text"][style*="right"]', function() {
        calculateOtherSubsidyTotal();
    });

    // 綁定申請海委會補助和自籌款輸入事件（使用 ID）
    $('#txtApplyAmount, #txtSelfAmount').off('input').on('input', function() {
        calculateMainFundsTotal();
    });
}

/**
 * 新增一行其他補助資料
 */
function addOtherSubsidyRow() {
    // 使用 ID 選擇 tbody
    const $tbody = $('#tbodyOtherSubsidy');

    // 計算新的序號
    const currentRowCount = $tbody.find('tr').length;
    const newRowNumber = currentRowCount + 1;

    // 建立新的一行 HTML
    const newRowHtml = `
        <tr data-subsidy-id="0">
            <td>${newRowNumber}</td>
            <td>
                <div class="input-group">
                    <input class="form-control" placeholder="請輸入" type="text">
                </div>
            </td>
            <td>
                <input class="form-control" placeholder="請輸入" type="text" style="text-align: right;">
            </td>
            <td class="text-end">0%</td>
            <td>
                <textarea class="form-control textarea-auto-resize" placeholder="請輸入" rows="1"></textarea>
            </td>
            <td>
                <div class="d-flex gap-2">
                    <button class="btn btn-sm btn-teal-dark m-0" type="button">
                        <i class="fas fa-trash-alt"></i>
                    </button>
                </div>
            </td>
        </tr>
    `;

    // 將新行加入到 tbody
    $tbody.append(newRowHtml);

    // 重新計算總金額和比例
    calculateOtherSubsidyTotal();
}

/**
 * 刪除一行其他補助資料
 * @param {jQuery} $row - 要刪除的行元素
 */
function deleteOtherSubsidyRow($row) {
    // 確認刪除
    Swal.fire({
        icon: 'warning',
        title: '確認刪除',
        text: '確定要刪除這筆其他補助資料嗎？',
        showCancelButton: true,
        confirmButtonText: '確定刪除',
        cancelButtonText: '取消',
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            // 執行刪除
            performDeleteOtherSubsidyRow($row);
        }
    });
}

/**
 * 執行刪除其他補助資料行
 * @param {jQuery} $row - 要刪除的行元素
 */
function performDeleteOtherSubsidyRow($row) {
    // 移除該行
    $row.remove();

    // 重新編號所有行
    renumberOtherSubsidyRows();

    // 重新計算總金額和比例
    calculateOtherSubsidyTotal();

    // 顯示刪除成功訊息
    Swal.fire({
        icon: 'success',
        title: '刪除成功',
        text: '已成功刪除該筆其他補助資料',
        timer: 1500,
        showConfirmButton: false
    });
}

/**
 * 重新編號其他補助資料行
 */
function renumberOtherSubsidyRows() {
    // 使用 ID 選擇 tbody
    const $tbody = $('#tbodyOtherSubsidy');

    $tbody.find('tr').each(function(index) {
        $(this).find('td:first').text(index + 1);
    });
}

/**
 * 計算其他機關補助總金額和比例
 */
function calculateOtherSubsidyTotal() {
    // 使用 ID 選擇 tbody
    const $tbody = $('#tbodyOtherSubsidy');

    let totalOtherSubsidy = 0;
    const rowData = [];

    // 收集所有金額並計算總和
    $tbody.find('tr').each(function() {
        const $row = $(this);
        const $amountInput = $row.find('td').eq(2).find('input'); // 第3個 td 的 input
        const amountText = $amountInput.val() || '';
        const amount = parseFloat(amountText.replace(/,/g, '')) || 0;

        rowData.push({
            $row: $row,
            amount: amount
        });

        totalOtherSubsidy += amount;
    });

    // 更新每一行的比例
    rowData.forEach(item => {
        let ratio = 0;
        if (totalOtherSubsidy > 0) {
            ratio = (item.amount / totalOtherSubsidy) * 100;
        }
        const $ratioCell = item.$row.find('td').eq(3); // 第4個 td
        $ratioCell.text(ratio.toFixed(1) + '%');
    });

    // 更新第一個表格中「其他機關補助／合作總金額(元) (C)」的顯示
    updateOtherSubsidyTotalDisplay(totalOtherSubsidy);

    // 重新計算計畫總經費
    calculateMainFundsTotal();
}

/**
 * 更新其他機關補助總金額顯示
 * @param {number} total - 總金額
 */
function updateOtherSubsidyTotalDisplay(total) {
    // 使用 ID 更新顯示
    $('#lblOtherAmount').text(total.toLocaleString());
}

/**
 * 計算計畫總經費 (A + B + C)
 */
function calculateMainFundsTotal() {
    // 使用 ID 取得各項金額
    // A: 申請海委會補助／合作金額
    const applyAmountText = $('#txtApplyAmount').val() || '';
    const applyAmount = parseFloat(applyAmountText.replace(/,/g, '')) || 0;

    // B: 申請單位自籌款
    const selfAmountText = $('#txtSelfAmount').val() || '';
    const selfAmount = parseFloat(selfAmountText.replace(/,/g, '')) || 0;

    // C: 其他機關補助／合作總金額（從顯示的文字取得）
    const otherAmountText = $('#lblOtherAmount').text().replace(/,/g, '');
    const otherAmount = parseFloat(otherAmountText) || 0;

    // 計算總經費
    const totalFunds = applyAmount + selfAmount + otherAmount;

    // 使用 ID 更新計畫總經費顯示
    $('#lblTotalAmount').text(totalFunds.toLocaleString());
}

/**
 * 收集所有其他補助資料（供表單提交時使用）
 * @returns {Array} 其他補助資料陣列
 */
function collectOtherSubsidyData() {
    const subsidyData = [];
    // 使用 ID 選擇 tbody
    const $tbody = $('#tbodyOtherSubsidy');

    $tbody.find('tr').each(function() {
        const $row = $(this);
        const id = $row.data('subsidy-id') || 0;

        // 取得各欄位值
        const unit = $row.find('td').eq(1).find('input').val() || ''; // 單位名稱
        const amountText = $row.find('td').eq(2).find('input').val() || ''; // 金額
        const amount = parseInt(amountText.replace(/,/g, '')) || 0;
        const content = $row.find('td').eq(4).find('textarea').val() || ''; // 申請合作項目

        // 只收集有填寫單位名稱或金額的資料
        if (unit.trim() !== '' || amount > 0) {
            subsidyData.push({
                ID: id,
                Unit: unit,
                Amount: amount,
                Content: content
            });
        }
    });

    return subsidyData;
}

/**
 * 驗證其他補助資料
 * @returns {Object} 驗證結果 { isValid: boolean, errorMessages: Array }
 */
function validateOtherSubsidyData() {
    const $tbody = $('#tbodyOtherSubsidy');
    const $rows = $tbody.find('tr');
    const errorMessages = [];

    // 如果沒有任何列，則不需要驗證
    if ($rows.length === 0) {
        return {
            isValid: true,
            errorMessages: []
        };
    }

    // 驗證每一列
    $rows.each(function(index) {
        const $row = $(this);
        const rowNumber = index + 1;

        // 取得各欄位值
        const unit = $row.find('td').eq(1).find('input').val() || '';
        const amountText = $row.find('td').eq(2).find('input').val() || '';
        const amount = parseInt(amountText.replace(/,/g, '')) || 0;
        const content = $row.find('td').eq(4).find('textarea').val() || '';

        // 檢查欄位是否都有填寫
        const hasUnit = unit.trim() !== '';
        const hasAmount = amount > 0;
        const hasContent = content.trim() !== '';

        // 如果任何一個欄位有填寫，則其他欄位也必須填寫
        if (hasUnit || hasAmount || hasContent) {
            if (!hasUnit) {
                errorMessages.push(`其他機關補助明細第 ${rowNumber} 列：請輸入單位名稱`);
            }
            if (!hasAmount) {
                errorMessages.push(`其他機關補助明細第 ${rowNumber} 列：請輸入申請／分攤補助金額`);
            }
            if (!hasContent) {
                errorMessages.push(`其他機關補助明細第 ${rowNumber} 列：請輸入申請合作項目`);
            }
        }
    });

    return {
        isValid: errorMessages.length === 0,
        errorMessages: errorMessages
    };
}

/**
 * 驗證經費預算規劃資料
 * @returns {Object} 驗證結果 { isValid: boolean, errorMessages: Array }
 */
function validateBudgetPlanData() {
    const $tbody = $('#tbodyBudgetPlan');
    const $rows = $tbody.find('tr');
    const errorMessages = [];

    // 如果沒有任何列，則不需要驗證
    if ($rows.length === 0) {
        return {
            isValid: true,
            errorMessages: []
        };
    }

    // 驗證每一列
    $rows.each(function(index) {
        const $row = $(this);
        const rowNumber = index + 1;

        // 取得各欄位值
        const title = $row.find('td').eq(0).find('input').val() || '';
        const amountText = $row.find('td').eq(1).find('input').val() || '';
        const amount = parseInt(amountText.replace(/,/g, '')) || 0;
        const otherAmountText = $row.find('td').eq(2).find('input').val() || '';
        const otherAmount = parseInt(otherAmountText.replace(/,/g, '')) || 0;
        const description = $row.find('td').eq(4).find('textarea').val() || '';

        // 檢查欄位是否都有填寫
        const hasTitle = title.trim() !== '';
        const hasAmount = amountText.trim() !== '';
        const hasOtherAmount = otherAmountText.trim() !== '';
        const hasDescription = description.trim() !== '';

        // 如果任何一個欄位有填寫，則所有欄位都必須填寫
        if (hasTitle || hasAmount || hasOtherAmount || hasDescription) {
            if (!hasTitle) {
                errorMessages.push(`經費預算規劃第 ${rowNumber} 列：請輸入預算項目`);
            }
            if (!hasAmount) {
                errorMessages.push(`經費預算規劃第 ${rowNumber} 列：請輸入預算金額(海洋委員會經費)`);
            }
            if (!hasOtherAmount) {
                errorMessages.push(`經費預算規劃第 ${rowNumber} 列：請輸入預算金額(其他配合經費)`);
            }
            if (!hasDescription) {
                errorMessages.push(`經費預算規劃第 ${rowNumber} 列：請輸入計算方式及說明`);
            }

            // 檢查兩個金額欄位不可同時為 0
            if (hasAmount && hasOtherAmount && amount === 0 && otherAmount === 0) {
                errorMessages.push(`經費預算規劃第 ${rowNumber} 列：海洋委員會經費與其他配合經費不可同時為 0`);
            }
        }
    });

    return {
        isValid: errorMessages.length === 0,
        errorMessages: errorMessages
    };
}

// ========================================
// 經費預算規劃管理功能
// ========================================

/**
 * 初始化經費預算規劃管理功能
 */
function initializeBudgetPlanManagement() {
    // 綁定新增按鈕事件
    bindAddBudgetPlanRowEvent();

    // 綁定刪除按鈕事件（使用事件委派）
    bindDeleteBudgetPlanRowEvents();

    // 綁定金額輸入事件（用於自動計算小計）
    bindBudgetPlanAmountInputEvents();
}

/**
 * 綁定新增經費預算規劃按鈕事件
 */
function bindAddBudgetPlanRowEvent() {
    $('#btnAddBudgetPlan').off('click').on('click', function(e) {
        e.preventDefault();
        addBudgetPlanRow();
    });
}

/**
 * 綁定刪除按鈕事件（使用事件委派）
 */
function bindDeleteBudgetPlanRowEvents() {
    // 使用 ID 選擇器配合事件委派
    $('#tbodyBudgetPlan').off('click', '.btn-teal-dark:has(.fa-trash-alt)').on('click', '.btn-teal-dark:has(.fa-trash-alt)', function(e) {
        e.preventDefault();
        const $row = $(this).closest('tr');
        deleteBudgetPlanRow($row);
    });
}

/**
 * 綁定經費預算規劃金額輸入事件
 */
function bindBudgetPlanAmountInputEvents() {
    // 使用 ID 選擇器配合事件委派綁定金額輸入欄位
    $('#tbodyBudgetPlan').off('input', 'input[type="text"][style*="right"]').on('input', 'input[type="text"][style*="right"]', function() {
        const $row = $(this).closest('tr');
        calculateBudgetPlanSubtotal($row);
    });
}

/**
 * 新增一列經費預算規劃
 */
function addBudgetPlanRow() {
    const $tbody = $('#tbodyBudgetPlan');

    const rowHtml = `
        <tr data-budget-id="0">
            <td>
                <div class="input-group">
                    <input class="form-control" placeholder="請輸入" type="text">
                </div>
            </td>
            <td>
                <input class="form-control" placeholder="請輸入" type="text" style="text-align: right;">
            </td>
            <td>
                <input class="form-control" placeholder="請輸入" type="text" style="text-align: right;">
            </td>
            <td class="text-end budget-subtotal">0</td>
            <td>
                <textarea class="form-control textarea-auto-resize" placeholder="請輸入" rows="1"></textarea>
            </td>
            <td>
                <div class="d-flex gap-2">
                    <button class="btn btn-sm btn-teal-dark m-0" type="button"><i class="fas fa-trash-alt"></i></button>
                </div>
            </td>
        </tr>
    `;

    $tbody.append(rowHtml);
}

/**
 * 刪除一列經費預算規劃
 * @param {jQuery} $row - 要刪除的列
 */
function deleteBudgetPlanRow($row) {
    Swal.fire({
        title: '確認刪除',
        text: '確定要刪除這筆預算規劃資料嗎？',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            $row.remove();
        }
    });
}

/**
 * 計算單列預算小計
 * @param {jQuery} $row - 要計算的列
 */
function calculateBudgetPlanSubtotal($row) {
    // 取得海洋委員會經費
    const amountText = $row.find('td').eq(1).find('input').val() || '';
    const amount = parseInt(amountText.replace(/,/g, '')) || 0;

    // 取得其他配合經費
    const otherAmountText = $row.find('td').eq(2).find('input').val() || '';
    const otherAmount = parseInt(otherAmountText.replace(/,/g, '')) || 0;

    // 計算小計
    const subtotal = amount + otherAmount;

    // 更新顯示
    $row.find('.budget-subtotal').text(subtotal.toLocaleString());
}

/**
 * 收集所有經費預算規劃資料
 * @returns {Array} 經費預算規劃資料陣列
 */
function collectBudgetPlanData() {
    const budgetPlanData = [];
    const $tbody = $('#tbodyBudgetPlan');

    $tbody.find('tr').each(function() {
        const $row = $(this);
        const id = $row.data('budget-id') || 0;

        // 取得各欄位值
        const title = $row.find('td').eq(0).find('input').val() || '';
        const amountText = $row.find('td').eq(1).find('input').val() || '';
        const amount = parseInt(amountText.replace(/,/g, '')) || 0;
        const otherAmountText = $row.find('td').eq(2).find('input').val() || '';
        const otherAmount = parseInt(otherAmountText.replace(/,/g, '')) || 0;
        const description = $row.find('td').eq(4).find('textarea').val() || '';

        // 只收集有填寫預算項目的資料
        if (title.trim() !== '' || amount > 0 || otherAmount > 0) {
            budgetPlanData.push({
                ID: id,
                Title: title,
                Amount: amount,
                OtherAmount: otherAmount,
                Description: description
            });
        }
    });

    return budgetPlanData;
}

/**
 * 載入經費預算規劃資料到表格
 * @param {Array} budgetPlanData - 經費預算規劃資料陣列
 */
function loadBudgetPlanData(budgetPlanData) {
    if (!budgetPlanData || budgetPlanData.length === 0) {
        return;
    }

    const $tbody = $('#tbodyBudgetPlan');

    // 清空現有資料
    $tbody.empty();

    // 載入每一筆資料
    budgetPlanData.forEach((item) => {
        const subtotal = (item.Amount || 0) + (item.OtherAmount || 0);
        const rowHtml = `
            <tr data-budget-id="${item.ID || 0}">
                <td>
                    <div class="input-group">
                        <input class="form-control" placeholder="請輸入" type="text" value="${escapeHtml(item.Title || '')}">
                    </div>
                </td>
                <td>
                    <input class="form-control" placeholder="請輸入" type="text" style="text-align: right;" value="${item.Amount != null ? item.Amount.toLocaleString() : ''}">
                </td>
                <td>
                    <input class="form-control" placeholder="請輸入" type="text" style="text-align: right;" value="${item.OtherAmount != null ? item.OtherAmount.toLocaleString() : ''}">
                </td>
                <td class="text-end budget-subtotal">${subtotal.toLocaleString()}</td>
                <td>
                    <textarea class="form-control textarea-auto-resize" placeholder="請輸入" rows="1">${escapeHtml(item.Description || '')}</textarea>
                </td>
                <td>
                    <div class="d-flex gap-2">
                        <button class="btn btn-sm btn-teal-dark m-0" type="button"><i class="fas fa-trash-alt"></i></button>
                    </div>
                </td>
            </tr>
        `;
        $tbody.append(rowHtml);
    });
}

/**
 * 載入基本經費資料到表單
 * @param {Object} fundsData - 經費資料物件
 * @param {string} fundsData.applyAmount - 申請海委會補助／合作金額(元)
 * @param {string} fundsData.selfAmount - 申請單位自籌款(元)
 * @param {boolean} fundsData.isPreviouslySubsidized - 最近兩年曾獲本會補助
 */
function loadFundsData(fundsData) {
    if (!fundsData) {
        return;
    }

    // 載入申請海委會補助／合作金額(元) (A)
    if (fundsData.applyAmount) {
        $('#txtApplyAmount').val(fundsData.applyAmount);
    }

    // 載入申請單位自籌款(元) (B)
    if (fundsData.selfAmount) {
        $('#txtSelfAmount').val(fundsData.selfAmount);
    }

    // 載入「最近兩年曾獲本會補助」的值
    if (typeof fundsData.isPreviouslySubsidized !== 'undefined') {
        if (fundsData.isPreviouslySubsidized) {
            $('[id*="rbPreviouslySubsidizedYes"]').prop('checked', true);
        } else {
            $('[id*="rbPreviouslySubsidizedNo"]').prop('checked', true);
        }
        // 觸發變更事件，顯示或隱藏經費說明欄位
        handlePreviouslySubsidizedChange();
    }

    // 重新計算總金額（會自動計算其他補助總金額和全部經費總計）
    calculateMainFundsTotal();
}

/**
 * 載入其他補助資料到表格
 * @param {Array} subsidyData - 其他補助資料陣列
 */
function loadOtherSubsidyData(subsidyData) {
    if (!subsidyData || subsidyData.length === 0) {
        return;
    }

    // 使用 ID 選擇 tbody
    const $tbody = $('#tbodyOtherSubsidy');

    // 清空現有資料
    $tbody.empty();

    // 載入每一筆資料
    subsidyData.forEach((item, index) => {
        const rowNumber = index + 1;
        const rowHtml = `
            <tr data-subsidy-id="${item.ID || 0}">
                <td>${rowNumber}</td>
                <td>
                    <div class="input-group">
                        <input class="form-control" placeholder="請輸入" type="text" value="${escapeHtml(item.Unit || '')}">
                    </div>
                </td>
                <td>
                    <input class="form-control" placeholder="請輸入" type="text" style="text-align: right;" value="${item.Amount || ''}">
                </td>
                <td class="text-end">0%</td>
                <td>
                    <textarea class="form-control textarea-auto-resize" placeholder="請輸入" rows="1">${escapeHtml(item.Content || '')}</textarea>
                </td>
                <td>
                    <div class="d-flex gap-2">
                        <button class="btn btn-sm btn-teal-dark m-0" type="button">
                            <i class="fas fa-trash-alt"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
        $tbody.append(rowHtml);
    });

    // 重新計算總金額和比例
    calculateOtherSubsidyTotal();
}

/**
 * HTML 轉義函數（防止 XSS）
 * @param {string} text - 要轉義的文字
 * @returns {string} 轉義後的文字
 */
function escapeHtml(text) {
    if (!text) return '';
    const map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return String(text).replace(/[&<>"']/g, function(m) { return map[m]; });
}

// ========================================
// 經費說明管理功能
// ========================================

/**
 * 初始化經費說明管理功能
 */
function initializeFundingDescriptionManagement() {
    // 綁定新增按鈕事件
    bindAddFundingDescriptionRowEvent();

    // 綁定刪除按鈕事件（使用事件委派）
    bindDeleteFundingDescriptionRowEvents();
}

/**
 * 綁定新增經費說明按鈕事件
 */
function bindAddFundingDescriptionRowEvent() {
    $('#btnAddFundingDescription').off('click').on('click', function(e) {
        e.preventDefault();
        addFundingDescriptionRow();
    });
}

/**
 * 綁定刪除按鈕事件（使用事件委派）
 */
function bindDeleteFundingDescriptionRowEvents() {
    // 使用 ID 選擇器配合事件委派
    $('#tbodyFundingDescription').off('click', '.btn-teal-dark:has(.fa-trash-alt)').on('click', '.btn-teal-dark:has(.fa-trash-alt)', function(e) {
        e.preventDefault();
        const $row = $(this).closest('tr');
        deleteFundingDescriptionRow($row);
    });
}

/**
 * 新增一列經費說明
 */
function addFundingDescriptionRow() {
    const $tbody = $('#tbodyFundingDescription');

    const rowHtml = `
        <tr data-description-id="0">
            <td>
                <input class="form-control" placeholder="請輸入計畫名稱" type="text">
            </td>
            <td>
                <input class="form-control" placeholder="請輸入金額" type="text" style="text-align: right;">
            </td>
            <td>
                <div class="d-flex gap-2">
                    <button class="btn btn-sm btn-teal-dark m-0" type="button"><i class="fas fa-trash-alt"></i></button>
                </div>
            </td>
        </tr>
    `;

    $tbody.append(rowHtml);
}

/**
 * 刪除一列經費說明
 * @param {jQuery} $row - 要刪除的列
 */
function deleteFundingDescriptionRow($row) {
    Swal.fire({
        title: '確認刪除',
        text: '確定要刪除這筆經費說明資料嗎？',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            $row.remove();

            Swal.fire({
                icon: 'success',
                title: '刪除成功',
                text: '已成功刪除該筆經費說明資料',
                timer: 1500,
                showConfirmButton: false
            });
        }
    });
}

/**
 * 收集所有經費說明資料
 * @returns {Array} 經費說明資料陣列
 */
function collectFundingDescriptionData() {
    const descriptionData = [];
    const $tbody = $('#tbodyFundingDescription');

    $tbody.find('tr').each(function() {
        const $row = $(this);
        const id = $row.data('description-id') || 0;

        // 取得各欄位值
        const projectName = $row.find('td').eq(0).find('input').val() || '';
        const amountText = $row.find('td').eq(1).find('input').val() || '';
        const amount = parseInt(amountText.replace(/,/g, '')) || 0;

        // 只收集有填寫計畫名稱或金額的資料
        if (projectName.trim() !== '' || amount > 0) {
            descriptionData.push({
                ID: id,
                ProjectName: projectName,
                Amount: amount
            });
        }
    });

    return descriptionData;
}

/**
 * 驗證經費說明資料
 * @returns {Object} 驗證結果 { isValid: boolean, errorMessages: Array }
 */
function validateFundingDescriptionData() {
    const $tbody = $('#tbodyFundingDescription');
    const $rows = $tbody.find('tr');
    const errorMessages = [];

    // 如果沒有任何列，則不需要驗證
    if ($rows.length === 0) {
        return {
            isValid: true,
            errorMessages: []
        };
    }

    // 驗證每一列
    $rows.each(function(index) {
        const $row = $(this);
        const rowNumber = index + 1;

        // 取得各欄位值
        const projectName = $row.find('td').eq(0).find('input').val() || '';
        const amountText = $row.find('td').eq(1).find('input').val() || '';
        const amount = parseInt(amountText.replace(/,/g, '')) || 0;

        // 檢查欄位是否都有填寫
        const hasProjectName = projectName.trim() !== '';
        const hasAmount = amount > 0;

        // 如果任何一個欄位有填寫，則所有欄位都必須填寫
        if (hasProjectName || hasAmount) {
            if (!hasProjectName) {
                errorMessages.push(`經費說明第 ${rowNumber} 列：請輸入計畫名稱`);
            }
            if (!hasAmount) {
                errorMessages.push(`經費說明第 ${rowNumber} 列：請輸入海委會補助經費`);
            }
        }
    });

    return {
        isValid: errorMessages.length === 0,
        errorMessages: errorMessages
    };
}

/**
 * 載入經費說明資料到表格
 * @param {Array} descriptionData - 經費說明資料陣列
 */
function loadFundingDescriptionData(descriptionData) {
    if (!descriptionData || descriptionData.length === 0) {
        return;
    }

    const $tbody = $('#tbodyFundingDescription');

    // 清空現有資料
    $tbody.empty();

    // 載入每一筆資料
    descriptionData.forEach((item) => {
        const rowHtml = `
            <tr data-description-id="${item.ID || 0}">
                <td>
                    <input class="form-control" placeholder="請輸入計畫名稱" type="text" value="${escapeHtml(item.ProjectName || '')}">
                </td>
                <td>
                    <input class="form-control" placeholder="請輸入金額" type="text" style="text-align: right;" value="${item.Amount ? item.Amount.toLocaleString() : ''}">
                </td>
                <td>
                    <div class="d-flex gap-2">
                        <button class="btn btn-sm btn-teal-dark m-0" type="button"><i class="fas fa-trash-alt"></i></button>
                    </div>
                </td>
            </tr>
        `;
        $tbody.append(rowHtml);
    });
}

// 將函數公開給全域範圍以供 HTML 中的 onclick 事件使用
window.handleFileUpload = handleFileUpload;
window.deleteFileById = deleteFileById;
window.downloadTemplate = downloadTemplate;
window.downloadUploadedFileById = downloadUploadedFileById;
window.updateFileStatusUIFromJS = updateFileStatusUIFromJS;
window.submitApplicationFinal = submitApplicationFinal;
window.navigateToStep = navigateToStep;
window.switchTab = switchTab;
window.initializeTabSystem = initializeTabSystem;
window.handleTempSave = handleTempSave;
window.handleSaveAndNext = handleSaveAndNext;
window.handleSubmitApplication = handleSubmitApplication;

// 公開經費資訊管理函數
window.initializeFundsManagement = initializeFundsManagement;
window.addOtherSubsidyRow = addOtherSubsidyRow;
window.deleteOtherSubsidyRow = deleteOtherSubsidyRow;
window.collectOtherSubsidyData = collectOtherSubsidyData;
window.validateOtherSubsidyData = validateOtherSubsidyData;
window.loadFundsData = loadFundsData;
window.loadOtherSubsidyData = loadOtherSubsidyData;
window.calculateOtherSubsidyTotal = calculateOtherSubsidyTotal;
window.calculateMainFundsTotal = calculateMainFundsTotal;
window.initializeAmountInputValidation = initializeAmountInputValidation;
window.validateAndFormatAmountInput = validateAndFormatAmountInput;

// 公開經費預算規劃管理函數
window.initializeBudgetPlanManagement = initializeBudgetPlanManagement;
window.addBudgetPlanRow = addBudgetPlanRow;
window.deleteBudgetPlanRow = deleteBudgetPlanRow;
window.calculateBudgetPlanSubtotal = calculateBudgetPlanSubtotal;
window.collectBudgetPlanData = collectBudgetPlanData;
window.loadBudgetPlanData = loadBudgetPlanData;
window.validateBudgetPlanData = validateBudgetPlanData;

// 公開經費說明管理函數
window.initializeFundingDescriptionManagement = initializeFundingDescriptionManagement;
window.addFundingDescriptionRow = addFundingDescriptionRow;
window.deleteFundingDescriptionRow = deleteFundingDescriptionRow;
window.collectFundingDescriptionData = collectFundingDescriptionData;
window.validateFundingDescriptionData = validateFundingDescriptionData;
window.loadFundingDescriptionData = loadFundingDescriptionData;

/**
 * 初始化字數計算功能
 */
function initializeCharCounter() {
    // 定義需要計算字數的欄位
    var charCounterFields = [
        { id: 'txtPurpose', maxLength: 500 },
        { id: 'txtPlanContent', maxLength: 500 },
        { id: 'txtPreBenefits', maxLength: 500 }
    ];

    // 為每個欄位綁定字數計算事件
    charCounterFields.forEach(function(field) {
        var $input = $('#' + field.id);
        var $counter = $('#' + field.id + '_count');

        if ($input.length && $counter.length) {
            // 初始化時更新字數
            updateCharCount($input, $counter);

            // 綁定 input 事件
            $input.on('input', function() {
                updateCharCount($input, $counter);
            });

            // 綁定 keyup 事件（處理某些特殊情況）
            $input.on('keyup', function() {
                updateCharCount($input, $counter);
            });
        }
    });
}

/**
 * 更新字數顯示
 * @param {jQuery} $input - 輸入框 jQuery 對象
 * @param {jQuery} $counter - 字數顯示 jQuery 對象
 */
function updateCharCount($input, $counter) {
    var currentLength = $input.val().length;
    $counter.text(currentLength);

    // 如果超過限制，改變顏色提示
    if (currentLength >= 500) {
        $counter.parent().addClass('text-danger').removeClass('text-muted');
    } else {
        $counter.parent().addClass('text-muted').removeClass('text-danger');
    }
}

/**
 * 驗證字數是否超過限制
 * @returns {Object|null} 如果有欄位超過限制，回傳錯誤資訊；否則回傳 null
 */
function validateCharCount() {
    var errors = [];
    var charCounterFields = [
        { id: 'txtPurpose', name: '目的', maxLength: 500 },
        { id: 'txtPlanContent', name: '計畫內容', maxLength: 500 },
        { id: 'txtPreBenefits', name: '預期效益', maxLength: 500 }
    ];

    charCounterFields.forEach(function(field) {
        var $input = $('#' + field.id);
        if ($input.length) {
            var currentLength = $input.val().length;
            if (currentLength > field.maxLength) {
                errors.push({
                    fieldName: field.name,
                    currentLength: currentLength,
                    maxLength: field.maxLength
                });
            }
        }
    });

    if (errors.length > 0) {
        return errors;
    }
    return null;
}

// 公開字數計算函數
window.initializeCharCounter = initializeCharCounter;
window.validateCharCount = validateCharCount;