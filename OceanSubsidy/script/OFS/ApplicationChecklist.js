/**
 * ApplicationChecklist.js
 * 申請案件清單頁面的JavaScript功能
 */

// 全局變量
let tabsInitialized = false;

// 更新表格資料
function updateTableData(htmlContent) {
    var tableBody = document.getElementById('dataTableBody');
    if (tableBody) {
        tableBody.innerHTML = htmlContent;
        // 重新綁定事件
        bindCheckboxEvents();
    }
}

// 綁定 checkbox 事件
function bindCheckboxEvents() {
    // 全選/反選功能
    var selectAllCheckbox = document.getElementById('selectAllCheckbox');
    if (selectAllCheckbox) {
        selectAllCheckbox.addEventListener('change', function() {
            var caseCheckboxes = document.querySelectorAll('.case-checkbox');
            caseCheckboxes.forEach(function(checkbox) {
                checkbox.checked = selectAllCheckbox.checked;
            });
        });
    }
    
    // 單個checkbox的變更事件
    var caseCheckboxes = document.querySelectorAll('.case-checkbox');
    caseCheckboxes.forEach(function(checkbox) {
        checkbox.addEventListener('change', function() {
            updateSelectAllState();
        });
    });
}

// 更新全選checkbox的狀態
function updateSelectAllState() {
    var selectAllCheckbox = document.getElementById('selectAllCheckbox');
    var caseCheckboxes = document.querySelectorAll('.case-checkbox');
    var checkedCount = document.querySelectorAll('.case-checkbox:checked').length;
    
    if (checkedCount === 0) {
        selectAllCheckbox.checked = false;
        selectAllCheckbox.indeterminate = false;
    } else if (checkedCount === caseCheckboxes.length) {
        selectAllCheckbox.checked = true;
        selectAllCheckbox.indeterminate = false;
    } else {
        selectAllCheckbox.checked = false;
        selectAllCheckbox.indeterminate = true;
    }
}

// 收集選中的案件
function getSelectedCases() {
    var selectedCases = [];
    var checkedBoxes = document.querySelectorAll('.case-checkbox:checked');
    
    checkedBoxes.forEach(function(checkbox) {
        selectedCases.push({
            projectId: checkbox.getAttribute('data-projectid'),
            status: checkbox.getAttribute('data-status'),
            stage: checkbox.getAttribute('data-stage')
        });
    });
    
    return selectedCases;
}

// 篩選標籤功能
function initTabs() {
    if (tabsInitialized) return; // 防止重複初始化
    
    const tabs = document.querySelectorAll('.total-item');
    tabs.forEach(function(tab, index) {
        tab.addEventListener('click', function(e) {
            e.preventDefault();
            
            // 檢查是否為禁用狀態
            if (tab.classList.contains('disabled')) {
                return; // 禁用的標籤不處理點擊事件
            }
            
            // 移除所有active狀態
            tabs.forEach(function(t) {
                t.classList.remove('active');
            });
            
            // 添加active狀態到當前標籤
            tab.classList.add('active');
            
            // 取得選中的申請類別
            const categoryText = tab.querySelector('.total-item-title').textContent.trim();
            
            // 設定隱藏欄位的值 - 需要在 server side 設定 ClientID
            const hidSelectedStageElements = document.querySelectorAll('input[id*="hidSelectedStage"]');
            const btnStageFilterElements = document.querySelectorAll('input[id*="btnStageFilter"]');
            
            if (hidSelectedStageElements.length > 0) {
                hidSelectedStageElements[0].value = categoryText;
                
                // 觸發後端查詢
                if (btnStageFilterElements.length > 0) {
                    btnStageFilterElements[0].click();
                }
            }
        });
    });
    
    tabsInitialized = true;
}

// 更新標籤統計數字
function updateTabCounts(counts) {
    const categories = ['總申請', '科專', '文化', '學校民間', '學校社團', '多元', '素養', '無障礙'];
    const tabs = document.querySelectorAll('.total-item');
    
    tabs.forEach(function(tab, index) {
        if (index < categories.length) {
            const countElement = tab.querySelector('.count');
            const category = categories[index];
            const count = counts[category] || 0;
            if (countElement) {
                countElement.textContent = count;
            }
        }
    });
}

