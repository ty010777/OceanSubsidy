<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SciExamReview.aspx.cs" Inherits="OFS_SCI_SciExamReview" Culture="zh-TW" UICulture="zh-TW" %>

<!doctype html>
<html class="no-js" lang="zh-Hant">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="x-ua-compatible" content="ie=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>期中末審查 | 海洋科學調查活動填報系統</title>
    
    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="<%= ResolveUrl("~/assets/vendor/bootstrap-5.3.3/dist/css/bootstrap.min.css") %>">
    
    <!-- FontAwesome -->
    <link rel="stylesheet" href="<%= ResolveUrl("~/assets/vendor/fontawesome-free-6.5.2-web/css/all.min.css") %>">
    <link rel="stylesheet" href="<%= ResolveUrl("~/assets/vendor/fontawesome-free-6.5.2-web/css/all.css") %>">
        
    <!-- Google Icons -->
    <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Symbols+Rounded">
    <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Icons+Round">
    <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" />
    
    <!-- Google Fonts -->
    <link href="https://fonts.googleapis.com/css2?family=Roboto:ital,wght@0,100..900;1,100..900&display=swap" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Inter:ital,opsz,wght@0,14..32,100..900;1,14..32,100..900&display=swap" rel="stylesheet">
    
    <!-- 自訂 CSS -->
    <link rel="stylesheet" href="<%= ResolveUrl("~/assets/css/login.css") %>">
    <link rel="stylesheet" href="<%= ResolveUrl("~/assets/css/main.css") %>">
