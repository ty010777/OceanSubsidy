<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReportManage.ascx.cs" Inherits="OSI_ReportManage" %>

<!-- 1) 查詢區 -->
<asp:UpdatePanel ID="upSearch" runat="server" UpdateMode="Always">
    <ContentTemplate>
        <div class="search">
            <h3>
                <i class="fa-solid fa-magnifying-glass"></i>
                查詢
            </h3>
            <div class="search-form">
                <div class="column-2">
                    <!-- 填報週期 -->
                    <div class="search-item">
                        <div class="fs-16 text-gray mb-2">填報週期</div>
                        <div class="row">
                            <div class="col-12 col-lg-6">
                                <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlYear_SelectedIndexChanged" />
                            </div>
                            <div class="col-12 col-lg-6">
                                <asp:DropDownList ID="ddlQuarter" runat="server" CssClass="form-select" Enabled="false" />
                            </div>
                        </div>
                    </div>
                    <!-- 填報機關 -->
                    <div class="search-item">
                        <div class="fs-16 text-gray mb-2">填報機關</div>
                        <div class="row">
                            <div class="col-12 col-lg-12">
                                <asp:DropDownList ID="ddlUnit" runat="server" CssClass="form-select" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="column-2">
                    <!-- 活動性質 -->
                    <div class="search-item">
                        <div class="fs-16 text-gray mb-2">活動性質</div>
                        <div class="row">
                            <div class="col-12 col-lg-12">
                                <asp:DropDownList ID="ddlActivityNatures" runat="server" CssClass="form-select" />
                            </div>
                        </div>
                    </div>
                    <!-- 載具類別 -->
                    <div class="search-item">
                        <div class="fs-16 text-gray mb-2">載具類別</div>
                        <div class="row">
                            <div class="col-12 col-lg-12">
                                <asp:DropDownList ID="ddlCarrierTypes" runat="server" CssClass="form-select" />
                            </div>
                        </div>
                    </div>
                </div>
                <!-- 研究調查項目 -->
                <div class="search-item">
                    <div class="fs-16 text-gray mb-2">研究調查項目</div>
                    <div class="row">
                        <div class="col-12 col-lg-12">
                            <asp:DropDownList ID="ddlResearchItems" runat="server" CssClass="form-select" />
                        </div>
                    </div>
                </div>
                <!-- 關鍵字 -->
                <div class="search-item">
                    <div class="fs-16 text-gray mb-2">關鍵字</div>
                    <asp:TextBox ID="txtKeySearch" runat="server" CssClass="form-control" />
                    <div class="d-flex align-items-start gap-1 mt-3 text-blue">
                        <span class="text-nowrap">關鍵字搜尋欄位：</span>
                        <div class="d-flex flex-wrap gap-1">
                            <span>活動名稱</span>,
                           
                            <span>活動性質補充說明</span>,
                            <span>活動執行者</span>,
                            <span>載具核准文號</span>,
                            <span>研究調查項目補充說明</span>,
                            <span>研究調查儀器</span>
                            <span>研究調查活動內容概述</span>

                        </div>
                    </div>
                </div>
                <!-- 查詢按鈕 -->
                <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-blue d-table mx-auto" Text="查詢" OnClick="btnSearch_Click" />
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>


