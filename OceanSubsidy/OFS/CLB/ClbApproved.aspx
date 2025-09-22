<%@ Page Title="" Language="C#" MasterPageFile="~/OFS/CLB/ClbInprogress.master" AutoEventWireup="true" CodeFile="ClbApproved.aspx.cs" Inherits="OFS_CLB_ClbApproved" %>
<%@ Register TagPrefix="uc" TagName="ClbApplicationControl" Src="~/OFS/CLB/UserControls/ClbApplicationControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadExtra" Runat="Server">
    <!-- SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    
    <!-- ClbApproved JavaScript 功能 -->
    <script src="<%= ResolveUrl("~/script/OFS/CLB/ClbApproved.js") %>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
    
    <!--  計畫變更紀錄 下載核定計畫書 -->
    <div class="block rounded-top-4 py-4 d-flex justify-content-between" style="position: sticky; top: 180px; z-index: 15;">
        <div>
            <button class="btn btn-teal-dark" type="button" onclick="applyChange()">
                <i class="fas fa-exchange"></i>
                計畫變更申請
            </button>
            <button class="btn btn-teal-dark" type="button" onclick="showChangeHistory()">
                <i class="fas fa-history"></i>
                計畫變更紀錄
            </button>
         
            <button class="btn btn-teal-dark" type="button" onclick="downloadApprovedPlan()">
                <i class="fa-solid fa-download"></i>
                下載核定計畫書
            </button>
        </div>
        <div class="d-flex gap-3 align-items-center">
            <div class="text-muted small">
                承辦人員：<asp:Label ID="lblReviewerName" runat="server" CssClass="fw-bold text-dark" Text="載入中..." />
            </div>
            <button type="button" id="btnTransferProject" class="btn btn-teal" runat="server" data-bs-toggle="modal" data-bs-target="#transferCaseModal" ClientIDMode="Static">
                移轉案件
            </button>
            <button class="btn btn-pink" type="button" onclick="alert('計畫終止功能 (靜態展示)')">
                計畫終止
            </button>
        </div>
    </div>
    
    <!-- 申請表的進度圖 -->
    <div class="application-step-container">
        <div class="application-step">
            <div class="step-item active" role="button" data-application-step="1">
                <div class="step-content">
                    <div class="step-label">申請表內容</div>
                    <div class="step-status">檢視中</div>
                </div>
            </div>
            <div class="step-item" role="button" data-application-step="2">
                <div class="step-content">
                    <div class="step-label">上傳附件</div>
                </div>
            </div>
        </div>
    </div>
    
    <!-- 資料檢視區域 -->
    <div class="data-view-section">
        <!-- 分頁內容 -->
        <div class="tab-content">
            <!-- 第一頁：申請表內容 -->
            <div class="tab-pane active" id="tab1">
                <uc:ClbApplicationControl ID="ucClbApplication" runat="server" />
            </div>
            
            <!-- 第二頁：上傳附件 -->
            <div class="tab-pane" id="tab2" style="display: none;">
                <!-- 使用 UserControl 的上傳附件區塊 -->
                <div id="uploadAttachmentFromUserControl"></div>
            </div>
        </div>
    </div>
    
    <!-- Modal 計畫變更 -->
    <div class="modal fade" id="planChangeModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="planChangeModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="fs-24 fw-bold text-green-light">計畫變更申請</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </div>
                <div class="modal-body">
                    <div class="mb-4">
                        <div class="fs-16 text-gray mb-3">
                            <i class="fas fa-info-circle text-primary me-2"></i>
                            計畫變更原因
                        </div>
                        <textarea id="planChangeReason" class="form-control" rows="6"
                                  placeholder="請詳細說明計畫變更的原因與內容..."
                                  style="resize: vertical; min-height: 150px;"></textarea>
                        <div class="text-muted small mt-2">
                            <i class="fas fa-exclamation-triangle text-warning me-1"></i>
                            請詳細說明變更原因，此資訊將作為審核依據
                        </div>
                    </div>

                    <div class="d-flex gap-4 flex-wrap justify-content-center mt-5">
                        <button type="button" class="btn btn-gray" data-bs-dismiss="modal">
                            <i class="fas fa-times me-1"></i>
                            取消
                        </button>
                        <button type="button" class="btn btn-teal" onclick="confirmPlanChange()">
                            <i class="fas fa-check me-1"></i>
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
                    <h4 class="fs-24 fw-bold text-green-light" id="h4TranProject" runat="server" >移轉案件</h4>
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
                        <button type="button" class="btn btn-teal" onclick="confirmTransfer()">
                            確認移轉
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>