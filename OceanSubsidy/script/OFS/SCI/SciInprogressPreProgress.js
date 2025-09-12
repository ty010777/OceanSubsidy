// 預定進度頁面 JavaScript 處理函數

function handleSubmit() {
    // 收集表單資料
    var formData = collectFormData();
    
    // 驗證必填欄位
    var validation = validateRequiredFields();
    if (!validation.isValid) {
        Swal.fire({
            title: '資料驗證失敗',
            text: validation.message,
            icon: 'warning'
        });
        return;
    }
    
    // 顯示確認對話框
    Swal.fire({
        title: '確認提送',
        text: '確定要提送預定分月進度嗎？',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: '確定提送',
        cancelButtonText: '取消'
    }).then((result) => {
        if (result.isConfirmed) {
            submitData(formData);
        }
    });
}

function collectFormData() {
    var data = {
        projectID: window.projectID || '', // 從全域變數取得
        coExecutingUnit: document.getElementById(window.txtCoExecutingUnitID).value,
        midReviewDate: document.getElementById(window.txtMidReviewDateID).value,
        finalReviewDate: document.getElementById(window.txtFinalReviewDateID).value,
        monthlyProgress: []
    };
    
    // 收集預定分月進度資料
    var workAbstractElements = document.querySelectorAll('.work-abstract');
    var preProgressElements = document.querySelectorAll('.pre-progress');
    
    for (var i = 0; i < workAbstractElements.length; i++) {
        var month = workAbstractElements[i].getAttribute('data-month');
        var workAbstract = workAbstractElements[i].value;
        var preProgress = preProgressElements[i].value;
        
        data.monthlyProgress.push({
            month: month,
            workAbstract: workAbstract,
            preProgress: preProgress
        });
    }
    
    return data;
}

function validateRequiredFields() {
    var errors = [];
    
    // 驗證期中審查預定日期
    var midReviewDate = document.getElementById(window.txtMidReviewDateID).value;
    if (!midReviewDate) {
        errors.push('期中審查預定日期為必填項目');
    }
    
    // 驗證期末審查預定日期
    var finalReviewDate = document.getElementById(window.txtFinalReviewDateID).value;
    if (!finalReviewDate) {
        errors.push('期末審查預定日期為必填項目');
    }
    
    // 驗證日期邏輯
    if (midReviewDate && finalReviewDate) {
        var midDate = new Date(midReviewDate);
        var finalDate = new Date(finalReviewDate);
        if (finalDate <= midDate) {
            errors.push('期末審查預定日期必須晚於期中審查預定日期');
        }
    }
    
    // 驗證工作摘要（必填）
    var workAbstractElements = document.querySelectorAll('.work-abstract');
    var hasEmptyWorkAbstract = false;
    for (var i = 0; i < workAbstractElements.length; i++) {
        if (!workAbstractElements[i].value.trim()) {
            hasEmptyWorkAbstract = true;
            break;
        }
    }
    if (hasEmptyWorkAbstract) {
        errors.push('所有月份的工作摘要都必須填寫');
    }
    
    return {
        isValid: errors.length === 0,
        message: errors.join('\n')
    };
}

function submitData(formData) {
    // 顯示載入中
    Swal.fire({
        title: '處理中...',
        text: '正在提送資料，請稍候',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    
    // 調用頁面的WebMethod
    $.ajax({
        type: "POST",
        url: "SciInprogress_PreProgress.aspx/SubmitPreProgress",
        data: JSON.stringify({ jsonData: JSON.stringify(formData) }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function(response) {
            var data = response.d;
            if (data.success) {
                Swal.fire({
                    title: '提送成功',
                    text: '預定分月進度已提送',
                    icon: 'success'
                }).then(() => {
                    // 可選：重新載入頁面或跳轉
                    location.reload();
                });
            } else {
                Swal.fire({
                    title: '提送失敗',
                    text: data.message || '系統發生錯誤，請稍後再試',
                    icon: 'error'
                });
            }
        },
        error: function(xhr, status, error) {
            console.error('Error:', error);
            Swal.fire({
                title: '系統錯誤',
                text: '提送時發生錯誤，請稍後再試',
                icon: 'error'
            });
        }
    });
}