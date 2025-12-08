/**
 * ApplicationChecklist.js
 * 申請案件清單頁面的JavaScript功能
 */

// 全局變量
let tabsInitialized = false;

// 純前端分頁系統全局變數
let currentPageState = {
    pageNumber: 1,
    pageSize: 5,
    totalPages: 1,
    totalRecords: 0,
    searchText: '',
    contentKeyword: '',
    year: '',
    status: '',
    stage: '',
    department: '',
    reviewer: '',
    waitingReply: false,
    selectedStage: '總申請'
};

// 儲存完整的篩選後資料集
let filteredDataSet = [];

// 更新表格資料
function updateTableData(htmlContent) {
    var tableBody = document.getElementById('dataTableBody');
    if (tableBody) {
        tableBody.innerHTML = htmlContent;
        // 重新綁定事件
        bindCheckboxEvents();
        // 重新初始化 tooltips
        initTooltips();
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
//
// // 收集選中的案件
// function getSelectedCases() {
//     var selectedCases = [];
//     var checkedBoxes = document.querySelectorAll('.case-checkbox:checked');
//
//     checkedBoxes.forEach(function(checkbox) {
//         selectedCases.push({
//             projectId: checkbox.getAttribute('data-projectid'),
//             status: checkbox.getAttribute('data-status'),
//             stage: checkbox.getAttribute('data-stage')
//         });
//     });
//
//     return selectedCases;
// }

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

            // 使用新的 AJAX 篩選方式
            filterByCategory(categoryText);
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

// 排序功能 (支援純前端分頁)
function initSortButtons() {
    document.querySelectorAll("thead .sort").forEach((btn) => {
        btn.addEventListener("click", (e) => {
            e.preventDefault(); // 避免 PostBack

            // 如果沒有資料就直接返回
            if (filteredDataSet.length === 0) return;

            // 排序欄位索引對應的資料欄位
            const colIndex = parseInt(btn.dataset.col);
            const sortField = getSortFieldByColumn(colIndex);

            if (!sortField) return;

            // 判斷升序或降序
            const isAsc = !btn.classList.contains("up");

            // 清除其他按鈕狀態
            document.querySelectorAll("thead .sort").forEach(b => b.classList.remove("up", "down"));

            // 設定當前按鈕狀態
            btn.classList.toggle("up", isAsc);
            btn.classList.toggle("down", !isAsc);

            // 對完整資料集進行排序
            filteredDataSet.sort((a, b) => {
                const aValue = a[sortField] || '';
                const bValue = b[sortField] || '';

                // 嘗試數字排序
                const aNum = parseFloat(String(aValue).replace(/,/g, ''));
                const bNum = parseFloat(String(bValue).replace(/,/g, ''));

                const isANum = !isNaN(aNum) && /^\d+(\.\d+)?$/.test(String(aValue).replace(/,/g, ''));
                const isBNum = !isNaN(bNum) && /^\d+(\.\d+)?$/.test(String(bValue).replace(/,/g, ''));

                if (isANum && isBNum) {
                    return isAsc ? aNum - bNum : bNum - aNum;
                } else {
                    const aText = String(aValue);
                    const bText = String(bValue);
                    return isAsc
                        ? aText.localeCompare(bText, 'zh-Hant')
                        : bText.localeCompare(aText, 'zh-Hant');
                }
            });

            // 重新顯示當前頁面
            updateCurrentPageDisplay();
        });
    });
}

// 根據欄位索引取得對應的資料欄位名稱
function getSortFieldByColumn(colIndex) {
    const fieldMap = {
        1: 'Year',              // 年度
        2: 'ProjectID',         // 計畫編號
        3: 'ProjectNameTw',     // 計畫名稱
        4: 'OrgName',           // 申請單位
        5: 'Category',          // 類別
        6: 'Req_SubsidyAmount', // 申請補助金額
        7: 'Statuses',          // 階段
        8: 'StatusesName'       // 狀態
    };
    return fieldMap[colIndex];
}

// 初始化搜尋表單
function initSearchForm() {
    // 為搜尋表單添加Enter鍵支援
    const searchInputs = document.querySelectorAll('.search-form input, .search-form select');
    searchInputs.forEach(function(input) {
        input.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                performSearch();
            }
        });
    });

    // 綁定搜尋按鈕事件
    const searchBtn = document.querySelector('input[id*="btnSearch"]');
    if (searchBtn) {
        searchBtn.addEventListener('click', function(e) {
            e.preventDefault();
            performSearch();
        });
    }
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

        // 從表單讀取搜尋條件（可能來自 URL 參數）
        loadSearchConditionsFromForm();

        // 載入初始篩選資料
        loadFilteredData();
    }, 100);
});

