<%@ Page Language="C#" AutoEventWireup="true" CodeFile="~/OFS/SCI/SciFunding.aspx.cs" Inherits="OFS_SciFunding" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/SCI/OFSApplicationMaster.master" %>
<%@ Register Src="~/OFS/SCI/UserControls/SciFundingControl.ascx" TagName="SciFundingControl" TagPrefix="uc" %>
<%@ Register TagPrefix="uc" TagName="ChangeDescriptionControl" Src="~/OFS/SCI/UserControls/ChangeDescriptionControl.ascx" %>

<asp:Content ID="ApplicationTitle" ContentPlaceHolderID="ApplicationTitle" runat="server">
    計畫申請 - 經費/人事 - 海洋領域補助計畫管理資訊系統
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <!-- Moment.js and Taiwan date picker -->
    <script src="<%= ResolveUrl("~/assets/vendor/moment.min.js") %>"></script>
    <script src="<%= ResolveUrl("~/assets/vendor/locale/zh-tw.js") %>"></script>
    <script src="<%= ResolveUrl("~/assets/vendor/moment-taiwan.js") %>"></script>
    <script src="<%= ResolveUrl("~/assets/vendor/daterangepicker.js") %>"></script>
    <link rel="stylesheet" href="<%= ResolveUrl("~/assets/vendor/daterangepicker.css") %>" />

    <!-- 自訂JavaScript -->
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciFunding.js") %>"></script>
</asp:Content>

<asp:Content ID="ApplicationContent" ContentPlaceHolderID="ApplicationContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
    
    <!-- 使用 UserControl -->
    <div class="tab-pane" id="tab3">
        <uc:SciFundingControl ID="sciFundingControl" runat="server" />
    </div>

</asp:Content>