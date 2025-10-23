<%@ Page Title="" Language="C#" MasterPageFile="~/OFS/CLB/ClbInprogress.master" AutoEventWireup="true" CodeFile="ClbApproved.aspx.cs" Inherits="OFS_CLB_ClbApproved" %>
<%@ Register TagPrefix="uc" TagName="ClbApplicationControl" Src="~/OFS/CLB/UserControls/ClbApplicationControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadExtra" Runat="Server">
    <!-- SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    
    <!-- ClbApproved JavaScript 功能 -->
    <script src="<%= ResolveUrl("~/script/OFS/CLB/ClbApproved.js") %>"></script>
    <script src="<%= ResolveUrl("~/script/OFS/CLB/ClbApplication.js") %>"></script>

    <!-- 計畫變更審核功能 -->
    <script>
        $(document).ready(function() {
            // 綁定審核結果變更事件
            $('input[name="changeReviewResult"]').change(function() {
                if ($(this).val() === 'reject') {
                    // 退回修改：顯示審核意見輸入框並設為必填
                    $('#changeReviewNotes').show().attr('data-required', 'true');
                } else {
                    // 通過：隱藏審核意見輸入框並清空內容
                    $('#changeReviewNotes').hide().removeAttr('data-required').val('');
                }
            });

            // 初始化：預設隱藏審核意見輸入框
            $('#changeReviewNotes').hide();

            // 確認審核按鈕點擊事件
            $('[id$="btnConfirmChangeReview"]').click(function(e) {
                const selectedResult = $('input[name="changeReviewResult"]:checked').val();
                const reviewNotesElement = $('#changeReviewNotes');
                const reviewNotes = reviewNotesElement.val().trim(); // 使用 val() 而不是 text()

                // 設定隱藏欄位值
                $('#changeReviewNotesHidden').val(reviewNotes);

                // 驗證：退回修改時必須填寫審核意見
                if (selectedResult === 'reject' && !reviewNotes) {
                    Swal.fire({
                        title: '提醒',
                        text: '退回修改時請輸入審核意見',
                        icon: 'warning',
                        confirmButtonText: '確定'
                    });
                    e.preventDefault();
                    return false;
                }

                // 確認審核動作
                const actionText = selectedResult === 'approve' ? '通過' : '退回修改';

                e.preventDefault(); // 先阻止默認提交

                Swal.fire({
                    title: '確認審核',
                    text: `確定要${actionText}此計畫變更申請嗎？`,
                    icon: 'question',
                    showCancelButton: true,
                    confirmButtonText: '確定',
                    cancelButtonText: '取消'
                }).then((result) => {
                    if (result.isConfirmed) {
                        // 用戶確認後，觸發實際的按鈕點擊
                        $(e.target).off('click').click();
                    }
                });
            });
        });
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />

    <!--  計畫變更紀錄 下載核定計畫書 -->
    <div class="block rounded-top-4 py-4 d-flex justify-content-between" style="position: sticky; top: 180px; z-index: 15;">
        <div>
            <button class="btn btn-teal-dark" type="button" data-bs-toggle="modal" data-bs-target="#planChangeModal">
                <i class="fas fa-exchange"></i>
                計畫變更申請
            </button>
            <a href='<%= ResolveUrl("~/OFS/PlanChangeRecords.aspx?ProjectID=" + Request.QueryString["ProjectID"]) %>' class="btn btn-teal-dark">
                <i class="fas fa-history"></i>
                計畫變更紀錄
            </a>
         
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
            <button class="btn btn-pink" type="button" id="btnPlanStop" runat="server" data-bs-toggle="modal" data-bs-target="#planStopModal" ClientIDMode="Static">
                計畫終止
            </button>
        </div>
    </div>
    
    <!-- 申請表的進度圖 -->
  <div class="application-step">
          <div class="step-item active" role="button" onclick="switchTab('application')" id="applicationTab">
              <div class="step-content">
                  <div class="step-label">申請表</div>
                  <!-- 狀態將由後端動態設定 -->
              </div>
          </div>
          <div class="step-item" role="button" onclick="switchTab('upload')" id="uploadTab">
              <div class="step-content">
                  <div class="step-label">上傳附件/提送申請</div>
                  <!-- 狀態將由後端動態設定 -->
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

    <!-- 計畫變更審核面板 (僅限承辦人員且狀態為2才顯示) -->
    <div class="scroll-bottom-panel" id="changeReviewPanel" runat="server" Visible="false">
        <h5 class="text-pink fs-18 fw-bold mb-3">計畫變更審核</h5>

        <div class="d-flex gap-2 mb-3">
            <span class="text-gray">變更原因：</span>
            <asp:Label ID="lblChangeReason" runat="server" CssClass="fw-bold text-dark" />
        </div>

        <div class="d-flex gap-2 mb-4">
            <span class="text-gray mt-2">審核結果：</span>
            <div class="d-flex flex-column gap-2 align-items-start flex-grow-1 checkPass">
                <div class="form-check-input-group d-flex text-nowrap mb-2 align-items-center">
                    <input id="radio-approve" class="form-check-input check-teal radioApprove" type="radio" name="changeReviewResult" value="approve" checked="">
                    <label for="radio-approve">通過</label>
                    <input id="radio-reject" class="form-check-input check-teal radioReject" type="radio" name="changeReviewResult" value="reject">
                    <label for="radio-reject">退回修改</label>
                </div>
                <textarea class="form-control textarea-auto-resize w-100" placeholder="請輸入審核意見" name="changeReviewNotes" id="changeReviewNotes" rows="4"></textarea>
                <input type="hidden" name="changeReviewNotesHidden" id="changeReviewNotesHidden">
            </div>
        </div>

        <asp:Button ID="btnConfirmChangeReview" runat="server" Text="確認審核" CssClass="btn btn-teal d-table mx-auto" OnClick="btnConfirmChangeReview_Click"/>
    </div>

</asp:Content>