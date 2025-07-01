<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/OSI/OSIMaster.master" CodeFile="ActivityReportDetail.aspx.cs" Inherits="OSI_ActivityReportDetail" MaintainScrollPositionOnPostback="true" %>

<%@ Register TagPrefix="uc1" TagName="ReportForm" Src="~/OSI/ReportForm.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    活動資料填報  | 海洋科學調查活動填報系統
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Bread" runat="server">
    <img src="<%= ResolveUrl("~/assets/img/title-icon01.svg") %>" alt="logo">
    <div>
        <span>目前位置</span>
        <h2>活動資料填報</h2>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:reportform id="ReportForm" runat="server" />
</asp:Content>
