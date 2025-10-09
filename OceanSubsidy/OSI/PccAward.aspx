<%@ Page Language="C#" MasterPageFile="~/OSI/OSIMaster.master" AutoEventWireup="true" CodeFile="PccAward.aspx.cs" Inherits="OSI_PccAward" %>

<asp:Content ID="cTitle" ContentPlaceHolderID="TitleContent" runat="server">
    標案計畫查詢  | 海洋科學調查活動填報系統
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            var hf = document.getElementById('<%= hfActiveTab.ClientID %>');
            document.querySelectorAll('.tab-link[data-bs-toggle="tab"]')
                .forEach(function (t) {
                    t.addEventListener('shown.bs.tab', function (e) {
                        hf.value = e.target.getAttribute('data-bs-target');
                    });
                });
        });
    </script>
    <style>
        .pagination > span {
            display: flex;
            gap: 4px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Bread" runat="server">
    <img src="<%= ResolveUrl("~/assets/img/title-icon06.svg") %>" alt="logo">
    <div>
        <span>目前位置</span>
        <h2>標案計畫查詢</h2>
    </div>
</asp:Content>

<asp:Content ID="cMain" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hfActiveTab" runat="server" />
    <!-- TAB -->
    <nav>
        <div class="tab" role="tablist">
            <button class="tab-link active" data-bs-toggle="tab" data-bs-target="#tabGovContract" type="button">政府標案</button>
            <button class="tab-link" data-bs-toggle="tab" data-bs-target="#tabNSTCGrant" type="button">國科會補助研究計畫</button>
        </div>
    </nav>

    <div class="tab-content">
        <!-- 政府標案查詢 分頁 -->
        <div id="tabGovContract" class="tab-pane fade show active">
            <!-- 搜尋 -->
            <div class="search">
                <h3>
                    <i class="fa-solid fa-magnifying-glass"></i>
                    查詢
                </h3>

                <div class="search-form">
                    <div class="column-2">
                        <!-- 決標公告 -->
                        <div class="search-item">
                            <div class="fs-16 text-gray mb-2">決標公告</div>
                            <div class="input-group">
                                <asp:TextBox ID="txtAwardDateFrom" runat="server" CssClass="form-control rocDate" TextMode="SingleLine" placeholder="開始日期" />
                                <span class="input-group-text">至</span>
                                <asp:TextBox ID="txtAwardDateTo" runat="server" CssClass="form-control rocDate" TextMode="SingleLine" placeholder="結束日期" />
                            </div>
                        </div>

                        <!-- 決標金額 -->
                        <div class="search-item">
                            <div class="fs-16 text-gray mb-2">決標金額</div>
                            <div class="input-group">
                                <asp:TextBox ID="txtPriceFrom" runat="server" CssClass="form-control" TextMode="Number" placeholder="最小金額" />
                                <span class="input-group-text">至</span>
                                <asp:TextBox ID="txtPriceTo" runat="server" CssClass="form-control" TextMode="Number" placeholder="最大金額" />
                            </div>
                        </div>
                    </div>

                    <!-- 標案名稱 -->
                    <div class="search-item">
                        <div class="fs-16 text-gray mb-2">標案名稱</div>
                        <asp:TextBox ID="txtKeyword" runat="server" CssClass="form-control" placeholder="請輸入關鍵字，如:海洋、海域等" />
                        <div class="d-flex align-items-start gap-1 mt-3 text-blue">
                            <span class="text-nowrap">建議關鍵字：</span>
                            <div class="d-flex flex-wrap gap-1">
                                <a class="link" href="#" onclick="setKeyword('海洋'); return false;">海洋</a>,
                        <a class="link" href="#" onclick="setKeyword('海域'); return false;">海域</a>
                            </div>
                        </div>
                    </div>

                    <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-blue d-table mx-auto"
                        OnClick="btnSearch_Click">
                        <i class="fa-solid fa-magnifying-glass"></i>
                        查詢
                    </asp:LinkButton>
                </div>
            </div>

            <!-- 列表內容 -->
            <asp:UpdatePanel ID="upList" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="block rounded-bottom-4">
                        <div class="title">
                            <h4>
                                <img src="<%= ResolveUrl("~/assets/img/title-icon02.svg") %>" alt="logo">
                                列表 (資料最後更新日期 :
                                <asp:Label ID="lblLatestAwardDate" runat="server" Text="載入中..."></asp:Label>)
                            </h4>
                        </div>

                        <div class="table-responsive">
                            <asp:ListView ID="lvPccAward" runat="server"
                                DataKeyNames="Id"
                                OnItemCommand="lvPccAward_ItemCommand"
                                OnPagePropertiesChanging="lvPccAward_PagePropertiesChanging">
                                <LayoutTemplate>
                                    <table class="table cyan-table">
                                        <thead>
                                            <tr>
                                                <th width="80">項次</th>
                                                <th width="200" style="text-align:left" >機關名稱</th>
                                                <th width="360" style="text-align:left">標案名稱</th>
                                                <th style="text-align:right">決標金額</th>
                                                <th>決標日</th>
                                                <th width="80">功能</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr id="itemPlaceholder" runat="server"></tr>
                                        </tbody>
                                    </table>
                                </LayoutTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td data-th="排序:"><%# Container.DisplayIndex + 1 + (dpPccAward.StartRowIndex) %></td>
                                        <td data-th="機關名稱:" style="text-align:left"><%# Eval("OrgName") %></td>
                                        <td data-th="標案名稱:" style="text-align:left"><%# Eval("AwardName") %></td>
                                        <td data-th="決標金額:" style="text-align:right"><%# FormatPrice(Eval("AwardPrice")) %></td>
                                        <td data-th="決標日:"><%# FormatRocDate(Eval("AwardDate")) %></td>
                                        <td data-th="功能:">
                                            <div class="d-flex flex-wrap justify-content-center gap-2">
                                                <asp:LinkButton ID="btnDetail" runat="server"
                                                    CssClass="btn btn-sm btn-outline-blue"
                                                    CommandName="ViewDetail"
                                                    CommandArgument='<%# Eval("Url") %>'>明細</asp:LinkButton>
                                            </div>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <EmptyDataTemplate>
                                    <table class="table cyan-table">
                                        <thead>
                                            <tr>
                                                <th width="80">項次</th>
                                                <th width="200" style="text-align:left">機關名稱</th>
                                                <th width="360" style="text-align:left">標案名稱</th>
                                                <th style="text-align:right">決標金額</th>
                                                <th>決標日</th>
                                                <th width="80">功能</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td colspan="6" class="text-center">查無資料</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </EmptyDataTemplate>
                            </asp:ListView>
                        </div>

                        <!-- 分頁 -->
                        <nav class="pagination d-flex gap-1 justify-content-center mt-3" aria-label="Pagination">
                            <asp:DataPager ID="dpPccAward" runat="server"
                                PagedControlID="lvPccAward"
                                PageSize="10"
                                OnPreRender="dpPccAward_PreRender">
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
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>

        </div>
    </div>

    <div class="tab-content">
        <!-- 國科會補助研究計畫 分頁 -->
        <div id="tabNSTCGrant" class="tab-pane fade">
            <!-- 搜尋 -->
            <div class="search">
                <h3>
                    <i class="fa-solid fa-magnifying-glass"></i>
                    查詢
                </h3>

                <div class="search-form">
                    <div class="row-line">
                        <!-- 年度 -->
                        <div class="search-item">
                            <div class="fs-16 text-gray mb-2">年度</div>
                            <asp:DropDownList ID="ddlNSTCYear" runat="server" CssClass="form-select" Style="width: 180px;">
                                <asp:ListItem Value="" Text="全部" />
                            </asp:DropDownList>
                        </div>

                        <!-- 總核定金額 -->
                        <div class="search-item">
                            <div class="fs-16 text-gray mb-2">總核定金額</div>
                            <div class="input-group">
                                <asp:TextBox ID="txtNSTCAmountFrom" runat="server" CssClass="form-control" TextMode="Number" placeholder="最小金額" />
                                <span class="input-group-text">至</span>
                                <asp:TextBox ID="txtNSTCAmountTo" runat="server" CssClass="form-control" TextMode="Number" placeholder="最大金額" />
                            </div>
                        </div>
                    </div>

                    <!-- 計畫名稱 -->
                    <div class="search-item">
                        <div class="fs-16 text-gray mb-2">計畫名稱</div>
                        <asp:TextBox ID="txtNSTCProjectName" runat="server" CssClass="form-control" placeholder="請輸入關鍵字" />
                    </div>

                    <asp:LinkButton ID="btnSearchNSTC" runat="server" CssClass="btn btn-blue d-table mx-auto"
                        OnClick="btnSearchNSTC_Click">
                        <i class="fa-solid fa-magnifying-glass"></i>
                        查詢
                    </asp:LinkButton>
                </div>
            </div>

            <!-- 列表內容 -->
            <asp:UpdatePanel ID="upNSTCList" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="block rounded-bottom-4">
                        <div class="title">
                            <h4>
                                <img src="<%= ResolveUrl("~/assets/img/title-icon02.svg") %>" alt="logo">
                                列表 (資料最後更新日期 :
                            <asp:Label ID="lblLatestNSTCDate" runat="server" Text="載入中..."></asp:Label>)
                            </h4>
                        </div>

                        <div class="table-responsive">
                            <asp:ListView ID="lvNSTCGrant" runat="server"
                                DataKeyNames="ID"
                                OnPagePropertiesChanging="lvNSTCGrant_PagePropertiesChanging">
                                <LayoutTemplate>
                                    <table class="table cyan-table">
                                        <thead>
                                            <tr>
                                                <th width="80">項次</th>
                                                <th width="80">年度</th>
                                                <th width="300" style="text-align:left">執行機關</th>
                                                <th style="text-align:left">計畫名稱</th>
                                                <th width="150" style="text-align:right">總核定金額</th>
                                                <th width="200">執行起迄日期</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr id="itemPlaceholder" runat="server"></tr>
                                        </tbody>
                                    </table>
                                </LayoutTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td data-th="項次:"><%# Container.DisplayIndex + 1 + (dpNSTCGrant.StartRowIndex) %></td>
                                        <td data-th="年度:"><%# Eval("Year") %></td>
                                        <td data-th="執行機關:" style="text-align:left"><%# Eval("Unit") %></td>
                                        <td data-th="計畫名稱:" style="text-align:left"><%# Eval("tName") %></td>
                                        <td data-th="總核定金額:" style="text-align:right"><%# FormatNSTCAmount(Eval("TotalApprovedAmount")) %></td>
                                        <td data-th="執行起迄日期:"><%# FormatExecutionDate(Eval("ExecutionStart")) %>~<%# FormatExecutionDate(Eval("ExecutionEnd")) %></td>
                                    </tr>
                                </ItemTemplate>
                                <EmptyDataTemplate>
                                    <table class="table cyan-table">
                                        <thead>
                                            <tr>
                                                <th width="80">項次</th>
                                                <th width="80">年度</th>
                                                <th width="300" style="text-align:left">執行機關</th>
                                                <th style="text-align:left">計畫名稱</th>
                                                <th width="150" style="text-align:right">總核定金額</th>
                                                <th width="200">執行起迄日期</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td colspan="6" class="text-center">查無資料</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </EmptyDataTemplate>
                            </asp:ListView>
                        </div>

                        <!-- 分頁 -->
                        <nav class="pagination d-flex gap-1 justify-content-center mt-3" aria-label="Pagination">
                            <asp:DataPager ID="dpNSTCGrant" runat="server"
                                PagedControlID="lvNSTCGrant"
                                PageSize="10"
                                OnPreRender="dpNSTCGrant_PreRender">
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
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnSearchNSTC" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>

    <script type="text/javascript">
        function setKeyword(keyword) {
            document.getElementById('<%= txtKeyword.ClientID %>').value = keyword;
        }
    </script>
</asp:Content>

