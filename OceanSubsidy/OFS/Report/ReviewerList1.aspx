<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReviewerList1.aspx.cs" Inherits="Report_ReviewerList1" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="BackUrl" runat="server">
    <a class="text-teal-dark text-decoration-none" href="ReviewerList.aspx">
        <i class="fas fa-angle-left"></i>
        返回列表
    </a>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon06.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">報表查詢 / 審查委員名單</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <report-reviewer-details :id="id" type="1"></report-reviewer-details>
    <script>
        setupVueApp({
            setup() {
                const id = "<%= Request.QueryString["ID"] %>";

                return { id };
            }
        });
    </script>
</asp:Content>
