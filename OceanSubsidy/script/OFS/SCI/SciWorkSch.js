
//#region 工作項目表格管理器
class WorkItemTableManager {
    constructor(parentManager) {
        this.parent = parentManager;
        this.currentLetters = ['A', 'B'];
        this.init();
    }

    init() {
        this.bindEvents();
        this.updatePlusButtonVisibility();
    }

    bindEvents() {
        // 綁定新增工作項目按鈕
        const addWorkItemBtns = document.querySelectorAll('.btn-blue-green2[type="button"]');
        addWorkItemBtns.forEach(btn => {
            if (btn.textContent.includes('新增工作項目')) {
                btn.addEventListener('click', () => this.addNewWorkItem());
            }
        });

        this.bindPlusButtons();
        this.bindDeleteButtons();
        this.bindWorkItemInputs();
    }

    bindPlusButtons() {
        const workItemTable = document.querySelector('.sub-table');
        if (workItemTable) {
            const plusButtons = workItemTable.querySelectorAll('.btn-dark-green2 .fa-plus');
            plusButtons.forEach(btn => {
                const button = btn.closest('button');
                if (!button.hasAttribute('data-bound')) {
                    button.addEventListener('click', (e) => this.addSubItem(e));
                    button.setAttribute('data-bound', 'true');
                }
            });
        }
    }

    bindDeleteButtons() {
        const workItemTable = document.querySelector('.sub-table');
        if (workItemTable) {
            const deleteButtons = workItemTable.querySelectorAll('.btn-dark-green2 .fa-trash-alt');
            deleteButtons.forEach(btn => {
                const button = btn.closest('button');
                if (!button.hasAttribute('data-bound')) {
                    button.addEventListener('click', (e) => this.deleteItem(e));
                    button.setAttribute('data-bound', 'true');
                }
            });
        }
    }

    bindWorkItemInputs() {
        const allRows = document.querySelectorAll('.sub-table tbody tr');
        allRows.forEach(row => {
            const cell = row.cells[0];
            const code = cell ? cell.textContent.trim() : '';

            if (/^[A-Z]\d+$/.test(code)) {
                const workItemInput = row.cells[1] ? row.cells[1].querySelector('input[type="text"]') : null;
                if (workItemInput && !workItemInput.hasAttribute('data-workitem-bound')) {
                    workItemInput.addEventListener('input', () => this.parent.checkStandards.updateCheckpointOptions());
                    workItemInput.setAttribute('data-workitem-bound', 'true');
                }
            }
        });
    }

    getNextLetter() {
        const lastLetter = this.currentLetters[this.currentLetters.length - 1];
        return String.fromCharCode(lastLetter.charCodeAt(0) + 1);
    }

    addNewWorkItem() {
        this.updateCurrentLetters();

        if (this.currentLetters.length >= 26) {
            alert('已達到工作項目上限（最多26個項目：A到Z）');
            return;
        }

        const nextLetter = this.getNextLetter();
        if (nextLetter.charCodeAt(0) > 90) {
            alert('已達到工作項目上限（最多26個項目：A到Z）');
            return;
        }

        this.currentLetters.push(nextLetter);
        const newTbody = this.createNewWorkItemGroup(nextLetter);

        const tfoot = document.querySelector('.sub-table table tfoot');
        tfoot.parentNode.insertBefore(newTbody, tfoot);

        this.updatePlusButtonVisibility();
        this.rebindAllEvents();
        this.parent.calculator.updateTotals();
    }

    createNewWorkItemGroup(letter) {
        const tbody = document.createElement('tbody');
        const mainRow = this.createMainItemRow(letter);
        const subRow = this.createSubItemRow(letter, 1, true);

        tbody.appendChild(mainRow);
        tbody.appendChild(subRow);

        return tbody;
    }

