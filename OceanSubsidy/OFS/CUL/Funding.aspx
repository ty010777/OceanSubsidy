<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Funding.aspx.cs" Inherits="OFS_CulFunding" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon03.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫申請</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <culture-progress-bar :id="id" :step="3"></culture-progress-bar>
        <culture-funding :id="id" v-on:next="next"></culture-funding>
    </div>
    <script>
        setupVueApp({
            setup() {
                const id = "<%= Request.QueryString["ID"] %>";

                const next = () => window.location.href = `Other.aspx?ID=${id}`;

                return { id, next };
            }
        });
    </script>
</asp:Content>
