
//#region 通用工具類
class EventBindingHelper {
    static bindElementsWithAttribute(selector, eventType, handler, attributeName, container = document) {
        const elements = container.querySelectorAll(selector);
        elements.forEach(element => {
            if (!element.hasAttribute(attributeName)) {
                element.addEventListener(eventType, handler);
                element.setAttribute(attributeName, 'true');
            }
        });
    }

    static bindButtonsByIcon(iconClass, eventType, handler, attributeName, container = document) {
        const icons = container.querySelectorAll(iconClass);
        icons.forEach(icon => {
            const button = icon.closest('button');
            if (button && !button.hasAttribute(attributeName)) {
                button.addEventListener(eventType, handler);
                button.setAttribute(attributeName, 'true');
            }
        });
    }
}

class DOMCache {
    constructor() {
        this.cache = new Map();
    }

    get(selector) {
        if (!this.cache.has(selector)) {
            this.cache.set(selector, document.querySelector(selector));
        }
        return this.cache.get(selector);
    }

    getAll(selector) {
        const cacheKey = `all:${selector}`;
        if (!this.cache.has(cacheKey)) {
            this.cache.set(cacheKey, document.querySelectorAll(selector));
        }
        return this.cache.get(cacheKey);
    }

    clear() {
        this.cache.clear();
    }
}

class ErrorHandler {
    static handle(error, context = '') {
        console.warn(`${context} error:`, error);
    }

    static safeExecute(fn, context = '') {
        try {
            return fn();
        } catch (error) {
            this.handle(error, context);
            return null;
        }
    }
}

class HiddenFieldUpdater {
    static updateHiddenFields(dataManager, checkStandards) {
        ErrorHandler.safeExecute(() => {
            const workItemsData = dataManager.collectWorkItemsData();
            const checkStandardsData = dataManager.collectCheckStandardsData();

            dataManager.updateAspNetHiddenField('hiddenWorkItemsData', JSON.stringify(workItemsData));
            dataManager.updateAspNetHiddenField('hiddenCheckStandardsData', JSON.stringify(checkStandardsData));
        }, 'Hidden field update');
    }
}
//#endregion

//#region 工作項目表格管理器
class WorkItemTableManager {
    constructor(parentManager) {
        this.parent = parentManager;
        this.currentLetters = ['A', 'B'];
        this.domCache = new DOMCache();
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
        const workItemTable = this.domCache.get('.sub-table');
        if (workItemTable) {
            EventBindingHelper.bindButtonsByIcon(
                '.btn-dark-green2 .fa-plus',
                'click',
                (e) => this.addSubItem(e),
                'data-bound',
                workItemTable
            );
        }
    }

    bindDeleteButtons() {
        const workItemTable = this.domCache.get('.sub-table');
        if (workItemTable) {
            EventBindingHelper.bindButtonsByIcon(
                '.btn-dark-green2 .fa-trash-alt',
                'click',
                (e) => this.deleteItem(e),
                'data-bound',
                workItemTable
            );
        }
    }