// 初始化下拉選單切換功能
function initDropdownToggle() {
    // 讓 Bootstrap 處理下拉式選單，移除自定義邏輯
    // Bootstrap 5 會自動處理 data-bs-toggle="dropdown" 的行為

    // 如果需要額外的事件處理，可以監聽 Bootstrap 事件
    document.addEventListener('shown.bs.dropdown', function (event) {
        // 下拉選單顯示時的額外處理
        console.log('Dropdown shown:', event.target);
    });

    document.addEventListener('hidden.bs.dropdown', function (event) {
        // 下拉選單隱藏時的額外處理
        console.log('Dropdown hidden:', event.target);
    });
}

// 表格工具提示初始化
function initTooltips() {
    // 先銷毀所有現有的 tooltips
    var existingTooltips = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    existingTooltips.forEach(function(element) {
        var tooltip = bootstrap.Tooltip.getInstance(element);
        if (tooltip) {
            tooltip.dispose();
        }
    });

    // 重新初始化 Bootstrap tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    if (window.bootstrap && window.bootstrap.Tooltip) {
        var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
}

// 初始化分頁按鈕
function initPaginationButtons() {
    // 分頁按鈕已經在 ASPX 中設定好文字
    // 這裡只需要初始化分頁狀態
    updatePagination(1, 1); // 預設第1頁，總共1頁
}

