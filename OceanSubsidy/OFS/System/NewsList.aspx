<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NewsList.aspx.cs" Inherits="Admin_NewsList" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon07.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">系統管理 / 公告管理</asp:Content>

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
            <button type="button" class="btn btn-teal-dark" onclick="location.href='NewsEdit.aspx'">
                <i class="fa-solid fa-plus"></i>
                新增公告
            </button>
        </div>

        <div class="table-responsive" style="min-height: 400px;">
            <table class="table teal-table" aria-label="公告列表">
                <thead>
                    <tr>
                        <th scope="col" style="width: 220px;">公告期間</th>
                        <th class="text-start" scope="col">公告標題</th>
                        <th class="text-start" scope="col" style="width: 200px;">發布者</th>
                        <th scope="col" style="width: 180px;">功能</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td data-th="公告期間:">114/09/22 ~ 114/11/10</td>
                        <td data-th="公告標題:" class="text-start">公告標題444444</td>
                        <td data-th="發布者:" class="text-start">海洋科技科 鄒海委</td>
                        <td data-th="功能:" class="text-center">
                            <div class="d-inline-flex gap-2">
                                <button class="btn btn-sm btn-teal-dark" type="button" onclick="location.href='NewsEdit.aspx?id=1'"><i class="fa-solid fa-pen" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="編輯"></i></button>
                                <button class="btn btn-sm btn-teal-dark" type="button" onclick="deleteNews()"><i class="fa-solid fa-xmark" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="刪除"></i></button>
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
                        <option value="1">1</option>
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

    <!-- Modal 刪除公告 -->
    <div class="modal fade" id="newsDeleteModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="newsDeleteModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="fs-24 fw-bold text-green-light">確定刪除補助計畫案件?</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </div>
                <div class="modal-body">
                    <div class="d-flex gap-4 flex-wrap justify-content-center mt-5">
                        <button type="button" class="btn btn-gray" data-bs-dismiss="modal">
                            取消
                        </button>
                        <asp:Button ID="btnConfirmDelete" runat="server" Text="確認刪除" CssClass="btn btn-teal" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        // 刪除公告
        function deleteNews(newsId) {
            $('#newsDeleteModal').modal('show');
        }
    </script>

</asp:Content>