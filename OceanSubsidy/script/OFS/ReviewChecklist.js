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

                // 重新初始化 checkbox 功能
                updateCheckboxTargets();

                console.log('已渲染', results.length, '筆搜尋結果');
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
            console.log('已渲染 Type-4', results.length, '筆搜尋結果');
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
     * 建立 Type4 表格行 (決審核定)
     * @param {Object} item - 資料項目
     * @param {number} index - 序號
     * @returns {string} HTML 字串
     */
    function createType4TableRow(item, index) {
        const formattedApplicationAmount = formatAmount(item.ApplicationAmount);
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
                <td data-th="申請經費:" width="120" style="text-align: center; text-wrap: nowrap;">${formattedApplicationAmount}</td>
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
            url: '/OFS/ReviewChecklist.aspx/SaveApprovalMode_Type4',
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
     * 共用函數：載入審查組別選項
     * @param {string} category - 選擇的類別
     * @param {jQuery} $targetSelect - 目標下拉選單元素
     */
    function loadReviewGroupOptions(category, $targetSelect) {
        // 清空現有選項
        $targetSelect.empty();

        if (!category) {
            // 類別為空時，保持空白
            return;
        }

        // 顯示載入中
        $targetSelect.prop('disabled', true);
        $targetSelect.html('<option value="">載入中...</option>');

        // AJAX 請求取得審查組別選項
        $.ajax({
            type: "POST",
            url: "ReviewChecklist.aspx/GetReviewGroupOptions",
            data: JSON.stringify({ category: category }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function(response) {
                if (response.d.success) {
                    // 清空並重新填入選項（不自動加入「全部」選項）
                    $targetSelect.empty();
                    $.each(response.d.options, function(index, option) {
                        $targetSelect.append($('<option>', {
                            value: option.Value,
                            text: option.Text
                        }));
                    });
                } else {
                    console.error('載入審查組別選項失敗:', response.d.message);
                    $targetSelect.empty();
                }
                $targetSelect.prop('disabled', false);
            },
            error: function(xhr, status, error) {
                console.error('AJAX 請求失敗:', error);
                $targetSelect.empty();
                $targetSelect.prop('disabled', false);
            }
        });
    }

    /**
     * 載入排序模式的審查組別選項 (當類別改變時)
     */
    function loadSortingReviewGroupOptions() {
        const category = $('#sortingCategory').val();
        const $reviewGroupSelect = $('#sortingReviewGroup');
        loadReviewGroupOptions(category, $reviewGroupSelect);
    }

    /**
     * 初始化排序模式 Modal (當 Modal 開啟時)
     */
    function initSortingModal() {
        // 確保預設選中科專
        $('#sortingCategory').val('SCI');
        
        // 自動載入科專的審查組別選項
        loadSortingReviewGroupOptions();
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
                console.log('排序已更新，請手動儲存');
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
        loadSortingReviewGroupOptions: loadSortingReviewGroupOptions,
        loadReviewGroupOptions: loadReviewGroupOptions,
        initSortingModal: initSortingModal,
    };
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
    console.log('打開計畫詳情:', projectId);
    
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
        console.log('後端回應:', response);
        
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
        url: "ReviewChecklist.aspx/BatchApproveType1", // 註：此方法實際支援所有審查類型（Type1-4）
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