    bindWorkItemInputs() {
        const allRows = this.domCache.getAll('.sub-table tbody tr');
        allRows.forEach(row => {
            const cell = row.cells[0];
            const code = cell ? cell.textContent.trim() : '';

            if (/^[A-Z]\d+$/.test(code)) {
                const workItemInput = row.cells[1] ? row.cells[1].querySelector('input[type="text"]') : null;
                if (workItemInput && !workItemInput.hasAttribute('data-workitem-bound')) {
                    workItemInput.addEventListener('input', () => {
                        this.parent.checkStandards.updateCheckpointOptions();
                        this.updateHiddenFieldsNow();
                    });
                    workItemInput.setAttribute('data-workitem-bound', 'true');
                }
            }

            // 綁定其他輸入欄位
            const otherInputs = row.querySelectorAll('input[type="text"], input[type="number"], select');
            otherInputs.forEach(input => {
                if (!input.hasAttribute('data-bound-update')) {
                    ['input', 'change'].forEach(eventType => {
                        input.addEventListener(eventType, () => this.updateHiddenFieldsNow());
                    });
                    input.setAttribute('data-bound-update', 'true');
                }
            });
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
        this.updateHiddenFieldsNow();
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
        this.updateHiddenFieldsNow();
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
        this.updateHiddenFieldsNow();
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
        this.domCache.clear();
        this.bindPlusButtons();
        this.bindDeleteButtons();
        this.bindWorkItemInputs();
        this.parent.calculator.bindInputs();
        // 更新查核標準的下拉選單選項
        this.parent.checkStandards.updateCheckpointOptions();
    }

    // 載入工作項目資料
    loadWorkItems(workItemsData) {
        try {
            if (!Array.isArray(workItemsData) || workItemsData.length === 0) {
                return;
            }

            this.clearWorkItemsTable();
            this.buildWorkItemsFromData(workItemsData);
            this.rebindAllEvents();

            setTimeout(() => {
                this.parent.checkStandards.updateCheckpointOptions();
            }, 100);

            this.parent.calculator.updateTotals();
        } catch (error) {
            // 靜默處理錯誤
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

    updateHiddenFieldsNow() {
        HiddenFieldUpdater.updateHiddenFields(this.parent.dataManager, this.parent.checkStandards);
    }
}
//#endregion

//#region 計算管理器（權重和人月數）
class CalculationManager {
    constructor(parentManager) {
        this.parent = parentManager;
        this.domCache = parentManager.workItems.domCache;
    }

    bindInputs() {
        this.bindWeightInputs();
        this.bindPersonMonthInputs();
    }

    bindWeightInputs() {
        this.bindExistingInputs('.weight-input', 'data-weight-bound', () => this.calculateWeights());
        this.bindRowInputs(3, 'weight-input', 'data-weight-bound', () => this.calculateWeights());
    }

    bindPersonMonthInputs() {
        this.bindExistingInputs('.person-month-input', 'data-month-bound', () => this.calculatePersonMonths());
        this.bindRowInputs(4, 'person-month-input', 'data-month-bound', () => this.calculatePersonMonths());
    }

    bindExistingInputs(selector, attributeName, handler) {
        EventBindingHelper.bindElementsWithAttribute(selector, 'input', handler, attributeName);
    }

    bindRowInputs(cellIndex, className, attributeName, handler) {
        const allRows = this.domCache.getAll('.sub-table tbody tr');
        allRows.forEach(row => {
            const cell = row.cells[0];
            const code = cell ? cell.textContent.trim() : '';

            if (/^[A-Z]\d+$/.test(code)) {
                const input = row.cells[cellIndex] ? row.cells[cellIndex].querySelector('input[type="text"]') : null;
                if (input && !input.hasAttribute(attributeName)) {
                    input.classList.add(className);
                    const letter = code.charAt(0);
                    input.setAttribute('data-letter', letter);
                    input.addEventListener('input', handler);
                    input.setAttribute(attributeName, 'true');
                }
            }
        });
    }

    calculateWeights() {
        this.calculateByType('weight', '.weight-input', 3, '%');
    }

    calculatePersonMonths() {
        this.calculateByType('personMonth', '.person-month-input', 4, '');
    }

    calculateByType(type, inputSelector, cellIndex, suffix) {
        const letterGroups = this.parent.workItems.getLetterGroups();
        
        Object.keys(letterGroups).forEach(letter => {
            this.calculateMainItemByType(letter, letterGroups, inputSelector, cellIndex, suffix);
        });
        
        this.calculateTotalByType(letterGroups, cellIndex, suffix);
    }

    calculateMainItemByType(letter, letterGroups, inputSelector, cellIndex, suffix) {
        if (!letterGroups[letter]) return;

        let total = 0;
        letterGroups[letter].subItems.forEach(subItem => {
            const row = subItem.closest('tr');
            const input = row.querySelector(inputSelector);
            if (input && input.value) {
                const value = parseFloat(input.value.replace('%', '')) || 0;
                total += value;
            }
        });

        const mainItemRow = this.findMainItemRow(letter);
        if (mainItemRow) {
            const cell = mainItemRow.cells[cellIndex];
            cell.textContent = total + suffix;
        }
    }

    calculateTotalByType(letterGroups, cellIndex, suffix) {
        let grandTotal = 0;
        
        Object.keys(letterGroups).forEach(letter => {
            const mainItemRow = this.findMainItemRow(letter);
            if (mainItemRow) {
                const cellText = mainItemRow.cells[cellIndex].textContent.replace('%', '');
                const value = parseFloat(cellText) || 0;
                grandTotal += value;
            }
        });

        const tfootRow = this.domCache.get('.sub-table table tfoot tr');
        if (tfootRow) {
            const targetCellIndex = cellIndex === 3 ? 1 : 2; // 權重在第2欄，人月在第3欄
            tfootRow.cells[targetCellIndex].textContent = grandTotal + suffix;
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
        if (!checkpointTable) return;

        // 綁定加號按鈕
        EventBindingHelper.bindButtonsByIcon(
            '.fa-plus', 'click', 
            (e) => this.addCheckpointRow(e), 
            'data-checkpoint-plus-bound', 
            checkpointTable
        );

        // 綁定刪除按鈕
        EventBindingHelper.bindButtonsByIcon(
            '.fa-trash-alt', 'click', 
            (e) => this.deleteCheckpointRow(e), 
            'data-checkpoint-delete-bound', 
            checkpointTable
        );

        // 綁定對應工項選擇事件
        EventBindingHelper.bindElementsWithAttribute(
            'select', 'change', 
            (e) => {
                this.recalculateAllCheckpointNumbers();
                this.updateHiddenFieldsNow();
            }, 
            'data-checkpoint-select-bound', 
            checkpointTable
        );

        // 綁定日期輸入事件
        EventBindingHelper.bindElementsWithAttribute(
            'input[type="date"]', 'change', 
            () => this.updateHiddenFieldsNow(), 
            'data-checkpoint-date-bound', 
            checkpointTable
        );

        // 綁定內容編輯事件
        const editableSpans = checkpointTable.querySelectorAll('span[contenteditable]');
        editableSpans.forEach(span => {
            if (!span.hasAttribute('data-checkpoint-content-bound')) {
                ['input', 'blur'].forEach(eventType => {
                    span.addEventListener(eventType, () => this.updateHiddenFieldsNow());
                });
                span.setAttribute('data-checkpoint-content-bound', 'true');
            }
        });
    }

    addCheckpointRow(event) {
        const button = event.currentTarget;
        const row = button.closest('tr');

        const newRow = this.createCheckpointRow();
        row.parentNode.insertBefore(newRow, row.nextSibling);

        this.bindCheckpointButtons();
        this.updateNewRowCheckpointOptions(newRow);
        this.updateHiddenFieldsNow();
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
        this.updateHiddenFieldsNow();
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

    updateHiddenFieldsNow() {
        HiddenFieldUpdater.updateHiddenFields(this.parent.dataManager, this);
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

        } catch (error) {
            // 靜默處理錯誤
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
                // 阻止預設行為，不進行真正的上傳
                e.preventDefault();

                // 只進行前端驗證和顯示
                if (!this.validateFileForUpload()) {
                    return false;
                }
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

    validateFileForUpload() {
        const fileInput = document.getElementById('fileUploadDiagram');

        if (!fileInput || !fileInput.files.length) {
            alert('請先選擇要上傳的檔案');
            return false;
        }

        const file = fileInput.files[0];

        if (!file.type.match(/^image\/(jpeg|jpg|png)$/i)) {
            alert('請選擇JPG或PNG格式的圖片文件');
            return false;
        }

        if (file.size > 10 * 1024 * 1024) {
            alert('文件大小不能超過10MB');
            return false;
        }

        return true;
    }

    handleDiagramPreview() {
        const fileInput = document.getElementById('fileUploadDiagram');
        const file = fileInput.files[0];

        this.showFilePreview(file);
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
            if (filePath && fileName) {
                const fullPath = filePath.startsWith('/') ? filePath : `/${filePath}`;
                this.displayDiagramPreview(fullPath, fileName);
            }
        } catch (error) {
            // 靜默處理錯誤
        }
    }

    collectDiagramData() {
        const previewImg = document.getElementById('diagramPreview');
        const previewContainer = document.getElementById('diagramPreviewContainer');
        const fileInput = document.getElementById('fileUploadDiagram');

        // 檢查是否有檔案準備上傳
        const hasFile = fileInput && fileInput.files.length > 0;
        const hasPreview = previewContainer && previewContainer.style.display !== 'none' && previewImg && previewImg.src;

        if (hasFile || hasPreview) {
            return {
                hasImage: true,
                imageSrc: previewImg ? previewImg.src : '',
                imageAlt: previewImg ? previewImg.alt : '',
                hasFileToUpload: hasFile
            };
        }

        return {
            hasImage: false,
            imageSrc: '',
            imageAlt: '',
            hasFileToUpload: false
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
            // 檢查是否有檔案需要上傳
            const fileInput = document.getElementById('fileUploadDiagram');
            const uploadBtn = document.getElementById('btnUploadDiagram');

            // 如果有檔案且顯示"已上傳"狀態，表示需要真正上傳
            if (fileInput && fileInput.files.length > 0 && uploadBtn && uploadBtn.textContent === '已上傳') {
            }

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

        } catch (error) {
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

                // 修改條件：只要有任何一個欄位不是空的就收集
                if (workItem || description || plannedFinishDate) {
                    checkStandards.push({
                        workItem: workItem,
                        serialNumber: serialNumber,
                        plannedFinishDate: plannedFinishDate,
                        description: description,
                        order: index + 1
                    });
                } else {
                    // 即使是空的也要收集，讓後端可以知道有這一行
                    checkStandards.push({
                        workItem: '',
                        serialNumber: serialNumber,
                        plannedFinishDate: '',
                        description: '',
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

        // 更新 ASP.NET 隱藏欄位（不是創建新的）
        this.updateAspNetHiddenField('hiddenWorkItemsData', JSON.stringify(data.workItems));
        this.updateAspNetHiddenField('hiddenCheckStandardsData', JSON.stringify(data.checkStandards));

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

    updateAspNetHiddenField(fieldId, value) {
        const hiddenField = document.getElementById(fieldId);
        if (hiddenField) {
            hiddenField.value = value;
        }
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