    createMainItemRow(letter) {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${letter}</td>
            <td>
                <div class="mb-2">工作項目</div>
                <input type="text" class="form-control" placeholder="請輸入">
            </td>
            <td></td>
            <td>0%</td>
            <td class="text-center">0</td>
            <td class="text-center"></td>
            <td>
                <button class="btn btn-sm btn-dark-green2" type="button"><i class="fas fa-trash-alt"></i></button>
            </td>
        `;
        return tr;
    }

    createSubItemRow(letter, number, isLast = true) {
        const tr = document.createElement('tr');
        const itemCode = `${letter}${number}`;
        const plusButton = isLast ? '<button class="btn btn-sm btn-dark-green2" type="button"><i class="fas fa-plus"></i></button>' : '';

        tr.innerHTML = `
            <td>${itemCode}</td>
            <td>
                <div class="mb-2">子項目</div>
                <input type="text" class="form-control" placeholder="請輸入">
            </td>
            <td>${this.createMonthSelectsHTML()}</td>
            <td>
                <input type="text" class="form-control weight-input" placeholder="" data-letter="${letter}">
            </td>
            <td class="text-center">
                <input type="text" class="form-control person-month-input" placeholder="" data-letter="${letter}">
            </td>
            <td class="text-center">
                <input class="form-check-input blue-green-check" type="checkbox" value="">
            </td>
            <td>
                <button class="btn btn-sm btn-dark-green2" type="button"><i class="fas fa-trash-alt"></i></button>
                ${plusButton}
            </td>
        `;
        return tr;
    }

    createMonthSelectsHTML() {
        const monthOptions = Array.from({length: 12}, (_, i) =>
            `<option value="${i + 1}">${String(i + 1).padStart(2, '0')}</option>`
        ).join('');

        return `
            <div class="input-group">
                <span class="input-group-text">開始</span>
                <select class="form-select month-select">
                    <option value="" selected disabled>請選擇</option>
                    ${monthOptions}
                </select>
            </div>
            <div class="input-group mt-2">
                <span class="input-group-text">結束</span>
                <select class="form-select month-select">
                    <option value="" selected disabled>請選擇</option>
                    ${monthOptions}
                </select>
            </div>
        `;
    }

    addSubItem(event) {
        const button = event.currentTarget;
        const row = button.closest('tr');
        const cell = row.cells[0];
        const currentCode = cell.textContent.trim();

        const letter = currentCode.charAt(0);
        const currentNumber = parseInt(currentCode.substring(1));
        const nextNumber = currentNumber + 1;

        // 隱藏當前行的加號按鈕
        const currentPlusBtn = row.querySelector('.fa-plus');
        if (currentPlusBtn) {
            currentPlusBtn.closest('button').remove();
        }

        const newRow = this.createSubItemRow(letter, nextNumber, true);
        row.parentNode.insertBefore(newRow, row.nextSibling);

        this.rebindAllEvents();
    }

    deleteItem(event) {
        const button = event.currentTarget;
        const row = button.closest('tr');
        const tbody = row.closest('tbody');
        const cell = row.cells[0];
        const code = cell.textContent.trim();

        if (/^[A-Z]$/.test(code)) {
            // 主項目刪除
            const letterGroups = this.getLetterGroups();
            const currentLetters = Object.keys(letterGroups).sort();

            if (currentLetters.length === 1 && currentLetters[0] === 'A') {
                alert('至少需要保留一個工作項目（A項目）');
                return;
            }

            tbody.remove();
            this.updateCurrentLetters();
            this.renumberMainItems();
        } else {
            // 子項目刪除
            const letter = code.charAt(0);
            const letterGroups = this.getLetterGroups();

            if (letter === 'A' && letterGroups[letter].subItems.length === 1) {
                alert('A項目至少需要保留一個子項目');
                return;
            }

            row.remove();
            this.renumberSubItems(letter);
            this.ensureLastRowHasPlusButton(letter);
        }

        this.updatePlusButtonVisibility();
        this.parent.calculator.updateTotals();
    }

    updateCurrentLetters() {
        const letterGroups = this.getLetterGroups();
        this.currentLetters = Object.keys(letterGroups).sort();
    }

    getLetterGroups() {
        const groups = {};
        const allRows = document.querySelectorAll('.sub-table tbody tr');

        allRows.forEach(row => {
            const cell = row.cells[0];
            const code = cell.textContent.trim();

            if (/^[A-Z]$/.test(code)) {
                if (!groups[code]) {
                    groups[code] = { mainItem: cell, subItems: [] };
                }
            } else if (/^[A-Z]\d+$/.test(code)) {
                const letter = code.charAt(0);
                if (!groups[letter]) {
                    groups[letter] = { mainItem: null, subItems: [] };
                }
                groups[letter].subItems.push(cell);
            }
        });

        return groups;
    }

    updatePlusButtonVisibility() {
        const workItemTable = document.querySelector('.sub-table');
        if (workItemTable) {
            const workItemPlusButtons = workItemTable.querySelectorAll('.btn-dark-green2 .fa-plus');
            workItemPlusButtons.forEach(btn => {
                btn.closest('button').style.display = 'none';
            });

            const letterGroups = this.getLetterGroups();
            Object.keys(letterGroups).forEach(letter => {
                const lastSubItem = letterGroups[letter].subItems[letterGroups[letter].subItems.length - 1];
                if (lastSubItem) {
                    const row = lastSubItem.closest('tr');
                    const plusButton = row.querySelector('.fa-plus');
                    if (plusButton) {
                        plusButton.closest('button').style.display = 'inline-block';
                    }
                }
            });
        }
    }

    renumberMainItems() {
        const letterGroups = this.getLetterGroups();
        const sortedLetters = Object.keys(letterGroups).sort();

        sortedLetters.forEach((oldLetter, index) => {
            const newLetter = String.fromCharCode(65 + index);

            if (oldLetter !== newLetter) {
                const group = letterGroups[oldLetter];

                if (group.mainItem) {
                    group.mainItem.textContent = newLetter;
                }

                group.subItems.forEach((subItem, subIndex) => {
                    subItem.textContent = `${newLetter}${subIndex + 1}`;
                });
            }
        });

        this.currentLetters = sortedLetters.map((_, index) => String.fromCharCode(65 + index));
    }

    renumberSubItems(letter) {
        const letterGroups = this.getLetterGroups();
        if (letterGroups[letter]) {
            letterGroups[letter].subItems.forEach((subItem, index) => {
                subItem.textContent = `${letter}${index + 1}`;
            });
        }
    }

    ensureLastRowHasPlusButton(letter) {
        const letterGroups = this.getLetterGroups();
        if (letterGroups[letter] && letterGroups[letter].subItems.length > 0) {
            const lastSubItem = letterGroups[letter].subItems[letterGroups[letter].subItems.length - 1];
            const lastRow = lastSubItem.closest('tr');
            const functionsCell = lastRow.cells[lastRow.cells.length - 1];

            if (!functionsCell.querySelector('.fa-plus')) {
                const plusButton = document.createElement('button');
                plusButton.className = 'btn btn-sm btn-dark-green2';
                plusButton.type = 'button';
                plusButton.innerHTML = '<i class="fas fa-plus"></i>';
                functionsCell.appendChild(plusButton);

                this.bindPlusButtons();
            }
        }
    }

    rebindAllEvents() {
        this.bindPlusButtons();
        this.bindDeleteButtons();
        this.bindWorkItemInputs();
        this.parent.calculator.bindWeightInputs();
        this.parent.calculator.bindPersonMonthInputs();
    }

    // 載入工作項目資料
    loadWorkItems(workItemsData) {
        try {
            console.log('開始載入工作項目資料：', workItemsData);

            if (!Array.isArray(workItemsData) || workItemsData.length === 0) {
                console.warn('工作項目資料為空或格式不正確');
                return;
            }

            this.clearWorkItemsTable();
            this.buildWorkItemsFromData(workItemsData);
            this.rebindAllEvents();

            setTimeout(() => {
                this.parent.checkStandards.updateCheckpointOptions();
            }, 100);

            this.parent.calculator.updateTotals();
            console.log('工作項目資料載入完成');
        } catch (error) {
            console.error('載入工作項目資料時發生錯誤：', error);
        }
    }

    clearWorkItemsTable() {
        const subTable = document.querySelector('.sub-table table');
        if (!subTable) return;

        const tbodies = subTable.querySelectorAll('tbody');
        tbodies.forEach(tbody => tbody.remove());
    }

    buildWorkItemsFromData(workItemsData) {
        const subTable = document.querySelector('.sub-table table');
        if (!subTable) return;

        const tfoot = subTable.querySelector('tfoot');
        const hasItemType = workItemsData.some(item => item.itemType);

        let letterGroups = {};

        if (hasItemType) {
            workItemsData.forEach(item => {
                if (item.itemType === 'main') {
                    if (!letterGroups[item.code]) {
                        letterGroups[item.code] = { main: item, subs: [] };
                    }
                    letterGroups[item.code].main = item;
                } else if (item.itemType === 'sub') {
                    const letter = item.code.charAt(0);
                    if (!letterGroups[letter]) {
                        letterGroups[letter] = { main: null, subs: [] };
                    }
                    letterGroups[letter].subs.push(item);
                }
            });
        } else {
            workItemsData.forEach(item => {
                const code = item.itemCode || item.code;
                if (/^[A-Z]$/.test(code)) {
                    if (!letterGroups[code]) {
                        letterGroups[code] = { main: item, subs: [] };
                    } else {
                        letterGroups[code].main = item;
                    }
                } else if (/^[A-Z]\d+$/.test(code)) {
                    const letter = code.charAt(0);
                    if (!letterGroups[letter]) {
                        letterGroups[letter] = { main: null, subs: [] };
                    }
                    letterGroups[letter].subs.push(item);
                }
            });
        }

        this.currentLetters = Object.keys(letterGroups).sort();

        Object.keys(letterGroups).sort().forEach(letter => {
            const group = letterGroups[letter];
            const tbody = document.createElement('tbody');

            if (group.main) {
                const mainRow = this.createMainItemRow(letter);
                this.populateMainItemRow(mainRow, group.main);
                tbody.appendChild(mainRow);
            }

            if (group.subs && group.subs.length > 0) {
                group.subs.forEach((subItem, index) => {
                    const code = subItem.itemCode || subItem.code;
                    const subNumber = parseInt(code.substring(1)) || (index + 1);
                    const isLastInGroup = (index === group.subs.length - 1);
                    const subRow = this.createSubItemRow(letter, subNumber, isLastInGroup);

                    this.populateSubItemRow(subRow, subItem);
                    tbody.appendChild(subRow);
                });
            } else {
                const subRow = this.createSubItemRow(letter, 1, true);
                tbody.appendChild(subRow);
            }

            if (tfoot) {
                tfoot.parentNode.insertBefore(tbody, tfoot);
            } else {
                subTable.appendChild(tbody);
            }
        });
    }

    populateMainItemRow(row, mainItem) {
        const nameInput = row.cells[1].querySelector('input[type="text"]');
        if (nameInput && mainItem.itemName) {
            nameInput.value = mainItem.itemName;
        }

        const weightCell = row.cells[3];
        if (weightCell && mainItem.weight !== null && mainItem.weight !== undefined) {
            weightCell.textContent = `${mainItem.weight}%`;
        }

        const personMonthCell = row.cells[4];
        if (personMonthCell && mainItem.personMonth !== null && mainItem.personMonth !== undefined) {
            personMonthCell.textContent = mainItem.personMonth.toString();
        }
    }

    populateSubItemRow(row, subItem) {
        const nameInput = row.cells[1].querySelector('input[type="text"]');
        if (nameInput && subItem.itemName) {
            nameInput.value = subItem.itemName;
        }

        const startMonthSelect = row.cells[2].querySelectorAll('select')[0];
        const endMonthSelect = row.cells[2].querySelectorAll('select')[1];

        if (startMonthSelect && subItem.startMonth) {
            startMonthSelect.value = subItem.startMonth.toString();
        }

        if (endMonthSelect && subItem.endMonth) {
            endMonthSelect.value = subItem.endMonth.toString();
        }

        const weightInput = row.cells[3].querySelector('input[type="text"]');
        if (weightInput && subItem.weight !== null && subItem.weight !== undefined) {
            const weightValue = subItem.weight.toString();
            weightInput.value = weightValue.includes('%') ? weightValue : `${weightValue}%`;
        }

        const personMonthInput = row.cells[4].querySelector('input[type="text"]');
        if (personMonthInput && subItem.personMonth !== null && subItem.personMonth !== undefined) {
            personMonthInput.value = subItem.personMonth.toString();
        }

        const outsourcedCheckbox = row.cells[5].querySelector('input[type="checkbox"]');
        if (outsourcedCheckbox) {
            outsourcedCheckbox.checked = subItem.isOutsourced || false;
        }
    }

    getWorkItemOptions() {
        const options = [];
        const letterGroups = this.getLetterGroups();

        Object.keys(letterGroups).sort().forEach(letter => {
            letterGroups[letter].subItems.forEach(subItem => {
                const row = subItem.closest('tr');
                const code = subItem.textContent.trim();
                const workItemInput = row.cells[1] ? row.cells[1].querySelector('input[type="text"]') : null;

                let workItemText = '';
                if (workItemInput && workItemInput.value.trim()) {
                    workItemText = workItemInput.value.trim();
                } else {
                    workItemText = '未填寫';
                }

                options.push({
                    value: code,
                    text: `${code}.${workItemText}`
                });
            });
        });

        return options;
    }
}
//#endregion

//#region 計算管理器（權重和人月數）
class CalculationManager {
    constructor(parentManager) {
        this.parent = parentManager;
    }

    bindWeightInputs() {
        const weightInputs = document.querySelectorAll('.weight-input');
        weightInputs.forEach(input => {
            if (!input.hasAttribute('data-weight-bound')) {
                input.addEventListener('input', () => this.calculateWeights());
                input.setAttribute('data-weight-bound', 'true');
            }
        });

        const allRows = document.querySelectorAll('.sub-table tbody tr');
        allRows.forEach(row => {
            const cell = row.cells[0];
            const code = cell ? cell.textContent.trim() : '';

            if (/^[A-Z]\d+$/.test(code)) {
                const weightInput = row.cells[3] ? row.cells[3].querySelector('input[type="text"]') : null;
                if (weightInput && !weightInput.hasAttribute('data-weight-bound')) {
                    weightInput.classList.add('weight-input');
                    const letter = code.charAt(0);
                    weightInput.setAttribute('data-letter', letter);
                    weightInput.addEventListener('input', () => this.calculateWeights());
                    weightInput.setAttribute('data-weight-bound', 'true');
                }
            }
        });
    }

    bindPersonMonthInputs() {
        const personMonthInputs = document.querySelectorAll('.person-month-input');
        personMonthInputs.forEach(input => {
            if (!input.hasAttribute('data-month-bound')) {
                input.addEventListener('input', () => this.calculatePersonMonths());
                input.setAttribute('data-month-bound', 'true');
            }
        });

        const allRows = document.querySelectorAll('.sub-table tbody tr');
        allRows.forEach(row => {
            const cell = row.cells[0];
            const code = cell ? cell.textContent.trim() : '';

            if (/^[A-Z]\d+$/.test(code)) {
                const personMonthInput = row.cells[4] ? row.cells[4].querySelector('input[type="text"]') : null;
                if (personMonthInput && !personMonthInput.hasAttribute('data-month-bound')) {
                    personMonthInput.classList.add('person-month-input');
                    const letter = code.charAt(0);
                    personMonthInput.setAttribute('data-letter', letter);
                    personMonthInput.addEventListener('input', () => this.calculatePersonMonths());
                    personMonthInput.setAttribute('data-month-bound', 'true');
                }
            }
        });
    }

    calculateWeights() {
        const letterGroups = this.parent.workItems.getLetterGroups();

        Object.keys(letterGroups).forEach(letter => {
            this.calculateMainItemWeight(letter);
        });

        this.calculateTotalWeight();
    }

    calculateMainItemWeight(letter) {
        const letterGroups = this.parent.workItems.getLetterGroups();
        if (!letterGroups[letter]) return;

        let totalWeight = 0;

        letterGroups[letter].subItems.forEach(subItem => {
            const row = subItem.closest('tr');
            const weightInput = row.querySelector('.weight-input');
            if (weightInput && weightInput.value) {
                const weight = parseFloat(weightInput.value.replace('%', '')) || 0;
                totalWeight += weight;
            }
        });

        const mainItemRow = this.findMainItemRow(letter);
        if (mainItemRow) {
            const weightCell = mainItemRow.cells[3];
            weightCell.textContent = totalWeight + '%';
        }
    }

    calculateTotalWeight() {
        const letterGroups = this.parent.workItems.getLetterGroups();
        let totalWeight = 0;

        Object.keys(letterGroups).forEach(letter => {
            const mainItemRow = this.findMainItemRow(letter);
            if (mainItemRow) {
                const weightText = mainItemRow.cells[3].textContent.replace('%', '');
                const weight = parseFloat(weightText) || 0;
                totalWeight += weight;
            }
        });

        const tfootRow = document.querySelector('.sub-table table tfoot tr');
        if (tfootRow) {
            tfootRow.cells[1].textContent = totalWeight + '%';
        }
    }

    calculatePersonMonths() {
        const letterGroups = this.parent.workItems.getLetterGroups();

        Object.keys(letterGroups).forEach(letter => {
            this.calculateMainItemPersonMonth(letter);
        });

        this.calculateTotalPersonMonth();
    }

    calculateMainItemPersonMonth(letter) {
        const letterGroups = this.parent.workItems.getLetterGroups();
        if (!letterGroups[letter]) return;

        let totalPersonMonth = 0;

        letterGroups[letter].subItems.forEach(subItem => {
            const row = subItem.closest('tr');
            const personMonthInput = row.querySelector('.person-month-input');
            if (personMonthInput && personMonthInput.value) {
                const personMonth = parseFloat(personMonthInput.value) || 0;
                totalPersonMonth += personMonth;
            }
        });

        const mainItemRow = this.findMainItemRow(letter);
        if (mainItemRow) {
            const personMonthCell = mainItemRow.cells[4];
            personMonthCell.textContent = totalPersonMonth;
        }
    }

    calculateTotalPersonMonth() {
        const letterGroups = this.parent.workItems.getLetterGroups();
        let totalPersonMonth = 0;

        Object.keys(letterGroups).forEach(letter => {
            const mainItemRow = this.findMainItemRow(letter);
            if (mainItemRow) {
                const personMonthText = mainItemRow.cells[4].textContent;
                const personMonth = parseFloat(personMonthText) || 0;
                totalPersonMonth += personMonth;
            }
        });

        const tfootRow = document.querySelector('.sub-table table tfoot tr');
        if (tfootRow) {
            tfootRow.cells[2].textContent = totalPersonMonth;
        }
    }

    findMainItemRow(letter) {
        const allRows = document.querySelectorAll('.sub-table tbody tr');
        for (let row of allRows) {
            const cell = row.cells[0];
            if (cell && cell.textContent.trim() === letter) {
                return row;
            }
        }
        return null;
    }

    updateTotals() {
        this.calculateWeights();
        this.calculatePersonMonths();
        this.parent.checkStandards.updateCheckpointOptions();
    }
}
//#endregion

//#region 查核標準管理器
class CheckStandardManager {
    constructor(parentManager) {
        this.parent = parentManager;
        this.init();
    }

    init() {
        this.bindCheckpointButtons();
    }

    bindCheckpointButtons() {
        const checkpointTable = document.getElementById('checkStandards');
        if (checkpointTable) {
            // 綁定加號按鈕
            const plusButtons = checkpointTable.querySelectorAll('.fa-plus');
            plusButtons.forEach(btn => {
                const button = btn.closest('button');
                if (!button.hasAttribute('data-checkpoint-plus-bound')) {
                    button.addEventListener('click', (e) => this.addCheckpointRow(e));
                    button.setAttribute('data-checkpoint-plus-bound', 'true');
                }
            });

            // 綁定刪除按鈕
            const deleteButtons = checkpointTable.querySelectorAll('.fa-trash-alt');
            deleteButtons.forEach(btn => {
                const button = btn.closest('button');
                if (!button.hasAttribute('data-checkpoint-delete-bound')) {
                    button.addEventListener('click', (e) => this.deleteCheckpointRow(e));
                    button.setAttribute('data-checkpoint-delete-bound', 'true');
                }
            });

            // 綁定對應工項選擇事件
            const selects = checkpointTable.querySelectorAll('select');
            selects.forEach(select => {
                if (!select.hasAttribute('data-checkpoint-select-bound')) {
                    select.addEventListener('change', (e) => this.updateCheckpointNumber(e));
                    select.setAttribute('data-checkpoint-select-bound', 'true');
                }
            });
        }
    }

    addCheckpointRow(event) {
        const button = event.currentTarget;
        const row = button.closest('tr');

        const newRow = this.createCheckpointRow();
        row.parentNode.insertBefore(newRow, row.nextSibling);

        this.bindCheckpointButtons();
        this.updateNewRowCheckpointOptions(newRow);
    }

    createCheckpointRow() {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td class="align-middle">
                <select name="" class="form-select">
                    <option value="" selected disabled>請選擇</option>
                </select>
            </td>
            <td class="align-middle">請選擇</td>
            <td class="align-middle">
                <input type="date" name="" class="form-control">
            </td>
            <td class="align-middle" width="500">
                <span class="form-control textarea" role="textbox" contenteditable="" data-placeholder="請輸入" aria-label="文本輸入區域"></span>
            </td>
            <td class="align-middle">
                <button class="btn btn-sm btn-dark-green2" type="button"><i class="fas fa-trash-alt"></i></button>
                <button class="btn btn-sm btn-dark-green2" type="button"><i class="fas fa-plus"></i></button>
            </td>
        `;
        return tr;
    }

