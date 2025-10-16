<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ApplyList.aspx.cs" Inherits="Report_ApplyList" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="BackUrl" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon06.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">報表查詢 / 申請計畫報表</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <report-apply-list></report-apply-list>
</asp:Content>
