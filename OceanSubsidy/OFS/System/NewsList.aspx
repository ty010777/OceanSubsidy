<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NewsList.aspx.cs" Inherits="Admin_NewsList" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon07.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">系統管理 / 公告管理</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <system-news-list></system-news-list>
</asp:Content>
