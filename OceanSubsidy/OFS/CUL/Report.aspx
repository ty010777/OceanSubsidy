<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report.aspx.cs" Inherits="OFS_CulReport" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon05.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">計畫執行</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <project-title :id="id" type="culture"></project-title>
        <project-progress :id="id" :step="5" type="culture"></project-progress>
        <div class="block rounded-top-4 pb-0">
            <ul class="teal-dark-tabs mb-0">
                <li :class="{ active: item.id === stage }" :key="item" v-for="(item) in stages">
                    <a class="tab-link" @click.prevent="stage = item.id" href="javascript:void(0)">{{ item.title }}</a>
                </li>
            </ul>
        </div>
        <template :key="item" v-for="(item) in stages">
            <project-report :data="item" :id="id" @next="next" v-if="item.id === stage"></project-report>
        </template>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="FloatContent" runat="server">
    <script>
        setupVueApp({
            setup() {
                const { onMounted, ref } = Vue;
                const { api, useProgressStore } = OceanSubsidyComponents;

                const current = ref(1);
                const id = "<%= Request.QueryString["ID"] %>";
                const stage = ref(1);
                const store = useProgressStore();

                const stages = [{
                    id: 1,
                    title: "期中報告",
                    subtitle: "期中報告審查",
                    description: "請下載報告書範本，填寫資料及公文用印後上傳。<br>期中報告書及相關檔案，請壓縮ZIP上傳（檔案100MB以內）。",
                    docs: [
                        { Title: "期中報告書", Template: "../../Template/CUL/期中期末成果報告書.docx" }
                    ]
                }, {
                    id: 2,
                    title: "期末報告",
                    subtitle: "期末報告審查",
                    description: "請下載報告書範本，填寫資料及公文用印後上傳。<br>期末報告書及相關檔案（成果照片、影像等），請壓縮ZIP上傳（檔案100MB以內）。",
                    docs: [
                        { Title: "期末報告書", Template: "../../Template/CUL/期中期末成果報告書.docx" }
                    ]
                }];

                const change = (step) => current.value = step;

                const next = () => {};

                onMounted(() => {
                    api.culture("findApplication", { ID: id }).subscribe((result) => store.init("culture", result.Project));

                    initScrollListener();
                });

                return { change, current, id, next, stage, stages, store };
            }
        });
    </script>
</asp:Content>
