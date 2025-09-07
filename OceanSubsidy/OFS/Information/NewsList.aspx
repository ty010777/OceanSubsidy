<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NewsList.aspx.cs" Inherits="Information_NewsList" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon02.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">資訊公告欄</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <!-- 列表內容 -->
    <div class="block rounded-4 mt-4">
        <div class="title border-teal">
            <div class="d-flex align-items-center gap-2">
                <h4 class="text-teal">
                    <img src="<%= ResolveUrl("~/assets/img/title-icon02-teal.svg") %>" alt="logo">
                    <span>列表</span>
                </h4>
            </div>
        </div>

        <div class="table-responsive" style="min-height: 400px;">
            <table class="table teal-table" aria-label="公告列表">
                <thead>
                    <tr>
                        <th scope="col" style="width: 220px;">公告時間</th>
                        <th scope="col" class="text-start">標題</th>
                        <th scope="col" style="width: 300px;">發布單位</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td data-th="公告時間:">114/09/22</td>
                        <td data-th="公告標題:" class="text-start">
                            <a class="link-black" href="NewsDetail.aspx">公告標題444444</a>
                        </td>
                        <td data-th="發布單位:" class="text-start">海洋委員會科技文教處海洋科技科</td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!-- 分頁控制 -->
        <div class="d-flex align-items-center justify-content-between flex-wrap gap-2">
            <nav class="pagination" aria-label="Pagination">
                <button class="nav-button" aria-label="Previous page" disabled="">
                    <i class="fas fa-chevron-left"></i>
                </button>
                <button class="nav-button" aria-label="Next page" disabled="">
                    <i class="fas fa-chevron-right"></i>
                </button>
            </nav>
            <div class="page-number-control">
                <div class="page-number-control-item">
                    <span>跳到</span>
                    <select class="form-select jump-to-page">
                        <!-- 動態渲染頁數選項 -->
                    </select>
                    <span>頁</span>
                    <span>,</span>
                </div>
                <div class="page-number-control-item">
                    <span>每頁顯示</span>
                    <select id="ddlPageSize" class="form-select" onchange="changePageSize()">
                        <option value="10">10</option>
                        <option value="20">20</option>
                        <option value="30">30</option>
                    </select>
                    <span>筆</span>
                </div>
            </div>
        </div>
    </div>

</asp:Content>