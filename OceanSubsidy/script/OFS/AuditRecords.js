$(document).ready(function() {
    // 初始化日期選擇器
    initializeDatePickers();
    
    // 同步已載入的日期資料到隱藏欄位
    syncLoadedDatesToHiddenFields();
});

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
        const gregorianDate = selectedMoment.format('YYYY-MM-DD');
        
        $(this).val(displayDate);
        $(this).data('gregorian-date', gregorianDate);
        $(this).attr('data-gregorian-date', gregorianDate);
        
        // 建立隱藏欄位儲存西元日期供後端使用
        const hiddenFieldName = $(this).attr('name') + '_gregorian';
        let hiddenField = $(`input[name="${hiddenFieldName}"]`);
        if (hiddenField.length === 0) {
            hiddenField = $(`<input type="hidden" name="${hiddenFieldName}" />`);
            $(this).after(hiddenField);
        }
        hiddenField.val(gregorianDate);
    };
    
    // 初始化查核日期選擇器
    $('input[id*="txtAuditDate"]').daterangepicker(datePickerConfig)
        .on('apply.daterangepicker', handleDateSelection);
    
    // 設定查核日期的預設值為今天
    const auditDateInput = $('input[id*="txtAuditDate"]');
    if (auditDateInput.length > 0) {
        const today = moment();
        const todayTaiwan = today.format('tYY/MM/DD');
        const todayGregorian = today.format('YYYY-MM-DD');
        
        auditDateInput.val(todayTaiwan);
        auditDateInput.data('gregorian-date', todayGregorian);
        auditDateInput.attr('data-gregorian-date', todayGregorian);
        
        // 建立隱藏欄位
        const hiddenFieldName = auditDateInput.attr('name') + '_gregorian';
        let hiddenField = $(`input[name="${hiddenFieldName}"]`);
        if (hiddenField.length === 0) {
            hiddenField = $(`<input type="hidden" name="${hiddenFieldName}" />`);
            auditDateInput.after(hiddenField);
        }
        hiddenField.val(todayGregorian);
    }
}

function syncLoadedDatesToHiddenFields() {
    // 同步頁面上已有的日期資料到隱藏欄位
    $('.taiwan-date-picker').each(function() {
        const $input = $(this);
        const dateValue = $input.val();
        
        if (dateValue && dateValue.trim() !== '') {
            // 如果是民國年格式，轉換為西元年
            let gregorianDate;
            if (dateValue.includes('/')) {
                try {
                    const momentDate = moment(dateValue, 'tYY/MM/DD');
                    if (momentDate.isValid()) {
                        gregorianDate = momentDate.format('YYYY-MM-DD');
                        $input.data('gregorian-date', gregorianDate);
                        $input.attr('data-gregorian-date', gregorianDate);
                        
                        // 建立或更新隱藏欄位
                        const hiddenFieldName = $input.attr('name') + '_gregorian';
                        let hiddenField = $(`input[name="${hiddenFieldName}"]`);
                        if (hiddenField.length === 0) {
                            hiddenField = $(`<input type="hidden" name="${hiddenFieldName}" />`);
                            $input.after(hiddenField);
                        }
                        hiddenField.val(gregorianDate);
                    }
                } catch (e) {
                    console.error('日期格式轉換錯誤:', e);
                }
            }
        }
    });
}

// 顯示全域訊息
function showGlobalMessage(message, type = 'info') {
    var icon = 'info';
    var title = '提示';
    
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
    }
    
    Swal.fire({
        icon: icon,
        title: title,
        text: message,
        confirmButtonText: '確定'
    });
}

// 確認對話框
function confirmAction(message, callback) {
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
}

// 驗證查核表單
function validateAuditForm() {
    var auditorName = $('#<%= txtAuditorName.ClientID %>').val().trim();
    var auditDate = $('#<%= txtAuditDate.ClientID %>').attr('data-gregorian-date');
    var riskAssessment = $('#<%= ddlRiskAssessment.ClientID %>').val();
    var auditComment = $('#<%= txtAuditComment.ClientID %>').val().trim();
    
    if (!auditorName) {
        showGlobalMessage('請輸入查核人員', 'warning');
        $('#<%= txtAuditorName.ClientID %>').focus();
        return false;
    }
    
    if (!auditDate) {
        showGlobalMessage('請選擇查核日期', 'warning');
        $('#<%= txtAuditDate.ClientID %>').focus();
        return false;
    }
    
    if (!riskAssessment) {
        showGlobalMessage('請選擇風險評估', 'warning');
        $('#<%= ddlRiskAssessment.ClientID %>').focus();
        return false;
    }
    
    if (!auditComment) {
        showGlobalMessage('請輸入查核意見', 'warning');
        $('#<%= txtAuditComment.ClientID %>').focus();
        return false;
    }
    
    return true;
}

