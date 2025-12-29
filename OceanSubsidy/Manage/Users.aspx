<%@ Page Language="C#" MasterPageFile="~/Manage/ManageMaster.master" AutoEventWireup="true" CodeFile="Users.aspx.cs" Inherits="Manage_Users" %>

<%@ Import Namespace="GS.App" %>

<asp:Content ID="cTitle" ContentPlaceHolderID="TitleContent" runat="server">
    帳號權限管理  | 使用者管理
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
    <div class="page-title d-flex align-items-center">
        <img src="<%= ResolveUrl("~/assets/img/title-icon07.svg") %>" alt="logo" />
        <div>
            <span>目前位置</span>
            <h2>使用者管理</h2>
        </div>
    </div>
</asp:Content>

<asp:Content ID="C2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hfActiveTab" runat="server" />
    <!-- TAB -->
    <nav>
        <div class="tab" role="tablist">
            <button class="tab-link active" data-bs-toggle="tab" data-bs-target="#tabMaintain" type="button">使用者維護</button>
            <button class="tab-link" data-bs-toggle="tab" data-bs-target="#tabPending" type="button">待審核帳號</button>
            <button class="tab-link" data-bs-toggle="tab" data-bs-target="#tabLoginHistory" type="button">帳號登入歷程</button>
        </div>
    </nav>

    <div class="tab-content">
        <!-- 使用者維護 分頁 -->
        <div id="tabMaintain" class="tab-pane fade show active">
            <div class="search">
                <h3><i class="fa-solid fa-magnifying-glass"></i>查詢</h3>
                <div class="search-form">
                    <div class="row-line">
                        <div class="search-item">
                            <div class="fs-16 text-gray mb-2">系統名稱</div>
                            <asp:DropDownList ID="ddlSearchApp" runat="server" CssClass="form-select" Style="width: 260px;" />
                        </div>
                        <div class="search-item">
                            <div class="fs-16 text-gray mb-2">單位類別</div>
                            <asp:DropDownList ID="ddlSearchUnitType" runat="server" CssClass="form-select" Style="width: 180px;" />
                        </div>
                        <div class="search-item">
                            <div class="fs-16 text-gray mb-2">單位名稱</div>
                            <asp:DropDownList ID="ddlSearchUnit" runat="server" CssClass="form-select" Style="width: 200px;" />
                        </div>
                        <div class="search-item">
                            <div class="fs-16 text-gray mb-2">關鍵字</div>
                            <asp:TextBox ID="txtSearchKeyword" runat="server"
                                CssClass="form-control"
                                Style="width: 200px;"
                                placeholder="單位名稱/帳號/姓名/電話" />
                        </div>
                        <div class="search-item">
                            <div class="fs-16 text-gray mb-2">狀態</div>
                            <asp:DropDownList ID="ddlSearchStatus" runat="server" CssClass="form-select" Style="width: 120px;">
                                <asp:ListItem Text="全部" Value="-1" />
                                <asp:ListItem Text="啟用" Value="1" Selected="True" />
                                <asp:ListItem Text="停用" Value="0" />
                            </asp:DropDownList>
                        </div>
                        <asp:LinkButton ID="btnSearchUser" runat="server" CssClass="btn btn-blue" OnClick="btnSearchUser_Click">
                                <i class="fa-solid fa-magnifying-glass"></i>
                                查詢
                        </asp:LinkButton>
                    </div>
                </div>
            </div>

            <div class="block rounded-bottom-4">
                <div class="title">
                    <h4>
                        <img src="<%= ResolveUrl("~/assets/img/title-icon02.svg") %>" alt="logo" />
                        列表
                    </h4>
                    <asp:LinkButton ID="btnAddUser" runat="server" CssClass="btn btn-cyan" OnClick="btnAddUser_Click" CausesValidation="false">
                            <i class="fa-solid fa-plus"></i>新增
                    </asp:LinkButton>
                </div>
                <!-- 使用者列表 -->
                <asp:UpdatePanel ID="upListUsers" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="table-responsive">
                            <asp:ListView
                                ID="lvUsers"
                                runat="server"
                                DataKeyNames="UserID"
                                OnItemCommand="lvUsers_ItemCommand"
                                OnItemDataBound="lvUsers_ItemDataBound"
                                OnPagePropertiesChanging="dpUsers_PagePropertiesChanging">

                                <LayoutTemplate>
                                    <table class="table cyan-table">
                                        <thead>
                                            <tr>
                                                <th width="50">排序</th>
                                                <th class="text-start">海洋科學調查活動</th>
                                                <th class="text-start">海洋領域補助計畫管理資訊系統</th>
                                                <th width="180" class="text-start">單位名稱</th>
                                                <th class="text-start">帳號</th>
                                                <th class="text-start">姓名</th>
                                                <th class="text-start">電話</th>
                                                <th class="text-start">最後登入</th>
                                                <th>狀態</th>
                                                <th>更改狀態</th>
                                                <th width="150">功能</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                                        </tbody>
                                    </table>
                                </LayoutTemplate>

                                <ItemTemplate>
                                    <tr id="itemRow" runat="server">
                                        <td data-th="排序:"><%# Container.DisplayIndex + 1 %></td>
                                        <td data-th="海洋科學調查活動:" class="text-start"><%# Eval("OSIRoleName") %></td>
                                        <td data-th="海洋領域補助計畫管理資訊系統:" class="text-start"><%# Eval("OFSRoleName") %></td>
                                        <td data-th="單位名稱:" class="text-start"><%# Eval("UnitName") %></td>
                                        <td data-th="帳號:" class="text-start"><%# Eval("Account") %></td>
                                        <td data-th="姓名:" class="text-start"><%# Eval("Name") %></td>
                                        <td data-th="電話:" class="text-start"><%# Eval("Tel") %></td>
                                        <td data-th="最後登入:" class="text-start">
                                            <%# DateTimeHelper.ToMinguoDateTime(Eval("LastLoginTime") as DateTime?) %>
                                        </td>
                                        <td data-th="狀態:">
                                            <asp:Label ID="lblStatus" runat="server"
                                                Text='<%# (bool)Eval("IsActive") ? "啟用" : "停用" %>'
                                                CssClass='<%# (bool)Eval("IsActive") ? "" : "text-pink" %>' />
                                        </td>
                                        <td data-th="更改狀態:">
                                            <asp:LinkButton
                                                runat="server"
                                                CssClass="btn btn-sm btn-outline-blue"
                                                CommandName="ToggleActive"
                                                CommandArgument='<%# Eval("UserID") %>'>
                                                <%# (bool)Eval("IsActive") ? "停用" : "啟用" %>
                                            </asp:LinkButton>
                                        </td>
                                        <td data-th="功能:" width="150">
                                            <div class="d-flex flex-row flex-md-column flex-wrap gap-2 justify-content-center align-items-center">
                                                <asp:LinkButton
                                                    runat="server"
                                                    CssClass="btn btn-sm btn-outline-blue"
                                                    CommandName="EditUser"
                                                    CommandArgument='<%# Eval("UserID") %>'>編輯
                                                </asp:LinkButton>
                                                <asp:LinkButton
                                                    ID="lnkDelUser"
                                                    runat="server"
                                                    CssClass="btn btn-sm btn-outline-pink"
                                                    CommandName="DeleteUser"
                                                    CommandArgument='<%# Eval("UserID") %>'>刪除
                                                </asp:LinkButton>
                                            </div>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:ListView>
                        </div>

                        <!-- 分頁器 -->
                        <nav class="pagination d-flex gap-1 justify-content-center mt-3" aria-label="Pagination">
                            <asp:DataPager
                                ID="dpUsers"
                                runat="server"
                                PagedControlID="lvUsers"
                                PageSize="30"
                                OnPreRender="dpUsers_PreRender">
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

                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnSearchUser" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnAddUser" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveUser" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnConfirmDeleteUser" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnConfirmToggleActive" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="lvUsers" EventName="PagePropertiesChanging" />
                        <asp:AsyncPostBackTrigger ControlID="dpUsers" EventName="PreRender" />
                    </Triggers>
                </asp:UpdatePanel>


            </div>

        </div>

        <!-- 待審核帳號 分頁 -->
        <div id="tabPending" class="tab-pane fade">
            <div class="block rounded-bottom-4">
                <div class="title">
                    <h4>
                        <img src="<%= ResolveUrl("~/assets/img/title-icon02.svg") %>" alt="logo" />
                        列表
                    </h4>
                </div>

                <asp:UpdatePanel ID="upListPending" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <div class="table-responsive">
                            <asp:ListView
                                ID="lvPendingUsers"
                                runat="server"
                                DataKeyNames="UserID"
                                OnItemCommand="lvPendingUsers_ItemCommand"
                                OnPagePropertiesChanging="lvPendingUsers_PagePropertiesChanging">

                                <LayoutTemplate>
                                    <table class="table cyan-table">
                                        <thead>
                                            <tr>
                                                <th width="50">排序</th>
                                                <th class="text-start">申請來源</th>
                                                <th class="text-start">欲申請之系統平台</th>
                                                <th class="text-start">申請時間</th>
                                                <th class="text-start">單位名稱</th>
                                                <th class="text-start">帳號</th>
                                                <th class="text-start">姓名</th>
                                                <th width="150">功能</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                                        </tbody>
                                    </table>
                                </LayoutTemplate>

                                <ItemTemplate>
                                    <tr id="itemRow" runat="server">
                                        <td data-th="排序:"><%# Container.DisplayIndex + 1 %></td>
                                        <td data-th="申請來源:" class="text-start"><%# Eval("ApprovedSource") %></td>
                                        <td data-th="系統平台:" class="text-start"><%# Eval("SystemName") %></td>
                                        <td data-th="申請時間:" class="text-start">
                                            <%# ((DateTime)Eval("CreateTime")).ToString("yyyy/MM/dd HH:mm") %>
                                        </td>
                                        <td data-th="單位名稱:" class="text-start"><%# Eval("UnitName") %></td>
                                        <td data-th="帳號:" class="text-start"><%# Eval("Account") %></td>
                                        <td data-th="姓名:" class="text-start"><%# Eval("Name") %></td>
                                        <td data-th="功能:" width="150">
                                            <div class="d-flex flex-wrap gap-2 justify-content-center">
                                                <asp:LinkButton
                                                    runat="server"
                                                    CssClass="btn btn-sm btn-outline-blue"
                                                    CommandName="ApproveUser"
                                                    CommandArgument='<%# Eval("UserID") %>'>
                                                    審核
                                                </asp:LinkButton>
                                            </div>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:ListView>
                        </div>

                        <nav class="pagination d-flex gap-1 justify-content-center mt-3" aria-label="Pagination">
                            <asp:DataPager
                                ID="dpPendingUsers"
                                runat="server"
                                PagedControlID="lvPendingUsers"
                                PageSize="30"
                                OnPreRender="dpPendingUsers_PreRender">
                                <Fields>
                                    <asp:NextPreviousPagerField
                                        ButtonType="Button"
                                        ButtonCssClass="nav-button"
                                        ShowPreviousPageButton="True"
                                        PreviousPageText="‹"
                                        ShowNextPageButton="False" />
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
                                        NextPageText="›" />
                                </Fields>
                            </asp:DataPager>
                        </nav>
                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>

        <!-- 帳號登入歷程 分頁 -->
        <div id="tabLoginHistory" class="tab-pane fade">
            <div class="block rounded-bottom-4">
                <div class="title">
                    <h4>
                        <img src="<%= ResolveUrl("~/assets/img/title-icon02.svg") %>" alt="logo" />
                        列表
                    </h4>
                </div>

                <asp:UpdatePanel ID="upListLoginHistory" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <div class="table-responsive">
                            <asp:ListView
                                ID="lvLoginHistory"
                                runat="server"
                                DataKeyNames="LoginID"
                                OnPagePropertiesChanging="lvLoginHistory_PagePropertiesChanging">

                                <LayoutTemplate>
                                    <table class="table cyan-table">
                                        <thead>
                                            <tr>
                                                <th width="50">排序</th>
                                                <th class="text-start">單位名稱</th>
                                                <th class="text-start">帳號</th>
                                                <th class="text-start">姓名</th>
                                                <th class="text-start">登入時間</th>
                                                <th class="text-start">來源IP</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                                        </tbody>
                                    </table>
                                </LayoutTemplate>

                                <ItemTemplate>
                                    <tr id="itemRow" runat="server">
                                        <td data-th="排序:"><%# Container.DisplayIndex + 1 %></td>
                                        <td data-th="單位名稱:" class="text-start"><%# Eval("UnitName") %></td>
                                        <td data-th="帳號:" class="text-start"><%# Eval("Account") %></td>
                                        <td data-th="姓名:" class="text-start"><%# Eval("Name") %></td>
                                        <td data-th="登入時間:" class="text-start">
                                            <%# DateTimeHelper.ToMinguoDateTime(Eval("LoginTime") as DateTime?) %>
                                        </td>
                                        <td data-th="來源IP:" class="text-start"><%# Eval("LoginIP") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:ListView>
                        </div>

                        <nav class="pagination d-flex gap-1 justify-content-center mt-3" aria-label="Pagination">
                            <asp:DataPager
                                ID="dpLoginHistory"
                                runat="server"
                                PagedControlID="lvLoginHistory"
                                PageSize="30"
                                OnPreRender="dpLoginHistory_PreRender">
                                <Fields>
                                    <asp:NextPreviousPagerField
                                        ButtonType="Button"
                                        ButtonCssClass="nav-button"
                                        ShowPreviousPageButton="True"
                                        PreviousPageText="‹"
                                        ShowNextPageButton="False" />
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
                                        NextPageText="›" />
                                </Fields>
                            </asp:DataPager>
                        </nav>
                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>
    </div>

    <!-- 新增/編輯 使用者 Modal -->
    <div class="modal fade" id="userModal" tabindex="-1" data-bs-backdrop="static" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content">
                <asp:UpdatePanel ID="upUserModal" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <asp:HiddenField ID="hfUserID" runat="server" />

                        <div class="modal-header">
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                                <i class="fa-solid fa-circle-xmark"></i>
                            </button>
                        </div>
                        <div class="modal-body">
                            <div class="table-responsive">
                                <table class="table align-middle gray-table side-table">
                                    <tbody>
                                        <tr>
                                            <th><span class="text-pink">*</span>單位類別</th>
                                            <td>
                                                <asp:RadioButtonList ID="rblUserUnitType" runat="server"
                                                    RepeatDirection="Horizontal"
                                                    RepeatLayout="Flow"
                                                    AutoPostBack="true"
                                                    CssClass="form-check-input-group"
                                                    OnSelectedIndexChanged="rblUserUnitType_SelectedIndexChanged" />
                                            </td>
                                        </tr>
                                        <tr id="divGovUnit" runat="server" visible="true">
                                            <th><span class="text-pink">*</span>單位名稱</th>
                                            <td class="d-flex gap-2">
                                                <asp:DropDownList
                                                    ID="ddlUserUnit" runat="server"
                                                    CssClass="form-select"
                                                    AutoPostBack="true"
                                                    OnSelectedIndexChanged="ddlUserUnit_SelectedIndexChanged" />
                                                <asp:TextBox ID="txtOtherGovUnit" runat="server"
                                                    CssClass="form-control"
                                                    Placeholder="請輸入其他單位名稱"
                                                    Visible="false" />
                                                <asp:RequiredFieldValidator ID="rfvOtherGovUnit" runat="server"
                                                    ControlToValidate="txtOtherGovUnit"
                                                    ErrorMessage="請輸入其他單位名稱"
                                                    ValidationGroup="UserModal"
                                                    CssClass="invalid"
                                                    Display="Dynamic"
                                                    Enabled="false" />
                                            </td>
                                        </tr>
                                        <tr id="divOtherUnit" runat="server" visible="false">
                                            <th><span class="text-pink">*</span>單位名稱</th>
                                            <td>
                                                <asp:TextBox ID="txtOtherUnit" runat="server"
                                                    CssClass="form-control"
                                                    Placeholder="請輸入單位名稱" />
                                                <asp:RequiredFieldValidator ID="rfvOtherUnit" runat="server"
                                                    ControlToValidate="txtOtherUnit"
                                                    ErrorMessage="請輸入單位名稱"
                                                    ValidationGroup="UserModal"
                                                    CssClass="invalid"
                                                    Display="Dynamic"
                                                    Enabled="false" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <th><span class="text-pink">*</span>帳號(電子郵件)</th>
                                            <td>
                                                <asp:TextBox ID="txtUserAccount" runat="server"
                                                    CssClass="form-control"
                                                    TextMode="Email"
                                                    Placeholder="帳號(電子郵件)" />
                                                <asp:RequiredFieldValidator ID="rfvUserAccount" runat="server"
                                                    ControlToValidate="txtUserAccount"
                                                    ErrorMessage="請輸入帳號"
                                                    ValidationGroup="UserModal"
                                                    CssClass="invalid"
                                                    Display="Dynamic" />
                                                <asp:RegularExpressionValidator ID="revUserAccount" runat="server"
                                                    ControlToValidate="txtUserAccount"
                                                    ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                                                    ErrorMessage="格式不正確"
                                                    ValidationGroup="UserModal"
                                                    CssClass="invalid"
                                                    Display="Dynamic" />
                                                <asp:CustomValidator ID="cvUserAccount" runat="server"
                                                    ControlToValidate="txtUserAccount"
                                                    OnServerValidate="cvUserAccount_ServerValidate"
                                                    ErrorMessage="帳號已存在"
                                                    ValidationGroup="UserModal"
                                                    CssClass="invalid"
                                                    Display="Dynamic" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <th><span class="text-pink">*</span>姓名</th>
                                            <td>
                                                <asp:TextBox ID="txtUserName" runat="server"
                                                    CssClass="form-control"
                                                    Placeholder="請輸入中文姓名" />
                                                <asp:RequiredFieldValidator ID="rfvUserName" runat="server"
                                                    ControlToValidate="txtUserName"
                                                    ErrorMessage="請輸入姓名"
                                                    ValidationGroup="UserModal"
                                                    CssClass="invalid"
                                                    Display="Dynamic" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <th><span class="text-pink">*</span>電話</th>
                                            <td>
                                                <asp:TextBox ID="txtUserTel" runat="server"
                                                    CssClass="form-control"
                                                    Placeholder="請輸入電話號碼或手機" />
                                                <asp:RequiredFieldValidator ID="rfvUserTel" runat="server"
                                                    ControlToValidate="txtUserTel"
                                                    ErrorMessage="請輸入電話"
                                                    ValidationGroup="UserModal"
                                                    CssClass="invalid"
                                                    Display="Dynamic" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <th><span class="text-pink">*</span> 活動填報角色</th>
                                            <td>
                                                <div class="form-check-input-group">
                                                    <asp:RadioButtonList
                                                        ID="rblUserOSIRole" runat="server"
                                                        CssClass="form-check-input-group"
                                                        RepeatDirection="Horizontal" RepeatLayout="Flow" />
                                                    <asp:RequiredFieldValidator ID="rfvUserOSIRole" runat="server"
                                                        ControlToValidate="rblUserOSIRole"
                                                        InitialValue=""
                                                        ErrorMessage="請選擇一個角色"
                                                        ValidationGroup="UserModal"
                                                        CssClass="invalid"
                                                        Display="Dynamic" />
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th><span class="text-pink">*</span> 計畫管理角色</th>
                                            <td>
                                                <div class="form-check-input-group">
                                                    <asp:RadioButtonList ID="rblUserNeedOFSRoles" runat="server"
                                                        RepeatDirection="Horizontal"
                                                        RepeatLayout="Flow"
                                                        CssClass="form-check-input-group"
                                                        AutoPostBack="true"
                                                        OnSelectedIndexChanged="rblUserNeedOFSRoles_SelectedIndexChanged">
                                                        <asp:ListItem Text="無需使用" Value="0" />
                                                        <asp:ListItem Text="需使用" Value="1" />
                                                    </asp:RadioButtonList>
                                                </div>
                                                <asp:Panel ID="pnlUserOFSRoleCheckboxes" runat="server"
                                                    Visible="false">
                                                    <asp:CheckBoxList ID="cblUserOFSRoles" runat="server"
                                                        RepeatDirection="Horizontal" RepeatLayout="Flow"
                                                        CssClass="form-check-input-group mt-4"
                                                        Style="--grid-columns: 3;" />
                                                    <asp:CustomValidator ID="cvUserOFSRoles" runat="server"
                                                        ErrorMessage="請選擇計畫管理角色"
                                                        ValidationGroup="UserModal"
                                                        OnServerValidate="cvUserOFSRoles_ServerValidate"
                                                        EnableClientScript="false"
                                                        CssClass="invalid"
                                                        Display="Dynamic" />
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <asp:Button ID="btnSaveUser" runat="server"
                                CssClass="btn btn-cyan d-table mx-auto mt-3"
                                Text="儲存" ValidationGroup="UserModal"
                                OnClick="btnSaveUser_Click" />

                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <!-- 刪除確認 Modal -->
    <div class="modal fade" id="deleteUserModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="deleteUserModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-md">
            <div class="modal-content">
                <asp:UpdatePanel ID="upDelUserModal" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                                <i class="fa-solid fa-circle-xmark"></i>
                            </button>
                        </div>
                        <div class="modal-body">
                            <div class="d-flex flex-column align-items-center mb-5 gap-3">
                                <asp:HiddenField ID="hfDelUserID" runat="server" />
                                <h4 class="text-blue-green fw-bold">確定要刪除此使用者?</h4>
                            </div>

                            <div class="d-flex gap-4 flex-wrap justify-content-center">
                                <button type="button" class="btn btn-gray" data-bs-dismiss="modal">
                                    取消</button>
                                <asp:Button ID="btnConfirmDeleteUser" runat="server"
                                    CssClass="btn btn-cyan"
                                    Text="確定" OnClick="btnConfirmDeleteUser_Click" />
                            </div>

                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <!-- 審核 Modal -->
    <div class="modal fade" id="approveModal" tabindex="-1" data-bs-backdrop="static" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content">
                <asp:UpdatePanel ID="upApproveModal" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <asp:HiddenField ID="hfApproveUserID" runat="server" />

                        <div class="modal-header">
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                                <i class="fa-solid fa-circle-xmark"></i>
                            </button>
                        </div>

                        <div class="modal-body">

                            <div class="table-responsive">
                                <table class="table align-middle gray-table side-table">
                                    <tbody>
                                        <tr>
                                            <th>申請來源</th>
                                            <td>
                                                <asp:Label ID="lblApproveSource" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>欲申請之系統平台</th>
                                            <td>
                                                <asp:Label ID="lblApproveSystems" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>單位類別</th>
                                            <td>
                                                <div class="form-check-input-group">
                                                    <asp:RadioButtonList ID="rblApproveUnitType" runat="server"
                                                        RepeatDirection="Horizontal"
                                                        AutoPostBack="true"
                                                        RepeatLayout="Flow"
                                                        CssClass="form-check-input-group"
                                                        OnSelectedIndexChanged="rblApproveUnitType_SelectedIndexChanged" />
                                                    <asp:RequiredFieldValidator ID="rfvApproveUnitType" runat="server"
                                                        ControlToValidate="rblApproveUnitType"
                                                        InitialValue="" ErrorMessage="請選擇單位類別"
                                                        ValidationGroup="ApproveGroup"
                                                        CssClass="invalid" Display="Dynamic" />
                                                </div>
                                            </td>
                                        </tr>
                                        <tr id="divApproveGovUnit" runat="server">
                                            <th>單位名稱</th>
                                            <td class="d-flex gap-2">
                                                <asp:DropDownList ID="ddlApproveUnit" runat="server"
                                                    CssClass="form-select" AutoPostBack="true"
                                                    OnSelectedIndexChanged="ddlApproveUnit_SelectedIndexChanged" />
                                                <asp:TextBox ID="txtApproveOtherGovUnit" runat="server"
                                                    CssClass="form-control" Placeholder="請輸入其他單位名稱"
                                                    Visible="false" />
                                                <asp:RequiredFieldValidator ID="rfvApproveOtherGovUnit" runat="server"
                                                    ControlToValidate="txtApproveOtherGovUnit"
                                                    ErrorMessage="請輸入其他單位名稱"
                                                    ValidationGroup="ApproveGroup"
                                                    CssClass="invalid" Display="Dynamic" Enabled="false" />
                                            </td>
                                        </tr>
                                        <tr id="divApproveOtherUnit" runat="server" visible="false">
                                            <th>單位名稱</th>
                                            <td>
                                                <asp:TextBox ID="txtApproveOtherUnit" runat="server"
                                                    CssClass="form-control" Placeholder="請輸入單位名稱" />
                                                <asp:RequiredFieldValidator ID="rfvApproveOtherUnit" runat="server"
                                                    ControlToValidate="txtApproveOtherUnit"
                                                    ErrorMessage="請輸入單位名稱"
                                                    ValidationGroup="ApproveGroup"
                                                    CssClass="invalid" Display="Dynamic" Enabled="false" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <th>帳號(電子郵件)</th>
                                            <td>
                                                <asp:TextBox ID="txtApproveAccount" runat="server"
                                                    CssClass="form-control" TextMode="Email" />
                                                <asp:RequiredFieldValidator ID="rfvApproveAccount" runat="server"
                                                    ControlToValidate="txtApproveAccount"
                                                    ErrorMessage="請輸入帳號"
                                                    ValidationGroup="ApproveGroup"
                                                    CssClass="invalid" Display="Dynamic" />
                                                <asp:RegularExpressionValidator ID="revApproveAccount" runat="server"
                                                    ControlToValidate="txtApproveAccount"
                                                    ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                                                    ErrorMessage="格式不正確"
                                                    ValidationGroup="ApproveGroup"
                                                    CssClass="invalid" Display="Dynamic" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>姓名</th>
                                            <td>
                                                <asp:TextBox ID="txtApproveName" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvApproveName" runat="server"
                                                    ControlToValidate="txtApproveName"
                                                    ErrorMessage="請輸入姓名"
                                                    ValidationGroup="ApproveGroup"
                                                    CssClass="invalid" Display="Dynamic" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>電話</th>
                                            <td>
                                                <asp:TextBox ID="txtApproveTel" runat="server"
                                                    CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvApproveTel" runat="server"
                                                    ControlToValidate="txtApproveTel"
                                                    ErrorMessage="請輸入電話"
                                                    ValidationGroup="ApproveGroup"
                                                    CssClass="invalid" Display="Dynamic" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <th><span class="text-pink">*</span> 活動填報角色</th>
                                            <td>
                                                <div class="form-check-input-group">
                                                    <asp:RadioButtonList ID="rblApproveOSIRole" runat="server"
                                                        RepeatDirection="Horizontal"
                                                        RepeatLayout="Flow"
                                                        CssClass="form-check-input-group"
                                                        ValidationGroup="ApproveGroup" />
                                                    <asp:RequiredFieldValidator ID="rfvApproveOSIRole" runat="server"
                                                        ControlToValidate="rblApproveOSIRole"
                                                        InitialValue="" ErrorMessage="請選擇"
                                                        ValidationGroup="ApproveGroup"
                                                        CssClass="invalid" Display="Dynamic" />
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th><span class="text-pink">*</span> 計畫管理角色</th>
                                            <td>
                                                <div class="form-check-input-group">
                                                    <asp:RadioButtonList ID="rblNeedOFSRoles" runat="server"
                                                        RepeatDirection="Horizontal"
                                                        AutoPostBack="true"
                                                        RepeatLayout="Flow"
                                                        CssClass="form-check-input-group"
                                                        OnSelectedIndexChanged="rblNeedOFSRoles_SelectedIndexChanged">
                                                        <asp:ListItem Text="無需使用" Value="0" />
                                                        <asp:ListItem Text="需使用" Value="1" />
                                                    </asp:RadioButtonList>
                                                </div>

                                                <asp:Panel ID="pnlOFSRoleCheckboxes" runat="server">
                                                    <asp:CheckBoxList ID="cblApproveOFSRoles" runat="server"
                                                        RepeatDirection="Horizontal" RepeatLayout="Flow"
                                                        CssClass="form-check-input-group mt-4"
                                                        Style="--grid-columns: 3;" />
                                                    <asp:CustomValidator
                                                        ID="cvApproveOFSRoles"
                                                        runat="server"
                                                        ErrorMessage="請選擇計畫管理角色"
                                                        ValidationGroup="ApproveGroup"
                                                        Display="Dynamic"
                                                        EnableClientScript="false"
                                                        OnServerValidate="cvApproveOFSRoles_ServerValidate"
                                                        CssClass="invalid" />
                                                </asp:Panel>
                                            </td>
                                        </tr>

                                    </tbody>
                                </table>
                            </div>
                        <div class="d-flex gap-4 flex-wrap justify-content-center">
                            <asp:LinkButton
                                ID="btnReject"
                                runat="server"
                                CssClass="btn btn-outline-pink"
                                OnClick="btnReject_Click">
                                    <i class="fa-solid fa-xmark"></i>審核退回
                            </asp:LinkButton>
                            <asp:LinkButton
                                ID="btnApprove"
                                runat="server"
                                CssClass="btn btn-cyan"
                                ValidationGroup="ApproveGroup"
                                OnClick="btnApprove_Click">
                                    <i class="fa-solid fa-check"></i>審核通過
                            </asp:LinkButton>
                        </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <!-- 狀態更改確認 Modal -->
    <div class="modal fade" id="toggleActiveModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="accountCheckModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-md">
            <div class="modal-content">
                <asp:UpdatePanel ID="upToggleActiveModal" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                                <i class="fa-solid fa-circle-xmark"></i>
                            </button>
                        </div>
                        <div class="modal-body">
                            <asp:HiddenField ID="hfToggleUserID" runat="server" />
                            <asp:HiddenField ID="hfToggleUserAccount" runat="server" />
                            <asp:HiddenField ID="hfToggleNewStatus" runat="server" />

                            <div class="d-flex flex-column align-items-center mb-5 gap-3">
                                <asp:Literal ID="ltlToggleConfirmMessage" runat="server" Mode="PassThrough" />
                            </div>

                            <div class="d-flex gap-4 flex-wrap justify-content-center">
                                <button type="button" class="btn btn-gray" data-bs-dismiss="modal">
                                    取消</button>
                                <asp:Button ID="btnConfirmToggleActive" runat="server"
                                    CssClass="btn btn-cyan"
                                    Text="確認" OnClick="btnConfirmToggleActive_Click" />
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>


