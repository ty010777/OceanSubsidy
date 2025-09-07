<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GrantList.aspx.cs" Inherits="Admin_GrantList" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon07.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">系統管理 / 補助計畫管理</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <!-- 查詢表單 -->
    <div class="search-form" action="">
        <div class="row g-3">
            <div class="col-3">
                <div class="fs-16 text-gray mb-2">年度</div>
                <select class="form-select" name="" id="">
                    <option value="">114年</option>
                    <option value="">113年</option>
                </select>
            </div>
        </div>
    </div>

    <!-- 列表內容 -->
    <div class="block rounded-4 mt-4">
        <div class="title border-teal">
            <div class="d-flex align-items-center gap-2">
                <h4 class="text-teal">
                    <img src="<%= ResolveUrl("~/assets/img/title-icon02-teal.svg") %>" alt="logo">
                    <span>列表</span>
                </h4>
            </div>
            <button type="button" class="btn btn-teal-dark" onclick="location.href='GrantEdit.aspx'">
                <i class="fa-solid fa-plus"></i>
                新增類別
            </button>
        </div>

        <div class="table-responsive" style="min-height: 400px;">
            <table class="table teal-table" aria-label="公告列表">
                <thead>
                    <tr>
                        <th scope="col">年度</th>
                        <th scope="col" class="text-start">補助類別全稱</th>
                        <th scope="col" class="text-start">簡稱</th>
                        <th scope="col">代碼</th>
                        <th scope="col">申請期間</th>
                        <th scope="col">管理</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td data-th="年度:">114</td>
                        <td data-th="補助類別全稱:" class="text-start">114年度補助學術機構、研究機關(構)及海洋科技業者執行海洋科技專案</td>
                        <td data-th="簡稱:" class="text-start">科專</td>
                        <td data-th="代碼:" class="text-start">SCI</td>
                        <td data-th="申請期間:" class="text-start">114/02/01 ~ 114/10/31</td>
                        <td data-th="管理:" class="text-center">
                            <div class="d-inline-flex gap-2">
                                <button class="btn btn-sm btn-teal-dark" type="button" onclick="location.href='GrantEdit.aspx?ID=1'"><i class="fa-solid fa-pen" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="管理"></i></button>
                            </div>
                        </td>
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