<!-- 2) 列表區 + 新增按鈕 -->
<div class="block">
    <div class="title">
        <h4>
            <img src="<%= ResolveUrl("~/assets/img/title-icon02.svg") %>" alt="logo" />
            列表
        </h4>
        <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-cyan" OnClick="btnAdd_Click">
            <i class="fa-solid fa-plus"></i>
            新增單筆
        </asp:LinkButton>
    </div>

    <asp:UpdatePanel ID="upList" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div class="table-responsive">
                <asp:ListView ID="lvReports" runat="server"
                    DataKeyNames="ReportID"
                    OnItemCommand="lvReports_ItemCommand"
                    OnItemDataBound="lvReports_ItemDataBound"
                    OnPagePropertiesChanging="lvReports_PagePropertiesChanging">
                    <LayoutTemplate>
                        <table class="table cyan-table">
                            <thead>
                                <tr>
                                    <th width="50">排序</th>
                                    <th width="100">填報週期</th>
                                    <th width="150">填報機關</th>
                                    <th width="300">活動名稱</th>
                                    <th width="200">活動性質</th>
                                    <th>活動執行者</th>
                                    <th width="150">研究調查日期</th>
                                    <th width="150">資料更新日期</th>
                                    <th>活動空間範圍</th>
                                    <th width="100">功能</th>
                                </tr>
                            </thead>
                            <tbody id="itemPlaceholder" runat="server"></tbody>
                        </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%# Container.DisplayIndex + 1 + (dpReports.StartRowIndex) %></td>
                            <td data-th="填報週期:"><%# Eval("Period") %></td>
                            <td data-th="填報機關:">
                                <asp:Literal ID="litReportingUnit" runat="server" Text='<%# Eval("ReportingUnit") %>' /></td>
                            <td data-th="活動名稱:" style="text-align: left;"><%# Eval("ActivityName") %></td>
                            <td data-th="活動性質:" style="text-align: left;"><%# Eval("NatureName") %></td>
                            <td data-th="活動執行者:" style="text-align: left;"><%# Eval("Executors") %></td>
                            <td data-th="研究調查日期:"><%# Eval("StartDateDisplay","{0:yyy/MM/dd}") %> 至 <%# Eval("EndDateDisplay","{0:yyy/MM/dd}") %></td>
                            <td data-th="資料更新日期:"><%# Eval("LastUpdatedDisplay","{0:yyy/MM/dd}") %></td>
                            <td data-th="活動空間範圍:">
                                <asp:PlaceHolder ID="phGeo" runat="server" />
                            </td>
                            <td data-th="功能:">
                                <div class="d-flex d-md-grid gap-2 justify-content-center">
                                    <asp:LinkButton runat="server" CommandName="EditReport" CommandArgument='<%# Eval("ReportID") %>' CssClass="btn btn-sm btn-outline-blue me-1">編輯</asp:LinkButton>
                                    <asp:LinkButton runat="server" CommandName="AskDelete" CommandArgument='<%# Eval("ReportID") %>' CssClass="btn btn-sm btn-outline-pink">刪除</asp:LinkButton>
                                </div>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:ListView>
            </div>

            <!-- 分頁器 -->
            <nav class="pagination d-flex gap-1 justify-content-center mt-3" aria-label="Pagination">
                <asp:DataPager
                    ID="dpReports" runat="server"
                    PagedControlID="lvReports"
                    PageSize="10"
                    OnPreRender="dpReports_PreRender">
                    <Fields>
                        <asp:NextPreviousPagerField
                            ButtonType="Button"
                            ButtonCssClass="nav-button"
                            ShowPreviousPageButton="True"
                            PreviousPageText="‹"
                            ShowNextPageButton="False"
                            ShowFirstPageButton="False"
                            ShowLastPageButton="False" />

                        <asp:NumericPagerField
                            ButtonType="Button"
                            ButtonCount="5"
                            NumericButtonCssClass="pagination-item"
                            CurrentPageLabelCssClass="pagination-item active" />

                        <asp:NextPreviousPagerField
                            ButtonType="Button"
                            ButtonCssClass="nav-button"
                            ShowPreviousPageButton="False"
                            ShowNextPageButton="True"
                            NextPageText="›"
                            ShowFirstPageButton="False"
                            ShowLastPageButton="False" />
                    </Fields>
                </asp:DataPager>
            </nav>

        </ContentTemplate>
    </asp:UpdatePanel>
</div>

<!-- 底部匯出區 -->
<div class="block-bottom">
    <asp:Button ID="btnExport" runat="server" CssClass="btn btn-cyan d-table mx-auto" Text="匯出列表資料" OnClick="btnExport_Click" />
</div>

<!-- 刪除確認 Modal -->
<div id="deleteModal" runat="server" class="modal fade" tabindex="-1" data-bs-backdrop="static" aria-labelledby="deleteModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered modal-md">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                    <i class="fa-solid fa-circle-xmark"></i>
                </button>
            </div>
            <div class="modal-body">
                <div class="d-flex flex-column align-items-center mb-5 gap-3">
                    <asp:HiddenField ID="hfDeleteID" runat="server" />
                    <h4 class="text-blue-green fw-bold">確定要刪除此筆資料?</h4>
                </div>

                <div class="d-flex gap-4 flex-wrap justify-content-center">
                    <button type="button" class="btn btn-gray" data-bs-dismiss="modal">
                        取消</button>
                    <asp:Button ID="btnConfirmDeleteUser" runat="server"
                        CssClass="btn btn-cyan"
                        Text="確定" OnClick="btnConfirmDelete_Click" />
                </div>
            </div>
        </div>
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

<script type="text/javascript">
    window.showModal = function (id) {
        var el = document.getElementById(id);
        if (!el) return;
        bootstrap.Modal
            .getOrCreateInstance(el)
            .show();
    };
    window.hideModal = function (id) {
        var el = document.getElementById(id);
        if (!el) return;
        bootstrap.Modal
            .getOrCreateInstance(el)
            .hide();
    };

    // 開啟圖台時觸發
    var mapModalEl = document.getElementById('mapModal');
    mapModalEl.addEventListener('show.bs.modal', function (e) {
        // e.relatedTarget 是觸發這個 Modal 的按鈕節點
        var button = e.relatedTarget;
        // 從按鈕上讀 data-reportid
        var reportId = button.getAttribute('data-reportid');
        // 把這顆 reportId 帶進 iframe 的 src
        var urlBase = '<%= ResolveUrl("~/Map.aspx") %>';
        var iframe = document.getElementById('mapFrame');
        iframe.src = urlBase + '?codes=02,05,01,04&reportId=' + reportId;
    });
</script>

