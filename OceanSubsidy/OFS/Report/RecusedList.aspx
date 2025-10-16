<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RecusedList.aspx.cs" Inherits="Report_RecusedList" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="BackUrl" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon06.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">報表查詢 / 迴避審查委員名單</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <report-recused-list></report-recused-list>
</asp:Content>
