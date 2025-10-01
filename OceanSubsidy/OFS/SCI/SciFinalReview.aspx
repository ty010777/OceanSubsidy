<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SciFinalReview.aspx.cs" Inherits="OFS_SCI_Review_SciFinalReview" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/SCI/OFSReviewMaster.master" EnableViewState="true" %>
<%@ Register TagPrefix="uc" TagName="SciApplicationControl" Src="~/OFS/SCI/UserControls/SciApplicationControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="SciWorkSchControl" Src="~/OFS/SCI/UserControls/SciWorkSchControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="SciFundingControl" Src="~/OFS/SCI/UserControls/SciFundingControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="SciRecusedListControl" Src="~/OFS/SCI/UserControls/SciRecusedListControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="SciUploadAttachmentsControl" Src="~/OFS/SCI/UserControls/SciUploadAttachmentsControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="ChangeDescriptionControl" Src="~/OFS/SCI/UserControls/ChangeDescriptionControl.ascx" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <!-- SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    
    <!-- UserControl 相關 JavaScript 檔案 -->
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciWorkSch.js") %>"></script>
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciApplication.js") %>"></script>
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciFunding.js") %>"></script>
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciRecusedList.js") %>"></script>
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciUploadAttachments.js") %>"></script>
    
    <!-- 動態載入變更說明的 JavaScript -->
    <script type="text/javascript">
        // 加入自定義邏輯來處理步驟點擊
        function setupChangeDescriptionLoader() {
            // 獲取 Master Page 的元素
            var stepItems = document.querySelectorAll('.application-step .step-item');

            for (var i = 0; i < stepItems.length; i++) {
                stepItems[i].addEventListener('click', function() {
                    var stepNumber = this.getAttribute('data-review-step');
                    loadChangeDescriptionForStep(stepNumber);
                });
            }
        }

        // 根據步驟載入對應的變更說明資料
        function loadChangeDescriptionForStep(stepNumber) {
            var sourcePage = '';
            var tabId = '';

            switch(stepNumber) {
                case '1':
                    sourcePage = 'SciApplication';
                    tabId = 'tab1';
                    break;
                case '2':
                    sourcePage = 'SciWorkSch';
                    tabId = 'tab2';
                    break;
                case '3':
                    sourcePage = 'SciFunding';
                    tabId = 'tab3';
                    break;
                case '4':
                    sourcePage = 'SciRecusedList';
                    tabId = 'tab4';
                    break;
                case '5':
                    sourcePage = 'SciUploadAttachments';
                    tabId = 'tab5';
                    break;
            }

            // 從預載的資料中讀取並更新顯示
            if (window.allChangeDescriptions && window.allChangeDescriptions[sourcePage]) {
                var changeData = window.allChangeDescriptions[sourcePage];

                // 根據 tabId 找到對應的變更說明元素（因為每個 tab 都有自己的 ChangeDescriptionControl）
                var tabContainer = document.getElementById(tabId);
                if (tabContainer) {
                    // 在特定 tab 容器內尋找變更說明元素
                    var changeBeforeElement = tabContainer.querySelector('#txtChangeBefore');
                    var changeAfterElement = tabContainer.querySelector('#txtChangeAfter');

                    if (changeBeforeElement) {
                        changeBeforeElement.textContent = changeData.ChangeBefore || '';
                        console.log('已更新 txtChangeBefore:', changeData.ChangeBefore);
                    } else {
                        console.warn('在 ' + tabId + ' 中找不到 txtChangeBefore 元素');
                    }

                    if (changeAfterElement) {
                        changeAfterElement.textContent = changeData.ChangeAfter || '';
                        console.log('已更新 txtChangeAfter:', changeData.ChangeAfter);
                    } else {
                        console.warn('在 ' + tabId + ' 中找不到 txtChangeAfter 元素');
                    }

                    console.log('已載入 ' + sourcePage + ' 的變更說明到 ' + tabId);
                } else {
                    console.error('找不到 tab 容器: ' + tabId);
                }
            } else {
                console.log('沒有找到 ' + sourcePage + ' 的變更說明內容');
            }
        }

        // 等待頁面載入完成後初始化
        document.addEventListener('DOMContentLoaded', function() {
            // 等待 Master Page 的 JavaScript 執行完成
            setTimeout(setupChangeDescriptionLoader, 500);

            // 初始載入第一個 tab 的變更說明
            setTimeout(function() {
                loadChangeDescriptionForStep('1');
            }, 800);
        });
    </script>
    
    <!-- 審查結果互動功能 -->
    <script>
        $(function() {
            // 同步 contenteditable 內容到隱藏欄位
            $('#rejectReasonText').on('input blur', function() {
                $('#rejectReason').val($(this).text());
            });
            
            // 表單提交前檢查
            $('#<%= btnConfirmReview.ClientID %>').click(function(e) {
                var reviewResult = $('input[name="reviewResult"]:checked').val();
                var rejectReason = $('#rejectReasonText').text().trim();
                
                if (reviewResult === 'fail' && !rejectReason) {
                    e.preventDefault();
                    Swal.fire({
                        title: '提醒',
                        text: '選擇不通過時，必須填寫原因',
                        icon: 'warning',
                        confirmButtonText: '確定'
                    });
                    return false;
                }
                
                // 同步內容到隱藏欄位
                $('#rejectReason').val(rejectReason);
            });
        });

        // 下載核定版計畫書
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
                    var downloadUrl = '<%= ResolveUrl("~/Service/SCI_Download.ashx") %>?action=downloadApprovedPlan&projectID=' + projectId;
                    window.open(downloadUrl, '_blank');
                });
            }
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
                <button type="button" id="btnTransferProject" class="btn btn-teal" data-bs-toggle="modal" data-bs-target="#transferCaseModal">
                    移轉案件
                </button>
            </div>
           <button type="button" id="btnDownloadPlan" class="btn btn-teal-dark">
               <i class="fa-solid fa-download"></i> 下載計畫書
           </button>
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
                    <h4 class="fs-24 fw-bold text-green-light">移轉案件</h4>
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
    <div class="scroll-bottom-panel">
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
                    </div>
                    <span class="form-control textarea w-100" role="textbox" contenteditable="" data-placeholder="請輸入原因(不通過時必填)" aria-label="文本輸入區域" id="rejectReasonText"></span>
                    <input type="hidden" name="rejectReason" id="rejectReason" />
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
