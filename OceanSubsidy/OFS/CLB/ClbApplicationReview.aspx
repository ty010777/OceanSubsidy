<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ClbApplicationReview.aspx.cs" Inherits="OFS_CLB_Review_ClbApplicationReview" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFSMaster.master" EnableViewState="true" %>
<%@ Register TagPrefix="uc" TagName="ClbApplicationControl" Src="~/OFS/CLB/UserControls/ClbApplicationControl.ascx" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    社團補助案內容審查 - 海洋領域補助計畫管理資訊系統
</asp:Content>

<asp:Content ID="BreadcrumbsContent" ContentPlaceHolderID="Breadcrumbs" runat="server">
    <div class="page-title">
        <img src="<%= ResolveUrl("~/assets/img/information-system-title-icon05.svg") %>" alt="logo">
        <div>
            <span>目前位置</span>
            <div class="d-flex align-items-end gap-3">
                <h2 class="text-teal-dark">社團補助案內容審查</h2>
                <a class="text-teal-dark text-decoration-none" href="<%= ResolveUrl("~/OFS/ReviewChecklist.aspx") %>">
                    <i class="fas fa-angle-left"></i>
                    返回列表
                </a>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <!-- SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    
    <!-- ClbApplication JavaScript 功能 (包含進度條切換) -->
    <script src="<%= ResolveUrl("~/script/OFS/CLB/ClbApplication.js") %>"></script>
    
    <!-- 審查結果互動功能 -->
    <script>
        $(function() {
            $('input[name="reviewResult"]').change(function() {
                if ($(this).val() === 'return') {
                    $('#returnDate').show();
                    // 預設帶入今天 + 7天
                    if (!$('#returnDate').val()) {
                        var defaultDate = new Date();
                        defaultDate.setDate(defaultDate.getDate() + 7);
                        $('#returnDate').val(defaultDate.toISOString().split('T')[0]);
                    }
                } else {
                    $('#returnDate').hide().val('');
                }
            });
        });
        
        // 在按鈕點擊時處理審查意見
        function handleReviewSubmit() {
            const reviewNotesElement = $('#reviewNotes');
            const reviewNotes = reviewNotesElement.text().trim();
            $('#reviewNotesHidden').val(reviewNotes);
            console.log('Review notes element found:', reviewNotesElement.length);
            console.log('Review notes content:', reviewNotes);
            console.log('Hidden field value set to:', $('#reviewNotesHidden').val());
            
            // 取得選中的審查結果
            const selectedReviewResult = $('input[name="reviewResult"]:checked').val();
            console.log('Selected review result:', selectedReviewResult);
            
            // 如果是「通過」則不需要審查意見，其他情況需要審查意見
            if (selectedReviewResult !== 'pass' && !reviewNotes) {
                alert('請輸入審查意見');
                return false;
            }
            
            return true;
        }
        
        // 綁定到確認審查按鈕
        $(document).ready(function() {
            // 使用更精確的選擇器
            $('[id$="btnConfirmReview"]').click(function(e) {
                if (!handleReviewSubmit()) {
                    e.preventDefault();
                    return false;
                }
            });
            
            // 初始化進度條狀態
            $('.application-step .step-item').eq(0).addClass('active');
        });

        // 覆寫進度條點擊行為，只在當前頁面切換顯示
        function navigateToStepByUrl(step) {
            if (step === 0) {
                // 顯示申請表區塊
                $('#applicationFormSection').show();
                $('#uploadAttachmentSection').hide();
                // 更新進度條狀態
                $('.application-step .step-item').removeClass('active');
                $('.application-step .step-item').eq(0).addClass('active');
            } else if (step === 1) {
                // 顯示上傳附件區塊
                $('#applicationFormSection').hide();
                $('#uploadAttachmentSection').show();
                // 更新進度條狀態
                $('.application-step .step-item').removeClass('active');
                $('.application-step .step-item').eq(1).addClass('active');
            }
        }

        // 下載申請資料
        document.addEventListener('DOMContentLoaded', function() {
            var btnDownloadPlan = document.getElementById('btnDownloadPlan');
            if (btnDownloadPlan) {
                btnDownloadPlan.addEventListener('click', function() {
                    var projectId = '<%= ProjectID %>';

                    if (!projectId) {
                        Swal.fire({
                            title: '錯誤',
                            text: '找不到計畫ID',
                            icon: 'error',
                            confirmButtonText: '確定'
                        });
                        return;
                    }

                    // 直接開啟下載 URL
                    var downloadUrl = '<%= ResolveUrl("~/Service/CLB_download.ashx") %>?action=downloadPlan&projectID=' + projectId;
                    window.open(downloadUrl, '_blank');
                });
            }
        });
    </script>
    
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
    
    <!-- 審核承辦人員 -->
    <div class="top-wrapper">
        <div class="top-block">
            <div class="d-flex align-items-center gap-3">
                <span>審核承辦人員：<asp:Label ID="lblReviewerName" runat="server" CssClass="fw-bold" /></span>
                <button type="button" id="btnTransferProject" class="btn btn-teal" runat="server" data-bs-toggle="modal" data-bs-target="#transferCaseModal" ClientIDMode="Static">
                    移轉案件
                </button>
            </div>
           <button type="button" id="btnDownloadPlan" class="btn btn-teal-dark">
               <i class="fa-solid fa-download"></i> 下載申請資料
           </button>
        </div>
    </div>
    
    <!-- 申請流程進度條 -->
    <div class="application-step">
        <div class="step-item" role="button" onclick="navigateToStepByUrl(0)">
            <div class="step-content">
                <div class="step-label">申請表</div>
                <!-- 狀態將由後端動態設定 -->
            </div>
        </div>
        <div class="step-item" role="button" onclick="navigateToStepByUrl(1)">
            <div class="step-content">
                <div class="step-label">上傳附件/提送申請</div>
                <!-- 狀態將由後端動態設定 -->
            </div>
        </div>
    </div>

    <!-- 申請資料內容 -->
    <div class="tab-content">
        <!-- 社團申請資料 -->
        <div class="tab-pane active" id="tab1">
            <uc:ClbApplicationControl ID="ucClbApplication" runat="server" />
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
                        <asp:Button ID="btnConfirmTransfer" runat="server" 
                            Text="確認移轉" 
                            CssClass="btn btn-teal" 
                            OnClick="btnConfirmTransfer_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- 審查結果面板 -->
    <div class="scroll-bottom-panel" id="scrollBottomPanel" runat="server" >
        <h5 class="text-pink fs-18 fw-bold mb-3">審查結果</h5>

        <ul class="d-flex flex-column gap-3 mb-3">
            <li class="d-flex gap-2">
                <span class="text-gray">同單位申請計畫數 :</span>
                <asp:Label ID="lblSameUnitProjectCount" runat="server" CssClass="link-teal fw-bold" Text="0" />
            </li>
            <li class="d-flex gap-2">
                <span class="text-gray">風險評估 :</span>
                <div>
                    <asp:Label ID="lblRiskLevel" runat="server" CssClass="text-pink" Text="低風險" />
                    <span>( <a class="link-teal fw-bold" href="#" data-bs-toggle="modal" data-bs-target="#riskAssessmentModal">
                        <asp:Label ID="lblRiskRecordCount" runat="server" Text="0" />
                    </a> 筆記錄)</span>
                </div>
            </li>
            <li class="d-flex gap-2">
                <span class="text-gray mt-2">審查結果 :</span>
                <div class="d-flex flex-column gap-2 align-items-start flex-grow-1 checkPass">
                    <div class="form-check-input-group d-flex text-nowrap mb-2 align-items-center">
                        <input id="radio-pass" class="form-check-input check-teal radioPass" type="radio" name="reviewResult" value="pass" checked="">
                        <label for="radio-pass">通過</label>
                        <input id="radio-fail" class="form-check-input check-teal radioFail" type="radio" name="reviewResult" value="fail">
                        <label for="radio-fail">不通過</label>
                        <input id="radio-return" class="form-check-input check-teal radioReturn" type="radio" name="reviewResult" value="return">
                        <label for="radio-return">退回補正補件</label>
                        <input id="returnDate" class="form-control" type="date" name="returnDate" style="display: none;">
                    </div>
                    <span class="form-control textarea w-100" role="textbox" contenteditable="" data-placeholder="請輸入原因" aria-label="文本輸入區域" name="reviewNotes" id="reviewNotes"></span>
                    <input type="hidden" name="reviewNotesHidden" id="reviewNotesHidden">
                </div>
            </li>
        </ul>
        <asp:Button ID="btnConfirmReview" runat="server" Text="確定" CssClass="btn btn-teal d-table mx-auto" OnClick="btnConfirmReview_Click"/>
        <asp:HiddenField ID="hdnAssignedReviewerAccount" runat="server" />
    </div>

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
                                <span class="text-gray">申請單位:</span>
                                <asp:Label ID="lblExecutingUnit" runat="server" />
                            </li>
                            <li>
                                <span class="text-gray">風險評估:</span>
                                <asp:Label ID="lblModalRiskLevel" runat="server" CssClass="text-pink" />
                            </li>
                        </ul>
                    </div>

                    <div class="table-responsive">
                        <table class="table align-middle gray-table lh-base">
                            <thead>
                                <tr>
                                    <th>計畫編號 / 計畫名稱</th>
                                    <th>查核日期 / 人員</th>
                                    <th>風險評估</th>
                                    <th>查核意見</th>
                                    <th>申請單位回覆</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblProjectInfo" runat="server" CssClass="link-teal" />
                                    </td>
                                    <td class="text-center">
                                        <div><asp:Label ID="lblCheckDate" runat="server" /></div>
                                        <div><asp:Label ID="lblChecker" runat="server" /></div>
                                    </td>
                                    <td><asp:Label ID="lblTableRiskLevel" runat="server" /></td>
                                    <td><asp:Label ID="lblCheckOpinion" runat="server" /></td>
                                    <td><asp:Label ID="lblUnitReply" runat="server" /></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>