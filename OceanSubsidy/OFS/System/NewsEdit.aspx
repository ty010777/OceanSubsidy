<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NewsList.aspx.cs" Inherits="Admin_NewsList" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon07.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">公告管理</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <!-- 列表內容 -->
    <div class="block rounded-4 mt-4 d-flex flex-column gap-5">

        <!-- 公告內容 -->
        <div>
            <h5 class="square-title">公告內容</h5>
            <div class="table-responsive mt-3 mb-0">
                <table class="table align-middle gray-table side-table">
                    <tbody>
                        <tr>
                            <th>發佈者</th>
                            <td>海洋委員會科技文教處海洋科技科&nbsp; &nbsp; 鄭海委</td>
                        </tr>
                        <tr>
                            <th>
                                <span class="text-pink">*</span>公告標題
                            </th>
                            <td>
                                <input type="text" class="form-control" placeholder="請輸入標題">
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <span class="text-pink">*</span>公告期間
                            </th>
                            <td>
                                <div class="d-flex align-items-center flex-wrap gap-2">
                                    <div class="input-group" style="width: 360px;">
                                        <input type="date" class="form-control" aria-label="公告開始日期">
                                        <span class="input-group-text">至</span>
                                        <input type="date" class="form-control" aria-label="公告結束日期">
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <span class="text-pink">*</span>內容
                            </th>
                            <td>
                                <span class="form-control textarea" role="textbox" contenteditable="" data-placeholder="請輸入" aria-label="文本輸入區域" style="height: auto; overflow-y: hidden;"></span>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

        <!-- 相關附件 -->
        <div>
            <div class="d-flex mb-2 gap-3">
                <h5 class="square-title">
                    <span>相關附件</span>
                </h5>
                <button type="button" class="btn btn-sm btn-teal-dark py-1">
                    <i class="fa-solid fa-file-arrow-up"></i>
                    上傳
                </button>
            </div>
            <span class="text-pink">支援文件 pdf, odt, doc, docx (10MB以內)、ZIP 壓縮檔 (50MB以內)</span>
            <div class="tag-group mt-3">
                <span class="tag tag-green-light">
                    <a class="tag-link" href="#" target="_blank">1140001_海洋科技科專案計畫書.pdf</a>
                    <button type="button" class="tag-btn">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </span>
            </div>
        </div>

        <!-- 相關圖片 -->
        <div>
            <div class="d-flex mb-2 gap-3">
                <h5 class="square-title">
                    <span>相關圖片</span>
                </h5>
                <button type="button" class="btn btn-sm btn-teal-dark py-1">
                    <i class="fa-solid fa-file-arrow-up"></i>
                    上傳
                </button>
            </div>
            <span class="text-pink">支援 JPG, PNG，檔案大小 10MB以內</span>
            <div class="image-preview mt-3">
                <div class="image-preview__item">
                    <img src="assets/img/ex-map.png" alt="picture">
                    <div class="image-preview__item-overlay">
                        <button type="button" class="btn btn-remove-image" aria-label="移除圖片">
                            <i class="fa-solid fa-xmark"></i>
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <!-- 相關影片 -->
        <div>
            <h5 class="square-title">相關影片</h5>
            <div class="table-responsive mt-3">
                <table class="table align-middle gray-table">
                    <thead>
                        <tr>
                            <th width="80" class="text-center">項次</th>
                            <th>影片名稱</th>
                            <th>影片網址</th>
                            <th width="120">功能</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td class="text-center">1</td>
                            <td>
                                <input type="text" class="form-control" placeholder="請輸入影片名稱">
                            </td>
                            <td>
                                <input type="url" class="form-control" placeholder="請輸入影片網址">
                            </td>
                            <td>
                                <div class="d-flex gap-1">
                                    <button type="button" class="btn btn-sm btn-teal-dark" aria-label="刪除影片" tabindex="0">
                                        <i class="fas fa-trash-alt"></i>
                                    </button>
                                    <button type="button" class="btn btn-sm btn-teal-dark" aria-label="新增影片" tabindex="0">
                                        <i class="fas fa-plus"></i>
                                    </button>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>

</asp:Content>