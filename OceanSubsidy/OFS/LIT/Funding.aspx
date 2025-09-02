<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Funding.aspx.cs" Inherits="OFS_LitFunding" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon03.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫申請</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <literacy-progress-bar :id="id" :step="3"></literacy-progress-bar>
        <literacy-funding :id="id" v-on:next="next"></literacy-funding>
    </div>
    <script>
        setupVueApp({
            setup() {
                const id = "<%= Request.QueryString["ID"] %>";

                const next = () => window.location.href = `Benefit.aspx?ID=${id}`;

                return { id, next };
            }
        });
    </script>
</asp:Content>
