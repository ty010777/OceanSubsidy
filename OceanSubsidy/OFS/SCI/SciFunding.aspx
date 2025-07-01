<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SciFunding.aspx.cs" Inherits="OFS_SciFunding" Culture="zh-TW" UICulture="zh-TW" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>海洋科技研發人員人事費明細表</title>
    <style>
        body {
            font-family: "Microsoft JhengHei", Arial, sans-serif;
            margin: 20px;
            background-color: #f5f5f5;
        }
        
        .container {
            background-color: white;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        
        .header {
            background-color: #4a90e2;
            color: white;
            padding: 12px 20px;
            margin: -20px -20px 20px -20px;
            border-radius: 8px 8px 0 0;
            font-size: 16px;
            font-weight: bold;
        }
        
        .header-icon {
            display: inline-block;
            width: 20px;
            height: 20px;
            background-color: #2c5aa0;
            margin-right: 10px;
            border-radius: 3px;
        }
        
        .main-table {
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 20px;
            font-size: 14px;
        }
        
        .main-table th {
            background-color: #e8f4fd;
            border: 1px solid #ccc;
            padding: 8px;
            text-align: center;
            font-weight: bold;
        }
        
        .main-table td {
            border: 1px solid #ccc;
            padding: 8px;
            text-align: center;
        }
        
        .main-table .name-cell {
            text-align: left;
            padding-left: 12px;
        }
        
        .main-table .total-row {
            background-color: #e8f4fd;
            font-weight: bold;
        }
        
        .main-table .total-amount {
            color: #d9534f;
            font-weight: bold;
        }
        
        .checkbox-cell {
            width: 40px;
        }
        
        .name-cell {
            width: 80px;
        }
        
        .position-cell {
            width: 150px;
        }
        
        .salary-cell {
            width: 100px;
            text-align: right;
        }
        
        .months-cell {
            width: 80px;
        }
        
        .total-cell {
            width: 100px;
            text-align: right;
        }
        
        .action-cell {
            width: 60px;
        }
        
        .dropdown-list {
            width: 100%;
            padding: 4px;
            border: 1px solid #ccc;
            border-radius: 3px;
        }
        
        .textbox {
            width: 90%;
            padding: 4px;
            border: 1px solid #ccc;
            border-radius: 3px;
            text-align: right;
        }
        
        .textbox[rows] {
            text-align: left;
        }
        
        .multiline-textbox {
            text-align: left;
            resize: vertical;
        }
        
        .btn-delete {
            background-color: #5bc0de;
            color: white;
            border: none;
            padding: 4px 8px;
            border-radius: 3px;
            cursor: pointer;
            font-size: 12px;
        }
        
        .btn-delete:hover {
            background-color: #31b0d5;
        }
        
        .btn-add {
            background-color: #5bc0de;
            color: white;
            border: none;
            padding: 4px 8px;
            border-radius: 3px;
            cursor: pointer;
            font-size: 16px;
            margin-left: 5px;
        }
        
        .btn-add:hover {
            background-color: #31b0d5;
        }
        
        .notes {
            margin-top: 20px;
            font-size: 13px;
            line-height: 1.6;
        }
        
        .notes ul {
            padding-left: 20px;
        }
        
        .notes li {
            margin-bottom: 8px;
        }
    </style>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css">
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
   
    <!-- 加上 script：初始化 flatpickr -->
    <script type="text/javascript">
        window.addEventListener('DOMContentLoaded', function () {
            flatpickr(".flatpickr-date", {
                dateFormat: "Y-m-d", // 實際值：西元
                locale: "zh",
                onChange: function (selectedDates, dateStr, instance) {
                    const date = selectedDates[0];
                    if (date) {
                        const rocYear = date.getFullYear() - 1911;
                        const formatted = `民國${rocYear}年${date.getMonth() + 1}月${date.getDate()}日`;
                        instance._input.value = formatted;
                    }
                },
                onOpen: function (selectedDates, dateStr, instance) {
                    instance._input.value = ""; // 避免再次開啟時顯示舊格式
                }
            });
        });
    </script>
<script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
<script  type="text/javascript">

//region 1.海洋科技研發人員人事費明細表 的JS
window.onload = function () {
    
    const P_ddl = document.getElementById('ddlPerson1');
    ddlPersonOptions.forEach(option => {
        const opt = document.createElement('option');
        opt.value = option.value;
        opt.textContent = option.text;
        P_ddl.appendChild(opt);
    });
     // 新增 onchange 事件：切換選項時，將 salary 欄位清為 0
        P_ddl.addEventListener("change", function () {
            const salaryInput = document.getElementById(`personSalary1`);
            if (salaryInput) {
                salaryInput.value = 0;
                calculateAndUpdateTotal(); // 如有需要可即時重新計算
            }
        });
    const M_ddl = document.getElementById('MaterialUnit1');
    ddlMaterialOptions.forEach(option => {
        const opt = document.createElement('option');
        opt.value = option.value;
        opt.textContent = option.text;
        M_ddl.appendChild(opt);
    });
    M_ddl.addEventListener("change", function () {
        const salaryInput = document.getElementById(`MaterialUnitPrice1`);
        if (salaryInput) {
            salaryInput.value = 0;
            calculateMaterial(); // 如有需要可即時重新計算
        }
    });
     const O_ddl = document.getElementById('otherJobTitle1');
        ddlOtherOptions.forEach(option => {
            const opt = document.createElement('option');
            opt.value = option.value;
            opt.textContent = option.text;
            O_ddl.appendChild(opt);
        });
        
        // 為第一個職稱下拉選單加入 onchange 事件
        O_ddl.addEventListener("change", function () {
            calculateOther();
        });
    
};
function P_deleteRow(button) {
    const confirmed = confirm('確定要刪除此行資料嗎？');
    if (confirmed) {
        // 找到該按鈕所在的 <tr>
        const row = button.closest('tr');
        if (row) {
            row.remove();
            calculateAndUpdateTotal(); // 如有總計等更新
            // renumberRows(); // 重新編號其餘行
        } else {
            alert('找不到要刪除的行');
        }
    }
}
 
  function checkSalaryLimit(rowIndex) {
      const salaryInput = document.getElementById(`personSalary${rowIndex}`);
      const ddl = document.getElementById(`ddlPerson${rowIndex}`);
      const selectedCode = ddl.value;
      const salary = parseInt(salaryInput.value, 10);
  
      const selectedItem = ddlPersonOptions.find(x => x.value === selectedCode);
      if (selectedItem && salary > selectedItem.maxLimit) {
          alert(`輸入金額 ${salary} 超過上限：${selectedItem.maxLimit}`);
          salaryInput.value = selectedItem.maxLimit; // 自動修正為上限
      }
  }
  function calculateAndUpdateTotal() {
      let total = 0;
      const table = document.querySelector('.person tbody');
      const dataRows = table.querySelectorAll('tr:not(.total-row)');
      
      dataRows.forEach((row, index) => {
          // 動態尋找每行的薪資和月份輸入框
            const salaryInput = row.cells[3].querySelector('input'); // 第4欄是 Salary
            const monthsInput = row.cells[4].querySelector('input'); // 第5欄是 Months
            const totalCell = row.cells[5]; // 第6欄是小計
          
          if (salaryInput && monthsInput) {
              const salary = parseFloat(salaryInput.value.replace(/,/g, '')) || 0;
              const months = parseFloat(monthsInput.value) || 0;
              const rowTotal = salary * months;
              
              // 更新該行的小計顯示
              if (totalCell) {
                  totalCell.textContent = rowTotal.toLocaleString();
              }
              
              total += rowTotal;
          }
      });
      
      // 更新總計顯示
      updatePersonTotal(total);
  }
  

  function updatePersonTotal(total) {
      // 更新總計顯示
      const totalCell = document.getElementById('PersonTotal');
      if (totalCell) {
          totalCell.textContent = total.toLocaleString();
      }
      // 更新經費總表
      updateBudgetSummary();
  }
        // 新增行功能（動態新增）
        function P_addNewRow() {
            const table = document.querySelector('.person tbody');
            const totalRow = table.querySelector('.total-row');
            
            // 建立新行
            const newRow = document.createElement('tr');
            const rowCount = table.children.length; // 包含總計行
            const ddlSelect = document.createElement('select');
                ddlSelect.className = "dropdown-list";
                ddlSelect.id = `ddlPerson${rowCount}`;
            
                // 加入選項
                ddlPersonOptions.forEach(option => {
                    const opt = document.createElement('option');
                    opt.value = option.value;
                    opt.textContent = option.text;
                    ddlSelect.appendChild(opt);
                });
                 // 新增 onchange 事件：切換選項時，將 salary 欄位清為 0
                    ddlSelect.addEventListener("change", function () {
                        const salaryInput = document.getElementById(`personSalary${rowCount}`);
                        if (salaryInput) {
                            salaryInput.value = 0;
                            calculateAndUpdateTotal(); // 如有需要可即時重新計算
                        }
                    });

            newRow.innerHTML = `
                <td class="name-cell"><input type="text" id="personName${rowCount}" class="textbox"></td>
                <td><input type="checkbox" id="stay${rowCount}" /></td>
                 <td></td> <!-- select 會動態插入這格 -->
                <td><input type="text" class="textbox" id="personSalary${rowCount}" value="0" onblur="checkSalaryLimit(${rowCount}); calculateAndUpdateTotal()"></td>
                <td><input type="text" class="textbox" id="personMonths${rowCount}" value="0" onblur="calculateAndUpdateTotal()"></td>
                <td class="salary-cell">0</td>
                <td>
                    <button type="button" class="btn-delete" onclick="P_deleteRow(this)">🗑</button>
                    <button type="button" class="add-btn" onclick="P_addNewRow()">+</button>
                </td>
            `;
            // 在總計行之前插入新行
            table.insertBefore(newRow, totalRow);
            // 將 select 插入第 3 個 <td>
             newRow.children[2].appendChild(ddlSelect);

        }

//endregion
//region 2.消耗性器材及原材料費 的JS

  function M_deleteRow(button) {
        const confirmed = confirm('確定要刪除此行資料嗎？');
        if (confirmed) {
            // 找到該按鈕所在的 <tr>
            const row = button.closest('tr');
            if (row) {
                row.remove();
              calculateMaterial();
                // renumberRows(); // 重新編號其餘行
            } else {
                alert('找不到要刪除的行');
            }
        }
    }
  function calculateMaterial() {
      let total = 0;
      const table = document.querySelector('.Material tbody');
      const dataRows = table.querySelectorAll('tr:not(.total-row)');
      
      dataRows.forEach((row, index) => {
          // 動態尋找每行的薪資和月份輸入框
            const NumInput = row.cells[3].querySelector('input'); // 第4欄是 Num
            const unitPriceInput = row.cells[4].querySelector('input'); // 第5欄是 unitPrice
            const totalCell = row.cells[5]; // 第6欄是小計
          
          if (NumInput && unitPriceInput) {
              const Num = parseFloat(NumInput.value.replace(/,/g, '')) || 0;
              const unitPrice = parseFloat(unitPriceInput.value) || 0;
              const rowTotal = Num * unitPrice;
              
              // 更新該行的小計顯示
              if (totalCell) {
                  totalCell.textContent = rowTotal.toLocaleString();
              }
              
              total += rowTotal;
          }
      });
      
      // 更新總計顯示
      updateTotalMaterialTotal(total);
  }
  
  function updateTotalMaterialTotal(total) {
      // 更新總計顯示
      const totalCell = document.getElementById('MaterialTotal') ;
      if (totalCell) {
          totalCell.textContent = total.toLocaleString();
      }
      // 更新經費總表
      updateBudgetSummary();
  }

function checkMaterialLimit(rowIndex) {
        const MaterialUnitPriceInput = document.getElementById(`MaterialUnitPrice${rowIndex}`);
        const ddl = document.getElementById(`MaterialUnit${rowIndex}`);
        const selectedCode = ddl.value;
        const MaterialUnitPrice = parseInt(MaterialUnitPriceInput.value, 10);
    
        const selectedItem = ddlMaterialOptions.find(x => x.value === selectedCode);
        if (selectedItem && MaterialUnitPrice > selectedItem.maxLimit && selectedItem.maxLimit != 0) {
            alert(`輸入金額 ${MaterialUnitPrice} 超過上限：${selectedItem.maxLimit}`);
            MaterialUnitPriceInput.value = selectedItem.maxLimit; // 自動修正為上限
        }
    }


// 新增行功能（材料費）
function M_addNewRow() {
    const table = document.querySelector('.Material tbody');
    const totalRow = table.querySelector('.total-row');
    
    // 建立新行
    const newRow = document.createElement('tr');
    const rowCount = table.children.length; // 包含總計行
    const ddlSelect = document.createElement('select');
                    ddlSelect.className = "dropdown-list";
                    ddlSelect.id = `MaterialUnit${rowCount}`;
                
                    // 加入選項
                    ddlMaterialOptions.forEach(option => {
                        const opt = document.createElement('option');
                        opt.value = option.value;
                        opt.textContent = option.text;
                        ddlSelect.appendChild(opt);
                    });
                     // 新增 onchange 事件：切換選項時，將 salary 欄位清為 0
                    ddlSelect.addEventListener("change", function () {
                        const MaterialUnitPriceInput = document.getElementById(`MaterialUnitPrice${rowCount}`);
                        if (MaterialUnitPriceInput) {
                            MaterialUnitPriceInput.value = 0;
                            calculateMaterial(); // 如有需要可即時重新計算
                        }
                    });
    newRow.innerHTML = `
            <td class="name-cell"><input type="text" id="MaterialName${rowCount}" class="textbox" /></td>
            <td class="name-cell"><input type="text" id="MaterialDescription${rowCount}" class="textbox" /></td>
            <td>
            </td>
            <td><input type="text" id="MaterialNum${rowCount}" class="textbox" onblur="calculateMaterial()" /></td>
            <td><input type="text" id="MaterialUnitPrice${rowCount}" class="textbox" onblur="checkMaterialLimit(${rowCount});calculateMaterial()" /></td>
            <td class="salary-cell"></td>
            <td>
                <button type="button" class="btn-delete" onclick="M_deleteRow(this)">🗑</button>
                <button type="button" class="add-btn" onclick="M_addNewRow()">+</button></td>
        
    `;
    // 在總計行之前插入新行
    table.insertBefore(newRow, totalRow);
    newRow.children[2].appendChild(ddlSelect);
}

//endregion
//region 3. 技術移轉、委託研究或驗證費
    
  // 金額格式化 + 合計計算
    function calculateResearch() {
        let total = 0;
        document.querySelectorAll('.money').forEach(input => {
            let raw = input.value.replace(/,/g, '');
            let val = parseInt(raw) || 0;
            input.value = val.toLocaleString(); // 金額千分位
            total += val;
        });

        document.getElementById("ResearchFeesTotal").innerText = total.toLocaleString();
        // 更新經費總表
        updateBudgetSummary();
    }


function R_DeleteRow(rowNumber) {
  const confirmed = confirm('確定要刪除此行資料嗎？');
       if (confirmed) {
           // 找到要刪除的行
           const table = document.querySelector('.ResearchFees tbody');
           const rows = table.querySelectorAll('tr:not(.total-row)');
           
           // 根據行號找到對應的行（rowNumber從1開始，陣列索引從0開始）
           const targetRow = rows[rowNumber - 1];
           
           if (targetRow) {
               // 移除整個行
               targetRow.remove();
               calculateResearch();
               R_renumberRows();
           } else {
               alert('找不到要刪除的行');
           }
       }
}
function R_renumberRows (){
    const table = document.querySelector('.ResearchFees tbody');
    const rows = table.querySelectorAll('tr:not(.total-row)');
    
    rows.forEach((row, index) => {
        const newRowNumber = index + 1;
         // 更新品名的ID
        const ResearchFeesNameInput = row.querySelector('[id*="ResearchFeesName"]');
        if (ResearchFeesNameInput) {
            ResearchFeesNameInput.id = `ResearchFeesName${newRowNumber}`;
        }
      
        const ResearchFeesReason = row.querySelector('[id*="ResearchFeesReason"]');
        if (ResearchFeesReason) {
            ResearchFeesReason.id = `ResearchFeesReason${newRowNumber}`;
        } 
        
        const ResearchFeesArea = row.querySelector('[id*="ResearchFeesArea"]');
        if (ResearchFeesArea) {
            ResearchFeesArea.id = `ResearchFeesArea${newRowNumber}`;
        }
        const ResearchFeesDays = row.querySelector('[id*="ResearchFeesDays"]');
        if (ResearchFeesDays) {
            ResearchFeesDays.id = `ResearchFeesDays${newRowNumber}`;
        }  
        const ResearchFeesPeople = row.querySelector('[id*="ResearchFeesPeople"]');
        if (ResearchFeesPeople) {
            ResearchFeesPeople.id = `ResearchFeesPeople${newRowNumber}`;
        }
        const ResearchFeesPrice = row.querySelector('[id*="ResearchFeesPrice"]');
        if (ResearchFeesPrice) {
            ResearchFeesPrice.id = `ResearchFeesPrice${newRowNumber}`;
        }
        // 更新刪除按鈕的onclick事件
        const deleteButton = row.querySelector('.btn-delete');
        if (deleteButton) {
            deleteButton.onclick = () => R_deleteRow(newRowNumber);
        }
    });
}
    window.addEventListener('DOMContentLoaded', calculateResearch);

//endregion
//region  4. 國內差旅費
function calculateTravel() {
        let total = 0;
        const prices = document.querySelectorAll('.travel .price');

        prices.forEach(priceInput => {
            const raw = priceInput.value.replace(/,/g, '');
            const value = parseInt(raw) || 0;
            total += value;

            // 如果不是空白就加上千分位
            priceInput.value = value > 0 ? value.toLocaleString() : '';
        });

        document.getElementById('travelTotal').innerText = total.toLocaleString();
        // 更新經費總表
        updateBudgetSummary();
    }

    function T_DeleteRow(button) {
            const confirmed = confirm('確定要刪除此行資料嗎？');
        if (confirmed) {
            // 找到該按鈕所在的 <tr>
            const row = button.closest('tr');
            if (row) {
                row.remove();
                calculateAndUpdateTotal(); // 如有總計等更新
            } else {
                alert('找不到要刪除的行');
            }
        }
    }
    function T_addRow() {
       const table = document.querySelector('.travel tbody');
       const totalRow = table.querySelector('.total-row');
      
      // 建立新行
        const newRow = document.createElement('tr');
        const rowCount = table.children.length; // 包含總計行
        
        newRow.innerHTML = `
            <td><input type="text" ID="travelReason${rowCount}" class="textbox" /></td>
            <td><input type="text" ID="travelArea${rowCount}" class="textbox" /></td>
            <td><input type="text" ID="travelDays${rowCount}" class="textbox days"/></td>
            <td><input type="text" ID="travelPeople${rowCount}" class="textbox people" </td>
            <td><input type="text" ID="travelPrice${rowCount}" class="textbox price" onblur="calculateTravel()" /></td>
            <td><button type="button" class="btn-delete" onclick="T_DeleteRow(this)">🗑</button></td>
        `;
        table.insertBefore(newRow, totalRow);
    }

    
    // function T_renumberRows (){
    //     const table = document.querySelector('.travel tbody');
    //     const rows = table.querySelectorAll('tr:not(.total-row)');
    //    
    //     rows.forEach((row, index) => {
    //         const newRowNumber = index + 1;
    //          // 更新品名的ID
    //         const travelReason = row.querySelector('[id*="travelReason"]');
    //         if (travelReason) {
    //             travelReason.id = `travelReason${newRowNumber}`;
    //         }
    //
    //         const travelArea = row.querySelector('[id*="travelArea"]');
    //         if (travelArea) {
    //             travelArea.id = `travelArea${newRowNumber}`;
    //         }
    //                  
    //         const travelDays = row.querySelector('[id*="travelDays"]');
    //         if (travelDays) {
    //             travelDays.id = `travelDays${newRowNumber}`;
    //         } 
    //        
    //         const travelPeople = row.querySelector('[id*="travelPeople"]');
    //         if (travelPeople) {
    //             travelPeople.id = `travelPeople${newRowNumber}`;
    //         }  
    //         const travelPrice = row.querySelector('[id*="travelPrice"]');
    //         if (travelPrice) {
    //             travelPrice.id = `travelPrice${newRowNumber}`;
    //         }
    //
    //         // 更新刪除按鈕的onclick事件
    //         const deleteButton = row.querySelector('.btn-delete');
    //         if (deleteButton) {
    //             deleteButton.onclick = () => T_DeleteRow(newRowNumber);
    //         }
    //     });
    // }
//endregion
//region5. 其他業務費
function calculateOther() {
      let total = 0;
      const table = document.querySelector('.other tbody');
      const dataRows = table.querySelectorAll('tr:not(.total-row)');
      
      dataRows.forEach((row, index) => {
          // 動態尋找每行的薪資和月份輸入框
            const avgSalaryInput = row.cells[1].querySelector('input'); // 第2欄是 平均月薪
            const monthInput = row.cells[2].querySelector('input'); // 第3欄是 參與人月
            const peopleInput = row.cells[3].querySelector('input'); // 第4欄是 人數
            const totalCell = row.cells[4]; // 第5欄是人事費小計	
          
          if (avgSalaryInput && monthInput && peopleInput) {
              const avgSalary= parseFloat(avgSalaryInput.value) || 0;
              const month = parseFloat(monthInput.value) || 0;
              const people = parseFloat(peopleInput.value) || 0;
              const rowTotal = (avgSalary * month * people);
              
              // 更新該行的小計顯示
              if (totalCell) {
                  totalCell.textContent = rowTotal.toLocaleString();
              }
              
              total += rowTotal;
          }
      });
      
      // 更新總計顯示
      updateTotalOtherTotal(total);
  }
  

  function updateTotalOtherTotal(total) {
      // 更新總計顯示
      const totalCell = document.getElementById('otherTotal') ;
      if (totalCell) {
          totalCell.textContent = total.toLocaleString();
      }
      
      // 將其他業務費合計帶入勞務委託費金額欄位
      const serviceCashSpan = document.getElementById('serviceCash');
      if (serviceCashSpan) {
          serviceCashSpan.textContent = total.toLocaleString();
      }
      
      // 自動生成勞務委託費的計算方式及說明
      generateServiceDescription(total);
      
      // 更新租金+勞務委託費合計
      calculateOtherRentTotal();
      
      // 更新經費總表
      updateBudgetSummary();
  }
  
  function generateServiceDescription(total) {
      const table = document.querySelector('.other tbody');
      const dataRows = table.querySelectorAll('tr:not(.total-row)');
      let descriptionLines = [];
      
      dataRows.forEach((row, index) => {
          const jobTitleSelect = row.cells[0].querySelector('select');
          const avgSalaryInput = row.cells[1].querySelector('input');
          const monthInput = row.cells[2].querySelector('input');
          const peopleInput = row.cells[3].querySelector('input');
          
          if (jobTitleSelect && avgSalaryInput && monthInput && peopleInput) {
              const jobTitle = jobTitleSelect.options[jobTitleSelect.selectedIndex]?.text || '';
              const avgSalary = parseFloat(avgSalaryInput.value) || 0;
              const month = parseFloat(monthInput.value) || 0;
              const people = parseFloat(peopleInput.value) || 0;
              
              // 只有當有實際數值時才加入說明
              if (jobTitle && avgSalary > 0 && month > 0 && people > 0) {
                  const salaryInThousands = (avgSalary / 1000).toFixed(1);
                  const line = `${jobTitle} 人員${salaryInThousands}千元*${month}月*${people}人`;
                  descriptionLines.push(line);
              }
          }
      });
      
      // 加入總計行
      if (total > 0) {
          const totalInThousands = (total / 1000).toFixed(0);
          descriptionLines.push(`總計: ${totalInThousands}千元`);
      }
      
      // 更新說明欄位
      const serviceDescriptionSpan = document.getElementById('serviceDescription');
      if (serviceDescriptionSpan) {
          serviceDescriptionSpan.textContent = descriptionLines.join('\n');
      }
  }
  
  function calculateOtherRentTotal() {
      // 取得租金金額
      const rentCashInput = document.getElementById('rentCash');
      const rentAmount = parseFloat(rentCashInput?.value?.replace(/,/g, '') || '0');
      
      // 取得勞務委託費金額
      const serviceCashSpan = document.getElementById('serviceCash');
      const serviceAmount = parseFloat(serviceCashSpan?.textContent?.replace(/,/g, '') || '0');
      
      // 計算合計
      const total = rentAmount + serviceAmount;
      
      // 更新合計顯示
      const totalCell = document.getElementById('otherRentTotal');
      if (totalCell) {
          totalCell.textContent = total.toLocaleString();
      }
      
      // 更新經費總表中的其他業務費補助款 (第5項)
      updateAmountA('5', total);
      
      // 重新計算經費總表
      updateItemTotals();
      updateGrandTotals();
      updatePercentages();
  }
  
    function O_DeleteRow(button) {
                const confirmed = confirm('確定要刪除此行資料嗎？');
            if (confirmed) {
                // 找到該按鈕所在的 <tr>
                const row = button.closest('tr');
                if (row) {
                    row.remove();
                    calculateOther(); // 如有總計等更新
                } else {
                    alert('找不到要刪除的行');
                }
            }
        }
     function O_addRow() {
        const table = document.querySelector('.other tbody');
        const totalRow = table.querySelector('.total-row');
       
       // 建立新行
         const newRow = document.createElement('tr');
         const rowCount = table.children.length; // 包含總計行
         const ddlSelect = document.createElement('select');
         ddlSelect.className = "dropdown-list";
         ddlSelect.id = `otherJobTitle${rowCount}`;
         
         // 加入選項
         ddlOtherOptions.forEach(option => {
             const opt = document.createElement('option');
             opt.value = option.value;
             opt.textContent = option.text;
             ddlSelect.appendChild(opt);
         });
         
         // 為新增的職稱下拉選單加入 onchange 事件
         ddlSelect.addEventListener("change", function () {
             calculateOther();
         });

         newRow.innerHTML = `
             <td></td>
             <td><input type="text" ID="otherAvgSalary${rowCount}" class="textbox" onblur="calculateOther()"/></td>
             <td><input type="text" ID="otherMonth${rowCount}" class="textbox days" onblur="calculateOther()" /> </td>
             <td><input type="text" ID="otherPeople${rowCount}" class="textbox people" onblur="calculateOther()" /></td>
             <td></td>
             <td><button type="button" class="btn-delete" onclick="O_DeleteRow(this)">🗑</button></td>
         `;
         table.insertBefore(newRow, totalRow);
         // 將 select 插入第一個 <td>
         newRow.children[0].appendChild(ddlSelect);
     }
 
//endregion
//region 6. 經費總表自動更新功能

function updateBudgetSummary() {
    // 取得各個 table 的總計
    const personTotal = parseFloat(document.getElementById('PersonTotal')?.textContent?.replace(/,/g, '') || '0');
    const materialTotal = parseFloat(document.getElementById('MaterialTotal')?.textContent?.replace(/,/g, '') || '0');
    const researchTotal = parseFloat(document.getElementById('ResearchFeesTotal')?.textContent?.replace(/,/g, '') || '0');
    const travelTotal = parseFloat(document.getElementById('travelTotal')?.textContent?.replace(/,/g, '') || '0');
    const otherRentTotal = parseFloat(document.getElementById('otherRentTotal')?.textContent?.replace(/,/g, '') || '0');
    
    // 更新經費總表中的補助款 (A) - 除了行政管理費外
    updateAmountA('1', personTotal);        // 人事費
    updateAmountA('2', materialTotal);      // 消耗性器材及原材料費
    updateAmountA('3', researchTotal);      // 技術移轉、委託研究或驗證費
    updateAmountA('4', travelTotal);        // 國內差旅費
    updateAmountA('5', otherRentTotal);     // 其他業務費（租金+勞務委託費合計）
    // 行政管理費保持原狀，不自動更新
    
    // 更新每個項目的合計 (C) = (A) + (B)
    updateItemTotals();
    
    // 更新經費總計
    updateGrandTotals();
    
    // 更新百分比
    updatePercentages();
}

function updateAmountA(rowIndex, amount) {
    const rows = document.querySelectorAll('.main-table tbody tr');
    let targetRow = null;
    
    // 根據科目名稱找到對應的行
    rows.forEach(row => {
        const firstCell = row.cells[0]?.textContent;
        if (firstCell?.includes(`${rowIndex}.`)) {
            targetRow = row;
        }
    });
    
    if (targetRow) {
        const amountACell = targetRow.querySelector('.amount-a');
        if (amountACell) {
            amountACell.textContent = amount.toLocaleString();
        }
    }
}

function updateItemTotals() {
    const rows = document.querySelectorAll('.main-table tbody tr:not(.total-row):not(.percentage-row)');
    
    rows.forEach(row => {
        const amountACell = row.querySelector('.amount-a');
        const amountBCell = row.querySelector('.amount-b');
        const totalCell = row.querySelector('.amount-total');
        
        if (amountACell && amountBCell && totalCell) {
            // 處理補助款 (A) - 可能是 textContent 或 input value
            let amountA = 0;
            const amountAInput = amountACell.querySelector('input');
            if (amountAInput) {
                // 如果是輸入框 (如行政管理費)
                amountA = parseFloat(amountAInput.value?.replace(/,/g, '') || '0');
            } else {
                // 如果是文字內容 (如其他自動計算的項目)
                amountA = parseFloat(amountACell.textContent?.replace(/,/g, '') || '0');
            }
            
            const amountBInput = amountBCell.querySelector('input');
            const amountB = parseFloat(amountBInput?.value?.replace(/,/g, '') || '0');
            
            const total = amountA + amountB;
            totalCell.textContent = total.toLocaleString();
        }
    });
}

function updateGrandTotals() {
    const rows = document.querySelectorAll('.main-table tbody tr:not(.total-row):not(.percentage-row)');
    let totalA = 0;
    let totalB = 0;
    let totalC = 0;
    
    rows.forEach(row => {
        const amountACell = row.querySelector('.amount-a');
        const amountBCell = row.querySelector('.amount-b');
        const totalCell = row.querySelector('.amount-total');
        
        if (amountACell && amountBCell && totalCell) {
            // 處理補助款 (A) - 可能是 textContent 或 input value
            let amountA = 0;
            const amountAInput = amountACell.querySelector('input');
            if (amountAInput) {
                // 如果是輸入框 (如行政管理費)
                amountA = parseFloat(amountAInput.value?.replace(/,/g, '') || '0');
            } else {
                // 如果是文字內容 (如其他自動計算的項目)
                amountA = parseFloat(amountACell.textContent?.replace(/,/g, '') || '0');
            }
            
            const amountBInput = amountBCell.querySelector('input');
            const amountB = parseFloat(amountBInput?.value?.replace(/,/g, '') || '0');
            const amountC = parseFloat(totalCell.textContent?.replace(/,/g, '') || '0');
            
            totalA += amountA;
            totalB += amountB;
            totalC += amountC;
        }
    });
    
    // 更新經費總計行 - 尋找包含"經費總計"的行
    const allRows = document.querySelectorAll('.main-table tbody tr');
    let totalRow = null;
    
    allRows.forEach(row => {
        if (row.textContent.includes('經費總計')) {
            totalRow = row;
        }
    });
    
    if (totalRow) {
        const cells = totalRow.querySelectorAll('.number-cell');
        if (cells.length >= 3) {
            cells[0].innerHTML = `${totalA.toLocaleString()}<br>(I)`;
            cells[1].textContent = totalB.toLocaleString();
            cells[2].innerHTML = `${totalC.toLocaleString()}<br>(II)`;
        }
    }
}

function updatePercentages() {
    // 先取得經費總計 (I) 和 (II) 的值
    const allRows = document.querySelectorAll('.main-table tbody tr');
    let totalRow = null;
    let percentageRow = null;
    
    allRows.forEach(row => {
        if (row.textContent.includes('經費總計')) {
            totalRow = row;
        }
        if (row.textContent.includes('百分比')) {
            percentageRow = row;
        }
    });
    
    if (!totalRow) {
        console.log('找不到經費總計行');
        return;
    }
    
    const totalCells = totalRow.querySelectorAll('.number-cell');
    if (totalCells.length < 3) return;
    
    // 取得經費總計數值
    const totalI = parseFloat(totalCells[0].textContent?.replace(/,/g, '').replace(/\(I\)/g, '').replace(/<br>/g, '') || '0');
    const totalB = parseFloat(totalCells[1].textContent?.replace(/,/g, '') || '0');
    const totalII = parseFloat(totalCells[2].textContent?.replace(/,/g, '').replace(/\(II\)/g, '').replace(/<br>/g, '') || '0');
    
    console.log('經費總計數值:', { totalI, totalB, totalII });
    
    // 更新各科目的百分比
    const itemRows = document.querySelectorAll('.main-table tbody tr:not(.total-row):not(.percentage-row)');
    
    itemRows.forEach(row => {
        const amountACell = row.querySelector('.amount-a');
        const totalCell = row.querySelector('.amount-total');
        const allCells = row.querySelectorAll('td');
        
        if (amountACell && totalCell && allCells.length >= 6) {
            // 取得該項目的 A 和 C 值
            let amountA = 0;
            const amountAInput = amountACell.querySelector('input');
            if (amountAInput) {
                amountA = parseFloat(amountAInput.value?.replace(/,/g, '') || '0');
            } else {
                amountA = parseFloat(amountACell.textContent?.replace(/,/g, '') || '0');
            }
            
            const amountC = parseFloat(totalCell.textContent?.replace(/,/g, '') || '0');
            
            // 計算百分比
            const percentageC_II = totalII > 0 ? ((amountC / totalII) * 100).toFixed(2) + '%' : '0%';
            const percentageA_I = totalI > 0 ? ((amountA / totalI) * 100).toFixed(2) + '%' : '0%';
            
            // 更新百分比欄位 (第5和第6個td)
            if (allCells[4]) {
                allCells[4].textContent = percentageC_II;  // 佔總經費比率 (C)/(II)
                console.log('更新佔總經費比率:', percentageC_II);
            }
            if (allCells[5]) {
                allCells[5].textContent = percentageA_I;   // 各科目補助比率 (A)/(I)
                console.log('更新各科目補助比率:', percentageA_I);
            }
        }
    });
    
    // 更新百分比行 (經費總計的百分比)
    if (percentageRow && totalII > 0) {
        const percentageCells = percentageRow.querySelectorAll('.number-cell');
        if (percentageCells.length >= 3) {
            const percentageA = ((totalI / totalII) * 100).toFixed(2) + '%';
            const percentageB = ((totalB / totalII) * 100).toFixed(2) + '%';
            
            percentageCells[0].textContent = percentageA;  // 補助款百分比
            percentageCells[1].textContent = percentageB;  // 配合款百分比
            percentageCells[2].textContent = '100%';       // 總計百分比
        }
    }
}

//endregion
//region 7. 儲存功能
function collectFormData(){
    const data = {
        personnel: [],
        materials: [],
        researchFees: [],
        travel: [],
        otherFees: [],
        otherRent: [],
    };

    // --- 人事費 ---
    document.querySelectorAll(".person tbody tr:not(.total-row)").forEach(tr => {
        data.personnel.push({
            name: tr.querySelector("input[id^='personName']")?.value || "",
            stay: tr.querySelector("input[type='checkbox']")?.checked || false,
            title: tr.querySelector("select[id^='ddlPerson']")?.value || "",
            salary: parseFloat(tr.querySelector("input[id^='personSalary']")?.value || "0"),
            months: parseFloat(tr.querySelector("input[id^='personMonths']")?.value || "0")
        });
    });

    // --- 消耗性器材及原材料費 ---
    document.querySelectorAll(".Material tbody tr:not(.total-row)").forEach(tr => {
        data.materials.push({
            name: tr.querySelector("input[id^='MaterialName']")?.value || "",
            description: tr.querySelector("input[id^='MaterialDescription']")?.value || "",
            unit: tr.querySelector("select[id^='MaterialUnit']")?.value || "",
            quantity: parseFloat(tr.querySelector("input[id^='MaterialNum']")?.value || "0"),
            unitPrice: parseFloat(tr.querySelector("input[id^='MaterialUnitPrice']")?.value || "0")
        });
    });

    // --- 技術移轉/委託研究 ---
    document.querySelectorAll(".ResearchFees tbody tr:not(.total-row)").forEach(tr => {
        data.researchFees.push({
            category: tr.querySelector("span[id^='FeeCategory']")?.innerText.trim() || "",
            dateStart: tr.querySelector("input[id^='txtDate'][id*='Start']")?.value || "",
            dateEnd: tr.querySelector("input[id^='txtDate'][id*='End']")?.value || "",
            projectName: tr.querySelector("input[id^='ResearchFeesName']")?.value || "",
            targetPerson: tr.querySelector("input[id^='ResearchFeesPersonName']")?.value || "",
            price: parseFloat(tr.querySelector("input[id^='ResearchFeesPrice']")?.value || "0")
        });
    });

    // --- 差旅費 ---
    document.querySelectorAll("#travelTable tbody tr:not(.total-row)").forEach(tr => {
        data.travel.push({
            reason: tr.querySelector("input[id^='travelReason']")?.value || "",
            area: tr.querySelector("input[id^='travelArea']")?.value || "",
            days: parseInt(tr.querySelector("input[id^='travelDays']")?.value || "0"),
            people: parseInt(tr.querySelector("input[id^='travelPeople']")?.value || "0"),
            price: parseFloat(tr.querySelector("input[id^='travelPrice']")?.value || "0")
        });
    });

    // --- 其他業務費 ---
    document.querySelectorAll("#otherTable tbody tr:not(.total-row)").forEach(tr => {
        data.otherFees.push({
            title: tr.querySelector("select[id^='otherJobTitle']")?.value || "",
            avgSalary: parseFloat(tr.querySelector("input[id^='otherAvgSalary']")?.value || "0"),
            months: parseFloat(tr.querySelector("input[id^='otherMonth']")?.value || "0"),
            people: parseInt(tr.querySelector("input[id^='otherPeople']")?.value || "0")
        });
    });

    // --- 最後一塊：租金與勞務委託費 ---
    // 租金
    const rentCashInput = document.getElementById('rentCash');
    const rentDescInput = document.getElementById('rentDescription');
    data.otherRent.push({
        item: "租金",
        amount: parseFloat(rentCashInput?.value?.replace(/,/g, '') || "0"),
        note: rentDescInput?.value || ""
    });
    
    // 勞務委託費
    const serviceCashSpan = document.getElementById('serviceCash');
    const serviceDescSpan = document.getElementById('serviceDescription');
    data.otherRent.push({
        item: "勞務委託費",
        amount: parseFloat(serviceCashSpan?.textContent?.replace(/,/g, '') || "0"),
        note: serviceDescSpan?.textContent || ""
    });

    // --- 經費總表 ---
    data.totalFees = [];
    const budgetRows = document.querySelectorAll("table.main-table:last-of-type tbody tr:not(.total-row):not(.percentage-row)");
    budgetRows.forEach(tr => {
        const cells = tr.querySelectorAll("td");
        if (cells.length >= 4) {
            const accountingItem = cells[0]?.textContent?.trim() || "";
            
            // 獲取補助款 (A) - 可能是文字內容或輸入框值
            let subsidyAmount = 0;
            const subsidyCell = cells[1];
            const subsidyInput = subsidyCell?.querySelector("input");
            if (subsidyInput) {
                subsidyAmount = parseFloat(subsidyInput.value?.replace(/,/g, '') || "0");
            } else {
                subsidyAmount = parseFloat(subsidyCell?.textContent?.replace(/,/g, '') || "0");
            }
            
            // 獲取配合款 (B) - 通常是輸入框
            let coopAmount = 0;
            const coopCell = cells[2];
            const coopInput = coopCell?.querySelector("input");
            if (coopInput) {
                coopAmount = parseFloat(coopInput.value?.replace(/,/g, '') || "0");
            }
            
            // 只有當會計科目不為空時才加入
            if (accountingItem) {
                data.totalFees.push({
                    accountingItem: accountingItem,
                    subsidyAmount: subsidyAmount,
                    coopAmount: coopAmount
                });
            }
        }
    });

    return data
}
function btnSave_Click() {
    const jsonData = collectFormData();
    const projectId = new URLSearchParams(window.location.search).get("ProjectID");
    console.log(jsonData);
    $.ajax({
        type: "POST",
        url: "SciFunding.aspx/SaveForm",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            formData: {  // 包裝在 formData 參數中
                ProjectID: projectId,
                Personnel: jsonData.personnel,
                Materials: jsonData.materials,
                ResearchFees: jsonData.researchFees,
                Travel: jsonData.travel,
                OtherFees: jsonData.otherFees,
                OtherRent: jsonData.otherRent,
                TotalFees: jsonData.totalFees
            }
        }),
        dataType: "json",
        success: function (response) {
            alert("儲存成功！");
        },
        error: function (err) {
            console.error("儲存失敗", err);
        }
    });
    
}
//endregion

