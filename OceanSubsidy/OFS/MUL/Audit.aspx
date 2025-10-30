<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Audit.aspx.cs" Inherits="OFS_MulAudit" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="BackUrl" runat="server">
    <a class="text-teal-dark text-decoration-none" href="../inprogressList.aspx">
        <i class="fas fa-angle-left"></i>
        返回列表
    </a>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon05.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫執行</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <project-title :id="id" type="multiple"></project-title>
        <project-progress :excludes="[4]" :id="id" :step="1" type="multiple"></project-progress>
        <project-toolbar :id="id" type="multiple"></project-toolbar>
        <multiple-progress-bar class="scroll-sticky-top rounded-top-0" @click="change" :id="id" review :step="current" style="top:270px"></multiple-progress-bar>
        <multiple-application :id="id" @next="current++" v-if="current === 1"></multiple-application>
        <multiple-work-schedule :id="id" @next="current++" v-else-if="current === 2"></multiple-work-schedule>
        <multiple-funding :id="id" @next="current++" v-else-if="current === 3"></multiple-funding>
        <multiple-benefit :id="id" @next="current++" v-else-if="current === 4"></multiple-benefit>
        <multiple-attachment :id="id" @next="next" v-else-if="current === 5"></multiple-attachment>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="FloatContent" runat="server">
    <project-change-review :id="id" type="multiple" v-if="store.multiple.changeStatus === 2"></project-change-review>
    <script>
        setupVueApp({
            setup() {
                const { onMounted, ref } = Vue;
                const { useProgressStore } = OceanSubsidyComponents;

                const current = ref(1);
                const id = "<%= Request.QueryString["ID"] %>";
                const store = useProgressStore();

                const change = (step) => current.value = step;

                const next = () => {};

                onMounted(initScrollListener);

                return { change, current, id, next, store };
            }
        });
    </script>
</asp:Content>
