$(document).ready(function() {
    // 初始化頁面
    initializePage();
    
    // 綁定事件
    bindEvents();
    
    // 初始化進度條
    initializeStepBar();
});

function initializePage() {
    // 初始化日期選擇器
    initializeDatePickers();
    
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

function initializeStepBar() {
    // 使用 OFSApplicationMaster 的進度條樣式
    $('.application-step .step-item').first().addClass('active');
    
    // 綁定進度條點擊事件
    $('.application-step .step-item').on('click', function() {
        let index = $(this).index();
        switchToStep(index);
    });
}

function switchToStep(stepIndex) {
    // 更新進度條狀態
    $('.application-step .step-item').removeClass('active');
    $('.application-step .step-item').eq(stepIndex).addClass('active');
    
    // 切換內容區塊
    if (stepIndex === 0) {
        // 顯示申請表
        $('#applicationFormSection').show();
        $('#uploadAttachmentSection').hide();
    } else if (stepIndex === 1) {
        // 顯示上傳附件
        $('#applicationFormSection').hide();
        $('#uploadAttachmentSection').show();
    }
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

function initializeNumberInputs() {
    // 綁定經費欄位變化事件進行自動計算
    $('input[id*="Funds"]').on('input', function() {
        calculateTotalFunds();
    });
    
    // 綁定經費計算
    $('input[id*="Funds"]').addClass('funds-input');
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

function updateStepStatus(stepNumber, status) {
    $('.application-step .step-item').removeClass('active completed');
    
    $('.application-step .step-item').each(function(index) {
        if (index < stepNumber - 1) {
            $(this).addClass('completed');
        } else if (index === stepNumber - 1) {
            $(this).addClass(status);
        }
    });
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