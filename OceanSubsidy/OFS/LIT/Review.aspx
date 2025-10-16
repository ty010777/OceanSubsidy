<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Review.aspx.cs" Inherits="OFS_LitReview" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

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
                <project-organizer :id="id" type="literacy"></project-organizer>
                <a class="btn btn-teal-dark" download :href="`../../Service/OFS/DownloadPdf.ashx?Type=LIT&ProjectID=${id}&Version=1`">
                    <i class="fa-solid fa-download"></i>
                    下載計劃書
                </a>
            </div>
        </div>
        <literacy-progress-bar class="scroll-sticky-top" @click="change" :id="id" review :step="current"></literacy-progress-bar>
        <literacy-application :id="id" v-if="current === 1"></literacy-application>
        <literacy-work-schedule :id="id" v-else-if="current === 2"></literacy-work-schedule>
        <literacy-funding :id="id" v-else-if="current === 3"></literacy-funding>
        <literacy-benefit :id="id" v-else-if="current === 4"></literacy-benefit>
        <literacy-attachment :id="id" v-else-if="current === 5"></literacy-attachment>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="FloatContent" runat="server">
    <project-review :id="id" type="literacy" v-if="store.literacy.status === 11"></project-review>
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
