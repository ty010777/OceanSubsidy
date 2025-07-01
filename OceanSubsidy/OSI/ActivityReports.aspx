<%@ Page Language="C#" MasterPageFile="~/OSI/OSIMaster.master" AutoEventWireup="true" CodeFile="ActivityReports.aspx.cs" Inherits="OSI_ActivityReports" %>
<%@ Register TagPrefix="uc1" TagName="ReportManage" Src="~/OSI/ReportManage.ascx" %>

<asp:Content ID="cTitle" ContentPlaceHolderID="TitleContent" runat="server">
    活動資料填報  | 海洋科學調查活動填報系統
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .pagination > span {
            display: flex;
            gap: 4px;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Bread" runat="server">
    <img src="<%= ResolveUrl("~/assets/img/title-icon01.svg") %>" alt="logo">
    <div>
        <span>目前位置</span>
        <h2>活動資料填報</h2>
    </div>
</asp:Content>

<asp:Content ID="cMain" ContentPlaceHolderID="MainContent" runat="server">

    <uc1:ReportManage ID="ucReportStandalone" runat="server" />
</asp:Content>
