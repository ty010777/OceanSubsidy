// SciFunding.js - 經費/人事頁面的JavaScript功能

// 初始化
document.addEventListener('DOMContentLoaded', function () {
    initializeDropdowns();
    calculateResearch();

    // 延遲初始化日期選擇器，確保所有相關套件都已載入
    setTimeout(function() {
        initializeDatePickers(); // 初始化日期選擇器
    }, 500);

    // 延遲載入資料，確保所有初始化完成
    setTimeout(function() {
        loadExistingDataToForm();
        // 根據 OrgCategory 決定是否隱藏行政管理費
        handleAdminFeeVisibility();
    }, 800);
});

// 確保頁面完全載入後再次檢查日期選擇器
window.addEventListener('load', function() {
    setTimeout(function() {
        // 檢查日期選擇器是否正確初始化
        const dateInputs = document.querySelectorAll('input[id*="txtDate"]');
        dateInputs.forEach(input => {
            if (input.value && input.value.includes('NaN')) {
                // 如果發現 NaN，重新初始化該欄位
                input.value = '';
                console.log('發現 NaN，重新初始化日期欄位:', input.id);
            }
        });

        // 強制重新渲染日期選擇器
        if (typeof $.fn.daterangepicker !== 'undefined') {
            $('input[id*="txtDate1Start"], input[id*="txtDate1End"], input[id*="txtDate2Start"], input[id*="txtDate2End"]').each(function() {
                if ($(this).data('daterangepicker')) {
                    $(this).data('daterangepicker').remove();
                }
            });

            // 重新初始化
            setTimeout(initializeDatePickers, 100);
        }
    }, 300);
});

// 初始化台灣日期選擇器
function initializeDatePickers() {
    if (typeof moment !== 'undefined') {
        moment.locale('zh-tw');

        // 統一初始化所有日期選擇器（都使用民國年顯示）
        initializeAllDatePickers();
    }
}

function syncLoadedDatesToHiddenFields() {
    // 同步所有具有 data-gregorian-date 屬性的日期欄位到隱藏欄位
    $('.taiwan-date-picker').each(function() {
        const $input = $(this);
        const gregorianDate = $input.data('gregorian-date');

        if (gregorianDate) {
            // 建立隱藏欄位儲存西元日期供後端使用
            const hiddenFieldName = $input.attr('name') + '_gregorian';
            let hiddenField = $(`input[name="${hiddenFieldName}"]`);
            if (hiddenField.length === 0) {
                hiddenField = $(`<input type="hidden" name="${hiddenFieldName}" />`);
                $input.after(hiddenField);
            }
            hiddenField.val(gregorianDate);
        }
    });
}

function initializeAllDatePickers() {
    // 統一的日期選擇器配置 - 所有日期都使用民國年顯示，但儲存為西元年
    const datePickerConfig = {
        singleDatePicker: true,
        showDropdowns: true,
        autoUpdateInput: false,
        autoApply: true,
        locale: {
            format: 'tYY/MM/DD' // 使用 moment-taiwan 提供的 tYY 格式
        }
    };

    // 通用的日期選擇處理函數
    const handleDateSelection = function(ev, picker) {
        const selectedMoment = moment(picker.startDate);

        // 顯示：民國年格式
        const displayDate = selectedMoment.format('tYY/MM/DD');

        // 儲存：西元年格式
        const gregorianDate = selectedMoment.format('YYYY/MM/DD');

        $(this).val(displayDate);
        $(this).data('gregorian-date', gregorianDate);

        // 找到對應的隱藏欄位並設定西元年日期
        const inputId = $(this).attr('id');
        if (inputId) {
            const hiddenFieldId = inputId.replace('txt', 'hdn');
            const hiddenField = $(`#${hiddenFieldId}`);
            if (hiddenField.length > 0) {
                hiddenField.val(gregorianDate);
                console.log(`設定隱藏欄位 ${hiddenFieldId} 值為: ${gregorianDate}`);
            }
        }
    };

    // 初始化技術移轉期間日期選擇器
    $('input[id*="txtDate1Start"], input[id*="txtDate1End"]').daterangepicker(datePickerConfig)
        .on('apply.daterangepicker', handleDateSelection);

    // 初始化委託研究期間日期選擇器
    $('input[id*="txtDate2Start"], input[id*="txtDate2End"]').daterangepicker(datePickerConfig)
        .on('apply.daterangepicker', handleDateSelection);
}


