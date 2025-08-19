<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Attachment.aspx.cs" Inherits="OFS_CulAttachment" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫申請</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <culture-progress-bar :id="<%= Request.QueryString["ID"] %>" :step="5"></culture-progress-bar>
        <culture-attachment :id="<%= Request.QueryString["ID"] %>"></culture-attachment>
    </div>
    <script>
        startVueApp(".mis-content");
    </script>
</asp:Content>
