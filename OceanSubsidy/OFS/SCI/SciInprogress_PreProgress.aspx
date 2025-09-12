<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SciInprogress_PreProgress.aspx.cs" Inherits="OFS_SCI_SciInprogress_PreProgress" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/SCI/SciInprogress.master" EnableViewState="true" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <!-- SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    
    <!-- 預定進度頁面特定樣式 -->
    <style>
        .block {
            background: white;
            border-radius: 12px;
            padding: 30px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            margin-bottom: 20px;
        }
        
        .square-title {
            color: #26A69A;
            font-weight: 600;
            font-size: 18px;
            border-left: 4px solid #26A69A;
            padding-left: 12px;
            margin-bottom: 20px;
        }
        
        .side-table th {
            background: #f8f9fa;
            width: 150px;
            font-weight: 600;
            padding: 15px;
        }
        
        .side-table td {
            padding: 15px;
        }
        
        .block-bottom {
            background: #e8f4f3;
            padding: 20px;
            border-radius: 0 0 12px 12px;
            text-align: center;
            margin-top: -20px;
        }
        
        .block-bottom .btn {
            margin: 0 10px;
            min-width: 100px;
        }
        
        .btn-submit-icon:before {
            font-family: "Font Awesome 5 Free";
            content: "\f00c";
            font-weight: 900;
            margin-right: 5px;
        }
    </style>
    
    <!-- 引用獨立的JavaScript文件 -->
    <script src="../../script/OFS/SCI/SciInprogressPreProgress.js"></script>
    
    <!-- 頁面初始化腳本 -->
    <script>
        // 設定全域變數供JS文件使用
        window.projectID = '<%= ProjectID %>';
        window.txtCoExecutingUnitID = '<%= txtCoExecutingUnit.ClientID %>';
        window.txtMidReviewDateID = '<%= txtMidReviewDate.ClientID %>';
        window.txtFinalReviewDateID = '<%= txtFinalReviewDate.ClientID %>';
    </script>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
    
    <!-- 內容區塊 -->
    <div class="block rounded-top-4">
        <h5 class="square-title">基本資料</h5>
        <table class="table align-middle gray-table side-table mt-4">
            <tbody>
                <tr>
                    <th>計畫名稱</th>
                    <td><asp:Label ID="lblProjectName" runat="server" Text="" /></td>
                </tr>
                <tr>
                    <th>計畫編號</th>
                    <td><asp:Label ID="lblProjectID" runat="server" Text="" /></td>
                </tr>
                <tr>
                    <th>受補助單位</th>
                    <td><asp:Label ID="lblExecutingUnit" runat="server" Text="" /></td>
                </tr>
                <tr>
                    <th>計畫主持人</th>
                    <td><asp:Label ID="lblProjectManager" runat="server" Text="" /></td>
                </tr>
                <tr>
                    <th>共同執行單位</th>
                    <td><asp:TextBox ID="txtCoExecutingUnit" runat="server" CssClass="form-control" placeholder="請輸入共同執行單位" /></td>
                </tr>
                <tr>
                    <th>執行期程</th>
                    <td><asp:Label ID="lblExecutionPeriod" runat="server" Text="" /></td>
                </tr>
            </tbody>
        </table>

        <h5 class="square-title mt-5">計畫聯絡人資訊</h5>
        <asp:PlaceHolder ID="contactPersonnelContainer" runat="server" />
        <h5 class="square-title mt-5">預定報告期程</h5>
        <table class="table align-middle gray-table side-table mt-4">
            <tbody>
                <tr>
                    <th nowrap>
                        <span class="text-pink">*</span>
                        期中審查預定日期
                    </th>
                    <td>
                        <asp:TextBox ID="txtMidReviewDate" runat="server" CssClass="form-control" TextMode="Date" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <th nowrap>
                        <span class="text-pink">*</span>
                        期末審查預定日期
                    </th>
                    <td>
                        <asp:TextBox ID="txtFinalReviewDate" runat="server" CssClass="form-control" TextMode="Date" style="width: 250px;" />
                    </td>
                </tr>
            </tbody>
        </table>

        <h5 class="square-title mt-5">預定分月進度</h5>
        <p class="mt-3 mb-2 lh-base">各月份均應填列預定執行工作項目摘要(條列式呈現)。<br>
            查核點及預定進度欄位請依核定版計畫書之「預定進度及查核標準」內容查填。
        </p>
        <asp:PlaceHolder ID="monthlyProgressContainer" runat="server" />

        <!-- 預定請款時程 - 已隱藏
        <h5 class="square-title mt-5">預定請款時程</h5>
        <p class="mt-3 mb-2 lh-base text-teal fw-bold">核定經費:<asp:Label ID="lblApprovedBudget" runat="server" Text="1,000,000" />元</p>
        <div class="table-responsive mt-3">
            <table class="table align-middle gray-table">
                <thead>
                    <tr>
                        <th class="text-center" width="100">期數</th>
                        <th>請款階段</th>
                        <th>規定</th>
                        <th class="text-end">預定金額</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td class="text-center">1</td>
                        <td>契約簽訂後</td>
                        <td>核定經費 40%</td>
                        <td class="text-end"><asp:Label ID="lblFirstPayment" runat="server" Text="400,000" /></td>
                    </tr>
                </tbody>
            </table>
        </div>
        -->

    </div>
    
    <!-- 底部區塊 -->
    <div class="block-bottom bg-light-teal">
        <%-- <asp:Button ID="btnTempSave" runat="server" Text="暫存" CssClass="btn btn-outline-teal" OnClick="btnTempSave_Click" /> --%>
        <button type="button" id="btnSubmit" class="btn btn-teal btn-submit-icon" onclick="handleSubmit()">提送</button>
    </div>
</asp:Content>