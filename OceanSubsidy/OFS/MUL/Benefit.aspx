<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Benefit.aspx.cs" Inherits="OFS_MulBenefit" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon03.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫申請</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <multiple-progress-bar :id="<%= Request.QueryString["ID"] %>" :step="4"></multiple-progress-bar>
        <multiple-benefit :id="<%= Request.QueryString["ID"] %>"></multiple-benefit>
    </div>
</asp:Content>
