<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ChangeDescriptionControl.ascx.cs" Inherits="OFS_SCI_UserControls_ChangeDescriptionControl" %>

<!-- 變更說明區塊 -->
<div id="changeDescriptionSection" runat="server" ClientIDMode="Static" class="mt-4" >
    <div class="text-pink fw-normal fs-16 mt-2">本頁若有資料變更，請務必詳細說明「變更欄位」及「變更前／變更後」之資料內容。若有多項欄位請條列式(1,2,3,...)說明。</div>
    <div class="text-pink fw-normal fs-16 mt-2">本頁若無任何修改，請填寫「無」</div>

    <h5 class="square-title mt-4">變更說明</h5>
    
    <table class="table align-middle gray-table side-table mt-3">
        <tbody>
            <tr>
                <th width="120">
                    <span class="text-pink">*</span>
                    變更前
                </th>
                <td>
                    <span class="form-control textarea" role="textbox" contenteditable=""
                          data-placeholder="請輸入變更前的內容" aria-label="變更前文本輸入區域"
                          id="txtChangeBefore" runat="server" ClientIDMode="Predictable" ></span>
                </td>
            </tr>
            <tr>
                <th>
                    <span class="text-pink">*</span>
                    變更後
                </th>
                <td>
                    <span class="form-control textarea" role="textbox" contenteditable=""
                          data-placeholder="請輸入變更後的內容" aria-label="變更後文本輸入區域"
                          id="txtChangeAfter" runat="server" ClientIDMode="Predictable"  ></span>
                </td>
            </tr>
        </tbody>
    </table>
</div>

<!-- 隱藏欄位用於資料儲存 -->
<asp:HiddenField ID="hdnChangeBefore" runat="server" ClientIDMode="Predictable" />
<asp:HiddenField ID="hdnChangeAfter" runat="server" ClientIDMode="Predictable" />

<script type="text/javascript">
    (function() {   
        // 同時註冊通用的同步函數（同步所有變更說明控制項）
        if (!window.syncAllChangeDescriptions) {
            window.syncAllChangeDescriptions = function() {
                console.log('=== syncAllChangeDescriptions 開始執行 ===');
                // 找到所有變更說明控制項的隱藏欄位
                var allHiddenFields = document.querySelectorAll('input[id*="hdnChangeBefore"], input[id*="hdnChangeAfter"]');
                console.log('找到隱藏欄位數量:', allHiddenFields.length);
                var processedControls = new Set();

                allHiddenFields.forEach(function(hiddenField) {
                    console.log('處理隱藏欄位 ID:', hiddenField.id);
                    var controlPrefix = hiddenField.id.replace(/(hdnChangeBefore|hdnChangeAfter)$/, '');
                    console.log('控制項前綴:', controlPrefix);

                    if (!processedControls.has(controlPrefix)) {
                        processedControls.add(controlPrefix);

                        var txtBeforeId = controlPrefix + 'txtChangeBefore';
                        var txtAfterId = controlPrefix + 'txtChangeAfter';
                        var hdnBeforeId = controlPrefix + 'hdnChangeBefore';
                        var hdnAfterId = controlPrefix + 'hdnChangeAfter';

                        console.log('尋找元素:', { txtBeforeId, txtAfterId, hdnBeforeId, hdnAfterId });

                        var txtBefore = document.getElementById(txtBeforeId);
                        var txtAfter = document.getElementById(txtAfterId);
                        var hdnBefore = document.getElementById(hdnBeforeId);
                        var hdnAfter = document.getElementById(hdnAfterId);

                        console.log('找到元素:', { txtBefore: !!txtBefore, txtAfter: !!txtAfter, hdnBefore: !!hdnBefore, hdnAfter: !!hdnAfter });

                        if (txtBefore && hdnBefore) {
                            var beforeValue = txtBefore.textContent || txtBefore.innerText || '';
                            hdnBefore.value = beforeValue;
                            // 確保 name 屬性與 ASP.NET 的 UniqueID 一致
                            if (hdnBefore.name && hdnBefore.name.indexOf('$') > -1) {
                                console.log('✅ 同步變更前 -', hdnBeforeId, '=', beforeValue, '(長度:', beforeValue.length, '), name:', hdnBefore.name);
                            } else {
                                console.warn('⚠️ hdnBefore.name 可能不正確:', hdnBefore.name);
                            }
                        } else {
                            console.warn('❌ 找不到變更前元素 -', { txtBeforeId, hdnBeforeId });
                        }

                        if (txtAfter && hdnAfter) {
                            var afterValue = txtAfter.textContent || txtAfter.innerText || '';
                            hdnAfter.value = afterValue;
                            // 確保 name 屬性與 ASP.NET 的 UniqueID 一致
                            if (hdnAfter.name && hdnAfter.name.indexOf('$') > -1) {
                                console.log('✅ 同步變更後 -', hdnAfterId, '=', afterValue, '(長度:', afterValue.length, '), name:', hdnAfter.name);
                            } else {
                                console.warn('⚠️ hdnAfter.name 可能不正確:', hdnAfter.name);
                            }
                        } else {
                            console.warn('❌ 找不到變更後元素 -', { txtAfterId, hdnAfterId });
                        }
                    }
                });
                console.log('=== syncAllChangeDescriptions 執行完畢 ===');
            };
        }
    })();
</script>