// 收集所有表單資料並填入隱藏欄位
function collectAllFormData() {
    // 收集人員資料
    const personnelData = [];
    document.querySelectorAll('.person tbody tr:not(.total-row)').forEach(row => {
        const nameInput = row.querySelector('input[id*="personName"]');
        const stayCheckbox = row.querySelector('input[type="checkbox"]');
        const titleSelect = row.querySelector('select[id*="ddlPerson"]');
        const salaryInput = row.querySelector('input[id*="personSalary"]');
        const monthsInput = row.querySelector('input[id*="personMonths"]');
        
        if (nameInput && nameInput.value.trim()) {
            personnelData.push({
                name: nameInput.value,
                stay: stayCheckbox ? stayCheckbox.checked : false,
                title: titleSelect ? titleSelect.value : "",
                salary: parseFloat(salaryInput?.value?.replace(/,/g, '') || '0'),
                months: parseFloat(monthsInput?.value || '0')
            });
        }
    });
    
    // 收集材料資料
    const materialData = [];
    document.querySelectorAll('.Material tbody tr:not(.total-row)').forEach(row => {
        const nameInput = row.querySelector('input[id*="MaterialName"]');
        const descInput = row.querySelector('input[id*="MaterialDescription"]');
        const unitSelect = row.querySelector('select[id*="MaterialUnit"]');
        const numInput = row.querySelector('input[id*="MaterialNum"]');
        const priceInput = row.querySelector('input[id*="MaterialUnitPrice"]');
        
        if (nameInput && nameInput.value.trim()) {
            materialData.push({
                name: nameInput.value,
                description: descInput ? descInput.value : "",
                unit: unitSelect ? unitSelect.value : "",
                quantity: parseFloat(numInput?.value?.replace(/,/g, '') || '0'),
                unitPrice: parseFloat(priceInput?.value?.replace(/,/g, '') || '0')
            });
        }
    });
    
    // 收集差旅費資料
    const travelData = [];
    document.querySelectorAll('.travel tbody tr:not(.total-row)').forEach(row => {
        const reasonInput = row.querySelector('input[id*="travelReason"]');
        const areaInput = row.querySelector('input[id*="travelArea"]');
        const daysInput = row.querySelector('input[id*="travelDays"]');
        const peopleInput = row.querySelector('input[id*="travelPeople"]');
        const priceInput = row.querySelector('input[id*="travelPrice"]');
        
        if (reasonInput && reasonInput.value.trim()) {
            travelData.push({
                reason: reasonInput.value,
                area: areaInput ? areaInput.value : "",
                days: parseFloat(daysInput?.value || '0'),
                people: parseFloat(peopleInput?.value || '0'),
                price: parseFloat(priceInput?.value?.replace(/,/g, '') || '0')
            });
        }
    });
    
    // 收集其他業務費資料
    const otherData = [];
    document.querySelectorAll('.other tbody tr:not(.total-row)').forEach(row => {
        const titleSelect = row.querySelector('select[id*="otherJobTitle"]');
        const salaryInput = row.querySelector('input[id*="otherAvgSalary"]');
        const monthInput = row.querySelector('input[id*="otherMonth"]');
        const peopleInput = row.querySelector('input[id*="otherPeople"]');
        
        if (titleSelect && titleSelect.value) {
            otherData.push({
                title: titleSelect.value,
                avgSalary: parseFloat(salaryInput?.value || '0'),
                months: parseFloat(monthInput?.value || '0'),
                people: parseFloat(peopleInput?.value || '0')
            });
        }
    });
    
    // 收集租金與勞務資料
    const otherRentData = [];
    const rentCashInput = document.getElementById('rentCash');
    const rentDescInput = document.getElementById('rentDescription');
    const serviceCashSpan = document.getElementById('serviceCash');
    const serviceDescSpan = document.getElementById('serviceDescription');
    
    if (rentCashInput) {
        otherRentData.push({
            item: "租金",
            amount: parseFloat(rentCashInput.value?.replace(/,/g, '') || '0'),
            note: rentDescInput ? rentDescInput.value : ""
        });
    }
    
    if (serviceCashSpan) {
        otherRentData.push({
            item: "勞務委託費",
            amount: parseFloat(serviceCashSpan.textContent?.replace(/,/g, '') || '0'),
            note: serviceDescSpan ? serviceDescSpan.textContent : ""
        });
    }
    
    // 收集經費總表資料
    const totalFeesData = [];
    $('.main-table tbody tr').each(function(index, row) {
        const $allCells = $(row).find('th, td');
        if ($allCells.length >= 4) {
            const accountingItem = $allCells.eq(0).text().trim() || "";
            
            if (/^\d+\./.test(accountingItem)) {
                let subsidyAmount = 0;
                const $subsidyCell = $allCells.eq(1);
                if ($subsidyCell.length) {
                    // 檢查是否有 input 元素（如行政管理費）
                    const $subsidyInput = $subsidyCell.find('input');
                    if ($subsidyInput.length) {
                        subsidyAmount = parseFloat($subsidyInput.val().replace(/,/g, '') || '0');
                    } else {
                        // 否則從 text 讀取（其他項目）
                        const text = $subsidyCell.text().replace(/,/g, '') || '0';
                        subsidyAmount = parseFloat(text) || 0;
                    }
                }
                
                let coopAmount = 0;
                const $coopCell = $allCells.eq(2);
                const $coopInput = $coopCell.find('input');
                if ($coopInput.length) {
                    coopAmount = parseFloat($coopInput.val().replace(/,/g, '') || '0');
                }
                
                totalFeesData.push({
                    accountingItem: accountingItem,
                    subsidyAmount: subsidyAmount,
                    coopAmount: coopAmount
                });
            }
        }
    });
    
    // 將資料填入隱藏欄位
    $('#hdnPersonnelData').val(JSON.stringify(personnelData));
    $('#hdnMaterialData').val(JSON.stringify(materialData));
    $('#hdnTravelData').val(JSON.stringify(travelData));
    $('#hdnOtherData').val(JSON.stringify(otherData));
    $('#hdnOtherRentData').val(JSON.stringify(otherRentData));
    $('#hdnTotalFeesData').val(JSON.stringify(totalFeesData));
}

// 初始化下拉選單
function initializeDropdowns() {
    // 人員職稱下拉選單
    if (typeof ddlPersonOptions !== 'undefined') {
        const P_ddl = document.getElementById('ddlPerson1');
        if (P_ddl) {
            ddlPersonOptions.forEach(option => {
                const opt = document.createElement('option');
                opt.value = option.value;
                opt.textContent = option.text;
                P_ddl.appendChild(opt);
            });
            
            P_ddl.addEventListener("change", function () {
                document.getElementById('personSalary1').value = 0;
                calculateAndUpdateTotal();
            });
        }
    }

    // 材料單位下拉選單
    if (typeof ddlMaterialOptions !== 'undefined') {
        const M_ddl = document.getElementById('MaterialUnit1');
        if (M_ddl) {
            ddlMaterialOptions.forEach(option => {
                const opt = document.createElement('option');
                opt.value = option.value;
                opt.textContent = option.text;
                M_ddl.appendChild(opt);
            });
            
            M_ddl.addEventListener("change", function () {
                document.getElementById('MaterialUnitPrice1').value = 0;
                calculateMaterial();
            });
        }
    }

    // 其他業務費職稱下拉選單
    if (typeof ddlOtherOptions !== 'undefined') {
        const O_ddl = document.getElementById('otherJobTitle1');
        if (O_ddl) {
            ddlOtherOptions.forEach(option => {
                const opt = document.createElement('option');
                opt.value = option.value;
                opt.textContent = option.text;
                O_ddl.appendChild(opt);
            });
            
            O_ddl.addEventListener("change", function () {
                calculateOther();
            });
        }
    }
}

// 1.人事費明細表
function P_deleteRow(button) {
    const table = document.querySelector('.person tbody');
    const dataRows = table.querySelectorAll('tr:not(.total-row)');

    if (dataRows.length <= 1) {
        Swal.fire({
            icon: 'warning',
            title: '無法刪除',
            text: '至少需保留一行資料',
            confirmButtonText: '確定'
        });
        return;
    }

    Swal.fire({
        icon: 'question',
        title: '確定要刪除嗎？',
        text: '確定要刪除此行資料嗎？',
        showCancelButton: true,
        confirmButtonText: '確定',
        cancelButtonText: '取消'
    }).then((result) => {
        if (result.isConfirmed) {
            button.closest('tr').remove();
            calculateAndUpdateTotal();
        }
    });
}

function checkSalaryLimit(rowIndex) {
    const salaryInput = document.getElementById(`personSalary${rowIndex}`);
    const ddl = document.getElementById(`ddlPerson${rowIndex}`);
    const selectedCode = ddl.value;
    const salary = parseInt(salaryInput.value, 10);

    const selectedItem = ddlPersonOptions.find(x => x.value === selectedCode);
    if (selectedItem && salary > selectedItem.maxLimit) {
        Swal.fire({
            icon: 'warning',
            title: '金額超過上限',
            text: `輸入金額 ${salary} 超過上限：${selectedItem.maxLimit}`,
            confirmButtonText: '確定'
        });
        salaryInput.value = selectedItem.maxLimit;
    }
}

