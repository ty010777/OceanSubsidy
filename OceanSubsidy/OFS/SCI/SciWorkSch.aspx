<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SciWorkSch.aspx.cs" Inherits="OFS_SciWorkSch" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/SCI/OFSApplicationMaster.master" %>
<%@ Register Src="~/OFS/SCI/UserControls/SciWorkSchControl.ascx" TagName="SciWorkSchControl" TagPrefix="uc" %>
<%@ Register TagPrefix="uc" TagName="ChangeDescriptionControl" Src="~/OFS/SCI/UserControls/ChangeDescriptionControl.ascx" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <!-- SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciWorkSch.js") %>"></script>
</asp:Content>

<asp:Content ID="ApplicationContent" ContentPlaceHolderID="ApplicationContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
    
    <!-- 使用 UserControl -->
    <div class="tab-pane" id="tab2">
        <uc:SciWorkSchControl ID="sciWorkSchControl" runat="server" />
    </div>
</asp:Content>