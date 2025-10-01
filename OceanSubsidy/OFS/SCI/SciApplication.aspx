<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SciApplication.aspx.cs" Inherits="OFS_SciApplication" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/SCI/OFSApplicationMaster.master" %>
<%@ Register TagPrefix="uc" TagName="SciApplicationControl" Src="~/OFS/SCI/UserControls/SciApplicationControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="ChangeDescriptionControl" Src="~/OFS/SCI/UserControls/ChangeDescriptionControl.ascx" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciApplication.js") %>"></script>
</asp:Content>

<asp:Content ID="ApplicationContent" ContentPlaceHolderID="ApplicationContent" runat="server">
    <!-- 使用 UserControl -->
    <div class="tab-pane active" id="tab1">
        <uc:SciApplicationControl ID="ucSciApplication" runat="server" />
    </div>
</asp:Content>



   