// 更新分頁顯示
function updatePagination(currentPage, totalPages) {
    const paginationNav = document.getElementById('paginationNav');
    if (!paginationNav) return;

    const prevButton = document.getElementById('btnPrevPage');
    const nextButton = document.getElementById('btnNextPage');

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
            prevButton.classList.add('disabled');
        } else {
            prevButton.classList.remove('disabled');
        }
    }

    if (nextButton) {
        nextButton.disabled = currentPage >= totalPages;
        if (currentPage >= totalPages) {
            nextButton.classList.add('disabled');
        } else {
            nextButton.classList.remove('disabled');
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

    if (isActive) {
        // 當前頁面禁用按鈕
        button.disabled = true;
    } else {
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
    const pageDropdown = document.getElementById('ddlPageNumber');
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

// AJAX 載入篩選資料 (純前端分頁架構)
function loadFilteredData(showLoading = true) {
    if (showLoading) {
        showLoadingMessage();
    }

    // 準備 AJAX 請求參數 (不再包含分頁參數)
    const requestData = {
        searchText: currentPageState.searchText,
        contentKeyword: currentPageState.contentKeyword,
        year: currentPageState.year,
        status: currentPageState.status,
        stage: currentPageState.stage,
        department: currentPageState.department,
        reviewer: currentPageState.reviewer,
        waitingReply: currentPageState.waitingReply,
        selectedStage: currentPageState.selectedStage
    };


    $.ajax({
        type: 'POST',
        url: 'ApplicationChecklist.aspx/GetFilteredData',
        data: JSON.stringify(requestData),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function(response) {
            if (response.d && response.d.success) {
                const responseData = response.d;

                // 儲存完整的篩選後資料集
                filteredDataSet = responseData.data || [];

                // 更新總記錄數和重新計算分頁
                currentPageState.totalRecords = filteredDataSet.length;
                currentPageState.totalPages = Math.ceil(currentPageState.totalRecords / currentPageState.pageSize) || 1;

                // 確保當前頁面在有效範圍內
                if (currentPageState.pageNumber > currentPageState.totalPages) {
                    currentPageState.pageNumber = 1;
                }

                // 更新當前頁面的表格資料
                updateCurrentPageDisplay();

                // 更新分頁控制項
                updatePaginationControls();

                // 更新記錄資訊
                updateRecordInfo();

                // 更新標籤統計
                updateTabCounts(responseData.categoryCounts);
                setActiveTab(currentPageState.selectedStage);

                console.log('Filtered data loaded successfully:', {
                    totalRecords: currentPageState.totalRecords,
                    totalPages: currentPageState.totalPages,
                    currentPage: currentPageState.pageNumber,
                    dataLength: filteredDataSet.length
                });
            } else {
                showErrorMessage(response.d ? response.d.message : '載入資料失敗');
            }
        },
        error: function(xhr, status, error) {
            console.error('AJAX Error:', error);
            showErrorMessage('載入資料時發生錯誤');
        }
    });
}

// 更新當前頁面顯示 (純前端分頁)
function updateCurrentPageDisplay() {
    const startIndex = (currentPageState.pageNumber - 1) * currentPageState.pageSize;
    const endIndex = startIndex + currentPageState.pageSize;
    const currentPageData = filteredDataSet.slice(startIndex, endIndex);

    // 生成當前頁面的 HTML
    let tableHtml = '';
    if (currentPageData.length === 0) {
        tableHtml = '<tr><td colspan="9" style="text-align: center; padding: 20px; color: #6c757d;"><i class="fas fa-info-circle"></i> 沒有符合條件的資料</td></tr>';
    } else {
        currentPageData.forEach(record => {
            tableHtml += generateRowHtml(record);
        });
    }

    // 更新表格
    updateTableData(tableHtml);
}

// 生成單筆記錄的 HTML
function generateRowHtml(record) {
    // 處理狀態顯示：優先顯示撤案狀態
    let displayStatus = '';
    let statusClass = '';

    if (record.isWithdrawal === true) {
        displayStatus = '已撤案';
        statusClass = getStatusColorClass('已撤案');
    } else {
        displayStatus = record.StatusesName || '';
        statusClass = getStatusColorClass(record.StatusesName);
    }

    return `
        <tr>
            <td data-th="年度:">${record.Year || ''}</td>
            <td data-th="計畫編號:" style="text-align: left;" nowrap>${record.ProjectID || ''}</td>
            <td data-th="計畫名稱:" style="text-align: left;">
                ${generateProjectNameLink(record)}
            </td>
            <td data-th="申請單位:" style="text-align: left;">${record.OrgName || ''}</td>
            <td data-th="類別:">${record.Category || ''}</td>
            <td data-th="申請補助金額:">${record.Req_SubsidyAmount || '0'}</td>
            <td data-th="階段:" nowrap><span class="">${record.Statuses || ''}</span></td>
            <td data-th="狀態:" style="text-align: center;">
                <span class="${statusClass}">${displayStatus}</span>
            </td>
            <td data-th="功能:">
                <div class="d-flex align-items-center justify-content-end gap-1">
                    ${generateActionButtons(record)}
                </div>
            </td>
        </tr>
    `;
}

// 取得狀態顏色樣式
function getStatusColorClass(status) {
    if (!status) return '';

    switch (status.trim()) {
        case '補正補件':
        case '待回覆':
        case '待修正':
            return 'text-royal-blue';
        case '逾期未補':
        case '未通過':
        case '已撤案':
            return 'text-pink';
        case '已核定':
            return 'text-teal';
        case '尚未提送':
            return 'text-royal-blue';
        default:
            return '';
    }
}

// 生成操作按鈕
function generateActionButtons(record) {
    let buttons = '';

    if (!record.ProjectID) return buttons;

    const status = record.Statuses || '';
    const statusName = record.StatusesName || '';
    const isWithdrawn = record.isWithdrawal === true;

    // 編輯按鈕（只有特定狀態可編輯）
    if (canEdit(status, statusName)) {
        const editUrl = getEditUrl(record);
        if (editUrl && editUrl !== '#') {
            buttons += `<a href="${editUrl}" class="btn btn-sm btn-teal-dark" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="編輯">
                <i class="fa-solid fa-pen"></i>
            </a>`;
        }
    }

    // 上傳技術審查/初審檔案按鈕（只有在複審或技術審查階段顯示）
    if (status === '複審' || status === '技術審查') {
        buttons += `<button class="btn btn-sm btn-teal-dark" type="button" onclick="showUploadModal('${record.ProjectID}')"
                    data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="上傳 技術審查/複審 檔案">
                    <i class="fas fa-file-upload"></i>
                </button>`;
    }

    if((record.Category == "科專" || record.Category == "文化")&&(record.Statuses != "尚未提送"  && record.Statuses != "資格審查" )){
    // 回覆按鈕（檢視審查意見）
        buttons += `<button class="btn btn-sm btn-teal-dark" type="button" onclick="showReviewComments('${record.ProjectID}')"
                    data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="檢視審查意見">
                    <i class="fas fa-comment-dots"></i>
                </button>`;
    }
    // 歷程按鈕
    buttons += `<button class="btn btn-sm btn-teal-dark" type="button" onclick="showHistory('${record.ProjectID}')"
                data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="歷程">
                <i class="fas fa-history"></i>
            </button>`;

    // 更多操作選單
    buttons += `<div class="dropdown">
        <button class="btn btn-sm btn-outline-teal" type="button" data-bs-toggle="dropdown" aria-expanded="false">
            <i class="fas fa-ellipsis-h"></i>
        </button>
        <ul class="dropdown-menu" style="min-width: 120px;">`;

    // 撤案按鈕（只有在指定狀態下且未撤案時顯示）
    if (canWithdraw(status) && !isWithdrawn) {
        buttons += `<li><a class="dropdown-menu-item gap-1" href="#" onclick="handleWithdraw('${record.ProjectID}'); return false;">
            <i class="fas fa-redo text-teal-dark"></i>撤案
        </a></li>`;
    }

    // 刪除按鈕（只有「尚未提送」狀態可刪除）
    if (canDelete(status)) {
        buttons += `<li><a class="dropdown-menu-item gap-1" href="#" onclick="handleDelete('${record.ProjectID}'); return false;">
            <i class="fas fa-times text-teal-dark"></i>刪除
        </a></li>`;
    }

    // 恢復案件按鈕（只有已撤案的案件顯示）
    if (isWithdrawn) {
        buttons += `<li><a class="dropdown-menu-item gap-1" href="#" onclick="handleRestore('${record.ProjectID}')">
            <i class="fas fa-undo text-teal-dark"></i>恢復案件
        </a></li>`;
    }

    buttons += `</ul></div>`;

    return buttons;
}

// 檢查是否可編輯
function canEdit(status, statusName) {
    if (status === '決審核定') {
        return statusName === '計畫書修正中';
    } else {
        return statusName === '編輯中' || statusName === '補正補件';
    }
}

// 檢查是否可撤案
function canWithdraw(status) {
    const withdrawableStatuses = ['資格審查', '內容審查', '實質審查', '初審', '技術審查', '複審', '決審核定'];
    return withdrawableStatuses.includes(status);
}

// 檢查是否可刪除
function canDelete(status) {
    return status === '尚未提送';
}

// 取得編輯頁面的網址
function getEditUrl(record) {
    if (!record.ProjectID) return '#';

    const category = record.Category;
    switch (category) {
        case '科專':
            return `SCI/SciApplication.aspx?ProjectID=${record.ProjectID}`;
        case '文化':
            return `CUL/Application.aspx?ID=${record.ProjectID}`;
        case '學校社團':
            return `Clb/ClbApplication.aspx?ProjectID=${record.ProjectID}`;
        case '學校民間':
            return `EDC/Application.aspx?ID=${record.ProjectID}`;
        case '多元':
            return `MUL/Application.aspx?ID=${record.ProjectID}`;
        case '素養':
            return `LIT/Application.aspx?ID=${record.ProjectID}`;
        case '無障礙':
            return `ACC/Application.aspx?ID=${record.ProjectID}`;
        default:
            return '#'; // 尚未有對應的編輯頁面
    }
}

// 換頁功能 (純前端)
function changePage(direction) {
    if (direction === 'prev' && currentPageState.pageNumber > 1) {
        currentPageState.pageNumber--;
        updateCurrentPageDisplay();
        updatePaginationControls();
    } else if (direction === 'next' && currentPageState.pageNumber < currentPageState.totalPages) {
        currentPageState.pageNumber++;
        updateCurrentPageDisplay();
        updatePaginationControls();
    }
}

// 跳轉到指定頁面 (純前端)
function goToPage(pageNumber) {
    pageNumber = parseInt(pageNumber);
    if (pageNumber >= 1 && pageNumber <= currentPageState.totalPages) {
        currentPageState.pageNumber = pageNumber;
        updateCurrentPageDisplay();
        updatePaginationControls();
    }
}

// 改變每頁筆數 (純前端)
function changePageSize() {
    const pageSizeSelect = document.getElementById('ddlPageSize');

    if (pageSizeSelect) {
        currentPageState.pageSize = parseInt(pageSizeSelect.value);
        currentPageState.pageNumber = 1; // 重置到第一頁

        // 重新計算總頁數
        currentPageState.totalPages = Math.ceil(currentPageState.totalRecords / currentPageState.pageSize) || 1;

        // 更新顯示
        updateCurrentPageDisplay();
        updatePaginationControls();
    }
}

// 從表單載入搜尋條件到 currentPageState
function loadSearchConditionsFromForm() {
    currentPageState.searchText = document.querySelector('input[id*="txtSearch"]')?.value || '';
    currentPageState.contentKeyword = document.querySelector('input[id*="txtContentKeyword"]')?.value || '';
    currentPageState.year = document.querySelector('select[id*="ddlYear"]')?.value || '';
    currentPageState.status = document.querySelector('select[id*="ddlStatus"]')?.value || '';
    currentPageState.stage = document.querySelector('select[id*="ddlStage"]')?.value || '';
    currentPageState.department = document.querySelector('input[id*="txtDepartment"]')?.value || '';
    currentPageState.reviewer = document.querySelector('select[id*="ddlReviewer"]')?.value || '';
    currentPageState.waitingReply = document.querySelector('input[id*="waitingReply"]')?.checked || false;

    // 讀取隱藏欄位的選中標籤類別
    const hidSelectedStage = document.querySelector('input[id*="hidSelectedStage"]')?.value;
    if (hidSelectedStage) {
        currentPageState.selectedStage = hidSelectedStage;
    }

    console.log('從表單載入搜尋條件:', currentPageState);
}

// 執行搜尋
function performSearch() {
    // 收集搜尋條件
    loadSearchConditionsFromForm();

    // 重置到第一頁
    currentPageState.pageNumber = 1;

    // 載入篩選資料
    loadFilteredData();
}

// 標籤篩選
function filterByCategory(category) {
    currentPageState.selectedStage = category;
    currentPageState.pageNumber = 1; // 重置到第一頁
    loadFilteredData();
}

// 產生計畫名稱連結
function generateProjectNameLink(record) {
    const projectId = record.ProjectID || '';
    const projectName = record.ProjectNameTw || '';

    if (!projectId || !projectName) {
        return projectName;
    }

    // 檢查 ProjectID 是否包含 SCI 來決定路由
    if (projectId.includes('SCI')) {
        return `<a href="../OFS/SCI/SciApplication.aspx?ProjectID=${projectId}" class="link-black" target="_blank">${projectName}</a>`;
    } else if (projectId.includes('CUL'))  {
        return `<a href="../OFS/CUL/Application.aspx?ID=${projectId}" class="link-black" target="_blank">${projectName}</a>`;
    } else if (projectId.includes('EDC'))  {
        return `<a href="../OFS/EDC/Application.aspx?ID=${projectId}" class="link-black" target="_blank">${projectName}</a>`;
    }else if (projectId.includes('CLB'))  {
        return `<a href="../OFS/CLB/ClbApplication.aspx?ProjectID=${projectId}" class="link-black" target="_blank">${projectName}</a>`;
    } else if (projectId.includes('MUL'))  {
        return `<a href="../OFS/MUL/Application.aspx?ID=${projectId}" class="link-black" target="_blank">${projectName}</a>`;
    } else if (projectId.includes('LIT'))  {
        return `<a href="../OFS/LIT/Application.aspx?ID=${projectId}" class="link-black" target="_blank">${projectName}</a>`;
    } else if (projectId.includes('ACC'))  {
        return `<a href="../OFS/ACC/Application.aspx?ID=${projectId}" class="link-black" target="_blank">${projectName}</a>`;
    }
}

// 顯示載入訊息
function showLoadingMessage() {
    const tableBody = document.getElementById('dataTableBody');
    if (tableBody) {
        tableBody.innerHTML = '<tr><td colspan="9" style="text-align: center; padding: 20px;"><i class="fas fa-spinner fa-spin"></i> 載入中...</td></tr>';
    }
}

// 顯示錯誤訊息
function showErrorMessage(message) {
    const tableBody = document.getElementById('dataTableBody');
    if (tableBody) {
        tableBody.innerHTML = `<tr><td colspan="9" style="text-align: center; padding: 20px; color: #dc3545;"><i class="fas fa-exclamation-triangle"></i> ${message}</td></tr>`;
    }
}

// 更新記錄資訊
function updateRecordInfo() {
    const recordInfo = document.querySelector('span.text-teal');
    if (recordInfo) {
        recordInfo.textContent = currentPageState.totalRecords;
    }
}

// 更新分頁控制項
function updatePaginationControls() {
    // 統一使用 updatePagination 方法來更新所有分頁相關 UI
    updatePagination(currentPageState.pageNumber, currentPageState.totalPages);

    // 更新每頁筆數下拉選單
    const pageSizeDropdown = document.getElementById('ddlPageSize');
    if (pageSizeDropdown) {
        pageSizeDropdown.value = currentPageState.pageSize;
    }
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

    // 顯示載入中到兩個表格
    const domainTableBody = document.getElementById('domainReviewCommentsTableBody');
    const technicalTableBody = document.getElementById('technicalReviewCommentsTableBody');

    if (domainTableBody) {
        domainTableBody.innerHTML = '<tr><td colspan="3" class="text-center p-4">載入中...</td></tr>';
    }

    if (technicalTableBody) {
        technicalTableBody.innerHTML = '<tr><td colspan="3" class="text-center p-4">載入中...</td></tr>';
    }
}

// 顯示審查意見資料
function displayReviewCommentsData(data) {
    if (!data) {
        showCommentErrorState('沒有資料');
        return;
    }

    // 填入計畫基本資訊
    if (data.projectInfo) {
        const projectId = data.projectInfo.ProjectID || '';
        document.getElementById('projectIdDisplay').textContent = projectId;
        document.getElementById('projectYearDisplay').textContent = data.projectInfo.year || '';
        document.getElementById('projectCategoryDisplay').textContent = '科專'; // 預設值或從資料取得
        document.getElementById('reviewGroupDisplay').textContent = data.projectInfo.reviewGroup || '';
        document.getElementById('applicantUnitDisplay').textContent = data.projectInfo.applicantUnit || '';
        document.getElementById('projectNameDisplay').textContent = data.projectInfo.projectName || '';

        // 根據專案類型設定審查階段標題和按鈕文字
        updateReviewSectionTitles(projectId);
    }

    // 建立實質審查意見表格
    const domainTableBody = document.getElementById('domainReviewCommentsTableBody');
    let domainTableRows = '';

    if (data.domainReviewComments && data.domainReviewComments.length > 0) {
        data.domainReviewComments.forEach(function(comment) {
            domainTableRows += `
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
        domainTableRows = `
            <tr>
                <td colspan="3" class="text-center p-4 text-muted">
                    <i class="fas fa-info-circle"></i> 尚未有實質審查意見
                </td>
            </tr>
        `;
    }
    domainTableBody.innerHTML = domainTableRows;

    // 建立技術審查意見表格
    const technicalTableBody = document.getElementById('technicalReviewCommentsTableBody');
    let technicalTableRows = '';

    if (data.technicalReviewComments && data.technicalReviewComments.length > 0) {
        data.technicalReviewComments.forEach(function(comment) {
            technicalTableRows += `
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
        technicalTableRows = `
            <tr>
                <td colspan="3" class="text-center p-4 text-muted">
                    <i class="fas fa-info-circle"></i> 尚未有技術審查意見
                </td>
            </tr>
        `;
    }
    technicalTableBody.innerHTML = technicalTableRows;

    // 檢查提送按鈕狀態
    const submitButton = document.querySelector('button[onclick="submitReply()"]');
    if (submitButton) {
        const allComments = [...(data.domainReviewComments || []), ...(data.technicalReviewComments || [])];

        // 情況1：還沒有領域或技術審查意見 → 鎖定
        if (allComments.length === 0) {
            submitButton.disabled = true;
            submitButton.innerHTML = '<i class="fas fa-info-circle"></i> 暫無審查意見';
            submitButton.classList.add('btn-secondary');
            submitButton.classList.remove('btn-teal');
        }
        // 情況2：有審查意見，檢查回覆狀態
        else {
            // 檢查是否有任何回覆是 null 或空值
            const hasAnyEmptyReply = allComments.some(comment =>
                !comment.replyComment || comment.replyComment.trim() === ''
            );

            if (hasAnyEmptyReply) {
                // 開放回覆
                submitButton.disabled = false;
                submitButton.innerHTML = '<i class="fas fa-check"></i> 提送回覆';
                submitButton.classList.add('btn-teal');
                submitButton.classList.remove('btn-secondary');
            } else {
                // 全部都有回覆 → 鎖定（已提送狀態）
                submitButton.disabled = true;
                submitButton.innerHTML = '<i class="fas fa-check"></i> 已提送回覆';
                submitButton.classList.add('btn-secondary');
                submitButton.classList.remove('btn-teal');
            }
        }
    }
}

// 顯示審查意見錯誤狀態
function showCommentErrorState(message) {
    // 清空兩個表格
    const domainTableBody = document.getElementById('domainReviewCommentsTableBody');
    const technicalTableBody = document.getElementById('technicalReviewCommentsTableBody');

    if (domainTableBody) {
        domainTableBody.innerHTML = `<tr><td colspan="3" class="text-center p-4 "> ${message}</td></tr>`;
    }

    if (technicalTableBody) {
        technicalTableBody.innerHTML = `<tr><td colspan="3" class="text-center p-4 "> ${message}</td></tr>`;
    }

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
    // 收集實質審查回覆內容
    const domainReplySpans = document.querySelectorAll('#domainReviewCommentsTableBody span[data-review-id]');
    const domainReplies = [];

    domainReplySpans.forEach(span => {
        const reviewId = span.getAttribute('data-review-id');
        const replyContent = span.textContent.trim();

        if (reviewId && replyContent) {
            domainReplies.push({
                reviewId: reviewId,
                replyContent: replyContent
            });
        }
    });

    // 收集技術審查回覆內容
    const technicalReplySpans = document.querySelectorAll('#technicalReviewCommentsTableBody span[data-review-id]');
    const technicalReplies = [];

    technicalReplySpans.forEach(span => {
        const reviewId = span.getAttribute('data-review-id');
        const replyContent = span.textContent.trim();

        if (reviewId && replyContent) {
            technicalReplies.push({
                reviewId: reviewId,
                replyContent: replyContent
            });
        }
    });

    // 合併所有回覆
    const allReplies = [...domainReplies, ...technicalReplies];

    if (allReplies.length === 0) {
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
        data: JSON.stringify({ replies: allReplies }),
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

// 向後相容性別名 - 其他程式碼可能仍會呼叫 loadPageData
function loadPageData(showLoading = true) {
    loadFilteredData(showLoading);
}

// 匯出審查意見回覆表
function exportReviewCommentReply(reviewType) {
    // 取得當前顯示的計畫ID
    const projectId = document.getElementById('projectIdDisplay').textContent.trim();

    if (!projectId) {
        alert('無法取得計畫編號');
        return;
    }

    // 根據 reviewType 確認對應的審查意見資料
    let hasComments = false;
    let exportTypeName = '';

    if (reviewType === 'domain') {
        const domainComments = document.querySelectorAll('#domainReviewCommentsTableBody tr').length;
        hasComments = domainComments > 0;
        exportTypeName = '實質審查意見回覆表';
    } else if (reviewType === 'technical') {
        const technicalComments = document.querySelectorAll('#technicalReviewCommentsTableBody tr').length;
        hasComments = technicalComments > 0;
        exportTypeName = '技術審查意見回覆表';
    } else {
        alert('無效的審查類型');
        return;
    }

    if (!hasComments) {
        alert(`目前沒有${exportTypeName.replace('回覆表', '')}資料可供匯出`);
        return;
    }

    // 建立下載連結
    const downloadUrl = `../Service/DownloadApplicationChecklistFile.ashx?action=exportReviewCommentReply&projectId=${encodeURIComponent(projectId)}&reviewType=${encodeURIComponent(reviewType)}`;

    // 建立隱藏的下載連結並觸發下載
    const downloadLink = document.createElement('a');
    downloadLink.href = downloadUrl;
    downloadLink.download = `${projectId}_${exportTypeName}_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.docx`;
    downloadLink.style.display = 'none';

    document.body.appendChild(downloadLink);
    downloadLink.click();
    document.body.removeChild(downloadLink);
}

// 根據專案類型更新審查階段標題和按鈕文字
function updateReviewSectionTitles(projectId) {
    let domainTitle = '';
    let technicalTitle = '';
    let domainButtonText = '';
    let technicalButtonText = '';

    if (projectId.includes('SCI')) {
        // 科專專案
        domainTitle = '實質審查意見回覆';
        technicalTitle = '技術審查意見回覆';
        domainButtonText = '匯出實質審查意見回覆表';
        technicalButtonText = '匯出技術審查意見回覆表';
    } else if (projectId.includes('CUL')) {
        // 文化專案
        domainTitle = '初審意見回覆';
        technicalTitle = '複審意見回覆';
        domainButtonText = '匯出初審意見回覆表';
        technicalButtonText = '匯出複審意見回覆表';
    } else {
        // 預設值（其他專案類型）
        domainTitle = '實質審查意見回覆';
        technicalTitle = '技術審查意見回覆';
        domainButtonText = '匯出實質審查意見回覆表';
        technicalButtonText = '匯出技術審查意見回覆表';
    }

    // 更新標題 - 使用更精確的選擇器
    const titleElements = document.querySelectorAll('#planCommentModal h5.square-title');

    // 找到實質審查/初審標題（通常是第一個）
    titleElements.forEach((element, index) => {
        const text = element.textContent;
        if (text.includes('實質審查') || text.includes('初審')) {
            element.textContent = domainTitle;
        } else if (text.includes('技術審查') || text.includes('複審')) {
            element.textContent = technicalTitle;
        }
    });

    // 更新按鈕文字
    const domainButton = document.querySelector('button[onclick*="exportReviewCommentReply(\'domain\')"]');
    if (domainButton) {
        domainButton.innerHTML = `<i class="fas fa-download"></i>${domainButtonText}`;
    }

    const technicalButton = document.querySelector('button[onclick*="exportReviewCommentReply(\'technical\')"]');
    if (technicalButton) {
        technicalButton.innerHTML = `<i class="fas fa-download"></i>${technicalButtonText}`;
    }
}

// 匯出申請資料（計畫書 PDF）
function exportApplicationData() {
    // 取得當前顯示的計畫ID
    const projectId = document.getElementById('projectIdDisplay').textContent.trim();

    if (!projectId) {
        alert('無法取得計畫編號');
        return;
    }

    // 建立下載連結
    const downloadUrl = `../Service/DownloadApplicationChecklistFile.ashx?action=exportApplicationData&projectId=${encodeURIComponent(projectId)}`;

    // 建立隱藏的下載連結並觸發下載
    const downloadLink = document.createElement('a');
    downloadLink.href = downloadUrl;
    downloadLink.target = '_blank'; // 在新視窗開啟，類似 SciApplicationReview 的實作
    downloadLink.style.display = 'none';

    document.body.appendChild(downloadLink);
    downloadLink.click();
    document.body.removeChild(downloadLink);
}
