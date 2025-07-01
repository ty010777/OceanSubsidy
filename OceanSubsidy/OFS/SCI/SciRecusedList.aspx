<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SciRecusedList.aspx.cs" Inherits="OFS_SciAvoidList" Culture="zh-TW" UICulture="zh-TW" %>
<!DOCTYPE html>
<html lang="zh-TW">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>.NET WebForm UI 設計文件</title>
    <style>
        body {
            font-family: 'Microsoft YaHei', Arial, sans-serif;
            margin: 0;
            padding: 20px;
            background-color: #f5f5f5;
        }
        
        .container {
            max-width: 1200px;
            margin: 0 auto;
            background: white;
            padding: 30px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        
        h1 {
            color: #2c3e50;
            border-bottom: 3px solid #3498db;
            padding-bottom: 10px;
            margin-bottom: 30px;
        }
        
        h2 {
            color: #34495e;
            margin-top: 40px;
            margin-bottom: 20px;
            border-left: 4px solid #3498db;
            padding-left: 15px;
        }
        
        h3 {
            color: #2c3e50;
            margin-top: 30px;
            margin-bottom: 15px;
        }
        
        .section {
            margin-bottom: 40px;
            padding: 20px;
            background: #f8f9fa;
            border-radius: 5px;
            border-left: 4px solid #3498db;
        }
        
        .form-preview {
            border: 2px solid #ddd;
            border-radius: 8px;
            padding: 20px;
            margin: 20px 0;
            background: white;
        }
        
        .form-header {
            background: linear-gradient(90deg, #3498db, #2980b9);
            color: white;
            padding: 12px 20px;
            margin: -20px -20px 20px -20px;
            border-radius: 6px 6px 0 0;
            font-weight: bold;
        }
        
        .form-table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 15px;
        }
        
        .form-table th {
            background: #ecf0f1;
            padding: 12px;
            text-align: left;
            border: 1px solid #bdc3c7;
            font-weight: bold;
            color: #2c3e50;
        }
        
        .form-table td {
            padding: 12px;
            border: 1px solid #bdc3c7;
        }
        
        .form-table input[type="text"], .form-table select {
            width: 100%;
            padding: 8px;
            border: 1px solid #bdc3c7;
            border-radius: 4px;
            box-sizing: border-box;
        }
        
        .trl-select {
            background-color: white;
            width: 100%;
            margin-bottom: 5px;
        }
        
        .form-table td div {
            margin-bottom: 8px;
        }
        
        .form-table td div:last-child {
            margin-bottom: 0;
        }
        
        .required {
            color: #e74c3c;
            font-weight: bold;
        }
        
        .checkbox-row {
            margin-bottom: 15px;
        }
        
        .checkbox-row input[type="checkbox"] {
            margin-right: 8px;
        }
        
        .action-buttons {
            text-align: center;
            margin-top: 20px;
        }
        
        .btn {
            padding: 10px 20px;
            margin: 0 5px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
        }
        
        .btn-primary {
            background: #3498db;
            color: white;
        }
        
        .btn-danger {
            background: #e74c3c;
            color: white;
        }
        
        .btn-success {
            background: #27ae60;
            color: white;
        }
        
        .code-block {
            background: #2c3e50;
            color: #ecf0f1;
            padding: 15px;
            border-radius: 5px;
            margin: 15px 0;
            overflow-x: auto;
            font-family: 'Consolas', 'Monaco', monospace;
            font-size: 13px;
        }
        
        .property-table {
            width: 100%;
            border-collapse: collapse;
            margin: 15px 0;
        }
        
        .property-table th,
        .property-table td {
            padding: 10px;
            text-align: left;
            border: 1px solid #ddd;
        }
        
        .property-table th {
            background: #f8f9fa;
            font-weight: bold;
        }
        
        .note {
            background: #fff3cd;
            border: 1px solid #ffeaa7;
            border-radius: 4px;
            padding: 15px;
            margin: 15px 0;
        }
        
        .warning {
            background: #f8d7da;
            border: 1px solid #f5c6cb;
            border-radius: 4px;
            padding: 15px;
            margin: 15px 0;
        }
    </style>
</head>

<body>

    <div class="container">
        <div class="links">
          <a href="http://localhost:50929/OFS/SciFunding.aspx?ProjectID=114SCI0006" target="_blank">📄 計畫經費填報</a>
          <a href="http://localhost:50929/OFS/SciApplication.aspx?ProjectID=114SCI0006" target="_blank">📝 科專申請資料</a>
          <a href="http://localhost:50929/OFS/SciOutcomes.aspx?ProjectID=114SCI0006" target="_blank">📊 成果與績效</a>
          <a href="http://localhost:50929/OFS/SciRecusedList.aspx?ProjectID=114SCI0006" target="_blank">📊 其他</a>
        
        </div>
        <h1>.NET WebForm UI 設計文件</h1>
        <p><strong>專案名稱：</strong>建議迴避委員會委員清單管理系統</p>
        <p><strong>文件版本：</strong>1.0</p>
        <p><strong>建立日期：</strong>2025年6月</p>
        
        <div class="section">
            <h2>1. 系統概述</h2>
            <p>本系統用於管理建議迴避委員會的委員清單，提供委員資料的新增、編輯、刪除等功能。系統採用 .NET WebForm 架構開發，提供直觀的網頁操作介面。</p>
        </div>
        
        <div class="section">
            <h2>2. 頁面結構設計</h2>
            
            <h3>2.1 主要頁面 (AvoidanceCommitteeList.aspx)</h3>
            <form id="form1" runat="server">
            <div class="form-preview">
                <div class="form-header">
                    🏢 建議迴避委員會委員清單
                </div>
                
                <div class="checkbox-row">
                    <asp:CheckBox ID="chkNoAvoidance" runat="server" Text="無需迴避之審查委員" />
                </div>
                
                <table class="form-table">
                    <thead>
                        <tr>
                            <th><span class="required">*</span>姓名</th>
                            <th><span class="required">*</span>任職單位</th>
                            <th><span class="required">*</span>職稱</th>
                            <th><span class="required">*</span>應迴避之具體理由及事證</th>
                            <th style="width: 100px;">操作</th>
                        </tr>
                    </thead>
                    <tbody id="committeeTableBody">
                        <tr>
                            <td><input type="text" name="committeeName" value="沈大大" /></td>
                            <td><input type="text" name="committeeUnit" /></td>
                            <td><input type="text" name="committeePosition" /></td>
                            <td><input type="text" name="committeeReason" /></td>
                            <td>
                                <button type="button" class="btn btn-success">+</button>
                            </td>
                        </tr>
                        <tr>
                            <td><input type="text" name="committeeName" /></td>
                            <td><input type="text" name="committeeUnit" /></td>
                            <td><input type="text" name="committeePosition" /></td>
                            <td><input type="text" name="committeeReason" /></td>
                            <td>
                                <button type="button" class="btn btn-danger">🗑</button>
                            </td>
                        </tr>
                    </tbody>
                </table>
                
         
            </div>
            <div class="form-preview">
                <div class="form-header">
                    🏢 技術能力
                </div>
                
                <table class="form-table">
                    <thead>
                        <tr>
                            <th><span class="required">*</span>研發技術項目</th>
                            <th><span class="required">*</span>TRL層級</th>
                            <th><span class="required">*</span>技術進程概述</th>
                            <th style="width: 100px;">操作</th>
                        </tr>
                    </thead>
                    <tbody id="techTableBody">
                        <tr>
                            <td><input type="text" name="techItem" /></td>
                            <td>
                                <div>計畫執行前：<select name="trlPlanLevel" class="trl-select"><option value="">請選擇</option></select></div>
                                <div>計畫執行後：<select name="trlTrackLevel" class="trl-select"><option value="">請選擇</option></select></div>
                            </td>
                            <td><input type="text" name="techProcess" /></td>
                            <td>
                                <button type="button" class="btn btn-success">+</button>
                            </td>
                        </tr>
                        <tr>
                            <td><input type="text" name="techItem" /></td>
                            <td>
                                <div>計畫執行前：<select name="trlPlanLevel" class="trl-select"><option value="">請選擇</option></select></div>
                                <div>計畫執行後：<select name="trlTrackLevel" class="trl-select"><option value="">請選擇</option></select></div>
                            </td>
                            <td><input type="text" name="techProcess" /></td>
                            <td>
                                <button type="button" class="btn btn-danger">🗑</button>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="action-buttons">
                <asp:Button ID="btnSave" runat="server" Text="儲存" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                <%-- <asp:Button ID="btnAddCommittee" runat="server" Text="新增委員" CssClass="btn btn-success" OnClick="btnAddCommittee_Click" /> --%>
            </div>
            </form>
            <div class="note">
                <strong>說明：</strong>
                <ol>
                    <li>請填列與委員可能人有利益衝突受審查委員建議迴避者，以利審查作業順利完成檢證。以符公平審查原則。</li>
                    <li>若無建議迴避之審查委員，請勾選「無需迴避之審查委員」。</li>
                    <li>建議迴避之審查委員，請務必具體陳明理由及事證，否則不予以採納。</li>
                </ol>
            </div>
        </div>
        
    </div>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script>
        // 全域變數來儲存 TRL 選項和現有資料
        window.trlOptions = window.trlOptions || [];
        window.existingData = window.existingData || { recusedData: [], techData: [] };
        
        $(document).ready(function() {
             // 載入現有資料
               loadExistingData();
            // 為「+」按鈕添加點擊事件
            $(document).on('click', '.btn-success', function(e) {
                e.preventDefault();
                if ($(this).text().trim() === '+') {
                    addNewRow($(this));
                }
            });
            
            // 為「🗑」按鈕添加點擊事件
            $(document).on('click', '.btn-danger', function(e) {
                e.preventDefault();
                if ($(this).text().trim() === '🗑') {
                    deleteRow($(this));
                }
            });
        });
        
     
        
        function generateTrlOptionsHtml(selectedValue) {
            var html = '<option value="">請選擇</option>';
            if (window.trlOptions && window.trlOptions.length > 0) {
                for (var i = 0; i < window.trlOptions.length; i++) {
                    var selected = (selectedValue && selectedValue === window.trlOptions[i].Code) ? ' selected' : '';
                    html += '<option value="' + window.trlOptions[i].Code + '"' + selected + '>' + 
                           window.trlOptions[i].Code + ' - ' + window.trlOptions[i].Descname + '</option>';
                }
            }
            return html;
        }
        
       
        
        function loadExistingData() {
            if (!window.existingData) {
                // 如果沒有資料，確保表格有空行
                ensureEmptyRows();
                return;
            }
            
            // 載入委員迴避清單資料
            if (window.existingData.recusedData && window.existingData.recusedData.length > 0) {
                loadCommitteeData(window.existingData.recusedData);
            } else {
                // 沒有委員資料時，顯示空行
                var $tbody = $('#committeeTableBody');
                $tbody.empty();
                addEmptyCommitteeRow($tbody);
            }
            
            // 載入技術成熟度資料
            if (window.existingData.techData && window.existingData.techData.length > 0) {
                loadTechData(window.existingData.techData);
            } else {
                // 沒有技術資料時，顯示空行
                var $tbody = $('#techTableBody');
                $tbody.empty();
                addEmptyTechRow($tbody);
            }
        }
        
        function ensureEmptyRows() {
            // 確保委員表格有空行
            var $committeeBody = $('#committeeTableBody');
            if ($committeeBody.find('tr').length === 0) {
                addEmptyCommitteeRow($committeeBody);
            }
            
            // 確保技術表格有空行
            var $techBody = $('#techTableBody');
            if ($techBody.find('tr').length === 0) {
                addEmptyTechRow($techBody);
            }
        }
        
        function loadCommitteeData(data) {
            var $tbody = $('#committeeTableBody');
            $tbody.empty(); // 清空現有資料
            
            if (data.length === 0) {
                // 如果沒有資料，至少保留一行空的
                addEmptyCommitteeRow($tbody);
                return;
            }
            
            // 為每筆資料創建一行
            for (var i = 0; i < data.length; i++) {
                var item = data[i];
                var isFirst = (i === 0);
                var buttonHtml = isFirst ? 
                    '<button type="button" class="btn btn-success">+</button>' :
                    '<button type="button" class="btn btn-danger">🗑</button>';
                
                var rowHtml = `
                    <tr>
                        <td><input type="text" name="committeeName" value="${escapeHtml(item.committeeName || '')}" /></td>
                        <td><input type="text" name="committeeUnit" value="${escapeHtml(item.committeeUnit || '')}" /></td>
                        <td><input type="text" name="committeePosition" value="${escapeHtml(item.committeePosition || '')}" /></td>
                        <td><input type="text" name="committeeReason" value="${escapeHtml(item.committeeReason || '')}" /></td>
                        <td>${buttonHtml}</td>
                    </tr>`;
                
                $tbody.append(rowHtml);
            }
        }
        
        function loadTechData(data) {
            console.log('Loading tech data:', data);
            var $tbody = $('#techTableBody');
            $tbody.empty(); // 清空現有資料
            
            if (data.length === 0) {
                // 如果沒有資料，至少保留一行空的
                addEmptyTechRow($tbody);
                return;
            }
            
            // 為每筆資料創建一行
            for (var i = 0; i < data.length; i++) {
                var item = data[i];
                console.log('Processing item:', item);
                console.log('TRL Plan Level:', item.trlPlanLevel);
                console.log('TRL Track Level:', item.trlTrackLevel);
                
                var isFirst = (i === 0);
                var buttonHtml = isFirst ? 
                    '<button type="button" class="btn btn-success">+</button>' :
                    '<button type="button" class="btn btn-danger">🗑</button>';
                
                // 為每個下拉選單生成帶有預選值的選項
                var trlPlanOptionsHtml = generateTrlOptionsHtml(item.trlPlanLevel);
                var trlTrackOptionsHtml = generateTrlOptionsHtml(item.trlTrackLevel);
                
               var $row = $('<tr>');
               $row.append(`<td><input type="text" name="techItem" value="${escapeHtml(item.techItem || '')}" /></td>`);
               $row.append(`<td>
                   <div>計畫執行前：<select name="trlPlanLevel" class="trl-select"></select></div>
                   <div>計畫執行後：<select name="trlTrackLevel" class="trl-select"></select></div>
               </td>`);
               $row.append(`<td><input type="text" name="techProcess" value="${escapeHtml(item.techProcess || '')}" /></td>`);
               $row.append(`<td>${buttonHtml}</td>`);
               
               // 正確將 HTML 塞進 select（而非當成文字）
               $row.find('select[name="trlPlanLevel"]').html(trlPlanOptionsHtml);
               $row.find('select[name="trlTrackLevel"]').html(trlTrackOptionsHtml);
               
               // 加入到表格
               $tbody.append($row);

            }
        }
        
        function addEmptyCommitteeRow($tbody) {
            var rowHtml = `
                <tr>
                    <td><input type="text" name="committeeName" /></td>
                    <td><input type="text" name="committeeUnit" /></td>
                    <td><input type="text" name="committeePosition" /></td>
                    <td><input type="text" name="committeeReason" /></td>
                    <td><button type="button" class="btn btn-success">+</button></td>
                </tr>`;
            $tbody.append(rowHtml);
        }
        
        function addEmptyTechRow($tbody) {
            var trlOptionsHtml = generateTrlOptionsHtml();
            var rowHtml = `
                <tr>
                    <td><input type="text" name="techItem" /></td>
                    <td>
                        <div>計畫執行前：<select name="trlPlanLevel" class="trl-select">${trlOptionsHtml}</select></div>
                        <div>計畫執行後：<select name="trlTrackLevel" class="trl-select">${trlOptionsHtml}</select></div>
                    </td>
                    <td><input type="text" name="techProcess" /></td>
                    <td><button type="button" class="btn btn-success">+</button></td>
                </tr>`;
            $tbody.append(rowHtml);
        }
        
        function escapeHtml(text) {
            if (!text) return '';
            return text.replace(/&/g, '&amp;')
                      .replace(/</g, '&lt;')
                      .replace(/>/g, '&gt;')
                      .replace(/"/g, '&quot;')
                      .replace(/'/g, '&#039;');
        }
        
        function addNewRow($btn) {
            var $table = $btn.closest('table');
            var $tbody = $table.find('tbody');
            var isCommitteeTable = $tbody.attr('id') === 'committeeTableBody';
            
            var newRowHtml = '';
            if (isCommitteeTable) {
                // 委員清單表格的新行
                newRowHtml = `
                    <tr>
                        <td><input type="text" name="committeeName" /></td>
                        <td><input type="text" name="committeeUnit" /></td>
                        <td><input type="text" name="committeePosition" /></td>
                        <td><input type="text" name="committeeReason" /></td>
                        <td>
                            <button type="button" class="btn btn-danger">🗑</button>
                        </td>
                    </tr>`;
            } else {
                // 技術能力表格的新行
                var trlOptionsHtml = generateTrlOptionsHtml();
                newRowHtml = `
                    <tr>
                        <td><input type="text" name="techItem" /></td>
                        <td>
                            <div>計畫執行前：<select name="trlPlanLevel" class="trl-select">${trlOptionsHtml}</select></div>
                            <div>計畫執行後：<select name="trlTrackLevel" class="trl-select">${trlOptionsHtml}</select></div>
                        </td>
                        <td><input type="text" name="techProcess" /></td>
                        <td>
                            <button type="button" class="btn btn-danger">🗑</button>
                        </td>
                    </tr>`;
            }
            
            // 在表格最後添加新行
            $tbody.append(newRowHtml);
        }
        
        function deleteRow($btn) {
            var $row = $btn.closest('tr');
            var $tbody = $row.closest('tbody');
            
            // 至少保留一行
            if ($tbody.find('tr').length <= 1) {
                alert('至少需要保留一行資料');
                return;
            }
            
            $row.remove();
        }
    </script>
</body>