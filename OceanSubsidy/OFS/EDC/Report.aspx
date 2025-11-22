<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report.aspx.cs" Inherits="OFS_EdcReport" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

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
        <project-title :id="id" type="education"></project-title>
        <project-progress :excludes="[4]" :id="id" :step="5" type="education"></project-progress>
        <div class="block rounded-top-4 pb-0">
            <ul class="teal-dark-tabs mb-0">
                <li :class="{ active: item.id === stage }" :key="item" v-for="(item) in stages">
                    <a class="tab-link" @click.prevent="stage = item.id" href="javascript:void(0)">{{ item.title }}</a>
                </li>
            </ul>
        </div>
        <template :key="item" v-for="(item) in stages">
            <project-report :data="item" :id="id" @next="next" type="education" v-if="item.id === stage"></project-report>
        </template>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="FloatContent" runat="server">
    <project-report-review :id="id" :stage="stage" type="education" v-if="store.education.reportStatus === '審核中'"></project-report-review>
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
                    title: "成果報告",
                    subtitle: "成果報告審查",
                    description: "請下載報告書範本，填寫資料及公文用印後上傳。<br>成果報告書及相關檔案，請壓縮ZIP上傳（檔案500MB以內）。",
                    docs: [
                        { Type: 1, Title: "成果報告書", Template: "../../Template/EDC/成果報告書.docx" }
                    ]
                }];

                const change = (step) => current.value = step;

                const next = (callback) => Swal.fire({ title: "提送成功", icon: "success" }).then(callback);

                onMounted(() => {
                    api.education("findApplication", { ID: id }).subscribe((result) => store.init("education", result.Project));

                    initScrollListener();
                });

                return { change, current, id, next, stage, stages, store };
            }
        });
    </script>
</asp:Content>
