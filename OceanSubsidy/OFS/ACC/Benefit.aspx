<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Benefit.aspx.cs" Inherits="OFS_AccBenefit" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon03.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫申請</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <accessibility-progress-bar :id="id" :step="4"></accessibility-progress-bar>
        <accessibility-benefit apply :id="id" v-on:next="next"></accessibility-benefit>
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
