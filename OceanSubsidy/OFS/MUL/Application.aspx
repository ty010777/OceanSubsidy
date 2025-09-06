<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Application.aspx.cs" Inherits="OFS_MulApplication" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon03.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫申請</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <multiple-progress-bar :id="id" :step="1"></multiple-progress-bar>
        <multiple-application :id="id" v-on:next="next"></multiple-application>
    </div>
    <script>
        setupVueApp({
            setup() {
                const id = "<%= Request.QueryString["ID"] %>";

                const next = (projectId) => window.location.href = `WorkSchedule.aspx?ID=${projectId}`;

                return { id, next };
            }
        });
    </script>
</asp:Content>
