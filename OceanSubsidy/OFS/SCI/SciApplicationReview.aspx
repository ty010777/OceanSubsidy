<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SciApplicationReview.aspx.cs" Inherits="OFS_SCI_Review_SciApplicationReview" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/SCI/OFSReviewMaster.master" EnableViewState="true" %>
<%@ Register TagPrefix="uc" TagName="SciApplicationControl" Src="~/OFS/SCI/UserControls/SciApplicationControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="SciWorkSchControl" Src="~/OFS/SCI/UserControls/SciWorkSchControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="SciFundingControl" Src="~/OFS/SCI/UserControls/SciFundingControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="SciRecusedListControl" Src="~/OFS/SCI/UserControls/SciRecusedListControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="SciUploadAttachmentsControl" Src="~/OFS/SCI/UserControls/SciUploadAttachmentsControl.ascx" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <!-- SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    
    <!-- UserControl 相關 JavaScript 檔案 -->
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciWorkSch.js") %>"></script>
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciApplication.js") %>"></script>
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciFunding.js") %>"></script>
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciRecusedList.js") %>"></script>
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciUploadAttachments.js") %>"></script>
    
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
        $('form').on('submit', function () {
            const reviewNotes = $('[name="reviewNotes"]').text().trim();
            $('#reviewNotesHidden').val(reviewNotes);
        });
    </script>
    
</asp:Content>

<asp:Content ID="ApplicationContent" ContentPlaceHolderID="ApplicationContent" runat="server">
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
           <asp:LinkButton ID="btnDownloadPlan" runat="server"
               CssClass="btn btn-teal-dark"
               OnClick="btnDownloadPlan_Click"
               CausesValidation="false">
               <i class="fa-solid fa-download"></i> 下載計畫書
           </asp:LinkButton>
        </div>
    </div>
    
    <!-- 分頁內容 -->
    <div class="tab-content">
        <!-- 第一頁：申請表/聲明書 -->
        <div class="tab-pane active" id="tab1">
            <uc:SciApplicationControl ID="ucSciApplication" runat="server" />
        </div>
        
        <!-- 第二頁：期程及工作項目 -->
        <div class="tab-pane" id="tab2" style="display: none;">
            <uc:SciWorkSchControl ID="ucSciWorkSch" runat="server" />
        </div>
        
        <!-- 第三頁：經費/人事費明細 -->
        <div class="tab-pane" id="tab3" style="display: none;">
            <uc:SciFundingControl ID="ucSciFunding" runat="server" />
        </div>
        
        <!-- 第四頁：委員迴避清單 -->
        <div class="tab-pane" id="tab4" style="display: none;">
            <uc:SciRecusedListControl ID="ucSciRecusedList" runat="server" />
        </div>
        
        <!-- 第五頁：上傳附件 -->
        <div class="tab-pane" id="tab5" style="display: none;">
            <uc:SciUploadAttachmentsControl ID="ucSciUploadAttachments" runat="server" />
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

    <!-- 審查結果面板 (計畫審查-資格審查-點選不通過或退回補正補件) -->
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
                    <span class="form-control textarea w-100" role="textbox" contenteditable="" data-placeholder="請輸入原因" aria-label="文本輸入區域" name="reviewNotes"></span>
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
                                <span class="text-gray">執行單位:</span>
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
                                    <th>執行單位回覆</th>
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