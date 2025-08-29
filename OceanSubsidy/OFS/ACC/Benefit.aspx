<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Benefit.aspx.cs" Inherits="OFS_AccBenefit" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon03.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫申請</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <accessibility-progress-bar :id="<%= Request.QueryString["ID"] %>" :step="4"></accessibility-progress-bar>
        <accessibility-benefit :id="<%= Request.QueryString["ID"] %>"></accessibility-benefit>
    </div>
</asp:Content>