function calculateAndUpdateTotal() {
    let total = 0;
    const dataRows = document.querySelectorAll('.person tbody tr:not(.total-row)');
    
    dataRows.forEach((row) => {
        const salaryInput = row.cells[3].querySelector('input');
        const monthsInput = row.cells[4].querySelector('input');
        const totalCell = row.cells[5];
        
        if (salaryInput && monthsInput) {
            const salary = parseFloat(salaryInput.value.replace(/,/g, '')) || 0;
            const months = parseFloat(monthsInput.value) || 0;
            const rowTotal = salary * months;
            
            totalCell.textContent = rowTotal.toLocaleString();
            total += rowTotal;
        }
    });
    
    document.getElementById('PersonTotal').textContent = total.toLocaleString();
    updateBudgetSummary();
}

function P_addNewRow() {
    const table = document.querySelector('.person tbody');
    const totalRow = table.querySelector('.total-row');
    const rowCount = table.children.length;
    
    const ddlSelect = document.createElement('select');
    ddlSelect.className = "form-select";
    ddlSelect.id = `ddlPerson${rowCount}`;
    
    ddlPersonOptions.forEach(option => {
        const opt = document.createElement('option');
        opt.value = option.value;
        opt.textContent = option.text;
        ddlSelect.appendChild(opt);
    });
    
    ddlSelect.addEventListener("change", function () {
        document.getElementById(`personSalary${rowCount}`).value = 0;
        calculateAndUpdateTotal();
    });

    const newRow = document.createElement('tr');
    newRow.innerHTML = `
        <td><input type="text" id="personName${rowCount}" class="form-control" placeholder="請輸入姓名"></td>
        <td class="text-center"><input type="checkbox" id="stay${rowCount}" class="form-check-input check-teal" /></td>
        <td></td>
        <td><input  class="form-control text-end" id="personSalary${rowCount}" value="0" onkeypress="return event.charCode != 45" onblur="checkSalaryLimit(${rowCount}); calculateAndUpdateTotal()"></td>
        <td><input  class="form-control text-end" id="personMonths${rowCount}" value="0" onkeypress="return event.charCode != 45" onblur="calculateAndUpdateTotal()"></td>
        <td class="text-end">0</td>
        <td>
            <button type="button" class="btn btn-sm btn-teal" onclick="P_deleteRow(this)"><i class="fas fa-trash-alt"></i></button>
            <button type="button" class="btn btn-sm btn-teal" onclick="P_addNewRow()"><i class="fas fa-plus"></i></button>
        </td>
    `;
    
    table.insertBefore(newRow, totalRow);
    newRow.children[2].appendChild(ddlSelect);
}

// 2.消耗性器材及原材料費
function M_deleteRow(button) {
    const table = document.querySelector('.Material tbody');
    const dataRows = table.querySelectorAll('tr:not(.total-row)');

    if (dataRows.length <= 1) {
        Swal.fire({
            icon: 'warning',
            title: '無法刪除',
            text: '至少需保留一行資料',
            confirmButtonText: '確定'
        });
        return;
    }

    Swal.fire({
        icon: 'question',
        title: '確定要刪除嗎？',
        text: '確定要刪除此行資料嗎？',
        showCancelButton: true,
        confirmButtonText: '確定',
        cancelButtonText: '取消'
    }).then((result) => {
        if (result.isConfirmed) {
            button.closest('tr').remove();
            calculateMaterial();
        }
    });
}

function calculateMaterial() {
    let total = 0;
    const dataRows = document.querySelectorAll('.Material tbody tr:not(.total-row)');
    
    dataRows.forEach((row) => {
        const NumInput = row.cells[3].querySelector('input');
        const unitPriceInput = row.cells[4].querySelector('input');
        const totalCell = row.cells[5];
        
        if (NumInput && unitPriceInput) {
            const Num = parseFloat(NumInput.value.replace(/,/g, '')) || 0;
            const unitPrice = parseFloat(unitPriceInput.value.replace(/,/g, '')) || 0;
            const rowTotal = Num * unitPrice;
            
            totalCell.textContent = rowTotal.toLocaleString();
            total += rowTotal;
        }
    });
    
    document.getElementById('MaterialTotal').textContent = total.toLocaleString();
    updateBudgetSummary();
}

function checkMaterialLimit(rowIndex) {
    const MaterialUnitPriceInput = document.getElementById(`MaterialUnitPrice${rowIndex}`);
    const ddl = document.getElementById(`MaterialUnit${rowIndex}`);
    const selectedCode = ddl.value;
    const MaterialUnitPrice = parseInt(MaterialUnitPriceInput.value, 10);

    const selectedItem = ddlMaterialOptions.find(x => x.value === selectedCode);
    if (selectedItem && MaterialUnitPrice > selectedItem.maxLimit && selectedItem.maxLimit != 0) {
        Swal.fire({
            icon: 'warning',
            title: '金額超過上限',
            text: `輸入金額 ${MaterialUnitPrice} 超過上限：${selectedItem.maxLimit}`,
            confirmButtonText: '確定'
        });
        MaterialUnitPriceInput.value = selectedItem.maxLimit;
    }
}

function M_addNewRow() {
    const table = document.querySelector('.Material tbody');
    const totalRow = table.querySelector('.total-row');
    const rowCount = table.children.length;
    
    const ddlSelect = document.createElement('select');
    ddlSelect.className = "form-select";
    ddlSelect.id = `MaterialUnit${rowCount}`;
    
    ddlMaterialOptions.forEach(option => {
        const opt = document.createElement('option');
        opt.value = option.value;
        opt.textContent = option.text;
        ddlSelect.appendChild(opt);
    });
    
    ddlSelect.addEventListener("change", function () {
        document.getElementById(`MaterialUnitPrice${rowCount}`).value = 0;
        calculateMaterial();
    });
    
    const newRow = document.createElement('tr');
    newRow.innerHTML = `
        <td><input type="text" id="MaterialName${rowCount}" class="form-control" placeholder="請輸入" /></td>
        <td><input type="text" id="MaterialDescription${rowCount}" class="form-control" placeholder="請輸入" /></td>
        <td></td>
        <td><input id="MaterialNum${rowCount}" class="form-control text-end" placeholder="請輸入" onkeypress="return event.charCode != 45" onblur="calculateMaterial()" /></td>
        <td><input id="MaterialUnitPrice${rowCount}" class="form-control text-end" placeholder="請輸入" onkeypress="return event.charCode != 45" onblur="checkMaterialLimit(${rowCount});calculateMaterial()" /></td>
        <td class="text-end">0</td>
        <td>
            <button type="button" class="btn btn-sm btn-teal" onclick="M_deleteRow(this)"><i class="fas fa-trash-alt"></i></button>
            <button type="button" class="btn btn-sm btn-teal" onclick="M_addNewRow()"><i class="fas fa-plus"></i></button>
        </td>
    `;
    
    table.insertBefore(newRow, totalRow);
    newRow.children[2].appendChild(ddlSelect);
}

