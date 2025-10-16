<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReviewCommitteeInfo.aspx.cs" Inherits="OFS_ReviewCommitteeInfo" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/BaseMaster.master" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="BaseTitleContent" runat="server">
    海洋領域補助計畫管理資訊系統
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <%-- DatePicker --%>
    <link href="<%= ResolveUrl("~/assets/vendor/daterangepicker.css") %>" rel="stylesheet" />
    <script src="<%= ResolveUrl("~/assets/vendor/moment.min.js") %>"></script>
    <script src="<%= ResolveUrl("~/assets/vendor/moment-taiwan.js") %>"></script>
    <script src="<%= ResolveUrl("~/assets/vendor/locale/zh-tw.js") %>"></script>
    <script src="<%= ResolveUrl("~/assets/vendor/daterangepicker.js") %>"></script>
    <%-- Vue --%>
    <script src="<%= ResolveUrl("~/assets/vendor/vue-3.5.13.global.prod.min.js") %>"></script>
    <%-- Pinia --%>
    <script src="<%= ResolveUrl("~/assets/vendor/vue-demi.iife.min.js") %>"></script>
    <script src="<%= ResolveUrl("~/assets/vendor/pinia.iife.prod.min.js") %>"></script>
    <%-- RxJs --%>
    <script src="<%= ResolveUrl("~/assets/vendor/rxjs.umd.min.js") %>"></script>
    <%-- Custom --%>
    <link href="<%= ResolveUrl("~/assets/js/ocean-subsidy-components.css") %>?<%= Guid.NewGuid().ToString("N") %>" rel="stylesheet" />
    <script src="<%= ResolveUrl("~/assets/js/ocean-subsidy-components.umd.js") %>?<%= Guid.NewGuid().ToString("N") %>"></script>
    <script src="<%= ResolveUrl("~/assets/js/vue-custom.js") %>"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyContent" runat="server">
    <form id="form1" runat="server">
        <main>
            <div class="mis-layout">
                <div class="mis-content">
                    <div class="mis-container">
                        <review-committee-info :token="token"></review-committee-info>
                    </div>
                </div>
            </div>
        </main>
    </form>
    <script>
        setupVueApp({
            setup() {
                const token = "<%= Request.QueryString["Token"] %>";

                return { token };
            }
        });
        startVueApp(".mis-content");
    </script>
</asp:Content>
