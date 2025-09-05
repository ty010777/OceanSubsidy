<%@ Page Title="" Language="C#" MasterPageFile="~/OFS/CLB/ClbInprogress.master" AutoEventWireup="true" CodeFile="ClbPayment.aspx.cs" Inherits="OFS_CLB_ClbPayment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadExtra" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    
    <!-- 內容區塊 -->
    <div class="block rounded-top-4">
        <!-- 檔案上傳區塊 -->
        <div id="fileUploadSection">
            <h5 class="square-title mt-4">檔案上傳</h5>
            <p class="text-pink mt-3 lh-base">請下載附件範本，填寫完畢後上傳相關檔案。<br>
                所有檔案請上傳PDF格式。
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
                                <div><span class="text-pink">*</span>收支明細表</div>
                                <button class="btn btn-sm btn-teal-dark rounded-pill mt-2" type="button" onclick="downloadTemplate(1)">
                                    <i class="fas fa-file-download me-1"></i>
                                    範本下載
                                </button>
                            </td>
                            <td class="text-center">
                                <span id="uploadStatus1" class="text-success">已上傳</span>
                            </td>
                            <td>
                                <div class="d-flex align-items-center gap-2">
                                    <input type="file" id="fileInput1" accept=".pdf" style="display: none;" onchange="handleFileUpload(1, this)">
                                    <button class="btn btn-sm btn-teal-dark" type="button" onclick="document.getElementById('fileInput1').click()">
                                        <i class="fas fa-file-upload me-1"></i>
                                        上傳
                                    </button>
                                    <div id="uploadedFile1" style="display: flex;" class="d-flex align-items-center gap-2">
                                        <a href="#" id="downloadLink1" class="btn btn-sm btn-outline-teal" onclick="downloadUploadedFile(1)">
                                            <i class="fas fa-download me-1"></i>
                                            <span id="fileName1">CLB1140009_收支明細表.pdf</span>
                                        </a>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="text-center">2</td>
                            <td>
                                <div><span class="text-pink">*</span>受補助清單</div>
                                <button class="btn btn-sm btn-teal-dark rounded-pill mt-2" type="button" onclick="downloadTemplate(2)">
                                    <i class="fas fa-file-download me-1"></i>
                                    範本下載
                                </button>
                            </td>
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
                                    <div id="uploadedFile2" style="display: none;" class="d-flex align-items-center gap-2">
                                        <a href="#" id="downloadLink2" class="btn btn-sm btn-outline-teal" onclick="downloadUploadedFile(2)">
                                            <i class="fas fa-download me-1"></i>
                                            <span id="fileName2"></span>
                                        </a>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="text-center">3</td>
                            <td>
                                <div><span class="text-pink">*</span>經費分攤表</div>
                                <button class="btn btn-sm btn-teal-dark rounded-pill mt-2" type="button" onclick="downloadTemplate(3)">
                                    <i class="fas fa-file-download me-1"></i>
                                    範本下載
                                </button>
                            </td>
                            <td class="text-center">
                                <span id="uploadStatus3" class="text-success">已上傳</span>
                            </td>
                            <td>
                                <div class="d-flex align-items-center gap-2">
                                    <input type="file" id="fileInput3" accept=".pdf" style="display: none;" onchange="handleFileUpload(3, this)">
                                    <button class="btn btn-sm btn-teal-dark" type="button" onclick="document.getElementById('fileInput3').click()">
                                        <i class="fas fa-file-upload me-1"></i>
                                        上傳
                                    </button>
                                    <div id="uploadedFile3" style="display: flex;" class="d-flex align-items-center gap-2">
                                        <a href="#" id="downloadLink3" class="btn btn-sm btn-outline-teal" onclick="downloadUploadedFile(3)">
                                            <i class="fas fa-download me-1"></i>
                                            <span id="fileName3">CLB1140009_經費分攤表.pdf</span>
                                        </a>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr> 
                            <td class="text-center">4</td>
                            <td><span class="text-pink">*</span>憑證</td>
                            <td class="text-center">
                                <span id="uploadStatus4" class="text-pink">尚未上傳</span>
                            </td>
                            <td>
                                <div class="d-flex align-items-center gap-2">
                                    <input type="file" id="fileInput4" accept=".pdf" style="display: none;" onchange="handleFileUpload(4, this)">
                                    <button class="btn btn-sm btn-teal-dark" type="button" onclick="document.getElementById('fileInput4').click()">
                                        <i class="fas fa-file-upload me-1"></i>
                                        上傳
                                    </button>
                                    <div id="uploadedFile4" style="display: none;" class="d-flex align-items-center gap-2">
                                        <a href="#" id="downloadLink4" class="btn btn-sm btn-outline-teal" onclick="downloadUploadedFile(4)">
                                            <i class="fas fa-download me-1"></i>
                                            <span id="fileName4"></span>
                                        </a>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr> 
                            <td class="text-center">5</td>
                            <td><span class="text-pink">*</span>領據（含帳戶資料）</td>
                            <td class="text-center">
                                <span id="uploadStatus5" class="text-pink">尚未上傳</span>
                            </td>
                            <td>
                                <div class="d-flex align-items-center gap-2">
                                    <input type="file" id="fileInput5" accept=".pdf" style="display: none;" onchange="handleFileUpload(5, this)">
                                    <button class="btn btn-sm btn-teal-dark" type="button" onclick="document.getElementById('fileInput5').click()">
                                        <i class="fas fa-file-upload me-1"></i>
                                        上傳
                                    </button>
                                    <div id="uploadedFile5" style="display: none;" class="d-flex align-items-center gap-2">
                                        <a href="#" id="downloadLink5" class="btn btn-sm btn-outline-teal" onclick="downloadUploadedFile(5)">
                                            <i class="fas fa-download me-1"></i>
                                            <span id="fileName5"></span>
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
            <p class="text-teal fw-bold mb-0">核定經費: <span id="approvedSubsidy">500,000</span> 元</p>
            <p class="text-orange fw-bold mb-0" id="remainingAmountSection">賸餘款: <span id="remainingAmount">150,000</span> 元</p>
        </div>
        <!-- 隱藏欄位：累計撥付金額 -->
        <input type="hidden" id="totalActualPaidAmount" value="200000">
        <div class="table-responsive">
            <table class="table align-middle gray-table">
                <thead>
                    <tr>
                        <th width="120" class="text-center">本期請款金額</th>
                        <th width="120" class="text-center">前期已撥付金額</th>
                        <th width="120" class="text-center">累積實支金額</th>
                        <th width="120" class="text-center">累積經費執行率</th>
                        <th width="120" class="text-center">支用比</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td class="text-center" id="currentAmount">--</td>
                        <td class="text-center" id="previousAmount">200,000</td>
                        <td class="text-center" id="accumulatedAmountCell">
                            <input type="number" id="accumulatedAmountInput" class="form-control text-center" placeholder="請輸入累積實支金額" min="0" step="1" style="width: 200px; margin: 0 auto;" value="350000">
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
                            <th width="200" class="text-center">本期實際撥款</th>
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
    <div class="scroll-bottom-panel" id="reviewPanel" style="display: none;">
        <h5 class="text-pink fs-18 fw-bold mb-3">審查結果</h5>

        <ul class="d-flex flex-column gap-3 mb-3">
            <li class="d-flex gap-4 align-items-center">
                <!-- 本期 -->
                <div class="d-flex gap-2">
                    <span class="text-gray">本期撥款 :</span>
                    <input type="number" id="currentPayment" 
                           class="form-control text-center text-teal fw-bold" 
                           value="0" min="0" step="1" style="width: 120px;">
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
        <button type="button" class="btn btn-teal d-table mx-auto" id="confirmReviewBtn" onclick="submitReview()">確定撥款</button>
    </div>

    <script>
        var currentProjectID = 'CLB001'; // 模擬計畫ID
        
        // 頁面載入時初始化
        document.addEventListener('DOMContentLoaded', function() {
            // 計算第二期數值
            calculatePhase2Values();
            
            // 綁定累積實支金額輸入框的變化事件
            document.getElementById('accumulatedAmountInput').addEventListener('input', function() {
                calculatePhase2Values();
                updateRemainingAmount();
            });
            
            // 顯示審查面板（模擬審核中狀態）
            document.getElementById('reviewPanel').style.display = 'block';
        });
        
        // 第二期動態計算
        function calculatePhase2Values() {
            var accumulated = parseFloat(document.getElementById('accumulatedAmountInput').value) || 0;
            var previous = parseFloat(document.getElementById('previousAmount').textContent.replace(/,/g, '')) || 0;
            var approved = parseFloat(document.getElementById('approvedSubsidy').textContent.replace(/,/g, '')) || 0;
            var totalPaid = parseFloat(document.getElementById('totalActualPaidAmount').value) || 0;
            
            // 本期請款金額 = 累積實支 - 前期撥付
            document.getElementById('currentAmount').textContent = formatNumber(Math.max(0, accumulated - previous));
            
            // 執行率 = 累積實支 ÷ 核定經費 × 100%
            var rate = approved > 0 ? (accumulated / approved * 100) : 0;
            document.getElementById('executionRate').textContent = rate.toFixed(2) + '%';
            
            // 支用比 = 累積實支 ÷ 累計撥付 × 100% (最大100%)
            var usage = totalPaid > 0 ? Math.min((accumulated / totalPaid * 100), 100) : 0;
            document.getElementById('usageRatio').textContent = usage.toFixed(2) + '%';
            
            // 同步更新審查面板的本期撥款
            document.getElementById('currentPayment').value = Math.max(0, accumulated - previous);
        }
        
        // 更新賸餘款顯示
        function updateRemainingAmount() {
            var approvedSubsidy = parseFloat(document.getElementById('approvedSubsidy').textContent.replace(/,/g, '')) || 0;
            var accumulatedAmount = parseFloat(document.getElementById('accumulatedAmountInput').value) || 0;
            var remainingAmount = approvedSubsidy - accumulatedAmount;
            
            document.getElementById('remainingAmount').textContent = formatNumber(remainingAmount);
        }
        
        // 下載範本 (靜態功能)
        function downloadTemplate(fileType) {
            var templateNames = {
                1: '收支明細表',
                2: '受補助清單',
                3: '經費分攤表',
                4: '憑證',
                5: '領據（含帳戶資料）'
            };
            alert(`下載${templateNames[fileType]}範本功能 (靜態展示)`);
        }

        // 處理檔案上傳 (靜態功能)
        function handleFileUpload(fileType, fileInput) {
            const file = fileInput.files[0];
            if (!file) {
                return;
            }
            
            // 驗證檔案類型
            const fileExt = '.' + file.name.split('.').pop().toLowerCase();
            if (fileExt !== '.pdf') {
                alert('請上傳 PDF 格式的檔案');
                fileInput.value = '';
                return;
            }
            
            // 模擬上傳成功
            document.getElementById('uploadStatus' + fileType).textContent = '已上傳';
            document.getElementById('uploadStatus' + fileType).className = 'text-success';
            document.getElementById('fileName' + fileType).textContent = file.name;
            document.getElementById('uploadedFile' + fileType).style.display = 'flex';
            
            alert('檔案上傳成功 (靜態展示)');
        }
        
        // 下載已上傳檔案 (靜態功能)
        function downloadUploadedFile(fileType) {
            alert('下載已上傳檔案功能 (靜態展示)');
        }
        
        // 提送請款（isDraft: true=暫存, false/undefined=提送）
        function submitReimbursement(isDraft = false) {
            var currentAmount = parseFloat(document.getElementById('currentAmount').textContent.replace(/,/g, '')) || 0;
            var accumulatedAmount = parseFloat(document.getElementById('accumulatedAmountInput').value) || 0;
            
            // 驗證
            if (accumulatedAmount <= 0) {
                alert('請輸入累積實支金額');
                return;
            }
            if (!isDraft && currentAmount <= 0) {
                alert('本期請款金額不可為0');
                return;
            }
            
            // 請款金額限制檢查
            var approvedSubsidy = parseFloat(document.getElementById('approvedSubsidy').textContent.replace(/,/g, '')) || 0;
            var maxAllowedAmount = approvedSubsidy * 0.6; // 核定金額的60%
            
            if (currentAmount > maxAllowedAmount) {
                alert(`請款金額不可超過核定金額的60% (${formatNumber(maxAllowedAmount)} 元)`);
                return;
            }
            
            // 確認操作
            let actionText = isDraft ? '暫存' : '提送';
            if (confirm(`確定${actionText}請款？\n本期請款: ${formatNumber(currentAmount)} 元`)) {
                alert(`${actionText}請款成功 (靜態展示)`);
                
                // 模擬提送後顯示審查面板
                if (!isDraft) {
                    document.getElementById('reviewPanel').style.display = 'block';
                    document.querySelector('.block-bottom').style.display = 'none';
                }
            }
        }

        // 提交審核 (靜態功能)
        function submitReview() {
            let currentPayment = parseFloat(document.getElementById('currentPayment').value) || 0;
            let reviewResult = document.getElementById('radio-pass').checked ? 'pass' : 'return';
            let reviewComment = document.getElementById('reviewComment').textContent.trim();
            
            var resultText = reviewResult === 'pass' ? '通過' : '退回修改';
            
            if (confirm(`確定提交審核結果？\n本期撥款: ${formatNumber(currentPayment)} 元\n審查結果: ${resultText}`)) {
                alert('審核結果提交成功 (靜態展示)');
                
                // 模擬通過後顯示實際撥款統計
                if (reviewResult === 'pass') {
                    document.getElementById('actualPaymentSection').style.display = 'block';
                    document.getElementById('currentActualPayment').textContent = formatNumber(currentPayment);
                    document.getElementById('cumulativeActualPayment').textContent = formatNumber(currentPayment + 200000);
                }
            }
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