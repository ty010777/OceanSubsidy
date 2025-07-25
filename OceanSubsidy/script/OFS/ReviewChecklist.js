/**
 * ReviewChecklist.js
 * 審查清單頁面的JavaScript功能
 * 處理頁面切換、URL參數管理等功能
 */

window.ReviewChecklist = (function() {
    'use strict';

    // 私有變數
    var currentType = '1';
    var isInitialized = false;

    /**
     * 初始化模組
     */
    function init() {
        if (isInitialized) {
            return;
        }

        try {
            // 從URL取得當前審查類型
            currentType = getUrlParameter('type') || '1';
            // 綁定事件
            bindEvents();
            
            // 設定初始狀態
            setActiveType(currentType);
            
            isInitialized = true;
            console.log('ReviewChecklist模組已初始化');
        } catch (error) {
            console.error('初始化ReviewChecklist模組時發生錯誤:', error);
        }
    }

    /**
     * 綁定頁面事件
     */
    function bindEvents() {
        try {
            // 審查類型切換事件
            $('input[name="reviewType"]').on('change', function() {
                var selectedType = $(this).val();
                if (selectedType && selectedType !== currentType) {
                    switchReviewType(selectedType);
                }
            });

            // 監聽瀏覽器的前進/後退按鈕
            $(window).on('popstate', function(event) {
                var type = getUrlParameter('type') || '1';
                if (type !== currentType) {
                    // 重新載入頁面以觸發後端 LoadReviewContent
                    window.location.reload();
                }
            });
        } catch (error) {
            console.error('綁定事件時發生錯誤:', error);
        }
    }

    /**
     * 切換審查類型
     * @param {string} type - 審查類型 (1-4)
     */
    function switchReviewType(type) {
        try {
            type = type.toString();
            // 直接跳轉到新的 URL，觸發頁面重新載入和後端 LoadReviewContent
            var newUrl = updateUrlParameter(window.location.href, 'type', type);
            window.location.href = newUrl;
        } catch (error) {
            console.error('切換審查類型時發生錯誤:', error);
        }
    }

    /**
     * 設定選中的審查類型
     * @param {string} type - 審查類型
     */
    function setActiveType(type) {
        try {
            currentType = type;
            // 先移除所有 id 以 "total-item-" 開頭的元素的 active class
            $('[id^="total-item-"]').removeClass('active');

            // 把對應 id 的元素加上 active class
            $('#total-item-' + currentType).addClass('active');
            
            
            // 切換內容顯示
            $('.review-content').hide();
            $('#content-type-' + currentType).show();

            // 重新初始化當前內容區域的 checkbox 功能
            updateCheckboxTargets();

        } catch (error) {
            console.error('設定審查類型時發生錯誤:', error);
        }
    }


    /**
     * 驗證審查類型是否有效
     * @param {string} type - 審查類型
     * @returns {boolean} 是否有效
     */
  

    /**
     * 從URL取得參數值
     * @param {string} param - 參數名稱
     * @returns {string|null} 參數值
     */
    function getUrlParameter(param) {
        try {
            var urlParams = new URLSearchParams(window.location.search);
            return urlParams.get(param);
        } catch (error) {
            console.error('取得URL參數時發生錯誤:', error);
            return null;
        }
    }

    /**
     * 更新URL參數
     * @param {string} url - 原始URL
     * @param {string} param - 參數名稱
     * @param {string} value - 參數值
     * @returns {string} 更新後的URL
     */
    function updateUrlParameter(url, param, value) {
        try {
            var urlObj = new URL(url);
            urlObj.searchParams.set(param, value);
            return urlObj.toString();
        } catch (error) {
            console.error('更新URL參數時發生錯誤:', error);
            return url;
        }
    }

    /**
     * 重新綁定當前內容區域的 checkbox 功能
     */
    function updateCheckboxTargets() {
        try {
            const currentContent = $('#content-type-' + currentType);
            const checkAll = currentContent.find('.checkAll')[0];
            const checkPlan = currentContent.find('.checkPlan');
            const checkPlanBtnPanel = currentContent.find('.checkPlanBtnPanel')[0];

            if (!checkAll || !checkPlanBtnPanel || checkPlan.length === 0) {
                return;
            }

            // 檢查是否有任何項目被勾選，並控制批次按鈕面板的顯示
            function toggleBatchButtons() {
                let hasChecked = false;
                checkPlan.each(function() {
                    if (this.checked) {
                        hasChecked = true;
                    }
                });
                
                checkPlanBtnPanel.style.display = hasChecked ? 'block' : 'none';
            }

            // 移除舊的事件監聽器
            $(checkAll).off('change.reviewChecklist');
            checkPlan.off('change.reviewChecklist');

            // 全選按鈕事件
            $(checkAll).on('change.reviewChecklist', function() {
                checkPlan.each(function() {
                    this.checked = checkAll.checked;
                });
                toggleBatchButtons();
            });

            // 個別 checkbox 事件
            checkPlan.on('change.reviewChecklist', function() {
                let allChecked = true;
                checkPlan.each(function() {
                    if (!this.checked) {
                        allChecked = false;
                    }
                });
                checkAll.checked = allChecked;
                toggleBatchButtons();
            });

            // 初始化狀態
            toggleBatchButtons();
            
        } catch (error) {
            console.error('更新 checkbox 目標時發生錯誤:', error);
        }
    }

    /**
     * 取得當前審查類型
     * @returns {string} 當前審查類型
     */
    function getCurrentType() {
        return currentType;
    }

    /**
     * 收集選中的專案編號
     * @param {string} targetType - 指定審查類型，如果不指定則使用當前類型
     * @returns {Array<string>} 選中的專案編號陣列
     */
    function getSelectedProjectIds(targetType) {
        try {
            const type = targetType || currentType;
            const currentContent = $('#content-type-' + type);
            const selectedCheckboxes = currentContent.find('.checkPlan:checked');
            
            const projectIds = [];
            const invalidProjects = [];
            
            selectedCheckboxes.each(function() {
                const projectId = $(this).val();
                if (projectId && projectId.trim() !== '') {
                    
                    // 找到對應的表格行
                    const $checkbox = $(this);
                    const $row = $checkbox.closest('tr');
                    
                    // 從表格行中取得狀態名稱
                    const statusCell = $row.find('td[data-th="狀態:"] span').text().trim() ||
                                     $row.find('td[data-th="狀態:"]').text().trim() ||
                                     $row.find('[data-status-name]').attr('data-status-name') ||
                                     $row.find('.status-cell').text().trim();
                    
                    console.log(`專案 ${projectId} 的狀態: ${statusCell}`);
                    
                    // 檢查狀態是否為 '通過'
                    if (statusCell === '通過') {
                        projectIds.push(projectId.trim());
                    } else {
                        invalidProjects.push({
                            projectId: projectId.trim(),
                            status: statusCell
                        });
                    }
                }
            });
            
            // 如果有不符合條件的專案，顯示提醒
            if (invalidProjects.length > 0) {
                const invalidList = invalidProjects.map(p => `${p.projectId} (狀態: ${p.status})`).join('\n');
                console.warn('以下專案狀態不是「通過」，無法進行批次處理:', invalidList);
                
                // 如果所有選中項目都不符合條件
                if (projectIds.length === 0) {
                    Swal.fire({
                        title: '無法批次處理',
                        text: '只有狀態為「通過」的計畫才能進行批次處理\n\n選中的計畫狀態:\n' + invalidList,
                        icon: 'warning',
                        confirmButtonText: '確定',
                        confirmButtonColor: '#26A69A'
                    });
                    return [];
                }
                
                // 部分項目不符合條件的提醒
                Swal.fire({
                    title: '部分計畫無法處理',
                    text: `將處理 ${projectIds.length} 件狀態為「通過」的計畫\n\n以下計畫因狀態不符合而跳過:\n${invalidList}`,
                    icon: 'info',
                    confirmButtonText: '確定',
                    confirmButtonColor: '#26A69A'
                });
            }
            
            console.log(`Type${type} 符合條件的專案編號:`, projectIds);
            return projectIds;
        } catch (error) {
            console.error('收集選中專案編號時發生錯誤:', error);
            return [];
        }
    }

    /**
     * 批次通過確認功能
     * @param {string} actionText - 動作文字 (如: "轉入下一階段", "進入決審")
     * @param {string} targetType - 指定審查類型，如果不指定則使用當前類型
     * @returns {Promise} 返回 Promise，resolve 時包含選中的專案編號
     */
    function confirmBatchApproval(actionText, targetType) {
        return new Promise((resolve, reject) => {
            try {
                const selectedIds = getSelectedProjectIds(targetType);
                
                // 檢查是否有選中項目
                if (selectedIds.length === 0) {
                    Swal.fire({
                        title: '提醒',
                        text: '請先選擇要處理的計畫項目',
                        icon: 'warning',
                        confirmButtonText: '確定',
                        confirmButtonColor: '#26A69A'
                    });
                    reject(new Error('沒有選中項目'));
                    return;
                }

                // 顯示確認對話框
                Swal.fire({
                    title: `共 ${selectedIds.length} 件計畫`,
                    text: `確定批次通過，${actionText}？`,
                    icon: 'question',
                    showCancelButton: true,
                    confirmButtonText: '確定',
                    cancelButtonText: '取消',
                    confirmButtonColor: '#26A69A',
                    cancelButtonColor: '#d33',
                    reverseButtons: true
                }).then((result) => {
                    if (result.isConfirmed) {
                        resolve(selectedIds);
                    } else {
                        reject(new Error('用戶取消操作'));
                    }
                });
                
            } catch (error) {
                console.error('批次通過確認時發生錯誤:', error);
                Swal.fire({
                    title: '錯誤',
                    text: '處理批次操作時發生錯誤',
                    icon: 'error',
                    confirmButtonText: '確定',
                    confirmButtonColor: '#26A69A'
                });
                reject(error);
            }
        });
    }

    /**
     * 呼叫批次審核 API
     * @param {Array<string>} selectedIds - 選中的專案編號陣列
     * @param {string} actionType - 動作類型
     * @returns {Promise} 返回 AJAX Promise
     */
    function callBatchApprovalAPI(selectedIds, actionType) {
        return new Promise((resolve, reject) => {
            const requestData = {
                projectIds: selectedIds,
                actionType: actionType,
                reviewType: currentType
            };

            console.log('發送批次審核請求:', requestData);

            $.ajax({
                type: "POST",
                url: "ReviewChecklist.aspx/BatchApproveType1",
                data: JSON.stringify(requestData),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                timeout: 30000, // 30秒超時
                beforeSend: function() {
                    // 顯示載入中
                    Swal.fire({
                        title: '處理中...',
                        text: '正在執行批次操作，請稍候',
                        icon: 'info',
                        allowOutsideClick: false,
                        allowEscapeKey: false,
                        showConfirmButton: false,
                        didOpen: () => {
                            Swal.showLoading();
                        }
                    });
                }
            }).done(function(response) {
                console.log('後端回應:', response);
                Swal.close(); // 關閉載入中對話框
                resolve(response);
            }).fail(function(jqXHR, textStatus, errorThrown) {
                console.error('AJAX 請求失敗:', textStatus, errorThrown);
                Swal.close(); // 關閉載入中對話框
                reject({
                    status: jqXHR.status,
                    statusText: textStatus,
                    error: errorThrown,
                    responseText: jqXHR.responseText
                });
            });
        });
    }

    /**
     * 處理批次審核成功回應
     * @param {Object} response - 後端回應資料
     */
    function handleBatchResponse(response) {
        try {
            let result;
            if (typeof response.d === 'string') {
                result = JSON.parse(response.d);
            } else {
                result = response.d || response;
            }

            console.log('處理成功回應:', result);

            if (result.Success) {
                // 成功處理
                Swal.fire({
                    title: '已完成',
                    text: result.Message || `成功處理 ${result.SuccessCount || 0} 件計畫`,
                    icon: 'success',
                    confirmButtonText: '確定',
                    confirmButtonColor: '#26A69A'
                }).then(() => {
                    // 重新執行當前頁面的查詢
                    executeCurrentPageSearch();
                });
            } else {
                // 處理失敗
                handleBatchError({
                    message: result.Message || '批次處理失敗',
                    details: result.ErrorMessages || []
                });
            }
        } catch (error) {
            console.error('解析回應資料時發生錯誤:', error);
            handleBatchError({
                message: '回應資料格式錯誤',
                details: [error.message]
            });
        }
    }

    /**
     * 處理批次審核錯誤回應
     * @param {Object} error - 錯誤資訊
     */
    function handleBatchError(error) {
        console.error('批次處理錯誤:', error);

        let errorMessage = '批次處理時發生錯誤';
        let errorDetails = '';

        if (error.message) {
            errorMessage = error.message;
        }

        if (error.details && Array.isArray(error.details) && error.details.length > 0) {
            errorDetails = '\n\n詳細錯誤:\n' + error.details.join('\n');
        } else if (error.responseText) {
            errorDetails = '\n\n系統錯誤: ' + error.statusText;
        }

        Swal.fire({
            title: '操作失敗',
            text: errorMessage + errorDetails,
            icon: 'error',
            confirmButtonText: '確定',
            confirmButtonColor: '#26A69A'
        });
    }

    /**
     * 重新載入當前審查類型的內容
     */
    function reloadCurrentContent() {
        try {
            // 重新載入頁面以觸發後端 LoadReviewContent
            window.location.reload();
        } catch (error) {
            console.error('重新載入內容時發生錯誤:', error);
        }
    }

    /**
     * 執行當前頁面的查詢
     */
    function executeCurrentPageSearch() {
        try {
            console.log(`重新執行 Type${currentType} 的查詢`);
            
            // 根據當前類型觸發對應的搜尋按鈕
            let buttonId;
            switch (currentType) {
                case '1':
                    buttonId = 'MainContent_btnSearch_Type1';
                    break;
                case '2':
                    buttonId = 'MainContent_btnSearch_Type2';
                    break;
                case '3':
                    buttonId = 'MainContent_btnSearch_Type3';
                    break;
                default:
                    console.warn(`不支援的審查類型: ${currentType}`);
                    return;
            }
            
            
            // 使用 jQuery 來觸發點擊
            const $button = $(`#${buttonId}`);
            if ($button.length > 0) {
                $button.trigger('click');
            } else {
                console.log('所有可用的搜尋按鈕:', $('[id*="btnSearch_Type"]').map(function() { return this.id; }).get());
                reloadCurrentContent();
            }
        } catch (error) {
            console.error('執行當前頁面查詢時發生錯誤:', error);
            // 發生錯誤時回到重新載入頁面
            reloadCurrentContent();
        }
    }

    /**
     * 渲染搜尋結果到表格
     * @param {Array} results - 搜尋結果陣列
     * @param {string} type - 審查類型
     */
    function renderSearchResults(results, type) {
        try {
            const tableBodySelector = '#content-type-' + type + ' tbody';
            const $tableBody = $(tableBodySelector);
            
            if (!$tableBody.length) {
                console.error('找不到表格 tbody 元素:', tableBodySelector);
                return;
            }

            // 清空現有內容
            $tableBody.empty();

            if (!results || results.length === 0) {
                // 顯示無資料訊息
                $tableBody.append(`
                    <tr>
                        <td colspan="11" class="text-center">無符合條件的資料</td>
                    </tr>
                `);
                return;
            }

            // 渲染每一行資料
            results.forEach(function(item, index) {
                const row = createTableRow(item, index + 1);
                $tableBody.append(row);
            });

            // 重新初始化 checkbox 功能
            updateCheckboxTargets();

            console.log('已渲染', results.length, '筆搜尋結果');
        } catch (error) {
            console.error('渲染搜尋結果時發生錯誤:', error);
        }
    }

    /**
     * 建立表格行
     * @param {Object} item - 資料項目
     * @param {number} index - 序號
     * @returns {string} HTML 字串
     */
    function createTableRow(item, index) {
        try {
            // 根據當前類型決定使用哪種表格格式
            if (currentType === '2' || currentType === '3') {
                return createType2TableRow(item, index);
            } else {
                return createType1TableRow(item, index);
            }
        } catch (error) {
            console.error('建立表格行時發生錯誤:', error);
            return '<tr><td colspan="11">資料格式錯誤</td></tr>';
        }
    }

    /**
     * 建立 Type1 表格行 (資格審查/內容審查)
     * @param {Object} item - 資料項目
     * @param {number} index - 序號
     * @returns {string} HTML 字串
     */
    function createType1TableRow(item, index) {
        const statusClass = getStatusClass(item.StatusesName);
        const formattedAmount = formatAmount(item.ApplicationAmount);
        const expirationDate = formatDate(item.ExpirationDate);
        const projectCategory = getProjectCategory(item.ProjectID);
        const year = item.Year || '';
        
        return `
            <tr>
                <td>
                    <input class="form-check-input check-teal checkPlan" type="checkbox" value="${item.ProjectID || ''}" />
                </td>
                <td data-th="年度:">${year}</td>
                <td data-th="類別:" style="text-align: center;">${projectCategory}</td>
                <td data-th="計畫編號:" style="text-align: left;" nowrap>${item.ProjectID || ''}</td>
                <td data-th="計畫名稱:" style="text-align: left;">
                    <a href="#" class="link-black" target="_blank">${item.ProjectNameTw || ''}</a>
                </td>
                <td data-th="申請單位:" style="text-align: left;">${item.UserOrg || ''}</td>
                <td data-th="申請經費:">${formattedAmount}</td>
                <td data-th="狀態:" nowrap>
                    <span class="${statusClass}">${item.StatusesName || ''}</span>
                </td>
                <td data-th="補正件期限:" style="text-align: center;">
                    <span class="text-royal-blue">${expirationDate || ''}</span>
                </td>
                <td data-th="承辦人員:">${item.SupervisoryPersonName || ''}</td>
                <td data-th="功能:">
                    <div class="d-flex align-items-center justify-content-end gap-1">
                        ${getActionButtons(item)}
                    </div>
                </td>
            </tr>
        `;
    }

    /**
     * 建立 Type2 表格行 (領域審查/初審)
     * @param {Object} item - 資料項目
     * @param {number} index - 序號
     * @returns {string} HTML 字串
     */
    function createType2TableRow(item, index) {
        const statusClass = getStatusClass(item.StatusesName);
        const formattedAmount = formatAmount(item.ApplicationAmount);
        const projectCategory = getProjectCategory(item.ProjectID);
        const year = item.Year || '';
        const reviewGroup = getReviewGroup(item); // 審查組別
        const reviewProgressDisplay = item.ReviewProgressDisplay || '--';
        const replyProgressDisplay = item.ReplyProgressDisplay || '--';
        
        return `
            <tr>
                <td>
                    <input class="form-check-input check-teal checkPlan" type="checkbox" value="${item.ProjectID || ''}" />
                </td>
                <td data-th="年度:">${year}</td>
                <td data-th="類別:" style="text-align: center;">${projectCategory}</td>
                <td data-th="計畫編號:" style="text-align: left;" nowrap>${item.ProjectID || ''}</td>
                <td data-th="計畫名稱:" style="text-align: left;">
                    <a href="#" class="link-black" target="_blank">${item.ProjectNameTw || ''}</a>
                </td>
                <td data-th="申請單位:" style="text-align: left;">${item.UserOrg || ''}</td>
                <td data-th="審查組別:">${reviewGroup}</td>
                <td data-th="審查進度:" nowrap>
                    <span class="">${reviewProgressDisplay}</span>
                </td>
                <td data-th="回覆狀態:" style="text-align: center;">
                    <span class="">${replyProgressDisplay}</span>
                </td>
                <td data-th="承辦人員:">${item.SupervisoryPersonName || ''}</td>
                <td data-th="功能:">
                    <div class="d-flex align-items-center justify-content-end gap-1">
                        ${getActionButtons(item)}
                    </div>
                </td>
            </tr>
        `;
    }

    /**
     * 取得審查組別
     * @param {Object} item - 資料項目
     * @returns {string} 審查組別
     */
    function getReviewGroup(item) {
        // 目前暫時返回預設值，實際應該從資料庫的 Field_Descname 取得
        // 這個欄位需要在 SQL 查詢中加入 JOIN Sys_ZgsCode 來取得
        return item.ReviewGroup || item.Field_Descname || '--';
    }

    /**
     * 取得狀態對應的 CSS 類別
     * @param {string} status - 狀態
     * @returns {string} CSS 類別
     */
    function getStatusClass(status) {
        if (!status) return '';
        
        switch (status) {
            case '未通過': 
            case '逾期未補': 
                return 'text-danger';
            default: 
                return '';
        }
    }

    /**
     * 格式化金額
     * @param {string|number} amount - 金額
     * @returns {string} 格式化後的金額
     */
    function formatAmount(amount) {
        if (!amount || amount === '0') return '-';
        
        try {
            const num = parseInt(amount);
            return num.toLocaleString('zh-TW');
        } catch (error) {
            return amount.toString();
        }
    }

    /**
     * 格式化日期
     * @param {string} dateStr - 日期字串
     * @returns {string} 格式化後的日期
     */
    function formatDate(dateStr) {
        if (!dateStr) return '';
        
        try {
            const date = new Date(dateStr);
            if (isNaN(date.getTime())) return '';
            
            const year = date.getFullYear();
            const month = String(date.getMonth() + 1).padStart(2, '0');
            const day = String(date.getDate()).padStart(2, '0');
            
            return `${year}/${month}/${day}`;
        } catch (error) {
            return '';
        }
    }

    /**
     * 取得計畫類別
     * @param {string} projectId - 計畫編號
     * @returns {string} 計畫類別
     */
    function getProjectCategory(projectId) {
        if (!projectId) return '未知';
        
        if (projectId.includes('SCI')) return '科專';
        if (projectId.includes('CUL')) return '文化';
        if (projectId.includes('EDC')) return '學校民間';
        if (projectId.includes('CLB')) return '學校社團';
        if (projectId.includes('MUL')) return '多元';
        if (projectId.includes('LIT')) return '素養';
        if (projectId.includes('ACC')) return '無障礙';
        
        return '其他';
    }

    /**
     * 取得操作按鈕
     * @param {Object} item - 資料項目
     * @returns {string} 按鈕 HTML
     */
    function getActionButtons(item) {
        if (!item.StatusesName || !item.ProjectID) return '';
        
        // 只有審查中的案件才顯示審查按鈕
        if (item.StatusesName === '審查中') {
            const reviewUrl = getReviewUrl(item.ProjectID);
            if (reviewUrl) {
                return `<button class="btn btn-sm btn-teal-dark" type="button" onclick="window.location.href='${reviewUrl}'">
                            <i class="fas fa-clipboard-check" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="審查"></i>
                        </button>`;
            }
        }
        
        return '';
    }

    /**
     * 根據 ProjectID 取得對應的審查頁面 URL
     * @param {string} projectId - 計畫編號
     * @returns {string|null} 審查頁面 URL
     */
    function getReviewUrl(projectId) {
        if (!projectId) return null;
        
        // 根據不同補助案類型返回對應的審查頁面
        if (projectId.includes('SCI')) {
            // 科專補助案
            return `SCI/SciApplicationReview.aspx?ProjectID=${projectId}`;
        }
        // 預留其他補助案類型的空間
        else if (projectId.includes('CUL')) {
            // 文化補助案 - 未實作
            // return `OFS/CUL/CulApplicationReview.aspx?ProjectID=${projectId}`;
            return null;
        }
        else if (projectId.includes('EDC')) {
            // 學校民間補助案 - 未實作
            // return `OFS/EDC/EdcApplicationReview.aspx?ProjectID=${projectId}`;
            return null;
        }
        else if (projectId.includes('CLB')) {
            // 學校社團補助案 - 未實作
            // return `OFS/CLB/ClbApplicationReview.aspx?ProjectID=${projectId}`;
            return null;
        }
        else if (projectId.includes('MUL')) {
            // 多元補助案 - 未實作
            // return `OFS/MUL/MulApplicationReview.aspx?ProjectID=${projectId}`;
            return null;
        }
        else if (projectId.includes('LIT')) {
            // 素養補助案 - 未實作
            // return `OFS/LIT/LitApplicationReview.aspx?ProjectID=${projectId}`;
            return null;
        }
        else if (projectId.includes('ACC')) {
            // 無障礙補助案 - 未實作
            // return `OFS/ACC/AccApplicationReview.aspx?ProjectID=${projectId}`;
            return null;
        }
        
        // 未知的補助案類型
        return null;
    }

    // 公開API
    return {
        init: init,
        switchReviewType: switchReviewType,
        getCurrentType: getCurrentType,
        reloadCurrentContent: reloadCurrentContent,
        renderSearchResults: renderSearchResults,
        getSelectedProjectIds: getSelectedProjectIds,
        confirmBatchApproval: confirmBatchApproval,
        callBatchApprovalAPI: callBatchApprovalAPI,
        handleBatchResponse: handleBatchResponse,
        handleBatchError: handleBatchError
    };
})();

