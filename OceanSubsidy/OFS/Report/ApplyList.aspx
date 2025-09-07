<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ApplyList.aspx.cs" Inherits="Report_ApplyList" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon06.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">報表查詢 / 申請計畫報表</asp:Content>

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
                        <option value="">114年</option>
                        <option value="">113年</option>
                    </select>
                </div>
                <div class="col-12 col-lg-4">
                    <div class="fs-16 text-gray mb-2">全部類別</div>
                    <select class="form-select" name="" id="">
                        <option value="">全部</option>
                        <option value="">科專</option>
                        <option value="">文化</option>
                        <option value="">學校民間</option>
                        <option value="">學校社團</option>
                    </select>
                </div>
                <div class="col-12 col-lg-4">
                    <div class="fs-16 text-gray mb-2">全部主管單位</div>
                    <select class="form-select" name="" id="">
                        <option value="">全部</option>
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


    <!-- 總計列表 -->
    <ul class="total-list multiple-option">
        <li class="all-total">
            <div class="total-item-title">總申請</div>
            <div class="total-item-content">
                <span class="count">36</span>
                <span class="unit">件</span>
            </div>
        </li>
        <li class="total-item">
            <label class="checke-total" for="apply-total">
                <div>
                    <div class="total-item-title">申請中</div>
                    <div class="total-item-content">
                        <span class="count">9</span>
                        <span class="unit">件</span>
                    </div>
                </div>
                <img class="check-icon" src="../../assets/img/check-white.svg">
            </label>
            <input id="apply-total" class="total-checkbox" type="checkbox" checked="">
        </li>
        <li class="total-item">
            <label class="checke-total" for="review-total">
                <div>
                    <div class="total-item-title">審查中</div>
                    <div class="total-item-content">
                        <span class="count">9</span>
                        <span class="unit">件</span>
                    </div>
                </div>
                <img class="check-icon" src="../../assets/img/check-white.svg">
            </label>
            <input id="review-total" class="total-checkbox" type="checkbox" checked="">
        </li>
        <li class="total-item">
            <label class="checke-total" for="pass-total">
                <div>
                    <div class="total-item-title">已核定</div>
                    <div class="total-item-content">
                        <span class="count">9</span>
                        <span class="unit">件</span>
                    </div>
                </div>
                <img class="check-icon" src="../../assets/img/check-white.svg">
            </label>
            <input id="pass-total" class="total-checkbox" type="checkbox">
        </li>
        <li class="total-item">
            <label class="checke-total" for="fail-total">
                <div>
                    <div class="total-item-title">未通過</div>
                    <div class="total-item-content">
                        <span class="count">9</span>
                        <span class="unit">件</span>
                    </div>
                </div>
                <img class="check-icon" src="../../assets/img/check-white.svg">
            </label>
            <input id="fail-total" class="total-checkbox" type="checkbox">
        </li>
    </ul>

    <!-- 列表內容 -->
    <div class="block rounded-bottom-4">
        <div class="title border-teal">
            <div class="d-flex align-items-center gap-2">
                <h4 class="text-teal">
                    <img src="../../assets/img/title-icon02-teal.svg" alt="logo">
                    <span>列表</span>
                </h4>
                <span>共 <span class="text-teal">35</span> 筆資料</span>
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
                            <div class="hstack align-items-center justify-content-center">
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
                                <span>申請本會補助</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-end">
                                <span>配合款</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-end">
                                <span>總經費</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-center">
                                <span>狀態</span>
                            </div>
                        </th>
                    </tr>
                    <tr class="total-row">
                        <td colspan="4" style="text-align: end;">總計</td>
                        <td class="text-end">60,000,000</td>
                        <td class="text-end">12,840,000</td>
                        <td class="text-end">82,840,000</td>
                        <td></td>
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
                        <td data-th="申請本會補助:" class="text-end">1,000,000</td>
                        <td data-th="配合款:" class="text-end">1,500,000</td>
                        <td data-th="總經費:" class="text-end">2,500,000</td>
                        <td data-th="狀態:" style="text-align: center; width: 100px;">
                            <span class="text-teal">申請中</span>
                        </td>
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