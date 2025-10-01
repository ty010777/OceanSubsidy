
//#region 民國年日期處理
class TaiwanDateHandler {
    static initializeDatePickers() {
        // 設定 moment.js 為繁體中文
        if (typeof moment !== 'undefined') {
            moment.locale('zh-tw');
            this.initializeAllDatePickers();
        }
    }

    static initializeAllDatePickers() {
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
            // 顯示：民國年格式 - moment-taiwan.js 會自動處理轉換
            const displayDate = selectedMoment.format('tYY/MM/DD');
            $(this).val(displayDate);
        };

        // 初始化計畫期程日期選擇器
        $('input[id*="startDate"], input[id*="endDate"]').daterangepicker(datePickerConfig)
            .on('apply.daterangepicker', handleDateSelection);

        // 初始化查核標準的預定完成日期選擇器
        $(document).on('focus', '.taiwan-date-picker', function() {
            if (!$(this).hasClass('daterangepicker-initialized')) {
                $(this).addClass('daterangepicker-initialized');
                $(this).daterangepicker(datePickerConfig)
                    .on('apply.daterangepicker', handleDateSelection);
                $(this).trigger('click'); // 觸發開啟日期選擇器
            }
        });
    }


    // 直接使用前端已有的民國年日期，無需額外處理
    // 因為後端已經使用 ToMinguoDate() 處理並設定 data-gregorian-date 屬性
}
//#endregion

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

    // 統一的輸入框綁定方法
    static bindInputsInRows(rows, cellConfigs, updateCallback) {
        rows.forEach(row => {
            const cell = row.cells[0];
            const code = cell ? cell.textContent.trim() : '';

            if (/^[A-Z]\d+$/.test(code)) {
                cellConfigs.forEach(config => {
                    const input = row.cells[config.cellIndex] ? 
                        row.cells[config.cellIndex].querySelector(config.selector) : null;
                    
                    if (input && !input.hasAttribute(config.attributeName)) {
                        if (config.className) input.classList.add(config.className);
                        if (config.dataAttribute) input.setAttribute(config.dataAttribute, code.charAt(0));
                        
                        config.events.forEach(eventType => {
                            input.addEventListener(eventType, updateCallback);
                        });
                        
                        input.setAttribute(config.attributeName, 'true');
                    }
                });
            }

            // 綁定其他輸入欄位
            const otherInputs = row.querySelectorAll('input[type="text"], input[type="number"], select, input[type="checkbox"]');
            otherInputs.forEach(input => {
                if (!input.hasAttribute('data-bound-update')) {
                    ['input', 'change'].forEach(eventType => {
                        input.addEventListener(eventType, updateCallback);
                    });
                    input.setAttribute('data-bound-update', 'true');
                }
            });
        });
    }

    // 統一的按鈕綁定方法
    static bindButtonsInContainer(container, buttonConfigs) {
        buttonConfigs.forEach(config => {
            if (config.type === 'icon') {
                this.bindButtonsByIcon(config.selector, config.eventType, config.handler, config.attributeName, container);
            } else if (config.type === 'element') {
                this.bindElementsWithAttribute(config.selector, config.eventType, config.handler, config.attributeName, container);
            }
        });
    }
}

