<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ChangeDescriptionControl.ascx.cs" Inherits="OFS_SCI_UserControls_ChangeDescriptionControl" %>

<!-- 變更說明區塊 -->
<div id="changeDescriptionSection" runat="server" ClientIDMode="Static" class="mt-4" >
    <div class="text-pink fw-normal fs-16 mt-2">本頁若有資料變更，請務必詳細說明「變更欄位」及「變更前／變更後」之資料內容。若有多項欄位請條列式(1,2,3,...)說明。</div>
    
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
                          id="txtChangeBefore" name="txtChangeBefore" ClientIDMode="Static" ></span>
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
                          id="txtChangeAfter" name="txtChangeAfter" ClientIDMode="Static"  ></span>
                </td>
            </tr>
        </tbody>
    </table>
</div>

<!-- 隱藏欄位用於資料儲存 -->
<asp:HiddenField ID="hdnChangeBefore" runat="server" />
<asp:HiddenField ID="hdnChangeAfter" runat="server" />

<script type="text/javascript">
    // 同步 contenteditable 內容到隱藏欄位
    function syncChangeDescription() {
        const changeBeforeElement = document.getElementById('txtChangeBefore');
        const changeAfterElement = document.getElementById('txtChangeAfter');
        const hdnChangeBeforeElement = document.getElementById('<%= hdnChangeBefore.ClientID %>');
        const hdnChangeAfterElement = document.getElementById('<%= hdnChangeAfter.ClientID %>');
        
        if (changeBeforeElement && hdnChangeBeforeElement) {
            hdnChangeBeforeElement.value = changeBeforeElement.textContent || changeBeforeElement.innerText || '';
        }
        
        if (changeAfterElement && hdnChangeAfterElement) {
            hdnChangeAfterElement.value = changeAfterElement.textContent || changeAfterElement.innerText || '';
        }
    }

    // 在表單提交前同步資料
    document.addEventListener('DOMContentLoaded', function() {
        // 監聽 contenteditable 變更
        const changeBeforeElement = document.getElementById('txtChangeBefore');
        const changeAfterElement = document.getElementById('txtChangeAfter');
        
        if (changeBeforeElement) {
            changeBeforeElement.addEventListener('blur', syncChangeDescription);
            changeBeforeElement.addEventListener('input', syncChangeDescription);
        }
        
        if (changeAfterElement) {
            changeAfterElement.addEventListener('blur', syncChangeDescription);
            changeAfterElement.addEventListener('input', syncChangeDescription);
        }
        
        // 表單提交前同步
        const forms = document.querySelectorAll('form');
        forms.forEach(function(form) {
            form.addEventListener('submit', syncChangeDescription);
        });
    });
</script>