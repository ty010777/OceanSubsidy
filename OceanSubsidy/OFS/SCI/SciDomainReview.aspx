<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SciDomainReview.aspx.cs" Inherits="OFS_SCI_SciDomainReview" Culture="zh-TW" UICulture="zh-TW" %>

<!doctype html>
<html class="no-js" lang="zh-Hant">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="x-ua-compatible" content="ie=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>海洋科學調查活動填報系統</title>
    
    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="<%= ResolveUrl("~/assets/vendor/bootstrap-5.3.3/dist/css/bootstrap.min.css") %>">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <!-- FontAwesome -->
    <link rel="stylesheet" href="<%= ResolveUrl("~/assets/vendor/fontawesome-free-6.5.2-web/css/all.min.css") %>">
    <link rel="stylesheet" href="<%= ResolveUrl("~/assets/vendor/fontawesome-free-6.5.2-web/css/all.css") %>">
    
    <!-- Google Icons -->
    <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Symbols+Rounded">
    <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Icons+Round">
    <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" />
    
    <!-- Google Fonts -->
    <link href="https://fonts.googleapis.com/css2?family=Roboto:ital,wght@0,100..900;1,100..900&display=swap" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Inter:ital,opsz,wght@0,14..32,100..900;1,14..32,100..900&display=swap" rel="stylesheet">
    
    <!-- 自訂 CSS -->
    <link rel="stylesheet" href="<%= ResolveUrl("~/assets/css/login.css") %>">
    <link rel="stylesheet" href="<%= ResolveUrl("~/assets/css/main.css") %>">