// 設定選中標籤的 active 狀態
function setActiveTab(categoryName) {
    const tabs = document.querySelectorAll('.total-item');
    tabs.forEach(function(tab) {
        const titleElement = tab.querySelector('.total-item-title');
        if (titleElement && titleElement.textContent.trim() === categoryName) {
            // 移除所有標籤的 active 狀態
            tabs.forEach(function(t) {
                t.classList.remove('active');
            });
            // 為當前標籤添加 active 狀態
            tab.classList.add('active');
        }
    });
}

// 排序功能
function initSortButtons() {
    const sortButtons = document.querySelectorAll('.sort');
    sortButtons.forEach(function(button) {
        button.addEventListener('click', function() {
            // 移除其他按鈕的排序狀態
            sortButtons.forEach(function(btn) {
                if (btn !== button) {
                    btn.classList.remove('up', 'down');
                }
            });
            
            // 切換當前按鈕的排序狀態
            if (button.classList.contains('up')) {
                button.classList.remove('up');
                button.classList.add('down');
            } else if (button.classList.contains('down')) {
                button.classList.remove('down');
            } else {
                button.classList.add('up');
            }
        });
    });
}

// 初始化搜尋表單
function initSearchForm() {
    // 為搜尋表單添加Enter鍵支援
    const searchInputs = document.querySelectorAll('.search-form input, .search-form select');
    searchInputs.forEach(function(input) {
        input.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                const searchBtn = document.querySelector('.btn-search') || document.querySelector('input[id*="btnSearch"]');
                if (searchBtn) {
                    searchBtn.click();
                }
            }
        });
    });
}

// 頁面載入完成後的初始化
document.addEventListener('DOMContentLoaded', function() {
    // 延遲初始化，確保ASP.NET控制項已渲染
    setTimeout(function() {
        bindCheckboxEvents();
        initTabs();
        initSortButtons();
        initSearchForm();
        initPaginationButtons();
        initDropdownToggle();
    }, 100);
});

// 初始化下拉選單切換功能
function initDropdownToggle() {
    // 使用事件委派監聽下拉按鈕點擊
    document.addEventListener('click', function(e) {
        const dropdownButton = e.target.closest('[data-bs-toggle="dropdown"]');
        
        if (dropdownButton) {
            e.preventDefault();
            const dropdown = dropdownButton.closest('.dropdown');
            const dropdownMenu = dropdown.querySelector('.dropdown-menu');
            
            if (dropdownMenu.classList.contains('show')) {
                // 關閉下拉選單
                dropdownButton.classList.remove('show');
                dropdownButton.setAttribute('aria-expanded', 'false');
                dropdownMenu.classList.remove('show');
            } else {
                // 先關閉其他所有下拉選單
                document.querySelectorAll('.dropdown .show').forEach(el => el.classList.remove('show'));
                document.querySelectorAll('[data-bs-toggle="dropdown"]').forEach(btn => btn.setAttribute('aria-expanded', 'false'));
                
                // 打開當前下拉選單
                dropdownButton.classList.add('show');
                dropdownButton.setAttribute('aria-expanded', 'true');
                dropdownMenu.classList.add('show');
            }
        } else {
            // 點擊其他地方時關閉所有下拉選單
            document.querySelectorAll('.dropdown .show').forEach(el => el.classList.remove('show'));
            document.querySelectorAll('[data-bs-toggle="dropdown"]').forEach(btn => btn.setAttribute('aria-expanded', 'false'));
        }
    });
}

// 表格工具提示初始化
function initTooltips() {
    // 初始化Bootstrap tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    if (window.bootstrap && window.bootstrap.Tooltip) {
        var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
}

