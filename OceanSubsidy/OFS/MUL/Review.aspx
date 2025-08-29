<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WorkSchedule.aspx.cs" Inherits="OFS_CulWorkSchedule" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon04.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫審查</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <div class="top-wrapper">
            <div class="top-block">
                <project-organizer :id="id" type="multiple"></project-organizer>
                <button class="btn btn-teal-dark" type="button">
                    <i class="fa-solid fa-download"></i>
                    下載計劃書
                </button>
            </div>
        </div>
        <multiple-progress-bar class="scroll-sticky-top" @click="change" :id="id" review :step="current"></multiple-progress-bar>
        <multiple-application :id="id" v-if="current === 1"></multiple-application>
        <multiple-work-schedule :id="id" v-else-if="current === 2"></multiple-work-schedule>
        <multiple-funding :id="id" v-else-if="current === 3"></multiple-funding>
        <multiple-benefit :id="id" v-else-if="current === 4"></multiple-benefit>
        <multiple-attachment :id="id" v-else-if="current === 5"></multiple-attachment>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="FloatContent" runat="server">
    <project-review :id="id" type="multiple"></project-review>
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
