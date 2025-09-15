<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SciUploadAttachmentsControl.ascx.cs" Inherits="OFS_SCI_UserControls_SciUploadAttachmentsControl" %>

<!-- 內容區塊 -->
<div class="block">
    <h5 class="square-title">上傳附件</h5>
    <p class="text-pink view-mode lh-base mt-3">
        請下載附件範本，填寫資料及公文用印後上傳（僅支援PDF格式上傳，每個檔案10MB以內）<br>
        計畫書請自行留存送審版電子檔，待決審核定經費後請提交修正計畫書以供核定。
    </p>
    <!-- 學研表單 (預設顯示，Academic/Legal) -->
    <div class="table-responsive mt-3 mb-0" id="academicForm" runat="server">
        <table class="table align-middle gray-table" id="academicTable">
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
                        <a href="javascript:void(0)" class="link-teal" onclick="window.SciUploadAttachments.downloadTemplate('FILE_AC1')">海洋委員會海洋科技專案補助作業要點</a>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td class="text-center">2</td>
                    <td>
                        <div>
                            <span class="text-pink view-mode">*</span>
                            海洋科技科專案計畫書
                        </div>
                        <button type="button" class="btn btn-sm btn-teal-dark rounded-pill mt-2 view-mode" 
                                onclick="window.SciUploadAttachments.downloadTemplate('FILE_AC2')">
                            <i class="fas fa-file-download me-1"></i> 範本下載
                        </button>
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatusAcademic2" runat="server" Text="未上傳" CssClass="text-muted"></asp:Label>
                    </td>
                    <td>
                        <input type="file" id="fileInput_FILE_AC2" accept=".pdf" style="display: none;" onchange="handleFileUpload('FILE_AC2', this)" />
                        <button class="btn btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput_FILE_AC2').click()">
                            <i class="fas fa-file-upload me-1"></i> 上傳
                        </button>
                        <asp:Panel ID="pnlFilesAcademic2" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">3</td>
                    <td>
                     
                        <div>
                            <span class="text-pink view-mode">*</span>
                            建議迴避之審查委員清單
                        </div>
                        <button type="button" class="btn btn-sm btn-teal-dark rounded-pill mt-2 view-mode" 
                                onclick="window.SciUploadAttachments.downloadTemplate('FILE_AC3')">
                            <i class="fas fa-file-download me-1"></i> 範本下載
                        </button>
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatusAcademic3" runat="server" Text="未上傳" CssClass="text-muted"></asp:Label>
                    </td>
                    <td>
                        <input type="file" id="fileInput_FILE_AC3" accept=".pdf" style="display: none;" onchange="handleFileUpload('FILE_AC3', this)" />
                        <button class="btn btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput_FILE_AC3').click()">
                            <i class="fas fa-file-upload me-1"></i> 上傳
                        </button>
                        <asp:Panel ID="pnlFilesAcademic3" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">4</td>
                    <td>

                         <div>
                            <span class="text-pink view-mode">*</span>
                            未違反公職人員利益衝突迴避法切結書
                        </div>
                        <button type="button" class="btn btn-sm btn-teal-dark rounded-pill mt-2 view-mode" 
                                onclick="window.SciUploadAttachments.downloadTemplate('FILE_AC4')">
                            <i class="fas fa-file-download me-1"></i> 範本下載
                        </button>
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatusAcademic4" runat="server" Text="未上傳" CssClass="text-muted"></asp:Label>
                    </td>
                    <td>
                        <input type="file" id="fileInput_FILE_AC4" accept=".pdf" style="display: none;" onchange="handleFileUpload('FILE_AC4', this)" />
                        <button class="btn btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput_FILE_AC4').click()">
                            <i class="fas fa-file-upload me-1"></i> 上傳
                        </button>
                        <asp:Panel ID="pnlFilesAcademic4" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">5</td>
                    <td>
                        <div>
                            <span class="text-pink view-mode">*</span>
                            蒐集個人資料告知事項暨個人資料提供同意書
                        </div>
                        <button type="button" class="btn btn-sm btn-teal-dark rounded-pill mt-2 view-mode" 
                                onclick="window.SciUploadAttachments.downloadTemplate('FILE_AC5')">
                            <i class="fas fa-file-download me-1"></i> 範本下載
                        </button> 
                             
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatusAcademic5" runat="server" Text="未上傳" CssClass="text-muted"></asp:Label>
                    </td>
                    <td>
                        <input type="file" id="fileInput_FILE_AC5" accept=".pdf" style="display: none;" onchange="handleFileUpload('FILE_AC5', this)" />
                        <button class="btn btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput_FILE_AC5').click()">
                            <i class="fas fa-file-upload me-1"></i> 上傳
                        </button>
                        <asp:Panel ID="pnlFilesAcademic5" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">6</td>
                    <td>
                        <div>
                            <span class="text-pink view-mode">*</span>
                            共同執行單位基本資料表
                        </div>
                        <button type="button" class="btn btn-sm btn-teal-dark rounded-pill mt-2 view-mode" 
                                onclick="window.SciUploadAttachments.downloadTemplate('FILE_AC6')">
                            <i class="fas fa-file-download me-1"></i> 範本下載
                        </button> 
                             
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatusAcademic6" runat="server" Text="未上傳" CssClass="text-muted"></asp:Label>
                    </td>
                    <td>
                        <input type="file" id="fileInput_FILE_AC6" accept=".pdf" style="display: none;" onchange="handleFileUpload('FILE_AC6', this)" />
                        <button class="btn btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput_FILE_AC6').click()">
                            <i class="fas fa-file-upload me-1"></i> 上傳
                        </button>
                        <asp:Panel ID="pnlFilesAcademic6" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">7</td>
                    <td>
                        <div>
                            <span class="text-pink view-mode">*</span>
                            申請人自我檢查表
                        </div>
                        <button type="button" class="btn btn-sm btn-teal-dark rounded-pill mt-2 view-mode" 
                                onclick="window.SciUploadAttachments.downloadTemplate('FILE_AC7')">
                            <i class="fas fa-file-download me-1"></i> 範本下載
                        </button> 
                             
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatusAcademic7" runat="server" Text="未上傳" CssClass="text-muted"></asp:Label>
                    </td>
                    <td>
                        <input type="file" id="fileInput_FILE_AC7" accept=".pdf" style="display: none;" onchange="handleFileUpload('FILE_AC7', this)" />
                        <button class="btn btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput_FILE_AC7').click()">
                            <i class="fas fa-file-upload me-1"></i> 上傳
                        </button>
                        <asp:Panel ID="pnlFilesAcademic7" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">8</td>
                    <td>
                        <a href="javascript:void(0)" class="link-teal" onclick="window.SciUploadAttachments.downloadTemplate('FILE_AC8')">簽約注意事項</a>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td class="text-center">9</td>
                    <td>
                        <div>
                            <span class="text-pink view-mode">*</span>
                            <asp:Label ID="lblContractName" runat="server" Text="海洋委員會補助科技專案計畫契約書"></asp:Label>
                        </div>
                        <button type="button" class="btn btn-sm btn-teal-dark rounded-pill mt-2 view-mode" 
                                onclick="window.SciUploadAttachments.downloadTemplate('FILE_AC9')">
                            <i class="fas fa-file-download me-1"></i>範本下載
                        </button> 
                        
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatus9" runat="server" Text="未上傳" CssClass="text-muted"></asp:Label>
                    </td>
                    <td>
                        <input type="file" id="fileInput_FILE_AC9" accept=".pdf" style="display: none;" onchange="handleFileUpload('FILE_AC9', this)" />
                        <button class="btn btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput_FILE_AC9').click()">
                            <i class="fas fa-file-upload me-1"></i> 上傳
                        </button>
                        <asp:Panel ID="pnlFilesAcademic9" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">10</td>
                    <td>
                        <a href="javascript:void(0)" class="link-teal" onclick="window.SciUploadAttachments.downloadTemplate('FILE_AC10')">海洋科技專案計畫會計科目編列與執行原則</a>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td class="text-center">11</td>
                    <td>
                        <div>
                            <span class="text-pink view-mode">*</span>
                            海洋科技專案成效追蹤自評表
                        </div>
                        <button type="button" class="btn btn-sm btn-teal-dark rounded-pill mt-2 view-mode"  
                                onclick="window.SciUploadAttachments.downloadTemplate('FILE_AC11')">
                            <i class="fas fa-file-download me-1"></i> 範本下載
                        </button> 
                        
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatusAcademic11" runat="server" Text="未上傳" CssClass="text-muted"></asp:Label>
                    </td>
                    <td>
                        <input type="file" id="fileInput_FILE_AC11" accept=".pdf" style="display: none;" onchange="handleFileUpload('FILE_AC11', this)" />
                        <button class="btn btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput_FILE_AC11').click()">
                            <i class="fas fa-file-upload me-1"></i> 上傳
                        </button>
                        <asp:Panel ID="pnlFilesAcademic11" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">12</td>
                    <td>
                        <a href="javascript:void(0)" class="link-teal" onclick="window.SciUploadAttachments.downloadTemplate('FILE_AC12')">研究紀錄簿使用原則</a>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td class="text-center">13</td>
                    <td>
                        <a href="javascript:void(0)" class="link-teal" onclick="window.SciUploadAttachments.downloadTemplate('FILE_AC13')">計畫書書脊（側邊）格式</a>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
            </tbody>
        </table>
    </div>

    <!-- 業者表單 (OceanTech) -->
    <div class="table-responsive mt-3 mb-0 d-none" id="oceanTechForm" runat="server">
        <table class="table align-middle gray-table" id="oceanTechTable">
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
                        <a href="javascript:void(0)" class="link-teal" onclick="window.SciUploadAttachments.downloadTemplate('FILE_OTech1')">海洋委員會海洋科技專案補助作業要點</a>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td class="text-center">2</td>
                    <td>
                        <div>
                            <span class="text-pink view-mode">*</span>
                            海洋科技科專案計畫書
                        </div>
                        <button type="button" class="btn btn-sm btn-teal-dark rounded-pill mt-2 view-mode" 
                                onclick="window.SciUploadAttachments.downloadTemplate('FILE_OTech2')">
                            <i class="fas fa-file-download me-1"></i> 範本下載
                        </button>
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatus_OTech2" runat="server" Text="未上傳" CssClass="text-muted"></asp:Label>
                    </td>
                    <td>
                        <input type="file" id="fileInput_FILE_OTech2" accept=".pdf" style="display: none;" onchange="handleFileUpload('FILE_OTech2', this)" />
                        <button class="btn btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput_FILE_OTech2').click()">
                            <i class="fas fa-file-upload me-1"></i> 上傳
                        </button>
                        <asp:Panel ID="pnlFiles_OTech2" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">3</td>
                    <td>
                        <div>
                            <span class="text-pink view-mode">*</span>
                            建議迴避之審查委員清單
                        </div>
                        <button type="button" class="btn btn-sm btn-teal-dark rounded-pill mt-2 view-mode" 
                                onclick="window.SciUploadAttachments.downloadTemplate('FILE_OTech3')">
                            <i class="fas fa-file-download me-1"></i> 範本下載
                        </button>
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatus_OTech3" runat="server" Text="未上傳" CssClass="text-muted"></asp:Label>
                    </td>
                    <td>
                        <input type="file" id="fileInput_FILE_OTech3" accept=".pdf" style="display: none;" onchange="handleFileUpload('FILE_OTech3', this)" />
                        <button class="btn btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput_FILE_OTech3').click()">
                            <i class="fas fa-file-upload me-1"></i> 上傳
                        </button>
                        <asp:Panel ID="pnlFiles_OTech3" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">4</td>
                    <td>
                        <div>
                            <span class="text-pink view-mode">*</span>
                            未違反公職人員利益衝突迴避法切結書
                        </div>
                        <button type="button" class="btn btn-sm btn-teal-dark rounded-pill mt-2 view-mode" 
                                onclick="window.SciUploadAttachments.downloadTemplate('FILE_OTech4')">
                            <i class="fas fa-file-download me-1"></i> 範本下載
                        </button>
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatus_OTech4" runat="server" Text="未上傳" CssClass="text-muted"></asp:Label>
                    </td>
                    <td>
                        <input type="file" id="fileInput_FILE_OTech4" accept=".pdf" style="display: none;" onchange="handleFileUpload('FILE_OTech4', this)" />
                        <button class="btn btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput_FILE_OTech4').click()">
                            <i class="fas fa-file-upload me-1"></i> 上傳
                        </button>
                        <asp:Panel ID="pnlFiles_OTech4" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">5</td>
                    <td>
                        <div>
                            <span class="text-pink view-mode">*</span>
                            蒐集個人資料告知事項暨個人資料提供同意書
                        </div>
                        <button type="button" class="btn btn-sm btn-teal-dark rounded-pill mt-2 view-mode" 
                                onclick="window.SciUploadAttachments.downloadTemplate('FILE_OTech5')">
                            <i class="fas fa-file-download me-1"></i> 範本下載
                        </button>
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatus_OTech5" runat="server" Text="未上傳" CssClass="text-muted"></asp:Label>
                    </td>
                    <td>
                        <input type="file" id="fileInput_FILE_OTech5" accept=".pdf" style="display: none;" onchange="handleFileUpload('FILE_OTech5', this)" />
                        <button class="btn btn-teal-dark view-mode"  type="button" onclick="document.getElementById('fileInput_FILE_OTech5').click()">
                            <i class="fas fa-file-upload me-1"></i> 上傳
                        </button>
                        <asp:Panel ID="pnlFiles_OTech5" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">6</td>
                    <td>
                        <div>
                            <span class="text-pink view-mode">*</span>
                            申請人自我檢查表
                        </div>
                        <button type="button" class="btn btn-sm btn-teal-dark rounded-pill mt-2 view-mode"  
                                onclick="window.SciUploadAttachments.downloadTemplate('FILE_OTech6')">
                            <i class="fas fa-file-download me-1"></i> 
                            範本下載
                        </button>
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatus6_OT" runat="server" Text="未上傳" CssClass="text-muted"></asp:Label>
                    </td>
                    <td>
                        <input type="file" id="fileInput_FILE_OTech6" accept=".pdf" style="display: none;" onchange="handleFileUpload('FILE_OTech6', this)" />
                        <button class="btn btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput_FILE_OTech6').click()">
                            <i class="fas fa-file-upload me-1"></i> 上傳
                        </button>
                        <asp:Panel ID="pnlFiles_OTech6" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">7</td>
                    <td>
                        <a href="javascript:void(0)" class="link-teal" onclick="window.SciUploadAttachments.downloadTemplate('FILE_OTech7')">簽約注意事項</a>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td class="text-center">8</td>
                    <td>
                        <div>
                            <span class="text-pink view-mode">*</span>
                            海洋科技業者科專計畫補助契約書
                        </div>
                   
                        <button type="button" class="btn btn-sm btn-teal-dark rounded-pill mt-2 view-mode" 
                                onclick="window.SciUploadAttachments.downloadTemplate('FILE_OTech8')">
                            <i class="fas fa-file-download me-1"></i>
                            範本下載
                        </button>
                    </td>
                    <td class="text-center">
                        <asp:Label ID="lblStatusOTech8" runat="server" Text="未上傳" CssClass="text-muted"></asp:Label>
                    </td>
                    <td>
                        <input type="file" id="fileInput_FILE_OTech8" accept=".pdf" style="display: none;" onchange="handleFileUpload('FILE_OTech8', this)" />
                        <button class="btn btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput_FILE_OTech8').click()">
                            <i class="fas fa-file-upload me-1"></i> 上傳
                        </button>
                        <asp:Panel ID="pnlFiles_OTech8" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="text-center">9</td>
                    <td>
                        <a href="javascript:void(0)" class="link-teal" onclick="window.SciUploadAttachments.downloadTemplate('FILE_OTech9')">研究紀錄簿使用原則</a>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td class="text-center">10</td>
                    <td>
                        <a href="javascript:void(0)" class="link-teal" onclick="window.SciUploadAttachments.downloadTemplate('FILE_OTech10')">海洋科技專案計畫會計科目編列與執行原則</a>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td class="text-center">11</td>
                    <td>
                        <a href="javascript:void(0)" class="link-teal" onclick="window.SciUploadAttachments.downloadTemplate('FILE_OTech11')">計畫書書脊（側邊）格式</a>
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

