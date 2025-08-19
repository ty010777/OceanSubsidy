<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Other.aspx.cs" Inherits="OFS_CulOther" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫申請</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <culture-progress-bar :id="<%= Request.QueryString["ID"] %>" :step="4"></culture-progress-bar>
        <culture-other :id="<%= Request.QueryString["ID"] %>"></culture-other>
    </div>
    <script>
        startVueApp(".mis-content");
    </script>
</asp:Content>
