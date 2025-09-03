<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Review.aspx.cs" Inherits="OFS_LitReview" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon04.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫審查</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <div class="top-wrapper">
            <div class="top-block">
                <project-organizer :id="id" type="literacy"></project-organizer>
                <button class="btn btn-teal-dark" type="button">
                    <i class="fa-solid fa-download"></i>
                    下載計劃書
                </button>
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
    <project-review :id="id" type="literacy" v-if="store.literacy.status === 2"></project-review>
    <script>
        setupVueApp({
            setup() {
                const { onMounted, ref } = Vue;

                const current = ref(1);
                const id = "<%= Request.QueryString["ID"] %>";

                const change = (step) => current.value = step;

                onMounted(initScrollListener);

                return { change, current, id };
            }
        });
    </script>
</asp:Content>
