<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SciRecusedListControl.ascx.cs" Inherits="OFS_SCI_UserControls_SciRecusedListControl" %>
<%@ Register TagPrefix="uc" TagName="ChangeDescriptionControl" Src="~/OFS/SCI/UserControls/ChangeDescriptionControl.ascx" %>

<!-- 內容區塊 -->
<div class="block">
    <h5 class="square-title">建議迴避之審查委員清單</h5>
    <div class="d-flex align-items-center gap-1 mt-4">
        <input type="checkbox" ID="chkNoAvoidance" runat="server"  Class="form-check-input check-teal" />
        <label for="<%=chkNoAvoidance.ClientID%>">無需迴避之審查委員</label>
    </div>
    <div class="table-responsive mt-3 mb-0">
        <table class="table align-middle gray-table" id="committeeTable">
            <thead class="text-center">
                <tr>
                    <th width="180">
                        <span class="text-pink view-mode">*</span>
                        姓名
                    </th>
                    <th>
                        <span class="text-pink view-mode">*</span>
                        任職單位
                    </th>
                    <th width="180">
                        <span class="text-pink view-mode">*</span>
                        職稱
                    </th>
                    <th>
                        <span class="text-pink view-mode">*</span>
                        應迴避之具體理由及事證
                    </th>
                    <th>功能</th>
                </tr>
            </thead>
            <tbody id="committeeTableBody">
                <tr>
                    <td><input type="text" class="form-control" name="committeeName" maxlength="100" /></td>
                    <td><input type="text" class="form-control" name="committeeUnit" placeholder="請輸入任職單位" maxlength="200" /></td>
                    <td><input type="text" class="form-control" name="committeePosition" placeholder="請輸入職稱" maxlength="100" /></td>
                    <td><input type="text" class="form-control" name="committeeReason" placeholder="請輸入應迴避之具體理由及事證" maxlength="500" /></td>
                    <td>
                        <button type="button" class="btn btn-sm btn-teal add-row me-1">
                            <i class="fas fa-plus"></i>
                        </button>
                        <button type="button" class="btn btn-sm btn-teal delete-row">
                            <i class="fas fa-trash-alt"></i>
                        </button>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    
    <ul>
        <li>1. 請填列與貴申請人有利益衝突專家學者建議清單，以利審查作業時能先排除邀請，以符公平審查原則。</li>
        <li>2. 若無建議迴避之審查委員，請勾選「無需迴避之審查委員」。</li>
        <li>3. 建議迴避之審查委員，請務必具體說明迴避理由及事證，否則不予以採納。</li>
    </ul>

    <h5 class="square-title mt-5">技術能力</h5>
    
    <div class="table-responsive mt-3 mb-0" id="techTable"  runat="server">
        <table class="table align-middle gray-table " >
            <thead class="text-center">
                <tr>
                    <th>
                        <span class="text-pink view-mode">*</span>
                        研發技術項目
                    </th>
                    <th>
                        <span class="text-pink view-mode">*</span>
                        TRL層級
                        <button type="button" class="btn-tooltip" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="本計畫執行前後技術成熟度(Technology Readiness Level)階段">
                            <i class="fas fa-question-circle"></i>
                        </button>
                    </th>
                    <th width="360">
                        <span class="text-pink view-mode">*</span>
                        技術進程概述
                    </th>
                    <th>功能</th>
                </tr>
            </thead>
            <tbody id="techTableBody">
                <tr>
                    <td><input type="text" class="form-control" name="techItem" placeholder="請輸入" maxlength="200" /></td>
                    <td>
                        <div class="input-group">
                            <span class="input-group-text">執行前</span>
                            <select class="form-select" name="trlPlanLevel">
                                <option value="0" selected disabled>請選擇</option>
                                <option value="1">TRL 1：界定機會與挑戰</option>
                                <option value="2">TRL 2：構思因應方案</option>
                                <option value="3">TRL 3：進行概念性驗證實驗</option>
                                <option value="4">TRL 4：進行關鍵要素之現場試驗</option>
                                <option value="5">TRL 5：驗證商品化之可行性</option>
                                <option value="6">TRL 6：完成實用性原型開發</option>
                                <option value="7">TRL 7：市場可及性</option>
                                <option value="8">TRL 8：建立商用</option>
                                <option value="9">TRL 9：達成持續生產</option>
                            </select>
                        </div>
                        <div class="input-group mt-2">
                            <span class="input-group-text">執行後</span>
                            <select class="form-select" name="trlTrackLevel">
                                <option value="0" selected disabled>請選擇</option>
                                <option value="1">TRL 1：界定機會與挑戰</option>
                                <option value="2">TRL 2：構思因應方案</option>
                                <option value="3">TRL 3：進行概念性驗證實驗</option>
                                <option value="4">TRL 4：進行關鍵要素之現場試驗</option>
                                <option value="5">TRL 5：驗證商品化之可行性</option>
                                <option value="6">TRL 6：完成實用性原型開發</option>
                                <option value="7">TRL 7：市場可及性</option>
                                <option value="8">TRL 8：建立商用</option>
                                <option value="9">TRL 9：達成持續生產</option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <textarea class="form-control" rows="3" name="techProcess" placeholder="請輸入" maxlength="500"></textarea>
                    </td>
                    <td>
                        <button type="button" class="btn btn-sm btn-teal add-row me-1">
                            <i class="fas fa-plus"></i>
                        </button>
                        <button type="button" class="btn btn-sm btn-teal delete-row">
                            <i class="fas fa-trash-alt"></i>
                        </button>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    
    <div class="mt-3">
        <table class="table align-middle gray-table side-table mt-2">
            <tbody>
                <tr>
                    <th>
                        <span class="text-pink view-mode">*</span>
                        技術能力與技術關聯圖
                    </th>
                    <td>
                        <a href="<%=ResolveUrl("~/assets/img/technology-related-examples.png")%>" class="link-teal view-mode" target="_blank">範例圖下載<i class="fas fa-file-download ms-1"></i></a>
                        <div class="input-group mt-3">
                            <input type="file" id="fileUploadTechDiagram" 
                                   class="form-control view-mode" 
                                   accept="image/*" />
                            <button type="button" id="btnUploadTechDiagram" 
                                    class="btn btn-teal view-mode">
                                上傳
                            </button>
                        </div>

                        <div id="techDiagramPreviewContainer" class="mt-3" style="display: none;">
                            <button type="button" id="btnDeleteTechDiagram" 
                                    class="btn btn-outline-danger ms-auto d-table mb-2 view-mode" >
                                刪除
                            </button>
                            <img id="techDiagramPreview" class="img-fluid" src="" alt="技術能力與技術關聯圖" />
                        </div>

                        <ul class="list-unstyled text-gray lh-base mt-2 view-mode">
                            <li>標記說明：『＊』表示我國已有之技術或產品（並註明公司名稱）</li>
                            <li>『＋』表示我國正在發展之技術或產品（並註明公司名稱</li>
                            <li>『－』表示我國尚未發展之技術或產品（並註明公司名稱）</li>
                        </ul>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</div>

<!-- 變更說明區塊 -->
    <uc:ChangeDescriptionControl ID="tab4_ucChangeDescription" runat="server" SourcePage="SciRecusedList" />
    
    <!-- 底部區塊 -->
    <div class="block-bottom bg-light-teal view-mode">
        <asp:Button ID="tab4_btnTempSave" runat="server" Text="暫存" CssClass="btn btn-outline-teal"
                    OnClientClick="if (typeof syncAllChangeDescriptions === 'function') { syncAllChangeDescriptions(); } return true;"
                    OnClick="btnSave_Click" />
        <asp:Button ID="tab4_btnNext" runat="server" Text="完成本頁，下一步" CssClass="btn btn-teal"
                    OnClientClick="if (typeof syncAllChangeDescriptions === 'function') { syncAllChangeDescriptions(); } return true;"
                    OnClick="btnNext_Click" />
    </div>
<!-- 隱藏欄位用於資料交換 -->
<asp:HiddenField ID="hdnCommitteeData" runat="server" />
<asp:HiddenField ID="hdnTechData" runat="server" />
<asp:HiddenField ID="hdnUploadedFile" runat="server" />