    deleteCheckpointRow(event) {
        const button = event.currentTarget;
        const row = button.closest('tr');
        const checkpointTable = document.getElementById('checkStandards');

        const allRows = checkpointTable.querySelectorAll('tbody tr');
        if (allRows.length <= 1) {
            alert('至少需要保留一個查核標準項目');
            return;
        }

        row.remove();
        this.recalculateAllCheckpointNumbers();
    }

    updateNewRowCheckpointOptions(row) {
        const select = row.querySelector('select');
        if (select) {
            select.innerHTML = '<option value="" selected disabled>請選擇</option>';

            this.parent.workItems.getWorkItemOptions().forEach(option => {
                const optionElement = document.createElement('option');
                optionElement.value = option.value;
                optionElement.textContent = option.text;
                select.appendChild(optionElement);
            });
        }
    }

    updateCheckpointNumber(event) {
        this.recalculateAllCheckpointNumbers();
    }

    recalculateAllCheckpointNumbers() {
        const checkpointTable = document.getElementById('checkStandards');
        const allRows = checkpointTable.querySelectorAll('tbody tr');

        const workItemCounters = {};

        allRows.forEach(row => {
            const select = row.querySelector('select');
            const numberCell = row.cells[1];

            if (select && select.value) {
                const workItemCode = select.value;

                if (!workItemCounters[workItemCode]) {
                    workItemCounters[workItemCode] = 0;
                }
                workItemCounters[workItemCode]++;

                numberCell.textContent = `${workItemCode}.${workItemCounters[workItemCode]}`;
            } else {
                numberCell.textContent = '請選擇';
            }
        });
    }

