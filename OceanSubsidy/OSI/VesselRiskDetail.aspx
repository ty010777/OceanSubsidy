<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/OSI/OSIMaster.master" CodeFile="VesselRiskDetail.aspx.cs" Inherits="OSI_VesselRiskDetail" MaintainScrollPositionOnPostback="true" %>

<%@ Register TagPrefix="uc1" TagName="VesselRiskForm" Src="~/OSI/VesselRiskForm.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    研究船風險檢核  | 海洋科學調查活動填報系統
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Bread" runat="server">
    <img src="<%= ResolveUrl("~/assets/img/title-icon01.svg") %>" alt="logo">
    <div>
        <span>目前位置</span>
        <h2>研究船風險檢核</h2>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:VesselRiskForm id="VesselRiskForm" runat="server" />
</asp:Content>