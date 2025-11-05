<%@ Page Title="" Language="C#" MasterPageFile="~/OFSMaster.master" AutoEventWireup="true" CodeFile="AuditRecords.aspx.cs" Inherits="OFS_AuditRecords" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <!-- SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <script src="<%= ResolveUrl("~/script/OFS/AuditRecords.js") %>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
      <div class="d-flex justify-content-between mb-4">
            <div class="page-title">
                <img src="<%= ResolveUrl("~/assets/img/information-system-title-icon03.svg") %>" alt="logo">
                <div>
                    <span>目前位置</span>
                    <div class="d-flex align-items-end gap-3">
                        <h2 class="text-dark-green2">查核紀錄</h2>
                        <a class="text-dark-green2 text-decoration-none" href="<%= ResolveUrl("~/OFS/inprogressList.aspx") %>" >
                            <i class="fas fa-angle-left"></i>
                            返回列表
                        </a>
                    </div>
                </div>
            </div>
        </div>
    <div class="container-fluid">
        <!-- 計畫資料區塊 -->
        <div class = "block rounded-top-4">
            <div class="row mb-4">
                <div class="col-12">
                    <div class="d-flex justify-content-between align-items-center mb-3">
                        <h5 class="square-title">查核作業</h5>
                    </div>

                    <div class="bg-light-gray p-4">
                        <style>
                            .lh-lg li {
                                margin-bottom: 8px;
                            }
                            .lh-lg li:last-child {
                                margin-bottom: 0;
                            }
                        </style>
                        <ul class="lh-lg" style="margin-bottom: 0;">
                            <asp:Literal ID="litProjectInfo" runat="server"></asp:Literal>
                            <!-- 基本資料會由後端程式碼動態產生 -->
                            <li>
                                <span class="text-gray fw-bold General-view">查核人員：</span>
                                <asp:TextBox ID="txtAuditorName" runat="server" CssClass="form-control d-inline-block General-view" placeholder="請輸入查核人員" style="width: 200px;" />
                            </li>
                            <li>
                                <span class="text-gray fw-bold General-view">查核日期：</span>
                                <asp:TextBox ID="txtAuditDate" runat="server" CssClass="form-control taiwan-date-picker General-view d-inline-block"
                                    placeholder="請點選選擇日期" style="width: 180px;" readonly="true" />
                            </li>
                            <li>
                                <span class="text-gray fw-bold General-view">風險評估：</span>
                                <asp:DropDownList ID="ddlRiskAssessment" runat="server" CssClass="form-select General-view d-inline-block" style="width: 150px;">
                                    <asp:ListItem Text="請選擇" Value="" />
                                    <asp:ListItem Text="低風險" Value="Low" />
                                    <asp:ListItem Text="中風險" Value="Medium" />
                                    <asp:ListItem Text="高風險" Value="High" />
                                </asp:DropDownList>
                            </li>
                            <li>
                                <span class="text-gray fw-bold General-view">查核意見：</span>
                                <asp:TextBox ID="txtAuditComment" runat="server" CssClass="form-control General-view textarea-auto-resize"
                                    TextMode="MultiLine" Rows="4" placeholder="請輸入查核意見" MaxLength="2000" style="width: 100%;" />
                            </li>
                        </ul>
                    </div>
                </div>
            </div>

            <!-- 查核紀錄區塊 -->
            <div class="row">
                <div class="col-12">
                    <div class="d-flex justify-content-between align-items-center mb-3">
                        <h5 class="square-title">查核紀錄</h5>
                        <asp:Button ID="btnExportRecords" runat="server" Text="匯出查核紀錄" CssClass="btn btn-teal-dark" OnClick="btnExportRecords_Click" />
                    </div>

                    <div class="table-responsive">
                        <table id="RecordsTable" class="table table-bordered align-middle gray-table">
                            <thead class="table-light">
                                <tr>
                                    <th width="12%" class="text-center">查核日期</th>
                                    <th width="15%" class="text-center">查核人員</th>
                                    <th width="10%" class="text-center">風險評估</th>
                                    <th width="30%" class="text-center">查核意見</th>
                                    <th width="33%" class="text-center">執行單位回覆</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptAuditRecords" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="text-center">
                                                <%# FormatCheckDate(Eval("CheckDate")) %>
                                            </td>
                                            <td class="text-center">
                                                <%# GetDisplayValue(Eval("ReviewerName").ToString()) %>
                                            </td>
                                            <td class="text-center">
                                                <%# GetRiskDisplayValue(Eval("Risk").ToString()) %>
                                            </td>
                                            <td class="text-start">
                                                <div style="max-height: 120px; overflow-y: auto;">
                                                    <%# GetDisplayValue(Eval("ReviewerComment").ToString()) %>
                                                </div>
                                            </td>
                                            <td>
                                                <asp:HiddenField ID="hiddenIdx" runat="server" Value='<%# Eval("idx") %>' />
                                                <asp:Panel ID="pnlExecutorReply" runat="server" Visible='<%# Eval("CanEditExecutorComment") %>'>
                                                    <asp:TextBox ID="txtExecutorReply" runat="server"
                                                        CssClass="form-control"
                                                        TextMode="MultiLine"
                                                        Rows="3"
                                                        placeholder="請輸入回覆內容"
                                                        style="resize: vertical; min-height: 80px;" />
                                                </asp:Panel>
                                                <asp:Panel ID="pnlExecutorComment" runat="server" Visible='<%# !((bool)Eval("CanEditExecutorComment")) %>'>
                                                    <div class="p-2 rounded">
                                                        <%# GetDisplayValue(Eval("ExecutorComment").ToString()) %>
                                                    </div>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>

                    <!-- 操作按鈕 -->
                    <div class="d-flex gap-3 justify-content-center mt-4">
                        <asp:Button ID="btnSubmitAuditResult" runat="server"
                            Text="提送查核結果"
                            CssClass="btn btn-teal"
                            OnClick="btnSubmitAuditResult_Click" />
                        <asp:Button ID="btnSubmitReply" runat="server"
                            Text="提送"
                            CssClass="btn btn-teal"
                            OnClick="btnSubmitReply_Click" />
                    </div>
                </div>
            </div>
            </div>
    </div>
</asp:Content>