// 將模組暴露為全域變數，供 C# 後端調用
window.ReviewChecklistManager = window.ReviewChecklist;

/**
 * 簡化版批次審核處理函數 - 全域函數供按鈕直接調用
 * @param {string} actionText - 動作文字 (如: "轉入下一階段", "進入決審")
 */
function handleBatchApproval(actionText) {
    try {
        const currentType = window.ReviewChecklist.getCurrentType();
        
        // 1. 收集選中的專案
        const currentContent = $('#content-type-' + currentType);
        const selectedCheckboxes = currentContent.find('.checkPlan:checked');
        
        if (selectedCheckboxes.length === 0) {
            Swal.fire({
                title: '提醒',
                text: '請先選擇要處理的計畫項目',
                icon: 'warning',
                confirmButtonText: '確定'
            });
            return;
        }

        const selectedIds = [];
        const invalidProjects = [];
        
        selectedCheckboxes.each(function() {
            const projectId = $(this).val();
            const $row = $(this).closest('tr');
            const statusText = $row.find('td[data-th="狀態:"] span').text().trim();
            
            // 只有資格審查（Type1）才需要檢查狀態是否為「通過」
            if (currentType === '1') {
                if (statusText === '通過') {
                    selectedIds.push(projectId);
                } else {
                    invalidProjects.push({ id: projectId, status: statusText });
                }
            } else {
                // Type2, Type3 不需要檢查狀態，直接加入
                selectedIds.push(projectId);
            }
        });

        if (invalidProjects.length > 0) {
            Swal.fire({
                title: '提醒',
                text: `資格審查階段只有狀態為「通過」的計畫可以${actionText}`,
                icon: 'warning',
                confirmButtonText: '確定'
            });
            return;
        }

        // 2. 顯示確認對話框
        Swal.fire({
            title: '確認批次操作',
            text: `共 ${selectedIds.length} 件計畫\n\n確定批次通過，${actionText}？`,
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            confirmButtonColor: '#26A69A',
            cancelButtonColor: '#d33'
        }).then((result) => {
            if (result.isConfirmed) {
                // 3. 執行批次處理
                batchProcess(selectedIds, actionText, currentType);
            }
        });

    } catch (error) {
        console.error('批次處理時發生錯誤:', error);
        Swal.fire({
            title: '系統錯誤',
            text: '處理批次操作時發生錯誤',
            icon: 'error',
            confirmButtonText: '確定'
        });
    }
}

