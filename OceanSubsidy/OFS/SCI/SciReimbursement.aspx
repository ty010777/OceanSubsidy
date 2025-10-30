<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SciReimbursement.aspx.cs" Inherits="OFS_SCI_SciReimbursement" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/SCI/SciInprogress.master" EnableViewState="true" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />

    <!-- 內容區塊 -->
    <div class="block rounded-top-4">
        <ul class="teal-dark-tabs">
            <li class="active"><a class="tab-link" href="#" onclick="loadPhaseData(1)">第一期</a></li>
            <li><a class="tab-link" href="#" onclick="loadPhaseData(2)">第二期</a></li>
        </ul>
        
        <!-- 檔案上傳區塊 - 第一期時隱藏 -->
        <div id="fileUploadSection">
            <h5 class="square-title mt-4">檔案上傳</h5>
            <p class="text-pink mt-3 lh-base">請下載附件範本，經費支用表及明細表請上傳Excel檔。
            </p>
            
            <div class="table-responsive mt-3">
                <table class="table align-middle gray-table">
                    <thead>
                        <tr>
                            <th width="70" class="text-center">附件編號</th>
                            <th>附件名稱</th>
                            <th width="200" class="text-center">狀態</th>
                            <th>上傳附件</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td class="text-center">1</td>
                            <td>
                                <div><span class="text-pink">*</span>經費支用表及明細表(Excel)</div>
                                <button class="btn btn-sm btn-teal-dark rounded-pill mt-2" type="button" onclick="downloadTemplate()">
                                    <i class="fas fa-file-download me-1"></i>
                                    範本下載
                                </button>
                            </td>
                            <td class="text-center">
                                <span id="uploadStatus1" class="text-pink">尚未上傳</span>
                            </td>
                            <td>
                                <div class="d-flex align-items-center gap-2">
                                    <input type="file" id="fileInput1" accept=".xlsx,.xls" style="display: none;" onchange="handleFileUpload(1, this)">
                                    <button class="btn btn-sm btn-teal-dark" type="button" onclick="document.getElementById('fileInput1').click()">
                                        <i class="fas fa-file-upload me-1"></i>
                                        上傳
                                    </button>
                                    <div id="uploadedFile1" style="display: none;" class=" align-items-center gap-2">
                                        <a href="#" id="downloadLink1" class="btn btn-sm btn-outline-teal" onclick="downloadUploadedFile(1)">
                                            <i class="fas fa-download me-1"></i>
                                            <span id="fileName1"></span>
                                        </a>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr> 
                            <td class="text-center">2</td>
                            <td><span class="text-pink">*</span>憑證(PDF)</td>
                            <td class="text-center">
                                <span id="uploadStatus2" class="text-pink">尚未上傳</span>
                            </td>
                            <td>
                                <div class="d-flex align-items-center gap-2">
                                    <input type="file" id="fileInput2" accept=".pdf" style="display: none;" onchange="handleFileUpload(2, this)">
                                    <button class="btn btn-sm btn-teal-dark" type="button" onclick="document.getElementById('fileInput2').click()">
                                        <i class="fas fa-file-upload me-1"></i>
                                        上傳
                                    </button>
                                    <div id="uploadedFile2"  class=" align-items-center gap-2">
                                        <a href="#" id="downloadLink2" class="btn btn-sm btn-outline-teal" onclick="downloadUploadedFile(2)">
                                            <i class="fas fa-download me-1"></i>
                                            <span id="fileName2"></span>
                                        </a>
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

        <h5 class="square-title mt-4">實際請款 <span id="phaseNote" class="text-muted fs-6"></span></h5>
        <div class="d-flex gap-4 py-3">
            <p class="text-teal fw-bold mb-0">核定經費: <span id="approvedSubsidy">0</span> 元</p>
            <p class="text-orange fw-bold mb-0" id="remainingAmountSection" style="display: none;">賸餘款: <span id="remainingAmount">0</span> 元</p>
        </div>
        <!-- 隱藏欄位：累計撥付金額 -->
        <input type="hidden" id="totalActualPaidAmount" value="0">
        <div class="table-responsive">
            <table class="table align-middle gray-table">
                <thead>
                    <tr>
                        <th width="120" class="text-center">本期請款金額(元)</th>
                        <th width="120" class="text-center">前期已撥付金額(元)</th>
                        <th width="120" class="text-center">累積實支金額(元)</th>
                        <th width="120" class="text-center">累積經費執行率</th>
                        <th width="120" class="text-center">支用比</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td class="text-center" id="currentAmount">--</td>
                        <td class="text-center" id="previousAmount">--</td>
                        <td class="text-center" id="accumulatedAmountCell">
                            <span id="accumulatedAmount">--</span>
                            <input type="number" id="accumulatedAmountInput" class="form-control text-center" placeholder="請輸入累積實支金額" min="0" step="1" oninput="this.value = this.value.replace(/[^0-9]/g, '')" onkeypress="return event.charCode >= 48 && event.charCode <= 57" style="width: 200px; margin: 0 auto;display: none">
                        </td>
                        <td class="text-center" id="executionRate">--</td>
                        <td class="text-center" id="usageRatio">--</td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!-- 實際撥款統計表格 - 只在狀態為「通過」時顯示 -->
        <div id="actualPaymentSection" style="display: none;">
            <h5 class="square-title mt-4">實際撥款統計</h5>
            <div class="table-responsive">
                <table class="table align-middle gray-table">
                    <thead>
                        <tr>
                            <th width="200" class="text-center">本期實際撥款(元)</th>
                            <th width="200" class="text-center">累積實際撥款</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td class="text-center" id="currentActualPayment">--</td>
                            <td class="text-center" id="cumulativeActualPayment">--</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
    
    <!-- 底部區塊 -->
    <div class="block-bottom bg-light-teal">
        <button type="button" class="btn btn-outline-teal" onclick="submitReimbursement(true)">
            暫存
        </button>
        <button type="button" class="btn btn-teal" onclick="submitReimbursement()">
            <i class="fas fa-check"></i>
            提送
        </button>
    </div>

    <!-- 審查結果視窗 -->
    <div class="scroll-bottom-panel">
        <h5 class="text-pink fs-18 fw-bold mb-3">審查結果</h5>

        <ul class="d-flex flex-column gap-3 mb-3">

            <li class="d-flex gap-4 align-items-center">

                <!-- 本期 -->
                <div class="d-flex gap-2">
                    <span class="text-gray">本期撥款 :</span>
                    <input type="number" id="currentPayment"
                           class="form-control text-center text-teal fw-bold"
                           value="0" min="0" step="1" oninput="this.value = this.value.replace(/[^0-9]/g, '')" onkeypress="return event.charCode >= 48 && event.charCode <= 57" style="width: 120px;">
                </div>

                <!-- 如果有上一期 -->
                <div class="d-flex gap-2" id="previousPaymentContainer" >
                    <span class="text-gray">上期撥款 :</span>
                    <span class="text-teal fw-bold" id="previousPayment">0</span>
                </div>

            </li>
            <li class="d-flex gap-2">
                <span class="text-gray mt-2">審查結果 :</span>
                <div class="d-flex flex-column gap-2 align-items-start flex-grow-1">
                    <div class="form-check-input-group d-flex text-nowrap mt-2 align-items-center">
                        <input id="radio-pass" class="form-check-input check-teal" type="radio" name="reviewResult" >
                        <label for="radio-pass">通過</label>
                        <input id="radio-return" class="form-check-input check-teal" type="radio" name="reviewResult" checked="">
                        <label for="radio-return">退回修改</label>
                    </div>
                    <span class="form-control textarea w-100" role="textbox" contenteditable="" data-placeholder="請輸入原因" aria-label="文本輸入區域" id="reviewComment"></span>
                </div>
            </li>

        </ul>
        <button type="button" class="btn btn-teal d-table mx-auto" id = "confirmReviewBtn">確定撥款</button>
    </div>

    <script>
        var currentProjectID = '<%= Request.QueryString["ProjectID"] %>';
        var currentPhase = 1; // 公共變數：代表目前是第幾期 (1, 2)
        
        // 頁面載入時自動帶入期別資料
        $(document).ready(function() {
            // 從URL參數讀取stage，預設為1
            var urlParams = new URLSearchParams(window.location.search);
            var stage = parseInt(urlParams.get('stage')) || 1;
            
            loadPhaseData(stage);
            $('#confirmReviewBtn').on('click', submitReview);

        });
        
        // 載入期別資料
        function loadPhaseData(phase) {
            currentPhase = phase; // 更新公共變數
            console.log('Loading phase:', phase, 'for project:', currentProjectID);
            
            $.ajax({
                url: 'SciReimbursement.aspx/GetPhaseData',
                type: 'POST',
                data: JSON.stringify({ 
                    projectID: currentProjectID, 
                    phaseOrder: phase 
                }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function(response) {
                    if (response.d && response.d.Success) {
                        var data = response.d.Data;
                        
                        // 更新核定經費
                        $('#approvedSubsidy').text(formatNumber(data.ApprovedSubsidy));
                        
                        // 更新說明
                        $('#phaseNote').text(data.Note || '');
                        
                        // 更新隱藏的累計撥付金額
                        $('#totalActualPaidAmount').val(data.TotalActualPaidAmount || 0);
                        
                        // 更新表格資料
                        updateTableData(phase, data);
                        
                        // 更新審查結果視窗
                        updateReviewPanel(phase, data);
                        
                        // 控制檔案上傳區塊顯示
                        updateFileUploadVisibility(phase);
                        
                        // 更新標籤狀態
                        updateTabStatus(phase);
                        
                        // 更新實際撥款統計資料
                        updateActualPaymentStats(data);
                        
                        // 更新賸餘款顯示
                        updateRemainingAmount();
                        
                        // 根據請款狀態控制UI顯示
                        updateUIVisibilityByStatus(data.IsReimbursementInProgress, data.CurrentStatus);
                        
                        // 載入已上傳檔案
                        loadUploadedFiles();
                        
                        console.log('Phase data loaded successfully:', data);
                    } else {
                        console.error('Failed to load phase data:', response.d.Message);
                        alert('載入資料失敗: ' + (response.d.Message || '未知錯誤'));
                    }
                },
                error: function(xhr, status, error) {
                    console.error('Ajax error:', error);
                    alert('載入資料時發生錯誤: ' + error);
                }
            });
        }
        
        // 更新表格資料
        function updateTableData(phase, data) {
            $('#currentAmount').text(formatNumber(data.CurrentAmount));
            $('#previousAmount').text(data.PreviousAmount || '--');
            $('#executionRate').text(data.ExecutionRate || '--');
            $('#usageRatio').text(data.UsageRatio || '--');
            
            if (phase === 1) {
                // 第一期：顯示文字
                $('#accumulatedAmount').show().text(data.AccumulatedAmount || '--');
                $('#accumulatedAmountInput').hide();
            } else {
                // 第二期：顯示輸入框
                $('#accumulatedAmount').hide();
                $('#accumulatedAmountInput').show().val(data.AccumulatedAmount);
                calculatePhase2Values();
                
                // 綁定累積實支金額輸入框的變化事件
                $('#accumulatedAmountInput').off('input').on('input', function() {
                    calculatePhase2Values();
                    updateRemainingAmount(); // 即時更新賸餘款
                });
            }
        }
        
        // 第二期動態計算
        function calculatePhase2Values() {
            var accumulated = parseFloat($('#accumulatedAmountInput').val()) || 0;
            var previous = parseFloat($('#previousAmount').text().replace(/,/g, '')) || 0;
            var approved = parseFloat($('#approvedSubsidy').text().replace(/,/g, '')) || 0;
            var totalPaid = parseFloat($('#totalActualPaidAmount').val()) || 0;
            
            // 本期請款金額 = 累積實支 - 前期撥付
            $('#currentAmount').text(formatNumber(Math.max(0, accumulated - previous)));
            
            // 執行率 = 累積實支 ÷ 核定經費 × 100%
            var rate = approved > 0 ? (accumulated / approved * 100) : 0;
            $('#executionRate').text(rate.toFixed(2) + '%');
            
            // 支用比 = 累積實支 ÷ 累計撥付 × 100%
            var usage = totalPaid > 0 ? (accumulated / totalPaid * 100) : 0;
            $('#usageRatio').text(usage.toFixed(2) + '%');
        }
        
        // 更新標籤狀態
        function updateTabStatus(activePhase) {
            $('.teal-dark-tabs li').removeClass('active');
            $('.teal-dark-tabs li').eq(activePhase - 1).addClass('active');
        }
        
        // 更新審查結果視窗
        function updateReviewPanel(phase, data) {
            // 更新本期撥款
            $('#currentPayment').val(data.CurrentAmount);
            
            // 根據期別控制上期撥款顯示
            if (phase === 1) {
                $('#previousPayment').text(data.PreviousAmount);
            } else {
                $('#previousPaymentContainer').show();
                $('#previousPayment').text(data.PreviousAmount || '--');
            }
        }
        
        // 根據期別控制檔案上傳區塊顯示（此函數會被狀態控制覆蓋）
        function updateFileUploadVisibility(phase) {
            // 此函數的邏輯現在由 updateUIVisibilityByStatus 統一處理
            // 保留此函數以避免錯誤，但實際顯示控制由狀態決定
        }
        
        // 提送請款（isDraft: true=暫存, false/undefined=提送）
        function submitReimbursement(isDraft = false) {
            var currentAmount = parseFloat($('#currentAmount').text().replace(/,/g, '')) || 0;
            var accumulatedAmount = currentPhase === 2 ? (parseFloat($('#accumulatedAmountInput').val()) || 0) : 0;
            
            // 驗證
            if (currentPhase === 2 && accumulatedAmount <= 0) {
                Swal.fire('錯誤', '請輸入累積實支金額', 'error');
                return;
            }
            if (!isDraft && currentAmount <= 0) {
                Swal.fire('錯誤', '本期請款金額不可為0', 'error');
                return;
            }
            
            // 第二期請款金額限制檢查
            if (currentPhase === 2) {
                var approvedSubsidy = parseFloat($('#approvedSubsidy').text().replace(/,/g, '')) || 0;
                var maxAllowedAmount = approvedSubsidy * 0.6; // 核定金額的60%
                
                if (currentAmount > maxAllowedAmount) {
                    Swal.fire('錯誤', `第二期請款金額不可超過核定金額的60% (${formatNumber(maxAllowedAmount)} 元)`, 'error');
                    return;
                }
            }
            
            // 確認操作
            let actionText = isDraft ? '暫存' : '提送';
            let confirmText = isDraft ? '暫存' : '提送';
            Swal.fire({
                title: `確定${actionText}請款？`,
                text: '本期請款: ' + formatNumber(currentAmount) + ' 元',
                icon: 'question',
                showCancelButton: true,
                confirmButtonText: confirmText,
                cancelButtonText: '取消'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: 'SciReimbursement.aspx/SubmitReimbursement',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify({
                            projectID: currentProjectID,
                            phaseOrder: currentPhase,
                            currentRequestAmount: currentAmount,
                            accumulatedAmount: accumulatedAmount,
                            isDraft: isDraft
                        }),
                        dataType: 'json',
                        success: function(response) {
                            if (response.d.Success) {
                                Swal.fire('成功', response.d.Message, 'success').then(() => location.reload());
                            } else {
                                Swal.fire('失敗', response.d.Message, 'error');
                            }
                        },
                        error: function() {
                            Swal.fire('錯誤', '系統發生錯誤', 'error');
                        }
                    });
                }
            });
        }
        function submitReview() {
            let currentPayment = parseFloat($('#currentPayment').val()) || 0;
            let reviewResult = $('#radio-pass').is(':checked') ? 'pass' : 'return';
            let reviewComment = $('#reviewComment').text().trim();

            // 如果是第二期請款且審核通過，顯示專案將設為「已結案」的警告
            if (currentPhase === 2 && reviewResult === 'pass') {
                Swal.fire({
                    title: '確認通過請款',
                    text: '請款通過後專案將設為「已結案」，是否通過請款？',
                    icon: 'question',
                    showCancelButton: true,
                    confirmButtonColor: '#3085d6',
                    cancelButtonColor: '#6c757d',
                    confirmButtonText: '通過',
                    cancelButtonText: '取消'
                }).then((result) => {
                    if (result.isConfirmed) {
                        executeReviewSubmit(currentPayment, reviewResult, reviewComment);
                    }
                });
            } else {
                executeReviewSubmit(currentPayment, reviewResult, reviewComment);
            }
        }

        // 執行審核提交
        function executeReviewSubmit(currentPayment, reviewResult, reviewComment) {
            $.ajax({
                url: 'SciReimbursement.aspx/ReviewPayment',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: JSON.stringify({
                    projectID: currentProjectID,
                    phaseOrder: currentPhase,
                    currentPayment: currentPayment,
                    reviewResult: reviewResult,
                    reviewComment: reviewComment
                }),
                success: function (response) {
                    if (response.d && response.d.Success) {
                        Swal.fire('成功', '審查結果已提交', 'success').then(() => {
                            location.reload();
                        });
                    } else {
                        Swal.fire('失敗', response.d ? response.d.Message : '未知錯誤', 'error');
                    }
                },
                error: function () {
                    Swal.fire('錯誤', '系統發生錯誤', 'error');
                }
            });
        }

        // 更新實際撥款統計資料
        function updateActualPaymentStats(data) {
            if (data.CurrentStatus === '通過') {
                // 更新統計數據
                $('#currentActualPayment').text(formatNumber(data.CurrentActualPayment));
                $('#cumulativeActualPayment').text(formatNumber(data.CumulativeActualPayment));
                // 顯示實際撥款統計區塊
                $('#actualPaymentSection').show();
            } else {
                // 隱藏實際撥款統計區塊
                $('#actualPaymentSection').hide();
            }
        }

        // 更新賸餘款顯示（只在第二期顯示）
        function updateRemainingAmount() {
            if (currentPhase === 2) {
                var approvedSubsidy = parseFloat($('#approvedSubsidy').text().replace(/,/g, '')) || 0;
                var accumulatedAmount = parseFloat($('#accumulatedAmountInput').val()) || 0;
                var remainingAmount = approvedSubsidy - accumulatedAmount;
                
                $('#remainingAmount').text(formatNumber(remainingAmount));
                $('#remainingAmountSection').show();
            } else {
                $('#remainingAmountSection').hide();
            }
        }

        // 根據審核狀態控制UI顯示
        function updateUIVisibilityByStatus(isReimbursementInProgress, currentStatus) {
            // 檔案上傳區塊只在第二期顯示
            if (currentPhase === 2) {
                if (isReimbursementInProgress) {
                    // 當狀態為「審核中」時，顯示審核相關UI元素
                    $('#fileUploadSection').show(); // 檔案上傳區塊隱藏
                    $('.block-bottom').hide(); // 暫存/提送按鈕隱藏
                    $('.scroll-bottom-panel').show(); // 審核結果視窗顯示
                } else if (currentStatus === '通過') {
                    // 當狀態為「通過」時，只顯示檔案上傳區塊供查看，隱藏操作按鈕
                    $('#fileUploadSection').show(); // 檔案上傳區塊顯示（供查看）
                    $('.block-bottom').hide(); // 暫存/提送按鈕隱藏
                    $('.scroll-bottom-panel').hide(); // 審核結果視窗隱藏
                } else {
                    // 當狀態為「請款中」或其他狀態時，顯示編輯UI
                    $('#fileUploadSection').show(); // 檔案上傳區塊顯示
                    $('.block-bottom').show(); // 暫存/提送按鈕顯示
                    $('.scroll-bottom-panel').hide(); // 審核結果視窗隱藏
                }
            } else {
                // 第一期：隱藏檔案上傳區塊
                $('#fileUploadSection').hide();
                
                if (isReimbursementInProgress) {
                    // 當狀態為「審核中」時，顯示審核相關UI元素
                    $('.block-bottom').hide(); // 暫存/提送按鈕隱藏
                    $('.scroll-bottom-panel').show(); // 審核結果視窗顯示
                } else if (currentStatus === '通過') {
                    // 當狀態為「通過」時，隱藏操作按鈕
                    $('.block-bottom').hide(); // 暫存/提送按鈕隱藏
                    $('.scroll-bottom-panel').hide(); // 審核結果視窗隱藏
                } else {
                    // 當狀態為「請款中」或其他狀態時，顯示編輯UI
                    $('.block-bottom').show(); // 暫存/提送按鈕顯示
                    $('.scroll-bottom-panel').hide(); // 審核結果視窗隱藏
                }
            }
        }

        // 下載範本
        function downloadTemplate() {
            window.location.href = `SciReimbursement.aspx?action=download&projectID=${encodeURIComponent(currentProjectID)}`;
        }

        // 處理檔案上傳
        function handleFileUpload(fileType, fileInput) {
            const file = fileInput.files[0];
            if (!file) {
                return;
            }
            
            // 驗證檔案類型
            const allowedTypes = fileType === 1 ? ['.xlsx', '.xls'] : ['.pdf'];
            const fileExt = '.' + file.name.split('.').pop().toLowerCase();
            if (!allowedTypes.includes(fileExt)) {
                Swal.fire('錯誤', `請上傳 ${allowedTypes.join(' 或 ')} 格式的檔案`, 'error');
                fileInput.value = '';
                return;
            }
            
            // 顯示上傳進度
            Swal.fire({
                title: '上傳中...',
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });
            
            // 建立FormData
            const formData = new FormData();
            formData.append('file', file);
            formData.append('fileType', fileType);
            
            // 使用iframe方式上傳（因為需要處理檔案）
            const iframe = document.createElement('iframe');
            iframe.style.display = 'none';
            iframe.name = 'uploadFrame';
            document.body.appendChild(iframe);
            
            const form = document.createElement('form');
            form.method = 'POST';
            form.enctype = 'multipart/form-data';
            form.target = 'uploadFrame';
            form.action = `SciReimbursement.aspx?action=upload&ProjectID=${encodeURIComponent(currentProjectID)}`;
            
            const fileField = document.createElement('input');
            fileField.type = 'file';
            fileField.name = 'file';
            fileField.files = fileInput.files;
            form.appendChild(fileField);
            
            const typeField = document.createElement('input');
            typeField.type = 'hidden';
            typeField.name = 'fileType';
            typeField.value = fileType;
            form.appendChild(typeField);
            
            document.body.appendChild(form);
            
            // 監聽上傳完成
            iframe.onload = function() {
                try {
                    const response = iframe.contentDocument.body.textContent;
                    if (response.startsWith('SUCCESS:')) {
                        const fileName = response.substring(8);
                        Swal.fire('成功', '檔案上傳成功', 'success');
                        loadUploadedFiles(); // 重新載入檔案列表
                    } else if (response.startsWith('ERROR:')) {
                        const errorMsg = response.substring(6);
                        Swal.fire('錯誤', errorMsg, 'error');
                    } else {
                        Swal.fire('錯誤', '上傳過程發生未知錯誤', 'error');
                    }
                } catch (e) {
                    Swal.fire('錯誤', '上傳過程發生錯誤', 'error');
                }
                
                // 清理
                document.body.removeChild(form);
                document.body.removeChild(iframe);
                fileInput.value = '';
            };
            
            form.submit();
        }
        
        // 載入已上傳檔案
        function loadUploadedFiles() {
            $.ajax({
                url: 'SciReimbursement.aspx/GetUploadedFiles',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ projectID: currentProjectID }),
                dataType: 'json',
                success: function(response) {
                    if (response.d && response.d.Success) {
                        updateUploadedFilesDisplay(response.d.Files);
                    }
                },
                error: function() {
                    console.error('載入已上傳檔案清單失敗');
                }
            });
        }
        
        // 更新已上傳檔案顯示
        function updateUploadedFilesDisplay(files) {
            // 初始化狀態
            $('#uploadStatus1').text('尚未上傳').removeClass('text-success').addClass('text-pink');
            $('#uploadStatus2').text('尚未上傳').removeClass('text-success').addClass('text-pink');
            $('#uploadedFile1').hide();
            $('#uploadedFile2').hide();
            
            // 更新檔案狀態
            files.forEach(function(file) {
                let fileNum = file.FileCode === 'REIMBURSE_EXPENSE' ? 1 : 2;
                
                $(`#uploadStatus${fileNum}`).text('已上傳').removeClass('text-pink').addClass('text-success');
                $(`#fileName${fileNum}`).text(file.FileName);
                $(`#uploadedFile${fileNum}`).show();
            });
        }
        
        // 下載已上傳檔案
        function downloadUploadedFile(fileType) {
            const fileCode = fileType === 1 ? 'REIMBURSE_EXPENSE' : 'REIMBURSE_RECEIPT';
            window.location.href = `SciReimbursement.aspx?action=download&projectID=${encodeURIComponent(currentProjectID)}&fileCode=${encodeURIComponent(fileCode)}`;
        }

        // 格式化數字顯示
        function formatNumber(num) {
            if (num === null || num === undefined || num === 0) {
                return '0';
            }
            return Math.round(num).toLocaleString();
        }
    </script>
</asp:Content>