<%@ Page Title="" Language="C#" MasterPageFile="~/OFS/CLB/ClbInprogress.master" AutoEventWireup="true" CodeFile="ClbApproved.aspx.cs" Inherits="OFS_CLB_ClbApproved" %>
<%@ Register TagPrefix="uc" TagName="ClbApplicationControl" Src="~/OFS/CLB/UserControls/ClbApplicationControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadExtra" Runat="Server">
    <!-- SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    
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
        
        // 模擬功能按鈕
        function downloadApprovedPlan() {
            alert('下載核定計畫書功能 (靜態展示)');
        }
        
        function showChangeHistory() {
            alert('顯示計畫變更紀錄功能 (靜態展示)');
        }
        
        function applyChange() {
            alert('計畫變更申請功能 (靜態展示)');
        }
    </script>
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
                承辦人員：<span class="fw-bold text-dark">李小華</span>
            </div>
            <button type="button" class="btn btn-teal" onclick="alert('移轉案件功能 (靜態展示)')">
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
                <div class="block">
                    <h5 class="square-title">上傳附件</h5>
                    <p class="text-pink lh-base mt-3">
                        已上傳的申請附件檔案，供檢視下載。
                    </p>
                    <div class="table-responsive mt-3 mb-0">
                        <table class="table align-middle gray-table">
                            <thead class="text-center">
                                <tr>
                                    <th width="60">附件編號</th>
                                    <th>附件名稱</th>
                                    <th width="180">狀態</th>
                                    <th width="350">檔案下載</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td class="text-center">1</td>
                                    <td>
                                        <div>申請表</div>
                                    </td>
                                    <td class="text-center">
                                        <span class="text-success">已上傳</span>
                                    </td>
                                    <td>
                                        <button class="btn btn-sm btn-outline-teal" onclick="alert('下載申請表功能 (靜態展示)')">
                                            <i class="fas fa-download me-1"></i>
                                            CLB1140009_申請表.pdf
                                        </button>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="text-center">2</td>
                                    <td>
                                        <div>計畫書</div>
                                    </td>
                                    <td class="text-center">
                                        <span class="text-success">已上傳</span>
                                    </td>
                                    <td>
                                        <button class="btn btn-sm btn-outline-teal" onclick="alert('下載計畫書功能 (靜態展示)')">
                                            <i class="fas fa-download me-1"></i>
                                            CLB1140009_計畫書.pdf
                                        </button>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="text-center">3</td>
                                    <td>
                                        <div>未違反公職人員利益衝突迴避法切結書及事前揭露表</div>
                                    </td>
                                    <td class="text-center">
                                        <span class="text-success">已上傳</span>
                                    </td>
                                    <td>
                                        <button class="btn btn-sm btn-outline-teal" onclick="alert('下載切結書功能 (靜態展示)')">
                                            <i class="fas fa-download me-1"></i>
                                            CLB1140009_切結書.pdf
                                        </button>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="text-center">4</td>
                                    <td>
                                        <div>相關佐證資料</div>
                                    </td>
                                    <td class="text-center">
                                        <span class="text-muted">未上傳</span>
                                    </td>
                                    <td>
                                        <span class="text-muted">無檔案</span>
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