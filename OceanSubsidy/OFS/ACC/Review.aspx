<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Review.aspx.cs" Inherits="OFS_AccReview" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="BackUrl" runat="server">
    <a class="text-teal-dark text-decoration-none" href="../ReviewChecklist.aspx">
        <i class="fas fa-angle-left"></i>
        返回列表
    </a>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon04.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫審查</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <div class="top-wrapper">
            <div class="top-block">
                <project-organizer :id="id" type="accessibility"></project-organizer>
                <a class="btn btn-teal-dark" download :href="`../../Service/OFS/DownloadPdf.ashx?Type=ACC&ProjectID=${id}&Version=1`">
                    <i class="fa-solid fa-download"></i>
                    下載計劃書
                </a>
            </div>
        </div>
        <accessibility-progress-bar class="scroll-sticky-top" @click="change" :id="id" review :step="current"></accessibility-progress-bar>
        <accessibility-application :id="id" v-if="current === 1"></accessibility-application>
        <accessibility-work-schedule :id="id" v-else-if="current === 2"></accessibility-work-schedule>
        <accessibility-funding :id="id" v-else-if="current === 3"></accessibility-funding>
        <accessibility-benefit :id="id" v-else-if="current === 4"></accessibility-benefit>
        <accessibility-attachment :id="id" v-else-if="current === 5"></accessibility-attachment>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="FloatContent" runat="server">
    <project-review :id="id" type="accessibility" v-if="store.accessibility.status === 11"></project-review>
    <project-correction-review :id="id" type="accessibility" v-else-if="store.accessibility.status === 43"></project-correction-review>
    <script>
        setupVueApp({
            setup() {
                const { onMounted, ref } = Vue;
                const { useProgressStore } = OceanSubsidyComponents;

                const current = ref(1);
                const id = "<%= Request.QueryString["ID"] %>";
                const store = useProgressStore();

                const change = (step) => current.value = step;

                onMounted(initScrollListener);

                return { change, current, id, store };
            }
        });
    </script>
</asp:Content>
