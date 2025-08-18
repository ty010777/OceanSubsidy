<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SciInterimReport.aspx.cs" Inherits="OFS_SCI_SciInterimReport" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/SCI/SciInprogress.master" EnableViewState="true" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />

    <!-- 內容區塊 -->
    <div class="block rounded-top-4">
        <ul class="teal-dark-tabs">
            <li class="active"><a class="tab-link" href="#" onclick="loadReportData(1)">期中報告</a></li>
            <li><a class="tab-link" href="#" onclick="loadReportData(2)">期末報告</a></li>
        </ul>
        <h5 class="square-title mt-4" id="reportTitle">期中報告審查</h5>
        <p class="text-pink mt-3 lh-base">請下載報告書範本，填寫資料及公文用印後上傳。<br>
            <span id="reportDescription">期中報告書</span>及相關檔案，請壓縮ZIP上傳（檔案100MB以內）。
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
                            <div id="reportName1">期中報告書_初版</div>
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
                            <input type="text" class="form-control" id="reportName2" value="期中報告書_修訂版">
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
 
     <div class="scroll-bottom-panel" id="reviewPanel" style="display: none;">
               <h5 class="text-pink fs-18 fw-bold mb-3">審查結果</h5>
               <ul class="d-flex flex-column gap-3 mb-3">
           
                    <li class="d-flex gap-2">
                       <span class="text-gray mt-2">審查方式 :</span>
                       <div class="d-flex flex-column gap-2 align-items-start flex-grow-1">
                           <div class="form-check-input-group d-flex text-nowrap mt-2 align-items-center">
                               <input id="book" class="form-check-input check-teal" type="radio" name="reviewType" value="書面審查"/>
                               <label for="book">書面審查</label>
                               <input id="meeting" class="form-check-input check-teal" type="radio" name="reviewType" value="會議審查"/>
                               <label for="meeting">會議審查</label>
                               
                               <input id="needmember" class="form-check-input check-teal" type="checkbox" onchange="toggleReviewerList()"/>
                               <label for="needmember">需審查委員</label>
                           </div>
                           
                       </div>
                   </li>
                   <li  id="reviewerListSection" style="display: none ;">
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
                   <li class="d-flex gap-2"> 
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
               <button type="button" class="btn btn-teal d-table mx-auto" onclick="submitReview()">確定</button>
     
          </div>
                                  

    <!-- 審查意見詳情模態視窗 -->
    <div class="modal fade" id="reportDetailModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="reportDetailModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-md">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="fs-24 text-green-light fw-bold">審查意見及回覆</h4>
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
        var currentProjectID = '<%= Request.QueryString["ProjectID"] %>';
        var currentReportType = 1; // 公共變數：代表目前是期中報告(1)還是期末報告(2)
        
        // 頁面載入時載入期中報告資料
        $(document).ready(function() {
            loadReportData(1);
            checkReviewPermissionAndStatus();
        });
        
        // 載入報告資料
        function loadReportData(reportType) {
            currentReportType = reportType; // 更新公共變數
            console.log('Loading report type:', reportType, 'for project:', currentProjectID);
            
            // 更新介面文字和樣式
            updateUIForReportType(reportType);
            
            // 載入已上傳檔案
            loadUploadedFiles();
            
            // 檢查審核權限和狀態
            checkReviewPermissionAndStatus();
        }
        
        // 更新介面元素根據報告類型
        function updateUIForReportType(reportType) {
            if (reportType === 1) {
                // 期中報告
                $('#reportTitle').text('期中報告審查');
                $('#reportDescription').text('期中報告書');
                $('#reportName1').text('期中報告書_初版');
                $('#reportName2').val('期中報告書_修訂版');
            } else {
                // 期末報告
                $('#reportTitle').text('期末報告審查');
                $('#reportDescription').text('期末報告書');
                $('#reportName1').text('期末報告書_初版');
                $('#reportName2').val('期末報告書_修訂版');
            }
            
            // 更新標籤狀態
            updateTabStatus(reportType);
        }
        
        // 更新標籤狀態
        function updateTabStatus(activeType) {
            $('.teal-dark-tabs li').removeClass('active');
            $('.teal-dark-tabs li').eq(activeType - 1).addClass('active');
        }
        
        // 下載範本
        function downloadTemplate() {
            window.location.href = `SciInterimReport.aspx?action=download&projectID=${encodeURIComponent(currentProjectID)}`;
        }
        
        // 處理檔案上傳
        function handleFileUpload(fileType, fileInput) {
            const file = fileInput.files[0];
            if (!file) {
                return;
            }
            
            // 驗證檔案類型
            const fileExt = '.' + file.name.split('.').pop().toLowerCase();
            if (fileExt !== '.zip') {
                Swal.fire('錯誤', '請上傳 ZIP 格式的檔案', 'error');
                fileInput.value = '';
                return;
            }
            
            // 檢查檔案大小 (100MB = 100 * 1024 * 1024 bytes)
            const maxSize = 100 * 1024 * 1024;
            if (file.size > maxSize) {
                Swal.fire('錯誤', '檔案大小不可超過 100MB', 'error');
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
            
            // 使用iframe方式上傳
            const iframe = document.createElement('iframe');
            iframe.style.display = 'none';
            iframe.name = 'uploadFrame';
            document.body.appendChild(iframe);
            
            const form = document.createElement('form');
            form.method = 'POST';
            form.enctype = 'multipart/form-data';
            form.target = 'uploadFrame';
            form.action = `SciInterimReport.aspx?action=upload&ProjectID=${encodeURIComponent(currentProjectID)}`;
            
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
            
            const reportTypeField = document.createElement('input');
            reportTypeField.type = 'hidden';
            reportTypeField.name = 'reportType';
            reportTypeField.value = currentReportType;
            form.appendChild(reportTypeField);
            
            // 如果是修訂版 (fileType == 2)，傳遞自訂名稱
            if (fileType == 2) {
                const customNameField = document.createElement('input');
                customNameField.type = 'hidden';
                customNameField.name = 'customName';
                customNameField.value = $('#reportName2').val();
                form.appendChild(customNameField);
            }
            
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
                url: 'SciInterimReport.aspx/GetUploadedFiles',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ 
                    projectID: currentProjectID,
                    reportType: currentReportType
                }),
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
            
            // 重置檔案名稱
            $('#fileName1').text('');
            $('#fileName2').text('');
            
            // 更新檔案狀態 - 後端已經根據 reportType 過濾了，直接處理
            files.forEach(function(file) {
                let fileNum = 0;
                
                // 根據 FileCode 決定檔案編號
                if (file.FileCode.endsWith('_revise')) {
                    fileNum = 2; // 修訂版
                } else {
                    fileNum = 1; // 初版
                }
                
                $(`#uploadStatus${fileNum}`).text('已上傳').removeClass('text-pink').addClass('text-success');
                $(`#fileName${fileNum}`).text(file.FileName);
                $(`#uploadedFile${fileNum}`).show();
            });
        }
        
        // 下載已上傳檔案
        function downloadUploadedFile(fileType) {
            let fileCode;
            if (currentReportType === 1) {
                fileCode = fileType === 1 ? 'MidExamFile' : 'MidExamFile_revise';
            } else {
                fileCode = fileType === 1 ? 'FinalExamFile' : 'FinalExamFile_revise';
            }
            window.location.href = `SciInterimReport.aspx?action=download&projectID=${encodeURIComponent(currentProjectID)}&fileCode=${encodeURIComponent(fileCode)}`;
        }
        
        // 提送報告 (isDraft: true=暫存, false=提送)
        function submitReport(isDraft = false) {
            let actionText = isDraft ? '暫存' : '提送';
            let reportTypeName = currentReportType === 1 ? '期中報告' : '期末報告';
            
            // 確認操作
            Swal.fire({
                title: `確定${actionText}${reportTypeName}？`,
                icon: 'question',
                showCancelButton: true,
                confirmButtonText: actionText,
                cancelButtonText: '取消'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: 'SciInterimReport.aspx/SubmitReport',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify({
                            projectID: currentProjectID,
                            stage: currentReportType,
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
        
        // 切換審查委員名單顯示
        function toggleReviewerList() {
            var needMemberCheckbox = document.getElementById('needmember');
            var reviewerListSection = document.getElementById('reviewerListSection');
                
            if (needMemberCheckbox.checked) {
                reviewerListSection.style.setProperty('display', 'flex');
                // 預設選擇逐筆輸入
                document.getElementById('radio-single').checked = true;
                document.getElementById('radio-batch').checked = false;
                // 觸發輸入模式切換，顯示逐筆輸入並添加預設一行
                toggleInputMode();
            } else {
                reviewerListSection.style.setProperty('display', 'none');
                // 清空所有資料
                clearAllReviewerRows();
                // 重置為批次輸入預設
                document.getElementById('radio-single').checked = false;
                document.getElementById('radio-batch').checked = true;
            }
        }
        
        // 切換輸入模式（逐筆輸入 vs 批次輸入）
        function toggleInputMode() {
            var singleInput = document.getElementById('radio-single');
            var batchInput = document.getElementById('radio-batch');
            var singleSection = document.getElementById('singleInputSection');
            var batchSection = document.getElementById('batchInputSection');
            var tbody = document.getElementById('reviewerTableBody');
            
            if (singleInput.checked) {
                // 顯示逐筆輸入，隱藏批次輸入
                singleSection.style.display = 'block';
                batchSection.style.display = 'none';
                // 如果沒有行，添加預設的一行
                if (tbody.rows.length === 0) {
                    addReviewerRow();
                }
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
        
        // 提送審查委員
        function submitReviewers() {
            // 1. 檢查輸入模式
            var singleInput = document.getElementById('radio-single');
            var batchInput = document.getElementById('radio-batch');
            
            if (!singleInput.checked && !batchInput.checked) {
                Swal.fire('錯誤', '請選擇輸入模式', 'error');
                return;
            }
            
            // 2. 收集審查委員資料
            var reviewers = [];
            var parseInfo = null; // 解析信息
            
            if (singleInput.checked) {
                // 逐筆輸入模式
                var result = collectSingleInputReviewersWithInfo();
                reviewers = result.reviewers;
                parseInfo = result.parseInfo;
            } else {
                // 批次輸入模式
                var result = collectBatchInputReviewersWithInfo();
                reviewers = result.reviewers;
                parseInfo = result.parseInfo;
            }
            
            // 3. 驗證資料
            if (reviewers.length === 0) {
                Swal.fire('錯誤', '請至少輸入一位審查委員', 'error');
                return;
            }
            
            // 4. 顯示解析結果並確認提送
            var confirmationHtml = buildConfirmationHtml(singleInput.checked, reviewers, parseInfo);
            
            Swal.fire({
                title: '確定提送審查委員？',
                html: confirmationHtml,
                icon: parseInfo && (parseInfo.skippedCount > 0) ? 'warning' : 'question',
                showCancelButton: true,
                confirmButtonText: '確定提送',
                cancelButtonText: '取消',
                width: '600px'
            }).then((result) => {
                if (result.isConfirmed) {
                    // 5. 呼叫後端API
                    $.ajax({
                        url: 'SciInterimReport.aspx/SubmitReviewers',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify({
                            projectID: currentProjectID,
                            stage: currentReportType,
                            reviewers: reviewers
                        }),
                        dataType: 'json',
                        success: function(response) {
                            if (response.d.Success) {
                                Swal.fire('成功', response.d.Message, 'success');
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
        
        // 建立確認提送的HTML內容
        function buildConfirmationHtml(isSingleInput, reviewers, parseInfo) {
            var inputMode = isSingleInput ? '逐筆輸入' : '批次輸入';
            var reviewerNames = reviewers.map(function(r) { return r.name; }).join('、');
            
            var html = `<div style="text-align: left;">`;
            
            // 基本信息
            html += `
                <div style="margin-bottom: 15px; padding: 12px; background: #e3f2fd; border-radius: 6px;">
                    <p style="margin: 0 0 8px 0;"><strong>📋 輸入模式：</strong>${inputMode}</p>
                    <p style="margin: 0 0 8px 0;"><strong>✅ 有效委員：</strong><span style="color: green; font-weight: bold;">${reviewers.length} 位</span></p>
                    <p style="margin: 0;"><strong>👥 委員名單：</strong>${reviewerNames}</p>
                </div>
            `;
            
            // 如果有解析信息（批次輸入或有重複的逐筆輸入）
            if (parseInfo && (parseInfo.skippedCount > 0 || parseInfo.totalLines)) {
                html += `<div style="margin-bottom: 15px; padding: 12px; background: #fff3e0; border-radius: 6px;">`;
                html += `<p style="margin: 0 0 8px 0; font-weight: bold; color: #f57500;">⚠️ 處理摘要：</p>`;
                
                if (parseInfo.totalLines) {
                    html += `<p style="margin: 0 0 4px 0;">📊 總計行數：${parseInfo.totalLines} 行</p>`;
                }
                if (parseInfo.skippedCount > 0) {
                    html += `<p style="margin: 0 0 8px 0;">🚫 跳過行數：<span style="color: orange;">${parseInfo.skippedCount} 行</span></p>`;
                }
                
                // 顯示跳過的詳細信息
                if (parseInfo.failedLines && parseInfo.failedLines.length > 0) {
                    html += `<p style="margin: 0 0 4px 0; font-weight: bold;">❌ 格式錯誤：</p>`;
                    html += `<div style="background: #ffebee; padding: 6px; border-radius: 4px; font-size: 11px; max-height: 80px; overflow-y: auto;">${parseInfo.failedLines.join('<br>')}</div>`;
                }
                
                if (parseInfo.duplicateEmails && parseInfo.duplicateEmails.length > 0) {
                    html += `<p style="margin: 8px 0 4px 0; font-weight: bold;">🔄 重複Email：</p>`;
                    html += `<div style="background: #fff3e0; padding: 6px; border-radius: 4px; font-size: 11px; max-height: 80px; overflow-y: auto;">${parseInfo.duplicateEmails.join('<br>')}</div>`;
                }
                
                html += `</div>`;
            }
            
            // 提示信息
            if (parseInfo && parseInfo.skippedCount > 0) {
                html += `
                    <div style="padding: 10px; background: #f5f5f5; border-radius: 4px; font-size: 12px;">
                        <strong>💡 提示：</strong>只有 <span style="color: green; font-weight: bold;">${reviewers.length} 位有效委員</span> 會被提送到資料庫，跳過的項目不會影響提送結果。
                    </div>
                `;
            }
            
            html += `</div>`;
            return html;
        }
        
        // 收集逐筆輸入的審查委員資料（帶解析信息）
        function collectSingleInputReviewersWithInfo() {
            var reviewers = [];
            var duplicateRows = [];
            var emailSet = new Set();
            var tbody = document.getElementById('reviewerTableBody');
            var rows = tbody.getElementsByTagName('tr');
            
            for (var i = 0; i < rows.length; i++) {
                var nameInput = rows[i].cells[1].querySelector('input');
                var emailInput = rows[i].cells[2].querySelector('input');
                
                var name = nameInput ? nameInput.value.trim() : '';
                var email = emailInput ? emailInput.value.trim() : '';
                
                if (name && email) {
                    var emailLower = email.toLowerCase();
                    if (emailSet.has(emailLower)) {
                        duplicateRows.push(`第${i+1}行: ${name} &lt;${email}&gt;`);
                        emailInput.style.borderColor = '#ff6b6b';
                        emailInput.title = '此Email已重複輸入';
                    } else {
                        emailSet.add(emailLower);
                        reviewers.push({
                            name: name,
                            email: email
                        });
                        emailInput.style.borderColor = '';
                        emailInput.title = '';
                    }
                }
            }
            
            var parseInfo = {
                skippedCount: duplicateRows.length,
                duplicateEmails: duplicateRows,
                failedLines: []
            };
            
            return {
                reviewers: reviewers,
                parseInfo: parseInfo
            };
        }
        
        // 收集批次輸入的審查委員資料（帶解析信息）
        function collectBatchInputReviewersWithInfo() {
            var reviewers = [];
            var failedLines = [];
            var duplicateEmails = [];
            var emailSet = new Set();
            var textarea = document.querySelector('#batchInputSection .textarea');
            
            if (!textarea) {
                return { reviewers: [], parseInfo: null };
            }
            
            var content = textarea.textContent || textarea.innerText || '';
            content = content.trim();
            
            if (!content || content === '輸入格式範例「姓名 <xxx@email.com>」，多筆請斷行輸入') {
                return { reviewers: [], parseInfo: null };
            }
            
            var lines = content.split('\n');
            var totalLines = 0;
            
            for (var i = 0; i < lines.length; i++) {
                var line = lines[i].trim();
                if (!line) continue;
                
                totalLines++;
                var parsed = parseBatchInputLine(line);
                
                if (parsed && parsed.name && parsed.email) {
                    var emailLower = parsed.email.toLowerCase();
                    if (emailSet.has(emailLower)) {
                        duplicateEmails.push(`第${i+1}行: ${parsed.name} &lt;${parsed.email}&gt;`);
                    } else {
                        emailSet.add(emailLower);
                        reviewers.push({
                            name: parsed.name,
                            email: parsed.email
                        });
                    }
                } else {
                    failedLines.push(`第${i+1}行: ${line}`);
                }
            }
            
            var parseInfo = {
                totalLines: totalLines,
                skippedCount: failedLines.length + duplicateEmails.length,
                failedLines: failedLines,
                duplicateEmails: duplicateEmails
            };
            
            return {
                reviewers: reviewers,
                parseInfo: parseInfo
            };
        }
        
        // 收集逐筆輸入的審查委員資料
        function collectSingleInputReviewers() {
            var reviewers = [];
            var duplicateRows = [];
            var emailSet = new Set();
            var tbody = document.getElementById('reviewerTableBody');
            var rows = tbody.getElementsByTagName('tr');
            
            for (var i = 0; i < rows.length; i++) {
                var nameInput = rows[i].cells[1].querySelector('input');
                var emailInput = rows[i].cells[2].querySelector('input');
                
                var name = nameInput ? nameInput.value.trim() : '';
                var email = emailInput ? emailInput.value.trim() : '';
                
                // 如果姓名或Email為空，跳過該行
                if (name && email) {
                    var emailLower = email.toLowerCase();
                    if (emailSet.has(emailLower)) {
                        // 標記重複的行
                        duplicateRows.push(`第${i+1}行: ${name} <${email}>`);
                        // 可以選擇高亮顯示重複的行
                        emailInput.style.borderColor = '#ff6b6b';
                        emailInput.title = '此Email已重複輸入';
                    } else {
                        emailSet.add(emailLower);
                        reviewers.push({
                            name: name,
                            email: email
                        });
                        // 清除之前可能的錯誤樣式
                        emailInput.style.borderColor = '';
                        emailInput.title = '';
                    }
                }
            }
            
            // 如果有重複，提醒用戶
            if (duplicateRows.length > 0) {
                Swal.fire({
                    title: '發現重複Email',
                    html: `<div style="text-align: left;">
                        <p>以下行有重複的Email，將被自動跳過：</p>
                        <pre style="background: #fff3e0; padding: 10px; border-radius: 4px; font-size: 12px;">${duplicateRows.join('\n')}</pre>
                        <p>有效委員：${reviewers.length} 位</p>
                    </div>`,
                    icon: 'warning',
                    confirmButtonText: '了解'
                });
            }
            
            return reviewers;
        }
        
        
        // 解析批次輸入的單行格式
        function parseBatchInputLine(line) {
            // 移除前後空白
            line = line.trim();
            
            // 正則表達式匹配格式：姓名 <email@domain.com>
            // 支援多種可能的格式變化
            var patterns = [
                /^(.+?)\s*<(.+?)>$/,           // 姓名 <email>
                /^(.+?)\s+<(.+?)>$/,          // 姓名 <email> (確保有空格)
                /^(.+?)\s*\<(.+?)\>$/,        // 姓名 <email> (轉義版本)
                /^(.+?)\s*【(.+?)】$/,         // 姓名 【email】(中文括號)
                /^(.+?)\s*（(.+?)）$/,         // 姓名 （email）(中文括號)
                /^(.+?)\s*\((.+?)\)$/         // 姓名 (email) (英文括號)
            ];
            
            for (var i = 0; i < patterns.length; i++) {
                var match = line.match(patterns[i]);
                if (match) {
                    var name = match[1].trim();
                    var email = match[2].trim();
                    
                    // 驗證email格式
                    if (name && email && isValidEmail(email)) {
                        return {
                            name: name,
                            email: email
                        };
                    }
                }
            }
            
            // 如果無法解析，記錄錯誤但不中斷處理
            console.warn('無法解析行：' + line);
            return null;
        }
        
        // 簡單的email格式驗證
        function isValidEmail(email) {
            var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            return emailRegex.test(email);
        }
        
        // 檢查審核權限和狀態
        function checkReviewPermissionAndStatus() {
            // 檢查使用者權限
            $.ajax({
                url: 'SciInterimReport.aspx/CheckReviewPermission',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function(response) {
                    if (response.d && response.d.Success) {
                        if (response.d.HasPermission) {
                            // 有權限，檢查當前階段狀態
                            checkStageStatus();
                        } else {
                            // 沒有權限，隱藏審核面板
                            $('#reviewPanel').hide();
                        }
                    }
                },
                error: function() {
                    console.error('檢查審核權限失敗');
                    $('#reviewPanel').hide();
                }
            });
        }
        
        // 檢查階段狀態
        function checkStageStatus() {
            $.ajax({
                url: 'SciInterimReport.aspx/GetStageExamStatus',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({
                    projectID: currentProjectID,
                    stage: currentReportType
                }),
                dataType: 'json',
                success: function(response) {
                    if (response.d && response.d.Success) {
                        var status = response.d.Data.Status;
                        // 只有狀態為「審核中」時才顯示審核面板
                        if (status === '審核中') {
                            $('#reviewPanel').show();
                        } else {
                            $('#reviewPanel').hide();
                        }
                    }
                },
                error: function() {
                    console.error('檢查階段狀態失敗');
                    $('#reviewPanel').hide();
                }
            });
        }
        
        // 提交審核
        function submitReview() {
            // 取得審查方式
            var reviewMethod = $('input[name="reviewType"]:checked').val();
            if (!reviewMethod) {
                Swal.fire('錯誤', '請選擇審查方式', 'error');
                return;
            }
            
            // 取得審查結果
            var reviewResult = $('input[name="reviewResult"]:checked').val();
            if (!reviewResult) {
                Swal.fire('錯誤', '請選擇審查結果', 'error');
                return;
            }
            
            // 取得審查意見
            var reviewComment = $('#reviewComment').text().trim();
            if (!reviewComment || reviewComment === '請輸入原因') {
                if (reviewResult === 'reject') {
                    Swal.fire('錯誤', '不通過時請輸入原因', 'error');
                    return;
                }
                reviewComment = '';
            }
            
            var reportTypeName = currentReportType === 1 ? '期中報告' : '期末報告';
            var resultText = reviewResult === 'pass' ? '通過' : '不通過';
            
            // 確認提交
            Swal.fire({
                title: `確定提交${reportTypeName}審核結果？`,
                text: `審查方式：${reviewMethod}，審查結果：${resultText}`,
                icon: 'question',
                showCancelButton: true,
                confirmButtonText: '確定',
                cancelButtonText: '取消'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: 'SciInterimReport.aspx/ReviewStageExam',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify({
                            projectID: currentProjectID,
                            stage: currentReportType,
                            reviewMethod: reviewMethod,
                            reviewResult: reviewResult,
                            reviewComment: reviewComment
                        }),
                        dataType: 'json',
                        success: function(response) {
                            if (response.d.Success) {
                                Swal.fire('成功', response.d.Message, 'success').then(() => {
                                    location.reload();
                                });
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
    </script>
</asp:Content>