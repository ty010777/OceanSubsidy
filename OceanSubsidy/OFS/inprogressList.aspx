<%@ Page Language="C#" AutoEventWireup="true" CodeFile="inprogressList.aspx.cs" Inherits="inprogressList" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFSMaster.master" EnableViewState="true" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    計畫執行
</asp:Content>


<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <!-- 引用 inprogressList.js -->
    <script src="<%= ResolveUrl("~/script/OFS/inprogressList.js") %>"></script>
</asp:Content>

<asp:Content ID="Breadcrumbs" ContentPlaceHolderID="Breadcrumbs" runat="server">
     <!-- 頁面標題 -->
    <div class="d-flex justify-content-between mb-4">
        <div class="page-title">
            <img src="<%= ResolveUrl("~/assets/img/information-system-title-icon05.svg") %>" alt="logo">
            <div>
                <span>目前位置</span>
                <div class="d-flex align-items-end gap-3">
                    <h2 class="text-teal-dark">計畫執行</h2>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- 公告bar -->
    <div id="news-marquee">
        <news-marquee></news-marquee>
    </div>

    <!-- 查詢 -->
    <div class="search bg-gray mt-4">
        <h3 class="text-teal">
            <i class="fa-solid fa-magnifying-glass"></i>
            查詢
        </h3>

        <!-- 查詢表單 -->
        <div class="search-form">
            <div class="column-2">
                <div class="row g-3">
                    <div class="col-12 col-lg-6">
                        <div class="fs-16 text-gray mb-2">年度</div>
                        <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-select">
                        </asp:DropDownList>
                    </div>
                    <div class="col-12 col-lg-6">
                        <div class="fs-16 text-gray mb-2">類別</div>
                        <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row g-3">
                    <div class="col-12 col-lg-6">
                        <div class="fs-16 text-gray mb-2">申請單位</div>
                        <asp:DropDownList ID="ddlApplyUnit" runat="server" CssClass="form-select">
                        </asp:DropDownList>
                    </div>
                    <div class="col-12 col-lg-6">
                        <div class="fs-16 text-gray mb-2">主管單位</div>
                        <asp:DropDownList ID="ddlSupervisoryUnit" runat="server" CssClass="form-select">
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <div class="column-2">
                <!-- 計畫編號或名稱關鍵字 -->
                <div class="search-item">
                    <div class="fs-16 text-gray mb-2">計畫編號或名稱關鍵字</div>
                    <asp:TextBox ID="txtProjectKeyword" runat="server" CssClass="form-control" placeholder="請輸入計畫編號、計畫名稱相關文字"></asp:TextBox>
                </div>

                <!-- 計畫內容關鍵字 -->
                <div class="search-item">
                    <div class="fs-16 text-gray mb-2">計畫內容關鍵字</div>
                    <asp:TextBox ID="txtContentKeyword" runat="server" CssClass="form-control" placeholder="計畫內容關鍵字"></asp:TextBox>
                </div>
            </div>

            <div class="form-check-input-group d-flex justify-content-center">
                <input id="chkPendingReply" class="form-check-input check-teal" type="checkbox"
                       name="chkPendingReply" runat="server"/>
                <label for="chkPendingReply">待回覆</label>
            </div>

            <asp:Button ID="btnSearch" runat="server" Text="查詢" CssClass="btn btn-teal-dark d-table mx-auto" OnClick="btnSearch_Click" />
        </div>
    </div>

    <!-- 總計列表 -->
    <ul class="total-list tab-white" id="statusTabsList">
        <li class="total-item active" data-status="all">
            <a href="javascript:void(0);">
                <div class="total-item-title">總核定計畫</div>
                <div class="total-item-content">
                    <span class="count" id="countTotal">0</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item" data-status="inprogress">
            <a href="javascript:void(0);">
                <div class="total-item-title">執行中</div>
                <div class="total-item-content">
                    <span class="count" id="countInProgress">0</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item" data-status="overdue">
            <a href="javascript:void(0);">
                <div class="total-item-title">進度落後</div>
                <div class="total-item-content">
                    <span class="count" id="countOverdue">0</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item" data-status="closed">
            <a href="javascript:void(0);">
                <div class="total-item-title">已結案</div>
                <div class="total-item-content">
                    <span class="count" id="countClosed">0</span>
                    <span class="unit">件</span>
                </div>
            </a>
        </li>
        <li class="total-item" data-status="terminated">
            <a href="javascript:void(0);">
                <div class="total-item-title">已終止</div>
                <div class="total-item-content">
                    <span class="count" id="countTerminated">0</span>
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
                <span>共 <span class="text-teal" id="totalRecordsSpan"><asp:Label ID="lblTotalRecords" runat="server" Text="0"></asp:Label></span> 筆資料</span>
            </div>
        </div>

        <div class="table-responsive" style="min-height: 400px;">
            <table class="table teal-table">
                <thead>
                    <tr>
                        <th width="80">年度</th>
                        <th width="80">
                            <div class="hstack align-items-center">
                                <span>類別</span>
                                <button class="sort" data-col="2">
                                    <i class="fa-solid fa-sort-up"></i>
                                    <i class="fa-solid fa-sort-down"></i>
                                </button>
                            </div>
                        </th>
                        <th width="150">
                            <div class="hstack align-items-center">
                                <span>計畫編號</span>
                                <button class="sort" data-col="3">
                                    <i class="fa-solid fa-sort-up"></i>
                                    <i class="fa-solid fa-sort-down"></i>
                                </button>
                            </div>
                        </th>
                        <th width="240">
                            <div class="hstack align-items-center">
                                <span>計畫名稱</span>
                                <button class="sort" data-col="4">
                                    <i class="fa-solid fa-sort-up"></i>
                                    <i class="fa-solid fa-sort-down"></i>
                                </button>
                            </div>
                        </th>
                        <th width="240">
                            <div class="hstack align-items-center">
                                <span>執行單位</span>
                                <button class="sort" data-col="5">
                                    <i class="fa-solid fa-sort-up"></i>
                                    <i class="fa-solid fa-sort-down"></i>
                                </button>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-center">
                                <span>完成狀態</span>
                                <button class="sort" data-col="6">
                                    <i class="fa-solid fa-sort-up"></i>
                                    <i class="fa-solid fa-sort-down"></i>
                                </button>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-center">
                                <span>待辦事項</span>
                            </div>
                        </th>
                        <th>功能</th>
                    </tr>
                </thead>
                <tbody id="tableBody">
                    <!-- 資料列將由 JavaScript 動態產生 -->
                </tbody>
            </table>
        </div>

        <!-- 前端分頁系統 -->
        <div class="d-flex align-items-center justify-content-between flex-wrap gap-2">
            <nav class="pagination justify-content-start" aria-label="Pagination" id="paginationNav">
                <button type="button" id="btnPrevPage" class="nav-button" aria-label="Previous page">‹</button>
                <!-- 動態分頁按鈕將在這裡插入 -->
                <button type="button" id="btnNextPage" class="nav-button" aria-label="Next page">›</button>
            </nav>

            <div class="page-number-control">
                <div class="page-number-control-item">
                    <span>跳到</span>
                    <select id="ddlGoToPage" class="form-select jump-to-page">
                        <!-- 選項將由 JavaScript 動態生成 -->
                    </select>
                    <span>頁</span>
                    <span>,</span>
                </div>
                <div class="page-number-control-item">
                    <span>每頁顯示</span>
                    <select id="ddlPageSize" class="form-select page-size-selector">
                        <option value="5" selected>5</option>
                        <option value="10" >10</option>
                        <option value="20">20</option>
                        <option value="30">30</option>
                        <option value="50">50</option>
                    </select>
                    <span>筆</span>
                </div>
                <div class="pagination-info ms-3 text-muted small" id="paginationInfo">
                    <!-- 分頁資訊將顯示在這裡 -->
                </div>
            </div>
        </div>
    </div>

    <!-- modal 計畫執行列表 審查意見回覆 -->
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
                        <button class="btn btn-teal-dark" type="button"><i class="fas fa-download"></i>匯出申請資料</button>
                    </div>

                    <div class="bg-light-gray p-3 mb-5 mt-3">
                        <ul class="lh-lg">
                            <li>
                                <span class="text-gray">年度 :</span>
                                <span>114</span>
                            </li>
                            <li>
                                <span class="text-gray">計畫編號 :</span>
                                <span>1140023</span>
                            </li>
                            <li>
                                <span class="text-gray">計畫類別 :</span>
                                <span>科專</span>
                            </li>
                            <li>
                                <span class="text-gray">審查組別 : </span>
                                <span>環境工程</span>
                            </li>
                            <li>
                                <span class="text-gray">計畫名稱 : </span>
                                <span>海洋環境監測與預警系統建置計畫</span>
                            </li>
                            <li>
                                <span class="text-gray">申請單位 : </span>
                                <span>國家海洋研究院環境監測中心</span>
                            </li>
                        </ul>
                    </div>

                    <div class="d-flex justify-content-between">
                        <h5 class="square-title">實質審查意見回覆</h5>
                        <button class="btn btn-teal-dark" type="button"><i class="fas fa-download"></i>匯出審查意見回覆表</button>
                    </div>
                    <div class="table-responsive mt-3">
                        <table class="table align-middle gray-table lh-base">
                            <thead>
                                <tr>
                                    <th width="160"></th>
                                    <th>審查意見</th>
                                    <th width="50%">申請單位回覆</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>委員A</td>
                                    <td>意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見內容意見</td>
                                    <td>
                                        <span class="form-control textarea" role="textbox" contenteditable="" data-placeholder="請輸入" aria-label="文本輸入區域" style="height: auto; overflow-y: hidden;"></span>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <div class="d-flex gap-3 flex-wrap justify-content-center mt-4">
                        <button type="button" class="btn btn-outline-teal">
                            暫存
                        </button>
                        <button type="button" class="btn btn-teal">
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
