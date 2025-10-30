<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReviewerList.aspx.cs" Inherits="Report_ReviewerList" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="BackUrl" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon06.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">報表查詢 / 審查委員資料</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <report-reviewer-list></report-reviewer-list>
</asp:Content>
