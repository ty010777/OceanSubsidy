<%@ Page Language="C#" MasterPageFile="~/Manage/ManageMaster.master" AutoEventWireup="true" CodeFile="Units.aspx.cs" Inherits="Manage_Units" %>

<asp:Content ID="cTitle" ContentPlaceHolderID="TitleContent" runat="server">
    帳號權限管理  | 活動調查填報單位管理
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .pagination > span {
            display: flex;
            gap: 4px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Bread" runat="server">
    <img src="<%= ResolveUrl("~/assets/img/title-icon08.svg") %>" alt="logo">
    <div>
        <span>目前位置</span>
        <h2>活動調查填報單位管理</h2>
    </div>
</asp:Content>

<asp:Content ID="C2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="block rounded-4">
        <div class="title">
            <h4>
                <img src="<%= ResolveUrl("~/assets/img/title-icon02.svg") %>" alt="logo" />
                列表
            </h4>
            <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-cyan" OnClick="btnAdd_Click" CausesValidation="false">
                <i class="fa-solid fa-plus"></i>
                新增
            </asp:LinkButton>
        </div>

        <asp:UpdatePanel ID="upList" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="table-responsive">
                    <asp:ListView
                        ID="lvUnits" runat="server"
                        DataKeyNames="UnitID"
                        OnItemCommand="lvUnits_ItemCommand"
                        OnItemDataBound="lvUnits_ItemDataBound"
                        OnPagePropertiesChanging="lvUnits_PagePropertiesChanging">

                        <LayoutTemplate>
                            <table class="table cyan-table">
                                <thead>
                                    <tr>
                                        <th width="120">政府機關類別</th>
                                        <th width="50">排序</th>
                                        <th class="text-start">單位名稱</th>
                                        <th width="120">功能</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                                </tbody>
                            </table>
                        </LayoutTemplate>

                        <ItemTemplate>
                            <tr id="itemRow" runat="server">
                                <td data-th="政府機關類別:"><%# Eval("GovUnitTypeName") %></td>
                                <td data-th="排序:"><%# GetDisplayNumber(Eval("UnitID")) %></td>
                                <td data-th="單位名稱:" class="text-start"><%# Eval("UnitName") %></td>
                                <td data-th="功能:" width="120">
                                    <div class="d-flex gap-2">
                                        <asp:LinkButton
                                            ID="lkEditUnit"
                                            runat="server"
                                            CssClass="btn btn-sm btn-outline-blue me-1"
                                            CommandName="EditUnit"
                                            CommandArgument='<%# Eval("UnitID") %>'>
                                            編輯
                                        </asp:LinkButton>
                                        <asp:LinkButton
                                            ID="lkDeleteUnit"
                                            runat="server"
                                            CssClass="btn btn-sm btn-outline-pink"
                                            CommandName="DeleteUnit"
                                            CommandArgument='<%# Eval("UnitID") %>'>
                                            刪除
                                        </asp:LinkButton>
                                    </div>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:ListView>
                </div>

                <nav class="pagination d-flex gap-1 justify-content-center mt-3" aria-label="Pagination">
                    <asp:DataPager
                        ID="dpUnits"
                        runat="server"
                        PagedControlID="lvUnits"
                        PageSize="30"
                        OnPreRender="dpUnits_PreRender">
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
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnSave" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnConfirmDelete" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="lvUnits" EventName="PagePropertiesChanging" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
    <!-- 新增／編輯 Modal -->
    <div class="modal fade" id="unitModal" tabindex="-1" data-bs-backdrop="static" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content">
                <asp:UpdatePanel ID="upUnitModal" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <asp:Panel runat="server" ID="pnlUnit">
                            <div class="modal-header">
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                                    <i class="fa-solid fa-circle-xmark"></i>
                                </button>
                            </div>
                            <div class="modal-body">

                                <asp:HiddenField ID="hfUnitID" runat="server" />

                                <div class="table-responsive">
                                    <table class="table align-middle gray-table side-table">
                                        <tbody>
                                            <tr>
                                                <th>政府機關類別</th>
                                                <td>
                                                    <asp:RadioButtonList 
                                                        ID="rblGovUnitType" runat="server"
                                                        RepeatDirection="Horizontal"
                                                        RepeatLayout="Flow"
                                                        AutoPostBack="true"
                                                        CssClass="form-check-input-group"
                                                        OnSelectedIndexChanged="rblGovUnitType_SelectedIndexChanged" />
                                                </td>
                                            </tr>

                                            <tr>
                                                <th>上層單位</th>
                                                <td>
                                                    <asp:DropDownList
                                                        ID="ddlModalParent"
                                                        runat="server"
                                                        CssClass="form-select">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>

                                            <tr>
                                                <th><span class="text-pink">*</span>單位名稱</th>
                                                <td>
                                                    <asp:TextBox
                                                        ID="txtModalName"
                                                        runat="server"
                                                        CssClass="form-control"
                                                        MaxLength="100" />
                                                    <asp:RequiredFieldValidator runat="server"
                                                        ControlToValidate="txtModalName"
                                                        ErrorMessage="請輸入單位名稱"
                                                        ValidationGroup="UnitModal"
                                                        CssClass="invalid"
                                                        Display="Dynamic" />
                                                    <asp:CustomValidator
                                                        ID="cvUnitName"
                                                        runat="server"
                                                        ControlToValidate="txtModalName"
                                                        ErrorMessage="單位名稱已存在"
                                                        ValidationGroup="AddUnitModal"
                                                        OnServerValidate="cvUnitName_ServerValidate"
                                                        CssClass="invalid"
                                                        Display="Dynamic" />
                                                </td>
                                            </tr>

                                        </tbody>
                                    </table>
                                </div>
                                <asp:LinkButton
                                    ID="btnSave"
                                    runat="server"
                                    CssClass="btn btn-cyan d-table mx-auto mt-3"
                                    ValidationGroup="UnitModal"
                                    OnClick="btnSave_Click">
                                <i class="fa-solid fa-check"></i>
                                儲存
                                </asp:LinkButton>

                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <!-- 刪除確認 Modal -->
    <div class="modal fade" id="deleteModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="deleteModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-md">
            <div class="modal-content">
                <asp:UpdatePanel ID="upDeleteModal" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                                <i class="fa-solid fa-circle-xmark"></i>
                            </button>
                        </div>
                        <div class="modal-body">
                            <div class="d-flex flex-column align-items-center mb-5 gap-3">
                                <asp:HiddenField ID="hfDeleteID" runat="server" />
                                <h4 class="text-blue-green fw-bold">確定要刪除此單位?</h4>
                            </div>

                            <div class="d-flex gap-4 flex-wrap justify-content-center">
                                <button type="button" class="btn btn-gray" data-bs-dismiss="modal">
                                    取消</button>
                                <asp:Button ID="btnConfirmDelete" runat="server"
                                    CssClass="btn btn-cyan"
                                    Text="確定" OnClick="btnConfirmDelete_Click" />
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
                $('#<%= rblGovUnitType.ClientID %> input[type=radio]')
                    .addClass('form-check-input');
            }

            $('#unitModal').on('shown.bs.modal', applyClasses);

            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(applyClasses);
        });
    </script>
</asp:Content>


