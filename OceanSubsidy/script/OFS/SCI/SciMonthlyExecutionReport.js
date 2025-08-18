/**
 * SciMonthlyExecutionReport.js
 * 科技類計畫每月執行報告頁面功能
 */

// 月份切換功能
function switchMonth(month) {
    console.log('切換到月份: ' + month);
    
    // 更新 timeline active 狀態
    updateTimelineActive(month);
    
    // 更新頁面標題中的月份顯示
    updateMonthDisplay(month);
    
    // AJAX 載入該月份的資料
    loadMonthData(month);
}

// 更新 timeline 的 active 狀態
function updateTimelineActive(selectedMonth) {
    // 移除所有 active 狀態
    document.querySelectorAll('.timeline .month li').forEach(function(li) {
        li.classList.remove('active');
    });
    
    // 為選中的月份添加 active 狀態
    document.querySelectorAll('.timeline .month li a').forEach(function(link) {
        if (link.getAttribute('onclick').includes(selectedMonth)) {
            link.parentElement.classList.add('active');
        }
    });
}

// 更新頁面月份顯示
function updateMonthDisplay(month) {
    // 使用 querySelector 找到包含 currentMonth 的 span 元素
    var currentMonth = document.querySelector('span[id*="currentMonth"]:not([id*="currentMonth2"])');
    var currentMonth2 = document.querySelector('span[id*="currentMonth2"]');
    
    if (currentMonth) {
        currentMonth.textContent = month;
    }
    if (currentMonth2) {
        currentMonth2.textContent = month;
    }
}

// AJAX 載入月份資料
function loadMonthData(month) {
    var projectID = getProjectID();
    
    console.log('載入月份資料:', month, 'ProjectID:', projectID);
    
    if (!projectID) {
        console.error('ProjectID 不存在');
        Swal.fire('錯誤', 'ProjectID 不存在', 'error');
        return;
    }

    // 顯示載入中
    showLoadingSpinner();

    // AJAX 呼叫
    fetch('SciMonthlyExecutionReport.aspx/LoadMonthData', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=utf-8'
        },
        body: JSON.stringify({
            projectID: projectID,
            month: month
        })
    })
    .then(response => response.json())
    .then(data => {
        hideLoadingSpinner();
        
        console.log('收到伺服器回應:', data);
        
        if (data.d && data.d.success) {
            console.log('資料載入成功:', data.d);
            // 渲染實際進度表格
            renderActualProgress(data.d.progressData);
            // 渲染查核點表格
            renderCheckPoints(data.d.checkPoints, data.d.achievementRate);
            // 渲染累計執行經費
            renderMonthlyBudget(data.d.progressData);
        } else {
            console.error('載入資料失敗:', data.d ? data.d.message : '未知錯誤');
            Swal.fire('錯誤', data.d ? data.d.message : '載入月份資料失敗', 'error');
        }
    })
    .catch(error => {
        hideLoadingSpinner();
        console.error('AJAX 呼叫失敗:', error);
        Swal.fire('錯誤', '載入月份資料時發生錯誤', 'error');
    });
}

// 取得 ProjectID (從 ASPX 頁面)
function getProjectID() {
    // 這個函數需要在 ASPX 頁面中定義，以便從後端取得 ProjectID
    if (typeof window.projectID !== 'undefined') {
        return window.projectID;
    }
    
    // 嘗試從隱藏欄位或其他地方獲取
    var hiddenProjectID = document.querySelector('input[name*="ProjectID"], input[id*="ProjectID"]');
    if (hiddenProjectID) {
        return hiddenProjectID.value;
    }
    
    // 從 URL QueryString 獲取
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('ProjectID') || '';
}

// 顯示載入中
function showLoadingSpinner() {
    var containers = document.querySelectorAll('#actualProgressContainer, #checkPointContainer');
    containers.forEach(function(container) {
        if (container) {
            container.innerHTML = '<tr><td colspan="5" class="text-center py-4"><i class="fas fa-spinner fa-spin"></i> 載入中...</td></tr>';
        }
    });
}

// 隱藏載入中
function hideLoadingSpinner() {
    // 由 renderActualProgress 和 renderCheckPoints 處理
}

