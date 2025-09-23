<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ClbApplication.aspx.cs" Inherits="OFS_CLB_ClbApplication" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFSMaster.master" %>
<%@ Register TagPrefix="uc" TagName="ClbApplicationControl" Src="~/OFS/CLB/UserControls/ClbApplicationControl.ascx" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <!-- SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <script src="<%= ResolveUrl("~/script/OFS/CLB/ClbApplication.js") %>"></script>

    <style>
        /* Bootstrap disabled 樣式擴展 */
        .application-step .step-item.disabled {
            opacity: 0.65;
            pointer-events: none;
            cursor: default;
        }

        .application-step .step-item.disabled .step-label,
        .application-step .step-item.disabled .step-status {
            color: #6c757d !important;
        }

        /* 確保無障礙性 */
        .application-step .step-item[aria-disabled="true"] {
            cursor: not-allowed;
        }
    </style>

</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- 頁面標題 -->
    <div class="d-flex justify-content-between mb-4">
        <div class="page-title">
            <img src="<%= ResolveUrl("~/assets/img/information-system-title-icon03.svg") %>" alt="logo">
            <div>
                <span>目前位置</span>
                <div class="d-flex align-items-end gap-3">
                    <h2 class="text-dark-green2">計畫申請</h2>
                    <a class="text-dark-green2 text-decoration-none" href="<%= ResolveUrl("~/OFS/ApplicationChecklist.aspx") %>" >
                        <i class="fas fa-angle-left"></i>
                        返回列表
                    </a>
                </div>
            </div>
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
    <!-- Hidden field to store ProjectID -->
    <asp:HiddenField ID="hdnProjectID" runat="server" />

    <!-- Hidden field to store current step index -->
    <asp:HiddenField ID="hdnStepIndex" runat="server" Value="0" />

    <!-- 使用 UserControl -->
    <uc:ClbApplicationControl ID="ucClbApplication" runat="server" />

    <!-- 底部區塊 -->
    <div class="block-bottom bg-light-teal">
        <asp:Button ID="btnTempSave" runat="server"
                    Text="暫存"
                    CssClass="btn btn-outline-teal"
                    OnClick="btnTempSave_Click" />

        <asp:Button ID="btnSaveAndNext" runat="server"
                    Text="完成本頁，下一步"
                    CssClass="btn btn-teal"
                    OnClick="btnSaveAndNext_Click" />

        <asp:Button ID="btnSubmitApplication" runat="server"
                    Text="完成本頁，提送申請"
                    CssClass="btn btn-teal"
                    Style="display: none;"
                    OnClick="btnSubmitApplication_Click" />
    </div>

</asp:Content>
