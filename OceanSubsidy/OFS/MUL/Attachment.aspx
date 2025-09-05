<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Attachment.aspx.cs" Inherits="OFS_MulAttachment" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon03.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫申請</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <multiple-progress-bar :id="id" :step="5"></multiple-progress-bar>
        <multiple-attachment :id="id" v-on:next="next"></multiple-attachment>
    </div>
    <script>
        setupVueApp({
            setup() {
                const id = "<%= Request.QueryString["ID"] %>";

                const next = () => window.location.href = "../ApplicationChecklist.aspx";

                return { id, next };
            }
        });
    </script>
</asp:Content>
