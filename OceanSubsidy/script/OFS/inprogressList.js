/**
 * inprogressList.js - 計畫執行列表 JavaScript 功能
 */

$(document).ready(function() {
    // 初始化分頁管理器
    const paginationManager = {
        currentPage: 1,
        pageSize: 5,
        totalPages: 1,
        data: [],
        originalData: [], // 保存原始資料用於搜尋過濾
        currentFilter: 'all', // 當前篩選狀態
        overdueProjectIDs: new Set(), // 儲存進度落後的 ProjectID

        // 初始化
        init: function() {
            this.bindEvents();
            // 移除自動搜尋，等待後端資料載入
        },

        // 綁定事件
        bindEvents: function() {
            const self = this;

            // Tab 點擊事件 - 篩選資料
            $(document).on('click', '#statusTabsList .total-item', function(e) {
                e.preventDefault();
                const status = $(this).data('status');

                // 更新 active 狀態
                $('#statusTabsList .total-item').removeClass('active');
                $(this).addClass('active');

                // 應用篩選
                self.currentFilter = status;
                self.applyFilter();
            });

            // 移除搜尋按鈕點擊事件處理，由後端處理

            // 分頁按鈕點擊事件
            $(document).on('click', '.pagination-wrapper .pagination-btn', function(e) {
                e.preventDefault();
                const page = parseInt($(this).data('page'));
                if (page && page !== self.currentPage) {
                    self.currentPage = page;
                    self.renderPage();
                }
            });

            // 前一頁按鈕
            $(document).on('click', '#btnPrevPage', function(e) {
                e.preventDefault();
                if (self.currentPage > 1) {
                    self.currentPage--;
                    self.renderPage();
                }
            });

            // 下一頁按鈕
            $(document).on('click', '#btnNextPage', function(e) {
                e.preventDefault();
                if (self.currentPage < self.totalPages) {
                    self.currentPage++;
                    self.renderPage();
                }
            });

            // 每頁顯示筆數選擇
            $(document).on('change', '#ddlPageSize', function() {
                const newPageSize = parseInt($(this).val());
                if (newPageSize && newPageSize !== self.pageSize) {
                    self.pageSize = newPageSize;
                    self.calculateTotalPages(); // 重新計算總頁數

                    // 確保當前頁碼在有效範圍內
                    if (self.currentPage > self.totalPages) {
                        self.currentPage = self.totalPages;
                    }
                    if (self.currentPage < 1) {
                        self.currentPage = 1;
                    }

                    self.renderPage(); // 重新渲染頁面和分頁控制項
                }
            });

            // 跳到指定頁
            $(document).on('change', '#ddlGoToPage', function() {
                const targetPage = parseInt($(this).val());
                if (targetPage && targetPage !== self.currentPage && targetPage >= 1 && targetPage <= self.totalPages) {
                    self.currentPage = targetPage;
                    self.renderPage();
                }
            });
        },

        // 設定資料（由後端呼叫）
        setData: function(data) {
            this.originalData = data || [];
            this.data = [...this.originalData];
            this.currentPage = 1;
            this.currentFilter = 'all'; // 重設篩選狀態
            this.calculateTotalPages();
            this.renderPage();
            this.updateTotalRecords();
        },

        // 應用篩選
        applyFilter: function() {
            if (this.currentFilter === 'all') {
                // 顯示全部資料
                this.data = [...this.originalData];
            } else if (this.currentFilter === 'inprogress') {
                // 執行中: StatusName = '' 或 '審核中'
                this.data = this.originalData.filter(item => {
                    const status = (item.StatusName || '').trim();
                    return status === '' || status === '審核中';
                });
            } else if (this.currentFilter === 'closed') {
                // 已結案: StatusName = '已結案'
                this.data = this.originalData.filter(item => {
                    const status = (item.StatusName || '').trim();
                    return status === '已結案';
                });
            } else if (this.currentFilter === 'terminated') {
                // 已終止: StatusName = '已終止'
                this.data = this.originalData.filter(item => {
                    const status = (item.StatusName || '').trim();
                    return status === '已終止';
                });
            } else if (this.currentFilter === 'overdue') {
                // 進度落後: 執行中 + 有逾期待辦事項
                // 方法1: 使用後端提供的 IsOverdue 標記
                this.data = this.originalData.filter(item => {
                    const status = (item.StatusName || '').trim();
                    const isInProgress = (status === '' || status === '審核中');
                    const isOverdue = (item.IsOverdue === '1' || item.IsOverdue === 1 || item.IsOverdue === true);
                    return isInProgress && isOverdue;
                });

                // 方法2: 使用 overdueProjectIDs 集合（與方法1擇一使用）
                // this.data = this.originalData.filter(item => {
                //     const status = (item.StatusName || '').trim();
                //     const isInProgress = (status === '' || status === '審核中');
                //     return isInProgress && this.overdueProjectIDs.has(item.ProjectID);
                // });
            }

            // 重設到第一頁
            this.currentPage = 1;
            this.calculateTotalPages();
            this.renderPage();
            this.updateTotalRecords();
        },

        // 計算總頁數
        calculateTotalPages: function() {
            this.totalPages = Math.ceil(this.data.length / this.pageSize);
            if (this.totalPages === 0) this.totalPages = 1;
        },

        // 渲染頁面
        renderPage: function() {
            this.renderTable();
            this.updatePaginationControls();
            this.updatePageNumberControls();
        },

        // 渲染表格
        renderTable: function() {
            const $tableBody = $('#tableBody');
            $tableBody.empty();

            if (this.data.length === 0) {
                $tableBody.append('<tr><td colspan="8" class="text-center">無資料</td></tr>');
                return;
            }

            const startIndex = (this.currentPage - 1) * this.pageSize;
            const endIndex = Math.min(startIndex + this.pageSize, this.data.length);
            const pageData = this.data.slice(startIndex, endIndex);

            pageData.forEach((item, index) => {
                const isAlternate = index % 2 === 1;
                const rowClass = isAlternate ? 'table-row-alternate' : '';

                const row = `
                    <tr class="${rowClass}">
                        <td data-th="年度:">${item.Year || ''}</td>
                        <td data-th="類別:">${this.getCategoryName(item.Category)}</td>
                        <td data-th="計畫編號:" style="text-align: left;" nowrap>${item.ProjectID || ''}</td>
                        <td data-th="計畫名稱:" style="text-align: left;">
                            ${this.generateProjectNameLink(item)}
                        </td>
                        <td data-th="執行單位:" style="text-align: left;">${item.OrgName || ''}</td>
                        <td data-th="完成狀態:">${item.LastOperation || ''}</td>
                        <td data-th="待辦事項:"><span class="text-teal">${item.TaskName || ''}</span></td>
                        <td data-th="功能:">
                            <div class="d-flex align-items-center justify-content-center gap-1">
                                ${this.generateFunctionButton(item)}
                            </div>
                        </td>
                    </tr>
                `;
                $tableBody.append(row);
            });
        },

        // 將類別代碼轉換為中文名稱
        getCategoryName: function(category) {
            const categoryMap = {
                'SCI': '科專',
                'CUL': '文化',
                'EDC': '學校民間',
                'CLB': '學校社團',
                'MUL': '多元',
                'LIT': '素養',
                'ACC': '無障礙'
            };
            return categoryMap[category] || category || '';
        },

        // 生成計畫名稱連結
        generateProjectNameLink: function(item) {
            const category = item.Category || '';
            const projectNameTw = item.ProjectNameTw || '';
            const projectID = item.ProjectID || '';
            const basePath = window.location.origin + window.AppRootPath; // 從 AppRootPath ,正式區路經 ， web.config 取得

            if (category === 'SCI' && projectID) {
                const href = `${basePath}/OFS/SCI/SciInprogress_Approved.aspx?ProjectID=${encodeURIComponent(projectID)}`;
                return `<a class="link-black" href="${href}">${projectNameTw}</a>`;
            }if (category === 'CLB' && projectID) {
                const href = `${basePath}/OFS/Clb/ClbApproved.aspx?ProjectID=${encodeURIComponent(projectID)}`;
                return `<a class="link-black" href="${href}">${projectNameTw}</a>`;
            }
            else {
                const href = `${basePath}/OFS/${category}/Audit.aspx?ID=${encodeURIComponent(projectID)}`;
                return `<a class="link-black" href="${href}">${projectNameTw}</a>`;
            }
        },

        // 生成功能按鈕
        generateFunctionButton: function(item) {
            const category = item.Category || '';
            const taskNameEn = item.TaskNameEn || '';
            const projectID = item.ProjectID || '';
            const taskName = item.TaskName || '';
            let href = window.location.origin + window.AppRootPath;

            // 如果沒有待辦事項，不顯示按鈕
            if (!taskName.trim()) {
                return '';
            }
            // 針對 SCI 類別處理不同的 TaskNameEn
            if (category === 'SCI') {
                let buttonText = '編輯';

                switch (taskNameEn) {
                    case 'Contract':
                        href += `/OFS/SCI/SciInprogress_Contract.aspx?ProjectID=${encodeURIComponent(projectID)}`;
                        break;
                    case 'Payment1':
                        href += `/OFS/SCI/SciReimbursement.aspx?ProjectID=${encodeURIComponent(projectID)}&stage=1`;
                        break;
                    case 'Schedule':
                        href += `/OFS/SCI/SciInprogress_PreProgress.aspx?ProjectID=${encodeURIComponent(projectID)}`;
                        break;
                    case 'MidReport':
                        href += `/OFS/SCI/SciInterimReport.aspx?ProjectID=${encodeURIComponent(projectID)}&stage=1`;
                        break;
                    case 'FinalReport':
                        href += `/OFS/SCI/SciInterimReport.aspx?ProjectID=${encodeURIComponent(projectID)}&stage=2`;
                        break;
                    case 'MonthlyReport':
                        href += `/OFS/SCI/SciMonthlyExecutionReport.aspx?ProjectID=${encodeURIComponent(projectID)}`;
                        break;
                    case 'Payment2':
                        href += `/OFS/SCI/SciReimbursement.aspx?ProjectID=${encodeURIComponent(projectID)}&stage=2`;
                        break;
                    default:
                        return ``;
                }
                
            }else if (category == 'CLB'){
                switch (taskNameEn) {
                    case 'Change' :
                        href += `/OFS/CLB/ClbApproved.aspx?ProjectID=${encodeURIComponent(projectID)}`;
                        break;
                    case 'Report' :
                        href += `/OFS/CLB/ClbStageReport.aspx?ProjectID=${encodeURIComponent(projectID)}`;
                        break;
                    case 'Payment' :
                        href += `/OFS/CLB/ClbPayment.aspx?ProjectID=${encodeURIComponent(projectID)}`;
                        break;
                    default:
                        return ``;
                }
            } 
            else {
                switch (taskNameEn) {
                    case 'MonthlyReport':
                        href += `/OFS/${category}/Progress.aspx?ID=${encodeURIComponent(projectID)}`;
                        break;
                    case 'MidReport':
                        href += `/OFS/${category}/Report.aspx?ID=${encodeURIComponent(projectID)}&stage=1`;
                        break;
                    case 'FinalReport':
                        href += `/OFS/${category}/Report.aspx?ID=${encodeURIComponent(projectID)}&stage=2`;
                        break;
                    case 'Payment1':
                        href += `/OFS/${category}/Payment.aspx?ID=${encodeURIComponent(projectID)}&stage=1`;
                        break;
                    case 'Payment2':
                        href += `/OFS/${category}/Payment.aspx?ID=${encodeURIComponent(projectID)}&stage=2`;
                        break;
                    default:
                        href += `/OFS/${category}/Audit.aspx?ID=${encodeURIComponent(projectID)}`;
                }

            }
            return `<a href="${href}" class="btn btn-sm btn-teal-dark" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="編輯"><i class="fa-solid fa-pen"></i></a>`;
            // 預設按鈕（非 SCI 或無對應 TaskNameEn）
            // return `<button class="btn btn-sm btn-teal-dark" type="button">
            //             <i class="fa-solid fa-pen" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="編輯"></i>
            //         </button>`;
        },

        // 更新分頁控制項
        updatePaginationControls: function() {
            const $paginationNav = $('#paginationNav');
            if (!$paginationNav.length) return;

            const $prevButton = $('#btnPrevPage');
            const $nextButton = $('#btnNextPage');

            // 清除現有的頁碼按鈕（保留前後頁按鈕）
            $paginationNav.find('.pagination-item, .ellipsis').remove();

            // 生成頁碼按鈕並插入到下一頁按鈕前
            const pageButtons = this.generatePageButtons();
            pageButtons.forEach(button => {
                $nextButton.before(button);
            });

            // 更新按鈕狀態（參考 ApplicationChecklist 的樣式）
            if ($prevButton.length) {
                $prevButton.prop('disabled', this.currentPage <= 1);
                if (this.currentPage <= 1) {
                    $prevButton.addClass('disabled');
                } else {
                    $prevButton.removeClass('disabled');
                }
            }

            if ($nextButton.length) {
                $nextButton.prop('disabled', this.currentPage >= this.totalPages);
                if (this.currentPage >= this.totalPages) {
                    $nextButton.addClass('disabled');
                } else {
                    $nextButton.removeClass('disabled');
                }
            }
        },

        // 生成頁碼按鈕陣列（參考 ApplicationChecklist 的邏輯）
        generatePageButtons: function() {
            const buttons = [];
            const currentPage = this.currentPage;
            const totalPages = this.totalPages;

            if (totalPages <= 7) {
                // 頁數少時，顯示所有頁碼
                for (let i = 1; i <= totalPages; i++) {
                    buttons.push(this.createPageButtonElement(i, i === currentPage));
                }
            } else {
                // 頁數多時，使用省略號
                if (currentPage <= 4) {
                    // 當前頁在前面
                    for (let i = 1; i <= 5; i++) {
                        buttons.push(this.createPageButtonElement(i, i === currentPage));
                    }
                    buttons.push(this.createEllipsisElement());
                    buttons.push(this.createPageButtonElement(totalPages, false));
                } else if (currentPage >= totalPages - 3) {
                    // 當前頁在後面
                    buttons.push(this.createPageButtonElement(1, false));
                    buttons.push(this.createEllipsisElement());
                    for (let i = totalPages - 4; i <= totalPages; i++) {
                        buttons.push(this.createPageButtonElement(i, i === currentPage));
                    }
                } else {
                    // 當前頁在中間
                    buttons.push(this.createPageButtonElement(1, false));
                    buttons.push(this.createEllipsisElement());
                    for (let i = currentPage - 1; i <= currentPage + 1; i++) {
                        buttons.push(this.createPageButtonElement(i, i === currentPage));
                    }
                    buttons.push(this.createEllipsisElement());
                    buttons.push(this.createPageButtonElement(totalPages, false));
                }
            }

            return buttons;
        },

        // 創建頁碼按鈕元素（參考 ApplicationChecklist 的樣式）
        createPageButtonElement: function(pageNumber, isActive) {
            const $button = $(`<button type="button" class="pagination-item${isActive ? ' active' : ''}"><span class="page-number">${pageNumber}</span></button>`);

            if (!isActive) {
                $button.on('click', () => {
                    this.currentPage = pageNumber;
                    this.renderPage();
                });
            }

            return $button;
        },

        // 創建省略號元素（參考 ApplicationChecklist 的樣式）
        createEllipsisElement: function() {
            return $('<div class="pagination-item ellipsis"><span class="">...</span></div>');
        },

        // 更新分頁資訊
        updatePageNumberControls: function() {
            // 更新分頁資訊顯示
            const startRecord = this.data.length === 0 ? 0 : (this.currentPage - 1) * this.pageSize + 1;
            const endRecord = Math.min(this.currentPage * this.pageSize, this.data.length);
            const totalRecords = this.data.length;

            $('#paginationInfo').text(`顯示第 ${startRecord}-${endRecord} 筆，共 ${totalRecords} 筆`);

            // 更新跳頁下拉選單
            const $dropdown = $('#ddlGoToPage');
            $dropdown.empty();

            for (let i = 1; i <= this.totalPages; i++) {
                const $option = $(`<option value="${i}">${i}</option>`);
                if (i === this.currentPage) {
                    $option.prop('selected', true);
                }
                $dropdown.append($option);
            }

            $dropdown.prop('disabled', this.totalPages <= 1);
        },

        // 顯示載入狀態
        showLoading: function(show) {
            if (show) {
                // 可以在這裡添加載入動畫
                console.log('載入中...');
            } else {
                console.log('載入完成');
            }
        },

        // 更新總記錄數顯示
        updateTotalRecords: function() {
            const $totalElement = $('#totalRecordsSpan');
            if ($totalElement.length) {
                $totalElement.text(this.data.length);
            }
        },

        // HTML 轉義函數
        escapeHtml: function(text) {
            const map = {
                '&': '&amp;',
                '<': '&lt;',
                '>': '&gt;',
                '"': '&quot;',
                "'": '&#039;'
            };
            return text.replace(/[&<>"']/g, function(m) { return map[m]; });
        }
    };

    // 初始化分頁管理器
    paginationManager.init();

    // 將分頁管理器暴露到全域供後端呼叫
    window.inprogressPaginationManager = paginationManager;

    // 檢查是否有暫存的資料需要處理
    if (window.pendingPaginationData !== null) {
        paginationManager.setData(window.pendingPaginationData);
        window.pendingPaginationData = null; // 清空暫存
    }
});

// 暫存資料，等待分頁管理器初始化完成
window.pendingPaginationData = null;

// 提供給後端呼叫的全域函式
function updatePaginationData(jsonData) {
    try {
        const data = typeof jsonData === 'string' ? JSON.parse(jsonData) : jsonData;

        if (window.inprogressPaginationManager) {
            // 分頁管理器已就緒，直接設定資料
            window.inprogressPaginationManager.setData(data);
        } else {
            // 分頁管理器尚未就緒，暫存資料並等待初始化完成
            window.pendingPaginationData = data;

            // 如果 DOM 已經載入完成但分頁管理器還沒初始化，嘗試延遲處理
            if (document.readyState === 'complete' || document.readyState === 'interactive') {
                setTimeout(function() {
                    if (window.inprogressPaginationManager && window.pendingPaginationData !== null) {
                        window.inprogressPaginationManager.setData(window.pendingPaginationData);
                        window.pendingPaginationData = null;
                    }
                }, 100);
            }
        }
    } catch (error) {
        console.error('解析資料時發生錯誤:', error);
        if (window.inprogressPaginationManager) {
            window.inprogressPaginationManager.setData([]);
        } else {
            window.pendingPaginationData = [];
        }
    }
}

// 提供給後端呼叫的全域函式 - 更新統計數字
function updateStatistics(statsData) {
    try {
        const stats = typeof statsData === 'string' ? JSON.parse(statsData) : statsData;

        // 更新各 Tab 的數量顯示
        $('#countTotal').text(stats.Total || 0);
        $('#countInProgress').text(stats.InProgress || 0);
        $('#countOverdue').text(stats.Overdue || 0);
        $('#countClosed').text(stats.Closed || 0);
        $('#countTerminated').text(stats.Terminated || 0);

        // 從後端取得進度落後的 ProjectID 列表
        // 注意: 這部分需要後端提供完整的 ProjectID 列表,而不只是數量
        // 目前我們透過前端檢查 TaskQueue 的邏輯來判定
        if (window.inprogressPaginationManager && window.inprogressPaginationManager.originalData) {
            updateOverdueProjects(window.inprogressPaginationManager.originalData);
        }

    } catch (error) {
        console.error('更新統計數字時發生錯誤:', error);
    }
}

// 更新進度落後的 ProjectID 集合
function updateOverdueProjects(data) {
    if (!window.inprogressPaginationManager) return;

    // 清空現有集合
    window.inprogressPaginationManager.overdueProjectIDs.clear();

    // 透過資料中的標記來判斷是否為進度落後
    // 條件: 執行中 (StatusName = '' 或 '審核中') + IsOverdue = 1
    data.forEach(item => {
        const status = (item.StatusName || '').trim();
        const isInProgress = (status === '' || status === '審核中');
        const isOverdue = (item.IsOverdue === '1' || item.IsOverdue === 1 || item.IsOverdue === true);

        if (isInProgress && isOverdue) {
            window.inprogressPaginationManager.overdueProjectIDs.add(item.ProjectID);
        }
    });
}