</asp:Content>

<asp:Content ID="Scripts" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script type="text/javascript">
        // 樣式調整
        $(function () {
            function applyClasses() {
                $('#<%= rblUserUnitType.ClientID %> input[type=radio]')
                    .addClass('form-check-input');
                $('#<%= rblUserOSIRole.ClientID %> input[type=radio]')
                    .addClass('form-check-input');
                $('#<%= rblUserNeedOFSRoles.ClientID %> input[type=radio]')
                    .addClass('form-check-input');
                $('#<%= rblApproveUnitType.ClientID %> input[type=radio]')
                    .addClass('form-check-input');
                $('#<%= rblApproveOSIRole.ClientID %> input[type=radio]')
                    .addClass('form-check-input');
                $('#<%= rblNeedOFSRoles.ClientID %> input[type=radio]')
                    .addClass('form-check-input');
                $('#<%= cblUserOFSRoles.ClientID %> input[type=checkbox]')
                    .addClass('form-check-input');
                $('#<%= cblApproveOFSRoles.ClientID %> input[type=checkbox]')
                    .addClass('form-check-input');

            }

            $('#userModal').on('shown.bs.modal', applyClasses);
            $('#deleteUserModal').on('shown.bs.modal', applyClasses);
            $('#approveModal').on('shown.bs.modal', applyClasses);

            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(applyClasses);
        });
    </script>
</asp:Content>