// 3.技術移轉、委託研究或驗證費
function calculateResearch() {
    let total = 0;
    document.querySelectorAll('.money').forEach(input => {
        let raw = input.value.replace(/,/g, '');
        let val = parseInt(raw) || 0;
        input.value = val.toLocaleString();
        total += val;
    });

    document.getElementById("ResearchFeesTotal").innerText = total.toLocaleString();
    updateBudgetSummary();
}

// 4.國內差旅費
function calculateTravel() {
    let total = 0;
    const prices = document.querySelectorAll('.travel .price');

    prices.forEach(priceInput => {
        const raw = priceInput.value.replace(/,/g, '');
        const value = parseInt(raw) || 0;
        total += value;
        priceInput.value = value > 0 ? value.toLocaleString() : '';
    });

    document.getElementById('travelTotal').innerText = total.toLocaleString();
    updateBudgetSummary();
}

function T_DeleteRow(button) {
    const table = document.querySelector('.travel tbody');
    const dataRows = table.querySelectorAll('tr:not(.total-row)');

    if (dataRows.length <= 1) {
        Swal.fire({
            icon: 'warning',
            title: '無法刪除',
            text: '至少需保留一行資料',
            confirmButtonText: '確定'
        });
        return;
    }

    Swal.fire({
        icon: 'question',
        title: '確定要刪除嗎？',
        text: '確定要刪除此行資料嗎？',
        showCancelButton: true,
        confirmButtonText: '確定',
        cancelButtonText: '取消'
    }).then((result) => {
        if (result.isConfirmed) {
            button.closest('tr').remove();
            calculateTravel();
        }
    });
}

function T_addRow() {
    const table = document.querySelector('.travel tbody');
    const totalRow = table.querySelector('.total-row');
    const rowCount = table.children.length;
    
    const newRow = document.createElement('tr');
    newRow.innerHTML = `
        <td><input type="text" ID="travelReason${rowCount}" class="form-control" /></td>
        <td><input type="text" ID="travelArea${rowCount}" class="form-control" /></td>
        <td><input  ID="travelDays${rowCount}" class="form-control days" onkeypress="return event.charCode != 45" /></td>
        <td><input  ID="travelPeople${rowCount}" class="form-control people" onkeypress="return event.charCode != 45" /></td>
        <td><input  ID="travelPrice${rowCount}" class="form-control text-end price" onkeypress="return event.charCode != 45" onblur="calculateTravel()" /></td>
        <td>
            <button type="button" class="btn btn-sm btn-teal" onclick="T_DeleteRow(this)"><i class="fas fa-trash-alt"></i></button>
            <button type="button" class="btn btn-sm btn-teal" onclick="T_addRow()"><i class="fas fa-plus"></i></button>
        </td>
    `;
    table.insertBefore(newRow, totalRow);
}

// 5.其他業務費
function calculateOther() {
    let total = 0;
    const dataRows = document.querySelectorAll('.other tbody tr:not(.total-row)');
    
    dataRows.forEach((row) => {
        const avgSalaryInput = row.cells[1].querySelector('input');
        const monthInput = row.cells[2].querySelector('input');
        const peopleInput = row.cells[3].querySelector('input');
        const totalCell = row.cells[4];
        
        if (avgSalaryInput && monthInput && peopleInput) {
            const avgSalary = parseFloat(avgSalaryInput.value) || 0;
            const month = parseFloat(monthInput.value) || 0;
            const people = parseFloat(peopleInput.value) || 0;
            const rowTotal = (avgSalary * month * people);
            
            totalCell.textContent = rowTotal.toLocaleString();
            total += rowTotal;
        }
    });
    
    document.getElementById('otherTotal').textContent = total.toLocaleString();
    document.getElementById('serviceCash').textContent = total.toLocaleString();
    
    generateServiceDescription(total);
    calculateOtherRentTotal();
    updateBudgetSummary();
}

function generateServiceDescription(total) {
    const dataRows = document.querySelectorAll('.other tbody tr:not(.total-row)');
    let descriptionLines = [];
    
    dataRows.forEach((row) => {
        const jobTitleSelect = row.cells[0].querySelector('select');
        const avgSalaryInput = row.cells[1].querySelector('input');
        const monthInput = row.cells[2].querySelector('input');
        const peopleInput = row.cells[3].querySelector('input');
        
        if (jobTitleSelect && avgSalaryInput && monthInput && peopleInput) {
            const jobTitle = jobTitleSelect.options[jobTitleSelect.selectedIndex]?.text || '';
            const avgSalary = parseFloat(avgSalaryInput.value) || 0;
            const month = parseFloat(monthInput.value) || 0;
            const people = parseFloat(peopleInput.value) || 0;
            
            if (jobTitle && avgSalary > 0 && month > 0 && people > 0) {
                const salaryInThousands = (avgSalary / 1000).toFixed(1);
                const line = `${jobTitle} 人員${salaryInThousands}千元*${month}月*${people}人`;
                descriptionLines.push(line);
            }
        }
    });
    
    if (total > 0) {
        const totalInThousands = (total / 1000).toFixed(0);
        descriptionLines.push(`總計: ${totalInThousands}千元`);
    }

    document.getElementById('serviceDescription').textContent = descriptionLines.join(',\n');
}

function calculateOtherRentTotal() {
    const rentCashInput = document.getElementById('rentCash');
    const rentAmount = parseFloat(rentCashInput?.value?.replace(/,/g, '') || '0');
    
    const serviceCashSpan = document.getElementById('serviceCash');
    const serviceAmount = parseFloat(serviceCashSpan?.textContent?.replace(/,/g, '') || '0');
    
    const total = rentAmount + serviceAmount;
    
    document.getElementById('otherRentTotal').textContent = total.toLocaleString();
    
    updateAmountA('5', total);
    updateItemTotals();
    updateGrandTotals();
    updatePercentages();
}

function O_DeleteRow(button) {
    const table = document.querySelector('.other tbody');
    const dataRows = table.querySelectorAll('tr:not(.total-row)');

    if (dataRows.length <= 1) {
        Swal.fire({
            icon: 'warning',
            title: '無法刪除',
            text: '至少需保留一行資料',
            confirmButtonText: '確定'
        });
        return;
    }

    Swal.fire({
        icon: 'question',
        title: '確定要刪除嗎？',
        text: '確定要刪除此行資料嗎？',
        showCancelButton: true,
        confirmButtonText: '確定',
        cancelButtonText: '取消'
    }).then((result) => {
        if (result.isConfirmed) {
            button.closest('tr').remove();
            calculateOther();
        }
    });
}

