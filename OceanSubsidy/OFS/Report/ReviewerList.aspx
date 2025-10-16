<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReviewerList.aspx.cs" Inherits="Report_ReviewerList" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="BackUrl" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon06.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">報表查詢 / 審查委員名單</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <!-- 查詢 -->
    <div class="search bg-gray mt-4">
        <h3 class="text-teal">
            <i class="fa-solid fa-magnifying-glass"></i>
            查詢
        </h3>

        <!-- 查詢表單 -->
        <div class="search-form" action="">
            <div class="row g-3">
                <div class="col-3">
                    <div class="fs-16 text-gray mb-2">審查階段</div>
                    <select class="form-select" name="" id="">
                        <option value="">申請計畫審查</option>
                        <option value="">執行計畫審查</option>
                    </select>
                </div>
                <div class="col-3">
                    <div class="fs-16 text-gray mb-2">委員姓名, Email</div>
                    <input type="text" name="" id="" class="form-control" placeholder="請輸入委員姓名, Email">
                </div>
                <div class="col-3">
                    <div class="fs-16 text-gray mb-2">開始時間</div>
                    <input type="date" name="" class="form-control" data-checkpoint-date-bound="true">
                </div>
                <div class="col-3">
                    <div class="fs-16 text-gray mb-2">結束時間</div>
                    <input type="date" name="" class="form-control" data-checkpoint-date-bound="true">
                </div>
            </div>

            <button type="button" class="btn btn-teal-dark d-table mx-auto">
                <i class="fa-solid fa-magnifying-glass"></i>
                查詢
            </button>
        </div>
    </div>

    <!-- 列表內容 -->
    <div class="block rounded-bottom-4">
        <div class="title border-teal">
            <div class="d-flex align-items-center gap-2">
                <h4 class="text-teal">
                    <img src="../../assets/img/title-icon02-teal.svg" alt="logo">
                    <span>列表</span>
                </h4>
                <span>共 <span class="text-teal">1</span> 筆資料</span>
            </div>

            <!-- 匯出功能 -->
            <button class="btn btn-teal-dark" type="button"><i class="fas fa-download"></i>匯出</button>
        </div>

        <div class="table-responsive" style="min-height: 400px;">
            <table class="table teal-table">
                <thead>
                    <tr>
                        <th>
                            <div class="hstack align-items-center justify-content-center">
                                <span>委員姓名</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-start">
                                <span>Email</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-center">
                                <span>審查階段</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-center">
                                <span>審查計畫件數</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-start">
                                <span>銀行帳戶</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-start">
                                <span>戶籍地址</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-start">
                                <span>更新時間</span>
                            </div>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td data-th="委員姓名:">林某某</td>
                        <td data-th="Email:" class="text-start">xxxxxxxxx@gmail.com</td>
                        <td data-th="審查階段:">申請計畫</td>
                        <td data-th="審查計畫件數:">
                            <a href="" class="link-black" target="_blank">10</a>
                        </td>
                        <td data-th="銀行帳戶:" class="text-start">004 臺灣銀行12345678909876</td>
                        <td data-th="戶籍地址:" class="text-start">407臺中市OO區OO路OO號6樓</td>
                        <td data-th="更新時間:" class="text-start">114/04/15 11:23:14</td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!-- 分頁 -->
        <div class="d-flex align-items-center justify-content-between flex-wrap gap-2">
            <nav class="pagination justify-content-start" aria-label="Pagination">
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
                    <select class="form-select" aria-label="選擇頁數">
                        <option value="1">1</option>
                    </select>
                    <span>頁</span>
                    <span>,</span>
                </div>
                <div class="page-number-control-item">
                    <span>每頁顯示</span>
                    <select class="form-select" aria-label="選擇每頁顯示筆數">
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
