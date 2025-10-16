<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Payment.aspx.cs" Inherits="OFS_EdcPayment" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="BackUrl" runat="server">
    <a class="text-teal-dark text-decoration-none" href="../inprogressList.aspx">
        <i class="fas fa-angle-left"></i>
        返回列表
    </a>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon05.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫執行</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div v-if="settings">
        <project-title :id="id" type="education"></project-title>
        <project-progress :excludes="[4]" :id="id" :step="6" type="education"></project-progress>
        <education-payment :id="id" :setting="settings[0]" v-on:next="next"></education-payment>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="FloatContent" runat="server">
    <project-payment-review :id="id" type="education"></project-payment-review>
    <script>
        setupVueApp({
            setup() {
                const { onMounted, ref } = Vue;
                const { api, useProgressStore } = OceanSubsidyComponents;

                const id = "<%= Request.QueryString["ID"] %>";
                const settings = ref();
                const store = useProgressStore();

                const next = () => {};

                onMounted(() => {
                    api.education("getPaymentPhaseSettings", { TypeCode: "EDC" }).subscribe((res) => settings.value = res);

                    initScrollListener();
                });

                return { id, next, settings, store };
            }
        });
    </script>
</asp:Content>
