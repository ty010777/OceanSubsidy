<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ClbApplicationControl.ascx.cs" Inherits="OFS_CLB_UserControls_ClbApplicationControl" %>


<!-- 申請表區塊 -->
<div id="applicationFormSection">

    <div class="block">
        <h5 class="square-title">申請基本資料</h5>
        <div class="mt-4">
            <table class="table align-middle gray-table side-table">
                <tbody>
                    <tr>
                        <th style="width: 15%;">年度</th>
                        <td style="width: 35%;">
                            <asp:Label ID="lblYear" runat="server"  /> 
                            <asp:HiddenField ID="hidYear" runat="server" />
                        </td>
                        <th style="width: 15%; background-color: #f8f9fa; color: #6c757d; text-align: end; padding-right: 24px; vertical-align: middle;">計畫編號</th>
                        <td style="width: 35%;">
                            <asp:Label ID="lblProjectID" runat="server"  />
                            <asp:HiddenField ID="hidProjectID" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <th>補助計畫類別 <span class="text-danger view-mode">*</span></th>
                        <td colspan="3">
                            <asp:Label ID="lblSubsidyPlanType" runat="server" />
                            <asp:HiddenField ID="hidSubsidyPlanType" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <th>計畫名稱 <span class="text-danger view-mode">*</span></th>
                        <td colspan="3">
                            <asp:TextBox ID="txtProjectNameTw" runat="server" CssClass="form-control" 
                                       placeholder="請輸入計畫名稱" MaxLength="200" style="width: 500px;" 
                                       Text="" />
                        </td>
                    </tr>
                    <tr>
                        <th>申請補助類型 <span class="text-danger view-mode">*</span></th>
                        <td colspan="3">
                            <div class="form-check-input-group d-flex">
                                <asp:RadioButton ID="rbSubsidyTypeCreate" runat="server" GroupName="SubsidyType" 
                                               Text="創社補助" Value="Startup" ClientIDMode="Static" />
                                <asp:RadioButton ID="rbSubsidyTypeOperation" runat="server" GroupName="SubsidyType" 
                                               Text="社務補助" Value="Admin" ClientIDMode="Static"/>
                                <asp:RadioButton ID="rbSubsidyTypeActivity" runat="server" GroupName="SubsidyType" 
                                               Text="公共活動費" Value="Public" ClientIDMode="Static"/>
                            </div>
                        </td>
                    </tr>
                  
                    <tr>
                        <th>申請單位 <span class="text-danger view-mode">*</span></th>
                        <td colspan="3">
                            <div class="d-flex align-items-center gap-2">
                                <asp:TextBox ID="txtSchoolName" runat="server" CssClass="form-control" 
                                           placeholder="學校名稱" MaxLength="100" style="width: 250px;" />
                                <asp:TextBox ID="txtClubName" runat="server" CssClass="form-control" 
                                           placeholder="社團全名" MaxLength="100" style="width: 250px;" />
                            </div>
                        </td>
                    </tr>
                    <tr class="affairs-view">
                        <th >成立日期<span class="text-danger view-mode">*</span></th>
                        <td colspan="3">
                            <asp:TextBox ID="txtCreationDate" runat="server" CssClass="form-control taiwan-date-picker"
                                       placeholder="請點選選擇日期" style="width: 200px;" readonly />

                        </td>
                    </tr>
                    <tr class="public-view">
                        <th>學校統一編號 <span class="text-danger view-mode">*</span></th>
                        <td colspan="3">
                            <asp:TextBox ID="txtSchoolIDNumber" runat="server" CssClass="form-control" 
                                       placeholder="請輸入學校統一編號" MaxLength="10" style="width: 200px;" />
                        </td>
                    </tr>
                    <tr class="public-view">
                        <th>地址 <span class="text-danger view-mode">*</span></th>
                        <td colspan="3">
                            <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" 
                                       placeholder="請輸入學校地址" MaxLength="200" style="width: 500px;" />
                        </td>
                    </tr>
                </tbody>
            </table>
            <table class="table align-middle gray-table side-table">
                <thead>
                    <tr>
                        <th style="width: 15%;">人員 </th>
                        <th style="width: 25%;">姓名 <span class="text-danger view-mode">*</span></th>
                        <th style="width: 25%;">職稱 <span class="text-danger view-mode">*</span></th>
                        <th style="width: 35%;">手機號碼 <span class="text-danger view-mode">*</span></th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <th>社團指導老師 <span class="text-danger view-mode">*</span></th>
                        <td>
                            <asp:TextBox ID="txtTeacherName" runat="server" CssClass="form-control" 
                                       placeholder="請輸入姓名" MaxLength="50" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtTeacherJobTitle" runat="server" CssClass="form-control" 
                                       placeholder="請輸入職稱" MaxLength="50" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtTeacherPhone" runat="server" CssClass="form-control" 
                                       placeholder="請輸入手機號碼" MaxLength="20" />
                        </td>
                    </tr>
                    <tr>
                        <th>社團業務聯絡人</th>
                        <td>
                            <asp:TextBox ID="txtContactName" runat="server" CssClass="form-control" 
                                       placeholder="請輸入姓名" MaxLength="50" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtContactJobTitle" runat="server" CssClass="form-control" 
                                       placeholder="請輸入職稱" MaxLength="50" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtContactPhone" runat="server" CssClass="form-control" 
                                       placeholder="請輸入手機號碼" MaxLength="20" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>

    <div class="block">
        <h5 class="square-title">計畫資訊</h5>
        <div class="mt-4">
            <table class="table align-middle gray-table side-table">
                <tbody>
                    <tr>
                        <th>計畫執行期間 <span class="text-danger view-mode">*</span></th>
                        <td>
                            <div class="d-flex align-items-center gap-2">
                                <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control taiwan-date-picker" 
                                           placeholder="請選擇開始日期" style="width: 220px;" readonly />
                                <span>至</span>
                                <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control taiwan-date-picker" 
                                           placeholder="請選擇結束日期" style="width: 220px;" readonly />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th>目的 <span class="text-danger view-mode ">*</span></th>
                        <td>
                            <asp:TextBox ID="txtPurpose" runat="server" CssClass="form-control textarea-auto-resize" 
                                       TextMode="MultiLine" Rows="4" placeholder="請描述計畫目的" 
                                       MaxLength="1000" style="width: 100%;" />
                        </td>
                    </tr>
                    <tr>
                        <th>計畫內容 <span class="text-danger view-mode">*</span></th>
                        <td>
                            <asp:TextBox ID="txtPlanContent" runat="server" CssClass="form-control textarea-auto-resize" 
                                       TextMode="MultiLine" Rows="6" placeholder="請詳細描述計畫內容" 
                                       MaxLength="2000" style="width: 100%;" />
                        </td>
                    </tr>
                    <tr>
                        <th>預期效益 <span class="text-danger view-mode ">*</span></th>
                        <td>
                            <asp:TextBox ID="txtPreBenefits" runat="server" CssClass="form-control textarea-auto-resize" 
                                       TextMode="MultiLine" Rows="4" placeholder="請描述預期效益" 
                                       MaxLength="1000" style="width: 100%;" />
                        </td>
                    </tr>
                    <tr class="public-view">
                        <th>計畫地點</th>
                        <td>
                            <asp:TextBox ID="txtPlanLocation" runat="server" CssClass="form-control" 
                                       placeholder="請輸入計畫執行地點" MaxLength="200" style="width: 100%;" />
                        </td>
                    </tr>
                    <tr>
                        <th>參加對象及預估人數</th>
                        <td>
                            <asp:TextBox ID="txtEstimatedPeople" runat="server" CssClass="form-control" 
                                       placeholder="請輸入預估參與人數" MaxLength="200" style="width: 100%;" />
                        </td>
                    </tr>
                    <tr class="public-view">
                        <th>相關緊急應變計畫</th>
                        <td>
                            <asp:TextBox ID="txtEmergencyPlan" runat="server" CssClass="form-control textarea-auto-resize" 
                                       TextMode="MultiLine" Rows="4" placeholder="請描述緊急應變計畫" 
                                       MaxLength="1000" style="width: 100%;" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    <div class="block">
        <h5 class="square-title">經費資訊</h5>
        <div class="mt-4">
            <div class="table-responsive mt-3 mb-0">
                <table class="table align-middle gray-table">
                    <thead>
                    <tr>
                        <th class="text-end">申請海委會補助／合作金額(元) (A)</th>
                        <th class="text-end">申請單位自籌款(元) (B)</th>
                        <th class="text-end">其他機關補助／合作總金額(元) (C)</th>
                        <th class="text-end">計畫總經費(元) (A+B+C)</th>
                    </tr>
                    </thead>
                    <tbody>
                    <tr>
                        <td>
                            <input id="txtApplyAmount" class="form-control" placeholder="請輸入" type="text" style="text-align: right;">
                        </td>
                        <td>
                            <input id="txtSelfAmount" class="form-control" placeholder="請輸入" type="text" style="text-align: right;">
                        </td>
                        <td id="lblOtherAmount" class="text-end">0</td>
                        <td id="lblTotalAmount" class="text-end">0</td>
                    </tr>
                    </tbody>
                </table>
            </div>
            <div class="table-responsive">
                <table class="table align-middle gray-table">
                    <thead>
                    <tr>
                        <th colspan="6" style="border-bottom-width: 1px;">其他機關補助／合作金額 (C)</th>
                    </tr>
                    <tr>
                        <th width="1"></th>
                        <th>單位名稱</th>
                        <th class="text-end">申請／分攤補助金額(元)（含尚未核定者）</th>
                        <th>比例</th>
                        <th>申請合作項目</th>
                        <th width="1">功能</th>
                    </tr>
                    </thead>
                    <tbody id="tbodyOtherSubsidy">
                    </tbody>
                </table><button id="btnAddOtherSubsidy" class="btn btn-sm btn-teal-dark m-0" type="button"><i class="fas fa-plus"></i> 新增</button>
            </div>
        </div>
        <h5 class="square-title">經費預算規劃</h5>

        <div class="table-responsive mt-3 mb-0">
            <table class="table align-middle gray-table">
                <thead>
                <tr>
                    <th>預算項目</th>
                    <th class="text-end">預算金額(元)<br>海洋委員會經費</th>
                    <th class="text-end">預算金額(元)<br>其他配合經費</th>
                    <th class="text-end">預算金額(元)<br>小計</th>
                    <th>計算方式及說明</th>
                    <th width="1">功能</th>
                </tr>
                </thead>
                <tbody id="tbodyBudgetPlan">
                </tbody>
            </table>
            <button id="btnAddBudgetPlan" class="btn btn-sm btn-teal-dark m-0" type="button"><i class="fas fa-plus"></i> 新增</button>
        </div>
        <table class="table align-middle gray-table side-table">
                        <tbody>
                            
                            <tr>
                                <th>最近兩年曾獲本會補助</th>
                                <td>
                                    <div class="form-check-input-group d-flex">
                                        <input id="rbPreviouslySubsidizedYes" type="radio" name="rbPreviouslySubsidized" value="true" class="form-check-input check-teal"><label for="rbPreviouslySubsidizedYes">是</label>
                                        <input id="rbPreviouslySubsidizedNo" type="radio" name="rbPreviouslySubsidized" value="false" checked="checked" class="form-check-input check-teal"><label for="rbPreviouslySubsidizedNo">否</label>
                                    </div>
                                </td>
                            </tr>
                            <tr class="funding-description-row d-none">
                                <th>經費說明</th>
                                <td>
                                    <table class="table align-middle gray-table">
                                        <thead>
                                        <tr>
                                            <th>計畫名稱</th>
                                            <th class="text-end">海委會補助經費(元)</th>
                                            <th width="1">功能</th>
                                        </tr>
                                        </thead>
                                        <tbody id="tbodyFundingDescription">
                                        </tbody>
                                    </table>
                                    <button id="btnAddFundingDescription" class="btn btn-sm btn-teal-dark m-0" type="button"><i class="fas fa-plus"></i> 新增</button>
                                </td>
                            </tr>
                        </tbody>        
                    </table>
    </div>
   
