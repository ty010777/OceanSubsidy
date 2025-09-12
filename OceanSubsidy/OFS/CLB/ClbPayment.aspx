<%@ Page Title="" Language="C#" MasterPageFile="~/OFS/CLB/ClbInprogress.master" AutoEventWireup="true" CodeFile="ClbPayment.aspx.cs" Inherits="OFS_CLB_ClbPayment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadExtra" Runat="Server">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="<%= ResolveUrl("~/script/OFS/CLB/ClbPayment.js") %>"></script>
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
                <table  id="paymentTable" class="table align-middle gray-table">
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
                                <div><span class="text-pink view-mode">*</span>收支明細表</div>
                                <button class="btn view-mode btn-sm btn-teal-dark rounded-pill mt-2" type="button" onclick="downloadTemplate(1)">
                                    <i class="fas fa-file-download me-1"></i>
                                    範本下載
                                </button>
                            </td>
                            <td class="text-center">
                                <span id="uploadStatus1" class="text-pink">尚未上傳</span>
                            </td>
                            <td>
                                <div class="d-flex align-items-center gap-2">
                                    <input type="file" id="fileInput1" accept=".pdf" style="display: none;" onchange="handleFileUpload(1, this)">
                                    <button class="btn btn-sm btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput1').click()">
                                        <i class="fas fa-file-upload me-1"></i>
                                        上傳
                                    </button>
                                    <div id="uploadedFile1" style="display: none;" class=" align-items-center gap-2">
                                        <span class="tag tag-green-light">
                                            <a class="tag-link" href="#" id="downloadLink1" onclick="downloadUploadedFile(1)">
                                                <span id="fileName1"></span>
                                            </a>
                                            <button type="button" class="tag-btn" onclick="deleteUploadedFile(1)">
                                                <i class="fa-solid fa-circle-xmark"></i>
                                            </button>
                                        </span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="text-center">2</td>
                            <td>
                                <div><span class="text-pink view-mode">*</span>受補助清單</div>
                                <button class="btn btn-sm btn-teal-dark rounded-pill mt-2 view-mode" type="button" onclick="downloadTemplate(2)">
                                    <i class="fas fa-file-download  me-1"></i>
                                    範本下載
                                </button>
                            </td>
                            <td class="text-center">
                                <span id="uploadStatus2" class="text-pink">尚未上傳</span>
                            </td>
                            <td>
                                <div class="d-flex align-items-center gap-2">
                                    <input type="file" id="fileInput2" accept=".pdf" style="display: none;" onchange="handleFileUpload(2, this)">
                                    <button class="btn btn-sm btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput2').click()">
                                        <i class="fas fa-file-upload me-1"></i>
                                        上傳
                                    </button>
                                    <div id="uploadedFile2" style="display: none;" class="align-items-center gap-2">
                                        <span class="tag tag-green-light">
                                            <a class="tag-link" href="#" id="downloadLink2" onclick="downloadUploadedFile(2)">
                                                <span id="fileName2"></span>
                                            </a>
                                            <button type="button" class="tag-btn" onclick="deleteUploadedFile(2)">
                                                <i class="fa-solid fa-circle-xmark"></i>
                                            </button>
                                        </span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="text-center">3</td>
                            <td>
                                <div><span class="text-pink view-mode">*</span>經費分攤表</div>
                                <button class="btn btn-sm btn-teal-dark rounded-pill mt-2 view-mode" type="button" onclick="downloadTemplate(3)">
                                    <i class="fas fa-file-download me-1 "></i>
                                    範本下載
                                </button>
                            </td>
                            <td class="text-center">
                                <span id="uploadStatus3" class="text-pink">尚未上傳</span>
                            </td>
                            <td>
                                <div class="d-flex align-items-center gap-2 ">
                                    <input type="file" id="fileInput3" accept=".pdf" style="display: none;" onchange="handleFileUpload(3, this)">
                                    <button class="btn btn-sm btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput3').click()">
                                        <i class="fas fa-file-upload me-1"></i>
                                        上傳
                                    </button>
                                    <div id="uploadedFile3" style="display: none;" class=" align-items-center gap-2">
                                        <span class="tag tag-green-light">
                                            <a class="tag-link" href="#" id="downloadLink3" onclick="downloadUploadedFile(3)">
                                                <span id="fileName3"></span>
                                            </a>
                                            <button type="button" class="tag-btn" onclick="deleteUploadedFile(3)">
                                                <i class="fa-solid fa-circle-xmark"></i>
                                            </button>
                                        </span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr> 
                            <td class="text-center">4</td>
                            <td>
                                <div><span class="text-pink view-mode">*</span>憑證</div>
                                <button class="btn btn-sm btn-teal-dark rounded-pill mt-2 view-mode" type="button" onclick="downloadTemplate(4)">
                                    <i class="fas fa-file-download me-1 "></i>
                                    範本下載
                                </button>
                            </td>
                            <td class="text-center">
                                <span id="uploadStatus4" class="text-pink">尚未上傳</span>
                            </td>
                            <td>
                                <div class="d-flex align-items-center gap-2">
                                    <input type="file" id="fileInput4" accept=".pdf" style="display: none;" onchange="handleFileUpload(4, this)">
                                    <button class="btn btn-sm btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput4').click()">
                                        <i class="fas fa-file-upload me-1"></i>
                                        上傳
                                    </button>
                                    <div id="uploadedFile4" style="display: none;" class="align-items-center gap-2">
                                        <span class="tag tag-green-light">
                                            <a class="tag-link" href="#" id="downloadLink4" onclick="downloadUploadedFile(4)">
                                                <span id="fileName4"></span>
                                            </a>
                                            <button type="button" class="tag-btn" onclick="deleteUploadedFile(4)">
                                                <i class="fa-solid fa-circle-xmark"></i>
                                            </button>
                                        </span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr> 
                            <td class="text-center">5</td>
                            <td>
                                <div><span class="text-pink view-mode">*</span>領據（含帳戶資料）</div>
                                <button class="btn btn-sm btn-teal-dark rounded-pill mt-2 view-mode" type="button" onclick="downloadTemplate(5)">
                                    <i class="fas fa-file-download me-1 "></i>
                                    範本下載
                                </button>
                            </td>
                            <td class="text-center">
                                <span id="uploadStatus5" class="text-pink">尚未上傳</span>
                            </td>
                            <td>
                                <div class="d-flex align-items-center gap-2">
                                    <input type="file" id="fileInput5" accept=".pdf" style="display: none;" onchange="handleFileUpload(5, this)">
                                    <button class="btn btn-sm btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput5').click()">
                                        <i class="fas fa-file-upload me-1"></i>
                                        上傳
                                    </button>
                                    <div id="uploadedFile5" style="display: none;" class=" align-items-center gap-2">
                                        <span class="tag tag-green-light">
                                            <a class="tag-link" href="#" id="downloadLink5" onclick="downloadUploadedFile(5)">
                                                <span id="fileName5"></span>
                                            </a>
                                            <button type="button" class="tag-btn" onclick="deleteUploadedFile(5)">
                                                <i class="fa-solid fa-circle-xmark"></i>
                                            </button>
                                        </span>
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
            <p class="text-orange fw-bold mb-0" id="remainingAmountSection">賸餘款: <span id="remainingAmount">0</span> 元</p>
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
                        <td class="text-center" id="previousAmount">0</td>
                        <td class="text-center" id="accumulatedAmountCell">
                            <input type="number" id="accumulatedAmountInput" class="form-control text-center" placeholder="請輸入累積實支金額" min="0" step="1" style="width: 200px; margin: 0 auto;" value="0">
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
        <div class="scroll-bottom-panel">
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
                            <input id="radio-return" class="form-check-input check-teal" type="radio" name="reviewResult" >
                            <label for="radio-return">退回修改</label>
                        </div>
                        <span class="form-control textarea w-100" role="textbox" contenteditable="" data-placeholder="請輸入原因" aria-label="文本輸入區域" id="reviewComment"></span>
                    </div>
                </li>
    
            </ul>
            <button type="button" class="btn btn-teal d-table mx-auto" id = "confirmReviewBtn" onclick="submitReview()">確定撥款</button>
        </div>
</asp:Content>