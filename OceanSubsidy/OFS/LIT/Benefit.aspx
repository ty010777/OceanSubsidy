<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Benefit.aspx.cs" Inherits="OFS_LitBenefit" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon03.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫申請</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <literacy-progress-bar :id="id" :step="4"></literacy-progress-bar>
        <literacy-benefit apply :id="id" v-on:next="next"></literacy-benefit>
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