// 初始化分頁按鈕圖示
function initPaginationButtons() {
    // 為前一頁按鈕添加圖示
    const prevButton = document.querySelector('input[id*="btnPrevPage"]');
    if (prevButton) {
        prevButton.innerHTML = '<i class="fas fa-chevron-left"></i>';
    }
    
    // 為下一頁按鈕添加圖示
    const nextButton = document.querySelector('input[id*="btnNextPage"]');
    if (nextButton) {
        nextButton.innerHTML = '<i class="fas fa-chevron-right"></i>';
    }
    
    // 初始化分頁
    updatePagination(1, 1); // 預設第1頁，總共1頁
}

// 更新分頁顯示
function updatePagination(currentPage, totalPages) {
    const paginationNav = document.getElementById('paginationNav');
    if (!paginationNav) return;
    
    const prevButton = paginationNav.querySelector('input[id*="btnPrevPage"]');
    const nextButton = paginationNav.querySelector('input[id*="btnNextPage"]');
    
    // 清除現有的頁碼按鈕
    const existingPageButtons = paginationNav.querySelectorAll('.pagination-item, .ellipsis');
    existingPageButtons.forEach(btn => btn.remove());
    
    // 生成頁碼按鈕
    const pageButtons = generatePageButtons(currentPage, totalPages);
    
    // 在下一頁按鈕前插入頁碼按鈕
    pageButtons.forEach(button => {
        nextButton.parentNode.insertBefore(button, nextButton);
    });
    
    // 更新跳頁下拉選單
    updatePageDropdown(totalPages, currentPage);
    
    // 更新按鈕狀態
    if (prevButton) {
        prevButton.disabled = currentPage <= 1;
        if (currentPage <= 1) {
            prevButton.setAttribute('disabled', 'disabled');
        } else {
            prevButton.removeAttribute('disabled');
        }
    }
    
    if (nextButton) {
        nextButton.disabled = currentPage >= totalPages;
        if (currentPage >= totalPages) {
            nextButton.setAttribute('disabled', 'disabled');
        } else {
            nextButton.removeAttribute('disabled');
        }
    }
}

// 生成頁碼按鈕
function generatePageButtons(currentPage, totalPages) {
    const buttons = [];
    
    if (totalPages <= 7) {
        // 頁數少時，顯示所有頁碼
        for (let i = 1; i <= totalPages; i++) {
            buttons.push(createPageButton(i, i === currentPage));
        }
    } else {
        // 頁數多時，使用省略號
        if (currentPage <= 4) {
            // 當前頁在前面
            for (let i = 1; i <= 5; i++) {
                buttons.push(createPageButton(i, i === currentPage));
            }
            buttons.push(createEllipsis());
            buttons.push(createPageButton(totalPages, false));
        } else if (currentPage >= totalPages - 3) {
            // 當前頁在後面
            buttons.push(createPageButton(1, false));
            buttons.push(createEllipsis());
            for (let i = totalPages - 4; i <= totalPages; i++) {
                buttons.push(createPageButton(i, i === currentPage));
            }
        } else {
            // 當前頁在中間
            buttons.push(createPageButton(1, false));
            buttons.push(createEllipsis());
            for (let i = currentPage - 1; i <= currentPage + 1; i++) {
                buttons.push(createPageButton(i, i === currentPage));
            }
            buttons.push(createEllipsis());
            buttons.push(createPageButton(totalPages, false));
        }
    }
    
    return buttons;
}

// 創建頁碼按鈕
function createPageButton(pageNumber, isActive) {
    const button = document.createElement('button');
    button.className = isActive ? 'pagination-item active' : 'pagination-item';
    button.innerHTML = `<span class="page-number">${pageNumber}</span>`;
    
    if (!isActive) {
        button.addEventListener('click', function() {
            // 觸發頁面跳轉邏輯
            goToPage(pageNumber);
        });
    }
    
    return button;
}

// 創建省略號
function createEllipsis() {
    const ellipsis = document.createElement('div');
    ellipsis.className = 'pagination-item ellipsis';
    ellipsis.innerHTML = '<span class="">...</span>';
    return ellipsis;
}

