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
                            <asp:Label ID="lblYear" runat="server" Text="114" /> 
                            <asp:HiddenField ID="hidYear" runat="server" Value="114" />
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
                            <asp:Label ID="lblSubsidyPlanType" runat="server" Text="學校社團（海洋委員會鼓勵各級學校社團辦理海洋活動補助原則）" />
                            <asp:HiddenField ID="hidSubsidyPlanType" runat="server" Value="學校社團" />
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
            <table class="table align-middle gray-table side-table">
                <tbody>
                    <tr>
                        <th>經費明細 <span class="text-danger view-mode">*</span></th>
                        <td>
                            <div style="font-size: 18px;">
                                <!-- 第一行：申請海委會補助 + 自籌款 -->
                                <div class="d-flex align-items-center gap-5 mb-3">
                                    <div class="d-flex align-items-center">
                                        <label class="mb-0 fw-medium" style="white-space: nowrap; width: 140px;">申請海委會補助:</label>
                                        <asp:TextBox ID="txtSubsidyFunds" runat="server" CssClass="form-control text-end"
                                                   placeholder="0" style="width: 150px;" TextMode="Number" step="any"  min="0" onkeypress="return event.charCode != 45 && event.charCode != 46"/>
                                        <span class="ms-2">元</span>
                                    </div>
                                    <div class="d-flex align-items-center">
                                        <label class="mb-0" style="white-space: nowrap; width: 140px;">自籌款:</label>
                                        <asp:TextBox ID="txtSelfFunds" runat="server" CssClass="form-control text-end"
                                                   placeholder="0" style="width: 150px;" TextMode="Number" step="any"  min="0" onkeypress="return event.charCode != 45 && event.charCode != 46"/>
                                        <span class="ms-2">元</span>
                                    </div>
                                </div>

                                <!-- 第二行：其他政府補助 + 其他單位補助 -->
                                <div class="d-flex align-items-center gap-5 mb-3">
                                    <div class="d-flex align-items-center">
                                        <label class="mb-0" style="white-space: nowrap; width: 140px;">其他政府補助:</label>
                                        <asp:TextBox ID="txtOtherGovFunds" runat="server" CssClass="form-control text-end"
                                                   placeholder="0" style="width: 150px;" TextMode="Number" step="any"  min="0" onkeypress="return event.charCode != 45  && event.charCode != 46"/>
                                        <span class="ms-2">元</span>
                                    </div>
                                    <div class="d-flex align-items-center">
                                        <label class="mb-0" style="white-space: nowrap; width: 140px;">其他單位補助:</label>
                                        <asp:TextBox ID="txtOtherUnitFunds" runat="server" CssClass="form-control text-end"
                                                   placeholder="0" style="width: 150px;" TextMode="Number" step="any"  min="0" onkeypress="return event.charCode != 45  && event.charCode != 46"/>
                                        <span class="ms-2">元</span>
                                    </div>
                                </div>
                            </div>
                            <div class="d-flex align-items-center gap-2 pt-2 border-top">
                                <label class="form-label fw-bold mb-0">計畫總經費:</label>
                                <asp:Label ID="lblTotalFunds" runat="server" CssClass="fw-bold text-primary fs-5" Text="0" />
                                <span class="fw-medium">元</span>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th>最近兩年曾獲本會補助</th>
                        <td>
                            <div class="form-check-input-group d-flex">
                                <asp:RadioButton ID="rbPreviouslySubsidizedYes"  runat="server" GroupName="PreviouslySubsidized"
                                               Text="是" Value="true" ClientIDMode="Static" />
                                <asp:RadioButton ID="rbPreviouslySubsidizedNo" runat="server" GroupName="PreviouslySubsidized"
                                               Text="否" Value="false" Checked="true"  ClientIDMode="Static" />
                            </div>
                        </td>
                    </tr>
                    <tr class="funding-description-row d-none">
                        <th>經費說明</th>
                        <td>
                            <asp:TextBox ID="txtFundingDescription" runat="server" CssClass="form-control textarea-auto-resize"
                                       TextMode="MultiLine" Rows="4" placeholder="請說明經費用途規劃"
                                       MaxLength="1000" style="width: 100%;" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
   
</div>


   
<!-- 申請表區塊結束 -->

<!-- 上傳附件區塊 -->
<div id="uploadAttachmentSection" style="display: none;">
    <div class="block">
        <h5 class="square-title">上傳附件</h5>
        <p class="text-pink lh-base mt-3">
            請下載附件範本，填寫資料及公文用印後上傳（僅支援PDF格式上傳，每個檔案10MB以內）<br>
            申請計畫書請自行留存送審版電子檔，待審核結果公告後請提交修正計畫書以供核定。
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