    updateCheckpointOptions() {
        const checkpointSelects = document.querySelectorAll('#checkStandards select');

        checkpointSelects.forEach(select => {
            const currentValue = select.value;
            select.innerHTML = '<option value="" selected disabled>請選擇</option>';

            this.parent.workItems.getWorkItemOptions().forEach(option => {
                const optionElement = document.createElement('option');
                optionElement.value = option.value;
                optionElement.textContent = option.text;
                select.appendChild(optionElement);
            });

            if (currentValue && select.querySelector(`option[value="${currentValue}"]`)) {
                select.value = currentValue;
            }
        });
    }

    loadCheckStandards(checkStandardsData) {
        try {
            console.log('開始載入查核標準資料：', checkStandardsData);

            const checkpointTable = document.getElementById('checkStandards');
            if (!checkpointTable) return;

            const tbody = checkpointTable.querySelector('tbody');
            if (!tbody) return;

            tbody.innerHTML = '';

            checkStandardsData.forEach((checkStandard, index) => {
                const row = this.createCheckpointRow(index === checkStandardsData.length - 1);

                const workItemSelect = row.querySelector('select');
                if (workItemSelect && checkStandard.workItem) {
                    this.updateNewRowCheckpointOptions(row);
                    setTimeout(() => {
                        workItemSelect.value = checkStandard.workItem;
                    }, 50);
                }

                const dateInput = row.querySelector('input[type="date"]');
                if (dateInput && checkStandard.plannedFinishDate) {
                    dateInput.value = checkStandard.plannedFinishDate;
                }

                const descriptionSpan = row.querySelector('span[contenteditable]');
                if (descriptionSpan && checkStandard.description) {
                    descriptionSpan.textContent = checkStandard.description;
                    descriptionSpan.removeAttribute('data-placeholder');
                }

                tbody.appendChild(row);
            });

            if (checkStandardsData.length === 0) {
                const emptyRow = this.createCheckpointRow(true);
                tbody.appendChild(emptyRow);
            }

            this.bindCheckpointButtons();

            setTimeout(() => {
                this.recalculateAllCheckpointNumbers();
            }, 100);

            console.log('查核標準資料載入完成');
        } catch (error) {
            console.error('載入查核標準資料時發生錯誤：', error);
        }
    }
}
//#endregion