</script>
</head>
<div class="links">
  <a href="http://localhost:50929/OFS/SciFunding.aspx?ProjectID=114SCI0006" target="_blank">📄 計畫經費填報</a>
  <a href="http://localhost:50929/OFS/SciApplication.aspx?ProjectID=114SCI0006" target="_blank">📝 科專申請資料</a>
  <a href="http://localhost:50929/OFS/SciOutcomes.aspx?ProjectID=114SCI0006" target="_blank">📊 成果與績效</a>
  <a href="http://localhost:50929/OFS/SciRecusedList.aspx?ProjectID=114SCI0006" target="_blank">📊 其他</a>

</div>

<body>
    <form id="formContainer" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <div class="container">
            <div class="header">
                <span class="header-icon"></span>
                1.海洋科技研發人員人事費明細表
            </div>
           <table class="main-table person">
                <thead>
                    <tr>
                        <th class="name-cell">姓名</th>
                        <th class="checkbox-cell">待聘</th>
                        <th class="position-cell">職稱</th>
                        <th class="salary-cell">平均月薪</th>
                        <th class="months-cell">參與人月</th>
                        <th class="total-cell">人事費小計</th>
                        <th class="action-cell">操作</th>
                    </tr>
                </thead>
                <tbody>
                    
                    <tr>
                        <td class="name-cell"><asp:TextBox ID="personName1" runat="server" CssClass="textbox"/></td>
                        <td><asp:CheckBox ID="stay1" runat="server" /></td>

                        <td>
                          <select id="ddlPerson1" class="dropdown-list"></select>

                        </td>
                        <td><asp:TextBox ID="personSalary1" runat="server" CssClass="textbox" Text="0" onblur="checkSalaryLimit(1); calculateAndUpdateTotal()"/></td>
                        <td><asp:TextBox ID="personMonths1" runat="server" CssClass="textbox" Text="0" onblur="calculateAndUpdateTotal()" /></td>
                        <td class="salary-cell">0</td>
                        <td>
                            <button type="button" class="add-btn" onclick="P_addNewRow()">+</button>
                        </td>
                    </tr>
                    <tr class="total-row">
                        <td colspan="2">合計</td>
                        <td>--</td>
                        <td>--</td>
                        <td>--</td>
                          <td class="total-amount" id="PersonTotal"></td>
                        <td><button type="button" class="add-btn" onclick="calculateAndUpdateTotal()">計算費用</button></td>
                    </tr>
                </tbody>
            </table>
            <div class="header">
                <span class="header-icon"></span>
                2.消耗性器材及原材料費
            </div>
            <table class="main-table Material">
                <thead>
                    <tr>
                        
                        <th class="name-cell">品名</th>
                        <th class="checkbox-cell">說明</th>
                        <th class="position-cell">單位</th>
                        <th class="salary-cell">預估需求數量</th>
                        <th class="months-cell">單價</th>
                        <th class="total-cell">總價</th>
                        <th class="action-cell">操作</th>
                    </tr>
                </thead>
                <tbody>
                    
                    <tr>
                        <td class="name-cell"><asp:TextBox ID="MaterialName1" runat="server" CssClass="textbox"/></td>
                        <td class="name-cell"><asp:TextBox ID="MaterialDescription1" runat="server" CssClass="textbox"/></td>
                        <td>
                      
                            <select id="MaterialUnit1" class="dropdown-list"></select>

                        </td>
                        <td><asp:TextBox ID="MaterialNum1" runat="server" CssClass="textbox" Text="" onblur="calculateMaterial()"/></td>
                        <td><asp:TextBox ID="MaterialUnitPrice1" runat="server" CssClass="textbox" Text="" onblur="checkMaterialLimit(1);calculateMaterial()" /></td>
                        <td class="salary-cell"></td>
                        <td>
                            <button type="button" class="add-btn" onclick="M_addNewRow()" >+</button>
                        </td>
                    </tr>
                    <tr class="total-row">
                        <td colspan="2">合計</td>
                        <td>--</td>
                        <td>--</td> 
                        <td>--</td>
                        <td class="total-amount" id="MaterialTotal"></td>
                        <td></td>
                    </tr>
                </tbody>
            </table>
            <div class="header">
                <span class="header-icon"></span>
                3. 技術移轉、委託研究或驗證費
            </div>
            <table class="main-table ResearchFees">
                <thead>
                    <tr>
                        <th>　</th>
                        <th colspan="2">期間</th>
                        <th>委託項目名稱</th>
                        <th>委託對象</th>
                        <th>金額</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td><span ID="FeeCategory1" runat="server">技術移轉</span></td>
                        <td><asp:TextBox ID="txtDate1Start" runat="server" CssClass="flatpickr-date textbox" /></td>
                        <td><asp:TextBox ID="txtDate1End" runat="server" CssClass="flatpickr-date textbox" /></td>
                        <td><asp:TextBox ID="ResearchFeesName1" runat="server" CssClass="textbox" /></td>
                        <td><asp:TextBox ID="ResearchFeesPersonName1" runat="server" CssClass="textbox" /></td>
                        <td><asp:TextBox ID="ResearchFeesPrice1" runat="server" CssClass="textbox money" onblur="calculateResearch()" /></td>
                    </tr>
                    <tr>
                        <td><span ID="FeeCategory2" runat="server">轉委託研究</span></td>
                        <td><asp:TextBox ID="txtDate2Start" runat="server" CssClass="flatpickr-date textbox" /></td>
                        <td><asp:TextBox ID="txtDate2End" runat="server" CssClass="flatpickr-date textbox" /></td>
                        <td><asp:TextBox ID="ResearchFeesName2" runat="server" CssClass="textbox" /></td>
                        <td><asp:TextBox ID="ResearchFeesPersonName2" runat="server" CssClass="textbox" /></td>
                        <td><asp:TextBox ID="ResearchFeesPrice2" runat="server" CssClass="textbox money" onblur="calculateResearch()" /></td>
                    </tr>
                    <tr class="total-row">
                        <td colspan="5">合計</td>
                        <td id="ResearchFeesTotal">0</td>
                    </tr>
                </tbody>
            </table>
             <div class="header">
                <span class="header-icon"></span>
                4. 國內差旅費
             </div>
            <table class="main-table travel" id="travelTable">
                <thead>
                    <tr>
                        <th>出差事由</th>
                        <th>地區</th>
                        <th>天數</th>
                        <th>人次</th>
                        <th>金額</th>
                        <th>操作</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td><asp:TextBox ID="travelReason1" runat="server" CssClass="textbox" Text="" /></td>
                        <td><asp:TextBox ID="travelArea1" runat="server" CssClass="textbox" Text="" /></td>
                        <td><asp:TextBox ID="travelDays1" runat="server" CssClass="textbox days" Text="0"  /></td>
                        <td><asp:TextBox ID="travelPeople1" runat="server" CssClass="textbox people" Text="0"  /></td>
                        <td><asp:TextBox ID="travelPrice1" runat="server" CssClass="textbox price" Text="0" onblur="calculateTravel()" /></td>
                        <td>
                            <button type="button" class="icon-btn" onclick="T_addRow()">+</button>
                        </td>
                    </tr>
                    <tr class="total-row">
                        <td colspan="4">合計</td>
                        <td id="travelTotal">0</td>
                        <td></td>
                    </tr>
                </tbody>
            </table>
            <div class="header">
                <span class="header-icon"></span>
                5. 其他業務費
             </div>
            <table class="main-table other" id="otherTable">
                <thead>
                    <tr>
                        <th>職稱</th>
                        <th>平均月薪</th>
                        <th>參與人月</th>
                        <th>人數</th>
                        <th>人事費小計</th>
                        <th>操作</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td><select id="otherJobTitle1" class="dropdown-list"></select></td>
                        <td><asp:TextBox ID="otherAvgSalary1" runat="server" CssClass="textbox" onblur="calculateOther()" Text="" /></td>
                        <td><asp:TextBox ID="otherMonth1" runat="server" CssClass="textbox Month" onblur="calculateOther()" Text="0"  /></td>
                        <td><asp:TextBox ID="otherPeople1" runat="server" CssClass="textbox people" onblur="calculateOther()" Text="0"  /></td>
                        <td></td>
                        <td>
                            <button type="button" class="icon-btn" onclick="O_addRow()">+</button>
                        </td>
                    </tr>
                    <tr class="total-row">
                        <td colspan="4">合計</td>
                        <td id="otherTotal">0</td>
                        <td></td>
                    </tr>
                </tbody>
            </table>
            
            <table class="main-table otherRent">
            <thead>
                <tr>
                    <th>項目</th>
                    <th>金額</th>
                    <th>計算方式及說明</th>
                   
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>租金</td>
                    <td><asp:TextBox runat="server" CssClass="textbox" ID="rentCash" Text="" onblur="calculateOtherRentTotal()" /></td>
                    <td><asp:TextBox runat="server" CssClass="textbox " ID="rentDescription" Text=""  /></td>
                </tr> 
                <tr>
                    <td>勞務委託費</td>
                    <td><span id="serviceCash">0</span></td>
                    <td><span id="serviceDescription" style="white-space: pre-line; text-align: left; padding: 8px; display: block; min-height: 60px;"></span></td>
                </tr>
            
                <tr class="total-row">
                    <td >合計</td>
                    <td id="otherRentTotal">0</td>
                    <td></td>
                </tr>
            </tbody>
        </table>
            <div class="header">
                <span class="header-icon"></span>
                經費總表
            </div>
            <table class="main-table ">
                <thead>
                    <tr class="header-row">
                        <th>會計科目</th>
                        <th>補助款 (A)</th>
                        <th>配合款 (B)</th>
                        <th>合計 (C)</th>
                        <th>佔總經費比率 (C)/(II)</th>
                        <th>各科目補助比率 (A)/(I)</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td class="subcategory">1.人事費</td>
                        <td class="number-cell amount-a">0</td>
                        <td class="number-cell amount-b"><asp:TextBox  runat="server" CssClass="textbox" Text="0" onblur="updateBudgetSummary()" /></td>
                        <td class="number-cell amount-total">0</td>
                        <td class="number-cell">0%</td>
                        <td class="number-cell">0%</td>
                    </tr>
                    <tr>
                        <td class="subcategory">2.消耗性器材及原材料費</td>
                        <td class="number-cell amount-a">0</td>
                        <td class="number-cell amount-b"><asp:TextBox  runat="server" CssClass="textbox" Text="0" onblur="updateBudgetSummary()" /></td>
                        <td class="number-cell amount-total">0</td>
                        <td class="number-cell">0%</td>
                        <td class="number-cell">0%</td>
                    </tr>
                    <tr>
                        <td class="subcategory">3. 技術移轉、委託研究或驗證費</td>
                        <td class="number-cell amount-a">0</td>
                        <td class="number-cell amount-b"><asp:TextBox  runat="server" CssClass="textbox" Text="0" onblur="updateBudgetSummary()" /></td>
                        <td class="number-cell amount-total">0</td>
                        <td class="number-cell">0%</td>
                        <td class="number-cell">0%</td>
                    </tr>
                    <tr>
                        <td class="subcategory">4. 國內差旅費</td>
                        <td class="number-cell amount-a">0</td>
                        <td class="number-cell amount-b"><asp:TextBox  runat="server" CssClass="textbox" Text="0" onblur="updateBudgetSummary()" /></td>
                        <td class="number-cell amount-total">0</td>
                        <td class="number-cell">0%</td>
                        <td class="number-cell">0%</td>
                    </tr>
                    <tr>
                        <td class="subcategory">5. 其他業務費</td>
                        <td class="number-cell amount-a">0</td>
                        <td class="number-cell amount-b"><asp:TextBox  runat="server" CssClass="textbox" Text="0" onblur="updateBudgetSummary()" /></td>
                        <td class="number-cell amount-total">0</td>
                        <td class="number-cell">0%</td>
                        <td class="number-cell">0%</td>
                    </tr>
                    <tr>
                        <td class="subcategory">6. 行政管理費</td>
                        <td class="number-cell amount-a"><asp:TextBox  runat="server" CssClass="textbox" Text="0" onblur="updateBudgetSummary()" /></td>
                        <td class="number-cell amount-b"><asp:TextBox  runat="server" CssClass="textbox" Text="0" onblur="updateBudgetSummary()" /></td>
                        <td class="number-cell amount-total">0</td>
                        <td class="number-cell">0%</td>
                        <td class="number-cell">0%</td>
                    </tr>
                    <tr class="total-row">
                        <td class="category-header">經費總計</td>
                        <td class="number-cell">0<br>(I)</td>
                        <td class="number-cell">0</td>
                        <td class="number-cell">0<br>(II)</td>
                        <td class="number-cell">--</td>
                        <td class="number-cell">--</td>
                    </tr>
                    <tr class="percentage-row">
                        <td class="category-header">百分比</td>
                        <td class="number-cell">0%</td>
                        <td class="number-cell">0%</td>
                        <td class="number-cell">0%</td>
                        <td class="number-cell">--</td>
                        <td class="number-cell">--</td>
                    </tr>
                </tbody>
            </table>
       </div>
        
         <div style="text-align: center;">
            <asp:Button ID="btnTempSave" runat="server" Text="暫時儲存表單" CssClass="save-btn" OnClientClick="btnSave_Click(); return false;" />
            <asp:Button ID="btnSubmit" runat="server" Text="提交申請" CssClass="save-btn" style="margin-left: 10px; background-color: #007bff;" OnClientClick="btnSave_Click(); return false;" />
        </div>
</form>
</body>
</html>