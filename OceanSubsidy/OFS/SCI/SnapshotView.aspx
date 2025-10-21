<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SnapshotView.aspx.cs" Inherits="OFS_SCI_SnapshotView" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFSMaster.master" EnableViewState="true" %>
<%@ Register TagPrefix="uc" TagName="SciApplicationControl" Src="~/OFS/SCI/UserControls/SciApplicationControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="SciWorkSchControl" Src="~/OFS/SCI/UserControls/SciWorkSchControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="SciFundingControl" Src="~/OFS/SCI/UserControls/SciFundingControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="SciRecusedListControl" Src="~/OFS/SCI/UserControls/SciRecusedListControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="SciUploadAttachmentsControl" Src="~/OFS/SCI/UserControls/SciUploadAttachmentsControl.ascx" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    快照檢視 - 海洋領域補助計畫管理資訊系統
</asp:Content>

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
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SnapshotView.js") %>"></script>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />

    <!-- 頁面標題 -->
    <div class="d-flex justify-content-between mb-4">
        <div class="page-title">
            <img src="<%= ResolveUrl("~/assets/img/information-system-title-icon05.svg") %>" alt="logo">
            <div>
                <span>目前位置</span>
                <div class="d-flex align-items-end gap-3">
                    <h2 class="text-teal-dark">快照檢視</h2>
                    <a class="text-teal-dark text-decoration-none" href="javascript:history.back()">
                        <i class="fas fa-angle-left"></i>
                        返回
                    </a>
                </div>
            </div>
        </div>
    </div>

    <!-- 快照資訊 -->
    <div class="top-wrapper mb-4">
        <div class="top-block">
            <div class="d-flex align-items-center gap-3">
                <span>［<asp:Literal ID="litGrantType" runat="server" Text="科專" />］<asp:Literal ID="litProjectInfo" runat="server" Text="" /></span>
                <span class="badge bg-secondary">快照時間：<asp:Literal ID="litSnapshotTime" runat="server" /></span>
            </div>
        </div>
    </div>

    <!-- 用於儲存當前頁簽狀態 -->
    <asp:HiddenField ID="hdnCurrentStep" runat="server" Value="1" ClientIDMode="Static" />

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
</asp:Content>