//#region 計畫架構圖管理器
class DiagramManager {
    constructor(parentManager) {
        this.parent = parentManager;
        this.init();
    }

    init() {
        this.bindDiagramUpload();
    }

    bindDiagramUpload() {
        const uploadBtn = document.getElementById('btnUploadDiagram');
        const deleteBtn = document.getElementById('btnDeleteDiagram');

        if (uploadBtn) {
            uploadBtn.addEventListener('click', (e) => {
                e.preventDefault();
                this.handleDiagramPreview();
            });
        }

        if (deleteBtn) {
            deleteBtn.addEventListener('click', (e) => {
                e.preventDefault();
                this.handleDiagramDelete();
            });
        }
    }

    handleDiagramPreview() {
        const fileInput = document.getElementById('fileUploadDiagram');

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
    }

    prepareFileForUpload() {
        const fileInput = document.getElementById('fileUploadDiagram');
        if (fileInput && fileInput.files.length > 0) {
            // 確保檔案 input 有正確的 name 屬性
            fileInput.name = 'fileUploadDiagram';
            
            const form = document.getElementById('form1');
            
            // 檢查檔案 input 是否在正確的 form 中
            const currentForm = fileInput.closest('form');
            
            // 只有當檔案 input 不在目標 form 中時才移動
            if (form && currentForm !== form) {
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
            this.displayDiagramPreview(e.target.result, file.name);
        };
        reader.readAsDataURL(file);
    }

    handleDiagramDelete() {
        const fileInput = document.getElementById('fileUploadDiagram');
        if (fileInput) {
            fileInput.value = '';
        }

        this.hideDiagramPreview();
    }

    displayDiagramPreview(imageSrc, fileName) {
        const previewContainer = document.getElementById('diagramPreviewContainer');
        const previewImg = document.getElementById('diagramPreview');

        if (previewContainer && previewImg) {
            previewImg.src = imageSrc;
            previewImg.alt = `計畫架構圖 - ${fileName}`;
            previewContainer.style.display = 'block';
        }
    }

    hideDiagramPreview() {
        const previewContainer = document.getElementById('diagramPreviewContainer');
        const previewImg = document.getElementById('diagramPreview');

        if (previewContainer) {
            previewContainer.style.display = 'none';
        }

        if (previewImg) {
            previewImg.src = '';
            previewImg.alt = '';
        }
    }

    loadDiagramFile(filePath, fileName) {
        try {
            console.log('開始載入計畫架構圖：', filePath, fileName);

            if (filePath && fileName) {
                const fullPath = filePath.startsWith('/') ? filePath : `/${filePath}`;
                this.displayDiagramPreview(fullPath, fileName);
                console.log('計畫架構圖載入完成：', fullPath);
            }
        } catch (error) {
            console.error('載入計畫架構圖時發生錯誤：', error);
        }
    }

    collectDiagramData() {
        const previewImg = document.getElementById('diagramPreview');
        const previewContainer = document.getElementById('diagramPreviewContainer');

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
//#endregion

//#region 資料管理器
class DataManager {
    constructor(parentManager) {
        this.parent = parentManager;
        this.init();
    }

    init() {
        this.bindFormSubmission();
    }

    bindFormSubmission() {
        const form = document.getElementById('form1');
        if (form) {
            form.addEventListener('submit', (e) => {
                this.collectAndSubmitData();
            });
        }

        const tempSaveBtn = document.getElementById('btnTempSave');
        const saveNextBtn = document.getElementById('btnSaveAndNext');

        if (tempSaveBtn) {
            tempSaveBtn.addEventListener('click', (e) => {
                this.collectAndSubmitData();
            });
        }

        if (saveNextBtn) {
            saveNextBtn.addEventListener('click', (e) => {
                this.collectAndSubmitData();
            });
        }
    }

    collectAndSubmitData() {
        try {
            // 在收集資料前先準備檔案上傳
            console.log('=== collectAndSubmitData 開始 ===');
            this.parent.diagram.prepareFileForUpload();
            
            const scheduleData = this.collectScheduleData();
            const workItemsData = this.collectWorkItemsData();
            const checkStandardsData = this.collectCheckStandardsData();
            const diagramData = this.parent.diagram.collectDiagramData();

            this.setFormData({
                schedule: scheduleData,
                workItems: workItemsData,
                checkStandards: checkStandardsData,
                diagram: diagramData
            });

            const fileInput = document.getElementById('fileUploadDiagram');
            if (fileInput && fileInput.files.length > 0 && diagramData.hasImage) {
                // 使用正確的檔案名稱，與後端一致
                fileInput.name = 'fileUploadDiagram';
                console.log('設定檔案 input name 為:', fileInput.name);

                const form = document.getElementById('form1');
                if (form && !form.contains(fileInput)) {
                    form.appendChild(fileInput);
                }
            }

            console.log('=== 收集到的資料 (重構版) ===');
            console.log('計畫期程:', scheduleData);
            console.log('工作項目:', workItemsData);
            console.log('查核標準:', checkStandardsData);
            console.log('計畫架構圖:', diagramData);

        } catch (error) {
            console.error('收集資料時發生錯誤：', error);
            alert('資料收集失敗：' + error.message);
        }
    }

    collectScheduleData() {
        const startDateInput = document.querySelector('input[type="date"]');
        const endDateInput = document.querySelectorAll('input[type="date"]')[1];

        return {
            startDate: startDateInput ? startDateInput.value : '',
            endDate: endDateInput ? endDateInput.value : ''
        };
    }

    collectWorkItemsData() {
        this.parent.calculator.calculateWeights();

        const workItems = [];
        const letterGroups = this.parent.workItems.getLetterGroups();

        Object.keys(letterGroups).sort().forEach(letter => {
            // 主項目
            const mainItem = letterGroups[letter].mainItem;
            const mainRow = mainItem ? mainItem.closest('tr') : null;

            if (mainRow) {
                const mainWorkNameInput = mainRow.cells[1] ? mainRow.cells[1].querySelector('input[type="text"]') : null;
                const mainWorkName = mainWorkNameInput ? mainWorkNameInput.value : '';

                const mainWeightCell = mainRow.cells[3];
                const mainWeightText = mainWeightCell ? mainWeightCell.textContent.trim() : '';
                const mainWeight = mainWeightText ? parseFloat(mainWeightText.replace('%', '')) : null;

                let totalPersonMonth = 0;
                letterGroups[letter].subItems.forEach(subItem => {
                    const subRow = subItem.closest('tr');
                    const personMonthInput = subRow.cells[4] ? subRow.cells[4].querySelector('input[type="text"]') : null;
                    const personMonthStr = personMonthInput ? personMonthInput.value.trim() : '';
                    const personMonth = personMonthStr ? parseFloat(personMonthStr) : 0;
                    totalPersonMonth += personMonth;
                });

                workItems.push({
                    code: letter,
                    itemName: mainWorkName,
                    startMonth: null,
                    endMonth: null,
                    weight: mainWeight,
                    personMonth: totalPersonMonth > 0 ? totalPersonMonth : null,
                    isOutsourced: false,
                    itemType: 'main'
                });
            }

            // 子項目
            letterGroups[letter].subItems.forEach((subItem, index) => {
                const row = subItem.closest('tr');
                const code = subItem.textContent.trim();

                const workNameInput = row.cells[1] ? row.cells[1].querySelector('input[type="text"]') : null;
                const workName = workNameInput ? workNameInput.value : '';

                const startMonthSelect = row.cells[2] ? row.cells[2].querySelectorAll('select')[0] : null;
                const endMonthSelect = row.cells[2] ? row.cells[2].querySelectorAll('select')[1] : null;
                const startMonth = startMonthSelect && startMonthSelect.value ? parseInt(startMonthSelect.value) : null;
                const endMonth = endMonthSelect && endMonthSelect.value ? parseInt(endMonthSelect.value) : null;

                const weightInput = row.cells[3] ? row.cells[3].querySelector('input[type="text"]') : null;
                const weightStr = weightInput ? weightInput.value.replace('%', '').trim() : '';
                const weight = weightStr ? parseFloat(weightStr) : null;

                const personMonthInput = row.cells[4] ? row.cells[4].querySelector('input[type="text"]') : null;
                const personMonthStr = personMonthInput ? personMonthInput.value.trim() : '';
                const personMonth = personMonthStr ? parseFloat(personMonthStr) : null;

                const outsourcedCheckbox = row.cells[5] ? row.cells[5].querySelector('input[type="checkbox"]') : null;
                const isOutsourced = outsourcedCheckbox ? outsourcedCheckbox.checked : false;

                workItems.push({
                    code: code,
                    itemName: workName,
                    startMonth: startMonth,
                    endMonth: endMonth,
                    weight: weight,
                    personMonth: personMonth,
                    isOutsourced: isOutsourced,
                    itemType: 'sub'
                });
            });
        });

        return workItems;
    }

    collectCheckStandardsData() {
        const checkStandards = [];
        const checkpointTable = document.getElementById('checkStandards');

        if (checkpointTable) {
            const rows = checkpointTable.querySelectorAll('tbody tr');

            rows.forEach((row, index) => {
                const workItemSelect = row.cells[0] ? row.cells[0].querySelector('select') : null;
                const workItem = workItemSelect ? workItemSelect.value : '';

                const serialNumberCell = row.cells[1];
                const serialNumber = serialNumberCell ? serialNumberCell.textContent.trim() : '';

                const dateInput = row.cells[2] ? row.cells[2].querySelector('input[type="date"]') : null;
                const plannedFinishDate = dateInput ? dateInput.value : '';

                const descriptionSpan = row.cells[3] ? row.cells[3].querySelector('span[contenteditable]') : null;
                const description = descriptionSpan ? descriptionSpan.textContent.trim() : '';

                if (workItem || description) {
                    checkStandards.push({
                        workItem: workItem,
                        serialNumber: serialNumber,
                        plannedFinishDate: plannedFinishDate,
                        description: description,
                        order: index + 1
                    });
                }
            });
        }

        return checkStandards;
    }

    setFormData(data) {
        this.createHiddenField('startDate', data.schedule.startDate);
        this.createHiddenField('endDate', data.schedule.endDate);
        this.createHiddenField('workItemsData', JSON.stringify(data.workItems));
        this.createHiddenField('checkStandardsData', JSON.stringify(data.checkStandards));
        this.createHiddenField('diagramData', JSON.stringify(data.diagram));
    }

    createHiddenField(name, value) {
        const form = document.getElementById('form1');
        if (!form) return;

        let hiddenField = form.querySelector(`input[name="${name}"]`);

        if (!hiddenField) {
            hiddenField = document.createElement('input');
            hiddenField.type = 'hidden';
            hiddenField.name = name;
            form.appendChild(hiddenField);
        }

        hiddenField.value = value;
    }
}
//#endregion

//#region 主控制器（重構版）
class SciWorkSchManager {
    constructor() {
        // 初始化各個管理器
        this.workItems = new WorkItemTableManager(this);
        this.calculator = new CalculationManager(this);
        this.checkStandards = new CheckStandardManager(this);
        this.diagram = new DiagramManager(this);
        this.dataManager = new DataManager(this);

        console.log('重構版 SciWorkSchManager 初始化完成');
    }

    // 提供給後端呼叫的載入方法
    loadWorkItems(workItemsData) {
        this.workItems.loadWorkItems(workItemsData);
    }

    loadCheckStandards(checkStandardsData) {
        this.checkStandards.loadCheckStandards(checkStandardsData);
    }

    loadDiagramFile(filePath, fileName) {
        this.diagram.loadDiagramFile(filePath, fileName);
    }
}
//#endregion

//#region 頁面初始化
document.addEventListener('DOMContentLoaded', function() {
    // 將重構版管理器實例設為全域變數
    window.sciWorkSchManager = new SciWorkSchManager();
});
//#endregion