// 渲染實際進度表格
function renderActualProgress(data) {
    var container = document.getElementById('actualProgressContainer');
    
    if (!container || !data) {
        return;
    }

    var html = 
        '<tr>' +
            '<th class="text-end">' + escapeHtml(data.month || '') + '</th>' +
            '<td>' + escapeHtml(data.PreWorkAbstract || '') + '</td>' +
            '<td>' +
                '<span class="form-control textarea" role="textbox" contenteditable="" ' +
                      'data-placeholder="請輸入" aria-label="文本輸入區域" ' +
                      'style="height: auto; overflow-y: hidden;" ' +
                      'id="actWorkAbstract">' + escapeHtml(data.ActWorkAbstract || '') + '</span>' +
            '</td>' +
            '<td class="text-end">' + escapeHtml(data.PreProgress || '') + '%</td>' +
            '<td>' +
                '<input type="text" class="form-control" placeholder="請輸入" ' +
                       'id="actProgress" value="' + escapeHtml(data.ActProgress || '') + '">' +
            '</td>' +
        '</tr>';
    
    container.innerHTML = html;
    
    // 重新初始化 textarea 自動調整高度
    initTextareaAutoResize();
}

// 渲染查核點表格
function renderCheckPoints(data, achievementRate) {
    var container = document.getElementById('checkPointContainer');
    
    if (!container) {
        return;
    }
    
    if (!data || !Array.isArray(data) || data.length === 0) {
        container.innerHTML = '<tr><td colspan="5" class="text-center text-muted py-4">本月無查核點資料</td></tr>';
        return;
    }

    var html = '';
    data.forEach(function(item, index) {
        
        var radioName = 'checkPoint_' + index;
        var isFinish = parseInt(item.isFinish) || 0; // 確保轉換為數字
        
        // 判斷是否需要顯示落後原因和改善措施 (部分完成或未完成時)
        var shouldShowDelayFields = (isFinish === 1 || isFinish === 2);
        
        // 完成狀態的HTML - 如果是「本月無查核點」則顯示 "--"
        var completionHtml = '';
        var delayFieldsHtml = '';
        
        if (item.checkDescription === "本月無查核點") {
            completionHtml = '<span class="text-muted">--</span>';
            delayFieldsHtml = ''; // 無查核點時不顯示落後原因和改善措施
        } else {
            // 有查核點時的完成狀態HTML - 根據 isFinish 值正確設定 checked 狀態
            completionHtml = 
                '<div class="form-check-input-group d-flex mb-3">' +
                    '<input id="' + radioName + '_incomplete" class="form-check-input check-teal me-1" type="radio" name="' + radioName + '" value="1" ' + (isFinish === 1 ? 'checked' : '') + ' onchange="toggleDelayFields(' + index + ')">' +
                    '<label for="' + radioName + '_incomplete" class="me-3">未完成</label>' +
                    '<input id="' + radioName + '_partial" class="form-check-input check-teal me-1" type="radio" name="' + radioName + '" value="2" ' + (isFinish === 2 ? 'checked' : '') + ' onchange="toggleDelayFields(' + index + ')">' +
                    '<label for="' + radioName + '_partial" class="me-3">部分完成</label>' +
                    '<input id="' + radioName + '_complete" class="form-check-input check-teal me-1" type="radio" name="' + radioName + '" value="3" ' + (isFinish === 3 ? 'checked' : '') + ' onchange="toggleDelayFields(' + index + ')">' +
                    '<label for="' + radioName + '_complete">完成</label>' +
                '</div>';
            
            // 落後原因和改善措施 HTML - 填入資料庫中的值
            delayFieldsHtml = 
                '<div id="delayFields_' + index + '" class="delay-fields" style="' + (shouldShowDelayFields ? '' : 'display: none;') + '">' +
                    '<div class="mb-3">' +
                        '<div class="text-teal pb-2">落後原因</div>' +
                        '<span class="form-control textarea" role="textbox" contenteditable aria-label="落後原因輸入區域" data-placeholder="請輸入" style="min-height: 38px;">' + escapeHtml(item.delayReason || '') + '</span>' +
                    '</div>' +
                    '<div class="mb-3">' +
                        '<div class="text-teal pb-2">改善措施</div>' +
                        '<span class="form-control textarea" role="textbox" contenteditable aria-label="改善措施輸入區域" data-placeholder="請輸入" style="min-height: 38px;">' + escapeHtml(item.improvement || '') + '</span>' +
                    '</div>' +
                '</div>';
        }
        
        // 實際完成時間欄位 - 如果是「本月無查核點」則顯示 "--"
        var actFinishTimeHtml = '';
        if (item.checkDescription === "本月無查核點") {
            actFinishTimeHtml = '<span class="text-muted">--</span>';
        } else {
            actFinishTimeHtml = '<input type="date" class="form-control" name="actFinishTime_' + index + '" value="' + escapeHtml(item.actFinishTime || '') + '" aria-label="實際完成時間">';
        }
        
        // 年度目標達成數欄位 (只在第一行顯示，其他行合併)
        var achievementCell = '';
        if (index === 0) {
            // 第一行顯示年度目標達成率，並設定rowspan為資料筆數
            achievementCell = '<td class="text-center align-middle" rowspan="' + data.length + '">' +
                             '<strong class="text-primary">' + escapeHtml(achievementRate || '0.00%') + '</strong>' +
                             '</td>';
        }
        // 其他行不需要年度目標達成數欄位，因為已經被第一行的rowspan覆蓋
        
        html += 
            '<tr data-checkpoint-id="' + (item.id || '') + '">' +
                '<th class="text-end">' + escapeHtml(item.month || '') + '</th>' +
                '<td>' + escapeHtml(item.checkDescription || '無描述') + '</td>' +
                '<td>' +
                    completionHtml +
                    delayFieldsHtml +
                '</td>' +
                '<td>' + actFinishTimeHtml + '</td>' +
                achievementCell +
            '</tr>';
    });
    
    container.innerHTML = html;
    
    // 重新初始化 textarea 自動調整高度
    initTextareaAutoResize();
}

