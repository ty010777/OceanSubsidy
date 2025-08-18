<%@ Page Language="C#" MasterPageFile="~/OSI/OSIMaster.master" AutoEventWireup="true" CodeFile="ActivityManage.aspx.cs" Inherits="OSI_ActivityManage" %>

<%@ MasterType VirtualPath="~/OSI/OSIMaster.master" %>

<%@ Register TagPrefix="uc1" TagName="ReportManage" Src="~/OSI/ReportManage.ascx" %>

<%@ Import Namespace="GS.App" %>

<asp:Content ID="cTitle" ContentPlaceHolderID="TitleContent" runat="server">
    活動資料管理  | 海洋科學調查活動填報系統
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .pagination > span {
            display: flex;
            gap: 4px;
        }
    </style>

    <script>
        // 存目前的tab
        document.addEventListener("DOMContentLoaded", function () {
            // 假設你的 tab 按鈕都有 data-bs-toggle="tab"
            var tabs = document.querySelectorAll('[data-bs-toggle="tab"]');
            tabs.forEach(function (tab) {
                tab.addEventListener('shown.bs.tab', function (e) {
                    // e.target 是剛被激活的那個 button/a
                    // 取出 data-bs-target="#adm" 或 href="#adm"
                    var target = e.target.getAttribute('data-bs-target')
                        || e.target.getAttribute('href');
                    // 存到 HiddenField（Server ID 會被改成 ClientID）
                    var hf = document.getElementById('<%= hfActiveTab.ClientID %>');
                    if (hf) hf.value = target;
                });
            });
        });
    </script>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="Bread" runat="server">
    <img src="<%= ResolveUrl("~/assets/img/title-icon05.svg") %>" alt="logo">
    <div>
        <span>目前位置</span>
        <h2>活動資料管理</h2>
    </div>
</asp:Content>

