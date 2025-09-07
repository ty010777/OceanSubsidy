<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Payment.aspx.cs" Inherits="OFS_MulPayment" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon05.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫執行</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <project-title :id="id" type="multiple"></project-title>
        <project-progress :excludes="[4]" :id="id" :step="6" type="multiple"></project-progress>
        <div class="block rounded-top-4 pb-0">
            <ul class="teal-dark-tabs mb-0">
                <li :class="{ active: stage === 1, disabled: false }">
                    <a class="tab-link" href="javascript:void(0)" v-on:click="stage = 1">第一期</a>
                </li>
                <li :class="{ active: stage === 2, disabled: false }">
                    <a class="tab-link" href="javascript:void(0)" v-on:click="stage = 2">第二期</a>
                </li>
                <li :class="{ active: stage === 3, disabled: false }">
                    <a class="tab-link" href="javascript:void(0)" v-on:click="stage = 3">結案核銷</a>
                </li>
            </ul>
        </div>
        <multiple-payment1 :id="id" v-on:next="next" v-if="stage === 1"></multiple-payment1>
        <multiple-payment2 :id="id" v-on:next="next" v-else-if="stage === 2"></multiple-payment2>
        <multiple-payment3 :id="id" v-on:next="next" v-else-if="stage === 3"></multiple-payment3>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="FloatContent" runat="server">
    <script>
        setupVueApp({
            setup() {
                const { onMounted, ref } = Vue;
                const { useProgressStore } = OceanSubsidyComponents;

                const id = "<%= Request.QueryString["ID"] %>";
                const stage = ref(1);
                const store = useProgressStore();

                const next = () => {};

                onMounted(initScrollListener);

                return { id, next, stage, store };
            }
        });
    </script>
</asp:Content>
