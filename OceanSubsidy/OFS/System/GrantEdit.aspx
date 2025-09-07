<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GrantEdit.aspx.cs" Inherits="Admin_GrantEdit" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon07.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">系統管理 / 補助計畫管理</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <div class="mt-3">
        <!-- 切換按鈕 -->
        <nav>
            <div class="tab" role="tablist">
                <button class="tab-link active" data-bs-toggle="tab" data-bs-target="#setting" type="button" aria-selected="true" role="tab">內容管理</button>
                <button class="tab-link" data-bs-toggle="tab" data-bs-target="#adm" type="button" aria-selected="false" tabindex="-1" role="tab">機制設定</button>
            </div>
        </nav>
        <!-- 內容 -->
        <div class="tab-content">
            <div class="tab-pane fade show" id="setting" role="tabpanel">
                <div class="block">
                    <table class="table align-middle gray-table side-table">
                        <tbody>
                            <tr>
                                <th>
                                    <span class="text-pink">*</span>
                                    補助類別(代碼)
                                </th>
                                <td>
                                    <asp:DropDownList CssClass="form-select" ID="ddlApplicationType1" runat="server"  />
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <span class="text-pink">*</span>
                                    年度
                                </th>
                                <td>
                                    <input type="text" class="form-control" placeholder="請輸入年度">
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <span class="text-pink">*</span>
                                    補助類別全稱
                                </th>
                                <td>
                                    <input type="text" class="form-control" placeholder="請輸補助類別全稱">
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <span class="text-pink">*</span>
                                    內容
                                </th>
                                <td>
                                    <textarea type="textarea" class="form-control" placeholder="請輸入年度"></textarea>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    申辦資格
                                </th>
                                <td>
                                    <textarea type="textarea" class="form-control" placeholder="請輸入年度"></textarea>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <span class="text-pink">*</span>
                                    申辦流程
                                </th>
                                <td>
                                    <textarea type="textarea" class="form-control" placeholder="請輸入年度"></textarea>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    應備物品
                                </th>
                                <td>
                                    <input type="text" class="form-control" placeholder="請輸補助類別全稱">
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <span class="text-pink">*</span>
                                    聯絡窗口
                                </th>
                                <td>
                                    <div class="d-flex align-items-center flex-wrap">
                                        <div class="d-flex align-items-center text-nowrap me-3">
                                            <label class="me-2">聯絡電話及分機</label>
                                            <input type="text" class="form-control" placeholder="請輸入聯絡電話及分機">
                                        </div>
                                        <div class="d-flex align-items-center text-nowrap">
                                            <label class="me-2">聯絡人</label>
                                            <input type="text" class="form-control" placeholder="請輸入聯絡人姓名">
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    線上申辦<br>
                                    參考資料
                                </th>
                                <td>
                                    <input type="text" class="form-control" placeholder="請輸補助類別全稱">
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <span class="text-pink">*</span>
                                    相關檔案<br>
                                    (顯示於首頁)
                                </th>
                                <td>
                                    <button class="btn btn-sm btn-teal-dark" type="button" onclick="">
                                        <i class="fas fa-file-upload me-1"></i>
                                        上傳
                                    </button>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    申請下架
                                </th>
                                <td>
                                    <input class="form-check-input" type="checkbox" id="" value="">
                                    <label class="form-check-label ms-1 mb-2" for="">申請E政府下架（請輸入原因）</label>
                                    <input type="text" class="form-control" placeholder="請輸補助類別全稱">
                                </td>
                            </tr>
                        </tbody>
                    </table>

                    <div class="d-flex gap-3 flex-wrap justify-content-center mt-4">
                        <button type="button" class="btn btn-outline-teal">
                            暫存
                        </button>
                        <button type="button" class="btn btn-teal">
                            <i class="fas fa-check"></i>
                            確定發布
                        </button>
                    </div>

                </div>
            </div>
            <div class="tab-pane fade show active" id="adm" role="tabpanel">
                <div class="block">
                    <table class="table align-middle gray-table side-table">
                        <tbody>
                            <tr>
                                <th>
                                    補助類別 全稱
                                </th>
                                <td>
                                    114年度海洋文化領航計畫
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    補助類別 簡稱
                                </th>
                                <td class="p-0">
                                    <div class="row align-items-center mb-0">
                                        <div class="col p-4">
                                            文化
                                        </div>
                                        <div class="col p-4 text-nowrap" style="border-right:8px solid #DDDDDD; background-color:#F5F5F5">代碼</div>
                                        <div class="col p-4">
                                            CUL
                                        </div>
                                        <div class="col p-4 text-nowrap" style="border-right:8px solid #DDDDDD; background-color:#F5F5F5">年度</div>
                                        <div class="col p-4">
                                            114
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    總預算經費(元)
                                </th>
                                <td>
                                    <input type="text" class="form-control" placeholder="總預算經費(元)">
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    身分標籤
                                </th>
                                <td>
                                    <input type="text" class="form-control">
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    申請期間
                                </th>
                                <td>
                                    <div class="d-flex align-items-center flex-wrap gap-2">
                                        <div class="input-group" style="width: 360px;">
                                            <input type="date" class="form-control" aria-label="申請期間開始日期">
                                            <span class="input-group-text">至</span>
                                            <input type="date" class="form-control" aria-label="申請期間結束日期">
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    計畫期程迄日
                                </th>
                                <td>
                                    <div class="d-flex align-items-center flex-wrap gap-2">
                                        <div class="input-group" style="width: 360px;">
                                            <input type="date" class="form-control" aria-label="計畫期程迄日">
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    申請階段審查
                                </th>
                                <td>
                                    <div>
                                        <div class="d-flex align-items-center mb-2">
                                            <input class="form-check-input" type="checkbox" id="" value="">
                                            <label class="form-check-label ms-2 me-2" for="">1.</label>
                                            <input type="text" class="form-control" aria-label="資格審查" style="width: 200px;" placeholder="資格審查">
                                        </div>
                                        <div class="d-flex align-items-center mb-2">
                                            <input class="form-check-input" type="checkbox" id="" value="">
                                            <label class="form-check-label ms-2 me-2" for="">2.</label>
                                            <input type="text" class="form-control" aria-label="初審" style="width: 200px;" placeholder="初審">
                                        </div>
                                        <div class="d-flex align-items-center mb-2">
                                            <input class="form-check-input" type="checkbox" id="" value="">
                                            <label class="form-check-label ms-2 me-2" for="">3.</label>
                                            <input type="text" class="form-control" aria-label="複審" style="width: 200px;" placeholder="複審">
                                        </div>
                                        <div class="d-flex align-items-center mb-2">
                                            <input class="form-check-input" type="checkbox" id="" value="">
                                            <label class="form-check-label ms-2 me-2" for="">4.</label>
                                            <input type="text" class="form-control" aria-label="決審核定" style="width: 200px;" placeholder="決審核定">
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    補正補件期限
                                </th>
                                <td class="d-flex align-items-center flex-wrap gap-2">
                                    <input class="form-check-input" type="checkbox" id="" value="">
                                    <label class="form-check-label" for="">有期限</label>
                                    <input type="text" class="form-control" style="width:100px" placeholder="">天
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    期中報告<br>
                                    繳交期限
                                </th>
                                <td class="d-flex align-items-center flex-wrap gap-2">
                                    <span class="d-flex align-items-center flex-wrap gap-2">
                                        <span class="input-group" style="width: 360px;">
                                            <input type="date" class="form-control" aria-label="期中報告繳交期限">
                                        </span>
                                    </span>
                                    <span class="p-2">或</span>
                                    <input class="form-check-input" type="checkbox" id="" value="">
                                    <label class="form-check-label" for="">計畫結束一個月後</label>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    期末/成果報告<br>
                                    繳交期限
                                </th>
                                <td>
                                    <div class="d-flex align-items-center flex-wrap gap-2">
                                        <div class="input-group" style="width: 360px;">
                                            <input type="date" class="form-control" aria-label="期末/成果報告繳交期限">
                                        </div>
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>

                    <div class="d-flex gap-3 flex-wrap justify-content-center mt-4">
                        <button type="button" class="btn btn-outline-teal">
                            儲存
                        </button>
                    </div>

                </div>
            </div>
        </div>
    </div>

</asp:Content>