function O_addRow() {
    const table = document.querySelector('.other tbody');
    const totalRow = table.querySelector('.total-row');
    const rowCount = table.children.length;
    
    const ddlSelect = document.createElement('select');
    ddlSelect.className = "form-select";
    ddlSelect.id = `otherJobTitle${rowCount}`;
    
    ddlOtherOptions.forEach(option => {
        const opt = document.createElement('option');
        opt.value = option.value;
        opt.textContent = option.text;
        ddlSelect.appendChild(opt);
    });
    
    ddlSelect.addEventListener("change", function () {
        calculateOther();
    });

    const newRow = document.createElement('tr');
    newRow.innerHTML = `
        <td></td>
        <td><input ID="otherAvgSalary${rowCount}" class="form-control text-end" onkeypress="return event.charCode != 45" onblur="calculateOther()"/></td>
        <td><input ID="otherMonth${rowCount}" class="form-control text-end" onkeypress="return event.charCode != 45" onblur="calculateOther()" /> </td>
        <td><input ID="otherPeople${rowCount}" class="form-control text-end" onkeypress="return event.charCode != 45" onblur="calculateOther()" /></td>
        <td class="text-end">0</td>
        <td>
            <button type="button" class="btn btn-sm btn-teal" onclick="O_DeleteRow(this)"><i class="fas fa-trash-alt"></i></button>
            <button type="button" class="btn btn-sm btn-teal" onclick="O_addRow()"><i class="fas fa-plus"></i></button>
        </td>
    `;
    table.insertBefore(newRow, totalRow);
    newRow.children[0].appendChild(ddlSelect);
}

// 6.經費總表自動更新功能
function updateBudgetSummary() {
    const personTotal = parseFloat($('#PersonTotal').text().replace(/,/g, '') || '0');
    const materialTotal = parseFloat($('#MaterialTotal').text().replace(/,/g, '') || '0');
    const researchTotal = parseFloat($('#ResearchFeesTotal').text().replace(/,/g, '') || '0');
    const travelTotal = parseFloat($('#travelTotal').text().replace(/,/g, '') || '0');
    const otherRentTotal = parseFloat($('#otherRentTotal').text().replace(/,/g, '') || '0');
    
    updateAmountA('1', personTotal);
    updateAmountA('2', materialTotal);
    updateAmountA('3', researchTotal);
    updateAmountA('4', travelTotal);
    updateAmountA('5', otherRentTotal);
    
    updateItemTotals();
    updateGrandTotals();
    updatePercentages();
}

function updateAmountA(rowIndex, amount) {
    $('.main-table tbody tr').each(function() {
        const firstCell = $(this).children().first().text();
        if (firstCell.includes(`${rowIndex}.`)) {
            const $amountACell = $(this).find('.amount-a');
            if ($amountACell.length) {
                $amountACell.text(amount.toLocaleString());
            }
        }
    });
}

function updateItemTotals() {
    $('.main-table tbody tr:not(.total-row):not(.percentage-row)').each(function() {
        const $amountACell = $(this).find('.amount-a');
        const $amountBCell = $(this).find('.amount-b');
        const $totalCell = $(this).find('.amount-total');
        
        if ($amountACell.length && $amountBCell.length && $totalCell.length) {
            let amountA = 0;
            const $amountAInput = $amountACell.find('input');
            if ($amountAInput.length) {
                let aValue = $amountAInput.val().replace(/,/g, '');
                amountA = parseFloat(aValue) || 0;
                if (amountA >= 0) {
                    $amountAInput.val(amountA.toLocaleString());
                }
            } else {
                amountA = parseFloat($amountACell.text().replace(/,/g, '') || '0');
            }
            
            const $amountBInput = $amountBCell.find('input');
            let amountB = 0;
            if ($amountBInput.length) {
                let bValue = $amountBInput.val().replace(/,/g, '');
                amountB = parseFloat(bValue) || 0;
                if (amountB >= 0) {
                    $amountBInput.val(amountB.toLocaleString());
                }
            }
            
            const total = amountA + amountB;
            $totalCell.text(total.toLocaleString());
        }
    });
}

function updateGrandTotals() {
    let totalA = 0;
    let totalB = 0;
    let totalC = 0;

    $('.main-table tbody tr:not(.total-row):not(.percentage-row)').each(function() {
        const $amountACell = $(this).find('.amount-a');
        const $amountBCell = $(this).find('.amount-b');
        const $totalCell = $(this).find('.amount-total');

        if ($amountACell.length && $amountBCell.length && $totalCell.length) {
            let amountA = 0;
            const $amountAInput = $amountACell.find('input');
            if ($amountAInput.length) {
                amountA = parseFloat($amountAInput.val().replace(/,/g, '') || '0');
            } else {
                amountA = parseFloat($amountACell.text().replace(/,/g, '') || '0');
            }

            const $amountBInput = $amountBCell.find('input');
            const amountB = parseFloat($amountBInput.val().replace(/,/g, '') || '0');
            const amountC = parseFloat($totalCell.text().replace(/,/g, '') || '0');

            totalA += amountA;
            totalB += amountB;
            totalC += amountC;
        }
    });

    $('.main-table tbody tr').each(function() {
        if ($(this).text().includes('經費總計')) {
            const $cells = $(this).find('.number-cell');
            if ($cells.length >= 3) {
                // 檢查補助款是否超過上限
                // grantLimit 單位：萬元，需要 * 10000 轉為元
                const grantLimit = window.grantLimitSettings ? window.grantLimitSettings.grantLimit : 0;
                const grantLimitInYuan = grantLimit * 10000; // 轉換為元

                // 判斷是否超過上限，決定顏色
                if (grantLimitInYuan > 0 && totalA > grantLimitInYuan) {
                    $cells.eq(0).html(`<span style="color: red;">${totalA.toLocaleString()}</span><br>(I)`);
                } else {
                    $cells.eq(0).html(`${totalA.toLocaleString()}<br>(I)`);
                }

                $cells.eq(1).text(totalB.toLocaleString());
                $cells.eq(2).html(`${totalC.toLocaleString()}<br>(II)`);
            }
        }
    });
}

