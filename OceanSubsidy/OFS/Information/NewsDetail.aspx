<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NewsDetail.aspx.cs" Inherits="Information_NewsDetail" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon02.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">資訊公告欄</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/@fancyapps/ui@6.0/dist/fancybox/fancybox.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/@fancyapps/ui@6.0/dist/fancybox/fancybox.umd.js"></script>
    <news-detail :id="id"></news-detail>
    <script>
        setupVueApp({
            setup() {
                const id = "<%= Request.QueryString["ID"] %>";

                return { id };
            }
        });
    </script>
</asp:Content>