/**
 * 執行批次處理 - 簡化版本
 */
function batchProcess(selectedIds, actionText, currentType) {
    // 顯示處理中
    Swal.fire({
        title: '處理中...',
        text: '正在執行批次操作，請稍候...',
        allowOutsideClick: false,
        showConfirmButton: false,
        willOpen: () => {
            Swal.showLoading();
        }
    });

    $.ajax({
        type: "POST",
        url: "ReviewChecklist.aspx/BatchApproveType1",
        data: JSON.stringify({
            projectIds: selectedIds,
            actionType: actionText,
            reviewType: currentType
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        timeout: 30000
    }).done(function(response) {
        const result = response.d || response;
        
        if (result.Success) {
            // 成功
            Swal.fire({
                title: '已完成',
                text: result.Message || `成功處理 ${result.SuccessCount || 0} 件計畫`,
                icon: 'success',
                confirmButtonText: '確定'
            }).then(() => {
                // 重新執行查詢
                const buttonId = `MainContent_btnSearch_Type${currentType}`;
                const $button = $(`#${buttonId}`);
                if ($button.length > 0) {
                    $button.trigger('click');
                } else {
                    window.location.reload();
                }
            });
        } else {
            // 失敗
            Swal.fire({
                title: '操作失敗',
                text: result.Message || '批次處理失敗',
                icon: 'error',
                confirmButtonText: '確定'
            });
        }
    }).fail(function(jqXHR, textStatus, errorThrown) {
        console.error('AJAX 請求失敗:', textStatus, errorThrown);
        Swal.fire({
            title: '系統錯誤',
            text: '批次處理請求失敗，請稍後再試',
            icon: 'error',
            confirmButtonText: '確定'
        });
    });
}