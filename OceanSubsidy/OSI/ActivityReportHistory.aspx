<%@  Language="C#" MasterPageFile="~/OSI/OSIMaster.master" AutoEventWireup="true" CodeFile="ActivityReportHistory.aspx.cs" Inherits="OSI_ActivityReportHistory" %>
<%@ Register Src="~/OSI/ReportFormHistory.ascx" TagName="ReportFormHistory" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    活動資料填報  | 海洋科學調查活動填報系統
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Bread" runat="server">
    <img src="<%= ResolveUrl("~/assets/img/title-icon01.svg") %>" alt="logo">
    <div>
        <span>目前位置</span>
        <h2>活動填報歷程資料</h2>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- 歷程選擇區 -->
    <div class="block rounded-top-4">
        <div class="title">
            <h4>
                <img src="<%= ResolveUrl("~/assets/img/title-icon03.svg") %>" alt="logo" />
                歷程資料
            </h4>
        </div>
        <div class="p-3">
            <div class="row align-items-center">
                <div class="col-auto">
                    <label class="form-label fw-bold">選擇修改日期：</label>
                </div>
                <div class="col-auto">
                    <asp:DropDownList ID="ddlHistory" runat="server" CssClass="form-select"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlHistory_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
            </div>
        </div>

        <!-- 歷程內容區 -->
        <asp:PlaceHolder ID="phHistoryContent" runat="server" Visible="false">
            <uc1:reportFormHistory ID="ReportFormHistory" runat="server" />
        </asp:PlaceHolder>
    </div>


</asp:Content>