<asp:Content ID="cMain" ContentPlaceHolderID="MainContent" runat="server">

    <!-- TAB -->
    <nav>
        <div class="tab" role="tablist">
            <button class="tab-link active" data-bs-toggle="tab" data-bs-target="#setting" type="button">
                週期設定
            </button>
            <button class="tab-link" data-bs-toggle="tab" data-bs-target="#adm" type="button">
                填報管理
            </button>
        </div>
    </nav>

    <div class="tab-content">
        <%--週期設定--%>
        <div id="setting" class="tab-pane fade show active">


            <%--查詢--%>
            <asp:UpdatePanel ID="upSearch" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="search">
                        <h3><i class="fa-solid fa-magnifying-glass"></i>查詢</h3>
                        <div class="search-form">
                            <div class="column-2">
                                <div class="search-item">
                                    <div class="fs-16 text-gray mb-2">填報年度</div>
                                    <div class="d-flex flex-column flex-lg-row align-items-center gap-3">
                                        <div class="input-group">
                                            <asp:DropDownList ID="ddlYearFrom"
                                                runat="server"
                                                CssClass="form-control"
                                                AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlYearFrom_SelectedIndexChanged" />
                                            <span class="input-group-text">至</span>
                                            <asp:DropDownList ID="ddlYearTo"
                                                runat="server"
                                                CssClass="form-control" />
                                        </div>
                                        <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-blue d-table mx-auto" OnClick="btnSearch_Click">
                                            <i class="fa-solid fa-magnifying-glass"></i>
                                            查詢
                                        </asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>


            <asp:UpdatePanel ID="upSetting" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <!-- 隱藏 ID -->
                    <asp:HiddenField ID="hfYearFrom" runat="server" />
                    <asp:HiddenField ID="hfYearTo" runat="server" />

                    <%--列表--%>
                    <div class="block rounded-bottom-4">
                        <div class="title">
                            <h4>
                                <img src='<%= ResolveUrl("~/assets/img/title-icon02.svg") %>' alt="logo" />
                                列表
                            </h4>
                        </div>

                        <div class="table-responsive">
                            <asp:ListView ID="lvPeriods" runat="server"
                                DataKeyNames="PeriodID"
                                OnItemEditing="lvPeriods_ItemEditing"
                                OnItemCanceling="lvPeriods_ItemCanceling"
                                OnItemUpdating="lvPeriods_ItemUpdating"
                                OnItemCommand="lvPeriods_ItemCommand"
                                OnPagePropertiesChanging="dpPeriods_PagePropertiesChanging">
                                <LayoutTemplate>
                                    <table class="table cyan-table">
                                        <thead>
                                            <tr>
                                                <th width="80">排序</th>
                                                <th width="150">填報週期</th>
                                                <th width="360">日期區間</th>
                                                <th width="180">中央機關填報數<br />
                                                    (已填/總單位數)</th>
                                                <th width="180">縣市政府填報數<br />
                                                    (已填/總單位數)</th>
                                                <th width="80">案件數</th>
                                                <th>活動範圍標示繪製顏色</th>
                                                <th width="220">功能</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                                        </tbody>
                                    </table>
                                </LayoutTemplate>

                                <ItemTemplate>
                                    <tr>
                                        <td data-th="排序:"><%# Container.DisplayIndex + 1 %></td>
                                        <td data-th="填報週期:">
                                            <%# Eval("PeriodYear") %>年<%# Eval("PeriodQuarter") %>
                                        </td>
                                        <td data-th="日期區間:">
                                            <%# ((DateTime)Eval("StartDate")).ToMinguoDate() %> ～ 
                                            <%# ((DateTime)Eval("EndDate")).ToMinguoDate() %>
                                        </td>
                                        <td data-th="中央機關填報數(已填/總單位數):">
                                            <%# Eval("CentralFilledCount") %> / <%# Eval("CentralTotalUnit") %>
                                        </td>
                                        <td data-th="縣市政府填報數(已填/總單位數):">
                                            <%# Eval("LocalFilledCount") %> / <%# Eval("LocalTotalUnit") %>
                                        </td>
                                        <td data-th="案件數:">
                                            <%# Eval("ReportCount") %>
                                        </td>
                                        <td data-th="活動範圍標示繪製顏色:" class="p-0">
                                            <span class="color-tag d-inline-block"
                                                style="background-color: <%# Eval("Color") %>"></span>
                                        </td>
                                        <td data-th="功能:">
                                            <div class="d-flex gap-2">
                                                <asp:LinkButton ID="btnEdit" runat="server"
                                                    CssClass="btn btn-sm btn-outline-blue"
                                                    CommandName="Edit" CommandArgument='<%# Eval("PeriodID") %>'>
                                                    編輯
                                                </asp:LinkButton>
                                                <asp:LinkButton ID="btnReminder" runat="server"
                                                    CssClass="btn btn-sm btn-outline-blue"
                                                    CommandName="Reminder"
                                                    CommandArgument='<%# Eval("PeriodID") %>'
                                                    Visible='<%# (int)Eval("CentralFilledCount") < (int)Eval("CentralTotalUnit") || (int)Eval("LocalFilledCount") < (int)Eval("LocalTotalUnit") %>'>
                                                    發送未填報提醒信
                                                </asp:LinkButton>
                                            </div>
                                        </td>
                                    </tr>
                                </ItemTemplate>

                                <EditItemTemplate>
                                    <tr>
                                        <td data-th="排序:"><%# Container.DisplayIndex + 1 %></td>
                                        <td data-th="填報週期:">
                                            <%# Eval("PeriodYear") %>年<%# Eval("PeriodQuarter") %>
                                        </td>
                                        <td data-th="日期區間:">
                                            <div class="input-group">
                                                <asp:TextBox ID="txtEditStart" runat="server"
                                                    Text='<%# ((DateTime)Eval("StartDate")).ToMinguoDate() %>'
                                                    CssClass="form-control rocDate"
                                                    TextMode="SingleLine" />
                                                <span class="input-group-text">至</span>
                                                <asp:TextBox ID="txtEditEnd" runat="server"
                                                    Text='<%# ((DateTime)Eval("EndDate")).ToMinguoDate() %>'
                                                    CssClass="form-control rocDate"
                                                    TextMode="SingleLine" />
                                            </div>
                                        </td>
                                        <td data-th="中央機關填報數(已填/總單位數):">
                                            <%# Eval("CentralFilledCount") %> / <%# Eval("CentralTotalUnit") %>
                                        </td>
                                        <td data-th="縣市政府填報數(已填/總單位數):">
                                            <%# Eval("LocalFilledCount") %> / <%# Eval("LocalTotalUnit") %>
                                        </td>
                                        <td data-th="案件數:">
                                            <%# Eval("ReportCount") %>
                                        </td>
                                        <td data-th="活動範圍標示繪製顏色:" class="p-0">
                                            <asp:TextBox ID="txtEditColor" runat="server"
                                                Text='<%# Bind("Color") %>'
                                                CssClass="form-control form-control-color d-inline-block"
                                                TextMode="Color" />
                                        </td>
                                        <td data-th="功能:">
                                            <asp:LinkButton ID="lnkUpdate" runat="server"
                                                CssClass="btn btn-sm btn-outline-blue"
                                                CommandName="Update">
                                                儲存
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="lnkCancel" runat="server"
                                                CssClass="btn btn-sm btn-outline-blue ms-2"
                                                CommandName="Cancel">
                                                取消
                                            </asp:LinkButton>
                                        </td>
                                    </tr>
                                </EditItemTemplate>

                            </asp:ListView>
                        </div>

                        <%-- 分頁器 --%>
                        <nav class="pagination d-flex gap-1 justify-content-center mt-3" aria-label="Pagination">
                            <asp:DataPager
                                ID="dpPeriods" runat="server"
                                PagedControlID="lvPeriods"
                                PageSize="10"
                                OnPreRender="dpPeriods_PreRender">
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
                    <asp:AsyncPostBackTrigger ControlID="lvPeriods" EventName="ItemEditing" />
                    <asp:AsyncPostBackTrigger ControlID="lvPeriods" EventName="ItemCanceling" />
                    <asp:AsyncPostBackTrigger ControlID="lvPeriods" EventName="ItemUpdating" />
                    <asp:AsyncPostBackTrigger ControlID="lvPeriods" EventName="ItemCommand" />
                    <asp:AsyncPostBackTrigger ControlID="lvPeriods" EventName="PagePropertiesChanging" />
                </Triggers>

            </asp:UpdatePanel>
        </div>

        <asp:HiddenField ID="hfActiveTab" runat="server" />

        <%--填報管理--%>
        <div id="adm" class="tab-pane fade show">
            <uc1:ReportManage ID="ucReport" runat="server" />
        </div>
    </div>

</asp:Content>





