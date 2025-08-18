<%@ Page Language="C#" MasterPageFile="~/OSI/OSIMaster.master" AutoEventWireup="true" CodeFile="Import.aspx.cs" Inherits="OSI_Import" %>

<asp:Content ID="cTitle" ContentPlaceHolderID="TitleContent" runat="server">
    活動批次匯入  | 海洋科學調查活動填報系統
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Bread" runat="server">
    <img src="<%= ResolveUrl("~/assets/img/title-icon05.svg") %>" alt="logo">
    <div>
        <span>目前位置</span>
        <h2>活動批次匯入</h2>
    </div>
</asp:Content>

<asp:Content ID="cMain" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hdnPeriodID" runat="server" />

    <asp:MultiView ID="mvSteps" runat="server" ActiveViewIndex="0">

        <!-- ========== STEP1：上傳 Excel ========== -->
        <asp:View ID="viewStep1" runat="server">
            <div class="block rounded-4">

                <!-- 步驟導覽 -->
                <ul class="step mb-40">
                    <li class="step-item active">
                        <h4><span class="step-number">step1.</span> <span class="step-title">資料匯入</span></h4>
                        <div class="icon"><i class="fa-solid fa-arrow-right"></i></div>
                    </li>
                    <li class="step-item">
                        <h4><span class="step-number">step2.</span> <span class="step-title">資料檢核</span></h4>
                        <div class="icon"><i class="fa-solid fa-arrow-right"></i></div>
                    </li>
                    <li class="step-item">
                        <h4><span class="step-number">step3.</span> <span class="step-title">匯入結果</span></h4>
                        <div class="icon"><i class="fa-solid fa-arrow-right"></i></div>
                    </li>
                </ul>

                <!-- 上傳檔案 -->
                <ul class="d-grid gap-3 fs-18 mb-4">
                    <li>1. 請先下載範例檔案　
                        <span class="tag tag-gray">
                            <a href="<%= ResolveUrl("~/wwwroot/OSI/活動批次匯入範本.xlsx") %>" class="tag-link">Excel範例下載
                                <img src="<%= ResolveUrl("~/assets/img/icon-download.svg") %>" alt="範例下載">
                            </a>
                        </span>
                        <span class="tag tag-gray">
                            <a href="<%= ResolveUrl("~/wwwroot/OSI/活動批次匯入範本.csv") %>" class="tag-link">CSV範例下載
                                <img src="<%= ResolveUrl("~/assets/img/icon-download.svg") %>" alt="範例下載">
                            </a>
                        </span>
                        <span class="tag tag-gray">
                            <a href="<%= ResolveUrl("~/wwwroot/OSI/活動批次匯入範本.ods") %>" class="tag-link">ODS範例下載
                                <img src="<%= ResolveUrl("~/assets/img/icon-download.svg") %>" alt="範例下載">
                            </a>
                        </span>
                    </li>
                    <li>2. 上傳欲檢核的檔案（支援 Excel、CSV、ODS 格式）
                        <div class="input-group mt-3">
                            <asp:FileUpload ID="fuExcel" runat="server" CssClass="form-control" />

                            <asp:Button ID="btnUpload" runat="server"
                                Text="上傳"
                                CssClass="btn btn-cyan"
                                OnClick="btnUpload_Click" />
                        </div>
                    </li>
                </ul>

                <!-- 錯誤訊息 -->
                <asp:Label ID="lblUploadError" runat="server" CssClass="invalid"></asp:Label>


            </div>
        </asp:View>


        <!-- ========== STEP2：檢核結果（通過或失敗皆在此） ========== -->
        <asp:View ID="viewStep2" runat="server">
            <div class="block rounded-4">

                <!-- 步驟導覽 -->
                <ul class="step mb-40">
                    <li class="step-item active">
                        <h4><span class="step-number">step1.</span> <span class="step-title">資料匯入</span></h4>
                        <div class="icon"><i class="fa-solid fa-arrow-right"></i></div>
                    </li>
                    <li class="step-item active">
                        <h4><span class="step-number">step2.</span> <span class="step-title">資料檢核</span></h4>
                        <div class="icon"><i class="fa-solid fa-arrow-right"></i></div>
                    </li>
                    <li class="step-item">
                        <h4><span class="step-number">step3.</span> <span class="step-title">匯入結果</span></h4>
                        <div class="icon"><i class="fa-solid fa-arrow-right"></i></div>
                    </li>
                </ul>

                <h5 class="text-blue-green fw-bold mb-4">檢核結果</h5>
                <!-- 由後端填入「通過」或「錯誤筆數」訊息 -->
                <div class="mb-4">
                    <asp:Literal ID="litStep2Message" runat="server" />
                </div>

                <!-- 多筆檢核結果 -->
                <div class="table-responsive mb-4">
                    <asp:GridView ID="gvCheckResults" runat="server"
                        CssClass="table align-middle gray-table"
                        Width="100%" Style="table-layout: fixed;"
                        AutoGenerateColumns="true"
                        UseAccessibleHeader="true"
                        OnRowCreated="gvCheckResults_RowCreated"
                        OnRowDataBound="gvCheckResults_RowDataBound"
                        OnPreRender="gvCheckResults_PreRender" />
                </div>

                <!-- 上一步／下一步 -->
                <asp:Button ID="btnStep2Prev" runat="server"
                    Text="返回上一步，重新匯入"
                    CssClass="btn btn-outline-secondary me-2"
                    OnClick="btnReupload_Click" />

                <asp:Button ID="btnStep2Next" runat="server"
                    Text="下一步"
                    CssClass="btn btn-cyan"
                    OnClick="btnToStep3_Click"
                    Enabled="false" />

            </div>
        </asp:View>


        <!-- ========== STEP3：匯入完成 ========== -->
        <asp:View ID="viewStep3" runat="server">
            <div class="block rounded-4">

                <!-- 步驟導覽 -->
                <ul class="step mb-40">
                    <li class="step-item active">
                        <h4><span class="step-number">step1.</span><span class="step-title">資料匯入</span></h4>
                        <div class="icon"><i class="fa-solid fa-arrow-right"></i></div>
                    </li>
                    <li class="step-item active">
                        <h4><span class="step-number">step2.</span><span class="step-title">資料檢核</span></h4>
                        <div class="icon"><i class="fa-solid fa-arrow-right"></i></div>
                    </li>
                    <li class="step-item active">
                        <h4><span class="step-number">step3.</span><span class="step-title">匯入結果</span></h4>
                        <div class="icon"><i class="fa-solid fa-arrow-right"></i></div>
                    </li>
                </ul>

                <h5 class="text-blue-green fw-bold mb-4">成功匯入資料</h5>
                <p class="mb-4">請至「活動資料填報」編修或查詢您所匯入的資料。</p>

                <asp:Button ID="btnFinish" runat="server"
                    Text="完成"
                    CssClass="btn btn-cyan"
                    OnClick="btnFinish_Click" />
            </div>
        </asp:View>

    </asp:MultiView>


</asp:Content>




