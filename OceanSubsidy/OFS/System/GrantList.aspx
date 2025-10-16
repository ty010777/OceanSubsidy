<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GrantList.aspx.cs" Inherits="Admin_GrantList" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="BackUrl" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon07.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">系統管理 / 補助計畫管理</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <system-grant-list></system-grant-list>
</asp:Content>
