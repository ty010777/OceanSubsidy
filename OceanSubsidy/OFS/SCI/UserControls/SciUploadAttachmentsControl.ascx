<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SciUploadAttachmentsControl.ascx.cs" Inherits="OFS_SCI_UserControls_SciUploadAttachmentsControl" %>
<%@ Register TagPrefix="uc" TagName="ChangeDescriptionControl" Src="~/OFS/SCI/UserControls/ChangeDescriptionControl.ascx" %>

<!-- 內容區塊 -->
<div class="block">
    <h5 class="square-title">上傳附件</h5>
    <p class="text-pink lh-base mt-3">
        請下載附件範本，填寫資料及公文用印後上傳（僅支援PDF格式上傳，每個檔案10MB以內）<br>
        計畫書請自行留存送審版電子檔，待決審核定經費後請提交修正計畫書以供核定。
    </p>
    <div class="table-responsive mt-3 mb-0">
        <table class="table align-middle gray-table">
            <thead class="text-center">
                <tr>
                    <th width="60">附件編號</th>
                    <th>附件名稱</th>
                    <th width="180">狀態</th>
                    <th width="350">上傳附件</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td class="text-center">1</td>
                    <td>
                        <a href="#" class="link-teal">海洋委員會海洋科技專案補助作業要點</a>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td class="text-center">2</td>
                    <td>
                        <div>
                            <span class="text-pink">*</span>
                            海洋科技科專案計畫書
                        </div>
                        <asp:Button ID="btnDownloadTemplate2" runat="server" 
                            CssClass="btn btn-sm btn-teal-dark rounded-pill mt-2" 
                            Text="📄 範本下載" OnClick="btnDownloadTemplate2_Click" />
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatus2" runat="server" CssClass="text-pink" Text="尚未上傳"></asp:Label>
                    </td>
                    <td>
                        <asp:FileUpload ID="fuAttachment2" runat="server" style="display:none;" />
                        <asp:Button ID="btnUpload2" runat="server" 
                            CssClass="btn btn-teal-dark" 
                            Text="📤 上傳" OnClick="btnUpload2_Click" />
                        <asp:Panel ID="pnlFiles2" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                            <!-- 上傳檔案標籤會動態顯示在這裡 -->
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">3</td>
                    <td>
                        <div>
                            <span class="text-pink">*</span>
                            建議迴避之審查委員清單
                        </div>
                        <asp:Button ID="btnDownloadTemplate3" runat="server" 
                            CssClass="btn btn-sm btn-teal-dark rounded-pill mt-2" 
                            Text="📄 範本下載" OnClick="btnDownloadTemplate3_Click" />
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatus3" runat="server" Text="已上傳"></asp:Label>
                    </td>
                    <td>
                        <asp:FileUpload ID="fuAttachment3" runat="server" style="display:none;" />
                        <asp:Button ID="btnUpload3" runat="server" 
                            CssClass="btn btn-teal-dark" 
                            Text="📤 上傳" OnClick="btnUpload3_Click" />
                        <asp:Panel ID="pnlFiles3" runat="server" CssClass="tag-group mt-2 gap-1">
                            <span class="tag tag-green-light">
                                <a class="tag-link" href="#" target="_blank">1140001_海洋科技科專案計畫書.pdf</a>
                                <button type="button" class="tag-btn">
                                    <i class="fa-solid fa-circle-xmark"></i>
                                </button>
                            </span>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">4</td>
                    <td>
                        <div>
                            <span class="text-pink">*</span>
                            未違反公職人員利益衝突迴避法切結書
                        </div>
                        <asp:Button ID="btnDownloadTemplate4" runat="server" 
                            CssClass="btn btn-sm btn-teal-dark rounded-pill mt-2" 
                            Text="📄 範本下載" OnClick="btnDownloadTemplate4_Click" />
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatus4" runat="server" Text="已上傳"></asp:Label>
                    </td>
                    <td>
                        <asp:FileUpload ID="fuAttachment4" runat="server" style="display:none;" />
                        <asp:Button ID="btnUpload4" runat="server" 
                            CssClass="btn btn-teal-dark" 
                            Text="📤 上傳" OnClick="btnUpload4_Click" />
                        <asp:Panel ID="pnlFiles4" runat="server" CssClass="tag-group mt-2 gap-1">
                            <span class="tag tag-green-light">
                                <a class="tag-link" href="#" target="_blank">1140001_海洋科技科專案計畫書111111111.pdf</a>
                                <button type="button" class="tag-btn">
                                    <i class="fa-solid fa-circle-xmark"></i>
                                </button>
                            </span>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">5</td>
                    <td>
                        <div>
                            <span class="text-pink">*</span>
                            蒐集個人資料告知事項暨個人資料提供同意書
                        </div>
                        <asp:Button ID="btnDownloadTemplate5" runat="server" 
                            CssClass="btn btn-sm btn-teal-dark rounded-pill mt-2" 
                            Text="📄 範本下載" OnClick="btnDownloadTemplate5_Click" />
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatus5" runat="server" CssClass="text-pink" Text="尚未上傳"></asp:Label>
                    </td>
                    <td>
                        <asp:FileUpload ID="fuAttachment5" runat="server" style="display:none;" />
                        <asp:Button ID="btnUpload5" runat="server" 
                            CssClass="btn btn-teal-dark" 
                            Text="📤 上傳" OnClick="btnUpload5_Click" />
                        <asp:Panel ID="pnlFiles5" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                            <!-- 上傳檔案標籤會動態顯示在這裡 -->
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">6</td>
                    <td>
                        <div>
                            <span class="text-pink">*</span>
                            共同執行單位基本資料表
                        </div>
                        <asp:Button ID="btnDownloadTemplate6" runat="server" 
                            CssClass="btn btn-sm btn-teal-dark rounded-pill mt-2" 
                            Text="📄 範本下載" OnClick="btnDownloadTemplate6_Click" />
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatus6" runat="server" CssClass="text-pink" Text="尚未上傳"></asp:Label>
                    </td>
                    <td>
                        <asp:FileUpload ID="fuAttachment6" runat="server" style="display:none;" />
                        <asp:Button ID="btnUpload6" runat="server" 
                            CssClass="btn btn-teal-dark" 
                            Text="📤 上傳" OnClick="btnUpload6_Click" />
                        <asp:Panel ID="pnlFiles6" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                            <!-- 上傳檔案標籤會動態顯示在這裡 -->
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">7</td>
                    <td>
                        <div>
                            <span class="text-pink">*</span>
                            申請人自我檢查表
                        </div>
                        <asp:Button ID="btnDownloadTemplate7" runat="server" 
                            CssClass="btn btn-sm btn-teal-dark rounded-pill mt-2" 
                            Text="📄 範本下載" OnClick="btnDownloadTemplate7_Click" />
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatus7" runat="server" CssClass="text-pink" Text="尚未上傳"></asp:Label>
                    </td>
                    <td>
                        <asp:FileUpload ID="fuAttachment7" runat="server" style="display:none;" />
                        <asp:Button ID="btnUpload7" runat="server" 
                            CssClass="btn btn-teal-dark" 
                            Text="📤 上傳" OnClick="btnUpload7_Click" />
                        <asp:Panel ID="pnlFiles7" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                            <!-- 上傳檔案標籤會動態顯示在這裡 -->
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">8</td>
                    <td>
                        <a href="#" class="link-teal">簽約注意事項</a>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td class="text-center">9</td>
                    <td>
                        <div>
                            <span class="text-pink">*</span>
                            海洋委員會補助科技專案計畫契約書
                        </div>
                        <asp:Button ID="btnDownloadTemplate9" runat="server" 
                            CssClass="btn btn-sm btn-teal-dark rounded-pill mt-2" 
                            Text="📄 範本下載" OnClick="btnDownloadTemplate9_Click" />
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatus9" runat="server" CssClass="text-pink" Text="尚未上傳"></asp:Label>
                    </td>
                    <td>
                        <asp:FileUpload ID="fuAttachment9" runat="server" style="display:none;" />
                        <asp:Button ID="btnUpload9" runat="server" 
                            CssClass="btn btn-teal-dark" 
                            Text="📤 上傳" OnClick="btnUpload9_Click" />
                        <asp:Panel ID="pnlFiles9" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                            <!-- 上傳檔案標籤會動態顯示在這裡 -->
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">10</td>
                    <td>
                        <a href="#" class="link-teal">海洋科技專案計畫會計科目編列與執行原則</a>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td class="text-center">11</td>
                    <td>
                        <div>
                            <span class="text-pink">*</span>
                            海洋科技專案成效追蹤自評表
                        </div>
                        <asp:Button ID="btnDownloadTemplate11" runat="server" 
                            CssClass="btn btn-sm btn-teal-dark rounded-pill mt-2" 
                            Text="📄 範本下載" OnClick="btnDownloadTemplate11_Click" />
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatus11" runat="server" CssClass="text-pink" Text="尚未上傳"></asp:Label>
                    </td>
                    <td>
                        <asp:FileUpload ID="fuAttachment11" runat="server" style="display:none;" />
                        <asp:Button ID="btnUpload11" runat="server" 
                            CssClass="btn btn-teal-dark" 
                            Text="📤 上傳" OnClick="btnUpload11_Click" />
                        <asp:Panel ID="pnlFiles11" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                            <!-- 上傳檔案標籤會動態顯示在這裡 -->
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">12</td>
                    <td>
                        <a href="#" class="link-teal">研究紀錄簿使用原則</a>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td class="text-center">13</td>
                    <td>
                        <a href="#" class="link-teal">計畫書書脊（側邊）格式</a>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
            </tbody>
        </table>
    </div>
</div>

<!-- 隱藏欄位用於資料交換 -->
<asp:HiddenField ID="hdnAttachmentData" runat="server" />

<!-- 變更說明 UserControl -->
<uc:ChangeDescriptionControl ID="ucChangeDescription" runat="server" />