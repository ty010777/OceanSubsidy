<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WorkSchedule.aspx.cs" Inherits="OFS_LitWorkSchedule" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon03.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫申請</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <literacy-progress-bar :id="<%= Request.QueryString["ID"] %>" :step="2"></literacy-progress-bar>
        <literacy-work-schedule :id="<%= Request.QueryString["ID"] %>"></literacy-work-schedule>
    </div>
</asp:Content>
