<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AuditHistory.aspx.cs" Inherits="OFSAuditHistory" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="BackUrl" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon03.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">查核紀錄檢視</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <audit-history :name="name"></audit-history>
    <script>
        setupVueApp({
            setup() {
                const name = "<%= Request.QueryString["Name"] %>";

                return { name };
            }
        });
    </script>
</asp:Content>
