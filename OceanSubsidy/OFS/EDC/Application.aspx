<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Application.aspx.cs" Inherits="OFS_EdcApplication" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon03.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫申請</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <education-progress-bar :id="id" :step="1"></education-progress-bar>
        <education-application apply :id="id" v-on:next="next"></education-application>
    </div>
    <script>
        setupVueApp({
            setup() {
                const id = "<%= Request.QueryString["ID"] %>";

                const next = (projectId) => window.location.href = `Attachment.aspx?ID=${projectId}`;

                return { id, next };
            }
        });
    </script>
</asp:Content>
