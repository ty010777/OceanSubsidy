<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Payment.aspx.cs" Inherits="OFS_EdcPayment" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon05.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫執行</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <project-title :id="id" type="education"></project-title>
        <project-progress :excludes="[4]" :id="id" :step="6" type="education"></project-progress>
        <education-payment :id="id" v-on:next="next"></education-payment>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="FloatContent" runat="server">
    <script>
        setupVueApp({
            setup() {
                const { onMounted, ref } = Vue;
                const { useProgressStore } = OceanSubsidyComponents;

                const id = "<%= Request.QueryString["ID"] %>";
                const store = useProgressStore();

                const next = () => {};

                onMounted(initScrollListener);

                return { id, next, store };
            }
        });
    </script>
</asp:Content>
