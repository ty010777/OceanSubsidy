<%@ Page Language="C#" MasterPageFile="~/OSI/OSIMaster.master" AutoEventWireup="true" CodeFile="ActivityMap.aspx.cs" Inherits="OSI_ActivityMap" %>

<asp:Content ID="cTitle" ContentPlaceHolderID="TitleContent" runat="server">
    空間資訊圖台  | 海洋科學調查活動填報系統
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        /* 確保 iframe 佔滿整個可視範圍 */
        html, body {
            margin: 0;
            padding: 0;
            width: 100%;
            height: 100%;
            height: 100%;
        }

        #mapFrame {
            width: 100%;
            height: 100vh;
            border: none;
        }

        .mis-container {
            max-width: none;
            width: 100%;
            margin: 0 !important;
            padding: 0 !important;
        }
    </style>
</asp:Content>

<asp:Content ID="ContentBody" ContentPlaceHolderID="MainContent" runat="server">
    <!-- 直接用 iframe 嵌入我們剛才修改好的 Map.aspx -->
    <iframe
        id="mapFrame"
        src="<%= ResolveUrl("~/Map.aspx?codes=02,05,01,04,09") %>"
        frameborder="0"
        scrolling="no"></iframe>
</asp:Content>