</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
        
        <main>
            <div class="mis-layout" style="grid-template-columns: 1fr;">
                <!-- 主要內容 -->
                <div class="mis-content">
                    <div class="mis-container">  
                        <div class="close-menu-logo d-block">
                            <div class="d-flex align-items-center flex-wrap">
                                <img class="img-fluid" src="<%= ResolveUrl("~/assets/img/ocean-logo.png") %>" alt="logo" style="width: 180px;"> 
                                <h2 class="text-dark-green">海洋領域補助計畫管理資訊系統</h2>
                            </div>
                        </div>

                        <div class="block rounded-4">
                            <!-- 上方資訊區 -->
                            <h5 class="square-title mb-3"> 計畫資料</h5>

                            <div class="bg-light-gray p-4 mb-5">
                                <ul class="d-flex flex-column gap-3">
                                    <li>
                                        <span class="text-gray">年度 :</span>
                                        <asp:Label ID="lblYear" runat="server" />
                                    </li>
                                    <li>
                                        <span class="text-gray">計畫編號 :</span>
                                        <asp:Label ID="lblProjectNumber" runat="server" />
                                    </li>
                                    <li>
                                        <span class="text-gray">計畫類別 :</span>
                                        <asp:Label ID="lblProjectCategory" runat="server" />
                                    </li>
                                    <li>
                                        <span class="text-gray">領域 :</span>
                                        <asp:Label ID="lblReviewGroup" runat="server" />
                                    </li>
                                    <li>
                                        <span class="text-gray">計畫名稱 :</span>
                                        <asp:Label ID="lblProjectName" runat="server" />
                                    </li>
                                    <li>
                                        <span class="text-gray">執行單位 :</span>
                                        <asp:Label ID="lblApplicantUnit" runat="server" />
                                    </li>
                                   
                                </ul>
                            </div>

                            <!-- 報告檔案清單 -->
                            <div id="reportSection">
                                <h5 class="square-title mb-3" id="reportTitle">報告審查</h5>
                                <p class="text-pink mt-3 lh-base">
                                    請下載報告檔案進行審查。
                                </p>
                                
                                <!-- 檔案清單表格 -->
                                <div class="table-responsive mt-3">
                                    <table class="table align-middle gray-table">
                                        <thead>
                                            <tr>
                                                <th width="70" class="text-center">附件編號</th>
                                                <th>附件名稱</th>
                                                <th class="text-center">檔案類型</th>
                                                <th>操作</th>
                                            </tr>
                                        </thead>
                                        <tbody id="reportFilesTable">
                                            <!-- 動態載入檔案清單 -->
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                            
                            <!-- 審查委員資訊區 -->
                            <div id="reviewerInfoSection" class="mt-5">
                                <h5 class="square-title mb-3">審查委員資訊</h5>

                                <div class="table-responsive mt-3">
                                    <table class="table align-middle gray-table">
                                        <tbody>
                                            <tr>
                                                <th width="200" class="bg-light">審查委員</th>
                                                <td>
                                                    <span id="reviewerDisplayName">委員姓名</span>
                                                    <span id="reviewerAccount" class="text-muted ms-2"></span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="bg-light">銀行金融代號、銀行帳號</th>
                                                <td>
                                                    <div class="d-flex align-items-center gap-3">
                                                        <select id="bankCodeSelect" class="form-select" style="max-width: 400px;">
                                                            <option value="">請選擇銀行</option>
                                                        </select>
                                                        <input type="text" id="bankAccountInput" class="form-control" style="max-width: 300px;" placeholder="請輸入銀行帳號">
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="bg-light">戶籍地址</th>
                                                <td>
                                                    <input type="text" id="registrationAddressInput" class="form-control" placeholder="請輸入戶籍地址">
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>

                            <!-- 審查結果區 -->
                            <div id="reviewSection" class="mt-5">
                                <h5 class="square-title mb-3">審查結果</h5>

                                <div class="table-responsive mt-3">
                                    <table class="table align-middle gray-table">
                                        <thead>
                                            <tr>
                                                <th width="150">審查委員</th>
                                                <th>審查意見（請上傳PDF）</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td id="reviewerName">委員姓名</td>
                                                <td>
                                                    <div class="d-flex align-items-center gap-2" id="reviewUploadSection">
                                                        <input type="file" id="reviewFileInput" accept=".pdf" style="display: none;" onchange="handleReviewFileUpload(this)">
                                                        <button class="btn btn-sm btn-teal-dark" type="button" onclick="document.getElementById('reviewFileInput').click()">
                                                            <i class="fas fa-file-upload me-1"></i>
                                                            上傳PDF
                                                        </button>
                                                        <div id="uploadedReviewFile" style="display: none;" class="d-flex align-items-center gap-2">
                                                            <a href="#" id="reviewDownloadLink" class="btn btn-sm btn-outline-teal">
                                                                <i class="fas fa-download me-1"></i>
                                                                <span id="reviewFileName"></span>
                                                            </a>
                                                            <span class="text-success">
                                                                <i class="fas fa-check-circle me-1"></i>已上傳
                                                            </span>
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>

                                <div class="d-flex justify-content-center mt-4">
                                    <button type="button" class="btn btn-teal" id="btnSubmitReview">
                                        <i class="fas fa-check me-2"></i>提交審查結果
                                    </button>
                                </div>
                            </div>
                           

                         
                    
                        </div>
                    </div>
                    
                    <footer>
                        <ul>
                            <li>地址：806高雄市前鎮區成功二路25號4樓</li>
                            <li>電話：(07)338-1810     Copyright © 海洋委員會 版權所有</li>
                        </ul>
                    </footer>        
                </div>
                <!-- 主要內容 END-->
            </div>
        </main>

        <!-- Modal 風險評估 -->
        <div class="modal fade" id="riskAssessmentModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="riskAssessmentModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-xl">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="fs-24 fw-bold text-green-light">風險評估</h4>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                            <i class="fa-solid fa-circle-xmark"></i>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div class="bg-light-gray p-3 mb-4">
                            <ul class="lh-lg">
                                <li>
                                    <span class="text-gray">執行單位:</span>
                                    <asp:Label ID="lblExecutingUnit" runat="server" />
                                </li>
                                <li>
                                    <span class="text-gray">風險評估:</span>
                                    <asp:Label ID="lblModalRiskLevel" runat="server" CssClass="text-pink" />
                                </li>
                            </ul>
                        </div>
                        
                    </div>
                </div>
            </div>
        </div>

        <!-- JavaScript 文件 -->
        <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
        <script src="<%= ResolveUrl("~/assets/vendor/bootstrap-5.3.3/dist/js/bootstrap.bundle.min.js") %>"></script>
        <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
        
        <script>
        let currentToken = '';
        let currentProjectID = '';
        let currentStage = 1;
        
        $(document).ready(function() {
            // 從URL取得token
            const urlParams = new URLSearchParams(window.location.search);
            currentToken = urlParams.get('token');

            if (currentToken) {
                loadExamData();
                loadBankCodeList();
            } else {
                alert('無效的審查連結，請檢查Token參數');
            }

            // 綁定提交審查結果事件
            $('#btnSubmitReview').click(function() {
                submitReviewResult();
            });
        });
        
        // 載入審查資料
        function loadExamData() {
            $.ajax({
                type: 'POST',
                url: 'SciExamReview.aspx/GetExamData',
                data: JSON.stringify({ token: currentToken }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function(response) {
                    if (response.d && response.d.Success) {
                        const data = response.d.Data;
                        currentProjectID = data.ProjectID;
                        currentStage = data.Stage;
                        
                        // 更新標題
                        $('#reportTitle').text(data.StageName + '審查');

                        // 設定審查委員姓名
                        $('#reviewerName').text(data.Reviewer || '審查委員');
                        $('#reviewerDisplayName').text(data.Reviewer || '審查委員');
                        if (data.Account) {
                            $('#reviewerAccount').text('(' + data.Account + ')');
                        }

                        // 設定審查委員銀行資訊
                        if (data.BankCode) {
                            $('#bankCodeSelect').val(data.BankCode);
                        }
                        if (data.BankAccount) {
                            $('#bankAccountInput').val(data.BankAccount);
                        }
                        if (data.RegistrationAddress) {
                            $('#registrationAddressInput').val(data.RegistrationAddress);
                        }

                        // 載入檔案清單
                        loadReportFiles(data.Files);

                        // 檢查是否已有上傳的審查檔案
                        checkExistingReviewFile();
                    } else {
                        alert(response.d ? response.d.Message : '載入審查資料失敗');
                    }
                },
                error: function(xhr, status, error) {
                    console.error('AJAX Error:', error);
                    alert('載入審查資料時發生錯誤');
                }
            });
        }
        
        // 載入報告檔案清單
        function loadReportFiles(files) {
            const tbody = $('#reportFilesTable');
            tbody.empty();
            
            if (!files || files.length === 0) {
                tbody.append('<tr><td colspan="4" class="text-center text-muted">尚無上傳檔案</td></tr>');
                return;
            }
            
            files.forEach(function(file, index) {
                const row = `
                    <tr>
                        <td class="text-center">${index + 1}</td>
                        <td>
                            <div>${file.FileName}</div>
                        </td>
                        <td class="text-center">
                            <span class="text-info">${file.FileType}</span>
                        </td>
                        <td>
                            <div class="d-flex align-items-center gap-2">
                                <button type="button" class="btn btn-sm btn-teal-dark" 
                                        onclick="downloadFile('${file.FileCode}')">
                                    <i class="fas fa-download me-1"></i>
                                    下載
                                </button>
                            </div>
                        </td>
                    </tr>
                `;
                
                tbody.append(row);
            });
        }
        
        // 下載檔案
        function downloadFile(fileCode) {
            if (!currentProjectID || !fileCode) {
                alert('下載參數不完整');
                return;
            }
            
            const downloadUrl = `SciExamReview.aspx?action=download&projectID=${currentProjectID}&fileCode=${fileCode}`;
            window.open(downloadUrl, '_blank');
        }
        
        // 處理審查PDF檔案上傳
        function handleReviewFileUpload(fileInput) {
            const file = fileInput.files[0];
            if (!file) {
                return;
            }
            
            // 驗證檔案類型
            const fileExt = '.' + file.name.split('.').pop().toLowerCase();
            if (fileExt !== '.pdf') {
                Swal.fire('錯誤', '請上傳 PDF 格式的檔案', 'error');
                fileInput.value = '';
                return;
            }
            
            // 檢查檔案大小 (10MB = 10 * 1024 * 1024 bytes)
            const maxSize = 10 * 1024 * 1024;
            if (file.size > maxSize) {
                Swal.fire('錯誤', '檔案大小不可超過 10MB', 'error');
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
            iframe.name = 'reviewUploadFrame';
            document.body.appendChild(iframe);
            
            const form = document.createElement('form');
            form.method = 'POST';
            form.enctype = 'multipart/form-data';
            form.target = 'reviewUploadFrame';
            form.action = `SciExamReview.aspx?action=uploadReview`;
            
            const fileField = document.createElement('input');
            fileField.type = 'file';
            fileField.name = 'file';
            fileField.files = fileInput.files;
            form.appendChild(fileField);
            
            const tokenField = document.createElement('input');
            tokenField.type = 'hidden';
            tokenField.name = 'token';
            tokenField.value = currentToken;
            form.appendChild(tokenField);
            
            document.body.appendChild(form);
            
            // 監聽上傳完成
            iframe.onload = function() {
                try {
                    const response = iframe.contentDocument.body.textContent;
                    const result = JSON.parse(response);
                    
                    Swal.close();
                    
                    if (result.Success) {
                        Swal.fire('成功', '審查意見檔案上傳成功', 'success');
                        
                        // 更新UI顯示已上傳狀態
                        $('#reviewFileName').text(result.FileName || file.name);
                        $('#reviewDownloadLink').attr('href', `SciExamReview.aspx?action=downloadReview&token=${currentToken}`);
                        $('#uploadedReviewFile').show();
                        $('#reviewFileInput').closest('button').hide();
                    } else {
                        Swal.fire('失敗', result.Message || '上傳失敗', 'error');
                    }
                } catch (e) {
                    Swal.close();
                    Swal.fire('錯誤', '上傳過程發生錯誤', 'error');
                }
                
                // 清理
                document.body.removeChild(form);
                document.body.removeChild(iframe);
            };
            
            form.submit();
            
            fileInput.value = '';
        }
        
        // 檢查是否已有上傳的審查檔案
        function checkExistingReviewFile() {
            $.ajax({
                type: 'POST',
                url: 'SciExamReview.aspx/CheckExistingReviewFile',
                data: JSON.stringify({ token: currentToken }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function(response) {
                    if (response.d && response.d.Success && response.d.HasFile) {
                        // 已有上傳檔案，顯示下載連結
                        $('#reviewFileName').text(response.d.FileName);
                        $('#reviewDownloadLink').attr('href', `SciExamReview.aspx?action=downloadReview&token=${currentToken}`);
                        $('#uploadedReviewFile').show();
                        $('#reviewFileInput').closest('button').hide();
                        
                        // 如果已提交，禁用操作
                        if (response.d.IsSubmitted) {
                            $('#btnSubmitReview').prop('disabled', true).html('<i class="fas fa-check me-2"></i>已提交');
                            $('#reviewFileInput').prop('disabled', true);
                        }
                    }
                },
                error: function(xhr, status, error) {
                    console.error('檢查已上傳檔案時發生錯誤:', error);
                }
            });
        }
        
        // 提交審查結果
        function submitReviewResult() {
            // 檢查是否已上傳審查意見PDF
            if ($('#uploadedReviewFile').is(':hidden')) {
                Swal.fire('錯誤', '請先上傳審查意見PDF檔案', 'error');
                return;
            }

            // 取得銀行資訊
            const bankCode = $('#bankCodeSelect').val();
            const bankAccount = $('#bankAccountInput').val();
            const registrationAddress = $('#registrationAddressInput').val();

            // 驗證銀行資訊必填欄位
            if (!bankCode) {
                Swal.fire('錯誤', '請選擇銀行金融代號', 'error');
                return;
            }

            if (!bankAccount) {
                Swal.fire('錯誤', '請輸入銀行帳號', 'error');
                return;
            }

            if (!registrationAddress) {
                Swal.fire('錯誤', '請輸入戶籍地址', 'error');
                return;
            }

            Swal.fire({
                title: '確定要提交審查結果嗎？',
                text: '提交後將無法修改',
                icon: 'question',
                showCancelButton: true,
                confirmButtonText: '確定提交',
                cancelButtonText: '取消'
            }).then((result) => {
                if (result.isConfirmed) {
                    // 顯示載入中
                    Swal.fire({
                        title: '處理中...',
                        text: '正在儲存資料並提交審查結果',
                        allowOutsideClick: false,
                        didOpen: () => {
                            Swal.showLoading();
                        }
                    });

                    // 提交審查結果（包含銀行資訊）
                    $.ajax({
                        type: 'POST',
                        url: 'SciExamReview.aspx/SubmitReviewResult',
                        data: JSON.stringify({
                            token: currentToken,
                            bankCode: bankCode,
                            bankAccount: bankAccount,
                            registrationAddress: registrationAddress
                        }),
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        success: function(response) {
                            Swal.close();
                            if (response.d && response.d.Success) {
                                Swal.fire({
                                    title: '成功',
                                    text: '審查結果提交成功',
                                    icon: 'success',
                                    confirmButtonText: '確定'
                                }).then(() => {
                                    // 提交成功後導向ReviewChecklist.aspx
                                    window.location.href = '../ReviewChecklist.aspx';
                                });
                            } else {
                                Swal.fire('失敗', response.d ? response.d.Message : '提交審查結果失敗', 'error');
                            }
                        },
                        error: function(xhr, status, error) {
                            Swal.close();
                            console.error('AJAX Error:', error);
                            Swal.fire('錯誤', '提交審查結果時發生錯誤', 'error');
                        }
                    });
                }
            });
        }

        // 載入銀行代碼清單
        function loadBankCodeList() {
            $.ajax({
                type: 'POST',
                url: 'SciExamReview.aspx/GetBankCodeList',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function(response) {
                    if (response.d && response.d.Success) {
                        const bankCodes = response.d.Data;
                        const select = $('#bankCodeSelect');

                        // 清空現有選項（保留預設選項）
                        select.find('option:not(:first)').remove();

                        // 新增銀行代碼選項
                        bankCodes.forEach(function(bank) {
                            select.append(
                                $('<option></option>')
                                    .val(bank.Code)
                                    .text(bank.DisplayText)
                            );
                        });
                    } else {
                        console.error('載入銀行代碼清單失敗:', response.d ? response.d.Message : '未知錯誤');
                    }
                },
                error: function(xhr, status, error) {
                    console.error('載入銀行代碼清單時發生錯誤:', error);
                }
            });
        }

        </script>
    </form>
</body>
</html>