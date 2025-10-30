<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ApprovedList.aspx.cs" Inherits="Report_ApprovedList" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="BackUrl" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon06.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">報表查詢 / 核定計畫報表</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <report-approved-list :category="category"></report-approved-list>
    <script>
        setupVueApp({
            setup() {
                const category = "<%= Request.QueryString["category"] %>";

                return { category };
            }
        });
    </script>
</asp:Content>
