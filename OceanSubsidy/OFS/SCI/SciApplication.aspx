<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SciApplication.aspx.cs" Inherits="OFS_SciApplication" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/SCI/OFSApplicationMaster.master" %>
<%@ Register TagPrefix="uc" TagName="SciApplicationControl" Src="~/OFS/SCI/UserControls/SciApplicationControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="ChangeDescriptionControl" Src="~/OFS/SCI/UserControls/ChangeDescriptionControl.ascx" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciApplication.js") %>"></script>
</asp:Content>

<asp:Content ID="ApplicationContent" ContentPlaceHolderID="ApplicationContent" runat="server">
    <!-- 使用 UserControl -->
    <uc:SciApplicationControl ID="ucSciApplication" runat="server" />
    
    <!-- 變更說明 UserControl -->
    <uc:ChangeDescriptionControl ID="ucChangeDescription" runat="server" SourcePage="SciApplication" />
                  
                  <!-- 底部區塊 -->
                  <div class="block-bottom bg-light-teal">
                      
                       <asp:Button ID="btnTempSave" runat="server"  
                                 Text="暫存"  
                                 CssClass="btn btn-outline-teal"  
                                 OnClick="btnSave_Click" /> 
                      
                       <asp:Button ID="btnSubmit" runat="server" 
                                 Text="完成本頁，下一步"  
                                 CssClass="btn btn-teal"  
                                 OnClick="btnSave_Click" /> 
                  </div>
</asp:Content>



   