function updatePercentages() {
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
    
    if (!totalRow) return;
    
    const totalCells = totalRow.querySelectorAll('.number-cell');
    if (totalCells.length < 3) return;
    
    const totalI = parseFloat(totalCells[0].textContent?.replace(/,/g, '').replace(/\(I\)/g, '').replace(/<br>/g, '') || '0');
    const totalB = parseFloat(totalCells[1].textContent?.replace(/,/g, '') || '0');
    const totalII = parseFloat(totalCells[2].textContent?.replace(/,/g, '').replace(/\(II\)/g, '').replace(/<br>/g, '') || '0');
    
    const itemRows = document.querySelectorAll('.main-table tbody tr:not(.total-row):not(.percentage-row)');
    
    itemRows.forEach(row => {
        const amountACell = row.querySelector('.amount-a');
        const totalCell = row.querySelector('.amount-total');
        const allCells = row.querySelectorAll('th, td');
        
        if (amountACell && totalCell && allCells.length >= 6) {
            let amountA = 0;
            const amountAInput = amountACell.querySelector('input');
            if (amountAInput) {
                amountA = parseFloat(amountAInput.value?.replace(/,/g, '') || '0');
            } else {
                amountA = parseFloat(amountACell.textContent?.replace(/,/g, '') || '0');
            }
            
            const amountC = parseFloat(totalCell.textContent?.replace(/,/g, '') || '0');
            
            const percentageC_II = totalII > 0 ? ((amountC / totalII) * 100).toFixed(2) + '%' : '0%';
            const percentageA_I = totalI > 0 ? ((amountA / totalI) * 100).toFixed(2) + '%' : '0%';
            
            if (allCells[4]) {
                allCells[4].textContent = percentageC_II;
            }
            if (allCells[5]) {
                allCells[5].textContent = percentageA_I;
            }
        }
    });
    
    if (percentageRow && totalII > 0) {
        const percentageCells = percentageRow.querySelectorAll('.number-cell');
        if (percentageCells.length >= 3) {
            const percentageA = ((totalI / totalII) * 100).toFixed(2) + '%';
            const percentageB = ((totalB / totalII) * 100).toFixed(2) + '%';
            
            percentageCells[0].textContent = percentageA;
            percentageCells[1].textContent = percentageB;
            percentageCells[2].textContent = '100%';
        }
    }
}

// 載入現有資料到表單
function loadExistingDataToForm() {
    if (typeof window.loadedData === 'undefined') {
        return; // 如果沒有載入資料，則不處理
    }
    
    try {
        // 按序載入各項資料，使用 Promise 鏈確保順序
        Promise.resolve()
            .then(() => loadPersonnelData(window.loadedData.personnel))
            .then(() => new Promise(resolve => setTimeout(resolve, 50))) // 等待 DOM 更新
            .then(() => loadMaterialData(window.loadedData.material))
            .then(() => new Promise(resolve => setTimeout(resolve, 50))) // 等待 DOM 更新
            .then(() => loadResearchData(window.loadedData.research))
            .then(() => new Promise(resolve => setTimeout(resolve, 50))) // 等待 DOM 更新
            .then(() => loadTravelData(window.loadedData.travel))
            .then(() => new Promise(resolve => setTimeout(resolve, 50))) // 等待 DOM 更新
            .then(() => loadOtherData(window.loadedData.other))
            .then(() => new Promise(resolve => setTimeout(resolve, 50))) // 等待 DOM 更新
            .then(() => loadOtherRentData(window.loadedData.otherRent))
            .then(() => new Promise(resolve => setTimeout(resolve, 50))) // 等待 DOM 更新
            .then(() => loadTotalFeesData(window.loadedData.totalFees))
            .then(() => new Promise(resolve => setTimeout(resolve, 100))) // 等待所有資料載入完成
            .then(() => {
                // 重新計算所有總計
                calculateAndUpdateTotal();
                calculateMaterial();
                calculateResearch();
                calculateTravel();
                calculateOther();
                calculateOtherRentTotal();
                updateBudgetSummary();
            })
            .catch(error => {
                console.error('載入資料時發生錯誤:', error);
            });
        
    } catch (error) {
        console.error('載入資料時發生錯誤:', error);
    }
}

// 載入人員資料
function loadPersonnelData(personnelData) {
    if (!personnelData || personnelData.length === 0) return Promise.resolve();
    
    return new Promise((resolve) => {
        const table = document.querySelector('.person tbody');
        const totalRow = table.querySelector('.total-row');
        
        // 清除所有現有的資料行（只保留總計行）
        const existingRows = table.querySelectorAll('tr:not(.total-row)');
        existingRows.forEach(row => row.remove());
        
        // 新增所有需要的行（包含第一行）
        for (let i = 0; i < personnelData.length; i++) {
            P_addNewRow();
        }
        
        // 延遲填入資料，確保所有行都已建立
        setTimeout(() => {
            personnelData.forEach((person, index) => {
                const rowIndex = index + 1;
                
                const nameInput = document.getElementById(`personName${rowIndex}`);
                const stayCheckbox = document.getElementById(`stay${rowIndex}`);
                const titleSelect = document.getElementById(`ddlPerson${rowIndex}`);
                const salaryInput = document.getElementById(`personSalary${rowIndex}`);
                const monthsInput = document.getElementById(`personMonths${rowIndex}`);
                
                if (nameInput) nameInput.value = person.name || '';
                if (stayCheckbox) stayCheckbox.checked = person.stay || false;
                if (titleSelect) titleSelect.value = person.title || '';
                if (salaryInput) salaryInput.value = person.salary || '0';
                if (monthsInput) monthsInput.value = person.months || '0';
            });
            
            resolve();
        }, 50);
    });
}

// 載入材料資料
function loadMaterialData(materialData) {
    if (!materialData || materialData.length === 0) return Promise.resolve();
    
    return new Promise((resolve) => {
        const table = document.querySelector('.Material tbody');
        const totalRow = table.querySelector('.total-row');
        
        // 清除所有現有的資料行（只保留總計行）
        const existingRows = table.querySelectorAll('tr:not(.total-row)');
        existingRows.forEach(row => row.remove());
        
        // 新增所有需要的行（包含第一行）
        for (let i = 0; i < materialData.length; i++) {
            M_addNewRow();
        }
        
        // 延遲填入資料，確保所有行都已建立
        setTimeout(() => {
            materialData.forEach((material, index) => {
                const rowIndex = index + 1;
                
                const nameInput = document.getElementById(`MaterialName${rowIndex}`);
                const descInput = document.getElementById(`MaterialDescription${rowIndex}`);
                const unitSelect = document.getElementById(`MaterialUnit${rowIndex}`);
                const numInput = document.getElementById(`MaterialNum${rowIndex}`);
                const priceInput = document.getElementById(`MaterialUnitPrice${rowIndex}`);
                
                if (nameInput) nameInput.value = material.name || '';
                if (descInput) descInput.value = material.description || '';
                if (unitSelect) unitSelect.value = material.unit || '';
                if (numInput) numInput.value = material.quantity || '0';
                if (priceInput) priceInput.value = material.unitPrice || '0';
            });
            
            resolve();
        }, 50);
    });
}

