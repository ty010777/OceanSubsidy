<%@ Page Language="C#" MasterPageFile="~/LoginMaster.master" AutoEventWireup="true" CodeFile="Logout.aspx.cs" Inherits="Logout" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">登出中 | 海洋委員會</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="login-wrap">
        <div class="login-container">
            <div class="logo">
                <img src="assets/img/login/login-logo.svg" alt="logo">
            </div>
            <h1>海洋領域補助案入口網</h1>
            <div class="text-center mt-4">
                <h2>登出中...</h2>
                <p>正在為您登出系統，請稍候...</p>
            </div>
        </div>
    </div>
</asp:Content>