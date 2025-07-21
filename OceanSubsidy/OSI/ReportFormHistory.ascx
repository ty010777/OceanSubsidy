<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReportFormHistory.ascx.cs" Inherits="OSI_ReportFormHistory" %>


<table class="table align-middle gray-table side-table">
    <tbody>
        <!-- 資料時間 -->
        <tr>
            <th>資料時間</th>
            <td>
                <asp:Label ID="lblDataPeriod" runat="server" />
            </td>
        </tr>
        <!-- 填報機關 -->
        <tr>
            <th><span class="text-pink">*</span>填報機關</th>
            <td>
                <asp:Label ID="lblUnit" runat="server" />
            </td>
        </tr>
        <!-- 活動名稱 -->
        <tr>
            <th><span class="text-pink">*</span>活動名稱</th>
            <td>
                <asp:Label ID="lblActivityName" runat="server" />
            </td>
        </tr>
        <!-- 活動性質 -->
        <tr>
            <th><span class="text-pink">*</span>活動性質</th>
            <td>
                <asp:Label ID="lblNature" runat="server" />
                <asp:Label ID="lblNatureDetail" runat="server" CssClass="d-block mt-2 text-muted" />
            </td>
        </tr>
        <!-- 活動執行者 -->
        <tr>
            <th><span class="text-pink">*</span>活動執行者</th>
            <td>
                <div class="tag-group">
                    <asp:Repeater ID="rptExecList" runat="server">
                        <ItemTemplate>
                            <span class="tag tag-gray me-1">
                                <span class="tag-content">
                                    <%# Eval("CategoryName") %>：<%# Eval("ExecutorName") %>
                                </span>
                            </span>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </td>
        </tr>
        <!-- 研究調查日期 -->
        <tr>
            <th><span class="text-pink">*</span>研究調查日期</th>
            <td>
                <div class="tag-group">
                    <asp:Repeater ID="rptResList" runat="server">
                        <ItemTemplate>
                            <span class="tag tag-gray me-1">
                                <span class="tag-content">
                                    <%# Eval("StartDateRoc", "{0:yy/MM/dd}") %>–<%# Eval("EndDateRoc", "{0:yy/MM/dd}") %>
                                    <%# Eval("PeriodLabel") %>
                                </span>
                            </span>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </td>
        </tr>
        <!-- 使用載具名稱 & 核准文號 -->
        <tr>
            <th>使用載具名稱</th>
            <td>
                <div class="tag-group">
                    <asp:Repeater ID="rptCarrierList" runat="server">
                        <ItemTemplate>
                            <span class="tag tag-gray me-1">
                                <span class="tag-content">
                                    <%# Eval("CarrierTypeName") %>
                                    <%# !string.IsNullOrEmpty(Eval("CarrierTypeName").ToString()) && 
                                            !string.IsNullOrEmpty(Eval("CarrierDetail").ToString()) ? "：" : "" %>
                                    <%# Eval("CarrierDetail") %>
                                    <%# !string.IsNullOrEmpty(Eval("CarrierNo").ToString()) ? " (" + Eval("CarrierNo") + ")" : "" %>
                                </span>
                            </span>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </td>
        </tr>
        <!-- 研究調查項目 -->
        <tr>
            <th>研究調查項目</th>
            <td>
                <asp:Label ID="lblResearchCategory" runat="server" />
                <asp:Label ID="lblResItemNote" runat="server" CssClass="d-block mt-2 text-muted" />
            </td>
        </tr>
        <!-- 研究調查儀器 -->
        <tr>
            <th>研究調查儀器</th>
            <td>
                <asp:Label ID="lblResInstruments" runat="server" CssClass="text-pre-wrap" />
            </td>
        </tr>
        <!-- 活動內容概述 -->
        <tr>
            <th>研究調查活動<br />
                內容概述</th>
            <td>
                <asp:Label ID="lblActivityOverview" runat="server" CssClass="text-pre-wrap" />
            </td>
        </tr>
        <!-- 相關附件 -->
        <tr>
            <th>相關附件</th>
            <td>
                <div class="tag-group">
                    <asp:Repeater ID="rptActivityFile" runat="server" OnItemCommand="rptActivityFile_ItemCommand">
                        <ItemTemplate>
                            <span class="tag tag-gray me-1">
                                <span class="tag-content">
                                    <%# Eval("FileName") %>
                                </span>
                            </span>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </td>
        </tr>
        <!-- 研究調查範圍 -->
        <tr>
            <th>研究調查範圍</th>
            <td>
                <div class="tag-group">
                    <asp:Repeater ID="rptScopeList" runat="server">
                        <ItemTemplate>
                            <span class="tag tag-gray me-1">
                                <span class="tag-content">
                                    <%# Eval("SurveyScope") %>
                                </span>
                            </span>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </td>
        </tr>
        <!-- 活動空間範圍標定 -->
        <tr>
            <th>活動空間範圍<br />
                標定</th>
            <td>
                <div class="d-flex flex-wrap gap-2">
                    <button
                        type="button"
                        id="btnOpenMap"
                        runat="server"
                        class="btn btn-cyan"
                        data-bs-toggle="modal"
                        data-bs-target="#mapModal"
                        data-historyId="">
                        <i class="fas fa-map"></i>檢視圖台
                    </button>
                </div>
            </td>
        </tr>
        <!-- 標示修正說明 -->
        <tr id="trCorrectionNotes" runat="server">
            <th><span class="text-pink">*</span>標示修正說明</th>
            <td>
                <asp:Label ID="lblCorrectionNotes" runat="server" CssClass="text-pre-wrap" />
            </td>
        </tr>
    </tbody>