// 更新跳頁下拉選單
function updatePageDropdown(totalPages, currentPage) {
    const pageDropdown = document.querySelector('select[id*="ddlPageNumber"]');
    if (!pageDropdown) return;
    
    // 清空現有選項
    pageDropdown.innerHTML = '';
    
    // 添加新選項
    for (let i = 1; i <= totalPages; i++) {
        const option = document.createElement('option');
        option.value = i;
        option.textContent = i;
        if (i === currentPage) {
            option.selected = true;
        }
        pageDropdown.appendChild(option);
    }
}

// 跳轉到指定頁面
function goToPage(pageNumber) {
    // 這裡可以觸發後端分頁邏輯
    console.log('跳轉到第', pageNumber, '頁');
    // 實際實現時可以觸發PostBack或AJAX請求
}

// 在頁面載入後初始化工具提示
window.addEventListener('load', function() {
    initTooltips();
});

// 案件操作相關功能
let currentVersionId = null;
// 處理回覆操作
function handleReply(versionId) {
    // 跳轉到回覆頁面或顯示回覆模態框
    // 這裡可以根據需求實現具體功能
    alert('回覆功能待實現，Version ID: ' + versionId);
}

// 顯示案件歷程
function showHistory(projectId) {
    // 顯示 Modal 並設定載入狀態
    const modal = new bootstrap.Modal(document.getElementById('planHistoryModal'));
    showLoadingState();
    modal.show();
    
    // 發送 AJAX 請求載入歷程資料
    loadHistoryData(projectId);
}

