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
    }, 100);
});

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
function showHistory(versionId) {
    // 載入案件歷程資料並顯示模態框
    // 這裡可以根據需求實現具體功能
    const modal = new bootstrap.Modal(document.getElementById('planHistoryModal'));
    modal.show();
    // 可以在這裡發送AJAX請求載入歷程資料
}