<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReviewChecklist.aspx.cs" Inherits="OFS_ReviewChecklist" Culture="zh-TW" UICulture="zh-TW" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>審查系統</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: "Microsoft JhengHei", Arial, sans-serif;
            background-color: #f5f5f5;
            color: #333;
        }

        .container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
        }

        /* 導航區域 */
        .nav-breadcrumb {
            background: white;
            padding: 10px 20px;
            border-radius: 5px;
            margin-bottom: 20px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }

        .nav-breadcrumb span {
            color: #666;
            font-size: 14px;
        }

        /* 標籤區域 */
        .tabs-container {
            display: flex;
            gap: 5px;
            margin-bottom: 30px;
        }

        .tab-item {
            background: white;
            border: 1px solid #ddd;
            border-radius: 8px 8px 0 0;
            padding: 15px 25px;
            text-align: center;
            min-width: 120px;
            cursor: pointer;
            transition: all 0.3s;
            position: relative;
        }

        .tab-item.active {
            background: #4a90e2;
            color: white;
            border-color: #4a90e2;
        }

        .tab-item:not(.active):hover {
            background: #f0f8ff;
        }
        
        .tab-item.tab-disabled {
            background: #f5f5f5;
            color: #999;
            cursor: not-allowed;
            opacity: 0.6;
        }
        
        .tab-item.tab-disabled:hover {
            background: #f5f5f5;
        }

        .tab-title {
            font-weight: bold;
            margin-bottom: 5px;
        }

        .tab-count {
            font-size: 24px;
            font-weight: bold;
        }

        .tab-unit {
            font-size: 12px;
            margin-left: 2px;
        }

        /* 搜尋過濾區域 */
        .filter-section {
            background: white;
            padding: 20px;
            border-radius: 5px;
            margin-bottom: 20px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }

        .filter-row {
            display: flex;
            align-items: center;
            gap: 15px;
            flex-wrap: wrap;
        }

        .search-input {
            padding: 8px 12px;
            border: 1px solid #ddd;
            border-radius: 4px;
            width: 200px;
        }

        .filter-select {
            padding: 8px 12px;
            border: 1px solid #ddd;
            border-radius: 4px;
            background: white;
            min-width: 120px;
        }

        .search-btn {
            background: #5cb3cc;
            color: white;
            border: none;
            padding: 8px 20px;
            border-radius: 4px;
            cursor: pointer;
            font-weight: bold;
        }

        .search-btn:hover {
            background: #4a9bb8;
        }

        /* 資料表格區域 */
        .data-section {
            background: white;
            border-radius: 5px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }

        .data-header {
            padding: 15px 20px;
            border-bottom: 1px solid #eee;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .record-info {
            color: #666;
            font-size: 14px;
        }

        .pagination-info {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .pagination-select {
            padding: 5px;
            border: 1px solid #ddd;
            border-radius: 3px;
        }

        /* 表格樣式 */
        .data-table {
            width: 100%;
            border-collapse: collapse;
        }

        .data-table th {
            background: #f8f9fa;
            padding: 12px 8px;
            text-align: center;
            border-bottom: 2px solid #dee2e6;
            font-weight: bold;
            cursor: pointer;
        }

        .data-table th:hover {
            background: #e9ecef;
        }

        .data-table td {
            padding: 12px 8px;
            text-align: center;
            border-bottom: 1px solid #eee;
        }

        .data-table tr:nth-child(even) {
            background: #fafafa;
        }

        .data-table tr:hover {
            background: #f0f8ff;
        }

        /* 狀態標籤 */
        .status-tag {
            padding: 4px 8px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: bold;
        }

        .status-review { background: #fff3cd; color: #856404; }
        .status-pass { background: #d4edda; color: #155724; }
        .status-fail { background: #f8d7da; color: #721c24; }
        .status-pending { background: #cce5ff; color: #004085; }

        /* 操作按鈕 */
        .action-btn {
            padding: 4px 12px;
            border: none;
            border-radius: 3px;
            cursor: pointer;
            font-size: 12px;
            font-weight: bold;
        }

        .btn-review {
            background: #5cb3cc;
            color: white;
        }

        .btn-review:hover {
            background: #4a9bb8;
        }

        /* 底部按鈕區域 */
        .bottom-actions {
            padding: 20px;
            display: flex;
            gap: 15px;
        }

        .batch-btn {
            padding: 10px 20px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-weight: bold;
            color: white;
        }

        .btn-batch-pass {
            background: #28a745;
        }

        .btn-batch-reject {
            background: #6c757d;
        }

        .btn-export {
            background: #17a2b8;
            margin-left: auto;
        }

        .batch-btn:hover {
            opacity: 0.9;
        }

        /* 分頁控制 */
        .pagination-controls {
            padding: 15px 20px;
            border-top: 1px solid #eee;
            display: flex;
            justify-content: center;
            align-items: center;
            gap: 10px;
        }

        .page-btn {
            padding: 5px 10px;
            border: 1px solid #ddd;
            background: white;
            cursor: pointer;
            border-radius: 3px;
        }

        .page-btn:hover {
            background: #f0f8ff;
        }

        .page-btn.active {
            background: #007bff;
            color: white;
            border-color: #007bff;
        }

        .page-select {
            padding: 5px;
            border: 1px solid #ddd;
            border-radius: 3px;
        }
    </style>
      <script type="text/javascript">
            function updateTableData(htmlContent) {
                var tableBody = document.getElementById('dataTableBody');
                if (tableBody) {
                    tableBody.innerHTML = htmlContent;
                    // 重新綁定事件
                    bindCheckboxEvents();
                }
            }
            
            // 綁定 checkbox 事件
            function bindCheckboxEvents() {
                // 全選/反選功能
                var selectAllCheckbox = document.getElementById('selectAllCheckbox');
                if (selectAllCheckbox) {
                    selectAllCheckbox.addEventListener('change', function() {
                        var caseCheckboxes = document.querySelectorAll('.case-checkbox');
                        caseCheckboxes.forEach(function(checkbox) {
                            checkbox.checked = selectAllCheckbox.checked;
                        });
                    });
                }
                
                // 單個checkbox的變更事件
                var caseCheckboxes = document.querySelectorAll('.case-checkbox');
                caseCheckboxes.forEach(function(checkbox) {
                    checkbox.addEventListener('change', function() {
                        updateSelectAllState();
                    });
                });
            }
            
            // 更新全選checkbox的狀態
            function updateSelectAllState() {
                var selectAllCheckbox = document.getElementById('selectAllCheckbox');
                var caseCheckboxes = document.querySelectorAll('.case-checkbox');
                var checkedCount = document.querySelectorAll('.case-checkbox:checked').length;
                
                if (checkedCount === 0) {
                    selectAllCheckbox.checked = false;
                    selectAllCheckbox.indeterminate = false;
                } else if (checkedCount === caseCheckboxes.length) {
                    selectAllCheckbox.checked = true;
                    selectAllCheckbox.indeterminate = false;
                } else {
                    selectAllCheckbox.checked = false;
                    selectAllCheckbox.indeterminate = true;
                }
            }
            
            // 收集選中的案件
            function getSelectedCases() {
                var selectedCases = [];
                var checkedBoxes = document.querySelectorAll('.case-checkbox:checked');
                
                checkedBoxes.forEach(function(checkbox) {
                    selectedCases.push({
                        projectId: checkbox.getAttribute('data-projectid'),
                        status: checkbox.getAttribute('data-status'),
                        stage: checkbox.getAttribute('data-stage')
                    });
                });
                
                return selectedCases;
            }
            
            // 批次通過前的檢查
            function validateBatchPass() {
                var selectedCases = getSelectedCases();
                
                if (selectedCases.length === 0) {
                    alert('請至少選擇一個案件');
                    return false;
                }
                
                // 檢查是否都是「通過」
                var invalidCases = selectedCases.filter(function(item) {
                    return item.status !== '通過' ;
                });
                
                if (invalidCases.length > 0) {
                    alert('只能對狀態為「通過」案件進行批次通過操作');
                    return false;
                }
                
                // 確認操作
                if (confirm(`確定要將 ${selectedCases.length} 個案件批次通過，進入下一階段嗎？`)) {
                    // 將選中的 ProjectID 傳送到後端
                    var projectIds = selectedCases.map(function(item) { return item.projectId; });
                    document.getElementById('<%= hidSelectedProjectIds.ClientID %>').value = JSON.stringify(projectIds);
                    return true;
                }
                
                return false;
            }
            
            // 批次不通過前的檢查
            function validateBatchReject() {
                var selectedCases = getSelectedCases();
                
                if (selectedCases.length === 0) {
                    alert('請至少選擇一個案件');
                    return false;
                }
                
                // 檢查是否都是「不通過」或「逾期未補」
                var invalidCases = selectedCases.filter(function(item) {
                    return item.status !== '不通過' && item.status !== '逾期未補';
                });
                
                if (invalidCases.length > 0) {
                    alert('只能對狀態為「不通過」或「逾期未補」的案件進行批次不通過操作');
                    return false;
                }
                
                // 確認操作
                if (confirm(`確定要將 ${selectedCases.length} 個案件批次不通過並結案嗎？此操作不可逆轉！`)) {
                    // 將選中的 ProjectID 傳送到後端
                    var projectIds = selectedCases.map(function(item) { return item.projectId; });
                    document.getElementById('<%= hidSelectedProjectIds.ClientID %>').value = JSON.stringify(projectIds);
                    return true;
                }
                
                return false;
            }
            
            // 篩選標籤功能
            function initTabs() {
                const tabs = document.querySelectorAll('.tab-item');
                tabs.forEach(function(tab, index) {
                    tab.addEventListener('click', function() {
                        // 檢查是否為禁用狀態
                        if (tab.classList.contains('tab-disabled')) {
                            return; // 禁用的標籤不處理點擊事件
                        }
                        
                        // 取得選中的階段
                        const stageText = tab.querySelector('.tab-title').textContent;
                        
                        // 設定隱藏欄位的值
                        document.getElementById('<%= hidSelectedStage.ClientID %>').value = stageText;
                        
                        // 觸發後端查詢
                        document.getElementById('<%= btnStageFilter.ClientID %>').click();
                    });
                });
            }
            
            // 更新標籤統計數字
            function updateTabCounts(counts) {
                const stages = ['資格審查', '領域審查/初審', '技術審查/複審', '決審核定'];
                const tabs = document.querySelectorAll('.tab-item');
                
                tabs.forEach(function(tab, index) {
                    if (index < stages.length) {
                        const countElement = tab.querySelector('.tab-count');
                        const stage = stages[index];
                        const count = counts[stage] || 0;
                        countElement.innerHTML = count + '<span class="tab-unit">件</span>';
                    }
                });
            }
            
            // 設定選中標籤的 active 狀態
            function setActiveTab(stageName) {
                const tabs = document.querySelectorAll('.tab-item');
                tabs.forEach(function(tab) {
                    const titleElement = tab.querySelector('.tab-title');
                    if (titleElement && titleElement.textContent === stageName) {
                        // 移除所有標籤的 active 狀態
                        tabs.forEach(function(t) {
                            if (!t.classList.contains('tab-disabled')) {
                                t.classList.remove('active');
                            }
                        });
                        // 為當前標籤添加 active 狀態
                        tab.classList.add('active');
                    }
                });
            }
            
            // 頁面載入完成後的初始化
            document.addEventListener('DOMContentLoaded', function() {
                bindCheckboxEvents();
                initTabs();
            });
        </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:HiddenField ID="hidSelectedProjectIds" runat="server" />
        <asp:HiddenField ID="hidSelectedStage" runat="server" />
        <asp:Button ID="btnStageFilter" runat="server" OnClick="btnStageFilter_Click" style="display: none;" />
        <div class="container">
            <!-- 導航區域 -->
            <div class="nav-breadcrumb">
                <span>🏠 首頁 / 計畫審查</span>
            </div>

            <!-- 標籤區域 -->
            <div class="tabs-container">
                <div class="tab-item active">
                    <div class="tab-title">資格審查</div>
                    <div class="tab-count">0<span class="tab-unit">件</span></div>
                </div>
                <div class="tab-item">
                    <div class="tab-title">領域審查/初審</div>
                    <div class="tab-count">0<span class="tab-unit">件</span></div>
                </div>
                <div class="tab-item">
                    <div class="tab-title">技術審查/複審</div>
                    <div class="tab-count">0<span class="tab-unit">件</span></div>
                </div>
                <div class="tab-item">
                    <div class="tab-title">決審核定</div>
                    <div class="tab-count">0<span class="tab-unit">件</span></div>
                </div>
                <div class="tab-item tab-disabled">
                    <div class="tab-title">計畫變更審核</div>
                    <div class="tab-count">0<span class="tab-unit">件</span></div>
                </div>
                <div class="tab-item tab-disabled">
                    <div class="tab-title">執行計畫審核</div>
                    <div class="tab-count">0<span class="tab-unit">件</span></div>
                </div>
            </div>

            <!-- 搜尋過濾區域 -->
            <div class="filter-section">
                <div class="filter-row">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="計畫編號、計畫名稱"></asp:TextBox>
                    
                    <asp:DropDownList ID="ddlYear" runat="server" CssClass="filter-select">
                        <asp:ListItem Text="114年" Value="114"></asp:ListItem>
                        <asp:ListItem Text="113年" Value="113"></asp:ListItem>
                        <asp:ListItem Text="112年" Value="112"></asp:ListItem>
                    </asp:DropDownList>
                    
                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="filter-select">
                        <asp:ListItem Text="全部類別" Value=""></asp:ListItem>
                        <asp:ListItem Text="科專" Value="SCI"></asp:ListItem>
                        <asp:ListItem Text="文化" Value="CUL"></asp:ListItem>
                        <asp:ListItem Text="學校民間" Value="EDC"></asp:ListItem>
                        <asp:ListItem Text="學校社團" Value="CLB"></asp:ListItem>
                    </asp:DropDownList>
                    
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="filter-select">
                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                        <asp:ListItem Text="審查中" Value="審查中"></asp:ListItem>
                        <asp:ListItem Text="補正補件" Value="補正補件"></asp:ListItem>
                        <asp:ListItem Text="逾期未補" Value="逾期未補"></asp:ListItem>
                        <asp:ListItem Text="未通過" Value="未通過"></asp:ListItem>
                        <asp:ListItem Text="通過" Value="通過"></asp:ListItem>
                    </asp:DropDownList>
                    
                    <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="filter-select">
                        <asp:ListItem Text="全部申請單位" Value=""></asp:ListItem>
                        <asp:ListItem Text="OO研究中心" Value="OO研究中心"></asp:ListItem>
                        <asp:ListItem Text="OO有限公司" Value="OO有限公司"></asp:ListItem>
                    </asp:DropDownList>
                    
                    <asp:DropDownList ID="ddlReviewer" runat="server" CssClass="filter-select">
                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                        <asp:ListItem Text="鄭海燕" Value="鄭海燕"></asp:ListItem>
                        <asp:ListItem Text="其他審查員" Value="其他"></asp:ListItem>
                    </asp:DropDownList>
                    
                    <asp:Button ID="btnSearch" runat="server" Text="🔍 查詢" CssClass="search-btn" OnClick="btnSearch_Click" />
                </div>
            </div>

            <!-- 資料表格區域 -->
            <div class="data-section">
                <div class="data-header">
                    <div class="record-info">
                        <asp:Literal ID="litRecordInfo" runat="server" Text="共0筆資料，第1/1頁"></asp:Literal>
                    </div>
                    <div class="pagination-info">
                        <span>每頁顯示</span>
                        <asp:DropDownList ID="ddlPageSize" runat="server" CssClass="pagination-select">
                            <asp:ListItem Text="20" Value="20"></asp:ListItem>
                            <asp:ListItem Text="50" Value="50"></asp:ListItem>
                            <asp:ListItem Text="100" Value="100"></asp:ListItem>
                        </asp:DropDownList>
                        <span>筆</span>
                    </div>
                </div>

                <table class="data-table">
                    <thead>
                        <tr>
                            <th><input type="checkbox" id="selectAllCheckbox" /></th>
                            <th>年度</th>
                            <th>類別</th>
                            <th>計畫編號</th>
                            <th>計畫名稱</th>
                            <th>申請單位</th>
                            <th>申請經費</th>
                            <th>狀態</th>
                            <th>補正截止期限</th>
                            <th>承辦人員</th>
                            <th>操作</th>
                        </tr>
                    </thead>
                    <tbody id="dataTableBody">
                        <!-- 動態資料將在這裡插入 -->
                        <tr>
                            <td colspan="11" style="text-align: center; padding: 20px;">載入中...</td>
                        </tr>
                    </tbody>
                </table>

                <!-- 底部操作按鈕 -->
                <div class="bottom-actions">
                    <asp:Button ID="btnBatchPass" runat="server" Text="🗸 批次通過，專入下一階段" CssClass="batch-btn btn-batch-pass" OnClientClick="return validateBatchPass();" OnClick="btnBatchPass_Click" />
                    <asp:Button ID="btnBatchReject" runat="server" Text="批次不通過，提送至申請者" CssClass="batch-btn btn-batch-reject" OnClientClick="return validateBatchReject();" OnClick="btnBatchReject_Click" />
                    <asp:Button ID="btnExport" runat="server" Text="⬇ 批次匯出審查表結果" CssClass="batch-btn btn-export" />
                </div>

                <!-- 分頁控制 -->
                <div class="pagination-controls">
                    <asp:Button ID="btnFirstPage" runat="server" Text="≪" CssClass="page-btn" />
                    <asp:Button ID="btnPrevPage" runat="server" Text="＜" CssClass="page-btn" />
                    
                    <span>到第</span>
                    <asp:DropDownList ID="ddlPageNumber" runat="server" CssClass="page-select">
                        <asp:ListItem Text="1" Value="1"></asp:ListItem>
                    </asp:DropDownList>
                    <span>頁</span>
                    
                    <asp:Button ID="btnNextPage" runat="server" Text="＞" CssClass="page-btn" />
                    <asp:Button ID="btnLastPage" runat="server" Text="≫" CssClass="page-btn" />
                </div>
            </div>
        </div>
    </form>
    
  
</body>
</html>