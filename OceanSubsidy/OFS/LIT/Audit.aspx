<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Audit.aspx.cs" Inherits="OFS_LitAudit" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon05.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫執行</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <project-title :id="id" type="literacy"></project-title>
        <project-progress :id="id" :step="1" type="literacy"></project-progress>
        <project-toolbar :id="id" type="literacy"></project-toolbar>
        <literacy-progress-bar class="scroll-sticky-top rounded-top-0" @click="change" :id="id" review :step="current" style="top:270px"></literacy-progress-bar>
        <literacy-application :id="id" @next="current++" v-if="current === 1"></literacy-application>
        <literacy-work-schedule :id="id" @next="current++" v-else-if="current === 2"></literacy-work-schedule>
        <literacy-funding :id="id" @next="current++" v-else-if="current === 3"></literacy-funding>
        <literacy-other :id="id" @next="current++" v-else-if="current === 4"></literacy-other>
        <literacy-attachment :id="id" @next="next" v-else-if="current === 5"></literacy-attachment>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="FloatContent" runat="server">
    <project-change-review :id="id" type="literacy" v-if="store.literacy.changeStatus === 2"></project-change-review>
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
