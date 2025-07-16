<%@ Page Language="C#" MasterPageFile="~/LoginMaster.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="cTitle" ContentPlaceHolderID="TitleContent" runat="server">
    海洋領域補助案入口網 | 海洋委員會
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="login-wrap">
        <div class="portal-container">
            <div class="logo">
                <img src="<%= ResolveUrl("~/assets/img/login/login-logo.svg") %>" alt="logo" class="img-fluid" />
            </div>
            <h1>海洋領域補助案入口網</h1>

            <div class="portal-menu">

                <!-- 系統 1 -->
                <div class="portal-menu-item">
                    <div class="info-wrap">
                        <h2>海洋科學調查活動<span>填報系統</span></h2>
                        <a href="<%= ResolveUrl("~/OSI/ActivityReports.aspx") %>" class="btn btn-ocean-blue rounded-pill">由此進入</a>
                    </div>
                    <div class="img">
                        <img
                            class="img-fluid w-100"
                            src="<%= ResolveUrl("~/assets/img/portal-menu1.svg") %>"
                            alt="海洋科學調查活動填報系統" />
                    </div>
                </div>

                <!-- 系統 2 -->
                <div class="portal-menu-item">
                    <div class="info-wrap">
                        <h2>海洋領域補助計畫管理<span>資訊系統</span></h2>
                        <a href="#" class="btn btn-ocean-blue rounded-pill">由此進入</a>
                    </div>
                    <div class="img">
                        <img
                            class="img-fluid w-100"
                            src="<%= ResolveUrl("~/assets/img/portal-menu2.svg") %>"
                            alt="海洋領域補助計畫管理資訊系統" />
                    </div>
                </div>

                <!-- 系統 3 -->
                <div id="manage" runat="server" class="portal-menu-item">
                    <div class="info-wrap">
                        <h2 class="text-blue-purple">帳號權限管理</h2>
                        <a href="<%= ResolveUrl("~/Manage/Users.aspx") %>" class="btn btn-blue-purple rounded-pill">由此進入</a>                     
                    </div>
                    <div class="img align-items-center justify-content-center">
                        <img
                            class="img-fluid"
                            src="<%= ResolveUrl("~/assets/img/portal-menu3.svg") %>"
                            alt="帳號權限管理" />
                    </div>
                </div>

            </div>

        </div>
    </div>
</asp:Content>
