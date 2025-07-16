<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReportForm.ascx.cs" Inherits="OSI_ReportForm" %>
<%@ Register Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>

<div class="block rounded-top-4">
    <!-- 隱藏 ID -->
    <asp:HiddenField ID="hfReportID" runat="server" />
    <asp:HiddenField ID="hfIsNew" runat="server" />
    <asp:HiddenField ID="hfTempKey" runat="server" />
    <asp:HiddenField ID="hfToday" runat="server" />
    <asp:HiddenField ID="hdnGeo3826WKT" runat="server" Value="" ClientIDMode="Static" />

    <div class="title mb-3">
        <h4>
            <img src="<%= ResolveUrl("~/assets/img/title-icon03.svg") %>" alt="logo" />
            填報
        </h4>
    </div>

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
                    <asp:DropDownList ID="ddlUnit" runat="server" CssClass="form-select" AppendDataBoundItems="true" />
                </td>
            </tr>
            <!-- 活動名稱 -->
            <tr>
                <th><span class="text-pink">*</span>活動名稱</th>
                <td>
                    <asp:TextBox ID="txtActivityName" runat="server" CssClass="form-control" MaxLength="100" />
                    <asp:RequiredFieldValidator ID="rfvActivityName" runat="server" ValidationGroup="Main"
                        ControlToValidate="txtActivityName" ErrorMessage="必填"
                        CssClass="invalid" Display="Dynamic" />
                </td>
            </tr>
            <!-- 活動性質 -->
            <tr>
                <th><span class="text-pink">*</span>活動性質</th>
                <td>
                    <asp:DropDownList ID="ddlNature" runat="server" CssClass="form-select" AppendDataBoundItems="true" />
                    <asp:TextBox ID="txtNatureDetail" runat="server" CssClass="form-control mt-2"
                        Placeholder="補充說明" MaxLength="200" />
                </td>
            </tr>
            <!-- 活動執行者 -->
            <tr>
                <th><span class="text-pink">*</span>活動執行者</th>
                <td>
                    <asp:UpdatePanel ID="upExecList" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="row g-2">
                                <div class="col-12 col-md-4 col-lg-3">
                                    <asp:DropDownList ID="ddlExecCategory" runat="server" CssClass="form-select" />
                                </div>
                                <div class="col-12 col-md-8 col-lg-9">
                                    <div class="input-group">
                                        <asp:TextBox ID="txtExecName" runat="server" CssClass="form-control" MaxLength="100" />
                                        <asp:Button ID="btnAddExec" runat="server" Text="新增" CssClass="btn btn-cyan" OnClick="btnAddExec_Click" />
                                    </div>
                                </div>
                            </div>
                            <asp:HiddenField ID="hfDelExecIndex" runat="server" />
                            <asp:LinkButton ID="btnDelExec" runat="server" OnClick="btnDelExec_Click" Style="display: none;" />
                            <div class="tag-group mt-2">
                                <asp:Repeater ID="rptExecList" runat="server">
                                    <ItemTemplate>
                                        <span class="tag tag-gray me-1">
                                            <span class="tag-content">
                                                <%# Eval("CategoryName") %>：<%# Eval("ExecutorName") %>
                                            </span>
                                            <button type="button" class="tag-btn" onclick="deleteExec(<%# Container.ItemIndex %>)">
                                                <img src="<%= ResolveUrl("~/assets/img/close-circle.svg") %>" alt="" />
                                            </button>
                                        </span>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <asp:CustomValidator ID="cvExecList" runat="server" ValidationGroup="Main"
                                    CssClass="invalid" Display="Dynamic"
                                    OnServerValidate="cvExecList_ServerValidate" ErrorMessage="至少要新增一筆活動執行者" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <!-- 研究調查日期 -->
            <tr>
                <th><span class="text-pink">*</span>研究調查日期</th>
                <td>
                    <asp:UpdatePanel ID="upResList" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="row g-2 align-items-center">
                                <div class="col-12 col-md-6 col-lg-5">
                                    <div class="input-group">
                                        <asp:TextBox ID="txtResFrom" runat="server" CssClass="form-control rocDate" TextMode="SingleLine" />
                                        <span class="input-group-text">至</span>
                                        <asp:TextBox ID="txtResTo" runat="server" CssClass="form-control rocDate" TextMode="SingleLine" />
                                    </div>
                                </div>
                                <div class="col-12 col-md-6 col-lg-7">
                                    <div class="input-group">
                                        <asp:TextBox ID="txtResRemark" runat="server" CssClass="form-control" Placeholder="備註說明 (20字內)" MaxLength="20" />
                                        <asp:Button ID="btnAddRes" runat="server" Text="新增" CssClass="btn btn-cyan" OnClick="btnAddRes_Click" />
                                    </div>
                                </div>
                            </div>
                            <asp:HiddenField ID="hfDelResIndex" runat="server" />
                            <asp:LinkButton ID="btnDelRes" runat="server" OnClick="btnDelRes_Click" Style="display: none;" />
                            <div class="tag-group mt-2">
                                <asp:Repeater ID="rptResList" runat="server">
                                    <ItemTemplate>
                                        <span class="tag tag-gray me-1">
                                            <span class="tag-content">
                                                <%# Eval("StartDateRoc", "{0:yy/MM/dd}") %>–<%# Eval("EndDateRoc", "{0:yy/MM/dd}") %>
                                                <%# Eval("PeriodLabel") %>
                                            </span>
                                            <button type="button" class="tag-btn" onclick="deleteRes(<%# Container.ItemIndex %>)">
                                                <img src="<%= ResolveUrl("~/assets/img/close-circle.svg") %>" alt="" />
                                            </button>
                                        </span>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <asp:CustomValidator ID="cvDateList" runat="server" ValidationGroup="Main"
                                    CssClass="invalid" Display="Dynamic"
                                    OnServerValidate="cvResList_ServerValidate" ErrorMessage="至少要新增一筆調查日期" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <!-- 使用載具名稱 & 核准文號 -->
            <tr>
                <th>使用載具名稱</th>
                <td>
                    <asp:UpdatePanel ID="upCarrierList" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="row g-2 align-items-center">
                                <div class="col-12 col-md-4 col-lg-2">
                                    <asp:DropDownList ID="ddlCarrierType" runat="server" CssClass="form-select" AppendDataBoundItems="true" />
                                </div>
                                <div class="col-12 col-md-8 col-lg-5">
                                    <asp:TextBox ID="txtCarrierName" runat="server" CssClass="form-control" Placeholder="請輸入使用載具名稱" MaxLength="200" />
                                </div>
                                <div class="col-12 col-md-12 col-lg-5">
                                    <div class="input-group">
                                        <span class="input-group-text">核准文號</span>
                                        <asp:TextBox ID="txtCarrierApproval" runat="server" CssClass="form-control" MaxLength="200" Placeholder="請輸入核准文號" />
                                        <asp:Button ID="btnAddCarrier" runat="server" Text="新增" CssClass="btn btn-cyan" OnClick="btnAddCarrier_Click" />
                                    </div>
                                </div>
                            </div>
                            <asp:HiddenField ID="hfDelCarrierIndex" runat="server" />
                            <asp:LinkButton ID="btnDelCarrier" runat="server" OnClick="btnDelCarrier_Click" Style="display: none;" />
                            <div class="tag-group mt-2">
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
                                            <button type="button" class="tag-btn" onclick="deleteCarrier(<%# Container.ItemIndex %>)">
                                                <img src="<%= ResolveUrl("~/assets/img/close-circle.svg") %>" alt="" />
                                            </button>
                                        </span>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <!-- 研究調查項目 -->
            <tr>
                <th>研究調查項目</th>
                <td>
                    <div class="row g-2">
                        <div class="col-12 col-md-12 col-lg-12">
                            <asp:DropDownList ID="ddlResearchCategory" runat="server" CssClass="form-select" AppendDataBoundItems="true" />
                        </div>
                    </div>
                    <div class="input-group mt-2">
                        <asp:TextBox ID="txtResItemNote" runat="server" CssClass="form-control" Placeholder="補充說明" />
                    </div>
                </td>
            </tr>
            <!-- 研究調查儀器 -->
            <tr>
                <th>研究調查儀器</th>
                <td>
                    <asp:TextBox ID="txtResInstruments" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" MaxLength="500" Placeholder="請輸入調查項目及儀器" />
                </td>
            </tr>
            <!-- 活動內容概述 -->
            <tr>
                <th>研究調查活動<br />
                    內容概述</th>
                <td>
                    <asp:TextBox ID="txtActivityOverview" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" MaxLength="500" Placeholder="請輸入概述" />
                    <div class="text-pink mt-2">限500字</div>
                </td>
            </tr>
            <!-- 相關附件 -->
            <tr>
                <th>相關附件</th>
                <td>
                    <div class="input-group">
                        <asp:FileUpload ID="fuActivityFile" runat="server" AllowMultiple="true" CssClass="form-control" accept=".doc,.docx,.xls,.xlsx,.odf,.ods,.pdf" />
                        <asp:Button
                            ID="btnUpload"
                            runat="server"
                            Text="上傳"
                            CssClass="btn btn-cyan"
                            OnClick="btnUpload_Click"
                            ValidationGroup="None"
                            Enabled="false" />
                    </div>
                    <div class="text-pink mt-2">檔案格式限:doc/docx/xls/xlsx/odf/ods/pdf</div>
                    <asp:UpdatePanel ID="upFileList" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:HiddenField ID="hfDelFileIndex" runat="server" />
                            <asp:LinkButton ID="btnDelFile" runat="server" OnClick="btnDelFile_Click" Style="display: none;" />
                            <div class="tag-group mt-2">
                                <asp:Repeater ID="rptActivityFile" runat="server"
                                    OnItemCommand="rptActivityFile_ItemCommand">
                                    <ItemTemplate>
                                        <span class="tag tag-gray me-1">
                                            <asp:LinkButton
                                                runat="server"
                                                ID="lnkDownload"
                                                CommandName="Download"
                                                CommandArgument='<%# Eval("FilePath") %>'
                                                CssClass="tag-link">
                                                <%# Eval("FileName") %>
                                                <img src="<%= ResolveUrl("~/assets/img/icon-download.svg") %>" alt="download" />
                                            </asp:LinkButton>
                                            <button type="button" class="tag-btn" onclick="deleteFile(<%# Container.ItemIndex %>)">
                                                <img src="<%= ResolveUrl("~/assets/img/close-circle.svg") %>" alt="" />
                                            </button>
                                        </span>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <!-- 研究調查範圍 -->
            <tr>
                <th>研究調查範圍</th>
                <td>
                    <asp:UpdatePanel ID="upScopeList" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="input-group">
                                <asp:TextBox ID="txtSurveyScope" runat="server" CssClass="form-control" MaxLength="200" Placeholder="請輸入研究調查範圍" />
                                <asp:Button ID="btnAddScope" runat="server" Text="新增" CssClass="btn btn-cyan" OnClick="btnAddScope_Click" />
                            </div>
                            <asp:HiddenField ID="hfDelScopeIndex" runat="server" />
                            <asp:LinkButton ID="btnDelScope" runat="server" OnClick="btnDelScope_Click" Style="display: none;" />
                            <div class="tag-group mt-2">
                                <asp:Repeater ID="rptScopeList" runat="server">
                                    <ItemTemplate>
                                        <span class="tag tag-gray me-1">
                                            <span class="tag-content">
                                                <%# Eval("SurveyScope") %>
                                            </span>
                                            <button type="button" class="tag-btn" onclick="deleteScope(<%# Container.ItemIndex %>)">
                                                <img src="<%= ResolveUrl("~/assets/img/close-circle.svg") %>" alt="" />
                                            </button>
                                        </span>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
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
                            data-reportid="">
                            <i class="fas fa-map"></i>開啟圖台
                        </button>

                        <asp:LinkButton
                            ID="btnDownloadFeatures"
                            runat="server"
                            CssClass="btn btn-cyan"
                            OnClientClick="downloadFeaturesFromIframe(); return false;">
                            <i class="fas fa-download"></i>下載圖資
                        </asp:LinkButton>
                    </div>
                </td>
            </tr>



        </tbody>
    </table>