// 載入研究費資料
function loadResearchData(researchData) {
    if (!researchData || researchData.length === 0) return Promise.resolve();

    return new Promise((resolve) => {
        researchData.forEach((research) => {
            if (research.category === '技術移轉') {
                const startDateInput = document.getElementById('txtDate1Start');
                const endDateInput = document.getElementById('txtDate1End');
                const nameInput = document.getElementById('ResearchFeesName1');
                const personInput = document.getElementById('ResearchFeesPersonName1');
                const priceInput = document.getElementById('ResearchFeesPrice1');

                // 處理日期格式 - 將西元年轉換為民國年顯示
                if (startDateInput && research.dateStart) {
                    const displayDate = convertToMinguoFormat(research.dateStart);
                    startDateInput.value = displayDate;
                    $(startDateInput).data('gregorian-date', research.dateStart);
                    // 設定隱藏欄位
                    $('#hdnDate1Start').val(research.dateStart);
                }
                if (endDateInput && research.dateEnd) {
                    const displayDate = convertToMinguoFormat(research.dateEnd);
                    endDateInput.value = displayDate;
                    $(endDateInput).data('gregorian-date', research.dateEnd);
                    // 設定隱藏欄位
                    $('#hdnDate1End').val(research.dateEnd);
                }
                if (nameInput) nameInput.value = research.projectName || '';
                if (personInput) personInput.value = research.targetPerson || '';
                if (priceInput) priceInput.value = research.price || '0';
            } else if (research.category === '委託研究') {
                const startDateInput = document.getElementById('txtDate2Start');
                const endDateInput = document.getElementById('txtDate2End');
                const nameInput = document.getElementById('ResearchFeesName2');
                const personInput = document.getElementById('ResearchFeesPersonName2');
                const priceInput = document.getElementById('ResearchFeesPrice2');

                // 處理日期格式 - 將西元年轉換為民國年顯示
                if (startDateInput && research.dateStart) {
                    const displayDate = convertToMinguoFormat(research.dateStart);
                    startDateInput.value = displayDate;
                    $(startDateInput).data('gregorian-date', research.dateStart);
                    // 設定隱藏欄位
                    $('#hdnDate2Start').val(research.dateStart);
                }
                if (endDateInput && research.dateEnd) {
                    const displayDate = convertToMinguoFormat(research.dateEnd);
                    endDateInput.value = displayDate;
                    $(endDateInput).data('gregorian-date', research.dateEnd);
                    // 設定隱藏欄位
                    $('#hdnDate2End').val(research.dateEnd);
                }
                if (nameInput) nameInput.value = research.projectName || '';
                if (personInput) personInput.value = research.targetPerson || '';
                if (priceInput) priceInput.value = research.price || '0';
            }
        });

        // 載入完成後同步隱藏欄位
        setTimeout(() => {
            syncLoadedDatesToHiddenFields();
            resolve();
        }, 50);
    });
}

// 輔助函數：將西元年日期轉換為民國年格式
function  convertToMinguoFormat(gregorianDate) {
    if (!gregorianDate) return '';

    try {
        // 先嘗試用多種格式解析日期
        let momentObj = null;

        if (typeof moment !== 'undefined') {
            // 嘗試不同的日期格式
            const possibleFormats = [
                'YYYY/MM/DD',
                'YYYY-MM-DD',
                'YYYY/M/D',
                'YYYY-M-D',
                'MM/DD/YYYY',
                'M/D/YYYY'
            ];

            // 先嘗試直接解析
            momentObj = moment(gregorianDate);

            // 如果無效，嘗試特定格式
            if (!momentObj.isValid()) {
                for (let format of possibleFormats) {
                    momentObj = moment(gregorianDate, format);
                    if (momentObj.isValid()) {
                        break;
                    }
                }
            }

            // 如果解析成功，轉換為民國年
            if (momentObj.isValid()) {
                return momentObj.format('tYY/MM/DD');
            }
        }

        // 如果 moment.js 解析失敗，使用原生 JavaScript
        const date = new Date(gregorianDate);
        if (!isNaN(date.getTime())) {
            const minguoYear = date.getFullYear() - 1911;
            const month = String(date.getMonth() + 1).padStart(2, '0');
            const day = String(date.getDate()).padStart(2, '0');
            return `${minguoYear}/${month}/${day}`;
        }

        // 如果都失敗，返回原始值
        return gregorianDate;

    } catch (error) {
        console.error('日期轉換錯誤:', error, '輸入值:', gregorianDate);
        return gregorianDate; // 如果轉換失敗，返回原始值
    }
}

// 載入差旅費資料
function loadTravelData(travelData) {
    if (!travelData || travelData.length === 0) return Promise.resolve();
    
    return new Promise((resolve) => {
        const table = document.querySelector('.travel tbody');
        const totalRow = table.querySelector('.total-row');
        
        // 清除所有現有的資料行（只保留總計行）
        const existingRows = table.querySelectorAll('tr:not(.total-row)');
        existingRows.forEach(row => row.remove());
        
        // 新增所有需要的行（包含第一行）
        for (let i = 0; i < travelData.length; i++) {
            T_addRow();
        }
        
        // 延遲填入資料，確保所有行都已建立
        setTimeout(() => {
            travelData.forEach((travel, index) => {
                const rowIndex = index + 1;
                
                const reasonInput = document.getElementById(`travelReason${rowIndex}`);
                const areaInput = document.getElementById(`travelArea${rowIndex}`);
                const daysInput = document.getElementById(`travelDays${rowIndex}`);
                const peopleInput = document.getElementById(`travelPeople${rowIndex}`);
                const priceInput = document.getElementById(`travelPrice${rowIndex}`);
                
                if (reasonInput) reasonInput.value = travel.reason || '';
                if (areaInput) areaInput.value = travel.area || '';
                if (daysInput) daysInput.value = travel.days || '0';
                if (peopleInput) peopleInput.value = travel.people || '0';
                if (priceInput) priceInput.value = travel.price || '0';
            });
            
            resolve();
        }, 50);
    });
}

// 載入其他人事費資料
function loadOtherData(otherData) {
    if (!otherData || otherData.length === 0) return Promise.resolve();
    
    return new Promise((resolve) => {
        const table = document.querySelector('.other tbody');
        const totalRow = table.querySelector('.total-row');
        
        // 清除所有現有的資料行（只保留總計行）
        const existingRows = table.querySelectorAll('tr:not(.total-row)');
        existingRows.forEach(row => row.remove());
        
        // 新增所有需要的行（包含第一行）
        for (let i = 0; i < otherData.length; i++) {
            O_addRow();
        }
        
        // 延遲填入資料，確保所有行都已建立
        setTimeout(() => {
            otherData.forEach((other, index) => {
                const rowIndex = index + 1;
                
                const titleSelect = document.getElementById(`otherJobTitle${rowIndex}`);
                const salaryInput = document.getElementById(`otherAvgSalary${rowIndex}`);
                const monthInput = document.getElementById(`otherMonth${rowIndex}`);
                const peopleInput = document.getElementById(`otherPeople${rowIndex}`);
                
                if (titleSelect) titleSelect.value = other.title || '';
                if (salaryInput) salaryInput.value = other.avgSalary || '0';
                if (monthInput) monthInput.value = other.months || '0';
                if (peopleInput) peopleInput.value = other.people || '0';
            });
            
            resolve();
        }, 50);
    });
}