// 渲染累計執行經費
function renderMonthlyBudget(progressData) {
    // 取出值，若為 null 或 undefined 則設為空字串
    const monthlySubsidy = progressData.MonthlySubsidy || "";
    const monthlyCoop = progressData.MonthlyCoop || "";
    const monthlyTotal = progressData.MonthlyTotal || "";

    // 指定 ID 的欄位賦值
    document.getElementById("txtSubsidyAmount").value = monthlySubsidy;
    document.getElementById("txtMatchingAmount").value = monthlyCoop;
    document.getElementById("lblTotalBudget").innerText = monthlyTotal;
}

// 切換落後原因和改善措施欄位的顯示
function toggleDelayFields(index) {
    var radioName = 'checkPoint_' + index;
    var delayFields = document.getElementById('delayFields_' + index);
    var selectedRadio = document.querySelector('input[name="' + radioName + '"]:checked');
    
    if (delayFields && selectedRadio) {
        var value = parseInt(selectedRadio.value);
        // 只有未完成(1)或部分完成(2)時才顯示
        if (value === 1 || value === 2) {
            delayFields.style.display = '';
        } else {
            delayFields.style.display = 'none';
        }
    }
}

// HTML 轉義函數
function escapeHtml(text) {
    if (!text) return '';
    var div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// 初始化 textarea 自動調整高度
function initTextareaAutoResize() {
    var textareas = document.querySelectorAll('.form-control.textarea');
    textareas.forEach(function(textarea) {
        textarea.removeEventListener('input', autoResizeHandler);
        textarea.addEventListener('input', autoResizeHandler);
    });
}

// textarea 自動調整高度處理函數
function autoResizeHandler() {
    this.style.height = 'auto';
    this.style.height = (this.scrollHeight) + 'px';
}

// 暫存月份報告
function saveMonthlyReport() {
    try {
        const reportData = collectFormData();
        const projectID = getProjectID();
        const month = getCurrentMonth();
        
        if (!projectID || !month) {
            Swal.fire('錯誤', 'ProjectID 或月份資訊不完整', 'error');
            return;
        }
        
        // 顯示載入中
        Swal.fire({
            title: '暫存中...',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });
        
        // AJAX 呼叫暫存 WebMethod
        fetch('SciMonthlyExecutionReport.aspx/SaveMonthlyReport', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            },
            body: JSON.stringify({
                projectID: projectID,
                month: month,
                reportData: JSON.stringify(reportData)
            })
        })
        .then(response => response.json())
        .then(data => {
            console.log('暫存回應:', data);
            
            if (data.d && data.d.success) {
                Swal.fire('成功', data.d.message || '資料暫存成功', 'success').then(() => {
                    if (data.d.shouldReload) {
                        window.location.reload();
                    }
                });
            } else {
                Swal.fire('錯誤', data.d ? data.d.message : '暫存失敗', 'error');
            }
        })
        .catch(error => {
            console.error('暫存 AJAX 呼叫失敗:', error);
            Swal.fire('錯誤', '暫存資料時發生錯誤', 'error');
        });
        
    } catch (error) {
        console.error('暫存功能發生錯誤:', error);
        Swal.fire('錯誤', '暫存功能發生錯誤', 'error');
    }
}

