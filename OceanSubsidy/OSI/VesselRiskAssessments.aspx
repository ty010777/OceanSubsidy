<%@ Page Language="C#" MasterPageFile="~/OSI/OSIMaster.master" AutoEventWireup="true" CodeFile="VesselRiskAssessments.aspx.cs" Inherits="OSI_VesselRiskAssessments" %>
<%@ Register TagPrefix="uc1" TagName="VesselRiskManage" Src="~/OSI/VesselRiskManage.ascx" %>

<asp:Content ID="cTitle" ContentPlaceHolderID="TitleContent" runat="server">
    研究船風險檢核  | 海洋科學調查活動填報系統
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
        <h2>研究船風險檢核</h2>
    </div>
</asp:Content>

<asp:Content ID="cMain" ContentPlaceHolderID="MainContent" runat="server">

    <uc1:VesselRiskManage ID="ucVesselRiskManage" runat="server" />
</asp:Content>