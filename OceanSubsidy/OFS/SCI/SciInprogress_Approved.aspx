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
    
    <!-- 申請表進度切換 JavaScript -->
    <script>
        document.addEventListener('DOMContentLoaded', function() {
            // 綁定進度條點擊事件
            const stepItems = document.querySelectorAll('.application-step-container .step-item');
            const tabPanes = document.querySelectorAll('.tab-pane');

            stepItems.forEach((item, index) => {
                item.addEventListener('click', function() {
                    const stepNumber = this.getAttribute('data-application-step');
                    switchToApplicationStep(stepNumber);
                });
            });

            // 切換申請表步驟
            function switchToApplicationStep(stepNumber) {
                // 移除所有 active 狀態
                stepItems.forEach(item => {
                    item.classList.remove('active');
                    const statusElement = item.querySelector('.step-status');
                    if (statusElement) {
                        statusElement.textContent = '';
                    }
                });

                // 設定新的 active 狀態
                const targetStep = document.querySelector(`[data-application-step="${stepNumber}"]`);
                if (targetStep) {
                    targetStep.classList.add('active');
                    let statusElement = targetStep.querySelector('.step-status');
                    if (!statusElement) {
                        statusElement = document.createElement('div');
                        statusElement.className = 'step-status';
                        targetStep.querySelector('.step-content').appendChild(statusElement);
                    }
                    statusElement.textContent = '檢視中';
                }

                // 切換對應的分頁內容
                if (tabPanes.length > 0) {
                    tabPanes.forEach(pane => {
                        pane.classList.remove('active');
                        pane.style.display = 'none';
                    });

                    const targetTab = document.getElementById(`tab${stepNumber}`);
                    if (targetTab) {
                        targetTab.classList.add('active');
                        targetTab.style.display = 'block';
                    }
                }
            }

            // 初始化第一個步驟
            switchToApplicationStep('1');
        });
    </script>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
        <!--  計畫變更紀錄 下載核定計畫書 -->
        <div class="block rounded-top-4 py-4 d-flex justify-content-between" style="position: sticky; top: 180px; z-index: 15;">
            <div>
                <button class="btn btn-teal-dark" type="button" data-bs-toggle="modal" data-bs-target="#changePlanModal">
                    <i class="fas fa-exchange"></i>
                    計畫變更申請
                </button>
                <button class="btn btn-teal-dark" type="button" data-bs-toggle="modal" data-bs-target="#changePlanRecordModal">
                    <i class="fas fa-history"></i>
                    計畫變更紀錄
                </button>
                <a href="#" class="btn btn-teal-dark" target="_blank">
                    <i class="fa-solid fa-download"></i>
                    下載核定計畫書
                </a>
            </div>
            <div class="d-flex gap-3 align-items-center">
                <div class="text-muted small">
                    承辦人員：<asp:Label ID="lblCurrentReviewer" runat="server" CssClass="fw-bold text-dark" Text="未設定" />
                </div>
                <button type="button" id="btnTransferProject" class="btn btn-teal" runat="server" data-bs-toggle="modal" data-bs-target="#transferCaseModal" ClientIDMode="Static">
                    移轉案件
                </button>
                <button class="btn btn-pink" type="button" data-bs-toggle="modal" data-bs-target="#planStopModal">
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
</asp:Content>