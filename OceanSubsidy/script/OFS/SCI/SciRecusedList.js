/**
 * 建議迴避之審查委員清單 JavaScript 功能
 * 檔案: SciRecusedList.js
 * 作者: 系統生成
 * 說明: 處理建議迴避之審查委員清單頁面的前端互動功能
 */

// 頁面載入後初始化
$(document).ready(function () {
    // 初始化動態表格功能
    initializeDynamicTables();
    
    // 初始化工具提示
    initializeTooltips();
    
    // 初始化無需迴避之審查委員功能
    initializeNoAvoidanceFeature();
    
    // 載入現有資料
    loadExistingData();
    
    console.log("SciRecusedList 頁面已載入");
});

// 初始化動態表格功能
function initializeDynamicTables() {
    // 為新增按鈕綁定事件
    $(document).on('click', '.add-row', function(e) {
        e.preventDefault();
        addNewRow($(this));
    });
    
    // 為刪除按鈕綁定事件
    $(document).on('click', '.delete-row', function(e) {
        e.preventDefault();
        deleteRow($(this));
    });
}

// 初始化工具提示
function initializeTooltips() {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// 初始化無需迴避之審查委員功能
function initializeNoAvoidanceFeature() {
    // 尋找並綁定 checkbox 事件
    const checkboxes = document.querySelectorAll('input[type="checkbox"][id*="chkNoAvoidance"], input[type="checkbox"][class*="chkNoAvoidance"]');
    
    // 嘗試多種方式找到 checkbox
    let checkbox = document.getElementById('chkNoAvoidance');
    
    // 如果沒找到，嘗試其他方式
    if (!checkbox) {
        // 尋找包含 chkNoAvoidance 的 id
        checkbox = document.querySelector('input[type="checkbox"][id*="chkNoAvoidance"]');
    }
    
    // 如果還是沒找到，使用更廣泛的選擇器
    if (!checkbox) {
        const label = document.querySelector('label');
        if (label && label.textContent.includes('無需迴避之審查委員')) {
            const forAttribute = label.getAttribute('for');
            if (forAttribute) {
                checkbox = document.getElementById(forAttribute);
            }
        }
    }
    
    if (checkbox) {
        console.log('找到無需迴避之審查委員 checkbox:', checkbox.id);
        checkbox.addEventListener('change', handleNoAvoidanceChange);
    } else {
        console.warn('未找到無需迴避之審查委員 checkbox');
    }
}

// 處理無需迴避之審查委員 checkbox 變更事件
function handleNoAvoidanceChange(event) {
    const isChecked = event.target.checked;
    const $committeeTable = $('#committeeTable');
    const $tbody = $committeeTable.find('tbody');
    
    console.log('無需迴避之審查委員 checkbox 狀態:', isChecked);
    
    if (isChecked) {
        // 勾選時：清空資料、縮減到一行、鎖定欄位
        clearAndLockCommitteeTable($tbody);
    } else {
        // 取消勾選時：解鎖欄位
        unlockCommitteeTable($tbody);
    }
}

// 清空並鎖定委員表格
function clearAndLockCommitteeTable($tbody) {
    // 移除除了第一行以外的所有行
    $tbody.find('tr').slice(1).remove();
    
    // 清空第一行的所有輸入欄位
    const $firstRow = $tbody.find('tr').first();
    $firstRow.find('input[type="text"]').val('').prop('disabled', true);
    
    // 將按鈕改為加號按鈕並禁用
    const $btnCell = $firstRow.find('td').last();
    $btnCell.html(`
        <button type="button" class="btn btn-sm btn-teal" disabled>
            <i class="fas fa-plus"></i>
        </button>
    `);
    
    console.log('委員表格已清空並鎖定');
}

// 解鎖委員表格
function unlockCommitteeTable($tbody) {
    const $firstRow = $tbody.find('tr').first();
    
    // 解鎖所有輸入欄位
    $firstRow.find('input[type="text"]').prop('disabled', false);
    
    // 恢復按鈕功能
    const $btnCell = $firstRow.find('td').last();
    $btnCell.html(`
        <button type="button" class="btn btn-sm btn-teal add-row">
            <i class="fas fa-plus"></i>
        </button>
    `);
    
    console.log('委員表格已解鎖');
}

// 檢查無需迴避之審查委員是否被勾選
function isNoAvoidanceChecked() {
    // 嘗試多種方式找到 checkbox
    let checkbox = document.getElementById('chkNoAvoidance');
    
    if (!checkbox) {
        checkbox = document.querySelector('input[type="checkbox"][id*="chkNoAvoidance"]');
    }
    
    if (!checkbox) {
        const label = document.querySelector('label');
        if (label && label.textContent.includes('無需迴避之審查委員')) {
            const forAttribute = label.getAttribute('for');
            if (forAttribute) {
                checkbox = document.getElementById(forAttribute);
            }
        }
    }
    
    return checkbox ? checkbox.checked : false;
}

// 新增表格行
function addNewRow($btn) {
    var $table = $btn.closest('table');
    var $tbody = $table.find('tbody');
    var isCommitteeTable = $table.attr('id') === 'committeeTable';
    
    // 如果是委員表格，檢查是否已勾選無需迴避
    if (isCommitteeTable && isNoAvoidanceChecked()) {
        console.log('無需迴避之審查委員已勾選，無法新增行');
        return;
    }
    
    var newRowHtml = '';
    if (isCommitteeTable) {
        // 委員清單表格的新行
        newRowHtml = `
            <tr>
                <td><input type="text" class="form-control" name="committeeName" /></td>
                <td><input type="text" class="form-control" name="committeeUnit" placeholder="請輸入任職單位" /></td>
                <td><input type="text" class="form-control" name="committeePosition" placeholder="請輸入職稱" /></td>
                <td><input type="text" class="form-control" name="committeeReason" placeholder="請輸入應迴避之具體理由及事證" /></td>
                <td>
                    <button type="button" class="btn btn-sm btn-teal delete-row">
                        <i class="fas fa-trash-alt"></i>
                    </button>
                </td>
            </tr>`;
    } else {
        // 技術能力表格的新行
        newRowHtml = `
            <tr>
                <td><input type="text" class="form-control" name="techItem" placeholder="請輸入" /></td>
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
                    <textarea class="form-control" rows="3" name="techProcess" placeholder="請輸入"></textarea>
                </td>
                <td>
                    <button type="button" class="btn btn-sm btn-teal delete-row">
                        <i class="fas fa-trash-alt"></i>
                    </button>
                </td>
            </tr>`;
    }
    
    // 加號按鈕保持不變，直接在表格最後添加新行（新行帶有垃圾桶）
    $tbody.append(newRowHtml);
}

// 刪除表格行
function deleteRow($btn) {
    var $row = $btn.closest('tr');
    var $tbody = $row.closest('tbody');
    var $table = $row.closest('table');
    var isCommitteeTable = $table.attr('id') === 'committeeTable';
    
    // 如果是委員表格，檢查是否已勾選無需迴避
    if (isCommitteeTable && isNoAvoidanceChecked()) {
        console.log('無需迴避之審查委員已勾選，無法刪除行');
        return;
    }
    
    // 計算有加號按鈕的行和有垃圾桶按鈕的行
    var $addRows = $tbody.find('.add-row').closest('tr');
    var $deleteRows = $tbody.find('.delete-row').closest('tr');
    
    // 至少保留一行（加號按鈕的行）
    if ($addRows.length + $deleteRows.length <= 1) {
        alert('至少需要保留一行資料');
        return;
    }
    
    $row.remove();
}

// 收集委員清單資料
function collectCommitteeData() {
    // 如果勾選了無需迴避之審查委員，回傳空陣列
    if (isNoAvoidanceChecked()) {
        return [];
    }
    
    var data = [];
    $('#committeeTableBody tr').each(function() {
        var $row = $(this);
        var item = {
            name: $row.find('input[name="committeeName"]').val(),
            unit: $row.find('input[name="committeeUnit"]').val(),
            position: $row.find('input[name="committeePosition"]').val(),
            reason: $row.find('input[name="committeeReason"]').val()
        };
        
        // 只收集有填寫內容的行
        if (item.name || item.unit || item.position || item.reason) {
            data.push(item);
        }
    });
    
    return data;
}

// 收集技術能力資料
function collectTechData() {
    var data = [];
    $('#techTableBody tr').each(function() {
        var $row = $(this);
        var item = {
            techItem: $row.find('input[name="techItem"]').val(),
            trlPlanLevel: $row.find('select[name="trlPlanLevel"]').val(),
            trlTrackLevel: $row.find('select[name="trlTrackLevel"]').val(),
            techProcess: $row.find('textarea[name="techProcess"]').val()
        };
        
        // 只收集有填寫內容的行
        if (item.techItem || item.trlPlanLevel || item.trlTrackLevel || item.techProcess) {
            data.push(item);
        }
    });
    
    return data;
}

// 前端驗證函數
function validateForm() {
    var isValid = true;
    var errorMessages = [];
    
    // 驗證委員清單
    var committeeData = collectCommitteeData();
    var noAvoidanceChecked = isNoAvoidanceChecked();
    
    if (committeeData.length === 0 && !noAvoidanceChecked) {
        errorMessages.push('請填寫建議迴避之審查委員清單或勾選「無需迴避之審查委員」');
        isValid = false;
    }
    
    // 驗證技術能力
    var techData = collectTechData();
    if (techData.length === 0) {
        errorMessages.push('請填寫技術能力相關資料');
        isValid = false;
    }
    
    // 顯示錯誤訊息
    if (!isValid) {
        alert(errorMessages.join('\n'));
    }
    
    return isValid;
}
// 技術能力與技術關聯圖管理器
class TechDiagramManager {
    constructor() {
        this.init();
    }

    init() {
        this.bindTechDiagramUpload();
    }

    bindTechDiagramUpload() {
        const uploadBtn = document.getElementById('btnUploadTechDiagram');
        const deleteBtn = document.getElementById('btnDeleteTechDiagram');

        if (uploadBtn) {
            uploadBtn.addEventListener('click', (e) => {
                e.preventDefault();
                this.handleTechDiagramPreview();
            });
        }

        if (deleteBtn) {
            deleteBtn.addEventListener('click', (e) => {
                e.preventDefault();
                this.handleTechDiagramDelete();
            });
        }
    }

    handleTechDiagramPreview() {
        const fileInput = document.getElementById('fileUploadTechDiagram');

        if (!fileInput || !fileInput.files.length) {
            alert('請先選擇要上傳的檔案');
            return;
        }

        const file = fileInput.files[0];

        if (!file.type.match(/^image\/(jpeg|jpg|png)$/i)) {
            alert('請選擇JPG或PNG格式的圖片文件');
            return;
        }

        if (file.size > 10 * 1024 * 1024) {
            alert('文件大小不能超過10MB');
            return;
        }

        this.showFilePreview(file);
        this.prepareFileForUpload();
        console.log('技術能力與技術關聯圖預覽已載入:', file.name);
    }

    prepareFileForUpload() {
        const fileInput = document.getElementById('fileUploadTechDiagram');
        if (fileInput && fileInput.files.length > 0) {
            // 確保 form 包含 file input，並設定正確的 name 屬性
            fileInput.name = 'fileUploadTechDiagram';
            
            const form = document.getElementById('form1');
            if (form && !form.contains(fileInput)) {
                form.appendChild(fileInput);
            }
            
            // 設定 form 為 multipart/form-data
            if (form) {
                form.enctype = 'multipart/form-data';
            }
        }
    }

    showFilePreview(file) {
        const reader = new FileReader();
        reader.onload = (e) => {
            this.displayTechDiagramPreview(e.target.result, file.name);
        };
        reader.readAsDataURL(file);
    }

    handleTechDiagramDelete() {
        const fileInput = document.getElementById('fileUploadTechDiagram');
        if (fileInput) {
            fileInput.value = '';
        }

        this.hideTechDiagramPreview();
        console.log('技術能力與技術關聯圖預覽已清除');
    }

    displayTechDiagramPreview(imageSrc, fileName) {
        const previewContainer = document.getElementById('techDiagramPreviewContainer');
        const previewImg = document.getElementById('techDiagramPreview');

        if (previewContainer && previewImg) {
            previewImg.src = imageSrc;
            previewImg.alt = `技術能力與技術關聯圖 - ${fileName}`;
            previewContainer.style.display = 'block';
            console.log('技術能力與技術關聯圖預覽顯示成功:', fileName);
        }
    }

    hideTechDiagramPreview() {
        const previewContainer = document.getElementById('techDiagramPreviewContainer');
        const previewImg = document.getElementById('techDiagramPreview');

        if (previewContainer) {
            previewContainer.style.display = 'none';
        }

        if (previewImg) {
            previewImg.src = '';
            previewImg.alt = '';
        }
    }

    loadTechDiagramFile(filePath, fileName) {
        try {
            console.log('開始載入技術能力與技術關聯圖：', filePath, fileName);

            if (filePath && fileName) {
                const fullPath = filePath.startsWith('/') ? filePath : `/${filePath}`;
                this.displayTechDiagramPreview(fullPath, fileName);
                console.log('技術能力與技術關聯圖載入完成：', fullPath);
            }
        } catch (error) {
            console.error('載入技術能力與技術關聯圖時發生錯誤：', error);
        }
    }

    collectTechDiagramData() {
        const previewImg = document.getElementById('techDiagramPreview');
        const previewContainer = document.getElementById('techDiagramPreviewContainer');

        const hasPreview = previewContainer && previewContainer.style.display !== 'none' && previewImg && previewImg.src;

        if (hasPreview) {
            return {
                hasImage: true,
                imageSrc: previewImg.src,
                imageAlt: previewImg.alt
            };
        }

        return {
            hasImage: false,
            imageSrc: '',
            imageAlt: ''
        };
    }
}

// 載入現有資料並渲染到表格
function loadExistingData(retryCount = 0) {
    // 檢查是否有現有資料
    if (typeof window.existingData === 'undefined') {
        if (retryCount < 3) {
            console.log(`沒有現有資料需要載入，將在500ms後重試 (${retryCount + 1}/3)`);
            // 延遲重試，因為後端資料可能還沒載入完成
            setTimeout(() => loadExistingData(retryCount + 1), 500);
        } else {
            console.log('重試次數已達上限，停止載入現有資料');
        }
        return;
    }
    
    const data = window.existingData;
    console.log('找到現有資料，開始載入：', data);
    
    // 載入委員迴避清單資料
    if (data.recusedData && data.recusedData.length > 0) {
        loadCommitteeData(data.recusedData);
        console.log('已載入委員迴避清單資料：', data.recusedData);
    } else {
        console.log('沒有委員迴避清單資料需要載入');
    }
    
    // 載入技術能力資料
    if (data.techData && data.techData.length > 0) {
        loadTechData(data.techData);
        console.log('已載入技術能力資料：', data.techData);
    } else {
        console.log('沒有技術能力資料需要載入');
    }
}

// 載入委員迴避清單資料到表格
function loadCommitteeData(recusedData) {
    const $tbody = $('#committeeTableBody');
    
    // 清空現有資料，只保留第一行作為模板
    $tbody.find('tr').slice(1).remove();
    
    // 檢查是否有資料，如果沒有資料且無需迴避checkbox未勾選，則保持空白
    if (!recusedData || recusedData.length === 0) {
        // 確保第一行保持空白狀態
        const $firstRow = $tbody.find('tr').first();
        $firstRow.find('input[type="text"]').val('');
        return;
    }
    
    // 如果有資料，則載入資料並添加新行
    recusedData.forEach((item, index) => {
        if (index === 0) {
            // 第一筆資料填入現有的第一行
            const $firstRow = $tbody.find('tr').first();
            $firstRow.find('input[name="committeeName"]').val(item.committeeName || '');
            $firstRow.find('input[name="committeeUnit"]').val(item.committeeUnit || '');
            $firstRow.find('input[name="committeePosition"]').val(item.committeePosition || '');
            $firstRow.find('input[name="committeeReason"]').val(item.committeeReason || '');
            
            // 將第一行的加號按鈕改為垃圾桶按鈕（如果有多筆資料）
            if (recusedData.length > 1) {
                const $addBtn = $firstRow.find('.add-row');
                if ($addBtn.length > 0) {
                    $addBtn.removeClass('add-row').addClass('delete-row');
                    $addBtn.find('i').removeClass('fa-plus').addClass('fa-trash-alt');
                }
            }
        } else {
            // 其他資料創建新行
            const newRowHtml = `
                <tr>
                    <td><input type="text" class="form-control" name="committeeName" value="${item.committeeName || ''}" /></td>
                    <td><input type="text" class="form-control" name="committeeUnit" placeholder="請輸入任職單位" value="${item.committeeUnit || ''}" /></td>
                    <td><input type="text" class="form-control" name="committeePosition" placeholder="請輸入職稱" value="${item.committeePosition || ''}" /></td>
                    <td><input type="text" class="form-control" name="committeeReason" placeholder="請輸入應迴避之具體理由及事證" value="${item.committeeReason || ''}" /></td>
                    <td>
                        <button type="button" class="btn btn-sm btn-teal delete-row">
                            <i class="fas fa-trash-alt"></i>
                        </button>
                    </td>
                </tr>`;
            $tbody.append(newRowHtml);
        }
    });
    
    // 在最後一行添加加號按鈕
    const $lastRow = $tbody.find('tr').last();
    const $lastCell = $lastRow.find('td').last();
    if (!$lastCell.find('.add-row').length) {
        $lastCell.append(`
            <button type="button" class="btn btn-sm btn-teal add-row">
                <i class="fas fa-plus"></i>
            </button>
        `);
    }
}

// 載入技術能力資料到表格
function loadTechData(techData) {
    const $tbody = $('#techTableBody');
    
    // 清空現有資料，只保留第一行作為模板
    $tbody.find('tr').slice(1).remove();
    
    // 如果有資料，則載入資料並添加新行
    techData.forEach((item, index) => {
        if (index === 0) {
            // 第一筆資料填入現有的第一行
            const $firstRow = $tbody.find('tr').first();
            $firstRow.find('input[name="techItem"]').val(item.techItem || '');
            
            // 設定執行前TRL層級
            const $trlPlanSelect = $firstRow.find('select[name="trlPlanLevel"]');
            if ($trlPlanSelect.length > 0) {
                $trlPlanSelect.val(item.trlPlanLevel || '0');
            }
            
            // 設定執行後TRL層級
            const $trlTrackSelect = $firstRow.find('select[name="trlTrackLevel"]');
            if ($trlTrackSelect.length > 0) {
                $trlTrackSelect.val(item.trlTrackLevel || '0');
            }
            
            $firstRow.find('textarea[name="techProcess"]').val(item.techProcess || '');
            
            // 將第一行的加號按鈕改為垃圾桶按鈕（如果有多筆資料）
            if (techData.length > 1) {
                const $addBtn = $firstRow.find('.add-row');
                if ($addBtn.length > 0) {
                    $addBtn.removeClass('add-row').addClass('delete-row');
                    $addBtn.find('i').removeClass('fa-plus').addClass('fa-trash-alt');
                }
            }
        } else {
            // 其他資料創建新行
            const newRowHtml = `
                <tr>
                    <td><input type="text" class="form-control" name="techItem" placeholder="請輸入" value="${item.techItem || ''}" /></td>
                    <td>
                        <div class="input-group">
                            <span class="input-group-text">執行前</span>
                            <select class="form-select" name="trlPlanLevel">
                                <option value="0" ${(!item.trlPlanLevel || item.trlPlanLevel === '0') ? 'selected' : ''}>請選擇</option>
                                <option value="1" ${item.trlPlanLevel === '1' ? 'selected' : ''}>TRL 1：界定機會與挑戰</option>
                                <option value="2" ${item.trlPlanLevel === '2' ? 'selected' : ''}>TRL 2：構思因應方案</option>
                                <option value="3" ${item.trlPlanLevel === '3' ? 'selected' : ''}>TRL 3：進行概念性驗證實驗</option>
                                <option value="4" ${item.trlPlanLevel === '4' ? 'selected' : ''}>TRL 4：進行關鍵要素之現場試驗</option>
                                <option value="5" ${item.trlPlanLevel === '5' ? 'selected' : ''}>TRL 5：驗證商品化之可行性</option>
                                <option value="6" ${item.trlPlanLevel === '6' ? 'selected' : ''}>TRL 6：完成實用性原型開發</option>
                                <option value="7" ${item.trlPlanLevel === '7' ? 'selected' : ''}>TRL 7：市場可及性</option>
                                <option value="8" ${item.trlPlanLevel === '8' ? 'selected' : ''}>TRL 8：建立商用</option>
                                <option value="9" ${item.trlPlanLevel === '9' ? 'selected' : ''}>TRL 9：達成持續生產</option>
                            </select>
                        </div>
                        <div class="input-group mt-2">
                            <span class="input-group-text">執行後</span>
                            <select class="form-select" name="trlTrackLevel">
                                <option value="0" ${(!item.trlTrackLevel || item.trlTrackLevel === '0') ? 'selected' : ''}>請選擇</option>
                                <option value="1" ${item.trlTrackLevel === '1' ? 'selected' : ''}>TRL 1：界定機會與挑戰</option>
                                <option value="2" ${item.trlTrackLevel === '2' ? 'selected' : ''}>TRL 2：構思因應方案</option>
                                <option value="3" ${item.trlTrackLevel === '3' ? 'selected' : ''}>TRL 3：進行概念性驗證實驗</option>
                                <option value="4" ${item.trlTrackLevel === '4' ? 'selected' : ''}>TRL 4：進行關鍵要素之現場試驗</option>
                                <option value="5" ${item.trlTrackLevel === '5' ? 'selected' : ''}>TRL 5：驗證商品化之可行性</option>
                                <option value="6" ${item.trlTrackLevel === '6' ? 'selected' : ''}>TRL 6：完成實用性原型開發</option>
                                <option value="7" ${item.trlTrackLevel === '7' ? 'selected' : ''}>TRL 7：市場可及性</option>
                                <option value="8" ${item.trlTrackLevel === '8' ? 'selected' : ''}>TRL 8：建立商用</option>
                                <option value="9" ${item.trlTrackLevel === '9' ? 'selected' : ''}>TRL 9：達成持續生產</option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <textarea class="form-control" rows="3" name="techProcess" placeholder="請輸入">${item.techProcess || ''}</textarea>
                    </td>
                    <td>
                        <button type="button" class="btn btn-sm btn-teal delete-row">
                            <i class="fas fa-trash-alt"></i>
                        </button>
                    </td>
                </tr>`;
            $tbody.append(newRowHtml);
        }
    });
    
    // 在最後一行添加加號按鈕
    const $lastRow = $tbody.find('tr').last();
    const $lastCell = $lastRow.find('td').last();
    if (!$lastCell.find('.add-row').length) {
        $lastCell.append(`
            <button type="button" class="btn btn-sm btn-teal add-row">
                <i class="fas fa-plus"></i>
            </button>
        `);
    }
}

// 頁面載入後初始化技術能力與技術關聯圖管理器
document.addEventListener('DOMContentLoaded', function () {
    window.techDiagramManager = new TechDiagramManager();
});