class DOMCache {
    constructor() {
        this.cache = new Map();
        this.elements = {
            // 常用的DOM元素ID
            checkStandards: () => document.getElementById('checkStandards'),
            btnDeleteDiagram: () => document.getElementById('btnDeleteDiagram'),
            fileUploadDiagram: () => document.getElementById('fileUploadDiagram'),
            diagramPreviewContainer: () => document.getElementById('diagramPreviewContainer'),
            diagramPreview: () => document.getElementById('diagramPreview'),
            form1: () => document.getElementById('form1'),
            btnTempSave: () => document.getElementById('btnTempSave'),
            btnSaveAndNext: () => document.getElementById('btnSaveAndNext'),
            
            // 常用的選擇器
            subTable: () => document.querySelector('.sub-table'),
            subTableTable: () => document.querySelector('.sub-table table'),
            subTableRows: () => document.querySelectorAll('.sub-table tbody tr'),
            subTableTfoot: () => document.querySelector('.sub-table table tfoot'),
            workItemBtns: () => document.querySelectorAll('.btn-blue-green2[type="button"]'),
            checkStandardsSelects: () => document.querySelectorAll('#checkStandards select')
        };
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

    // 獲取常用元素的快捷方法
    getElement(elementName) {
        if (this.elements[elementName]) {
            return this.elements[elementName]();
        }
        return null;
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
        const addWorkItemBtns = this.domCache.getElement('workItemBtns');
        addWorkItemBtns.forEach(btn => {
            if (btn.textContent.includes('新增工作項目')) {
                btn.addEventListener('click', () => this.addNewWorkItem());
            }
        });

        this.bindTableButtons();
        this.bindWorkItemInputs();
    }

    bindTableButtons() {
        const workItemTable = this.domCache.getElement('subTable');
        if (workItemTable) {
            const buttonConfigs = [
                {
                    type: 'icon',
                    selector: '.btn-dark-green2 .fa-plus',
                    eventType: 'click',
                    handler: (e) => this.addSubItem(e),
                    attributeName: 'data-plus-bound'
                },
                {
                    type: 'icon',
                    selector: '.btn-dark-green2 .fa-trash-alt',
                    eventType: 'click',
                    handler: (e) => this.deleteItem(e),
                    attributeName: 'data-delete-bound'
                }
            ];

            EventBindingHelper.bindButtonsInContainer(workItemTable, buttonConfigs);
        }
    }

    bindWorkItemInputs() {
        this.bindAllRowInputs();
    }

    // 統一的行輸入綁定方法
    bindAllRowInputs() {
        const allRows = this.domCache.getElement('subTableRows');
        
        const cellConfigs = [
            {
                cellIndex: 1,
                selector: 'input[type="text"]',
                attributeName: 'data-workitem-bound',
                events: ['input']
            },
            {
                cellIndex: 3,
                selector: 'input[type="text"]',
                className: 'weight-input',
                dataAttribute: 'data-letter',
                attributeName: 'data-weight-bound',
                events: ['input']
            },
            {
                cellIndex: 4,
                selector: 'input[type="text"]',
                className: 'person-month-input',
                dataAttribute: 'data-letter',
                attributeName: 'data-month-bound',
                events: ['input']
            }
        ];

        EventBindingHelper.bindInputsInRows(allRows, cellConfigs, (event) => {
            const input = event.target;
            
            // 根據不同的輸入框類型執行不同的回調
            if (input.hasAttribute('data-workitem-bound')) {
                this.parent.checkStandards.updateCheckpointOptions();
                this.updateHiddenFieldsNow();
            } else if (input.hasAttribute('data-weight-bound') || input.hasAttribute('data-month-bound')) {
                this.parent.calculator.updateTotals();
            } else {
                this.updateHiddenFieldsNow();
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

        const tfoot = this.domCache.getElement('subTableTfoot');
        if (tfoot) {
            tfoot.parentNode.insertBefore(newTbody, tfoot);
        }

        this.updatePlusButtonVisibility();
        this.rebindAllEvents();
        this.parent.calculator.updateTotals();
        this.updateHiddenFieldsNow();
    }

    createNewWorkItemGroup(letter) {
        const tbody = document.createElement('tbody');
        const mainRow = this.createMainItemRow(letter);
        const subRow = this.createSubItemRow(letter, 1, true);
        tbody.classList.add('table-item');
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
                <input type="text" class="form-control " placeholder="請輸入">
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
                <input class="form-check-input blue-green-check" type="checkbox" value="" checked>
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
                <select name="" class="form-select month-select">
                    <option value="" selected disabled>請選擇</option>
                    ${monthOptions}
                </select>
            </div>
            <div class="input-group mt-2">
                <span class="input-group-text">結束</span>
                <select name="" class="form-select month-select">
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
        workItemTable
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

                this.bindTableButtons();
            }
        }
    }

    rebindAllEvents() {
        this.domCache.clear();
        this.bindTableButtons(); // 使用統一的按鈕綁定方法
        this.bindAllRowInputs(); // 使用統一的綁定方法
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
            tbody.classList.add('table-item');

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


    // 統一的自動加總函數
    calculateTotals() {
        const letterGroups = this.parent.workItems.getLetterGroups();
        
        // 計算權重
        this.calculateByColumn(letterGroups, '.weight-input', 3, '%', 1);
        
        // 計算人月數
        this.calculateByColumn(letterGroups, '.person-month-input', 4, '', 2);
    }

    calculateByColumn(letterGroups, inputSelector, mainCellIndex, suffix, footerCellIndex) {
        let grandTotal = 0;
        
        // 計算每個主項目的小計
        Object.keys(letterGroups).forEach(letter => {
            let subtotal = 0;
            
            letterGroups[letter].subItems.forEach(subItem => {
                const row = subItem.closest('tr');
                const input = row.querySelector(inputSelector);
                if (input && input.value) {
                    const value = parseFloat(input.value.replace('%', '')) || 0;
                    subtotal += value;
                }
            });

            // 更新主項目顯示
            const mainItemRow = this.findMainItemRow(letter);
            if (mainItemRow) {
                mainItemRow.cells[mainCellIndex].textContent = subtotal + suffix;
                grandTotal += subtotal;
            }
        });

        // 更新總計
        const tfoot = this.domCache.getElement('subTableTfoot');
        if (tfoot) {
            const tfootRow = tfoot.querySelector('tr');
            if (tfootRow) {
                tfootRow.cells[footerCellIndex].textContent = grandTotal + suffix;
            }
        }
    }

    findMainItemRow(letter) {
        const allRows = this.domCache.getElement('subTableRows');
        for (let row of allRows) {
            const cell = row.cells[0];
            if (cell && cell.textContent.trim() === letter) {
                return row;
            }
        }
        return null;
    }

    // 簡化的更新總計方法
    updateTotals() {
        this.calculateTotals();
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
        const checkpointTable = this.parent.workItems.domCache.getElement('checkStandards');
        if (!checkpointTable) return;

        const buttonConfigs = [
            {
                type: 'icon',
                selector: '.fa-plus',
                eventType: 'click',
                handler: (e) => this.addCheckpointRow(e),
                attributeName: 'data-checkpoint-plus-bound'
            },
            {
                type: 'icon',
                selector: '.fa-trash-alt',
                eventType: 'click',
                handler: (e) => this.deleteCheckpointRow(e),
                attributeName: 'data-checkpoint-delete-bound'
            },
            {
                type: 'element',
                selector: 'select',
                eventType: 'change',
                handler: (e) => {
                    this.recalculateAllCheckpointNumbers();
                    this.updateHiddenFieldsNow();
                },
                attributeName: 'data-checkpoint-select-bound'
            },
            {
                type: 'element',
                selector: 'input[type="date"]',
                eventType: 'change',
                handler: () => this.updateHiddenFieldsNow(),
                attributeName: 'data-checkpoint-date-bound'
            }
        ];

        EventBindingHelper.bindButtonsInContainer(checkpointTable, buttonConfigs);

        // 綁定 textarea 編輯事件
        const textareas = checkpointTable.querySelectorAll('textarea');
        textareas.forEach(textarea => {
            if (!textarea.hasAttribute('data-checkpoint-content-bound')) {
                ['input', 'blur', 'change'].forEach(eventType => {
                    textarea.addEventListener(eventType, () => this.updateHiddenFieldsNow());
                });
                textarea.setAttribute('data-checkpoint-content-bound', 'true');
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
                <input type="text" name="" class="form-control taiwan-date-picker" placeholder="請選擇日期" readonly>
            </td>
            <td class="align-middle" width="500">
                <textarea class="form-control" rows="3" placeholder="請輸入"></textarea>
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
        const checkpointTable = this.parent.workItems.domCache.getElement('checkStandards');

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
        const checkpointTable = this.parent.workItems.domCache.getElement('checkStandards');
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
        const checkpointSelects = this.parent.workItems.domCache.getElement('checkStandardsSelects');

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

                const dateInput = row.querySelector('input.taiwan-date-picker');
                if (dateInput && checkStandard.plannedFinishDate) {
                    // 後端已經使用 ToMinguoDate() 處理，直接使用民國年格式
                    dateInput.value = checkStandard.plannedFinishDate;
                }

                const descriptionTextarea = row.querySelector('textarea');
                if (descriptionTextarea && checkStandard.description) {
                    descriptionTextarea.value = checkStandard.description;
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
        const domCache = this.parent.workItems.domCache;
        const deleteBtn = domCache.getElement('btnDeleteDiagram');

        // 上傳按鈕現在由 ASP.NET 後端處理，不需要前端事件綁定

        if (deleteBtn) {
            deleteBtn.addEventListener('click', (e) => {
                e.preventDefault();
                this.handleDiagramDelete();
            });
        }
    }

    // 上傳相關方法已移至後端處理，不再需要前端驗證和預覽

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
            // 檔案上傳現在由後端立即處理，不需要前端檢查

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
        const startDateInput = document.querySelector('input[id*="startDate"]');
        const endDateInput = document.querySelector('input[id*="endDate"]');

        // 參考 CLB 的方式：優先從隱藏欄位或 data-gregorian-date 屬性取得西元年格式
        let startDate = '';
        let endDate = '';

        if (startDateInput) {
            // 嘗試從隱藏欄位取得
            const startHiddenField = document.querySelector(`input[name="${startDateInput.name}_gregorian"]`);
            if (startHiddenField && startHiddenField.value) {
                startDate = startHiddenField.value;
            } else {
                // 從 data-gregorian-date 屬性取得
                startDate = startDateInput.getAttribute('data-gregorian-date') || '';
            }

            // 如果還是沒有，且有輸入值，使用 moment-taiwan 解析
            if (!startDate && startDateInput.value) {
                const momentDate = moment(startDateInput.value, 'tYY/MM/DD');
                if (momentDate.isValid()) {
                    startDate = momentDate.format('YYYY-MM-DD');
                }
            }
        }

        if (endDateInput) {
            // 嘗試從隱藏欄位取得
            const endHiddenField = document.querySelector(`input[name="${endDateInput.name}_gregorian"]`);
            if (endHiddenField && endHiddenField.value) {
                endDate = endHiddenField.value;
            } else {
                // 從 data-gregorian-date 屬性取得
                endDate = endDateInput.getAttribute('data-gregorian-date') || '';
            }

            // 如果還是沒有，且有輸入值，使用 moment-taiwan 解析
            if (!endDate && endDateInput.value) {
                const momentDate = moment(endDateInput.value, 'tYY/MM/DD');
                if (momentDate.isValid()) {
                    endDate = momentDate.format('YYYY-MM-DD');
                }
            }
        }

        return {
            startDate: startDate,
            endDate: endDate
        };
    }

    collectWorkItemsData() {
        this.parent.calculator.updateTotals();

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

                const dateInput = row.cells[2] ? row.cells[2].querySelector('input.taiwan-date-picker') : null;
                let plannedFinishDate = '';

                if (dateInput) {
                    // 嘗試從隱藏欄位取得
                    const hiddenField = document.querySelector(`input[name="${dateInput.name}_gregorian"]`);
                    if (hiddenField && hiddenField.value) {
                        plannedFinishDate = hiddenField.value;
                    } else {
                        // 從 data-gregorian-date 屬性取得
                        plannedFinishDate = dateInput.getAttribute('data-gregorian-date') || '';
                    }

                    // 如果還是沒有，且有輸入值，使用 moment-taiwan 解析
                    if (!plannedFinishDate && dateInput.value) {
                        const momentDate = moment(dateInput.value, 'tYY/MM/DD');
                        if (momentDate.isValid()) {
                            plannedFinishDate = momentDate.format('YYYY-MM-DD');
                        }
                    }
                }

                const descriptionTextarea = row.cells[3] ? row.cells[3].querySelector('textarea') : null;
                const description = descriptionTextarea ? descriptionTextarea.value.trim() : '';

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

        // 初始化民國年日期處理
        this.initializeDateHandlers();
    }

    initializeDateHandlers() {
        // 延遲初始化，確保頁面載入完成
        setTimeout(() => {
            TaiwanDateHandler.initializeDatePickers();
        }, 100);
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

    // 日期載入應該由後端使用 ToMinguoDate() 處理，前端不需要額外載入方法
}
//#endregion

//#region 頁面初始化
document.addEventListener('DOMContentLoaded', function() {
    // 將重構版管理器實例設為全域變數
    window.sciWorkSchManager = new SciWorkSchManager();
});
//#endregion