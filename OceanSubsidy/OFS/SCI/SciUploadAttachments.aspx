<%@ Page Title="" Language="C#" MasterPageFile="~/OFS/SCI/OFSApplicationMaster.master" AutoEventWireup="true" CodeFile="SciUploadAttachments.aspx.cs" Inherits="OFS_SCI_SciUploadAttachments" %>
<%@ Register TagPrefix="uc" TagName="SciUploadAttachmentsControl" Src="~/OFS/SCI/UserControls/SciUploadAttachmentsControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ApplicationTitle" runat="server">
    上傳附件 - 海洋科技專案計畫申請
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadExtra" runat="server">
    <!-- SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <!-- 頁面特定的 JavaScript -->
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciUploadAttachments.js") %>"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ApplicationContent" runat="server">
    <!-- 上傳附件 UserControl -->
    <uc:SciUploadAttachmentsControl ID="ucSciUploadAttachments" runat="server" />
    
    <!-- 底部區塊 -->
    <div class="block-bottom bg-light-teal">
        <asp:Button ID="btnSave" runat="server" 
            CssClass="btn btn-outline-teal" 
            Text="暫存" OnClick="btnSave_Click" />
        <asp:Button ID="btnSubmit" runat="server" 
            CssClass="btn btn-teal" 
            Text="✓ 全部完成，提送申請" />
        
        <!-- 隱藏的確認提送按鈕 -->
        <asp:Button ID="btnSubmitConfirmed" runat="server" 
            OnClick="btnSubmitConfirmed_Click" 
            Style="display: none;" />
    </div>
</asp:Content>