// 載入其他租金資料
function loadOtherRentData(otherRentData) {
    if (!otherRentData || otherRentData.length === 0) return Promise.resolve();
    
    return new Promise((resolve) => {
        otherRentData.forEach((rent) => {
            if (rent.item === '租金') {
                const rentCashInput = document.getElementById('rentCash');
                const rentDescInput = document.getElementById('rentDescription');
                
                if (rentCashInput) rentCashInput.value = rent.amount || '0';
                if (rentDescInput) rentDescInput.value = rent.note || '';
            }
        });
        
        resolve();
    });
}

// 載入經費總表資料
function loadTotalFeesData(totalFeesData) {
    if (!totalFeesData || totalFeesData.length === 0) return Promise.resolve();

    return new Promise((resolve) => {
        const rows = document.querySelectorAll('.main-table tbody tr:not(.total-row):not(.percentage-row)');

        totalFeesData.forEach((totalFee) => {
            rows.forEach(row => {
                const firstCell = row.children[0]?.textContent;
                if (firstCell?.includes(totalFee.accountingItem)) {
                    const amountBCell = row.querySelector('.amount-b');
                    const amountBInput = amountBCell?.querySelector('input');

                    if (amountBInput) {
                        amountBInput.value = totalFee.coopAmount || '0';
                    }

                    // 如果是行政管理費，也需要設定補助款
                    if (totalFee.accountingItem.includes('行政管理費')) {
                        const amountACell = row.querySelector('.amount-a');
                        const amountAInput = amountACell?.querySelector('input');
                        if (amountAInput) {
                            amountAInput.value = totalFee.subsidyAmount || '0';
                        }
                    }
                }
            });
        });

        resolve();
    });
}

// 根據 OrgCategory 決定是否隱藏行政管理費
function handleAdminFeeVisibility() {
    try {
        // 檢查 window.loadedData 是否存在並包含 orgCategory
        if (typeof window.loadedData === 'undefined' || !window.loadedData.orgCategory) {
            return;
        }

        const orgCategory = window.loadedData.orgCategory;
        const isOceanTech = orgCategory.toLowerCase() === 'oceantech';

        // 找到行政管理費的行
        const rows = document.querySelectorAll('.main-table tbody tr:not(.total-row):not(.percentage-row)');
        let adminFeeRow = null;

        rows.forEach(row => {
            const firstCell = row.children[0]?.textContent;
            if (firstCell && firstCell.includes('6.行政管理費')) {
                adminFeeRow = row;
            }
        });

        if (adminFeeRow && isOceanTech) {
            // 隱藏行政管理費行
            adminFeeRow.style.display = 'none';

            // 將行政管理費的輸入欄位設為 0（確保儲存時為 0）
            const amountAInput = adminFeeRow.querySelector('.amount-a input');
            const amountBInput = adminFeeRow.querySelector('.amount-b input');

            if (amountAInput) {
                amountAInput.value = '0';
            }
            if (amountBInput) {
                amountBInput.value = '0';
            }

            console.log('已隱藏行政管理費行 (OrgCategory = OceanTech)');
        } else if (adminFeeRow && !isOceanTech) {
            // 確保行顯示（非 OceanTech 時）
            adminFeeRow.style.display = '';
            console.log('顯示行政管理費行 (OrgCategory ≠ OceanTech)');
        }
    } catch (error) {
        console.error('處理行政管理費顯示狀態時發生錯誤:', error);
    }
}

// 全域旗標:標記使用者是否已確認送出
window.isConfirmedToSubmit = false;

// 處理「完成本頁，下一步」按鈕點擊
function handleSaveAndNextClick() {
    try {
        // 如果已經確認過,直接允許提交
        if (window.isConfirmedToSubmit) {
            window.isConfirmedToSubmit = false; // 重置旗標
            return true; // 允許 PostBack
        }

        // 先收集表單資料
        collectAllFormData();

        // 同步變更說明資料
        if (typeof syncAllChangeDescriptions === 'function') {
            syncAllChangeDescriptions();
        }

        // 取得補助款總計 (I) 和配合款比例
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

        if (!totalRow || !percentageRow) {
            return true; // 找不到資料，直接提交
        }

        // 取得補助款總計 (I)
        const totalCells = totalRow.querySelectorAll('.number-cell');
        const totalIText = totalCells[0]?.textContent || '';
        const totalI = parseFloat(totalIText.replace(/,/g, '').replace(/\(I\)/g, '').replace(/<br>/g, '').replace(/<[^>]*>/g, '').replace(/\s/g, '') || '0');

        // 取得配合款百分比
        const percentageCells = percentageRow.querySelectorAll('.number-cell');
        const coopPercentage = parseFloat(percentageCells[1]?.textContent?.replace(/%/g, '') || '0');

        // 取得設定值
        const grantLimit = window.grantLimitSettings ? window.grantLimitSettings.grantLimit : 0;
        const matchingFund = window.grantLimitSettings ? window.grantLimitSettings.matchingFund : 0;
        const grantLimitInYuan = grantLimit * 10000; // 轉換為元

        let warningMessages = [];

        // 檢查 1: 補助款是否超過上限
        if (grantLimitInYuan > 0 && totalI > grantLimitInYuan) {
            warningMessages.push(`總補助款以不超過${grantLimit}萬元為原則。`);
        }

        // 檢查 2: 配合款比例是否未達標準
        if (matchingFund > 0 && coopPercentage < matchingFund) {
            warningMessages.push(`配合款比例未達標準（應高於 ${matchingFund}%）。`);
        }

        // 組合訊息
        let message = '';
        if (warningMessages.length > 0) {
            message = warningMessages.join('\n') + '\n\n是否提交下一步？';
        } else {
            message = '是否提交下一步？';
        }

        // 顯示確認對話框
        Swal.fire({
            title: warningMessages.length > 0 ? '提醒' : '確認',
            text: message,
            icon: warningMessages.length > 0 ? 'warning' : 'question',
            showCancelButton: true,
            confirmButtonText: '確定送出',
            cancelButtonText: '取消',
            customClass: {
                popup: 'animated fadeInDown'
            }
        }).then((result) => {
            if (result.isConfirmed) {
                // 使用者確認後,設定旗標並重新觸發按鈕
                window.isConfirmedToSubmit = true;

                // 再次同步變更說明(確保最新)
                if (typeof syncAllChangeDescriptions === 'function') {
                    syncAllChangeDescriptions();
                }

                // 重新觸發按鈕點擊,這次會因為旗標為 true 而執行 PostBack
                const btn = document.querySelector('[id*="tab3_btnSaveAndNextHidden"]');
                if (btn) {
                    btn.click();
                }
            }
        });

        return false; // 第一次點擊:阻止 PostBack,等待使用者確認
    } catch (error) {
        console.error('處理送出驗證時發生錯誤:', error);
        return true; // 發生錯誤時直接提交
    }
}