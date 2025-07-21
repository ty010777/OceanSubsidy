<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SciWorkSch.aspx.cs" Inherits="OFS_SciWorkSch" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/SCI/OFSApplicationMaster.master" %>
<%@ Register Src="~/OFS/SCI/UserControls/SciWorkSchControl.ascx" TagName="SciWorkSchControl" TagPrefix="uc" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <!-- SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciWorkSch.js") %>"></script>
</asp:Content>

<asp:Content ID="ApplicationContent" ContentPlaceHolderID="ApplicationContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
    
    <!-- 使用 UserControl -->
    <uc:SciWorkSchControl ID="sciWorkSchControl" runat="server" />
                  
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
    </div>
</asp:Content>