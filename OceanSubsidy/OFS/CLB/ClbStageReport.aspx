<%@ Page Title="" Language="C#" MasterPageFile="~/OFS/CLB/ClbInprogress.master" AutoEventWireup="true" CodeFile="ClbStageReport.aspx.cs" Inherits="OFS_CLB_ClbStageReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadExtra" Runat="Server">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="<%= ResolveUrl("~/script/OFS/CLB/ClbStageReport.js") %>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    
    <!-- 內容區塊 -->
    <div class="block rounded-top-4">
        <h5 class="square-title mt-4">成果報告審查</h5>
        <p class="text-pink mt-3 lh-base">請下載報告書範本，填寫資料及公文用印後上傳。<br>
            成果報告書及相關檔案，請壓縮ZIP上傳（檔案100MB以內）。
        </p>
        
        <div class="table-responsive mt-3">
            <table id="StageTable" class="table align-middle gray-table">
                <thead>
                    <tr>
                        <th width="70" class="text-center">附件編號</th>
                        <th>附件名稱</th>
                        <th class="text-center">狀態</th>
                        <th>上傳附件</th>
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
                                <div id="uploadedFile1" style="display: none;" class="align-items-center gap-2">
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

    
</asp:Content>