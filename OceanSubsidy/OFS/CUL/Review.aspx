<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Review.aspx.cs" Inherits="OFS_CulReview" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon04.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫審查</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <div class="top-wrapper">
            <div class="top-block">
                <project-organizer :id="id" type="culture"></project-organizer>
                <button class="btn btn-teal-dark" type="button">
                    <i class="fa-solid fa-download"></i>
                    下載計劃書
                </button>
            </div>
        </div>
        <culture-progress-bar class="scroll-sticky-top" @click="change" :id="id" review :step="current"></culture-progress-bar>
        <culture-application :id="id" v-if="current === 1"></culture-application>
        <culture-work-schedule :id="id" v-else-if="current === 2"></culture-work-schedule>
        <culture-funding :id="id" v-else-if="current === 3"></culture-funding>
        <culture-other :id="id" v-else-if="current === 4"></culture-other>
        <culture-attachment :id="id" v-else-if="current === 5"></culture-attachment>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="FloatContent" runat="server">
    <project-review :id="id" type="culture"></project-review>
    <script>
        setupVueApp({
            setup() {
                const { onMounted, ref } = Vue;

                const current = ref(1);
                const id = <%= Request.QueryString["ID"] %>;

                const change = (step) => current.value = step;

                onMounted(initScrollListener);

                return { change, current, id };
            }
        });
    </script>
</asp:Content>
