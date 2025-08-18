<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VesselRiskForm.ascx.cs" Inherits="OSI_VesselRiskForm" %>
<%@ Register Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>

<div class="block rounded-top-4">
    <!-- 隱藏 ID -->
    <asp:HiddenField ID="hfAssessmentId" runat="server" />
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
            <!-- 計畫類別 -->
            <tr>
                <th>計畫類別</th>
                <td>
                    <asp:CheckBoxList ID="cblRiskCategories" runat="server"
                        RepeatDirection="Horizontal" RepeatLayout="Flow"
                        CssClass="form-check-input-group"
                        Style="--grid-columns: 3;" />
                </td>
            </tr>
            <!-- 計畫主持人 -->
            <tr>
                <th><span class="text-pink">*</span>計畫主持人</th>
                <td>
                    <asp:TextBox ID="txtInvestigator" runat="server" CssClass="form-control" MaxLength="50" />
                    <asp:RequiredFieldValidator ID="rfvInvestigator" runat="server" ValidationGroup="Main"
                        ControlToValidate="txtInvestigator" ErrorMessage="必填"
                        CssClass="invalid" Display="Dynamic" />
                </td>
            </tr>
            <!-- 填表日期 -->
            <tr>
                <th><span class="text-pink">*</span>填表日期</th>
                <td>
                    <asp:TextBox ID="txtFormDate" runat="server" CssClass="form-control rocDate" TextMode="SingleLine" Placeholder="yyy/mm/dd" />
                    <asp:RequiredFieldValidator ID="rfvFormDate" runat="server" ValidationGroup="Main"
                        ControlToValidate="txtFormDate" ErrorMessage="必填"
                        CssClass="invalid" Display="Dynamic" />
                </td>
            </tr>
            <!-- 申請單位 -->
            <tr>
                <th><span class="text-pink">*</span>申請單位</th>
                <td>
                    <asp:TextBox ID="txtUnit" runat="server" CssClass="form-control" MaxLength="100" />
                    <asp:RequiredFieldValidator ID="rfvUnit" runat="server" ValidationGroup="Main"
                        ControlToValidate="txtUnit" ErrorMessage="必填"
                        CssClass="invalid" Display="Dynamic" />
                </td>
            </tr>
            <!-- 計畫名稱 -->
            <tr>
                <th><span class="text-pink">*</span>計畫名稱</th>
                <td>
                    <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" MaxLength="200" />
                    <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ValidationGroup="Main"
                        ControlToValidate="txtTitle" ErrorMessage="必填"
                        CssClass="invalid" Display="Dynamic" />
                </td>
            </tr>
            <!-- 預定作業期間 (起始) -->
            <tr>
                <th><span class="text-pink">*</span>預定作業期間<br />
                    起始</th>
                <td>
                    <div class="row g-2">
                        <div class="col-12 col-md-4">
                            <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control rocDate" TextMode="SingleLine" Placeholder="yyy/mm/dd" />
                            <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ValidationGroup="Main"
                                ControlToValidate="txtStartDate" ErrorMessage="必填"
                                CssClass="invalid" Display="Dynamic" />
                        </div>
                        <div class="col-12 col-md-2">
                            <asp:TextBox ID="txtStartTime" runat="server" CssClass="form-control" Placeholder="HH:mm" MaxLength="5" />
                        </div>
                        <div class="col-12 col-md-6">
                            <asp:TextBox ID="txtStartRemark" runat="server" CssClass="form-control" MaxLength="200" />
                        </div>
                    </div>
                </td>
            </tr>
            <!-- 預定作業期間 (結束) -->
            <tr>
                <th><span class="text-pink">*</span>預定作業期間<br />
                    結束</th>
                <td>
                    <div class="row g-2">
                        <div class="col-12 col-md-4">
                            <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control rocDate" TextMode="SingleLine" Placeholder="yyy/mm/dd" />
                            <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ValidationGroup="Main"
                                ControlToValidate="txtEndDate" ErrorMessage="必填"
                                CssClass="invalid" Display="Dynamic" />
                        </div>
                        <div class="col-12 col-md-2">
                            <asp:TextBox ID="txtEndTime" runat="server" CssClass="form-control" Placeholder="HH:mm" MaxLength="5" />
                        </div>
                        <div class="col-12 col-md-6">
                            <asp:TextBox ID="txtEndRemark" runat="server" CssClass="form-control" MaxLength="200" />
                        </div>
                    </div>
                </td>
            </tr>
            <!-- 預定作業期間共計天數 -->
            <tr>
                <th><span class="text-pink">*</span>預定作業期間<br />
                    共計天數</th>
                <td>
                    <asp:TextBox ID="txtDurationDays" runat="server" CssClass="form-control" ReadOnly="true" />
                    <asp:RequiredFieldValidator ID="rfvDurationDays" runat="server" ValidationGroup="Main"
                        ControlToValidate="txtDurationDays" ErrorMessage="必填"
                        CssClass="invalid" Display="Dynamic" />
                </td>
            </tr>
            <!-- 探測海域名稱 -->
            <tr>
                <th><span class="text-pink">*</span>探測海域名稱</th>
                <td>
                    <asp:TextBox ID="txtSurveyAreaName" runat="server" CssClass="form-control" MaxLength="200" />
                </td>
            </tr>
            <!-- 範圍標定 -->
            <tr>
                <th>範圍標定</th>
                <td>
                    <div class="d-flex flex-wrap gap-2">
                        <button
                            type="button"
                            id="btnOpenMap"
                            runat="server"
                            class="btn btn-cyan"
                            data-bs-toggle="modal"
                            data-bs-target="#mapModal"
                            data-assessmentid="">
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
            <!-- 預定航程規劃及作業項目 -->
            <tr>
                <th>預定航程規劃<br />
                    及作業項目</th>
                <td>
                    <asp:TextBox ID="txtVoyagePlanAndOperations" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" />
                </td>
            </tr>
            <!-- 相關附件 -->
            <tr>
                <th>相關附件</th>
                <td>
                    <div class="input-group">
                        <asp:FileUpload ID="fuVesselFile" runat="server" AllowMultiple="true" CssClass="form-control" accept=".doc,.docx,.xls,.xlsx,.odf,.ods,.pdf" />
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
                                <asp:Repeater ID="rptVesselFile" runat="server"
                                    OnItemCommand="rptVesselFile_ItemCommand">
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
        </tbody>
    </table>

    <!-- 風險評估內容 -->
    <h5 class="mt-4 mb-3 text-blue-green">風險評估內容</h5>
    <table class="table align-middle cyan-table">
        <thead>
            <tr>
                <th width="50">排序</th>
                <th>項目</th>
                <th width="80">是</th>
                <th width="80">否</th>
                <th width="80">未提供</th>
            </tr>
        </thead>
        <tbody>
            <!-- Q1 -->
            <tr>
                <td>1</td>
                <td>本航次是否會在涉敏感/重疊海域作業</td>
                <td class="text-center">
                    <asp:RadioButton ID="rbQ1Yes" runat="server" GroupName="Q1" Value="1" />
                </td>
                <td class="text-center">
                    <asp:RadioButton ID="rbQ1No" runat="server" GroupName="Q1" Value="0" />
                </td>
                <td class="text-center">
                    <asp:RadioButton ID="rbQ1Unknown" runat="server" GroupName="Q1" Value="2" Checked="true" />
                </td>
            </tr>
            <!-- Q2 -->
            <tr>
                <td>2</td>
                <td>是否盡可能縮短在此海域停留時間</td>
                <td class="text-center">
                    <asp:RadioButton ID="rbQ2Yes" runat="server" GroupName="Q2" Value="1" />
                </td>
                <td class="text-center">
                    <asp:RadioButton ID="rbQ2No" runat="server" GroupName="Q2" Value="0" />
                </td>
                <td class="text-center">
                    <asp:RadioButton ID="rbQ2Unknown" runat="server" GroupName="Q2" Value="2" Checked="true" />
                </td>
            </tr>
            <!-- Q3 -->
            <tr>
                <td>3</td>
                <td>是否已減少/移動作業站位，並盡可能向本國區域靠攏</td>
                <td class="text-center">
                    <asp:RadioButton ID="rbQ3Yes" runat="server" GroupName="Q3" Value="1" />
                </td>
                <td class="text-center">
                    <asp:RadioButton ID="rbQ3No" runat="server" GroupName="Q3" Value="0" />
                </td>
                <td class="text-center">
                    <asp:RadioButton ID="rbQ3Unknown" runat="server" GroupName="Q3" Value="2" Checked="true" />
                </td>
            </tr>
            <!-- Q4 -->
            <tr>
                <td>4</td>
                <td>若遭他國干擾，是否確切知曉通報海巡署及相關救援單位的方式</td>
                <td class="text-center">
                    <asp:RadioButton ID="rbQ4Yes" runat="server" GroupName="Q4" Value="1" />
                </td>
                <td class="text-center">
                    <asp:RadioButton ID="rbQ4No" runat="server" GroupName="Q4" Value="0" />
                </td>
                <td class="text-center">
                    <asp:RadioButton ID="rbQ4Unknown" runat="server" GroupName="Q4" Value="2" Checked="true" />
                </td>
            </tr>
            <!-- Q5 -->
            <tr>
                <td>5</td>
                <td>若遭他國強烈干擾(如欲強行登/扣船)，是否有應變措施(如勾選「是」，請於下方或另頁說明詳細應變措施。)</td>
                <td class="text-center">
                    <asp:RadioButton ID="rbQ5Yes" runat="server" GroupName="Q5" Value="1" />
                </td>
                <td class="text-center">
                    <asp:RadioButton ID="rbQ5No" runat="server" GroupName="Q5" Value="0" />
                </td>
                <td class="text-center">
                    <asp:RadioButton ID="rbQ5Unknown" runat="server" GroupName="Q5" Value="2" Checked="true" />
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
    .tag-group {
        display: flex;
        flex-wrap: wrap;
        gap: 0.5rem;
    }

    /* 限制單個標籤寬度和溢出處理 */
    .tag {
        overflow: hidden;
        display: inline-flex;
        line-height: 1.2;
    }

    /* 標籤文字區域 */
    .tag-content {
        flex: 1;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
        max-width: calc(100% - 20px);
        line-height: 1.2;
        display: inline-block;
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

    /* CheckBoxList 樣式 */
    .form-check input[type="checkbox"] {
        margin-right: 0.5rem;
    }

    .form-check label {
        margin-right: 1.5rem;
    }
</style>

<!-- jQuery Timepicker CSS -->
<link rel="stylesheet" href="//cdnjs.cloudflare.com/ajax/libs/timepicker/1.3.5/jquery.timepicker.min.css">

<script type="text/javascript">
    // 樣式調整
    $(function () {
        function applyClasses() {
            $('input[type=radio]')
                .addClass('form-check-input');
            $('#<%= cblRiskCategories.ClientID %> input[type=checkbox]')
                .addClass('form-check-input');

        }
        applyClasses();
    });

    document.addEventListener('DOMContentLoaded', function () {
        // 檔案上傳按鈕啟用/停用
        var fu = document.getElementById('<%= fuVesselFile.ClientID %>');
        var btn = document.getElementById('<%= btnUpload.ClientID %>');

        // 一開始檢查一次
        btn.disabled = !fu.files.length;

        // 檔案選擇變更時切換 disabled
        fu.addEventListener('change', function () {
            btn.disabled = !this.files.length;
        });

        // 計算作業天數
        var startDateInput = document.getElementById('<%= txtStartDate.ClientID %>');
        var endDateInput = document.getElementById('<%= txtEndDate.ClientID %>');
        var durationInput = document.getElementById('<%= txtDurationDays.ClientID %>');

        function calculateDuration() {
            var startValue = startDateInput.value;
            var endValue = endDateInput.value;

            if (startValue && endValue) {
                // 轉換民國年到西元年
                var startParts = startValue.split('/');
                var endParts = endValue.split('/');

                if (startParts.length === 3 && endParts.length === 3) {
                    var startDate = new Date(parseInt(startParts[0]) + 1911, parseInt(startParts[1]) - 1, parseInt(startParts[2]));
                    var endDate = new Date(parseInt(endParts[0]) + 1911, parseInt(endParts[1]) - 1, parseInt(endParts[2]));

                    if (!isNaN(startDate) && !isNaN(endDate) && endDate >= startDate) {
                        var timeDiff = endDate - startDate;
                        var daysDiff = Math.ceil(timeDiff / (1000 * 60 * 60 * 24)) + 1; // +1 包含起始日
                        durationInput.value = daysDiff;
                    } else {
                        durationInput.value = '';
                    }
                } else {
                    durationInput.value = '';
                }
            } else {
                durationInput.value = '';
            }
        }

        // 監聽日期變更
        startDateInput.addEventListener('change', calculateDuration);
        endDateInput.addEventListener('change', calculateDuration);
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
            link.download = "VesselRiskMapFeatures_" + timestamp + ".wkt";

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
        // 1. 先取得 iframe 元素
        var iframe = document.getElementById("mapFrame");
        if (!iframe || !iframe.contentWindow) {
            document.getElementById("hdnGeo3826WKT").value = "";
            return;
        }

        // 檢查 iframe 是否已經載入完成
        try {
            // 2. 呼叫 Map.aspx 裡面的 getAllFeaturesWKT3826()
            var wkt3826 = iframe.contentWindow.getAllFeaturesWKT3826();
            // 3. 把結果設定到隱藏欄位
            document.getElementById("hdnGeo3826WKT").value = wkt3826 || "";
        } catch (e) {
            console.error("無法取得地圖資料:", e);
            var hdnField = document.getElementById("hdnGeo3826WKT");
            if (!hdnField.value) {
                hdnField.value = "";
            }
        }
    }

    // 開啟圖台時觸發
    var mapModalEl = document.getElementById('mapModal');
    var mapIframeLoaded = false;

    mapModalEl.addEventListener('show.bs.modal', function (e) {
        var button = e.relatedTarget;
        var assessmentId = button.getAttribute('data-assessmentid');
        var iframe = document.getElementById('mapFrame');

        // 檢查 iframe 是否已經載入過相同的 assessmentId
        var currentAssessmentId = iframe.getAttribute('data-current-assessmentid');

        // 只有在第一次或 assessmentId 不同時才重新載入
        if (!mapIframeLoaded || currentAssessmentId !== assessmentId) {
            var urlBase = '<%= ResolveUrl("~/Map.aspx") %>';
            iframe.src = urlBase + '?codes=02,05,01,04,08&assessmentId=' + assessmentId;
            iframe.setAttribute('data-current-assessmentid', assessmentId);
            mapIframeLoaded = true;
        }
    });

    // 刪除檔案
    function deleteFile(index) {
        var hfDelFileIndex = document.getElementById('<%= hfDelFileIndex.ClientID %>');
        var btnDelFile = document.getElementById('<%= btnDelFile.ClientID %>');
        hfDelFileIndex.value = index;
        btnDelFile.click();
    }    
</script>

<!-- jQuery Timepicker JS -->
<script src="//cdnjs.cloudflare.com/ajax/libs/timepicker/1.3.5/jquery.timepicker.min.js"></script>

<script type="text/javascript">
    // 初始化時間選擇器
    $(document).ready(function() {
        // 設定時間選擇器
        $('#<%= txtStartTime.ClientID %>').timepicker({
            timeFormat: 'HH:mm',
            interval: 15,
            minTime: '00:00',
            maxTime: '23:45',
            defaultTime: '08:00',
            startTime: '00:00',
            dynamic: false,
            dropdown: true,
            scrollbar: true
        });
        
        $('#<%= txtEndTime.ClientID %>').timepicker({
            timeFormat: 'HH:mm',
            interval: 15,
            minTime: '00:00',
            maxTime: '23:45',
            defaultTime: '17:00',
            startTime: '00:00',
            dynamic: false,
            dropdown: true,
            scrollbar: true
        });
    });
</script>