</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
        <asp:HiddenField ID="hdnReviewID" runat="server" />
        
        <main>
            <div class="mis-layout" style="grid-template-columns: 1fr;">
                <!-- 主要內容 -->
                <div class="mis-content">
                    <div class="mis-container">  
                        <div class="close-menu-logo d-block">
                            <div class="d-flex align-items-center flex-wrap">
                                <img class="img-fluid" src="<%= ResolveUrl("~/assets/img/ocean-logo.png") %>" alt="logo" style="width: 180px;"> 
                                <h2 class="text-dark-green">海洋領域補助計畫管理資訊系統</h2>
                            </div>
                        </div>

                        <div class="block rounded-4">
                            <!-- 上方資訊區 -->
                            <div class="bg-light-gray p-4 mb-5">
                                <ul class="d-flex flex-column gap-3">
                                    <li>
                                        <span class="text-gray">年度 :</span>
                                        <asp:Label ID="lblYear" runat="server" />
                                    </li>
                                    <li>
                                        <span class="text-gray">計畫編號 :</span>
                                        <asp:Label ID="lblProjectNumber" runat="server" />
                                    </li>
                                    <li>
                                        <span class="text-gray">計畫類別 :</span>
                                        <asp:Label ID="lblProjectCategory" runat="server" />
                                    </li>
                                    <li>
                                        <span class="text-gray">審查組別 :</span>
                                        <asp:Label ID="lblReviewGroup" runat="server" />
                                    </li>
                                    <li>
                                        <span class="text-gray">計畫名稱 :</span>
                                        <asp:Label ID="lblProjectName" runat="server" />
                                    </li>
                                    <li>
                                        <span class="text-gray">申請單位 :</span>
                                        <asp:Label ID="lblApplicantUnit" runat="server" />
                                    </li>
                                    <li>
                                        <span class="text-gray">書面審查資料 :</span>
                                        <asp:LinkButton ID="btnDownloadDocument" runat="server" 
                                            CssClass="btn btn-sm btn-teal-dark rounded-pill d-inline-flex align-items-center gap-2 py-1 px-2" 
                                            OnClick="btnDownloadDocument_Click">
                                            <i class="fas fa-file-download"></i>
                                            <asp:Label ID="lblDocumentName" runat="server" />
                                        </asp:LinkButton>
                                    </li>
                                </ul>
                            </div>

                            <!-- 內容 -->
                            <h5 class="square-title mb-3">審查作業 – <asp:Label ID="lblReviewStatusName" runat="server" /></h5> 
                            <ul class="d-flex flex-column gap-2">
                                <li>
                                    <span class="text-gray">審查委員 :</span>
                                    <asp:Label ID="lblReviewerName" runat="server" />
                                </li>
                                <li>
                                    <span class="text-gray">風險評估 :</span>      
                                    <asp:Label ID="lblRiskLevel" runat="server" CssClass="text-pink" />
                                    <span>( <a class="link-teal fw-bold" href="#" data-bs-toggle="modal" data-bs-target="#riskAssessmentModal">
                                        <asp:Label ID="lblRiskRecordCount" runat="server" />
                                    </a> 筆記錄)</span>
                                </li>
                            </ul>

                            <!-- 表格 -->
                            <div class="mt-4">
                                <table class="table align-middle gray-table side-table">
                                    <tbody>
                                        <tr>
                                            <th nowrap>評分</th>
                                            <td style="border: unset">
                                                <div class="sub-table">
                                                    <table class="table align-middle gray-table" style="min-width: unset;">
                                                        <thead>
                                                            <tr>
                                                                <th>評審項目</th>
                                                                <th>權重</th>
                                                                <th>評分 (0~100)</th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            <asp:Repeater ID="rptReviewItems" runat="server">
                                                                <ItemTemplate>
                                                                    <tr>
                                                                        <td class="text-start">
                                                                            <%# Eval("ItemName") %>
                                                                        </td>
                                                                        <td><%# Eval("Weight") %>%</td>
                                                                        <td>
                                                                            <asp:TextBox ID="txtItemScore" runat="server" 
                                                                                CssClass="form-control" 
                                                                                placeholder="請輸入"
                                                                                Text='<%# Eval("Score") %>' />
                                                                            <asp:HiddenField ID="hdnItemId" runat="server" Value='<%# Eval("Id") %>' />
                                                                        </td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th nowrap>審查意見</th>
                                            <td style="border: unset">
                                                <asp:TextBox ID="txtReviewComment" runat="server" 
                                                    CssClass="form-control" 
                                                    TextMode="MultiLine" 
                                                    Rows="5"
                                                    placeholder="請輸入審查意見" 
                                                    style="min-height: 120px;" />
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>

                            <!-- 最後更新時間 -->
                            <div class="text-center">
                                最後異動紀錄：<asp:Label ID="lblLastUpdateTime" runat="server" />，審查委員 <asp:Label ID="lblLastUpdateBy" runat="server" />
                            </div>
                            
                            <div class="d-flex gap-4 mt-4 flex-wrap justify-content-center">
                                <asp:Button ID="btnSaveDraft" runat="server" 
                                    Text="暫存" 
                                    CssClass="btn btn-outline-teal"
                                    OnClick="btnSaveDraft_Click" />
                                <asp:Button ID="btnSubmitReview" runat="server" 
                                    Text="提送審查結果"
                                    CssClass="btn btn-teal"
                                    OnClick="btnSubmitReview_Click" />
                            </div>
                        </div>
                    </div>
                    
                    <footer>
                        <ul>
                            <li>地址：806高雄市前鎮區成功二路25號4樓</li>
                            <li>電話：(07)338-1810     Copyright © 海洋委員會 版權所有</li>
                        </ul>
                    </footer>        
                </div>
                <!-- 主要內容 END-->
            </div>
        </main>

        <!-- Modal 風險評估 -->
        <div class="modal fade" id="riskAssessmentModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="riskAssessmentModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-xl">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="fs-24 fw-bold text-green-light">風險評估</h4>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                            <i class="fa-solid fa-circle-xmark"></i>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div class="bg-light-gray p-3 mb-4">
                            <ul class="lh-lg">
                                <li>
                                    <span class="text-gray">執行單位:</span>
                                    <asp:Label ID="lblExecutingUnit" runat="server" />
                                </li>
                                <li>
                                    <span class="text-gray">風險評估:</span>
                                    <asp:Label ID="lblModalRiskLevel" runat="server" CssClass="text-pink" />
                                </li>
                            </ul>
                        </div>

                        <div class="table-responsive">
                            <table class="table align-middle gray-table lh-base">
                                <thead>
                                    <tr>
                                        <th>計畫編號 / 計畫名稱</th>
                                        <th>查核日期 / 人員</th>
                                        <th>風險評估</th>
                                        <th>查核意見</th>
                                        <th>執行單位回覆</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>
                                            <a href="#" class="link-teal">SCI1140001 / 海洋環境監測預警系統建置計畫</a>
                                        </td>
                                        <td class="text-center">
                                            <div>114/08/30</div>
                                            <div>劉某人</div>
                                        </td>
                                        <td>中風險</td>
                                        <td>申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆</td>
                                        <td>申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆申請單位回覆</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- JavaScript 文件 -->
        <script src="<%= ResolveUrl("~/assets/vendor/bootstrap-5.3.3/dist/js/bootstrap.bundle.min.js") %>"></script>
    </form>
</body>
</html>