// 載入案件歷程資料
function loadHistoryData(projectId) {
    $.ajax({
        type: 'POST',
        url: 'ApplicationChecklist.aspx/GetCaseHistory',
        data: JSON.stringify({ projectId: projectId }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function(response) {
            if (response.d && response.d.success) {
                displayHistoryData(response.d.data);
            } else {
                showErrorState(response.d ? response.d.message : '載入歷程資料失敗');
            }
        },
        error: function(xhr, status, error) {
            console.error('AJAX Error:', error);
            showErrorState('載入歷程資料時發生錯誤');
        }
    });
}

// 顯示載入狀態
function showLoadingState() {
    const modalBody = document.querySelector('#planHistoryModal .modal-body');
    modalBody.innerHTML = `
        <div class="table-responsive">
            <table class="table align-middle gray-table">
                <thead>
                    <tr>
                        <th>時間</th>
                        <th>人員</th>
                        <th>階段狀態</th>
                        <th>說明</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td colspan="4" style="text-align: center; padding: 20px;">
                            <i class="fas fa-spinner fa-spin"></i> 載入中...
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    `;
}

// 顯示歷程資料
function displayHistoryData(historyData) {
    const modalBody = document.querySelector('#planHistoryModal .modal-body');
    
    if (!historyData || historyData.length === 0) {
        modalBody.innerHTML = `
            <div class="table-responsive">
                <table class="table align-middle gray-table">
                    <thead>
                        <tr>
                            <th>時間</th>
                            <th>人員</th>
                            <th>階段狀態</th>
                            <th>說明</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td colspan="4" style="text-align: center; padding: 20px; color: #6c757d;">
                                <i class="fas fa-info-circle"></i> 目前沒有歷程資料
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        `;
        return;
    }
    
    let tableRows = '';
    historyData.forEach(function(item) {
        tableRows += `
            <tr>
                <td>${item.changeTime || ''}</td>
                <td>${item.userName || ''}</td>
                <td>${item.stageChange || ''}</td>
                <td>${item.description || ''}</td>
            </tr>
        `;
    });
    
    modalBody.innerHTML = `
        <div class="table-responsive">
            <table class="table align-middle gray-table">
                <thead>
                    <tr>
                        <th>時間</th>
                        <th>人員</th>
                        <th>階段狀態</th>
                        <th>說明</th>
                    </tr>
                </thead>
                <tbody>
                    ${tableRows}
                </tbody>
            </table>
        </div>
    `;
}

// 顯示錯誤狀態
function showErrorState(errorMessage) {
    const modalBody = document.querySelector('#planHistoryModal .modal-body');
    modalBody.innerHTML = `
        <div class="table-responsive">
            <table class="table align-middle gray-table">
                <thead>
                    <tr>
                        <th>時間</th>
                        <th>人員</th>
                        <th>階段狀態</th>
                        <th>說明</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td colspan="4" style="text-align: center; padding: 20px; color: #dc3545;">
                            <i class="fas fa-exclamation-triangle"></i> ${errorMessage}
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    `;
}

// 顯示審查意見回覆
function showReviewComments(projectId) {
    // 顯示 Modal 並設定載入狀態
    const modal = new bootstrap.Modal(document.getElementById('planCommentModal'));
    showCommentLoadingState();
    modal.show();
    
    // 發送 AJAX 請求載入審查意見資料
    loadReviewCommentsData(projectId);
}

// 載入審查意見回覆資料
function loadReviewCommentsData(projectId) {
    $.ajax({
        type: 'POST',
        url: 'ApplicationChecklist.aspx/GetReviewComments',
        data: JSON.stringify({ projectId: projectId }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function(response) {
            if (response.d && response.d.success) {
                displayReviewCommentsData(response.d.data);
            } else {
                showCommentErrorState(response.d ? response.d.message : '載入審查意見資料失敗');
            }
        },
        error: function(xhr, status, error) {
            console.error('AJAX Error:', error);
            showCommentErrorState('載入審查意見資料時發生錯誤');
        }
    });
}

// 顯示審查意見載入狀態
function showCommentLoadingState() {
    // 清空計畫基本資訊
    document.getElementById('projectIdDisplay').textContent = '';
    document.getElementById('projectYearDisplay').textContent = '';
    document.getElementById('projectCategoryDisplay').textContent = '';
    document.getElementById('reviewGroupDisplay').textContent = '';
    document.getElementById('applicantUnitDisplay').textContent = '';
    document.getElementById('projectNameDisplay').textContent = '';
    
    // 顯示載入中
    const tableBody = document.getElementById('reviewCommentsTableBody');
    tableBody.innerHTML = '<tr><td colspan="3" class="text-center p-4">載入中...</td></tr>';
}

// 顯示審查意見資料
function displayReviewCommentsData(data) {
    if (!data) {
        showCommentErrorState('沒有資料');
        return;
    }
    
    // 填入計畫基本資訊
    if (data.projectInfo) {
        document.getElementById('projectIdDisplay').textContent = data.projectInfo.ProjectID || '';
        document.getElementById('projectYearDisplay').textContent = data.projectInfo.year || '';
        document.getElementById('projectCategoryDisplay').textContent = '科專'; // 預設值或從資料取得
        document.getElementById('reviewGroupDisplay').textContent = data.projectInfo.reviewGroup || '';
        document.getElementById('applicantUnitDisplay').textContent = data.projectInfo.applicantUnit || '';  
        document.getElementById('projectNameDisplay').textContent = data.projectInfo.projectName || '';
    }
    
    // 建立審查意見表格內容 (遵照原始 UI 結構)
    const tableBody = document.getElementById('reviewCommentsTableBody');
    let tableRows = '';
    
    if (data.reviewComments && data.reviewComments.length > 0) {
        data.reviewComments.forEach(function(comment) {
            tableRows += `
                <tr>
                    <td>${comment.reviewerName || ''}</td>
                    <td>${comment.reviewComment || ''}</td>
                    <td>
                        <span class="form-control textarea" role="textbox" contenteditable="" 
                              data-placeholder="請輸入" aria-label="文本輸入區域" 
                              data-review-id="${comment.reviewerReviewID}"
                              style="height: auto; overflow-y: hidden;">${comment.replyComment || ''}</span>
                    </td>
                </tr>
            `;
        });
    } else {
        tableRows = `
            <tr>
                <td colspan="3" class="text-center p-4 text-muted">
                    <i class="fas fa-info-circle"></i> 尚未有審查意見
                </td>
            </tr>
        `;
    }
    
    tableBody.innerHTML = tableRows;
    
    // 檢查是否已有回覆，如果有則禁用提送按鈕
    const submitButton = document.querySelector('button[onclick="submitReply()"]');
    if (submitButton && data.reviewComments && data.reviewComments.length > 0) {
        const hasReply = data.reviewComments.some(comment => 
            comment.replyComment && comment.replyComment.trim() !== ''
        );
        
        if (hasReply) {
            submitButton.disabled = true;
            submitButton.innerHTML = '<i class="fas fa-check"></i> 已提送回覆';
            submitButton.classList.add('btn-secondary');
            submitButton.classList.remove('btn-teal');
        } else {
            submitButton.disabled = false;
            submitButton.innerHTML = '<i class="fas fa-check"></i> 提送回覆';
            submitButton.classList.add('btn-teal');
            submitButton.classList.remove('btn-secondary');
        }
    }
}

// 顯示審查意見錯誤狀態
function showCommentErrorState(message) {
    const tableBody = document.getElementById('reviewCommentsTableBody');
    tableBody.innerHTML = `<tr><td colspan="3" class="text-center p-4 text-danger">載入失敗: ${message}</td></tr>`;
    
    // 清空計畫基本資訊
    document.getElementById('projectIdDisplay').textContent = '';
    document.getElementById('projectYearDisplay').textContent = '';
    document.getElementById('projectCategoryDisplay').textContent = '';
    document.getElementById('reviewGroupDisplay').textContent = '';
    document.getElementById('applicantUnitDisplay').textContent = '';
    document.getElementById('projectNameDisplay').textContent = '';
}

// 儲存回覆內容 (根據原始 UI 使用 contenteditable span)
function saveReply(reviewId) {
    const editableSpan = document.querySelector(`span[data-review-id="${reviewId}"]`);
    if (!editableSpan) {
        alert('找不到回覆內容');
        return;
    }
    
    const replyContent = editableSpan.textContent.trim();
    if (!replyContent) {
        alert('請輸入回覆內容');
        return;
    }
    
    // TODO: 實作儲存回覆的 AJAX 請求
    console.log('儲存回覆:', reviewId, replyContent);
    alert('回覆儲存功能待實作');
}

// 提送回覆功能
function submitReply() {
    // 收集所有回覆內容
    const replySpans = document.querySelectorAll('span[data-review-id]');
    const replies = [];
    
    replySpans.forEach(span => {
        const reviewId = span.getAttribute('data-review-id');
        const replyContent = span.textContent.trim();
        
        if (reviewId && replyContent) {
            replies.push({
                reviewId: reviewId,
                replyContent: replyContent
            });
        }
    });
    
    if (replies.length === 0) {
        alert('請至少輸入一項回覆內容');
        return;
    }
    
    // 禁用按鈕防止重複提送
    const submitButton = document.querySelector('button[onclick="submitReply()"]');
    if (submitButton) {
        submitButton.disabled = true;
        submitButton.innerHTML = '<i class="fas fa-spinner fa-spin"></i> 提送中...';
    }
    
    // 發送 AJAX 請求
    $.ajax({
        type: 'POST',
        url: 'ApplicationChecklist.aspx/SubmitReply',
        data: JSON.stringify({ replies: replies }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function(response) {
            if (response.d && response.d.success) {
                alert('回覆提送成功');
                // 關閉 Modal
                const modal = bootstrap.Modal.getInstance(document.getElementById('planCommentModal'));
                if (modal) {
                    modal.hide();
                }
            } else {
                alert('提送失敗: ' + (response.d ? response.d.message : '未知錯誤'));
                // 恢復按鈕狀態
                if (submitButton) {
                    submitButton.disabled = false;
                    submitButton.innerHTML = '<i class="fas fa-check"></i> 提送回覆';
                }
            }
        },
        error: function(xhr, status, error) {
            console.error('AJAX Error:', error);
            alert('提送回覆時發生錯誤');
            // 恢復按鈕狀態
            if (submitButton) {
                submitButton.disabled = false;
                submitButton.innerHTML = '<i class="fas fa-check"></i> 提送回覆';
            }
        }
    });
}