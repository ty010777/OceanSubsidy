<%@ Page Title="" Language="C#" MasterPageFile="~/OFS/CLB/ClbInprogress.master" AutoEventWireup="true" CodeFile="ClbStageReport.aspx.cs" Inherits="OFS_CLB_ClbStageReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadExtra" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    
    <!-- 內容區塊 -->
    <div class="block rounded-top-4">
        <h5 class="square-title mt-4">成果報告審查</h5>
        <p class="text-pink mt-3 lh-base">請下載報告書範本，填寫資料及公文用印後上傳。<br>
            成果報告書及相關檔案，請壓縮ZIP上傳（檔案100MB以內）。
        </p>
        
        <div class="table-responsive mt-3">
            <table class="table align-middle gray-table">
                <thead>
                    <tr>
                        <th width="70" class="text-center">附件編號</th>
                        <th>附件名稱</th>
                        <th class="text-center">狀態</th>
                        <th>上傳附件</th>
                        <th width="100">審查意見</th>
                    </tr>
                </thead>
                <tbody>
                    <!-- 已送初版 -->   
                    <tr>
                        <td class="text-center">1</td>
                        <td>
                            <div>成果報告書_初版</div>
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
                                <input type="file" id="fileInput1" accept=".zip" style="display: none;" onchange="handleFileUpload(1, this)">
                                <button class="btn btn-sm btn-teal-dark" type="button" onclick="document.getElementById('fileInput1').click()">
                                    <i class="fas fa-file-upload me-1"></i>
                                    上傳
                                </button>
                                <div id="uploadedFile1" style="display: none;" class="d-flex align-items-center gap-2">
                                    <a href="#" id="downloadLink1" class="btn btn-sm btn-outline-teal" onclick="downloadUploadedFile(1)">
                                        <i class="fas fa-download me-1"></i>
                                        <span id="fileName1"></span>
                                    </a>
                                </div>
                            </div>
                        </td>
                        <td>
                            <button class="btn btn-sm btn-teal-dark mx-auto" type="button" data-bs-toggle="modal" data-bs-target="#reportDetailModal">
                                <i class="fas fa-file-alt" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="詳情"></i>
                            </button>
                        </td>
                    </tr>
                    
                    <!-- 修正版 -->
                    <tr> 
                        <td class="text-center">2</td>
                        <td>
                            <input type="text" class="form-control" id="reportName2" value="成果報告書_修訂版">
                        </td>
                        <td class="text-center">
                            <span id="uploadStatus2" class="text-pink">尚未上傳</span>
                        </td>
                        <td>
                            <div class="d-flex align-items-center gap-2">
                                <input type="file" id="fileInput2" accept=".zip" style="display: none;" onchange="handleFileUpload(2, this)">
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
                        <td>
                            <button class="btn btn-sm btn-teal-dark mx-auto" type="button" disabled>
                                <i class="fas fa-file-alt" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="詳情"></i>
                            </button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    
    <!-- 底部區塊 -->
    <div class="block-bottom bg-light-teal">
        <button type="button" class="btn btn-outline-teal" onclick="submitReport(true)">
            暫存
        </button>
        <button type="button" class="btn btn-teal" onclick="submitReport(false)">
            <i class="fas fa-check"></i>
            提送
        </button>
    </div>

    <!-- 審查結果區塊 -->
    <div class="scroll-bottom-panel" id="reviewPanel">
        <h5 class="text-pink fs-18 fw-bold mb-3">審查結果</h5>
        <ul class="d-flex flex-column gap-3 mb-3">
            
            <li id="reviewerListSection" style="display: none;">
                <span class="text-gray mt-2">審查委員名單 :</span>
                <div class="d-flex flex-column gap-2 align-items-start flex-grow-1 | checkinput">
                    <div class="form-check-input-group d-flex text-nowrap mt-2 align-items-center">
                        <input id="radio-single" class="form-check-input check-teal" type="radio" name="inputType" value="single" onchange="toggleInputMode()">
                        <label for="radio-single">逐筆輸入</label>
                        <input id="radio-batch" class="form-check-input check-teal" type="radio" name="inputType" value="batch" checked="" onchange="toggleInputMode()">
                        <label for="radio-batch">批次輸入</label>
                    </div>
    
                    <!-- 逐筆輸入 -->
                    <div class="w-100 fromOption1" id="singleInputSection" style="display: none;">
                        <div class="table-responsive" style="max-height: 270px;">
                            <table class="table align-middle gray-table">
                                <thead>
                                    <tr>
                                        <th>項次</th>
                                        <th>姓名</th>
                                        <th>Email</th>
                                        <th>功能</th>
                                    </tr>
                                </thead>
                                <tbody id="reviewerTableBody">
                                    <!-- 動態生成的tr會在這裡 -->
                                </tbody>
                            </table>
                        </div>
                    </div>
    
                    <!-- 批次輸入 -->
                    <div class="w-100 fromOption2" id="batchInputSection">
                        <span class="form-control textarea rounded-0" role="textbox" contenteditable="" data-placeholder="輸入格式範例「姓名 <xxx@email.com>」，多筆請斷行輸入" aria-label="文本輸入區域"></span>
                    </div>
                    <button class="btn btn-teal-dark rounded-0" type="button" onclick="submitReviewers()">提送審查委員</button>

                </div>
            </li>
            <li class="d-flex gap-2" id="reviewResultSection"> 
                <span class="text-gray mt-2">審查結果 :</span>
                <div class="d-flex flex-column gap-2 align-items-start flex-grow-1 checkPass">
                    <div class="form-check-input-group d-flex text-nowrap mt-2 align-items-center">
                        <input id="radioPass" class="form-check-input check-teal radioPass" type="radio" name="reviewResult" value="pass"/>
                        <label for="radioPass">通過</label>
                        <input id="radioReturn" class="form-check-input check-teal radioReturn" type="radio" name="reviewResult" value="reject"/>
                        <label for="radioReturn">不通過</label>
                    </div>
                    <span class="form-control textarea w-100" role="textbox" contenteditable="" data-placeholder="請輸入原因" aria-label="文本輸入區域" id="reviewComment"></span>
                </div>
            </li>
        </ul>
        <button type="button" class="btn btn-teal d-table mx-auto" id="submitReviewButton" onclick="submitReview()">確定</button>

    </div>

    <!-- 審查意見詳情模態視窗 -->
    <div class="modal fade" id="reportDetailModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="reportDetailModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-md">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="fs-24 text-green-light fw-bold">審查意見</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </div>
                <div class="modal-body">
                    <p class="text-pink lh-base py-3">請將委員審查意見整理放入修正版報告書並進行回覆</p>
                    <div class="table-responsive">
                        <table class="table align-middle gray-table lh-base">
                            <thead>
                                <tr>
                                    <th width="150">審查委員</th>
                                    <th>評分</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>委員Ａ</td>
                                    <td>
                                        <a href="#" class="link-black">審查意見.pdf</a>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>
        // 下載範本 (靜態功能)
        function downloadTemplate() {
            alert('下載範本功能 (靜態展示)');
        }
        
        // 處理檔案上傳 (靜態功能)
        function handleFileUpload(fileType, fileInput) {
            const file = fileInput.files[0];
            if (!file) {
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
        
        // 提送報告 (靜態功能)
        function submitReport(isDraft = false) {
            let actionText = isDraft ? '暫存' : '提送';
            if (confirm(`確定${actionText}成果報告？`)) {
                alert(`${actionText}成功 (靜態展示)`);
            }
        }
        
        // 切換審查委員名單顯示
        function toggleReviewerList() {
            var needMemberCheckbox = document.getElementById('needmember');
            var reviewerListSection = document.getElementById('reviewerListSection');
            var reviewResultSection = document.getElementById('reviewResultSection');
            var submitReviewButton = document.getElementById('submitReviewButton');
                
            if (needMemberCheckbox.checked) {
                // 顯示審查委員名單
                reviewerListSection.style.display = 'block';
                // 隱藏審查結果
                reviewResultSection.style.display = 'none';
                // 隱藏確定按鈕
                if (submitReviewButton) {
                    submitReviewButton.style.display = 'none';
                }
                // 預設選擇批次輸入
                document.getElementById('radio-batch').checked = true;
                toggleInputMode();
            } else {
                // 隱藏審查委員名單
                reviewerListSection.style.display = 'none';
                // 顯示審查結果
                reviewResultSection.style.display = 'flex';
                // 顯示確定按鈕
                if (submitReviewButton) {
                    submitReviewButton.style.display = 'block';
                }
            }
        }
        
        // 切換輸入模式（逐筆輸入 vs 批次輸入）
        function toggleInputMode() {
            var singleInput = document.getElementById('radio-single');
            var batchInput = document.getElementById('radio-batch');
            var singleSection = document.getElementById('singleInputSection');
            var batchSection = document.getElementById('batchInputSection');
            
            if (singleInput.checked) {
                // 顯示逐筆輸入，隱藏批次輸入
                singleSection.style.display = 'block';
                batchSection.style.display = 'none';
                // 添加預設的一行
                addReviewerRow();
            } else if (batchInput.checked) {
                // 顯示批次輸入，隱藏逐筆輸入
                singleSection.style.display = 'none';
                batchSection.style.display = 'block';
                // 清空逐筆輸入的所有行
                clearAllReviewerRows();
            }
        }
        
        // 添加審查委員行
        function addReviewerRow() {
            var tbody = document.getElementById('reviewerTableBody');
            var rowCount = tbody.rows.length + 1;
            
            var newRow = document.createElement('tr');
            newRow.innerHTML = `
                <td>${rowCount}</td>
                <td><input type="text" class="form-control" placeholder="姓名"></td>
                <td><input type="text" class="form-control" placeholder="Email"></td>
                <td>    
                    <div class="d-flex gap-2">
                        <button type="button" class="btn btn-sm btn-teal" onclick="removeReviewerRow(this)">
                            <i class="fas fa-trash-alt"></i>
                        </button>
                        <button type="button" class="btn btn-sm btn-teal" onclick="addReviewerRow()">
                            <i class="fas fa-plus"></i>
                        </button>
                    </div>
                </td>
            `;
            
            tbody.appendChild(newRow);
            updateRowNumbers();
        }
        
        // 刪除審查委員行
        function removeReviewerRow(button) {
            var tbody = document.getElementById('reviewerTableBody');
            // 至少保留一行
            if (tbody.rows.length > 1) {
                var row = button.closest('tr');
                row.remove();
                updateRowNumbers();
            }
        }
        
        // 更新行號
        function updateRowNumbers() {
            var tbody = document.getElementById('reviewerTableBody');
            var rows = tbody.getElementsByTagName('tr');
            
            for (var i = 0; i < rows.length; i++) {
                rows[i].cells[0].textContent = i + 1;
            }
        }
        
        // 清空所有審查委員行
        function clearAllReviewerRows() {
            var tbody = document.getElementById('reviewerTableBody');
            tbody.innerHTML = '';
        }
        
        // 提送審查委員 (靜態功能)
        function submitReviewers() {
            alert('提送審查委員功能 (靜態展示)');
        }
        
        // 提交審核 (靜態功能)
        function submitReview() {
            // 取得審查方式
            var reviewMethod = document.querySelector('input[name="reviewType"]:checked')?.value;
            if (!reviewMethod) {
                alert('請選擇審查方式');
                return;
            }
            
            // 取得審查結果
            var reviewResult = document.querySelector('input[name="reviewResult"]:checked')?.value;
            if (!reviewResult) {
                alert('請選擇審查結果');
                return;
            }
            
            var resultText = reviewResult === 'pass' ? '通過' : '不通過';
            
            if (confirm(`確定提交審核結果？\n審查方式：${reviewMethod}\n審查結果：${resultText}`)) {
                alert('審核結果提交成功 (靜態展示)');
            }
        }
        
        // 初始化頁面
        document.addEventListener('DOMContentLoaded', function() {
            // 預設隱藏審查委員名單，顯示審查結果
            document.getElementById('reviewerListSection').style.display = 'none';
            document.getElementById('reviewResultSection').style.display = 'flex';
        });
    </script>
    
</asp:Content>