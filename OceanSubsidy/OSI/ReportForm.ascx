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
                    <asp:RequiredFieldValidator ID="rfvNature" runat="server" ValidationGroup="Main"
                        ControlToValidate="txtNatureDetail" InitialValue="" ErrorMessage="必填"
                        CssClass="invalid" Display="Dynamic" />
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
                                        <asp:Button ID="btnAddExec" runat="server" Text="新增" CssClass="btn btn-blue-green" OnClick="btnAddExec_Click" />
                                    </div>
                                </div>
                            </div>
                            <div class="tag-group mt-2">
                                <asp:Repeater ID="rptExecList" runat="server">
                                    <ItemTemplate>
                                        <span class="tag tag-gray me-1">
                                            <%# Eval("CategoryName") %>：<%# Eval("ExecutorName") %>
                                            <button type="button" class="tag-btn" onclick="__doPostBack('<%= rptExecList.UniqueID %>','DelExec$<%# Container.ItemIndex %>')">
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
                                        <asp:Button ID="btnAddRes" runat="server" Text="新增" CssClass="btn btn-blue-green" OnClick="btnAddRes_Click" />
                                    </div>
                                </div>
                            </div>
                            <div class="tag-group mt-2">
                                <asp:Repeater ID="rptResList" runat="server">
                                    <ItemTemplate>
                                        <span class="tag tag-gray me-1">
                                            <%# Eval("StartDateRoc", "{0:yyyy/MM/dd}") %>–<%# Eval("EndDateRoc", "{0:yyyy/MM/dd}") %>
                                            <%# Eval("PeriodLabel") %>
                                            <button type="button" class="tag-btn" onclick="__doPostBack('<%= rptResList.UniqueID %>','DelDate$<%# Container.ItemIndex %>')">
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
                    <div class="row g-2">
                        <div class="col-12 col-md-4 col-lg-3">
                            <asp:DropDownList ID="ddlCarrierType" runat="server" CssClass="form-select" AppendDataBoundItems="true" />
                        </div>
                        <div class="col-12 col-md-8 col-lg-9">
                            <asp:TextBox ID="txtCarrierName" runat="server" CssClass="form-control" Placeholder="請輸入使用載具名稱" />
                        </div>
                    </div>
                    <div class="input-group mt-2">
                        <span class="input-group-text">核准文號</span>
                        <asp:TextBox ID="txtCarrierApproval" runat="server" CssClass="form-control" MaxLength="200" Placeholder="請輸入核准文號" />
                    </div>
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
                            CssClass="btn btn-blue-green"
                            OnClick="btnUpload_Click"
                            ValidationGroup="None"
                            Enabled="false" />
                    </div>
                    <div class="text-pink mt-2">檔案格式限:doc/docx/xls/xlsx/odf/ods/pdf</div>
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
                                    <button type="button" class="tag-btn" onclick="__doPostBack( '<%= rptActivityFile.UniqueID %>', 'DelAttach$<%# Container.ItemIndex %>')">
                                        <img src="<%= ResolveUrl("~/assets/img/close-circle.svg") %>" alt="" />
                                    </button>
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
                    <asp:TextBox ID="txtResearchScope" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" Placeholder="請輸入研究調查範圍" />
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
                            class="btn btn-blue-green"
                            data-bs-toggle="modal"
                            data-bs-target="#mapModal"
                            data-reportid="">
                            <i class="fas fa-map"></i>開啟圖台
                        </button>

                        <asp:LinkButton
                            ID="btnDownloadFeatures"
                            runat="server"
                            CssClass="btn btn-blue-green"
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
    <div id="lastUpdate" runat="server">最後更新：<asp:Label ID="lblLastUpdated" runat="server" CssClass="fw-semibold" /></div>

    <asp:LinkButton ID="btnSave" runat="server"
        CssClass="btn btn-blue-green d-table mx-auto mt-2"
        OnClientClick="prepareGeoData();"
        OnClick="btnSave_Click">
            <i class="fas fa-check"></i>儲存
    </asp:LinkButton>
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

</script>

