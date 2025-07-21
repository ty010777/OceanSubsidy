<%@ Page Language="C#" AutoEventWireup="true" CodeFile="~/OFS/SCI/SciFunding.aspx.cs" Inherits="OFS_SciFunding" Culture="zh-TW" UICulture="zh-TW" MasterPageFile="~/OFS/SCI/OFSApplicationMaster.master" %>
<%@ Register Src="~/OFS/SCI/UserControls/SciFundingControl.ascx" TagName="SciFundingControl" TagPrefix="uc" %>

<asp:Content ID="ApplicationTitle" ContentPlaceHolderID="ApplicationTitle" runat="server">
    計畫申請 - 經費/人事 - 海洋領域補助計畫管理資訊系統
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadExtra" runat="server">
    <!-- jQuery -->
    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
    <!-- 自訂JavaScript -->
    <script src="<%= ResolveUrl("~/script/OFS/SCI/SciFunding.js") %>"></script>
</asp:Content>

<asp:Content ID="ApplicationContent" ContentPlaceHolderID="ApplicationContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
    
    <!-- 使用 UserControl -->
    <uc:SciFundingControl ID="sciFundingControl" runat="server" />

    <!-- 底部區塊 -->
    <div class="block-bottom bg-light-teal">
        <asp:Button ID="btnTempSave" runat="server" 
            Text="暫存" 
            CssClass="btn btn-outline-teal" 
            OnClientClick="collectAllFormData(); return true;"
            OnClick="btnTempSave_Click" />
        <asp:Button ID="btnSaveAndNext" runat="server" 
            Text="完成本頁，下一步" 
            CssClass="btn btn-teal" 
            OnClientClick="collectAllFormData(); return true;"
            OnClick="btnSaveAndNext_Click" />
    </div>

    <!-- Modal 職稱說明 -->
    <div class="modal fade" id="jobDetailModal" tabindex="-1" data-bs-backdrop="static" aria-labelledby="jobDetailModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </div>
                <div class="modal-body">
                    <h4 class="fs-18 text-green-light">各級研究員定義</h4>
                    <ul class="mt-3 d-flex flex-column gap-3">
                        <li>
                            <p>1.研究員級：指具有國內(外)大專教授、專業研究機構研究員及政府機關簡任技正或經政府認定之工程師等身份，或具備下列資格之一者：</p>
                            <ul class="text-gray mt-1">
                                <li>(1) 曾任國內、外大專副教授或相當職務3年以上者。</li>
                                <li>(2) 國內、外大學或研究院(所)得有博士學位，曾從事學術研究工作或專業工作3年以上者。</li>
                                <li>(3) 國內、外大學或研究院(所)得有碩士學位，曾從事學術研究工作或專業工作6年以上者。</li>
                                <li>(4) 國內、外大學或獨立學院畢業者，曾從事學術研究工作或專業工作9年以上者。</li>
                                <li>(5) 國內、外專科畢業，曾從事學術研究工作或專業工作12年以上者。</li>
                                <li>(6) 國內、外高中(職)畢業，且從事協助研究工作或專業工作達15年以上者。</li>
                                <li>(7) 國內、外高中(職)以下畢業，且從事協助研究工作達18年以上者。</li>
                            </ul>
                        </li>
                        <li>
                            <p>2.副研究員級：指具有國內(外)大專副教授、專業研究機構副研究員及政府機關薦任技正或政府認定之副工程師等以上身份，或具備下列資格之一者：</p>
                            <ul class="text-gray mt-1">
                                <li>(1) 曾任國內、外大專講師或研究機構相當職務3年以上者。</li>
                                <li>(2) 國內、外大學或研究院(所)得有博士學位者。</li>
                                <li>(3) 國內、外大學或研究院(所)得有碩士學位，曾從事學術研究工作或專業工作3年以上者。</li>
                                <li>(4) 國內、外大學或獨立學院畢業者，曾從事學術研究工作或專業工作6年以上者。</li>
                                <li>(5) 國內、外專科畢業，曾從事學術研究工作或專業工作9年以上者。</li>
                                <li>(6) 國內、外高中(職)畢業，且從事協助研究工作或專業工作達12年以上者。</li>
                                <li>(7) 國內、外高中(職)以下畢業，且從事協助研究工作達5年以上者。</li>
                            </ul>
                        </li>
                        <li>
                            <p>3.助理研究員級：指具有國內(外)大專講師、專業研究機構助理研究員政府機關委任技士或政府認定之助理工程師等以上身份，或具備下列資格之一者：</p>
                            <ul class="text-gray mt-1">
                                <li>(1) 國內、外大學或研究院(所)有碩士學位者。</li>
                                <li>(2) 國內、外大學或獨立學院畢業者，曾從事學術研究工作或專業工作3年以上者。</li>
                                <li>(3) 國內、外專科畢業，曾從事學術研究工作或專業工作6年以上者。</li>
                                <li>(4) 國內、外高中(職)畢業，且從事協助研究工作或專業工作達9年以上者。</li>
                                <li>(5) 國內、外高中(職)以下畢業，且從事協助研究工作達12年以上者。</li>
                            </ul>
                        </li>
                        <li>
                            <p>4.研究助理員級：指具有國內(外)大專助教、專業研究機構研究助理等身份，或具備下列資格之一者：</p>
                            <ul class="text-gray mt-1">
                                <li>(1) 國內、外大學或獨立學院畢業，得有學士學位。</li>
                                <li>(2) 國內、外專科畢業，且從事協助研究工作或專業工作達3年以上者。</li>
                                <li>(3) 國內、外高中(職)畢業，且從事協助研究工作達6年以上者。</li>
                                <li>(4) 國內、外高中(職)以下畢業，且從事協助研究工作達9年以上者。</li>
                            </ul>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
</asp:Content>