</div>


   
<!-- 申請表區塊結束 -->

<!-- 上傳附件區塊 -->
<div id="uploadAttachmentSection" style="display: none;">
    <div class="block">
        <h5 class="square-title">請下載範本填寫用印並上傳</h5>
        <p class="text-pink lh-base mt-3">
            請下載附件範本，填寫資料及公文用印後上傳（僅支援PDF格式上傳，每個檔案10MB以內）<br>
            請以紙本公文發文郵寄本會憑辦，附件需含資料表或申請書(正本；須加蓋學校關防及負責人核章)、<br>
            計畫書、未違反公職人員利益衝突迴避法切結書及事前揭露表(正本；須加蓋學校、單位關防及負責人核章)，<br>
            及相關佐證資料(如社團成立證明、社團運作證明等)
        </p>
        <div class="table-responsive mt-3 mb-0">
            <table id="FileTable" class="table align-middle gray-table">
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
                            <div>
                                <span class="text-pink view-mode">*</span>
                                申請表
                            </div>
                               <button type="button" class="btn btn-sm btn-teal-dark rounded-pill mt-2 view-mode" 
                                      onclick="downloadTemplate('1')">
                                  <i class="fas fa-file-download me-1"></i> 範本下載
                              </button>
                        </td>
                        <td class="text-center">
                            <asp:Label ID="lblStatusCLB1" runat="server" Text="未上傳" CssClass="text-muted"></asp:Label>
                        </td>
                        <td>
                            <input type="file" id="fileInput_FILE_CLB1" accept=".pdf" style="display: none;" onchange="handleFileUpload('FILE_CLB1', this)" />
                            <button class="btn btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput_FILE_CLB1').click()">
                                <i class="fas fa-file-upload me-1"></i> 上傳
                            </button>
                            <asp:Panel ID="pnlFilesCLB1" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td class="text-center">2</td>
                        <td>
                            <div>
                                <span class="text-pink view-mode">*</span>
                                計畫書
                            </div>
                            <button type="button" class="btn btn-sm btn-teal-dark rounded-pill mt-2 view-mode" 
                                 onclick="downloadTemplate('2')">
                             <i class="fas fa-file-download me-1"></i> 範本下載
                            </button>
                        </td>
                        <td class="text-center">
                            <asp:Label ID="lblStatusCLB2" runat="server" Text="未上傳" CssClass="text-muted"></asp:Label>
                        </td>
                        <td>
                            <input type="file" id="fileInput_FILE_CLB2" accept=".pdf" style="display: none;" onchange="handleFileUpload('FILE_CLB2', this)" />
                            <button class="btn btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput_FILE_CLB2').click()">
                                <i class="fas fa-file-upload me-1"></i> 上傳
                            </button>
                            <asp:Panel ID="pnlFilesCLB2" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td class="text-center">3</td>
                        <td>
                            <div>
                                <span class="text-pink view-mode">*</span>
                                未違反公職人員利益衝突迴避法切結書及事前揭露表
                            </div>
                            <button type="button" class="btn btn-sm btn-teal-dark rounded-pill mt-2 view-mode" 
                                  onclick="downloadTemplate('3')">
                              <i class="fas fa-file-download me-1"></i> 範本下載
                          </button>
                        </td>
                        <td class="text-center">
                            <asp:Label ID="lblStatusCLB3" runat="server" Text="未上傳" CssClass="text-muted"></asp:Label>
                        </td>
                        <td>
                            <input type="file" id="fileInput_FILE_CLB3" accept=".pdf" style="display: none;" onchange="handleFileUpload('FILE_CLB3', this)" />
                            <button class="btn btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput_FILE_CLB3').click()">
                                <i class="fas fa-file-upload me-1"></i> 上傳
                            </button>
                            <asp:Panel ID="pnlFilesCLB3" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td class="text-center">4</td>
                        <td>
                            <div>
                                相關佐證資料
                            </div>
                            <div class="text-muted" style="font-size: 16px;">
                                (如:社團成立證明、社團運作證明等)
                            </div>
                        </td>
                        <td class="text-center">
                            <asp:Label ID="lblStatusCLB4" runat="server" Text="未上傳" CssClass="text-muted"></asp:Label>
                        </td>
                        <td>
                            <input type="file" id="fileInput_FILE_CLB4" accept=".pdf" style="display: none;" onchange="handleFileUpload('FILE_CLB4', this)" />
                            <button class="btn btn-teal-dark view-mode" type="button" onclick="document.getElementById('fileInput_FILE_CLB4').click()">
                                <i class="fas fa-file-upload me-1"></i> 上傳
                            </button>
                            <asp:Panel ID="pnlFilesCLB4" runat="server" CssClass="tag-group mt-2 gap-1" Visible="false">
                            </asp:Panel>
                        </td>
                    </tr>
                </tbody>
            </table>
            <div id="divApplyTime" runat="server" class="text-center text-neutral-700 lh-base mt-4" visible="false">
                申請送件時間 : <asp:Label ID="lblApplyTime" runat="server" />
            </div>
        </div>
    </div>

    <!-- 隱藏欄位用於資料交換 -->
    <asp:HiddenField ID="hdnAttachmentData" runat="server" />
