<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NewsList.aspx.cs" Inherits="Information_NewsList" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="BackUrl" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon02.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">資訊公告欄</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <news-list></news-list>
</asp:Content>
