<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ApplicationChecklist.aspx.cs" Inherits="OFS_ApplicationChecklist" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFSMaster.master" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    計畫申請 - 申請案件清單
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <script src="<%= ResolveUrl("~/script/OFS/ApplicationChecklist.js") %>"></script>
</asp:Content>

<asp:Content ID="Breadcrumbs" ContentPlaceHolderID="Breadcrumbs" runat="server">
     <div class="page-title">
            <img src="<%= ResolveUrl("~/assets/img/information-system-title-icon04.svg") %>" alt="logo">
            <div>
                <span>目前位置</span>
                <div class="d-flex align-items-end gap-3">
                    <h2 class="text-teal-dark">計畫申請</h2>
                </div>
            </div>
        </div>
</asp:Content>
<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hidSelectedStage" runat="server" />
    <asp:Button ID="btnStageFilter" runat="server" OnClick="btnStageFilter_Click" style="display: none;" />

    <!-- 頁面標題 -->


    <!-- 公告提醒 -->
    <div id="news-marquee">
        <news-marquee></news-marquee>
    </div>

    <!-- 搜尋表單 -->
    <div class="search bg-gray mt-4">
        <h3 class="text-teal">
            <i class="fa-solid fa-magnifying-glass"></i>
            查詢
        </h3>

        <div class="search-form">
            <div class="column-2">
                <!-- 計畫編號或名稱關鍵字 -->
                <div class="search-item">
                    <div class="fs-16 text-gray mb-2">計畫編號或名稱關鍵字</div>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="請輸入計畫編號、計畫名稱相關文字"></asp:TextBox>
                </div>

                <!-- 計畫內容關鍵字 -->
                <div class="search-item">
                    <div class="fs-16 text-gray mb-2">計畫內容關鍵字</div>
                    <asp:TextBox ID="txtContentKeyword" runat="server" CssClass="form-control" placeholder="計畫內容關鍵字"></asp:TextBox>
                </div>
            </div>

            <div class="column-2">
                <div class="row g-3">
                    <div class="col-12 col-lg-4">
                        <div class="fs-16 text-gray mb-2">年度</div>
                        <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-select">
                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                            <asp:ListItem Text="115年" Value="115"></asp:ListItem>
                            <asp:ListItem Text="114年" Value="114"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-12 col-lg-4">
                        <div class="fs-16 text-gray mb-2">階段</div>
                        <asp:DropDownList ID="ddlStage" runat="server" CssClass="form-select">
                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                            <asp:ListItem Text="尚未提送" Value="尚未提送"></asp:ListItem>
                            <asp:ListItem Text="資格審查" Value="資格審查"></asp:ListItem>
                            <asp:ListItem Text="領域審查" Value="領域審查"></asp:ListItem>
                            <asp:ListItem Text="技術審查" Value="技術審查"></asp:ListItem>
                            <asp:ListItem Text="決審核定" Value="決審核定"></asp:ListItem>
                            <asp:ListItem Text="計畫執行" Value="計畫執行"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-12 col-lg-4">
                        <div class="fs-16 text-gray mb-2">狀態</div>
                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                            <asp:ListItem Text="審核中" Value="審核中"></asp:ListItem>
                            <asp:ListItem Text="補正補件" Value="補正補件"></asp:ListItem>
                            <asp:ListItem Text="逾期未補" Value="逾期未補"></asp:ListItem>
                            <asp:ListItem Text="未通過" Value="未通過"></asp:ListItem>
                            <asp:ListItem Text="通過" Value="通過"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row g-3">
                    <div class="col-12 col-lg-6">
                        <div class="fs-16 text-gray mb-2">申請單位</div>
                        <asp:TextBox ID="txtDepartment" runat="server" CssClass="form-control" placeholder="請輸入申請單位"></asp:TextBox>
                    </div>
                    <div class="col-12 col-lg-6">
                        <div class="fs-16 text-gray mb-2">主管單位</div>
                        <asp:DropDownList ID="ddlReviewer" runat="server" CssClass="form-select">
                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                            <asp:ListItem Text="海洋科技科" Value="海洋科技科"></asp:ListItem>
                            <asp:ListItem Text="海洋文化科" Value="海洋文化科"></asp:ListItem>
                            <asp:ListItem Text="海洋教育科" Value="海洋教育科"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>

            <div class="form-check-input-group d-flex justify-content-center">
                <input id="waitingReply" class="form-check-input check-teal" type="checkbox"
                       name="waitingReply" runat="server"/>
                <label for="waitingReply">待回覆</label>
            </div>

            <asp:Button ID="btnSearch" runat="server" Text="查詢" CssClass="btn btn-teal-dark d-table mx-auto" OnClick="btnSearch_Click" />
        </div>
    </div>

    <!-- 總計列表 -->
    <ul class="total-list tab-white">
        <li class="total-item active">
            <a href="#">
                <div class="total-item-title">總申請</div>
                <div class="total-item-content">
                    <span class="count">0</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item">
            <a href="#">
                <div class="total-item-title">科專</div>
                <div class="total-item-content">
                    <span class="count">0</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item">
            <a href="#">
                <div class="total-item-title">文化</div>
                <div class="total-item-content">
                    <span class="count">0</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item">
            <a href="#">
                <div class="total-item-title">學校民間</div>
                <div class="total-item-content">
                    <span class="count">0</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item">
            <a href="#">
                <div class="total-item-title">學校社團</div>
                <div class="total-item-content">
                    <span class="count">0</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item">
            <a href="#">
                <div class="total-item-title">多元</div>
                <div class="total-item-content">
                    <span class="count">0</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item">
            <a href="#">
                <div class="total-item-title">素養</div>
                <div class="total-item-content">
                    <span class="count">0</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item">
            <a href="#">
                <div class="total-item-title">無障礙</div>
                <div class="total-item-content">
                    <span class="count">0</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
    </ul>

    <!-- 列表內容 -->
    <div class="block rounded-bottom-4">
        <div class="title border-teal">
            <div class="d-flex align-items-center gap-2">
                <h4 class="text-teal">
                    <img src="<%= ResolveUrl("~/assets/img/title-icon02-teal.svg") %>" alt="logo">
                    <span>列表</span>
                </h4>
                <span>共 <asp:Literal ID="litRecordInfo" runat="server" Text="<span class='text-teal'>0</span> 筆資料"></asp:Literal></span>
            </div>

            <button type="button" class="btn btn-teal-dark" data-bs-toggle="modal" data-bs-target="#planApplyModal">
                <i class="fa-solid fa-plus"></i>
                申請計畫
            </button>
        </div>

        <div class="table-responsive" style="min-height: 400px;">
            <table class="table teal-table">
                <thead>
                    <tr>
                        <th width="80">年度</th>
                        <th width="150">
                            <div class="hstack align-items-center">
                                <span>計畫編號</span>
                                <button type="button" class="sort" data-col="2">
                                    <i class="fa-solid fa-sort-up"></i>
                                    <i class="fa-solid fa-sort-down"></i>
                                </button>
                            </div>
                        </th>
                        <th width="240">
                            <div class="hstack align-items-center">
                                <span>計畫名稱</span>
                                <button type="button" class="sort" data-col="3">
                                    <i class="fa-solid fa-sort-up"></i>
                                    <i class="fa-solid fa-sort-down"></i>
                                </button>
                            </div>
                        </th>
                        <th width="200">
                            <div class="hstack align-items-center">
                                <span>申請單位</span>
                                <button type="button" class="sort" data-col="4">
                                    <i class="fa-solid fa-sort-up"></i>
                                    <i class="fa-solid fa-sort-down"></i>
                                </button>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-center">
                                <span>類別</span>
                                <button type="button" class="sort" data-col="5">
                                    <i class="fa-solid fa-sort-up"></i>
                                    <i class="fa-solid fa-sort-down"></i>
                                </button>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-center">
                                <span>申請補助金額</span>
                                <button type="button" class="sort" data-col="6">
                                    <i class="fa-solid fa-sort-up"></i>
                                    <i class="fa-solid fa-sort-down"></i>
                                </button>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-center">
                                <span>階段</span>
                                <button  type="button" class="sort" data-col="7">
                                    <i class="fa-solid fa-sort-up"></i>
                                    <i class="fa-solid fa-sort-down"></i>
                                </button>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-center">
                                <span>狀態</span>
                                <button  type="button" class="sort" data-col="8">
                                    <i class="fa-solid fa-sort-up"></i>
                                    <i class="fa-solid fa-sort-down"></i>
                                </button>
                            </div>
                        </th>
                        <th>功能</th>
                    </tr>
                </thead>
                <tbody id="dataTableBody">
                    <!-- 動態資料將在這裡插入 -->
                    <tr>
                        <td colspan="9" style="text-align: center; padding: 20px;">載入中...</td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!-- 分頁控制 -->
        <div class="d-flex align-items-center justify-content-between flex-wrap gap-2">
            <nav class="pagination justify-content-start" aria-label="Pagination" id="paginationNav">
                <button type="button" id="btnPrevPage" class="nav-button" aria-label="Previous page" onclick="changePage('prev')">‹</button>
                <!-- 動態分頁按鈕將在這裡插入 -->
                <button type="button" id="btnNextPage" class="nav-button" aria-label="Next page" onclick="changePage('next')">›</button>
            </nav>

            <div class="page-number-control">
                <div class="page-number-control-item">
                    <span>跳到</span>
                    <select id="ddlPageNumber" class="form-select" onchange="goToPage(this.value)">
                        <option value="1">1</option>
                    </select>
                    <span>頁</span>
                    <span>,</span>
                </div>
                <div class="page-number-control-item">
                    <span>每頁顯示</span>
                    <select id="ddlPageSize" class="form-select" onchange="changePageSize()">
                        <option value="5" selected>5</option>
                        <option value="10">10</option>
                        <option value="20">20</option>
                        <option value="30">30</option>
                    </select>
                    <span>筆</span>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal 申請計畫 -->
    <div class="modal fade" id="planApplyModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="planApplyModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="fs-24 fw-bold text-green-light">申請計畫</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </div>
                <div class="modal-body">
                    <div class="">
                        <div class="fs-16 text-gray mb-2">申請補助計畫類別</div>
                        <asp:DropDownList ID="ddlModalYear" runat="server" CssClass="form-select" onchange="updateSelectedTypeId()">
                        </asp:DropDownList>
                        <input type="hidden" id="hdnSelectedTypeId" name="hdnSelectedTypeId" value="" />
                    </div>

                    <div class="d-flex gap-4 flex-wrap justify-content-center mt-5">
                        <button type="button" class="btn btn-gray" data-bs-dismiss="modal">
                            取消
                        </button>
                        <asp:Button ID="btnCreateApplication" runat="server" Text="新增" CssClass="btn btn-teal" OnClick="btnCreateApplication_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal 案件歷程 -->
    <div class="modal fade" id="planHistoryModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="planHistoryModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-xl">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="fs-24 fw-bold text-green-light">案件歷程</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </div>
                <div class="modal-body">
                    <div class="table-responsive">
                        <table class="table align-middle gray-table">
                            <thead>
                                <tr>
                                    <th>時間</th>
                                    <th>人員</th>
                                    <th>階段狀態</th>
                                    <th>說明</th>
                                </tr>
                            </thead>
                            <tbody>

                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal 撤案 -->
    <div class="modal fade" id="planWithdrawModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="planWithdrawModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="fs-24 fw-bold text-green-light">確定撤案?</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </div>
                <div class="modal-body">
                    <div class="">
                        <div class="fs-16 text-gray mb-2">撤案原因</div>
                        <asp:TextBox ID="txtWithdrawReason" runat="server" TextMode="MultiLine"
                                    Rows="3" CssClass="form-control" placeholder="請輸入撤案原因"></asp:TextBox>
                        <asp:HiddenField ID="hdnWithdrawProjectId" runat="server" />
                    </div>

                    <div class="d-flex gap-4 flex-wrap justify-content-center mt-5">
                        <button type="button" class="btn btn-gray" data-bs-dismiss="modal">
                            取消
                        </button>
                        <asp:Button ID="btnConfirmWithdraw" runat="server" Text="確認撤案"
                                   CssClass="btn btn-teal" OnClick="btnConfirmWithdraw_Click"
                                   OnClientClick="return validateWithdrawReason();" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal 恢復案件 -->
    <div class="modal fade" id="planRestoreModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="planRestoreModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="fs-24 fw-bold text-green-light">確定恢復案件?</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </div>
                <div class="modal-body">
                    <div class="">
                        <div class="fs-16 text-gray mb-2">恢復原因</div>
                        <asp:TextBox ID="txtRestoreReason" runat="server" TextMode="MultiLine"
                                    Rows="3" CssClass="form-control" placeholder="請輸入恢復案件原因"></asp:TextBox>
                        <asp:HiddenField ID="hdnRestoreProjectId" runat="server" />
                    </div>

                    <div class="d-flex gap-4 flex-wrap justify-content-center mt-5">
                        <button type="button" class="btn btn-gray" data-bs-dismiss="modal">
                            取消
                        </button>
                        <asp:Button ID="btnConfirmRestore" runat="server" Text="確認恢復"
                                   CssClass="btn btn-teal" OnClick="btnConfirmRestore_Click"
                                   OnClientClick="return validateRestoreReason();" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal 上傳技術審查/初審檔案 -->
    <div class="modal fade" id="techReviewUploadModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="techReviewUploadModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="fs-24 fw-bold text-green-light">上傳 技術審查/複審 檔案</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="hdnUploadProjectId" runat="server" />

                    <div class="bg-light-gray p-3 mb-3">
                        <ul class="lh-lg" style="list-style: none; padding-left: 0;">
                            <li>
                                <span class="text-gray">計畫編號：</span>
                                <span id="uploadModalProjectId"></span>
                            </li>
                            <li>
                                <span class="text-gray">計畫名稱：</span>
                                <span id="uploadModalProjectName"></span>
                            </li>
                        </ul>
                    </div>

                    <div class="mb-3">
                        <div class="fs-16 text-gray mb-3">選擇檔案 (支援 PPT, PPTX 格式)</div>
                        <div class="text-muted mb-3">檔案大小限制：50MB</div>
                        <asp:FileUpload ID="fileUploadTechReview" runat="server" CssClass="form-control" accept=".ppt,.pptx" />
                        
                    </div>

                    <div id="currentFileDisplay" class="mb-3" style="display: none;">
                        <div class="d-flex align-items-center gap-2 p-3 bg-light rounded">
                            <i class="fas fa-file-powerpoint text-orange"></i>
                            <span id="currentFileName" class="flex-grow-1"></span>
                            <button type="button" class="btn btn-sm btn-teal" onclick="downloadCurrentFile()">
                                <i class="fas fa-download"></i> 下載
                            </button>
                        </div>
                    </div>

                    <div class="d-flex gap-4 flex-wrap justify-content-center mt-4">
                        <button type="button" class="btn btn-gray" data-bs-dismiss="modal">
                            取消
                        </button>
                        <asp:Button ID="btnUploadTechReview" runat="server" Text="上傳" CssClass="btn btn-teal" OnClick="btnUploadTechReview_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        // 更新選中的 TypeID
        function updateSelectedTypeId() {
            const dropdown = document.getElementById('<%= ddlModalYear.ClientID %>');
            const hiddenField = document.getElementById('hdnSelectedTypeId');

            if (dropdown && hiddenField) {
                const selectedOption = dropdown.options[dropdown.selectedIndex];
                // 從 data-typeid 屬性取得 TypeID
                const typeId = selectedOption.getAttribute('data-typeid');
                hiddenField.value = typeId || '';
            }
        }

        // 當 Modal 開啟時，初始化 TypeID
        document.addEventListener('DOMContentLoaded', function() {
            const planApplyModal = document.getElementById('planApplyModal');
            if (planApplyModal) {
                planApplyModal.addEventListener('shown.bs.modal', function() {
                    updateSelectedTypeId();
                });
            }
        });

        // 處理撤案操作
        function handleWithdraw(projectId) {
            // 設定要撤案的 ProjectID
            setWithdrawProjectId(projectId);
            // 顯示撤案模態框
            const modal = new bootstrap.Modal(document.getElementById('planWithdrawModal'));
            modal.show();
            // 清空原因輸入框
            document.getElementById('<%= txtWithdrawReason.ClientID %>').value = '';
        }

        // 客戶端驗證撤案原因
        function validateWithdrawReason() {
            const reason = document.getElementById('<%= txtWithdrawReason.ClientID %>').value.trim();
            if (!reason) {
                alert('請輸入撤案原因');
                return false;
            }
            return true;
        }

        // 設定撤案的 ProjectID（當開啟撤案 Modal 時呼叫）
        function setWithdrawProjectId(projectId) {
            document.getElementById('<%= hdnWithdrawProjectId.ClientID %>').value = projectId;
        }

        // 處理恢復案件操作
        function handleRestore(projectId) {
            // 設定要恢復的 ProjectID
            setRestoreProjectId(projectId);
            // 顯示恢復案件模態框
            const modal = new bootstrap.Modal(document.getElementById('planRestoreModal'));
            modal.show();
            // 清空原因輸入框
            document.getElementById('<%= txtRestoreReason.ClientID %>').value = '';
        }

        // 客戶端驗證恢復原因
        function validateRestoreReason() {
            const reason = document.getElementById('<%= txtRestoreReason.ClientID %>').value.trim();
            if (!reason) {
                alert('請輸入恢復案件原因');
                return false;
            }
            return true;
        }

        // 設定恢復案件的 ProjectID（當開啟恢復 Modal 時呼叫）
        function setRestoreProjectId(projectId) {
            document.getElementById('<%= hdnRestoreProjectId.ClientID %>').value = projectId;
        }

        // 處理刪除操作
        function handleDelete(projectId) {
            // 設定要刪除的 ProjectID
            setDeleteProjectId(projectId);
            // 顯示刪除模態框
            const modal = new bootstrap.Modal(document.getElementById('planDeleteModal'));
            modal.show();
            // 清空原因輸入框
            document.getElementById('<%= txtDeleteReason.ClientID %>').value = '';
        }

        // 客戶端驗證刪除原因
        function validateDeleteReason() {
            const reason = document.getElementById('<%= txtDeleteReason.ClientID %>').value.trim();
            if (!reason) {
                alert('請輸入刪除原因');
                return false;
            }
            return true;
        }

        // 設定刪除的 ProjectID（當開啟刪除 Modal 時呼叫）
        function setDeleteProjectId(projectId) {
            document.getElementById('<%= hdnDeleteProjectId.ClientID %>').value = projectId;
        }

        // 處理上傳技術審查檔案操作
        function showUploadModal(projectId) {
            // 設定要上傳檔案的 ProjectID
            document.getElementById('<%= hdnUploadProjectId.ClientID %>').value = projectId;

            // 檢查是否已有現有檔案並顯示
            checkExistingFile(projectId);

            // 顯示上傳模態框
            const modal = new bootstrap.Modal(document.getElementById('techReviewUploadModal'));
            modal.show();
        }

        // 檢查現有檔案
        function checkExistingFile(projectId) {
            // 透過 AJAX 檢查是否已有檔案
            $.ajax({
                type: "POST",
                url: "ApplicationChecklist.aspx/CheckTechReviewFile",
                data: JSON.stringify({ projectId: projectId }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(response) {
                    if (response.d && response.d.success) {
                        document.getElementById('uploadModalProjectId').textContent = response.d.ProjectID ?? "";
                        document.getElementById('uploadModalProjectName').textContent = response.d.ProjectName ?? "";

                        if (response.d.hasFile) {
                            // 顯示現有檔案區塊
                            document.getElementById('currentFileDisplay').style.display = 'block';
                            document.getElementById('currentFileName').textContent = response.d.fileName;
                        } else {
                            // 隱藏現有檔案區塊
                            document.getElementById('currentFileDisplay').style.display = 'none';
                        }
                    } else {
                        console.log('檢查檔案時發生錯誤：' + (response.d ? response.d.message : '未知錯誤'));
                        document.getElementById('currentFileDisplay').style.display = 'none';
                    }
                },
                error: function(xhr, status, error) {
                    console.log('AJAX 請求失敗：' + error);
                    document.getElementById('currentFileDisplay').style.display = 'none';
                }
            });
        }

        // 下載目前檔案
        function downloadCurrentFile() {
            const projectId = document.getElementById('<%= hdnUploadProjectId.ClientID %>').value;
            if (projectId) {
                // 建立下載連結
                window.open('../Service/DownloadApplicationChecklistFile.ashx?action=downloadTechReview&projectId=' + encodeURIComponent(projectId), '_blank');
            }
        }
    </script>

    <!-- Modal 刪除計畫 -->
    <div class="modal fade" id="planDeleteModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="planDeleteModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="fs-24 fw-bold text-green-light">確定刪除申請案件?</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </div>
                <div class="modal-body">
                    <div class="">
                        <div class="fs-16 text-gray mb-2">刪除說明</div>
                        <asp:TextBox ID="txtDeleteReason" runat="server" TextMode="MultiLine"
                                    Rows="3" CssClass="form-control" placeholder="請輸入刪除原因"></asp:TextBox>
                        <asp:HiddenField ID="hdnDeleteProjectId" runat="server" />
                    </div>

                    <div class="d-flex gap-4 flex-wrap justify-content-center mt-5">
                        <button type="button" class="btn btn-gray" data-bs-dismiss="modal">
                            取消
                        </button>
                        <asp:Button ID="btnConfirmDelete" runat="server" Text="確認刪除" 1
                                   CssClass="btn btn-teal" OnClick="btnConfirmDelete_Click"
                                   OnClientClick="return validateDeleteReason();" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal 審查意見回覆 -->
    <div class="modal fade" id="planCommentModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="planCommentModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-xl">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="fs-24 fw-bold text-green-light">審查意見回覆</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </div>
                <div class="modal-body">

                    <div class="d-flex justify-content-between">
                        <h5 class="square-title">計畫資料</h5>
                        <button class="btn btn-teal-dark" type="button" onclick="exportApplicationData()"><i class="fas fa-download"></i>匯出申請資料</button>
                    </div>

                    <div class="bg-light-gray p-3 mb-5 mt-3">
                        <ul class="lh-lg">
                            <li>
                                <span class="text-gray">年度 :</span>
                                <span id="projectYearDisplay"></span>
                            </li>
                            <li>
                                <span class="text-gray">計畫編號 :</span>
                                <span id="projectIdDisplay"></span>
                            </li>
                            <li>
                                <span class="text-gray">計畫類別 :</span>
                                <span id="projectCategoryDisplay"></span>
                            </li>
                            <li>
                                <span class="text-gray">審查組別 : </span>
                                <span id="reviewGroupDisplay"></span>
                            </li>
                            <li>
                                <span class="text-gray">計畫名稱 : </span>
                                <span id="projectNameDisplay"></span>
                            </li>
                            <li>
                                <span class="text-gray">申請單位 : </span>
                                <span id="applicantUnitDisplay"></span>
                            </li>
                        </ul>
                    </div>


                    <div class="d-flex justify-content-between">
                        <h5 class="square-title">領域審查意見回覆</h5>
                        <button class="btn btn-teal-dark" type="button" onclick="exportReviewCommentReply('domain')"><i class="fas fa-download"></i>匯出領域審查意見回覆表</button>
                    </div>
                    <div class="table-responsive mt-3">
                        <table class="table align-middle gray-table lh-base">
                            <thead>
                                <tr>
                                    <th width="160">審查委員</th>
                                    <th>審查意見</th>
                                    <th width="50%">申請單位回覆</th>
                                </tr>
                            </thead>
                            <tbody id="domainReviewCommentsTableBody">
                                <!-- 領域審查動態內容將在這裡插入 -->
                            </tbody>
                        </table>
                    </div>

                    <div class="d-flex justify-content-between mt-5">
                        <h5 class="square-title">技術審查意見回覆</h5>
                        <button class="btn btn-teal-dark" type="button" onclick="exportReviewCommentReply('technical')"><i class="fas fa-download"></i>匯出技術審查意見回覆表</button>
                    </div>
                    <div class="table-responsive mt-3">
                        <table class="table align-middle gray-table lh-base">
                            <thead>
                                <tr>
                                    <th width="160">審查委員</th>
                                    <th>審查意見</th>
                                    <th width="50%">申請單位回覆</th>
                                </tr>
                            </thead>
                            <tbody id="technicalReviewCommentsTableBody">
                                <!-- 技術審查動態內容將在這裡插入 -->
                            </tbody>
                        </table>
                    </div>


                    <div class="d-flex gap-3 flex-wrap justify-content-center mt-4">
                        <button type="button" class="btn btn-teal" onclick="submitReply()">
                            <i class="fas fa-check"></i>
                            提送回覆
                        </button>
                    </div>

                </div>

            </div>
        </div>
    </div>
    <script>
        startVueApp("#news-marquee");
    </script>
</asp:Content>