</div>
<!-- 底部儲存區塊 -->
<asp:Panel ID="pnlLastUpdated" runat="server" CssClass="block-bottom text-center">
    <div id="lastUpdate" class="mb-4" runat="server">最後更新：<asp:Label ID="lblLastUpdated" runat="server" CssClass="fw-semibold" /></div>

    <div class="d-flex gap-4 flex-wrap justify-content-center">
        <asp:LinkButton ID="btnBack" runat="server"
            CssClass="btn btn-outline-pink"
            OnClick="btnBack_Click">
                返回
        </asp:LinkButton>

        <asp:LinkButton ID="btnSave" runat="server"
            CssClass="btn btn-cyan"
            OnClientClick="prepareGeoData();"
            OnClick="btnSave_Click">
                <i class="fas fa-check"></i>儲存
        </asp:LinkButton>
    </div>
</asp:Panel>

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
    
    /* 限制標籤群組寬度 */
    /*.tag-group {
        max-width: 100%;
        overflow: hidden;
    }*/
    
    /* 限制單個標籤寬度和溢出處理 */
    .tag {
        overflow: hidden;
        display: inline-flex;
        line-height: 1.2; /* 覆蓋主要CSS的line-height: 0 */
    }
    
    /* 標籤文字區域 */
    .tag-content {
        flex: 1;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
        max-width: calc(100% - 20px); /* 為按鈕預留空間 */
        line-height: 1.2; /* 確保文字有適當的行高 */
        display: inline-block; /* 確保文字正常顯示 */
    }
    
    /* 確保按鈕顯示 */
    .tag .tag-btn {
        flex-shrink: 0;
        margin-left: 4px;
    }
    
    /* 地圖 Modal 樣式調整 */
    #mapModal .modal-dialog {
        width: 90vw;
        height: 90vh;
        max-width: 90vw;
        max-height: 90vh;
        margin: 5vh auto; /* 上下左右各留 5% 的空間 */
    }
    
    #mapModal .modal-content {
        height: 100%;
        display: flex;
        flex-direction: column;
    }
    
    #mapModal .modal-body {
        flex: 1;
        height: auto !important; /* 覆蓋內聯樣式 */
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

    document.addEventListener('DOMContentLoaded', function () {
        var fu = document.getElementById('<%= fuActivityFile.ClientID %>');
        var btn = document.getElementById('<%= btnUpload.ClientID %>');

        // 一開始檢查一次
        btn.disabled = !fu.files.length;

        // 檔案選擇變更時切換 disabled
        fu.addEventListener('change', function () {
            btn.disabled = !this.files.length;
        });
    });

    // 下載圖資
    function downloadFeaturesFromIframe() {
        var allWKT = "";
        var iframe = document.getElementById("mapFrame");

        // 優先嘗試從地圖 iframe 取得資料
        if (iframe && iframe.contentWindow && iframe.src && iframe.src !== '') {
            try {
                // 檢查函數是否存在
                if (typeof iframe.contentWindow.getAllFeaturesWKT3826 === 'function') {
                    allWKT = iframe.contentWindow.getAllFeaturesWKT3826();
                }
            }
            catch (ex) {
                console.log("無法從地圖取得資料，將使用原始資料");
            }
        }

        // 如果從地圖取不到資料，使用隱藏欄位中的原始資料
        if (!allWKT || allWKT === "") {
            var hdnField = document.getElementById("hdnGeo3826WKT");
            if (hdnField && hdnField.value) {
                allWKT = hdnField.value;
                console.log("使用原始儲存的地理資料");
            }
        }

        // 檢查是否有資料可下載
        if (!allWKT || allWKT === "") {
            alert("沒有地理標記資料可供下載。");
            return;
        }

        try {
            // 建立 WKT 檔案下載
            var blob = new Blob([allWKT], { type: "text/plain;charset=utf-8;" });
            var link = document.createElement("a");
            link.href = URL.createObjectURL(blob);

            // 使用時間戳記建立檔名
            var now = new Date();
            var timestamp = now.getFullYear() +
                ('0' + (now.getMonth() + 1)).slice(-2) +
                ('0' + now.getDate()).slice(-2) + '_' +
                ('0' + now.getHours()).slice(-2) +
                ('0' + now.getMinutes()).slice(-2);
            link.download = "MapFeatures_" + timestamp + ".wkt";

            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);

            // 釋放 URL 物件
            URL.revokeObjectURL(link.href);

            // 提示使用者
            console.log("地理資料下載完成");
        }
        catch (ex) {
            console.error("下載標記發生錯誤：", ex);
            alert("下載圖資時發生錯誤，請稍後重試。");
        }
    }

    // 取得資料
    function prepareGeoData() {
        // 1. 先取得 iframe 元素（假設你的 Map 是放在 id="mapFrame" 的 <iframe>）
        var iframe = document.getElementById("mapFrame");
        if (!iframe || !iframe.contentWindow) {
            // 如果找不到地圖 iframe，隱藏欄位就保留空字串
            document.getElementById("hdnGeo3826WKT").value = "";
            return;
        }

        // 檢查 iframe 是否已經載入完成
        try {
            // 2. 呼叫 Map.aspx 裡面我們剛寫的 getAllFeaturesWKT3826()
            var wkt3826 = iframe.contentWindow.getAllFeaturesWKT3826();
            // 3. 把結果設定到隱藏欄位
            document.getElementById("hdnGeo3826WKT").value = wkt3826 || "";
        } catch (e) {
            console.error("無法取得地圖資料:", e);
            // 如果出錯，保留現有值或設為空字串
            var hdnField = document.getElementById("hdnGeo3826WKT");
            if (!hdnField.value) {
                hdnField.value = "";
            }
        }
    }

    // 開啟圖台時觸發
    var mapModalEl = document.getElementById('mapModal');
    var mapIframeLoaded = false; // 追蹤 iframe 是否已載入

    mapModalEl.addEventListener('show.bs.modal', function (e) {
        // e.relatedTarget 是觸發這個 Modal 的按鈕節點
        var button = e.relatedTarget;
        // 從按鈕上讀 data-reportid
        var reportId = button.getAttribute('data-reportid');
        var iframe = document.getElementById('mapFrame');

        // 檢查 iframe 是否已經載入過相同的 reportId
        var currentReportId = iframe.getAttribute('data-current-reportid');

        // 只有在第一次或 reportId 不同時才重新載入
        if (!mapIframeLoaded || currentReportId !== reportId) {
            var urlBase = '<%= ResolveUrl("~/Map.aspx") %>';
            iframe.src = urlBase + '?codes=02,05,01,04,08&reportId=' + reportId;
            iframe.setAttribute('data-current-reportid', reportId);
            mapIframeLoaded = true;
        }
        // 否則保留現有內容，不重新載入
    });

    // 刪除活動執行者
    function deleteExec(index) {
        var hfDelExecIndex = document.getElementById('<%= hfDelExecIndex.ClientID %>');
        var btnDelExec = document.getElementById('<%= btnDelExec.ClientID %>');
        hfDelExecIndex.value = index;
        btnDelExec.click();
    }

    // 刪除研究調查日期
    function deleteRes(index) {
        var hfDelResIndex = document.getElementById('<%= hfDelResIndex.ClientID %>');
        var btnDelRes = document.getElementById('<%= btnDelRes.ClientID %>');
        hfDelResIndex.value = index;
        btnDelRes.click();
    }

    // 刪除檔案
    function deleteFile(index) {
        var hfDelFileIndex = document.getElementById('<%= hfDelFileIndex.ClientID %>');
        var btnDelFile = document.getElementById('<%= btnDelFile.ClientID %>');
        hfDelFileIndex.value = index;
        btnDelFile.click();
    }

    // 刪除研究調查範圍
    function deleteScope(index) {
        var hfDelScopeIndex = document.getElementById('<%= hfDelScopeIndex.ClientID %>');
        var btnDelScope = document.getElementById('<%= btnDelScope.ClientID %>');
        hfDelScopeIndex.value = index;
        btnDelScope.click();
    }

    // 刪除載具
    function deleteCarrier(index) {
        var hfDelCarrierIndex = document.getElementById('<%= hfDelCarrierIndex.ClientID %>');
        var btnDelCarrier = document.getElementById('<%= btnDelCarrier.ClientID %>');
        hfDelCarrierIndex.value = index;
        btnDelCarrier.click();
    }

</script>