</table>


<!-- 底部資訊區塊 -->
<div class="block-bottom text-center">
    <div class="mb-4">
        <span class="fw-semibold">修改時間：</span><asp:Label ID="lblAuditAt" runat="server" CssClass="fw-semibold" />
    </div>
</div>

<%--圖台Modal--%>
<div class="modal fade" id="mapModal" tabindex="-1" aria-labelledby="mapModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-xl modal-fullscreen-lg-down">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                    <i class="fa-solid fa-circle-xmark"></i>
                </button>
            </div>
            <div class="modal-body p-0" style="height: 80vh;">
                <iframe
                    id="mapFrame"
                    src=""
                    style="width: 100%; height: 100%; border: 0;"
                    allowfullscreen></iframe>
            </div>
        </div>
    </div>
</div>

<style type="text/css">
    /* 使文字保持換行格式 */
    .text-pre-wrap {
        white-space: pre-wrap;
    }

    /* 固定表格佈局 */
    .gray-table.side-table {
        table-layout: fixed;
    }

        /* 固定 th 和 td 寬度 */
        .gray-table.side-table th:first-child {
            width: 183px;
        }

        .gray-table.side-table td {
            width: auto;
        }

    /* 標籤樣式 */
    .tag {
        overflow: hidden;
        display: inline-flex;
        line-height: 1.2;
    }

    .tag-content {
        flex: 1;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
        line-height: 1.2;
        display: inline-block;
    }

    /* 地圖 Modal 樣式調整 */
    #mapModal .modal-dialog {
        width: 90vw;
        height: 90vh;
        max-width: 90vw;
        max-height: 90vh;
        margin: 5vh auto;
    }

    #mapModal .modal-content {
        height: 100%;
        display: flex;
        flex-direction: column;
    }

    #mapModal .modal-body {
        flex: 1;
        height: auto !important;
        overflow: hidden;
        padding: 0;
    }

    #mapModal iframe {
        width: 100%;
        height: 100%;
        border: 0;
    }
</style>

<script type="text/javascript">
    // 開啟圖台時觸發
    var mapModalEl = document.getElementById('mapModal');
    mapModalEl.addEventListener('show.bs.modal', function (e) {
        var button = e.relatedTarget;
        var historyId = button.getAttribute('data-historyId');
        var iframe = document.getElementById('mapFrame');
        var urlBase = '<%= ResolveUrl("~/Map.aspx") %>';
        iframe.src = urlBase + '?codes=02,05,01,04,08&historyId=' + historyId + '&readonly=true';
    });
</script>