// 提送月份報告
function submitMonthlyReport() {
    try {
        const reportData = collectFormData();
        const projectID = getProjectID();
        const month = getCurrentMonth();
        
        if (!projectID || !month) {
            Swal.fire('錯誤', 'ProjectID 或月份資訊不完整', 'error');
            return;
        }
        
        // 確認提送
        Swal.fire({
            title: '確認提送',
            text: '確定要提送這個月的執行報告嗎？',
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: '確定提送',
            cancelButtonText: '取消'
        }).then((result) => {
            if (result.isConfirmed) {
                // 顯示載入中
                Swal.fire({
                    title: '提送中...',
                    allowOutsideClick: false,
                    didOpen: () => {
                        Swal.showLoading();
                    }
                });
                
                // AJAX 呼叫提送 WebMethod
                fetch('SciMonthlyExecutionReport.aspx/SubmitMonthlyReport', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json; charset=utf-8'
                    },
                    body: JSON.stringify({
                        projectID: projectID,
                        month: month,
                        reportData: JSON.stringify(reportData)
                    })
                })
                .then(response => response.json())
                .then(data => {
                    console.log('提送回應:', data);
                    
                    if (data.d && data.d.success) {
                        let message = data.d.message || '資料提送成功';
                        if (data.d.emailNotification) {
                            message += '\n\n' + data.d.emailNotification;
                        }
                        Swal.fire('成功', message, 'success').then(() => {
                            if (data.d.shouldReload) {
                                window.location.reload();
                            }
                        });
                    } else {
                        Swal.fire('錯誤', data.d ? data.d.message : '提送失敗', 'error');
                    }
                })
                .catch(error => {
                    console.error('提送 AJAX 呼叫失敗:', error);
                    Swal.fire('錯誤', '提送資料時發生錯誤', 'error');
                });
            }
        });
        
    } catch (error) {
        console.error('提送功能發生錯誤:', error);
        Swal.fire('錯誤', '提送功能發生錯誤', 'error');
    }
}

// 收集表單資料
function collectFormData() {
    console.log('開始收集表單資料...');
    
    const actualProgress = collectActualProgressData();
    const checkPoints = collectCheckPointData();
    const monthlyBudget = collectMonthlyBudgetData();
    
    console.log('收集結果:', {
        actualProgress: actualProgress,
        checkPoints: checkPoints,
        monthlyBudget: monthlyBudget
    });
    
    const reportData = {
        actualProgress: actualProgress,
        checkPoints: checkPoints,
        monthlyBudget: monthlyBudget
    };
    
    return reportData;
}

// 收集實際進度資料
function collectActualProgressData() {
    try {
        const container = document.getElementById('actualProgressContainer');
        if (!container) {
            console.warn('actualProgressContainer 元素不存在');
            return {
                actWorkAbstract: '',
                actProgress: ''
            };
        }
        
        // 檢查容器是否有內容
        const rows = container.querySelectorAll('tr');
        if (rows.length === 0) {
            console.warn('actualProgressContainer 中沒有資料行');
            return {
                actWorkAbstract: '',
                actProgress: ''
            };
        }
        
        // 使用 querySelector 在容器內找元素
        const actWorkAbstractElement = container.querySelector('span.textarea[contenteditable]');
        const actProgressElement = container.querySelector('input[type="text"]');
        
        console.log('收集實際進度資料:', {
            container: !!container,
            actWorkAbstractElement: !!actWorkAbstractElement,
            actProgressElement: !!actProgressElement
        });
        
        return {
            actWorkAbstract: actWorkAbstractElement ? actWorkAbstractElement.textContent.trim() : '',
            actProgress: actProgressElement ? actProgressElement.value.trim() : ''
        };
    } catch (error) {
        console.error('收集實際進度資料時發生錯誤:', error);
        return {
            actWorkAbstract: '',
            actProgress: ''
        };
    }
}

