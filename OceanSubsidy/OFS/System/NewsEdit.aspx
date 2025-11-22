<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NewsEdit.aspx.cs" Inherits="Admin_NewsEdit" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/CUL/Layout.master" %>

<asp:Content ContentPlaceHolderID="BackUrl" runat="server">
    <a class="text-teal-dark text-decoration-none" href="NewsList.aspx">
        <i class="fas fa-angle-left"></i>
        返回列表
    </a>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainIcon" runat="server"><%= ResolveUrl("~/assets/img/information-system-title-icon07.svg") %></asp:Content>

<asp:Content ContentPlaceHolderID="MainTitle" runat="server">公告管理</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <system-news-form :id="id" v-on:next="next"></system-news-form>
    <script>
        setupVueApp({
            setup() {
                const id = "<%= Request.QueryString["ID"] %>";

                const next = (newsId) => {
                    Swal.fire({ title: "儲存成功", icon: "success" }).then(() => {
                        if (!id) {
                            location.href = `NewsEdit.aspx?ID=${newsId}`;
                        }
                    });
                };

                return { id, next };
            }
        });
    </script>
</asp:Content>
