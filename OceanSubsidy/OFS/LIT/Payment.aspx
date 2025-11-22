<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Payment.aspx.cs" Inherits="OFS_LitPayment" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

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
        <project-title :id="id" type="literacy"></project-title>
        <project-progress :excludes="[4]" :id="id" :step="6" type="literacy"></project-progress>
        <div class="block rounded-top-4 pb-0">
            <ul class="teal-dark-tabs mb-0">
                <li :class="{ active: stage === 1, disabled: false }">
                    <a class="tab-link" href="javascript:void(0)" v-on:click="stage = 1">{{ settings[0].PhaseName }}</a>
                </li>
                <li :class="{ active: stage === 2, disabled: false }">
                    <a class="tab-link" href="javascript:void(0)" v-on:click="stage = 2">{{ settings[1].PhaseName }}</a>
                </li>
            </ul>
        </div>
        <literacy-payment1 :id="id" :setting="settings[0]" v-on:next="next" v-if="stage === 1"></literacy-payment1>
        <literacy-payment2 :id="id" :setting="settings[1]" v-on:next="next" v-else-if="stage === 2"></literacy-payment2>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="FloatContent" runat="server">
    <project-payment-review :id="id" type="literacy"></project-payment-review>
    <script>
        setupVueApp({
            setup() {
                const { onMounted, ref } = Vue;
                const { api, useProgressStore } = OceanSubsidyComponents;

                const id = "<%= Request.QueryString["ID"] %>";
                const settings = ref();
                const stage = ref(parseInt("<%= Request.QueryString["Stage"] %>") || 1);
                const store = useProgressStore();

                const next = (callback) => Swal.fire({ title: "提送成功", icon: "success" }).then(callback);

                onMounted(() => {
                    api.literacy("getPaymentPhaseSettings", { TypeCode: "LIT" }).subscribe((res) => settings.value = res);

                    initScrollListener();
                });

                return { id, next, settings, stage, store };
            }
        });
    </script>
</asp:Content>
