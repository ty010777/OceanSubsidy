<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SciApplicationControl.ascx.cs" Inherits="OFS_SCI_UserControls_SciApplicationControl" %>
<%@ Register TagPrefix="uc" TagName="ChangeDescriptionControl" Src="~/OFS/SCI/UserControls/ChangeDescriptionControl.ascx" %>

<div class="block">

    <h5 class="square-title">申請基本資料</h5>
    <div class="mt-4">
        <table class="table align-middle gray-table side-table">
            <tbody>
                <tr>
                    <th>
                        年度
                    </th>
                    <td>
                        <asp:Label ID="txtYear" runat="server" Text=""  /> 
                    </td>
                </tr>
                <tr>
                    <th style="display: none">
                        延續性計畫
                    </th>
                    <td style="display: none">
                        
                        <div class="d-flex align-items-center gap-2">
                            <div class="input-group" style="width: 350px;">
                                <input ID="txtContinuousProject" type="text" class="form-control" placeholder="請輸入前期計畫編號">
                                <button class="btn btn-blue-green2" type="button">帶入資料</button>
                            </div>
                            <button type="button" 
                                class="btn-tooltip" 
                                data-bs-toggle="tooltip" 
                                data-bs-placement="top"
                                data-bs-title="若為延續性計畫，請輸入去年已核定計畫的計畫編號，系統將自動帶入計畫資料，以供修改及再次申請新年度補助">
                                <i class="fas fa-question-circle"></i>
                            </button>     
                        </div>
                    </td>
                </tr>
                <tr>
                    <th>
                        計畫編號
                    </th>
                    <td>
                        <span class="<%= String.IsNullOrEmpty(txtProjectID.Text) ? "text-light-gray" : "" %>">
                            <%= String.IsNullOrEmpty(txtProjectID.Text) ? "儲存後由系統自動產生" : txtProjectID.Text %>
                        </span>
                        <!-- 隱藏欄位 -->
                        <asp:Label ID="txtProjectID" runat="server" style="display:none;" />
                        <asp:Label ID="txtPersonID" runat="server" style="display:none;" />
                        <asp:Label ID="txtKeywordID" runat="server" style="display:none;" />
                    </td>
                </tr>
                <tr>
                    <th>
                        補助計畫類別
                    </th>
                    <td>
                        <asp:Label ID="txtSubsidyPlanType" runat="server" />
                    </td>
                </tr>
                <tr>
                    <th>
                        <span class="text-pink view-mode">*</span>
                        計畫名稱
                    </th>
                    <td>
                        <div class="input-group">
                            <span class="input-group-text" style="width: 70px;">
                                <span class="text-pink view-mode">*</span>
                                中文
                            </span>
                            <asp:TextBox ID="txtProjectNameCh"  CssClass="form-control" runat="server" /> 
                        </div>
                        <div class="input-group mt-2">
                            <span class="input-group-text" style="width: 70px;">
                                英文
                            </span>
                            <asp:TextBox ID="txtProjectNameEn" CssClass="form-control" runat="server"  />
                        </div>
                    </td>
                </tr>
                <tr>
                    <th>
                        <span class="text-pink view-mode">*</span>
                        申請屬性
                    </th>
                    <td>
                        <asp:DropDownList CssClass="form-select" ID="ddlApplicationType" runat="server"  />
                    </td>
                </tr>
                <tr>
                    <th>
                        <span class="text-pink view-mode">*</span>
                        計畫主題
                    </th>
                    <td>
                        <div class="row g-2">
                            <div class="col-12 col-md-6">
                                <div class="input-group">
                                    <span class="input-group-text">計畫主題</span>
                                    <asp:DropDownList ID="ddlTopic"  runat="server" CssClass="form-select" />
                                </div>
                            </div>
                            <div class="col-12 col-md-6" style="display:none;">
                                <div class="input-group">
                                    <span class="input-group-text">領域</span>
                                     <asp:DropDownList ID="ddlField" runat="server" CssClass="form-select"/>
                                </div>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr style="display:none;">
                    <th>
                        <span class="text-pink view-mode">*</span>
                        是否屬於國家核心科技
                    </th>
                    <td>
                        <div class="row g-4">
                            <div class="col-12 col-md-3">
                                <div class="text-gray">水下研究</div>
                                <div class="form-check-input-group mt-3 d-flex">
                                    <input id="rbUnderwaterYes" runat="server" class="form-check-input blue-green-check" type="radio" name="Underwater"  />
                                    <label for="rbUnderwaterYes">是</label>
                                    <input id="rbUnderwaterNo" runat="server" class="form-check-input blue-green-check" type="radio" name="Underwater"/>
                                    <label for="rbUnderwaterNo">否</label>
                                </div>
                            </div>
                            <div class="col-12 col-md-3">
                                <div class="text-gray">海洋地質</div>
                                <div class="form-check-input-group mt-3 d-flex">
                                    <input id="rbMarineYes" runat="server" class="form-check-input blue-green-check" type="radio" name="Marine" />
                                    <label for="rbMarineYes">是</label>
                                    <input id="rbMarineNo" runat="server" class="form-check-input blue-green-check" type="radio" name="Marine"/>
                                    <label for="rbMarineNo">否</label>
                                </div>
                            </div>
                            <div class="col-12 col-md-3">
                                <div class="text-gray">海洋物理</div>
                                <div class="form-check-input-group mt-3 d-flex">
                                    <input id="rbPhysicsYes" runat="server" class="form-check-input blue-green-check" type="radio" name="Physics" />
                                    <label for="rbPhysicsYes">是</label>
                                    <input id="rbPhysicsNo" runat="server" class="form-check-input blue-green-check" type="radio" name="Physics"/>
                                    <label for="rbPhysicsNo">否</label>
                                </div>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <th>
                        <span class="text-pink view-mode">*</span>
                        計畫內容是否涉及國家核心海洋資料項目
                    </th>
                    <td>
                        <div class="form-check mb-2">
                            <input id="rbCoreOceanDataYes" runat="server" class="form-check-input blue-green-check" type="radio" name="CoreOceanData" />
                            <label for="rbCoreOceanDataYes" class="form-check-label">
                                是，涉及國家核心海洋資料項目，包含1. 水下研究：我國禁限制水域內水下聲學研究之實海域聲場環境參數資料、2. 海洋地質：台灣本島領海內利用多音束聲納收集後未經船隻姿態、潮位及聲速等修正處理之原始水深資料，及其經修正處理後解析度200公尺以內之數位網格水深資料，外島地區則以禁限制水域為界、3. 海洋物理：我國禁限制水域內原始水文資料管制3年。應確實依「政府資助國家核心科技研究計畫安全管制作業手冊」及相關規定辦理。
                            </label>
                        </div>
                        <div class="form-check">
                            <input id="rbCoreOceanDataNo" runat="server" class="form-check-input blue-green-check" type="radio" name="CoreOceanData" />
                            <label for="rbCoreOceanDataNo" class="form-check-label">否</label>
                        </div>
                    </td>
                </tr>
                <tr>
                    <th>
                        <span class="text-pink view-mode">*</span>
                        申請單位<br>(含系所名稱)
                    </th>
                    <td>
                        <asp:TextBox ID="txtOrgName" runat="server" CssClass="form-control" placeholder="請輸入名稱" />
                    </td>
                </tr>
                <tr>
                    <th>
                        <span class="text-pink view-mode">*</span>
                        登記地址
                    </th>
                    <td>
                          <asp:TextBox ID="txtRegisteredAddress" runat="server" CssClass="form-control" placeholder="請輸入單位登記地址" />                                      
                    </td>
                </tr>
                <tr>
                    <th>
                        <span class="text-pink view-mode">*</span>
                        通訊地址<br>
                        (公文寄送地址)
                    </th>
                    <td>
                    <asp:TextBox ID="txtCorrespondenceAddress" runat="server" CssClass="form-control" placeholder="請輸入通訊地址（公文郵送地址）" />
                        </td>
                </tr>
            </tbody>
        </table>
    </div>
    
    
    <h5 class="square-title mt-5">人員聯絡資訊</h5>
    
    <div class="mt-4">
        <table class="table align-middle gray-table side-table">
            <tbody>
                <tr>
                    <th>
                        <span class="text-pink view-mode">*</span>
                        計畫主持人
                    </th>
                    <td style="display:none;"><asp:Label ID="txtPIIdx" runat="server" Text="0" /></td>

                    <td>
                        <div class="row g-3">
                            <div class="col-12 col-xl-2">
                                <div class="mb-2">
                                    <span class="text-pink view-mode">*</span>
                                    姓名
                                </div>
                                <asp:TextBox ID="txtPIName" runat="server" CssClass="form-control" placeholder="請輸入姓名" />

                            </div>
                            <div class="col-12 col-xl-3">
                                <div class="mb-2">
                                    <span class="text-pink view-mode">*</span>
                                    職稱
                                </div>
                                <asp:TextBox ID="txtPIJobTitle" runat="server" CssClass="form-control" placeholder="請輸入職稱"/>
                            </div>
                            <div class="col-12 col-xl-4">
                                <div class="mb-2">
                                    電話(分機)
                                </div>
                                <div class="d-flex gap-2">
                                    <asp:TextBox ID="txtPIPhone" runat="server" CssClass="form-control" placeholder="請輸入電話"/>
                                    <asp:TextBox ID="txtPIPhoneExt" runat="server" CssClass="form-control" placeholder="請輸入分機"/>
                                </div>
                                
                            </div>
                            <div class="col-12 col-xl-3">
                                <div class="mb-2">
                                    <span class="text-pink view-mode">*</span>
                                    手機號碼
                                </div>
                                <asp:TextBox ID="txtPIMobile" runat="server" CssClass="form-control" placeholder="請輸入手機號碼"/>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <th>
                        <span class="text-pink view-mode">*</span>
                        計畫聯絡人
                    </th>
                    <td style="display:none;"><asp:Label ID="txtContactIdx" runat="server" Text="0" /></td>
                    <td>
                        <div class="row g-3">
                            <div class="col-12 col-xl-2">
                                <div class="mb-2">
                                    <span class="text-pink view-mode">*</span>
                                    姓名
                                </div>
                                <asp:TextBox ID="txtContactName" runat="server" CssClass="form-control" placeholder="請輸入姓名"/>
                            </div>
                            <div class="col-12 col-xl-3">
                                <div class="mb-2">
                                    <span class="text-pink view-mode">*</span>
                                    職稱
                                </div>
                                <asp:TextBox ID="txtContactJobTitle" runat="server" CssClass="form-control" placeholder="請輸入職稱"/>
                            </div>
                            <div class="col-12 col-xl-4">
                                <div class="mb-2">
                                    電話(分機)
                                </div>
                                <div class="d-flex gap-2">
                                    <asp:TextBox ID="txtContactPhone" runat="server" CssClass="form-control" placeholder="請輸入電話"/>
                                    <asp:TextBox ID="txtContactPhoneExt" runat="server" CssClass="form-control" placeholder="請輸入分機"/>
                                </div>
                                
                            </div>
                            <div class="col-12 col-xl-3">
                                <div class="mb-2">
                                    <span class="text-pink view-mode">*</span>
                                    手機號碼
                                </div>
                                <asp:TextBox ID="txtContactMobile" runat="server" CssClass="form-control" placeholder="請輸入手機號碼"/>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <th>
                        會計聯絡人
                    </th>
                     <td style="display:none;"><asp:Label ID="txtAccountIdx" runat="server" Text="0" /></td> 
                    <td>
                        <div class="row g-3">
                            <div class="col-12 col-xl-2">
                                <div class="mb-2">
                                    姓名
                                </div>
                                <asp:TextBox ID="txtAccountName" runat="server" CssClass="form-control" placeholder="請輸入姓名"/>
                            </div>
                            <div class="col-12 col-xl-3">
                                <div class="mb-2">
                                    職稱
                                </div>
                                <asp:TextBox ID="txtAccountJobTitle" runat="server" CssClass="form-control" placeholder="請輸入職稱"/>
                            </div>
                            <div class="col-12 col-xl-4">
                                <div class="mb-2">
                                    電話(分機)
                                </div>
                                <div class="d-flex gap-2">
                                    <asp:TextBox ID="txtAccountPhone" runat="server" CssClass="form-control" placeholder="請輸入電話"/>
                                    <asp:TextBox ID="txtAccountPhoneExt" runat="server" CssClass="form-control" placeholder="請輸入分機"/>
                                </div>
                                
                            </div>
                            <div class="col-12 col-xl-3">
                                <div class="mb-2">
                                    手機號碼
                                </div>
                                <asp:TextBox ID="txtAccountMobile" runat="server" CssClass="form-control" placeholder="請輸入手機號碼"/>
                            </div>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    
    
    <h5 class="square-title mt-5">計畫內容與關鍵字</h5>
    
    
    <div class="mt-4">
        <table class="table align-middle gray-table side-table">
            <tbody>
                <tr>
                    <th>
                        <span class="text-pink view-mode">*</span>
                        計畫目標
                    </th>
                    <td>
                        <asp:TextBox  ID="txtTarget" runat="server"  CssClass="form-control textarea textarea-auto-resize " 
                            TextMode="MultiLine"  
                            Rows="6"  
                            placeholder="請輸入計畫目標"
                            data-max-length="500" />
                        <div class="fs-14 text-gray mt-2 view-mode"> <span class="text-pink char-count ">0</span> </div>
                    </td>
                </tr>
                <tr>
                    <th>
                        <span class="text-pink view-mode">*</span>
                        計畫內容摘要
                    </th>
                    <td>
                        <asp:TextBox ID="txtSummary" runat="server"  
                            TextMode="MultiLine"  
                            Rows="6"  
                            CssClass="form-control textarea textarea-auto-resize"  
                            placeholder="請輸入計畫內容摘要，說明主要研究內容與方法..."  
                            data-max-length="500" />
                        <div class="fs-14 text-gray mt-2 view-mode"><span class="text-pink char-count">0</span> </div>
                    </td>
                </tr>
                <tr>
                    <th>
                        <span class="text-pink view-mode">*</span>
                        計畫創新重點
                    </th>
                    <td>
                        
                         <asp:TextBox ID="txtInnovation" runat="server" 
                                    TextMode="MultiLine"  
                                    Rows="5"  
                                    CssClass="form-control textarea textarea-auto-resize"  
                                    placeholder="請輸入計畫的創新特色與技術突破點..."  
                                    data-max-length="250" /> 
                         <div class="fs-14 text-gray mt-2 view-mode"> <span class="text-pink char-count">0</span> </div>
                    </td>
                </tr>
                <tr>
                    <th>
                        <span class="text-pink view-mode">*</span>
                        關鍵字<br>(至少3組)
                    </th>
                    <td>
                        <div id="keywordsContainer" class="d-flex flex-column gap-3">
                            <!-- 動態關鍵字容器 -->
                        </div>
                        <button type="button" id="btnAddKeyword" class="btn btn-blue-green2 mt-3 ms-auto  view-mode">新增</button>
                        <!-- 隱藏欄位儲存關鍵字資料 -->
                        <asp:HiddenField ID="hiddenKeywordsData" runat="server" />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <div class="view-mode">
        <h5 class="square-title mt-5 " >聲明書</h5>
        <div class="agreement-panel">

            <div class="agreement-content" id="agreementContent">
                <div class="mb-3">一、同意事項</div>
                <ul class="mb-4">
                    <li class="mb-2">
                        （一）申請人同意，提出之計畫若非屬海委會業務職掌範圍，或非屬海洋科技產業技術發展所需之前瞻、關鍵、整合、共通或基礎性技術時，海委會得退件或建議申請其他政府補助計畫。
                    </li>
                    <li class="mb-2">
                        （二）申請人同意由專案辦公室轉請審查會議審查本公司提出之計畫書。
                    </li>
                    <li class="mb-2">
                        （三）申請人有義務回答各階段審查意見。
                    </li>
                    <li class="mb-2">
                        （四）申請人已瞭解倘所提出之計畫涉及國家核心海洋資料項目，則應確實依「政府資助國家核心科技研究計畫安全管制作業手冊」及相關規定辦理。
                    </li>
                </ul>
        
                <div class="mb-3">二、承諾事項</div>
                <p class="mb-3">本公司保證無下列情況發生，否則願負一切責任：</p>
                <ul class="mb-4">
                    <li class="mb-2">
                        （一）申請人保證本計畫僅適用於將進行之研發計畫，若為進行中或已開發完成者，均不得申請，且不得以相同或類似計畫重複申請政府其他計畫補助。
                    </li>
                    <li class="mb-2">
                        （二）申請人保證本計畫未依其他法令享有租稅優惠、獎勵或補助。

                    </li>
                    <li class="mb-2">
                        （三）申請人保證未有政府採購法第101條第1項規定之拒絕往來情形。
                    </li>
                    <li class="mb-2">
                        （四）申請人保證於申請日前5年內未曾有執行政府計畫之重大違約紀錄，亦無遭受停權處分而其期間尚未屆滿之情形。
                    </li>
                    <li class="mb-2">
                        （五）申請人保證於申請日前3年內，無嚴重違反環境保護、勞工或食品安全衛生之相關法律或違反身心障礙者權益保障法相關規定且情節重大之情形。
                    </li>
                    <li class="mb-2">
                        （六）申請人保證於申請日前3年內無欠繳應納稅捐情事。
                    </li>
                    <li class="mb-2">
                        （七）申請人保證代表人、計畫主持人及計畫相關參與人未具有大陸地區人民來臺投資許可辦法第3條所稱之投資人身分。
                        <small>（「大陸地區人民來臺投資許可辦法」第3條所稱投資人，指大陸地區人民、法人、團體、其他機構或其於第3地區投資之公司，依規定在臺灣地區從事投資行為者。）</small>
                    </li>
                    <li class="mb-2">
                        （八）申請人保證本計畫所列研發人員為申請人正式員工，絕無虛報投入人力之情事，且列報本計畫之研發人員薪資及其他各項費用符合本計畫會計科目編列與執行原則。
                    </li>
                    <li class="mb-2">
                        （九）申請人保證自投件申請日起，不得就申請行為、補助計畫、補助金額與之其他商業行為作不當連結、進行不當宣傳或為其他使人受誤導或混淆之行為。
                    </li>
                    <li class="mb-2">
                        （十）申請人保證上列資料及附件均屬正確，所提供之各項申請應備文件，均與事實相符，並保證不侵害他人之專利權、專門技術及著作權等相關智慧財產權，如有不實願負一切責任，海委會得不予受理或依職權撤銷補助並解除契約。
                    </li>
                    <li class="mb-2">
                        （十一）申請人保證知悉違反法令不得申請本案之補助，並追回違法期間內申請所獲得之補助。
                    </li>
                    
                </ul>
                
        
                <div class="mb-3">三、</div>
                <p class="mb-0">
                    申請人拒絕為前述之聲明，本會得不受理其申請案；其聲明不實經發現者，本會得不予受理，或撤銷補助、解除契約，並追回已撥付之補助款。
                    （送件時以本表申請免備文，務請於下列申請人(事業機構)印鑑處及代表人簽章處用印並填註送件日期。）
                </p>
        
            </div>
            <div class="agreement-consent p-3">
                <div class="hstack justify-content-center">
                    <div class="form-check d-flex align-items-center gap-1">
                        <input id="ChkAgreeTerms" runat="server" class="form-check-input blue-green-check" type="checkbox" name="type2" />
                          <label for="agreeTerms">我已了解並同意</label>
                    </div>  
                </div>
            </div>
        </div>
    </div>
    <!-- 變更說明 UserControl -->
    <uc:ChangeDescriptionControl ID="tab1_ucChangeDescription" runat="server" SourcePage="SciApplication" />

     <!-- 底部區塊 -->
       <div class="block-bottom bg-light-teal view-mode">

            <asp:Button ID="tab1_btnTempSave" runat="server"
                      Text="暫存"
                      CssClass="btn btn-outline-teal"
                      OnClientClick="if (typeof syncAllChangeDescriptions === 'function') { syncAllChangeDescriptions(); } return true;"
                      OnClick="btnSave_Click" />

            <asp:Button ID="tab1_btnSubmit" runat="server"
                      Text="完成本頁，下一步"
                      CssClass="btn btn-teal"
                      OnClientClick="if (typeof syncAllChangeDescriptions === 'function') { syncAllChangeDescriptions(); } return true;"
                      OnClick="btnSave_Click" />
       </div>
</div>

