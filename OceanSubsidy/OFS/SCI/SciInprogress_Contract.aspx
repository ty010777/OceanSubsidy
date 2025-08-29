<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SciInprogress_Contract.aspx.cs" Inherits="OFS_SCI_SciInprogress_Contract" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/SCI/SciInprogress.master" EnableViewState="true" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <!-- 契約資料頁面特定樣式 -->
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
        
        .tag-group {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            margin-top: 8px;
        }
        
        .tag-link {
            color: inherit;
            text-decoration: none;
        }
        
        .tag-link:hover {
            text-decoration-line: underline;
        }
        
        .tag-btn {
            background: none;
            border: none;
            color: inherit;
            margin-left: 8px;
            cursor: pointer;
        }
    </style>
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
                    <th>執行期程</th>
                    <td><asp:Label ID="lblExecutionPeriod" runat="server" Text="" /></td>
                </tr>
            </tbody>
        </table>

        <h5 class="square-title mt-5">契約資料</h5>
        <table class="table align-middle gray-table side-table mt-4">
            <tbody>
                <tr>
                    <th><span class="text-pink">*</span>發文文號</th>
                    <td>
                        <asp:TextBox ID="txtDocumentNumber" runat="server" CssClass="form-control" placeholder="請輸入發文文號" />
                    </td>
                </tr>
                <tr>
                    <th><span class="text-pink">*</span>簽約日期</th>
                    <td>
                        <asp:TextBox ID="txtContractDate" runat="server" CssClass="form-control" TextMode="Date" style="width: 250px;" />
                    </td>
                </tr>
            </tbody>
        </table>

        <h5 class="square-title mt-5">上傳附件</h5>
        <p class="text-pink mt-3">請下載附件範本，填寫資料及公文用印後上傳（僅支援PDF格式上傳，每個檔案10MB以內）</p>
        <div class="table-responsive mt-3">
            <table class="table align-middle gray-table">
                <thead>
                    <tr>
                        <th width="70" class="text-center">附件編號</th>
                        <th>附件名稱</th>
                        <th class="text-center">狀態</th>
                        <th>上傳附件</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td class="text-center">1</td>
                        <td>
                            <div><span class="text-pink">*</span>保密切結書（請包含共同執行單位）</div>
                            <button class="btn btn-sm btn-teal-dark rounded-pill mt-2" type="button">
                                <i class="fas fa-file-download me-1"></i>
                                範本下載
                            </button>
                        </td>
                        <td class="text-center"><span class="text-pink">尚未上傳</span></td>
                        <td>
                            <button class="btn btn-sm btn-teal-dark" type="button">
                                <i class="fas fa-file-upload me-1"></i>
                                上傳
                            </button>
                        </td>
                    </tr>
                    <tr>
                        <td class="text-center">2</td>
                        <td>
                            <div><span class="text-pink">*</span>個資同意書（請包含共同執行單位）</div>
                            <button class="btn btn-sm btn-teal-dark rounded-pill mt-2" type="button">
                                <i class="fas fa-file-download me-1"></i>
                                範本下載
                            </button>
                        </td>
                        <td class="text-center"><span>已上傳</span></td>
                        <td>
                            <div class="d-flex align-items-center gap-2">
                                <button class="btn btn-sm btn-teal-dark" type="button">
                                    <i class="fas fa-file-upload me-1"></i>
                                    上傳
                                </button>
                                <div class="tag-group mt-0 gap-1">
                                    <span class="tag tag-green-light">
                                        <a class="tag-link" href="#" target="_blank">1140001_海洋科技科專案計畫書.pdf</a>
                                        <button type="button" class="tag-btn">
                                            <i class="fa-solid fa-circle-xmark"></i>
                                        </button>
                                    </span>
                                </div>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    
    <!-- 底部區塊 -->
    <div class="block-bottom bg-light-teal">
        <%-- <asp:Button ID="btnSave" runat="server" CssClass="btn btn-outline-teal"  --%>
        <%--             Text="暫存" OnClick="btnSave_Click" /> --%>
        <button type="button" class="btn btn-teal" onclick="<%= ClientScript.GetPostBackEventReference(btnSubmit, null) %>">
            <i class="fas fa-check"></i>
            提送
        </button>
        <asp:Button ID="btnSubmit" runat="server" style="display:none;" OnClick="btnSubmit_Click" />
    </div>
</asp:Content>