<script type="text/javascript">
// 上傳附件相關 JavaScript 函數
window.SciUploadAttachments = {
    // 處理檔案上傳（參考 SciInterimReport 的做法）
    handleFileUpload: function(attachmentNumber, fileInput) {
        const file = fileInput.files[0];
        if (!file) {
            return;
        }
        
        // 驗證檔案格式
        const fileExt = '.' + file.name.split('.').pop().toLowerCase();
        if (fileExt !== '.pdf') {
            Swal.fire({
                icon: 'warning',
                title: '檔案格式錯誤',
                text: '僅支援PDF格式檔案上傳',
                confirmButtonText: '確定'
            });
            fileInput.value = '';
            return;
        }
        
        // 檢查檔案大小 (10MB)
        const maxSize = 10 * 1024 * 1024;
        if (file.size > maxSize) {
            Swal.fire({
                icon: 'warning',
                title: '檔案過大',
                text: '檔案大小不能超過10MB',
                confirmButtonText: '確定'
            });
            fileInput.value = '';
            return;
        }
        
        
        // 使用iframe方式上傳
        const iframe = document.createElement('iframe');
        iframe.style.display = 'none';
        iframe.name = 'uploadFrame_' + attachmentNumber;
        document.body.appendChild(iframe);
        
        const form = document.createElement('form');
        form.method = 'POST';
        form.enctype = 'multipart/form-data';
        form.target = 'uploadFrame_' + attachmentNumber;
        form.action = 'SciUploadAttachments.aspx?action=upload&attachmentNumber=' + attachmentNumber + '&projectId=' + encodeURIComponent('<%= GetProjectId() %>');
        
        const fileField = document.createElement('input');
        fileField.type = 'file';
        fileField.name = 'file';
        fileField.files = fileInput.files;
        form.appendChild(fileField);
        
        document.body.appendChild(form);
        
        // 監聽上傳完成
        iframe.onload = function() {
            try {
                const response = iframe.contentDocument.body.textContent;
                if (response.startsWith('SUCCESS:')) {
                    const fileName = response.substring(8);
                    Swal.fire({
                        icon: 'success',
                        title: '上傳成功',
                        text: '檔案已成功上傳',
                        confirmButtonText: '確定'
                    }).then(() => {
                        // 重新載入頁面，但不帶 action 參數
                        window.location.href = window.location.pathname + window.location.search.replace(/[\?&]action=[^&]*/g, '');
                    });
                } else if (response.startsWith('ERROR:')) {
                    const errorMsg = response.substring(6);
                    Swal.fire({
                        icon: 'error',
                        title: '上傳失敗',
                        text: errorMsg,
                        confirmButtonText: '確定'
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: '上傳錯誤',
                        text: '上傳過程發生未知錯誤',
                        confirmButtonText: '確定'
                    });
                }
            } catch (e) {
                Swal.fire({
                    icon: 'error',
                    title: '上傳錯誤',
                    text: '上傳過程發生錯誤',
                    confirmButtonText: '確定'
                });
            }
            
            // 清理
            document.body.removeChild(form);
            document.body.removeChild(iframe);
            fileInput.value = '';
        };
        
        form.submit();
    },
    
    // 下載已上傳的檔案
    downloadFile: function(projectId, fileCode, fileName) {
        console.log('downloadFile called with projectId:', projectId, 'fileCode:', fileCode, 'fileName:', fileName);
        var url = 'SciUploadAttachments.aspx?action=downloadFile&projectId=' + encodeURIComponent(projectId) + 
                  '&fileCode=' + encodeURIComponent(fileCode) + 
                  '&fileName=' + encodeURIComponent(fileName);
        window.location.href = url;
    },
    
    // 下載範本檔案
    downloadTemplate: function(fileCode) {
        console.log('downloadTemplate called with fileCode:', fileCode);
        var url = 'SciUploadAttachments.aspx?action=downloadTemplate&fileCode=' + encodeURIComponent(fileCode) + '&ProjectID=' + encodeURIComponent('<%= GetProjectId() %>');
        console.log('Download URL:', url);
        window.location.href = url;
    },
    
    // 刪除檔案
    deleteFile: function(projectId, fileCode, btnElement) {
        Swal.fire({
            title: '確認刪除',
            text: '確定要刪除這個檔案嗎？',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: '刪除',
            cancelButtonText: '取消'
        }).then((result) => {
            if (result.isConfirmed) {
                var url = 'SciUploadAttachments.aspx?action=deleteFile&projectId=' + encodeURIComponent(projectId) + 
                          '&fileCode=' + encodeURIComponent(fileCode);
                
                fetch(url, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded',
                    }
                })
                .then(response => response.text())
                .then(data => {
                    data = data.replace(/<[^>]*>/g, '').trim(); // 移除 HTML 標籤與空白
                    if(data === 'SUCCESS') {
                        Swal.fire({
                            icon: 'success',
                            title: '刪除成功',
                            text: '檔案已成功刪除',
                            confirmButtonText: '確定'
                        }).then(() => {
                            // 重新載入頁面，但不帶 action 參數
                            window.location.href = window.location.pathname + window.location.search.replace(/[\?&]action=[^&]*/g, '');
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: '刪除失敗',
                            text: data,
                            confirmButtonText: '確定'
                        });
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    Swal.fire({
                        icon: 'error',
                        title: '刪除錯誤',
                        text: '刪除檔案時發生錯誤',
                        confirmButtonText: '確定'
                    });
                });
            }
        });
    }
};

// 為了向後兼容性，保留全域函數
function handleFileUpload(attachmentNumber, fileInput) {
    return window.SciUploadAttachments.handleFileUpload(attachmentNumber, fileInput);
}

function downloadFile(projectId, fileCode, fileName) {
    return window.SciUploadAttachments.downloadFile(projectId, fileCode, fileName);
}

function downloadTemplate(fileCode) {
    return window.SciUploadAttachments.downloadTemplate(fileCode);
}

function deleteFile(projectId, fileCode, btnElement) {
    return window.SciUploadAttachments.deleteFile(projectId, fileCode, btnElement);
}
</script>

<!-- 變更說明 UserControl -->
