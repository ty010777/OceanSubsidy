<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SciMonthlyExecutionReport.aspx.cs" Inherits="OFS_SCI_SciMonthlyExecutionReport" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/SCI/SciInprogress.master" EnableViewState="true" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <!-- SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
    
    <!-- 內容區塊 -->
    <div class="block rounded-top-4">
        
        <!-- 月份切換 -->
        <asp:PlaceHolder ID="timelineContainer" runat="server" />

        <!-- 實際進度 -->
        <h5 class="square-title">
            <asp:Label ID="currentMonth" runat="server" Text="114年5月" /> 實際進度
        </h5>
        <div class="table-responsive mt-4">
            <table class="table align-middle gray-table">
                <thead>
                    <tr>
                        <th class="text-end">月份</th>
                        <th width="350">預定工作摘要</th>
                        <th>實際工作執行情形（條列式說明）</th>
                        <th width="150">累計預定進度%</th>
                        <th width="150">累計實際進度%</th>
                    </tr>
                </thead>
                <tbody id="actualProgressContainer">
                    <!-- 動態渲染實際進度內容 -->
                </tbody>
            </table>
        </div>

        <!-- 查核點 -->
        <div class="table-responsive">
            <table class="table align-middle gray-table">
                <thead>
                    <tr>
                        <th class="text-end">月份</th>
                        <th>本月查核點／未完成查核點</th>
                        <th>本月是否完成</th>
                        <th width="150">實際完成時間</th>
                        <th>年度目標達成數</th>
                    </tr>
                </thead>
                <tbody id="checkPointContainer">
                    <!-- 動態渲染查核點內容 -->
                </tbody>
            </table>
        </div>

        <!-- 累計執行經費 -->
        <h5 class="square-title mt-5">
            <asp:Label ID="currentMonth2" runat="server" Text="114年5月" /> 累計執行經費
        </h5>
        <div class="table-responsive mt-4">
            <table class="table align-middle gray-table">
                <thead>
                    <tr>
                        <th class="text-end"></th>
                        <th class="text-end">補助款 (A)</th>
                        <th class="text-end">配合款 (B)</th>
                        <th class="text-end">計畫總經費 (A+B)</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <th class="text-end">實支金額（支用數）
                                             
                        </th>
                        <td class="text-end">
                            <asp:TextBox ID="txtSubsidyAmount" ClientIDMode="Static"  runat="server" CssClass="form-control text-end" placeholder="請輸入" />
                        </td>
                        <td class="text-end">
                            <asp:TextBox ID="txtMatchingAmount"  ClientIDMode="Static"  runat="server" CssClass="form-control text-end" placeholder="請輸入" />
                        </td>
                        <td class="text-end">
                            <asp:Label ID="lblTotalBudget" ClientIDMode="Static"  runat="server" Text="" />
                        </td>
                    </tr>
                
                </tbody>
            </table>
        </div>

    </div>
    
    <!-- 底部區塊 -->
    <div class="block-bottom bg-light-teal">
        <button type="button" id="btnTempSave" class="btn btn-outline-teal" onclick="saveMonthlyReport()">暫存</button>
        <button type="button" id="btnSubmit" class="btn btn-teal btn-submit-icon" onclick="submitMonthlyReport()">提送</button>
    </div>

    <!-- JavaScript -->
    <script src="../../script/OFS/SCI/SciMonthlyExecutionReport.js"></script>
    <script>
        // 設定 ProjectID 供外部 JS 使用
        window.projectID = '<%= ProjectID %>';
    </script>
</asp:Content>