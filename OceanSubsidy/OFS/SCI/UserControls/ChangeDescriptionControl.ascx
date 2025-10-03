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
        // // 使用閉包保存此控制項的 ClientID
        // var txtChangeBeforeID = '<%= txtChangeBefore.ClientID %>';
        // var txtChangeAfterID = '<%= txtChangeAfter.ClientID %>';
        // var hdnChangeBeforeID = '<%= hdnChangeBefore.ClientID %>';
        // var hdnChangeAfterID = '<%= hdnChangeAfter.ClientID %>';
        //
        // // 同步此控制項的 contenteditable 內容到隱藏欄位
        // function syncThisChangeDescription() {
        //     var changeBeforeElement = document.getElementById(txtChangeBeforeID);
        //     var changeAfterElement = document.getElementById(txtChangeAfterID);
        //     var hdnChangeBeforeElement = document.getElementById(hdnChangeBeforeID);
        //     var hdnChangeAfterElement = document.getElementById(hdnChangeAfterID);
        //
        //     if (changeBeforeElement && hdnChangeBeforeElement) {
        //         hdnChangeBeforeElement.value = changeBeforeElement.textContent || changeBeforeElement.innerText || '';
        //         console.log('同步變更前:', hdnChangeBeforeID, '=', hdnChangeBeforeElement.value);
        //     }
        //
        //     if (changeAfterElement && hdnChangeAfterElement) {
        //         hdnChangeAfterElement.value = changeAfterElement.textContent || changeAfterElement.innerText || '';
        //         console.log('同步變更後:', hdnChangeAfterID, '=', hdnChangeAfterElement.value);
        //     }
        // }

        // 在表單提交前同步資料
        // document.addEventListener('DOMContentLoaded', function() {
        //     var changeBeforeElement = document.getElementById(txtChangeBeforeID);
        //     var changeAfterElement = document.getElementById(txtChangeAfterID);
        //
        //     if (changeBeforeElement) {
        //         changeBeforeElement.addEventListener('blur', syncThisChangeDescription);
        //         changeBeforeElement.addEventListener('input', syncThisChangeDescription);
        //     }
        //
        //     if (changeAfterElement) {
        //         changeAfterElement.addEventListener('blur', syncThisChangeDescription);
        //         changeAfterElement.addEventListener('input', syncThisChangeDescription);
        //     }
        //
        //     // 表單提交前同步
        //     var forms = document.querySelectorAll('form');
        //     forms.forEach(function(form) {
        //         form.addEventListener('submit', syncThisChangeDescription);
        //     });
        // });

        // 將同步函數註冊到全域，讓按鈕的 OnClientClick 可以呼叫
        // 使用唯一的函數名稱避免衝突
        // window['syncChangeDescription_' + hdnChangeBeforeID.replace(/[^a-zA-Z0-9]/g, '_')] = syncThisChangeDescription;

        // 同時註冊通用的同步函數（同步所有變更說明控制項）
        if (!window.syncAllChangeDescriptions) {
            window.syncAllChangeDescriptions = function() {
                // 找到所有變更說明控制項的隱藏欄位
                var allHiddenFields = document.querySelectorAll('input[id*="hdnChangeBefore"], input[id*="hdnChangeAfter"]');
                var processedControls = new Set();

                allHiddenFields.forEach(function(hiddenField) {
                    var controlPrefix = hiddenField.id.replace(/(hdnChangeBefore|hdnChangeAfter)$/, '');

                    if (!processedControls.has(controlPrefix)) {
                        processedControls.add(controlPrefix);

                        var txtBeforeId = controlPrefix + 'txtChangeBefore';
                        var txtAfterId = controlPrefix + 'txtChangeAfter';
                        var hdnBeforeId = controlPrefix + 'hdnChangeBefore';
                        var hdnAfterId = controlPrefix + 'hdnChangeAfter';

                        var txtBefore = document.getElementById(txtBeforeId);
                        var txtAfter = document.getElementById(txtAfterId);
                        var hdnBefore = document.getElementById(hdnBeforeId);
                        var hdnAfter = document.getElementById(hdnAfterId);

                        if (txtBefore && hdnBefore) {
                            hdnBefore.value = txtBefore.textContent || txtBefore.innerText || '';
                            console.log('全域同步變更前:', hdnBeforeId, '=', hdnBefore.value);
                        }

                        if (txtAfter && hdnAfter) {
                            hdnAfter.value = txtAfter.textContent || txtAfter.innerText || '';
                            console.log('全域同步變更後:', hdnAfterId, '=', hdnAfter.value);
                        }
                    }
                });
            };
        }
    })();
</script>