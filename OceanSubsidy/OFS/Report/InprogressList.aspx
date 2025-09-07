<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InprogressList.aspx.cs" Inherits="Report_InprogressList" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon06.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">報表查詢 / 計畫執行情況報表</asp:Content>

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
                <div class="col-12 col-lg-4">
                    <div class="fs-16 text-gray mb-2">年度</div>
                    <select class="form-select" name="" id="">
                        <option value="">全部</option>
                        <option value="">114年</option>
                        <option value="">113年</option>
                    </select>
                </div>
                <div class="col-12 col-lg-4">
                    <div class="fs-16 text-gray mb-2">類別</div>
                    <select class="form-select" name="" id="">
                        <option value="">全部類別</option>
                        <option value="">科專</option>
                        <option value="">文化</option>
                        <option value="">學校民間</option>
                        <option value="">學校社團</option>
                    </select>
                </div>
                <div class="col-12 col-lg-4">
                    <div class="fs-16 text-gray mb-2">主管單位</div>
                    <select class="form-select" name="" id="">
                        <option value="">全部主管單位</option>
                        <option value="">主管單位1</option>
                        <option value="">主管單位2</option>
                        <option value="">主管單位3</option>
                    </select>
                </div>
            </div>

            <div class="row g-3">
                <div class="col-12 col-lg-6">
                    <div class="fs-16 text-gray mb-2">申請補助單位</div>
                    <input type="text" name="" id="" class="form-control" placeholder="請輸入申請補助單位">
                </div>
                <div class="col-12 col-lg-6">
                    <div class="fs-16 text-gray mb-2">計畫編號, 計畫名稱</div>
                    <input type="text" name="" id="" class="form-control" placeholder="請輸入計畫編號, 計畫名稱">
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
                        <th width="80">年度</th>
                        <th>
                            <div class="hstack align-items-center">
                                <span>類別</span>
                            </div>
                        </th>
                        <th width="240">
                            <div class="hstack align-items-center">
                                <span>計畫名稱</span>
                            </div>
                        </th>
                        <th width="240">
                            <div class="hstack align-items-center">
                                <span>申請單位</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-end">
                                <span>本會補助金額</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-end">
                                <span>配合款</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-end">
                                <span>實支金額</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-end">
                                <span>累積經費執行率</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-end">
                                <span>本會已撥金額</span>
                            </div>
                        </th>
                    </tr>
                    <tr class="total-row">
                        <td colspan="4" style="text-align: end;">總計</td>
                        <td class="text-end">60,000,000</td>
                        <td class="text-end">12,840,000</td>
                        <td class="text-end">60,000,000</td>
                        <td class="text-end">60,000,000</td>
                        <td class="text-end">60,000,000</td>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td data-th="年度:">114</td>
                        <td data-th="類別:">科專</td>
                        <td data-th="計畫名稱:" style="text-align: left;">
                            <a href="../SCI/SciApplication.aspx?ProjectID=114SCI0001" class="link-black" target="_blank">海洋大學海洋科技研究與管理計畫</a>
                        </td>
                        <td data-th="申請單位:" style="text-align: left;">海洋大學海洋科技研究與管理處</td>
                        <td data-th="本會補助金額:" class="text-end">1,000,000</td>
                        <td data-th="配合款:" class="text-end">1,500,000</td>
                        <td data-th="實支金額:" class="text-end">1,500,000</td>
                        <td data-th="累積經費執行率:" class="text-end">1,500,000</td>
                        <td data-th="本會已撥金額:" class="text-end">1,500,000</td>
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
                        <option value="2">2</option>
                        <option value="3">3</option>
                        <option value="4">4</option>
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