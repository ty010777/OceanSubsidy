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
    <uc:SciRecusedListControl ID="sciRecusedListControl" runat="server" />
    
    <!-- 變更說明 UserControl -->
    <uc:ChangeDescriptionControl ID="ucChangeDescription" runat="server" SourcePage="SciRecusedList" />
    
    <!-- 底部區塊 -->
    <div class="block-bottom bg-light-teal">
        <asp:Button ID="btnTempSave" runat="server" Text="暫存" CssClass="btn btn-outline-teal" OnClick="btnSave_Click" />
        <asp:Button ID="btnNext" runat="server" Text="完成本頁，下一步" CssClass="btn btn-teal" OnClick="btnNext_Click" />
    </div>
</asp:Content>