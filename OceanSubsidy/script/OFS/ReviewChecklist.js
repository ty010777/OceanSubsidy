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
        renderSearchResults: renderSearchResults
    };
})();

// 將模組暴露為全域變數，供 C# 後端調用
window.ReviewChecklistManager = window.ReviewChecklist;