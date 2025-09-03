<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Attachment.aspx.cs" Inherits="OFS_CulAttachment" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon03.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫申請</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <culture-progress-bar :id="id" :step="5"></culture-progress-bar>
        <culture-attachment :id="id" v-on:next="next"></culture-attachment>
    </div>
    <script>
        setupVueApp({
            setup() {
                const id = "<%= Request.QueryString["ID"] %>";

                const next = () => {};

                return { id, next };
            }
        });
    </script>
</asp:Content>
