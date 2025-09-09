<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Audit.aspx.cs" Inherits="OFS_AccAudit" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon05.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫執行</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <project-title :id="id" type="accessibility"></project-title>
        <project-progress :excludes="[4]" :id="id" :step="1" type="accessibility"></project-progress>
        <project-toolbar :id="id" type="accessibility"></project-toolbar>
        <accessibility-progress-bar class="scroll-sticky-top rounded-top-0" @click="change" :id="id" review :step="current" style="top:270px"></accessibility-progress-bar>
        <accessibility-application :id="id" @next="current++" v-if="current === 1"></accessibility-application>
        <accessibility-work-schedule :id="id" @next="current++" v-else-if="current === 2"></accessibility-work-schedule>
        <accessibility-funding :id="id" @next="current++" v-else-if="current === 3"></accessibility-funding>
        <accessibility-other :id="id" @next="current++" v-else-if="current === 4"></accessibility-other>
        <accessibility-attachment :id="id" @next="next" v-else-if="current === 5"></accessibility-attachment>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="FloatContent" runat="server">
    <project-change-review :id="id" type="accessibility" v-if="store.accessibility.changeStatus === 2"></project-change-review>
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
