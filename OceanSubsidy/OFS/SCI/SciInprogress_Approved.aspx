<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SciInprogress_Approved.aspx.cs" Inherits="OFS_SCI_SciInprogress_Approved" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/SCI/SciInprogress.master" EnableViewState="true" %>
<%@ Register TagPrefix="uc" TagName="SciApplicationControl" Src="~/OFS/SCI/UserControls/SciApplicationControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="SciWorkSchControl" Src="~/OFS/SCI/UserControls/SciWorkSchControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="SciFundingControl" Src="~/OFS/SCI/UserControls/SciFundingControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="SciRecusedListControl" Src="~/OFS/SCI/UserControls/SciRecusedListControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="SciUploadAttachmentsControl" Src="~/OFS/SCI/UserControls/SciUploadAttachmentsControl.ascx" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <!-- SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <!-- UserControl 相關 JavaScript 檔案 -->
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciApplication.js") %>"></script>
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciWorkSch.js") %>"></script>
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciFunding.js") %>"></script>
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciRecusedList.js") %>"></script>
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciUploadAttachments.js") %>"></script>

    <!-- 頁面主要 JavaScript -->
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciInprogress_Approved.js") %>"></script>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />

    <!-- 用於儲存當前頁簽狀態 -->
    <asp:HiddenField ID="hdnCurrentStep" runat="server" Value="1" ClientIDMode="Static" />

        <!--  計畫變更紀錄 下載核定計畫書 -->
        <div class="block rounded-top-4 py-4 d-flex justify-content-between" style="position: sticky; top: 180px; z-index: 15;">
            <div>
                <button id="btnPlanChange" class="btn btn-teal-dark" type="button" data-bs-toggle="modal" data-bs-target="#changePlanModal">
                    <i class="fas fa-exchange"></i>
                    計畫變更申請
                </button>
                <a href='<%= ResolveUrl("~/OFS/PlanChangeRecords.aspx?ProjectID=" + Request.QueryString["ProjectID"]) %>' 
                   class="btn btn-teal-dark"
                   target="_blank"
                   >
                    <i class="fas fa-history"></i>
                    計畫變更紀錄
                </a>
                
                <button type="button" id="btnDownloadPlan" class="btn btn-teal-dark">
                    <i class="fa-solid fa-download"></i>
                    下載核定計畫書
                </button>
            </div>
            <div class="d-flex gap-3 align-items-center">
                <div class="text-muted small">
                    承辦人員：<asp:Label ID="lblCurrentReviewer" runat="server" CssClass="fw-bold text-dark" Text="未設定" />
                </div>
                <button type="button" id="btnTransferProject" class="btn btn-teal" runat="server" data-bs-toggle="modal" data-bs-target="#transferCaseModal" ClientIDMode="Static">
                    移轉案件
                </button>
                <button class="btn btn-pink" type="button" id="btnPlanStop" runat="server" data-bs-toggle="modal" data-bs-target="#planStopModal" ClientIDMode="Static">
                    計畫終止
                </button>
            </div>
        </div>
        
    <!-- 申請表的進度圖 -->
    <div class="application-step-container">
        <div class="application-step">
            <div class="step-item active" role="button" data-application-step="1">
                <div class="step-content">
                    <div class="step-label">申請表/聲明書</div>
                    <div class="step-status">檢視中</div>
                </div>
            </div>
            <div class="step-item" role="button" data-application-step="2">
                <div class="step-content">
                    <div class="step-label">期程／工作項目／查核</div>
                </div>
            </div>
            <div class="step-item" role="button" data-application-step="3">
                <div class="step-content">
                    <div class="step-label">經費／人事</div>
                </div>
            </div>
            <div class="step-item" role="button" data-application-step="4">
                <div class="step-content">
                    <div class="step-label">其他</div>
                </div>
            </div>
            <div class="step-item" role="button" data-application-step="5">
                <div class="step-content">
                    <div class="step-label">上傳附件/提送申請</div>
                </div>
            </div>
        </div>
    </div>
    
    <!-- 資料檢視區域 -->
    <div class="data-view-section">
        <!-- 分頁內容 -->
        <div class="tab-content">
            <!-- 第一頁：申請表/聲明書 -->
            <div class="tab-pane active" id="tab1">
                <uc:SciApplicationControl ID="ucSciApplication" runat="server" />
            </div>
            
            <!-- 第二頁：期程／工作項目／查核 -->
            <div class="tab-pane" id="tab2" style="display: none;">
                <uc:SciWorkSchControl ID="ucSciWorkSch" runat="server" />
            </div>
            
            <!-- 第三頁：經費／人事 -->
            <div class="tab-pane" id="tab3" style="display: none;">
                <uc:SciFundingControl ID="ucSciFunding" runat="server" />
            </div>
            
            <!-- 第四頁：其他 -->
            <div class="tab-pane" id="tab4" style="display: none;">
                <uc:SciRecusedListControl ID="ucSciRecusedList" runat="server" />
            </div>
            
            <!-- 第五頁：上傳附件/提送申請 -->
            <div class="tab-pane" id="tab5" style="display: none;">
                <uc:SciUploadAttachmentsControl ID="ucSciUploadAttachments" runat="server" />
            </div>
        </div>
    </div>

    <!-- Modal 計畫變更申請 -->
    <div class="modal fade" id="changePlanModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="changePlanModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-md">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="fs-24 fw-bold text-green-light">計畫變更申請</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </div>
                <div class="modal-body">
                    <!-- 提醒訊息 -->
                    <div class="alert alert-danger d-flex align-items-start mb-4" role="alert">
                        <i class="fas fa-exclamation-triangle me-2 mt-1"></i>
                        <div>
                            <strong>提醒您：</strong>計畫變更審核通過後，預定進度及每月進度資料將清空，必須重填！
                        </div>
                    </div>

                    <!-- 計畫變更原因 -->
                    <div>
                        <label for="txtChangeReason" class="form-label fs-16 text-gray mb-3">
                            計畫變更原因 <span class="text-danger">*</span>
                        </label>
                        <textarea
                            id="txtChangeReason"
                            class="form-control"
                            rows="8"
                            placeholder="請詳細說明計畫變更原因..."></textarea>
                    </div>

                    <!-- 按鈕群組 -->
                    <div class="d-flex gap-4 flex-wrap justify-content-center mt-5">
                        <button type="button" class="btn btn-gray" data-bs-dismiss="modal">
                            取消
                        </button>
                        <button type="button" id="btnConfirmChange" class="btn btn-teal">
                            確定變更
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal 移轉案件 -->
    <div class="modal fade" id="transferCaseModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="transferCaseModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-md">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="fs-24 fw-bold text-green-light" id="h4TransferProject" runat="server" >移轉案件</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </div>
                <div class="modal-body">
                    <div class="">
                        <div class="fs-16 text-gray mb-3">承辦人員</div>
                        <asp:UpdatePanel ID="upTransferCase" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlDepartment_SelectedIndexChanged">
                                </asp:DropDownList>
                                <asp:DropDownList ID="ddlReviewer" runat="server" CssClass="form-select mt-2">
                                </asp:DropDownList>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlDepartment" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>

                    <div class="d-flex gap-4 flex-wrap justify-content-center mt-5">
                        <button type="button" class="btn btn-gray" data-bs-dismiss="modal">
                            取消
                        </button>
                        <asp:Button ID="btnConfirmTransfer" runat="server"
                            Text="確認移轉"
                            CssClass="btn btn-teal"
                            OnClick="btnConfirmTransfer_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal 計畫終止 -->
    <div class="modal fade" id="planStopModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="planStopModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-md">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="fs-24 fw-bold text-green-light">計畫終止</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </div>
                <div class="modal-body">
                    <!-- 計畫終止原因 -->
                    <div class="mb-4">
                        <label for="txtStopReason" class="form-label fs-16 text-gray mb-3">
                            計畫終止原因 <span class="text-danger">*</span>
                        </label>
                        <textarea
                            id="txtStopReason"
                            class="form-control"
                            rows="5"
                            placeholder="請詳細說明計畫終止原因..."></textarea>
                    </div>

                    <!-- 已撥款金額 -->
                    <div class="mb-4">
                        <label class="form-label fs-16 text-gray mb-3">
                            已撥款金額
                        </label>
                        <div class="input-group">
                            <input type="text" id="txtPaidAmount" class="form-control" readonly value="載入中..." />
                            <span class="input-group-text">元</span>
                        </div>
                        <small class="text-muted">此金額為系統自動計算</small>
                    </div>

                    <!-- 已追回金額 -->
                    <div class="mb-4">
                        <label for="txtRecoveredAmount" class="form-label fs-16 text-gray mb-3">
                            已追回金額 <span class="text-danger">*</span>
                        </label>
                        <div class="input-group">
                            <input type="number" id="txtRecoveredAmount" class="form-control" placeholder="請輸入已追回金額" min="0" step="1" />
                            <span class="input-group-text">元</span>
                        </div>
                    </div>

                    <!-- 按鈕群組 -->
                    <div class="d-flex gap-4 flex-wrap justify-content-center mt-5">
                        <button type="button" class="btn btn-gray" data-bs-dismiss="modal">
                            取消
                        </button>
                        <button type="button" id="btnConfirmPlanStop" class="btn btn-pink">
                            送出
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- 計畫變更審查面板 (右下角浮動面板) -->
    <div class="scroll-bottom-panel" id="reviewPanel" runat="server" style="display: none;">
        <h5 class="text-pink fs-18 fw-bold mb-3">計畫變更審查</h5>
        <ul class="d-flex flex-column gap-3 mb-3">
            <li class="d-flex gap-2">
                <span class="text-gray mt-2">審查結果 :</span>
                <div class="d-flex flex-column gap-2 align-items-start flex-grow-1">
                    <div class="form-check-input-group d-flex text-nowrap mt-2 align-items-center">
                        <input id="radioPass" class="form-check-input check-teal" type="radio" name="reviewResult" value="pass"/>
                        <label for="radioPass">通過</label>
                        <input id="radioReject" class="form-check-input check-teal" type="radio" name="reviewResult" value="reject"/>
                        <label for="radioReject">不通過</label>
                    </div>
                </div>
            </li>
            <li class="d-flex gap-2 d-none" id="rejectReasonSection">
                <span class="text-gray mt-2">退回原因 :</span>
                <div class="d-flex flex-column gap-2 align-items-start flex-grow-1">
                    <span class="form-control textarea w-100" role="textbox" contenteditable="" data-placeholder="請輸入退回原因" aria-label="退回原因輸入區域" id="rejectReasonText" style="min-height: 120px;"></span>
                </div>
            </li>
        </ul>
        <button type="button" class="btn btn-teal d-table mx-auto" id="submitReviewButton" onclick="submitChangeReview()">確定</button>
    </div>
</asp:Content>