<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SciFundingControl.ascx.cs" Inherits="OFS_SCI_UserControls_SciFundingControl" %>
<%@ Register TagPrefix="uc" TagName="ChangeDescriptionControl" Src="~/OFS/SCI/UserControls/ChangeDescriptionControl.ascx" %>

<!-- Hidden Fields for dynamic data -->
<asp:HiddenField ID="hdnPersonnelData" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdnMaterialData" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdnTravelData" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdnOtherData" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdnOtherRentData" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdnTotalFeesData" runat="server" ClientIDMode="Static" />

<div class="anchor-wrapper">
    <!-- 錨點選單 -->
    <div class="anchor-menu">
        <a href="#point1" class="anchor-menu-item">1.海洋科技研發人員人事費明細表</a>
        <a href="#point2" class="anchor-menu-item">2.消耗性器材及原材料費</a>
        <a href="#point3" class="anchor-menu-item">3.技術移轉、委託研究或驗證費</a>
        <a href="#point4" class="anchor-menu-item">4.國內差旅費</a>
        <a href="#point5" class="anchor-menu-item">5.其他業務費</a>
        <a href="#point6" class="anchor-menu-item">【經費總表】</a>
    </div>
</div>

<!-- 內容區塊 -->
<div class="block">

    <div id="point1">
        <h5 class="square-title">1.海洋科技研發人員人事費明細表</h5>
        <div class="table-responsive mt-3 mb-0">
            <table class="table align-middle gray-table person">
                <thead>
                <tr>
                    <th width="150"><span class="text-pink">*</span>姓名</th>
                    <th class="text-center">待聘</th>
                    <th width="350">
                        <span class="text-pink">*</span>職稱
                        <button type="button" class="btn-tooltip" data-bs-toggle="modal" data-bs-target="#jobDetailModal">
                            <i class="fas fa-info-circle"></i>
                        </button>
                    </th>
                    <th class="text-end"><span class="text-pink">*</span>平均月薪</th>
                    <th class="text-end"><span class="text-pink">*</span>參與人月</th>
                    <th class="text-end"><span class="text-pink">*</span>人事費小計</th>
                    <th width="130">功能</th>
                </tr>
                </thead>
                <tbody>
                <tr>
                    <td>
                        <asp:TextBox ID="personName1" runat="server" CssClass="form-control" placeholder="請輸入姓名" />
                    </td>
                    <td class="text-center">
                        <input type="checkbox" class="form-check-input check-teal" runat="server" id="stay1"/>
                    </td>
                    <td>
                        <select id="ddlPerson1" class="form-select"></select>
                    </td>
                    <td>
                        <asp:TextBox ID="personSalary1" runat="server" ClientIDMode="Static" CssClass="form-control text-end" Text="0" onblur="checkSalaryLimit(1); calculateAndUpdateTotal()" />
                    </td>
                    <td>
                        <asp:TextBox ID="personMonths1" runat="server" ClientIDMode="Static" CssClass="form-control text-end" Text="0" onblur="calculateAndUpdateTotal()" />
                    </td>
                    <td class="text-end">0</td>
                    <td>
                        <button type="button" class="btn btn-sm btn-teal" onclick="P_addNewRow()">
                            <i class="fas fa-plus"></i>
                        </button>
                    </td>
                </tr>
                <tr class="total-row">
                    <td colspan="5">合計</td>
                    <td class="text-end" id="PersonTotal">0</td>
                    <td>
                        <button type="button" class="btn btn-sm btn-teal" onclick="calculateAndUpdateTotal()">計算</button>
                    </td>
                </tr>
                </tbody>
            </table>
        </div>

        <ul class="list-unstyled lh-base">
            <li>1.計畫所列海洋科技研發人員應具執行計畫所需能力、研究發展之能力與專案執行及研發成果管理能力。</li>
            <li>2.人事費不得超過計畫總經費50%。行政、會計、出納、美編及非實際參與研發工作人員等，均不得列為本計畫研發人員項目。</li>
            <li>3.應規劃提升海洋科技研發人員薪資機制或建置友善職場環境(如加薪規則、升遷管道、激勵措施及工作環境硬體設施等)。</li>
            <li>4.計畫所列海洋科技研發人員之實領薪資應高於或等於平均月薪。</li>
            <li>5.若有共同執行單位(企業)經費投入請編列至配合款，並於本表備註為共同執行單位之人員。</li>
            <li>6.所列人員須為申請人正式員工（具其勞保身份者），員工數不足5人(不含)以下，未具參加勞工保險投保資格者(已符合年資或退休)，須檢附證明文件(如職業災害保險)。</li>
        </ul>
    </div>

    <div id="point2">
        <h5 class="square-title mt-5">2.消耗性器材及原材料費</h5>
        <div class="table-responsive mt-3 mb-0">
            <table class="table align-middle gray-table Material">
                <thead>
                <tr>
                    <th>品名</th>
                    <th class="text-center">說明</th>
                    <th width="120">單位</th>
                    <th class="text-end">預估需求數量</th>
                    <th class="text-end">單價</th>
                    <th class="text-end">總價</th>
                    <th width="130">功能</th>
                </tr>
                </thead>
                <tbody>
                <tr>
                    <td>
                        <asp:TextBox ID="MaterialName1" runat="server" CssClass="form-control" placeholder="請輸入" />
                    </td>
                    <td class="text-center">
                        <asp:TextBox ID="MaterialDescription1" runat="server" CssClass="form-control" placeholder="請輸入" />
                    </td>
                    <td>
                        <select id="MaterialUnit1" class="form-select"></select>
                    </td>
                    <td>
                        <asp:TextBox ID="MaterialNum1" runat="server" ClientIDMode="Static" CssClass="form-control text-end" placeholder="請輸入" onblur="calculateMaterial()" />
                    </td>
                    <td>
                        <asp:TextBox ID="MaterialUnitPrice1" runat="server" ClientIDMode="Static" CssClass="form-control text-end" placeholder="請輸入" onblur="checkMaterialLimit(1);calculateMaterial()" />
                    </td>
                    <td class="text-end">0</td>
                    <td>
                        <button type="button" class="btn btn-sm btn-teal" onclick="M_addNewRow()">
                            <i class="fas fa-plus"></i>
                        </button>
                    </td>
                </tr>
                <tr class="total-row">
                    <td colspan="5">合計</td>
                    <td class="text-end" id="MaterialTotal">0</td>
                    <td></td>
                </tr>
                </tbody>
            </table>
        </div>

        <ul class="list-unstyled lh-base">
            <li>1.材料費之編列範圍包括研發用途之消耗性器材及原材料費，但不含辦公所需事務性耗材。</li>
            <li>2.專為執行開發計畫所發生之消耗性器材及原材料費，但不含模具、冶具、夾具等屬固定資產之設備。</li>
            <li>3.本項經費支出之憑證、發票等，其品名之填寫應完整，並與計畫書上所列一致，勿填列代號或簡稱。</li>
            <li>4.消耗性器材及原材料費不得超過計畫總經費之25%。</li>
            <li>5.可認列之消耗性器材及原材料費其單據日期應在計畫執行期間內。</li>
            <li>6.所有購買物品應列明品名、數量及單價。</li>
            <li>7.應依<a href="#" class="link-teal">附件十海洋科技專案計畫會計科目編列與執行原則</a>之規定辦理。</li>
            <li>8.若有共同執行單位(企業)經費投入請編列至配合款，並於本表備註為共同執行單位之費用。</li>
            <li>9.本會計科目之編列不含營業稅。</li>
        </ul>
    </div>

    <div id="point3">
        <h5 class="square-title mt-5">3.技術移轉、委託研究或驗證費</h5>
        <div class="table-responsive mt-3 mb-0">
            <table class="table align-middle gray-table ResearchFees">
                <thead>
                <tr>
                    <th></th>
                    <th>期間</th>
                    <th class="text-end">委託項目名稱</th>
                    <th class="text-end">委託對象</th>
                    <th class="text-end">金額</th>
                </tr>
                </thead>
                <tbody>
                <tr>
                    <td class="text-end" width="200">
                        <span id="FeeCategory1" runat="server">技術移轉</span>
                    </td>
                    <td class="text-center">
                        <div class="input-group" style="width: 400px;">
                            <asp:TextBox ID="txtDate1Start" runat="server" ClientIDMode="Static" CssClass="form-control" TextMode="Date" />
                            <span class="input-group-text">至</span>
                            <asp:TextBox ID="txtDate1End" runat="server" ClientIDMode="Static" CssClass="form-control" TextMode="Date" />
                        </div>
                    </td>
                    <td>
                        <asp:TextBox ID="ResearchFeesName1" runat="server" ClientIDMode="Static" CssClass="form-control" placeholder="請輸入" />
                    </td>
                    <td>
                        <asp:TextBox ID="ResearchFeesPersonName1" runat="server" ClientIDMode="Static" CssClass="form-control" placeholder="請輸入" />
                    </td>
                    <td>
                        <asp:TextBox ID="ResearchFeesPrice1" runat="server" ClientIDMode="Static" CssClass="form-control text-end money" placeholder="請輸入" onblur="calculateResearch()" />
                    </td>
                </tr>
                <tr>
                    <td class="text-end" width="200">
                        <span id="FeeCategory2" runat="server">委託研究</span>
                    </td>
                    <td class="text-center">
                        <div class="input-group" style="width: 400px;">
                            <asp:TextBox ID="txtDate2Start" runat="server" ClientIDMode="Static" CssClass="form-control" TextMode="Date" />
                            <span class="input-group-text">至</span>
                            <asp:TextBox ID="txtDate2End" runat="server" ClientIDMode="Static" CssClass="form-control" TextMode="Date" />
                        </div>
                    </td>
                    <td>
                        <asp:TextBox ID="ResearchFeesName2" runat="server" ClientIDMode="Static" CssClass="form-control" placeholder="請輸入" />
                    </td>
                    <td>
                        <asp:TextBox ID="ResearchFeesPersonName2" runat="server" ClientIDMode="Static" CssClass="form-control" placeholder="請輸入" />
                    </td>
                    <td>
                        <asp:TextBox ID="ResearchFeesPrice2" runat="server" ClientIDMode="Static" CssClass="form-control text-end money" placeholder="請輸入" onblur="calculateResearch()" />
                    </td>
                </tr>
                <tr class="total-row">
                    <td colspan="4">合計</td>
                    <td class="text-end" id="ResearchFeesTotal">0</td>
                </tr>
                </tbody>
            </table>
        </div>

        <ul class="list-unstyled lh-base">
            <li>1.所稱技術移轉為經由技術合作、技術授權(商標、執照、權利金、軟體及資料庫)、技術指導(設計、相關技術援助、技術訓練、技術諮詢、技術研究)、智財授權等方式，以取得並移轉技術(智財)者。</li>
            <li>2.委託研究<br>- 委託研究費﹕委託外界機構、單位專案研究或研發所需之費用。與技術研發或研發服務直接相關零組件、次系統理論分析模擬設計研發、製造、測試(含認證)；專利檢索；軟體電腦程式原始碼授權等；藥理、毒性、動物及臨床試驗等。<br>- 驗證費﹕檢測分析及認證費用。</li>
            <li>3.本會計科目之編列包含技術或關鍵智財之移轉及委託研究費，若契約約定執行期間超出計畫核准執行期間，應核減非計畫期間所應分攤之費用；由技術提供者採授權方式移轉者，其授權期間超出計畫核准執行期間，應核算計畫執行期間所平均攤提之授權費用。</li>
            <li>4.須附上技術移轉及委託研究契約或報價單，並以委任之政府所屬機關（構）首長或學校、法人及民間單位之該項委託工作項目負責人簽署用印為佐證。</li>
            <li>5.為確保計畫研發自主性，技術移轉及委託研究兩項經費合計不得超過計畫總經費40%，超過該比率者，不予受理。</li>
            <li>6.委託者須為單位不得為個人，若技術移轉提供者為個人除外(需提供佐證資料)。</li>
            <li>7.應依<a href="#" class="link-teal">附件十海洋科技專案計畫會計科目編列與執行原則</a>之規定辦理。</li>
            <li>8.若有共同執行單位(企業)經費投入請編列至配合款，並於本表備註為共同執行單位之費用。</li>
            <li>9.本會計科目之編列不含營業稅。</li>
        </ul>
    </div>

    <div id="point4">
        <h5 class="square-title mt-5">4.國內差旅費</h5>
        <div class="table-responsive mt-3 mb-0">
            <table class="table align-middle gray-table travel" id="travelTable">
                <thead>
                <tr>
                    <th width="350">出差事由</th>
                    <th>地區</th>
                    <th width="120">天數</th>
                    <th class="text-center">人次</th>
                    <th class="text-end">金額</th>
                    <th width="150">功能</th>
                </tr>
                </thead>
                <tbody>
                <tr>
                    <td>
                        <asp:TextBox ID="travelReason1" runat="server" CssClass="form-control" />
                    </td>
                    <td class="text-center">
                        <asp:TextBox ID="travelArea1" runat="server" CssClass="form-control" />
                    </td>
                    <td>
                        <asp:TextBox ID="travelDays1" runat="server" CssClass="form-control days" Text="0" />
                    </td>
                    <td>
                        <asp:TextBox ID="travelPeople1" runat="server" CssClass="form-control people" Text="0" />
                    </td>
                    <td width="220">
                        <asp:TextBox ID="travelPrice1" runat="server" ClientIDMode="Static" CssClass="form-control text-end price" Text="0" onblur="calculateTravel()" />
                    </td>
                    <td>
                        <button type="button" class="btn btn-sm btn-teal" onclick="T_addRow()">
                            <i class="fas fa-plus"></i>
                        </button>
                    </td>
                </tr>
                <tr class="total-row">
                    <td colspan="4">合計</td>
                    <td class="text-end" id="travelTotal">0</td>
                    <td></td>
                </tr>
                </tbody>
            </table>
        </div>

        <ul class="list-unstyled lh-base">
            <li>1.限為海洋科技執行計畫需要，於計畫執行期間內，派遣本計畫研發人員之出差地點應為國內技術轉移對象、委外測試或驗證機構、委託研究對象之所在地。出差事由應與國內技術移轉、委外測試或驗證、委託研究及參與計畫補助單位認可之特定公務相關。</li>
            <li>2.應依<a href="#" class="link-teal">附件十海洋科技專案計畫會計科目編列與執行原則</a>之規定辦理。</li>
            <li>3.差旅費不得超過計畫總經費之1.5%。</li>
            <li>4.若有共同執行單位(企業)經費投入請編列至配合款，並於本表備註為共同執行單位之費用。</li>
            <li>5.本會計科目之編列不含營業稅。</li>
        </ul>
    </div>

    <div id="point5">
        <h5 class="square-title mt-5">5.其他業務費</h5>
        <div class="table-responsive mt-3 mb-0">
            <table class="table align-middle gray-table other" id="otherTable">
                <caption>勞務委託費</caption>
                <thead>
                <tr>
                    <th width="350">
                        <span class="text-pink">*</span>職稱
                        <button type="button" class="btn-tooltip" data-bs-toggle="modal" data-bs-target="#jobDetailModal">
                            <i class="fas fa-info-circle"></i>
                        </button>
                    </th>
                    <th class="text-end"><span class="text-pink">*</span>平均月薪</th>
                    <th class="text-end"><span class="text-pink">*</span>參與人月</th>
                    <th class="text-end"><span class="text-pink">*</span>人數</th>
                    <th class="text-end"><span class="text-pink">*</span>人事費小計</th>
                    <th width="150">功能</th>
                </tr>
                </thead>
                <tbody>
                <tr>
                    <td>
                        <select id="otherJobTitle1" class="form-select"></select>
                    </td>
                    <td>
                        <asp:TextBox ID="otherAvgSalary1" runat="server" ClientIDMode="Static" CssClass="form-control text-end" onblur="calculateOther()" />
                    </td>
                    <td>
                        <asp:TextBox ID="otherMonth1" runat="server" ClientIDMode="Static" CssClass="form-control text-end Month" Text="0" onblur="calculateOther()" />
                    </td>
                    <td>
                        <asp:TextBox ID="otherPeople1" runat="server" ClientIDMode="Static" CssClass="form-control text-end people" Text="0" onblur="calculateOther()" />
                    </td>
                    <td class="text-end">0</td>
                    <td>
                        <button type="button" class="btn btn-sm btn-teal" onclick="O_addRow()">
                            <i class="fas fa-plus"></i>
                        </button>
                    </td>
                </tr>
                <tr class="total-row">
                    <td colspan="4">合計</td>
                    <td class="text-end" id="otherTotal">0</td>
                    <td></td>
                </tr>
                </tbody>
            </table>
            
            <table class="table align-middle gray-table mt-4 otherRent">
                <thead>
                <tr>
                    <th width="220" class="text-center">項目</th>
                    <th width="220" class="text-end">金額</th>
                    <th>計算方式及說明</th>
                </tr>
                </thead>
                <tbody>
                <tr>
                    <td class="text-end align-middle">租金</td>
                    <td class="align-middle">
                        <asp:TextBox ID="rentCash" runat="server" ClientIDMode="Static" CssClass="form-control text-end" placeholder="請輸入" onblur="calculateOtherRentTotal()" />
                    </td>
                    <td class="align-middle">
                        <asp:TextBox ID="rentDescription" runat="server" ClientIDMode="Static" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="請輸入" />
                    </td>
                </tr>
                <tr>
                    <td class="text-end align-middle">勞務委託費</td>
                    <td class="align-middle">
                        <span id="serviceCash">0</span>
                    </td>
                    <td class="align-middle">
                        <span id="serviceDescription" style="white-space: pre-line; text-align: left; padding: 8px; display: block; min-height: 60px;"></span>
                    </td>
                </tr>
                <tr class="total-row">
                    <td>合計</td>
                    <td class="text-end" id="otherRentTotal">0</td>
                    <td></td>
                </tr>
                </tbody>
            </table>
        </div>

        <ul class="list-unstyled lh-base">
            <li>1.租金：限為海洋科技執行計畫需要，於計畫執行期間內，向外界機構、單位以「營業租賃」方式租用各項機械、儀器設備、場地、載運機械設備車輛、船舶等之租金。</li>
            <li>2.勞務委託費：限為海洋科技執行計畫需要，於計畫執行期間內，聘僱臨時人員工資、兼任研究助理(碩士班研究生)、派遣人力等費用。</li>
            <li>3.應依<a href="#" class="link-teal">附件十海洋科技專案計畫會計科目編列與執行原則</a>之規定辦理。</li>
            <li>4.若有共同執行單位(企業)經費投入請編列至配合款，並於本表備註為共同執行單位之費用。</li>
            <li>5.本會計科目之編列不含營業稅。</li>
        </ul>
    </div>

    <div id="point6">
        <h5 class="square-title mt-5">6.經費總表</h5>
        <div class="table-responsive mt-3 mb-0">
            <table class="table align-middle gray-table text-end main-table">
                <thead>
                <tr>
                    <th></th>
                    <th width="130" class="text-end">
                        <span class="text-pink">*</span>補助款(A)
                    </th>
                    <th width="130" class="text-end">
                        <span class="text-pink">*</span>配合款(B)
                    </th>
                    <th width="130" class="text-end">合計(C)</th>
                    <th width="130" class="text-end">佔總經費比率<br>
                        (C)/(II)
                    </th>
                    <th width="130" class="text-end">各科目補助比率<br>
                        (A)/(I)
                    </th>
                </tr>
                </thead>
                <tbody>
                <tr>
                    <th class="text-nowrap">1.人事費</th>
                    <td class="number-cell amount-a">0</td>
                    <td class="number-cell amount-b">
                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control text-end" Text="0" onblur="updateBudgetSummary()" />
                    </td>
                    <td class="number-cell amount-total">0</td>
                    <td class="number-cell">0%</td>
                    <td class="number-cell">0%</td>
                </tr>
                <tr>
                    <th class="text-nowrap">2.消耗性器材及原材料費</th>
                    <td class="number-cell amount-a">0</td>
                    <td class="number-cell amount-b">
                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control text-end" Text="0" onblur="updateBudgetSummary()" />
                    </td>
                    <td class="number-cell amount-total">0</td>
                    <td class="number-cell">0%</td>
                    <td class="number-cell">0%</td>
                </tr>
                <tr>
                    <th class="text-nowrap">3.技術移轉、委託研究或驗證費</th>
                    <td class="number-cell amount-a">0</td>
                    <td class="number-cell amount-b">
                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control text-end" Text="0" onblur="updateBudgetSummary()" />
                    </td>
                    <td class="number-cell amount-total">0</td>
                    <td class="number-cell">0%</td>
                    <td class="number-cell">0%</td>
                </tr>
                <tr>
                    <th class="text-nowrap">4.國內差旅費</th>
                    <td class="number-cell amount-a">0</td>
                    <td class="number-cell amount-b">
                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control text-end" Text="0" onblur="updateBudgetSummary()" />
                    </td>
                    <td class="number-cell amount-total">0</td>
                    <td class="number-cell">0%</td>
                    <td class="number-cell">0%</td>
                </tr>
                <tr>
                    <th class="text-nowrap">5.其他業務費</th>
                    <td class="number-cell amount-a">0</td>
                    <td class="number-cell amount-b">
                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control text-end" Text="0" onblur="updateBudgetSummary()" />
                    </td>
                    <td class="number-cell amount-total">0</td>
                    <td class="number-cell">0%</td>
                    <td class="number-cell">0%</td>
                </tr>
                <tr>
                    <th class="text-nowrap">6.行政管理費</th>
                    <td class="number-cell amount-a">
                        <asp:TextBox runat="server" ID="AdminFeeSubsidy" ClientIDMode="Static" CssClass="form-control text-end" Text="0" onblur="updateBudgetSummary()" />
                    </td>
                    <td class="number-cell amount-b">
                        <asp:TextBox runat="server" ID="AdminFeeCoop" ClientIDMode="Static" CssClass="form-control text-end" Text="0" onblur="updateBudgetSummary()" />
                    </td>
                    <td class="number-cell amount-total">0</td>
                    <td class="number-cell">0%</td>
                    <td class="number-cell">0%</td>
                </tr>
                <tr class="total-row">
                    <td class="text-nowrap">經費總計</td>
                    <td class="number-cell">0<br>(I)</td>
                    <td class="number-cell">0</td>
                    <td class="number-cell">0<br>(II)</td>
                    <td class="number-cell">--</td>
                    <td class="number-cell">--</td>
                </tr>
                <tr class="percentage-row">
                    <td class="text-nowrap">百分比</td>
                    <td class="number-cell">0%</td>
                    <td class="number-cell">0%</td>
                    <td class="number-cell">0%</td>
                    <td class="number-cell">--</td>
                    <td class="number-cell">--</td>
                </tr>
                </tbody>
            </table>
        </div>
        <ul class="list-unstyled lh-base">
            <li>1.請依海洋科技專案計畫會計科目編列與執行原則編列。</li>
            <li>2.經費撥付方式見契約第5條。</li>
            <li>3.總補助款以不超過<span class="text-pink">500萬元</span>為原則。</li>
            <li>4.共同執行單位(企業)經費請編列至配合款。</li>
            <li>5.行政管理費不得超過計畫總經費10%。</li>
            <li>6.計畫執行期間若辦理計畫變更與經費調整，致各會計科目占總經費百分比超過經費編列規定時，經主管機關核可，則該科目得維持原經費額度，惟各科目補助比率不得超過該科目經費50%。</li>
            <li>7.申請人配合款以小於申請人實收資本額為原則。</li>
        </ul>
    </div>

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

<!-- 變更說明 UserControl -->
<uc:ChangeDescriptionControl ID="ucChangeDescription" runat="server" />