</div>


<!-- 變更說明區塊 -->
<div id="changeDescriptionSection" runat="server" class="mt-4">
  
     <div class="block">
                <h5 class="square-title mt-4">變更說明</h5>
                <div class="text-pink fw-normal fs-16 mt-2">本頁若有資料變更，請務必詳細說明「變更欄位」及「變更前／變更後」之資料內容。若有多項欄位請條列式(1,2,3,...)說明。</div>
                <div class="text-pink fw-normal fs-16 mt-2">本頁若無任何修改，請填寫「無」</div>
    
                <div class="mt-4">
                    
                    <table class="table align-middle gray-table side-table mt-3">
                        <tbody>
                            <tr>
                                <th width="120">
                                    <span class="text-pink">*</span>
                                    變更前
                                </th>
                                <td>
                                    <asp:TextBox ID="txtChangeBefore" runat="server" CssClass="form-control textarea-auto-resize"
                                               TextMode="MultiLine" Rows="4"
                                               placeholder="請輸入變更前的內容"  />
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <span class="text-pink">*</span>
                                    變更後
                                </th>
                                <td>
                                    <asp:TextBox ID="txtChangeAfter" runat="server" CssClass="form-control textarea-auto-resize"
                                               TextMode="MultiLine" Rows="4"
                                               placeholder="請輸入變更後的內容"  />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
        </div>
</div>

<!-- 上傳附件區塊結束 -->

<script type="text/javascript">
    function downloadTemplate(templateType) {
        // 取得當前的 ProjectID
        var projectID = document.getElementById('<%= hidProjectID.ClientID %>').value || '';
        
        // 取得網站根路徑
        var baseUrl = '<%= ResolveUrl("~/") %>';
        
        // 構建下載 URL
        var downloadUrl = baseUrl + 'Service/CLB_download.ashx?action=template&type=' + templateType;
        if (projectID) {
            downloadUrl += '&projectID=' + encodeURIComponent(projectID);
        }
        
        // 開啟下載
        window.open(downloadUrl, '_blank');
    }
</script>

<!-- 底部區塊 -->
<div class="block-bottom bg-light-teal">
    <button type="button" id="btnTempSave"
            class="btn btn-outline-teal view-mode"
            onclick="handleTempSave()">
        暫存
    </button>

    <button type="button" id="btnSaveAndNext"
            class="btn btn-teal"
            onclick="handleSaveAndNext()">
        完成本頁，下一步
    </button>

    <button type="button" id="btnSubmitApplication"
            class="btn btn-teal"
            onclick="handleSubmitApplication()">
        完成本頁，提送申請
    </button>
</div>

