<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VesselRiskManage.ascx.cs" Inherits="OSI_VesselRiskManage" %>

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
                    <!-- 預定作業年度 -->
                    <div class="search-item">
                        <div class="fs-16 text-gray mb-2">預定作業年度</div>
                        <div class="row">
                            <div class="col-12 col-lg-6">
                                <asp:DropDownList ID="ddlStartYear" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlStartYear_SelectedIndexChanged" />
                            </div>
                            <div class="col-12 col-lg-6">
                                <asp:DropDownList ID="ddlEndYear" runat="server" CssClass="form-select" />
                            </div>
                        </div>
                    </div>
                    <!-- 關鍵字 -->
                    <div class="search-item">
                        <div class="fs-16 text-gray mb-2">關鍵字搜尋</div>
                        <asp:TextBox ID="txtKeySearch" runat="server" CssClass="form-control" placeholder="可搜尋計畫名稱、申請單位" />
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
            新增
        </asp:LinkButton>
    </div>

    <asp:UpdatePanel ID="upList" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div class="table-responsive">
                <asp:ListView ID="lvVesselRisk" runat="server"
                    DataKeyNames="AssessmentId"
                    OnItemCommand="lvVesselRisk_ItemCommand"
                    OnItemDataBound="lvVesselRisk_ItemDataBound"
                    OnPagePropertiesChanging="lvVesselRisk_PagePropertiesChanging">
                    <LayoutTemplate>
                        <table class="table cyan-table">
                            <thead>
                                <tr>
                                    <th width="50">排序</th>
                                    <th width="150">計畫類別</th>
                                    <th width="350">計畫名稱</th>
                                    <th width="200">預定作業期間</th>
                                    <th width="100">計畫主持人</th>
                                    <th>申請單位</th>
                                    <th width="150">資料更新日期</th>
                                    <th width="100">功能</th>
                                </tr>
                            </thead>
                            <tbody id="itemPlaceholder" runat="server"></tbody>
                        </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%# Container.DisplayIndex + 1 + (dpVesselRisk.StartRowIndex) %></td>
                            <td data-th="計畫類別:">
                                <asp:Literal ID="litProjectType" runat="server" /></td>
                            <td data-th="計畫名稱:" style="text-align: left;"><%# Eval("Title") %></td>
                            <td data-th="預定作業期間:"><%# Eval("StartDateDisplay","{0:yyy/MM/dd}") %> 至<br />
                                <%# Eval("EndDateDisplay","{0:yyy/MM/dd}") %></td>
                            <td data-th="計畫主持人:" style="text-align: left;"><%# Eval("Investigator") %></td>
                            <td data-th="申請單位:" style="text-align: left;"><%# Eval("Unit") %></td>
                            <td data-th="資料更新日期:"><%# Eval("LastUpdatedDisplay","{0:yyy/MM/dd}") %></td>
                            <td data-th="功能:">
                                <div class="d-flex d-md-grid gap-2 justify-content-center">
                                    <asp:LinkButton runat="server" CommandName="EditRecord" CommandArgument='<%# Eval("AssessmentId") %>' CssClass="btn btn-sm btn-outline-blue me-1">編輯</asp:LinkButton>
                                    <asp:LinkButton runat="server" CommandName="AskDelete" CommandArgument='<%# Eval("AssessmentId") %>' CssClass="btn btn-sm btn-outline-pink">刪除</asp:LinkButton>
                                </div>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <EmptyDataTemplate>
                        <table class="table cyan-table">
                            <thead>
                                <tr>
                                    <th width="50">排序</th>
                                    <th width="150">計畫類別</th>
                                    <th width="350">計畫名稱</th>
                                    <th width="200">預定作業期間</th>
                                    <th width="100">計畫主持人</th>
                                    <th>申請單位</th>
                                    <th width="150">資料更新日期</th>
                                    <th width="100">功能</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td colspan="8" style="text-align: center;">查無資料</td>
                                </tr>
                            </tbody>
                        </table>
                    </EmptyDataTemplate>
                </asp:ListView>
            </div>

            <!-- 分頁器 -->
            <nav class="pagination d-flex gap-1 justify-content-center mt-3" aria-label="Pagination">
                <asp:DataPager
                    ID="dpVesselRisk" runat="server"
                    PagedControlID="lvVesselRisk"
                    PageSize="10"
                    OnPreRender="dpVesselRisk_PreRender">
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
                    <asp:Button ID="btnConfirmDelete" runat="server"
                        CssClass="btn btn-cyan"
                        Text="確定" OnClick="btnConfirmDelete_Click" />
                </div>
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
</script>
