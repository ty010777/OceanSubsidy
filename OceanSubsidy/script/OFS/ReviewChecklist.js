/**
 * ReviewChecklist.js
 * 審查清單頁面的JavaScript功能
 * 處理頁面切換、URL參數管理等功能
 */

/**
 * 將英文類別代碼轉換為中文類別名稱
 * @param {string} categoryCode - 英文類別代碼
 * @returns {string} 中文類別名稱
 */
function convertCategoryCodeToName(categoryCode) {
    if (!categoryCode) return '';

    switch (categoryCode.toUpperCase()) {
        case 'SCI':
            return '科專';
        case 'CUL':
            return '文化';
        case 'EDC':
            return '學校民間';
        case 'CLB':
            return '學校社團';
        case 'MUL':
            return '多元';
        case 'LIT':
            return '素養';
        case 'ACC':
            return '無障礙';
        default:
            return categoryCode;
    }
}

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

            // Type4 類別變更事件監聽
            $('#ddlCategory_Type4').on('change', function() {
                var selectedCategory = $(this).val();
                updateReviewGroupType4(selectedCategory);
            });

            // 排序模式中的類別變更事件監聽
            $('#sortingCategory').on('change', function() {
                var selectedCategory = $(this).val();
                updateSortingReviewGroup(selectedCategory);
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

            // 如果找不到全選checkbox，直接返回
            if (!checkAll) {
                return;
            }

            // 檢查是否有任何項目被勾選，並控制批次按鈕面板的顯示
            function toggleBatchButtons() {
                if (!checkPlanBtnPanel) return;

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
                let hasAny = false;
                checkPlan.each(function() {
                    hasAny = true;
                    if (!this.checked) {
                        allChecked = false;
                    }
                });

                // 只有當有個別checkbox時才更新全選狀態
                if (hasAny) {
                    checkAll.checked = allChecked;
                }
                toggleBatchButtons();
            });

            // 初始化狀態
            toggleBatchButtons();

            console.log('checkbox功能已初始化:', {
                checkAll: !!checkAll,
                checkPlan: checkPlan.length,
                checkPlanBtnPanel: !!checkPlanBtnPanel,
                currentType: currentType
            });

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


            $.ajax({
                type: "POST",
                url: "ReviewChecklist.aspx/BatchApproveType",
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
            if (type === '4') {
                // Type-4 需要渲染到兩個表格中
                renderType4SearchResults(results);
            } else {
                // 其他類型的正常渲染
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

                // Type6 特殊處理：控制審查委員進度欄位的顯示
                if (type === '6') {
                    handleType6ReviewProgressDisplay(results);
                }

                // 重新初始化 checkbox 功能
                updateCheckboxTargets();

            }
        } catch (error) {
            console.error('渲染搜尋結果時發生錯誤:', error);
        }
    }

    /**
     * 渲染 Type-4 搜尋結果到表格中
     * @param {Array} results - 搜尋結果
     */
    function renderType4SearchResults(results) {
        try {
            // 核定模式表格
            const $approvalTableBody = $('#content-type-4 .approval-mode-table tbody');

            if (!$approvalTableBody.length) {
                console.error('找不到 Type-4 表格 tbody 元素');
                return;
            }

            // 清空現有內容
            $approvalTableBody.empty();

            if (!results || results.length === 0) {
                // 顯示無資料訊息
                const noDataRow = `<tr><td colspan="10" class="text-center">無符合條件的資料</td></tr>`;
                $approvalTableBody.append(noDataRow);
                return;
            }

            // 渲染每一行資料
            results.forEach(function(item, index) {
                const approvalRow = createType4TableRow(item, index + 1);
                $approvalTableBody.append(approvalRow);
            });

            // 重新初始化 checkbox 功能
            updateCheckboxTargets();
        } catch (error) {
            console.error('渲染 Type-4 搜尋結果時發生錯誤:', error);
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
            if (currentType === '4') {
                return createType4TableRow(item, index);
            } else if (currentType === '2' || currentType === '3') {
                return createType2TableRow(item, index);
            } else if (currentType === '5') {
                return createType5TableRow(item, index);
            } else if (currentType === '6') {
                return createType6TableRow(item, index);
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
        const formattedAmount = formatAmount(item.Req_SubsidyAmount);
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
                    <a href="${getReviewUrl(item.ProjectID)}" class="link-black" target="_blank">${item.ProjectNameTw || ''}</a>
                </td>
                <td data-th="申請單位:" style="text-align: left;">${item.OrgName || ''}</td>
                <td data-th="申請經費:">${item.Req_SubsidyAmount}</td>
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
     * 建立 Type4 表格行 (決審核定)
     * @param {Object} item - 資料項目
     * @param {number} index - 序號
     * @returns {string} HTML 字串
     */
    function createType4TableRow(item, index) {
        const formattedReq_SubsidyAmount = formatAmount(item.Req_SubsidyAmount);
        const year = item.Year || '';
        const totalScore = item.TotalScore || '';
        const approvedSubsidy = item.ApprovedSubsidy || '';
        const finalReviewNotes = item.FinalReviewNotes || '';
        const statusesName = item.StatusesName || '';

        // 核定模式：有勾選框、有核定經費輸入框、有修正計畫書欄位
        let planRevisionContent = '';
        if (statusesName === '計畫書審核中') {
            planRevisionContent = `
                <div class="d-flex align-items-center justify-content-center gap-1">
                    <button class="btn btn-sm btn-teal-dark" type="button" onclick="window.location.href='SCI/SciFinalReview.aspx?ProjectID=${item.ProjectID || ''}'">
                        <i class="fas fa-clipboard-check" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="審查"></i>
                    </button>
                </div>
            `;
        } else {
            planRevisionContent = statusesName;
        }

        return `
            <tr>
                <td>
                    <input class="form-check-input check-teal checkPlan" type="checkbox" value="${item.ProjectID || ''}" />
                </td>
                <td data-th="排序:">${index}</td>
                <td data-th="年度:">${year}</td>
                <td data-th="計畫名稱:" style="text-align: left;">
                    <a href="#" class="link-black" target="_blank">${item.ProjectNameTw || ''}</a>
                </td>
                <td data-th="申請單位:" width="180" style="text-align: left;">${item.OrgName || ''}</td>
                <td data-th="總分:" nowrap>${totalScore}</td>
                <td data-th="申請經費:" width="120" style="text-align: center; text-wrap: nowrap;">${formattedReq_SubsidyAmount}</td>
                <td data-th="核定經費:">
                    <input type="text" class="form-control" value="${approvedSubsidy}" style="width: 160px;" data-project-id="${item.ProjectID || ''}" data-field="ApprovedSubsidy">
                </td>
                <td data-th="備註:">
                    <input type="text" class="form-control" value="${finalReviewNotes}" data-project-id="${item.ProjectID || ''}" data-field="FinalReviewNotes">
                </td>
                <td data-th="修正計畫書:" class="text-center">
                    ${planRevisionContent}
                </td>
            </tr>
        `;
    }

    /**
     * 建立 Type5 表格行 (計畫變更審核)
     * @param {Object} item - 資料項目
     * @param {number} index - 序號
     * @returns {string} HTML 字串
     */
    function createType5TableRow(item, index) {
        const year = item.Year || '';
        const projectId = item.ProjectID || '';
        const category = item.Category || '';
        const categoryDisplay = convertCategoryCodeToName(category); // 轉換為中文顯示
        const projectName = item.ProjectNameTw || '';
        const orgName = item.OrgName || '';

        return `
            <tr>
                <td data-th="年度:">${year}</td>
                <td data-th="計畫編號:" style="text-align: left;" nowrap>${projectId}</td>
                <td data-th="類別:" style="text-align: center;">${categoryDisplay}</td>
                <td data-th="計畫名稱:" style="text-align: left;">
                    <a href="#" class="link-black" target="_blank">${projectName}</a>
                </td>
                <td data-th="申請單位:" style="text-align: left;">${orgName}</td>
                <td data-th="操作:" style="text-align: center;">
                    <div class="d-flex align-items-center justify-content-center gap-1">
                        <button class="btn btn-sm btn-teal-dark" type="button" onclick="handlePlanChangeReview('${projectId}')">
                            <i class="fas fa-clipboard-check" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="審核"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
    }

    /**
     * 建立 Type6 表格行 (執行計畫審核)
     * @param {Object} item - 資料項目
     * @param {number} index - 序號
     * @returns {string} HTML 字串
     */
    function createType6TableRow(item, index) {
        const year = item.Year || '';
        const projectId = item.ProjectID || '';
        const category = item.Category || '';
        const categoryDisplay = convertCategoryCodeToName(category); // 轉換為中文顯示
        const projectName = item.ProjectNameTw || '';
        const orgName = item.OrgName || '';
        const reviewItem = item.ReviewTodo || '';  // 待審項目
        const reviewProgress = item.ReviewProgress || '';  // 審查委員進度

        // 決定是否顯示審查委員進度欄位 (僅科專專案)
        const showReviewProgress = category === 'SCI';
        const reviewProgressCell = showReviewProgress ?
            `<td data-th="審查委員進度:" class="review-progress-cell text-center">${reviewProgress}</td>` :
            `<td class="review-progress-cell" style="display: none;"></td>`;

        return `
            <tr>
                <td data-th="年度:">${year}</td>
                <td data-th="類別:" style="text-align: center;">${categoryDisplay}</td>
                <td data-th="計畫編號:" style="text-align: left;" nowrap>${projectId}</td>
                <td data-th="計畫名稱:" style="text-align: left;">
                    <a href="#" class="link-black" target="_blank">${projectName}</a>
                </td>
                <td data-th="申請單位:" style="text-align: left;">${orgName}</td>
                <td data-th="待審項目:" style="text-align: left;">${reviewItem}</td>
                ${reviewProgressCell}
                <td data-th="功能:" class="text-center">
                    <div class="d-flex align-items-center justify-content-center gap-1">
                        <button class="btn btn-sm btn-teal-dark" type="button" onclick="handleExecutionPlanReview('${projectId}','${reviewItem}')">
                            <i class="fas fa-clipboard-check" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="審查"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
    }

    /**
     * 處理 Type6 審查委員進度欄位的顯示/隱藏
     * @param {Array} results - 搜尋結果數組
     */
    function handleType6ReviewProgressDisplay(results) {
        // 檢查是否有科專專案
        const hasSciProjects = results.some(item => item.Category === 'SCI');

        // 控制表格標題的顯示/隱藏
        const $headerCell = $('#content-type-6 .review-progress-header');
        const $dataCells = $('#content-type-6 .review-progress-cell');

        if (hasSciProjects) {
            $headerCell.show();
            $dataCells.each(function() {
                const $cell = $(this);
                const $row = $cell.closest('tr');
                const categoryCell = $row.find('td:nth-child(2)');

                // 只顯示科專專案的進度資料
                if (categoryCell.text().trim() === '科專') {
                    $cell.show();
                } else {
                    $cell.hide();
                }
            });
        } else {
            $headerCell.hide();
            $dataCells.hide();
        }
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

        let buttons = '';

        // Type2 領域審查 和 Type3 技術審查 顯示詳情按鈕
        if (currentType === '2' || currentType === '3') {
            buttons += `<button class="btn btn-sm btn-teal-dark" type="button" data-bs-toggle="modal" data-bs-target="#planDetailModal" onclick="openPlanDetail('${item.ProjectID}')">
                            <i class="fas fa-file-alt" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="詳情"></i>
                        </button>`;
        }

        // 只有審核中的案件才顯示審查按鈕
        if (currentType === '1' && item.StatusesName === '審核中') {
            const reviewUrl = getReviewUrl(item.ProjectID);
            if (reviewUrl) {
                buttons += `<button class="btn btn-sm btn-teal-dark" type="button" onclick="window.location.href='${reviewUrl}'">
                            <i class="fas fa-clipboard-check" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="審查"></i>
                        </button>`;
            }
        }

        return buttons;
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
            // 文化補助案
            return `CUL/Review.aspx?ID=${projectId}`;
        }
        else if (projectId.includes('EDC')) {
            // 學校民間補助案
            return `EDC/Review.aspx?ID=${projectId}`;
        }
        else if (projectId.includes('CLB')) {
            // 學校社團補助案 - 未實作
            // return `OFS/CLB/ClbApplicationReview.aspx?ProjectID=${projectId}`;
            return null;
        }
        else if (projectId.includes('MUL')) {
            // 多元補助案
            return `MUL/Review.aspx?ID=${projectId}`;
        }
        else if (projectId.includes('LIT')) {
            // 素養補助案
            return `LIT/Review.aspx?ID=${projectId}`;
        }
        else if (projectId.includes('ACC')) {
            // 無障礙補助案
            return `ACC/Review.aspx?ID=${projectId}`;
        }

        // 未知的補助案類型
        return null;
    }

    // 公開API
    /**
     * Type4 核定模式儲存功能
     */
    function saveApprovalMode_Type4() {
        try {
            // 收集表格中的資料
            const approvalItems = [];
            const $table = $('#content-type-4 .approval-mode-table table tbody');

            $table.find('tr').each(function() {
                const $row = $(this);
                const projectId = $row.find('.checkPlan').val();
                const approvedSubsidy = $row.find('input[data-field="ApprovedSubsidy"]').val() || '0';
                const finalReviewNotes = $row.find('input[data-field="FinalReviewNotes"]').val() || '';

                // 從專案ID判斷類別
                let category = 'SCI'; // 預設科專
                if (projectId) {
                    if (projectId.includes('CUL')) category = 'CUL';
                    else if (projectId.includes('EDC')) category = 'EDC';
                    else if (projectId.includes('CLB')) category = 'CLB';
                }

                if (projectId) {
                    approvalItems.push({
                        ProjectID: projectId,
                        ApprovedSubsidy: approvedSubsidy,
                        FinalReviewNotes: finalReviewNotes,
                        Category: category
                    });
                }
            });

            if (approvalItems.length === 0) {
                Swal.fire({
                    title: '沒有資料',
                    text: '找不到可儲存的資料',
                    icon: 'warning',
                    confirmButtonText: '確定'
                });
                return;
            }

            // 顯示確認對話框
            Swal.fire({
                title: '確認儲存',
                text: `即將儲存 ${approvalItems.length} 筆核定資料，是否繼續？`,
                icon: 'question',
                showCancelButton: true,
                confirmButtonText: '儲存',
                cancelButtonText: '取消'
            }).then((result) => {
                if (result.isConfirmed) {
                    callSaveApprovalAPI(approvalItems);
                }
            });

        } catch (error) {
            console.error('收集儲存資料時發生錯誤:', error);
            Swal.fire({
                title: '錯誤',
                text: '準備儲存資料時發生錯誤',
                icon: 'error',
                confirmButtonText: '確定'
            });
        }
    }

    /**
     * 調用儲存 API
     */
    function callSaveApprovalAPI(approvalItems) {
            $.ajax({
            type: 'POST',
            url: 'ReviewChecklist.aspx/SaveApprovalMode_Type4',
            data: JSON.stringify({ approvalItems: approvalItems }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            beforeSend: function() {
                // 顯示載入中
                Swal.fire({
                    title: '儲存中...',
                    text: '正在處理您的要求',
                    allowOutsideClick: false,
                    didOpen: () => {
                        Swal.showLoading();
                    }
                });
            }
        }).done(function(response) {
            try {
                const result = typeof response.d === 'string' ? JSON.parse(response.d) : response.d;

                if (result.success) {
                    Swal.fire({
                        title: '儲存成功',
                        text: result.message || `成功儲存 ${result.count || 0} 筆資料`,
                        icon: 'success',
                        confirmButtonText: '確定'
                    }).then(() => {
                        // 重新執行查詢
                        const $button = $('#MainContent_btnSearch_Type4');
                        if ($button.length > 0) {
                            $button.trigger('click');
                        }
                    });
                } else {
                    Swal.fire({
                        title: '儲存失敗',
                        text: result.message || '儲存時發生未知錯誤',
                        icon: 'error',
                        confirmButtonText: '確定'
                    });
                }
            } catch (parseError) {
                console.error('解析回應時發生錯誤:', parseError);
                Swal.fire({
                    title: '系統錯誤',
                    text: '處理伺服器回應時發生錯誤',
                    icon: 'error',
                    confirmButtonText: '確定'
                });
            }
        }).fail(function(jqXHR, textStatus, errorThrown) {
            console.error('儲存 API 請求失敗:', textStatus, errorThrown);
            Swal.fire({
                title: '網路錯誤',
                text: '儲存請求失敗，請檢查網路連線並重試',
                icon: 'error',
                confirmButtonText: '確定'
            });
        });
    }


    /**
     * 排序模式查詢功能
     */
    function searchSortingMode() {
        const year = $('#sortingYear').val();
        const category = $('#sortingCategory').val();
        const reviewGroup = $('#sortingReviewGroup').val();

        // 顯示載入狀態
        showSortingLoading(true);

        $.ajax({
            type: "POST",
            url: "ReviewChecklist.aspx/SearchSortingMode",
            data: JSON.stringify({
                year: year,
                category: category,
                reviewGroupCode: reviewGroup
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        }).done(function(response) {
            try {
                const result = JSON.parse(response.d);
                if (result.success) {
                    renderSortingTable(result.data);
                    $('#sortingResultCount').text(`共 ${result.count} 筆資料`);
                } else {
                    console.error('查詢失敗:', result.message);
                    clearSortingTable();
                    $('#sortingResultCount').text('查詢失敗');
                }
            } catch (e) {
                console.error('解析回應時發生錯誤:', e);
                clearSortingTable();
                $('#sortingResultCount').text('查詢發生錯誤');
            }
        }).fail(function(jqXHR, textStatus, errorThrown) {
            console.error('AJAX 請求失敗:', textStatus, errorThrown);
            clearSortingTable();
            $('#sortingResultCount').text('請求失敗');
        }).always(function() {
            showSortingLoading(false);
        });
    }

    /**
     * 渲染排序表格
     */
    function renderSortingTable(data) {
        const tableBody = $('#sortingTableBody');
        tableBody.empty();

        if (!data || data.length === 0) {
            tableBody.append('<tr><td colspan="7" class="text-center">無資料</td></tr>');
            return;
        }

        data.forEach(function(item, index) {
            const row = `
                <tr data-project-id="${item.ProjectID}" data-category="${item.Category}" data-plan-name="${item.ProjectNameTw || ''}">
                    <td data-th="排序:">${index + 1}</td>
                    <td data-th="計畫編號:">${item.ProjectID}</td>
                    <td data-th="計畫名稱:" class="text-start">
                        <a href="#" class="link-black" target="_blank">${item.ProjectNameTw || ''}</a>
                    </td>
                    <td data-th="申請單位:" class="text-start">${item.OrgName || ''}</td>
                    <td data-th="總分:" nowrap>${item.TotalScore || '0'}</td>
                    <td data-th="備註:">
                        <input type="text" class="form-control sorting-notes"
                               value="${item.FinalReviewNotes || ''}" placeholder="備註">
                    </td>
                    <td data-th="功能:">
                        <div class="d-flex align-items-center justify-content-end gap-1">
                            <!-- 拖曳排序把手 -->
                            <button class="btn btn-sm btn-teal-dark btnDrag" type="button">
                                <i class="fas fa-arrows-alt" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="拖曳"></i>
                            </button>
                            <!-- 置頂按鈕 -->
                            <button class="btn btn-sm btn-outline-teal btnTop" type="button">
                                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 12 18" fill="none" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="置頂">
                                    <g clip-path="url(#clip0_893_5801)">
                                    <path d="M10.8519 10.038C10.6229 10.037 10.4038 9.94042 10.2435 9.76976L6.00054 5.3408L1.75763 9.76943C1.36703 10.0487 0.833601 9.94474 0.566094 9.53701C0.372381 9.24183 0.365701 8.85468 0.548917 8.55253L5.39205 3.48839C5.7254 3.13744 6.26805 3.13511 6.60426 3.48308C6.60585 3.48474 6.60744 3.4864 6.60935 3.48839L11.4522 8.55253C11.7846 8.9015 11.7846 9.46529 11.4522 9.81425C11.2871 9.96666 11.0724 10.0467 10.8523 10.038H10.8519Z" fill="#26A69A"/>
                                    <path d="M5.99981 17.4999C5.5265 17.4999 5.14258 17.0992 5.14258 16.6051V4.07891C5.14258 3.58484 5.5265 3.18408 5.99981 3.18408C6.47312 3.18408 6.85705 3.58484 6.85705 4.07891V16.6051C6.85705 17.0992 6.47312 17.4999 5.99981 17.4999Z" fill="#26A69A"/>
                                    <path d="M11.1431 2.28932H0.857234C0.383926 2.28932 0 1.88889 0 1.39482C0 0.900762 0.383926 0.5 0.857234 0.5H11.1431C11.6164 0.5 12.0003 0.900762 12.0003 1.39482C12.0003 1.88889 11.6164 2.28965 11.1431 2.28965V2.28932Z" fill="#26A69A"/>
                                    </g>
                                    <defs>
                                    <clipPath id="clip0_893_5801">
                                    <rect width="12" height="17" fill="white" transform="translate(0 0.5)"/>
                                    </clipPath>
                                    </defs>
                                </svg>
                            </button>
                        </div>
                    </td>
                </tr>
            `;
            tableBody.append(row);
        });

        // 使用 planAdmJS.js 中的拖曳功能
        initSortingTableSorter();
    }

    /**
     * 初始化排序表格的拖曳功能 (使用 planAdmJS.js 的 TableSorter)
     */
    function initSortingTableSorter() {
        // 檢查是否有 TableSorter 類別可用
        if (typeof TableSorter !== 'undefined') {
            const tableSorter = new TableSorter('sortingTable');
            // 覆寫保存排序的方法，讓它調用我們的儲存功能
            tableSorter.saveNewOrder = function() {
                // 當拖曳或置頂後，不需要立即儲存，讓使用者手動按儲存按鈕
            };
        } else {
            console.warn('TableSorter 類別不可用，請確認 planAdmJS.js 已載入');
        }
    }

    /**
     * 儲存排序結果
     */
    function saveSortingMode() {
        const sortingItems = [];

        $('#sortingTableBody tr').each(function(index) {
            const $row = $(this);
            const projectId = $row.data('project-id');
            const category = $row.data('category');
            const notes = $row.find('.sorting-notes').val();

            if (projectId) {
                sortingItems.push({
                    ProjectID: projectId,
                    FinalReviewOrder: index + 1,
                    FinalReviewNotes: notes || '',
                    Category: category
                });
            }
        });

        if (sortingItems.length === 0) {
            Swal.fire({
                title: '提醒',
                text: '沒有資料可以儲存',
                icon: 'warning',
                confirmButtonText: '確定'
            });
            return;
        }

        // 確認儲存
        Swal.fire({
            title: '確認儲存',
            text: `即將儲存 ${sortingItems.length} 筆排序資料，確定要繼續嗎？`,
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: '確定儲存',
            cancelButtonText: '取消'
        }).then((result) => {
            if (result.isConfirmed) {
                performSortingSave(sortingItems);
            }
        });
    }

    /**
     * 執行排序儲存
     */
    function performSortingSave(sortingItems) {
        $.ajax({
            type: "POST",
            url: "ReviewChecklist.aspx/SaveSortingMode",
            data: JSON.stringify({
                sortingItems: sortingItems
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        }).done(function(response) {
            try {
                const result = JSON.parse(response.d);
                if (result.success) {
                    Swal.fire({
                        title: '儲存成功',
                        text: result.message,
                        icon: 'success',
                        confirmButtonText: '確定'
                    });
                } else {
                    Swal.fire({
                        title: '儲存失敗',
                        text: result.message,
                        icon: 'error',
                        confirmButtonText: '確定'
                    });
                }
            } catch (e) {
                console.error('解析回應時發生錯誤:', e);
                Swal.fire({
                    title: '系統錯誤',
                    text: '儲存回應解析失敗',
                    icon: 'error',
                    confirmButtonText: '確定'
                });
            }
        }).fail(function(jqXHR, textStatus, errorThrown) {
            console.error('AJAX 請求失敗:', textStatus, errorThrown);
            Swal.fire({
                title: '系統錯誤',
                text: '儲存請求失敗，請檢查網路連線並重試',
                icon: 'error',
                confirmButtonText: '確定'
            });
        });
    }

    /**
     * 清空排序表格
     */
    function clearSortingTable() {
        $('#sortingTableBody').empty();
    }

    /**
     * 顯示/隱藏排序載入狀態
     */
    function showSortingLoading(show) {
        const $button = $('#btnSearchSorting');
        if (show) {
            $button.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> 查詢中...');
        } else {
            $button.prop('disabled', false).html('<i class="fas fa-search"></i> 查詢');
        }
    }


    // 手動重新初始化checkbox功能的公開方法
    function forceUpdateCheckboxTargets() {
        console.log('手動重新初始化checkbox功能');

    }

    /**
     * Type4 類別變更時更新審查組別選項
     * @param {string} category - 選中的類別代碼
     */
    function updateReviewGroupType4(category) {
        try {
            // 顯示載入狀態
            const $reviewGroupSelect = $('#ddlReviewGroup_Type4');
            $reviewGroupSelect.prop('disabled', true).html('<option>載入中...</option>');

            // 調用後端 WebMethod 取得審查組別選項
            $.ajax({
                type: 'POST',
                url: 'ReviewChecklist.aspx/GetReviewGroupOptionsByCategory',
                data: JSON.stringify({ category: category }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function(response) {
                    try {
                        const result = JSON.parse(response.d);

                        if (result.Success) {
                            // 清空並重新填充選項
                            $reviewGroupSelect.empty();

                            result.Options.forEach(function(option) {
                                $reviewGroupSelect.append($('<option>', {
                                    value: option.Value,
                                    text: option.Text
                                }));
                            });

                            $reviewGroupSelect.prop('disabled', false);
                        } else {
                            console.error('取得審查組別選項失敗:', result.Message);
                            $reviewGroupSelect.html('<option value="">取得選項失敗</option>').prop('disabled', false);
                        }
                    } catch (parseError) {
                        console.error('解析回應資料時發生錯誤:', parseError);
                        $reviewGroupSelect.html('<option value="">解析錯誤</option>').prop('disabled', false);
                    }
                },
                error: function(xhr, status, error) {
                    console.error('AJAX 請求失敗:', error);
                    $reviewGroupSelect.html('<option value="">載入失敗</option>').prop('disabled', false);
                }
            });
        } catch (error) {
            console.error('更新審查組別時發生錯誤:', error);
        }
    }

    /**
     * 排序模式中類別變更時更新審查組別選項
     * @param {string} category - 選中的類別代碼
     */
    function updateSortingReviewGroup(category) {
        try {
            // 顯示載入狀態
            const $reviewGroupSelect = $('#sortingReviewGroup');
            $reviewGroupSelect.prop('disabled', true).html('<option>載入中...</option>');

            // 調用後端 WebMethod 取得審查組別選項
            $.ajax({
                type: 'POST',
                url: 'ReviewChecklist.aspx/GetReviewGroupOptionsByCategory',
                data: JSON.stringify({ category: category }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function(response) {
                    try {
                        const result = JSON.parse(response.d);

                        if (result.Success) {
                            // 清空並重新填充選項
                            $reviewGroupSelect.empty();

                            result.Options.forEach(function(option) {
                                $reviewGroupSelect.append($('<option>', {
                                    value: option.Value,
                                    text: option.Text
                                }));
                            });

                            $reviewGroupSelect.prop('disabled', false);
                        } else {
                            console.error('取得排序審查組別選項失敗:', result.Message);
                            $reviewGroupSelect.html('<option value="">取得選項失敗</option>').prop('disabled', false);
                        }
                    } catch (parseError) {
                        console.error('解析排序回應資料時發生錯誤:', parseError);
                        $reviewGroupSelect.html('<option value="">解析錯誤</option>').prop('disabled', false);
                    }
                },
                error: function(xhr, status, error) {
                    console.error('排序 AJAX 請求失敗:', error);
                    $reviewGroupSelect.html('<option value="">載入失敗</option>').prop('disabled', false);
                }
            });
        } catch (error) {
            console.error('更新排序審查組別時發生錯誤:', error);
        }
    }

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
        handleBatchError: handleBatchError,
        saveApprovalMode_Type4: saveApprovalMode_Type4,
        searchSortingMode: searchSortingMode,
        saveSortingMode: saveSortingMode,

        updateCheckboxTargets: updateCheckboxTargets,
        updateReviewGroupType4: updateReviewGroupType4,
        updateSortingReviewGroup: updateSortingReviewGroup,
    };

    /**
     * 當審查組別資料準備好時的回調函數
     */

})();

// 將模組暴露為全域變數，供 C# 後端調用
window.ReviewChecklistManager = window.ReviewChecklist;

/**
 * Type4 核定模式儲存功能 - 全域函數供按鈕直接調用
 */
function handleType4ApprovalSave() {
    if (window.ReviewChecklist && typeof window.ReviewChecklist.saveApprovalMode_Type4 === 'function') {
        window.ReviewChecklist.saveApprovalMode_Type4();
    } else {
        console.error('ReviewChecklist 模組或 saveApprovalMode_Type4 方法未找到');
        Swal.fire({
            title: '系統錯誤',
            text: '儲存功能初始化失敗',
            icon: 'error',
            confirmButtonText: '確定'
        });
    }
}

/**
 * Type5 計畫變更審核處理函數 - 全域函數供按鈕直接調用
 * @param {string} projectId - 計畫編號
 */
function handlePlanChangeReview(projectId) {
    try {
        if (!projectId) {
            Swal.fire({
                title: '參數錯誤',
                text: '計畫編號不能為空',
                icon: 'error',
                confirmButtonText: '確定'
            });
            return;
        }

        // TODO: 導向到計畫變更審核頁面
        // 根據計畫編號的類型決定跳轉到對應的審核頁面
        // TODO 正文 請提供 變更計畫時 所需跳轉的各類型計畫變更審核頁面 URL
        let reviewUrl = '';

        if (projectId.includes('SCI')) {
            // 科專計畫變更審核頁面
            reviewUrl = `SCI/SciInprogress_Approved.aspx?ProjectID=${projectId}`;
        } else if (projectId.includes('CUL')) {
            // 文化計畫變更審核頁面

        } else if (projectId.includes('EDC')) {
            // 學校民間計畫變更審核頁面

        } else if (projectId.includes('CLB')) {
            // 學校社團計畫變更審核頁面

        } else if (projectId.includes('MUL')) {
            // 多元計畫變更審核頁面

        } else if (projectId.includes('LIT')) {
            // 素養計畫變更審核頁面

        } else if (projectId.includes('ACC')) {
            // 無障礙計畫變更審核頁面

        }

        if (reviewUrl) {
            // 跳轉到對應的審核頁面
            window.location.href = reviewUrl;
        } else {
            Swal.fire({
                title: '功能開發中',
                text: `該類型計畫 (${projectId}) 的變更審核功能尚未開放`,
                icon: 'info',
                confirmButtonText: '確定'
            });
        }

    } catch (error) {
        console.error('處理計畫變更審核時發生錯誤:', error);
        Swal.fire({
            title: '系統錯誤',
            text: '處理計畫變更審核時發生錯誤',
            icon: 'error',
            confirmButtonText: '確定'
        });
    }
}

/**
 * Type6 執行計畫審核處理函數 - 全域函數供按鈕直接調用
 * @param {string} projectId - 計畫編號
 */
function handleExecutionPlanReview(projectId,reviewItem) {
    try {
        if (!projectId) {
            Swal.fire({
                title: '參數錯誤',
                text: '計畫編號不能為空',
                icon: 'error',
                confirmButtonText: '確定'
            });
            return;
        }

        // TODO: 正文 導向到執行計畫審核頁面 的網址
        // 根據計畫編號的類型決定跳轉到對應的審核頁面
        let reviewUrl = '';

        if (projectId.includes('SCI')) {
            // 科專執行計畫審核頁面
            if(reviewItem.includes('檢核')) {
                reviewUrl = `SCI/SciInterimReport.aspx?ProjectID=${projectId}`;
            }
            else if (reviewItem.includes('請款')){
                reviewUrl = `SCI/SciReimbursement.aspx?ProjectID=${projectId}`;
            }
        } else if (projectId.includes('CUL')) {
            // 文化執行計畫審核頁面

        } else if (projectId.includes('EDC')) {
            // 學校民間執行計畫審核頁面
        } else if (projectId.includes('CLB')) {
            // 學校社團執行計畫審核頁面
        } else if (projectId.includes('MUL')) {
            // 多元執行計畫審核頁面
        } else if (projectId.includes('LIT')) {
            // 素養執行計畫審核頁面
        } else if (projectId.includes('ACC')) {
            // 無障礙執行計畫審核頁面
        }

        if (reviewUrl) {
            // 跳轉到對應的審核頁面
            window.location.href = reviewUrl;
        } else {
            Swal.fire({
                title: '功能開發中',
                text: `該類型計畫 (${projectId}) 的執行計畫審核功能尚未開放`,
                icon: 'info',
                confirmButtonText: '確定'
            });
        }

    } catch (error) {
        console.error('處理執行計畫審核時發生錯誤:', error);
        Swal.fire({
            title: '系統錯誤',
            text: '處理執行計畫審核時發生錯誤',
            icon: 'error',
            confirmButtonText: '確定'
        });
    }
}

/**
 * 處理提送申請者修正計畫書 - 全域函數供按鈕直接調用
 */
function handleSendToApplicant() {
    // 取得選中的專案ID列表
    const selectedIds = getSelectedProjectIds();

    if (!selectedIds || selectedIds.length === 0) {
        Swal.fire({
            title: '提醒',
            text: '請先選擇要處理的案件',
            icon: 'warning',
            confirmButtonText: '確定'
        });
        return;
    }

    // SweetAlert2 確認提示
    Swal.fire({
        title: '確定提送申請者進行資料修正？',
        text: `選中 ${selectedIds.length} 件計畫，確定要提送給申請者修正計畫書嗎？`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: '確定提送',
        cancelButtonText: '取消'
    }).then((result) => {
        if (result.isConfirmed) {
            // 執行提送動作
            submitSendToApplicant(selectedIds);
        }
    });
}

/**
 * 提交提送申請者的動作
 */
function submitSendToApplicant(selectedIds) {
    // 顯示載入中
    Swal.fire({
        title: '處理中...',
        text: '正在提送案件給申請者',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    // 將選中的ID放入HiddenField (需要從後端取得ClientID)
    const hiddenField = document.getElementById('MainContent_hdnSelectedProjectIds');

    const joinedIds = selectedIds.join(',');

    hiddenField.value = joinedIds;

    // 觸發後端的按鈕點擊事件 (需要從後端取得PostBackEventReference)
    __doPostBack('MainContent$btnSendToApplicant', '');
}

/**
 * Type2 和 Type3 的提送至申請者功能 - 全域函數供按鈕直接調用
 */
function handleSendToApplicantType2Type3() {
    try {
        // 取得當前審查類型
        const currentType = window.ReviewChecklist ? window.ReviewChecklist.getCurrentType() : '2';

        // 檢查是否為 Type2 或 Type3
        if (currentType !== '2' && currentType !== '3') {
            Swal.fire({
                title: '權限錯誤',
                text: '提送至申請者功能只適用於領域審查(Type2)和技術審查(Type3)',
                icon: 'error',
                confirmButtonText: '確定'
            });
            return;
        }

        // 1. 收集選中的專案 (參考 handleBatchApproval 的邏輯)
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
        selectedCheckboxes.each(function() {
            const projectId = $(this).val();
            if (projectId && projectId.trim() !== '') {
                selectedIds.push(projectId);
            }
        });

        console.log('選中的計畫ID:', selectedIds);

        // SweetAlert2 確認提示
        Swal.fire({
            title: '確定提送申請者？',
            text: `選中 ${selectedIds.length} 件計畫，確定要提送給申請者嗎？`,
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: '確定提送',
            cancelButtonText: '取消'
        }).then((result) => {
            if (result.isConfirmed) {
                // 執行提送動作
                submitSendToApplicantType2Type3(selectedIds, currentType);
            }
        });

    } catch (error) {
        console.error('提送至申請者處理時發生錯誤:', error);
        Swal.fire({
            title: '系統錯誤',
            text: '處理提送操作時發生錯誤',
            icon: 'error',
            confirmButtonText: '確定'
        });
    }
}

/**
 * 提交提送申請者的動作（Type2 和 Type3）
 */
function submitSendToApplicantType2Type3(selectedIds, reviewType) {
    // 顯示載入中
    Swal.fire({
        title: '處理中...',
        text: '正在提送案件給申請者',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    // AJAX 呼叫後端 WebMethod
    $.ajax({
        type: "POST",
        url: "ReviewChecklist.aspx/SendToApplicant",
        data: JSON.stringify({
            projectIds: selectedIds,
            reviewType: reviewType
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        timeout: 30000
    }).done(function(response) {
        const result = response.d || response;

        if (result.Success) {
            // 成功 - 只顯示成功寄出的筆數
            const successCount = result.SuccessCount || 0;
            const message = `成功提送 ${successCount} 筆計畫給申請者`;

            Swal.fire({
                title: '提送成功',
                text: message,
                icon: 'success',
                confirmButtonText: '確定'
            }).then(() => {
                // 重新載入頁面或更新清單
                location.reload();
            });
        } else {
            // 失敗
            let errorMessage = result.Message || '提送失敗';
            if (result.ErrorMessages && result.ErrorMessages.length > 0) {
                errorMessage += '\n錯誤詳情：\n' + result.ErrorMessages.join('\n');
            }

            Swal.fire({
                title: '提送失敗',
                text: errorMessage,
                icon: 'error',
                confirmButtonText: '確定'
            });
        }
    }).fail(function(xhr, status, error) {
        Swal.fire({
            title: '系統錯誤',
            text: '提送過程中發生系統錯誤，請稍後再試',
            icon: 'error',
            confirmButtonText: '確定'
        });
        console.error('提送至申請者失敗:', xhr.responseText);
    });
}

/**
 * 取得選中的專案ID列表（Type4專用）
 */
function getSelectedProjectIds() {
    // Type4 決審階段不需要狀態過濾，直接回傳所有勾選的項目
    const selectedIds = [];
    const checkedBoxes = $('#content-type-4 .checkPlan:checked');

    checkedBoxes.each(function(index) {
        const projectId = $(this).val();
        if (projectId && projectId.trim() !== '') {
            selectedIds.push(projectId);
        }
    });

    return selectedIds;
}

/**
 * 頁面初始化函數
 */
function initializeReviewChecklistPage() {
    // 初始化審查清單頁面
    if (window.ReviewChecklist) {
        window.ReviewChecklist.init();
    }

    // Type4 初始化審查組別選項
    if (window.ReviewChecklist && window.ReviewChecklist.updateReviewGroupType4) {
        // 延遲執行以確保頁面完全載入
        setTimeout(function() {
            var initialCategory = $('#ddlCategory_Type4').val() || 'SCI';
            window.ReviewChecklist.updateReviewGroupType4(initialCategory);
        }, 100);
    }

    // 排序模式 Modal 開啟事件
    $('#sortModeModal').on('shown.bs.modal', function() {
        // 根據主要頁面的選項設定排序模式的預設值
        if (window.ReviewChecklist && window.ReviewChecklist.updateSortingReviewGroup) {
            // 設定預設年度
            var mainYear = $('#ddlYear_Type4').val() || '114';
            $('#sortingYear').val(mainYear);

            // 設定預設類別
            var mainCategory = $('#ddlCategory_Type4').val() || 'SCI';
            $('#sortingCategory').val(mainCategory);

            // 根據類別更新審查組別選項
            window.ReviewChecklist.updateSortingReviewGroup(mainCategory);

            // 延遲設定審查組別的預設值，等待選項載入完成
            setTimeout(function() {
                var mainReviewGroup = $('#ddlReviewGroup_Type4').val() || '';
                $('#sortingReviewGroup').val(mainReviewGroup);
            }, 500);
        }

        if (window.ReviewChecklist && window.ReviewChecklist.initSortingModal) {
            window.ReviewChecklist.initSortingModal();
        }
    });
}

// 當 DOM 準備好時執行初始化
$(function() {
    initializeReviewChecklistPage();
});

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
                // Type2, Type3, Type4 不需要檢查狀態，直接加入
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
 * 打開計畫詳情 Modal
 * @param {string} projectId - 專案編號
 */
function openPlanDetail(projectId) {

    // 取得當前審查類型
    const currentType = window.ReviewChecklist.getCurrentType();

    // 更新 Modal 標題
    updateModalTitle(currentType);

    // 使用 jQuery AJAX 請求後端資料
    $.ajax({
        type: "POST",
        url: "ReviewChecklist.aspx/GetPlanDetail",
        data: JSON.stringify({
            projectId: projectId,
            reviewType: currentType
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    }).done(function(response) {

        // 解析回應資料
        let result;
        if (typeof response.d === 'string') {
            result = JSON.parse(response.d);
        } else {
            result = response.d || response;
        }

        if (result && result.Success) {
            // 渲染計畫詳情到 Modal
            renderPlanDetailToModal(result.Data);
        } else {
            showModalError('無法取得計畫詳情');
        }

    }).fail(function(jqXHR, textStatus, errorThrown) {
        console.error('請求失敗:', textStatus, errorThrown);
        showModalError('載入計畫詳情時發生錯誤，請稍後再試');
    });
}

/**
 * 更新 Modal 標題
 * @param {string} reviewType - 審查類型
 */
function updateModalTitle(reviewType) {
    let titleText = '';

    switch (reviewType) {
        case '2':
            titleText = '審查結果與意見回覆 - 領域審查/初審';
            break;
        case '3':
            titleText = '審查結果與意見回覆 - 技術審查/複審';
            break;
        default:
            titleText = '審查結果與意見回覆';
            break;
    }

    $('#planDetailModal .modal-title, #planDetailModal h4').text(titleText);
}

/**
 * 渲染計畫詳情到 Modal
 * @param {Object} data - 計畫詳細資料
 */
function renderPlanDetailToModal(data) {
    if (!data) {
        showModalError('資料格式錯誤');
        return;
    }

    // 更新計畫基本資訊
    updateModalBasicInfo(data);

    // 更新評審意見表格
    updateModalReviewTable(data.ReviewComments || []);
}

/**
 * 更新 Modal 計畫基本資訊
 * @param {Object} data - 計畫資料
 */
function updateModalBasicInfo(data) {
    const $modalBody = $('#planDetailModal .modal-body');

    // 找到基本資訊區域並更新
    const basicInfoHtml = `
        <div class="bg-light-gray p-3 mb-4">
            <ul class="lh-lg">
                <li>
                    <span class="text-gray">年度 :</span>
                    <span>${data.Year || '--'}</span>
                </li>
                <li>
                    <span class="text-gray">計畫編號 :</span>
                    <span>${data.ProjectID || '--'}</span>
                </li>
                <li>
                    <span class="text-gray">計畫類別 :</span>
                    <span>${data.SubsidyPlanType || '--'}</span>
                </li>
                <li>
                    <span class="text-gray">主題、領域 :</span>
                    <span>${data.TopicField || '--'}</span>
                </li>
                <li>
                    <span class="text-gray">計畫名稱 :</span>
                    <span>${data.ProjectNameTw || '--'}</span>
                </li>
                <li>
                    <span class="text-gray">申請單位 :</span>
                    <span>${data.OrgName || '--'}</span>
                </li>
            </ul>
        </div>
    `;

    // 更新基本資訊（保留表格和按鈕區域）
    $modalBody.find('.bg-light-gray').parent().find('.bg-light-gray').replaceWith(basicInfoHtml);
}

/**
 * 更新 Modal 評審意見表格
 * @param {Array} reviewComments - 評審意見陣列
 */
function updateModalReviewTable(reviewComments) {
    const $tableBody = $('#planDetailModal .table tbody');

    // 清空現有內容
    $tableBody.empty();

    if (!reviewComments || reviewComments.length === 0) {
        // 顯示尚未回覆意見
        $tableBody.append(`
            <tr>
                <td colspan="4" class="text-center text-muted">尚未回覆意見</td>
            </tr>
        `);
        return;
    }

    // 渲染每一筆評審意見
    reviewComments.forEach(function(comment) {
        const reviewerName = comment.ReviewerName || '--';
        const totalScore = comment.TotalScore || '未評分';
        const reviewComment = formatTextWithLineBreaks(comment.ReviewComment || '--');
        const replyComment = formatTextWithLineBreaks(comment.ReplyComment || '--');

        const rowHtml = `
            <tr>
                <td>${reviewerName}</td>
                <td class="text-center">${totalScore}</td>
                <td style="white-space: pre-wrap; word-wrap: break-word;">${reviewComment}</td>
                <td style="white-space: pre-wrap; word-wrap: break-word;">${replyComment}</td>
            </tr>
        `;

        $tableBody.append(rowHtml);
    });
}

/**
 * 格式化文字並適當換行
 * @param {string} text - 原始文字
 * @returns {string} 格式化後的文字
 */
function formatTextWithLineBreaks(text) {
    if (!text || text === '--') return text;

    // 將長文字適當斷行，每60個字元插入一個換行
    return text.replace(/(.{60})/g, '$1\n');
}

/**
 * 顯示 Modal 錯誤訊息
 * @param {string} message - 錯誤訊息
 */
function showModalError(message) {
    const $modalBody = $('#planDetailModal .modal-body');
    $modalBody.html(`
        <div class="d-flex justify-content-center align-items-center" style="min-height: 200px;">
            <div class="text-center">
                <i class="fas fa-exclamation-triangle text-warning fa-3x mb-3"></i>
                <div class="text-muted">${message}</div>
            </div>
        </div>
    `);
}

/**
 * 批次不通過處理
 */
function handleBatchReject(actionText) {
    const currentType = window.ReviewChecklist.getCurrentType();
    const selectedCheckboxes = $('#content-type-' + currentType).find('.checkPlan:checked');

    if (selectedCheckboxes.length === 0) {
        Swal.fire('提醒', '請先選擇要處理的計畫項目', 'warning');
        return;
    }

    const selectedIds = [];
    selectedCheckboxes.each(function() {
        selectedIds.push($(this).val());
    });

    Swal.fire({
        title: '確認批次不通過',
        text: `共 ${selectedIds.length} 件計畫，確定設為不通過？`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        confirmButtonColor: '#d33'
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                title: '處理中...',
                allowOutsideClick: false,
                showConfirmButton: false,
                willOpen: () => Swal.showLoading()
            });

            $.ajax({
                type: "POST",
                url: "ReviewChecklist.aspx/BatchRejectProjects",
                data: JSON.stringify({
                    projectIds: selectedIds,
                    actionType: actionText,
                    reviewType: currentType
                }),
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            }).done(function(response) {
                const result = response.d || response;
                if (result.Success) {
                    Swal.fire('已完成', result.Message, 'success').then(() => {
                        $(`#MainContent_btnSearch_Type${currentType}`).trigger('click');
                    });
                } else {
                    Swal.fire('操作失敗', result.Message, 'error');
                }
            }).fail(function() {
                Swal.fire('系統錯誤', '請求失敗，請稍後再試', 'error');
            });
        }
    });
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
        url: "ReviewChecklist.aspx/BatchApproveType", // 註：此方法實際支援所有審查類型（Type1-4）
        data: JSON.stringify({
            projectIds: selectedIds,
            actionType: actionText,
            reviewType: currentType        }),
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

/**
 * 分頁功能管理器 - 支援Type1、Type2和Type3
 */
window.PaginationManager = {
    // 數據存儲
    data: {
        type1: [],
        type2: [],
        type3: [],
        type5: [],
        type6: []
    },

    // 當前頁
    currentPage: {
        type1: 1,
        type2: 1,
        type3: 1,
        type5: 1,
        type6: 1
    },

    // 總頁數
    totalPages: {
        type1: 0,
        type2: 0,
        type3: 0,
        type5: 0,
        type6: 0
    },

    // 通用設定
    pageSize: 10,

    init: function() {
        this.bindEvents();
        this.setupGlobalFunctions();
    },

    /**
     * 設定全域函數供後端調用
     */
    setupGlobalFunctions: function() {
        const self = this;

        // Type1 數據收集函數
        window.collectType1DataNow = function() {
            self.collectTypeData('type1');
        };

        // Type2 數據收集函數
        window.collectType2DataNow = function() {
            self.collectTypeData('type2');
        };

        // Type3 數據收集函數
        window.collectType3DataNow = function() {
            self.collectTypeData('type3');
        };

        // Type5 數據收集函數
        window.collectType5DataNow = function() {
            self.collectTypeData('type5');
        };

        // Type6 數據收集函數
        window.collectType6DataNow = function() {
            self.collectTypeData('type6');
        };
    },

    /**
     * 綁定分頁相關事件
     */
    bindEvents: function() {
        const self = this;

        // Type1 分頁按鈕點擊事件
        $(document).on('click', '#pagination-type1 .pagination-btn', function(e) {
            e.preventDefault();
            const page = parseInt($(this).data('page'));
            if (page && page !== self.currentPage.type1) {
                self.currentPage.type1 = page;
                self.renderPage('type1');
            }
        });

        // Type1 前一頁按鈕
        $(document).on('click', '#pagination-type1 .btn-prev-page', function(e) {
            e.preventDefault();
            if (self.currentPage.type1 > 1) {
                self.currentPage.type1--;
                self.renderPage('type1');
            }
        });

        // Type1 下一頁按鈕
        $(document).on('click', '#pagination-type1 .btn-next-page', function(e) {
            e.preventDefault();
            if (self.currentPage.type1 < self.totalPages.type1) {
                self.currentPage.type1++;
                self.renderPage('type1');
            }
        });

        // Type1 每頁顯示筆數選擇
        $(document).on('change', '#pagination-type1 .page-size-selector', function() {
            const newPageSize = parseInt($(this).val());
            if (newPageSize && newPageSize !== self.pageSize) {
                self.pageSize = newPageSize;
                self.currentPage.type1 = 1; // 重設到第一頁
                self.renderPage('type1');
            }
        });

        // Type1 跳到指定頁
        $(document).on('change', '#pagination-type1 .jump-to-page', function() {
            const targetPage = parseInt($(this).val());
            if (targetPage && targetPage !== self.currentPage.type1 && targetPage >= 1 && targetPage <= self.totalPages.type1) {
                self.currentPage.type1 = targetPage;
                self.renderPage('type1');
            }
        });

        // Type2 分頁按鈕點擊事件
        $(document).on('click', '#pagination-type2 .pagination-btn', function(e) {
            e.preventDefault();
            const page = parseInt($(this).data('page'));
            if (page && page !== self.currentPage.type2) {
                self.currentPage.type2 = page;
                self.renderPage('type2');
            }
        });

        // Type2 前一頁按鈕
        $(document).on('click', '#pagination-type2 .btn-prev-page', function(e) {
            e.preventDefault();
            if (self.currentPage.type2 > 1) {
                self.currentPage.type2--;
                self.renderPage('type2');
            }
        });

        // Type2 下一頁按鈕
        $(document).on('click', '#pagination-type2 .btn-next-page', function(e) {
            e.preventDefault();
            if (self.currentPage.type2 < self.totalPages.type2) {
                self.currentPage.type2++;
                self.renderPage('type2');
            }
        });

        // Type2 每頁顯示筆數選擇
        $(document).on('change', '#pagination-type2 .page-size-selector', function() {
            const newPageSize = parseInt($(this).val());
            if (newPageSize && newPageSize !== self.pageSize) {
                self.pageSize = newPageSize;
                self.currentPage.type2 = 1; // 重設到第一頁
                self.renderPage('type2');
            }
        });

        // Type2 跳到指定頁
        $(document).on('change', '#pagination-type2 .jump-to-page', function() {
            const targetPage = parseInt($(this).val());
            if (targetPage && targetPage !== self.currentPage.type2 && targetPage >= 1 && targetPage <= self.totalPages.type2) {
                self.currentPage.type2 = targetPage;
                self.renderPage('type2');
            }
        });

        // Type3 分頁按鈕點擊事件
        $(document).on('click', '#pagination-type3 .pagination-btn', function(e) {
            e.preventDefault();
            const page = parseInt($(this).data('page'));
            if (page && page !== self.currentPage.type3) {
                self.currentPage.type3 = page;
                self.renderPage('type3');
            }
        });

        // Type3 前一頁按鈕
        $(document).on('click', '#pagination-type3 .btn-prev-page', function(e) {
            e.preventDefault();
            if (self.currentPage.type3 > 1) {
                self.currentPage.type3--;
                self.renderPage('type3');
            }
        });

        // Type3 下一頁按鈕
        $(document).on('click', '#pagination-type3 .btn-next-page', function(e) {
            e.preventDefault();
            if (self.currentPage.type3 < self.totalPages.type3) {
                self.currentPage.type3++;
                self.renderPage('type3');
            }
        });

        // Type3 每頁顯示筆數選擇
        $(document).on('change', '#pagination-type3 .page-size-selector', function() {
            const newPageSize = parseInt($(this).val());
            if (newPageSize && newPageSize !== self.pageSize) {
                self.pageSize = newPageSize;
                self.currentPage.type3 = 1; // 重設到第一頁
                self.renderPage('type3');
            }
        });

        // Type3 跳到指定頁
        $(document).on('change', '#pagination-type3 .jump-to-page', function() {
            const targetPage = parseInt($(this).val());
            if (targetPage && targetPage !== self.currentPage.type3 && targetPage >= 1 && targetPage <= self.totalPages.type3) {
                self.currentPage.type3 = targetPage;
                self.renderPage('type3');
            }
        });

        // Type5 分頁按鈕點擊事件
        $(document).on('click', '#pagination-type5 .pagination-btn', function(e) {
            e.preventDefault();
            const page = parseInt($(this).data('page'));
            if (page && page !== self.currentPage.type5) {
                self.currentPage.type5 = page;
                self.renderPage('type5');
            }
        });

        // Type5 前一頁按鈕
        $(document).on('click', '#pagination-type5 .btn-prev-page', function(e) {
            e.preventDefault();
            if (self.currentPage.type5 > 1) {
                self.currentPage.type5--;
                self.renderPage('type5');
            }
        });

        // Type5 下一頁按鈕
        $(document).on('click', '#pagination-type5 .btn-next-page', function(e) {
            e.preventDefault();
            if (self.currentPage.type5 < self.totalPages.type5) {
                self.currentPage.type5++;
                self.renderPage('type5');
            }
        });

        // Type5 每頁顯示筆數選擇
        $(document).on('change', '#pagination-type5 .page-size-selector', function() {
            const newPageSize = parseInt($(this).val());
            if (newPageSize && newPageSize !== self.pageSize) {
                self.pageSize = newPageSize;
                self.currentPage.type5 = 1; // 重設到第一頁
                self.renderPage('type5');
            }
        });

        // Type5 跳到指定頁
        $(document).on('change', '#pagination-type5 .jump-to-page', function() {
            const targetPage = parseInt($(this).val());
            if (targetPage && targetPage !== self.currentPage.type5 && targetPage >= 1 && targetPage <= self.totalPages.type5) {
                self.currentPage.type5 = targetPage;
                self.renderPage('type5');
            }
        });

        // Type6 分頁按鈕點擊事件
        $(document).on('click', '#pagination-type6 .pagination-btn', function(e) {
            e.preventDefault();
            const page = parseInt($(this).data('page'));
            if (page && page !== self.currentPage.type6) {
                self.currentPage.type6 = page;
                self.renderPage('type6');
            }
        });

        // Type6 前一頁按鈕
        $(document).on('click', '#pagination-type6 .btn-prev-page', function(e) {
            e.preventDefault();
            if (self.currentPage.type6 > 1) {
                self.currentPage.type6--;
                self.renderPage('type6');
            }
        });

        // Type6 下一頁按鈕
        $(document).on('click', '#pagination-type6 .btn-next-page', function(e) {
            e.preventDefault();
            if (self.currentPage.type6 < self.totalPages.type6) {
                self.currentPage.type6++;
                self.renderPage('type6');
            }
        });

        // Type6 每頁顯示筆數選擇
        $(document).on('change', '#pagination-type6 .page-size-selector', function() {
            const newPageSize = parseInt($(this).val());
            if (newPageSize && newPageSize !== self.pageSize) {
                self.pageSize = newPageSize;
                self.currentPage.type6 = 1; // 重設到第一頁
                self.renderPage('type6');
            }
        });

        // Type6 跳到指定頁
        $(document).on('change', '#pagination-type6 .jump-to-page', function() {
            const targetPage = parseInt($(this).val());
            if (targetPage && targetPage !== self.currentPage.type6 && targetPage >= 1 && targetPage <= self.totalPages.type6) {
                self.currentPage.type6 = targetPage;
                self.renderPage('type6');
            }
        });
    },

    /**
     * 收集指定類型的資料
     */
    collectTypeData: function(type) {
        const typeNum = type.replace('type', '');

        // 嘗試多種表格選擇器
        let $tableBody = $(`#DataTable_Type${typeNum} tbody`);

        // 如果找不到 DataTable_TypeX，嘗試content-type-X內的table tbody
        if ($tableBody.length === 0) {
            $tableBody = $(`#content-type-${typeNum} .table tbody`);
        }

        if ($tableBody.length === 0) {
            console.warn(`找不到Type${typeNum}的表格元素`);
            return;
        }

        this.data[type] = [];
        const rows = $tableBody.find('tr');

        rows.each((index, row) => {
            this.data[type].push($(row).prop('outerHTML'));
        });

        // 更新總筆數顯示
        this.updateTotalCount(type);

        if (this.data[type].length > 0) {
            this.currentPage[type] = 1;
            this.renderPage(type);
        }
    },

    /**
     * 渲染指定類型的當前頁面
     */
    renderPage: function(type) {
        if (!this.data[type] || this.data[type].length === 0) {
            return;
        }

        const typeNum = type.replace('type', '');

        // 所有類型統一使用前端分頁 (仿照Type=1)
        this.totalPages[type] = Math.ceil(this.data[type].length / this.pageSize);

        const startIndex = (this.currentPage[type] - 1) * this.pageSize;
        const endIndex = startIndex + this.pageSize;
        const pageData = this.data[type].slice(startIndex, endIndex);

        // 嘗試多種表格選擇器
        let $tableBody = $(`#DataTable_Type${typeNum} tbody`);

        // 如果找不到 DataTable_TypeX，嘗試content-type-X內的table tbody
        if ($tableBody.length === 0) {
            $tableBody = $(`#content-type-${typeNum} .table tbody`);
        }

        if ($tableBody.length === 0) {
            console.warn(`找不到Type${typeNum}的表格元素進行渲染`);
            return;
        }

        // 清空表格
        $tableBody.empty();

        // 填入當前頁資料 (所有類型統一處理 - 仿照Type=1)
        pageData.forEach(function(row) {
            $tableBody.append(row);
        });

        // 更新分頁控件
        this.updatePaginationControls(type);
    },

    /**
     * 更新分頁控件
     */
    updatePaginationControls: function(type) {
        const typeNum = type.replace('type', '');
        const $pagination = $(`#pagination-type${typeNum} nav.pagination`);

        // 清空現有的分頁按鈕，只保留上下頁按鈕
        $pagination.find('.pagination-item, .ellipsis').remove();

        // 找到上下頁按鈕的位置
        const $prevBtn = $pagination.find('.btn-prev-page');
        const $nextBtn = $pagination.find('.btn-next-page');

        // 渲染頁碼按鈕
        this.renderPageButtons(type, $prevBtn, $nextBtn);

        // 更新上下頁按鈕狀態
        $prevBtn.prop('disabled', this.currentPage[type] <= 1);
        $nextBtn.prop('disabled', this.currentPage[type] >= this.totalPages[type]);

        // 更新page-number-control區域
        this.updatePageNumberControls(type);
    },

    /**
     * 更新page-number-control控制元素
     */
    updatePageNumberControls: function(type) {
        const typeNum = type.replace('type', '');
        const $container = $(`#pagination-type${typeNum}`);

        // 更新分頁資訊顯示
        const startItem = (this.currentPage[type] - 1) * this.pageSize + 1;
        const endItem = Math.min(this.currentPage[type] * this.pageSize, this.data[type].length);
        const totalItems = this.data[type].length;

        $container.find('.pagination-info').text(`顯示第 ${startItem} - ${endItem} 筆，共 ${totalItems} 筆`);

        // 更新跳到指定頁的選項
        const $jumpToPage = $container.find('.jump-to-page');
        $jumpToPage.empty();

        for (let i = 1; i <= this.totalPages[type]; i++) {
            const selected = i === this.currentPage[type] ? 'selected' : '';
            $jumpToPage.append(`<option value="${i}" ${selected}>${i}</option>`);
        }

        // 確保每頁顯示筆數選擇器的值是正確的
        $container.find('.page-size-selector').val(this.pageSize);
    },

    /**
     * 更新總筆數顯示
     */
    updateTotalCount: function(type) {
        const typeNum = type.replace('type', '');

        // 檢查是否有真正的數據
        const hasData = this.data[type] &&
            this.data[type].length > 0 &&
            !this.data[type][0].includes('無符合條件的資料');

        const totalCount = hasData ? this.data[type].length : 0;

        // 更新對應的總筆數顯示
        $(`#total-count-type${typeNum}`).text(totalCount);
    },


    /**
     * 渲染頁碼按鈕
     */
    renderPageButtons: function(type, $prevBtn, $nextBtn) {
        const currentPage = this.currentPage[type];
        const totalPages = this.totalPages[type];

        if (totalPages <= 5) {
            // 如果總頁數 <= 5，顯示所有頁碼
            for (let i = 1; i <= totalPages; i++) {
                const isActive = i === currentPage;
                const $pageBtn = $(`<button class="pagination-item pagination-btn ${isActive ? 'active' : ''}" data-page="${i}"><span class="page-number">${i}</span></button>`);
                $pageBtn.insertBefore($nextBtn);
            }
        } else {
            // 總頁數 > 5，使用省略號邏輯
            if (currentPage <= 3) {
                // 當前頁在前面：1 2 3 ... 最後頁
                for (let i = 1; i <= 3; i++) {
                    const isActive = i === currentPage;
                    const $pageBtn = $(`<button class="pagination-item pagination-btn ${isActive ? 'active' : ''}" data-page="${i}"><span class="page-number">${i}</span></button>`);
                    $pageBtn.insertBefore($nextBtn);
                }

                if (totalPages > 4) {
                    $(`<div class="pagination-item ellipsis"><span>...</span></div>`).insertBefore($nextBtn);
                    const $lastBtn = $(`<button class="pagination-item pagination-btn" data-page="${totalPages}"><span class="page-number">${totalPages}</span></button>`);
                    $lastBtn.insertBefore($nextBtn);
                }
            } else if (currentPage >= totalPages - 2) {
                // 當前頁在後面：1 ... 倒數3頁
                const $firstBtn = $(`<button class="pagination-item pagination-btn" data-page="1"><span class="page-number">1</span></button>`);
                $firstBtn.insertBefore($nextBtn);

                $(`<div class="pagination-item ellipsis"><span>...</span></div>`).insertBefore($nextBtn);

                for (let i = totalPages - 2; i <= totalPages; i++) {
                    const isActive = i === currentPage;
                    const $pageBtn = $(`<button class="pagination-item pagination-btn ${isActive ? 'active' : ''}" data-page="${i}"><span class="page-number">${i}</span></button>`);
                    $pageBtn.insertBefore($nextBtn);
                }
            } else {
                // 當前頁在中間：1 ... 當前頁-1 當前頁 當前頁+1 ... 最後頁
                const $firstBtn = $(`<button class="pagination-item pagination-btn" data-page="1"><span class="page-number">1</span></button>`);
                $firstBtn.insertBefore($nextBtn);

                $(`<div class="pagination-item ellipsis"><span>...</span></div>`).insertBefore($nextBtn);

                for (let i = currentPage - 1; i <= currentPage + 1; i++) {
                    const isActive = i === currentPage;
                    const $pageBtn = $(`<button class="pagination-item pagination-btn ${isActive ? 'active' : ''}" data-page="${i}"><span class="page-number">${i}</span></button>`);
                    $pageBtn.insertBefore($nextBtn);
                }

                $(`<div class="pagination-item ellipsis"><span>...</span></div>`).insertBefore($nextBtn);

                const $lastBtn = $(`<button class="pagination-item pagination-btn" data-page="${totalPages}"><span class="page-number">${totalPages}</span></button>`);
                $lastBtn.insertBefore($nextBtn);
            }
        }
    },

};

// 當頁面載入完成後初始化分頁功能
$(document).ready(function() {
    setTimeout(function() {
        if (typeof window.PaginationManager !== 'undefined') {
            window.PaginationManager.init();
        }
    }, 500);
});

// 讓 ReviewChecklistManager 指向原本的 ReviewChecklist
window.ReviewChecklistManager = window.ReviewChecklist;

/**
 * AJAX搜尋功能 - 支援Type1、Type2、Type3、Type5、Type6
 * @param {number} searchType - 搜尋類型 (1, 2, 3, 5, 6)
 */
function performAjaxSearch(searchType) {
    try {
        // 顯示載入狀態
        showSearchLoading(searchType, true);

        // 重設分頁 (除非是分頁導航觸發的搜尋)
        if (window.PaginationManager && !window.PaginationManager._isPaginationNavigation) {
            if (searchType === 5 || searchType === 6) {
                const type = `type${searchType}`;
                window.PaginationManager.currentPage[type] = 1;
            }
        }

        // 收集對應類型的查詢條件
        const searchData = collectSearchConditions(searchType);

        // 發送AJAX請求
        $.ajax({
            type: "POST",
            url: `${window.location.pathname}/AjaxSearch_Type${searchType}`,
            data: JSON.stringify(searchData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 30000
        }).done(function(response) {
            handleSearchResponse(response, searchType);
        }).fail(function(jqXHR, textStatus, errorThrown) {
            handleSearchError(jqXHR, textStatus, errorThrown, searchType);
        }).always(function() {
            showSearchLoading(searchType, false);
        });

    } catch (error) {
        console.error(`Type${searchType} AJAX搜尋時發生錯誤:`, error);
        showSearchLoading(searchType, false);
        Swal.fire({
            title: '搜尋錯誤',
            text: '準備搜尋時發生錯誤，請稍後再試',
            icon: 'error',
            confirmButtonText: '確定'
        });
    }
}

/**
 * 收集指定類型的查詢條件
 * @param {number} searchType - 搜尋類型
 * @returns {Object} 查詢條件物件
 */
function collectSearchConditions(searchType) {
    const data = {};

    try {
        switch (searchType) {
            case 1:
                data.year = $(`select[name$="ddlYear_Type1"]`).val() || '';
                data.category = $(`select[name$="ddlCategory_Type1"]`).val() || '';
                data.status = $(`select[name$="ddlStatus_Type1"]`).val() || '';
                data.orgName = $(`select[name$="ddlOrg_Type1"]`).val() || '';
                data.supervisor = $(`select[name$="ddlSupervisor_Type1"]`).val() || '';
                data.keyword = $('input[name="txtKeyword_Type1"]').val() || '';
                break;

            case 2:
                data.year = $(`select[name$="ddlYear_Type2"]`).val() || '';
                data.category = $(`select[name$="ddlCategory_Type2"]`).val() || '';
                data.reviewGroup = $(`select[name$="ddlReviewGroup_Type2"]`).val() || '';
                data.progress = $(`select[name$="ddlProgress_Type2"]`).val() || '';
                data.replyStatus = $(`select[name$="ddlReplyStatus_Type2"]`).val() || '';
                data.orgName = $(`select[name$="ddlOrg_Type2"]`).val() || '';
                data.supervisor = $(`select[name$="ddlSupervisor_Type2"]`).val() || '';
                data.keyword = $('input[name="txtKeyword_Type2"]').val() || '';
                break;

            case 3:
                data.year = $(`select[name$="ddlYear_Type3"]`).val() || '';
                data.category = $(`select[name$="ddlCategory_Type3"]`).val() || '';
                data.progress = $(`select[name$="ddlProgress_Type3"]`).val() || '';
                data.replyStatus = $(`select[name$="ddlReplyStatus_Type3"]`).val() || '';
                data.orgName = $(`select[name$="ddlOrg_Type3"]`).val() || '';
                data.supervisor = $(`select[name$="ddlSupervisor_Type3"]`).val() || '';
                data.keyword = $('input[name="txtKeyword_Type3"]').val() || '';
                break;

            case 5:
                data.year = $(`select[name$="ddlYear_Type5"]`).val() || '';
                data.category = $(`select[name$="ddlCategory_Type5"]`).val() || '';
                data.orgName = $(`select[name$="ddlOrg_Type5"]`).val() || '';
                data.supervisoryUnit = $(`select[name$="ddlDepartment_Type5"]`).val() || '';
                data.keyword = $('input[name="txtKeyword_Type5"]').val() || '';
                break;

            case 6:
                data.year = $(`select[name$="ddlYear_Type6"]`).val() || '';
                data.category = $(`select[name$="ddlCategory_Type6"]`).val() || '';
                data.orgName = $(`select[name$="ddlOrg_Type6"]`).val() || '';
                data.supervisoryUnit = $(`select[name$="ddlDepartment_Type6"]`).val() || '';
                data.keyword = $('input[name="txtKeyword_Type6"]').val() || '';
                break;

            default:
                throw new Error(`不支援的搜尋類型: ${searchType}`);
        }

        return data;

    } catch (error) {
        console.error('收集查詢條件時發生錯誤:', error);
        throw error;
    }
}

/**
 * 顯示/隱藏搜尋載入狀態
 * @param {number} searchType - 搜尋類型
 * @param {boolean} isLoading - 是否載入中
 */
function showSearchLoading(searchType, isLoading) {
    const $button = $(`#btnSearch_Type${searchType}`);

    if (isLoading) {
        $button.prop('disabled', true);
        $button.html('<i class="fas fa-spinner fa-spin"></i> 查詢中...');
    } else {
        $button.prop('disabled', false);
        $button.html('🔍 查詢');
    }
}

/**
 * 處理搜尋回應
 * @param {Object} response - 後端回應
 * @param {number} searchType - 搜尋類型
 */
function handleSearchResponse(response, searchType) {
    try {
        let result;
        if (typeof response.d === 'string') {
            result = JSON.parse(response.d);
        } else {
            result = response.d || response;
        }

        if (result && result.success) {
            // 使用現有的renderSearchResults方法渲染結果 (所有類型統一處理)
            if (typeof window.ReviewChecklistManager !== 'undefined') {
                window.ReviewChecklistManager.renderSearchResults(result.data, searchType);
            }

            // 延遲執行分頁功能初始化
            setTimeout(function() {
                const collectFunction = window[`collectType${searchType}DataNow`];
                if (typeof collectFunction === 'function') {
                    collectFunction();
                }
                window.ReviewChecklistManager.updateCheckboxTargets();
            }, 500);

        } else {
            throw new Error(result.message || '搜尋失敗');
        }

    } catch (error) {
        console.error('處理搜尋回應時發生錯誤:', error);
        handleSearchError({ responseText: error.message }, 'parseError', error.toString(), searchType);
    }
}

/**
 * 處理搜尋錯誤
 * @param {Object} jqXHR - AJAX錯誤物件
 * @param {string} textStatus - 錯誤狀態文字
 * @param {string} errorThrown - 錯誤訊息
 * @param {number} searchType - 搜尋類型
 */
function handleSearchError(jqXHR, textStatus, errorThrown, searchType) {
    console.error(`Type${searchType} AJAX搜尋失敗:`, textStatus, errorThrown);

    let errorMessage = '搜尋時發生錯誤，請稍後再試';

    if (jqXHR.status === 500) {
        errorMessage = '伺服器內部錯誤，請聯繫系統管理員';
    } else if (jqXHR.status === 0) {
        errorMessage = '網路連線錯誤，請檢查網路連線';
    } else if (textStatus === 'timeout') {
        errorMessage = '搜尋請求超時，請稍後再試';
    }

    Swal.fire({
        title: '搜尋失敗',
        text: errorMessage,
        icon: 'error',
        confirmButtonText: '確定'
    });
}

