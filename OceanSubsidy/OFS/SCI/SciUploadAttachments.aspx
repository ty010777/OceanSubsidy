<%@ Page Title="" Language="C#" MasterPageFile="~/OFS/SCI/OFSApplicationMaster.master" AutoEventWireup="true" CodeFile="SciUploadAttachments.aspx.cs" Inherits="OFS_SCI_SciUploadAttachments" %>
<%@ Register TagPrefix="uc" TagName="SciUploadAttachmentsControl" Src="~/OFS/SCI/UserControls/SciUploadAttachmentsControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="ChangeDescriptionControl" Src="~/OFS/SCI/UserControls/ChangeDescriptionControl.ascx" %>

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
    <div class="tab-pane" id="tab5">
    <uc:SciUploadAttachmentsControl ID="ucSciUploadAttachments" runat="server" />
    </div>
</asp:Content>