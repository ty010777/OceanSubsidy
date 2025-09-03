<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Other.aspx.cs" Inherits="OFS_CulOther" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon03.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫申請</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <culture-progress-bar :id="id" :step="4"></culture-progress-bar>
        <culture-other :id="id" v-on:next="next"></culture-other>
    </div>
    <script>
        setupVueApp({
            setup() {
                const id = "<%= Request.QueryString["ID"] %>";

                const next = () => window.location.href = `Attachment.aspx?ID=${id}`;

                return { id, next };
            }
        });
    </script>
</asp:Content>
