<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SciRecusedList.aspx.cs" Inherits="OFS_SciAvoidList" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/SCI/OFSApplicationMaster.master" %>
<%@ Register Src="~/OFS/SCI/UserControls/SciRecusedListControl.ascx" TagPrefix="uc" TagName="SciRecusedListControl" %>
<%@ Register TagPrefix="uc" TagName="ChangeDescriptionControl" Src="~/OFS/SCI/UserControls/ChangeDescriptionControl.ascx" %>

<asp:Content ID="ApplicationTitle" ContentPlaceHolderID="ApplicationTitle" runat="server">
    其他 - 海洋領域補助計畫管理資訊系統
</asp:Content>

<asp:Content ID="HeadExtra" ContentPlaceHolderID="HeadExtra" runat="server">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="<%=ResolveUrl("~/script/OFS/SCI/SciRecusedList.js")%>"></script>
</asp:Content>

<asp:Content ID="ApplicationContent" ContentPlaceHolderID="ApplicationContent" runat="server">
    <!-- 使用 UserControl -->
    <div class="tab-pane" id="tab4">
    <uc:SciRecusedListControl ID="sciRecusedListControl" runat="server" />
    </div>

</asp:Content>