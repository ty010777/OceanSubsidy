<%@ Page Title="" Language="C#" MasterPageFile="~/OFSMaster.master" AutoEventWireup="true" CodeFile="PlanChangeRecords.aspx.cs" Inherits="OFS_PlanChangeRecords" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <!-- SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <script src="<%= ResolveUrl("~/script/OFS/PlanChangeRecords.js") %>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="d-flex justify-content-between mb-4">
        <div class="page-title">
            <img src="<%= ResolveUrl("~/assets/img/information-system-title-icon03.svg") %>" alt="logo">
            <div>
                <span>目前位置</span>
                <div class="d-flex align-items-end gap-3">
                    <h2 class="text-dark-green2">計畫變更紀錄</h2>
                 
                </div>
            </div>
        </div>
    </div>

    <div class="container-fluid">
        <!-- 計畫基本資料區塊 -->
        <div class="block rounded-top-4">
            <div class="row mb-4">
                <div class="col-12">
                    <h5 class="square-title">計畫資料</h5>
                    <div class="bg-light-gray p-4">
                        <ul class="lh-lg" style="margin-bottom: 0;">
                            <asp:Literal ID="litProjectInfo" runat="server"></asp:Literal>
                            <!-- 計畫基本資料會由後端程式碼動態產生 -->
                        </ul>
                    </div>
                </div>
            </div>

            <!-- 計畫變更紀錄區塊 -->
            <div class="row">
                <div class="col-12">
                    <h5 class="square-title mb-3">計畫變更紀錄</h5>

                    <div class="table-responsive">
                        <table id="ChangeRecordsTable" class="table table-bordered align-middle gray-table">
                            <thead class="table-light">
                                <tr>
                                    <th width="11%" class="text-center">版次</th>
                                    <th width="8%" class="text-center">變更時間</th>
                                    <th width="6%" class="text-center">變更者</th>
                                    <th width="12%" class="text-center">變更原因</th>
                                    <th width="31%" class="text-center">變更前</th>
                                    <th width="32%" class="text-center">變更後</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptChangeRecords" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="text-center" style="white-space: pre-line;"><%# Eval("Version") %></td>
                                            <td class="text-center"><%# Eval("ChangeDate") %></td>
                                            <td class="text-center"><%# Eval("ChangedBy") %></td>
                                            <td class="text-start">
                                                <div style="white-space: pre-wrap;"><%# Eval("ChangeReason") %></div>
                                            </td>
                                            <td class="text-start">
                                                <div style="white-space: pre-wrap;"><%# Eval("BeforeChange") %></div>
                                            </td>
                                            <td class="text-start">
                                                <div style="white-space: pre-wrap;"><%# Eval("AfterChange") %></div>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>

                    <!-- 沒有資料時的提示 -->
                    <asp:Panel ID="pnlNoData" runat="server" Visible="false" CssClass="text-center text-muted py-5">
                        <i class="fas fa-inbox fa-3x mb-3"></i>
                        <p class="fs-5">目前尚無變更紀錄</p>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