// 收集查核點資料
function collectCheckPointData() {
    try {
        const checkPointContainer = document.getElementById('checkPointContainer');
        if (!checkPointContainer) {
            console.warn('checkPointContainer 元素不存在');
            return [];
        }
        
        const rows = checkPointContainer.querySelectorAll('tr');
        if (rows.length === 0) {
            console.warn('checkPointContainer 中沒有資料行');
            return [];
        }
        
        console.log('找到查核點資料行數:', rows.length);
        
        const checkPoints = [];
        
        rows.forEach((row, index) => {
            const checkDescription = row.querySelector('td:nth-child(2)')?.textContent?.trim() || '';
            
            console.log(`第 ${index} 行查核點描述:`, checkDescription);
            
            // 如果是「本月無查核點」，跳過收集
            if (checkDescription === '本月無查核點') {
                console.log('跳過「本月無查核點」資料');
                return;
            }
            
            // 收集完成狀態
            const selectedRadio = row.querySelector('input[type="radio"]:checked');
            const isFinish = selectedRadio ? parseInt(selectedRadio.value) : 0;
            
            // 收集落後原因和改善措施 - 直接在 row 內找，不限制在 delay-fields 內
            const delayFieldsDiv = row.querySelector('.delay-fields');
            let delayReason = '';
            let improvement = '';
            
            if (delayFieldsDiv) {
                const textareas = delayFieldsDiv.querySelectorAll('.form-control.textarea[contenteditable]');
                delayReason = textareas[0] ? textareas[0].textContent.trim() : '';
                improvement = textareas[1] ? textareas[1].textContent.trim() : '';
            }
            
            const actFinishTimeElement = row.querySelector('input[type="date"]');
            
            // 取得 checkpoint ID (資料庫主鍵)
            const checkpointId = row.getAttribute('data-checkpoint-id') || '';
            
            checkPoints.push({
                index: index,
                checkDescription: checkDescription,
                isFinish: isFinish,
                delayReason: delayReason,
                improvement: improvement,
                actFinishTime: actFinishTimeElement ? actFinishTimeElement.value : '',
                id: checkpointId
            });
        });
        
        console.log('收集到的查核點資料:', checkPoints);
        return checkPoints;
    } catch (error) {
        console.error('收集查核點資料時發生錯誤:', error);
        return [];
    }
}

// 收集月份預算資料
function collectMonthlyBudgetData() {
    try {
        const subsidyElement = document.getElementById('txtSubsidyAmount');
        const coopElement = document.getElementById('txtMatchingAmount');
        
        return {
            monthlySubsidy: subsidyElement ? subsidyElement.value.trim() : '',
            monthlyCoop: coopElement ? coopElement.value.trim() : ''
        };
    } catch (error) {
        console.error('收集月份預算資料時發生錯誤:', error);
        return null;
    }
}

// 取得目前選擇的月份
function getCurrentMonth() {
    try {
        // 使用 querySelector 找到第一個 currentMonth span 元素
        const monthElement = document.querySelector('span[id*="currentMonth"]:not([id*="currentMonth2"])');
        if (monthElement && monthElement.textContent) {
            return monthElement.textContent.trim();
        }
        
        console.warn('無法取得目前月份');
        return '';
    } catch (error) {
        console.error('取得目前月份時發生錯誤:', error);
        return '';
    }
}

// 頁面載入完成時初始化
document.addEventListener('DOMContentLoaded', function() {
    initTextareaAutoResize();
    
    // 自動載入第一個可用的月份資料
    setTimeout(function() {
        const firstActiveMonth = document.querySelector('.timeline .month li.active a');
        if (firstActiveMonth) {
            const onclickAttr = firstActiveMonth.getAttribute('onclick');
            if (onclickAttr) {
                // 從 onclick 屬性中提取月份參數
                const monthMatch = onclickAttr.match(/switchMonth\("([^"]+)"\)/);
                if (monthMatch && monthMatch[1]) {
                    console.log('自動載入第一個月份資料:', monthMatch[1]);
                    loadMonthData(monthMatch[1]);
                }
            }
        } else {
            console.warn('找不到第一個活動月份，嘗試載入當前顯示月份');
            const currentMonth = getCurrentMonth();
            if (currentMonth) {
                loadMonthData(currentMonth);
            }
        }
    }, 500); // 延遲500ms確保頁面DOM完全載入
});