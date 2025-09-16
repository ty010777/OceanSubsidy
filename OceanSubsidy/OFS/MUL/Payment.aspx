<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Payment.aspx.cs" Inherits="OFS_MulPayment" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon05.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫執行</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div v-if="settings">
        <project-title :id="id" type="multiple"></project-title>
        <project-progress :excludes="[4]" :id="id" :step="6" type="multiple"></project-progress>
        <div class="block rounded-top-4 pb-0">
            <ul class="teal-dark-tabs mb-0">
                <li :class="{ active: stage === 1, disabled: false }">
                    <a class="tab-link" href="javascript:void(0)" v-on:click="stage = 1">{{ settings[0].PhaseName }}</a>
                </li>
                <li :class="{ active: stage === 2, disabled: false }">
                    <a class="tab-link" href="javascript:void(0)" v-on:click="stage = 2">{{ settings[1].PhaseName }}</a>
                </li>
                <li :class="{ active: stage === 3, disabled: false }">
                    <a class="tab-link" href="javascript:void(0)" v-on:click="stage = 3">{{ settings[2].PhaseName }}</a>
                </li>
            </ul>
        </div>
        <multiple-payment1 :id="id" :setting="settings[0]" v-on:next="next" v-if="stage === 1"></multiple-payment1>
        <multiple-payment2 :id="id" :setting="settings[1]" v-on:next="next" v-else-if="stage === 2"></multiple-payment2>
        <multiple-payment3 :id="id" :setting="settings[2]" v-on:next="next" v-else-if="stage === 3"></multiple-payment3>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="FloatContent" runat="server">
    <project-payment-review type="multiple"></project-payment-review>
    <script>
        setupVueApp({
            setup() {
                const { onMounted, ref } = Vue;
                const { api, useProgressStore } = OceanSubsidyComponents;

                const id = "<%= Request.QueryString["ID"] %>";
                const settings = ref();
                const stage = ref(1);
                const store = useProgressStore();

                const next = () => {};

                onMounted(() => {
                    api.multiple("getPaymentPhaseSettings", { TypeCode: "MUL" }).subscribe((res) => settings.value = res);

                    initScrollListener();
                });

                return { id, next, settings, stage, store };
            }
        });
    </script>
</asp:Content>
