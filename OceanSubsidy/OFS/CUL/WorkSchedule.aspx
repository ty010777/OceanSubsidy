<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WorkSchedule.aspx.cs" Inherits="OFS_CulWorkSchedule" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫申請</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <culture-progress-bar :id="<%= Request.QueryString["ID"] %>" :step="2"></culture-progress-bar>
        <culture-work-schedule :id="<%= Request.QueryString["ID"] %>"></culture-work-schedule>
    </div>
    <script>
        startVueApp(".mis-content");
    </script>
</asp:Content>
