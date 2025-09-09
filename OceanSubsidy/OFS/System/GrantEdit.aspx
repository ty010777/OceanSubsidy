<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GrantEdit.aspx.cs" Inherits="Admin_GrantEdit" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon07.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">系統管理 / 補助計畫管理</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="mt-3">
        <nav>
            <div class="tab" role="tablist">
                <button :aria-selected="stage === 1 ? 'true' : 'false'" class="tab-link" :class="{ active: stage === 1 }" v-on:click="stage = 1" role="tab" type="button">內容管理</button>
                <button :aria-selected="stage === 2 ? 'true' : 'false'" class="tab-link" :class="{ active: stage === 2 }" v-on:click="stage = 2" role="tab" type="button" v-if="id">機制設定</button>
                <button :aria-selected="stage === 3 ? 'true' : 'false'" class="tab-link" :class="{ active: stage === 3 }" v-on:click="stage = 3" role="tab" type="button" v-if="id">補助經費</button>
                <button :aria-selected="stage === 4 ? 'true' : 'false'" class="tab-link" :class="{ active: stage === 4 }" v-on:click="stage = 4" role="tab" type="button" v-if="id && ['SCI','CUL'].includes(grant.type)">審查委員</button>
            </div>
        </nav>
        <system-grant-form1 :id="id" v-on:next="next" v-if="stage === 1"></system-grant-form1>
        <system-grant-form2 :id="id" v-on:next="next" v-else-if="stage === 2"></system-grant-form2>
        <system-grant-form3 :id="id" v-on:next="next" v-else-if="stage === 3"></system-grant-form3>
        <system-grant-form4 :id="id" v-on:next="next" v-else-if="stage === 4"></system-grant-form4>
    </div>
    <script>
        setupVueApp({
            setup() {
                const { ref } = Vue;
                const { useGrantStore } = OceanSubsidyComponents;

                const grant = useGrantStore();
                const id = "<%= Request.QueryString["ID"] %>";
                const stage = ref(1);

                const next = () => location.href = "GrantList.aspx";

                return { grant, id, next, stage };
            }
        });
    </script>
</asp:Content>
